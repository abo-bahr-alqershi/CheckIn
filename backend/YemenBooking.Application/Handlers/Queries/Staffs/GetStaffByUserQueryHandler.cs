using System;
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
    /// معالج استعلام الحصول على بيانات الموظف للمستخدم
    /// Handles GetStaffByUserQuery and returns staff details for a user
    /// </summary>
    public class GetStaffByUserQueryHandler : IRequestHandler<GetStaffByUserQuery, ResultDto<StaffDto>>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetStaffByUserQueryHandler> _logger;

        public GetStaffByUserQueryHandler(
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            ILogger<GetStaffByUserQueryHandler> logger)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<StaffDto>> Handle(GetStaffByUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب بيانات الموظف للمستخدم: {UserId}", request.UserId);

            if (request.UserId == Guid.Empty)
            {
                _logger.LogWarning("معرف المستخدم غير صالح");
                return ResultDto<StaffDto>.Failure("معرف المستخدم غير صالح");
            }

            if (_currentUserService.Role != "Admin" && _currentUserService.UserId != request.UserId)
            {
                _logger.LogWarning("ليس لدى المستخدم صلاحية الوصول إلى بيانات الموظف");
                return ResultDto<StaffDto>.Failure("ليس لديك صلاحية الوصول إلى بيانات الموظف");
            }

            var staff = await _staffRepository.GetStaffByUserAsync(request.UserId, cancellationToken);
            if (staff == null)
            {
                _logger.LogWarning("الموظف غير موجود للمستخدم: {UserId}", request.UserId);
                return ResultDto<StaffDto>.Failure("الموظف غير موجود");
            }

            var dto = new StaffDto
            {
                Id = staff.Id,
                UserId = staff.UserId,
                UserName = string.Empty,
                PropertyId = staff.PropertyId,
                PropertyName = string.Empty,
                Position = staff.Position,
                Permissions = staff.Permissions
            };

            return ResultDto<StaffDto>.Ok(dto);
        }
    }
} 