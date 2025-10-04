using MediatR;
using YemenBooking.Application.Commands.CP.Sections;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Sections
{
	public class AddPropertyToSectionsCommandHandler : IRequestHandler<AddPropertyToSectionsCommand, ResultDto>
	{
		private readonly ISectionRepository _repository;

		public AddPropertyToSectionsCommandHandler(ISectionRepository repository)
		{
			_repository = repository;
		}

		public async Task<ResultDto> Handle(AddPropertyToSectionsCommand request, CancellationToken cancellationToken)
		{
			foreach (var sectionId in request.SectionIds)
			{
				await _repository.AddPropertiesAsync(sectionId, new[] { request.PropertyId }, cancellationToken);
			}
			return ResultDto.Ok();
		}
	}
}