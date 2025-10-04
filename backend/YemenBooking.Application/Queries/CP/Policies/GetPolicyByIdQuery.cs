using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Policies;

/// <summary>
/// استعلام للحصول على سياسة محددة
/// Query to get specific policy
/// </summary>
public class GetPolicyByIdQuery : IRequest<ResultDto<PolicyDetailsDto>>
{
    /// <summary>
    /// معرف السياسة
    /// Policy ID
    /// </summary>
    public Guid PolicyId { get; set; }
} 