using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Events;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.EventHandlers
{
    /// <summary>
    /// معالج أحداث الوحدات
    /// </summary>
    public class UnitEventHandler :
        INotificationHandler<UnitCreatedEvent>,
        INotificationHandler<UnitUpdatedEvent>,
        INotificationHandler<UnitDeletedEvent>
    {
        private readonly IIndexingService _indexService;
        private readonly ILogger<UnitEventHandler> _logger;

        public UnitEventHandler(
            IIndexingService indexService,
            ILogger<UnitEventHandler> logger)
        {
            _indexService = indexService;
            _logger = logger;
        }

        public async Task Handle(UnitCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _indexService.OnUnitCreatedAsync(
                notification.UnitId,
                notification.PropertyId,
                cancellationToken);
        }

        public async Task Handle(UnitUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await _indexService.OnUnitUpdatedAsync(
                notification.UnitId,
                notification.PropertyId,
                cancellationToken);
        }

        public async Task Handle(UnitDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _indexService.OnUnitDeletedAsync(
                notification.UnitId,
                notification.PropertyId,
                cancellationToken);
        }
    }
}