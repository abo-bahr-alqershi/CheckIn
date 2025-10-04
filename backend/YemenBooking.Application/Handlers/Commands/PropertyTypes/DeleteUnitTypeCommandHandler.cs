using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.PropertyTypes;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.PropertyTypes
{
    /// <summary>
    /// معالج أمر حذف نوع الوحدة
    /// </summary>
    public class DeleteUnitTypeCommandHandler : IRequestHandler<DeleteUnitTypeCommand, ResultDto<bool>>
    {
        private readonly IUnitTypeRepository _repository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteUnitTypeCommandHandler> _logger;

        public DeleteUnitTypeCommandHandler(
            IUnitTypeRepository repository,
            IUnitRepository unitRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeleteUnitTypeCommandHandler> logger)
        {
            _repository = repository;
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeleteUnitTypeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء حذف نوع الوحدة: UnitTypeId={UnitTypeId}", request.UnitTypeId);

            // التحقق من المدخلات
            if (request.UnitTypeId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف نوع الوحدة مطلوب");

            // التحقق من الصلاحيات (مسؤول)
            if (_currentUserService.Role != "Admin")
                return ResultDto<bool>.Failed("غير مصرح لك بحذف نوع الوحدة");

            // التحقق من الوجود
            var unitType = await _repository.GetUnitTypeByIdAsync(request.UnitTypeId, cancellationToken);
            if (unitType == null)
                return ResultDto<bool>.Failed("نوع الوحدة غير موجود");

            // التحقق من عدم وجود وحدات مرتبطة
            bool hasUnits = await _unitRepository.ExistsAsync(u => u.UnitTypeId == request.UnitTypeId, cancellationToken);
            if (hasUnits)
                return ResultDto<bool>.Failed("لا يمكن حذف نوع الوحدة لوجود وحدات مرتبطة به");

            // تنفيذ الحذف
            var success = await _repository.DeleteUnitTypeAsync(request.UnitTypeId, cancellationToken);
            if (!success)
                return ResultDto<bool>.Failed("فشل حذف نوع الوحدة");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "DeleteUnitType",
                $"تم حذف نوع الوحدة {request.UnitTypeId}",
                request.UnitTypeId,
                "UnitType",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل حذف نوع الوحدة: UnitTypeId={UnitTypeId}", request.UnitTypeId);
            return ResultDto<bool>.Succeeded(true, "تم حذف نوع الوحدة بنجاح");
        }
    }
} 