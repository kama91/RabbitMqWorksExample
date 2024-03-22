using Infrastructure.RabbitMq.Abstractions;
using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMq
{
    public sealed record RabbitMqBusSubscription(AsyncEventingBasicConsumer Consumer, string QueueName)
        : IRabbitMqBusSubscription
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
