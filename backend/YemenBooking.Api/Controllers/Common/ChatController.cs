using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Api.Controllers.Common;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.Queries.Chat;
using YemenBooking.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace YemenBooking.Api.Controllers.Common
{
    /// <summary>
    /// متحكم للشات: المحادثات والرسائل والإعدادات
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/common/[controller]")]
    public class ChatController : BaseCommonController
    {
        public ChatController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء محادثة جديدة
        /// </summary>
        [HttpPost("conversations")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// جلب المحادثات الخاصة بالمستخدم
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations([FromQuery] GetConversationsByParticipantQuery query)
        {
            var data = await _mediator.Send(query);
            return Ok(new
            {
                conversations = data.Items,
                total_count = data.TotalCount,
                has_more = data.HasNextPage,
                next_page = data.NextPageNumber
            });
        }

        /// <summary>
        /// جلب محادثة واحدة بناءً على المعرف
        /// </summary>
        [HttpGet("conversations/{conversationId}")]
        public async Task<IActionResult> GetConversation(Guid conversationId)
        {
            var result = await _mediator.Send(new GetConversationByIdQuery { ConversationId = conversationId });
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// أرشفة محادثة
        /// </summary>
        [HttpPost("conversations/{conversationId}/archive")]
        public async Task<IActionResult> ArchiveConversation(Guid conversationId)
        {
            var result = await _mediator.Send(new ArchiveConversationCommand { ConversationId = conversationId });
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// إلغاء أرشفة محادثة
        /// </summary>
        [HttpPost("conversations/{conversationId}/unarchive")]
        public async Task<IActionResult> UnarchiveConversation(Guid conversationId)
        {
            var result = await _mediator.Send(new UnarchiveConversationCommand { ConversationId = conversationId });
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// جلب الرسائل في محادثة
        /// </summary>
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] GetMessagesByConversationQuery query)
        {
            query.ConversationId = conversationId;
            var data = await _mediator.Send(query);
            return Ok(new
            {
                messages = data.Items,
                total_count = data.TotalCount,
                has_more = data.HasNextPage,
                next_page = data.NextPageNumber
            });
        }

        /// <summary>
        /// إرسال رسالة في المحادثة
        /// </summary>
        [HttpPost("conversations/{conversationId}/messages")]
        public async Task<IActionResult> SendMessage(Guid conversationId, [FromForm] SendMessageCommand command)
        {
            command.ConversationId = conversationId;
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result.Data);
        }

        /// <summary>
        /// إضافة تفاعل على رسالة
        /// </summary>
        [HttpPost("messages/{messageId}/reactions")]
        public async Task<IActionResult> AddReaction(Guid messageId, [FromBody] AddReactionCommand command)
        {
            command.MessageId = messageId;
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// إزالة تفاعل من رسالة
        /// </summary>
        [HttpDelete("messages/{messageId}/reactions/{reactionType}")]
        public async Task<IActionResult> RemoveReaction(Guid messageId, string reactionType)
        {
            var result = await _mediator.Send(new RemoveReactionCommand { MessageId = messageId, ReactionType = reactionType });
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// تحديث حالة الرسالة (sent, delivered, read, failed)
        /// </summary>
        [HttpPut("messages/{messageId}/status")]
        public async Task<IActionResult> UpdateMessageStatus(Guid messageId, [FromBody] UpdateMessageStatusCommand command)
        {
            command.MessageId = messageId;
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok();
        }

        /// <summary>
        /// حذف رسالة
        /// </summary>
        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var result = await _mediator.Send(new DeleteMessageCommand { MessageId = messageId });
            if (!result.Success) return NotFound(result);
            return Ok(result.Message);
        }

        /// <summary>
        /// تعديل محتوى رسالة
        /// </summary>
        [HttpPut("messages/{messageId}")]
        public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] EditMessageCommand command)
        {
            command.MessageId = messageId;
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// بحث في المحادثات والرسائل
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchChats([FromQuery] SearchChatsQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Success) return BadRequest(result);
            var data = result.Data;
            return Ok(new
            {
                messages = data.Messages,
                conversations = data.Conversations,
                total_count = data.TotalCount,
                has_more = data.HasMore,
                next_page = data.NextPageNumber
            });
        }

        /// <summary>
        /// جلب قائمة المستخدمين المتاحين للمحادثة
        /// </summary>
        [HttpGet("users/available")]
        public async Task<IActionResult> GetAvailableUsers([FromQuery] GetAvailableUsersQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Success) return BadRequest(result);
            return Ok(new { users = result.Data, success = true });
        }

        /// <summary>
        /// جلب حسابات الإدارة فقط (ADMIN, SUPER_ADMIN)
        /// </summary>
        [HttpGet("users/admins")]
        public async Task<IActionResult> GetAdminUsers()
        {
            var result = await _mediator.Send(new GetAdminUsersQuery());
            if (!result.Success) return BadRequest(result);
            return Ok(new { users = result.Data, success = true });
        }

        /// <summary>
        /// تحديث حالة المستخدم (online, offline, away, busy)
        /// </summary>
        [HttpPut("users/status")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok();
        }

        /// <summary>
        /// جلب إعدادات الشات الخاصة بالمستخدم
        /// </summary>
        [HttpGet("settings")]
        public async Task<IActionResult> GetChatSettings()
        {
            var result = await _mediator.Send(new GetChatSettingsQuery());
            if (!result.Success) return BadRequest(result);
            return Ok(result.Data);
        }

        /// <summary>
        /// تحديث إعدادات الشات الخاصة بالمستخدم
        /// </summary>
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateChatSettings([FromBody] UpdateChatSettingsCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result.Data);
        }

        /// <summary>
        /// حذف محادثة
        /// </summary>
        [HttpDelete("conversations/{conversationId}")]
        public async Task<IActionResult> DeleteConversation(Guid conversationId)
        {
            var result = await _mediator.Send(new DeleteConversationCommand { ConversationId = conversationId });
            if (!result.Success) return NotFound(result);
            return Ok(result.Message);
        }

        /// <summary>
        /// رفع ملف مرفق في الشات
        /// Upload chat attachment file
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(new { attachment = result.Data, success = true });
        }
    }
} 