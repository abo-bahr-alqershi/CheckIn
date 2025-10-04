using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Analytics;

/// <summary>
/// استعلام للحصول على تحليل أفواج العملاء ضمن نطاق زمني
/// Query to get customer cohort analysis within a date range
/// </summary>
public class GetCustomerCohortAnalysisQuery : IRequest<ResultDto<List<CohortDto>>>
{
    /// <summary>
    /// النطاق الزمني
    /// Date range
    /// </summary>
    public DateRangeDto Range { get; set; }

    public GetCustomerCohortAnalysisQuery(DateRangeDto range)
    {
        Range = range;
    }
} 