using MediatR;
using YemenBooking.Application.DTOs.Amenities;

namespace YemenBooking.Application.Queries.Amenities
{
    /// <summary>
    /// Query to get popular amenities
    /// </summary>
    public class GetPopularAmenitiesQuery : IRequest<List<AmenityDto>>
    {
        public int Limit { get; set; } = 10;
    }
}

