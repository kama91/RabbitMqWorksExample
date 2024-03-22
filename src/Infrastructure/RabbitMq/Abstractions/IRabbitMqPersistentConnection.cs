using RabbitMQ.Client;

namespace Infrastructure.RabbitMq.Abstractions
{
    public interface IRabbitMqPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
