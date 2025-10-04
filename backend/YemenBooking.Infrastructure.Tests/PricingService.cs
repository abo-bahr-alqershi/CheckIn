using Xunit;
using Moq;
using FluentAssertions;
using YemenBooking.Application.Services;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Commands.CP.Pricing;
using YemenBooking.Application.Queries.CP.Pricing;
using YemenBooking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YemenBooking.Application.Features.PricingRules.Commands;
using System.Threading;

namespace YemenBooking.Tests.Services
{
    public class PricingServiceTests
    {
        private readonly Mock<IPricingRuleRepository> _pricingRepositoryMock;
        private readonly Mock<IUnitRepository> _unitRepositoryMock;
        private readonly PricingService _service;

        public PricingServiceTests()
        {
            _pricingRepositoryMock = new Mock<IPricingRuleRepository>();
            _unitRepositoryMock = new Mock<IUnitRepository>();
            _service = new Application.Services.PricingService(
                _pricingRepositoryMock.Object,
                _unitRepositoryMock.Object);
        }

        #region CalculatePriceAsync Tests

        [Fact]
        public async Task CalculatePriceAsync_WhenUnitNotFound_ShouldThrowException()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Unit)null!);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _service.CalculatePriceAsync(unitId, checkIn, checkOut));
        }

        [Fact]
        public async Task CalculatePriceAsync_WithNoSpecialRules_ShouldUseBasePrice()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(4); // 3 nights
            var basePrice = 100m;
            var unit = new Unit 
            { 
                Id = unitId,
                PropertyId = Guid.NewGuid(),
                UnitTypeId = Guid.NewGuid(),
                Name = "Test Unit",
                BasePrice = new Money(basePrice, "USD"),
                IsAvailable = true,
                CustomFeatures = "{}",
                PricingMethod = YemenBooking.Core.Enums.PricingMethod.Daily
            };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync((PricingRule?)null);

            // Act
            var result = await _service.CalculatePriceAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().Be(basePrice * 3); // 3 nights * 100
        }

        [Fact]
        public async Task CalculatePriceAsync_WithFixedPriceRule_ShouldUseRulePrice()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(3); // 2 nights
            var basePrice = 100m;
            var rulePrice = 150m;
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(basePrice, "USD")
            };
            var rule = new PricingRule
            {
                PriceAmount = rulePrice,
                PercentageChange = null
            };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.CalculatePriceAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().Be(rulePrice * 2); // 2 nights * 150
        }

        [Fact]
        public async Task CalculatePriceAsync_WithPercentageIncreaseRule_ShouldApplyPercentage()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(2); // 1 night
            var basePrice = 100m;
            var percentageIncrease = 20m; // 20% increase
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(basePrice, "USD")
            };
            var rule = new PricingRule
            {
                PriceAmount = 0, // Should be ignored when percentage is set
                PercentageChange = percentageIncrease
            };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.CalculatePriceAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().Be(120m); // 100 + (100 * 20%)
        }

        [Fact]
        public async Task CalculatePriceAsync_WithPercentageDecreaseRule_ShouldApplyDiscount()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(2); // 1 night
            var basePrice = 100m;
            var percentageDecrease = -30m; // 30% discount
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(basePrice, "USD")
            };
            var rule = new PricingRule
            {
                PercentageChange = percentageDecrease
            };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.CalculatePriceAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().Be(70m); // 100 - (100 * 30%)
        }

        [Fact]
        public async Task CalculatePriceAsync_WithMixedRules_ShouldCalculateCorrectly()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(4); // 3 nights
            var basePrice = 100m;
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(basePrice, "USD")
            };

            // Different rules for different days
            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            
            var ruleDay1 = new PricingRule { PriceAmount = 150m };
            var ruleDay2 = (PricingRule?)null;
            var ruleDay3 = new PricingRule { PercentageChange = 50m };
            
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync((Guid u, DateTime d) =>
                {
                    if (d == checkIn.Date) return ruleDay1;
                    if (d == checkIn.Date.AddDays(1)) return ruleDay2;
                    if (d == checkIn.Date.AddDays(2)) return ruleDay3;
                    return null;
                });

            // Act
            var result = await _service.CalculatePriceAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().Be(150m + 100m + 150m); // 400 total
        }

        [Fact]
        public async Task CalculatePriceAsync_WithSameDayCheckInCheckOut_ShouldReturnZero()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var date = DateTime.Today.AddDays(1);
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(100m, "USD")
            };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);

            // Act
            var result = await _service.CalculatePriceAsync(unitId, date, date);

            // Assert
            result.Should().Be(0m);
        }

        [Theory]
        [InlineData(100, 0, 100)]      // No change
        [InlineData(100, 100, 200)]    // 100% increase
        [InlineData(100, -50, 50)]     // 50% discount
        [InlineData(100, -100, 0)]     // 100% discount (free)
        [InlineData(100, 200, 300)]    // 200% increase
        public async Task CalculatePriceAsync_WithVariousPercentages_ShouldCalculateCorrectly(
            decimal basePrice, decimal percentageChange, decimal expectedPrice)
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(2); // 1 night
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(basePrice, "USD")
            };
            var rule = new PricingRule { PercentageChange = percentageChange };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.CalculatePriceAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().Be(expectedPrice);
        }

        #endregion

        #region GetPricingCalendarAsync Tests

        [Fact]
        public async Task GetPricingCalendarAsync_ShouldReturnCalendarFromRepository()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var year = 2024;
            var month = 3;
            var expectedCalendar = new Dictionary<DateTime, decimal>
            {
                { new DateTime(2024, 3, 1), 100m },
                { new DateTime(2024, 3, 2), 120m }
            };

            _pricingRepositoryMock.Setup(x => x.GetPricingCalendarAsync(unitId, year, month))
                .ReturnsAsync(expectedCalendar);

            // Act
            var result = await _service.GetPricingCalendarAsync(unitId, year, month);

            // Assert
            result.Should().BeEquivalentTo(expectedCalendar);
        }

        #endregion

        #region ApplySeasonalPricingAsync Tests

        [Fact]
        public async Task ApplySeasonalPricingAsync_ShouldCreateRulesForAllSeasons()
        {
            var unitId = Guid.NewGuid();
            var seasonalPricing = new SeasonalPricingDto
            {
                Currency = "USD",
                Seasons = new List<YemenBooking.Application.Features.PricingRules.Commands.SeasonDto>
                {
                    new SeasonDto
                    {
                        Type = "High",
                        StartDate = DateTime.Today.AddDays(1),
                        EndDate = DateTime.Today.AddDays(30),
                        Price = 200m,
                        Priority = 1,
                        Description = "High Season"
                    },
                    new SeasonDto
                    {
                        Type = "Low",
                        StartDate = DateTime.Today.AddDays(31),
                        EndDate = DateTime.Today.AddDays(60),
                        Price = 80m,
                        Priority = 2,
                        PercentageChange = -20m,
                        MinPrice = 50m,
                        MaxPrice = 100m,
                        Description = "Low Season"
                    }
                }
            };

            List<PricingRule> capturedRules = null;
            _pricingRepositoryMock.Setup(x => x.BulkCreateAsync(It.IsAny<List<PricingRule>>()))
                .Callback<IEnumerable<PricingRule>>(r => capturedRules = r.ToList())
                .Returns(Task.CompletedTask);

            await _service.ApplySeasonalPricingAsync(unitId, seasonalPricing);

            capturedRules.Should().HaveCount(2);
            var highSeasonRule = capturedRules[0];
            highSeasonRule.UnitId.Should().Be(unitId);
            highSeasonRule.PriceType.Should().Be("High");
            highSeasonRule.PriceAmount.Should().Be(200m);
            highSeasonRule.PricingTier.Should().Be("1");
            highSeasonRule.Currency.Should().Be("USD");
            
            var lowSeasonRule = capturedRules[1];
            lowSeasonRule.PriceType.Should().Be("Low");
            lowSeasonRule.PriceAmount.Should().Be(80m);
            lowSeasonRule.PercentageChange.Should().Be(-20m);
            lowSeasonRule.MinPrice.Should().Be(50m);
            lowSeasonRule.MaxPrice.Should().Be(100m);
        }

        [Fact]
        public async Task ApplySeasonalPricingAsync_WithEmptySeasons_ShouldNotFail()
        {
            var unitId = Guid.NewGuid();
            var seasonalPricing = new SeasonalPricingDto
            {
                Currency = "USD",
                Seasons = new List<YemenBooking.Application.Features.PricingRules.Commands.SeasonDto>()
            };

            await _service.ApplySeasonalPricingAsync(unitId, seasonalPricing);

            _pricingRepositoryMock.Verify(x => x.BulkCreateAsync(It.Is<List<PricingRule>>(l => l.Count == 0)), Times.Once);
        }

        #endregion

        #region ApplyBulkPricingAsync Tests

        [Fact]
        public async Task ApplyBulkPricingAsync_WithOverwrite_ShouldDeleteExistingAndCreateNew()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var periods = new List<PricingPeriodDto>
            {
                new PricingPeriodDto
                {
                    PriceType = "Weekend",
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(11, 0, 0),
                    Price = 150m,
                    Tier = "Premium",
                    Currency = "USD",
                    OverwriteExisting = true
                }
            };

            List<PricingRule> capturedRules = null;
            _pricingRepositoryMock.Setup(x => x.BulkCreateAsync(It.IsAny<List<PricingRule>>()))
                .Callback<IEnumerable<PricingRule>>(r => capturedRules = r.ToList())
                .Returns(Task.CompletedTask);

            // Act
            await _service.ApplyBulkPricingAsync(unitId, periods);

            // Assert
            _pricingRepositoryMock.Verify(x => x.DeleteRangeAsync(unitId, periods[0].StartDate, periods[0].EndDate), Times.Once);
            capturedRules.Should().HaveCount(1);
            capturedRules[0].StartTime.Should().Be(new TimeSpan(14, 0, 0));
            capturedRules[0].EndTime.Should().Be(new TimeSpan(11, 0, 0));
        }

        [Fact]
        public async Task ApplyBulkPricingAsync_WithMultiplePeriods_ShouldProcessAll()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var periods = new List<PricingPeriodDto>
            {
                new PricingPeriodDto
                {
                    PriceType = "Weekday",
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(5),
                    Price = 100m,
                    OverwriteExisting = true
                },
                new PricingPeriodDto
                {
                    PriceType = "Weekend",
                    StartDate = DateTime.Today.AddDays(6),
                    EndDate = DateTime.Today.AddDays(7),
                    Price = 150m,
                    PercentageChange = 50m,
                    MinPrice = 100m,
                    MaxPrice = 200m,
                    OverwriteExisting = false
                }
            };

            List<PricingRule> capturedRules = null;
            _pricingRepositoryMock.Setup(x => x.BulkCreateAsync(It.IsAny<List<PricingRule>>()))
                .Callback<IEnumerable<PricingRule>>(r => capturedRules = r.ToList())
                .Returns(Task.CompletedTask);

            // Act
            await _service.ApplyBulkPricingAsync(unitId, periods);

            // Assert
            _pricingRepositoryMock.Verify(x => x.DeleteRangeAsync(unitId, periods[0].StartDate, periods[0].EndDate), Times.Once);
            _pricingRepositoryMock.Verify(x => x.DeleteRangeAsync(unitId, periods[1].StartDate, periods[1].EndDate), Times.Never);
            capturedRules.Should().HaveCount(2);
        }

        #endregion

        #region GetPricingBreakdownAsync Tests

        [Fact]
        public async Task GetPricingBreakdownAsync_ShouldReturnDetailedBreakdown()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(4); // 3 nights
            var basePrice = 100m;
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(basePrice, "USD")
            };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            
            // Day 1: Special price
            var ruleDay1 = new PricingRule 
                { 
                    PriceAmount = 150m, 
                    PriceType = "Weekend",
                    Description = "Weekend Rate"
            };
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.Is<DateTime>(d => d == checkIn.Date)))
                .ReturnsAsync(ruleDay1);
            
            // Day 2: Base price
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.Is<DateTime>(d => d == checkIn.Date.AddDays(1))))
                .ReturnsAsync((PricingRule?)null);
            
            // Day 3: Discount
            var ruleDay3 = new PricingRule 
                { 
                    PercentageChange = -20m,
                    PriceType = "Discount",
                    Description = "Special Discount"
            };
            _pricingRepositoryMock.Setup(x => x.GetPriceForDateAsync(It.IsAny<Guid>(), It.Is<DateTime>(d => d == checkIn.Date.AddDays(2))))
                .ReturnsAsync(ruleDay3);

            // Act
            var result = await _service.GetPricingBreakdownAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().NotBeNull();
            result.CheckIn.Should().Be(checkIn);
            result.CheckOut.Should().Be(checkOut);
            result.Currency.Should().Be("USD");
            result.TotalNights.Should().Be(3);
            result.Days.Should().HaveCount(3);
            
            result.Days[0].Date.Should().Be(checkIn.Date);
            result.Days[0].Price.Should().Be(150m);
            result.Days[0].PriceType.Should().Be("Weekend");
            result.Days[0].Description.Should().Be("Weekend Rate");
            
            result.Days[1].Price.Should().Be(100m);
            result.Days[1].PriceType.Should().Be("Base");
            
            result.Days[2].Price.Should().Be(80m); // 100 - 20%
            result.Days[2].PriceType.Should().Be("Discount");
            
            result.SubTotal.Should().Be(330m); // 150 + 100 + 80
            result.Total.Should().Be(330m);
        }

        [Fact]
        public async Task GetPricingBreakdownAsync_WithZeroNights_ShouldReturnEmptyBreakdown()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var date = DateTime.Today.AddDays(1);
            var unit = new Unit 
            { 
                Id = unitId,
                BasePrice = new Money(100m, "USD")
            };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);

            // Act
            var result = await _service.GetPricingBreakdownAsync(unitId, date, date);

            // Assert
            result.TotalNights.Should().Be(0);
            result.Days.Should().BeEmpty();
            result.SubTotal.Should().Be(0m);
            result.Total.Should().Be(0m);
        }

        [Fact]
        public async Task GetPricingBreakdownAsync_WhenUnitNotFound_ShouldThrowException()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(3);

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Unit)null!);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _service.GetPricingBreakdownAsync(unitId, checkIn, checkOut));
        }

        #endregion

        // Helper DTOs for tests to satisfy IPricingService expectations
        private class SeasonalPricingDto : YemenBooking.Application.Features.PricingRules.Commands.SeasonalPricingDto
        {
            public SeasonalPricingDto()
            {
                Currency = "USD";
                Seasons = new List<YemenBooking.Application.Features.PricingRules.Commands.SeasonDto>();
            }
        }

        private class SeasonDto : YemenBooking.Application.Features.PricingRules.Commands.SeasonDto
        {
            public SeasonDto()
            {
                Type = string.Empty;
            }
        }
    }
}