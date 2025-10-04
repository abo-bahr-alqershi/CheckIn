using MediatR;
using YemenBooking.Application.DTOs.Amenities;

namespace YemenBooking.Application.Queries.Amenities
{
    /// <summary>
    /// Query to get amenity by id
    /// </summary>
    public class GetAmenityByIdQuery : IRequest<AmenityDto>
    {
        public Guid AmenityId { get; set; }
    }
}

