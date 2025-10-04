// أنواع بيانات أنواع الوحدات (Unit Types)
// جميع الحقول موثقة بالعربي لضمان الوضوح والتوافق مع الباك اند

import type { UnitTypeFieldDto } from "./unit-type-field.types";

/**
 * بيانات نوع الوحدة الأساسية
 */
export interface UnitTypeDto {
  id: string;
  propertyTypeId: string;
  name: string;
  description: string;
  defaultPricingRules: string;
  fieldGroups: FieldGroupDto[];
  filters: SearchFilterDto[];
  icon: string;
  isHasAdults: boolean;
  isHasChildren: boolean;
  isMultiDays: boolean;
  isRequiredToDetermineTheHour: boolean;
}

/**
 * أمر إنشاء نوع وحدة جديد
 */
export interface CreateUnitTypeCommand {
  propertyTypeId: string;
  name: string;
  maxCapacity: number;
  icon: string;
  isHasAdults: boolean;
  isHasChildren: boolean;
  isMultiDays: boolean; 
  isRequiredToDetermineTheHour: boolean;
}

/**
 * أمر تحديث نوع الوحدة
 */
export interface UpdateUnitTypeCommand {
  unitTypeId: string;
  name: string;
  maxCapacity: number;
  icon: string;
  isHasAdults: boolean;
  isHasChildren: boolean;
  isMultiDays: boolean;
  isRequiredToDetermineTheHour: boolean;
}

/**
 * أمر حذف نوع وحدة
 */
export interface DeleteUnitTypeCommand {
  unitTypeId: string;
}

/**
 * استعلام جلب نوع وحدة بواسطة المعرف
 */
export interface GetUnitTypeByIdQuery {
  unitTypeId: string;
}

/**
 * استعلام جلب أنواع الوحدات لنوع كيان معين
 */
export interface GetUnitTypesByPropertyTypeQuery {
  /** معرف نوع الكيان */
  propertyTypeId: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * مجموعة حقول نوع الوحدة
 */
export interface FieldGroupDto {
  /** معرف المجموعة */
  groupId: string;
  /** معرف نوع الكيان */
  propertyTypeId: string;
  /** اسم المجموعة */
  groupName: string;
  /** الاسم المعروض للمجموعة */
  displayName: string;
  /** وصف المجموعة */
  description: string;
  /** ترتيب المجموعة */
  sortOrder: number;
  /** قابلية الطي للمجموعة */
  isCollapsible: boolean;
  /** حالة التوسع الافتراضي */
  isExpandedByDefault: boolean;
  /** حقول المجموعة */
  fields: UnitTypeFieldDto[];
}


/**
 * فلتر البحث الديناميكي
 */
export interface SearchFilterDto {
  /** معرف الفلتر */
  filterId: string;
  /** معرف الحقل */
  fieldId: string;
  /** نوع الفلتر */
  filterType: string;
  /** الاسم المعروض */
  displayName: string;
  /** خيارات الفلتر (JSON) */
  filterOptions: Record<string, any>;
  /** حالة التفعيل */
  isActive: boolean;
  /** ترتيب الفلتر */
  sortOrder: number;
  /** معلومات الحقل الديناميكي المرتبط */
  field: UnitTypeFieldDto;
}
