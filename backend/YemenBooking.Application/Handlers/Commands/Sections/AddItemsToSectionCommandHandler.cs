using MediatR;
using YemenBooking.Application.Commands.CP.Sections;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Sections
{
    public class AddItemsToSectionCommandHandler : IRequestHandler<AddItemsToSectionCommand, ResultDto>
    {
        private readonly ISectionRepository _repository;

        public AddItemsToSectionCommandHandler(ISectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultDto> Handle(AddItemsToSectionCommand request, CancellationToken cancellationToken)
        {
            if (request.PropertyIds?.Count > 0)
                await _repository.AddPropertiesAsync(request.SectionId, request.PropertyIds, cancellationToken);
            if (request.UnitIds?.Count > 0)
                await _repository.AddUnitsAsync(request.SectionId, request.UnitIds, cancellationToken);
            return ResultDto.Ok();
        }
    }
}

