using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.PropertySearch;

namespace YemenBooking.Application.Queries.MobileApp.Sections
{
	public class GetSectionItemsQuery : PaginationDto, IRequest<PaginatedResult<object>>
	{
		public Guid SectionId { get; set; }
	}
}