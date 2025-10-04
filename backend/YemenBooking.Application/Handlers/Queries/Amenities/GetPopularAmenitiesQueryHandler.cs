using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.DTOs.Amenities;
using YemenBooking.Application.Queries.Amenities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Amenities
{
    public class GetPopularAmenitiesQueryHandler : IRequestHandler<GetPopularAmenitiesQuery, List<AmenityDto>>
    {
        private readonly IAmenityRepository _amenityRepository;

        public GetPopularAmenitiesQueryHandler(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<List<AmenityDto>> Handle(GetPopularAmenitiesQuery request, CancellationToken cancellationToken)
        {
            var allPa = (await _amenityRepository.GetAllPropertyAmenitiesAsync(cancellationToken)).ToList();
            var grouped = allPa
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity)
                .OrderByDescending(g => g.Count())
                .Take(request.Limit)
                .Select(g => new AmenityDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Description = g.Key.Description,
                    Icon = g.Key.Icon,
                    IsActive = g.Key.IsActive,
                    CreatedAt = g.Key.CreatedAt,
                    UpdatedAt = g.Key.UpdatedAt
                })
                .ToList();

            return grouped;
        }
    }
}

