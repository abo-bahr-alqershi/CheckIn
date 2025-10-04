using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.Sections
{
	public class AddUnitToSectionsCommand : IRequest<ResultDto>
	{
		public Guid UnitId { get; set; }
		public List<Guid> SectionIds { get; set; } = new();
	}
}