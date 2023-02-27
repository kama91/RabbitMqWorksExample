namespace Infrastructure.RabbitMQ
{
    public record class MessageConsumerConfiguration(string ExchangeName, string QueueName, string RoutingKey);
}
