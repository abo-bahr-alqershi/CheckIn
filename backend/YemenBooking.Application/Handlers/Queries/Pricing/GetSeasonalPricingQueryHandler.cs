using MediatR;
using AutoMapper;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.CP.Pricing;

namespace YemenBooking.Application.Handlers.Queries.Pricing;

public class GetSeasonalPricingQueryHandler : IRequestHandler<GetSeasonalPricingQuery, ResultDto<SeasonalPricingResponse>>
{
    private readonly IPricingRuleRepository _pricingRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSeasonalPricingQueryHandler> _logger;

    public GetSeasonalPricingQueryHandler(
        IPricingRuleRepository pricingRepository,
        IUnitRepository unitRepository,
        IMapper mapper,
        ILogger<GetSeasonalPricingQueryHandler> logger)
    {
        _pricingRepository = pricingRepository;
        _unitRepository = unitRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultDto<SeasonalPricingResponse>> Handle(GetSeasonalPricingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var unit = await _unitRepository.GetByIdAsync(request.UnitId);
            if (unit == null)
                return ResultDto<SeasonalPricingResponse>.Failure("الوحدة غير موجودة");

            // تحديد الفترة الزمنية للبحث
            DateTime startDate, endDate;

            if (request.Year.HasValue)
            {
                startDate = new DateTime(request.Year.Value, 1, 1);
                endDate = new DateTime(request.Year.Value, 12, 31, 23, 59, 59);
            }
            else
            {
                // البحث عن المواسم للسنة الحالية والقادمة
                startDate = DateTime.Now.Date;
                endDate = startDate.AddYears(1);
            }

            // جلب قواعد التسعير الموسمية
            var pricingRules = await _pricingRepository.GetByDateRangeAsync(
                request.UnitId,
                startDate,
                endDate);

            // فلترة القواعد الموسمية فقط
            var seasonalRules = pricingRules
                .Where(r => r.PriceType == "Seasonal" ||
                           r.PriceType == "Peak" ||
                           r.PriceType == "Off-Peak" ||
                           r.PriceType == "Holiday" ||
                           r.PriceType == "Special")
                .OrderBy(r => r.StartDate)
                .ThenBy(r => r.PricingTier);

            // تجميع القواعد حسب الاسم/الوصف لتحديد المواسم
            var groupedSeasons = new List<QuerySeasonalPricingDto>();
            var currentDate = DateTime.Now.Date;

            foreach (var rule in seasonalRules)
            {
                var isActive = rule.StartDate <= currentDate && rule.EndDate >= currentDate;
                var isExpired = rule.EndDate < currentDate;

                // تضمين المواسم المنتهية إذا طُلب ذلك
                if (isExpired && !request.IncludeExpired)
                    continue;

                var seasonDto = new QuerySeasonalPricingDto
                {
                    Id = rule.Id,
                    Name = rule.Description ?? $"موسم {rule.PriceType}",
                    Type = rule.PriceType,
                    StartDate = rule.StartDate,
                    EndDate = rule.EndDate,
                    Price = rule.PriceAmount,
                    PercentageChange = rule.PercentageChange,
                    Currency = rule.Currency,
                    PricingTier = rule.PricingTier,
                    Priority = int.TryParse(rule.PricingTier, out var priority) ? priority : 1,
                    Description = rule.Description,
                    IsActive = isActive,
                    IsRecurring = false, // يمكن تحديدها من حقل إضافي
                    DaysCount = (rule.EndDate - rule.StartDate).Days + 1,
                    TotalRevenuePotential = rule.PriceAmount * ((rule.EndDate - rule.StartDate).Days + 1)
                };

                groupedSeasons.Add(seasonDto);
            }

            // حساب الإحصائيات
            var statistics = new SeasonalPricingStatsDto
            {
                TotalSeasons = groupedSeasons.Count,
                ActiveSeasons = groupedSeasons.Count(s => s.IsActive),
                UpcomingSeasons = groupedSeasons.Count(s => s.StartDate > currentDate),
                ExpiredSeasons = groupedSeasons.Count(s => s.EndDate < currentDate),
                AverageSeasonalPrice = groupedSeasons.Any() ? groupedSeasons.Average(s => s.Price) : 0,
                MaxSeasonalPrice = groupedSeasons.Any() ? groupedSeasons.Max(s => s.Price) : 0,
                MinSeasonalPrice = groupedSeasons.Any() ? groupedSeasons.Min(s => s.Price) : 0,
                TotalDaysCovered = groupedSeasons.Sum(s => s.DaysCount)
            };

            var response = new SeasonalPricingResponse
            {
                UnitId = unit.Id,
                UnitName = unit.Name,
                Seasons = groupedSeasons,
                Statistics = statistics
            };

            return ResultDto<SeasonalPricingResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"خطأ في جلب التسعير الموسمي للوحدة {request.UnitId}");
            return ResultDto<SeasonalPricingResponse>.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}