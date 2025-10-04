using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reviews
{
    /// <summary>
    /// الحصول على تقييم مرتبط بحجز محدد (لوحة التحكم)
    /// </summary>
    public class GetReviewByBookingQuery : IRequest<ResultDto<AdminReviewDetailsDto>>
    {
        public Guid BookingId { get; set; }
    }
}

