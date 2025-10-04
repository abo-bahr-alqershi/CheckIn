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
    /// معالج أمر تحديث نوع الوحدة
    /// </summary>
    public class UpdateUnitTypeCommandHandler : IRequestHandler<UpdateUnitTypeCommand, ResultDto<bool>>
    {
        private readonly IUnitTypeRepository _repository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateUnitTypeCommandHandler> _logger;

        public UpdateUnitTypeCommandHandler(
            IUnitTypeRepository repository,
            IUnitRepository unitRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateUnitTypeCommandHandler> logger)
        {
            _repository = repository;
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateUnitTypeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث نوع الوحدة: UnitTypeId={UnitTypeId}", request.UnitTypeId);

            // التحقق من المدخلات
            if (request.UnitTypeId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف نوع الوحدة مطلوب");

            // التحقق من الصلاحيات (مسؤول)
            if (_currentUserService.Role != "Admin")
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث نوع الوحدة");

            // التحقق من الوجود
            var unitType = await _repository.GetUnitTypeByIdAsync(request.UnitTypeId, cancellationToken);
            if (unitType == null)
                return ResultDto<bool>.Failed("نوع الوحدة غير موجود");

            // التحقق من تأثير السعة على الحجوزات
            if (request.MaxCapacity < unitType.MaxCapacity)
            {
                bool hasActiveBookings = await _unitRepository.CheckActiveBookingsAsync(request.UnitTypeId, cancellationToken);
                if (hasActiveBookings)
                    return ResultDto<bool>.Failed("لا يمكن تخفيض السعة لوجود حجوزات نشطة");
            }

            // تنفيذ التحديث
            unitType.Name = request.Name.Trim();
            unitType.MaxCapacity = request.MaxCapacity;
            unitType.Icon = request.Icon.Trim();
            unitType.IsHasAdults = request.IsHasAdults;
            unitType.IsHasChildren = request.IsHasChildren;
            unitType.IsMultiDays = request.IsMultiDays;
            unitType.IsRequiredToDetermineTheHour = request.IsRequiredToDetermineTheHour;
            unitType.UpdatedBy = _currentUserService.UserId;
            unitType.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateUnitTypeAsync(unitType, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdateUnitType",
                $"تم تحديث نوع الوحدة {request.UnitTypeId}",
                request.UnitTypeId,
                "UnitType",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تحديث نوع الوحدة: UnitTypeId={UnitTypeId}", request.UnitTypeId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث نوع الوحدة بنجاح");
        }
    }
} 