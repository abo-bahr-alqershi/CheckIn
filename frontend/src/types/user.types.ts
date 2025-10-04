// أنواع بيانات المستخدمين (Users)

import type { PropertyImageDto } from './property-image.types';

export interface UserDto {
  id: string;
  name: string;
  /** دور المستخدم */
  role: string;
  email: string;
  phone: string;
  profileImage: string;
  createdAt: string;
  isActive: boolean;
  /** إعدادات المستخدم بصيغة JSON */
  settingsJson: string;
  /** قائمة المفضلة للمستخدم بصيغة JSON */
  favoritesJson: string;
}

export interface CreateUserCommand {
  name: string;
  email: string;
  password: string;
  phone: string;
  profileImage: string;
}

export interface UpdateUserCommand {
  userId: string;
  name?: string;
  email?: string;
  phone?: string;
  profileImage?: string;
}

export interface ActivateUserCommand {
  userId: string;
}

export interface DeactivateUserCommand {
  userId: string;
}

export interface GetAllUsersQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  isAscending?: boolean;
  roleId?: string;
  isActive?: boolean;
  createdAfter?: string;
  createdBefore?: string;
  lastLoginAfter?: string;
  loyaltyTier?: string;
  minTotalSpent?: number;
}

export interface SearchUsersQuery {
  searchTerm: string;
  filterCriteria?: string;
  pageNumber?: number;
  pageSize?: number;
  roleId?: string;
  isActive?: boolean;
  createdAfter?: string;
  createdBefore?: string;
  lastLoginAfter?: string;
  loyaltyTier?: string;
  minTotalSpent?: number;
  sortBy?: string;
}

/**
 * استعلام جلب بيانات مستخدم بواسطة المعرف
 */
export interface GetUserByIdQuery {
  userId: string;
}

/**
 * أمر تخصيص دور للمستخدم
 */
export interface AssignUserRoleCommand {
  /** معرف المستخدم */
  userId: string;
  /** معرف الدور */
  roleId: string;
}

/**
 * استعلام جلب المستخدمين حسب الدور
 */
export interface GetUsersByRoleQuery {
  roleName: string;
  pageNumber?: number;
  pageSize?: number;
}

/**
 * استعلام سجلات نشاط المستخدم
 */
export interface GetUserActivityLogQuery {
  /** معرف المستخدم */
  userId: string;
  /** تاريخ البداية (ISO) */
  from?: string;
  /** تاريخ النهاية (ISO) */
  to?: string;
}

/**
 * استعلام إحصائيات المستخدم مدى الحياة
 */
export interface GetUserLifetimeStatsQuery {
  /** معرف المستخدم */
  userId: string;
}

/**
 * استعلام إشعارات المستخدم
 */
export interface GetUserNotificationsQuery {
  /** معرف المستخدم */
  userId: string;
  /** حالة الإشعار (مقروء/غير مقروء) */
  isRead?: boolean;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
  /** فلترة بنوع الإشعار */
  notificationType?: string;
  /** فلترة بتاريخ الإرسال بعد */
  sentAfter?: string;
  /** فلترة بتاريخ الإرسال قبل */
  sentBefore?: string;
  /** خيارات الترتيب */
  sortBy?: string;
}

/**
 * استعلام إرجاع أدوار المستخدم
 */
export interface UserLifetimeStatsDto {
  totalNightsStayed: number;
  totalMoneySpent: number;
  favoriteCity: string;
}

export interface GetUserRolesQuery {
  userId: string;
  pageNumber?: number;
  pageSize?: number;
}

/**
 * استعلام للحصول على بيانات المستخدم الحالي
 * Query to get current logged-in user data
 */
export interface GetCurrentUserQuery {}

export interface UpdateUserFavoritesCommand {
  favoritesJson: string;
}

export interface UpdateUserProfilePictureCommand {
  userId: string;
  profileImageUrl: string;
}

export interface UpdateUserSettingsCommand {
  settingsJson: string;
}

export interface LoyaltyProgressDto {
  currentTier: string;
  nextTier: string;
  amountNeededForNextTier: number;
}

export interface OwnerRegistrationResultDto {
  userId: string;
  propertyId: string;
}

/**
 * استعلام لجلب مستخدم بواسطة البريد الإلكتروني
 */
export interface GetUserByEmailQuery {
  email: string;
}

/**
 * أمر لتسجيل مالك كيان جديد مع بيانات الكيان الكاملة والحقول الديناميكية
 */
export interface RegisterPropertyOwnerCommand {
  /** اسم المالك */
  name: string;
  /** بريد المالك الإلكتروني */
  email: string;
  /** كلمة مرور المالك */
  password: string;
  /** رقم هاتف المالك */
  phone: string;
  /** رابط صورة الملف الشخصي (اختياري) */
  profileImage?: string;
  /** معرف نوع الكيان */
  propertyTypeId: string;
  /** اسم الكيان */
  propertyName: string;
  /** وصف الكيان (اختياري) */
  description?: string;
  /** عنوان الكيان */
  address: string;
  /** المدينة */
  city: string;
  /** خط العرض (اختياري) */
  latitude?: number;
  /** خط الطول (اختياري) */
  longitude?: number;
  /** تقييم النجوم */
  starRating: number;
  /** الصور الأولية للكيان (اختياري) */
  initialImages?: PropertyImageDto[];
}

/**
 * نوع بيانات تفصيلية عن المستخدم
 */
export interface UserDetailsDto {
  // معلومات مشتركة لجميع المستخدمين
  /** معرف المستخدم */
  id: string;
  /** اسم المستخدم */
  userName: string;
  /** رابط صورة المستخدم */
  avatarUrl: string;
  /** البريد الإلكتروني */
  email: string;
  /** رقم الهاتف */
  phoneNumber: string;
  /** تاريخ إنشاء الحساب (ISO) */
  createdAt: string;
  /** هل الحساب مفعل */
  isActive: boolean;
  
  // بيانات حساب العميل وايضا الكيان
  /** إجمالي عدد الحجوزات للمستخدم في حال كان الحساب عميل */
  /** إجمالي عدد الحجوزات للكيان في حال كان الحساب مالك او موظف */
  bookingsCount: number;
  /** عدد الحجوزات الملغاة للمستخدم في حال كان الحساب عميل */
  /** عدد الحجوزات الملغاة للكيان في حال كان الحساب مالك او موظف */
  canceledBookingsCount: number;
  /** عدد الحجوزات المعلقة للمستخدم في حال كان الحساب عميل */
  /** عدد الحجوزات المعلقة للكيان في حال كان الحساب مالك او موظف */
  pendingBookingsCount: number;
  /** تاريخ أول حجز للمستخدم في حال كان الحساب عميل */
  /** تاريخ أول حجز للكيان في حال كان الحساب مالك او موظف */
  firstBookingDate: string | null;
  /** تاريخ آخر حجز للمستخدم في حال كان الحساب عميل */
  /** تاريخ آخر حجز للكيان في حال كان الحساب مالك او موظف */
  lastBookingDate: string | null;
  /** عدد البلاغات التي أنشأها المستخدم في حال كان الحساب عميل */
  /** عدد البلاغات التي أنشأها الفندق (سواء مالك او موظفين) في حال كان الحساب مالك او موظف */
  reportsCreatedCount: number;
  /** عدد البلاغات ضده (على المستخدم) في حال كان الحساب عميل */
  /** عدد البلاغات ضده (على الكيان) في حال كان الحساب مالك او موظف */
  reportsAgainstCount: number;
  /** إجمالي المدفوعات للمستخدم في حال كان الحساب عميل */
  /** إجمالي المدفوعات للكيان (سواء مالك او موظفين) في حال كان الحساب مالك او موظف */
  totalPayments: number;
  /** إجمالي المردودات للمستخدم في حال كان الحساب عميل */
  /** إجمالي المردودات للكيان (سواء مالك او موظفين) في حال كان الحساب مالك او موظف */
  totalRefunds: number;
  /** عدد المراجعات التي أجرىها المستخدم في حال كان الحساب عميل */
  /** عدد المراجعات التي أجراها العملاء على الكيان في حال كان الحساب مالك او موظف */
  reviewsCount: number;

  // بيانات حساب المالك أو الموظف (Owner/Staff-only) للكيان
  /** وظيفة المستخدم (Owner أو Staff) */
  role?: 'Owner' | 'Staff';
  /** معرف الكيان المرتبط */
  propertyId?: string;
  /** اسم الكيان المرتبط */
  propertyName?: string;
  /** عدد الوحدات في الكيان */
  unitsCount?: number;
  /** عدد صور الكيان */
  propertyImagesCount?: number;
  /** عدد صور الوحدات */
  unitImagesCount?: number;
  /** صافي الإيراد للكيان */
  netRevenue?: number;
  /** عدد الردود على البلاغات (غير مستخدم حاليًا) */
  repliesCount?: number;
}