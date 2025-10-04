using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Units
{
    /// <summary>
    /// معالج استعلام الحصول على الفترات المتاحة للوحدة
    /// Query handler for GetUnitAvailabilityQuery
    /// </summary>
    public class GetUnitAvailabilityQueryHandler : IRequestHandler<GetUnitAvailabilityQuery, ResultDto<UnitAvailabilityDto>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ILogger<GetUnitAvailabilityQueryHandler> _logger;

        public GetUnitAvailabilityQueryHandler(
            IUnitRepository unitRepository,
            ILogger<GetUnitAvailabilityQueryHandler> logger)
        {
            _unitRepository = unitRepository;
            _logger = logger;
        }

        public async Task<ResultDto<UnitAvailabilityDto>> Handle(GetUnitAvailabilityQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام توفر الوحدة: {UnitId}", request.UnitId);

            var start = request.StartDate ?? DateTime.Today;
            var end = request.EndDate ?? DateTime.Today.AddDays(30);

            var availability = await _unitRepository.GetUnitAvailabilityAsync(request.UnitId, start, end, cancellationToken);

            if (availability == null || !availability.Any())
                return ResultDto<UnitAvailabilityDto>.Failure("لا توجد بيانات للتوفر");

            var availableDates = availability.Where(kv => kv.Value).Select(kv => kv.Key).OrderBy(d => d).ToList();
            if (!availableDates.Any())
                return ResultDto<UnitAvailabilityDto>.Failure("لا توجد تواريخ متاحة");

            var blockStart = availableDates.First();
            var blockEnd = blockStart;
            foreach (var date in availableDates.Skip(1))
            {
                if (date == blockEnd.AddDays(1))
                    blockEnd = date;
                else
                    break;
            }

            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId, cancellationToken);
            var unitName = unit?.Name ?? string.Empty;

            var dto = new UnitAvailabilityDto
            {
                UnitId = request.UnitId,
                UnitName = unitName,
                StartDate = blockStart,
                EndDate = blockEnd
            };

            _logger.LogInformation("تم جلب بيانات التوفر للوحدة بنجاح: {UnitId}", request.UnitId);
            return ResultDto<UnitAvailabilityDto>.Succeeded(dto);
        }
    }
} 