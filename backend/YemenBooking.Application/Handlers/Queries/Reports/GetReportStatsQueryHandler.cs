using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Reports;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Reports
{
    /// <summary>
    /// معالج استعلام إحصائيات البلاغات
    /// Handles GetReportStatsQuery and returns report analytics
    /// </summary>
    public class GetReportStatsQueryHandler : IRequestHandler<GetReportStatsQuery, ReportStatsDto>
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILogger<GetReportStatsQueryHandler> _logger;

        public GetReportStatsQueryHandler(
            IReportRepository reportRepository,
            ILogger<GetReportStatsQueryHandler> logger)
        {
            _reportRepository = reportRepository;
            _logger = logger;
        }

        public async Task<ReportStatsDto> Handle(GetReportStatsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة استعلام GetReportStats");
            // جلب كل البلاغات دون فلترة
            var allReports = await _reportRepository.GetReportsAsync(null, null, null, cancellationToken);

            // الإحصائيات الأساسية
            var total = allReports.Count();
            var pending = allReports.Count(r => r.Status.Equals("pending", StringComparison.OrdinalIgnoreCase));
            var resolved = allReports.Count(r => r.Status.Equals("resolved", StringComparison.OrdinalIgnoreCase));
            var dismissed = allReports.Count(r => r.Status.Equals("dismissed", StringComparison.OrdinalIgnoreCase));
            var escalated = allReports.Count(r => r.Status.Equals("escalated", StringComparison.OrdinalIgnoreCase));

            // حساب متوسط زمن الحل للبلاغات المحلولة
            var resolutionTimes = allReports
                .Where(r => r.Status.Equals("resolved", StringComparison.OrdinalIgnoreCase) && r.UpdatedAt > r.CreatedAt)
                .Select(r => (r.UpdatedAt - r.CreatedAt).TotalDays);
            var avgResolution = resolutionTimes.Any() ? Math.Round(resolutionTimes.Average(), 2) : 0;

            // عدد البلاغات حسب السبب (category)
            var byCategory = allReports
                .GroupBy(r => r.Reason)
                .ToDictionary(g => g.Key, g => g.Count());

            // اتجاه البلاغات خلال آخر 7 أيام
            var today = DateTime.UtcNow.Date;
            var trend = Enumerable.Range(0, 7)
                .Select(i =>
                {
                    var date = today.AddDays(-i);
                    var count = allReports.Count(r => r.CreatedAt.Date == date);
                    return new ReportTrendItem { Date = date, Count = count };
                })
                .Reverse()
                .ToList();

            return new ReportStatsDto
            {
                TotalReports = total,
                PendingReports = pending,
                ResolvedReports = resolved,
                DismissedReports = dismissed,
                EscalatedReports = escalated,
                AverageResolutionTime = avgResolution,
                ReportsByCategory = byCategory,
                ReportsTrend = trend
            };
        }
    }
} 