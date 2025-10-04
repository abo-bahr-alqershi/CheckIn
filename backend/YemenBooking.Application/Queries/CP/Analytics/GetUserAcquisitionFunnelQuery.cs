using System;
using MediatR;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Analytics;

/// <summary>
/// استعلام للحصول على بيانات قمع اكتساب العملاء ضمن نطاق زمني
/// Query to get user acquisition funnel data within a date range
/// </summary>
public class GetUserAcquisitionFunnelQuery : IRequest<ResultDto<UserFunnelDto>>
{
    /// <summary>
    /// النطاق الزمني
    /// Date range
    /// </summary>
    public DateRangeDto Range { get; set; }

    public GetUserAcquisitionFunnelQuery(DateRangeDto range)
    {
        Range = range;
    }
} 