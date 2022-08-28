using Core.Data.Notifications;

namespace Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions
{
    public class RabbitMQBusMessage : IRabbitMQBusMessage<Notification>
    {
        public Guid Id { get; set; }

        public Notification Data { get; set; } = null!;
    }
}
