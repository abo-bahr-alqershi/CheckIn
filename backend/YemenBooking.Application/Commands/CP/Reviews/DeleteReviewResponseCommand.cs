using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Reviews
{
    /// <summary>
    /// أمر لحذف رد تقييم
    /// </summary>
    public class DeleteReviewResponseCommand : IRequest<ResultDto<bool>>
    {
        public Guid ResponseId { get; set; }
    }
}

