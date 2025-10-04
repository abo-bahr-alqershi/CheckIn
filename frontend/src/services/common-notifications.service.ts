import { apiClient } from './api.service';
import type { ResultDto } from '../types/common.types';

// المسار الأساسي لتعاملات الإشعارات المشتركة
const API_BASE = '/api/common/notifications';

export const CommonNotificationsService = {
  // وضع علامة مقروء على جميع الإشعارات
  markAllAsRead: () =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/mark-all-read`).then(res => res.data),

  // وضع علامة مقروء على إشعار محدد
  markAsRead: (notificationId: string) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${notificationId}/read`).then(res => res.data),
};