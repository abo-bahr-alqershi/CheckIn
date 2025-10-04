// أنواع بيانات قيم الحقول للوحدات (Unit Field Values)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

import type { UnitTypeFieldDto } from "./unit-type-field.types";

/**
 * بيانات قيمة حقل للوحدة
 */
export interface UnitFieldValueDto {
  valueId: string;
  unitId: string;
  fieldId: string;
  fieldName: string;
  displayName: string;
  fieldValue: string;
  field: UnitTypeFieldDto;
  createdAt: string;
  updatedAt: string;
}

/**
 * أمر إنشاء قيمة حقل للوحدة
 */
export interface CreateUnitFieldValueCommand {
  unitId: string;
  fieldId: string;
  fieldValue: string;
}

/**
 * أمر تحديث قيمة حقل للوحدة
 */
export interface UpdateUnitFieldValueCommand {
  valueId: string;
  fieldValue: string;
}

/**
 * أمر حذف قيمة حقل للوحدة
 */
export interface DeleteUnitFieldValueCommand {
  valueId: string;
}

/**
 * استعلام جلب قيمة حقل للوحدة حسب المعرف
 */
export interface GetUnitFieldValueByIdQuery {
  valueId: string;
}

/**
 * استعلام جلب جميع قيم الحقول لوحدة معينة
 */
export interface GetUnitFieldValuesQuery {
  unitId: string;
  isPublic?: boolean;
}

/**
 * واجهة لقيمة الحقل الأساسية
 */
export interface FieldValueDto {
  fieldId: string;
  fieldValue: string;
}

/**
 * أمر تحديث متعدد لقيم الحقول
 */
export interface BulkUpdateUnitFieldValueCommand {
  /** معرف الوحدة */
  unitId: string;
  /** قائمة قيم الحقول */
  fieldValues: FieldValueDto[];
}

/**
 * استعلام جلب قيم الحقول مجمعة حسب المجموعات
 */
export interface GetUnitFieldValuesGroupedQuery {
  /** معرف الوحدة */
  unitId: string;
  /** عام فقط (اختياري) */
  isPublic?: boolean;
}

/**
 * واجهة لحقل مع قيمته
 */
export interface FieldWithValueDto {
  valueId: string;
  fieldId: string;
  fieldName: string;
  displayName: string;
  value: string;
}

/**
 * مجموعة الحقول مع قيمها
 */
export interface FieldGroupWithValuesDto {
  groupId: string;
  groupName: string;
  displayName: string;
  description: string;
  fieldValues: FieldWithValueDto[];
}

/**
 * أمر إنشاء قيم حقول للوحدة بشكل مجمع
 */
export interface BulkCreateUnitFieldValueCommand {
  unitId: string;
  fieldValues: FieldValueDto[];
}

/**
 * أمر حذف قيم حقول للوحدة بشكل مجمع
 */
export interface BulkDeleteUnitFieldValueCommand {
  valueIds: string[];
}