import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminAmenitiesService } from '../services/admin-amenities.service';
import type { AmenityDto, CreateAmenityCommand, UpdateAmenityCommand, AssignAmenityToPropertyCommand, GetAllAmenitiesQuery } from '../types/amenity.types';
import type { PaginatedResult, ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وعمليات المرافق للوحة الإدارة
 * يعزل التعامل مع react-query وخدمات المرافق في مكان واحد
 * @param params معايير الاستعلام (صفحات، بحث)
 * @returns بيانات المرافق، حالات التحميل والأخطاء، ودوال الإنشاء والتحديث والحذف والربط
 */
export const useAdminAmenities = (params: GetAllAmenitiesQuery) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-amenities', params] as const;

  // جلب جميع المرافق مع صفحات
  const { data: amenitiesData, isLoading, error } = useQuery<PaginatedResult<AmenityDto>, Error>({
    queryKey,
    queryFn: () =>
      AdminAmenitiesService.getAllAmenities(params).then(res => res.data),
  });

  // إنشاء مرفق جديد
  const createAmenity = useMutation<ResultDto<string>, Error, CreateAmenityCommand>({
    mutationFn: (data) => AdminAmenitiesService.createAmenity(data).then(res => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-amenities'] });
    },
  });

  // تحديث مرفق
  const updateAmenity = useMutation<ResultDto<boolean>, Error, { amenityId: string; data: UpdateAmenityCommand }>({
    mutationFn: ({ amenityId, data }) =>
      AdminAmenitiesService.updateAmenity(amenityId, data).then(res => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-amenities'] });
    },
  });

  // حذف مرفق
  const deleteAmenity = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (amenityId) => AdminAmenitiesService.deleteAmenity(amenityId).then(res => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-amenities'] });
    },
  });

  // ربط المرفق بكيان
  const assignAmenityToProperty = useMutation<ResultDto<boolean>, Error, { amenityId: string; propertyId: string; data: AssignAmenityToPropertyCommand }>({
    mutationFn: ({ amenityId, propertyId, data }) =>
      AdminAmenitiesService.assignAmenityToProperty(amenityId, propertyId, data).then(res => res.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-amenities'] });
    },
  });

  return { amenitiesData, isLoading, error, createAmenity, updateAmenity, deleteAmenity, assignAmenityToProperty };
}; 