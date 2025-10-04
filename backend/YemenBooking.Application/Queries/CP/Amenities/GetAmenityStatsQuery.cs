using MediatR;
using YemenBooking.Application.DTOs.Amenities;

namespace YemenBooking.Application.Queries.Amenities
{
    /// <summary>
    /// Query to get amenity statistics
    /// </summary>
    public class GetAmenityStatsQuery : IRequest<AmenityStatsDto>
    {
    }
}

