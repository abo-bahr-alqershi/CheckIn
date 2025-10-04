import { useQuery } from '@tanstack/react-query';
import { AdminAuditLogsService } from '../services/admin-audit-logs.service';
import type { AuditLogsQuery } from '../types/audit-log.types';

/**
 * هوك لجلب سجلات التدقيق العامة
 */
export const useAuditLogs = (query: AuditLogsQuery) => {
  return useQuery({
    queryKey: ['admin', 'audit-logs', query],
    queryFn: () => AdminAuditLogsService.getAuditLogs(query),
    placeholderData: (previousData) => previousData,
  });
};