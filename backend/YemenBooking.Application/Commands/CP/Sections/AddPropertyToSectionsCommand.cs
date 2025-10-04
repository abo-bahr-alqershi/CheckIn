using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Sections
{
	public class AddPropertyToSectionsCommand : IRequest<ResultDto>
	{
		public Guid PropertyId { get; set; }
		public List<Guid> SectionIds { get; set; } = new();
	}
}