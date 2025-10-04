import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  UserDto,
  RegisterPropertyOwnerCommand,
  UpdateUserCommand,
  GetUserByIdQuery,
  GetUsersByRoleQuery,
  GetUserByEmailQuery,
  SearchUsersQuery,
  GetUserActivityLogQuery,
  GetUserLifetimeStatsQuery,
  GetUserNotificationsQuery,
} from '../types/user.types';

// المسار الأساسي لتعاملات المستخدمين لأصحاب الكيانات
const API_BASE = '/api/property/users';

export const PropertyUsersService = {
  // تسجيل صاحب كيان جديد
  registerPropertyOwner: (data: RegisterPropertyOwnerCommand) =>
    apiClient.post<ResultDto<string>>(`${API_BASE}/register`, data).then(res => res.data),

};