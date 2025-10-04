using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Units;

/// <summary>
/// أمر لحذف الوحدة
/// Command to delete a unit
/// </summary>
public class DeleteUnitCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }
} 