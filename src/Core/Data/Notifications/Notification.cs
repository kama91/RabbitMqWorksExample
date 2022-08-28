using System.Text.Json.Serialization;

namespace Core.Data.Notifications
{
    public class Notification
    {
        [JsonPropertyName("deltas")]
        public NotificationDelta[] Deltas { get; set; }
    }
}
