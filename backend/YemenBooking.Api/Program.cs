using YemenBooking.Api.Extensions;
using YemenBooking.Api.Services;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.Mappings;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Settings;
using YemenBooking.Infrastructure;
using YemenBooking.Infrastructure.Data;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Infrastructure.Dapper;
// using YemenBooking.Infrastructure.Migrations;
using YemenBooking.Infrastructure.Services;
using YemenBooking.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
// using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using YemenBooking.Application.Handlers.Queries.MobileApp.Properties;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using YemenBooking.Api.Transformers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using YemenBooking.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Listen on port 5000 only in Development; in hosting, rely on platform binding
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5000);
    });
}

// WebSocket chat disabled: using FCM for real-time notifications

// إضافة خدمات Dapper
builder.Services.AddDapperRepository(builder.Configuration);

// Add services to the container.
// Configuring Swagger/OpenAPI with JWT security
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "YemenBooking API",
        Version = "v1",
        Description = "وثائق واجهة برمجة تطبيقات YemenBooking"
    });
    // تعريف أمان JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "أدخل 'Bearer ' متبوعًا برمز JWT"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    // تضمين تعليقات XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // استخدام الاسم الكامل للنوع كمُعرّف للمخطط لمنع التعارضات بين الأنواع المتشابهة الاسم
    options.CustomSchemaIds(type => (type.FullName ?? type.Name).Replace('+', '.'));

    // تمكين دعم رفع الملفات عبر فلتر مخصص
    options.OperationFilter<YemenBooking.Api.Swagger.SwaggerFileOperationFilter>();
});

// إضافة MediatR مع معالجات الأوامر
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(GetPropertyDetailsQueryHandler).Assembly);
});

// إضافة AutoMapper مع تقييد البحث على مجلد Mappings في طبقة Application
builder.Services.AddAutoMapper(
    cfg => cfg.AddMaps(typeof(QueryMappingProfile).Assembly),
    typeof(QueryMappingProfile).Assembly);

// إضافة خدمات المشروع
builder.Services.AddYemenBookingServices();
// إضافة التخزين المؤقت في الذاكرة لحفظ الفهارس
builder.Services.AddMemoryCache();

// إضافة دعم Controllers مع تحويل PascalCase إلى kebab-case في المسارات ودعم تحويل الـ enum كسلاسل نصية
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Bind file storage settings so URLs are absolute and paths are correct
builder.Services.Configure<YemenBooking.Infrastructure.Settings.FileStorageSettings>(
    builder.Configuration.GetSection("FileStorageSettings"));

// إضافة سياسة CORS للسماح بالاتصالات من الواجهة الأمامية
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
            "http://localhost:5000", // Your actual frontend URL
            "http://localhost:5173", 
            "https://localhost:5173"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true)
        .WithExposedHeaders("*")
    );
});

// تسجيل إعدادات JWT من ملفات التكوين
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// تسجيل إعدادات البريد الإلكتروني من ملفات التكوين
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// إعداد المصادقة باستخدام JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.RequireHttpsMetadata = false; // Changed to false for development
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidateIssuerSigningKey = true
    };
});

// إضافة التفويض
builder.Services.AddAuthorization();

// إعداد DbContext لاستخدام SQL Server بدلاً من SQLite
builder.Services.AddDbContext<YemenBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.ModelValidationKeyDefaultValueWarning))
);

// إضافة HttpContextAccessor لاستخدامه في CurrentUserService
builder.Services.AddHttpContextAccessor();

// إضافة HttpClient للخدمات التي تحتاجه
builder.Services.AddHttpClient<IGeolocationService, GeolocationService>();
builder.Services.AddHttpClient<IPaymentGatewayService, PaymentGatewayService>();

// تسجيل خدمة الفهرسة باستخدام LiteDB
builder.Services.AddSingleton<YemenBooking.Infrastructure.Indexing.Services.ILiteDbWriteQueue>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var dbPath = Path.Combine(env.ContentRootPath, "Data", "PropertyIndex.db");
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
    return new YemenBooking.Infrastructure.Indexing.Services.QueuedLiteDbService(
        dbPath,
        provider.GetRequiredService<ILogger<YemenBooking.Infrastructure.Indexing.Services.QueuedLiteDbService>>()
    );
});

builder.Services.AddHostedService(provider => (YemenBooking.Infrastructure.Indexing.Services.QueuedLiteDbService)provider.GetRequiredService<YemenBooking.Infrastructure.Indexing.Services.ILiteDbWriteQueue>());

// إشغّل مرسل الإشعارات المجدولة
builder.Services.AddHostedService<ScheduledNotificationsDispatcher>();

// IMPORTANT: IIndexingService depends on scoped repositories/services, so register it as Scoped
builder.Services.AddScoped<IIndexingService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var dbPath = Path.Combine(env.ContentRootPath, "Data", "PropertyIndex.db");
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
    return new YemenBooking.Infrastructure.Indexing.Services.LiteDbIndexingService(
        dbPath,
        provider.GetRequiredService<YemenBooking.Core.Interfaces.Repositories.IPropertyRepository>(),
        provider.GetRequiredService<YemenBooking.Core.Interfaces.Repositories.IUnitRepository>(),
        provider.GetRequiredService<YemenBooking.Application.Interfaces.Services.IAvailabilityService>(),
        provider.GetRequiredService<YemenBooking.Application.Interfaces.Services.IPricingService>(),
        provider.GetRequiredService<IMemoryCache>(),
        provider.GetRequiredService<ILogger<YemenBooking.Infrastructure.Indexing.Services.LiteDbIndexingService>>(),
        provider.GetRequiredService<YemenBooking.Infrastructure.Indexing.Services.ILiteDbWriteQueue>()
    );
});

var app = builder.Build();

// التأكد من وجود الإجراءات المخزنة عند بدء التطبيق
using (var scope = app.Services.CreateScope())
{
    var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
    connection.Open();
    StoredProceduresInitializer.EnsureAdvancedSearchProc(connection);
}

// تطبيق المهاجرات وتشغيل البذور عند بدء التشغيل
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<YemenBookingDbContext>();
        await db.Database.MigrateAsync();

        var seeder = new DataSeedingService(db);
        await seeder.SeedAsync();
        logger.LogInformation("Database migrated and seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to migrate and seed database on startup");
        // لا نرمي الاستثناء لكي لا يمنع تشغيل التطبيق في بيئات التطوير
    }
}

// استخدام امتداد لتكوين كافة middleware الخاصة بالتطبيق
app.UseYemenBookingMiddlewares();

// بناء/إعادة بناء فهرس LiteDB بعد بدء التطبيق لضمان تشغيل Hosted Services (طابور الكتابة)
app.Lifetime.ApplicationStarted.Register(() =>
{
    _ = Task.Run(async () =>
    {
        using var scope = app.Services.CreateScope();
        var indexService = scope.ServiceProvider.GetRequiredService<IIndexingService>();
        try
        {
            await indexService.RebuildIndexAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "خطأ في بناء الفهرس الأولي");
        }
    });
});

// Initialize Firebase Admin SDK
try
{
    if (FirebaseApp.DefaultInstance == null)
    {
        GoogleCredential credential;
        var credentialsPath = builder.Configuration["Firebase:CredentialsPath"]; // file path
        var credentialsJson = builder.Configuration["Firebase:CredentialsJson"]; // raw JSON (appsettings or env)
        var credentialsBase64 = builder.Configuration["Firebase:CredentialsBase64"]; // base64-encoded JSON (env-friendly)

        if (!string.IsNullOrWhiteSpace(credentialsPath) && System.IO.File.Exists(credentialsPath) && new FileInfo(credentialsPath).Length > 0)
        {
            credential = GoogleCredential.FromFile(credentialsPath);
        }
        else if (!string.IsNullOrWhiteSpace(credentialsJson))
        {
            using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(credentialsJson));
            credential = GoogleCredential.FromStream(ms);
        }
        else if (!string.IsNullOrWhiteSpace(credentialsBase64))
        {
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(credentialsBase64));
            using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            credential = GoogleCredential.FromStream(ms);
        }
        else if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS_JSON")))
        {
            var envJson = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS_JSON")!;
            using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(envJson));
            credential = GoogleCredential.FromStream(ms);
        }
        else if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
        {
            // Use standard ADC path if explicitly provided via env var
            credential = GoogleCredential.GetApplicationDefault();
        }
        else
        {
            app.Logger.LogWarning("Firebase credentials not provided or empty. Skipping Firebase Admin initialization until credentials are configured.");
            credential = null!; // won't be used
        }

        if (credential != null)
        {
            FirebaseApp.Create(new AppOptions { Credential = credential });
        }
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Failed to initialize Firebase Admin SDK. Configure Firebase:CredentialsPath or credentials JSON (Firebase:CredentialsJson / Firebase:CredentialsBase64 / GOOGLE_APPLICATION_CREDENTIALS_JSON).");
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}