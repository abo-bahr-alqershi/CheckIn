using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.PropertyTypes;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.PropertyTypes
{
    /// <summary>
    /// معالج أمر إنشاء نوع وحدة جديد
    /// </summary>
    public class CreateUnitTypeCommandHandler : IRequestHandler<CreateUnitTypeCommand, ResultDto<Guid>>
    {
        private readonly IUnitTypeRepository _repository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CreateUnitTypeCommandHandler> _logger;

        public CreateUnitTypeCommandHandler(
            IUnitTypeRepository repository,
            IPropertyTypeRepository propertyTypeRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<CreateUnitTypeCommandHandler> logger)
        {
            _repository = repository;
            _propertyTypeRepository = propertyTypeRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreateUnitTypeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إنشاء نوع وحدة جديد: PropertyTypeId={PropertyTypeId}, Name={Name}", request.PropertyTypeId, request.Name);

            // التحقق من المدخلات
            if (request.PropertyTypeId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف نوع الكيان مطلوب");
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<Guid>.Failed("اسم نوع الوحدة مطلوب");
            if (request.MaxCapacity <= 0)
                return ResultDto<Guid>.Failed("السعة يجب أن تكون أكبر من صفر");

            // التحقق من الصلاحيات (مسؤول)
            if (_currentUserService.Role != "Admin")
                return ResultDto<Guid>.Failed("غير مصرح لك بإنشاء نوع وحدة");

            // التحقق من وجود نوع الكيان
            var propertyType = await _propertyTypeRepository.GetPropertyTypeByIdAsync(request.PropertyTypeId, cancellationToken);
            if (propertyType == null)
                return ResultDto<Guid>.Failed("نوع الكيان غير موجود");

            // التحقق من التكرار ضمن نفس نوع الكيان
            bool exists = await _repository.ExistsAsync(ut => ut.PropertyTypeId == request.PropertyTypeId && ut.Name.Trim() == request.Name.Trim(), cancellationToken);
            if (exists)
                return ResultDto<Guid>.Failed("يوجد نوع وحدة بنفس الاسم لنفس نوع الكيان");

            // إنشاء نوع الوحدة
            var unitType = new UnitType
            {
                PropertyTypeId = request.PropertyTypeId,
                Name = request.Name.Trim(),
                MaxCapacity = request.MaxCapacity,
                Icon = request.Icon.Trim(),
                IsHasAdults = request.IsHasAdults,
                IsHasChildren = request.IsHasChildren,
                IsMultiDays = request.IsMultiDays,
                IsRequiredToDetermineTheHour = request.IsRequiredToDetermineTheHour,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _repository.CreateUnitTypeAsync(unitType, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreateUnitType",
                $"تم إنشاء نوع وحدة جديد {created.Id} باسم {created.Name}",
                created.Id,
                "UnitType",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إنشاء نوع الوحدة: UnitTypeId={UnitTypeId}", created.Id);
            return ResultDto<Guid>.Succeeded(created.Id, "تم إنشاء نوع الوحدة بنجاح");
        }
    }
} 