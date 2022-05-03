using Core.Data.Notifications;
using Core.RabbitMQ;
using Core.RabbitMQ.Abstractions;
using Core.RabbitMQ.QueueEvents;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ProcessMessageConsoleService.QueueEventHandlers;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMessageConsoleService.HostedService
{
    public class MessageReceiveHostedService : BackgroundService
    {
        private readonly IEventBus _eventBus;
        private readonly MessageConsumerConfiguration _msgConsumerConfiguration;
        private readonly ILogger<MessageReceiveHostedService> _logger;

        public MessageReceiveHostedService(
            IEventBus eventBus,
            RabbitMQSettings rabbitMqSettings,
            ILogger<MessageReceiveHostedService> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            if (rabbitMqSettings == null)
            {
                throw new ArgumentNullException(nameof(rabbitMqSettings));
            }
            _msgConsumerConfiguration = new MessageConsumerConfiguration(rabbitMqSettings.ExchangeName, rabbitMqSettings.QueueName, "nylas");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _eventBus.Subscribe<BusMessageHandler, BusMessage, Notification>(_msgConsumerConfiguration);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError("Subscription error", ex);
                
                return Task.FromException(ex);
            }
        }
    }
}
   