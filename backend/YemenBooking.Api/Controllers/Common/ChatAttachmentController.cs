using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Api.Controllers.Common
{
    /// <summary>
    /// متحكم لتنزيل مرفقات الشات بشكل آمن
    /// Controller for secure download of chat attachments
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/common/chat/attachments")]
    public class ChatAttachmentController : ControllerBase
    {
        private readonly IChatAttachmentRepository _attachmentRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ChatAttachmentController> _logger;

        public ChatAttachmentController(
            IChatAttachmentRepository attachmentRepo,
            IChatConversationRepository conversationRepo,
            ICurrentUserService currentUserService,
            ILogger<ChatAttachmentController> logger)
        {
            _attachmentRepo = attachmentRepo;
            _conversationRepo = conversationRepo;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// تنزيل مرفق محادثة بعد التحقق من صلاحيات المستخدم
        /// Download a chat attachment after verifying user access
        /// </summary>
        [HttpGet("{attachmentId}")]
        public async Task<IActionResult> DownloadAsync(Guid attachmentId)
        {
            var userId = _currentUserService.UserId;
            var attachment = await _attachmentRepo.GetByIdAsync(attachmentId);
            if (attachment == null)
                return NotFound();

            // تحقق من كون المستخدم مشاركاً في المحادثة
            var conv = await _conversationRepo.GetByIdWithParticipantsAsync(attachment.ConversationId);
            if (conv == null || !conv.Participants.Any(p => p.Id == userId))
                return Forbid();

            var filePath = attachment.FilePath;
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _logger.LogInformation("User {UserId} downloaded attachment {AttachmentId}", userId, attachmentId);
            return File(fileStream, attachment.ContentType, attachment.FileName);
        }
    }
} 