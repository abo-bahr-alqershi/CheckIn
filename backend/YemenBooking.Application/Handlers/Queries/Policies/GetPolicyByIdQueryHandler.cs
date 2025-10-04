using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Policies;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Policies
{
    /// <summary>
    /// معالج استعلام الحصول على سياسة محددة
    /// Query handler for GetPolicyByIdQuery
    /// </summary>
    public class GetPolicyByIdQueryHandler : IRequestHandler<GetPolicyByIdQuery, ResultDto<PolicyDetailsDto>>
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly ILogger<GetPolicyByIdQueryHandler> _logger;

        public GetPolicyByIdQueryHandler(
            IPolicyRepository policyRepository,
            ILogger<GetPolicyByIdQueryHandler> logger)
        {
            _policyRepository = policyRepository;
            _logger = logger;
        }

        public async Task<ResultDto<PolicyDetailsDto>> Handle(GetPolicyByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام السياسة: {PolicyId}", request.PolicyId);

            if (request.PolicyId == Guid.Empty)
                throw new ValidationException(nameof(request.PolicyId), "معرف السياسة غير صالح");

            var policy = await _policyRepository.GetPolicyByIdAsync(request.PolicyId, cancellationToken);
            if (policy == null)
                return ResultDto<PolicyDetailsDto>.Failure($"السياسة بالمعرف {request.PolicyId} غير موجودة");

            var dto = new PolicyDetailsDto
            {
                Id = policy.Id,
                PropertyId = policy.PropertyId,
                PolicyType = policy.Type,
                Description = policy.Description,
                Rules = policy.Rules
            };

            return ResultDto<PolicyDetailsDto>.Ok(dto, "تم جلب بيانات السياسة بنجاح");
        }
    }
} 