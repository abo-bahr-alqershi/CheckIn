using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.Queries.Reviews;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Reviews
{
    public class GetReviewByBookingQueryHandler : IRequestHandler<GetReviewByBookingQuery, ResultDto<AdminReviewDetailsDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetReviewByBookingQueryHandler(
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService)
        {
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<AdminReviewDetailsDto>> Handle(GetReviewByBookingQuery request, CancellationToken cancellationToken)
        {
            if (request.BookingId == Guid.Empty)
                return ResultDto<AdminReviewDetailsDto>.Failed("معرّف الحجز مطلوب");

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null || !await _currentUserService.IsInRoleAsync("Admin"))
                return ResultDto<AdminReviewDetailsDto>.Failed("غير مصرح لك");

            var review = await _reviewRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Unit)
                        .ThenInclude(u => u.Property)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(r => r.BookingId == request.BookingId, cancellationToken);

            if (review == null)
                return ResultDto<AdminReviewDetailsDto>.Ok(null, arabicMessage: "لا يوجد تقييم لهذا الحجز");

            var dto = new AdminReviewDetailsDto
            {
                Id = review.Id,
                BookingId = review.BookingId,
                PropertyId = review.PropertyId,
                UnitId = review.Booking?.UnitId,
                Cleanliness = review.Cleanliness,
                Service = review.Service,
                Location = review.Location,
                Value = review.Value,
                AverageRating = review.AverageRating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                IsApproved = !review.IsPendingApproval,
                IsPending = review.IsPendingApproval,
                ResponseText = review.ResponseText,
                ResponseDate = review.ResponseDate,
                RespondedBy = review.UpdatedBy,
                Images = review.Images.Select(img => new ReviewImageDto
                {
                    Id = img.Id,
                    ReviewId = img.ReviewId,
                    Name = img.Name,
                    Url = img.Url,
                    SizeBytes = img.SizeBytes,
                    Type = img.Type,
                    Category = img.Category,
                    Caption = img.Caption,
                    AltText = img.AltText,
                    UploadedAt = img.UploadedAt
                }).ToList(),
                PropertyName = review.Booking?.Unit?.Property?.Name ?? string.Empty,
                UnitName = review.Booking?.Unit?.Name,
                PropertyCity = review.Booking?.Unit?.Property?.City,
                PropertyAddress = review.Booking?.Unit?.Property?.Address,
                UserName = review.Booking?.User?.Name ?? string.Empty,
                UserEmail = review.Booking?.User?.Email,
                UserPhone = review.Booking?.User?.Phone,
                BookingCheckIn = review.Booking?.CheckIn,
                BookingCheckOut = review.Booking?.CheckOut,
                GuestsCount = review.Booking?.GuestsCount,
                BookingStatus = review.Booking?.Status.ToString(),
                BookingSource = review.Booking?.BookingSource,
            };

            return ResultDto<AdminReviewDetailsDto>.Ok(dto);
        }
    }
}

