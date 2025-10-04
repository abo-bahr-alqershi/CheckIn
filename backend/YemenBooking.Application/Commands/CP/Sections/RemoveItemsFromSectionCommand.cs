using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Sections
{
    public class RemoveItemsFromSectionCommand : IRequest<ResultDto>
    {
        public Guid SectionId { get; set; }
        public List<Guid> ItemIds { get; set; } = new();
    }
}

