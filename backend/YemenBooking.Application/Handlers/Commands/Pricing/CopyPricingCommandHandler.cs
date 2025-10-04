using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.CP.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Pricing;

public class CopyPricingCommandHandler : IRequestHandler<CopyPricingCommand, ResultDto>
{
    private readonly IPricingRuleRepository _pricingRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<CopyPricingCommandHandler> _logger;

    public CopyPricingCommandHandler(
        IPricingRuleRepository pricingRepository,
        IUnitRepository unitRepository,
        ILogger<CopyPricingCommandHandler> logger)
    {
        _pricingRepository = pricingRepository;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    public async Task<ResultDto> Handle(CopyPricingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // التحقق من صحة التواريخ
            if (request.SourceStartDate >= request.SourceEndDate)
                return ResultDto.Failure("تاريخ بداية المصدر يجب أن يكون قبل تاريخ النهاية");

            if (request.RepeatCount < 1 || request.RepeatCount > 365)
                return ResultDto.Failure("عدد التكرار يجب أن يكون بين 1 و 365");

            // جلب قواعد التسعير من الفترة المصدر
            var sourceRules = await _pricingRepository.GetByDateRangeAsync(
                request.UnitId,
                request.SourceStartDate,
                request.SourceEndDate);

            if (!sourceRules.Any())
                return ResultDto.Failure("لا توجد قواعد تسعير في الفترة المصدر المحددة");

            var newRules = new List<PricingRule>();
            var sourceDuration = (request.SourceEndDate - request.SourceStartDate).Days;

            for (int repeatIndex = 0; repeatIndex < request.RepeatCount; repeatIndex++)
            {
                // حساب بداية الفترة الجديدة لكل تكرار
                var currentTargetStart = request.TargetStartDate.AddDays((sourceDuration + 1) * repeatIndex);

                foreach (var sourceRule in sourceRules)
                {
                    // حساب الإزاحة النسبية للقاعدة من بداية الفترة المصدر
                    var dayOffsetStart = (sourceRule.StartDate - request.SourceStartDate).Days;
                    var dayOffsetEnd = (sourceRule.EndDate - request.SourceStartDate).Days;

                    var newStartDate = currentTargetStart.AddDays(dayOffsetStart);
                    var newEndDate = currentTargetStart.AddDays(dayOffsetEnd);

                    // حذف القواعد الموجودة إذا طُلب ذلك
                    if (request.OverwriteExisting)
                    {
                        await _pricingRepository.DeleteRangeAsync(
                            request.UnitId,
                            newStartDate,
                            newEndDate);
                    }

                    // حساب السعر الجديد مع التعديلات
                    decimal newPrice = sourceRule.PriceAmount;
                    decimal? newPercentageChange = sourceRule.PercentageChange;

                    switch (request.AdjustmentType?.ToLower())
                    {
                        case "fixed":
                            newPrice += request.AdjustmentValue;
                            break;
                        case "percentage":
                            newPrice += newPrice * (request.AdjustmentValue / 100);
                            if (newPercentageChange.HasValue)
                                newPercentageChange += request.AdjustmentValue;
                            else
                                newPercentageChange = request.AdjustmentValue;
                            break;
                    }

                    // تطبيق الحد الأدنى والأقصى إن وجد
                    if (sourceRule.MinPrice.HasValue && newPrice < sourceRule.MinPrice.Value)
                        newPrice = sourceRule.MinPrice.Value;
                    
                    if (sourceRule.MaxPrice.HasValue && newPrice > sourceRule.MaxPrice.Value)
                        newPrice = sourceRule.MaxPrice.Value;

                    var newRule = new PricingRule
                    {
                        Id = Guid.NewGuid(),
                        UnitId = request.UnitId,
                        PriceType = sourceRule.PriceType,
                        StartDate = newStartDate,
                        EndDate = newEndDate,
                        StartTime = sourceRule.StartTime,
                        EndTime = sourceRule.EndTime,
                        PriceAmount = newPrice,
                        Currency = sourceRule.Currency,
                        PricingTier = sourceRule.PricingTier,
                        PercentageChange = newPercentageChange,
                        MinPrice = sourceRule.MinPrice,
                        MaxPrice = sourceRule.MaxPrice,
                        Description = $"منسوخ من {sourceRule.StartDate:yyyy-MM-dd} - {sourceRule.Description}",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty // يجب تعيينه من السياق
                    };

                    newRules.Add(newRule);
                }
            }

            // حفظ جميع القواعد الجديدة
            await _pricingRepository.BulkCreateAsync(newRules);

            _logger.LogInformation($"تم نسخ {newRules.Count} قاعدة تسعير للوحدة {request.UnitId}");

            return ResultDto.Ok($"تم نسخ التسعير بنجاح ({newRules.Count} قاعدة)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"خطأ في نسخ التسعير للوحدة {request.UnitId}");
            return ResultDto.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}