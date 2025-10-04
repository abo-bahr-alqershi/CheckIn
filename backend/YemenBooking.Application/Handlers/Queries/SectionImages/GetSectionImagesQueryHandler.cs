using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.Queries.CP.SectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using System.Collections.Generic;
using YemenBooking.Core.Enums;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Queries.SectionImages
{
    /// <summary>
    /// معالج استعلام الحصول على صور القسم
    /// Handler for GetSectionImagesQuery
    /// </summary>
    public class GetSectionImagesQueryHandler : IRequestHandler<GetSectionImagesQuery, ResultDto<PaginatedResultDto<ImageDto>>>
    {
        private readonly ISectionImageRepository _sectionImageRepository;

        public GetSectionImagesQueryHandler(ISectionImageRepository sectionImageRepository)
        {
            _sectionImageRepository = sectionImageRepository;
        }

        public async Task<ResultDto<PaginatedResultDto<ImageDto>>> Handle(GetSectionImagesQuery request, CancellationToken cancellationToken)
        {
            // 1. بناء الاستعلام مع الفلاتر
            var query = _sectionImageRepository.GetQueryable().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.TempKey))
                query = query.Where(i => i.TempKey == request.TempKey);

            if (request.SectionId.HasValue)
                query = query.Where(i => i.SectionId == request.SectionId.Value);

            // إذا لم يتم تحديد أي معيار
            if (!request.SectionId.HasValue && string.IsNullOrWhiteSpace(request.TempKey))
            {
                // نرجع قائمة فارغة
                return ResultDto<PaginatedResultDto<ImageDto>>.Ok(new PaginatedResultDto<ImageDto>
                {
                    Items = new List<ImageDto>(),
                    Total = 0,
                    Page = request.Page,
                    Limit = request.Limit,
                    TotalPages = 0
                });
            }

            // 2. تطبيق الفرز
            var sortBy = request.SortBy?.Trim().ToLower();
            var ascending = string.Equals(request.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                // Default: order by DisplayOrder then UploadedAt (ascending)
                query = query
                    .OrderBy(i => i.DisplayOrder)
                    .ThenBy(i => i.UploadedAt);
            }
            else
            {
                query = sortBy switch
                {
                    "uploadedat" or "date" => ascending
                        ? query.OrderBy(i => i.UploadedAt)
                        : query.OrderByDescending(i => i.UploadedAt),
                    "order" => ascending
                        ? query.OrderBy(i => i.DisplayOrder)
                        : query.OrderByDescending(i => i.DisplayOrder),
                    "filename" or "name" => ascending
                        ? query.OrderBy(i => i.Name)
                        : query.OrderByDescending(i => i.Name),
                    "size" => ascending
                        ? query.OrderBy(i => i.SizeBytes)
                        : query.OrderByDescending(i => i.SizeBytes),
                    _ => query.OrderBy(i => i.DisplayOrder).ThenBy(i => i.UploadedAt),
                };
            }

            // 3. تطبيق الترقيم
            var page = request.Page;
            var limit = request.Limit;
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);

            // 4. تحويل إلى DTO
            var dtos = items.Select(i => new ImageDto
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
                SectionId = i.SectionId,
                Category = i.Category,
                Tags = string.IsNullOrWhiteSpace(i.Tags)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(i.Tags) ?? new List<string>(),
                ProcessingStatus = i.Status.ToString(),
                Thumbnails = new ImageThumbnailsDto
                {
                    Small = ResolveThumbnail(i, false),
                    Medium = ResolveThumbnail(i, false),
                    Large = ResolveThumbnail(i, true),
                    Hd = ResolveThumbnail(i, true)
                },
                MediaType = string.IsNullOrWhiteSpace(i.MediaType)
                    ? ((i.Type?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ?? false) ? "video" : "image")
                    : i.MediaType,
                Duration = i.DurationSeconds,
                VideoThumbnail = string.IsNullOrWhiteSpace(i.VideoThumbnailUrl) ? null : i.VideoThumbnailUrl,
            }).ToList();

            // 5. إعداد نتيجة الترقيم
            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
            var paged = new PaginatedResultDto<ImageDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = page,
                Limit = limit,
                TotalPages = totalPages
            };

            return ResultDto<PaginatedResultDto<ImageDto>>.Ok(paged);
        }

        private static string ResolveThumbnail(Core.Entities.SectionImage i, bool preferHd)
        {
            var isVideo = string.Equals(i.MediaType, "video", StringComparison.OrdinalIgnoreCase)
                          || (i.Type?.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ?? false)
                          || i.Url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
                          || i.Url.EndsWith(".webm", StringComparison.OrdinalIgnoreCase)
                          || i.Url.EndsWith(".mov", StringComparison.OrdinalIgnoreCase)
                          || i.Url.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase);

            if (isVideo)
            {
                // استخدم VideoThumbnail إن توفر؛ وإلا اترك الحقل فارغاً كي تتعامل الواجهة مع Placeholder
                return string.IsNullOrWhiteSpace(i.VideoThumbnailUrl) ? string.Empty : i.VideoThumbnailUrl!;
            }
            return i.Sizes ?? string.Empty;
        }
    }
}