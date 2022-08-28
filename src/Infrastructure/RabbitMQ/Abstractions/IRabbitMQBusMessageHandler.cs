using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

namespace Infrastructure.RabbitMQ.Abstractions
{
    public interface IRabbitMQBusMessageHandler<TE, TD>
        where TE : IRabbitMQBusMessage<TD>
    {
        Task HandleAsync(IRabbitMQBusMessage<TD> @event, CancellationToken cancellationToken = default);
    }
}
