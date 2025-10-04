using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تحديث صورة ملف المستخدم الشخصي
    /// </summary>
    public class UpdateUserProfilePictureCommandHandler : IRequestHandler<UpdateUserProfilePictureCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateUserProfilePictureCommandHandler> _logger;

        public UpdateUserProfilePictureCommandHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateUserProfilePictureCommandHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateUserProfilePictureCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث صورة المستخدم: UserId={UserId}", request.UserId);

            // التحقق من المدخلات
            if (request.UserId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف المستخدم مطلوب");
            if (string.IsNullOrWhiteSpace(request.ProfileImageUrl))
                return ResultDto<bool>.Failed("رابط الصورة مطلوب");

            // التحقق من الوجود
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return ResultDto<bool>.Failed("المستخدم غير موجود");

            // التحقق من الصلاحيات (المستخدم نفسه أو مسؤول)
            if (_currentUserService.Role != "Admin" && user.Id != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث صورة الملف الشخصي لهذا المستخدم");

            // التنفيذ
            user.ProfileImage = request.ProfileImageUrl.Trim();
            user.UpdatedBy = _currentUserService.UserId;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdateUserProfilePicture",
                $"تم تحديث صورة الملف الشخصي للمستخدم {request.UserId}",
                request.UserId,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تحديث صورة الملف الشخصي بنجاح: UserId={UserId}", request.UserId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث صورة الملف الشخصي بنجاح");
        }
    }
} 