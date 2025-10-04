namespace YemenBooking.Application.Queries.FieldGroups;

using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب مجموعات الحقول لنوع وحدة معين
/// Get field groups by unit type
/// </summary>
public class GetFieldGroupsByUnitTypeQuery : IRequest<List<FieldGroupDto>>
{
    /// <summary>
    /// معرف نوع الوحدة
    /// UnitTypeId
    /// </summary>
    public string UnitTypeId { get; set; }
} 