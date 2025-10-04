 using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Services;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Handlers.Commands.Services
{
    /// <summary>
    /// معالج أمر إنشاء خدمة جديدة للكيان
    /// </summary>
    public class CreatePropertyServiceCommandHandler : IRequestHandler<CreatePropertyServiceCommand, ResultDto<Guid>>
    {
        private readonly IPropertyServiceRepository _serviceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CreatePropertyServiceCommandHandler> _logger;

        public CreatePropertyServiceCommandHandler(
            IPropertyServiceRepository serviceRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<CreatePropertyServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreatePropertyServiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إنشاء خدمة للكيان: PropertyId={PropertyId}, Name={Name}", request.PropertyId, request.Name);

            // التحقق من المدخلات
            if (request.PropertyId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف الكيان مطلوب");
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<Guid>.Failed("اسم الخدمة مطلوب");
            if (request.Price == null || request.Price.Amount <= 0)
                return ResultDto<Guid>.Failed("السعر يجب أن يكون أكبر من صفر");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            var property = await _serviceRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<Guid>.Failed("الكيان غير موجود");
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<Guid>.Failed("غير مصرح لك بإنشاء خدمة لهذا الكيان");

            // التحقق من التكرار
            bool exists = await _serviceRepository.ExistsAsync(s => s.PropertyId == request.PropertyId && s.Name.Trim() == request.Name.Trim(), cancellationToken);
            if (exists)
                return ResultDto<Guid>.Failed("يوجد خدمة بنفس الاسم لهذا الكيان");

            // إنشاء الكيان
            var service = new PropertyService
            {
                PropertyId = request.PropertyId,
                Name = request.Name.Trim(),
                Price = new Money(request.Price.Amount, request.Price.Currency),
                PricingModel = request.PricingModel,
                Icon = (request.Icon ?? string.Empty).Trim(),
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _serviceRepository.CreatePropertyServiceAsync(service, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreatePropertyService",
                $"تم إنشاء خدمة جديدة {created.Id} للكيان {created.PropertyId}",
                created.Id,
                "PropertyService",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إنشاء الخدمة بنجاح: ServiceId={ServiceId}", created.Id);
            return ResultDto<Guid>.Succeeded(created.Id, "تم إنشاء الخدمة بنجاح");
        }
    }
}