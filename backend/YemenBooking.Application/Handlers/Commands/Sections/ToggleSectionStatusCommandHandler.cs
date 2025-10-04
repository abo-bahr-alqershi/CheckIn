using MediatR;
using YemenBooking.Application.Commands.CP.Sections;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Sections
{
    public class ToggleSectionStatusCommandHandler : IRequestHandler<ToggleSectionStatusCommand, ResultDto>
    {
        private readonly ISectionRepository _repository;

        public ToggleSectionStatusCommandHandler(ISectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultDto> Handle(ToggleSectionStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.SectionId, cancellationToken);
            if (entity == null) return ResultDto.Failure("Section not found");
            entity.IsActive = request.IsActive;
            await _repository.UpdateAsync(entity, cancellationToken);
            return ResultDto.Ok();
        }
    }
}

