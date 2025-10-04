// أنواع بيانات الخدمات (Services)
import type { MoneyDto } from './amenity.types';
// جميع الحقول موثقة بالعربي لضمان الوضوح والتوافق مع الباك اند

/**
 * بيانات الخدمة الأساسية
 */
export interface ServiceDto {
  id: string;
  propertyId: string;
  propertyName: string;
  name: string;
  price: MoneyDto;
  pricingModel: PricingModel;
  icon: string;
}

export type PricingModel = {
  PerBooking: 0,
  PerDay: 1,
  PerPerson: 2,
  PerUnit: 3,
}


/**
 * أمر إنشاء خدمة جديدة
 */
export interface CreateServiceCommand {
  name: string;
  description: string;
  type: string;
  details: Record<string, any>;
  price: MoneyDto;
}

/**
 * أمر تحديث خدمة
 */
export interface UpdateServiceCommand {
  serviceId: string;
  name: string;
  description: string;
  type: string;
  details: Record<string, any>;
  price: MoneyDto;
}

/**
 * أمر حذف خدمة
 */
export interface DeleteServiceCommand {
  serviceId: string;
}

/**
 * أمر لإنشاء خدمة جديدة للكيان (Property Service)
 */
export interface CreatePropertyServiceCommand {
  /** معرف الكيان */
  propertyId: string;
  /** اسم الخدمة */
  name: string;
  /** سعر الخدمة */
  price: MoneyDto;
  /** نموذج التسعير */
  pricingModel: string;
  /** أيقونة الخدمة */
  icon: string;
}

/**
 * أمر لتحديث خدمة كيان (Property Service)
 */
export interface UpdatePropertyServiceCommand {
  /** معرف الخدمة */
  serviceId: string;
  /** اسم الخدمة (اختياري) */
  name?: string;
  /** سعر الخدمة (اختياري) */
  price?: MoneyDto;
  /** نموذج التسعير (اختياري) */
  pricingModel?: string;
  /** أيقونة الخدمة */
  icon: string;

}

/**
 * أمر لحذف خدمة كيان (Property Service)
 */
export interface DeletePropertyServiceCommand {
  /** معرف الخدمة */
  serviceId: string;
}

/**
 * تفاصيل الخدمة
 */
export interface ServiceDetailsDto {
  id: string;
  propertyId: string;
  propertyName: string;
  name: string;
  price: MoneyDto;
  pricingModel: PricingModel;
  icon: string;
}

/**
 * استعلام لجلب خدمات الكيان
 */
export interface GetPropertyServicesQuery {
  /** معرف الكيان */
  propertyId: string;
}

/**
 * استعلام لجلب بيانات خدمة بواسطة المعرف
 */
export interface GetServiceByIdQuery {
  /** معرف الخدمة */
  serviceId: string;
}

/**
 * استعلام لجلب الخدمات حسب النوع
 */
export interface GetServicesByTypeQuery {
  /** نوع الخدمة */
  serviceType: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}
