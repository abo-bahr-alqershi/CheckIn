using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر تحديث إعدادات الشات الخاصة بالمستخدم
    /// </summary>
    public class UpdateChatSettingsCommandHandler : IRequestHandler<UpdateChatSettingsCommand, ResultDto<ChatSettingsDto>>
    {
        private readonly IChatSettingsRepository _settingsRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateChatSettingsCommandHandler> _logger;

        public UpdateChatSettingsCommandHandler(
            IChatSettingsRepository settingsRepo,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<UpdateChatSettingsCommandHandler> logger)
        {
            _settingsRepo = settingsRepo;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<ChatSettingsDto>> Handle(UpdateChatSettingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                _logger.LogInformation("تحديث إعدادات الشات للمستخدم {UserId}", userId);

                var settings = await _settingsRepo.GetByUserIdAsync(userId, cancellationToken)
                    ?? new ChatSettings { UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

                settings.NotificationsEnabled = request.NotificationsEnabled;
                settings.SoundEnabled = request.SoundEnabled;
                settings.ShowReadReceipts = request.ShowReadReceipts;
                settings.ShowTypingIndicator = request.ShowTypingIndicator;
                settings.Theme = request.Theme;
                settings.FontSize = request.FontSize;
                settings.AutoDownloadMedia = request.AutoDownloadMedia;
                settings.BackupMessages = request.BackupMessages;
                settings.UpdatedAt = DateTime.UtcNow;

                if (settings.Id == default)
                {
                    await _unitOfWork.Repository<ChatSettings>().AddAsync(settings, cancellationToken);
                }
                else
                {
                    _unitOfWork.Repository<ChatSettings>().UpdateAsync(settings, cancellationToken).GetAwaiter().GetResult();
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var dto = _mapper.Map<ChatSettingsDto>(settings);
                return ResultDto<ChatSettingsDto>.Ok(dto, "تم تحديث إعدادات الشات بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث إعدادات الشات");
                return ResultDto<ChatSettingsDto>.Failed("حدث خطأ أثناء تحديث الإعدادات");
            }
        }
    }
} 