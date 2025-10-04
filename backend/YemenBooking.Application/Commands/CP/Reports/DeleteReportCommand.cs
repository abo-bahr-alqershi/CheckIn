using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Reports;

/// <summary>
/// أمر لحذف بلاغ
/// Command to delete a report
/// </summary>
public class DeleteReportCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف البلاغ
    /// Report identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// سبب الحذف (اختياري)
    /// Deletion reason (optional)
    /// </summary>
    public string? DeletionReason { get; set; }
} 