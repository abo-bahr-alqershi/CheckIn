using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Notifications
{
    /// <summary>
    /// استعلام إحصائيات الإشعارات للوحة الإدارة
    /// </summary>
    public class GetNotificationsStatsQuery : IRequest<NotificationsStatsDto>
    {
        public string? Type { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}

