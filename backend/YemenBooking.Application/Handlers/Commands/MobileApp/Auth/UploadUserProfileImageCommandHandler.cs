using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Auth
{
    public class UploadUserProfileImageCommandHandler : IRequestHandler<UploadUserProfileImageCommand, ResultDto<UploadUserProfileImageResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<UploadUserProfileImageCommandHandler> _logger;

        public UploadUserProfileImageCommandHandler(
            IUserRepository userRepository,
            IFileUploadService fileUploadService,
            ILogger<UploadUserProfileImageCommandHandler> logger)
        {
            _userRepository = userRepository;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        public async Task<ResultDto<UploadUserProfileImageResponse>> Handle(UploadUserProfileImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                {
                    return ResultDto<UploadUserProfileImageResponse>.Failed("معرف المستخدم غير صالح", "INVALID_USER_ID");
                }
                if (request.FileBytes == null || request.FileBytes.Length == 0)
                {
                    return ResultDto<UploadUserProfileImageResponse>.Failed("لا توجد صورة مرفوعة", "FILE_EMPTY");
                }
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!_fileUploadService.IsValidFileType(request.FileName, allowed))
                {
                    return ResultDto<UploadUserProfileImageResponse>.Failed("نوع الملف غير مدعوم", "INVALID_FILE_TYPE");
                }
                if (!_fileUploadService.IsValidFileSize(request.FileBytes.LongLength, 5))
                {
                    return ResultDto<UploadUserProfileImageResponse>.Failed("حجم الملف يتجاوز 5MB", "FILE_TOO_LARGE");
                }

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return ResultDto<UploadUserProfileImageResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
                }

                using var stream = new MemoryStream(request.FileBytes);
                var uniqueName = _fileUploadService.GenerateUniqueFileName(request.FileName);
                var imageUrl = await _fileUploadService.UploadProfileImageAsync(stream, uniqueName, cancellationToken);

                user.ProfileImageUrl = imageUrl;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user, cancellationToken);

                var response = new UploadUserProfileImageResponse
                {
                    UserId = user.Id,
                    ProfileImageUrl = user.ProfileImageUrl,
                    UpdatedAt = user.UpdatedAt
                };

                return ResultDto<UploadUserProfileImageResponse>.Ok(response, "تم رفع صورة الملف الشخصي بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل رفع صورة الملف الشخصي للمستخدم {UserId}", request.UserId);
                return ResultDto<UploadUserProfileImageResponse>.Failed("حدث خطأ أثناء رفع الصورة", "UPLOAD_FAILED");
            }
        }
    }
}