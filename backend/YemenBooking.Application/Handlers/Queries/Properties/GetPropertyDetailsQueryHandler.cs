using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;
using YemenBooking.Application.Queries.Properties;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام جلب تفاصيل الكيان مع الحقول الديناميكية والوحدات الاختيارية
    /// Query handler to get property details with optional dynamic fields and units
    /// </summary>
    public class GetPropertyDetailsQueryHandler : IRequestHandler<GetPropertyDetailsQuery, ResultDto<PropertyDetailsDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFieldGroupRepository _fieldGroupRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyDetailsQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertyDetailsQueryHandler(
            IPropertyRepository propertyRepository,
            IFieldGroupRepository fieldGroupRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyDetailsQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _fieldGroupRepository = fieldGroupRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<ResultDto<PropertyDetailsDto>> Handle(GetPropertyDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء جلب تفاصيل الكيان: {PropertyId}", request.PropertyId);

                // 1. التحقق من صحة المعرف
                if (request.PropertyId == Guid.Empty)
                    return ResultDto<PropertyDetailsDto>.Failure("معرف الكيان غير صالح");

                // 2. بناء الاستعلام
                var query = _propertyRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(p => p.PropertyType)
                    .Include(p => p.Owner)
                    .Include(p => p.Policies)
                    .Include(p => p.Images.Where(i => !i.IsDeleted))
                    .Include(p => p.Amenities).ThenInclude(pa => pa.PropertyTypeAmenity).ThenInclude(pta => pta.Amenity);


                var property = await query.FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

                if (property == null)
                    return ResultDto<PropertyDetailsDto>.Failure($"الكيان بالمعرف {request.PropertyId} غير موجود");

                // 3. الصلاحيات
                var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
                if (!property.IsApproved)
                {
                    if (currentUser == null)
                        return ResultDto<PropertyDetailsDto>.Failure("يجب تسجيل الدخول لعرض هذا الكيان");
                    if (property.OwnerId != _currentUserService.UserId && !_currentUserService.UserRoles.Contains("Admin"))
                        return ResultDto<PropertyDetailsDto>.Failure("ليس لديك صلاحية لعرض هذا الكيان");
                }

                // 4. بناء DTO
                var dto = new PropertyDetailsDto
                {
                    Id = property.Id,
                    OwnerId = property.OwnerId,
                    TypeId = property.TypeId,
                    Name = property.Name,
                    Address = property.Address,
                    City = property.City,
                    Latitude = property.Latitude,
                    Longitude = property.Longitude,
                    StarRating = property.StarRating,
                    Description = property.Description,
                    IsApproved = property.IsApproved,
                    CreatedAt = property.CreatedAt,
                    OwnerName = property.Owner.Name,
                    TypeName = property.PropertyType.Name,
                    Policies = property.Policies?.Select(pp => new PropertyPolicyDto
                    {
                        Id = pp.Id,
                        PolicyType = pp.Type.ToString(),
                        Description = pp.Description,
                        IsActive = pp.IsActive,
                        Type = pp.Type.ToString(),
                        Rules = pp.Rules != null ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(pp.Rules) : new Dictionary<string, object>()
                    }).ToList() ?? new List<PropertyPolicyDto>(),
                    Images = property.Images.Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        Name = i.Name,
                        SizeBytes = i.SizeBytes,
                        IsMain = i.IsMain,
                        Type = i.Type,
                        AltText = i.AltText
                    }).ToList(),
                    Units = request.IncludeUnits && property.Units != null
                        ? property.Units.Select(u => new YemenBooking.Application.DTOs.Units.UnitDetailsDto
                        {
                            Id = u.Id,
                            PropertyId = u.PropertyId,
                            PropertyName = property.Name ?? string.Empty,
                            Name = u.Name ?? string.Empty,
                            UnitTypeId = u.UnitTypeId,
                            UnitTypeName = u.UnitType?.Name ?? string.Empty,
                            BasePrice = new MoneyDto { Amount = u.BasePrice.Amount, Currency = u.BasePrice.Currency ?? "YER" },
                            Currency = u.BasePrice.Currency ?? "YER",
                            MaxCapacity = u.MaxCapacity,
                            PricingMethod = u.PricingMethod.ToString(),
                            IsAvailable = u.IsAvailable,
                            CustomFeatures = u.CustomFeatures ?? string.Empty
                        }).ToList()
                        : new List<YemenBooking.Application.DTOs.Units.UnitDetailsDto>(),
                    Amenities = property.Amenities?.Select(pa => new PropertyAmenityDto
                    {
                        Id = pa.PropertyTypeAmenity.AmenityId,
                        Name = pa.PropertyTypeAmenity.Amenity.Name ?? string.Empty,
                        Description = pa.PropertyTypeAmenity.Amenity.Description ?? string.Empty
                    }).ToList() ?? new List<PropertyAmenityDto>()
                };


                return ResultDto<PropertyDetailsDto>.Ok(dto, "تم استرجاع تفاصيل الكيان بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب تفاصيل الكيان: {PropertyId}", request.PropertyId);
                return ResultDto<PropertyDetailsDto>.Failure("حدث خطأ داخلي أثناء جلب التفاصيل");
            }
        }
        #endregion
    }
} 