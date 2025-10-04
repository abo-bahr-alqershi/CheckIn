using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.PropertySearch;
using YemenBooking.Application.Queries.MobileApp.Sections;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Sections
{
	public class GetSectionItemsQueryHandler : IRequestHandler<GetSectionItemsQuery, PaginatedResult<object>>
	{
    	private readonly ISectionRepository _sections;
		private readonly IPropertyRepository _properties;
		private readonly IUnitRepository _units;
		private readonly IPropertyImageRepository _images;

		public GetSectionItemsQueryHandler(
			ISectionRepository sections,
			IPropertyRepository properties,
			IUnitRepository units,
			IPropertyImageRepository images)
		{
			_sections = sections;
			_properties = properties;
			_units = units;
			_images = images;
		}

		public async Task<PaginatedResult<object>> Handle(GetSectionItemsQuery request, CancellationToken cancellationToken)
		{
			if (request.PageNumber <= 0) request.PageNumber = 1;
			if (request.PageSize <= 0) request.PageSize = 10;

			var section = await _sections.GetByIdAsync(request.SectionId, cancellationToken);
			if (section == null)
				return PaginatedResult<object>.Empty(request.PageNumber, request.PageSize);

            // Use rich tables instead of legacy SectionItems
            if (section.Target == SectionTarget.Properties)
            {
                var allItems = (await _sections.GetPropertyItemsAsync(request.SectionId, cancellationToken)).ToList();
                var total = allItems.Count;
                if (total == 0)
                    return PaginatedResult<object>.Empty(request.PageNumber, request.PageSize);

                var pagedItems = allItems
                    .OrderBy(i => i.DisplayOrder)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var resultObjects = new List<object>();
                foreach (var p in pagedItems)
                {
                    var imgs = (await _images.GetImagesByPropertyAsync(p.PropertyId, cancellationToken))
                        .OrderBy(i => i.DisplayOrder)
                        .ToList();
                    // Append additional images linked specifically to this PropertyInSection record
                    // using the new PropertyInSectionId linkage
                    var extra = imgs.Where(i => i.PropertyInSectionId == p.Id).ToList();
                    if (extra.Count > 0)
                    {
                        // merge but keep order; extra already part of imgs if repository returns by property only
                        // If repository does not include relation filter, ensure union distinct by Id
                        var map = imgs.ToDictionary(i => i.Id, i => i);
                        foreach (var e in extra)
                            map[e.Id] = e;
                        imgs = map.Values.OrderBy(i => i.DisplayOrder).ToList();
                    }
                    var mainImage = string.IsNullOrWhiteSpace(p.MainImage)
                        ? (imgs.FirstOrDefault(i => i.IsMainImage)?.Url ?? imgs.FirstOrDefault()?.Url)
                        : p.MainImage;
                    var mainImageId = imgs.FirstOrDefault(i => i.IsMainImage)?.Id;
                    var additional = imgs.Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        PropertyId = i.PropertyId,
                        UnitId = i.UnitId,
                        Name = i.Name,
                        Url = i.Url,
                        SizeBytes = i.SizeBytes,
                        Type = i.Type,
                        Category = i.Category,
                        Caption = i.Caption,
                        AltText = i.AltText,
                        Tags = i.Tags,
                        Sizes = i.Sizes,
                        IsMain = i.IsMainImage,
                        DisplayOrder = i.DisplayOrder,
                        UploadedAt = i.UploadedAt,
                        Status = i.Status,
                        AssociationType = i.UnitId.HasValue ? "Unit" : "Property"
                    }).ToList();

                    resultObjects.Add(new
                    {
                        Id = p.PropertyId,
                        PropertyInSectionId = p.Id,
                        Name = p.PropertyName,
                        Description = p.ShortDescription ?? string.Empty,
                        City = p.City,
                        Address = p.Address,
                        StarRating = p.StarRating,
                        AverageRating = p.AverageRating,
                        ReviewCount = p.ReviewsCount,
                        MinPrice = p.BasePrice,
                        Currency = p.Currency,
                        MainImageUrl = mainImage,
                        MainImageId = mainImageId,
                        ImageUrls = additional.Select(a => a.Url).ToList(),
                        AdditionalImages = additional,
                        Amenities = new List<string>(),
                        PropertyType = p.PropertyType,
                        DistanceKm = (decimal?)null,
                        IsAvailable = true,
                        AvailableUnitsCount = 0,
                        MaxCapacity = 0,
                        IsFeatured = p.IsFeatured,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                return PaginatedResult<object>.Create(resultObjects, request.PageNumber, request.PageSize, total);
            }
            else
            {
                var allItems = (await _sections.GetUnitItemsAsync(request.SectionId, cancellationToken)).ToList();
                var total = allItems.Count;
                if (total == 0)
                    return PaginatedResult<object>.Empty(request.PageNumber, request.PageSize);

                var pagedItems = allItems
                    .OrderBy(i => i.DisplayOrder)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var resultItems = new List<object>();
                foreach (var u in pagedItems)
                {
                    var imgs = (await _images.GetImagesByUnitAsync(u.UnitId, cancellationToken))
                        .OrderBy(i => i.DisplayOrder)
                        .ToList();
                    // Append additional images linked to UnitInSection record
                    var extra = imgs.Where(i => i.UnitInSectionId == u.Id).ToList();
                    if (extra.Count > 0)
                    {
                        var map = imgs.ToDictionary(i => i.Id, i => i);
                        foreach (var e in extra)
                            map[e.Id] = e;
                        imgs = map.Values.OrderBy(i => i.DisplayOrder).ToList();
                    }
                    var mainImage = string.IsNullOrWhiteSpace(u.MainImage)
                        ? (imgs.FirstOrDefault(i => i.IsMainImage)?.Url ?? imgs.FirstOrDefault()?.Url)
                        : u.MainImage;
                    var mainImageId = imgs.FirstOrDefault(i => i.IsMainImage)?.Id;
                    var additional = imgs.Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        PropertyId = i.PropertyId,
                        UnitId = i.UnitId,
                        Name = i.Name,
                        Url = i.Url,
                        SizeBytes = i.SizeBytes,
                        Type = i.Type,
                        Category = i.Category,
                        Caption = i.Caption,
                        AltText = i.AltText,
                        Tags = i.Tags,
                        Sizes = i.Sizes,
                        IsMain = i.IsMainImage,
                        DisplayOrder = i.DisplayOrder,
                        UploadedAt = i.UploadedAt,
                        Status = i.Status,
                        AssociationType = i.UnitId.HasValue ? "Unit" : "Property"
                    }).ToList();

                    resultItems.Add(new
                    {
                        Id = u.UnitId,
                        UnitInSectionId = u.Id,
                        Name = u.UnitName,
                        PropertyId = u.PropertyId,
                        UnitTypeId = u.UnitTypeId,
                        IsAvailable = u.IsAvailable,
                        MaxCapacity = u.MaxCapacity,
                        MainImageUrl = mainImage,
                        MainImageId = mainImageId,
                        ImageUrls = additional.Select(a => a.Url).ToList(),
                        AdditionalImages = additional,
                        Badge = u.Badge,
                        BadgeColor = u.BadgeColor,
                        DiscountPercentage = u.DiscountPercentage,
                        DiscountedPrice = u.DiscountedPrice
                    });
                }

                return PaginatedResult<object>.Create(resultItems, request.PageNumber, request.PageSize, total);
            }
		}
	}
}