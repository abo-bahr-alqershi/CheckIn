// أنواع بيانات الإشعارات (Notifications)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * بيانات الإشعار الأساسية
 */
export interface NotificationDto {
  id: string;
  type: string;
  title: string;
  message: string;
  priority: string;
  status: string;
  recipientId: string;
  recipientName: string;
  senderId?: string;
  senderName: string;
  isRead: boolean;
  readAt?: string;
  createdAt: string;
}

/**
 * أمر إنشاء إشعار جديد
 */
export interface CreateNotificationCommand {
  type: string;
  title: string;
  message: string;
  recipientId: string;
  senderId?: string;
}

/**
 * استعلام جلب إشعارات النظام مع التصفية والصفحات
 */
export interface GetSystemNotificationsQuery {
  notificationType?: string;
  pageNumber?: number;
  pageSize?: number;
  recipientId?: string;
  status?: string;
  sentAfter?: string;
  sentBefore?: string;
  sortBy?: string;
}

/**
 * استعلام جلب إشعارات مستخدم مع التصفية والصفحات
 */
export interface GetUserNotificationsQuery {
  userId: string;
  isRead?: boolean;
  pageNumber?: number;
  pageSize?: number;
  notificationType?: string;
  sentAfter?: string;
  sentBefore?: string;
  sortBy?: string;
}
