import { apiClient } from './api.service';
import type {
  UnitDto,
  UnitDetailsDto,
  CreateUnitCommand,
  UpdateUnitCommand,
  DeleteUnitCommand,
  GetUnitByIdQuery,
} from '../types/unit.types';
import type { ResultDto, PaginatedResult } from '../types/common.types';

// المسار الأساسي لتعاملات الوحدات للمدراء
const API_BASE = '/api/admin/Units';

/**
 * جميع خدمات الوحدات للإدارة
 * موثقة بالعربي لضمان الوضوح والتوافق مع الباك اند
 */
export const AdminUnitsService = {
  /** جلب جميع الوحدات مع صفحات وفلاتر */
  getAll: (params?: Record<string, any>) =>
    apiClient.get<PaginatedResult<UnitDto>>(API_BASE, { params }).then(res => res.data),

  // جلب تفاصيل وحدة بواسطة المعرف
  getById: (query: GetUnitByIdQuery) =>
    apiClient.get<ResultDto<UnitDetailsDto>>(`${API_BASE}/${query.unitId}`).then(res => res.data),

  // إنشاء وحدة جديدة
  create: (data: CreateUnitCommand) =>
    apiClient.post<ResultDto<string>>(API_BASE, data).then(res => res.data),

  // تحديث بيانات وحدة
  update: (unitId: string, data: UpdateUnitCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${unitId}`, data).then(res => res.data),

  // حذف وحدة
  delete: (unitId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${unitId}`).then(res => res.data),
  /** جلب تفاصيل الوحدة مع الحقول الديناميكية */
  getDetails: (unitId: string, includeDynamicFields = true) =>
    apiClient.get<ResultDto<UnitDetailsDto>>(`${API_BASE}/${unitId}/details`, { params: { includeDynamicFields } }).then(res => res.data),

  /** إضافة الوحدة لأقسام متعددة */
  addUnitToSections: (unitId: string, sectionIds: string[]) =>
    apiClient.post(`/api/admin/units/${unitId}/sections`, { sectionIds }).then(res => res.data),
};
