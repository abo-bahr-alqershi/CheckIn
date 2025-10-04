using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.CP.PropertyInSectionImages;
using YemenBooking.Application.Commands.Images;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.CP.PropertyInSectionImages;
using YemenBooking.Core.Enums;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// إدارة صور "عقار في قسم"
    /// </summary>
    [Route("api/admin/property-in-section-images")]
    [ApiController]
    [Authorize]
    public class PropertyInSectionImagesController : BaseAdminController
    {
        public PropertyInSectionImagesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// رفع صورة لعقار في قسم
        /// </summary>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
            IFormFile file, 
            IFormFile? videoThumbnail,
            [FromForm] Guid? propertyInSectionId,
            [FromForm] string? tempKey,
            [FromForm] string? category, 
            [FromForm] string? alt, 
            [FromForm] bool? isPrimary, 
            [FromForm] int? order, 
            [FromForm] string? tags)
        {
            if (file == null || file.Length == 0) 
                return BadRequest(new { success = false, message = "file is required" });

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            
            FileUploadRequest? poster = null;
            if (videoThumbnail != null)
            {
                using var ps = new MemoryStream();
                await videoThumbnail.CopyToAsync(ps);
                poster = new FileUploadRequest 
                { 
                    FileName = videoThumbnail.FileName, 
                    FileContent = ps.ToArray(), 
                    ContentType = videoThumbnail.ContentType 
                };
            }

            var cmd = new UploadPropertyInSectionImageCommand
            {
                PropertyInSectionId = propertyInSectionId,
                TempKey = string.IsNullOrWhiteSpace(tempKey) ? null : tempKey,
                File = new FileUploadRequest 
                { 
                    FileName = file.FileName, 
                    FileContent = ms.ToArray(), 
                    ContentType = file.ContentType 
                },
                VideoThumbnail = poster,
                Name = Path.GetFileNameWithoutExtension(file.FileName),
                Extension = Path.GetExtension(file.FileName),
                Category = Enum.TryParse<ImageCategory>(category, true, out var cat) ? cat : ImageCategory.Gallery,
                Alt = alt,
                IsPrimary = isPrimary ?? false,
                Order = order,
                Tags = string.IsNullOrWhiteSpace(tags) ? null : new System.Collections.Generic.List<string>(tags.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            };

            var result = await _mediator.Send(cmd);
            return Ok(new { success = result.Success, message = result.Message, data = result.Data });
        }

        /// <summary>
        /// الحصول على صور عقار في قسم
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? propertyInSectionId,
            [FromQuery] string? tempKey,
            [FromQuery] string? sortBy = "order",
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] int? page = 1, 
            [FromQuery] int? limit = 50)
        {
            Guid? parsedId = null;
            if (!string.IsNullOrWhiteSpace(propertyInSectionId) && Guid.TryParse(propertyInSectionId, out var id))
            {
                parsedId = id;
            }

            var q = new GetPropertyInSectionImagesQuery 
            { 
                PropertyInSectionId = parsedId,
                TempKey = string.IsNullOrWhiteSpace(tempKey) ? null : tempKey,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = page ?? 1, 
                Limit = limit ?? 50 
            };

            var result = await _mediator.Send(q);
            if (!result.Success) 
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { 
                success = true, 
                images = result.Data,
                items = result.Data // للتوافقية
            });
        }

        /// <summary>
        /// تحديث بيانات صورة
        /// </summary>
        [HttpPut("{imageId}")]
        public async Task<IActionResult> Update(Guid imageId, [FromBody] UpdatePropertyInSectionImageCommand command)
        {
            command.ImageId = imageId;
            var result = await _mediator.Send(command);
            return Ok(new { success = result.Success, message = result.Message, data = result.Data });
        }

        /// <summary>
        /// حذف صورة
        /// </summary>
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(Guid imageId, [FromQuery] bool permanent = false)
        {
            var result = await _mediator.Send(new DeletePropertyInSectionImageCommand 
            { 
                ImageId = imageId, 
                Permanent = permanent 
            });
            
            return Ok(new { success = result.Success, message = result.Message });
        }

        /// <summary>
        /// إعادة ترتيب الصور
        /// </summary>
        [HttpPost("reorder")]
        public async Task<IActionResult> Reorder([FromBody] ReorderImagesRequest request)
        {
            var assignments = request.ImageIds
                .Select((id, index) => new ImageOrderAssignment 
                { 
                    ImageId = Guid.Parse(id), 
                    DisplayOrder = index + 1 
                })
                .ToList();

            var result = await _mediator.Send(new ReorderPropertyInSectionImagesCommand 
            { 
                Assignments = assignments 
            });

            if (!result.Success) 
                return BadRequest(new { success = false, message = result.Message });

            return NoContent();
        }

        /// <summary>
        /// تعيين صورة كرئيسية
        /// </summary>
        [HttpPost("{imageId}/set-primary")]
        public async Task<IActionResult> SetPrimary(
            Guid imageId,
            [FromBody] SetPrimaryRequest? request = null)
        {
            var result = await _mediator.Send(new UpdatePropertyInSectionImageCommand 
            { 
                ImageId = imageId, 
                IsPrimary = true,
                PropertyInSectionId = request?.PropertyInSectionId,
                TempKey = request?.TempKey
            });

            if (!result.Success) 
                return BadRequest(new { success = false, message = result.Message });

            return NoContent();
        }

        public class ReorderImagesRequest
        {
            public List<string> ImageIds { get; set; } = new();
            public Guid? PropertyInSectionId { get; set; }
            public string? TempKey { get; set; }
        }

        public class SetPrimaryRequest
        {
            public Guid? PropertyInSectionId { get; set; }
            public string? TempKey { get; set; }
        }
    }
}