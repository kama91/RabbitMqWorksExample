
using Core.Data.Notifications;
using Core.NotificationHandlers;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PublisherService.MediatorService;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarService.API.Controllers
{
    [ApiController]
    [Route("api/notification/")]
    public class NotificationController : ControllerBase
    {
        private readonly INotifierMediatorService _notifierMediatorService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotifierMediatorService notifierMediatorService,
            ILogger<NotificationController> logger)
        {
            _notifierMediatorService = notifierMediatorService ?? throw new ArgumentNullException(nameof(notifierMediatorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("events")]
        public async Task<ActionResult<string>> NotifyEvents([FromBody] Notification notification)
        {
            await _notifierMediatorService.Notify(new EventNotification { Notification = notification });                

            return Ok();
        }
    }
}
