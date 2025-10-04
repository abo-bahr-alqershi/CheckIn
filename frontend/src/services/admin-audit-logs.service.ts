import { apiClient } from './api.service';
import type { PaginatedResult } from '../types/common.types';
import type { AuditLogDto, GetCustomerActivityLogsQuery, GetPropertyActivityLogsQuery, GetAdminActivityLogsQuery, AuditLogsQuery } from '../types/audit-log.types';

// المسار الأساسي لنقاط نهاية سجلات التدقيق للإدارة
const API_BASE = '/api/admin/auditlogs';

/**
 * خدمات سجلات التدقيق للإدارة
 */
export const AdminAuditLogsService = {
  /** جلب سجلات التدقيق العامة */
  getAuditLogs: (query: AuditLogsQuery) =>
    apiClient.get<PaginatedResult<AuditLogDto>>(`${API_BASE}/audit-logs`, { params: query }).then(res => res.data),
}; 