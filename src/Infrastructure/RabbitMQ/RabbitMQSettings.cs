namespace Infrastructure.RabbitMQ
{
    public record class RabbitMQSettings
    {
        public required string HostName { get; init; }

        public required string QueueName { get; init; }

        public required string ExchangeName { get; init; }

        public string RetryConnectCount { get; init; }
    }
}
