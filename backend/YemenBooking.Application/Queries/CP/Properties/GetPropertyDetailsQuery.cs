namespace YemenBooking.Application.Queries.Properties;

using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;

/// <summary>
/// جلب تفاصيل الكيان مع الحقول الديناميكية
/// Get property details including dynamic field values
/// </summary>
public class GetPropertyDetailsQuery : IRequest<ResultDto<PropertyDetailsDto>>
{
    /// <summary>
    /// معرف الكيان
    /// PropertyId
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// تضمين الوحدات الفرعية (اختياري)
    /// IncludeUnits
    /// </summary>
    public bool IncludeUnits { get; set; } = true;

    /// <summary>
    /// تضمين الحقول الديناميكية (اختياري)
    /// Include dynamic fields (optional)
    /// </summary>
    public bool IncludeDynamicFields { get; set; } = true;
} 