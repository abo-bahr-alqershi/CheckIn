using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Units;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Entities; // added for SearchLog
using UnitEntity = YemenBooking.Core.Entities.Unit;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Queries.Units
{
    /// <summary>
    /// معالج استعلام البحث عن الوحدات المتقدم
    /// Handler for SearchUnitsQuery
    /// </summary>
    public class SearchUnitsQueryHandler : IRequestHandler<SearchUnitsQuery, PaginatedResult<UnitDto>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IAvailabilityService _availabilityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<SearchUnitsQueryHandler> _logger;
        private readonly ISearchLogRepository _searchLogRepository;

        public SearchUnitsQueryHandler(
            IUnitRepository unitRepository,
            IAvailabilityService availabilityService,
            ICurrentUserService currentUserService,
            ILogger<SearchUnitsQueryHandler> logger,
            ISearchLogRepository searchLogRepository)
        {
            _unitRepository = unitRepository;
            _availabilityService = availabilityService;
            _currentUserService = currentUserService;
            _logger = logger;
            _searchLogRepository = searchLogRepository;
        }

        public async Task<PaginatedResult<UnitDto>> Handle(SearchUnitsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء البحث عن الوحدات - موقع: {Location}, السعر من {MinPrice} إلى {MaxPrice}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.Location, request.MinPrice, request.MaxPrice, request.PageNumber, request.PageSize);

            var userRole = _currentUserService.Role;
            var userId = _currentUserService.UserId;

            var query = _unitRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.Property)
                .Include(u => u.UnitType)
                .Include(u => u.FieldValues).ThenInclude(fv => fv.UnitTypeField)
                .Include(u => u.Images.Where(i => !i.IsDeleted))
                .Where(u => !u.Property.IsDeleted);

            // تطبيق فلاتر الأمان على الكيان المرتبط
            query = userRole switch
            {
                "Admin" => query,
                "Owner" => query.Where(u => u.Property.IsApproved || u.Property.OwnerId == userId),
                _ => query.Where(u => u.Property.IsApproved)
            };

            // تطبيق الفلاتر المتقدمة
            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var term = request.Location.Trim().ToLower();
                query = query.Where(u => u.Property.City.ToLower().Contains(term) || u.Property.Address.ToLower().Contains(term));
            }
            if (request.PropertyId.HasValue)
                query = query.Where(u => u.PropertyId == request.PropertyId.Value);
            if (request.UnitTypeId.HasValue)
                query = query.Where(u => u.UnitTypeId == request.UnitTypeId.Value);
            if (request.MinPrice.HasValue)
                query = query.Where(u => u.BasePrice.Amount >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(u => u.BasePrice.Amount <= request.MaxPrice.Value);
            if (request.IsAvailable.HasValue)
                query = query.Where(u => u.IsAvailable == request.IsAvailable.Value);
            // فلترة بالاسم أو الرقم
            if (!string.IsNullOrWhiteSpace(request.NameContains))
            {
                var term = request.NameContains.Trim().ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(term) || u.Property.Name.ToLower().Contains(term));
            }
            // فلترة بطريقة التسعير
            if (!string.IsNullOrWhiteSpace(request.PricingMethod))
            {
                if (Enum.TryParse<PricingMethod>(request.PricingMethod.Trim(), true, out var pricingMethodEnum))
                {
                    query = query.Where(u => u.PricingMethod == pricingMethodEnum);
                }
                else
                {
                    query = query.Where(u => false);
                }
            }
            if (request.HasActiveBookings.HasValue)
                query = request.HasActiveBookings.Value
                    ? query.Where(u => u.BookingCount > 0)
                    : query.Where(u => u.BookingCount == 0);

            // فلترة الحقول الديناميكية
            if (request.DynamicFieldFilters != null && request.DynamicFieldFilters.Any())
            {
                foreach (var filter in request.DynamicFieldFilters)
                {
                    var searchValue = filter.FieldValue.Trim().ToLower();
                    query = query.Where(u => u.FieldValues.Any(fv =>
                        fv.UnitTypeFieldId == filter.FieldId && fv.FieldValue.ToLower().Contains(searchValue)));
                }
            }

            // جلب النتائج إلى الذاكرة لتطبيق فلتر التوفر والمسافة
            var unitsList = await query.ToListAsync(cancellationToken);
            var distancesDict = new Dictionary<Guid, double>();

            // تطبيق فلتر التوفر بالوقت
            if (request.CheckInDate.HasValue && request.CheckOutDate.HasValue)
            {
                var checkIn = request.CheckInDate.Value;
                var checkOut = request.CheckOutDate.Value;
                var guestCount = request.NumberOfGuests ?? 1;
                var availableUnits = new List<UnitEntity>();
                foreach (var unit in unitsList)
                {
                    var isAvailable = await _availabilityService.CheckAvailabilityAsync(unit.Id, checkIn, checkOut);
                    if (isAvailable)
                        availableUnits.Add(unit);
                }
                unitsList = availableUnits;
            }

            // تطبيق فلتر الموقع الجغرافي والمسافة
            if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
            {
                static double ToRadians(double deg) => deg * (Math.PI / 180);
                double lat1 = request.Latitude.Value, lon1 = request.Longitude.Value;
                var withDistance = unitsList.Select(u =>
                {
                    var lat2 = (double)u.Property.Latitude;
                    var lon2 = (double)u.Property.Longitude;
                    var dLat = ToRadians(lat2 - lat1);
                    var dLon = ToRadians(lon2 - lon1);
                    var a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                            Math.Sin(dLon/2) * Math.Sin(dLon/2);
                    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    var distanceKm = 6371 * c;
                    return new { Unit = u, DistanceKm = distanceKm };
                })
                .Where(x => x.DistanceKm <= request.RadiusKm.Value)
                .OrderBy(x => x.DistanceKm)
                .ToList();
                unitsList = withDistance.Select(x => x.Unit).ToList();
                distancesDict = withDistance.ToDictionary(x => x.Unit.Id, x => x.DistanceKm);
            }

            // تطبيق الترتيب
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var key = request.SortBy.Trim().ToLower();
                unitsList = key switch
                {
                    "popularity" => unitsList.OrderByDescending(u => u.BookingCount).ToList(),
                    "price_asc" => unitsList.OrderBy(u => u.BasePrice.Amount).ToList(),
                    "price_desc" => unitsList.OrderByDescending(u => u.BasePrice.Amount).ToList(),
                    "name_asc" => unitsList.OrderBy(u => u.Name).ToList(),
                    "name_desc" => unitsList.OrderByDescending(u => u.Name).ToList(),
                    _ => unitsList.OrderByDescending(u => u.CreatedAt).ToList(),
                };
            }

            // تنفيذ الصفحات
            var totalCount = unitsList.Count;
            var pagedList = unitsList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                
                .ToList();

            // تحويل إلى DTO
            var dtos = pagedList.Select(u => new UnitDto
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
                Images = u.Images.Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        IsMain = i.IsMainImage,
                        DisplayOrder = i.DisplayOrder,
                        Category = i.Category,
                        Type = i.Type,
                        SizeBytes = i.SizeBytes
                    }).ToList(),
                FieldValues = u.FieldValues.Select(fv => new UnitFieldValueDto
                {
                    ValueId = fv.Id,
                    UnitId = fv.UnitId,
                    FieldId = fv.UnitTypeFieldId,
                    FieldName = fv.UnitTypeField?.FieldName ?? string.Empty,
                    DisplayName = fv.UnitTypeField?.DisplayName ?? string.Empty,
                    FieldType = fv.UnitTypeField?.FieldTypeId ?? string.Empty,
                    FieldValue = fv.FieldValue,
                    IsPrimaryFilter = fv.UnitTypeField?.IsPrimaryFilter ?? false,
                    Field = fv.UnitTypeField == null ? null : new UnitTypeFieldDto
                    {
                        FieldId = fv.UnitTypeField.Id.ToString(),
                        UnitTypeId = fv.UnitTypeField.UnitTypeId.ToString(),
                        FieldTypeId = fv.UnitTypeField.FieldTypeId.ToString(),
                        FieldName = fv.UnitTypeField.FieldName,
                        DisplayName = fv.UnitTypeField.DisplayName,
                        Description = fv.UnitTypeField.Description,
                        FieldOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(fv.UnitTypeField.FieldOptions) ?? new Dictionary<string, object>(),
                        ValidationRules = JsonSerializer.Deserialize<Dictionary<string, object>>(fv.UnitTypeField.ValidationRules) ?? new Dictionary<string, object>(),
                        IsRequired = fv.UnitTypeField.IsRequired,
                        IsSearchable = fv.UnitTypeField.IsSearchable,
                        IsPublic = fv.UnitTypeField.IsPublic,
                        SortOrder = fv.UnitTypeField.SortOrder,
                        Category = fv.UnitTypeField.Category,
                        GroupId = fv.UnitTypeField.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty,
                        IsForUnits = fv.UnitTypeField.IsForUnits,
                        ShowInCards = fv.UnitTypeField.ShowInCards,
                        IsPrimaryFilter = fv.UnitTypeField.IsPrimaryFilter,
                        Priority = fv.UnitTypeField.Priority
                    },
                    CreatedAt = fv.CreatedAt,
                    UpdatedAt = fv.UpdatedAt
                }).ToList(),
                DistanceKm = distancesDict.TryGetValue(u.Id, out var d) ? d : (double?)null
            }).ToList();

            _logger.LogInformation("تم جلب {Count} وحدة من إجمالي {TotalCount} بعد البحث", dtos.Count, totalCount);

            // سجل عملية البحث
            await _searchLogRepository.AddAsync(new SearchLog
            {
                UserId = _currentUserService.UserId,
                SearchType = "Unit",
                CriteriaJson = JsonSerializer.Serialize(request),
                ResultCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            }, cancellationToken);

            return new PaginatedResult<UnitDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}