// أنواع بيانات الكيانات (Properties)

import type { UnitDto } from './unit.types';
import type { AmenityDto } from './amenity.types';
import type { UnitTypeFieldDto } from './unit-type-field.types';
import type { PropertyImageDto } from './property-image.types';

export interface PropertyDto {
  id: string;
  ownerId: string;
  typeId: string;
  name: string;
  address: string;
  city: string;
  latitude: number;
  longitude: number;
  starRating: number;
  description: string;
  isApproved: boolean;
  createdAt: string; // ISO date
  /** تفاصيل صور الكيان */
  images?: PropertyImageDto[];
  ownerName: string;
  typeName: string;
  distanceKm?: number;
}

export interface CreatePropertyCommand {
  name: string;
  address: string;
  propertyTypeId: string;
  ownerId: string;
  description: string;
  latitude: number;
  longitude: number;
  city: string;
  starRating: number;
  // /** URLs of uploaded images */
  images?: string[];
}

export interface UpdatePropertyCommand {
  propertyId: string;
  name?: string;
  address?: string;
  description?: string;
  latitude?: number;
  longitude?: number;
  city?: string;
  starRating?: number;
  // /** URLs of uploaded images */
  images?: string[];
}

export interface DeletePropertyCommand {
  propertyId: string;
}

export interface GetAllPropertiesQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  propertyTypeId?: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  isAscending?: boolean;
  amenityIds?: string[];
  starRatings?: number[];
  minAverageRating?: number;
  isApproved?: boolean;
  hasActiveBookings?: boolean;
}

/**
 * استعلام للحصول على بيانات كيان بواسطة المعرف
 */
export interface GetPropertyByIdQuery {
  /** معرف الكيان */
  propertyId: string;
}

/**
 * استعلام للحصول على الكيانات في انتظار الموافقة
 */
export interface GetPendingPropertiesQuery {
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام جلب تفاصيل الكيان مع خيارات الوحدات والحقول الديناميكية
 */
export interface GetPropertyDetailsQuery {
  /** معرف الكيان */
  propertyId: string;
  /** تضمين الوحدات الفرعية */
  includeUnits?: boolean;
  /** تضمين القيم الديناميكية */
}

/**
 * استعلام جلب بيانات الكيان للتحرير
 */
export interface GetPropertyForEditQuery {
  /** معرف الكيان */
  propertyId: string;
  /** معرف المالك */
  ownerId: string;
}

/**
 * استعلام لجلب مرافق الكيان
 */
export interface GetPropertyAmenitiesQuery {
  /** معرف الكيان */
  propertyId: string;
}

/**
 * استعلام لجلب الكيانات حسب المدينة
 */
export interface GetPropertiesByCityQuery {
  /** اسم المدينة */
  cityName: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام لجلب كيانات المالك
 */
export interface GetPropertiesByOwnerQuery {
  /** معرف المالك */
  ownerId: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام لجلب الكيانات حسب النوع
 */
export interface GetPropertiesByTypeQuery {
  /** معرف نوع الكيان */
  propertyTypeId: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام لإحصائيات تقييم الكيان
 */
export interface GetPropertyRatingStatsQuery {
  /** معرف الكيان */
  propertyId: string;
}

/**
 * بيانات إحصائيات تقييم الكيان
 */
export interface DestinationDto {
  city: string;
  viewCount: number;
}

export interface PropertyAmenityDto {
  amenityId: string;
  isAvailable: boolean;
  extraCost?: number;
  description?: string;
}

export interface PropertyPolicyDto {
  policyType: string;
  policyContent: string;
  isActive: boolean;
}

export interface UnitDynamicFieldFilterDto {
  fieldId: string;
  fieldValue: string;
}

export interface UnitDynamicFieldValueDto {
  fieldId: string;
  fieldValue: string;
  isPublic: boolean;
}

export interface PropertyRatingStatsDto {
  averageRating: number;
  totalReviews: number;
}

// إضافة DTO لتفاصيل الكيان الكامل
export interface PropertyDetailsDto extends PropertyDto {
  /** قائمة الوحدات التابعة للكيان */
  units: UnitDto[];
  /** قائمة المرافق التابعة للكيان */
  amenities: AmenityDto[];
}

// إضافة DTO لبيانات تحرير الكيان
export interface PropertyEditDto {
  /** معرف الكيان */
  propertyId: string;
  /** اسم الكيان */
  name: string;
  /** العنوان الكامل */
  address: string;
  /** المدينة */
  city: string;
  /** خط العرض */
  latitude?: number;
  /** خط الطول */
  longitude?: number;
  /** تقييم النجوم */
  starRating?: number;
  /** وصف الكيان */
  description: string;
  /** معرف نوع الكيان */
  propertyTypeId: string;
  /** الحقول الديناميكية مع قيمها */
}

// إضافة تعريف مجموعة الحقول مع تفاصيل الحقول
export interface FieldGroupWithFieldsDto {
  /** معرف المجموعة */
  groupId: string;
  /** اسم المجموعة */
  groupName: string;
  /** الاسم المعروض للمجموعة */
  displayName: string;
  /** وصف المجموعة */
  description: string;
  /** ترتيب العرض */
  sortOrder: number;
  /** هل المجموعة قابلة للطي */
  isCollapsible: boolean;
  /** هل تكون الموسعة افتراضياً */
  isExpandedByDefault: boolean;
  /** الحقول المرتبطة بالمجموعة */
  fields: UnitTypeFieldDto[];
}
