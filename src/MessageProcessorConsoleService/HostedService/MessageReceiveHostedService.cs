using Core.Data.Notifications;

using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Abstractions;
using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProcessMessageConsoleService.QueueEventHandlers;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMessageConsoleService.HostedService
{
    public class MessageReceiveHostedService : IHostedService
    {
        private readonly IRabbitMQBus _rabbitMqBus;
        private readonly MessageConsumerConfiguration _consumerConfiguration;
        private IRabbitMQBusSubscription _subscription;
        private readonly ILogger<MessageReceiveHostedService> _logger;

        public MessageReceiveHostedService(
            IRabbitMQBus rabbitMqBus,
            RabbitMQSettings rabbitMqSettings,
            ILogger<MessageReceiveHostedService> logger)
        {
            _rabbitMqBus = rabbitMqBus ?? throw new ArgumentNullException(nameof(rabbitMqBus));
            if (rabbitMqSettings == null)
            {
                throw new ArgumentNullException(nameof(rabbitMqSettings));
            }
            _consumerConfiguration = new MessageConsumerConfiguration(rabbitMqSettings.ExchangeName, rabbitMqSettings.QueueName, "routeKey");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription = _rabbitMqBus.Subscribe<RabbitMQMessageHandler, RabbitMQBusMessage, Notification>(_consumerConfiguration);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_subscription != null)
            {
                _rabbitMqBus.Unsubscribe<Notification>(_subscription);
            }

            return Task.CompletedTask;
        }
    }
}
   