using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reports;

/// <summary>
/// استعلام للحصول على البلاغات المتعلقة بكيان معين
/// Query to get reports for a specific property
/// </summary>
public class GetReportsByPropertyQuery : IRequest<PaginatedResult<ReportDto>>
{
    /// <summary>
    /// معرف الكيان
    /// Property identifier
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
} 