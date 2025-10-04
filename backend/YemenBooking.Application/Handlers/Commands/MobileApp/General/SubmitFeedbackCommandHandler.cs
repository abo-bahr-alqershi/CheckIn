using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Feedback.Commands;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.General;

/// <summary>
/// معالج أمر إرسال تعليقات المستخدم
/// </summary>
public class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, SubmitFeedbackResponse>
{
    private readonly ILogger<SubmitFeedbackCommandHandler> _logger;
    private readonly IEmailService _emailService;

    public SubmitFeedbackCommandHandler(ILogger<SubmitFeedbackCommandHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<SubmitFeedbackResponse> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("استلام تعليق جديد من المستخدم {UserId}", request.UserId);

        // TODO: حفظ التعليق في قاعدة البيانات (Feedback table)
        // في الوقت الحالي سنكتفي بإرسال بريد إلى فريق الدعم
        var body = $"نوع: {request.FeedbackType}\nموضوع: {request.Subject}\nمحتوى: {request.Content}";
        await _emailService.SendEmailAsync("support@yemenbooking.com", "تعليق جديد من التطبيق", body, true, cancellationToken);

        return new SubmitFeedbackResponse
        {
            Success = true,
            ReferenceNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
            Message = "تم استلام تعليقك، شكرًا لك!"
        };
    }
}
