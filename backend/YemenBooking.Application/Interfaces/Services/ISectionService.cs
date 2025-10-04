using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Sections;

namespace YemenBooking.Application.Interfaces.Services
{
    public interface ISectionService
    {
        Task<ResultDto<SectionDto>> CreateAsync(CreateSectionDto dto, CancellationToken cancellationToken = default);
        Task<ResultDto<SectionDto>> UpdateAsync(UpdateSectionDto dto, CancellationToken cancellationToken = default);
        Task<ResultDto> ToggleStatusAsync(Guid sectionId, bool isActive, CancellationToken cancellationToken = default);
    }
}

