using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تخصيص دور للمستخدم
    /// </summary>
    public class AssignUserRoleCommandHandler : IRequestHandler<AssignUserRoleCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AssignUserRoleCommandHandler> _logger;

        public AssignUserRoleCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<AssignUserRoleCommandHandler> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تخصيص دور للمستخدم: UserId={UserId}, RoleId={RoleId}", request.UserId, request.RoleId);

            // التحقق من المدخلات
            if (request.UserId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف المستخدم مطلوب");
            if (request.RoleId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الدور مطلوب");

            // التحقق من الصلاحيات (مسؤول عام فقط)
            if (_currentUserService.Role != "Admin")
                return ResultDto<bool>.Failed("غير مصرح لك بتخصيص دور");

            // التحقق من الوجود
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return ResultDto<bool>.Failed("المستخدم غير موجود");
            var role = await _roleRepository.GetRoleByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return ResultDto<bool>.Failed("الدور غير موجود");

            // الحصول على الأدوار الحالية
            var assignedRoles = await _userRepository.GetUserRolesAsync(request.UserId, cancellationToken);
            
            // التحقق من عدم التكرار
            if (assignedRoles.Any(r => r.RoleId == request.RoleId))
            {
                // إذا كان الدور مسنداً بالفعل، نعتبر ذلك نجاحاً (idempotent operation)
                _logger.LogInformation("الدور {RoleId} مخصص بالفعل للمستخدم {UserId}، سيتم تجاهل هذه العملية", request.RoleId, request.UserId);
                return ResultDto<bool>.Succeeded(true, "الدور مخصص للمستخدم بالفعل");
            }

            // حذف جميع الأدوار السابقة (المستخدم يجب أن يكون له دور واحد فقط)
            foreach (var oldRole in assignedRoles)
            {
                _logger.LogInformation("حذف الدور القديم {OldRoleId} من المستخدم {UserId}", oldRole.RoleId, request.UserId);
                var removeResult = await _roleRepository.RemoveRoleFromUserAsync(request.UserId, oldRole.RoleId, cancellationToken);
                if (!removeResult)
                {
                    _logger.LogWarning("فشل حذف الدور القديم {OldRoleId} من المستخدم {UserId}", oldRole.RoleId, request.UserId);
                }
            }

            // تخصيص الدور الجديد
            var result = await _roleRepository.AssignRoleToUserAsync(request.UserId, request.RoleId, cancellationToken);
            if (!result)
                return ResultDto<bool>.Failed("فشل تخصيص الدور للمستخدم");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "AssignUserRole",
                $"تم تخصيص الدور {request.RoleId} للمستخدم {request.UserId}",
                request.UserId,
                "UserRole",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تخصيص الدور بنجاح: UserId={UserId}, RoleId={RoleId}", request.UserId, request.RoleId);
            return ResultDto<bool>.Succeeded(true, "تم تخصيص الدور للمستخدم بنجاح");
        }
    }
} 