using System;
using System.Collections.Generic;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.DTOs.Bookings
{
    /// <summary>
    /// DTO لتفاصيل الحجز
    /// Booking details DTO
    /// </summary>
    public class BookingDetailsDto
    {
        // New properties to align with MobileApp handler expectations
        public string PropertyAddress { get; set; } = string.Empty;

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public int GuestsCount { get; set; }

        public DateTime BookedAt { get; set; }
        public string? BookingSource { get; set; }
        public string? CancellationReason { get; set; }
        public bool IsWalkIn { get; set; }
        public decimal? PlatformCommissionAmount { get; set; }
        public DateTime? ActualCheckInDate { get; set; }
        public DateTime? ActualCheckOutDate { get; set; }
        public decimal? FinalAmount { get; set; }
        public int? CustomerRating { get; set; }
        public string? CompletionNotes { get; set; }

        public List<BookingServiceDto> Services { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
        public List<string> UnitImages { get; set; } = new();
        /// <summary>
        /// معرف الحجز
        /// Booking ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// رقم الحجز
        /// Booking number
        /// </summary>
        public string BookingNumber { get; set; } = string.Empty;

        /// <summary>
        /// معرف المستخدم
        /// User ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// اسم المستخدم
        /// User name
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// معرف العقار
        /// Property ID
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// اسم العقار
        /// Property name
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// معرف الوحدة
        /// Unit ID
        /// </summary>
        public Guid? UnitId { get; set; }

        /// <summary>
        /// اسم الوحدة
        /// Unit name
        /// </summary>
        public string? UnitName { get; set; }

        /// <summary>
        /// تاريخ الوصول
        /// Check-in date
        /// </summary>
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// تاريخ المغادرة
        /// Check-out date
        /// </summary>
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// عدد الليالي
        /// Number of nights
        /// </summary>
        public int NumberOfNights { get; set; }

        /// <summary>
        /// عدد الضيوف البالغين
        /// Number of adult guests
        /// </summary>
        public int AdultGuests { get; set; }

        /// <summary>
        /// عدد الضيوف الأطفال
        /// Number of child guests
        /// </summary>
        public int ChildGuests { get; set; }

        /// <summary>
        /// المبلغ الإجمالي
        /// Total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// العملة
        /// Currency
        /// </summary>
        public string Currency { get; set; } = "YER";

        /// <summary>
        /// حالة الحجز
        /// Booking status
        /// </summary>
        public BookingStatus Status { get; set; }

        /// <summary>
        /// تاريخ الحجز
        /// Booking date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// ملاحظات خاصة
        /// Special notes
        /// </summary>
        public string? SpecialNotes { get; set; }

        /// <summary>
        /// معلومات الاتصال
        /// Contact information
        /// </summary>
        public ContactInfoDto ContactInfo { get; set; } = new();

        /// <summary>
        /// تفاصيل الدفع
        /// Payment details
        /// </summary>
        public List<PaymentDetailsDto> PaymentDetails { get; set; } = new();
    }

    /// <summary>
    /// DTO لمعلومات الاتصال
    /// Contact information DTO
    /// </summary>
    public class ContactInfoDto
    {
        /// <summary>
        /// رقم الهاتف
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// Email address
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO لتفاصيل الدفع
    /// Payment details DTO
    /// </summary>
    public class PaymentDetailsDto
    {
        /// <summary>
        /// معرف الدفعة
        /// Payment ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// المبلغ
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// طريقة الدفع
        /// Payment method
        /// </summary>
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// حالة الدفع
        /// Payment status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ الدفع
        /// Payment date
        /// </summary>
        public DateTime PaymentDate { get; set; }
    }
}
