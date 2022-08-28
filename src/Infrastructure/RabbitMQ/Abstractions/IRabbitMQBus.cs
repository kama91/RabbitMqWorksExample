using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

namespace Infrastructure.RabbitMQ.Abstractions
{
    public interface IRabbitMQBus
    {
        Task Publish<TM>(IRabbitMQBusMessage<TM> message, string exchangeName, string routingKey);

        public IRabbitMQBusSubscription Subscribe<TH, TE, TD>(MessageConsumerConfiguration messageConsumerConfiguration)
            where TH : IRabbitMQBusMessageHandler<TE, TD>
            where TE : IRabbitMQBusMessage<TD>;

        public bool Unsubscribe<TD>(IRabbitMQBusSubscription subscription);
    }
}
