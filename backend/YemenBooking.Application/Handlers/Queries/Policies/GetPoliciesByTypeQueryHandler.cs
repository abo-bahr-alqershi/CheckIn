using System;
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
    /// معالج استعلام الحصول على السياسات حسب النوع
    /// Query handler for GetPoliciesByTypeQuery
    /// </summary>
    public class GetPoliciesByTypeQueryHandler : IRequestHandler<GetPoliciesByTypeQuery, PaginatedResult<PolicyDto>>
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly ILogger<GetPoliciesByTypeQueryHandler> _logger;

        public GetPoliciesByTypeQueryHandler(
            IPolicyRepository policyRepository,
            ILogger<GetPoliciesByTypeQueryHandler> logger)
        {
            _policyRepository = policyRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<PolicyDto>> Handle(GetPoliciesByTypeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام السياسات حسب النوع: {PolicyType}", request.PolicyType);

            if (string.IsNullOrWhiteSpace(request.PolicyType))
                throw new ValidationException(nameof(request.PolicyType), "نوع السياسة غير صالح");

            var policies = await _policyRepository.GetPoliciesByTypeAsync(request.PolicyType, cancellationToken);

            var dtos = policies.Select(p => new PolicyDto
            {
                Id = p.Id,
                PropertyId = p.PropertyId,
                PolicyType = p.Type,
                Description = p.Description,
                Rules = p.Rules
            }).ToList();

            var totalCount = dtos.Count;
            var items = dtos
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PaginatedResult<PolicyDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
} 