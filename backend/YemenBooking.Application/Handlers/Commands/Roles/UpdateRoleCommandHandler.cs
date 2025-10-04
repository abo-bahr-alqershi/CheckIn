using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Roles;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Roles
{
    /// <summary>
    /// معالج أمر تحديث دور
    /// </summary>
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ResultDto<bool>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(
            IRoleRepository roleRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateRoleCommandHandler> logger)
        {
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث الدور: RoleId={RoleId}, Name={Name}", request.RoleId, request.Name);

            // التحقق من المدخلات
            if (request.RoleId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الدور مطلوب");
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<bool>.Failed("اسم الدور مطلوب");

            // التحقق من الصلاحيات (مسؤول عام)
            if (_currentUserService.Role != "Admin")
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث أدوار");

            // التحقق من الوجود
            var role = await _roleRepository.GetRoleByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return ResultDto<bool>.Failed("الدور غير موجود");

            // التحقق من التكرار عند تغيير الاسم
            if (!string.Equals(role.Name, request.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                bool duplicate = await _roleRepository.ExistsAsync(r => r.Name.Trim().ToLower() == request.Name.Trim().ToLower() && r.Id != request.RoleId, cancellationToken);
                if (duplicate)
                    return ResultDto<bool>.Failed("يوجد دور آخر بنفس الاسم");
                role.Name = request.Name.Trim();
            }

            role.UpdatedBy = _currentUserService.UserId;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateRoleAsync(role, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdateRole",
                $"تم تحديث الدور {request.RoleId}",
                request.RoleId,
                "Role",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تحديث الدور بنجاح: RoleId={RoleId}", request.RoleId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث الدور بنجاح");
        }
    }
} 