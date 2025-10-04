import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminUnitTypesService } from '../services/admin-unit-types.service';
import type {
  UnitTypeDto,
  CreateUnitTypeCommand,
  UpdateUnitTypeCommand,
  GetUnitTypesByPropertyTypeQuery
} from '../types/unit-type.types';
import type { PaginatedResult, ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وعمليات أنواع الوحدات المرتبطة بنوع كيان (CRUD) للوحة الإدارة
 * يعزل التعامل مع react-query والخدمات في مكان واحد
 * @param params معايير استعلام أنواع الوحدات (معرف نوع الكيان، رقم الصفحة، حجم الصفحة)
 * @returns بيانات PaginatedResult لأنواع الوحدات، حالات التحميل والأخطاء، ودوال الإنشاء والتحديث والحذف
 */
export const useAdminUnitTypesByPropertyType = (params: GetUnitTypesByPropertyTypeQuery) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-unit-types-by-property', params] as const;

  // جلب أنواع الوحدات حسب نوع الكيان
  const { data: unitTypesData, isLoading, error } = useQuery<PaginatedResult<UnitTypeDto>, Error>({
    queryKey,
    queryFn: () => AdminUnitTypesService.getByPropertyType(params),
    enabled: Boolean(params.propertyTypeId)
  });

  // إنشاء نوع وحدة
  const createUnitType = useMutation<ResultDto<string>, Error, CreateUnitTypeCommand>({
    mutationFn: (data) => AdminUnitTypesService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  // تحديث نوع وحدة
  const updateUnitType = useMutation<ResultDto<boolean>, Error, { unitTypeId: string; data: UpdateUnitTypeCommand }>({
    mutationFn: ({ unitTypeId, data }) => AdminUnitTypesService.update(unitTypeId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  // حذف نوع وحدة
  const deleteUnitType = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (unitTypeId) => AdminUnitTypesService.delete(unitTypeId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  return { unitTypesData, isLoading, error, createUnitType, updateUnitType, deleteUnitType };
}; 