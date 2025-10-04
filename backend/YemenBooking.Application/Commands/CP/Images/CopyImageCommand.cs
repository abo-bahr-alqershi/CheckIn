using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Images
{
    /// <summary>
    /// أمر لنسخ صورة لكيان أو وحدة أخرى
    /// Command to copy an image to another property or unit
    /// </summary>
    public class CopyImageCommand : IRequest<ResultDto<ImageDto>>
    {
        /// <summary>
        /// معرف الصورة التي سيتم نسخها
        /// Identifier of the image to copy
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// معرف الكيان الهدف (اختياري)
        /// Target property ID (optional)
        /// </summary>
        public Guid? TargetPropertyId { get; set; }

        /// <summary>
        /// معرف الوحدة الهدف (اختياري)
        /// Target unit ID (optional)
        /// </summary>
        public Guid? TargetUnitId { get; set; }
    }
} 