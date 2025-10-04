using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.PropertySearch;
using YemenBooking.Application.Queries.MobileApp.Properties;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Indexing.Models;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Properties
{
    /// <summary>
    /// معالج البحث عن العقارات باستخدام فهرس LiteDB
    /// </summary>
    public class SearchPropertiesQueryHandler : IRequestHandler<SearchPropertiesQuery, ResultDto<SearchPropertiesResponse>>
    {
        private readonly IIndexingService _indexService;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IAvailabilityService _availabilityService;
        private readonly IPricingService _pricingService;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrencyExchangeRepository _currencyExchangeRepository;
        private readonly ILogger<SearchPropertiesQueryHandler> _logger;

        public SearchPropertiesQueryHandler(
            IIndexingService indexService,
            IPropertyRepository propertyRepository,
            IUnitRepository unitRepository,
            IAvailabilityService availabilityService,
            IPricingService pricingService,
            IReviewRepository reviewRepository,
            ICurrencyExchangeRepository currencyExchangeRepository,
            ILogger<SearchPropertiesQueryHandler> logger)
        {
            _indexService = indexService;
            _propertyRepository = propertyRepository;
            _unitRepository = unitRepository;
            _availabilityService = availabilityService;
            _pricingService = pricingService;
            _reviewRepository = reviewRepository;
            _currencyExchangeRepository = currencyExchangeRepository;
            _logger = logger;
        }

        public async Task<ResultDto<SearchPropertiesResponse>> Handle(
            SearchPropertiesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء البحث عن العقارات");

                // التحقق من صحة الطلب
                var validationResult = ValidateRequest(request);
                if (!validationResult.IsSuccess)
                    return validationResult;

                // بناء طلب البحث للفهرس
                var searchRequest = await BuildSearchRequest(request, cancellationToken);

                // البحث في الفهرس
                var searchResult = await _indexService.SearchAsync(searchRequest, cancellationToken);

                // تحويل النتائج إلى DTOs
                var propertyDtos = await ConvertToPropertyDtos(
                    searchResult.Properties,
                    request,
                    cancellationToken);
            _logger.LogInformation(" ####################### {Count}", propertyDtos.Count);
            _logger.LogInformation(" ####################### {Count}", propertyDtos.Count);
            _logger.LogInformation(" ####################### {Count}", propertyDtos.Count);
            _logger.LogInformation(" ####################### {Count}", propertyDtos.Count);
            _logger.LogInformation(" ####################### {Count}", propertyDtos.Count);
            _logger.LogInformation(" ####################### {Count}", propertyDtos.Count);

                // بناء الاستجابة
                var response = new SearchPropertiesResponse
                {
                    Properties = propertyDtos,
                    TotalCount = searchResult.TotalCount,
                    CurrentPage = searchResult.PageNumber,
                    TotalPages = searchResult.TotalPages
                };

                _logger.LogInformation("تم العثور على {Count} عقار", propertyDtos.Count);

                return ResultDto<SearchPropertiesResponse>.Ok(response, "تم البحث بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء البحث عن العقارات");
                return ResultDto<SearchPropertiesResponse>.Failed(
                    $"حدث خطأ أثناء البحث: {ex.Message}",
                    "SEARCH_ERROR");
            }
        }

        private ResultDto<SearchPropertiesResponse> ValidateRequest(SearchPropertiesQuery request)
        {
            if (request.PageNumber < 1)
            {
                return ResultDto<SearchPropertiesResponse>.Failed(
                    "رقم الصفحة يجب أن يكون أكبر من صفر",
                    "INVALID_PAGE_NUMBER");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return ResultDto<SearchPropertiesResponse>.Failed(
                    "حجم الصفحة يجب أن يكون بين 1 و 100",
                    "INVALID_PAGE_SIZE");
            }

            if (request.CheckIn.HasValue && request.CheckOut.HasValue)
            {
                if (request.CheckIn >= request.CheckOut)
                {
                    return ResultDto<SearchPropertiesResponse>.Failed(
                        "تاريخ الوصول يجب أن يكون قبل تاريخ المغادرة",
                        "INVALID_DATE_RANGE");
                }
            }

            if (request.MinPrice.HasValue && request.MaxPrice.HasValue)
            {
                if (request.MinPrice > request.MaxPrice)
                {
                    return ResultDto<SearchPropertiesResponse>.Failed(
                        "السعر الأدنى يجب أن يكون أقل من السعر الأقصى",
                        "INVALID_PRICE_RANGE");
                }
            }

            return ResultDto<SearchPropertiesResponse>.Ok(null);
        }

        private async Task<PropertySearchRequest> BuildSearchRequest(
            SearchPropertiesQuery query,
            CancellationToken cancellationToken)
        {
            var request = new PropertySearchRequest
            {
                SearchText = query.SearchTerm,
                City = query.City,
                MinPrice = query.MinPrice,
                MaxPrice = query.MaxPrice,
                MinRating = query.MinStarRating,
                CheckIn = query.CheckIn,
                CheckOut = query.CheckOut,
                GuestsCount = query.GuestsCount,
                Latitude = query.Latitude.HasValue ? (double?)query.Latitude.Value : null,
                Longitude = query.Longitude.HasValue ? (double?)query.Longitude.Value : null,
                RadiusKm = query.RadiusKm,
                SortBy = query.SortBy,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            // تحويل معرف نوع العقار إلى اسم
            if (query.PropertyTypeId.HasValue)
            {
                var propertyType = await _propertyRepository
                    .GetPropertyTypeByIdAsync(query.PropertyTypeId.Value, cancellationToken);
                request.PropertyType = propertyType?.Name;
            }

            // تحويل معرفات المرافق
            if (query.RequiredAmenities?.Any() == true)
            {
                request.RequiredAmenityIds = query.RequiredAmenities
                    .Select(id => id.ToString())
                    .ToList();
            }

            // الحقول الديناميكية
            if (query.DynamicFieldFilters?.Any() == true)
            {
                request.DynamicFieldFilters = query.DynamicFieldFilters
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? "");
            }

            return request;
        }

        private async Task<List<PropertySearchResultDto>> ConvertToPropertyDtos(
            List<PropertySearchItem> items,
            SearchPropertiesQuery query,
            CancellationToken cancellationToken)
        {
            var dtos = new List<PropertySearchResultDto>();

            foreach (var item in items)
            {
                try
                {
                    var propertyId = Guid.Parse(item.Id);

                    // جلب تفاصيل العقار من SQL Server
                    var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
                    if (property == null) continue;

                    var dto = new PropertySearchResultDto
                    {
                        Id = propertyId,
                        Name = property.Name,
                        Description = property.ShortDescription ?? property.Description,
                        PropertyType = property.PropertyType?.Name ?? "",
                        Address = property.Address,
                        City = property.City,
                        MinPrice = item.MinPrice,
                        DiscountedPrice = item.MinPrice,
                        Currency = item.Currency,
                        StarRating = item.StarRating,
                        AverageRating = item.AverageRating,
                        ReviewsCount = property.Reviews?.Count ?? 0,
                        MainImageUrl = item.ImageUrls?.FirstOrDefault() ?? "",
                        ImageUrls = item.ImageUrls ?? new List<string>(),
                        IsAvailable = true,
                        IsFavorite = false,
                        IsFeatured = property.IsFeatured,
                        Latitude = property.Latitude,
                        Longitude = property.Longitude,
                        MaxCapacity = item.MaxCapacity,
                        AvailableUnitsCount = item.UnitsCount,
                        DynamicFieldValues = item.DynamicFields != null
                            ? item.DynamicFields.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value)
                            : new Dictionary<string, object>(),
                        LastUpdated = property.CreatedAt
                    };

                    // جمع المرافق
                    dto.MainAmenities = property.Amenities?
                        .Where(a => a.IsAvailable && a.PropertyTypeAmenity?.Amenity != null)
                        .Select(a => a.PropertyTypeAmenity.Amenity.Name)
                        .Distinct()
                        .ToList() ?? new List<string>();

                    // حساب الإتاحة والسعر إذا كانت هناك تواريخ
                    if (query.CheckIn.HasValue && query.CheckOut.HasValue)
                    {
                        await CalculateAvailabilityAndPricing(dto, query, cancellationToken);
                    }
                    else if (query.UnitTypeId.HasValue || query.GuestsCount.HasValue)
                    {
                        await SelectBestUnit(dto, query, cancellationToken);
                    }

                    // تحويل العملة إذا طُلب
                    if (!string.IsNullOrWhiteSpace(query.PreferredCurrency))
                    {
                        await ConvertCurrency(dto, query.PreferredCurrency, cancellationToken);
                    }

                    // حساب المسافة إذا كان بحث جغرافي
                    if (query.Latitude.HasValue && query.Longitude.HasValue)
                    {
                        dto.DistanceKm = CalculateDistance(
                            (double)query.Latitude.Value,
                            (double)query.Longitude.Value,
                            (double)dto.Latitude,
                            (double)dto.Longitude);
                    }

                    dtos.Add(dto);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "خطأ في تحويل العقار {PropertyId}", item.Id);
                }
            }

            return dtos;
        }

        private async Task CalculateAvailabilityAndPricing(
            PropertySearchResultDto dto,
            SearchPropertiesQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var checkIn = query.CheckIn!.Value;
                var checkOut = query.CheckOut!.Value;
                var guestCount = query.GuestsCount ?? 1;

                // الحصول على الوحدات المتاحة
                var availableUnitIds = await _availabilityService
                    .GetAvailableUnitsInPropertyAsync(dto.Id, checkIn, checkOut, guestCount, cancellationToken);

                dto.AvailableUnitsCount = availableUnitIds.Count();
                dto.IsAvailable = dto.AvailableUnitsCount > 0;

                if (!availableUnitIds.Any()) return;

                // اختيار الوحدة المناسبة
                Guid selectedUnitId = Guid.Empty;

                if (query.UnitTypeId.HasValue)
                {
                    foreach (var unitId in availableUnitIds)
                    {
                        var unit = await _unitRepository.GetUnitByIdAsync(unitId, cancellationToken);
                        if (unit != null && unit.UnitTypeId == query.UnitTypeId.Value && unit.MaxCapacity >= guestCount)
                        {
                            selectedUnitId = unitId;
                            break;
                        }
                    }
                }

                if (selectedUnitId == Guid.Empty)
                {
                    selectedUnitId = availableUnitIds.FirstOrDefault();
                }

                if (selectedUnitId != Guid.Empty)
                {
                    dto.UnitId = selectedUnitId;
                    var unitEntity = await _unitRepository.GetUnitByIdAsync(selectedUnitId, cancellationToken);

                    if (unitEntity != null)
                    {
                        dto.UnitName = unitEntity.Name;
                        dto.MaxCapacity = unitEntity.MaxCapacity;

                        // حساب السعر
                        var totalPrice = await _pricingService.CalculatePriceAsync(
                            selectedUnitId, checkIn, checkOut);

                        var nights = Math.Max(1, (checkOut - checkIn).Days);
                        var perNight = totalPrice / nights;

                        dto.MinPrice = perNight;
                        dto.DiscountedPrice = perNight;
                        dto.Currency = unitEntity.BasePrice.Currency ?? dto.Currency;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "خطأ في حساب الإتاحة والتسعير للعقار {PropertyId}", dto.Id);
            }
        }

        private async Task SelectBestUnit(
            PropertySearchResultDto dto,
            SearchPropertiesQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var units = await _unitRepository.GetByPropertyIdAsync(dto.Id, cancellationToken);
                var guestCount = query.GuestsCount ?? 1;

                var filteredUnits = units
                    .Where(u => u.IsAvailable)
                    .Where(u => !query.UnitTypeId.HasValue || u.UnitTypeId == query.UnitTypeId.Value)
                    .Where(u => u.MaxCapacity >= guestCount)
                    .ToList();

                if (filteredUnits.Any())
                {
                    var chosenUnit = filteredUnits
                        .OrderBy(u => u.BasePrice.Amount)
                        .First();

                    dto.UnitId = chosenUnit.Id;
                    dto.UnitName = chosenUnit.Name;
                    dto.MinPrice = chosenUnit.BasePrice.Amount;
                    dto.DiscountedPrice = chosenUnit.BasePrice.Amount * (1 - chosenUnit.DiscountPercentage / 100);
                    dto.Currency = chosenUnit.BasePrice.Currency;
                    dto.MaxCapacity = chosenUnit.MaxCapacity;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "خطأ في اختيار وحدة للعقار {PropertyId}", dto.Id);
            }
        }

        private async Task ConvertCurrency(
            PropertySearchResultDto dto,
            string targetCurrency,
            CancellationToken cancellationToken)
        {
            try
            {
                var target = targetCurrency.ToUpperInvariant();
                if (string.Equals(dto.Currency, target, StringComparison.OrdinalIgnoreCase))
                    return;

                var rate = await _currencyExchangeRepository.GetLatestExchangeRateAsync(
                    dto.Currency, target, cancellationToken);

                if (rate != null && rate.Rate > 0)
                {
                    dto.MinPrice = Math.Round(dto.MinPrice * rate.Rate, 2);
                    dto.DiscountedPrice = Math.Round(dto.DiscountedPrice * rate.Rate, 2);
                    dto.Currency = target;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "خطأ في تحويل العملة للعقار {PropertyId}", dto.Id);
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;
    }
}