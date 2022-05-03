using Core.RabbitMQ.QueueEvents.Abstractions;

using System.Threading.Tasks;

namespace Core.RabbitMQ.Abstractions
{
    public interface IEventBus
    {
        Task Publish<TD>(IBusMessage<TD> @event, string exchangeName, string routingKey);

        public void Subscribe<TH, TE, TD>(MessageConsumerConfiguration messageConsumerConfiguration)
            where TH : IBusMessageHandler<TE, TD>
            where TE : IBusMessage<TD>;
    }
}
