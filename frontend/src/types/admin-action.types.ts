// أنواع بيانات إجراءات الإدارة (Admin Actions)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * أنواع الأهداف (TargetType)
 */
export type TargetType = 'Property' | 'User' | 'Booking';

/**
 * أنواع الإجراءات (ActionType)
 */
export type ActionType = 'Create' | 'Update' | 'Delete' | 'Approve';

/**
 * بيانات إجراء الإدارة الأساسية
 */
export interface AdminActionDto {
  /** معرف المدير */
  adminId: string;
  /** معرف الهدف */
  targetId: string;
  /** نوع الهدف (property, user, booking) */
  targetType: TargetType;
  /** نوع الإجراء (create, update, delete, approve) */
  actionType: ActionType;
  /** الطابع الزمني للإجراء */
  timestamp: string;
  /** التغييرات (JSON) */
  changes: string;
}