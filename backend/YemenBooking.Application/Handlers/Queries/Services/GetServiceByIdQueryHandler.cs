using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Services;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Services
{
    /// <summary>
    /// معالج استعلام الحصول على تفاصيل الخدمة بواسطة المعرف
    /// Query handler for GetServiceByIdQuery
    /// </summary>
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ResultDto<ServiceDetailsDto>>
    {
        private readonly IPropertyServiceRepository _serviceRepository;
        private readonly ILogger<GetServiceByIdQueryHandler> _logger;

        public GetServiceByIdQueryHandler(
            IPropertyServiceRepository serviceRepository,
            ILogger<GetServiceByIdQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<ResultDto<ServiceDetailsDto>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام تفاصيل الخدمة: {ServiceId}", request.ServiceId);

            var service = await _serviceRepository.GetQueryable()
                .AsNoTracking()
                .Include(s => s.Property)
                .FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken);

            if (service == null)
            {
                return ResultDto<ServiceDetailsDto>.Failure("الخدمة غير موجودة");
            }

            var dto = new ServiceDetailsDto
            {
                Id = service.Id,
                PropertyId = service.PropertyId,
                PropertyName = service.Property?.Name ?? string.Empty,
                Name = service.Name,
                Icon = service.Icon,
                Price = new MoneyDto
                {
                    Amount = service.Price.Amount,
                    Currency = service.Price.Currency
                },
                PricingModel = service.PricingModel
            };

            return ResultDto<ServiceDetailsDto>.Ok(dto, "تم جلب بيانات الخدمة بنجاح");
        }
    }
} 