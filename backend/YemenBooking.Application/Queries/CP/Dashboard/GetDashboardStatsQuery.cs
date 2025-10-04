using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Dashboard
{
    /// <summary>
    /// استعلام للحصول على إحصائيات لوحة التحكم
    /// Query to get dashboard statistics
    /// </summary>
    public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
        // لا توجد معلمات
    }
} 