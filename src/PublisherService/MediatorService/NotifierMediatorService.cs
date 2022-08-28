using Infrastructure.NotificationHandlers;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace PublisherService.MediatorService
{
    public interface INotifierMediatorService
    {
        Task Notify(NotificationCommand notificationCommand, CancellationToken cancelleationToken = default);
    }

    public class NotifierMediatorService : INotifierMediatorService
    {
        private readonly IMediator _mediator;

        public NotifierMediatorService(IMediator mediator)
        {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task Notify(NotificationCommand notificationCommand, CancellationToken cancelleationToken)
        {
            await _mediator.Publish(notificationCommand, cancelleationToken);
        }
    }
}
