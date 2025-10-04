using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.CP.PropertyInSectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.PropertyInSectionImages
{
    public class ReorderPropertyInSectionImagesCommandHandler : IRequestHandler<ReorderPropertyInSectionImagesCommand, ResultDto<bool>>
    {
        private readonly IPropertyInSectionImageRepository _repo;
        public ReorderPropertyInSectionImagesCommandHandler(IPropertyInSectionImageRepository repo) { _repo = repo; }

        public async Task<ResultDto<bool>> Handle(ReorderPropertyInSectionImagesCommand request, CancellationToken cancellationToken)
        {
            var tuples = request.Assignments.ConvertAll(a => (a.ImageId, a.DisplayOrder));
            var ok = await _repo.UpdateDisplayOrdersAsync(tuples, cancellationToken);
            return ok ? ResultDto<bool>.Ok(true) : ResultDto<bool>.Failed("فشل إعادة الترتيب");
        }
    }
}

