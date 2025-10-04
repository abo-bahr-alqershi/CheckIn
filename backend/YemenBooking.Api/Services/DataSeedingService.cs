using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Core.Seeds;
using YemenBooking.Core.Entities;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Api.Services
{
    public class DataSeedingService
    {
        private readonly YemenBookingDbContext _context;

        public DataSeedingService(YemenBookingDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Initialize currencies
            if (!await _context.Currencies.AnyAsync())
            {
                _context.Currencies.AddRange(
                    new Currency { Code = "YER", ArabicCode = "ريال", Name = "Yemeni Rial", ArabicName = "الريال اليمني", IsDefault = true },
                    new Currency { Code = "USD", ArabicCode = "دولار", Name = "US Dollar", ArabicName = "الدولار الأمريكي", IsDefault = false, ExchangeRate = 0.004m, LastUpdated = DateTime.UtcNow }
                );
                await _context.SaveChangesAsync();
            }

            // Initialize cities
            if (!await _context.Cities.AnyAsync())
            {
                _context.Cities.AddRange(
                    new City { Name = "صنعاء", Country = "اليمن", ImagesJson = "[]" },
                    new City { Name = "عدن", Country = "اليمن", ImagesJson = "[]" },
                    new City { Name = "تعز", Country = "اليمن", ImagesJson = "[]" }
                );
                await _context.SaveChangesAsync();
            }

            // Users
            if (!await _context.Users.AnyAsync())
            {
                _context.Users.AddRange(new UserSeeder().SeedData());
                await _context.SaveChangesAsync();
            }

            // Roles and UserRoles are seeded via Entity configurations/migrations

            // Property types
            if (!await _context.PropertyTypes.AnyAsync())
            {
                _context.PropertyTypes.AddRange(new PropertyTypeSeeder().SeedData());
                await _context.SaveChangesAsync();
            }

            // Unit types: create one default unit type per property type
            if (!await _context.UnitTypes.AnyAsync())
            {
                var propertyTypes = await _context.PropertyTypes.AsNoTracking().ToListAsync();
                var unitTypes = propertyTypes.Select(pt => new UnitType
                {
                    Id = Guid.NewGuid(),
                    PropertyTypeId = pt.Id,
                    Name = pt.Name + " Default",
                    Description = pt.Name + " default unit type",
                    DefaultPricingRules = "[]",
                    MaxCapacity = 4,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                }).ToList();
                _context.UnitTypes.AddRange(unitTypes);
                await _context.SaveChangesAsync();
            }

            // Properties: assign valid TypeId, OwnerId and City (must match seeded City.Name values)
            if (!await _context.Properties.AnyAsync())
            {
                var propertyTypes = await _context.PropertyTypes.AsNoTracking().ToListAsync();
                var cities = await _context.Cities.AsNoTracking().Select(c => c.Name).ToListAsync();
                var seededProperties = new PropertySeeder().SeedData().ToList();
                var rnd = new Random();
                var ownerId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
                foreach (var prop in seededProperties)
                {
                    prop.Currency = "YER"; // ensure existing currency code
                    prop.TypeId = propertyTypes[rnd.Next(propertyTypes.Count)].Id; // valid FK
                    prop.OwnerId = ownerId; // existing seeded user
                    if (cities.Count > 0)
                    {
                        // Override faker random city with a valid seeded city to satisfy FK constraint
                        prop.City = cities[rnd.Next(cities.Count)];
                    }
                }
                _context.Properties.AddRange(seededProperties);
                await _context.SaveChangesAsync();
            }

            // Units: assign valid PropertyId and UnitTypeId
            if (!await _context.Units.AnyAsync())
            {
                var properties = await _context.Properties.AsNoTracking().ToListAsync();
                var unitTypes = await _context.UnitTypes.AsNoTracking().ToListAsync();
                var seededUnits = new UnitSeeder().SeedData().ToList();
                var rnd = new Random();
                foreach (var u in seededUnits)
                {
                    u.PropertyId = properties[rnd.Next(properties.Count)].Id;
                    u.UnitTypeId = unitTypes[rnd.Next(unitTypes.Count)].Id;
                    // Force unit base currency to property's currency if available, otherwise YER
                    var prop = properties.FirstOrDefault(p => p.Id == u.PropertyId);
                    var code = (prop?.Currency ?? "YER").ToUpperInvariant();
                    u.BasePrice = new Money(u.BasePrice.Amount, code);
                }
                _context.Units.AddRange(seededUnits);
                await _context.SaveChangesAsync();
            }

            // Property images: assign valid PropertyId and optional UnitId
            if (!await _context.PropertyImages.AnyAsync())
            {
                var properties = await _context.Properties.AsNoTracking().ToListAsync();
                var units = await _context.Units.AsNoTracking().ToListAsync();
                var seededImages = new PropertyImageSeeder().SeedData().ToList();
                var rnd = new Random();
                foreach (var img in seededImages)
                {
                    img.PropertyId = properties[rnd.Next(properties.Count)].Id;
                    if (units.Any()) img.UnitId = units[rnd.Next(units.Count)].Id;
                }
                _context.PropertyImages.AddRange(seededImages);
                await _context.SaveChangesAsync();
            }

            // Amenities
            if (!await _context.Amenities.AnyAsync())
            {
                _context.Amenities.AddRange(new AmenitySeeder().SeedData());
                await _context.SaveChangesAsync();
            }

            // Bookings: assign valid UserId and UnitId
            if (!await _context.Bookings.AnyAsync())
            {
                var users = await _context.Users.AsNoTracking().ToListAsync();
                var units = await _context.Units.AsNoTracking().ToListAsync();
                var seededBookings = new BookingSeeder().SeedData().ToList();
                var rnd = new Random();
                foreach (var b in seededBookings)
                {
                    b.UserId = users[rnd.Next(users.Count)].Id;
                    b.UnitId = units[rnd.Next(units.Count)].Id;
                }
                _context.Bookings.AddRange(seededBookings);
                await _context.SaveChangesAsync();
            }

            // Reviews: seed one Arabic review per existing booking
            if (!await _context.Reviews.AnyAsync())
            {
                var bookingsList = await _context.Bookings
                    .Include(b => b.Unit)
                    .AsNoTracking()
                    .ToListAsync();
                // Arabic comments pool
                var comments = new[]
                {
                    "الخدمة ممتازة والنظافة عالية.",
                    "الإقامة كانت رائعة، أنصح به بشدة.",
                    "الموقع جيد لكن السعر مرتفع قليلاً.",
                    "المنظر كان خلاباً والخدمة رائعة.",
                    "التجربة كانت مرضية لكن كان هناك بعض الضوضاء.",
                    "الوحدة كانت نظيفة ومريحة للغاية.",
                    "التواصل مع المالك كان سلساً وودوداً.",
                    "التقييم العام جيد جداً، سأعود مرة أخرى."
                };
                var rnd = new Random();
                var reviewsToAdd = bookingsList.Select(b => new Review
                {
                    Id = Guid.NewGuid(),
                    PropertyId = b.Unit.PropertyId,
                    BookingId = b.Id,
                    Cleanliness = rnd.Next(1, 6),
                    Service = rnd.Next(1, 6),
                    Location = rnd.Next(1, 6),
                    Value = rnd.Next(1, 6),
                    Comment = comments[rnd.Next(comments.Length)],
                    CreatedAt = DateTime.UtcNow.AddDays(-rnd.Next(0, 30)),
                    ResponseText = null,
                    ResponseDate = null,
                    IsPendingApproval = true
                }).ToList();
                _context.Reviews.AddRange(reviewsToAdd);
                await _context.SaveChangesAsync();
            }

            // Reports: seed diverse reports in Arabic with relationships
            if (!await _context.Reports.AnyAsync())
            {
                var users = await _context.Users.AsNoTracking().ToListAsync();
                var properties = await _context.Properties.AsNoTracking().ToListAsync();
                var rnd = new Random();
                var reasons = new[]
                {
                    "محتوى مسيء",
                    "سلوك غير لائق",
                    "مشكلة في الحجز",
                    "خطأ تقني",
                    "طلب إلغاء غير منطقي",
                    "معلومات خاطئة",
                    "انتهاك للقواعد",
                    "شكاوى أخرى"
                };
                var descriptions = new[]
                {
                    "تم العثور على محتوى مسيء في وصف الوحدة.",
                    "سلوك المستخدم كان غير لائق خلال فترة الإقامة.",
                    "واجهت مشكلة في عملية الحجز لم يتم حلها.",
                    "تعذر الوصول إلى تفاصيل الحجز بسبب خطأ تقني.",
                    "طلب الإلغاء لم يتم قبوله من قبل الإدارة.",
                    "المعلومات المعروضة لا تتطابق مع الواقع.",
                    "تم انتهاك قواعد السكن بوجود ضيوف إضافيين.",
                    "بلاغ عام حول مشاكل أخرى تتعلق بالخدمة."
                };
                var statuses = new[] { "Open", "InReview", "Resolved", "Dismissed" };
                var reportsToAdd = users.SelectMany(u =>
                {
                    int count = rnd.Next(1, 7);
                    return Enumerable.Range(1, count).Select(_ => new Report
                    {
                        Id = Guid.NewGuid(),
                        ReporterUserId = u.Id,
                        ReportedUserId = rnd.Next(2) == 0 ? users[rnd.Next(users.Count)].Id : (Guid?)null,
                        ReportedPropertyId = properties.Any() && rnd.Next(2) == 1
                            ? properties[rnd.Next(properties.Count)].Id : (Guid?)null,
                        Reason = reasons[rnd.Next(reasons.Length)],
                        Description = descriptions[rnd.Next(descriptions.Length)],
                        Status = statuses[rnd.Next(statuses.Length)],
                        CreatedAt = DateTime.UtcNow.AddDays(-rnd.Next(0, 30)),
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true,
                        ActionNote = string.Empty,
                        AdminId = null
                    });
                }).ToList();
                _context.Reports.AddRange(reportsToAdd);
                await _context.SaveChangesAsync();
            }

            // // Payments: seed random payments per booking for testing
            // if (!await _context.Payments.AnyAsync())
            // {
            //     var bookingsList = await _context.Bookings
            //         .AsNoTracking()
            //         .ToListAsync();
            //     var paymentSeeder = new PaymentSeeder(bookingsList, Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"));
            //     var seededPayments = paymentSeeder.SeedData().ToList();
            //     _context.Payments.AddRange(seededPayments);
            //     await _context.SaveChangesAsync();
            // }

            // Seed Arabic notifications for admin@example.com
            if (!await _context.Notifications.AnyAsync())
            {
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == "admin@example.com");
                if (admin != null)
                {
                    var now = DateTime.UtcNow;
                    var notifications = new List<Notification>
                    {
                        new Notification
                        {
                            Id = Guid.NewGuid(),
                            RecipientId = admin.Id,
                            Type = "BOOKING_CREATED",
                            Title = "حجز جديد",
                            Message = "تم إنشاء حجز جديد برقم HBK-2025-001",
                            TitleAr = "حجز جديد",
                            MessageAr = "تم إنشاء حجز جديد برقم HBK-2025-001",
                            Priority = "MEDIUM",
                            Data = "{\"bookingNumber\":\"HBK-2025-001\"}",
                            Channels = new List<string> { "IN_APP" },
                            CreatedAt = now.AddMinutes(-30),
                            UpdatedAt = now.AddMinutes(-30)
                        },
                        new Notification
                        {
                            Id = Guid.NewGuid(),
                            RecipientId = admin.Id,
                            Type = "BOOKING_CANCELLED",
                            Title = "إلغاء حجز",
                            Message = "تم إلغاء الحجز رقم HBK-2025-002",
                            TitleAr = "إلغاء حجز",
                            MessageAr = "تم إلغاء الحجز رقم HBK-2025-002",
                            Priority = "LOW",
                            Data = "{\"bookingNumber\":\"HBK-2025-002\"}",
                            Channels = new List<string> { "IN_APP" },
                            CreatedAt = now.AddHours(-2),
                            UpdatedAt = now.AddHours(-2)
                        },
                        new Notification
                        {
                            Id = Guid.NewGuid(),
                            RecipientId = admin.Id,
                            Type = "PAYMENT_UPDATE",
                            Title = "تحديث الدفع",
                            Message = "تم اعتماد دفعة بمبلغ 120,000 ريال يمني",
                            TitleAr = "تحديث الدفع",
                            MessageAr = "تم اعتماد دفعة بمبلغ 120,000 ريال يمني",
                            Priority = "HIGH",
                            Data = "{\"amount\":\"120000 YER\",\"status\":\"Approved\"}",
                            Channels = new List<string> { "IN_APP" },
                            CreatedAt = now.AddMinutes(-10),
                            UpdatedAt = now.AddMinutes(-10)
                        },
                        new Notification
                        {
                            Id = Guid.NewGuid(),
                            RecipientId = admin.Id,
                            Type = "PAYMENT_FAILED",
                            Title = "فشل عملية الدفع",
                            Message = "تعذر معالجة الدفعة الأخيرة، يرجى المحاولة لاحقاً",
                            TitleAr = "فشل عملية الدفع",
                            MessageAr = "تعذر معالجة الدفعة الأخيرة، يرجى المحاولة لاحقاً",
                            Priority = "URGENT",
                            RequiresAction = true,
                            Data = "{\"reason\":\"CardDeclined\"}",
                            Channels = new List<string> { "IN_APP" },
                            CreatedAt = now.AddMinutes(-5),
                            UpdatedAt = now.AddMinutes(-5)
                        },
                        new Notification
                        {
                            Id = Guid.NewGuid(),
                            RecipientId = admin.Id,
                            Type = "PROMOTION_OFFER",
                            Title = "عرض ترويجي جديد",
                            Message = "خصم 20% على الحجوزات لمدة محدودة",
                            TitleAr = "عرض ترويجي جديد",
                            MessageAr = "خصم 20% على الحجوزات لمدة محدودة",
                            Priority = "LOW",
                            Data = "{\"discount\":20,\"currency\":\"YER\"}",
                            Channels = new List<string> { "IN_APP" },
                            CreatedAt = now.AddDays(-1),
                            UpdatedAt = now.AddDays(-1)
                        },
                        new Notification
                        {
                            Id = Guid.NewGuid(),
                            RecipientId = admin.Id,
                            Type = "SYSTEM_UPDATE",
                            Title = "تحديث النظام",
                            Message = "تم تحديث النظام لتحسين الأداء والاستقرار",
                            TitleAr = "تحديث النظام",
                            MessageAr = "تم تحديث النظام لتحسين الأداء والاستقرار",
                            Priority = "MEDIUM",
                            Channels = new List<string> { "IN_APP" },
                            CreatedAt = now.AddDays(-2),
                            UpdatedAt = now.AddDays(-2)
                        },
                        new Notification
                        {
                            Id = Guid.NewGuid(),
                            RecipientId = admin.Id,
                            Type = "SECURITY_ALERT",
                            Title = "تنبيه أمني",
                            Message = "تم اكتشاف محاولة تسجيل دخول غير معتادة وتم حظرها",
                            TitleAr = "تنبيه أمني",
                            MessageAr = "تم اكتشاف محاولة تسجيل دخول غير معتادة وتم حظرها",
                            Priority = "HIGH",
                            Channels = new List<string> { "IN_APP" },
                            CreatedAt = now.AddHours(-6),
                            UpdatedAt = now.AddHours(-6)
                        }
                    };

                    _context.Notifications.AddRange(notifications);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}