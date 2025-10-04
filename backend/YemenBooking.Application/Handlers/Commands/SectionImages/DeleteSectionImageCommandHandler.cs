using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.CP.SectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.SectionImages
{
    public class DeleteSectionImageCommandHandler : IRequestHandler<DeleteSectionImageCommand, ResultDto<bool>>
    {
        private readonly ISectionImageRepository _repo;
        private readonly IFileStorageService _files;
        public DeleteSectionImageCommandHandler(ISectionImageRepository repo, IFileStorageService files)
        {
            _repo = repo;
            _files = files;
        }

        public async Task<ResultDto<bool>> Handle(DeleteSectionImageCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByIdAsync(request.ImageId, cancellationToken);
            if (entity == null) return ResultDto<bool>.Failed("الصورة غير موجودة");
            var url = entity.Url;
            var ok = await _repo.DeleteAsync(request.ImageId, cancellationToken);
            if (ok && request.Permanent && !string.IsNullOrWhiteSpace(url))
            {
                await _files.DeleteFileAsync(url!, cancellationToken);
            }
            return ResultDto<bool>.Ok(ok);
        }
    }
}

