import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminFieldGroupsService } from '../services/admin-field-groups.service';
import type {
  FieldGroupDto,
  CreateFieldGroupCommand,
  UpdateFieldGroupCommand,
  GetFieldGroupsByUnitTypeQuery
} from '../types/field-group.types';
import type { ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وعمليات مجموعات الحقول المرتبطة بنوع وحدة (CRUD) للوحة الإدارة
 * يعزل التعامل مع react-query والخدمات في مكان واحد
 * @param params معايير استعلام مجموعات الحقول (معرف نوع الوحدة)
 * @returns بيانات مصفوفة من FieldGroupDto، حالات التحميل والأخطاء، ودوال الإنشاء والتحديث والحذف
 */
export const useAdminFieldGroupsByUnitType = (params: GetFieldGroupsByUnitTypeQuery) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-field-groups-by-unit', params] as const;

  // جلب مجموعات الحقول حسب نوع الوحدة
  const { data: fieldGroupsData, isLoading, error } = useQuery<FieldGroupDto[], Error>({
    queryKey,
    queryFn: () => AdminFieldGroupsService.getByUnitType(params),
    enabled: Boolean(params.unitTypeId),
  });

  // إنشاء مجموعة حقول
  const createFieldGroup = useMutation<ResultDto<FieldGroupDto>, Error, CreateFieldGroupCommand>({
    mutationFn: (data) => AdminFieldGroupsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  // تحديث مجموعة حقول
  const updateFieldGroup = useMutation<ResultDto<FieldGroupDto>, Error, UpdateFieldGroupCommand>({
    mutationFn: (command) => AdminFieldGroupsService.update(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  // حذف مجموعة حقول
  const deleteFieldGroup = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (groupId) => AdminFieldGroupsService.delete({ groupId }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
    },
  });

  return { fieldGroupsData, isLoading, error, createFieldGroup, updateFieldGroup, deleteFieldGroup };
}; 