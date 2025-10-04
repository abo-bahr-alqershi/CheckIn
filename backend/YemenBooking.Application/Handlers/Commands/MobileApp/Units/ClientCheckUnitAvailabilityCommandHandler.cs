using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Commands.MobileApp.Units;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Units;

/// <summary>
/// معالج أمر فحص توفر الوحدة للعميل
/// Handler for client check unit availability command
/// </summary>
public class ClientCheckUnitAvailabilityCommandHandler : IRequestHandler<ClientCheckUnitAvailabilityCommand, ResultDto<ClientUnitAvailabilityResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientCheckUnitAvailabilityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// معالجة أمر فحص توفر الوحدة
    /// Handle check unit availability command
    /// </summary>
    /// <param name="request">الطلب</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<ClientUnitAvailabilityResponse>> Handle(ClientCheckUnitAvailabilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // التحقق من وجود الوحدة
            // Check if unit exists
            var unitRepo = _unitOfWork.Repository<Core.Entities.Unit>();
            var unit = await unitRepo.GetByIdAsync(request.UnitId);

            if (unit == null)
            {
                return ResultDto<ClientUnitAvailabilityResponse>.Failed("الوحدة غير موجودة", "UNIT_NOT_FOUND");
            }

            // فحص التوفر للفترة المطلوبة
            // Check availability for requested period
            var bookingRepo = _unitOfWork.Repository<Core.Entities.Booking>();
            var bookings = await bookingRepo.GetAllAsync();
            var conflictingBookings = bookings.Where(b => 
                b.UnitId == request.UnitId &&
                b.Status != BookingStatus.Cancelled &&
                !(request.CheckOutDate <= b.CheckIn || request.CheckInDate >= b.CheckOut)
            ).ToList();

            bool isAvailable = !conflictingBookings.Any();

            // حساب السعر للفترة
            // Calculate price for period
            decimal totalPrice = 0;
            var nights = (request.CheckOutDate - request.CheckInDate).Days;
            if (nights > 0)
            {
                totalPrice = unit.BasePrice.Amount * nights;
            }

            var response = new ClientUnitAvailabilityResponse
            {
                UnitId = request.UnitId,
                IsAvailable = isAvailable,
                TotalPrice = totalPrice,
                Currency = unit.BasePrice.Currency,
                PricePerNight = unit.BasePrice.Amount,
                NumberOfNights = nights,
                AdditionalFees = new List<ClientAdditionalFeeDto>(),
                TaxAmount = 0, // يمكن حسابها لاحقاً
                UnavailabilityReason = isAvailable ? null : "الوحدة محجوزة في هذه التواريخ",
                AlternativeDates = new List<ClientAlternativeDateDto>()
            };

            return ResultDto<ClientUnitAvailabilityResponse>.Ok(response, 
                isAvailable ? "الوحدة متاحة للحجز" : "الوحدة غير متاحة للفترة المطلوبة");
        }
        catch (Exception ex)
        {
            return ResultDto<ClientUnitAvailabilityResponse>.Failed($"حدث خطأ أثناء فحص التوفر: {ex.Message}", "CHECK_AVAILABILITY_ERROR");
        }
    }
}