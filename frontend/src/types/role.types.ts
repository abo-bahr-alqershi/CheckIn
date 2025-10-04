// أنواع بيانات الأدوار (Roles)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * بيانات الدور الأساسية
 */
export interface RoleDto {
  id: string;
  name: string;
}

/**
 * أدوار المستخدمين في النظام
 * (تم التحويل إلى نوع union string لضمان التوافق مع إعدادات TypeScript الحديثة)
 */
export type UserRole =
  | 'SUPER_ADMIN'
  | 'ADMIN'
  | 'HOTEL_OWNER'
  | 'HOTEL_MANAGER'
  | 'RECEPTIONIST'
  | 'CUSTOMER';

/**
 * أمر إنشاء دور جديد
 */
export interface CreateRoleCommand {
  /** اسم الدور */
  name: string;
}

/**
 * أمر تعديل دور
 */
export interface UpdateRoleCommand {
  /** معرف الدور */
  roleId: string;
  /** الاسم الجديد للدور */
  name: string;
}

/**
 * أمر حذف دور
 */
export interface DeleteRoleCommand {
  /** معرف الدور */
  roleId: string;
}

/**
 * استعلام جلب جميع الأدوار
 */
export interface GetAllRolesQuery {
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}
