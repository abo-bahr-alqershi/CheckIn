/**
 * تمثيل سجل النشاط
 */
export interface AuditLogDto {
  id: string;
  tableName: string;
  action: string;
  recordId: string;
  recordName: string;
  userId: string;
  username: string;
  changes: string;
  timestamp: string;
  notes: string;
  oldValues?: Record<string, any>;
  newValues?: Record<string, any>;
  metadata?: Record<string, any>;
  isSlowOperation: boolean;
}

/**
 * معلمات استعلام سجلات التدقيق
 */
export interface AuditLogsQuery {
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
  /** تصفية حسب معرف المستخدم */
  userId?: string;
  /** تاريخ البداية (ISO) */
  from?: string;
  /** تاريخ النهاية (ISO) */
  to?: string;
  /** مصطلح البحث */
  searchTerm?: string;
  /** نوع العملية */
  operationType?: string;
}

/**
 * استعلام جلب سجلات نشاط العميل
 */
export interface GetCustomerActivityLogsQuery {
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام جلب سجلات نشاط المالك والموظفين
 */
export interface GetPropertyActivityLogsQuery {
  /** معرف الكيان (اختياري) */
  propertyId?: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

export interface GetAdminActivityLogsQuery {
    pageNumber: number;
    pageSize: number;
}