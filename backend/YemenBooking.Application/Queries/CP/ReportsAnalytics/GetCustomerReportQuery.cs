using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.ReportsAnalytics
{
    /// <summary>
    /// استعلام للحصول على تقرير العملاء
    /// Query to get customer report
    /// </summary>
    public class GetCustomerReportQuery : IRequest<ResultDto<CustomerReportDto>>
    {
        /// <summary>
        /// تاريخ البداية
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ النهاية
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// معايير التصفية (اختياري)
        /// </summary>
        public string? Filters { get; set; }
    }
} 