using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Policies;

/// <summary>
/// أمر لحذف سياسة كيان
/// Command to delete a property policy
/// </summary>
public class DeletePropertyPolicyCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف السياسة
    /// Policy identifier
    /// </summary>
    public Guid PolicyId { get; set; }
} 