using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Policies;

/// <summary>
/// استعلام للحصول على سياسات الكيان
/// Query to get property policies
/// </summary>
public class GetPropertyPoliciesQuery : IRequest<ResultDto<IEnumerable<PolicyDto>>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }
}