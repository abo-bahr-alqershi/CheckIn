using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر رفع ملف مرفق في الشات
    /// Handler for UploadFileCommand
    /// </summary>
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, ResultDto<ChatAttachmentDto>>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IChatAttachmentRepository _attachmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IMediaMetadataService _mediaMetadataService;

        public UploadFileCommandHandler(
            IFileStorageService fileStorageService,
            IChatAttachmentRepository attachmentRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IMediaMetadataService mediaMetadataService)
        {
            _fileStorageService = fileStorageService;
            _attachmentRepository = attachmentRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _mediaMetadataService = mediaMetadataService;
        }

        public async Task<ResultDto<ChatAttachmentDto>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var file = request.File;
                using var stream = file.OpenReadStream();
                var uploadResult = await _fileStorageService.UploadFileAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    folder: "ChatAttachments",
                    cancellationToken: cancellationToken
                );

                if (!uploadResult.IsSuccess || string.IsNullOrEmpty(uploadResult.FilePath))
                    return ResultDto<ChatAttachmentDto>.Failed("فشل في رفع الملف");

                // Try to extract media duration if applicable
                int? durationSeconds = null;
                try
                {
                    durationSeconds = await _mediaMetadataService.TryGetDurationSecondsAsync(
                        uploadResult.FilePath!,
                        uploadResult.ContentType ?? file.ContentType,
                        cancellationToken);
                }
                catch { /* non-fatal */ }

                var attachment = new ChatAttachment
                {
                    ConversationId = request.ConversationId,
                    FileName = uploadResult.FileName ?? file.FileName,
                    ContentType = uploadResult.ContentType ?? file.ContentType,
                    FileSize = uploadResult.FileSizeBytes,
                    FilePath = uploadResult.FilePath!,
                    UploadedBy = _currentUserService.UserId,
                    CreatedAt = uploadResult.UploadedAt,
                    DurationSeconds = durationSeconds
                };

                await _attachmentRepository.AddAsync(attachment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var dto = _mapper.Map<ChatAttachmentDto>(attachment);
                return ResultDto<ChatAttachmentDto>.Ok(dto, "تم رفع الملف بنجاح");
            }
            catch (Exception ex)
            {
                return ResultDto<ChatAttachmentDto>.Failed($"خطأ أثناء رفع الملف: {ex.Message}");
            }
        }
    }
}