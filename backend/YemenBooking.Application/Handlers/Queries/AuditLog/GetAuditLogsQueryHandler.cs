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

            // استعلام مرقم وخفيف على مستوى قاعدة البيانات
            AuditAction? parsedAction = null;
            if (!string.IsNullOrWhiteSpace(request.OperationType) && Enum.TryParse<AuditAction>(request.OperationType, true, out var actionEnum))
            {
                parsedAction = actionEnum;
            }

            // استخدام الخدمة مع إسقاط الحقول الثقيلة، واحتساب العدد الكلي من DB
            var (pageLogs, totalCount) = await _auditService.SearchAuditLogsPagedAsync(
                searchTerm: request.SearchTerm,
                action: parsedAction,
                fromDate: request.From,
                toDate: request.To,
                entityType: request.EntityType,
                entityId: request.RecordId,
                performedBy: request.UserId,
                page: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

            // التحويل إلى DTO
            var dtos = new List<AuditLogDto>();
            foreach (var log in pageLogs)
            {
                var shortId = (log.EntityId ?? Guid.Empty).ToString();
                var shortUid = shortId.Length >= 8 ? shortId.Substring(0, 8) : shortId;
                var recordName = string.Equals(log.EntityType, "User", StringComparison.OrdinalIgnoreCase)
                    ? (string.IsNullOrWhiteSpace(log.Username) ? shortUid : log.Username!)
                    : shortUid;
                dtos.Add(new AuditLogDto
                {
                    Id = log.Id,
                    TableName = log.EntityType,
                    Action = log.Action.ToString(),
                    RecordId = log.EntityId ?? Guid.Empty,
                    RecordName = recordName,
                    UserId = log.PerformedBy ?? Guid.Empty,
                    Username = log.Username ?? string.Empty,
                    Notes = log.Notes ?? string.Empty,
                    // old/new/metadata are heavy; fetch in details endpoint when needed
                    OldValues = null,
                    NewValues = null,
                    Metadata = null,
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