import apiClient from './api.service';
import type { AxiosResponse } from 'axios';
import type {
  UnitAvailability,
  PricingRule,
  CreateAvailabilityRequest,
  CreatePricingRequest,
  ConflictCheckRequest,
  ConflictCheckResponse,
  UnitManagementData,
  AvailabilityError,
  PricingError,
} from '../types/availability.types';

// Use shared apiClient
const api = apiClient;

// ===== خدمة إدارة الإتاحة =====
export class AvailabilityService {
  

  // إنشاء إتاحة جديدة
  static async createAvailability(request: CreateAvailabilityRequest): Promise<UnitAvailability> {
    try {
      const response = await api.post('/api/availability', request);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في إنشاء الإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // معالجة الأخطاء للإتاحة
  private static handleError(error: any, type: 'availability'): AvailabilityError {
    const errorData = error.response?.data;
    
    return {
      error_code: errorData?.error_code || 'UNKNOWN_ERROR',
      error_type: errorData?.error_type || 'system',
      message: errorData?.message || 'حدث خطأ غير متوقع',
      details: errorData?.details,
      suggested_action: errorData?.suggested_action || 'يرجى المحاولة مرة أخرى'
    };
  }
}

// ===== خدمة إدارة التسعير =====
export class PricingService {
  
  // إنشاء قاعدة تسعير جديدة
  static async createPricing(request: CreatePricingRequest): Promise<PricingRule> {
    try {
      const response = await api.post('/api/pricing', request);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في إنشاء قاعدة التسعير:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // معالجة الأخطاء للتسعير
  private static handleError(error: any, type: 'pricing'): PricingError {
    const errorData = error.response?.data;
    
    return {
      error_code: errorData?.error_code || 'UNKNOWN_ERROR',
      error_type: errorData?.error_type || 'system',
      message: errorData?.message || 'حدث خطأ غير متوقع',
      details: errorData?.details,
      suggested_action: errorData?.suggested_action || 'يرجى المحاولة مرة أخرى'
    };
  }
}

// ===== خدمة التحقق من التعارضات =====
export class BookingValidationService {
  
  // التحقق من التعارضات
  static async checkConflicts(request: ConflictCheckRequest): Promise<ConflictCheckResponse> {
    try {
      const response = await api.post('/api/validation/check-conflicts', request);
      return response.data;
    } catch (error) {
      console.error('خطأ في التحقق من التعارضات:', error);
      throw error;
    }
  }


}

// ===== خدمة البيانات الشاملة =====
export class ManagementDataService {

  // الحصول على بيانات وحدة محددة
  static async getUnitManagementData(unitId: string, startDate?: string, endDate?: string): Promise<UnitManagementData> {
    try {
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      const query = params.toString() ? `?${params.toString()}` : '';
      const response = await api.get(`/api/management/unit/${unitId}${query}`);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في الحصول على بيانات الوحدة:', error);
      throw error;
    }
  }


}

// ===== خدمة مجمعة تضم جميع الخدمات =====
export class AvailabilityAndPricingService {
  static availability = AvailabilityService;
  static pricing = PricingService;
  static validation = BookingValidationService;
  static management = ManagementDataService;

  // تهيئة الخدمة
  static init(config?: {
    baseURL?: string;
    timeout?: number;
    defaultHeaders?: Record<string, string>;
  }) {
    if (config?.baseURL) {
      api.defaults.baseURL = config.baseURL;
    }
    if (config?.timeout) {
      api.defaults.timeout = config.timeout;
    }
    if (config?.defaultHeaders) {
      Object.assign(api.defaults.headers, config.defaultHeaders);
    }
  }

  // تعيين الـ token
  static setAuthToken(token: string) {
    localStorage.setItem('auth_token', token);
  }

  // إزالة الـ token
  static removeAuthToken() {
    localStorage.removeItem('auth_token');
  }
}

export default AvailabilityAndPricingService;