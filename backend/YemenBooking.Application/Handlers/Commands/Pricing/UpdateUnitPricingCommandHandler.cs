using MediatR;
using YemenBooking.Application.Commands.CP.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.PricingRules.Commands;

public class UpdateUnitPricingCommandHandler : IRequestHandler<UpdateUnitPricingCommand, ResultDto>
{
    private readonly IPricingRuleRepository _pricingRepository;
    private readonly IUnitRepository _unitRepository;

    public UpdateUnitPricingCommandHandler(
        IPricingRuleRepository pricingRepository,
        IUnitRepository unitRepository)
    {
        _pricingRepository = pricingRepository;
        _unitRepository = unitRepository;
    }

    public async Task<ResultDto> Handle(UpdateUnitPricingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.EndDate <= request.StartDate)
                return ResultDto.Failure("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");

            // Ensure unit exists to avoid FK violations
            var unit = await _unitRepository.GetByIdAsync(request.UnitId, cancellationToken);
            if (unit == null)
                return ResultDto.Failure("الوحدة غير موجودة");

            if (request.OverwriteExisting)
            {
                await _pricingRepository.DeleteRangeAsync(request.UnitId, request.StartDate, request.EndDate);
            }

            // Build rule; rely on repository BulkCreateAsync to normalize currency and strings
            var rule = new PricingRule
            {
                Id = Guid.NewGuid(),
                UnitId = request.UnitId,
                PriceType = string.IsNullOrWhiteSpace(request.PriceType) ? "Custom" : request.PriceType.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PriceAmount = request.Price,
                Currency = (request.Currency ?? unit.BasePrice?.Currency ?? "YER")!,
                PricingTier = string.IsNullOrWhiteSpace(request.PricingTier) ? "1" : request.PricingTier.Trim(),
                PercentageChange = request.PercentageChange,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _pricingRepository.BulkCreateAsync(new[] { rule });
            return ResultDto.Ok();
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message;
            var msg = ex.Message + (inner != null ? $" | Inner: {inner}" : string.Empty);
            return ResultDto.Failure($"حدث خطأ: {msg}");
        }
    }
}