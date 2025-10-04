using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Sections;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Queries.CP.Sections
{
	public class GetSectionsQuery : PaginationDto, IRequest<PaginatedResult<SectionDto>>
	{
		public SectionTarget? Target { get; set; }
		public SectionType? Type { get; set; }
		public ContentType? ContentType { get; set; }
	}
}