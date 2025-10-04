using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reports;

/// <summary>
/// استعلام للحصول على البلاغات المقدمة ضد مستخدم معين
/// Query to get reports filed against a specific user
/// </summary>
public class GetReportsByReportedUserQuery : IRequest<PaginatedResult<ReportDto>>
{
    /// <summary>
    /// معرف المستخدم المبلغ عنه
    /// Reported user identifier
    /// </summary>
    public Guid UserId { get; set; }

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