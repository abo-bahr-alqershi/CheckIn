using MediatR;
using YemenBooking.Application.Commands.CP.Sections;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.Sections
{
    public class UpdateItemOrderCommandHandler : IRequestHandler<UpdateItemOrderCommand, ResultDto>
    {
        private readonly ISectionRepository _repository;

        public UpdateItemOrderCommandHandler(ISectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultDto> Handle(UpdateItemOrderCommand request, CancellationToken cancellationToken)
        {
            var orders = request.Orders.Select(o => (o.ItemId, o.SortOrder)).ToList();
            await _repository.ReorderItemsAsync(request.SectionId, orders, cancellationToken);
            return ResultDto.Ok();
        }
    }
}

