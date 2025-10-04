using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Favorites.Commands;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Favorites;

/// <summary>
/// معالج أمر إزالة عقار من المفضلة عبر تطبيق الجوال
/// </summary>
public class RemoveFromFavoritesCommandHandler : IRequestHandler<RemoveFromFavoritesCommand, RemoveFromFavoritesResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<RemoveFromFavoritesCommandHandler> _logger;

    public RemoveFromFavoritesCommandHandler(
        IUserRepository userRepository,
        IAuditService auditService,
        ILogger<RemoveFromFavoritesCommandHandler> logger)
    {
        _userRepository = userRepository;
        _auditService = auditService;
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

        await _auditService.LogBusinessOperationAsync(
            "RemoveFavorite",
            $"أُزيل العقار {request.PropertyId} من مفضلة المستخدم {user.Id}",
            user.Id,
            "User",
            user.Id,
            null,
            cancellationToken);

        return new RemoveFromFavoritesResponse { Success = true, Message = "تمت الإزالة بنجاح" };
    }
}
