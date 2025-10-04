using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Commands.CP.Availability;

public class DeleteAvailabilityCommand : IRequest<ResultDto>
{
    public Guid UnitId { get; set; }
    public Guid? AvailabilityId { get; set; } // حذف سجل محدد
    public DateTime? StartDate { get; set; } // حذف بالفترة
    public DateTime? EndDate { get; set; }
    public bool ForceDelete { get; set; } = false; // حذف حتى لو كانت محجوزة
}
