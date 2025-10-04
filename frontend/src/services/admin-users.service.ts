import { apiClient } from './api.service';
import type {
  UserDto,
  CreateUserCommand,
  UpdateUserCommand,
  ActivateUserCommand,
  DeactivateUserCommand,
  GetAllUsersQuery,
  SearchUsersQuery,
  GetUserByIdQuery,
  AssignUserRoleCommand,
  GetUsersByRoleQuery,
  GetUserActivityLogQuery,
  GetUserLifetimeStatsQuery,
  GetUserNotificationsQuery,
  GetUserRolesQuery,
  UserDetailsDto,
} from '../types/user.types';
import type { ResultDto, PaginatedResult } from '../types/common.types';

// خدمات إدارة المستخدمين (Users Service)
export const AdminUsersService = {
  /** جلب جميع المستخدمين */
  getAll: (query?: GetAllUsersQuery) =>
    apiClient.get<PaginatedResult<UserDto>>('/api/admin/Users', { params: query }).then(res => res.data),

  /** إنشاء مستخدم جديد */
  create: (data: CreateUserCommand) =>
    apiClient.post<ResultDto<string>>('/api/admin/Users', data).then(res => res.data),

  /** تحديث بيانات مستخدم */
  update: (userId: string, data: UpdateUserCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/admin/Users/${userId}`, data).then(res => res.data),

  /** تفعيل مستخدم */
  activate: (userId: string) =>
    apiClient.post<ResultDto<boolean>>(`/api/admin/Users/${userId}/activate`).then(res => res.data),

  /** إلغاء تفعيل مستخدم */
  deactivate: (userId: string) =>
    apiClient.post<ResultDto<boolean>>(`/api/admin/Users/${userId}/deactivate`).then(res => res.data),

  /** تخصيص دور للمستخدم */
  assignRole: (query: AssignUserRoleCommand) =>
    apiClient.post<ResultDto<boolean>>(`/api/admin/Users/${query.userId}/assign-role`, query).then(res => res.data),

  /** جلب المستخدمين حسب الدور */
  getByRole: (query: GetUsersByRoleQuery) =>
    apiClient.get<PaginatedResult<UserDto>>('/api/admin/Users/by-role', { params: query }).then(res => res.data),

  /** جلب إحصائيات المستخدم مدى الحياة */
  getLifetimeStats: (query: GetUserLifetimeStatsQuery) =>
    apiClient.get<ResultDto<any>>(`/api/admin/Users/${query.userId}/lifetime-stats`).then(res => res.data),

  /** جلب إشعارات المستخدم */
  getNotifications: (query: GetUserNotificationsQuery) => {
    const { userId, ...params } = query;
    return apiClient
      .get<PaginatedResult<any>>(`/api/admin/Users/${userId}/notifications`, { params })
      .then(res => res.data);
  },
  /** جلب تفاصيل مستخدم كاملة */
  getDetails: (userId: string) =>
    apiClient.get<ResultDto<UserDetailsDto>>(`/api/admin/Users/${userId}/details`).then(res => res.data),
};
