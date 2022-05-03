using Core.Data.Notifications;
using Core.RabbitMQ.QueueEvents.Abstractions;

using System;

namespace Core.RabbitMQ.QueueEvents
{
    public class BusMessage : IBusMessage<Notification>
    {
        public Guid Id { get; set; }

        public Notification Data { get; set; }
    }
}
