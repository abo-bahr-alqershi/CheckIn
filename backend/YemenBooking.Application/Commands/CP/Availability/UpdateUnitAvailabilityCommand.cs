using AutoMapper;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Commands.CP.Availability;

public class UpdateUnitAvailabilityCommand : IRequest<ResultDto>
{
    public Guid UnitId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } // Available, Blocked, Maintenance
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public bool OverwriteExisting { get; set; }
}
