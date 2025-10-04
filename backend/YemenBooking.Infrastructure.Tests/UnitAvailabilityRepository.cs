using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using YemenBooking.Infrastructure.Repositories;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemenBooking.Tests.Repositories
{
    public class UnitAvailabilityRepositoryTests : IDisposable
    {
        private readonly YemenBookingDbContext _context;
        private readonly UnitAvailabilityRepository _repository;
        private readonly string _dbName;

        public UnitAvailabilityRepositoryTests()
        {
            _dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<YemenBookingDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options;
            
            var httpContextAccessor = new HttpContextAccessor();
            _context = new YemenBookingDbContext(options, httpContextAccessor);
            _repository = new UnitAvailabilityRepository(_context);
        }

        #region GetByUnitIdAsync Tests

        [Fact]
        public async Task GetByUnitIdAsync_WithNoDateFilters_ShouldReturnAllAvailabilitiesForUnit()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(5)),
                CreateAvailability(unitId, DateTime.Today.AddDays(10), DateTime.Today.AddDays(15)),
                CreateAvailability(Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(5)) // Different unit
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId);

            // Assert
            result.Should().HaveCount(2);
            result.All(a => a.UnitId == unitId).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUnitIdAsync_WithStartDate_ShouldFilterCorrectly()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var filterDate = DateTime.Today.AddDays(7);
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(5)), // Ends before filter
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(10)), // Overlaps filter
                CreateAvailability(unitId, DateTime.Today.AddDays(8), DateTime.Today.AddDays(15)) // Starts after filter
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId, startDate: filterDate);

            // Assert
            result.Should().HaveCount(2);
            result.All(a => a.EndDate >= filterDate).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUnitIdAsync_WithEndDate_ShouldFilterCorrectly()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var filterDate = DateTime.Today.AddDays(7);
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(5)), // Starts before filter
                CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(10)), // Overlaps filter
                CreateAvailability(unitId, DateTime.Today.AddDays(10), DateTime.Today.AddDays(15)) // Starts after filter
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId, endDate: filterDate);

            // Assert
            result.Should().HaveCount(2);
            result.All(a => a.StartDate <= filterDate).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUnitIdAsync_ShouldOrderByStartDate()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today.AddDays(10), DateTime.Today.AddDays(15)),
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(5)),
                CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(9))
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId);

            // Assert
            var resultList = result.ToList();
            resultList[0].StartDate.Should().Be(DateTime.Today);
            resultList[1].StartDate.Should().Be(DateTime.Today.AddDays(6));
            resultList[2].StartDate.Should().Be(DateTime.Today.AddDays(10));
        }

        #endregion

        #region GetByDateRangeAsync Tests

        [Fact]
        public async Task GetByDateRangeAsync_ShouldReturnOverlappingAvailabilities()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var startDate = DateTime.Today.AddDays(5);
            var endDate = DateTime.Today.AddDays(10);
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(3)), // Before range
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(7)), // Overlaps start
                CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(9)), // Inside range
                CreateAvailability(unitId, DateTime.Today.AddDays(8), DateTime.Today.AddDays(15)), // Overlaps end
                CreateAvailability(unitId, DateTime.Today.AddDays(12), DateTime.Today.AddDays(20)) // After range
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateRangeAsync(unitId, startDate, endDate);

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetByDateRangeAsync_WithExactMatch_ShouldInclude()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var startDate = DateTime.Today.AddDays(5);
            var endDate = DateTime.Today.AddDays(10);
            var availability = CreateAvailability(unitId, startDate, endDate);
            
            await _context.Set<UnitAvailability>().AddAsync(availability);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateRangeAsync(unitId, startDate, endDate);

            // Assert
            result.Should().HaveCount(1);
        }

        #endregion

        #region IsUnitAvailableAsync Tests

        [Fact]
        public async Task IsUnitAvailableAsync_WithNoBlocks_ShouldReturnTrue()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(5);
            var checkOut = DateTime.Today.AddDays(10);

            // Act
            var result = await _repository.IsUnitAvailableAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsUnitAvailableAsync_WithBlockInPeriod_ShouldReturnFalse()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(5);
            var checkOut = DateTime.Today.AddDays(10);
            var block = CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(8), status: "Booked");
            
            await _context.Set<UnitAvailability>().AddAsync(block);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsUnitAvailableAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsUnitAvailableAsync_WithAvailableStatus_ShouldReturnTrue()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(5);
            var checkOut = DateTime.Today.AddDays(10);
            var availability = CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(8), status: "Available");
            
            await _context.Set<UnitAvailability>().AddAsync(availability);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsUnitAvailableAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsUnitAvailableAsync_WithOverlappingBlock_ShouldReturnFalse()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(5);
            var checkOut = DateTime.Today.AddDays(10);
            var blocks = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(6), status: "Booked"), // Overlaps start
                CreateAvailability(unitId, DateTime.Today.AddDays(9), DateTime.Today.AddDays(15), status: "Maintenance") // Overlaps end
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(blocks);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsUnitAvailableAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("Booked", false)]
        [InlineData("Blocked", false)]
        [InlineData("Maintenance", false)]
        [InlineData("Available", true)]
        [InlineData("", false)] // Empty status treated as block
        public async Task IsUnitAvailableAsync_WithVariousStatuses_ShouldReturnCorrectly(string status, bool expectedResult)
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Today.AddDays(5);
            var checkOut = DateTime.Today.AddDays(10);
            var availability = CreateAvailability(unitId, checkIn, checkOut, status: status);
            
            await _context.Set<UnitAvailability>().AddAsync(availability);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsUnitAvailableAsync(unitId, checkIn, checkOut);

            // Assert
            result.Should().Be(expectedResult);
        }

        #endregion

        #region GetBlockedPeriodsAsync Tests

        [Fact]
        public async Task GetBlockedPeriodsAsync_ShouldReturnOnlyBlockedPeriods()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var year = 2024;
            var month = 3;
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, new DateTime(2024, 3, 5), new DateTime(2024, 3, 10), status: "Booked"),
                CreateAvailability(unitId, new DateTime(2024, 3, 15), new DateTime(2024, 3, 20), status: "Available"),
                CreateAvailability(unitId, new DateTime(2024, 3, 25), new DateTime(2024, 3, 30), status: "Maintenance")
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBlockedPeriodsAsync(unitId, year, month);

            // Assert
            result.Should().HaveCount(2);
            result.All(a => a.Status != "Available").Should().BeTrue();
        }

        [Fact]
        public async Task GetBlockedPeriodsAsync_WithPeriodsSpanningMonths_ShouldInclude()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var year = 2024;
            var month = 3;
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, new DateTime(2024, 2, 25), new DateTime(2024, 3, 5), status: "Booked"), // Starts in Feb
                CreateAvailability(unitId, new DateTime(2024, 3, 28), new DateTime(2024, 4, 5), status: "Blocked") // Ends in Apr
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBlockedPeriodsAsync(unitId, year, month);

            // Assert
            result.Should().HaveCount(2);
        }

        #endregion

        #region BulkCreateAsync Tests

        [Fact]
        public async Task BulkCreateAsync_ShouldAddAllAvailabilities()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(5)),
                CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(10)),
                CreateAvailability(unitId, DateTime.Today.AddDays(11), DateTime.Today.AddDays(15))
            };

            // Act
            await _repository.BulkCreateAsync(availabilities);

            // Assert
            var saved = await _context.Set<UnitAvailability>().Where(a => a.UnitId == unitId).ToListAsync();
            saved.Should().HaveCount(3);
        }

        [Fact]
        public async Task BulkCreateAsync_WithEmptyList_ShouldNotFail()
        {
            // Arrange
            var availabilities = new List<UnitAvailability>();

            // Act
            Func<Task> act = async () => await _repository.BulkCreateAsync(availabilities);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task BulkCreateAsync_WithLargeDataSet_ShouldHandle()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var availabilities = new List<UnitAvailability>();
            
            for (int i = 0; i < 1000; i++)
            {
                availabilities.Add(CreateAvailability(unitId, DateTime.Today.AddDays(i), DateTime.Today.AddDays(i + 1)));
            }

            // Act
            await _repository.BulkCreateAsync(availabilities);

            // Assert
            var saved = await _context.Set<UnitAvailability>().Where(a => a.UnitId == unitId).CountAsync();
            saved.Should().Be(1000);
        }

        #endregion

        #region BulkUpdateAsync Tests

        [Fact]
        public async Task BulkUpdateAsync_ShouldUpdateAllAvailabilities()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(5), status: "Available"),
                CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(10), status: "Available")
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();
            
            _context.ChangeTracker.Clear();
            
            // Modify
            availabilities[0].Status = "Booked";
            availabilities[1].Status = "Maintenance";

            // Act
            await _repository.BulkUpdateAsync(availabilities);

            // Assert
            var updated = await _context.Set<UnitAvailability>().Where(a => a.UnitId == unitId).ToListAsync();
            updated[0].Status.Should().Be("Booked");
            updated[1].Status.Should().Be("Maintenance");
        }

        #endregion

        #region DeleteRangeAsync Tests

        [Fact]
        public async Task DeleteRangeAsync_ShouldSoftDeleteInRange()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var deleteStart = DateTime.Today.AddDays(5);
            var deleteEnd = DateTime.Today.AddDays(10);
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(3)), // Outside
                CreateAvailability(unitId, DateTime.Today.AddDays(5), DateTime.Today.AddDays(10)), // Exact match
                CreateAvailability(unitId, DateTime.Today.AddDays(6), DateTime.Today.AddDays(9)), // Inside
                CreateAvailability(unitId, DateTime.Today.AddDays(11), DateTime.Today.AddDays(15)) // Outside
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteRangeAsync(unitId, deleteStart, deleteEnd);

            // Assert
            var all = await _context.Set<UnitAvailability>().Where(a => a.UnitId == unitId).ToListAsync();
            all.Should().HaveCount(4);
            all.Count(a => a.IsDeleted).Should().Be(2);
            all.Where(a => a.IsDeleted).All(a => a.DeletedAt.HasValue).Should().BeTrue();
        }

        [Fact]
        public async Task DeleteRangeAsync_ShouldSetDeletedAtToCurrentTime()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var availability = CreateAvailability(unitId, DateTime.Today.AddDays(5), DateTime.Today.AddDays(10));
            
            await _context.Set<UnitAvailability>().AddAsync(availability);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteRangeAsync(unitId, DateTime.Today.AddDays(5), DateTime.Today.AddDays(10));

            // Assert
            var deleted = await _context.Set<UnitAvailability>().FirstAsync(a => a.Id == availability.Id);
            deleted.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        #endregion

        #region GetAvailabilityCalendarAsync Tests

        [Fact]
        public async Task GetAvailabilityCalendarAsync_ShouldReturnStatusForEachDay()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var year = 2024;
            var month = 3;
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, new DateTime(2024, 3, 5), new DateTime(2024, 3, 10), status: "Booked"),
                CreateAvailability(unitId, new DateTime(2024, 3, 15), new DateTime(2024, 3, 20), status: "Maintenance")
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAvailabilityCalendarAsync(unitId, year, month);

            // Assert
            result.Should().HaveCount(31); // March has 31 days
            result[new DateTime(2024, 3, 1)].Should().Be("Available");
            result[new DateTime(2024, 3, 5)].Should().Be("Booked");
            result[new DateTime(2024, 3, 10)].Should().Be("Booked");
            result[new DateTime(2024, 3, 11)].Should().Be("Available");
            result[new DateTime(2024, 3, 15)].Should().Be("Maintenance");
            result[new DateTime(2024, 3, 31)].Should().Be("Available");
        }

        [Fact]
        public async Task GetAvailabilityCalendarAsync_WithOverlappingPeriods_ShouldUseFirst()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var year = 2024;
            var month = 3;
            var availabilities = new List<UnitAvailability>
            {
                CreateAvailability(unitId, new DateTime(2024, 3, 5), new DateTime(2024, 3, 10), status: "Booked"),
                CreateAvailability(unitId, new DateTime(2024, 3, 8), new DateTime(2024, 3, 12), status: "Maintenance")
            };
            
            await _context.Set<UnitAvailability>().AddRangeAsync(availabilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAvailabilityCalendarAsync(unitId, year, month);

            // Assert
            result[new DateTime(2024, 3, 8)].Should().Be("Booked"); // First match wins
        }

        [Fact]
        public async Task GetAvailabilityCalendarAsync_ForFebruary_ShouldHandleLeapYear()
        {
            // Arrange
            var unitId = Guid.NewGuid();

            // Act
            var result2024 = await _repository.GetAvailabilityCalendarAsync(unitId, 2024, 2); // Leap year
            var result2023 = await _repository.GetAvailabilityCalendarAsync(unitId, 2023, 2); // Non-leap year

            // Assert
            result2024.Should().HaveCount(29);
            result2023.Should().HaveCount(28);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public async Task Repository_ShouldHandleNullBookingId()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var availability = CreateAvailability(unitId, DateTime.Today, DateTime.Today.AddDays(5));
            availability.BookingId = null;
            
            await _context.Set<UnitAvailability>().AddAsync(availability);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUnitIdAsync(unitId);

            // Assert
            result.Should().HaveCount(1);
            result.First().BookingId.Should().BeNull();
        }

        [Fact]
        public async Task Repository_ShouldHandleConcurrentOperations()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var tasks = new List<Task>();
            
            // Act - Create multiple concurrent operations
            for (int i = 0; i < 10; i++)
            {
                var localI = i;
                tasks.Add(Task.Run(async () =>
                {
                    var availability = CreateAvailability(
                        unitId, 
                        DateTime.Today.AddDays(localI * 10), 
                        DateTime.Today.AddDays(localI * 10 + 5));
                    
                    await using var context = new YemenBookingDbContext(
                        new DbContextOptionsBuilder<YemenBookingDbContext>()
                            .UseInMemoryDatabase(databaseName: _dbName)
                            .Options,
                        new HttpContextAccessor());
                    
                    var repo = new UnitAvailabilityRepository(context);
                    await repo.AddAsync(availability);
                    await repo.SaveChangesAsync();
                }));
            }
            
            await Task.WhenAll(tasks);

            // Assert
            var result = await _repository.GetByUnitIdAsync(unitId);
            result.Should().HaveCount(10);
        }

        #endregion

        #region Helper Methods

        private UnitAvailability CreateAvailability(
            Guid unitId,
            DateTime startDate,
            DateTime endDate,
            string status = "Available",
            Guid? bookingId = null)
        {
            return new UnitAvailability
            {
                Id = Guid.NewGuid(),
                UnitId = unitId,
                BookingId = bookingId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                Reason = "Test",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}