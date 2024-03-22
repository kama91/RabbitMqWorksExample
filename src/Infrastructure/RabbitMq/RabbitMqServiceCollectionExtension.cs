using Infrastructure.RabbitMq.Abstractions;
using Infrastructure.RabbitMq.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMq
{
    public static class RabbitMqServiceCollectionExtension
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var connection = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();
                var rabbitSettings = sp.GetRequiredService<RabbitMqSettings>();
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(rabbitSettings.ConnectionString),
                    DispatchConsumersAsync = true
                };

                return new RabbitMqPersistentConnection(factory, connection, rabbitSettings.RetryConnectCount);

            });

            serviceCollection.AddSingleton<IRabbitMqBus, RabbitMqBus>();

            return serviceCollection;
        }
    }
}
