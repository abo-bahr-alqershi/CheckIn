using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Services;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Services
{
    /// <summary>
    /// معالج استعلام الحصول على خدمات الكيان
    /// Query handler for GetPropertyServicesQuery
    /// </summary>
    public class GetPropertyServicesQueryHandler : IRequestHandler<GetPropertyServicesQuery, ResultDto<IEnumerable<ServiceDto>>>
    {
        private readonly IPropertyServiceRepository _serviceRepository;
        private readonly ILogger<GetPropertyServicesQueryHandler> _logger;

        public GetPropertyServicesQueryHandler(
            IPropertyServiceRepository serviceRepository,
            ILogger<GetPropertyServicesQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<ServiceDto>>> Handle(GetPropertyServicesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام خدمات الكيان: {PropertyId}", request.PropertyId);

            var services = await _serviceRepository.GetPropertyServicesAsync(request.PropertyId, cancellationToken);

            var dtos = services.Select(s => new ServiceDto
            {
                Id = s.Id,
                PropertyId = s.PropertyId,
                PropertyName = s.Property?.Name ?? string.Empty,
                Name = s.Name,
                Price = new MoneyDto
                {
                    Amount = s.Price.Amount,
                    Currency = s.Price.Currency
                },
                PricingModel = s.PricingModel
            }).ToList();

            return ResultDto<IEnumerable<ServiceDto>>.Ok(dtos, "تم جلب خدمات الكيان بنجاح");
        }
    }
} 