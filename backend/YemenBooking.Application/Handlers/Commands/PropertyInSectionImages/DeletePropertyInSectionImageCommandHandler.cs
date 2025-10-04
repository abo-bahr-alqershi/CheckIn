using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.CP.PropertyInSectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.PropertyInSectionImages
{
    public class DeletePropertyInSectionImageCommandHandler : IRequestHandler<DeletePropertyInSectionImageCommand, ResultDto<bool>>
    {
        private readonly IPropertyInSectionImageRepository _repo;
        private readonly IFileStorageService _files;
        public DeletePropertyInSectionImageCommandHandler(IPropertyInSectionImageRepository repo, IFileStorageService files)
        {
            _repo = repo;
            _files = files;
        }

        public async Task<ResultDto<bool>> Handle(DeletePropertyInSectionImageCommand request, CancellationToken cancellationToken)
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

