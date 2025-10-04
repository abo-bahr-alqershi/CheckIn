using MediatR;
using YemenBooking.Application.Commands.CP.Sections;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Sections
{
    public class RemoveItemsFromSectionCommandHandler : IRequestHandler<RemoveItemsFromSectionCommand, ResultDto>
    {
        private readonly ISectionRepository _repository;

        public RemoveItemsFromSectionCommandHandler(ISectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultDto> Handle(RemoveItemsFromSectionCommand request, CancellationToken cancellationToken)
        {
            foreach (var id in request.ItemIds)
            {
                await _repository.RemoveItemAsync(request.SectionId, id, cancellationToken);
            }
            return ResultDto.Ok();
        }
    }
}

