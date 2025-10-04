// namespace YemenBooking.Application.Interfaces.Services;

// /// <summary>
// /// واجهة خدمة التسعير
// /// Pricing service interface
// /// </summary>
// public interface IPricingService
// {
//     /// <summary>
//     /// حساب السعر
//     /// Calculate price
//     /// </summary>
//     Task<decimal> CalculatePriceAsync(
//         Guid unitId,
//         DateTime checkIn,
//         DateTime checkOut,
//         int guestCount,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// إعادة حساب السعر
//     /// Recalculate price
//     /// </summary>
//     Task<decimal> RecalculatePriceAsync(
//         Guid bookingId,
//         DateTime? newCheckIn = null,
//         DateTime? newCheckOut = null,
//         int? newGuestCount = null,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// حساب السعر الأساسي
//     /// Calculate base price
//     /// </summary>
//     Task<decimal> CalculateBasePriceAsync(
//         Guid unitId,
//         int nights,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// حساب الرسوم الإضافية
//     /// Calculate additional fees
//     /// </summary>
//     Task<decimal> CalculateAdditionalFeesAsync(
//         Guid unitId,
//         int guestCount,
//         IEnumerable<Guid>? serviceIds = null,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// حساب الخصومات
//     /// Calculate discounts
//     /// </summary>
//     Task<decimal> CalculateDiscountsAsync(
//         Guid unitId,
//         DateTime checkIn,
//         DateTime checkOut,
//         Guid? userId = null,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// حساب الضرائب
//     /// Calculate taxes
//     /// </summary>
//     Task<decimal> CalculateTaxesAsync(
//         decimal baseAmount,
//         Guid propertyId,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// الحصول على تفاصيل التسعير
//     /// Get pricing breakdown
//     /// </summary>
//     Task<object> GetPricingBreakdownAsync(
//         Guid unitId,
//         DateTime checkIn,
//         DateTime checkOut,
//         int guestCount,
//         CancellationToken cancellationToken = default);
// }
using YemenBooking.Application.Commands.CP.Pricing;
using YemenBooking.Application.Features.PricingRules.Commands;
using YemenBooking.Application.Queries.CP.Pricing;

namespace YemenBooking.Application.Interfaces.Services;

public interface IPricingService
{
    Task<decimal> CalculatePriceAsync(Guid unitId, DateTime checkIn, DateTime checkOut);
    Task<Dictionary<DateTime, decimal>> GetPricingCalendarAsync(Guid unitId, int year, int month);
    Task ApplySeasonalPricingAsync(Guid unitId, SeasonalPricingDto seasonalPricing);
    Task ApplyBulkPricingAsync(Guid unitId, List<PricingPeriodDto> periods);
    Task<PricingBreakdownDto> GetPricingBreakdownAsync(Guid unitId, DateTime checkIn, DateTime checkOut);
}