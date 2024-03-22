using System.Text.Json.Serialization;

namespace Core.Data.Notifications;

public sealed record Notification
{
    [JsonPropertyName("deltas")] public NotificationDelta[] Deltas { get; init; }
}