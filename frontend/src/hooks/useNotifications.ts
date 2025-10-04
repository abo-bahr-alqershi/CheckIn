import { useState, useCallback } from 'react';

export interface NotificationItem {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title?: string;
  message: string;
  duration?: number;
  isVisible: boolean;
}

interface UseNotificationsReturn {
  notifications: NotificationItem[];
  showSuccess: (message: string, title?: string, duration?: number) => void;
  showError: (message: string, title?: string, duration?: number) => void;
  showWarning: (message: string, title?: string, duration?: number) => void;
  showInfo: (message: string, title?: string, duration?: number) => void;
  removeNotification: (id: string) => void;
  clearAll: () => void;
}

export const useNotifications = (): UseNotificationsReturn => {
  const [notifications, setNotifications] = useState<NotificationItem[]>([]);

  const generateId = () => Math.random().toString(36).substr(2, 9);

  const addNotification = useCallback((
    type: NotificationItem['type'],
    message: string,
    title?: string,
    duration: number = 5000
  ) => {
    const id = generateId();
    const notification: NotificationItem = {
      id,
      type,
      title,
      message,
      duration,
      isVisible: true
    };

    setNotifications(prev => [notification, ...prev]);

    // إزالة الإشعار تلقائياً بعد المدة المحددة
    if (duration > 0) {
      setTimeout(() => {
        removeNotification(id);
      }, duration);
    }
  }, []);

  const removeNotification = useCallback((id: string) => {
    setNotifications(prev => prev.filter(notification => notification.id !== id));
  }, []);

  const clearAll = useCallback(() => {
    setNotifications([]);
  }, []);

  const showSuccess = useCallback((message: string, title?: string, duration?: number) => {
    addNotification('success', message, title, duration);
  }, [addNotification]);

  const showError = useCallback((message: string, title?: string, duration?: number) => {
    addNotification('error', message, title, duration);
  }, [addNotification]);

  const showWarning = useCallback((message: string, title?: string, duration?: number) => {
    addNotification('warning', message, title, duration);
  }, [addNotification]);

  const showInfo = useCallback((message: string, title?: string, duration?: number) => {
    addNotification('info', message, title, duration);
  }, [addNotification]);

  return {
    notifications,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    removeNotification,
    clearAll
  };
};