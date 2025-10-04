using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Infrastructure.Services;
using YemenBooking.Infrastructure.Settings;

namespace YemenBooking.Infrastructure;

/// <summary>
/// إعداد حقن التبعيات للبنية التحتية
/// Infrastructure dependency injection setup
/// </summary>
public static class DependencyInjection
{
	/// <summary>
	/// إضافة خدمات البنية التحتية
	/// Add infrastructure services
	/// </summary>
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
        // Email settings configuration
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        // إضافة خدمات أخرى
		services.AddHttpClient<ICurrencyExchangeService, CurrencyExchangeService>();
		services.AddScoped<ICurrencySettingsService, CurrencySettingsService>();
		services.AddScoped<ICitySettingsService, CitySettingsService>();
        services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<IEmailVerificationService, EmailVerificationService>();
		services.AddScoped<IFileUploadService, FileUploadService>();
		services.AddScoped<IPasswordResetService, PasswordResetService>();
		services.AddScoped<IPaymentService, PaymentService>();

		return services;
	}
}