using MediatR;
using YemenBooking.Application.Commands.CP.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Availability;

public class CloneAvailabilityCommandHandler : IRequestHandler<CloneAvailabilityCommand, ResultDto>
{
    private readonly IUnitAvailabilityRepository _repository;

    public CloneAvailabilityCommandHandler(IUnitAvailabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultDto> Handle(CloneAvailabilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get source availability
            var sourceAvailabilities = await _repository.GetByDateRangeAsync(
                request.UnitId, 
                request.SourceStartDate, 
                request.SourceEndDate);

            if (!sourceAvailabilities.Any())
                return ResultDto.Failure("لا توجد بيانات إتاحة في الفترة المصدر");

            var newAvailabilities = new List<UnitAvailability>();
            var daysDiff = (request.SourceEndDate - request.SourceStartDate).Days;

            for (int i = 0; i < request.RepeatCount; i++)
            {
                var targetStart = request.TargetStartDate.AddDays(daysDiff * i + i);
                
                foreach (var source in sourceAvailabilities)
                {
                    var sourceDayOffset = (source.StartDate - request.SourceStartDate).Days;
                    var newAvailability = new UnitAvailability
                    {
                        Id = Guid.NewGuid(),
                        UnitId = request.UnitId,
                        StartDate = targetStart.AddDays(sourceDayOffset),
                        EndDate = targetStart.AddDays(sourceDayOffset + (source.EndDate - source.StartDate).Days),
                        Status = source.Status,
                        Reason = source.Reason,
                        Notes = $"مستنسخ من {source.StartDate:yyyy-MM-dd}",
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    newAvailabilities.Add(newAvailability);
                }
            }

            await _repository.BulkCreateAsync(newAvailabilities);
            await _repository.SaveChangesAsync(cancellationToken);
            return ResultDto.Ok();
        }
        catch (Exception ex)
        {
            return ResultDto.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}