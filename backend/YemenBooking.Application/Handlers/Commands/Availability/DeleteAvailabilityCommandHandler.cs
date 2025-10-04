using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.CP.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Availability;

public class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand, ResultDto>
{
    private readonly IUnitAvailabilityRepository _availabilityRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<DeleteAvailabilityCommandHandler> _logger;

    public DeleteAvailabilityCommandHandler(
        IUnitAvailabilityRepository availabilityRepository,
        IBookingRepository bookingRepository,
        ILogger<DeleteAvailabilityCommandHandler> logger)
    {
        _availabilityRepository = availabilityRepository;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<ResultDto> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // حذف سجل محدد بالمعرف
            if (request.AvailabilityId.HasValue)
            {
                var availability = await _availabilityRepository.GetByIdAsync(request.AvailabilityId.Value);
                if (availability == null)
                    return ResultDto.Failure("سجل الإتاحة غير موجود");

                if (availability.UnitId != request.UnitId)
                    return ResultDto.Failure("سجل الإتاحة لا ينتمي للوحدة المحددة");

                // التحقق من وجود حجز مرتبط
                if (availability.BookingId.HasValue && !request.ForceDelete)
                {
                    var booking = await _bookingRepository.GetByIdAsync(availability.BookingId.Value);
                    if (booking != null && booking.Status == BookingStatus.Confirmed)
                        return ResultDto.Failure("لا يمكن حذف سجل إتاحة مرتبط بحجز مؤكد");
                }

                availability.IsDeleted = true;
                availability.DeletedAt = DateTime.UtcNow;
                availability.DeletedBy = Guid.Empty; // يجب تعيينه من السياق

                await _availabilityRepository.UpdateAsync(availability);
                await _availabilityRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"تم حذف سجل الإتاحة {request.AvailabilityId} للوحدة {request.UnitId}");
                
                return ResultDto.Ok("تم حذف سجل الإتاحة بنجاح");
            }

            // حذف بالفترة الزمنية
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.StartDate.Value >= request.EndDate.Value)
                    return ResultDto.Failure("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");

                // جلب السجلات في الفترة المحددة
                var availabilities = await _availabilityRepository.GetByDateRangeAsync(
                    request.UnitId,
                    request.StartDate.Value,
                    request.EndDate.Value);

                // التحقق من وجود حجوزات مرتبطة
                if (!request.ForceDelete)
                {
                    var hasBookings = availabilities.Any(a => 
                        a.BookingId.HasValue && 
                        a.Status == "Booked");
                    
                    if (hasBookings)
                        return ResultDto.Failure("توجد حجوزات مؤكدة في الفترة المحددة. استخدم خيار الحذف القسري إذا كنت متأكداً");
                }

                // حذف السجلات
                await _availabilityRepository.DeleteRangeAsync(
                    request.UnitId,
                    request.StartDate.Value,
                    request.EndDate.Value);
                // DeleteRangeAsync يقوم بالحفظ داخلياً

                _logger.LogInformation($"تم حذف سجلات الإتاحة للوحدة {request.UnitId} من {request.StartDate:yyyy-MM-dd} إلى {request.EndDate:yyyy-MM-dd}");
                
                return ResultDto.Ok($"تم حذف سجلات الإتاحة في الفترة المحددة");
            }

            return ResultDto.Failure("يجب تحديد معرف السجل أو الفترة الزمنية للحذف");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"خطأ في حذف سجلات الإتاحة للوحدة {request.UnitId}");
            return ResultDto.Failure($"حدث خطأ: {ex.Message}");
        }
    }
}