import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminPropertyServicesService } from '../services/admin-property-services.service';
import type {
  GetPropertyServicesQuery,
  GetServiceByIdQuery,
  GetServicesByTypeQuery,
  CreatePropertyServiceCommand,
  UpdatePropertyServiceCommand,
} from '../types/service.types';

/**
 * هوك لجلب خدمات كيان معين
 */
export const usePropertyServices = (query: GetPropertyServicesQuery) => {
  return useQuery({
    queryKey: ['admin', 'property-services', 'by-property', query.propertyId],
    queryFn: () => AdminPropertyServicesService.getByProperty(query),
    enabled: !!query.propertyId,
  });
};

/**
 * هوك لجلب تفاصيل خدمة معينة
 */
export const useServiceDetails = (query: GetServiceByIdQuery) => {
  return useQuery({
    queryKey: ['admin', 'property-services', 'details', query.serviceId],
    queryFn: () => AdminPropertyServicesService.getById(query),
    enabled: !!query.serviceId,
  });
};

/**
 * هوك لجلب الخدمات حسب النوع
 */
export const useServicesByType = (query: GetServicesByTypeQuery) => {
  return useQuery({
    queryKey: ['admin', 'property-services', 'by-type', query],
    queryFn: () => AdminPropertyServicesService.getByType(query),
    enabled: !!query.serviceType,
    placeholderData: (previousData) => previousData,
  });
};

/**
 * هوك لإنشاء خدمة جديدة
 */
export const useCreatePropertyService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreatePropertyServiceCommand) =>
      AdminPropertyServicesService.create(data),
    onSuccess: (result, variables) => {
      // إعادة تحديث قائمة الخدمات للكيان
      queryClient.invalidateQueries({
        queryKey: ['admin', 'property-services', 'by-property', variables.propertyId],
      });
      // إعادة تحديث قائمة الخدمات حسب النوع إذا كانت موجودة
      queryClient.invalidateQueries({
        queryKey: ['admin', 'property-services', 'by-type'],
      });
    },
  });
};

/**
 * هوك لتحديث خدمة
 */
export const useUpdatePropertyService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ serviceId, data }: { serviceId: string; data: UpdatePropertyServiceCommand }) =>
      AdminPropertyServicesService.update(serviceId, data),
    onSuccess: (result, variables) => {
      // إعادة تحديث تفاصيل الخدمة
      queryClient.invalidateQueries({
        queryKey: ['admin', 'property-services', 'details', variables.serviceId],
      });
      // إعادة تحديث جميع قوائم الخدمات
      queryClient.invalidateQueries({
        queryKey: ['admin', 'property-services'],
      });
    },
  });
};

/**
 * هوك لحذف خدمة
 */
export const useDeletePropertyService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (serviceId: string) =>
      AdminPropertyServicesService.delete(serviceId),
    onSuccess: () => {
      // إعادة تحديث جميع قوائم الخدمات
      queryClient.invalidateQueries({
        queryKey: ['admin', 'property-services'],
      });
    },
  });
};