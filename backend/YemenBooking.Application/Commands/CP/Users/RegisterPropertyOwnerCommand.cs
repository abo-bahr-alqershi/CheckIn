using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;

namespace YemenBooking.Application.Commands.CP.Users;

/// <summary>
/// أمر تسجيل مالك عقار جديد مع إنشاء عقار مرتبط
/// Register a new property owner and create a linked property
/// </summary>
public class RegisterPropertyOwnerCommand : IRequest<ResultDto<RegisterPropertyOwnerResponse>>
{
    // User fields
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Property fields
    public Guid PropertyTypeId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int StarRating { get; set; } = 3;
    public string? Description { get; set; }
    public string? Currency { get; set; }
}

