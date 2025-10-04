using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.CP.UnitInSectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.UnitInSectionImages
{
    public class ReorderUnitInSectionImagesCommandHandler : IRequestHandler<ReorderUnitInSectionImagesCommand, ResultDto<bool>>
    {
        private readonly IUnitInSectionImageRepository _repo;
        public ReorderUnitInSectionImagesCommandHandler(IUnitInSectionImageRepository repo) { _repo = repo; }

        public async Task<ResultDto<bool>> Handle(ReorderUnitInSectionImagesCommand request, CancellationToken cancellationToken)
        {
            var tuples = request.Assignments.ConvertAll(a => (a.ImageId, a.DisplayOrder));
            var ok = await _repo.UpdateDisplayOrdersAsync(tuples, cancellationToken);
            return ok ? ResultDto<bool>.Ok(true) : ResultDto<bool>.Failed("فشل إعادة الترتيب");
        }
    }
}

