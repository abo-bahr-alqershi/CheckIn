using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.PropertyInSectionImages
{
    public class DeletePropertyInSectionImageCommand : IRequest<ResultDto<bool>>
    {
        public Guid ImageId { get; set; }
        public bool Permanent { get; set; } = false;
    }
}

