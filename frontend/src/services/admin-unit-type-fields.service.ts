import { apiClient } from './api.service';
import type {
  CreateUnitTypeFieldCommand,
  UpdateUnitTypeFieldCommand,
  DeleteUnitTypeFieldCommand,
  ToggleUnitTypeFieldStatusCommand,
  ReorderUnitTypeFieldsCommand,
  GetUnitTypeFieldsQuery,
  GetUnitTypeFieldByIdQuery,
  GetUnitTypeFieldsGroupedQuery,
  AssignFieldToGroupCommand,
  AssignFieldsToGroupCommand,
  BulkAssignFieldsToGroupsCommand,
  RemoveFieldFromGroupCommand,
  ReorderFieldsInGroupCommand,
  FieldGroupWithFieldsDto,
  GetUngroupedFieldsQuery,
  BulkAssignFieldToGroupCommand,
} from '../types/unit-type-field.types';
import type { UnitTypeFieldDto } from '../types/unit-type-field.types';
import type { ResultDto } from '../types/common.types';
import type { PaginatedResult } from '../types/common.types';

/**
 * خدمات الحقول الديناميكية لنوع الوحدة (Admin)
 */
export const AdminUnitTypeFieldsService = {
  // Admin endpoints
  /** إنشاء حقل نوع الوحدة */
  create: (data: CreateUnitTypeFieldCommand) =>
    apiClient.post<ResultDto<string>>('/api/admin/unit-type-fields', data).then(res => res.data),

  /** تحديث بيانات حقل نوع الوحدة */
  update: (fieldId: string, data: UpdateUnitTypeFieldCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/admin/unit-type-fields/${fieldId}`, data).then(res => res.data),

  /** حذف حقل نوع الوحدة */
  delete: (fieldId: string) =>
    apiClient.delete<ResultDto<boolean>>(`/api/admin/unit-type-fields/${fieldId}`).then(res => res.data),

  /** جلب الحقول لنوع وحدة معين */
  getByUnitType: ({ unitTypeId, ...filters }: GetUnitTypeFieldsQuery) =>
    apiClient.get<UnitTypeFieldDto[]>(
      `/api/admin/unit-type-fields/unit-type/${unitTypeId}`,
      { params: filters }
    ).then(res => res.data),
};