using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Amenities;

/// <summary>
/// أمر لحذف مرفق
/// Command to delete an amenity
/// </summary>
public class DeleteAmenityCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف المرفق
    /// Amenity identifier
    /// </summary>
    public Guid AmenityId { get; set; }
} 