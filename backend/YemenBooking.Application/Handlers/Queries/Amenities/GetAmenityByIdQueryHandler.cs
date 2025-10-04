using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.DTOs.Amenities;
using YemenBooking.Application.Queries.Amenities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Amenities
{
    public class GetAmenityByIdQueryHandler : IRequestHandler<GetAmenityByIdQuery, AmenityDto>
    {
        private readonly IAmenityRepository _amenityRepository;

        public GetAmenityByIdQueryHandler(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<AmenityDto> Handle(GetAmenityByIdQuery request, CancellationToken cancellationToken)
        {
            var a = await _amenityRepository.GetAmenityByIdAsync(request.AmenityId, cancellationToken);
            if (a == null) return new AmenityDto();
            return new AmenityDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Icon = a.Icon,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            };
        }
    }
}

