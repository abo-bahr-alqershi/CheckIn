using MediatR;
using YemenBooking.Application.Commands.CP.Sections;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Sections
{
	public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, ResultDto>
	{
		private readonly ISectionRepository _repository;

		public DeleteSectionCommandHandler(ISectionRepository repository)
		{
			_repository = repository;
		}

		public async Task<ResultDto> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
		{
			var ok = await _repository.DeleteAsync(request.SectionId, cancellationToken);
			return ok ? ResultDto.Ok() : ResultDto.Failure("Section not found");
		}
	}
}