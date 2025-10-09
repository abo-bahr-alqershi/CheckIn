using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Favorites.Commands;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Favorites;

/// <summary>
/// معالج أمر إزالة عقار من المفضلة عبر تطبيق الجوال
/// </summary>
public class RemoveFromFavoritesCommandHandler : IRequestHandler<RemoveFromFavoritesCommand, RemoveFromFavoritesResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RemoveFromFavoritesCommandHandler> _logger;

    public RemoveFromFavoritesCommandHandler(
        IUserRepository userRepository,
        IAuditService auditService,
        ICurrentUserService currentUserService,
        ILogger<RemoveFromFavoritesCommandHandler> logger)
    {
        _userRepository = userRepository;
        _auditService = auditService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<RemoveFromFavoritesResponse> Handle(RemoveFromFavoritesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("إزالة العقار {PropertyId} من مفضلة المستخدم {UserId}", request.PropertyId, request.UserId);

        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return new RemoveFromFavoritesResponse { Success = false, Message = "المستخدم غير موجود" };

        var favorites = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(user.FavoritesJson) ?? new List<Guid>();
        if (!favorites.Remove(request.PropertyId))
            return new RemoveFromFavoritesResponse { Success = false, Message = "العقار غير موجود في المفضلة" };

        var json = System.Text.Json.JsonSerializer.Serialize(favorites);
        await _userRepository.UpdateUserFavoritesAsync(user.Id, json, cancellationToken);

        // تدقيق يدوي مع ذكر اسم ومعرف المنفذ
        var performerName = _currentUserService.Username;
        var performerId = _currentUserService.UserId;
        var notes = $"تمت إزالة العقار {request.PropertyId} من المفضلة بواسطة {performerName} (ID={performerId})";
        await _auditService.LogAuditAsync(
            entityType: "User",
            entityId: user.Id,
            action: YemenBooking.Core.Enums.AuditAction.UPDATE,
            oldValues: null,
            newValues: JsonSerializer.Serialize(new { FavoriteRemoved = request.PropertyId, UserId = user.Id }),
            performedBy: performerId,
            notes: notes,
            cancellationToken: cancellationToken);

        return new RemoveFromFavoritesResponse { Success = true, Message = "تمت الإزالة بنجاح" };
    }
}
