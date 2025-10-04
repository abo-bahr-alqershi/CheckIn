import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminUnitsService } from '../services/admin-units.service';
import type { UnitDto, CreateUnitCommand, UpdateUnitCommand } from '../types/unit.types';
import type { PaginatedResult, ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وعمليات الوحدات للوحة الإدارة
 * يعزل التعامل مع react-query والخدمات في مكان واحد
 * @param params معايير الاستعلام (صفحات، فرز، فلاتر)
 * @returns بيانات الوحدات وحالات التحميل والأخطاء ودوال الإنشاء والتحديث والحذف
 */
export const useAdminUnits = (params: Record<string, any>) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-units', params] as const;

  // جلب جميع الوحدات مع الفلاتر والصفحات
  const { data: unitsData, isLoading, error } = useQuery<PaginatedResult<UnitDto>, Error>({
    queryKey,
    queryFn: () => AdminUnitsService.getAll(params),
  });

  // إنشاء وحدة جديدة
  const createUnit = useMutation<ResultDto<string>, Error, CreateUnitCommand>({
    mutationFn: (data) => AdminUnitsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-units'] });
    },
  });

  // تحديث بيانات وحدة
  const updateUnit = useMutation<ResultDto<boolean>, Error, { unitId: string; data: UpdateUnitCommand }>({
    mutationFn: ({ unitId, data }) => AdminUnitsService.update(unitId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-units'] });
    },
  });

  // حذف وحدة
  const deleteUnit = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (unitId) => AdminUnitsService.delete(unitId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-units'] });
    },
  });

  return { unitsData, isLoading, error, createUnit, updateUnit, deleteUnit };
}; 