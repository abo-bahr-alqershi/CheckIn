using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Events;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.EventHandlers
{
    /// <summary>
    /// معالج أحداث العقارات
    /// </summary>
    public class PropertyEventHandler :
        INotificationHandler<PropertyCreatedEvent>,
        INotificationHandler<PropertyUpdatedEvent>,
        INotificationHandler<PropertyDeletedEvent>
    {
        private readonly IIndexingService _indexService;
        private readonly ILogger<PropertyEventHandler> _logger;

        public PropertyEventHandler(
            IIndexingService indexService,
            ILogger<PropertyEventHandler> logger)
        {
            _indexService = indexService;
            _logger = logger;
        }

        public async Task Handle(PropertyCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _indexService.OnPropertyCreatedAsync(notification.PropertyId, cancellationToken);
        }

        public async Task Handle(PropertyUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await _indexService.OnPropertyUpdatedAsync(notification.PropertyId, cancellationToken);
        }

        public async Task Handle(PropertyDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _indexService.OnPropertyDeletedAsync(notification.PropertyId, cancellationToken);
        }
    }

}