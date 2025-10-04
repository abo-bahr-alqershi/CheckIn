using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Images
{
    /// <summary>
    /// استعلام للحصول على صورة واحدة بواسطة المعرف
    /// Query to get a single image by its ID
    /// </summary>
    public class GetImageByIdQuery : IRequest<ResultDto<ImageDto>>
    {
        /// <summary>
        /// معرف الصورة
        /// Image unique identifier
        /// </summary>
        public Guid ImageId { get; set; }
    }
} 