import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminPropertiesService } from '../services/admin-properties.service';
import type {
  PropertyDto,
  CreatePropertyCommand,
  UpdatePropertyCommand,
  GetAllPropertiesQuery
} from '../types/property.types';
import type { PaginatedResult, ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وعمليات الكيانات في لوحة الإدارة
 * يعزل التعامل مع react-query وخدمات الكيانات في مكان واحد
 * @param params معايير استعلام الكيانات (صفحات، فرز، فلاتر)
 * @returns بيانات الكيانات، الكيانات المعلقة للموافقة، حالات التحميل والأخطاء، ودوال الإنشاء والتحديث والموافقة والرفض والحذف
 */
export const useAdminProperties = (params: GetAllPropertiesQuery) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-properties', params] as const;
  const pendingQueryKey = ['admin-properties-pending'] as const;

  // جلب جميع الكيانات مع الفلاتر والصفحات
  const { data: propertiesData, isLoading, error } = useQuery<PaginatedResult<PropertyDto>, Error>({
    queryKey,
    queryFn: () => AdminPropertiesService.getAll(params),
  });

  // جلب الكيانات المعلقة في انتظار الموافقة
  const { data: pendingPropertiesData } = useQuery<PaginatedResult<PropertyDto>, Error>({
    queryKey: pendingQueryKey,
    queryFn: () => AdminPropertiesService.getPending({ pageNumber: 1, pageSize: 50 }),
  });

  // إنشاء كيان جديد
  const createProperty = useMutation<ResultDto<string>, Error, CreatePropertyCommand>({
    mutationFn: (data) => AdminPropertiesService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: pendingQueryKey });
    },
  });

  // تحديث بيانات كيان
  const updateProperty = useMutation<ResultDto<boolean>, Error, { propertyId: string; data: UpdatePropertyCommand }>({
    mutationFn: ({ propertyId, data }) => AdminPropertiesService.update(propertyId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: pendingQueryKey });
    },
  });

  // الموافقة على كيان
  const approveProperty = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (propertyId) => AdminPropertiesService.approve(propertyId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: pendingQueryKey });
    },
  });

  // رفض كيان
  const rejectProperty = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (propertyId) => AdminPropertiesService.reject(propertyId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: pendingQueryKey });
    },
  });

  // حذف كيان
  const deleteProperty = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (propertyId) => AdminPropertiesService.delete(propertyId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: pendingQueryKey });
    },
  });

  return {
    propertiesData,
    pendingPropertiesData,
    isLoading,
    error,
    createProperty,
    updateProperty,
    approveProperty,
    rejectProperty,
    deleteProperty,
  };
}; 