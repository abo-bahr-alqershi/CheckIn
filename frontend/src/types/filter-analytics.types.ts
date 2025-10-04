// أنواع بيانات احصائيات فلاتر البحث الديناميكية للوحدات
// Types for dynamic filter analytics for unit searches

/**
 * عدد استخدام قيمة فلتر
 */
export interface FilterValueCountDto {
  filterValue: string;
  count: number;
}

/**
 * احصائيات فلتر ديناميكي
 */
export interface FieldFilterAnalyticsDto {
  fieldId: string;
  valueCounts: FilterValueCountDto[];
}

/**
 * استعلام لجلب احصائيات الفلاتر الديناميكية للوحدات
 */
export interface GetUnitDynamicFilterAnalyticsQuery {
  from?: string;
  to?: string;
} 