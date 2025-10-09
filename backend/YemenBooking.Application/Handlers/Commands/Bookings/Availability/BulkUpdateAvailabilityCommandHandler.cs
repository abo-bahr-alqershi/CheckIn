using MediatR;
using YemenBooking.Application.Commands.CP.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Availability;


public class BulkUpdateAvailabilityCommandHandler : IRequestHandler<BulkUpdateAvailabilityCommand, ResultDto>
{
    private readonly IAvailabilityService _availabilityService;

    public BulkUpdateAvailabilityCommandHandler(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    public async Task<ResultDto> Handle(BulkUpdateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _availabilityService.ApplyBulkAvailabilityAsync(request.UnitId, request.Periods);
            return ResultDto.Ok();
        }
        catch (Exception ex)
        {
            return ResultDto.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}