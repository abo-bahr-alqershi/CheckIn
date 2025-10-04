using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Services
{
    /// <summary>
    /// أمر لإضافة خدمة إلى الحجز
    /// Command to add a service to a booking
    /// </summary>
    public class AddServiceToBookingCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الحجز
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// معرف الخدمة
        /// </summary>
        public Guid ServiceId { get; set; }

        /// <summary>
        /// الكمية
        /// </summary>
        public int Quantity { get; set; }
    }
} 