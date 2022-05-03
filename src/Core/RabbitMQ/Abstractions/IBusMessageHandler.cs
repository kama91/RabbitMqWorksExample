using Core.RabbitMQ.QueueEvents.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.RabbitMQ.Abstractions
{
    public interface IBusMessageHandler<TE, TD>
        where TE : IBusMessage<TD>
    {
        Task HandleAsync(IBusMessage<TD> @event, CancellationToken cancellationToken = default);
    }
}
