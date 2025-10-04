using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام أفضل الكيانات أداءً
    /// </summary>
    public class GetTopPerformingPropertiesQueryHandler : IRequestHandler<GetTopPerformingPropertiesQuery, IEnumerable<PropertyDto>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetTopPerformingPropertiesQueryHandler> _logger;

        public GetTopPerformingPropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            ILogger<GetTopPerformingPropertiesQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<IEnumerable<PropertyDto>> Handle(GetTopPerformingPropertiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Top Performing Properties Query Count {Count}", request.Count);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null || _currentUserService.Role != "Admin")
                throw new UnauthorizedException("يجب أن تكون مسؤولًا للوصول إلى أفضل الكيانات أداءً");

            var query = _propertyRepository.GetQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Owner)
                .Include(p => p.PropertyType)
                .OrderByDescending(p => p.BookingCount)
                .Take(request.Count);

            var properties = await query.ToListAsync(cancellationToken);

            return properties.Select(p => new PropertyDto
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
                TypeName = p.PropertyType.Name
            });
        }
    }
} 