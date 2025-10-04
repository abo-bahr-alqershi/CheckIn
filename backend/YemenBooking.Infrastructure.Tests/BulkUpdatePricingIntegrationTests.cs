using System;
using System.Threading.Tasks;
// using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using Xunit;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Infrastructure.Repositories;
using YemenBooking.Application.Handlers.Commands.Pricing;
using YemenBooking.Application.Commands.CP.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Tests.Integration
{
    public class BulkUpdatePricingIntegrationTests
    {
        [Fact]
        public async Task BulkUpdate_WithYER_Currency_ShouldSucceed()
        {
            // Arrange: switched off in-memory SQLite for SQL Server environment
            var options = new DbContextOptionsBuilder<YemenBookingDbContext>()
                .UseSqlServer("Data Source=SQL5107.site4now.net;Initial Catalog=db_abd8fd_bookn2;User Id=db_abd8fd_bookn2_admin;Password=Qaz123@Wsx123@")
                .Options;

            // Create context and apply migrations
            await using var context = new YemenBookingDbContext(options, new HttpContextAccessor());
            await context.Database.MigrateAsync();

            // Seed a unit with base currency YER
            var unit = new Unit
            {
                Id = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                UnitTypeId = Guid.NewGuid(),
                Name = "TestUnit",
                BasePrice = new Money(100m, "YER"),
                MaxCapacity = 2,
                CustomFeatures = "{}",
                PricingMethod = Core.Enums.PricingMethod.Daily,
                IsAvailable = true
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();

            // Prepare handler with real repositories
            var pricingRepo = new PricingRuleRepository(context);
            var unitRepo = new UnitRepository(context);
            var handler = new BulkUpdatePricingCommandHandler(pricingRepo, unitRepo, NullLogger<BulkUpdatePricingCommandHandler>.Instance);

            // Create command with one period using default YER currency (null/empty input)
            var command = new BulkUpdatePricingCommand
            {
                UnitId = unit.Id,
                OverwriteExisting = true,
                Periods = new System.Collections.Generic.List<PricingPeriodDto>
                {
                    new PricingPeriodDto
                    {
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(1),
                        StartTime = null,
                        EndTime = null,
                        PriceType = "Custom",
                        Price = 120m,
                        Currency = null, // use unit base currency
                        Tier = "1",
                        PercentageChange = null,
                        MinPrice = null,
                        MaxPrice = null,
                        Description = "Integration test",
                        OverwriteExisting = true
                    }
                }
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess, result.Message);
            var saved = await context.Set<PricingRule>().FirstOrDefaultAsync(r => r.UnitId == unit.Id);
            Assert.NotNull(saved);
            Assert.Equal("YER", saved.Currency);
            Assert.Equal(120m, saved.PriceAmount);
        }
    }
}