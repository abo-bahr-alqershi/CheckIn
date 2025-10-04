// namespace YemenBooking.Application.Interfaces.Services;

// /// <summary>
// /// واجهة خدمة التوفر
// /// Availability service interface
// /// </summary>
// public interface IAvailabilityService
// {
//     /// <summary>
//     /// التحقق من التوفر
//     /// Check availability
//     /// </summary>
//     Task<bool> CheckAvailabilityAsync(
//         Guid unitId,
//         DateTime checkIn,
//         DateTime checkOut,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// التحقق من توفر متعدد الوحدات
//     /// Check availability for multiple units
//     /// </summary>
//     Task<IDictionary<Guid, bool>> CheckMultipleUnitsAvailabilityAsync(
//         IEnumerable<Guid> unitIds,
//         DateTime checkIn,
//         DateTime checkOut,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// الحصول على الوحدات المتاحة في الكيان
//     /// Get available units in property
//     /// </summary>
//     Task<IEnumerable<Guid>> GetAvailableUnitsInPropertyAsync(
//         Guid propertyId,
//         DateTime checkIn,
//         DateTime checkOut,
//         int guestCount,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// الحصول على فترات التوفر للوحدة
//     /// Get unit availability periods
//     /// </summary>
//     Task<IEnumerable<(DateTime Start, DateTime End, bool IsAvailable)>> GetUnitAvailabilityPeriodsAsync(
//         Guid unitId,
//         DateTime fromDate,
//         DateTime toDate,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// حجز فترة للوحدة
//     /// Block unit period
//     /// </summary>
//     Task<bool> BlockUnitPeriodAsync(
//         Guid unitId,
//         DateTime fromDate,
//         DateTime toDate,
//         string reason,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// إلغاء حجز فترة للوحدة
//     /// Unblock unit period
//     /// </summary>
//     Task<bool> UnblockUnitPeriodAsync(
//         Guid unitId,
//         DateTime fromDate,
//         DateTime toDate,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// حساب معدل الإشغال
//     /// Calculate occupancy rate
//     /// </summary>
//     Task<double> CalculateOccupancyRateAsync(
//         Guid unitId,
//         DateTime fromDate,
//         DateTime toDate,
//         CancellationToken cancellationToken = default);

//     /// <summary>
//     /// حساب معدل الإشغال للكيان
//     /// Calculate property occupancy rate
//     /// </summary>
//     Task<double> CalculatePropertyOccupancyRateAsync(
//         Guid propertyId,
//         DateTime fromDate,
//         DateTime toDate,
//         CancellationToken cancellationToken = default);
// }

using YemenBooking.Application.Commands.CP.Availability;

namespace YemenBooking.Application.Interfaces.Services;

public interface IAvailabilityService
{
    Task<bool> CheckAvailabilityAsync(Guid unitId, DateTime checkIn, DateTime checkOut);
    Task BlockForBookingAsync(Guid unitId, Guid bookingId, DateTime checkIn, DateTime checkOut);
    Task ReleaseBookingAsync(Guid bookingId);
    Task<Dictionary<DateTime, string>> GetMonthlyCalendarAsync(Guid unitId, int year, int month);
    Task ApplyBulkAvailabilityAsync(Guid unitId, List<AvailabilityPeriodDto> periods);
    
    /// <summary>
    /// الحصول على الوحدات المتاحة في الكيان
    /// Get available units in property
    /// </summary>
    Task<IEnumerable<Guid>> GetAvailableUnitsInPropertyAsync(
        Guid propertyId,
        DateTime checkIn,
        DateTime checkOut,
        int guestCount,
        CancellationToken cancellationToken = default);

}