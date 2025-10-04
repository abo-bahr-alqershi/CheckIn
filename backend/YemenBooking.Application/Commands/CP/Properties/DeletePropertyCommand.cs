using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Properties;

/// <summary>
/// أمر لحذف الكيان
/// Command to delete a property
/// </summary>
public class DeletePropertyCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }
} 