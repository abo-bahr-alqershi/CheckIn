using System;
using System.Collections.Generic;
using Bogus;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Core.Seeds
{
    /// <summary>
    /// مولد البيانات الأولية لكائن Payment مع ربطه بالحجوزات
    /// Generates seed data for Payment entities linked to seeded bookings
    /// </summary>
    public class PaymentSeeder : ISeeder<Payment>
    {
        private readonly IEnumerable<Booking> _bookings;
        private readonly Guid _processedByUserId;

        public PaymentSeeder(IEnumerable<Booking> bookings, Guid processedByUserId)
        {
            _bookings = bookings;
            _processedByUserId = processedByUserId;
        }

        public IEnumerable<Payment> SeedData()
        {
            var payments = new List<Payment>();
            var random = new Random();
            foreach (var booking in _bookings)
            {
                int count = random.Next(1, 4); // 1-3 payments per booking
                for (int i = 0; i < count; i++)
                {
                    var faker = new Faker<Payment>()
                        .RuleFor(p => p.Id, f => Guid.NewGuid())
                        .RuleFor(p => p.BookingId, f => booking.Id)
                        .RuleFor(p => p.Amount, f => new Money(f.Finance.Amount(50, 500), "USD"))
                        .RuleFor(p => p.Method, f => f.PickRandom<PaymentMethod>())
                        .RuleFor(p => p.TransactionId, f => f.Random.Replace("TXN-####-####"))
                        .RuleFor(p => p.Status, f => f.PickRandom<PaymentStatus>())
                        .RuleFor(p => p.PaymentDate, f => f.Date.Between(booking.CheckIn.AddDays(-1), booking.CheckOut.AddDays(30)))
                        .RuleFor(p => p.GatewayTransactionId, f => f.Random.Replace("GW-####"))
                        .RuleFor(p => p.ProcessedBy, f => _processedByUserId);
                    payments.Add(faker.Generate());
                }
            }
            return payments;
        }
    }
} 