using Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions;

namespace Infrastructure.RabbitMq.Abstractions
{
    public interface IRabbitMqBus
    {
        Task Publish<TM>(IRabbitMqBusMessage<TM> message, string exchangeName, string routingKey);

        public IRabbitMqBusSubscription Subscribe<TH, TE, TD>(MessageConsumerConfiguration messageConsumerConfiguration)
            where TH : IRabbitMqBusMessageHandler<TE, TD>
            where TE : IRabbitMqBusMessage<TD>;

        public bool Unsubscribe<TD>(IRabbitMqBusSubscription subscription);
    }
}
