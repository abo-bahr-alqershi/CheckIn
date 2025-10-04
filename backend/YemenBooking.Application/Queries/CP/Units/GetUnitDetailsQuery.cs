namespace YemenBooking.Application.Queries.Units;

using MediatR;
using System;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Units;

/// <summary>
/// جلب تفاصيل الوحدة مع الحقول الديناميكية
/// Get unit details including dynamic field values
/// </summary>
public class GetUnitDetailsQuery : IRequest<ResultDto<UnitDetailsDto>>
{
    /// <summary>
    /// معرف الوحدة
    /// UnitId
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// تضمين القيم الديناميكية (اختياري)
    /// IncludeDynamicFields
    /// </summary>
    public bool IncludeDynamicFields { get; set; } = true;
} 