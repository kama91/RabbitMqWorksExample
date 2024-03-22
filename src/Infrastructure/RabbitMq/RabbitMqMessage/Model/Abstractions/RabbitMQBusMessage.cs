using Core.Data.Notifications;

namespace Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions
{
    public sealed record RabbitMqBusMessage(Guid Id, Notification Data) : IRabbitMqBusMessage<Notification>;
}
