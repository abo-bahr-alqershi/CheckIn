using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Favorites.Commands;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Favorites;

/// <summary>
/// معالج أمر إضافة عقار إلى المفضلة من تطبيق الجوال
/// </summary>
public class AddToFavoritesCommandHandler : IRequestHandler<AddToFavoritesCommand, AddToFavoritesResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AddToFavoritesCommandHandler> _logger;

    public AddToFavoritesCommandHandler(
        IUserRepository userRepository,
        IAuditService auditService,
        ICurrentUserService currentUserService,
        ILogger<AddToFavoritesCommandHandler> logger)
    {
        _userRepository = userRepository;
        _auditService = auditService;
        _currentUserService = currentUserService;
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

        // تدقيق يدوي مع ذكر اسم ومعرف المنفذ
        var performerName = _currentUserService.Username;
        var performerId = _currentUserService.UserId;
        var notes = $"تمت إضافة العقار {request.PropertyId} إلى المفضلة بواسطة {performerName} (ID={performerId})";
        await _auditService.LogAuditAsync(
            entityType: "User",
            entityId: user.Id,
            action: YemenBooking.Core.Entities.AuditAction.UPDATE,
            oldValues: null,
            newValues: JsonSerializer.Serialize(new { FavoriteAdded = request.PropertyId, UserId = user.Id }),
            performedBy: performerId,
            notes: notes,
            cancellationToken: cancellationToken);

        return new AddToFavoritesResponse { Success = true, Message = "تمت الإضافة بنجاح" };
    }
}
