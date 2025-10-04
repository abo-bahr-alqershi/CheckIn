import axios from 'axios';

// إعداد الـ API client للاتصال بالباك اند
const runtimeBaseUrl = ((): string => {
  // استخدم متغير بيئة من Vite إن وجد
  const envUrl = (import.meta as any)?.env?.VITE_API_BASE_URL as string | undefined;
  if (envUrl && envUrl.trim().length > 0) return envUrl;

  // افتراض: نفس الأصل + مسار API النسبي
  if (typeof window !== 'undefined' && window.location) {
    return `${window.location.origin}`;
  }
  // fallback أخير
  return '';
})();

export const apiClient = axios.create({
  baseURL: runtimeBaseUrl,
  timeout: 10000,
  withCredentials: true,  // إرسال الكوكيز لجلسات المصادقة تلقائياً
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    // ملاحظة: Access-Control-Allow-Origin يُحدد من الخادم فقط
  }
});

// إضافة interceptor للتوكن
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Debug logging for availability requests
    if (config.url?.includes('/availability/bulk')) {
      console.log('=== API REQUEST DEBUG ===');
      console.log('URL:', config.url);
      console.log('Method:', config.method);
      console.log('Headers:', config.headers);
      console.log('Data:', config.data);
      console.log('=== END REQUEST DEBUG ===');
    }
    
    return config;
  },
  (error) => Promise.reject(error)
);

// متغير لتجنب التكرار أثناء تحديث التوكن
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value?: any) => void;
  reject: (reason?: any) => void;
}> = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) {
      reject(error);
    } else {
      resolve(token);
    }
  });
  
  failedQueue = [];
};

// إضافة interceptor للاستجابة مع تحديث التوكن التلقائي
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then(() => {
          return apiClient(originalRequest);
        }).catch(err => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = localStorage.getItem('refreshToken');
      
      if (refreshToken) {
        try {
          const response = await apiClient.post('/api/common/auth/refresh-token', {
            refreshToken
          });
          
          const { accessToken: newToken, refreshToken: newRefreshToken } = response.data.data;
          
          localStorage.setItem('token', newToken);
          localStorage.setItem('refreshToken', newRefreshToken);
          
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          processQueue(null, newToken);
          
          return apiClient(originalRequest);
        } catch (refreshError) {
          processQueue(refreshError, null);
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
          window.location.href = '/auth/login';
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      } else {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        window.location.href = '/auth/login';
      }
    }
    
    return Promise.reject(error);
  }
); 

export default apiClient; 