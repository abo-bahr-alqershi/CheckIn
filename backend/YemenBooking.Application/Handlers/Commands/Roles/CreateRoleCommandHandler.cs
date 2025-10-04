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
    /// معالج أمر إنشاء دور جديد
    /// </summary>
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ResultDto<Guid>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(
            IRoleRepository roleRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<CreateRoleCommandHandler> logger)
        {
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إنشاء دور جديد: Name={Name}", request.Name);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<Guid>.Failed("اسم الدور مطلوب");

            // التحقق من الصلاحيات (مسؤول عام)
            if (_currentUserService.Role != "Admin")
                return ResultDto<Guid>.Failed("غير مصرح لك بإنشاء أدوار");

            // التحقق من التكرار
            bool exists = await _roleRepository.ExistsAsync(r => r.Name.Trim().ToLower() == request.Name.Trim().ToLower(), cancellationToken);
            if (exists)
                return ResultDto<Guid>.Failed("يوجد دور بنفس الاسم");

            // إنشاء الكيان
            var role = new Role
            {
                Name = request.Name.Trim(),
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _roleRepository.CreateRoleAsync(role, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreateRole",
                $"تم إنشاء دور جديد {created.Id} باسم {created.Name}",
                created.Id,
                "Role",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إنشاء الدور بنجاح: RoleId={RoleId}", created.Id);
            return ResultDto<Guid>.Succeeded(created.Id, "تم إنشاء الدور بنجاح");
        }
    }
} 