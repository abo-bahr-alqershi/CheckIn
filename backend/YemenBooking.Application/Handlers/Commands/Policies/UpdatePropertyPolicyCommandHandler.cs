using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Policies;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Policies
{
    /// <summary>
    /// معالج أمر تحديث سياسة الكيان
    /// </summary>
    public class UpdatePropertyPolicyCommandHandler : IRequestHandler<UpdatePropertyPolicyCommand, ResultDto<bool>>
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdatePropertyPolicyCommandHandler> _logger;

        public UpdatePropertyPolicyCommandHandler(
            IPolicyRepository policyRepository,
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdatePropertyPolicyCommandHandler> logger)
        {
            _policyRepository = policyRepository;
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdatePropertyPolicyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث السياسة: PolicyId={PolicyId}", request.PolicyId);

            // التحقق من صحة المدخلات
            if (request.PolicyId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف السياسة مطلوب");

            // التحقق من وجود السياسة
            var policy = await _policyRepository.GetPolicyByIdAsync(request.PolicyId, cancellationToken);
            if (policy == null)
                return ResultDto<bool>.Failed("السياسة غير موجودة");

            // التحقق من وجود الكيان المرتبط
            var property = await _propertyRepository.GetPropertyByIdAsync(policy.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان المرتبط بالسياسة غير موجود");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث هذه السياسة");

            // تنفيذ التحديث
            if (!string.IsNullOrWhiteSpace(request.PolicyType) && Enum.TryParse<PolicyType>(request.PolicyType, true, out var newType))
                policy.Type = newType;
            if (!string.IsNullOrWhiteSpace(request.Description))
                policy.Description = request.Description;
            if (!string.IsNullOrWhiteSpace(request.Rules))
                policy.Rules = request.Rules;
            policy.UpdatedBy = _currentUserService.UserId;
            policy.UpdatedAt = DateTime.UtcNow;
            await _policyRepository.UpdatePropertyPolicyAsync(policy, cancellationToken);

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdatePropertyPolicy",
                $"تم تحديث السياسة {request.PolicyId}",
                request.PolicyId,
                "PropertyPolicy",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تحديث السياسة: PolicyId={PolicyId}", request.PolicyId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث السياسة بنجاح");
        }
    }
} 