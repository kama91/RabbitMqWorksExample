using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMq.Abstractions
{
    public interface IRabbitMqBusSubscription
    {
        Guid Id { get; }

        string QueueName { get; }

        AsyncEventingBasicConsumer Consumer { get; }
    }
}
