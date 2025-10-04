using System;
using MediatR;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Analytics;

/// <summary>
/// استعلام للحصول على تفصيل الإيرادات الكلي لمنصة ضمن نطاق زمني
/// Query to get total platform revenue breakdown within a date range
/// </summary>
public class GetPlatformRevenueBreakdownQuery : IRequest<ResultDto<RevenueBreakdownDto>>
{
    /// <summary>
    /// النطاق الزمني
    /// Date range
    /// </summary>
    public DateRangeDto Range { get; set; }

    public GetPlatformRevenueBreakdownQuery(DateRangeDto range)
    {
        Range = range;
    }
} 