using Core.Data.Notifications;

using Infrastructure.RabbitMQ.Abstractions;
using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMessageConsoleService.QueueEventHandlers
{
    public class RabbitMQMessageHandler : IRabbitMQBusMessageHandler<RabbitMQBusMessage, Notification>
    {
        private readonly ILogger<RabbitMQMessageHandler> _logger;

        public RabbitMQMessageHandler(
            ILogger<RabbitMQMessageHandler> logger)
        {
           _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task HandleAsync(IRabbitMQBusMessage<Notification> message, CancellationToken cancellationToken = default)
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
