using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMQ.Abstractions
{
    public interface IRabbitMQBusSubscription
    {
        Guid Id { get; }

        string QueueName { get; }

        AsyncEventingBasicConsumer Consumer { get; }
    }
}
