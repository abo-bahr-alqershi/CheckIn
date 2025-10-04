// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using AutoMapper;
// using YemenBooking.Core.Interfaces.Repositories;
// using YemenBooking.Application.DTOs.Management;
// using YemenBooking.Application.DTOs;
// using YemenBooking.Core.Entities;
// using System.Threading;

// namespace YemenBooking.Api.Controllers
// {
//     /// <summary>
//     /// متحكم البيانات الشاملة للإدارة
//     /// Controller for management page data and statistics
//     /// </summary>
//     [ApiController]
//     [Route("api/management")]
//     public class ManagementController : ControllerBase
//     {
//         private readonly IUnitRepository _unitRepository;
//         private readonly IUnitAvailabilityRepository _availabilityRepository;
//         private readonly IPricingRuleRepository _pricingRepository;
//         private readonly IBookingRepository _bookingRepository;
//         private readonly IMapper _mapper;

//         public ManagementController(
//             IUnitRepository unitRepository,
//             IUnitAvailabilityRepository availabilityRepository,
//             IPricingRuleRepository pricingRepository,
//             IBookingRepository bookingRepository,
//             IMapper mapper)
//         {
//             _unitRepository = unitRepository;
//             _availabilityRepository = availabilityRepository;
//             _pricingRepository = pricingRepository;
//             _bookingRepository = bookingRepository;
//             _mapper = mapper;
//         }

//         /// <summary>
//         /// الحصول على بيانات صفحة الإدارة
//         /// Get management page data
//         /// </summary>
//         [HttpGet("page-data")]
//         public async Task<IActionResult> GetManagementPageData(
//             [FromQuery] Guid? propertyId,
//             [FromQuery] DateTime? startDate,
//             [FromQuery] DateTime? endDate,
//             CancellationToken cancellationToken = default)
//         {
//             // Determine calendar date range, default to next 30 days
//             var start = startDate ?? DateTime.UtcNow;
//             var end = endDate   ?? start.AddDays(30);
//             // 'now' used for current availability and upcoming bookings
//             var now = DateTime.UtcNow;
//             var units = propertyId.HasValue
//                 ? await _unitRepository.GetUnitsByPropertyAsync(propertyId.Value, cancellationToken)
//                 : await _unitRepository.GetAllAsync(cancellationToken);
//             var list = new List<UnitManagementDataDto>();
//             foreach (var unit in units)
//             {
//                 var baseUnit = new BaseUnitDto
//                 {
//                     UnitId = unit.Id.ToString(),
//                     PropertyId = unit.PropertyId.ToString(),
//                     UnitName = unit.Name,
//                     UnitType = (await _unitRepository.GetUnitTypeByIdAsync(unit.UnitTypeId, cancellationToken))?.Name ?? string.Empty,
//                     Capacity = unit.MaxCapacity,
//                     BasePrice = unit.BasePrice.Amount,
//                     IsActive = unit.IsActive
//                 };
//                 // Parallel fetch availability, pricing, bookings, calendar entries
//                 var isAvailTask = _availabilityRepository.IsUnitAvailableAsync(unit.Id, now, now.AddDays(1), cancellationToken);
//                 var pricingTask = _pricingRepository.GetPricingRulesByUnitAsync(unit.Id, now, now.AddDays(1), cancellationToken);
//                 var bookingTask = _bookingRepository.GetBookingsByUnitAsync(unit.Id, now, now.AddDays(7), cancellationToken);
//                 var availabilityTask = _availabilityRepository.GetUnitAvailabilityAsync(unit.Id, start, end, cancellationToken);
//                 // تحميل تجاوزات الإتاحة، الحجوزات، وقواعد التسعير ضمن النطاق لتحسين دقة التقويم
//                 var manualOverridesTask = _availabilityRepository.FindAsync(u => u.UnitId == unit.Id && u.StartDate < end && u.EndDate > start, cancellationToken: cancellationToken);
//                 var calendarBookingsTask = _bookingRepository.GetBookingsByUnitAsync(unit.Id, start, end, cancellationToken);
//                 var pricingRulesTaskFull = _pricingRepository.GetPricingRulesByUnitAsync(unit.Id, start, end, cancellationToken);
//                 await Task.WhenAll(isAvailTask, pricingTask, bookingTask, availabilityTask, manualOverridesTask, calendarBookingsTask, pricingRulesTaskFull);
//                 var manualOverridesList = manualOverridesTask.Result;
//                 var calendarBookingsList = calendarBookingsTask.Result;
//                 var pricingRulesList = pricingRulesTaskFull.Result;

//                 // إنشاء قائمة التقويم بالتفصيل لكل يوم ضمن النطاق مع دعم التسعير
//                 var calendar = new List<AvailabilityCalendarDto>();
//                 for (var dateLoop = start.Date; dateLoop <= end.Date; dateLoop = dateLoop.AddDays(1))
//                 {
//                     // معالجة التجاوز اليدوي أولاً: إذا كان السبب صيانة نعرض 'maintenance'، وإلا نعرض 'blocked'
//                     var overrideEntry = manualOverridesList.FirstOrDefault(m => m.StartDate.Date <= dateLoop && m.EndDate.Date >= dateLoop);
//                     if (overrideEntry != null)
//                     {
//                         // ضبط الحالة حسب حقل Status مباشرة
//                         string status = overrideEntry.Status.ToLowerInvariant();
//                         // إضافة مدخل التقويم مع السبب والتسعير
//                         var pricingEntry = pricingRulesList.FirstOrDefault(r => r.StartDate.Date <= dateLoop && r.EndDate.Date >= dateLoop);
//                         calendar.Add(new AvailabilityCalendarDto
//                         {
//                             Date = dateLoop,
//                             Status = status,
//                             Reason = overrideEntry.Reason,
//                             PricingTier = pricingEntry?.PricingTier,
//                             CurrentPrice = pricingEntry?.PriceAmount ?? baseUnit.BasePrice,
//                             Currency = pricingEntry?.Currency
//                         });
//                         continue;
//                     }
//                     // ثم معالجة الحجوزات
//                     var isBookedDay = calendarBookingsList.Any(b => b.CheckIn.Date <= dateLoop && b.CheckOut.Date > dateLoop);
//                     if (isBookedDay)
//                     {
//                         // إضافة مدخل التقويم لليوم المحجوز مع التسعير
//                         var pricingEntry = pricingRulesList.FirstOrDefault(r => r.StartDate.Date <= dateLoop && r.EndDate.Date >= dateLoop);
//                         calendar.Add(new AvailabilityCalendarDto
//                         {
//                             Date = dateLoop,
//                             Status = "booked",
//                             PricingTier = pricingEntry?.PricingTier,
//                             CurrentPrice = pricingEntry?.PriceAmount ?? baseUnit.BasePrice,
//                             Currency = pricingEntry?.Currency
//                         });
//                         continue;
//                     }
//                     // خلاف ذلك اليوم متاح
//                     // إضافة مدخل التقويم لليوم المتاح مع التسعير الافتراضي
//                     var defaultPricing = pricingRulesList.FirstOrDefault(r => r.StartDate.Date <= dateLoop && r.EndDate.Date >= dateLoop);
//                     calendar.Add(new AvailabilityCalendarDto
//                     {
//                         Date = dateLoop,
//                         Status = "available",
//                         PricingTier = defaultPricing?.PricingTier,
//                         CurrentPrice = defaultPricing?.PriceAmount ?? baseUnit.BasePrice,
//                         Currency = defaultPricing?.Currency
//                     });
//                 }
//                 var currentAvail = isAvailTask.Result ? "available" : "unavailable";
//                 var activePricing = pricingTask.Result.Select(r => _mapper.Map<PricingRuleDto>(r)).ToList();
//                 var upcoming = bookingTask.Result.Select(b => new UpcomingBookingDto
//                 {
//                     BookingId = b.Id,
//                     GuestName = b.User.Name,
//                     StartDate = b.CheckIn,
//                     EndDate = b.CheckOut,
//                     Status = b.Status.ToString(),
//                     TotalAmount = b.TotalPrice.Amount
//                 }).ToList();
//                 list.Add(new UnitManagementDataDto
//                 {
//                     Unit = baseUnit,
//                     CurrentAvailability = currentAvail,
//                     ActivePricingRules = activePricing,
//                     UpcomingBookings = upcoming,
//                     AvailabilityCalendar = calendar
//                 });
//             }
//             var totalUnits = list.Count;
//             var availableUnits = list.Count(x => x.CurrentAvailability == "available");
//             var summary = new ManagementSummaryDto
//             {
//                 TotalUnits = totalUnits,
//                 AvailableUnits = availableUnits,
//                 UnavailableUnits = totalUnits - availableUnits,
//                 MaintenanceUnits = 0,
//                 BookedUnits = 0,
//                 TotalRevenueToday = await _bookingRepository.GetTotalRevenueAsync(propertyId, now.Date, now.Date.AddDays(1), cancellationToken),
//                 AvgOccupancyRate = totalUnits > 0 ? (double)availableUnits / totalUnits : 0,
//                 PriceAlerts = new List<PriceAlertDto>()
//             };
//             var response = new ManagementPageResponseDto { Units = list, Summary = summary };
//             return Ok(new { data = response });
//         }

//         /// <summary>
//         /// الحصول على بيانات وحدة محددة
//         /// Get unit management data
//         /// </summary>
//         [HttpGet("unit/{unitId}")]
//         public async Task<IActionResult> GetUnitManagementData([FromRoute] Guid unitId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken = default)
//         {
//             var unit = await _unitRepository.GetUnitByIdAsync(unitId, cancellationToken);
//             if (unit == null) return NotFound();
//             var now = DateTime.UtcNow;
//             // Determine requested range or default to next 30 days
//             var start = startDate?.Date ?? now.Date;
//             var end = endDate?.Date ?? now.AddDays(30).Date;
//             // تحميل تجاوزات الإتاحة، الحجوزات، وقواعد التسعير لنطاق الـ30 يومًا للوحدة
//             var manualOverridesForUnit = await _availabilityRepository.FindAsync(u => u.UnitId == unitId && u.StartDate < end && u.EndDate > start, cancellationToken: cancellationToken);
//             var calendarBookingsForUnit = await _bookingRepository.GetBookingsByUnitAsync(unitId, start, end, cancellationToken);
//             var pricingRulesForUnit = await _pricingRepository.GetPricingRulesByUnitAsync(unitId, start, end, cancellationToken);
//             var detailedCalendar = new List<AvailabilityCalendarDto>();
//             for (var dateLoop = start; dateLoop <= end; dateLoop = dateLoop.AddDays(1))
//             {
//                 var overrideEntry = manualOverridesForUnit.FirstOrDefault(m => m.StartDate.Date <= dateLoop && m.EndDate.Date >= dateLoop);
//                 if (overrideEntry != null)
//                 {
//                     // ضبط الحالة حسب حقل Status مباشرة
//                     string status = overrideEntry.Status.ToLowerInvariant();
//                     // إضافة مدخل التقويم بتجاوز الحالة والتسعير
//                     var pricingEntry = pricingRulesForUnit.FirstOrDefault(r => r.StartDate.Date <= dateLoop && r.EndDate.Date >= dateLoop);
//                     detailedCalendar.Add(new AvailabilityCalendarDto
//                     {
//                         Date = dateLoop,
//                         Status = status,
//                         Reason = overrideEntry.Reason,
//                         PricingTier = pricingEntry?.PricingTier,
//                         CurrentPrice = pricingEntry?.PriceAmount ?? unit.BasePrice.Amount,
//                         Currency = pricingEntry?.Currency
//                     });
//                     continue;
//                 }
//                 var isBookedDay = calendarBookingsForUnit.Any(b => b.CheckIn.Date <= dateLoop && b.CheckOut.Date > dateLoop);
//                 if (isBookedDay)
//                 {
//                     // إضافة مدخل التقويم لليوم المحجوز مع التسعير
//                     var pricingEntry = pricingRulesForUnit.FirstOrDefault(r => r.StartDate.Date <= dateLoop && r.EndDate.Date >= dateLoop);
//                     detailedCalendar.Add(new AvailabilityCalendarDto
//                     {
//                         Date = dateLoop,
//                         Status = "booked",
//                         PricingTier = pricingEntry?.PricingTier,
//                         CurrentPrice = pricingEntry?.PriceAmount ?? unit.BasePrice.Amount,
//                         Currency = pricingEntry?.Currency
//                     });
//                     continue;
//                 }
//                 // إضافة مدخل التقويم لليوم المتاح مع التسعير الافتراضي
//                 var defaultPricingEntry = pricingRulesForUnit.FirstOrDefault(r => r.StartDate.Date <= dateLoop && r.EndDate.Date >= dateLoop);
//                 detailedCalendar.Add(new AvailabilityCalendarDto
//                 {
//                     Date = dateLoop,
//                     Status = "available",
//                     PricingTier = defaultPricingEntry?.PricingTier,
//                     CurrentPrice = defaultPricingEntry?.PriceAmount ?? unit.BasePrice.Amount,
//                     Currency = defaultPricingEntry?.Currency
//                 });
//             }
//             var dto = new UnitManagementDataDto {
//                 Unit = new BaseUnitDto
//                 {
//                     UnitId = unit.Id.ToString(),
//                     PropertyId = unit.PropertyId.ToString(),
//                     UnitName = unit.Name,
//                     UnitType = (await _unitRepository.GetUnitTypeByIdAsync(unit.UnitTypeId, cancellationToken))?.Name ?? string.Empty,
//                     Capacity = unit.MaxCapacity,
//                     BasePrice = unit.BasePrice.Amount,
//                     IsActive = unit.IsActive
//                 },
//                 CurrentAvailability = (await _availabilityRepository.IsUnitAvailableAsync(unit.Id, now, now.AddDays(1), cancellationToken)) ? "available" : "unavailable",
//                 ActivePricingRules = (await _pricingRepository.GetPricingRulesByUnitAsync(unit.Id, now, now.AddDays(1), cancellationToken)).Select(r => _mapper.Map<PricingRuleDto>(r)).ToList(),
//                 UpcomingBookings = (await _bookingRepository.GetBookingsByUnitAsync(unit.Id, now, now.AddDays(7), cancellationToken)).Select(b => new UpcomingBookingDto {
//                     BookingId = b.Id, GuestName = b.User.Name, StartDate = b.CheckIn, EndDate = b.CheckOut, Status = b.Status.ToString(), TotalAmount = b.TotalPrice.Amount }).ToList(),
//                 AvailabilityCalendar = detailedCalendar
//             };
//             return Ok(new { data = dto });
//         }

//         /// <summary>
//         /// الحصول على إحصائيات الإتاحة
//         /// Get availability statistics
//         /// </summary>
//         [HttpPost("statistics/availability")]
//         public async Task<IActionResult> GetAvailabilityStatistics([FromBody] StatisticsRequestDto request, CancellationToken cancellationToken = default)
//         {
//             var start = request.DateRange.Start;
//             var end = request.DateRange.End;
//             var dict = new Dictionary<DateTime, bool>();
//             foreach (var id in request.UnitIds)
//             {
//                 var res = await _availabilityRepository.GetUnitAvailabilityAsync(id, start, end, cancellationToken);
//                 foreach (var kv in res) dict[kv.Key] = dict.ContainsKey(kv.Key) ? dict[kv.Key] && kv.Value : kv.Value;
//             }
//             var dto = new AvailabilityStatisticsDto
//             {
//                 DateRange = request.DateRange,
//                 TotalDays = dict.Count,
//                 AvailableDays = dict.Values.Count(v => v),
//                 UnavailableDays = dict.Values.Count(v => !v),
//                 MaintenanceDays = 0,
//                 BookedDays = 0,
//                 OccupancyRate = dict.Count > 0 ? (double)dict.Values.Count(v => v) / dict.Count : 0,
//                 RevenueLostDueToUnavailability = 0,
//                 AverageDailyRate = 0,
//                 RevenuePerAvailableUnit = 0
//             };
//             return Ok(new { data = dto });
//         }

//         /// <summary>
//         /// الحصول على إحصائيات التسعير
//         /// Get pricing statistics
//         /// </summary>
//         [HttpPost("statistics/pricing")]
//         public async Task<IActionResult> GetPricingStatistics([FromBody] StatisticsRequestDto request, CancellationToken cancellationToken = default)
//         {
//             var start = request.DateRange.Start;
//             var end = request.DateRange.End;
//             var listRules = new List<PricingRule>();
//             foreach (var id in request.UnitIds)
//                 listRules.AddRange(await _pricingRepository.GetPricingRulesByUnitAsync(id, start, end, cancellationToken));
//             var prices = listRules.Select(r => r.PriceAmount).ToList();
//             var dto = new PricingStatisticsDto
//             {
//                 DateRange = request.DateRange,
//                 AveragePrice = prices.Any() ? prices.Average() : 0,
//                 MinPrice = prices.Any() ? prices.Min() : 0,
//                 MaxPrice = prices.Any() ? prices.Max() : 0,
//                 PriceVariance = prices.Any() ? prices.Select(p => (p - prices.Average()) * (p - prices.Average())).Average() : 0,
//                 SeasonalAdjustments = new List<SeasonalAdjustmentDto>(),
//                 CompetitorComparison = null
//             };
//             return Ok(new { data = dto });
//         }

//     }
// } 