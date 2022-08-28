
using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Abstractions;
using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Infrastructure.NotificationHandlers
{
    public sealed class EventNotificationHandler : INotificationHandler<NotificationCommand>
    {
        private readonly IRabbitMQBus _eventBus;
        private readonly RabbitMQSettings _rabbitMqSettings;
        private readonly ILogger<EventNotificationHandler> _logger;

        public EventNotificationHandler(
            IRabbitMQBus eventBus,
            RabbitMQSettings rabbitMqSettings,
            ILogger<EventNotificationHandler> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _rabbitMqSettings = rabbitMqSettings ?? throw new ArgumentNullException(nameof(rabbitMqSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(NotificationCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                var busMessage = new RabbitMQBusMessage
                {
                    Id = Guid.NewGuid(),
                    Data = notification.Notification
                };
                _eventBus.Publish(busMessage, _rabbitMqSettings.ExchangeName, "routeKey");

                return Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                var ids = string.Join(", ", notification.Notification.Deltas.Select(d => d.Data.Id));
                _logger.LogError($"Error from handle notificationh next id's: {ids}", ex);

                return Task.FromException(ex);
            }
        }
    }
}
