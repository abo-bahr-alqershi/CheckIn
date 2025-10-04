using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using System.Text.Json;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Settings;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر إرسال رسالة في المحادثة
    /// </summary>
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ResultDto<ChatMessageDto>>
    {
        private readonly IChatConversationRepository _conversationRepository;
        private readonly IChatMessageRepository _messageRepository;
        private readonly IChatAttachmentRepository _attachmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly IFirebaseService _firebaseService;
        private readonly ChatAttachmentSettings _attachmentSettings;
        private readonly ILogger<SendMessageCommandHandler> _logger;

        public SendMessageCommandHandler(
            IChatConversationRepository conversationRepository,
            IChatMessageRepository messageRepository,
            IChatAttachmentRepository attachmentRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileStorageService fileStorageService,
            IMapper mapper,
            IFirebaseService firebaseService,
            IOptions<ChatAttachmentSettings> attachmentSettings,
            ILogger<SendMessageCommandHandler> logger)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _attachmentRepository = attachmentRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _firebaseService = firebaseService;
            _attachmentSettings = attachmentSettings.Value;
            _logger = logger;
        }

        public async Task<ResultDto<ChatMessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                _logger.LogInformation("المستخدم {UserId} يرسل رسالة في المحادثة {ConversationId}", userId, request.ConversationId);

                // تحقق من وجود المحادثة
                var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
                if (conversation == null)
                    return ResultDto<ChatMessageDto>.Failed("المحادثة غير موجودة", errorCode: "conversation_not_found");

                // السماح فقط بالمحادثات الثنائية المباشرة
                if (!string.Equals(conversation.ConversationType, "direct", StringComparison.OrdinalIgnoreCase))
                {
                    return ResultDto<ChatMessageDto>.Failed("إرسال الرسائل مسموح فقط في المحادثات الثنائية المباشرة", errorCode: "direct_only");
                }

                // تأكد أن عدد المشاركين اثنان فقط
                var participantCount = (conversation.Participants?.Count ?? 0);
                if (participantCount != 2)
                {
                    // حاول تحميل المشاركين الكاملين قبل الرفض (في حال جلب بدون تضمين المشاركين)
                    var convWithDetails = await _conversationRepository.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken);
                    participantCount = (convWithDetails?.Participants?.Count ?? participantCount);
                    if (participantCount != 2)
                    {
                        return ResultDto<ChatMessageDto>.Failed("هذه المحادثة ليست ثنائية صالحة", errorCode: "invalid_participants_count");
                    }
                    conversation = convWithDetails ?? conversation;
                }

                // تحقق من صلاحيات الإرسال وفق القيود المطلوبة
                // Admin: يستطيع مراسلة الجميع
                // Owner/Staff: مراسلة Admin أو العملاء الذين سبق مراسلتهُم للعقار
                // Client: مراسلة Admin أو العقارات
                var roles = (_currentUserService.UserRoles ?? Enumerable.Empty<string>()).Select(r => (r ?? string.Empty).ToLowerInvariant()).ToList();
                var accountRole = (_currentUserService.AccountRole ?? string.Empty).ToLowerInvariant();
                var isAdmin = roles.Contains("admin") || roles.Contains("super_admin") || accountRole == "admin";
                var isOwner = roles.Contains("owner") || roles.Contains("hotel_owner") || accountRole == "owner";
                var isStaff = roles.Contains("staff") || roles.Contains("hotel_manager") || roles.Contains("receptionist") || accountRole == "staff";
                var isClient = roles.Contains("client") || roles.Contains("customer") || accountRole == "client";

                if (!isAdmin)
                {
                    if (isOwner || isStaff)
                    {
                        if (!conversation.PropertyId.HasValue || !_currentUserService.PropertyId.HasValue || conversation.PropertyId.Value != _currentUserService.PropertyId!.Value)
                        {
                            return ResultDto<ChatMessageDto>.Failed("لا يمكنك الإرسال في محادثة لا تخص عقارك", errorCode: "property_mismatch");
                        }

                        // الطرف الآخر Admin أو عميل سبق مراسلة العقار
                        var otherParticipants = conversation.Participants.Where(p => p.Id != userId).ToList();
                        if (otherParticipants.Count != 1)
                        {
                            return ResultDto<ChatMessageDto>.Failed("محادثات المالك/الموظف يجب أن تكون ثنائية", errorCode: "invalid_conversation_participants");
                        }
                        var other = otherParticipants[0];
                        var otherRoles = (other.UserRoles ?? new List<UserRole>()).Select(ur => ur.Role?.Name?.ToLowerInvariant() ?? string.Empty).ToList();
                        var otherIsAdmin = otherRoles.Contains("admin") || otherRoles.Contains("super_admin");
                        var otherIsClient = otherRoles.Contains("client") || otherRoles.Contains("customer");
                        if (!otherIsAdmin)
                        {
                            var ok = await _conversationRepository.ExistsConversationBetweenClientAndPropertyAsync(other.Id, conversation.PropertyId.Value, cancellationToken);
                            if (!ok)
                            {
                                return ResultDto<ChatMessageDto>.Failed("لا يمكنك مراسلة هذا العميل لعدم وجود محادثة سابقة مع العقار", errorCode: "no_prior_client_property_chat");
                            }
                        }
                    }
                    else if (isClient)
                    {
                        // عميل: يستطيع الإرسال في محادثة مع Admin أو مع محادثة لعقار
                        // إذا كانت المحادثة بدون PropertyId وليست مع Admin، نمنع
                        var otherParticipants = conversation.Participants.Where(p => p.Id != userId).ToList();
                        var otherRoles = otherParticipants.SelectMany(p => (p.UserRoles ?? new List<UserRole>()).Select(ur => ur.Role?.Name?.ToLowerInvariant() ?? string.Empty)).ToList();
                        var anyAdmin = otherRoles.Contains("admin") || otherRoles.Contains("super_admin");
                        var isPropertyChat = conversation.PropertyId.HasValue;
                        if (!anyAdmin && !isPropertyChat)
                        {
                            return ResultDto<ChatMessageDto>.Failed("لا يمكنك الإرسال إلا لمحادثات مع الإدارة أو مع عقار", errorCode: "client_restriction");
                        }
                    }
                    else
                    {
                        // أدوار أخرى (إن وجدت): امنع بشكل آمن
                        return ResultDto<ChatMessageDto>.Failed("ليست لديك صلاحية للإرسال في هذه المحادثة", errorCode: "permission_denied");
                    }
                }

                // إنشاء الرسالة
                var message = new ChatMessage
                {
                    Id = Guid.NewGuid(),
                    ConversationId = request.ConversationId,
                    SenderId = userId,
                    MessageType = request.MessageType,
                    Content = request.Content,
                    LocationJson = request.LocationJson,
                    ReplyToMessageId = request.ReplyToMessageId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<ChatMessage>().AddAsync(message, cancellationToken);

                // معالجة المرفقات
                if (request.Attachments != null)
                {
                    var folder = $"{_attachmentSettings.BasePath}/{request.ConversationId}";
                    foreach (var file in request.Attachments)
                    {
                        var result = await _fileStorageService.UploadFileAsync(file.OpenReadStream(), file.FileName, file.ContentType, folder, cancellationToken);
                        if (result.IsSuccess)
                        {
                            var attachment = new ChatAttachment
                            {
                                Id = Guid.NewGuid(),
                                ConversationId = request.ConversationId,
                                FileName = result.FileName ?? file.FileName,
                                ContentType = result.ContentType ?? string.Empty,
                                FileSize = result.FileSizeBytes,
                                FilePath = result.FilePath ?? string.Empty,
                                UploadedBy = userId,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _unitOfWork.Repository<ChatAttachment>().AddAsync(attachment, cancellationToken);
                        }
                    }
                }

                // Associate pre-uploaded attachments if provided (link to this message)
                if (request.AttachmentIds != null && request.AttachmentIds.Count > 0)
                {
                    foreach (var attachId in request.AttachmentIds)
                    {
                        var attachment = await _attachmentRepository.GetByIdAsync(attachId, cancellationToken);
                        if (attachment != null && attachment.ConversationId == request.ConversationId)
                        {
                            attachment.MessageId = message.Id;
                            // ensure EF tracks update
                            await _unitOfWork.Repository<ChatAttachment>().UpdateAsync(attachment, cancellationToken);
                            message.Attachments.Add(attachment);
                        }
                    }
                }

                // حدث وقت آخر تحديث للمحادثة لضمان ترتيب القائمة حسب الأحدث
                // Important for conversations list ordering
                conversation.UpdatedAt = message.UpdatedAt;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var messageDto = _mapper.Map<ChatMessageDto>(message);

                // تأكد من تحميل المشاركين لإرسال الإشعارات عبر FCM للمشاركين كافة
                var conversationWithDetails = await _conversationRepository.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken)
                                            ?? conversation;

                // إرسال إشعار صامت للمرسل لتحديث الواجهة فورًا، وإشعار مرئي للمستقبل
                foreach (var p in conversationWithDetails.Participants)
                {
                    var isSender = p.Id == userId;
                    await _firebaseService.SendNotificationAsync($"user_{p.Id}", isSender ? string.Empty : "رسالة جديدة", isSender ? string.Empty : (message.Content ?? string.Empty), new Dictionary<string, string>
                    {
                        { "type", "new_message" },
                        { "conversation_id", request.ConversationId.ToString() },
                        { "message_id", message.Id.ToString() },
                        { "silent", isSender ? "true" : "false" }
                    }, cancellationToken);
                }

                return ResultDto<ChatMessageDto>.Ok(messageDto, "تم إرسال الرسالة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إرسال الرسالة");
                return ResultDto<ChatMessageDto>.Failed("حدث خطأ أثناء إرسال الرسالة");
            }
        }
    }
}