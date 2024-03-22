using System.Text.Json.Serialization;

namespace Core.Data.Notifications;

public sealed record NotificationDelta
{
    [JsonPropertyName("type")] public string Type { get; init; }

    [JsonPropertyName("object_data")] public NotificationData Data { get; init; }
}