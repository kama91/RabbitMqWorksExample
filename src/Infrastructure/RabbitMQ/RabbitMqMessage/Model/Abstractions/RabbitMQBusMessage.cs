using Core.Data.Notifications;

namespace Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions
{
    public record class RabbitMQBusMessage : IRabbitMQBusMessage<Notification>
    {
        public RabbitMQBusMessage(Guid id, Notification data)
        {
            Id = id;
            Data = data;
        }

        public Guid Id { get; }

        public Notification Data { get; }
    }
}
