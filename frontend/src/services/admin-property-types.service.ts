import { apiClient } from './api.service';
import type {
  PropertyTypeDto,
  CreatePropertyTypeCommand,
  UpdatePropertyTypeCommand,
  GetAllPropertyTypesQuery,
} from '../types/property-type.types';
import type { ResultDto, PaginatedResult } from '../types/common.types';

// المسار الأساسي لتعاملات أنواع الكيانات للمدراء
const API_BASE = '/api/admin/property-types';

export const AdminPropertyTypesService = {
  // إنشاء نوع كيان جديد
  create: (data: CreatePropertyTypeCommand) =>
    apiClient.post<ResultDto<string>>(API_BASE, data).then(res => res.data),

  // تحديث بيانات نوع كيان
  update: (propertyTypeId: string, data: UpdatePropertyTypeCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${propertyTypeId}`, data).then(res => res.data),

  // حذف نوع كيان
  delete: (propertyTypeId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${propertyTypeId}`).then(res => res.data),

  // جلب جميع أنواع الكيانات مع الفلاتر والصفحات
  getAll: (params?: GetAllPropertyTypesQuery) =>
    apiClient.get<PaginatedResult<PropertyTypeDto>>(`${API_BASE}`, { params }).then(res => res.data),
};