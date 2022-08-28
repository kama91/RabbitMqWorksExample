using Core.Data.Notifications;

using MediatR;

namespace Infrastructure.NotificationHandlers
{
    public class NotificationCommand : INotification
    {
        public Notification Notification { get; set; }
    }
}
