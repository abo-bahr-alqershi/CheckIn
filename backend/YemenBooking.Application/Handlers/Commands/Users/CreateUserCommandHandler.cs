using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر إنشاء حساب مستخدم جديد
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ResultDto<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            IEmailService emailService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إنشاء مستخدم جديد: Email={Email}, Name={Name}", request.Email, request.Name);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<Guid>.Failed("الاسم مطلوب");
            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultDto<Guid>.Failed("البريد الإلكتروني مطلوب");
            if (string.IsNullOrWhiteSpace(request.Password))
                return ResultDto<Guid>.Failed("كلمة المرور مطلوبة");
            if (string.IsNullOrWhiteSpace(request.Phone))
                return ResultDto<Guid>.Failed("رقم الهاتف مطلوب");

            // التحقق من قواعد العمل
            if (await _userRepository.CheckEmailExistsAsync(request.Email, cancellationToken))
                return ResultDto<Guid>.Failed("البريد الإلكتروني مستخدم بالفعل");

            // تحقق من قوة كلمة المرور
            var (isValid, issues) = await _passwordHashingService.ValidatePasswordStrengthAsync(request.Password, cancellationToken);
            if (!isValid)
                return ResultDto<Guid>.Failed($"كلمة المرور غير قوية: {string.Join(", ", issues)}");

            // التنفيذ
            var hashedPassword = await _passwordHashingService.HashPasswordAsync(request.Password, cancellationToken);
            var user = new User
            {
                Name = request.Name.Trim(),
                Email = request.Email.Trim(),
                Password = hashedPassword,
                Phone = request.Phone.Trim(),
                ProfileImage = string.IsNullOrWhiteSpace(request.ProfileImage) ? string.Empty : request.ProfileImage.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                EmailConfirmed = false
            };
            var created = await _userRepository.CreateUserAsync(user, cancellationToken);

            // إرسال بريد ترحيبي بعد إنشاء الحساب
            await _emailService.SendWelcomeEmailAsync(created.Email, created.Name, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreateUser",
                $"تم إنشاء مستخدم جديد {created.Id} ({created.Email})",
                created.Id,
                "User",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل إنشاء المستخدم بنجاح: UserId={UserId}", created.Id);
            return ResultDto<Guid>.Succeeded(created.Id, "تم إنشاء المستخدم بنجاح");
        }
    }
} 