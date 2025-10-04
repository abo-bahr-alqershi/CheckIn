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
    /// معالج استعلام البحث عن المستخدمين
    /// Query handler for SearchUsersQuery
    /// </summary>
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, PaginatedResult<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchUsersQueryHandler> _logger;

        public SearchUsersQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<SearchUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<UserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام البحث عن المستخدمين - مصطلح البحث: {SearchTerm}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.SearchTerm, request.PageNumber, request.PageSize);

            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                return PaginatedResult<UserDto>.Empty(request.PageNumber, request.PageSize);

            // صلاحيات المسؤول فقط
            if (!_currentUserService.UserRoles.Contains("Admin"))
                throw new UnauthorizedAccessException("ليس لديك صلاحية للبحث عن المستخدمين");

            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var term = request.SearchTerm.Trim().ToLower();
            var query = _userRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Where(u => u.Name.ToLower().Contains(term) || u.Email.ToLower().Contains(term) || u.Phone.ToLower().Contains(term));

            if (request.RoleId.HasValue)
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value));
            else if (!string.IsNullOrWhiteSpace(request.RoleName))
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == request.RoleName));

            if (request.IsActive.HasValue)
                query = query.Where(u => u.IsActive == request.IsActive.Value);

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
            query = request.SortBy?.Trim().ToLower() switch
            {
                "registration_date" => query.OrderBy(u => u.CreatedAt),
                "last_login" => query.OrderBy(u => u.LastLoginDate),
                "total_spent" => query.OrderBy(u => u.TotalSpent),
                _ => query.OrderBy(u => u.Name)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = users.Select(u => _mapper.Map<UserDto>(u)).ToList();

            _logger.LogInformation("تم جلب {Count} مستخدم من إجمالي {TotalCount} بناءً على البحث", dtos.Count, totalCount);
            return new PaginatedResult<UserDto>(dtos, pageNumber, pageSize, totalCount);
        }
    }
} 