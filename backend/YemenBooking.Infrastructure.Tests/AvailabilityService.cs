using Xunit;
using Moq;
using FluentAssertions;
using YemenBooking.Infrastructure.Services;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Commands.CP.Availability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YemenBooking.Tests.Services
{
    public class AvailabilityServiceTests
    {
        private readonly Mock<IUnitAvailabilityRepository> _availabilityRepositoryMock;
        private readonly Mock<IUnitRepository> _unitRepositoryMock;
        private readonly Infrastructure.Services.AvailabilityService _service;

        public AvailabilityServiceTests()
        {
            _availabilityRepositoryMock = new Mock<IUnitAvailabilityRepository>();
            _unitRepositoryMock = new Mock<IUnitRepository>();
            _service = new Infrastructure.Services.AvailabilityService(
                _availabilityRepositoryMock.Object,
                _unitRepositoryMock.Object);
        }

        #region CheckAvailabilityAsync Tests

        [Fact]
        public async Task CheckAvailabilityAsync_WhenUnitDoesNotExist_ShouldReturnFalse()
        {
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Unit)null!);

            var result = await _service.CheckAvailabilityAsync(unitId, checkIn, checkOut);

            result.Should().BeFalse();
            _unitRepositoryMock.Verify(x => x.GetByIdAsync(It.Is<Guid>(g => g == unitId), It.IsAny<CancellationToken>()), Times.Once);
            _availabilityRepositoryMock.Verify(x => x.IsUnitAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task CheckAvailabilityAsync_WhenUnitIsNotAvailable_ShouldReturnFalse()
        {
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var unit = new Unit { Id = unitId, IsAvailable = false };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);

            var result = await _service.CheckAvailabilityAsync(unitId, checkIn, checkOut);

            result.Should().BeFalse();
            _availabilityRepositoryMock.Verify(x => x.IsUnitAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task CheckAvailabilityAsync_WhenUnitIsAvailableAndNoBlocks_ShouldReturnTrue()
        {
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var unit = new Unit { Id = unitId, IsAvailable = true };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _availabilityRepositoryMock.Setup(x => x.IsUnitAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var result = await _service.CheckAvailabilityAsync(unitId, checkIn, checkOut);

            result.Should().BeTrue();
            _availabilityRepositoryMock.Verify(x => x.IsUnitAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task CheckAvailabilityAsync_WhenUnitHasBlocksInPeriod_ShouldReturnFalse()
        {
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var unit = new Unit { Id = unitId, IsAvailable = true };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _availabilityRepositoryMock.Setup(x => x.IsUnitAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            var result = await _service.CheckAvailabilityAsync(unitId, checkIn, checkOut);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 1)]   // Same day check-in/out
        [InlineData(1, 30)]  // Month-long stay
        [InlineData(1, 365)] // Year-long stay
        public async Task CheckAvailabilityAsync_WithVariousPeriods_ShouldWork(int daysFromNow, int stayDuration)
        {
            var unitId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(daysFromNow);
            var checkOut = checkIn.AddDays(stayDuration);
            var unit = new Unit { Id = unitId, IsAvailable = true };

            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unit);
            _availabilityRepositoryMock.Setup(x => x.IsUnitAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var result = await _service.CheckAvailabilityAsync(unitId, checkIn, checkOut);

            result.Should().BeTrue();
        }

        #endregion

        #region BlockForBookingAsync Tests

        [Fact]
        public async Task BlockForBookingAsync_ShouldCreateAvailabilityBlock()
        {
            var unitId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            UnitAvailability capturedAvailability = null;

            _availabilityRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UnitAvailability>(), It.IsAny<CancellationToken>()))
                .Callback<UnitAvailability, CancellationToken>((a, _) => capturedAvailability = a)
                .ReturnsAsync((UnitAvailability a, CancellationToken _) => a);

            await _service.BlockForBookingAsync(unitId, bookingId, checkIn, checkOut);

            capturedAvailability.Should().NotBeNull();
            capturedAvailability.UnitId.Should().Be(unitId);
            capturedAvailability.BookingId.Should().Be(bookingId);
            capturedAvailability.StartDate.Should().Be(checkIn);
            capturedAvailability.EndDate.Should().Be(checkOut);
            capturedAvailability.Status.Should().Be("Booked");
            capturedAvailability.Reason.Should().Be("Customer Booking");
            capturedAvailability.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            
            _availabilityRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UnitAvailability>(), It.IsAny<CancellationToken>()), Times.Once);
            _availabilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BlockForBookingAsync_WithInvalidDates_ShouldStillCreate()
        {
            var unitId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(3);
            var checkOut = DateTime.Now.AddDays(1); // Check-out before check-in

            await _service.BlockForBookingAsync(unitId, bookingId, checkIn, checkOut);

            _availabilityRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UnitAvailability>(), It.IsAny<CancellationToken>()), Times.Once);
            _availabilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BlockForBookingAsync_MultipleCallsWithSameBooking_ShouldCreateMultipleBlocks()
        {
            var unitId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var checkIn1 = DateTime.Now.AddDays(1);
            var checkOut1 = DateTime.Now.AddDays(3);
            var checkIn2 = DateTime.Now.AddDays(5);
            var checkOut2 = DateTime.Now.AddDays(7);

            await _service.BlockForBookingAsync(unitId, bookingId, checkIn1, checkOut1);
            await _service.BlockForBookingAsync(unitId, bookingId, checkIn2, checkOut2);

            _availabilityRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UnitAvailability>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _availabilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        #endregion

        #region ReleaseBookingAsync Tests

        [Fact]
        public async Task ReleaseBookingAsync_WithExistingBlocks_ShouldMarkAsDeleted()
        {
            var bookingId = Guid.NewGuid();
            var blocks = new List<UnitAvailability>
            {
                new UnitAvailability { Id = Guid.NewGuid(), BookingId = bookingId, IsDeleted = false },
                new UnitAvailability { Id = Guid.NewGuid(), BookingId = bookingId, IsDeleted = false }
            };

            _availabilityRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(blocks);

            await _service.ReleaseBookingAsync(bookingId);

            blocks.All(b => b.IsDeleted).Should().BeTrue();
            blocks.All(b => b.DeletedAt.HasValue).Should().BeTrue();
            blocks.Select(b => b.DeletedAt).Should().OnlyContain(dt => dt.HasValue);
            blocks.Select(b => b.DeletedAt!.Value).Should().AllSatisfy(dt => dt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5)));
            
            _availabilityRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<UnitAvailability>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _availabilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReleaseBookingAsync_WithNoBlocks_ShouldNotUpdate()
        {
            var bookingId = Guid.NewGuid();
            var blocks = new List<UnitAvailability>
            {
                new UnitAvailability { Id = Guid.NewGuid(), BookingId = Guid.NewGuid() }
            };

            _availabilityRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(blocks);

            await _service.ReleaseBookingAsync(bookingId);

            _availabilityRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<UnitAvailability>(), It.IsAny<CancellationToken>()), Times.Never);
            _availabilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReleaseBookingAsync_WithAlreadyDeletedBlocks_ShouldSkip()
        {
            var bookingId = Guid.NewGuid();
            var blocks = new List<UnitAvailability>
            {
                new UnitAvailability { Id = Guid.NewGuid(), BookingId = bookingId, IsDeleted = true },
                new UnitAvailability { Id = Guid.NewGuid(), BookingId = bookingId, IsDeleted = false }
            };

            _availabilityRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(blocks);

            await _service.ReleaseBookingAsync(bookingId);

            _availabilityRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<UnitAvailability>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        #endregion

        #region GetMonthlyCalendarAsync Tests

        [Fact]
        public async Task GetMonthlyCalendarAsync_ShouldReturnCalendarFromRepository()
        {
            var unitId = Guid.NewGuid();
            var year = 2024;
            var month = 3;
            var expectedCalendar = new Dictionary<DateTime, string>
            {
                { new DateTime(2024, 3, 1), "Available" },
                { new DateTime(2024, 3, 2), "Booked" }
            };

            _availabilityRepositoryMock.Setup(x => x.GetAvailabilityCalendarAsync(unitId, year, month))
                .ReturnsAsync(expectedCalendar);

            var result = await _service.GetMonthlyCalendarAsync(unitId, year, month);

            result.Should().BeEquivalentTo(expectedCalendar);
        }

        #endregion

        #region ApplyBulkAvailabilityAsync Tests

        [Fact]
        public async Task ApplyBulkAvailabilityAsync_WithOverwrite_ShouldDeleteExistingAndCreateNew()
        {
            var unitId = Guid.NewGuid();
            var periods = new List<AvailabilityPeriodDto>
            {
                new AvailabilityPeriodDto
                {
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(5),
                    Status = "Blocked",
                    Reason = "Maintenance",
                    Notes = "Test notes",
                    OverwriteExisting = true
                }
            };

            List<UnitAvailability> capturedAvailabilities = null;
            _availabilityRepositoryMock.Setup(x => x.BulkCreateAsync(It.IsAny<List<UnitAvailability>>()))
                .Callback<IEnumerable<UnitAvailability>>(a => capturedAvailabilities = a.ToList())
                .Returns(Task.CompletedTask);

            await _service.ApplyBulkAvailabilityAsync(unitId, periods);

            _availabilityRepositoryMock.Verify(x => x.DeleteRangeAsync(unitId, periods[0].StartDate, periods[0].EndDate), Times.Once);
            _availabilityRepositoryMock.Verify(x => x.BulkCreateAsync(It.IsAny<List<UnitAvailability>>()), Times.Once);
            
            capturedAvailabilities.Should().HaveCount(1);
            capturedAvailabilities[0].UnitId.Should().Be(unitId);
            capturedAvailabilities[0].Status.Should().Be("Blocked");
            capturedAvailabilities[0].Reason.Should().Be("Maintenance");
            capturedAvailabilities[0].Notes.Should().Be("Test notes");
        }

        [Fact]
        public async Task ApplyBulkAvailabilityAsync_WithoutOverwrite_ShouldOnlyCreateNew()
        {
            var unitId = Guid.NewGuid();
            var periods = new List<AvailabilityPeriodDto>
            {
                new AvailabilityPeriodDto
                {
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(5),
                    Status = "Available",
                    OverwriteExisting = false
                }
            };

            await _service.ApplyBulkAvailabilityAsync(unitId, periods);

            _availabilityRepositoryMock.Verify(x => x.DeleteRangeAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
            _availabilityRepositoryMock.Verify(x => x.BulkCreateAsync(It.IsAny<List<UnitAvailability>>()), Times.Once);
        }

        [Fact]
        public async Task ApplyBulkAvailabilityAsync_WithMultiplePeriods_ShouldProcessAll()
        {
            var unitId = Guid.NewGuid();
            var periods = new List<AvailabilityPeriodDto>
            {
                new AvailabilityPeriodDto
                {
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(5),
                    Status = "Blocked",
                    OverwriteExisting = true
                },
                new AvailabilityPeriodDto
                {
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(15),
                    Status = "Available",
                    OverwriteExisting = false
                }
            };

            List<UnitAvailability> capturedAvailabilities = null;
            _availabilityRepositoryMock.Setup(x => x.BulkCreateAsync(It.IsAny<List<UnitAvailability>>()))
                .Callback<IEnumerable<UnitAvailability>>(a => capturedAvailabilities = a.ToList())
                .Returns(Task.CompletedTask);

            await _service.ApplyBulkAvailabilityAsync(unitId, periods);

            _availabilityRepositoryMock.Verify(x => x.DeleteRangeAsync(unitId, periods[0].StartDate, periods[0].EndDate), Times.Once);
            capturedAvailabilities.Should().HaveCount(2);
        }

        [Fact]
        public async Task ApplyBulkAvailabilityAsync_WithEmptyPeriods_ShouldNotFail()
        {
            var unitId = Guid.NewGuid();
            var periods = new List<AvailabilityPeriodDto>();

            await _service.ApplyBulkAvailabilityAsync(unitId, periods);

            _availabilityRepositoryMock.Verify(x => x.BulkCreateAsync(It.Is<List<UnitAvailability>>(l => l.Count == 0)), Times.Once);
        }

        #endregion

        #region GetAvailableUnitsInPropertyAsync Tests

        [Fact]
        public async Task GetAvailableUnitsInPropertyAsync_WithAllAvailableUnits_ShouldReturnAll()
        {
            var propertyId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var guestCount = 2;
            var units = new List<Unit>
            {
                new Unit { Id = Guid.NewGuid(), IsAvailable = true },
                new Unit { Id = Guid.NewGuid(), IsAvailable = true }
            };

            _unitRepositoryMock.Setup(x => x.GetUnitsByPropertyAsync(propertyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(units);
            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => units.First(u => u.Id == id));
            _availabilityRepositoryMock.Setup(x => x.IsUnitAvailableAsync(It.IsAny<Guid>(), checkIn, checkOut))
                .ReturnsAsync(true);

            var result = await _service.GetAvailableUnitsInPropertyAsync(propertyId, checkIn, checkOut, guestCount);

            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(units.Select(u => u.Id));
        }

        [Fact]
        public async Task GetAvailableUnitsInPropertyAsync_WithSomeUnavailable_ShouldFilterOut()
        {
            var propertyId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var guestCount = 2;
            var units = new List<Unit>
            {
                new Unit { Id = Guid.NewGuid(), IsAvailable = true },
                new Unit { Id = Guid.NewGuid(), IsAvailable = true }
            };

            _unitRepositoryMock.Setup(x => x.GetUnitsByPropertyAsync(propertyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(units);
            _unitRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => units.FirstOrDefault(u => u.Id == id));
            _availabilityRepositoryMock.Setup(x => x.IsUnitAvailableAsync(units[0].Id, checkIn, checkOut))
                .ReturnsAsync(true);
            _availabilityRepositoryMock.Setup(x => x.IsUnitAvailableAsync(units[1].Id, checkIn, checkOut))
                .ReturnsAsync(false);

            var result = await _service.GetAvailableUnitsInPropertyAsync(propertyId, checkIn, checkOut, guestCount);

            result.Should().HaveCount(1);
            result.Should().Contain(units[0].Id);
        }

        [Fact]
        public async Task GetAvailableUnitsInPropertyAsync_WithNoUnits_ShouldReturnEmpty()
        {
            var propertyId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var guestCount = 2;

            _unitRepositoryMock.Setup(x => x.GetUnitsByPropertyAsync(propertyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Unit>());

            var result = await _service.GetAvailableUnitsInPropertyAsync(propertyId, checkIn, checkOut, guestCount);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAvailableUnitsInPropertyAsync_WithCancellation_ShouldPassToken()
        {
            var propertyId = Guid.NewGuid();
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);
            var guestCount = 2;
            var cancellationToken = new CancellationToken();

            _unitRepositoryMock.Setup(x => x.GetUnitsByPropertyAsync(propertyId, cancellationToken))
                .ReturnsAsync(new List<Unit>());

            await _service.GetAvailableUnitsInPropertyAsync(propertyId, checkIn, checkOut, guestCount, cancellationToken);

            _unitRepositoryMock.Verify(x => x.GetUnitsByPropertyAsync(propertyId, cancellationToken), Times.Once);
        }

        #endregion
    }
}