using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Analytics;

/// <summary>
/// استعلام للحصول على تحليل أسباب إلغاء الحجوزات ضمن نطاق زمني
/// Query to get cancellation reasons analysis within a date range
/// </summary>
public class GetPlatformCancellationAnalysisQuery : IRequest<ResultDto<List<CancellationReasonDto>>>
{
    /// <summary>
    /// النطاق الزمني
    /// Date range
    /// </summary>
    public DateRangeDto Range { get; set; }

    public GetPlatformCancellationAnalysisQuery(DateRangeDto range)
    {
        Range = range;
    }
} 