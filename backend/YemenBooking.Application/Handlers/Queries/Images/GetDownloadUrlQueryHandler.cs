using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Queries.Images;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Images
{
    /// <summary>
    /// معالج استعلام الحصول على رابط تنزيل مؤقت لصورة حسب الحجم
    /// Handler for GetDownloadUrlQuery to retrieve a temporary download URL for an image by size
    /// </summary>
    public class GetDownloadUrlQueryHandler : IRequestHandler<GetDownloadUrlQuery, ResultDto<string>>
    {
        private readonly IPropertyImageRepository _imageRepository;
        private readonly IFileStorageService _fileStorageService;

        public GetDownloadUrlQueryHandler(
            IPropertyImageRepository imageRepository,
            IFileStorageService fileStorageService)
        {
            _imageRepository = imageRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ResultDto<string>> Handle(GetDownloadUrlQuery request, CancellationToken cancellationToken)
        {
            // جلب بيانات الصورة
            var image = await _imageRepository.GetPropertyImageByIdAsync(request.ImageId, cancellationToken);
            if (image == null)
                return ResultDto<string>.Failure("الصورة غير موجودة");

            // تحديد المسار المطلوب حسب الحجم
            string filePath;
            if (!string.IsNullOrEmpty(request.Size))
            {
                try
                {
                    var sizes = JsonSerializer.Deserialize<Dictionary<string, string>>(image.Sizes);
                    if (sizes != null && sizes.TryGetValue(request.Size, out var specificPath))
                        filePath = specificPath;
                    else
                        return ResultDto<string>.Failure("الحجم المطلوب غير مدعوم");
                }
                catch
                {
                    return ResultDto<string>.Failure("خطأ في قراءة بيانات الأحجام");
                }
            }
            else
            {
                filePath = image.Url;
            }

            // جلب الرابط المؤقت من خدمة التخزين
            var downloadUrl = await _fileStorageService.GetFileUrlAsync(filePath, null, cancellationToken);
            return ResultDto<string>.Ok(downloadUrl);
        }
    }
} 