using System.Text.Json.Serialization;

namespace Core.Data.Notifications
{
    public sealed class NotificationDelta
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("object_data")]
        public NotificationData Data { get; set; }
    }
}
