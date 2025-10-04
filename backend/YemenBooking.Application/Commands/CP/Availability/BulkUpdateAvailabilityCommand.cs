using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Availability;

public class BulkUpdateAvailabilityCommand : IRequest<ResultDto>
{
    public Guid UnitId { get; set; }
    public List<AvailabilityPeriodDto> Periods { get; set; }
    public bool OverwriteExisting { get; set; }
}

public class AvailabilityPeriodDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public bool OverwriteExisting { get; set; }
}
