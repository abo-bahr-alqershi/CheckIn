using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Notifications;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Notifications
{
    /// <summary>
    /// معالج إحصائيات الإشعارات
    /// </summary>
    public class GetNotificationsStatsQueryHandler : IRequestHandler<GetNotificationsStatsQuery, NotificationsStatsDto>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetNotificationsStatsQueryHandler> _logger;

        public GetNotificationsStatsQueryHandler(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService,
            ILogger<GetNotificationsStatsQueryHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<NotificationsStatsDto> Handle(GetNotificationsStatsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.Role != "Admin")
                return new NotificationsStatsDto();

            var q = _notificationRepository.GetQueryable();
            if (!string.IsNullOrWhiteSpace(request.Type)) q = q.Where(n => n.Type == request.Type);
            if (request.From.HasValue) q = q.Where(n => n.CreatedAt >= request.From.Value);
            if (request.To.HasValue) q = q.Where(n => n.CreatedAt <= request.To.Value);

            var now = DateTime.UtcNow.Date;
            var last7 = now.AddDays(-7);
            var last30 = now.AddDays(-30);

            var stats = new NotificationsStatsDto
            {
                Total = q.Count(),
                Pending = q.Count(n => n.Status == "PENDING"),
                Sent = q.Count(n => n.Status == "SENT"),
                Delivered = q.Count(n => n.Status == "DELIVERED"),
                Read = q.Count(n => n.Status == "READ"),
                Failed = q.Count(n => n.Status == "FAILED"),
                Today = q.Count(n => n.CreatedAt >= now),
                Last7Days = q.Count(n => n.CreatedAt >= last7),
                Last30Days = q.Count(n => n.CreatedAt >= last30)
            };

            return await Task.FromResult(stats);
        }
    }
}

