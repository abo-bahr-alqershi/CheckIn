/**
 * سجل عمليات البحث
 */
export interface SearchLogDto {
  id: string;
  userId: string;
  searchType: string;
  criteriaJson: string;
  resultCount: number;
  pageNumber: number;
  pageSize: number;
  createdAt: string;
}

/**
 * استعلام لجلب سجلات البحث
 */
export interface GetSearchLogsQuery {
  from?: string;
  to?: string;
  userId?: string;
  searchType?: string;
  pageNumber?: number;
  pageSize?: number;
} 