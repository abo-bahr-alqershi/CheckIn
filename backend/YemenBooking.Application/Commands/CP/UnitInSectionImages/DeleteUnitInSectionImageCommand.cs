using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.UnitInSectionImages
{
    public class DeleteUnitInSectionImageCommand : IRequest<ResultDto<bool>>
    {
        public Guid ImageId { get; set; }
        public bool Permanent { get; set; } = false;
    }
}

