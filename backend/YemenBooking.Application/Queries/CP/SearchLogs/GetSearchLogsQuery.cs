using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.SearchLogs
{
    /// <summary>
    /// استعلام لجلب سجلات البحث
    /// Query to get search logs with pagination and filtering
    /// </summary>
    public class GetSearchLogsQuery : IRequest<PaginatedResult<SearchLogDto>>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? UserId { get; set; }
        public string? SearchType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
} 