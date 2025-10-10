using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.CP.UnitInSectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using System.Text.Json;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Entities;

namespace YemenBooking.Application.Handlers.Commands.UnitInSectionImages
{
    public class UpdateUnitInSectionImageCommandHandler : IRequestHandler<UpdateUnitInSectionImageCommand, ResultDto<ImageDto>>
    {
        private readonly IUnitInSectionImageRepository _repo;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUserService;
        public UpdateUnitInSectionImageCommandHandler(IUnitInSectionImageRepository repo, IAuditService auditService, ICurrentUserService currentUserService)
        {
            _repo = repo;
            _auditService = auditService;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<ImageDto>> Handle(UpdateUnitInSectionImageCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByIdAsync(request.ImageId, cancellationToken);
            if (entity == null) return ResultDto<ImageDto>.Failed("الصورة غير موجودة");
            if (request.Category.HasValue) entity.Category = request.Category.Value;
            if (!string.IsNullOrWhiteSpace(request.Alt)) { entity.AltText = request.Alt; entity.Caption = request.Alt; }
            if (request.Order.HasValue) entity.DisplayOrder = request.Order.Value;
            if (request.Tags != null) entity.Tags = JsonSerializer.Serialize(request.Tags);
            if (request.IsPrimary.HasValue && request.IsPrimary.Value)
            {
                await _repo.UpdateMainImageStatusAsync(entity.Id, true, cancellationToken);
            }
            else if (request.IsPrimary.HasValue && !request.IsPrimary.Value)
            {
                entity.IsMainImage = false;
                await _repo.UpdateAsync(entity, cancellationToken);
            }
            else
            {
                await _repo.UpdateAsync(entity, cancellationToken);
            }

            // Audit log
            await _auditService.LogAuditAsync(
                entityType: nameof(UnitInSectionImage),
                entityId: entity.UnitInSectionId ?? System.Guid.Empty,
                action: YemenBooking.Core.Entities.AuditAction.UPDATE,
                oldValues: null,
                newValues: JsonSerializer.Serialize(new { entity.Id, entity.Category, entity.DisplayOrder, entity.IsMainImage, entity.Tags, entity.AltText }),
                performedBy: _currentUserService.UserId,
                notes: $"تم تحديث صورة عنصر وحدة في القسم بواسطة {_currentUserService.Username} (ID={_currentUserService.UserId})",
                cancellationToken: cancellationToken);

            var dto = new ImageDto
            {
                Id = entity.Id,
                Url = entity.Url,
                Filename = entity.Name,
                Size = entity.SizeBytes,
                MimeType = entity.Type,
                Width = 0,
                Height = 0,
                Alt = entity.AltText,
                UploadedAt = entity.UploadedAt,
                UploadedBy = entity.CreatedBy ?? System.Guid.Empty,
                Order = entity.DisplayOrder,
                IsPrimary = entity.IsMainImage,
                Category = entity.Category,
                Tags = string.IsNullOrWhiteSpace(entity.Tags) ? new System.Collections.Generic.List<string>() : JsonSerializer.Deserialize<System.Collections.Generic.List<string>>(entity.Tags) ?? new System.Collections.Generic.List<string>(),
                ProcessingStatus = entity.Status.ToString(),
                Thumbnails = new ImageThumbnailsDto { Small = entity.Sizes ?? string.Empty, Medium = entity.Sizes ?? string.Empty, Large = entity.Sizes ?? string.Empty, Hd = entity.Sizes ?? string.Empty },
                MediaType = string.IsNullOrWhiteSpace(entity.MediaType) ? "image" : entity.MediaType,
                Duration = entity.DurationSeconds,
                VideoThumbnail = string.IsNullOrWhiteSpace(entity.VideoThumbnailUrl) ? null : entity.VideoThumbnailUrl
            };

            return ResultDto<ImageDto>.Ok(dto);
        }
    }
}

