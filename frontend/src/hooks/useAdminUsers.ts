import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminUsersService } from '../services/admin-users.service';
import type { UserDto, CreateUserCommand, UpdateUserCommand, GetAllUsersQuery, UserDetailsDto } from '../types/user.types';
import type { PaginatedResult, ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وعمليات المستخدمين للوحة الإدارة
 * يعزل التعامل مع react-query والخدمات في مكان واحد
 * @param params معايير الاستعلام (بحث، صفحات، فلاتر)
 * @returns بيانات المستخدمين وحالات التحميل والأخطاء ودوال الإنشاء والتحديث والتفعيل/إلغاء التفعيل
 */
export const useAdminUsers = (params: GetAllUsersQuery) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-users', params] as const;

  // جلب جميع المستخدمين مع الفلاتر والصفحات
  const { data: usersData, isLoading, error } = useQuery<PaginatedResult<UserDto>, Error>({
    queryKey,
    queryFn: () => AdminUsersService.getAll(params),
  });

  // إنشاء مستخدم جديد
  const createUser = useMutation<ResultDto<string>, Error, CreateUserCommand>({
    mutationFn: (data) => AdminUsersService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
    },
  });

  // تحديث بيانات مستخدم
  const updateUser = useMutation<ResultDto<boolean>, Error, { userId: string; data: UpdateUserCommand }>({
    mutationFn: ({ userId, data }) => AdminUsersService.update(userId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
    },
  });

  // تفعيل مستخدم
  const activateUser = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (userId) => AdminUsersService.activate(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
    },
  });

  // إلغاء تفعيل مستخدم
  const deactivateUser = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (userId) => AdminUsersService.deactivate(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
    },
  });

  return { usersData, isLoading, error, createUser, updateUser, activateUser, deactivateUser };
};

/**
 * هوك لجلب تفاصيل مستخدم واحد
 * @param userId معرف المستخدم
 * @returns تفاصيل المستخدم وحالات التحميل والأخطاء
 */
export const useUserDetails = (userId: string) => {
  const queryKey = ['user-details', userId] as const;

  const { data, isLoading, error, refetch } = useQuery<ResultDto<UserDetailsDto>, Error>({
    queryKey,
    queryFn: () => AdminUsersService.getDetails(userId),
    enabled: !!userId,
  });

  return { 
    userDetails: data?.data || null, 
    isLoading, 
    error, 
    refetch,
    isSuccess: data?.success || false,
    message: data?.message || null
  };
}; 