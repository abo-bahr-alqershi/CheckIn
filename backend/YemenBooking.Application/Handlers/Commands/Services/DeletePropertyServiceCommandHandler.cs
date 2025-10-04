using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Services;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Services
{
    /// <summary>
    /// معالج أمر حذف خدمة كيان
    /// </summary>
    public class DeletePropertyServiceCommandHandler : IRequestHandler<DeletePropertyServiceCommand, ResultDto<bool>>
    {
        private readonly IPropertyServiceRepository _serviceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeletePropertyServiceCommandHandler> _logger;

        public DeletePropertyServiceCommandHandler(
            IPropertyServiceRepository serviceRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeletePropertyServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeletePropertyServiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء حذف خدمة الكيان: ServiceId={ServiceId}", request.ServiceId);

            if (request.ServiceId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الخدمة مطلوب");

            var service = await _serviceRepository.GetServiceByIdAsync(request.ServiceId, cancellationToken);
            if (service == null)
                return ResultDto<bool>.Failed("الخدمة غير موجودة");

            var property = await _serviceRepository.GetPropertyByIdAsync(service.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان المرتبط بالخدمة غير موجود");

            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بحذف هذه الخدمة");

            // التحقق من عدم وجود حجوزات أو مدفوعات تعتمد على هذه الخدمة
            var hasBookingRefs = await _serviceRepository.ServiceHasBookingReferencesAsync(request.ServiceId, cancellationToken);
            if (hasBookingRefs)
                return ResultDto<bool>.Failed("لا يمكن حذف الخدمة لارتباطها بحجوزات جارية أو سابقة");

            var success = await _serviceRepository.DeletePropertyServiceAsync(request.ServiceId, cancellationToken);
            if (!success)
                return ResultDto<bool>.Failed("فشل حذف الخدمة");

            await _auditService.LogBusinessOperationAsync(
                "DeletePropertyService",
                $"تم حذف الخدمة {request.ServiceId}",
                request.ServiceId,
                "PropertyService",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل حذف الخدمة: ServiceId={ServiceId}", request.ServiceId);
            return ResultDto<bool>.Succeeded(true, "تم حذف الخدمة بنجاح");
        }
    }
} 