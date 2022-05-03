using Core.RabbitMQ.Abstractions;

namespace Core.RabbitMQ
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
