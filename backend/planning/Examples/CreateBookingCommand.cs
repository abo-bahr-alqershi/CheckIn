using MediatR;
using YemenBooking.Application.DTOs.Bookings;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Bookings;

/// <summary>
/// أمر إنشاء حجز جديد
/// Create booking command
/// </summary>
public class CreateBookingCommand : IRequest<ResultDto<BookingDto>>
{
    /// <summary>
    /// معرّف الفندق
    /// Hotel ID
    /// </summary>
    public Guid HotelId { get; set; }

    /// <summary>
    /// معرّف الغرفة
    /// Room ID
    /// </summary>
    public Guid RoomId { get; set; }

    

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
    /// عدد البالغين
    /// Number of adults
    /// </summary>
    public int Adults { get; set; }

    /// <summary>
    /// عدد الأطفال
    /// Number of children
    /// </summary>
    public int Children { get; set; }

    /// <summary>
    /// اسم الضيف الرئيسي
    /// Primary guest name
    /// </summary>
    public string GuestName { get; set; } = null!;

    /// <summary>
    /// بريد الضيف الإلكتروني
    /// Guest email
    /// </summary>
    public string GuestEmail { get; set; } = null!;

    /// <summary>
    /// هاتف الضيف
    /// Guest phone
    /// </summary>
    public string GuestPhone { get; set; } = null!;

    /// <summary>
    /// جنسية الضيف
    /// Guest nationality
    /// </summary>
    public string? GuestNationality { get; set; }

    /// <summary>
    /// تفاصيل الضيوف
    /// Guest details
    /// </summary>
    public IEnumerable<CreateBookingGuestDto>? Guests { get; set; }

    /// <summary>
    /// ملاحظات الحجز
    /// Booking notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// طلبات خاصة
    /// Special requests
    /// </summary>
    public string? SpecialRequests { get; set; }

    /// <summary>
    /// مصدر الحجز
    /// Booking source
    /// </summary>
    public BookingSource Source { get; set; } = BookingSource.DIRECT;

    /// <summary>
    /// مرجع خارجي
    /// External reference
    /// </summary>
    public string? ExternalReference { get; set; }

    /// <summary>
    /// رقم كوبون الخصم
    /// Coupon code
    /// </summary>
    public string? CouponCode { get; set; }

    /// <summary>
    /// الخدمات الإضافية
    /// Additional services
    /// </summary>
    

    /// <summary>
    /// بيانات الدفعة الأولية
    /// Initial payment details
    /// </summary>
    public CreateBookingPaymentDto? InitialPayment { get; set; }

    /// <summary>
    /// هل يتم تأكيد الحجز فوراً
    /// Auto confirm booking
    /// </summary>
    public bool AutoConfirm { get; set; } = true;

    /// <summary>
    /// تاريخ انتهاء صلاحية الحجز
    /// Booking expiry date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// معرّف المنشئ
    /// Created by user ID
    /// </summary>
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// عملة الحجز
    /// Booking currency
    /// </summary>
    public string Currency { get; set; } = "YER";

    /// <summary>
    /// قبول الشروط والأحكام
    /// Terms and conditions accepted
    /// </summary>
    public bool TermsAccepted { get; set; }

    /// <summary>
    /// قبول سياسة الإلغاء
    /// Cancellation policy accepted
    /// </summary>
    public bool CancellationPolicyAccepted { get; set; }

    /// <summary>
    /// قبول إرسال رسائل ترويجية
    /// Marketing emails consent
    /// </summary>
    public bool MarketingEmailsConsent { get; set; }

    /// <summary>
    /// رسوم الخدمة
    /// Service charges
    /// </summary>
    public decimal ServiceCharges { get; set; }
}

/// <summary>
/// DTO لإنشاء ضيف جديد
/// Create booking guest DTO
/// </summary>
public class CreateBookingGuestDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public IdentityType? IdType { get; set; }
    public string? IdNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public bool IsPrimaryGuest { get; set; }
    public bool IsChild { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO لإنشاء خدمة حجز
/// Create booking service DTO
/// </summary>


/// <summary>
/// DTO لإنشاء دفعة حجز
/// Create booking payment DTO
/// </summary>
public class CreateBookingPaymentDto
{
    public MoneyDto Amount { get; set; } = null!;
    public PaymentMethod Method { get; set; }
    public PaymentType Type { get; set; } = PaymentType.ADVANCE;
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
    public bool ProcessImmediately { get; set; } = true;
}