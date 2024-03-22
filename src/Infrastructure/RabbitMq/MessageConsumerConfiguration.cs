namespace Infrastructure.RabbitMq
{
    public sealed record MessageConsumerConfiguration(string ExchangeName, string QueueName, string RoutingKey);
}
