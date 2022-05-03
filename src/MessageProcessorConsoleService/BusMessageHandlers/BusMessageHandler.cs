using Core.Data.Notifications;
using Core.RabbitMQ.Abstractions;
using Core.RabbitMQ.QueueEvents;
using Core.RabbitMQ.QueueEvents.Abstractions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMessageConsoleService.QueueEventHandlers
{
    public class BusMessageHandler : IBusMessageHandler<BusMessage, Notification>
    {
        private readonly ILogger<BusMessageHandler> _logger;

        public BusMessageHandler(
            ILogger<BusMessageHandler> logger)
        {
           _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task HandleAsync(IBusMessage<Notification> message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Handle message with id: {message.Id}");

            foreach (var delta in message.Data.Deltas)
            {
                _logger.LogInformation($"Delta type is: {delta.Type} Delta Data AccountId: {delta.Data.AccountId} Delta Data Id {delta.Data.Id}");
            }

            return Task.FromResult(message);
        }
    }
}
