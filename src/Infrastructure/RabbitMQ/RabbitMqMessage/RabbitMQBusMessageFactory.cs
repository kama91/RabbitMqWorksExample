using Core.Data.Notifications;

using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

namespace Infrastructure.RabbitMQ.RabbitMqMessage
{
    public class RabbitMQBusMessageFactory
    {
        public static IRabbitMQBusMessage<TD> Create<TD>(TD message) where TD : Notification, new()
        {
            return (IRabbitMQBusMessage<TD>)new RabbitMQBusMessage
            {
                Id = Guid.NewGuid(),
                Data = message
            };
        }
    }
}
