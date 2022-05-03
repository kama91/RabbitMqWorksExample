using RabbitMQ.Client;
using System;

namespace Core.RabbitMQ.Abstractions
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
