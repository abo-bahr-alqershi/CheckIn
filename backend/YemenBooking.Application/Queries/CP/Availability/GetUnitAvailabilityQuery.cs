using MediatR;
using YemenBooking.Application.Commands.CP.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Queries.CP.Availability;

public class GetUnitAvailabilityQuery : IRequest<ResultDto<UnitAvailabilityDto>>
{
    public Guid UnitId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

public class UnitAvailabilityDto
{
    public Guid UnitId { get; set; }
    public string UnitName { get; set; }
    public Dictionary<DateTime, AvailabilityStatusDto> Calendar { get; set; }
    public List<AvailabilityPeriodDto> Periods { get; set; }
    public AvailabilityStatsDto Stats { get; set; }
}

public class AvailabilityStatusDto
{
    public string Status { get; set; }
    public string? Reason { get; set; }
    public string? BookingId { get; set; }
    public string ColorCode { get; set; }
}

public class AvailabilityStatsDto
{
    public int TotalDays { get; set; }
    public int AvailableDays { get; set; }
    public int BookedDays { get; set; }
    public int BlockedDays { get; set; }
    public int MaintenanceDays { get; set; }
    public decimal OccupancyRate { get; set; }
}