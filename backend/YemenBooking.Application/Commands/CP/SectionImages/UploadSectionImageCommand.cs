using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.CP.SectionImages
{
    /// <summary>
    /// أمر لرفع صورة لقسم محدد
    /// </summary>
    public class UploadSectionImageCommand : IRequest<ResultDto<ImageDto>>
    {
        public Guid? SectionId { get; set; }
        public string? TempKey { get; set; }
        public FileUploadRequest File { get; set; } = null!;
        public FileUploadRequest? VideoThumbnail { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public ImageCategory Category { get; set; } = ImageCategory.Gallery;
        public string? Alt { get; set; }
        public bool? IsPrimary { get; set; }
        public int? Order { get; set; }
        public List<string>? Tags { get; set; }
    }
}

