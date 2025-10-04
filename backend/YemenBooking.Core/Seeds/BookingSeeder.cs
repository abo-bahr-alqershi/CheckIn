using System;
using System.Collections.Generic;
using Bogus;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Core.Seeds
{
    /// <summary>
    /// مولد البيانات الأولية لكائن Booking
    /// </summary>
    public class BookingSeeder : ISeeder<Booking>
    {
        public IEnumerable<Booking> SeedData()
        {
            return new Faker<Booking>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.UnitId, f => f.Random.Guid())
                .RuleFor(b => b.UserId, f => f.Random.Guid())
                .RuleFor(b => b.Status, f => f.PickRandom<BookingStatus>())
                .RuleFor(b => b.CheckIn, f => f.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now))
                .RuleFor(b => b.CheckOut, (f, b) => b.CheckIn.AddDays(f.Random.Int(1, 7)))
                .RuleFor(b => b.GuestsCount, f => f.Random.Int(1, 4))
                .RuleFor(b => b.TotalPrice, f => Money.Usd(f.Finance.Amount(100, 1000)))
                .RuleFor(b => b.BookedAt, (f, b) => f.Date.Between(b.CheckIn, b.CheckOut))
                .RuleFor(b => b.PlatformCommissionAmount, f => f.Random.Decimal(0, 100))
                .RuleFor(b => b.FinalAmount, (f, b) => b.TotalPrice.Amount + b.PlatformCommissionAmount)
                .Generate(20);
        }
    }
} 