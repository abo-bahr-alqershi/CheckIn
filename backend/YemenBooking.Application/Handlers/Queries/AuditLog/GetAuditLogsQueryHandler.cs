using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.AuditLog;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Entities;

namespace YemenBooking.Application.Handlers.Queries.AuditLog
{
    /// <summary>
    /// معالج استعلام الحصول على سجلات التدقيق مع فلترة حسب المستخدم أو الفترة الزمنية
    /// Handler for GetAuditLogsQuery
    /// </summary>
    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PaginatedResult<AuditLogDto>>
    {
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAuditLogsQueryHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IBookingRepository _bookingRepository;

        public GetAuditLogsQueryHandler(
            IAuditService auditService,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IPropertyRepository propertyRepository,
            IUnitRepository unitRepository,
            IAmenityRepository amenityRepository,
            IBookingRepository bookingRepository,
            ILogger<GetAuditLogsQueryHandler> logger)
        {
            _auditService = auditService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
            _unitRepository = unitRepository;
            _amenityRepository = amenityRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing GetAuditLogsQuery. UserId: {UserId}, From: {From}, To: {To}, SearchTerm: {SearchTerm}, OperationType: {OperationType}, PageNumber: {PageNumber}, PageSize: {PageSize}", request.UserId, request.From, request.To, request.SearchTerm, request.OperationType, request.PageNumber, request.PageSize);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new BusinessRuleException("Unauthorized", "يجب تسجيل الدخول لعرض سجلات التدقيق");

            if (!await _currentUserService.IsInRoleAsync("Admin"))
                throw new BusinessRuleException("Forbidden", "ليس لديك صلاحية لعرض سجلات التدقيق");

            // جلب سجلات التدقيق
            // جلب جميع السجلات الأساسية (بإلغاء الترقيم في الخدمة)
            var allLogs = await _auditService.GetAuditTrailAsync(
                entityType: request.EntityType,
                entityId: request.RecordId,
                performedBy: request.UserId,
                fromDate: request.From,
                toDate: request.To,
                page: 1,
                pageSize: int.MaxValue,
                cancellationToken: cancellationToken);
            // تطبيق فلاتر البحث والنوع
            var filteredLogs = allLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filteredLogs = filteredLogs.Where(a =>
                    a.EntityType.Contains(request.SearchTerm) ||
                    (a.Notes != null && a.Notes.Contains(request.SearchTerm)) ||
                    (a.OldValues != null && a.OldValues.Contains(request.SearchTerm)) ||
                    (a.NewValues != null && a.NewValues.Contains(request.SearchTerm)));
            }
            if (!string.IsNullOrWhiteSpace(request.OperationType)
                && Enum.TryParse<AuditAction>(request.OperationType, true, out var actionEnum))
            {
                filteredLogs = filteredLogs.Where(a => a.Action == actionEnum);
            }
            var totalCount = filteredLogs.Count();
            // تطبيق الترقيم
            var pageLogs = filteredLogs
                .OrderByDescending(a => a.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            // التحويل إلى DTO
            var dtos = new List<AuditLogDto>();
            foreach (var log in pageLogs)
            {
                var recordName = await GetRecordNameAsync(log.EntityType, log.EntityId ?? Guid.Empty, cancellationToken);
                dtos.Add(new AuditLogDto
                {
                    Id = log.Id,
                    TableName = log.EntityType,
                    Action = log.Action.GetType()
                        .GetMember(log.Action.ToString())
                        .FirstOrDefault()?
                        .GetCustomAttribute<DisplayAttribute>()?.Name
                        ?? log.Action.ToString(),
                    RecordId = log.EntityId ?? Guid.Empty,
                    RecordName = recordName,
                    UserId = log.PerformedBy ?? Guid.Empty,
                    Username = log.Username ?? string.Empty,
                    Notes = log.Notes ?? string.Empty,
                    OldValues = log.GetOldValues(),
                    NewValues = log.GetNewValues(),
                    Metadata = log.GetMetadata(),
                    IsSlowOperation = log.IsSlowOperation,
                    Changes = log.Notes ?? string.Empty,
                    Timestamp = log.CreatedAt
                });
            }
            return new PaginatedResult<AuditLogDto>(dtos, request.PageNumber, request.PageSize, totalCount);
        }

        /// <summary>
        /// Retrieves display name for given entity type and id.
        /// </summary>
        // Helper to load entity name
        private async Task<string> GetRecordNameAsync(string entityType, Guid recordId, CancellationToken cancellationToken)
        {
            switch (entityType)
            {
                case "User":
                    var user = await _userRepository.GetUserByIdAsync(recordId, cancellationToken);
                    return user?.Name ?? recordId.ToString();
                case "Property":
                    var property = await _propertyRepository.GetPropertyByIdAsync(recordId, cancellationToken);
                    return property?.Name ?? recordId.ToString();
                case "Unit":
                    var unit = await _unitRepository.GetUnitByIdAsync(recordId, cancellationToken);
                    return unit?.Name ?? recordId.ToString();
                case "Amenity":
                    var amenity = await _amenityRepository.GetAmenityByIdAsync(recordId, cancellationToken);
                    return amenity?.Name ?? recordId.ToString();
                case "Booking":
                    var booking = await _bookingRepository.GetBookingByIdAsync(recordId, cancellationToken);
                    return booking != null ? booking.Id.ToString().Substring(0, 8) : recordId.ToString();
                default:
                    return recordId.ToString();
            }
        }
    }
} 