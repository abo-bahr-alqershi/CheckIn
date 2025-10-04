namespace YemenBooking.Application.Queries.SearchFilters;

using System;
using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب فلاتر البحث لنوع كيان معين
/// Get search filters
/// </summary>
public class GetSearchFiltersQuery : IRequest<List<SearchFilterDto>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// PropertyTypeId
    /// </summary>
    public Guid PropertyTypeId { get; set; }

    /// <summary>
    /// حالة التفعيل (اختياري)
    /// IsActive
    /// </summary>
    public bool? IsActive { get; set; }
} 