
using System;

namespace Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions
{
    public interface IRabbitMQBusMessage<TData>
    {
        Guid Id { get; set; }

        TData Data { get; set; }
    }
}
