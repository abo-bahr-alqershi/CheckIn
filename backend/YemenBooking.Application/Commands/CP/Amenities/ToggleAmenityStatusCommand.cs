using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Amenities
{
    /// <summary>
    /// Command to toggle amenity status
    /// </summary>
    public class ToggleAmenityStatusCommand : IRequest<ResultDto<bool>>
    {
        public Guid AmenityId { get; set; }
    }
}

