using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Queries.Images;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Queries.Images
{
    /// <summary>
    /// معالج استعلام الحصول على صورة واحدة بواسطة المعرف
    /// Handler for GetImageByIdQuery to retrieve a single image by its ID
    /// </summary>
    public class GetImageByIdQueryHandler : IRequestHandler<GetImageByIdQuery, ResultDto<ImageDto>>
    {
        private readonly IPropertyImageRepository _imageRepository;
        private readonly ISectionImageRepository _sectionImageRepository;
        private readonly IPropertyInSectionImageRepository _propertyInSectionImageRepository;
        private readonly IUnitInSectionImageRepository _unitInSectionImageRepository;

        public GetImageByIdQueryHandler(
            IPropertyImageRepository imageRepository,
            ISectionImageRepository sectionImageRepository,
            IPropertyInSectionImageRepository propertyInSectionImageRepository,
            IUnitInSectionImageRepository unitInSectionImageRepository)
        {
            _imageRepository = imageRepository;
            _sectionImageRepository = sectionImageRepository;
            _propertyInSectionImageRepository = propertyInSectionImageRepository;
            _unitInSectionImageRepository = unitInSectionImageRepository;
        }

        public async Task<ResultDto<ImageDto>> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
        {
            // جلب الصورة من المستودعات الخاصة أولاً للحفاظ على الدلالات الجديدة
            var s = await _sectionImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
            if (s != null) return ResultDto<ImageDto>.Ok(ToDto(s));
            var pis = await _propertyInSectionImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
            if (pis != null) return ResultDto<ImageDto>.Ok(ToDto(pis));
            var uis = await _unitInSectionImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
            if (uis != null) return ResultDto<ImageDto>.Ok(ToDto(uis));
            var image = await _imageRepository.GetPropertyImageByIdAsync(request.ImageId, cancellationToken);
            if (image == null)
                return ResultDto<ImageDto>.Failure("الصورة غير موجودة");

            // تحويل الكيان إلى DTO
            var dto = new ImageDto
            {
                Id = image.Id,
                Url = image.Url,
                Filename = image.Name,
                Size = image.SizeBytes,
                MimeType = image.Type,
                Width = 0,
                Height = 0,
                Alt = image.AltText,
                UploadedAt = image.UploadedAt,
                UploadedBy = image.CreatedBy ?? Guid.Empty,
                Order = image.DisplayOrder,
                IsPrimary = image.IsMain,
                PropertyId = image.PropertyId,
                UnitId = image.UnitId,
                Category = image.Category,
                Tags = string.IsNullOrWhiteSpace(image.Tags)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(image.Tags)!,
                ProcessingStatus = image.Status.ToString(),
                Thumbnails = new ImageThumbnailsDto
                {
                    Small = image.Sizes,
                    Medium = image.Sizes,
                    Large = image.Sizes,
                    Hd = image.Sizes
                },
                MediaType = string.IsNullOrWhiteSpace(image.MediaType)
                    ? ((image.Type?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ?? false) ? "video" : "image")
                    : image.MediaType,
                Duration = image.DurationSeconds,
                VideoThumbnail = string.IsNullOrWhiteSpace(image.VideoThumbnailUrl) ? null : image.VideoThumbnailUrl
            };

            return ResultDto<ImageDto>.Ok(dto);
        }

        private static ImageDto ToDto(Core.Entities.SectionImage i) => new ImageDto
        {
            Id = i.Id,
            Url = i.Url,
            Filename = i.Name,
            Size = i.SizeBytes,
            MimeType = i.Type,
            Width = 0,
            Height = 0,
            Alt = i.AltText,
            UploadedAt = i.UploadedAt,
            UploadedBy = i.CreatedBy ?? Guid.Empty,
            Order = i.DisplayOrder,
            IsPrimary = i.IsMainImage,
            Category = i.Category,
            Tags = string.IsNullOrWhiteSpace(i.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(i.Tags) ?? new List<string>(),
            ProcessingStatus = i.Status.ToString(),
            Thumbnails = new ImageThumbnailsDto { Small = i.Sizes ?? string.Empty, Medium = i.Sizes ?? string.Empty, Large = i.Sizes ?? string.Empty, Hd = i.Sizes ?? string.Empty },
            MediaType = string.IsNullOrWhiteSpace(i.MediaType) ? "image" : i.MediaType,
            Duration = i.DurationSeconds,
            VideoThumbnail = string.IsNullOrWhiteSpace(i.VideoThumbnailUrl) ? null : i.VideoThumbnailUrl
        };

        private static ImageDto ToDto(Core.Entities.PropertyInSectionImage i) => new ImageDto
        {
            Id = i.Id,
            Url = i.Url,
            Filename = i.Name,
            Size = i.SizeBytes,
            MimeType = i.Type,
            Width = 0,
            Height = 0,
            Alt = i.AltText,
            UploadedAt = i.UploadedAt,
            UploadedBy = i.CreatedBy ?? Guid.Empty,
            Order = i.DisplayOrder,
            IsPrimary = i.IsMainImage,
            Category = i.Category,
            Tags = string.IsNullOrWhiteSpace(i.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(i.Tags) ?? new List<string>(),
            ProcessingStatus = i.Status.ToString(),
            Thumbnails = new ImageThumbnailsDto { Small = i.Sizes ?? string.Empty, Medium = i.Sizes ?? string.Empty, Large = i.Sizes ?? string.Empty, Hd = i.Sizes ?? string.Empty },
            MediaType = string.IsNullOrWhiteSpace(i.MediaType) ? "image" : i.MediaType,
            Duration = i.DurationSeconds,
            VideoThumbnail = string.IsNullOrWhiteSpace(i.VideoThumbnailUrl) ? null : i.VideoThumbnailUrl
        };

        private static ImageDto ToDto(Core.Entities.UnitInSectionImage i) => new ImageDto
        {
            Id = i.Id,
            Url = i.Url,
            Filename = i.Name,
            Size = i.SizeBytes,
            MimeType = i.Type,
            Width = 0,
            Height = 0,
            Alt = i.AltText,
            UploadedAt = i.UploadedAt,
            UploadedBy = i.CreatedBy ?? Guid.Empty,
            Order = i.DisplayOrder,
            IsPrimary = i.IsMainImage,
            Category = i.Category,
            Tags = string.IsNullOrWhiteSpace(i.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(i.Tags) ?? new List<string>(),
            ProcessingStatus = i.Status.ToString(),
            Thumbnails = new ImageThumbnailsDto { Small = i.Sizes ?? string.Empty, Medium = i.Sizes ?? string.Empty, Large = i.Sizes ?? string.Empty, Hd = i.Sizes ?? string.Empty },
            MediaType = string.IsNullOrWhiteSpace(i.MediaType) ? "image" : i.MediaType,
            Duration = i.DurationSeconds,
            VideoThumbnail = string.IsNullOrWhiteSpace(i.VideoThumbnailUrl) ? null : i.VideoThumbnailUrl
        };
    }
} 