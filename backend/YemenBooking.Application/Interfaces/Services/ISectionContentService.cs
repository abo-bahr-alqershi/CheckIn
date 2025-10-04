using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Sections;
using YemenBooking.Core.Entities;

namespace YemenBooking.Application.Interfaces.Services
{
    public interface ISectionContentService
    {
        Task<ResultDto> AssignPropertyItemsAsync(Guid sectionId, IEnumerable<PropertyInSection> items, CancellationToken cancellationToken = default);
        Task<ResultDto> AssignUnitItemsAsync(Guid sectionId, IEnumerable<UnitInSection> items, CancellationToken cancellationToken = default);
    }
}

