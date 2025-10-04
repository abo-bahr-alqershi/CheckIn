using System;
using System.Collections.Generic;
using Bogus;
using YemenBooking.Core.Entities;
using YemenBooking.Core.ValueObjects;
using YemenBooking.Core.Enums;

namespace YemenBooking.Core.Seeds
{
    /// <summary>
    /// مولد البيانات الأولية لكائن Unit
    /// </summary>
    public class UnitSeeder : ISeeder<Unit>
    {
        public IEnumerable<Unit> SeedData()
        {
            return new Faker<Unit>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.PropertyId, f => Guid.NewGuid())
                .RuleFor(u => u.UnitTypeId, f => Guid.NewGuid())
                .RuleFor(u => u.Name, f => f.Commerce.ProductName())
                .RuleFor(u => u.BasePrice, f => new Money(f.Finance.Amount(50, 500), "YER"))
                .RuleFor(u => u.MaxCapacity, f => f.Random.Number(1, 10))
                .RuleFor(u => u.CustomFeatures, f => "[]")
                .RuleFor(u => u.IsAvailable, f => f.Random.Bool(0.8f))
                .RuleFor(u => u.ViewCount, f => f.Random.Number(0, 500))
                .RuleFor(u => u.BookingCount, f => f.Random.Number(0, 200))
                .RuleFor(u => u.PricingMethod, f => f.PickRandom<PricingMethod>())
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(30))
                .RuleFor(u => u.UpdatedAt, f => f.Date.Recent(30))
                .Generate(30);
        }
    }
} 