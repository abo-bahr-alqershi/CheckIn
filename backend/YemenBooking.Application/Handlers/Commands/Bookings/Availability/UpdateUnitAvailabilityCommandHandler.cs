using AutoMapper;
using MediatR;
using YemenBooking.Application.Commands.CP.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Availability;

public class UpdateUnitAvailabilityCommandHandler : IRequestHandler<UpdateUnitAvailabilityCommand, ResultDto>
{
    private readonly IUnitAvailabilityRepository _repository;
    private readonly IMapper _mapper;

    public UpdateUnitAvailabilityCommandHandler(
        IUnitAvailabilityRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResultDto> Handle(UpdateUnitAvailabilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate dates
            if (request.EndDate <= request.StartDate)
                return ResultDto.Failure("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");

            // Delete existing if overwrite is requested
            if (request.OverwriteExisting)
            {
                await _repository.DeleteRangeAsync(request.UnitId, request.StartDate, request.EndDate);
            }

            var availability = new UnitAvailability
            {
                Id = Guid.NewGuid(),
                UnitId = request.UnitId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                Reason = request.Reason,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(availability);
            await _repository.SaveChangesAsync(cancellationToken);
            return ResultDto.Ok("تم تحديث الإتاحة وحفظها");
        }
        catch (Exception ex)
        {
            return ResultDto.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}