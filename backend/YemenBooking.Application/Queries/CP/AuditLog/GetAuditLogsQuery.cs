using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.AuditLog;

/// <summary>
/// استعلام للحصول على سجلات التدقيق مع فلترة حسب المستخدم أو الفترة الزمنية
/// Query to get audit logs filtered by user or date range
/// </summary>
public class GetAuditLogsQuery : IRequest<PaginatedResult<AuditLogDto>>
{
    /// <summary>
    /// نوع الكيان للتصفية (اختياري) مثال: Booking, Property, Unit
    /// Entity type to filter (optional)
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// معرف السجل/الكيان للتصفية (اختياري)
    /// Record identifier to filter (optional)
    /// </summary>
    public Guid? RecordId { get; set; }

    /// <summary>
    /// معرف المستخدم (اختياري)
    /// User identifier (optional)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// تاريخ بداية الفلترة (اختياري)
    /// Start date for filtering (optional)
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// تاريخ نهاية الفلترة (اختياري)
    /// End date for filtering (optional)
    /// </summary>
    public DateTime? To { get; set; }

    /// <summary>
    /// مصطلح البحث النصي في سجلات التدقيق (اختياري)
    /// Full-text search term in audit logs (optional)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// نوع العملية للفلترة (اختياري)
    /// Operation type filter (optional)
    /// </summary>
    public string? OperationType { get; set; }

    /// <summary>
    /// رقم الصفحة (افتراضي 1)
    /// Page number (default 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة (افتراضي 20)
    /// Page size (default 20)
    /// </summary>
    public int PageSize { get; set; } = 20;
} 