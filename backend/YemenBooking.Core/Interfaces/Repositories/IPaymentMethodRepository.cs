using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;

namespace YemenBooking.Core.Interfaces.Repositories;

/// <summary>
/// واجهة مستودع طرق الدفع
/// Payment method repository interface
/// </summary>
public interface IPaymentMethodRepository
{
    /// <summary>
    /// الحصول على جميع طرق الدفع النشطة
    /// Get all active payment methods
    /// </summary>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة طرق الدفع النشطة</returns>
    Task<IEnumerable<PaymentMethod>> GetActivePaymentMethodsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على طرق الدفع المتاحة للعملاء
    /// Get payment methods available for clients
    /// </summary>
    /// <param name="countryCode">رمز البلد</param>
    /// <param name="currency">العملة</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة طرق الدفع المتاحة</returns>
    Task<IEnumerable<PaymentMethod>> GetAvailableForClientsAsync(string? countryCode = null, string? currency = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على طريقة دفع بالمعرف
    /// Get payment method by ID
    /// </summary>
    /// <param name="id">معرف طريقة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>طريقة الدفع أو null</returns>
    Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على طريقة دفع بالرمز
    /// Get payment method by code
    /// </summary>
    /// <param name="code">رمز طريقة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>طريقة الدفع أو null</returns>
    Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على طرق الدفع حسب النوع
    /// Get payment methods by type
    /// </summary>
    /// <param name="type">نوع طريقة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة طرق الدفع</returns>
    Task<IEnumerable<PaymentMethod>> GetByTypeAsync(PaymentMethodEnum type, CancellationToken cancellationToken = default);

    /// <summary>
    /// إضافة طريقة دفع جديدة
    /// Add new payment method
    /// </summary>
    /// <param name="paymentMethod">طريقة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>طريقة الدفع المضافة</returns>
    Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);

    /// <summary>
    /// تحديث طريقة دفع
    /// Update payment method
    /// </summary>
    /// <param name="paymentMethod">طريقة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>طريقة الدفع المحدثة</returns>
    Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف طريقة دفع
    /// Delete payment method
    /// </summary>
    /// <param name="id">معرف طريقة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>true إذا تم الحذف بنجاح</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من وجود طريقة دفع
    /// Check if payment method exists
    /// </summary>
    /// <param name="id">معرف طريقة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>true إذا كانت موجودة</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من وجود طريقة دفع بالرمز
    /// Check if payment method exists by code
    /// </summary>
    /// <param name="code">رمز طريقة الدفع</param>
    /// <param name="excludeId">معرف للاستثناء (للتحديث)</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>true إذا كانت موجودة</returns>
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// تفعيل أو إلغاء تفعيل طريقة دفع
    /// Activate or deactivate payment method
    /// </summary>
    /// <param name="id">معرف طريقة الدفع</param>
    /// <param name="isActive">حالة التفعيل</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>true إذا تم التحديث بنجاح</returns>
    Task<bool> SetActiveStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
}
