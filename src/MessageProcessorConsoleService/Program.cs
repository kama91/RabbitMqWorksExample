using Core;
using Core.Data.Notifications;
using Core.Serializer;
using Core.Serializer.Abstractions;

using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Abstractions;
using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProcessMessageConsoleService.HostedService;
using ProcessMessageConsoleService.QueueEventHandlers;

using System;
using System.Threading.Tasks;

namespace ProcessMessageConsoleService
{
    public  class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task<int> Main(string[] args)
        {
            try
            {
                await BuildGenericHost().RunAsync();
                return 0;
            }
            catch (OperationCanceledException ex)
            {
                Logger.Warn(ex, "Console service cannot be finished in timely fashion. Exiting explicitly...");
                return 2;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Console service has been finished unexpectedly. Exiting...");
                return 1;
            }
        }

        private static IHost BuildGenericHost()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(cfgBuilder =>
                {
                    cfgBuilder.AddJsonFile("appsettings.json", true, true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    var rabbitSettings = ctx.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();
                    services.AddSingleton(rabbitSettings);
                    services.AddRabbitMq();
                    services.AddTransient<IJsonByteArraySerializer, JsonByteArraySerializer>();
                    services.AddSingleton<IRabbitMQBusMessageHandler<RabbitMQBusMessage, Notification>, RabbitMQMessageHandler>();
                    services.AddHostedService<MessageReceiveHostedService>();
                })
                 .ConfigureLogging((_, logConfig) =>
                 {
                     logConfig.SetMinimumLevel(LogLevel.Trace);
                     logConfig.AddConsole();
                 })
                .UseConsoleLifetime()
                .Build();


            return host;
        }
    }
}
