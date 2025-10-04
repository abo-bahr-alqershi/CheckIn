using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Favorites.Commands;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Favorites;

/// <summary>
/// معالج أمر إضافة عقار إلى المفضلة من تطبيق الجوال
/// </summary>
public class AddToFavoritesCommandHandler : IRequestHandler<AddToFavoritesCommand, AddToFavoritesResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<AddToFavoritesCommandHandler> _logger;

    public AddToFavoritesCommandHandler(
        IUserRepository userRepository,
        IAuditService auditService,
        ILogger<AddToFavoritesCommandHandler> logger)
    {
        _userRepository = userRepository;
        _auditService = auditService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AddToFavoritesResponse> Handle(AddToFavoritesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("إضافة العقار {PropertyId} إلى مفضلة المستخدم {UserId}", request.PropertyId, request.UserId);

        // جلب المستخدم
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return new AddToFavoritesResponse { Success = false, Message = "المستخدم غير موجود" };

        // تحويل JSON إلى قائمة وتحديثها ثم حفظها
        var favorites = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(user.FavoritesJson) ?? new List<Guid>();
        if (favorites.Contains(request.PropertyId))
            return new AddToFavoritesResponse { Success = false, Message = "العقار موجود مسبقاً في المفضلة" };

        favorites.Add(request.PropertyId);
        var json = System.Text.Json.JsonSerializer.Serialize(favorites);
        await _userRepository.UpdateUserFavoritesAsync(user.Id, json, cancellationToken);

        await _auditService.LogBusinessOperationAsync(
            "AddFavorite",
            $"أُضيف العقار {request.PropertyId} إلى مفضلة المستخدم {user.Id}",
            user.Id,
            "User",
            user.Id,
            null,
            cancellationToken);

        return new AddToFavoritesResponse { Success = true, Message = "تمت الإضافة بنجاح" };
    }
}
