using Core.Serializer.Abstractions;
using Infrastructure.RabbitMQ.Abstractions;
using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;
using System.Net.Sockets;

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
        private Lazy<IModel?> _consumerChannel;

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
                    _logger.LogWarning("Could not publish message: {BusMessageId} after {TotalSeconds}s with error {Error}", message.Id, time.TotalSeconds, ex);
                });

            using var channel = _persistentConnection.CreateModel();

            var messageBody = _serializer.Serialize(message);

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true);

            _logger.LogInformation("Publishing message with Id: {MessageId} to RabbitMQ", message.Id);

            await policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.ContentType = "application/json";
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                channel.BasicPublish(exchangeName, routingKey, body: messageBody);

                _logger.LogInformation("Message with Id: {MessageId} successfully published", message.Id);

                return ValueTask.CompletedTask;
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

            _consumerChannel = new Lazy<IModel?>(CreateConsumerChannel(consumerConfiguration.QueueName));

            _consumerChannel.Value.QueueBind(consumerConfiguration.QueueName, consumerConfiguration.ExchangeName, consumerConfiguration.RoutingKey);

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel.Value);

            consumer.Received += (ReceivedMessage<TE, TD>);

            _consumerChannel.Value.BasicConsume(consumerConfiguration.QueueName, false, consumer);

            _logger.LogInformation("Bind to {QueueName} successfully", consumerConfiguration.QueueName);

            var subscription = new RabbitMQBusSubsription(consumer, consumerConfiguration.QueueName);
            _consumers.TryAdd(subscription.Id, consumer);

            return subscription;
        }

        private async Task ReceivedMessage<TE, TD>(object sender, BasicDeliverEventArgs args) where TE : IRabbitMQBusMessage<TD>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var handlers = scope.ServiceProvider.GetServices<IRabbitMQBusMessageHandler<TE, TD>>();
                var message = _serializer.Deserialize<TE>(args.Body.ToArray());

                foreach (var handler in handlers)
                {
                    await handler.HandleAsync(message);
                }

                _consumerChannel.Value.BasicAck(args.DeliveryTag, false);

                _logger.LogInformation("Message with Id:{MessageId} successfully received", message.Id);
            }
            catch (Exception ex)
            {
                _consumerChannel.Value.BasicNack(args.DeliveryTag, false, true);

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
                    consumer.Received -= (ReceivedMessage<IRabbitMQBusMessage<TD>, TD>);
                    if (_consumers.TryRemove(subscription.Id, out var _))
                    {
                        _logger.LogInformation("Successfully unsubscribe from queue {QueueName}", subscription.QueueName);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed unsubscribe from queue {QueueName}. Error {Error}", subscription.QueueName, ex);
                }
            }

            return false;
        }
    }
}
