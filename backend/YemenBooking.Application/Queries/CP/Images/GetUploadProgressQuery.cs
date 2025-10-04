using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Images
{
    /// <summary>
    /// استعلام لتتبع تقدم رفع الصورة بواسطة معرف المهمة
    /// Query to track image upload progress by task ID
    /// </summary>
    public class GetUploadProgressQuery : IRequest<ResultDto<UploadProgressDto>>
    {
        /// <summary>
        /// معرف المهمة لتتبع التقدم
        /// Task ID for progress tracking
        /// </summary>
        public string TaskId { get; set; } = string.Empty;
    }
} 