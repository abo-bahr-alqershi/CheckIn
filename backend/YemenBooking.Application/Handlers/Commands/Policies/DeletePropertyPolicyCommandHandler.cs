using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Policies;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Commands.Policies
{
    /// <summary>
    /// معالج أمر حذف سياسة كيان
    /// </summary>
    public class DeletePropertyPolicyCommandHandler : IRequestHandler<DeletePropertyPolicyCommand, ResultDto<bool>>
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeletePropertyPolicyCommandHandler> _logger;

        public DeletePropertyPolicyCommandHandler(
            IPolicyRepository policyRepository,
            IPropertyRepository propertyRepository,
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeletePropertyPolicyCommandHandler> logger)
        {
            _policyRepository = policyRepository;
            _propertyRepository = propertyRepository;
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeletePropertyPolicyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء حذف السياسة: PolicyId={PolicyId}", request.PolicyId);

            if (request.PolicyId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف السياسة مطلوب");

            var policy = await _policyRepository.GetPolicyByIdAsync(request.PolicyId, cancellationToken);
            if (policy == null)
                return ResultDto<bool>.Failed("السياسة غير موجودة");

            var property = await _propertyRepository.GetPropertyByIdAsync(policy.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان المرتبط بالسياسة غير موجود");

            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بحذف هذه السياسة");

            var bookings = await _bookingRepository.GetBookingsByPropertyAsync(property.Id, null, null, cancellationToken);
            if (bookings.Any(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                return ResultDto<bool>.Failed("لا يمكن حذف السياسة لأن هناك حجوزات نشطة");

            var success = await _policyRepository.DeletePolicyAsync(request.PolicyId, cancellationToken);
            if (!success)
                return ResultDto<bool>.Failed("فشل حذف السياسة");

            await _auditService.LogBusinessOperationAsync(
                "DeletePropertyPolicy",
                $"تم حذف السياسة {request.PolicyId}",
                request.PolicyId,
                "PropertyPolicy",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل حذف السياسة: PolicyId={PolicyId}", request.PolicyId);
            return ResultDto<bool>.Succeeded(true, "تم حذف السياسة بنجاح");
        }
    }
} 