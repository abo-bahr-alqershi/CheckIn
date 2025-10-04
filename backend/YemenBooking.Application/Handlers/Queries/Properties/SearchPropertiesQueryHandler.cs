using System;
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
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام البحث عن الكيانات المتقدم
    /// Handler for SearchPropertiesQuery
    /// </summary>
    public class SearchPropertiesQueryHandler : IRequestHandler<SearchPropertiesQuery, PaginatedResult<PropertyDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAvailabilityService _availabilityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<SearchPropertiesQueryHandler> _logger;
        #endregion

        #region Constructor
        public SearchPropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            IAvailabilityService availabilityService,
            ICurrentUserService currentUserService,
            ILogger<SearchPropertiesQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _availabilityService = availabilityService;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<PaginatedResult<PropertyDto>> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء البحث عن الكيانات - موقع: {Location}, السعر من {MinPrice} إلى {MaxPrice}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.Location, request.MinPrice, request.MaxPrice, request.PageNumber, request.PageSize);

            // بناء الاستعلام الأساسي
            var userRole = _currentUserService.Role;
            var userId = _currentUserService.UserId;

            var query = _propertyRepository.GetQueryable()
                .AsNoTracking()
                .Include(p => p.PropertyType)
                .Include(p => p.Owner)
                .Where(p => !p.IsDeleted);

            // تطبيق فلاتر الأمان
            query = userRole switch
            {
                "Admin" => query,
                "Owner" => query.Where(p => p.IsApproved || p.OwnerId == userId),
                _ => query.Where(p => p.IsApproved)
            };

            // تطبيق الفلاتر المتقدمة
            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var term = request.Location.Trim().ToLower();
                query = query.Where(p => p.City.ToLower().Contains(term) || p.Address.ToLower().Contains(term));
            }
            if (request.PropertyTypeId.HasValue)
                query = query.Where(p => p.TypeId == request.PropertyTypeId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Units.Any(u => u.BasePrice.Amount >= request.MinPrice.Value));
            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Units.Any(u => u.BasePrice.Amount <= request.MaxPrice.Value));

            if (request.AmenityIds != null && request.AmenityIds.Any())
                query = query.Where(p => request.AmenityIds.All(id => p.Amenities.Any(a => a.PropertyTypeAmenity.AmenityId == id && a.IsAvailable)));

            if (request.StarRatings != null && request.StarRatings.Any())
                query = query.Where(p => request.StarRatings.Contains(p.StarRating));

            if (request.MinAverageRating.HasValue)
                query = query.Where(p => p.Reviews.Any() &&
                    p.Reviews.Average(r => (r.Cleanliness + r.Service + r.Location + r.Value) / 4.0) >= request.MinAverageRating.Value);

            if (request.IsApproved.HasValue)
                query = query.Where(p => p.IsApproved == request.IsApproved.Value);

            if (request.HasActiveBookings.HasValue)
                query = request.HasActiveBookings.Value
                    ? query.Where(p => p.BookingCount > 0)
                    : query.Where(p => p.BookingCount == 0);
            

            // جلب النتائج إلى الذاكرة لتطبيق فلتر التوفر
            var propertiesList = await query.ToListAsync(cancellationToken);
            // Dictionary to hold computed distances for each property
            var distancesDict = new System.Collections.Generic.Dictionary<Guid, double>();
            if (request.CheckInDate.HasValue && request.CheckOutDate.HasValue)
            {
                var checkIn = request.CheckInDate.Value;
                var checkOut = request.CheckOutDate.Value;
                var guestCount = request.NumberOfGuests ?? 1;
                var availableProps = new System.Collections.Generic.List<Property>();
                foreach (var prop in propertiesList)
                {
                    var units = await _availabilityService.GetAvailableUnitsInPropertyAsync(
                        prop.Id, checkIn, checkOut, guestCount, cancellationToken);
                    if (units.Any())
                        availableProps.Add(prop);
                }
                propertiesList = availableProps;
            }

            // تطبيق فلتر الموقع الجغرافي والمسافة
            if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
            {
                // Haversine formula for distance calculation
                static double ToRadians(double deg) => deg * (Math.PI / 180);
                double lat1 = request.Latitude.Value, lon1 = request.Longitude.Value;
                var withDistance = propertiesList.Select(p =>
                {
                    var lat2 = (double)p.Latitude;
                    var lon2 = (double)p.Longitude;
                    var dLat = ToRadians(lat2 - lat1);
                    var dLon = ToRadians(lon2 - lon1);
                    var a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                            Math.Sin(dLon/2) * Math.Sin(dLon/2);
                    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    var distanceKm = 6371 * c; // Earth radius in km
                    return new { Prop = p, DistanceKm = distanceKm };
                })
                .Where(x => x.DistanceKm <= request.RadiusKm.Value)
                .OrderBy(x => x.DistanceKm)
                .ToList();
                propertiesList = withDistance.Select(x => x.Prop).ToList();
                distancesDict = withDistance.ToDictionary(x => x.Prop.Id, x => x.DistanceKm);
            }

            // تطبيق الترتيب
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var key = request.SortBy.Trim().ToLower();
                propertiesList = key switch
                {
                    "rating" => propertiesList.OrderByDescending(p =>
                        p.Reviews.Any()
                            ? p.Reviews.Average(r => (r.Cleanliness + r.Service + r.Location + r.Value) / 4.0)
                            : 0).ToList(),
                    "popularity" => propertiesList.OrderByDescending(p => p.BookingCount).ToList(),
                    "name_asc" => propertiesList.OrderBy(p => p.Name).ToList(),
                    "name_desc" => propertiesList.OrderByDescending(p => p.Name).ToList(),
                    _ => propertiesList.OrderByDescending(p => p.CreatedAt).ToList(),
                };
            }

            // تنفيذ الصفحات
            var totalCount = propertiesList.Count;
            var pagedList = propertiesList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // تحويل إلى DTO
            var dtos = pagedList.Select(p => new PropertyDto
            {
                Id = p.Id,
                OwnerId = p.OwnerId,
                TypeId = p.TypeId,
                Name = p.Name,
                Address = p.Address,
                City = p.City,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                StarRating = p.StarRating,
                Description = p.Description,
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt,
                OwnerName = p.Owner.Name,
                TypeName = p.PropertyType.Name,
                // تعيين المسافة إن وجدت
                DistanceKm = distancesDict.TryGetValue(p.Id, out var d) ? d : (double?)null
            }).ToList();

            _logger.LogInformation("تم جلب {Count} كيان من إجمالي {TotalCount} بعد البحث المتقدم", dtos.Count, totalCount);
            return new PaginatedResult<PropertyDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        #endregion
    }
}