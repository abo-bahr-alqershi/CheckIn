using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Commands.CP.Pricing;

public class ApplySeasonalPricingCommand : IRequest<ResultDto>
{
    public Guid UnitId { get; set; }
    public List<SeasonDto> Seasons { get; set; }
    public string Currency { get; set; }
}

public class SeasonDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public decimal? PercentageChange { get; set; }
    public int Priority { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Description { get; set; }
}
