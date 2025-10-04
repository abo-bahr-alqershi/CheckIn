using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reports
{
    /// <summary>
    /// استعلام للحصول على إحصائيات وتحليلات البلاغات
    /// Query to retrieve report analytics and statistics
    /// </summary>
    public class GetReportStatsQuery : IRequest<ReportStatsDto>
    {
    }
} 