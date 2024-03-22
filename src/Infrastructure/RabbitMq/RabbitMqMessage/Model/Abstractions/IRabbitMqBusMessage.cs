namespace Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions
{
    public interface IRabbitMqBusMessage<out TData>
    {
        Guid Id { get; }

        TData Data { get; }
    }
}
