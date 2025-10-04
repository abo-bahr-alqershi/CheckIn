using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.CP.SectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.SectionImages
{
    public class ReorderSectionImagesCommandHandler : IRequestHandler<ReorderSectionImagesCommand, ResultDto<bool>>
    {
        private readonly ISectionImageRepository _repo;
        public ReorderSectionImagesCommandHandler(ISectionImageRepository repo) { _repo = repo; }

        public async Task<ResultDto<bool>> Handle(ReorderSectionImagesCommand request, CancellationToken cancellationToken)
        {
            var tuples = request.Assignments.ConvertAll(a => (a.ImageId, a.DisplayOrder));
            var ok = await _repo.UpdateDisplayOrdersAsync(tuples, cancellationToken);
            return ok ? ResultDto<bool>.Ok(true) : ResultDto<bool>.Failed("فشل إعادة الترتيب");
        }
    }
}

