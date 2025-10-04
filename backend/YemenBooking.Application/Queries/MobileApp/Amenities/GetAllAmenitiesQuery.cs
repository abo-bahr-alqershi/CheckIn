using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;

namespace YemenBooking.Application.Queries.MobileApp.Amenities;

/// <summary>
/// استعلام الحصول على جميع وسائل الراحة
/// Query to get all amenities
/// </summary>
public class GetAllAmenitiesQuery : IRequest<ResultDto<List<AmenityDto>>>
{
    /// <summary>
    /// فلترة حسب الفئة (اختياري)
    /// </summary>
    public string? Category { get; set; }
}