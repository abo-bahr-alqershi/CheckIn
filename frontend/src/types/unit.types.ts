// أنواع بيانات الوحدات (Units)
// جميع الحقول موثقة بالعربي لضمان الوضوح والتوافق مع الباك اند

import type { PropertyImageDto } from './property-image.types';
import type { FieldValueDto, UnitFieldValueDto as UnitFieldValueDtoFull } from './unit-field-value.types';
import type { FieldWithValueDto } from './unit-field-value.types';

/**
 * بيانات الوحدة الأساسية
 */
export interface UnitDto {
  id: string;
  propertyId: string;
  unitTypeId: string;
  name: string;
  basePrice: MoneyDto;
  customFeatures: string;
  isAvailable: boolean;
  propertyName: string;
  unitTypeName: string;
  pricingMethod: PricingMethod;
  fieldValues: UnitFieldValueDtoFull[];
  dynamicFields: FieldGroupWithValuesDto[];
  distanceKm?: number;
  images?: PropertyImageDto[];
  adultCapacity?: number;
  childrenCapacity?: number;
}

/**
 * تفاصيل الوحدة (تشمل الحقول الديناميكية)
 */
export interface UnitDetailsDto extends UnitDto {
  maxCapacity: number;
  viewCount: number;
  bookingCount: number;
  dynamicFields: FieldGroupWithValuesDto[];
}

/**
 * أمر إنشاء وحدة جديدة
 */
export interface CreateUnitDto {
  /** معرف نوع الوحدة */
  unitTypeId: string;
  unitNumber: string;
  basePrice: number;
  currency: string;
  maxCapacity: number;
  bedroomsCount?: number;
  bathroomsCount?: number;
  areaSquareMeters?: number;
  isAvailable: boolean;
  /** قيم الحقول الديناميكية للوحدة */
  dynamicFields?: UnitFieldValueDtoFull[];
  adultCapacity?: number;
  childrenCapacity?: number;

}

export interface CreateUnitCommand {
  propertyId: string;
  unitTypeId: string;
  name: string;
  basePrice: MoneyDto;
  customFeatures: string;
  pricingMethod: PricingMethod;
  /** قيم الحقول الديناميكية للوحدة (اجباري في حال توفرت) */
  fieldValues?: FieldValueDto[];
  /** روابط الصور المرفوعة مؤقتاً للوحدة */
  images?: string[];
  adultCapacity?: number;
  childrenCapacity?: number;
}

/**
 * أمر تحديث بيانات الوحدة
 */
export interface UpdateUnitCommand {
  unitId: string;
  name?: string;
  basePrice?: MoneyDto;
  customFeatures?: string;
  pricingMethod?: PricingMethod;
  /** قيم الحقول الديناميكية للوحدة (اجباري في حال توفرت) */
  fieldValues?: FieldValueDto[];
  /** روابط الصور المرفوعة مؤقتاً للوحدة */
  images?: string[];
  adultCapacity?: number;
  childrenCapacity?: number;
}

/**
 * أمر حذف وحدة
 */
export interface DeleteUnitCommand {
  unitId: string;
}

/**
 * استعلام جلب تفاصيل الوحدة
 */
export interface GetUnitByIdQuery {
  unitId: string;
}

/**
 * استعلام جلب الوحدات الخاصة بكيان معين
 */
export interface GetUnitsByPropertyQuery {
  propertyId: string;
  pageNumber?: number;
  pageSize?: number;
  isAvailable?: boolean;
  minBasePrice?: number;
  maxBasePrice?: number;
  minCapacity?: number;
  nameContains?: string;
}

/**
 * استعلام جلب الوحدات حسب النوع
 */
export interface GetUnitPriceQuery {
  unitId: string;
  checkIn: string;
  checkOut: string;
  guestCount?: number;
}

export interface GetUnitsByTypeQuery {
  unitTypeId: string;
  pageNumber?: number;
  pageSize?: number;
  isAvailable?: boolean;
  minBasePrice?: number;
  maxBasePrice?: number;
  minCapacity?: number;
  nameContains?: string;
  /** تضمين القيم الديناميكية (اختياري) */
  includeDynamicFields?: boolean;
}

/**
 * استعلام جلب توفر الوحدة
 */
export interface GetUnitDetailsQuery {
  unitId: string;
  includeDynamicFields?: boolean;
}

/**
 * استعلام جلب بيانات الوحدة للتعديل
 */
export interface GetUnitForEditQuery {
  unitId: string;
  ownerId: string;
}

/**
 * استعلام جلب صور الوحدة
 */
export interface GetUnitImagesQuery {
  /** معرف الوحدة */
  unitId: string;
}

/**
 * أمر إنشاء وحدة مع قيم الحقول الديناميكية
 */
export interface CreateUnitWithFieldValuesCommand {
  /** معرف الكيان */
  propertyId: string;
  /** معرف نوع الوحدة */
  unitTypeId: string;
  /** اسم الوحدة */
  name: string;
  /** السعر الأساسي */
  basePrice: MoneyDto;
  /** ميزات مخصصة (JSON) */
  customFeatures: string;
  /** طريقة حساب السعر */
  pricingMethod: PricingMethod;
  /** قيم الحقول الديناميكية */
  fieldValues: FieldValueDto[];
}

/**
 * أمر تحديث توفر الوحدة
 */
export interface UpdateUnitAvailabilityCommand {
  /** معرف الوحدة */
  unitId: string;
  /** متاح أم لا */
  isAvailable: boolean;
}

/**
 * أمر تحديث متعدد لتوفر الوحدات
 */
export interface BulkUpdateUnitAvailabilityCommand {
  /** قائمة أوامر توفر الوحدات */
  commands: UpdateUnitAvailabilityCommand[];
}

/**
 * استعلام جلب الوحدات المتاحة
 */
export interface UnitDynamicFieldFilterDto {
  fieldId: string;
  fieldValue: string;
}

export interface GetAvailableUnitsQuery {
  propertyId: string;
  checkInDate: string;
  checkOutDate: string;
  guestsCount: number;
  unitTypeId?: string;
  minPrice?: number;
  maxPrice?: number;
  amenityIds?: string[];
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}

export interface UnitAvailabilityDto {
  unitId: string;
  startDate?: string;
  endDate?: string;
  unitName: string;
}

export interface UnitEditDto {
  unitId: string;
  name: string;
  basePrice: MoneyDto;
  customFeatures: Record<string, any>;
  dynamicFields: FieldGroupWithValuesDto[];
}

/**
 * طرق حساب السعر (مطابقة للباك اند)
 */
export const PricingMethod = {
  Hourly: 'Hourly',
  Daily: 'Daily',
  Weekly: 'Weekly',
  Monthly: 'Monthly',
} as const;
export type PricingMethod = keyof typeof PricingMethod;

// أنواع فرعية مستخدمة
export interface MoneyDto {
  amount: number;
  currency: string;
  formattedAmount?: string;
}


export interface FieldGroupWithValuesDto {
  /** معرف المجموعة */
  groupId: string;
  /** اسم المجموعة */
  groupName: string;
  /** الاسم المعروض للمجموعة */
  displayName: string;
  /** وصف المجموعة */
  description: string;
  /** قيم الحقول ضمن هذه المجموعة */
  fieldValues: FieldWithValueDto[];
}

/**
 * استعلام جلب توفر الوحدة ضمن نطاق زمني
 */
export interface GetUnitAvailabilityQuery {
  unitId: string;
  startDate?: string;
  endDate?: string;
}