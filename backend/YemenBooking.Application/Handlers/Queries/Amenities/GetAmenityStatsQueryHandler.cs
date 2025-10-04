using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.DTOs.Amenities;
using YemenBooking.Application.Queries.Amenities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Amenities
{
    public class GetAmenityStatsQueryHandler : IRequestHandler<GetAmenityStatsQuery, AmenityStatsDto>
    {
        private readonly IAmenityRepository _amenityRepository;

        public GetAmenityStatsQueryHandler(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<AmenityStatsDto> Handle(GetAmenityStatsQuery request, CancellationToken cancellationToken)
        {
            var amenities = (await _amenityRepository.GetAllAmenitiesAsync(cancellationToken)).ToList();
            var allPa = (await _amenityRepository.GetAllPropertyAmenitiesAsync(cancellationToken)).ToList();

            var stats = new AmenityStatsDto
            {
                TotalAmenities = amenities.Count,
                ActiveAmenities = amenities.Count(a => a.IsActive),
                TotalAssignments = allPa.Count,
                TotalRevenue = allPa.Sum(pa => (decimal)(pa.ExtraCost?.Amount ?? 0)),
            };

            stats.PopularAmenities = allPa
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity.Name)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.RevenueByAmenity = allPa
                .GroupBy(pa => pa.PropertyTypeAmenity.Amenity.Name)
                .ToDictionary(g => g.Key, g => g.Sum(x => (decimal)(x.ExtraCost?.Amount ?? 0)));

            return stats;
        }
    }
}

