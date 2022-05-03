using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using Core.RabbitMQ.Abstractions;
using Core.RabbitMQ.QueueEvents.Abstractions;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Core.Serializer.Abstractions;
using System.Threading.Tasks;

namespace Core.RabbitMQ.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IJsonByteArraySerializer _serializer;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private IModel _consumerChannel;
        private const int RetryConnectCount = 5;

        public EventBusRabbitMQ(
            IServiceProvider serviceProvider,
            IRabbitMQPersistentConnection persistentConnection,
            IJsonByteArraySerializer serializer,
            ILogger<EventBusRabbitMQ> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Publish<TD>(IBusMessage<TD> message, string exchangeName, string routingKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(RetryConnectCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, $"Could not publish message: {message.Id} after {time.TotalSeconds:n1}s ({ex.Message})");
                });

            using var channel = _persistentConnection.CreateModel();

            var messageBody = _serializer.Serialize(message);

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true);

            _logger.LogInformation($"Publishing message with Id: {message.Id} to RabbitMQ");

            await policy.Execute(() => 
            {
                var properties = channel.CreateBasicProperties();
                properties.ContentType = "application/json";
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                channel.BasicPublish(exchangeName, routingKey, body: messageBody);

                _logger.LogInformation($"Message with Id: {message.Id} successfully published");
                
                return Task.CompletedTask;
            });
        }

        public void Subscribe<TH, TE, TD>(MessageConsumerConfiguration messageConsumerConfiguration)
            where TH : IBusMessageHandler<TE, TD>
            where TE : IBusMessage<TD>
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel = CreateConsumerChannel(messageConsumerConfiguration.QueueName);

            _consumerChannel.QueueBind(messageConsumerConfiguration.QueueName, messageConsumerConfiguration.ExchangeName, messageConsumerConfiguration.RoutingKey);

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += async (obj, args) =>
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IBusMessageHandler<TE, TD>>();
                    var message = _serializer.Deserialize<TE>(args.Body.ToArray());
                    await handler.HandleAsync(message);
                    _consumerChannel.BasicAck(args.DeliveryTag, false);

                    _logger.LogInformation($"Message with Id:{message.Id} successfully received");
                }
                catch (Exception ex)
                {
                    _consumerChannel.BasicNack(args.DeliveryTag, false, true);

                    _logger.LogError("Message does not processed", ex.Message);
                }
            };

            _consumerChannel.BasicConsume(messageConsumerConfiguration.QueueName, false, consumer);

            _logger.LogInformation($"Bind to {messageConsumerConfiguration.QueueName} successfully");
        }

        private IModel CreateConsumerChannel(string queueName)
        {
            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare("EventExchange", ExchangeType.Direct, true);
            channel.QueueDeclare(queueName, true, false, false);
                
            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel(queueName);
            };

            return channel;   
        }
    }
}
