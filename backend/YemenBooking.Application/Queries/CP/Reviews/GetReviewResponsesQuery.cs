using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Reviews
{
    /// <summary>
    /// استعلام لجلب ردود التقييم
    /// </summary>
    public class GetReviewResponsesQuery : IRequest<ResultDto<List<ReviewResponseDto>>>
    {
        public Guid ReviewId { get; set; }
    }
}

