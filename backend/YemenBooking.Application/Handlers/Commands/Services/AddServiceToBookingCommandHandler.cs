using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Services;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Services
{
    /// <summary>
    /// معالج أمر إضافة خدمة إلى الحجز
    /// </summary>
    public class AddServiceToBookingCommandHandler : IRequestHandler<AddServiceToBookingCommand, ResultDto<bool>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPropertyServiceRepository _serviceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AddServiceToBookingCommandHandler> _logger;

        public AddServiceToBookingCommandHandler(
            IBookingRepository bookingRepository,
            IPropertyServiceRepository serviceRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<AddServiceToBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _serviceRepository = serviceRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(AddServiceToBookingCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إضافة خدمة للحجز: BookingId={BookingId}, ServiceId={ServiceId}, Quantity={Quantity}",
                request.BookingId, request.ServiceId, request.Quantity);

            // التحقق من المدخلات
            if (request.BookingId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الحجز مطلوب");
            if (request.ServiceId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الخدمة مطلوب");
            if (request.Quantity <= 0)
                return ResultDto<bool>.Failed("الكمية يجب أن تكون أكبر من صفر");

            // التحقق من وجود الحجز
            var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
                return ResultDto<bool>.Failed("الحجز غير موجود");

            // التحقق من وجود الخدمة
            var service = await _serviceRepository.GetServiceByIdAsync(request.ServiceId, cancellationToken);
            if (service == null)
                return ResultDto<bool>.Failed("الخدمة غير موجودة");

            // التحقق من الصلاحيات (صاحب الحجز أو مسؤول)
            if (_currentUserService.Role != "Admin"  && booking.UserId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بإضافة خدمة لهذا الحجز");

            // تنفيذ الإضافة
            bool added = await _bookingRepository.AddServiceToBookingAsync(request.BookingId, request.ServiceId, request.Quantity, cancellationToken);
            if (!added)
                return ResultDto<bool>.Failed("فشل إضافة الخدمة إلى الحجز");

            // إعادة حساب السعر
            await _bookingRepository.RecalculatePriceAsync(request.BookingId, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "AddServiceToBooking",
                $"تم إضافة الخدمة {request.ServiceId} إلى الحجز {request.BookingId} بكمية {request.Quantity}",
                request.BookingId,
                "BookingService",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتملت عملية إضافة الخدمة إلى الحجز بنجاح");
            return ResultDto<bool>.Succeeded(true, "تم إضافة الخدمة إلى الحجز بنجاح");
        }
    }
} 