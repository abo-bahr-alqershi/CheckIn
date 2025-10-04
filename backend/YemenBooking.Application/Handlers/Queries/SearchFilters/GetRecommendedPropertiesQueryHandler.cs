using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.SearchFilters;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System.Collections.Generic;

namespace YemenBooking.Application.Handlers.Queries.SearchFilters
{
    /// <summary>
    /// معالج استعلام الحصول على الكيانات المقترحة للمستخدم
    /// Query handler for GetRecommendedPropertiesQuery
    /// </summary>
    public class GetRecommendedPropertiesQueryHandler : IRequestHandler<GetRecommendedPropertiesQuery, PaginatedResult<PropertyDto>>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILogger<GetRecommendedPropertiesQueryHandler> _logger;

        public GetRecommendedPropertiesQueryHandler(
            IRecommendationService recommendationService,
            IPropertyRepository propertyRepository,
            ILogger<GetRecommendedPropertiesQueryHandler> logger)
        {
            _recommendationService = recommendationService;
            _propertyRepository = propertyRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<PropertyDto>> Handle(GetRecommendedPropertiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الكيانات المقترحة للمستخدم: {UserId}", request.UserId);

            var recommended = await _recommendationService.GetRecommendedPropertiesAsync(request.UserId, request.Count, cancellationToken);
            var ids = recommended.Select(p => p.Id).ToList();

            var properties = await _propertyRepository.GetQueryable()
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.PropertyType)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var dtos = properties
                .Select(p => new PropertyDto
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
                    OwnerName = p.Owner?.Name ?? string.Empty,
                    TypeName = p.PropertyType?.Name ?? string.Empty
                }).ToList();

            return new PaginatedResult<PropertyDto>
            {
                Items = dtos,
                PageNumber = 1,
                PageSize = request.Count,
                TotalCount = dtos.Count
            };
        }
    }
} 