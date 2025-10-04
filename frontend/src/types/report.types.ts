// أنواع بيانات التقارير (Reports)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * بيانات التقرير الأساسية
 */
export interface ReportDto {
  id: string;
  reporterUserId: string;
  reporterUserName: string;
  reportedUserId?: string;
  reportedUserName?: string;
  reportedPropertyId?: string;
  reportedPropertyName?: string;
  reason: string;
  description: string;
  createdAt: string;
}

/**
 * أمر إنشاء بلاغ جديد
 */
export interface CreateReportCommand {
  reporterUserId: string;
  reportedUserId?: string;
  reportedPropertyId?: string;
  reason: string;
  description: string;
}

/**
 * أمر تحديث بلاغ
 */
export interface UpdateReportCommand {
  id: string;
  reason?: string;
  description?: string;
}

/**
 * أمر حذف بلاغ
 */
export interface DeleteReportCommand {
  id: string;
  deletionReason?: string;
}

/**
 * استعلام جلب بلاغ بواسطة المعرف
 */
export interface GetReportByIdQuery {
  id: string;
}

/**
 * استعلام جلب جميع البلاغات مع إمكانية التصفية
 */
export interface GetAllReportsQuery {
  pageNumber?: number;
  pageSize?: number;
  reporterUserId?: string;
  reportedUserId?: string;
  reportedPropertyId?: string;
  /** نص البحث */
  searchTerm?: string;
  /** سبب البلاغ */
  reason?: string;
  /** حالة البلاغ */
  status?: string;
  /** من تاريخ */
  fromDate?: string;
  /** إلى تاريخ */
  toDate?: string;
}

/**
 * استعلام جلب التقارير المبلّغ عنها لكيان
 */
export interface GetReportsByPropertyQuery {
  /** معرف الكيان */
  propertyId: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام جلب التقارير المقدمة ضد مستخدم معين
 */
export interface GetReportsByReportedUserQuery {
  /** معرف المستخدم المبلغ عنه */
  reportedUserId: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * إحصائيات وتحليلات البلاغات
 */
export interface ReportStatsDto {
  /** إجمالي عدد البلاغات */
  totalReports: number;
  /** عدد البلاغات المعلقة */
  pendingReports: number;
  /** عدد البلاغات المحلولة */
  resolvedReports: number;
  /** عدد البلاغات المرفوضة */
  dismissedReports: number;
  /** عدد البلاغات المصعّدة */
  escalatedReports: number;
  /** متوسط زمن حل البلاغ (بالأيام) */
  averageResolutionTime: number;
  /** عدد البلاغات حسب السبب */
  reportsByCategory: Record<string, number>;
  /** اتجاه البلاغات على مدى الأيام */
  reportsTrend: Array<{ date: string; count: number }>;
}
