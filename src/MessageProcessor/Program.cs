using System;
using System.Reflection;
using System.Threading.Tasks;
using Core.Data.Notifications;
using Core.Serializer;
using Core.Serializer.Abstractions;
using Infrastructure.RabbitMq;
using Infrastructure.RabbitMq.Abstractions;
using Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions;
using MessageProcessor.BusMessageHandlers;
using MessageProcessor.HostedService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MessageProcessor;

public class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static async Task<int> Main(string[] args)
    {
        try
        {
            await BuildGenericHost(args).RunAsync();
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

    private static IHost BuildGenericHost(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration(cfgBuilder =>
            {
                cfgBuilder.AddJsonFile("appsettings.json", true, false)
                    .AddEnvironmentVariables()
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddCommandLine(args);
            })
            .ConfigureServices((ctx, services) =>
            {
                var rabbitSettings = ctx.Configuration.GetSection("rabbitmq").Get<RabbitMqSettings>();
                services.AddSingleton(rabbitSettings);
                services.AddRabbitMq();
                services.AddTransient<IJsonByteArraySerializer, JsonByteArraySerializer>();
                services
                    .AddSingleton<IRabbitMqBusMessageHandler<RabbitMqBusMessage, Notification>,
                        RabbitMqMessageHandler>();
                services.AddHostedService<MessageReceiveHostedService>();
            })
            .ConfigureLogging((_, logConfig) =>
            {
                logConfig.SetMinimumLevel(LogLevel.Information);
                logConfig.AddConsole();
            })
            .UseConsoleLifetime()
            .Build();


        return host;
    }
}