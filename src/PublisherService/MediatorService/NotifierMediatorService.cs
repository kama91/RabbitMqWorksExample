using Core.NotificationHandlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace PublisherService.MediatorService
{
    public interface INotifierMediatorService
    {
        Task Notify(EventNotification notificationMessage, CancellationToken cancelleationToken = default);
    }

    public class NotifierMediatorService : INotifierMediatorService
    {
        private readonly IMediator _mediator;

        public NotifierMediatorService(IMediator mediator)
        {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task Notify(EventNotification notificationMessage, CancellationToken cancelleationToken)
        {
            await _mediator.Publish(notificationMessage, cancelleationToken);
        }
    }
}
