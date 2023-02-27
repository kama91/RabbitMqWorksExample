namespace Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions
{
    public interface IRabbitMQBusMessage<TData>
    {
        Guid Id { get; }

        TData Data { get; }
    }
}
