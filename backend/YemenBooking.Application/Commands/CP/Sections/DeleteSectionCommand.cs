using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Sections
{
	public class DeleteSectionCommand : IRequest<ResultDto>
	{
		public Guid SectionId { get; set; }
	}
}