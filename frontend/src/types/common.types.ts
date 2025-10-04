export interface ResultDto<T> {
  success: boolean;
  message?: string;
  data: T;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  previousPageNumber?: number;
  nextPageNumber?: number;
  startIndex: number;
  endIndex: number;
  metadata?: any;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  itemCount: number;
  firstItemNumber: number;
  lastItemNumber: number;
}

export interface PaginationDto {
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  ascending: boolean;
  skip: number;
}

export interface BaseDto {
  id: string;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

export interface MoneyDto {
  amount: number;
  currency: string;
  formattedAmount: string;
}