using System.Text.Json.Serialization;

namespace Core.Data.Notifications;

public sealed record NotificationData
{
    [JsonPropertyName("id")] public string Id { get; init; }

    [JsonPropertyName("account_id")] public string AccountId { get; init; }
}