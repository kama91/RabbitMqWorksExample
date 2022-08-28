namespace Infrastructure.RabbitMQ.Abstractions
{
    public interface IRabbitMQSettings
    {
        string HostName { get; set; }

        string QueueName { get; set; }

        string ExchangeName { get; set; }

        string RetryConnectCount { get; set; }
    }
}
