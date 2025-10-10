using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Bookings;

/// <summary>
/// معالج أمر تسجيل الوصول
/// Check-in booking command handler
/// </summary>
public class CheckInBookingCommandHandler : IRequestHandler<CheckInBookingCommand, ResultDto<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly ILogger<CheckInBookingCommandHandler> _logger;

    public CheckInBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        IAuditService auditService,
        ILogger<CheckInBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _validationService = validationService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<ResultDto<bool>> Handle(CheckInBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate command
            var validation = await _validationService.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                return ResultDto<bool>.Failed(validation.Errors.Select(e => e.Message).ToArray());
            }

            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                return ResultDto<bool>.Failed("الحجز غير موجود");
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                return ResultDto<bool>.Failed("لا يمكن تسجيل الوصول إلا للحجز المؤكد");
            }

            // التحقق من تاريخ تنفيذ العملية: لا يمكن تسجيل الوصول قبل تاريخ الوصول أو بعد تاريخ المغادرة
            var todayUtc = DateTime.UtcNow.Date;
            if (todayUtc < booking.CheckIn.Date)
            {
                return ResultDto<bool>.Failed("لا يمكن تسجيل الوصول قبل تاريخ الوصول المحدد");
            }
            if (todayUtc > booking.CheckOut.Date)
            {
                return ResultDto<bool>.Failed("لا يمكن تسجيل الوصول لحجز منتهي");
            }

            booking.Status = BookingStatus.CheckedIn;
            booking.ActualCheckInDate = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Booking>().UpdateAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditService.LogAuditAsync(
                entityType: nameof(Booking),
                entityId: booking.Id,
                action: YemenBooking.Core.Entities.AuditAction.UPDATE,
                oldValues: null,
                newValues: System.Text.Json.JsonSerializer.Serialize(new { Status = booking.Status.ToString(), ActualCheckInDate = booking.ActualCheckInDate }),
                performedBy: _currentUserService.UserId,
                notes: $"تم تسجيل الوصول بواسطة {_currentUserService.Username} (ID={_currentUserService.UserId})",
                cancellationToken: cancellationToken);

            return ResultDto<bool>.Succeeded(true, "تم تسجيل الوصول بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تسجيل الوصول للحجز {BookingId}", request.BookingId);
            return ResultDto<bool>.Failed("حدث خطأ أثناء تسجيل الوصول");
        }
    }
}

