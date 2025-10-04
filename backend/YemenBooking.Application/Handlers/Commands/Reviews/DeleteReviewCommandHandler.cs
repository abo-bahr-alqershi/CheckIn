using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Reviews;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Reviews
{
    /// <summary>
    /// معالج أمر حذف تقييم
    /// </summary>
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, ResultDto<bool>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteReviewCommandHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IIndexingService _indexingService;

        public DeleteReviewCommandHandler(
            IReviewRepository reviewRepository,
            IUnitRepository unitRepository,
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeleteReviewCommandHandler> logger,
            IMediator mediator,
            IIndexingService indexingService)
        {
            _reviewRepository = reviewRepository;
            _unitRepository = unitRepository;
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
            _mediator = mediator;
            _indexingService = indexingService;
        }

        public async Task<ResultDto<bool>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء حذف التقييم: ReviewId={ReviewId}", request.ReviewId);

            // التحقق من المدخلات
            if (request.ReviewId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف التقييم مطلوب");

            // التحقق من الوجود
            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                return ResultDto<bool>.Failed("التقييم غير موجود");

            // التحقق من الصلاحيات
            if (_currentUserService.Role != "Admin" && review.CreatedBy != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بحذف هذا التقييم");

            // تنفيذ الحذف
            bool deleted = await _reviewRepository.DeleteReviewAsync(request.ReviewId, cancellationToken);
            if (!deleted)
                return ResultDto<bool>.Failed("فشل حذف التقييم");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "DeleteReview",
                $"تم حذف التقييم {request.ReviewId}",
                request.ReviewId,
                "Review",
                _currentUserService.UserId,
                null,
                cancellationToken);

            // Update property average rating
            var bookingEntity = await _reviewRepository.GetBookingByIdAsync(review.BookingId, cancellationToken);
            if (bookingEntity != null)
            {
                var unit = await _unitRepository.GetUnitByIdAsync(bookingEntity.UnitId, cancellationToken);
                if (unit != null)
                {
                    var propertyId = unit.PropertyId;
                    var (avgRating, totalReviews) = await _reviewRepository.GetPropertyRatingStatsAsync(propertyId, cancellationToken);
                    var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
                    if (property != null)
                    {
                        property.AverageRating = (decimal)avgRating;
                        await _propertyRepository.UpdatePropertyAsync(property, cancellationToken);

                        // تحديث مباشر لفهرس العقار
                        try
                        {
                            await _indexingService.OnPropertyUpdatedAsync(property.Id, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "تعذرت الفهرسة المباشرة للعقار بعد حذف التقييم {PropertyId}", property.Id);
                        }
                    }
                }
            }

            _logger.LogInformation("اكتمل حذف التقييم بنجاح: ReviewId={ReviewId}", request.ReviewId);
            return ResultDto<bool>.Succeeded(true, "تم حذف التقييم بنجاح");
        }
    }
} 