using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Queries.Units;

/// <summary>
/// استعلام للحصول على جدول توفر الوحدة
/// Query to get unit availability schedule
/// </summary>
public class GetUnitAvailabilityQuery : IRequest<ResultDto<UnitAvailabilityDto>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// تاريخ البداية (اختياري)
    /// Start date (optional)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية (اختياري)
    /// End date (optional)
    /// </summary>
    public DateTime? EndDate { get; set; }
} 