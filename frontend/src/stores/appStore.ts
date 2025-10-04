import { create } from 'zustand';
import { persist } from 'zustand/middleware';

// واجهة حالة التطبيق
interface AppState {
  // حالة المستخدم الحالي
  currentUser: {
    id: string;
    name: string;
    email: string;
    role: string;
    permissions: string[];
    profileImage?: string;
  } | null;
  
  // إعدادات التطبيق
  settings: {
    theme: 'light' | 'dark' | 'auto';
    language: 'ar' | 'en';
    currency: string;
    timezone: string;
    pageSize: number;
    dateFormat: string;
    notifications: {
      email: boolean;
      push: boolean;
      sms: boolean;
    };
  };
  
  // حالة UI
  ui: {
    sidebarCollapsed: boolean;
    loading: boolean;
    error: string | null;
    success: string | null;
    activeModal: string | null;
    breadcrumbs: Array<{
      label: string;
      href?: string;
    }>;
  };
  
  // بيانات مؤقتة
  cache: {
    properties: any[];
    users: any[];
    unitTypes: any[];
    lastUpdated: Record<string, number>;
  };
}

// واجهة الإجراءات
interface AppActions {
  // إجراءات المستخدم
  setCurrentUser: (user: AppState['currentUser']) => void;
  logout: () => void;
  
  // إجراءات الإعدادات
  updateSettings: (settings: Partial<AppState['settings']>) => void;
  
  // إجراءات UI
  setSidebarCollapsed: (collapsed: boolean) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  setSuccess: (success: string | null) => void;
  setActiveModal: (modal: string | null) => void;
  setBreadcrumbs: (breadcrumbs: AppState['ui']['breadcrumbs']) => void;
  
  // إجراءات التخزين المؤقت
  updateCache: (key: keyof AppState['cache'], data: any) => void;
  clearCache: (key?: keyof AppState['cache']) => void;
  
  // إجراءات الأدوات المساعدة
  clearNotifications: () => void;
  resetState: () => void;
}

// الحالة الافتراضية
const defaultState: AppState = {
  currentUser: null,
  settings: {
    theme: 'light',
    language: 'ar',
    currency: 'YER',
    timezone: 'Asia/Riyadh',
    pageSize: 20,
    dateFormat: 'DD/MM/YYYY',
    notifications: {
      email: true,
      push: true,
      sms: false,
    },
  },
  ui: {
    sidebarCollapsed: false,
    loading: false,
    error: null,
    success: null,
    activeModal: null,
    breadcrumbs: [],
  },
  cache: {
    properties: [],
    users: [],
    unitTypes: [],
    lastUpdated: {} as Record<string, number>,
  },
};

// إنشاء Store
export const useAppStore = create<AppState & AppActions>()(
  persist(
    (set, get) => ({
      ...defaultState,

      // إجراءات المستخدم
      setCurrentUser: (user) => 
        set((state) => ({ 
          currentUser: user,
          ui: { ...state.ui, error: null }
        })),

      logout: () => 
        set((state) => ({ 
          currentUser: null,
          cache: defaultState.cache,
          ui: { ...state.ui, activeModal: null, error: null, success: null }
        })),

      // إجراءات الإعدادات
      updateSettings: (newSettings) =>
        set((state) => ({
          settings: { ...state.settings, ...newSettings }
        })),

      // إجراءات UI
      setSidebarCollapsed: (collapsed) =>
        set((state) => ({
          ui: { ...state.ui, sidebarCollapsed: collapsed }
        })),

      setLoading: (loading) =>
        set((state) => ({
          ui: { ...state.ui, loading }
        })),

      setError: (error) =>
        set((state) => ({
          ui: { ...state.ui, error, success: error ? null : state.ui.success }
        })),

      setSuccess: (success) =>
        set((state) => ({
          ui: { ...state.ui, success, error: success ? null : state.ui.error }
        })),

      setActiveModal: (activeModal) =>
        set((state) => ({
          ui: { ...state.ui, activeModal }
        })),

      setBreadcrumbs: (breadcrumbs) =>
        set((state) => ({
          ui: { ...state.ui, breadcrumbs }
        })),

      // إجراءات التخزين المؤقت
      updateCache: (key, data) =>
        set((state) => ({
          cache: {
            ...state.cache,
            [key]: data,
            lastUpdated: {
              ...state.cache.lastUpdated,
              [key]: Date.now()
            }
          }
        })),

      clearCache: (key) =>
        set((state) => {
          if (key) {
            const newCache = { ...state.cache };
            // Assign default cache value by key with any-cast to bypass strict index type
            (newCache as any)[key] = (defaultState.cache as any)[key];
            delete (newCache as any).lastUpdated[key];
            return { cache: newCache };
          } else {
            return { cache: defaultState.cache };
          }
        }),

      // إجراءات الأدوات المساعدة
      clearNotifications: () =>
        set((state) => ({
          ui: { ...state.ui, error: null, success: null }
        })),

      resetState: () => set(defaultState),
    }),
    {
      name: 'bookn-app-store',
      partialize: (state) => ({
        currentUser: state.currentUser,
        settings: state.settings,
        ui: {
          sidebarCollapsed: state.ui.sidebarCollapsed,
        },
      }),
    }
  )
);

// Selectors مفيدة
export const useCurrentUser = () => useAppStore((state) => state.currentUser);
export const useSettings = () => useAppStore((state) => state.settings);
export const useUIState = () => useAppStore((state) => state.ui);
export const useIsLoading = () => useAppStore((state) => state.ui.loading);
export const useError = () => useAppStore((state) => state.ui.error);
export const useSuccess = () => useAppStore((state) => state.ui.success);

// Hook للمصادقة
export const useAuth = () => {
  const currentUser = useCurrentUser();
  const setCurrentUser = useAppStore((state) => state.setCurrentUser);
  const logout = useAppStore((state) => state.logout);

  const isAuthenticated = !!currentUser;
  const hasRole = (role: string) => currentUser?.role === role;
  const hasPermission = (permission: string) => 
    currentUser?.permissions.includes(permission) || false;

  return {
    currentUser,
    isAuthenticated,
    hasRole,
    hasPermission,
    setCurrentUser,
    logout,
  };
};

// Hook لإدارة الإشعارات
export const useNotifications = () => {
  const setError = useAppStore((state) => state.setError);
  const setSuccess = useAppStore((state) => state.setSuccess);
  const clearNotifications = useAppStore((state) => state.clearNotifications);
  const error = useError();
  const success = useSuccess();

  const showError = (message: string) => setError(message);
  const showSuccess = (message: string) => setSuccess(message);

  return {
    error,
    success,
    showError,
    showSuccess,
    clearNotifications,
  };
};

// Hook لإدارة التخزين المؤقت
export const useCache = () => {
  const cache = useAppStore((state) => state.cache);
  const updateCache = useAppStore((state) => state.updateCache);
  const clearCache = useAppStore((state) => state.clearCache);

  const isCacheValid = (key: keyof AppState['cache'], maxAge = 5 * 60 * 1000) => {
    const lastUpdated = cache.lastUpdated[key];
    return lastUpdated && (Date.now() - lastUpdated) < maxAge;
  };

  return {
    cache,
    updateCache,
    clearCache,
    isCacheValid,
  };
};

export type { AppState, AppActions };