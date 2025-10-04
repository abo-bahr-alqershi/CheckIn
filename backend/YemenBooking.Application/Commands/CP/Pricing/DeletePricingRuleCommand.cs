using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Commands.CP.Pricing;

public class DeletePricingRuleCommand : IRequest<ResultDto>
{
    public Guid UnitId { get; set; }
    public Guid? PricingRuleId { get; set; } // حذف قاعدة محددة
    public DateTime? StartDate { get; set; } // حذف بالفترة
    public DateTime? EndDate { get; set; }
}
