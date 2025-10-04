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
    /// معالج استعلام جلب تفاصيل الوحدة مع الحقول الديناميكية
    /// Query handler for GetUnitDetailsQuery
    /// </summary>
    public class GetUnitDetailsQueryHandler : IRequestHandler<GetUnitDetailsQuery, ResultDto<UnitDetailsDto>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetUnitDetailsQueryHandler> _logger;
        private readonly IFieldGroupRepository _groupRepository;

        public GetUnitDetailsQueryHandler(
            IUnitRepository unitRepository,
            ICurrentUserService currentUserService,
            ILogger<GetUnitDetailsQueryHandler> logger,
            IFieldGroupRepository groupRepository)
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _logger = logger;
            _groupRepository = groupRepository;
        }

        public async Task<ResultDto<UnitDetailsDto>> Handle(GetUnitDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام تفاصيل الوحدة: {UnitId}", request.UnitId);

            var unit = await _unitRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.Property)
                .Include(u => u.UnitType)
                .Include(u => u.FieldValues)
                    .ThenInclude(fv => fv.UnitTypeField)
                .FirstOrDefaultAsync(u => u.Id == request.UnitId, cancellationToken);

            if (unit == null)
                return ResultDto<UnitDetailsDto>.Failure($"الوحدة بالمعرف {request.UnitId} غير موجود");

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            var role = _currentUserService.Role;
            bool isOwner = currentUser != null && unit.Property.OwnerId == _currentUserService.UserId;
            if (role != "Admin" && !isOwner)
            {
                if (!unit.Property.IsApproved || !unit.IsAvailable)
                {
                    return ResultDto<UnitDetailsDto>.Failure("ليس لديك صلاحية لعرض هذه الوحدة");
                }
            }

            var dynamicFields = new List<FieldGroupWithValuesDto>();
            if (request.IncludeDynamicFields)
            {
                var groups = await _groupRepository.GetGroupsByUnitTypeIdAsync(unit.UnitTypeId, cancellationToken);
                foreach (var group in groups.OrderBy(g => g.SortOrder))
                {
                    var groupDto = new FieldGroupWithValuesDto
                    {
                        GroupId = group.Id,
                        GroupName = group.GroupName,
                        DisplayName = group.DisplayName,
                        Description = group.Description,
                        FieldValues = new List<FieldWithValueDto>()
                    };
                    foreach (var link in group.FieldGroupFields.OrderBy(l => l.SortOrder))
                    {
                        var valueEntity = unit.FieldValues.FirstOrDefault(v => v.UnitTypeFieldId == link.FieldId);
                        if (valueEntity != null)
                        {
                            groupDto.FieldValues.Add(new FieldWithValueDto
                            {
                                ValueId = valueEntity.Id,
                                FieldId = link.FieldId,
                                FieldName = link.UnitTypeField?.FieldName ?? string.Empty,
                                DisplayName = link.UnitTypeField?.DisplayName ?? string.Empty,
                                Value = valueEntity.FieldValue,
                                IsPrimaryFilter = link.UnitTypeField?.IsPrimaryFilter ?? false
                            });
                        }
                    }
                    dynamicFields.Add(groupDto);
                }
            }
            var dto = new UnitDetailsDto
            {
                Id = unit.Id,
                PropertyId = unit.PropertyId,
                UnitTypeId = unit.UnitTypeId,
                Name = unit.Name,
                BasePrice = new MoneyDto { Amount = unit.BasePrice.Amount, Currency = unit.BasePrice.Currency },
                CustomFeatures = unit.CustomFeatures,
                IsAvailable = unit.IsAvailable,
                MaxCapacity = unit.MaxCapacity,
                ViewCount = unit.ViewCount,
                BookingCount = unit.BookingCount,
                PropertyName = unit.Property.Name,
                UnitTypeName = unit.UnitType.Name,
                PricingMethod = unit.PricingMethod.ToString(),
                AllowsCancellation = unit.AllowsCancellation,
                CancellationWindowDays = unit.CancellationWindowDays,
                FieldValues = unit.FieldValues.Select(fv => new UnitFieldValueDto
                {
                    ValueId = fv.Id,
                    UnitId = fv.UnitId,
                    FieldId = fv.UnitTypeFieldId,
                    FieldName = fv.UnitTypeField?.FieldName ?? string.Empty,
                    DisplayName = fv.UnitTypeField?.DisplayName ?? string.Empty,
                    FieldType = fv.UnitTypeField?.FieldTypeId ?? string.Empty,
                    FieldValue = fv.FieldValue,
                    IsPrimaryFilter = fv.UnitTypeField?.IsPrimaryFilter ?? false,
                    Field = fv.UnitTypeField == null ? null : new UnitTypeFieldDto
                    {
                        FieldId = fv.UnitTypeField.Id.ToString(),
                        UnitTypeId = fv.UnitTypeField.UnitTypeId.ToString(),
                        FieldTypeId = fv.UnitTypeField.FieldTypeId,
                        FieldName = fv.UnitTypeField.FieldName,
                        DisplayName = fv.UnitTypeField.DisplayName,
                        Description = fv.UnitTypeField.Description,
                        // options and rules may be large; you can hydrate on demand
                        FieldOptions = new System.Collections.Generic.Dictionary<string, object>(),
                        ValidationRules = new System.Collections.Generic.Dictionary<string, object>(),
                        IsRequired = fv.UnitTypeField.IsRequired,
                        IsSearchable = fv.UnitTypeField.IsSearchable,
                        IsPublic = fv.UnitTypeField.IsPublic,
                        SortOrder = fv.UnitTypeField.SortOrder,
                        Category = fv.UnitTypeField.Category,
                        GroupId = fv.UnitTypeField.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty,
                        IsForUnits = fv.UnitTypeField.IsForUnits,
                        ShowInCards = fv.UnitTypeField.ShowInCards,
                        IsPrimaryFilter = fv.UnitTypeField.IsPrimaryFilter,
                        Priority = fv.UnitTypeField.Priority
                    },
                    CreatedAt = fv.CreatedAt,
                    UpdatedAt = fv.UpdatedAt
                }).ToList(),
                DynamicFields = dynamicFields
            };

            _logger.LogInformation("تم جلب تفاصيل الوحدة بنجاح: {UnitId}", request.UnitId);
            return ResultDto<UnitDetailsDto>.Succeeded(dto);
        }
    }
}