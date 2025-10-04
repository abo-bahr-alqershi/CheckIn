using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Notifications;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Notifications;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Commands.Notifications
{
    /// <summary>
    /// معالج أمر إنشاء إشعار جديد
    /// </summary>
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, ResultDto<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            INotificationService notificationService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<CreateNotificationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ResultDto<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إنشاء إشعار جديد: RecipientId={RecipientId}, Type={Type}", request.RecipientId, request.Type);

            // التحقق من صحة المدخلات
            if (request.RecipientId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف المستلم مطلوب");
            if (string.IsNullOrWhiteSpace(request.Title))
                return ResultDto<Guid>.Failed("عنوان الإشعار مطلوب");
            if (string.IsNullOrWhiteSpace(request.Message))
                return ResultDto<Guid>.Failed("محتوى الإشعار مطلوب");
            if (string.IsNullOrWhiteSpace(request.Type))
                return ResultDto<Guid>.Failed("نوع الإشعار مطلوب");

            // التحقق من وجود المستلم
            var recipient = await _userRepository.GetUserByIdAsync(request.RecipientId, cancellationToken);
            if (recipient == null)
                return ResultDto<Guid>.Failed("المستلم غير موجود");

            // إنشاء كيان الإشعار
            var notification = new Notification
            {
                RecipientId = request.RecipientId,
                SenderId = request.SenderId,
                Type = request.Type,
                Title = request.Title,
                Message = request.Message,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _unitOfWork.Repository<Notification>().AddAsync(notification, cancellationToken);
            var newId = created.Id;

            // إرسال الإشعار عبر الخدمة مباشرة بعد الإنشاء
            if (Enum.TryParse<NotificationType>(request.Type, true, out var notifType))
            {
                await _notificationService.SendAsync(new NotificationRequest
                {
                    UserId = request.RecipientId,
                    Type = notifType,
                    Title = request.Title,
                    Message = request.Message,
                    Data = new { SenderId = request.SenderId }
                }, cancellationToken);
            }
            else
            {
                // fallback: still send as system type
                await _notificationService.SendAsync(new NotificationRequest
                {
                    UserId = request.RecipientId,
                    Type = NotificationType.System,
                    Title = request.Title,
                    Message = request.Message,
                    Data = new { SenderId = request.SenderId, OriginalType = request.Type }
                }, cancellationToken);
                _logger.LogWarning("نوع الإشعار غير معروف، تم الإرسال بنوع System: {Type}", request.Type);
            }

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreateNotification",
                $"تم إنشاء إشعار جديد للمستخدم {request.RecipientId} من النوع {request.Type}",
                newId,
                "Notification",
                _currentUserService.UserId,
                new System.Collections.Generic.Dictionary<string, object>
                {
                    { "NotificationId", newId },
                    { "Type", request.Type }
                },
                cancellationToken);

            _logger.LogInformation("تم إنشاء الإشعار بنجاح: Id={NotificationId}", newId);
            return ResultDto<Guid>.Succeeded(newId, "تم إنشاء الإشعار بنجاح");
        }
    }
} 