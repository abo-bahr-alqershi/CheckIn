using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Sections;

namespace YemenBooking.Application.Queries.CP.Sections
{
    public class GetSectionByIdQuery : IRequest<ResultDto<SectionDto>>
    {
        public Guid SectionId { get; set; }
    }
}

