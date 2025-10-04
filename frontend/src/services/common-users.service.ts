import { apiClient } from './api.service';
// import { apiClient } from './api.service';
import type { ResultDto } from '../types/common.types';
import type {
  UserDto,
  GetCurrentUserQuery,
  UpdateUserProfilePictureCommand,
  UpdateUserSettingsCommand,
} from '../types/user.types';

// المسار الأساسي لتعاملات المستخدمين المشتركة
const API_BASE = '/api/common/users';

export const CommonUsersService = {
  // جلب بيانات المستخدم الحالي
  getCurrentUser: (query: GetCurrentUserQuery) =>
    apiClient.get<ResultDto<UserDto>>(`${API_BASE}/current`, { params: query }).then(res => res.data),

  // تحديث صورة الملف الشخصي
  updateProfilePicture: (data: UpdateUserProfilePictureCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/profile-picture`, data).then(res => res.data),

  // تحديث إعدادات المستخدم بصيغة JSON
  updateUserSettings: (data: UpdateUserSettingsCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/settings`, data).then(res => res.data),
};