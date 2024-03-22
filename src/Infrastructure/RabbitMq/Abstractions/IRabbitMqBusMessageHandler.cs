using Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions;

namespace Infrastructure.RabbitMq.Abstractions
{
    public interface IRabbitMqBusMessageHandler<TE, in TD>
        where TE : IRabbitMqBusMessage<TD>
    {
        Task HandleAsync(IRabbitMqBusMessage<TD> @event, CancellationToken cancellationToken = default);
    }
}
