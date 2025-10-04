using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Staffs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Staffs
{
    /// <summary>
    /// معالج استعلام الحصول على الموظفين حسب المنصب
    /// Handles GetStaffByPositionQuery and returns paginated staff by position
    /// </summary>
    public class GetStaffByPositionQueryHandler : IRequestHandler<GetStaffByPositionQuery, PaginatedResult<StaffDto>>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetStaffByPositionQueryHandler> _logger;

        public GetStaffByPositionQueryHandler(
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            ILogger<GetStaffByPositionQueryHandler> logger)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PaginatedResult<StaffDto>> Handle(GetStaffByPositionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب الموظفين بمنصب {Position}, Page={Page}, Size={Size}", request.Position, request.PageNumber, request.PageSize);

            if (string.IsNullOrWhiteSpace(request.Position))
                throw new ValidationException(nameof(request.Position), "المسمى الوظيفي غير صالح");

            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new BusinessRuleException("InvalidPagination", "رقم الصفحة وحجم الصفحة يجب أن يكونا أكبر من صفر");

            // Authorization: only admin
            if (_currentUserService.Role != "Admin")
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى بيانات الموظفين");
                throw new ForbiddenException("ليس لديك صلاحية الوصول إلى بيانات الموظفين");
            }

            var allStaff = await _staffRepository.GetStaffByPositionAsync(request.Position, request.PropertyId, cancellationToken);
            var totalCount = allStaff.Count();
            var items = allStaff
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new StaffDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    UserName = string.Empty,
                    PropertyId = s.PropertyId,
                    PropertyName = string.Empty,
                    Position = s.Position,
                    Permissions = s.Permissions
                })
                .ToList();

            return PaginatedResult<StaffDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
} 