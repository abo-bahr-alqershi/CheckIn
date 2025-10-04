using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Reports;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Reports
{
    /// <summary>
    /// معالج استعلام الحصول على البلاغات المتعلقة بكيان معين
    /// Handles GetReportsByPropertyQuery and returns paginated reports for a property
    /// </summary>
    public class GetReportsByPropertyQueryHandler : IRequestHandler<GetReportsByPropertyQuery, PaginatedResult<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetReportsByPropertyQueryHandler> _logger;

        public GetReportsByPropertyQueryHandler(
            IReportRepository reportRepository,
            ICurrentUserService currentUserService,
            ILogger<GetReportsByPropertyQueryHandler> logger)
        {
            _reportRepository = reportRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<ReportDto>> Handle(GetReportsByPropertyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب بلاغات الكيان: {PropertyId}, Page={Page}, Size={Size}", request.PropertyId, request.PageNumber, request.PageSize);

            if (request.PropertyId == Guid.Empty)
                throw new BusinessRuleException("InvalidPropertyId", "معرف الكيان غير صالح");

            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new BusinessRuleException("InvalidPagination", "رقم الصفحة وحجم الصفحة يجب أن يكونا أكبر من صفر");

            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.PropertyId != request.PropertyId)
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحيات لعرض بلاغات هذا الكيان");
                throw new ForbiddenException("ليس لديك صلاحية الوصول إلى بلاغات هذا الكيان");
            }

            var allReports = await _reportRepository.GetReportsAsync(null, null, request.PropertyId, cancellationToken);
            var totalCount = allReports.Count();
            var items = allReports
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReporterUserId = r.ReporterUserId,
                    ReporterUserName = r.ReporterUser.Name,
                    ReportedUserId = r.ReportedUserId,
                    ReportedUserName = r.ReportedUser != null ? r.ReportedUser.Name : null,
                    ReportedPropertyId = r.ReportedPropertyId,
                    ReportedPropertyName = r.ReportedProperty != null ? r.ReportedProperty.Name : null,
                    Reason = r.Reason,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            return PaginatedResult<ReportDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
} 