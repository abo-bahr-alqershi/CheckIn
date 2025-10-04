using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reviews
{
    /// <summary>
    /// استعلام جلب تفاصيل تقييم واحد للوحة التحكم
    /// Get admin review details by id
    /// </summary>
    public class GetReviewDetailsQuery : IRequest<ResultDto<AdminReviewDetailsDto>>
    {
        public Guid ReviewId { get; set; }
    }
}

