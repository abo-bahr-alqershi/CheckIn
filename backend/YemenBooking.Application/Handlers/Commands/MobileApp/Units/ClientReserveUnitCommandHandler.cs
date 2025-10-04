using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Units;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Units;

/// <summary>
/// معالج أمر حجز مؤقت لوحدة (تطبيق الجوال)
/// </summary>
public class ClientReserveUnitCommandHandler : IRequestHandler<ClientReserveUnitCommand, ResultDto<ClientUnitReservationResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientReserveUnitCommandHandler> _logger;

    public ClientReserveUnitCommandHandler(IUnitOfWork unitOfWork, ILogger<ClientReserveUnitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultDto<ClientUnitReservationResponse>> Handle(ClientReserveUnitCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("حجز مؤقت للوحدة {UnitId} من قبل المستخدم {UserId}", request.UnitId, request.UserId);

        // Basic validation & availability checks يمكن توسعتها لاحقاً
        var unitRepo = _unitOfWork.Repository<Core.Entities.Unit>();
        var unit = await unitRepo.GetByIdAsync(request.UnitId);
        if (unit == null)
            return ResultDto<ClientUnitReservationResponse>.Failed("الوحدة غير موجودة");

        // TODO: Check overlaps with existing bookings

        // إنشاء الحجز المؤقت (سيتم حفظه في جدول خاص أو في الذاكرة حسب التصميم)
        var reservation = new ClientUnitReservationResponse
        {
            ReservationId = Guid.NewGuid(),
            UnitId = unit.Id,
            UnitName = unit.Name,
            ExpiresAt = DateTime.UtcNow.AddMinutes(request.ReservationDurationMinutes),
            TotalPrice = 0, // TODO: حساب السعر الفعلي
            Currency = "YER",
            ReservationToken = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow
        };

        return ResultDto<ClientUnitReservationResponse>.Ok(reservation, "تم إنشاء الحجز المؤقت");
    }
}
