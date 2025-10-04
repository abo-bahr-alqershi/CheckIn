using System.ComponentModel.DataAnnotations;
using YemenBooking.Core.Enums;

namespace YemenBooking.Core.Entities;

/// <summary>
/// كيان طريقة الدفع
/// Payment method entity
/// </summary>
public class PaymentMethod
{
    /// <summary>
    /// معرف طريقة الدفع
    /// Payment method ID
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// اسم طريقة الدفع
    /// Payment method name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// اسم طريقة الدفع باللغة الإنجليزية
    /// Payment method name in English
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// وصف طريقة الدفع
    /// Payment method description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// نوع طريقة الدفع
    /// Payment method type
    /// </summary>
    [Required]
    public PaymentMethodEnum Type { get; set; }

    /// <summary>
    /// رمز طريقة الدفع
    /// Payment method code
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// رابط أيقونة طريقة الدفع
    /// Payment method icon URL
    /// </summary>
    [MaxLength(500)]
    public string? IconUrl { get; set; }

    /// <summary>
    /// هل طريقة الدفع نشطة
    /// Is payment method active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// هل متاحة للعملاء
    /// Is available for clients
    /// </summary>
    public bool IsAvailableForClients { get; set; } = true;

    /// <summary>
    /// هل تتطلب تحقق إضافي من الهوية
    /// Indicates if this payment method requires extra identity verification
    /// </summary>
    public bool RequiresVerification { get; set; } = false;

    /// <summary>
    /// العملات المدعومة (JSON)
    /// Supported currencies (JSON)
    /// </summary>
    [MaxLength(1000)]
    public string? SupportedCurrencies { get; set; }

    /// <summary>
    /// البلدان المدعومة (JSON)
    /// Supported countries (JSON)
    /// </summary>
    [MaxLength(2000)]
    public string? SupportedCountries { get; set; }

    /// <summary>
    /// الحد الأدنى للمبلغ
    /// Minimum amount
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// الحد الأقصى للمبلغ
    /// Maximum amount
    /// </summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// رسوم المعاملة (نسبة مئوية)
    /// Transaction fee (percentage)
    /// </summary>
    public decimal? FeePercentage { get; set; }

    /// <summary>
    /// رسوم ثابتة للمعاملة
    /// Fixed transaction fee
    /// </summary>
    public decimal? FixedFee { get; set; }

    /// <summary>
    /// ترتيب العرض
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// تاريخ الإنشاء
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// تاريخ آخر تحديث
    /// Last update date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// معرف المستخدم المنشئ
    /// Creator user ID
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// معرف المستخدم المحدث
    /// Updater user ID
    /// </summary>
    public Guid? UpdatedBy { get; set; }
}
