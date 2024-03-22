using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Data.Notifications;
using Infrastructure.RabbitMq.Abstractions;
using Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessageProcessor.BusMessageHandlers;

public class RabbitMqMessageHandler : IRabbitMqBusMessageHandler<RabbitMqBusMessage, Notification>
{
    private readonly ILogger<RabbitMqMessageHandler> _logger;

    public RabbitMqMessageHandler(
        ILogger<RabbitMqMessageHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task HandleAsync(IRabbitMqBusMessage<Notification> message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handle message with Id: {Id}", message.Id);

        foreach (var delta in message.Data.Deltas)
            _logger.LogInformation("Delta type is: {Type} Delta Data AccountId: {AccountId} Delta Data Id {Id}",
                delta.Type, delta.Data.AccountId, delta.Data.Id);

        return Task.FromResult(message);
    }
}