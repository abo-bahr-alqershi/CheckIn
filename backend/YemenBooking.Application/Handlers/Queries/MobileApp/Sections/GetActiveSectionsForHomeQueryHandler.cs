using MediatR;
using YemenBooking.Application.DTOs.Sections;
using YemenBooking.Application.Queries.MobileApp.Sections;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Sections
{
    public class GetActiveSectionsForHomeQueryHandler : IRequestHandler<GetActiveSectionsForHomeQuery, IEnumerable<SectionDto>>
    {
        private readonly ISectionRepository _repository;

        public GetActiveSectionsForHomeQueryHandler(ISectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SectionDto>> Handle(GetActiveSectionsForHomeQuery request, CancellationToken cancellationToken)
        {
            var items = await _repository.FindAsync(s => s.IsActive, cancellationToken);
            return items
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SectionDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    ContentType = s.ContentType,
                    DisplayStyle = s.DisplayStyle,
                    Name = s.Name,
                    Title = s.Title,
                    Subtitle = s.Subtitle,
                    Description = s.Description,
                    ShortDescription = s.ShortDescription,
                    DisplayOrder = s.DisplayOrder,
                    Target = s.Target,
                    IsActive = s.IsActive,
                    ColumnsCount = s.ColumnsCount,
                    ItemsToShow = s.ItemsToShow,
                    Icon = s.Icon,
                    ColorTheme = s.ColorTheme,
                    BackgroundImage = s.BackgroundImage,
                    FilterCriteria = s.FilterCriteria,
                    SortCriteria = s.SortCriteria,
                    CityName = s.CityName,
                    PropertyTypeId = s.PropertyTypeId,
                    UnitTypeId = s.UnitTypeId,
                    MinPrice = s.MinPrice,
                    MaxPrice = s.MaxPrice,
                    MinRating = s.MinRating,
                    IsVisibleToGuests = s.IsVisibleToGuests,
                    IsVisibleToRegistered = s.IsVisibleToRegistered,
                    RequiresPermission = s.RequiresPermission,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Metadata = s.Metadata
                });
        }
    }
}

