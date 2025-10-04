using MediatR;
using YemenBooking.Application.Commands.CP.Sections;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Sections
{
	public class AddUnitToSectionsCommandHandler : IRequestHandler<AddUnitToSectionsCommand, ResultDto>
	{
		private readonly ISectionRepository _repository;

		public AddUnitToSectionsCommandHandler(ISectionRepository repository)
		{
			_repository = repository;
		}

		public async Task<ResultDto> Handle(AddUnitToSectionsCommand request, CancellationToken cancellationToken)
		{
			foreach (var sectionId in request.SectionIds)
			{
				await _repository.AddUnitsAsync(sectionId, new[] { request.UnitId }, cancellationToken);
			}
			return ResultDto.Ok();
		}
	}
}