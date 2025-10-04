using System;
using System.Collections.Generic;
using Bogus;
using YemenBooking.Core.Entities;

namespace YemenBooking.Core.Seeds
{
    /// <summary>
    /// مولد البيانات الأولية لكائن Property
    /// </summary>
    public class PropertySeeder : ISeeder<Property>
    {
        public IEnumerable<Property> SeedData()
        {
            return new Faker<Property>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.OwnerId, f => Guid.NewGuid())
                .RuleFor(p => p.TypeId, f => Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Company.CompanyName())
                .RuleFor(p => p.Address, f => f.Address.FullAddress())
                .RuleFor(p => p.City, f => f.Address.City())
                .RuleFor(p => p.Latitude, f => (decimal)f.Address.Latitude())
                .RuleFor(p => p.Longitude, f => (decimal)f.Address.Longitude())
                .RuleFor(p => p.StarRating, f => f.Random.Number(1, 5))
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.IsApproved, f => f.Random.Bool(0.9f))
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(1))
                .RuleFor(p => p.ViewCount, f => f.Random.Number(0, 1000))
                .RuleFor(p => p.BookingCount, f => f.Random.Number(0, 500))
                // أسعار غير صفرية وعملة افتراضية
                .RuleFor(p => p.BasePricePerNight, f => Math.Round((decimal)f.Random.Double(10, 200) * 1000m, 2))
                .RuleFor(p => p.Currency, f => "YER")
                .Generate(20);
        }
    }
} 