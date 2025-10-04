import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  PropertyDto,
  CreatePropertyCommand,
  UpdatePropertyCommand,
  DeletePropertyCommand,
  GetAllPropertiesQuery,
  GetPropertyByIdQuery,
  GetPendingPropertiesQuery,
  GetPropertyDetailsQuery,
  GetPropertyForEditQuery,
  GetPropertyAmenitiesQuery,
  PropertyDetailsDto,
  PropertyEditDto,
  FieldGroupWithFieldsDto,
  GetPropertiesByCityQuery,
  GetPropertiesByOwnerQuery,
  GetPropertiesByTypeQuery,
  GetPropertyRatingStatsQuery,
  PropertyRatingStatsDto,
} from '../types/property.types';
import type { AmenityDto } from '../types/amenity.types';

// المسار الأساسي لتعاملات الكيانات للمدراء
const API_BASE = '/api/admin/Properties';

export const AdminPropertiesService = {
  // إنشاء كيان جديد
  create: (data: CreatePropertyCommand) =>
    apiClient.post<ResultDto<string>>(API_BASE, data).then(res => res.data),

  // تحديث بيانات كيان
  update: (propertyId: string, data: UpdatePropertyCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${propertyId}`, data).then(res => res.data),

  // حذف كيان
  delete: (propertyId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${propertyId}`).then(res => res.data),

  // جلب جميع الكيانات مع الفلاتر والصفحات
  getAll: (params?: GetAllPropertiesQuery) =>
    apiClient.get<PaginatedResult<PropertyDto>>(`${API_BASE}`, { params }).then(res => res.data),

  /** الموافقة على كيان */
  approve: (propertyId: string) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${propertyId}/approve`).then(res => res.data),
  /** رفض كيان */
  reject: (propertyId: string) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${propertyId}/reject`).then(res => res.data),
  /** جلب الكيانات في انتظار الموافقة */
  getPending: (params?: GetPendingPropertiesQuery) =>
    apiClient.get<PaginatedResult<PropertyDto>>(`${API_BASE}/pending`, { params }).then(res => res.data),
  /** جلب تفاصيل الكيان مع الوحدات والحقول الديناميكية */
  getDetails: (query: GetPropertyDetailsQuery) =>
    apiClient.get<ResultDto<PropertyDetailsDto>>(`${API_BASE}/${query.propertyId}/details`, { params: { includeUnits: query.includeUnits } }).then(res => res.data),

  /** إضافة الكيان لأقسام متعددة */
  addPropertyToSections: (propertyId: string, sectionIds: string[]) =>
    apiClient.post(`/api/admin/properties/${propertyId}/sections`, { sectionIds }).then(res => res.data),
};
