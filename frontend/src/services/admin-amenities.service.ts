import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type { GetAllAmenitiesQuery, AmenityDto, CreateAmenityCommand, UpdateAmenityCommand, AssignAmenityToPropertyCommand } from '../types/amenity.types';

const API_BASE = '/api/admin/amenities';

export const AdminAmenitiesService = {
  // إنشاء مرفق جديد
  createAmenity: (data: CreateAmenityCommand) =>
    apiClient.post<ResultDto<string>>(`${API_BASE}`, data),

  // تحديث مرفق
  updateAmenity: (amenityId: string, data: UpdateAmenityCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${amenityId}`, data),

  // حذف مرفق
  deleteAmenity: (amenityId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${amenityId}`),

  // جلب جميع المرافق مع صفحات
  getAllAmenities: (query?: GetAllAmenitiesQuery) =>
    apiClient.get<PaginatedResult<AmenityDto>>(`${API_BASE}`, { params: query }),

  // إسناد مرفق لكيان
  assignAmenityToProperty: (amenityId: string, propertyId: string, data: AssignAmenityToPropertyCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${amenityId}/assign/property/${propertyId}`, data),
};
