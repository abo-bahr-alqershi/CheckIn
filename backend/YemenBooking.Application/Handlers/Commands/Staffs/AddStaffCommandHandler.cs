using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Staffs;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Staffs
{
    /// <summary>
    /// معالج أمر إضافة موظف جديد
    /// </summary>
    public class AddStaffCommandHandler : IRequestHandler<AddStaffCommand, ResultDto<Guid>>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AddStaffCommandHandler> _logger;

        public AddStaffCommandHandler(
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<AddStaffCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(AddStaffCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إضافة موظف: UserId={UserId}, PropertyId={PropertyId}, Position={Position}", request.UserId, request.PropertyId, request.Position);

            // التحقق من المدخلات
            if (request.UserId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف المستخدم مطلوب");
            if (request.PropertyId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف الكيان مطلوب");
            if (string.IsNullOrWhiteSpace(request.Permissions))
                return ResultDto<Guid>.Failed("الصلاحيات مطلوبة");

            // التحقق من وجود المستخدم
            var user = await _staffRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null || !user.IsActive)
                return ResultDto<Guid>.Failed("المستخدم غير موجود أو غير نشط");

            // التحقق من وجود الكيان
            var property = await _staffRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<Guid>.Failed("الكيان غير موجود");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<Guid>.Failed("غير مصرح لك بإضافة موظف لهذا الكيان");

            // التحقق من التكرار
            bool exists = await _staffRepository.ExistsAsync(s => s.UserId == request.UserId && s.PropertyId == request.PropertyId, cancellationToken);
            if (exists)
                return ResultDto<Guid>.Failed("الموظف موجود بالفعل لهذا الكيان");

            // إنشاء الكيان
            var staff = new Staff
            {
                UserId = request.UserId,
                PropertyId = request.PropertyId,
                Position = request.Position,
                Permissions = request.Permissions.Trim(),
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _staffRepository.AddStaffAsync(staff, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "AddStaff",
                $"تم إضافة موظف جديد {created.Id} للمستخدم {created.UserId} في الكيان {created.PropertyId}",
                created.Id,
                "Staff",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إضافة الموظف بنجاح: StaffId={StaffId}", created.Id);
            return ResultDto<Guid>.Succeeded(created.Id, "تم إضافة الموظف بنجاح");
        }
    }
} 