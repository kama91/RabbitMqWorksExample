
using Core.Data.Notifications;

using Infrastructure.NotificationHandlers;

using Microsoft.AspNetCore.Mvc;

using PublisherService.MediatorService;

using System;
using System.Threading.Tasks;

namespace CalendarService.API.Controllers
{
    [ApiController]
    [Route("api/notification/")]
    public class NotificationController : ControllerBase
    {
        private readonly INotifierMediatorService _notifierMediatorService;

        public NotificationController(
            INotifierMediatorService notifierMediatorService)
        {
            _notifierMediatorService = notifierMediatorService ?? throw new ArgumentNullException(nameof(notifierMediatorService));
        }

        [HttpPost("events")]
        public async Task<ActionResult<string>> NotifyEvents([FromBody] Notification notification)
        {
            await _notifierMediatorService.Notify(new NotificationCommand { Notification = notification });                

            return Ok();
        }
    }
}
