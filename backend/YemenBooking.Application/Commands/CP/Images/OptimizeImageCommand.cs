using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Images
{
    /// <summary>
    /// أمر لتحسين جودة الصورة وإنشاء مصغرات حسب الإعدادات
    /// Command to optimize an image and generate thumbnails according to settings
    /// </summary>
    public class OptimizeImageCommand : IRequest<ResultDto<ImageDto>>
    {
        /// <summary>
        /// معرف الصورة
        /// Image unique identifier
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// إعدادات معالجة الصورة
        /// Image processing settings (quality, dimensions, format, thumbnails)
        /// </summary>
        public ImageProcessingSettingsDto Settings { get; set; } = null!;
    }
} 