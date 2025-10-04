using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Units;

namespace YemenBooking.Application.Queries.MobileApp.UnitTypes;

/// <summary>
/// استعلام الحصول على أنواع الوحدات المتاحة
/// Query to get available unit types
/// </summary>
public class GetUnitTypesQuery : IRequest<ResultDto<List<YemenBooking.Application.DTOs.UnitTypeDto>>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// </summary>
    public Guid PropertyTypeId { get; set; }
}