using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Commands.CP.Availability;

public class CloneAvailabilityCommand : IRequest<ResultDto>
{
    public Guid UnitId { get; set; }
    public DateTime SourceStartDate { get; set; }
    public DateTime SourceEndDate { get; set; }
    public DateTime TargetStartDate { get; set; }
    public int RepeatCount { get; set; } = 1;
}
