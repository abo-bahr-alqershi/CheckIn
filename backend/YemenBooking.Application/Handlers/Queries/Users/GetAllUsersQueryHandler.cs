using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Application.Queries.Users;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Users
{
    /// <summary>
    /// معالج استعلام الحصول على جميع المستخدمين
    /// Query handler for GetAllUsersQuery
    /// </summary>
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResult<object>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<object>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام جميع المستخدمين - الصفحة: {PageNumber}, الحجم: {PageSize}", request.PageNumber, request.PageSize);

            // التحقق من صلاحية المسؤول فقط
            var roles = _currentUserService.UserRoles;
            if (!roles.Contains("Admin"))
            {
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض قائمة المستخدمين");
            }

            // التحقق من صحة معاملات الصفحة
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _userRepository.GetQueryable().AsNoTracking();

            if (request.RoleId.HasValue)
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value));
            }
            else if (!string.IsNullOrWhiteSpace(request.RoleName))
            {
                var roleName = request.RoleName.Trim();
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == roleName));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(term)
                    || u.Email.ToLower().Contains(term)
                    || u.Phone.ToLower().Contains(term));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (request.CreatedAfter.HasValue)
                query = query.Where(u => u.CreatedAt >= request.CreatedAfter.Value);

            if (request.CreatedBefore.HasValue)
                query = query.Where(u => u.CreatedAt <= request.CreatedBefore.Value);

            if (request.LastLoginAfter.HasValue)
                query = query.Where(u => u.LastLoginDate.HasValue && u.LastLoginDate.Value >= request.LastLoginAfter.Value);

            if (!string.IsNullOrWhiteSpace(request.LoyaltyTier))
                query = query.Where(u => u.LoyaltyTier == request.LoyaltyTier);

            if (request.MinTotalSpent.HasValue)
                query = query.Where(u => u.TotalSpent >= request.MinTotalSpent.Value);

            // تطبيق الترتيب
            query = (request.SortBy?.Trim().ToLower(), request.IsAscending) switch
            {
                ("name", true) => query.OrderBy(u => u.Name),
                ("name", false) => query.OrderByDescending(u => u.Name),
                ("email", true) => query.OrderBy(u => u.Email),
                ("email", false) => query.OrderByDescending(u => u.Email),
                ("createdat", true) => query.OrderBy(u => u.CreatedAt),
                ("createdat", false) => query.OrderByDescending(u => u.CreatedAt),
                _ => query.OrderBy(u => u.Name)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = users.Select(u => (object)_mapper.Map<UserDto>(u)).ToList();

            _logger.LogInformation("تم جلب {Count} مستخدم من إجمالي {TotalCount}", dtos.Count, totalCount);
            return new PaginatedResult<object>(dtos, pageNumber, pageSize, totalCount);
        }
    }
} 