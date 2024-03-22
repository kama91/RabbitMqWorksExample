using Core.Data.Notifications;
using Core.Serializer;
using Core.Serializer.Abstractions;
using Infrastructure.RabbitMq;
using Infrastructure.RabbitMq.Abstractions;
using Infrastructure.RabbitMq.RabbitMqMessage.Model.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IJsonByteArraySerializer, JsonByteArraySerializer>();
var rabbitSettings = builder.Configuration.GetSection("rabbitmq").Get<RabbitMqSettings>();
builder.Services.AddSingleton(rabbitSettings);
builder.Services.AddRabbitMq();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PublisherService"));

app.UseHttpsRedirection();

app.MapPost("api/notification", async (IRabbitMqBus eventBus, RabbitMqSettings rabbitMqSettings, ILogger<Program> logger,
    Notification notification) =>
{
    try
    {
        await eventBus.Publish(new RabbitMqBusMessage(Guid.NewGuid(), notification), rabbitMqSettings.ExchangeName,
            "routeKey");

        return Results.Ok();
    }
    catch (Exception ex)
    {
        var ids = string.Join(", ", notification.Deltas.Select(d => d.Data.Id));

        logger.LogError("{Error} from handle notification next id's: {Ids}", ex, ids);

        return Results.BadRequest(ex);
    }
});

app.Run();