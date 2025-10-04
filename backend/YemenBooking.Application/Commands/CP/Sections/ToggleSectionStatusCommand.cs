using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Sections
{
    public class ToggleSectionStatusCommand : IRequest<ResultDto>
    {
        public Guid SectionId { get; set; }
        public bool IsActive { get; set; }
    }
}

