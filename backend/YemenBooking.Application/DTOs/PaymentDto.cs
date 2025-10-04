using System;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// DTO لبيانات الدفع
    /// DTO for payment data
    /// </summary>
    public class PaymentDto
    {
        /// <summary>
        /// معرف الدفعة
        /// Payment identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// معرف الحجز
        /// Booking identifier
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// المبلغ المدفوع
        /// Paid amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// رقم المعاملة
        /// Transaction identifier
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// طريقة الدفع
        /// Payment method
        /// </summary>
        public PaymentMethodEnum Method { get; set; }

        /// <summary>
        /// العملة
        /// Currency
        /// </summary>
        public string Currency { get; set; } = "YER";

        /// <summary>
        /// حالة الدفع
        /// Payment status
        /// </summary>
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// تاريخ الدفع
        /// Payment date
        /// </summary>
        public DateTime PaymentDate { get; set; }
    }
} 