using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Http;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة المراجعة والتدقيق
    /// Audit service implementation
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;
        private readonly YemenBookingDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(ILogger<AuditService> logger, YemenBookingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private void EnrichWithRequestContext(AuditLog log)
        {
            var http = _httpContextAccessor?.HttpContext;
            if (http is null) return;
            log.Username = http.User?.Identity?.Name ?? log.Username;
            log.IpAddress = http.Connection?.RemoteIpAddress?.ToString() ?? log.IpAddress;
            if (http.Request?.Headers.TryGetValue("User-Agent", out var ua) == true)
            {
                log.UserAgent = ua.ToString();
            }
        }


        /// <summary>
        /// اسم مستعار لـ LogAuditAsync لدعم استخدام LogAsync في المعالجات.
        /// </summary>
        public Task<bool> LogAsync(string entityType,
            string entityId,
            string notes,
            Guid performedBy,
            CancellationToken cancellationToken = default)
            => LogAuditAsync(entityType, Guid.Parse(entityId), AuditAction.UPDATE, null, notes, performedBy, null, cancellationToken);

        /// <summary>
        /// سجل النشاط مع دعم القيم القديمة والجديدة
        /// </summary>
        public Task<bool> LogActivityAsync(string entityType,
            string entityId,
            string action,
            string notes,
            object? oldValues,
            object? newValues,
            CancellationToken cancellationToken = default)
        {
            var auditAction = action.ToUpper() switch
            {
                "CREATE" => AuditAction.CREATE,
                "UPDATE" => AuditAction.UPDATE,
                "DELETE" => AuditAction.DELETE,
                "SOFT_DELETE" => AuditAction.SOFT_DELETE,
                "APPROVE" => AuditAction.APPROVE,
                "REJECT" => AuditAction.REJECT,
                _ => AuditAction.UPDATE
            };

            var oldJson = oldValues is null ? null : JsonSerializer.Serialize(oldValues);
            var newJson = newValues is null ? null : JsonSerializer.Serialize(newValues);

            return LogAuditAsync(
                entityType,
                Guid.Parse(entityId),
                auditAction,
                oldJson,
                newJson,
                null,
                notes,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> LogAuditAsync(string entityType, Guid entityId, AuditAction action, string? oldValues = null, string? newValues = null, Guid? performedBy = null, string? notes = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("تسجيل عملية تدقيق: {EntityType} ({EntityId}), Action: {Action}", entityType, entityId, action);
            try
            {
                var log = new AuditLog
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Action = action,
                    OldValues = oldValues,
                    NewValues = newValues,
                    PerformedBy = performedBy,
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow
                };
                EnrichWithRequestContext(log);
                await _dbContext.AuditLogs.AddAsync(log, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تسجيل التدقيق");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> LogEntityChangeAsync<T>(T entity, AuditAction action, T? previousState = default, Guid? performedBy = null, string? notes = null, CancellationToken cancellationToken = default) where T : BaseEntity<Guid>
        {
            _logger.LogInformation("تسجيل تغيير في الكيان من النوع: {EntityType}, Action: {Action}", typeof(T).Name, action);
            var oldJson = previousState is not null ? JsonSerializer.Serialize(previousState) : null;
            var newJson = JsonSerializer.Serialize(entity);
            return await LogAuditAsync(typeof(T).Name, entity.Id, action, oldJson, newJson, performedBy, notes, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> LogLoginAttemptAsync(string username, bool isSuccessful, string ipAddress, string userAgent, string? failureReason = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("تسجيل محاولة دخول للمستخدم: {Username}, ناجحة: {IsSuccessful}", username, isSuccessful);
            try
            {
                var log = new AuditLog
                {
                    EntityType = "User",
                    Action = AuditAction.LOGIN,
                    Username = username,
                    IsSuccessful = isSuccessful,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    ErrorMessage = failureReason,
                    CreatedAt = DateTime.UtcNow
                };
                EnrichWithRequestContext(log);
                await _dbContext.AuditLogs.AddAsync(log, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تسجيل محاولة الدخول");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> LogBusinessOperationAsync(string operationType, string operationDescription, Guid? entityId = null, string? entityType = null, Guid? performedBy = null, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("تسجيل عملية تجارية: {OperationType}، Description: {Description}", operationType, operationDescription);
            try
            {
                var log = new AuditLog
                {
                    EntityType = entityType ?? string.Empty,
                    EntityId = entityId,
                    // Map operation types to concrete actions where possible
                    Action = operationType.ToUpper() switch
                    {
                        "CREATE" or "CREATEPROPERTY" or "CREATEUNIT" or "CREATEUSER" => AuditAction.CREATE,
                        "UPDATE" or "UPDATEPROPERTY" or "UPDATEUNIT" or "UPDATEUSER" => AuditAction.UPDATE,
                        "DELETE" or "DELETEPROPERTY" or "DELETEUNIT" => AuditAction.DELETE,
                        "APPROVE" or "APPROVEPROPERTY" => AuditAction.APPROVE,
                        "REJECT" or "REJECTPROPERTY" => AuditAction.REJECT,
                        _ => AuditAction.VIEW
                    },
                    Notes = operationDescription,
                    PerformedBy = performedBy,
                    Metadata = metadata is not null ? JsonSerializer.Serialize(metadata) : null,
                    CreatedAt = DateTime.UtcNow
                };
                EnrichWithRequestContext(log);
                await _dbContext.AuditLogs.AddAsync(log, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تسجيل العملية التجارية");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AuditLog>> GetAuditTrailAsync(string? entityType = null, Guid? entityId = null, Guid? performedBy = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("الحصول على سجلات التدقيق");
            var query = _dbContext.AuditLogs.AsQueryable();
            if (!string.IsNullOrEmpty(entityType)) query = query.Where(a => a.EntityType == entityType);
            if (entityId.HasValue) query = query.Where(a => a.EntityId == entityId);
            if (performedBy.HasValue) query = query.Where(a => a.PerformedBy == performedBy);
            if (fromDate.HasValue) query = query.Where(a => a.CreatedAt >= fromDate);
            if (toDate.HasValue) query = query.Where(a => a.CreatedAt <= toDate);
            return await query.OrderByDescending(a => a.CreatedAt)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AuditLog?> GetAuditLogAsync(Guid auditLogId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("الحصول على سجل تدقيق محدد: {AuditLogId}", auditLogId);
            return await _dbContext.AuditLogs.FindAsync(new object[]{auditLogId}, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AuditLog>> SearchAuditLogsAsync(string searchTerm, AuditAction? action = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("البحث في سجلات التدقيق باستخدام: {SearchTerm}", searchTerm);
            var query = _dbContext.AuditLogs.AsQueryable();
            if (action.HasValue) query = query.Where(a => a.Action == action.Value);
            if (fromDate.HasValue) query = query.Where(a => a.CreatedAt >= fromDate);
            if (toDate.HasValue) query = query.Where(a => a.CreatedAt <= toDate);
            query = query.Where(a => a.EntityType.Contains(searchTerm) ||
                                      (a.Notes != null && a.Notes.Contains(searchTerm)) ||
                                      (a.OldValues != null && a.OldValues.Contains(searchTerm)) ||
                                      (a.NewValues != null && a.NewValues.Contains(searchTerm)));
            return await query.OrderByDescending(a => a.CreatedAt)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AuditStatistics> GetAuditStatisticsAsync(DateTime fromDate, DateTime toDate, string? entityType = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("الحصول على إحصائيات التدقيق من {FromDate} إلى {ToDate}", fromDate, toDate);
            var query = _dbContext.AuditLogs.Where(a => a.CreatedAt >= fromDate && a.CreatedAt <= toDate);
            if (!string.IsNullOrEmpty(entityType)) query = query.Where(a => a.EntityType == entityType);
            var total = await query.CountAsync(cancellationToken);
            var byAction = await query.GroupBy(a => a.Action).Select(g => new { Action = g.Key, Count = g.Count() }).ToListAsync(cancellationToken);
            var byEntity = await query.GroupBy(a => a.EntityType).Select(g => new { EntityType = g.Key, Count = g.Count() }).ToListAsync(cancellationToken);
            var byUser = await query.Where(a => a.PerformedBy.HasValue).GroupBy(a => a.PerformedBy).Select(g => new { User = g.Key.Value, Count = g.Count() }).ToListAsync(cancellationToken);
            var failed = await query.CountAsync(a => !a.IsSuccessful, cancellationToken);
            var uniqueUsers = await query.Where(a => a.PerformedBy.HasValue).Select(a => a.PerformedBy).Distinct().CountAsync(cancellationToken);
            return new AuditStatistics
            {
                TotalOperations = total,
                OperationsByAction = byAction.ToDictionary(x => x.Action, x => x.Count),
                OperationsByEntityType = byEntity.ToDictionary(x => x.EntityType, x => x.Count),
                OperationsByUser = byUser.ToDictionary(x => x.User.ToString(), x => x.Count),
                FromDate = fromDate,
                ToDate = toDate,
                UniqueUsers = uniqueUsers,
                FailedOperations = failed
            };
        }

        /// <inheritdoc />
        public async Task<int> ArchiveOldLogsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("أرشفة سجلات التدقيق الأقدم من: {OlderThan}", olderThan);
            var oldLogs = _dbContext.AuditLogs.Where(a => a.CreatedAt < olderThan);
            var count = await oldLogs.CountAsync(cancellationToken);
            _dbContext.AuditLogs.RemoveRange(oldLogs);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return count;
        }

        /// <inheritdoc />
        public async Task<byte[]> ExportAuditLogsAsync(DateTime fromDate, DateTime toDate, string format = "CSV", string? entityType = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("تصدير سجلات التدقيق من {FromDate} إلى {ToDate} بصيغة {Format}", fromDate, toDate, format);
            var logs = await GetAuditTrailAsync(entityType, null, null, fromDate, toDate, 1, int.MaxValue, cancellationToken);
            if (format.Equals("JSON", StringComparison.OrdinalIgnoreCase))
            {
                var json = JsonSerializer.Serialize(logs);
                return Encoding.UTF8.GetBytes(json);
            }
            var sb = new StringBuilder();
            sb.AppendLine("Id,EntityType,EntityId,Action,CreatedAt,PerformedBy,IsSuccessful");
            foreach (var log in logs)
            {
                sb.AppendLine($"{log.Id},{log.EntityType},{log.EntityId},{log.Action},{log.CreatedAt:o},{log.PerformedBy},{log.IsSuccessful}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
} 