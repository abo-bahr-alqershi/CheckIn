using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Properties;

/// <summary>
/// أمر للموافقة على الكيان من قبل الإدارة
/// Command to approve a property by administration
/// </summary>
public class ApprovePropertyCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// معرف المسؤول الذي قام بالموافقة
    /// Admin ID who approved
    /// </summary>
    public Guid AdminId { get; set; }
} 