using MediatR;
using System;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Units;

namespace YemenBooking.Application.Queries.Units;

/// <summary>
/// استعلام للحصول على بيانات الوحدة بواسطة المعرف
/// Query to get unit details by ID
/// </summary>
public class GetUnitByIdQuery : IRequest<ResultDto<UnitDetailsDto>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }
} 