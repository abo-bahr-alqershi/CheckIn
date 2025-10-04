import { apiClient } from './api.service';
import type {
  UnitTypeDto,
  CreateUnitTypeCommand,
  UpdateUnitTypeCommand,
  DeleteUnitTypeCommand,
  GetUnitTypeByIdQuery,
  GetUnitTypesByPropertyTypeQuery,
} from '../types/unit-type.types';
import type { PaginatedResult, ResultDto } from '../types/common.types';

// المسار الأساسي لتعاملات أنواع الوحدات للمدراء
const API_BASE = '/api/admin/unit-types';

/**
 * جميع خدمات أنواع الوحدات للإدارة
 * موثقة بالعربي لضمان الوضوح والتوافق مع الباك اند
 */
export const AdminUnitTypesService = {
  /** جلب جميع أنواع الوحدات مع صفحات وفلاتر */
  getAll: (params?: Record<string, any>) =>
    apiClient.get<PaginatedResult<UnitTypeDto>>(`${API_BASE}`, { params }).then(res => res.data),

  // إنشاء نوع وحدة جديد
  create: (data: CreateUnitTypeCommand) =>
    apiClient.post<ResultDto<string>>(`${API_BASE}`, data).then(res => res.data),

  // تحديث نوع وحدة
  update: (unitTypeId: string, data: UpdateUnitTypeCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${unitTypeId}`, data).then(res => res.data),

  // حذف نوع وحدة
  delete: (unitTypeId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${unitTypeId}`).then(res => res.data),
  /** جلب أنواع الوحدات حسب نوع الكيان مع صفحات */
  getByPropertyType: (params: GetUnitTypesByPropertyTypeQuery) => {
    const { propertyTypeId, pageNumber, pageSize } = params;
    return apiClient
      .get<PaginatedResult<UnitTypeDto>>(
        `${API_BASE}/property-type/${propertyTypeId}`,
        { params: { pageNumber, pageSize } }
      )
      .then(res => res.data);
  },
};
