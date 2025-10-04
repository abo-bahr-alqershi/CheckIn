using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Application.Queries.Users;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Users
{
    /// <summary>
    /// معالج استعلام الحصول على بيانات مستخدم بواسطة المعرف
    /// Query handler for GetUserByIdQuery
    /// </summary>
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ResultDto<object>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<object>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام بيانات المستخدم بالمعرف: {UserId}", request.UserId);

            if (request.UserId == Guid.Empty)
                return ResultDto<object>.Failure("معرف المستخدم غير صالح");

            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return ResultDto<object>.Failure($"المستخدم بالمعرف {request.UserId} غير موجود");

            var currentUserId = _currentUserService.UserId;
            var roles = _currentUserService.UserRoles;
            if (currentUserId != request.UserId && !roles.Contains("Admin"))
                return ResultDto<object>.Failure("ليس لديك صلاحية لعرض بيانات هذا المستخدم");

            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("تم جلب بيانات المستخدم بنجاح: {UserId}", request.UserId);
            return ResultDto<object>.Ok(userDto, "تم جلب بيانات المستخدم بنجاح");
        }
    }
} 