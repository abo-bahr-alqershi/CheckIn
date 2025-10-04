import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminUnitTypeFieldsService } from '../services/admin-unit-type-fields.service';
import type {
  UnitTypeFieldDto,
  CreateUnitTypeFieldCommand,
  UpdateUnitTypeFieldCommand,
  GetUnitTypeFieldsQuery
} from '../types/unit-type-field.types';
import type { ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وعمليات الحقول الديناميكية المرتبطة بنوع وحدة (CRUD) للوحة الإدارة
 * يعزل التعامل مع react-query والخدمات في مكان واحد
 * @param params معايير استعلام الحقول الديناميكية (معرف نوع الوحدة)
 * @returns بيانات مصفوفة من UnitTypeFieldDto، حالات التحميل والأخطاء، ودوال الإنشاء والتحديث والحذف
 */
export const useAdminUnitTypeFieldsByUnitType = (params: GetUnitTypeFieldsQuery) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-unit-type-fields-by-unit', params] as const;

  // جلب الحقول حسب نوع الوحدة
  const { data: unitTypeFieldsData, isLoading, error } = useQuery<UnitTypeFieldDto[], Error>({
    queryKey,
    queryFn: () => AdminUnitTypeFieldsService.getByUnitType(params),
    enabled: Boolean(params.unitTypeId),
  });

  // إنشاء حقل
  const createUnitTypeField = useMutation<ResultDto<string>, Error, CreateUnitTypeFieldCommand>({
    mutationFn: (data) => AdminUnitTypeFieldsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  // تحديث حقل
  const updateUnitTypeField = useMutation<ResultDto<boolean>, Error, { fieldId: string; data: UpdateUnitTypeFieldCommand }>({
    mutationFn: ({ fieldId, data }) => AdminUnitTypeFieldsService.update(fieldId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  // حذف حقل
  const deleteUnitTypeField = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (fieldId) => AdminUnitTypeFieldsService.delete(fieldId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  return { unitTypeFieldsData, isLoading, error, createUnitTypeField, updateUnitTypeField, deleteUnitTypeField };
}; 