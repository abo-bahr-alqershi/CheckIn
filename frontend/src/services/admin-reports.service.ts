// خدمات البلاغات (Reports Service)
// جميع الدوال موثقة بالعربي وتدعم العمليات الأساسية
import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  ReportDto,
  CreateReportCommand,
  UpdateReportCommand,
  DeleteReportCommand,
  GetReportByIdQuery,
  GetAllReportsQuery,
  GetReportsByPropertyQuery,
  GetReportsByReportedUserQuery,
  ReportStatsDto,
} from '../types/report.types';

/**
 * دوال التعامل مع البلاغات عبر API
 */
export class AdminReportsService {
  // المسار الأساسي لتعاملات البلاغات للمدراء
  private static readonly API_BASE = '/api/admin/Reports';

  /** جلب جميع البلاغات مع الفلاتر والصفحات */
  static async getAll(query?: GetAllReportsQuery): Promise<PaginatedResult<ReportDto>> {
    const response = await apiClient.get<PaginatedResult<ReportDto>>(this.API_BASE, { params: query });
    return response.data;
  }

  /** جلب تقرير حسب المعرف */
  static async getById(query: GetReportByIdQuery): Promise<ResultDto<ReportDto>> {
    const response = await apiClient.get<ResultDto<ReportDto>>(`${this.API_BASE}/${query.id}`);
    return response.data;
  }

  /** إنشاء تقرير جديد */
  static async create(data: CreateReportCommand): Promise<ResultDto<string>> {
    const response = await apiClient.post<ResultDto<string>>(this.API_BASE, data);
    return response.data;
  }

  /** تحديث تقرير */
  static async update(reportId: string, data: UpdateReportCommand): Promise<ResultDto<boolean>> {
    const response = await apiClient.put<ResultDto<boolean>>(`${this.API_BASE}/${reportId}`, data);
    return response.data;
  }

  /** حذف تقرير */
  static async delete(id: string, deletionReason?: string): Promise<ResultDto<boolean>> {
    const response = await apiClient.delete<ResultDto<boolean>>(`${this.API_BASE}/${id}`, { data: deletionReason ? { deletionReason } : undefined });
    return response.data;
  }

  /** جلب البلاغات حسب الكيان */
  static async getByProperty(propertyId: string, query?: any): Promise<ReportDto[]> {
    const response = await apiClient.get<PaginatedResult<ReportDto>>(`${this.API_BASE}/property/${propertyId}`, { params: query });
    return response.data.items;
  }

  /** جلب البلاغات حسب المستخدم المبلغ عنه */
  static async getByReportedUser(reportedUserId: string, query?: any): Promise<ReportDto[]> {
    const response = await apiClient.get<PaginatedResult<ReportDto>>(`${this.API_BASE}/reported-user/${reportedUserId}`, { params: query });
    return response.data.items;
  }

  /** اتخاذ إجراء على البلاغ */
  static async action(data: { id: string; action: string; actionNote?: string; adminId: string }): Promise<ResultDto<boolean>> {
    const response = await apiClient.post<ResultDto<boolean>>(`${this.API_BASE}/${data.id}/action`, data);
    return response.data;
  }
  /** جلب إحصائيات البلاغات */
  static async stats(): Promise<ReportStatsDto> {
    const response = await apiClient.get<ReportStatsDto>(`${this.API_BASE}/stats`);
    return response.data;
  }
}
