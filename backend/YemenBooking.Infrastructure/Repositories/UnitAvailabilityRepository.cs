// using System;
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using YemenBooking.Core.Entities;
// using YemenBooking.Core.Interfaces.Repositories;
// using YemenBooking.Infrastructure.Data.Context;

// namespace YemenBooking.Infrastructure.Repositories
// {
//     /// <summary>
//     /// تنفيذ مستودع توفر الوحدات
//     /// Unit availability repository implementation
//     /// </summary>
//     public class UnitAvailabilityRepository : BaseRepository<UnitAvailability>, IUnitAvailabilityRepository
//     {
//         public UnitAvailabilityRepository(YemenBookingDbContext context) : base(context) { }

//         public async Task<bool> UpdateAvailabilityAsync(Guid unitId, DateTime fromDate, DateTime toDate, bool isAvailable)
//         {
//             // تحديث حالة الإتاحة بناءً على الفترة المحددة
//             var availability = await GetByIdAsync(unitId, );
//             if (availability == null) return false;
//             // ضبط الحالة إلى available أو unavailable
//             availability.Status = isAvailable ? "available" : "unavailable";
//             _dbSet.Update(availability);
//             await _context.SaveChangesAsync();
//             return true;
//         }

//         public async Task<IDictionary<DateTime, bool>> GetUnitAvailabilityAsync(Guid unitId, DateTime fromDate, DateTime toDate)
//         {
//             var dict = new Dictionary<DateTime, bool>();
//             for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
//             {
//                 var overlapping = await _context.Bookings.AnyAsync(b => b.UnitId == unitId && b.CheckIn <= date && b.CheckOut > date, );
//                 dict[date] = !overlapping;
//             }
//             return dict;
//         }

//         public async Task<bool> IsUnitAvailableAsync(Guid unitId, DateTime checkIn, DateTime checkOut)
//         {
//             var overlapping = await _context.Bookings.AnyAsync(b => b.UnitId == unitId && b.CheckIn < checkOut && b.CheckOut > checkIn, );
//             return !overlapping;
//         }

//         public async Task<bool> BlockUnitPeriodAsync(Guid unitId, DateTime fromDate, DateTime toDate, string reason)
//         {
//             // حجز الوحدة (تعطيل التوفر) خلال الفترة المحددة
//             return await UpdateAvailabilityAsync(unitId, fromDate, toDate, false, );
//         }

//         public async Task<bool> UnblockUnitPeriodAsync(Guid unitId, DateTime fromDate, DateTime toDate)
//         {
//             // إلغاء حجز الوحدة (تفعيل التوفر) خلال الفترة المحددة
//             return await UpdateAvailabilityAsync(unitId, fromDate, toDate, true, );
//         }

//         public async Task<bool> HasOverlapAsync(Guid unitId, DateTime fromDate, DateTime toDate, Guid? excludeAvailabilityId = null)
//         {
//             // التحقق من تداخل الفترات في جدولة الإتاحة
//             var query = _dbSet.Where(a => a.UnitId == unitId &&
//                                          a.StartDate < toDate &&
//                                          a.EndDate > fromDate);

//             // استبعاد الإتاحة المحددة إذا تم توفير معرفها
//             if (excludeAvailabilityId.HasValue)
//             {
//                 query = query.Where(a => a.Id != excludeAvailabilityId.Value);
//             }

//             return await query.AnyAsync();
//         }
//     }
// } 

using Microsoft.EntityFrameworkCore;
using YemenBooking.Core.Entities;
using YemenBooking.Infrastructure.Data;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Core.Interfaces.Repositories;


namespace YemenBooking.Infrastructure.Repositories;

public class UnitAvailabilityRepository : BaseRepository<UnitAvailability>, IUnitAvailabilityRepository
{
    public UnitAvailabilityRepository(YemenBookingDbContext context) : base(context) { }


    public async Task<IEnumerable<UnitAvailability>> GetByUnitIdAsync(Guid unitId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.Where(ua => ua.UnitId == unitId && !ua.IsDeleted);

        if (startDate.HasValue)
            query = query.Where(ua => ua.EndDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ua => ua.StartDate <= endDate.Value);

        return await query
            .OrderBy(ua => ua.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<UnitAvailability>> GetByDateRangeAsync(Guid unitId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(ua => ua.UnitId == unitId
                && !ua.IsDeleted
                && ua.StartDate <= endDate
                && ua.EndDate >= startDate)
            .OrderBy(ua => ua.StartDate)
            .ToListAsync();
    }

    public async Task<bool> IsUnitAvailableAsync(Guid unitId, DateTime startDate, DateTime endDate)
    {
        var blockedPeriods = await _dbSet
            .Where(ua => ua.UnitId == unitId
                && !ua.IsDeleted
                && ua.Status != "Available"
                && ua.StartDate < endDate
                && ua.EndDate > startDate)
            .AnyAsync();

        return !blockedPeriods;
    }

    public async Task<IEnumerable<UnitAvailability>> GetBlockedPeriodsAsync(Guid unitId, int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        return await _dbSet
            .Where(ua => ua.UnitId == unitId
                && !ua.IsDeleted
                && ua.Status != "Available"
                && ua.StartDate <= endOfMonth
                && ua.EndDate >= startOfMonth)
            .OrderBy(ua => ua.StartDate)
            .ToListAsync();
    }

    public async Task BulkCreateAsync(IEnumerable<UnitAvailability> availabilities)
    {
        await _dbSet.AddRangeAsync(availabilities);
        await _context.SaveChangesAsync();
    }

    public async Task BulkUpdateAsync(IEnumerable<UnitAvailability> availabilities)
    {
        _dbSet.UpdateRange(availabilities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(Guid unitId, DateTime startDate, DateTime endDate)
    {
        var toDelete = await _dbSet
            .Where(ua => ua.UnitId == unitId
                && ua.StartDate >= startDate
                && ua.EndDate <= endDate)
            .ToListAsync();

        foreach (var item in toDelete)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Dictionary<DateTime, string>> GetAvailabilityCalendarAsync(Guid unitId, int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var availabilities = await GetByDateRangeAsync(unitId, startOfMonth, endOfMonth);
        var calendar = new Dictionary<DateTime, string>();

        for (var date = startOfMonth; date <= endOfMonth; date = date.AddDays(1))
        {
            var dayAvailability = availabilities.FirstOrDefault(a =>
                date >= a.StartDate.Date && date <= a.EndDate.Date);

            calendar[date] = dayAvailability?.Status ?? "Available";
        }

        return calendar;
    }
}