using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Amenities;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Amenities
{
    /// <summary>
    /// معالج استعلام الحصول على جميع المرافق
    /// Query handler for GetAllAmenitiesQuery
    /// </summary>
    public class GetAllAmenitiesQueryHandler : IRequestHandler<GetAllAmenitiesQuery, PaginatedResult<AmenityDto>>
    {
        private readonly IAmenityRepository _amenityRepository;
        private readonly ILogger<GetAllAmenitiesQueryHandler> _logger;

        public GetAllAmenitiesQueryHandler(
            IAmenityRepository amenityRepository,
            ILogger<GetAllAmenitiesQueryHandler> logger)
        {
            _amenityRepository = amenityRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<AmenityDto>> Handle(GetAllAmenitiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام جميع المرافق - الصفحة: {PageNumber}, الحجم: {PageSize}", request.PageNumber, request.PageSize);

            // التحقق من صحة معاملات الصفحة
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            // جلب جميع المرافق
            var amenities = (await _amenityRepository.GetAllAmenitiesAsync(cancellationToken)).ToList();

            // تطبيق بحث نصي اختياري
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                amenities = amenities
                    .Where(a => a.Name.ToLower().Contains(term) || a.Description.ToLower().Contains(term))
                    .ToList();
            }

            // Filter by property type if provided
            if (request.PropertyTypeId.HasValue)
            {
                var pta = await _amenityRepository.GetAmenitiesByPropertyTypeAsync(request.PropertyTypeId.Value, cancellationToken);
                var amenityIdsForType = pta.Select(x => x.AmenityId).ToHashSet();
                amenities = amenities.Where(a => amenityIdsForType.Contains(a.Id)).ToList();
            }

            // Apply filters for related property and extra cost
            if (request.PropertyId.HasValue)
            {
                var propertyAmenities = (await _amenityRepository.GetAmenitiesByPropertyAsync(request.PropertyId.Value, cancellationToken)).ToList();
                var assignedIds = propertyAmenities.Select(pa => pa.PropertyTypeAmenity.Amenity.Id).ToHashSet();
                if (request.IsAssigned.HasValue)
                {
                    amenities = request.IsAssigned.Value
                        ? amenities.Where(a => assignedIds.Contains(a.Id)).ToList()
                        : amenities.Where(a => !assignedIds.Contains(a.Id)).ToList();
                }
                if (request.IsFree.HasValue)
                {
                    var freeIds = propertyAmenities.Where(pa => pa.ExtraCost.Amount == 0)
                        .Select(pa => pa.PropertyTypeAmenity.Amenity.Id).ToHashSet();
                    amenities = request.IsFree.Value
                        ? amenities.Where(a => freeIds.Contains(a.Id)).ToList()
                        : amenities.Where(a => !freeIds.Contains(a.Id)).ToList();
                }
            }
            else
            {
                if (request.IsAssigned.HasValue)
                {
                    var allPropertyAmenities = (await _amenityRepository.GetAllPropertyAmenitiesAsync(cancellationToken)).ToList();
                    var allAssignedIds = allPropertyAmenities.Select(pa => pa.PropertyTypeAmenity.Amenity.Id).Distinct().ToHashSet();
                    amenities = request.IsAssigned.Value
                        ? amenities.Where(a => allAssignedIds.Contains(a.Id)).ToList()
                        : amenities.Where(a => !allAssignedIds.Contains(a.Id)).ToList();
                }
                if (request.IsFree.HasValue)
                {
                    var allPropertyAmenities = (await _amenityRepository.GetAllPropertyAmenitiesAsync(cancellationToken)).ToList();
                    var allFreeIds = allPropertyAmenities.Where(pa => pa.ExtraCost.Amount == 0)
                        .Select(pa => pa.PropertyTypeAmenity.Amenity.Id).Distinct().ToHashSet();
                    amenities = request.IsFree.Value
                        ? amenities.Where(a => allFreeIds.Contains(a.Id)).ToList()
                        : amenities.Where(a => !allFreeIds.Contains(a.Id)).ToList();
                }
            }

            var totalCount = amenities.Count;

            // تطبيق الصفحات
            var items = amenities
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Icon = a.Icon,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToList();

            _logger.LogInformation("تم جلب {Count} مرفق من إجمالي {TotalCount}", items.Count, totalCount);
            return new PaginatedResult<AmenityDto>(items, pageNumber, pageSize, totalCount);
        }
    }
} 