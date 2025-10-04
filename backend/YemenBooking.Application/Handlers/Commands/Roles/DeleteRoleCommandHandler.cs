using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Roles;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Roles
{
    /// <summary>
    /// معالج أمر حذف دور
    /// </summary>
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ResultDto<bool>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;

        public DeleteRoleCommandHandler(
            IRoleRepository roleRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeleteRoleCommandHandler> logger)
        {
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء حذف الدور: RoleId={RoleId}", request.RoleId);

            // التحقق من المدخلات
            if (request.RoleId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الدور مطلوب");

            // التحقق من الصلاحيات (مسؤول عام)
            if (_currentUserService.Role != "Admin")
                return ResultDto<bool>.Failed("غير مصرح لك بحذف أدوار");

            // التحقق من الوجود
            var role = await _roleRepository.GetRoleByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return ResultDto<bool>.Failed("الدور غير موجود");

            // تنفيذ الحذف
            bool deleted = await _roleRepository.DeleteRoleAsync(request.RoleId, cancellationToken);
            if (!deleted)
                return ResultDto<bool>.Failed("فشل حذف الدور");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "DeleteRole",
                $"تم حذف الدور {request.RoleId}",
                request.RoleId,
                "Role",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل حذف الدور بنجاح: RoleId={RoleId}", request.RoleId);
            return ResultDto<bool>.Succeeded(true, "تم حذف الدور بنجاح");
        }
    }
} 