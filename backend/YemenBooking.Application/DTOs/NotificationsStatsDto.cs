using System;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// إحصائيات الإشعارات المعروضة في لوحة الإدارة
    /// Notifications statistics for admin dashboard
    /// </summary>
    public class NotificationsStatsDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Sent { get; set; }
        public int Delivered { get; set; }
        public int Read { get; set; }
        public int Failed { get; set; }

        public int Today { get; set; }
        public int Last7Days { get; set; }
        public int Last30Days { get; set; }
    }
}

