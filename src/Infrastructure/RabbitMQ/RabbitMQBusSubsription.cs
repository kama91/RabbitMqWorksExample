using Infrastructure.RabbitMQ.Abstractions;

using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMQ
{
    public class RabbitMQBusSubsription : IRabbitMQBusSubscription
    {
        public Guid Id { get; }

        public string QueueName { get; }

        public AsyncEventingBasicConsumer Consumer { get; }
        
        public RabbitMQBusSubsription(AsyncEventingBasicConsumer consumer, string queueName)
        {
            Id = Guid.NewGuid();
            QueueName = queueName;
            Consumer = consumer;
        }
    }
}
