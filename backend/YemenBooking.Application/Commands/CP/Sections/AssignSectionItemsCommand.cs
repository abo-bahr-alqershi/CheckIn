using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Sections
{
	public class AssignSectionItemsCommand : IRequest<ResultDto>
	{
		public Guid SectionId { get; set; }
		public List<Guid> PropertyIds { get; set; } = new();
		public List<Guid> UnitIds { get; set; } = new();
	}
}