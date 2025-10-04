using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.CP.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Pricing;


public class DeletePricingRuleCommandHandler : IRequestHandler<DeletePricingRuleCommand, ResultDto>
{
    private readonly IPricingRuleRepository _pricingRepository;
    private readonly ILogger<DeletePricingRuleCommandHandler> _logger;

    public DeletePricingRuleCommandHandler(
        IPricingRuleRepository pricingRepository,
        ILogger<DeletePricingRuleCommandHandler> logger)
    {
        _pricingRepository = pricingRepository;
        _logger = logger;
    }

    public async Task<ResultDto> Handle(DeletePricingRuleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // حذف قاعدة محددة بالمعرف
            if (request.PricingRuleId.HasValue)
            {
                var rule = await _pricingRepository.GetByIdAsync(request.PricingRuleId.Value);
                if (rule == null)
                    return ResultDto.Failure("قاعدة التسعير غير موجودة");

                if (rule.UnitId != request.UnitId)
                    return ResultDto.Failure("قاعدة التسعير لا تنتمي للوحدة المحددة");

                rule.IsDeleted = true;
                rule.DeletedAt = DateTime.UtcNow;
                rule.DeletedBy = Guid.Empty; // يجب تعيينه من السياق

                await _pricingRepository.UpdateAsync(rule);
                await _pricingRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"تم حذف قاعدة التسعير {request.PricingRuleId} للوحدة {request.UnitId}");
                
                return ResultDto.Ok("تم حذف قاعدة التسعير بنجاح");
            }

            // حذف بالفترة الزمنية
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.StartDate.Value >= request.EndDate.Value)
                    return ResultDto.Failure("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");

                await _pricingRepository.DeleteRangeAsync(
                    request.UnitId,
                    request.StartDate.Value,
                    request.EndDate.Value);

                _logger.LogInformation($"تم حذف قواعد التسعير للوحدة {request.UnitId} من {request.StartDate:yyyy-MM-dd} إلى {request.EndDate:yyyy-MM-dd}");
                
                return ResultDto.Ok($"تم حذف قواعد التسعير في الفترة المحددة");
            }

            return ResultDto.Failure("يجب تحديد معرف القاعدة أو الفترة الزمنية للحذف");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"خطأ في حذف قواعد التسعير للوحدة {request.UnitId}");
            return ResultDto.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}