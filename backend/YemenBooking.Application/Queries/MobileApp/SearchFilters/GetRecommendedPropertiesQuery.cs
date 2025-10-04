using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.PropertySearch;

namespace YemenBooking.Application.Queries.MobileApp.SearchFilters;

/// <summary>
/// استعلام الحصول على الكيانات الموصى بها للمستخدم
/// Query to get recommended properties for user
/// </summary>
public class GetRecommendedPropertiesQuery : IRequest<ResultDto<PaginatedResult<PropertySearchResultDto>>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// عدد الكيانات المطلوبة
    /// </summary>
    public int Count { get; set; } = 5;
    
    /// <summary>
    /// المدينة (اختياري للتوصيات المحلية)
    /// </summary>
    public string? City { get; set; }
}