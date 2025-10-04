using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Sections
{
    public class UpdateItemOrderCommand : IRequest<ResultDto>
    {
        public Guid SectionId { get; set; }
        public List<ItemOrder> Orders { get; set; } = new();
    }

    public class ItemOrder
    {
        public Guid ItemId { get; set; }
        public int SortOrder { get; set; }
    }
}

