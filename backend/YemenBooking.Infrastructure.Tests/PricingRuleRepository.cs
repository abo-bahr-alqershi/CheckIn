using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using YemenBooking.Infrastructure.Repositories;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Core.Entities;
using YemenBooking.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemenBooking.Tests.Repositories
{
    public class PricingRuleRepositoryTests : IDisposable
    {
        private readonly YemenBookingDbContext _context;
        private readonly PricingRuleRepository _repository;

        public PricingRuleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<YemenBookingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new YemenBookingDbContext(options, new HttpContextAccessor());
            _repository = new PricingRuleRepository(_context);
        }

        #region GetByUnitIdAsync Tests

        [Fact]
        public async Task GetByUnitIdAsync_WithNoDateFilters_ShouldReturnAllRulesForUnit()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(5)),
                CreatePricingRule(unitId, DateTime.Today.AddDays(10), DateTime.Today.AddDays(15)),
                CreatePricingRule(Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(5)) // Different unit
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId);

            // Assert
            result.Should().HaveCount(2);
            result.All(r => r.UnitId == unitId).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUnitIdAsync_WithStartDate_ShouldFilterByStartDate()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var filterDate = DateTime.Today.AddDays(7);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(5)), // Ends before filter
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10)), // Overlaps filter
                CreatePricingRule(unitId, DateTime.Today.AddDays(8), DateTime.Today.AddDays(15)) // Starts after filter
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId, startDate: filterDate);

            // Assert
            result.Should().HaveCount(2);
            result.All(r => r.EndDate >= filterDate).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUnitIdAsync_WithEndDate_ShouldFilterByEndDate()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var filterDate = DateTime.Today.AddDays(7);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(5)), // Starts before filter
                CreatePricingRule(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(10)), // Overlaps filter
                CreatePricingRule(unitId, DateTime.Today.AddDays(10), DateTime.Today.AddDays(15)) // Starts after filter
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId, endDate: filterDate);

            // Assert
            result.Should().HaveCount(2);
            result.All(r => r.StartDate <= filterDate).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUnitIdAsync_ShouldExcludeDeletedRules()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(5)),
                CreatePricingRule(unitId, DateTime.Today.AddDays(10), DateTime.Today.AddDays(15), isDeleted: true)
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId);

            // Assert
            result.Should().HaveCount(1);
            result.All(r => !r.IsDeleted).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUnitIdAsync_ShouldOrderByStartDateAndTier()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today.AddDays(5), DateTime.Today.AddDays(10), tier: "2"),
                CreatePricingRule(unitId, DateTime.Today.AddDays(5), DateTime.Today.AddDays(10), tier: "1"),
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(5), tier: "1")
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId);

            // Assert
            result.Should().HaveCount(3);
            var resultList = result.ToList();
            resultList[0].StartDate.Should().Be(DateTime.Today);
            resultList[1].PricingTier.Should().Be("1");
            resultList[2].PricingTier.Should().Be("2");
        }

        #endregion

        #region GetActiveRulesAsync Tests

        [Fact]
        public async Task GetActiveRulesAsync_ShouldReturnRulesActiveOnDate()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var targetDate = DateTime.Today.AddDays(5);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(3)), // Before target
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10)), // Includes target
                CreatePricingRule(unitId, DateTime.Today.AddDays(7), DateTime.Today.AddDays(15)) // After target
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveRulesAsync(unitId, targetDate);

            // Assert
            result.Should().HaveCount(1);
            var rule = result.First();
            rule.StartDate.Should().BeOnOrBefore(targetDate);
            rule.EndDate.Should().BeOnOrAfter(targetDate);
        }

        [Fact]
        public async Task GetActiveRulesAsync_WithMultipleTiers_ShouldOrderByTier()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var targetDate = DateTime.Today.AddDays(5);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10), tier: "3"),
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10), tier: "1"),
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10), tier: "2")
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveRulesAsync(unitId, targetDate);

            // Assert
            result.Should().HaveCount(3);
            var resultList = result.ToList();
            resultList[0].PricingTier.Should().Be("1");
            resultList[1].PricingTier.Should().Be("2");
            resultList[2].PricingTier.Should().Be("3");
        }

        #endregion

        #region GetPriceForDateAsync Tests

        [Fact]
        public async Task GetPriceForDateAsync_WithMultipleRules_ShouldReturnHighestPriority()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var targetDate = DateTime.Today.AddDays(5);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10), tier: "2", price: 200m),
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10), tier: "1", price: 150m),
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(10), tier: "3", price: 250m)
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPriceForDateAsync(unitId, targetDate);

            // Assert
            result.Should().NotBeNull();
            result.PricingTier.Should().Be("1");
            result.PriceAmount.Should().Be(150m);
        }

        [Fact]
        public async Task GetPriceForDateAsync_WithNoMatchingRules_ShouldReturnNull()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var targetDate = DateTime.Today.AddDays(5);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(3)), // Before target
                CreatePricingRule(unitId, DateTime.Today.AddDays(7), DateTime.Today.AddDays(10)) // After target
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPriceForDateAsync(unitId, targetDate);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetByDateRangeAsync Tests

        [Fact]
        public async Task GetByDateRangeAsync_ShouldReturnOverlappingRules()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var startDate = DateTime.Today.AddDays(5);
            var endDate = DateTime.Today.AddDays(10);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(3)), // Before range
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(7)), // Overlaps start
                CreatePricingRule(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(9)), // Inside range
                CreatePricingRule(unitId, DateTime.Today.AddDays(8), DateTime.Today.AddDays(15)), // Overlaps end
                CreatePricingRule(unitId, DateTime.Today.AddDays(12), DateTime.Today.AddDays(20)) // After range
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateRangeAsync(unitId, startDate, endDate);

            // Assert
            result.Should().HaveCount(3);
        }

        #endregion

        #region BulkCreateAsync Tests

        [Fact]
        public async Task BulkCreateAsync_ShouldAddAllRules()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(5)),
                CreatePricingRule(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(10)),
                CreatePricingRule(unitId, DateTime.Today.AddDays(11), DateTime.Today.AddDays(15))
            };

            // Act
            await _repository.BulkCreateAsync(rules);

            // Assert
            var savedRules = await _context.Set<PricingRule>().Where(r => r.UnitId == unitId).ToListAsync();
            savedRules.Should().HaveCount(3);
        }

        [Fact]
        public async Task BulkCreateAsync_WithEmptyList_ShouldNotFail()
        {
            // Arrange
            var rules = new List<PricingRule>();

            // Act
            Func<Task> act = async () => await _repository.BulkCreateAsync(rules);

            // Assert
            await act.Should().NotThrowAsync();
        }

        #endregion

        #region BulkUpdateAsync Tests

        [Fact]
        public async Task BulkUpdateAsync_ShouldUpdateAllRules()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(5), price: 100m),
                CreatePricingRule(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(10), price: 100m)
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();
            
            // Detach entities to simulate disconnected scenario
            _context.ChangeTracker.Clear();
            
            // Modify rules
            rules[0].PriceAmount = 150m;
            rules[1].PriceAmount = 200m;

            // Act
            await _repository.BulkUpdateAsync(rules);

            // Assert
            var updatedRules = await _context.Set<PricingRule>().Where(r => r.UnitId == unitId).ToListAsync();
            updatedRules[0].PriceAmount.Should().Be(150m);
            updatedRules[1].PriceAmount.Should().Be(200m);
        }

        #endregion

        #region DeleteRangeAsync Tests

        [Fact]
        public async Task DeleteRangeAsync_ShouldSoftDeleteRulesInRange()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var deleteStart = DateTime.Today.AddDays(5);
            var deleteEnd = DateTime.Today.AddDays(10);
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, DateTime.Today, DateTime.Today.AddDays(3)), // Outside range
                CreatePricingRule(unitId, DateTime.Today.AddDays(5), DateTime.Today.AddDays(10)), // Exact match
                CreatePricingRule(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(9)), // Inside range
                CreatePricingRule(unitId, DateTime.Today.AddDays(11), DateTime.Today.AddDays(15)) // Outside range
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteRangeAsync(unitId, deleteStart, deleteEnd);

            // Assert
            var allRules = await _context.Set<PricingRule>().Where(r => r.UnitId == unitId).ToListAsync();
            allRules.Should().HaveCount(4);
            allRules.Count(r => r.IsDeleted).Should().Be(2);
            allRules.Where(r => r.IsDeleted).All(r => r.DeletedAt.HasValue).Should().BeTrue();
        }

        [Fact]
        public async Task DeleteRangeAsync_WithNoMatchingRules_ShouldNotFail()
        {
            // Arrange
            var unitId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _repository.DeleteRangeAsync(unitId, DateTime.Today, DateTime.Today.AddDays(5));

            // Assert
            await act.Should().NotThrowAsync();
        }

        #endregion

        #region GetPricingCalendarAsync Tests

        [Fact]
        public async Task GetPricingCalendarAsync_ShouldReturnPricesForEachDay()
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
            
            var year = 2024;
            var month = 3;
            var rules = new List<PricingRule>
            {
                CreatePricingRule(unitId, new DateTime(2024, 3, 5), new DateTime(2024, 3, 10), price: 150m, tier: "1"),
                CreatePricingRule(unitId, new DateTime(2024, 3, 8), new DateTime(2024, 3, 12), price: 200m, tier: "2") // Lower priority
            };
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPricingCalendarAsync(unitId, year, month);

            // Assert
            result.Should().HaveCount(31); // March has 31 days
            result[new DateTime(2024, 3, 1)].Should().Be(100m); // Base price
            result[new DateTime(2024, 3, 5)].Should().Be(150m); // Rule price
            result[new DateTime(2024, 3, 8)].Should().Be(150m); // Higher priority rule
            result[new DateTime(2024, 3, 11)].Should().Be(200m); // Only second rule active
            result[new DateTime(2024, 3, 15)].Should().Be(100m); // Back to base price
        }

        [Fact]
        public async Task GetPricingCalendarAsync_WithNoUnit_ShouldUseZeroBasePrice()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var year = 2024;
            var month = 3;

            // Act
            var result = await _repository.GetPricingCalendarAsync(unitId, year, month);

            // Assert
            result.Should().HaveCount(31);
            result.Values.All(v => v == 0m).Should().BeTrue();
        }

        [Fact]
        public async Task GetPricingCalendarAsync_ForFebruary_ShouldHandleLeapYear()
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

            // Act
            var result2024 = await _repository.GetPricingCalendarAsync(unitId, 2024, 2); // Leap year
            var result2023 = await _repository.GetPricingCalendarAsync(unitId, 2023, 2); // Non-leap year

            // Assert
            result2024.Should().HaveCount(29);
            result2023.Should().HaveCount(28);
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task Repository_ShouldHandleLargeDataSets()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var rules = new List<PricingRule>();
            
            // Create 1000 rules
            for (int i = 0; i < 1000; i++)
            {
                rules.Add(CreatePricingRule(unitId, DateTime.Today.AddDays(i), DateTime.Today.AddDays(i + 1)));
            }
            
            await _context.Set<PricingRule>().AddRangeAsync(rules);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId);

            // Assert
            result.Should().HaveCount(1000);
        }

        #endregion

        #region Helper Methods

        private PricingRule CreatePricingRule(
            Guid unitId, 
            DateTime startDate, 
            DateTime endDate,
            string tier = "1",
            decimal price = 100m,
            bool isDeleted = false)
        {
            return new PricingRule
            {
                Id = Guid.NewGuid(),
                UnitId = unitId,
                StartDate = startDate,
                EndDate = endDate,
                PricingTier = tier,
                PriceAmount = price,
                PriceType = "Standard",
                Currency = "USD",
                IsDeleted = isDeleted,
                CreatedAt = DateTime.UtcNow
            };
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}