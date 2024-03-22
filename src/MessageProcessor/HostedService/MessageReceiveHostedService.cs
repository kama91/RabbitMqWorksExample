using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Data.Notifications;
using Infrastructure.RabbitMq;
using Infrastructure.RabbitMq.Abstractions;
using Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions;
using MessageProcessor.BusMessageHandlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageProcessor.HostedService;

public class MessageReceiveHostedService : IHostedService
{
    private readonly MessageConsumerConfiguration _consumerConfiguration;
    private readonly ILogger<MessageReceiveHostedService> _logger;
    private readonly IRabbitMqBus _rabbitMqBus;
    private IRabbitMqBusSubscription _subscription;

    public MessageReceiveHostedService(
        IRabbitMqBus rabbitMqBus,
        RabbitMqSettings rabbitMqSettings,
        ILogger<MessageReceiveHostedService> logger)
    {
        _rabbitMqBus = rabbitMqBus ?? throw new ArgumentNullException(nameof(rabbitMqBus));
        if (rabbitMqSettings == null) throw new ArgumentNullException(nameof(rabbitMqSettings));
        _consumerConfiguration =
            new MessageConsumerConfiguration(rabbitMqSettings.ExchangeName, rabbitMqSettings.QueueName, "routeKey");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscription =
            _rabbitMqBus.Subscribe<RabbitMqMessageHandler, RabbitMqBusMessage, Notification>(_consumerConfiguration);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_subscription != null) _rabbitMqBus.Unsubscribe<Notification>(_subscription);

        return Task.CompletedTask;
    }
}