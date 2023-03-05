using Core.Data.Notifications;
using Core.Serializer;
using Core.Serializer.Abstractions;
using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Abstractions;
using Infrastructure.RabbitMQ.RabbitMqMessage.Model.Abstractions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddTransient<IJsonByteArraySerializer, JsonByteArraySerializer>();
var rabbitSettings = builder.Configuration.GetSection("rabbitmq").Get<RabbitMQSettings>();
builder.Services.AddSingleton(rabbitSettings);
builder.Services.AddRabbitMQ();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PublisherService"));
}

app.UseHttpsRedirection();

app.MapPost("api/notification", async (IRabbitMQBus eventBus, RabbitMQSettings rabbitSettings, ILogger<Program> logger, Notification notification) =>
{
    try
    {
        await eventBus.Publish(new RabbitMQBusMessage(Guid.NewGuid(), notification), rabbitSettings.ExchangeName, "routeKey");

        return Results.Ok();
    }
    catch (Exception ex)
    {
        var ids = string.Join(", ", notification.Deltas.Select(d => d.Data.Id));

        logger.LogError("{Error} from handle notificationh next id's: {Ids}", ex, ids);

        return Results.BadRequest(ex);
    }
});

app.Run();

