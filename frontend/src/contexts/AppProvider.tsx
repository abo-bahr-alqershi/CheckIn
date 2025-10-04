import React, { createContext, useContext, useEffect } from 'react';
import type { ReactNode } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { useAppStore, useNotifications } from '../stores/appStore';
import { ToastContainer } from '../components/ui/Toast';

// إعداد React Query Client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 3,
      retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),
      staleTime: 5 * 60 * 1000, // 5 دقائق
      gcTime: 10 * 60 * 1000, // 10 دقائق (cacheTime سابقاً)
      refetchOnWindowFocus: false,
      refetchOnReconnect: true,
    },
    mutations: {
      retry: 1,
      retryDelay: 1000,
    },
  },
});

// واجهة سياق التطبيق
interface AppContextType {
  queryClient: QueryClient;
  isOnline: boolean;
  theme: 'light' | 'dark' | 'auto';
  language: 'ar' | 'en';
}

// إنشاء السياق
const AppContext = createContext<AppContextType | undefined>(undefined);

// Hook لاستخدام السياق
export const useAppContext = () => {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error('useAppContext must be used within AppProvider');
  }
  return context;
};

// مكون Provider الرئيسي
interface AppProviderProps {
  children: ReactNode;
}

export const AppProvider: React.FC<AppProviderProps> = ({ children }) => {
  const settings = useAppStore((state) => state.settings);
  const { showError } = useNotifications();
  const [isOnline, setIsOnline] = React.useState(navigator.onLine);

  // مراقبة حالة الاتصال
  useEffect(() => {
    const handleOnline = () => {
      setIsOnline(true);
      console.log('🌐 Connection restored');
    };

    const handleOffline = () => {
      setIsOnline(false);
      showError('فقدان الاتصال بالإنترنت');
      console.log('📵 Connection lost');
    };

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, [showError]);

  // تطبيق إعدادات الثيم
  useEffect(() => {
    const root = document.documentElement;
    
    if (settings.theme === 'dark') {
      root.classList.add('dark');
    } else if (settings.theme === 'light') {
      root.classList.remove('dark');
    } else {
      // auto mode - تابع إعدادات النظام
      const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      if (isDark) {
        root.classList.add('dark');
      } else {
        root.classList.remove('dark');
      }
    }
  }, [settings.theme]);

  // تطبيق إعدادات اللغة
  useEffect(() => {
    document.documentElement.lang = settings.language;
    document.documentElement.dir = settings.language === 'ar' ? 'rtl' : 'ltr';
  }, [settings.language]);

  // معالج أخطاء React Query
  useEffect(() => {
    const handleQueryError = (error: any) => {
      console.error('Query Error:', error);
      
      if (error?.response?.status === 401) {
        showError('انتهت صلاحية الجلسة. يرجى تسجيل الدخول مرة أخرى.');
        // إعادة توجيه لصفحة تسجيل الدخول
        window.location.href = '/auth/login';
      } else if (error?.response?.status === 403) {
        showError('ليس لديك صلاحية للوصول لهذا المحتوى');
      } else if (error?.response?.status >= 500) {
        showError('خطأ في الخادم. يرجى المحاولة لاحقاً.');
      } else if (!isOnline) {
        showError('لا يوجد اتصال بالإنترنت');
      }
    };

    // إضافة معالج للأخطاء العامة
    queryClient.getQueryCache().subscribe((event) => {
      const q = (event as any).query;
      if (q?.state.status === 'error') {
        handleQueryError(q.state.error);
      }
    });

    queryClient.getMutationCache().subscribe((event) => {
      const m = (event as any).mutation;
      if (m?.state.status === 'error') {
        handleQueryError(m.state.error);
      }
    });
  }, [showError, isOnline]);

  const contextValue: AppContextType = {
    queryClient,
    isOnline,
    theme: settings.theme,
    language: settings.language,
  };

  return (
    <QueryClientProvider client={queryClient}>
      <AppContext.Provider value={contextValue}>
        {children}
        {/* Toast Container for notifications */}
        <ToastContainer />
        {/* إظهار React Query DevTools في التطوير فقط */}
        {process.env.NODE_ENV === 'development' && (
          <ReactQueryDevtools initialIsOpen={false} />
        )}
      </AppContext.Provider>
    </QueryClientProvider>
  );
};

// Hook لإدارة الحالة المتزامنة
export const useSyncState = () => {
  const settings = useAppStore((state) => state.settings);
  const updateSettings = useAppStore((state) => state.updateSettings);
  
  // مزامنة الإعدادات مع localStorage
  useEffect(() => {
    const savedSettings = localStorage.getItem('bookn-settings');
    if (savedSettings) {
      try {
        const parsed = JSON.parse(savedSettings);
        updateSettings(parsed);
      } catch (error) {
        console.error('Error parsing saved settings:', error);
      }
    }
  }, [updateSettings]);

  // حفظ الإعدادات عند تغييرها
  useEffect(() => {
    localStorage.setItem('bookn-settings', JSON.stringify(settings));
  }, [settings]);
};

// Hook لإدارة اختصارات لوحة المفاتيح
export const useKeyboardShortcuts = () => {
  const setActiveModal = useAppStore((state) => state.setActiveModal);
  const { showError, clearNotifications } = useNotifications();

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      // Ctrl/Cmd + K للبحث السريع
      if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
        event.preventDefault();
        setActiveModal('search');
      }

      // Escape لإغلاق الإشعارات
      if (event.key === 'Escape') {
        clearNotifications();
      }

      // Ctrl/Cmd + / لإظهار اختصارات المساعدة
      if ((event.ctrlKey || event.metaKey) && event.key === '/') {
        event.preventDefault();
        setActiveModal('shortcuts');
      }
    };

    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [setActiveModal, clearNotifications]);
};

export default AppProvider;