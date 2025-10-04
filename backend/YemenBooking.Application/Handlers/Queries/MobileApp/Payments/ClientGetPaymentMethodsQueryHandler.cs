using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using YemenBooking.Application.Queries.MobileApp.Payments;
using YemenBooking.Application.DTOs;
using ClientPaymentMethodDto = YemenBooking.Application.Queries.MobileApp.Payments.ClientPaymentMethodDto;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Payments;

/// <summary>
/// معالج استعلام الحصول على طرق الدفع المتاحة للعميل
/// Handler for client get payment methods query
/// </summary>
public class ClientGetPaymentMethodsQueryHandler : IRequestHandler<ClientGetPaymentMethodsQuery, ResultDto<List<ClientPaymentMethodDto>>>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ClientGetPaymentMethodsQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام طرق الدفع للعميل
    /// Constructor for client get payment methods query handler
    /// </summary>
    /// <param name="paymentMethodRepository">مستودع طرق الدفع</param>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="configuration">إعدادات التطبيق</param>
    /// <param name="logger">مسجل الأحداث</param>
    public ClientGetPaymentMethodsQueryHandler(
        IPaymentMethodRepository paymentMethodRepository,
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<ClientGetPaymentMethodsQueryHandler> logger)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على طرق الدفع المتاحة للعميل
    /// Handle client get payment methods query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة طرق الدفع المتاحة</returns>
    public async Task<ResultDto<List<ClientPaymentMethodDto>>> Handle(ClientGetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام طرق الدفع المتاحة للعميل. معرف المستخدم: {UserId}, البلد: {Country}, العملة: {Currency}, المبلغ: {Amount}", 
                request.UserId, request.Country, request.Currency, request.Amount);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // تحديد البلد إذا لم يتم تحديده
            string userCountry = request.Country ?? "YE"; // اليمن كبلد افتراضي
            
            // إذا تم تحديد معرف المستخدم، جلب بيانات المستخدم
            if (request.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);
                if (user != null)
                {
                    userCountry = "YE"; // افتراضي اليمن (سيتم تحديثه لاحقاً)
                }
            }
            
            var countryCode = "YE"; // افتراضي اليمن

            // الحصول على جميع طرق الدفع النشطة
            var allPaymentMethods = await _paymentMethodRepository.GetActivePaymentMethodsAsync(cancellationToken);
            
            if (allPaymentMethods == null || !allPaymentMethods.Any())
            {
                _logger.LogWarning("لا توجد طرق دفع نشطة في النظام");
                
                // إرجاع طرق دفع افتراضية
                var defaultMethods = GetDefaultPaymentMethods(request.Currency, userCountry);
                return ResultDto<List<ClientPaymentMethodDto>>.Ok(
                    defaultMethods, 
                    "تم إرجاع طرق الدفع الافتراضية"
                );
            }

            // فلترة طرق الدفع حسب البلد والعملة والمبلغ
            var availablePaymentMethods = new List<ClientPaymentMethodDto>();

            foreach (var method in allPaymentMethods)
            {
                // التحقق من دعم البلد
                if (method.SupportedCountries != null && method.SupportedCountries.Any() && 
                    !method.SupportedCountries.Contains(userCountry))
                {
                    continue;
                }

                // التحقق من دعم العملة
                if (method.SupportedCurrencies != null && method.SupportedCurrencies.Any() && 
                    !method.SupportedCurrencies.Contains(request.Currency))
                {
                    continue;
                }

                // التحقق من حدود المبلغ
                if (request.Amount.HasValue)
                {
                    if (request.Amount.Value < method.MinAmount || 
                        (method.MaxAmount > 0 && request.Amount.Value > method.MaxAmount))
                    {
                        continue;
                    }
                }

                // حساب الرسوم
                var (transactionFee, feePercentage) = CalculateFees(method, request.Amount ?? 0);

                // تحديد ما إذا كانت الطريقة متاحة
                var isAvailable = IsPaymentMethodAvailable(method, userCountry, request.Currency, request.Amount);

                var paymentMethodDto = new ClientPaymentMethodDto
                {
                    Id = method.Id.ToString(),
                    Name = method.Name ?? string.Empty,
                    Description = method.Description ?? string.Empty,
                    IconUrl = method.IconUrl ?? string.Empty,
                    LogoUrl = "/images/payment-default.png",
                    IsAvailable = isAvailable,
                    TransactionFee = transactionFee,
                    FeePercentage = feePercentage,
                    MinAmount = method.MinAmount.HasValue ? method.MinAmount.Value : 0,
                    MaxAmount = method.MaxAmount.HasValue ? method.MaxAmount.Value : 0,
                    SupportedCurrencies = method.SupportedCurrencies?.Split(',').ToList() ?? new List<string>(),
                    SupportedCountries = method.SupportedCountries?.Split(',').ToList() ?? new List<string>(),
                    ProcessingTime = "فوري",
                    Type = method.Type.ToString(),
                    RequiresVerification = method.RequiresVerification,
                    SupportsRefunds = true,
                    DisplayOrder = method.DisplayOrder,
                    IsRecommended = true,
                    WarningMessage = GetWarningMessage(method, request.Amount)
                };

                availablePaymentMethods.Add(paymentMethodDto);
            }

            // إضافة طرق دفع افتراضية إذا لم توجد طرق كافية
            if (availablePaymentMethods.Count == 0)
            {
                _logger.LogWarning("لا توجد طرق دفع متاحة للمعايير المحددة، إضافة طرق افتراضية");
                availablePaymentMethods.AddRange(GetDefaultPaymentMethods(request.Currency, userCountry));
            }

            // ترتيب طرق الدفع
            availablePaymentMethods = availablePaymentMethods
                .OrderByDescending(pm => pm.IsRecommended)
                .ThenBy(pm => pm.DisplayOrder)
                .ThenBy(pm => pm.Name)
                .ToList();

            _logger.LogInformation("تم العثور على {Count} طريقة دفع متاحة", availablePaymentMethods.Count);

            return ResultDto<List<ClientPaymentMethodDto>>.Ok(
                availablePaymentMethods, 
                $"تم العثور على {availablePaymentMethods.Count} طريقة دفع متاحة"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء الحصول على طرق الدفع المتاحة للعميل");
            return ResultDto<List<ClientPaymentMethodDto>>.Failed(
                $"حدث خطأ أثناء الحصول على طرق الدفع: {ex.Message}", 
                "GET_PAYMENT_METHODS_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<List<ClientPaymentMethodDto>> ValidateRequest(ClientGetPaymentMethodsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            _logger.LogWarning("العملة مطلوبة");
            return ResultDto<List<ClientPaymentMethodDto>>.Failed("العملة مطلوبة", "CURRENCY_REQUIRED");
        }

        if (request.Currency.Length != 3)
        {
            _logger.LogWarning("رمز العملة يجب أن يكون 3 أحرف");
            return ResultDto<List<ClientPaymentMethodDto>>.Failed("رمز العملة يجب أن يكون 3 أحرف", "INVALID_CURRENCY_CODE");
        }

        if (request.Amount.HasValue && request.Amount.Value < 0)
        {
            _logger.LogWarning("المبلغ لا يمكن أن يكون سالباً");
            return ResultDto<List<ClientPaymentMethodDto>>.Failed("المبلغ لا يمكن أن يكون سالباً", "INVALID_AMOUNT");
        }

        if (request.Amount.HasValue && request.Amount.Value > 10000000) // 10 مليون
        {
            _logger.LogWarning("المبلغ كبير جداً");
            return ResultDto<List<ClientPaymentMethodDto>>.Failed("المبلغ كبير جداً", "AMOUNT_TOO_LARGE");
        }

        return ResultDto<List<ClientPaymentMethodDto>>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// حساب رسوم المعاملة
    /// Calculate transaction fees
    /// </summary>
    /// <param name="method">طريقة الدفع</param>
    /// <param name="amount">المبلغ</param>
    /// <returns>الرسوم الثابتة ونسبة الرسوم</returns>
    private (decimal transactionFee, decimal feePercentage) CalculateFees(Core.Entities.PaymentMethod method, decimal amount)
    {
        var transactionFee = method.FixedFee ?? 0m;
        var feePercentage = method.FeePercentage ?? 0m;

        // تطبيق رسوم إضافية حسب نوع طريقة الدفع
        switch (method.Type)
        {
            case PaymentMethodEnum.CreditCard:
                feePercentage += 0.029m; // 2.9% رسوم إضافية للبطاقات الائتمانية
                break;
            // المحافظ الرقمية المتاحة
            case PaymentMethodEnum.JwaliWallet:
            case PaymentMethodEnum.CashWallet:
            case PaymentMethodEnum.OneCashWallet:
            case PaymentMethodEnum.FloskWallet:
            case PaymentMethodEnum.JaibWallet:
                feePercentage += 0.015m; // 1.5% رسوم إضافية للمحافظ الرقمية
                break;
        }

        return (transactionFee, feePercentage);
    }

    /// <summary>
    /// التحقق من توفر طريقة الدفع
    /// Check if payment method is available
    /// </summary>
    /// <param name="method">طريقة الدفع</param>
    /// <param name="country">البلد</param>
    /// <param name="currency">العملة</param>
    /// <param name="amount">المبلغ</param>
    /// <returns>هل الطريقة متاحة</returns>
    private bool IsPaymentMethodAvailable(Core.Entities.PaymentMethod method, string country, string currency, decimal? amount)
    {
        // التحقق من أن الطريقة نشطة
        if (!method.IsActive)
        {
            return false;
        }

        // التحقق من ساعات العمل (خاصية OperatingHours حُذفت، لذا نفترض أن الطريقة متاحة دائماً)

        // NOTE: تم حذف التحقق من حالة الخدمة ServiceStatus لعدم وجود الخاصية.

        return true;
    }

    /// <summary>
    /// الحصول على رسالة تحذيرية
    /// Get warning message
    /// </summary>
    /// <param name="method">طريقة الدفع</param>
    /// <param name="amount">المبلغ</param>
    /// <returns>رسالة التحذير إن وجدت</returns>
    private string? GetWarningMessage(Core.Entities.PaymentMethod method, decimal? amount)
    {
        if (method.RequiresVerification)
        {
            return "تتطلب هذه الطريقة تحقق إضافي من الهوية";
        }

        if (amount.HasValue && method.MaxAmount > 0 && amount.Value > method.MaxAmount * 0.9m)
        {
            return $"المبلغ قريب من الحد الأقصى المسموح ({method.MaxAmount:N0})";
        }

        // لا توجد طريقة تحويل بنكي في التعريف الحالي لطريقة الدفع
        return null;
    }

    /// <summary>
    /// الحصول على طرق دفع افتراضية
    /// Get default payment methods
    /// </summary>
    /// <param name="currency">العملة</param>
    /// <param name="country">البلد</param>
    /// <returns>قائمة طرق الدفع الافتراضية</returns>
    private List<ClientPaymentMethodDto> GetDefaultPaymentMethods(string currency, string country)
    {
        var defaultMethods = new List<ClientPaymentMethodDto>();

        // طرق دفع افتراضية للسوق اليمني
        if (country == "YE")
        {
            defaultMethods.AddRange(new[]
            {
                new ClientPaymentMethodDto
                {
                    Id = "cash_on_delivery",
                    Name = "الدفع عند الاستلام",
                    Description = "ادفع نقداً عند وصولك للعقار",
                    Type = "cash",
                    IsAvailable = true,
                    TransactionFee = 0,
                    FeePercentage = 0,
                    MinAmount = 1,
                    MaxAmount = 1000000,
                    SupportedCurrencies = new List<string> { "YER" },
                    SupportedCountries = new List<string> { "YE" },
                    ProcessingTime = "فوري",
                    SupportsRefunds = true,
                    DisplayOrder = 1,
                    IsRecommended = true
                },
                new ClientPaymentMethodDto
                {
                    Id = "bank_transfer",
                    Name = "تحويل بنكي",
                    Description = "حوّل المبلغ إلى حساب العقار مباشرة",
                    Type = "bank_transfer",
                    IsAvailable = true,
                    TransactionFee = 10,
                    FeePercentage = 0,
                    MinAmount = 100,
                    MaxAmount = 5000000,
                    SupportedCurrencies = new List<string> { "YER", "USD", "SAR" },
                    SupportedCountries = new List<string> { "YE", "SA", "AE" },
                    ProcessingTime = "1-3 أيام عمل",
                    SupportsRefunds = true,
                    DisplayOrder = 2
                },
                new ClientPaymentMethodDto
                {
                    Id = "mobile_wallet",
                    Name = "محفظة موبايل",
                    Description = "ادفع باستخدام محفظتك الرقمية",
                    Type = "digital_wallet",
                    IsAvailable = true,
                    TransactionFee = 5,
                    FeePercentage = 1.5m,
                    MinAmount = 10,
                    MaxAmount = 500000,
                    SupportedCurrencies = new List<string> { "YER" },
                    SupportedCountries = new List<string> { "YE" },
                    ProcessingTime = "فوري",
                    SupportsRefunds = true,
                    DisplayOrder = 3
                }
            });
        }

        // إضافة طرق دفع دولية إذا كانت العملة غير يمنية
        if (currency != "YER")
        {
            defaultMethods.Add(new ClientPaymentMethodDto
            {
                Id = "credit_card",
                Name = "بطاقة ائتمانية",
                Description = "ادفع باستخدام بطاقة فيزا أو ماستركارد",
                Type = "credit_card",
                MinAmount = 1,
                MaxAmount = 50000,
                SupportedCurrencies = new List<string> { "USD", "EUR", "SAR", "AED" },
                SupportedCountries = new List<string> { "YE", "SA", "AE", "US", "GB" },
                ProcessingTime = "فوري",
                RequiresVerification = true,
                SupportsRefunds = true,
                IsRecommended = true
            });
        }

        return defaultMethods;
    }
}
