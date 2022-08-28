
using Infrastructure.RabbitMQ.Abstractions;

namespace Infrastructure.RabbitMQ
{
    public class RabbitMQSettings : IRabbitMQSettings
    {
        public string HostName { get; set; }

        public string QueueName { get; set; }

        public string ExchangeName { get; set; }

        public string RetryConnectCount { get; set; }

        public RabbitMQSettings()
        {
        }
    }
}
