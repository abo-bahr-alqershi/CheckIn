using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Statistics;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.MobileApp.SearchFilters;

/// <summary>
/// استعلام الحصول على الوجهات الشعبية
/// Query to get popular destinations
/// </summary>
public class GetPopularDestinationsQuery : IRequest<ResultDto<List<PopularDestinationDto>>>
{
    /// <summary>
    /// عدد الوجهات المطلوبة
    /// </summary>
    public int Count { get; set; } = 10;
}