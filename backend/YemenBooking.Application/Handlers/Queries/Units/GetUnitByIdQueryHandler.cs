using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Application.DTOs.Units;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using System.Collections.Generic;

namespace YemenBooking.Application.Handlers.Queries.Units
{
    /// <summary>
    /// معالج استعلام الحصول على بيانات الوحدة بواسطة المعرف
    /// Query handler for GetUnitByIdQuery
    /// </summary>
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, ResultDto<UnitDetailsDto>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetUnitByIdQueryHandler> _logger;

        public GetUnitByIdQueryHandler(
            IUnitRepository unitRepository,
            ICurrentUserService currentUserService,
            ILogger<GetUnitByIdQueryHandler> logger)
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<UnitDetailsDto>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الوحدة: {UnitId}", request.UnitId);

            // الوصول إلى الوحدة مع البيانات المرتبطة
            var unit = await _unitRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.Property)
                .Include(u => u.UnitType)
                .Include(u => u.FieldValues)
                    .ThenInclude(fv => fv.UnitTypeField)
                .FirstOrDefaultAsync(u => u.Id == request.UnitId, cancellationToken);

            if (unit == null)
            {
                return ResultDto<UnitDetailsDto>.Failure($"الوحدة بالمعرف {request.UnitId} غير موجود");
            }

            // التحقق من الصلاحيات
            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            var role = _currentUserService.Role;
            bool isOwner = currentUser != null && _currentUserService.UserId == unit.Property.OwnerId;
            if (role != "Admin" && !isOwner)
            {
                // الزوار يرون الوحدات المعتمدة والمتاحة فقط
                if (!unit.Property.IsApproved || !unit.IsAvailable)
                {
                    return ResultDto<UnitDetailsDto>.Failure("ليس لديك صلاحية لعرض هذه الوحدة");
                }
            }

            // التحويل إلى DTO
            var dto = new UnitDetailsDto
            {
                Id = unit.Id,
                PropertyId = unit.PropertyId,
                UnitTypeId = unit.UnitTypeId,
                Name = unit.Name,
                BasePrice = new MoneyDto { Amount = unit.BasePrice.Amount, Currency = unit.BasePrice.Currency },
                CustomFeatures = unit.CustomFeatures,
                IsAvailable = unit.IsAvailable,
                PropertyName = unit.Property.Name,
                UnitTypeName = unit.UnitType.Name,
                PricingMethod = unit.PricingMethod.ToString(),
                FieldValues = unit.FieldValues.Select(fv => new UnitFieldValueDto
                {
                    FieldId = fv.UnitTypeFieldId,
                    FieldName = fv.UnitTypeField.FieldName,
                    DisplayName = fv.UnitTypeField.DisplayName,
                    FieldType = fv.UnitTypeField.FieldTypeId,
                    FieldValue = fv.FieldValue,
                    IsPrimaryFilter = fv.UnitTypeField.IsPrimaryFilter,
                }).ToList(),
                DynamicFields = new List<FieldGroupWithValuesDto>()
            };

            _logger.LogInformation("تم الحصول على بيانات الوحدة بنجاح: {UnitId}", request.UnitId);
            return ResultDto<UnitDetailsDto>.Succeeded(dto);
        }
    }
}