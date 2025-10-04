using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.SearchFilters;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.SearchFilters
{
    /// <summary>
    /// معالج استعلام جلب فلاتر البحث لنوع كيان معين
    /// Query handler for GetSearchFiltersQuery
    /// </summary>
    public class GetSearchFiltersQueryHandler : IRequestHandler<GetSearchFiltersQuery, List<SearchFilterDto>>
    {
        private readonly ISearchFilterRepository _filterRepository;
        private readonly ILogger<GetSearchFiltersQueryHandler> _logger;

        public GetSearchFiltersQueryHandler(
            ISearchFilterRepository filterRepository,
            ILogger<GetSearchFiltersQueryHandler> logger)
        {
            _filterRepository = filterRepository;
            _logger = logger;
        }

        public async Task<List<SearchFilterDto>> Handle(GetSearchFiltersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام جلب فلاتر البحث لنوع الكيان: {PropertyTypeId}", request.PropertyTypeId);

            if (request.PropertyTypeId == Guid.Empty)
                throw new ValidationException(nameof(request.PropertyTypeId), "معرف نوع الكيان غير صالح");

            var filters = await _filterRepository.GetQueryable()
                .AsNoTracking()
                .Include(sf => sf.UnitTypeField)
                .Where(sf => sf.UnitTypeField.UnitTypeId == request.PropertyTypeId)
                .Where(sf => !request.IsActive.HasValue || sf.IsActive == request.IsActive.Value)
                .OrderBy(sf => sf.SortOrder)
                .ToListAsync(cancellationToken);

            return filters.Select(sf => new SearchFilterDto
            {
                FilterId = sf.Id,
                FieldId = sf.FieldId,
                FilterType = sf.FilterType,
                DisplayName = sf.DisplayName,
                FilterOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(sf.FilterOptions) ?? new Dictionary<string, object>(),
                IsActive = sf.IsActive,
                SortOrder = sf.SortOrder,
                Field = new UnitTypeFieldDto
                {
                    FieldId = sf.UnitTypeField.Id.ToString(),
                    UnitTypeId = sf.UnitTypeField.UnitTypeId.ToString(),
                    FieldTypeId = sf.UnitTypeField.FieldTypeId.ToString(),
                    FieldName = sf.UnitTypeField.FieldName,
                    DisplayName = sf.UnitTypeField.DisplayName,
                    Description = sf.UnitTypeField.Description,
                    FieldOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(sf.UnitTypeField.FieldOptions) ?? new Dictionary<string, object>(),
                    ValidationRules = JsonSerializer.Deserialize<Dictionary<string, object>>(sf.UnitTypeField.ValidationRules) ?? new Dictionary<string, object>(),
                    IsRequired = sf.UnitTypeField.IsRequired,
                    IsSearchable = sf.UnitTypeField.IsSearchable,
                    IsPublic = sf.UnitTypeField.IsPublic,
                    SortOrder = sf.UnitTypeField.SortOrder,
                    Category = sf.UnitTypeField.Category,
                    GroupId = sf.UnitTypeField.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty
                }
            }).ToList();
        }
    }
} 