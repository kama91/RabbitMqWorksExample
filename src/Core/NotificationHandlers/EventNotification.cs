using Core.Data.Notifications;

using MediatR;

namespace Core.NotificationHandlers
{
    public class EventNotification : INotification
    {
        public Notification Notification { get; set; }
    }
}
