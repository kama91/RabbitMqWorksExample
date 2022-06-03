using System.Text.Json.Serialization;

namespace Core.Data.Notifications
{
    public class NotificationData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }
    }
}
