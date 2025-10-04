using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Properties;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Notifications;

namespace YemenBooking.Application.Handlers.Commands.Properties
{
    /// <summary>
    /// معالج أمر الموافقة على الكيان
    /// </summary>
    public class ApprovePropertyCommandHandler : IRequestHandler<ApprovePropertyCommand, ResultDto<bool>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ApprovePropertyCommandHandler> _logger;
        private readonly IIndexingService _indexingService;

        public ApprovePropertyCommandHandler(
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            IAuditService auditService,
            ILogger<ApprovePropertyCommandHandler> logger,
            IIndexingService indexingService)
        {
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _auditService = auditService;
            _logger = logger;
            _indexingService = indexingService;
        }

        public async Task<ResultDto<bool>> Handle(ApprovePropertyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء الموافقة على الكيان: PropertyId={PropertyId}", request.PropertyId);

            // التحقق من صحة المدخلات
            if (request.PropertyId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الكيان مطلوب");
            if (request.AdminId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف المسؤول مطلوب");

            // التحقق من الصلاحيات (مسؤول)
            if (_currentUserService.Role != "Admin")
                return ResultDto<bool>.Failed("غير مصرح لك بالموافقة على الكيان");

            // التحقق من وجود الكيان وحالته
            var property = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان غير موجود");
            if (property.IsApproved)
                return ResultDto<bool>.Failed("الكيان معتمد مسبقاً");

            // تنفيذ الموافقة
            var success = await _propertyRepository.ApprovePropertyAsync(request.PropertyId, cancellationToken);
            if (!success)
                return ResultDto<bool>.Failed("فشل الموافقة على الكيان");

            // تحديث فهرس LiteDB لتعكس حالة الاعتماد
            await _indexingService.OnPropertyUpdatedAsync(request.PropertyId, cancellationToken);

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "ApproveProperty",
                $"تمت الموافقة على الكيان {request.PropertyId}",
                request.PropertyId,
                "Property",
                _currentUserService.UserId,
                null,
                cancellationToken);

            // إرسال إشعار للمالك
            await _notificationService.SendAsync(new NotificationRequest
            {
                UserId = property.OwnerId,
                Type = NotificationType.BookingConfirmed,
                Title = "تمت الموافقة على الكيان",
                Message = $"تمت الموافقة على كيانك '{property.Name}' بنجاح"
            }, cancellationToken);

            _logger.LogInformation("اكتملت الموافقة على الكيان: PropertyId={PropertyId}", request.PropertyId);
            return ResultDto<bool>.Succeeded(true, "تمت الموافقة على الكيان بنجاح");
        }
    }
} 