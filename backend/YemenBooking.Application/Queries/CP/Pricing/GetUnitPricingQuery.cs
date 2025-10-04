using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Queries.CP.Pricing;

public class GetUnitPricingQuery : IRequest<ResultDto<UnitPricingDto>>
{
    public Guid UnitId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

public class UnitPricingDto
{
    public Guid UnitId { get; set; }
    public string UnitName { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; }
    public Dictionary<DateTime, PricingDayDto> Calendar { get; set; }
    public List<PricingRuleDto> Rules { get; set; }
    public PricingStatsDto Stats { get; set; }
}

public class PricingDayDto
{
    public decimal Price { get; set; }
    public string PriceType { get; set; }
    public string ColorCode { get; set; }
    public decimal? PercentageChange { get; set; }
    public string PricingTier { get; set; }
}

public class PricingRuleDto
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public string PriceType { get; set; }
    public string Description { get; set; }
}

public class PricingStatsDto
{
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int DaysWithSpecialPricing { get; set; }
    public decimal PotentialRevenue { get; set; }
}

public class GetUnitPricingQueryHandler : IRequestHandler<GetUnitPricingQuery, ResultDto<UnitPricingDto>>
{
    private readonly IPricingRuleRepository _pricingRepository;
    private readonly IUnitRepository _unitRepository;

    public GetUnitPricingQueryHandler(
        IPricingRuleRepository pricingRepository,
        IUnitRepository unitRepository)
    {
        _pricingRepository = pricingRepository;
        _unitRepository = unitRepository;
    }

    public async Task<ResultDto<UnitPricingDto>> Handle(GetUnitPricingQuery request, CancellationToken cancellationToken)
    {
        var unit = await _unitRepository.GetByIdAsync(request.UnitId);
        if (unit == null)
            return ResultDto<UnitPricingDto>.Failure("الوحدة غير موجودة");

        var calendarPrices = await _pricingRepository.GetPricingCalendarAsync(
            request.UnitId,
            request.Year,
            request.Month);

        var startOfMonth = new DateTime(request.Year, request.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var rules = await _pricingRepository.GetByDateRangeAsync(
            request.UnitId,
            startOfMonth,
            endOfMonth);

        var basePrice = unit.BasePrice.Amount;

        // Build calendar entries including rule-based priceType and tier color
        var calendar = new Dictionary<DateTime, PricingDayDto>();
        for (var date = startOfMonth; date <= endOfMonth; date = date.AddDays(1))
        {
            var price = calendarPrices.TryGetValue(date, out var p) ? p : basePrice;
            var ruleForDay = rules
                .Where(r => r.StartDate.Date <= date && r.EndDate.Date >= date)
                .OrderBy(r => GetTierPriority(r.PricingTier))
                .FirstOrDefault();

            var priceType = ruleForDay != null ? NormalizePriceType(ruleForDay.PriceType) : "base";
            var colorCode = ruleForDay != null
                ? GetTierColor(ruleForDay.PricingTier)
                : GetPriceColorCode(price, basePrice);

            calendar[date] = new PricingDayDto
            {
                Price = price,
                PriceType = priceType,
                ColorCode = colorCode,
                PercentageChange = CalculatePercentageChange(price, basePrice),
                PricingTier = NormalizeTier(ruleForDay?.PricingTier, price, basePrice)
            };
        }

        var dto = new UnitPricingDto
        {
            UnitId = unit.Id,
            UnitName = unit.Name,
            BasePrice = basePrice,
            Currency = unit.BasePrice.Currency,
            Calendar = calendar,
            Rules = rules.Select(r => new PricingRuleDto
            {
                Id = r.Id,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Price = r.PriceAmount,
                PriceType = r.PriceType,
                Description = r.Description
            }).ToList(),
            Stats = CalculatePricingStats(calendarPrices.Values.ToList(), basePrice)
        };

        return ResultDto<UnitPricingDto>.Ok(dto);
    }

    private string NormalizePriceType(string? priceType)
    {
        if (string.IsNullOrWhiteSpace(priceType)) return "custom";
        var t = priceType.Trim().ToLowerInvariant();
        return t switch
        {
            "base" => "base",
            "weekend" => "weekend",
            "seasonal" => "seasonal",
            "holiday" => "holiday",
            "special_event" => "special_event",
            "specialevent" => "special_event",
            _ => "custom"
        };
    }

    private string GetPriceColorCode(decimal price, decimal basePrice)
    {
        var percentage = ((price - basePrice) / basePrice) * 100;
        
        if (percentage > 20) return "#DC2626";      // Dark Red
        if (percentage > 10) return "#F59E0B";      // Orange
        if (percentage > 0) return "#FBBF24";       // Yellow
        if (percentage == 0) return "#10B981";      // Green
        if (percentage > -10) return "#60A5FA";     // Light Blue
        return "#3B82F6";                           // Blue
    }

    private int GetTierPriority(string? tier)
    {
        var t = (tier ?? string.Empty).Trim().ToLowerInvariant();
        return t switch
        {
            "peak" => 1,
            "high" => 2,
            "normal" => 3,
            "discount" => 4,
            _ => 5
        };
    }

    private string GetTierColor(string? tier)
    {
        var t = (tier ?? string.Empty).Trim().ToLowerInvariant();
        return t switch
        {
            "peak" => "#DC2626",      // red
            "high" => "#F59E0B",      // orange
            "discount" => "#10B981",  // green
            "normal" => "#3B82F6",    // blue
            _ => "#8B5CF6"              // purple for custom
        };
    }

    private string NormalizeTier(string? tier, decimal price, decimal basePrice)
    {
        if (!string.IsNullOrWhiteSpace(tier))
        {
            var t = tier!.Trim().ToLowerInvariant();
            if (t is "normal" or "high" or "peak" or "discount" or "custom")
                return t;
            // if tier is numeric/string code, derive from price delta
        }

        var pct = basePrice == 0 ? 0 : ((price - basePrice) / basePrice) * 100m;
        if (pct > 20) return "peak";
        if (pct > 5) return "high";
        if (pct < 0) return "discount";
        return "normal";
    }

    private decimal? CalculatePercentageChange(decimal price, decimal basePrice)
    {
        if (basePrice == 0) return null;
        return ((price - basePrice) / basePrice) * 100;
    }

    private PricingStatsDto CalculatePricingStats(List<decimal> prices, decimal basePrice)
    {
        if (!prices.Any())
            return new PricingStatsDto();

        return new PricingStatsDto
        {
            AveragePrice = prices.Average(),
            MinPrice = prices.Min(),
            MaxPrice = prices.Max(),
            DaysWithSpecialPricing = prices.Count(p => p != basePrice),
            PotentialRevenue = prices.Sum()
        };
    }
}