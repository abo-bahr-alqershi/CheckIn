import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  ServiceDto,
  ServiceDetailsDto,
  GetPropertyServicesQuery,
  GetServiceByIdQuery,
  GetServicesByTypeQuery,
  CreatePropertyServiceCommand,
  UpdatePropertyServiceCommand,
  DeletePropertyServiceCommand,
} from '../types/service.types';

// المسار الأساسي لخدمة خدمات الكيان للإدمن
const API_BASE = '/api/admin/propertyservices';

/**
 * خدمات إدارة خدمات الكيانات للإدمن
 */
export const AdminPropertyServicesService = {
  /** إنشاء خدمة جديدة لكيان */
  create: (data: CreatePropertyServiceCommand) =>
    apiClient.post<ResultDto<string>>(API_BASE, data).then(res => res.data),

  /** تحديث خدمة كيان */
  update: (serviceId: string, data: UpdatePropertyServiceCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${serviceId}`, data).then(res => res.data),

  /** حذف خدمة كيان */
  delete: (serviceId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${serviceId}`).then(res => res.data),

  /** جلب خدمات كيان معين */
  getByProperty: (query: GetPropertyServicesQuery) =>
    apiClient.get<ResultDto<ServiceDto[]>>(
      `${API_BASE}/property/${query.propertyId}`
    ).then(res => res.data),

  /** جلب خدمة كيان بحسب المعرف */
  getById: (query: GetServiceByIdQuery) =>
    apiClient.get<ResultDto<ServiceDetailsDto>>(
      `${API_BASE}/${query.serviceId}`
    ).then(res => res.data),

  /** جلب الخدمات حسب النوع */
  getByType: (query: GetServicesByTypeQuery) =>
    apiClient.get<PaginatedResult<ServiceDto>>(
      `${API_BASE}/type/${query.serviceType}`,
      { params: { pageNumber: query.pageNumber, pageSize: query.pageSize } }
    ).then(res => res.data),
}; 