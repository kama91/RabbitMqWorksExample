using Core.RabbitMQ;
using Core.RabbitMQ.Abstractions;
using Core.RabbitMQ.EventBusRabbitMQ;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();
                var rabbitSettings = sp.GetRequiredService<RabbitMQSettings>();
                var factory = new ConnectionFactory
                {
                    HostName = rabbitSettings.HostName,
                    Port = 5672,
                    DispatchConsumersAsync = true
                };

                var userName = "guest";
                var pass = "guest";
                if (!string.IsNullOrEmpty(userName))
                {
                    factory.UserName = userName;
                }

                if (!string.IsNullOrEmpty(pass))
                {
                    factory.Password = pass;
                }

                var retryConnectCount = 5;
                if (!string.IsNullOrEmpty(rabbitSettings.RetryConnectCount) && int.TryParse(rabbitSettings.RetryConnectCount, out var connectCount))
                {
                    retryConnectCount = connectCount;
                }

                return new RabbitMQPersistentConnection(factory, logger, retryConnectCount);
            
            });

            serviceCollection.AddSingleton<IEventBus, EventBusRabbitMQ>();

            return serviceCollection;
        } 
    }
}
