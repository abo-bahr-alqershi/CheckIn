using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Units
{
    /// <summary>
    /// معالج استعلام الحصول على الوحدات المتاحة
    /// Query handler for GetAvailableUnitsQuery
    /// </summary>
    public class GetAvailableUnitsQueryHandler : IRequestHandler<GetAvailableUnitsQuery, PaginatedResult<UnitDto>>
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAvailableUnitsQueryHandler> _logger;

        public GetAvailableUnitsQueryHandler(
            IAvailabilityService availabilityService,
            IUnitRepository unitRepository,
            ICurrentUserService currentUserService,
            ILogger<GetAvailableUnitsQueryHandler> logger)
        {
            _availabilityService = availabilityService;
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<UnitDto>> Handle(GetAvailableUnitsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الوحدات المتاحة للكيان: {PropertyId}", request.PropertyId);

            var availableIds = await _availabilityService.GetAvailableUnitsInPropertyAsync(
                request.PropertyId,
                request.CheckInDate,
                request.CheckOutDate,
                request.NumberOfGuests,
                cancellationToken);

            var query = _unitRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.Property)
                .Include(u => u.UnitType)
                .Include(u => u.FieldValues)
                    .ThenInclude(fv => fv.UnitTypeField)
                .Where(u => availableIds.Contains(u.Id));

            if (request.IsAvailable.HasValue)
                query = query.Where(u => u.IsAvailable == request.IsAvailable.Value);

            if (request.MinBasePrice.HasValue)
                query = query.Where(u => u.BasePrice.Amount >= request.MinBasePrice.Value);

            if (request.MaxBasePrice.HasValue)
                query = query.Where(u => u.BasePrice.Amount <= request.MaxBasePrice.Value);

            if (request.MinCapacity.HasValue)
                query = query.Where(u => u.MaxCapacity >= request.MinCapacity.Value);

            if (!string.IsNullOrWhiteSpace(request.NameContains))
                query = query.Where(u => u.Name.Contains(request.NameContains));

            var totalCount = await query.CountAsync(cancellationToken);

            var units = await query
                .OrderBy(u => u.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = units.Select(u => new UnitDto
            {
                Id = u.Id,
                PropertyId = u.PropertyId,
                UnitTypeId = u.UnitTypeId,
                Name = u.Name,
                BasePrice = new MoneyDto { Amount = u.BasePrice.Amount, Currency = u.BasePrice.Currency },
                CustomFeatures = u.CustomFeatures,
                IsAvailable = u.IsAvailable,
                PropertyName = u.Property.Name,
                UnitTypeName = u.UnitType.Name,
                PricingMethod = u.PricingMethod,
                FieldValues = u.FieldValues.Select(fv => new UnitFieldValueDto
                {
                    FieldId = fv.UnitTypeFieldId,
                    FieldName = fv.UnitTypeField?.FieldName ?? string.Empty,
                    DisplayName = fv.UnitTypeField?.DisplayName ?? string.Empty,
                    FieldType = fv.UnitTypeField?.FieldTypeId ?? string.Empty,
                    FieldValue = fv.FieldValue,
                    IsPrimaryFilter = fv.UnitTypeField?.IsPrimaryFilter ?? false
                }).ToList()
            }).ToList();

            return PaginatedResult<UnitDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}