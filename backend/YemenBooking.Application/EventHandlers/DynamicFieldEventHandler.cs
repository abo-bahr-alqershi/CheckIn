using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Events;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.EventHandlers
{

    /// <summary>
    /// معالج أحداث الحقول الديناميكية
    /// </summary>
    public class DynamicFieldEventHandler : INotificationHandler<DynamicFieldChangedEvent>
    {
        private readonly IIndexingService _indexService;

        public DynamicFieldEventHandler(IIndexingService indexService)
        {
            _indexService = indexService;
        }

        public async Task Handle(DynamicFieldChangedEvent notification, CancellationToken cancellationToken)
        {
            await _indexService.OnDynamicFieldChangedAsync(
                notification.PropertyId,
                notification.FieldName,
                notification.FieldValue,
                notification.IsAdd,
                cancellationToken);
        }
    }
}