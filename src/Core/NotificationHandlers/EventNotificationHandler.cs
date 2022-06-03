using Core.RabbitMQ;
using Core.RabbitMQ.Abstractions;
using Core.RabbitMQ.QueueEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.NotificationHandlers
{
    public sealed class EventNotificationHandler : INotificationHandler<EventNotification>
    {
        private readonly IEventBus _eventBus;
        private readonly RabbitMQSettings _rabbitMqSettings;
        private readonly ILogger<EventNotificationHandler> _logger;

        public EventNotificationHandler(
            IEventBus eventBus,
            RabbitMQSettings rabbitMqSettings,
            ILogger<EventNotificationHandler> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _rabbitMqSettings = rabbitMqSettings ?? throw new ArgumentNullException(nameof(rabbitMqSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(EventNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var busMessage = new BusMessage
                {
                    Id = Guid.NewGuid(),
                    Data = notification.Notification
                };
                _eventBus.Publish(busMessage, _rabbitMqSettings.ExchangeName, "routeKey");
    
                return Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {ex}");

                return Task.FromException(ex);
            }
        }
    }
}
