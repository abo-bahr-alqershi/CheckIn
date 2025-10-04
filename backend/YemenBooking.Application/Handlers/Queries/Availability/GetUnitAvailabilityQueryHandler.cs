using MediatR;
using YemenBooking.Application.Commands.CP.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.CP.Availability;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Availability;


public class GetUnitAvailabilityQueryHandler : IRequestHandler<GetUnitAvailabilityQuery, ResultDto<Application.Queries.CP.Availability.UnitAvailabilityDto>>
{
    private readonly IUnitAvailabilityRepository _availabilityRepository;
    private readonly IUnitRepository _unitRepository;

    public GetUnitAvailabilityQueryHandler(
        IUnitAvailabilityRepository availabilityRepository,
        IUnitRepository unitRepository)
    {
        _availabilityRepository = availabilityRepository;
        _unitRepository = unitRepository;
    }

    public async Task<ResultDto<Application.Queries.CP.Availability.UnitAvailabilityDto>> Handle(GetUnitAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var unit = await _unitRepository.GetByIdAsync(request.UnitId);
        if (unit == null)
            return ResultDto<Application.Queries.CP.Availability.UnitAvailabilityDto>.Failure("الوحدة غير موجودة");

        var calendar = await _availabilityRepository.GetAvailabilityCalendarAsync(
            request.UnitId, 
            request.Year, 
            request.Month);

        var startOfMonth = new DateTime(request.Year, request.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        
        var periods = await _availabilityRepository.GetByDateRangeAsync(
            request.UnitId,
            startOfMonth,
            endOfMonth);

        var dto = new Application.Queries.CP.Availability.UnitAvailabilityDto
        {
            UnitId = unit.Id,
            UnitName = unit.Name,
            Calendar = calendar.ToDictionary(
                kvp => kvp.Key,
                kvp => new AvailabilityStatusDto
                {
                    Status = kvp.Value,
                    ColorCode = GetColorCode(kvp.Value)
                }),
            Periods = periods.Select(p => new AvailabilityPeriodDto
            {
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                Reason = p.Reason,
                Notes = p.Notes
            }).ToList(),
            Stats = CalculateStats(calendar)
        };

        return ResultDto<Application.Queries.CP.Availability.UnitAvailabilityDto>.Ok(dto);
    }

    private string GetColorCode(string status)
    {
        return status switch
        {
            "Available" => "#10B981",    // Green
            "Booked" => "#EF4444",       // Red
            "Blocked" => "#F59E0B",      // Orange
            "Maintenance" => "#6B7280",  // Gray
            _ => "#E5E7EB"              // Light Gray
        };
    }

    private AvailabilityStatsDto CalculateStats(Dictionary<DateTime, string> calendar)
    {
        var total = calendar.Count;
        var available = calendar.Count(c => c.Value == "Available");
        var booked = calendar.Count(c => c.Value == "Booked");
        var blocked = calendar.Count(c => c.Value == "Blocked");
        var maintenance = calendar.Count(c => c.Value == "Maintenance");

        return new AvailabilityStatsDto
        {
            TotalDays = total,
            AvailableDays = available,
            BookedDays = booked,
            BlockedDays = blocked,
            MaintenanceDays = maintenance,
            OccupancyRate = total > 0 ? (decimal)booked / total * 100 : 0
        };
    }
}