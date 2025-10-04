using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Properties;

namespace YemenBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// واجهة مستودع البحث المتقدم عن الكيانات
    /// Defines methods for advanced property search queries
    /// </summary>
    public interface IAdvancedPropertySearchRepository
    {
        Task<IEnumerable<AdvancedPropertyDto>> SearchAsync(AdvancedPropertySearchQuery query, CancellationToken cancellationToken = default);
    }
} 