import { apiClient } from './api.service';
import type { ResultDto } from '../types/common.types';
import type {
  FieldGroupDto,
  CreateFieldGroupCommand,
  UpdateFieldGroupCommand,
  DeleteFieldGroupCommand,
  ReorderFieldGroupsCommand,
  GetFieldGroupByIdQuery,
  GetFieldGroupsByUnitTypeQuery,
  AssignFieldToGroupCommand
} from '../types/field-group.types';

// المسار الأساسي لخدمة مجموعات الحقول للإدمن
const API_BASE = '/api/admin/fieldgroups';

/**
 * خدمات إدارة مجموعات الحقول للإدمن
 */
export const AdminFieldGroupsService = {
  /** إنشاء مجموعة حقول جديدة */
  create: (command: CreateFieldGroupCommand) =>
    apiClient.post<ResultDto<FieldGroupDto>>(API_BASE, command).then(res => res.data),

  /** تحديث مجموعة حقول */
  update: (command: UpdateFieldGroupCommand) =>
    apiClient.put<ResultDto<FieldGroupDto>>(
      `${API_BASE}/${command.groupId}`,
      command
    ).then(res => res.data),

  /** حذف مجموعة حقول */
  delete: (command: DeleteFieldGroupCommand) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${command.groupId}`).then(res => res.data),

  /** جلب مجموعات الحقول حسب نوع الوحدة */
  getByUnitType: (query: GetFieldGroupsByUnitTypeQuery) =>
    apiClient.get<FieldGroupDto[]>(`${API_BASE}/unit-type/${query.unitTypeId}`).then(res => res.data),
}; 