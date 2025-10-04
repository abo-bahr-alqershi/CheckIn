using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.CP.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Pricing;

public class BulkUpdatePricingCommandHandler : IRequestHandler<BulkUpdatePricingCommand, ResultDto>
{
    private readonly IPricingRuleRepository _pricingRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<BulkUpdatePricingCommandHandler> _logger;

    public BulkUpdatePricingCommandHandler(
        IPricingRuleRepository pricingRepository,
        IUnitRepository unitRepository,
        ILogger<BulkUpdatePricingCommandHandler> logger)
    {
        _pricingRepository = pricingRepository;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    public async Task<ResultDto> Handle(BulkUpdatePricingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // التحقق من وجود الوحدة
            var unit = await _unitRepository.GetByIdAsync(request.UnitId);
            if (unit == null)
                return ResultDto.Failure("الوحدة غير موجودة");

            // التحقق من صحة الفترات
            var hasInvalidPeriods = request.Periods.Any(p => 
                p.StartDate > p.EndDate || 
                p.Price < 0 ||
                (p.MinPrice.HasValue && p.MaxPrice.HasValue && p.MinPrice > p.MaxPrice));

            if (hasInvalidPeriods)
                return ResultDto.Failure("توجد فترات غير صالحة في البيانات المدخلة");

            var pricingRules = new List<PricingRule>();

            foreach (var period in request.Periods)
            {
                // Normalize and validate inputs
                var priceType = string.IsNullOrWhiteSpace(period.PriceType) ? "Custom" : period.PriceType.Trim();
                var tier = string.IsNullOrWhiteSpace(period.Tier) ? "1" : period.Tier.Trim();
                var currencyCode = string.IsNullOrWhiteSpace(period.Currency)
                    ? (unit.BasePrice?.Currency ?? "YER")
                    : period.Currency.Trim();
                currencyCode = currencyCode.ToUpperInvariant();

                // حذف القواعد الموجودة إذا طُلب ذلك
                if (period.OverwriteExisting || request.OverwriteExisting)
                {
                    await _pricingRepository.DeleteRangeAsync(
                        request.UnitId, 
                        period.StartDate, 
                        period.EndDate);
                }

                // حساب السعر النهائي
                decimal finalPrice = period.Price;
                
                // إذا كان هناك نسبة تغيير، احسب السعر بناءً على السعر الأساسي
                if (period.PercentageChange.HasValue && period.PercentageChange.Value != 0)
                {
                    finalPrice = unit.BasePrice.Amount * (1 + period.PercentageChange.Value / 100);
                }

                // تطبيق الحد الأدنى والأقصى
                if (period.MinPrice.HasValue && finalPrice < period.MinPrice.Value)
                    finalPrice = period.MinPrice.Value;
                    
                if (period.MaxPrice.HasValue && finalPrice > period.MaxPrice.Value)
                    finalPrice = period.MaxPrice.Value;

                var rule = new PricingRule
                {
                    Id = Guid.NewGuid(),
                    UnitId = request.UnitId,
                    PriceType = priceType,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    StartTime = period.StartTime,
                    EndTime = period.EndTime,
                    PriceAmount = finalPrice,
                    Currency = currencyCode,
                    PricingTier = tier, // تعيين قيمة افتراضية إذا كانت فارغة
                    PercentageChange = period.PercentageChange,
                    MinPrice = period.MinPrice,
                    MaxPrice = period.MaxPrice,
                    Description = period.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = null, // تركها null بدلاً من Guid.Empty
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = null
                };

                pricingRules.Add(rule);
            }

            // حفظ جميع القواعد دفعة واحدة
            await _pricingRepository.BulkCreateAsync(pricingRules);

            _logger.LogInformation($"تم تحديث {pricingRules.Count} قاعدة تسعير للوحدة {request.UnitId}");

            return ResultDto.Ok();
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message;
            var message = ex.Message + (inner != null ? $" | Inner: {inner}" : string.Empty);
            _logger.LogError(ex, $"خطأ في تحديث التسعير المجمع للوحدة {request.UnitId} :: {message}");
            return ResultDto.Failure($"حدث خطأ: {message}");
        }
    }
}