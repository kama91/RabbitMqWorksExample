using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Core.Serializer.Abstractions;
using Infrastructure.RabbitMQ.Abstractions;
using RabbitMQ.Client;
using Polly;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;
using System.Collections.Concurrent;

namespace Infrastructure.RabbitMQ.EventBusRabbitMQ
{
    public class RabbitMQBus : IRabbitMQBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IJsonByteArraySerializer _serializer;
        private readonly ILogger<RabbitMQBus> _logger;
        private readonly ConcurrentDictionary<Guid, AsyncEventingBasicConsumer> _consumers = new();
        private const int RetryConnectCount = 5;
        private IModel _consumerChannel;

        public RabbitMQBus(
            IServiceProvider serviceProvider,
            IRabbitMQPersistentConnection persistentConnection,
            IJsonByteArraySerializer serializer,
            ILogger<RabbitMQBus> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Publish<TM>(IRabbitMQBusMessage<TM> message, string exchangeName, string routingKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(RetryConnectCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish message: {busMessageId} after {totalSeconds}s", message.Id, time.TotalSeconds);
                });

            using var channel = _persistentConnection.CreateModel();

            var messageBody = _serializer.Serialize(message);

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true);

            _logger.LogInformation("Publishing message with Id: {messageId} to RabbitMQ", message.Id);

            await policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.ContentType = "application/json";
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                channel.BasicPublish(exchangeName, routingKey, body: messageBody);

                _logger.LogInformation("Message with Id: {messageId} successfully published", message.Id);

                return Task.CompletedTask;
            });
        }

        public IRabbitMQBusSubscription Subscribe<TH, TE, TD>(MessageConsumerConfiguration consumerConfiguration)
            where TH : IRabbitMQBusMessageHandler<TE, TD>
            where TE : IRabbitMQBusMessage<TD>
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel = CreateConsumerChannel(consumerConfiguration.QueueName);

            _consumerChannel.QueueBind(consumerConfiguration.QueueName, consumerConfiguration.ExchangeName, consumerConfiguration.RoutingKey);

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += async (obj, args) => await ReceivedMessage<TE, TD>(obj, args);

            _consumerChannel.BasicConsume(consumerConfiguration.QueueName, false, consumer);

            _logger.LogInformation($"Bind to {consumerConfiguration.QueueName} successfully");
            
            var subscription = new RabbitMQBusSubsription(consumer, consumerConfiguration.QueueName);
            _consumers.TryAdd(subscription.Id, consumer);
            
            return subscription;
        }

        private async Task ReceivedMessage<TE, TD>(object sender, BasicDeliverEventArgs args) where TE : IRabbitMQBusMessage<TD>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IRabbitMQBusMessageHandler<TE, TD>>();
                var message = _serializer.Deserialize<TE>(args.Body.ToArray());
                await handler.HandleAsync(message);
                _consumerChannel.BasicAck(args.DeliveryTag, false);

                _logger.LogInformation("Message with Id:{messageId} successfully received", message.Id);
            }
            catch (Exception ex)
            {
                _consumerChannel.BasicNack(args.DeliveryTag, false, true);

                _logger.LogError("Message does not processed", ex.Message);
            }
        } 

        private IModel CreateConsumerChannel(string queueName)
        {
            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare("EventExchange", ExchangeType.Direct, true);
            channel.QueueDeclare(queueName, true, false, false);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                channel.Dispose();
                channel = CreateConsumerChannel(queueName);
            };

            return channel;
        }

        public bool Unsubscribe<TD>(IRabbitMQBusSubscription subscription)
        {
            return UnsubscribeFromQueue<TD>(subscription);
        }

        private bool UnsubscribeFromQueue<TD>(IRabbitMQBusSubscription subscription)
        {
            if (_consumers.TryGetValue(subscription.Id, out var consumer))
            {
                try
                {
                    consumer.Received -= (obj, args) => ReceivedMessage<IRabbitMQBusMessage<TD>, TD>(obj, args);
                    if (_consumers.TryRemove(subscription.Id, out var _))
                    {
                        _logger.LogInformation("Successfully unsubscribe from queue {subscription.QueueName}", subscription.QueueName);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed unsubscribe from queue {name}. Error {error}", subscription.QueueName, ex);
                }
            }

            return false;
        }
    }
}
