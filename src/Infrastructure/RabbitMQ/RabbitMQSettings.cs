namespace Infrastructure.RabbitMQ
{
    public record class RabbitMQSettings
    {
        public required string ConnectionString { get; init; }

        public required string ExchangeName { get; init; }

        public required string QueueName { get; init; }

        public int RetryConnectCount { get; init; }
    }
}
