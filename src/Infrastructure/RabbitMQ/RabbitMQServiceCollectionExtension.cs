using Infrastructure.RabbitMQ.Abstractions;
using Infrastructure.RabbitMQ.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMQ
{
    public static class RabbitMQServiceCollectionExtension
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var connection = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();
                var rabbitSettings = sp.GetRequiredService<RabbitMQSettings>();
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(rabbitSettings.ConnectionString),
                    DispatchConsumersAsync = true
                };

                return new RabbitMQPersistentConnection(factory, connection, rabbitSettings.RetryConnectCount);

            });

            serviceCollection.AddSingleton<IRabbitMQBus, RabbitMQBus>();

            return serviceCollection;
        }
    }
}
