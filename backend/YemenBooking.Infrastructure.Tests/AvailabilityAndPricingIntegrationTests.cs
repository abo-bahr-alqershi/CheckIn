using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using YemenBooking.Infrastructure.Services;
using YemenBooking.Application.Services;
using YemenBooking.Core.Interfaces.Repositories;
using Moq;
using Microsoft.AspNetCore.Http;
using YemenBooking.Infrastructure.Repositories;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Core.Entities;
using YemenBooking.Core.ValueObjects;
using YemenBooking.Application.Commands.CP.Availability;
using YemenBooking.Application.Commands.CP.Pricing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YemenBooking.Tests.Integration
{
    public class AvailabilityAndPricingIntegration : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly YemenBookingDbContext _context;
        private readonly Infrastructure.Services.AvailabilityService _availabilityService;
        private readonly Application.Services.PricingService _pricingService;

        public AvailabilityAndPricingIntegration()
        {
            var services = new ServiceCollection();
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<YemenBookingDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            
            services.AddScoped<UnitAvailabilityRepository>();
            services.AddScoped<PricingRuleRepository>();
            services.AddScoped<IUnitAvailabilityRepository, UnitAvailabilityRepository>();
            services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();
            services.AddScoped<Infrastructure.Services.AvailabilityService>();
            services.AddScoped<Application.Services.PricingService>();
            
            // Register concrete repositories
            services.AddScoped<IUnitRepository, UnitRepository>();
            
            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<YemenBookingDbContext>();
            _availabilityService = _serviceProvider.GetRequiredService<Infrastructure.Services.AvailabilityService>();
            _pricingService = _serviceProvider.GetRequiredService<Application.Services.PricingService>();
        }

        [Fact]
        public async Task CompleteBookingFlow_ShouldWorkEndToEnd()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(10);
            var checkOut = DateTime.Today.AddDays(15);
            
            // Setup unit
            var unit = new Unit
            {
                Id = unitId,
                PropertyId = Guid.NewGuid(),
                UnitTypeId = Guid.NewGuid(),
                Name = "Test Unit",
                BasePrice = new Money(100m, "USD"),
                IsAvailable = true,
                CustomFeatures = "{}",
                PricingMethod = YemenBooking.Core.Enums.PricingMethod.Daily
            };
            await _context.Set<Unit>().AddAsync(unit);
            
            // Setup pricing rules
            var pricingRule = new PricingRule
            {
                Id = Guid.NewGuid(),
                UnitId = unitId,
                StartDate = checkIn,
                EndDate = checkOut,
                PriceAmount = 150m,
                PriceType = "HighSeason",
                Currency = "USD",
                PricingTier = "1"
            };
            await _context.Set<PricingRule>().AddAsync(pricingRule);
            await _context.SaveChangesAsync();

            // Act & Assert - Check availability before booking
            var isAvailable = await _availabilityService.CheckAvailabilityAsync(unitId, checkIn, checkOut);
            isAvailable.Should().BeTrue();

            // Calculate price
            var price = await _pricingService.CalculatePriceAsync(unitId, checkIn, checkOut);
            price.Should().Be(750m); // 5 nights * 150

            // Block for booking
            await _availabilityService.BlockForBookingAsync(unitId, bookingId, checkIn, checkOut);

            // Check availability after booking
            var isStillAvailable = await _availabilityService.CheckAvailabilityAsync(unitId, checkIn, checkOut);
            isStillAvailable.Should().BeFalse();

            // Get pricing breakdown
            var breakdown = await _pricingService.GetPricingBreakdownAsync(unitId, checkIn, checkOut);
            breakdown.TotalNights.Should().Be(5);
            breakdown.Total.Should().Be(750m);

            // Release booking
            await _availabilityService.ReleaseBookingAsync(bookingId);

            // Check availability after release
            var isAvailableAgain = await _availabilityService.CheckAvailabilityAsync(unitId, checkIn, checkOut);
            isAvailableAgain.Should().BeTrue();
        }

        [Fact]
        public async Task BulkOperations_ShouldHandleComplexScenarios()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var unit = new Unit
            {
                Id = unitId,
                PropertyId = Guid.NewGuid(),
                UnitTypeId = Guid.NewGuid(),
                Name = "Test Unit",
                BasePrice = new Money(100m, "USD"),
                IsAvailable = true,
                CustomFeatures = "{}",
                PricingMethod = YemenBooking.Core.Enums.PricingMethod.Daily
            };
            await _context.Set<Unit>().AddAsync(unit);
            await _context.SaveChangesAsync();

            // Setup bulk availability
            var availabilityPeriods = new List<AvailabilityPeriodDto>
            {
                new AvailabilityPeriodDto
                {
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(10),
                    Status = "Maintenance",
                    Reason = "Annual Maintenance",
                    OverwriteExisting = true
                },
                new AvailabilityPeriodDto
                {
                    StartDate = DateTime.Today.AddDays(20),
                    EndDate = DateTime.Today.AddDays(25),
                    Status = "Blocked",
                    Reason = "Owner Use",
                    OverwriteExisting = false
                }
            };

            // Setup bulk pricing
            var pricingPeriods = new List<PricingPeriodDto>
            {
                new PricingPeriodDto
                {
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(31),
                    Price = 120m,
                    PriceType = "Standard",
                    Currency = "USD",
                    OverwriteExisting = true
                }
            };

            // Act
            await _availabilityService.ApplyBulkAvailabilityAsync(unitId, availabilityPeriods);
            await _pricingService.ApplyBulkPricingAsync(unitId, pricingPeriods);

            // Assert - Check calendars for the months that include the periods
            var firstAvailMid = availabilityPeriods[0].StartDate.AddDays(2).Date;
            var firstAvailCal = await _availabilityService.GetMonthlyCalendarAsync(
                unitId, firstAvailMid.Year, firstAvailMid.Month);
            firstAvailCal[firstAvailMid].Should().Be("Maintenance");

            var secondAvailMid = availabilityPeriods[1].StartDate.AddDays(2).Date;
            var secondAvailCal = await _availabilityService.GetMonthlyCalendarAsync(
                unitId, secondAvailMid.Year, secondAvailMid.Month);
            secondAvailCal[secondAvailMid].Should().Be("Blocked");
            
            // Verify pricing
            var priceMid = pricingPeriods[0].StartDate.AddDays(14).Date;
            var pricingCalendar = await _pricingService.GetPricingCalendarAsync(
                unitId, priceMid.Year, priceMid.Month);
            pricingCalendar[priceMid].Should().Be(120m);
        }

        [Fact]
        public async Task ConcurrentBookings_ShouldHandleRaceConditions()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var unit = new Unit
            {
                Id = unitId,
                PropertyId = Guid.NewGuid(),
                UnitTypeId = Guid.NewGuid(),
                Name = "Test Unit",
                BasePrice = new Money(100m, "USD"),
                IsAvailable = true,
                CustomFeatures = "{}",
                PricingMethod = YemenBooking.Core.Enums.PricingMethod.Daily
            };
            await _context.Set<Unit>().AddAsync(unit);
            await _context.SaveChangesAsync();

            var checkIn = DateTime.Today.AddDays(10);
            var checkOut = DateTime.Today.AddDays(15);
            
            // Act - Simulate concurrent booking attempts
            var booking1Task = Task.Run(async () =>
            {
                await Task.Delay(Random.Shared.Next(10, 50));
                return await _availabilityService.CheckAvailabilityAsync(unitId, checkIn, checkOut);
            });

            var booking2Task = Task.Run(async () =>
            {
                await Task.Delay(Random.Shared.Next(10, 50));
                await _availabilityService.BlockForBookingAsync(unitId, Guid.NewGuid(), checkIn, checkOut);
            });

            await Task.WhenAll(booking1Task, booking2Task);

            // Assert - After one booking, unit should not be available
            var finalAvailability = await _availabilityService.CheckAvailabilityAsync(unitId, checkIn, checkOut);
            finalAvailability.Should().BeFalse();
        }

        public void Dispose()
        {
            _context?.Dispose();
            _serviceProvider?.Dispose();
        }
    }
}