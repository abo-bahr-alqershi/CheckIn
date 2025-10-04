using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Staffs;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Staffs
{
    /// <summary>
    /// معالج أمر إزالة موظف
    /// </summary>
    public class RemoveStaffCommandHandler : IRequestHandler<RemoveStaffCommand, ResultDto<bool>>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<RemoveStaffCommandHandler> _logger;

        public RemoveStaffCommandHandler(
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<RemoveStaffCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(RemoveStaffCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إزالة موظف: StaffId={StaffId}", request.StaffId);

            // التحقق من المدخلات
            if (request.StaffId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الموظف مطلوب");

            // التحقق من الوجود
            var staff = await _staffRepository.GetStaffByIdAsync(request.StaffId, cancellationToken);
            if (staff == null)
                return ResultDto<bool>.Failed("الموظف غير موجود");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            var property = await _staffRepository.GetPropertyByIdAsync(staff.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان المرتبط بالموظف غير موجود");
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بحذف هذا الموظف");

            // تنفيذ الإزالة
            bool removed = await _staffRepository.RemoveStaffAsync(request.StaffId, cancellationToken);
            if (!removed)
                return ResultDto<bool>.Failed("فشل إزالة الموظف");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "RemoveStaff",
                $"تم إزالة الموظف {request.StaffId}",
                request.StaffId,
                "Staff",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتملت عملية إزالة الموظف بنجاح: StaffId={StaffId}", request.StaffId);
            return ResultDto<bool>.Succeeded(true, "تم إزالة الموظف بنجاح");
        }
    }
} 