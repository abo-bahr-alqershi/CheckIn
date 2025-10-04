using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Reviews;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Reviews
{
    /// <summary>
    /// معالج استعلام جلب جميع التقييمات مع دعم التصفية
    /// Handler for GetAllReviewsQuery
    /// </summary>
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, ResultDto<IEnumerable<ReviewDto>>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAllReviewsQueryHandler> _logger;

        public GetAllReviewsQueryHandler(
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService,
            ILogger<GetAllReviewsQueryHandler> logger)
        {
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<ReviewDto>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing GetAllReviewsQuery");

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null || !await _currentUserService.IsInRoleAsync("Admin"))
            {
                return ResultDto<IEnumerable<ReviewDto>>.Failure("ليس لديك صلاحية لعرض التقييمات");
            }

            var query = _reviewRepository.GetQueryable()
                .AsNoTracking()
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Unit)
                        .ThenInclude(u => u.Property)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Status) && request.Status != "all")
            {
                if (request.Status == "pending")
                    query = query.Where(r => r.IsPendingApproval);
                else if (request.Status == "approved")
                    query = query.Where(r => !r.IsPendingApproval);
                else if (request.Status == "rejected")
                    query = query.Where(r => false);
            }

            if (request.MinRating.HasValue)
                query = query.Where(r => (r.Cleanliness + r.Service + r.Location + r.Value) / 4.0 >= request.MinRating.Value);
            if (request.MaxRating.HasValue)
                query = query.Where(r => (r.Cleanliness + r.Service + r.Location + r.Value) / 4.0 <= request.MaxRating.Value);
            if (request.HasImages.HasValue)
                query = request.HasImages.Value
                    ? query.Where(r => r.Images.Any())
                    : query.Where(r => !r.Images.Any());
            if (request.PropertyId.HasValue)
                query = query.Where(r => r.Booking.Unit.PropertyId == request.PropertyId.Value);

            // تصفية حسب معرف الوحدة
            // Filter by unit Id
            if (request.UnitId.HasValue)
                query = query.Where(r => r.Booking.UnitId == request.UnitId.Value);

            if (request.UserId.HasValue)
                query = query.Where(r => r.Booking.UserId == request.UserId.Value);
            if (request.ReviewedAfter.HasValue)
                query = query.Where(r => r.CreatedAt >= request.ReviewedAfter.Value);
            if (request.ReviewedBefore.HasValue)
                query = query.Where(r => r.CreatedAt <= request.ReviewedBefore.Value);

            // Pagination defaults
            var pageNumber = (request.PageNumber ?? 1) < 1 ? 1 : (request.PageNumber ?? 1);
            var pageSize = (request.PageSize ?? 20) < 1 ? 20 : (request.PageSize ?? 20);

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                BookingId = r.BookingId,
                Cleanliness = r.Cleanliness,
                Service = r.Service,
                Location = r.Location,
                Value = r.Value,
                // متوسط التقييم المخزن
                AverageRating = r.AverageRating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                IsApproved = !r.IsPendingApproval,
                IsPending = r.IsPendingApproval,
                RespondedBy = null,
                // Related property and user names (safe navigation)
                PropertyName = r.Booking?.Unit?.Property?.Name ?? string.Empty,
                UserName = r.Booking?.User?.Name ?? string.Empty,
                Images = r.Images.Select(img => new ReviewImageDto
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
                }).ToList()
            });

            return ResultDto<IEnumerable<ReviewDto>>.Ok(reviewDtos);
        }
    }
} 