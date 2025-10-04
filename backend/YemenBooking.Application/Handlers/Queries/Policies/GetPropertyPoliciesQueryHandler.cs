using System;
using System.Collections.Generic;
using System.Linq;
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
    /// معالج استعلام الحصول على سياسات الكيان
    /// Query handler for GetPropertyPoliciesQuery
    /// </summary>
    public class GetPropertyPoliciesQueryHandler : IRequestHandler<GetPropertyPoliciesQuery, ResultDto<IEnumerable<PolicyDto>>>
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly ILogger<GetPropertyPoliciesQueryHandler> _logger;

        public GetPropertyPoliciesQueryHandler(
            IPolicyRepository policyRepository,
            ILogger<GetPropertyPoliciesQueryHandler> logger)
        {
            _policyRepository = policyRepository;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<PolicyDto>>> Handle(GetPropertyPoliciesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام سياسات الكيان: {PropertyId}", request.PropertyId);

            if (request.PropertyId == Guid.Empty)
                throw new ValidationException(nameof(request.PropertyId), "معرف الكيان غير صالح");

            var policies = await _policyRepository.GetPropertyPoliciesAsync(request.PropertyId, cancellationToken);

            var dtos = policies.Select(p => new PolicyDto
            {
                Id = p.Id,
                PropertyId = p.PropertyId,
                PolicyType = p.Type,
                Description = p.Description,
                Rules = p.Rules
            }).ToList();

            return ResultDto<IEnumerable<PolicyDto>>.Ok(dtos, "تم جلب سياسات الكيان بنجاح");
        }
    }
} 