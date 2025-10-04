using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Queries.Images;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Images
{
    /// <summary>
    /// معالج استعلام تتبع تقدم رفع الصورة بواسطة معرف المهمة
    /// Handler for GetUploadProgressQuery to track image upload progress by task ID
    /// </summary>
    public class GetUploadProgressQueryHandler : IRequestHandler<GetUploadProgressQuery, ResultDto<UploadProgressDto>>
    {
        private readonly IFileStorageService _fileStorageService;

        public GetUploadProgressQueryHandler(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<ResultDto<UploadProgressDto>> Handle(GetUploadProgressQuery request, CancellationToken cancellationToken)
        {
            // TODO: تنفيذ منطق تتبع تقدم الرفع باستخدام request.TaskId
            throw new NotImplementedException("منطق تتبع تقدم الرفع لم يتم تنفيذه بعد");
        }
    }
} 