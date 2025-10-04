/**
 * إحصائيات تحليلات البحث
 */
export interface SearchCountByDayDto {
  date: string;
  count: number;
}

export interface SearchAnalyticsDto {
  totalSearches: number;
  propertySearches: number;
  unitSearches: number;
  searchesByDay: SearchCountByDayDto[];
}

/**
 * استعلام لجلب تحليلات البحث
 */
export interface GetSearchAnalyticsQuery {
  from?: string;
  to?: string;
} 