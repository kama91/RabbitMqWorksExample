using System;

namespace Core.RabbitMQ.QueueEvents.Abstractions
{
    public interface IBusMessage<TData>
    {
        Guid Id { get; set; }

        TData Data { get; set; }
    }
}
