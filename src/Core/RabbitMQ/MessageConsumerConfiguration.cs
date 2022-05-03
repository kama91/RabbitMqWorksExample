using System;

namespace Core.RabbitMQ
{
    public class MessageConsumerConfiguration
    {
        public MessageConsumerConfiguration(string exchangeName, string queueName, string routingKey)
        {
            ExchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            RoutingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
        }

        public string ExchangeName { get; private set; }

        public string QueueName { get; private set; }

        public string RoutingKey { get; private set; }
    }
}
