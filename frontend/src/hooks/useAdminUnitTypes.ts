import { useQuery } from '@tanstack/react-query';
import { AdminUnitTypesService } from '../services/admin-unit-types.service';
import type { UnitTypeDto } from '../types/unit-type.types';
import type { PaginatedResult } from '../types/common.types';

/**
 * هوك لجلب أنواع الوحدات للإدارة
 * يعزل استعلام أنواع الوحدات عن الصفحة ويعالج حالة التحميل والأخطاء
 * @param params معايير الاستعلام (صفحات، فرز)
 * @returns بيانات أنواع الوحدات وحالة التحميل والخطأ
 */
export const useAdminUnitTypes = (params?: Record<string, any>) => {
  return useQuery<PaginatedResult<UnitTypeDto>, Error>({
    queryKey: ['admin-unit-types', params] as const,
    queryFn: () => AdminUnitTypesService.getAll(params),
  });
}; 