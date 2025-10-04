using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.Queries.CP.Pricing;

namespace YemenBooking.Application.Handlers.Queries.Pricing;

public class GetPricingBreakdownQueryHandler : IRequestHandler<GetPricingBreakdownQuery, ResultDto<PricingBreakdownDto>>
{
    private readonly IPricingService _pricingService;

    public GetPricingBreakdownQueryHandler(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public async Task<ResultDto<PricingBreakdownDto>> Handle(GetPricingBreakdownQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var breakdown = await _pricingService.GetPricingBreakdownAsync(
                request.UnitId,
                request.CheckIn,
                request.CheckOut);

            return ResultDto<PricingBreakdownDto>.Ok(breakdown);
        }
        catch (Exception ex)
        {
            return ResultDto<PricingBreakdownDto>.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}