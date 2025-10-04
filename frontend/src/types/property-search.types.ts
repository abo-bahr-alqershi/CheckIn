export interface PriceRangeDto {
  minPrice: number;
  maxPrice: number;
  averagePrice: number;
}

export interface PropertySearchItemDto {
  id: string;
  name: string;
  description?: string;
  city: string;
  address: string;
  starRating: number;
  averageRating?: number;
  reviewCount: number;
  minPrice: number;
  currency: string;
  mainImageUrl?: string;
  imageUrls: string[];
  amenities: string[];
  propertyType: string;
  distanceKm?: number;
  isAvailable: boolean;
  availableUnitsCount: number;
  maxCapacity: number;
  isFeatured: boolean;
  lastUpdated: string;
}

export interface PropertySearchResultDto {
  properties: PropertySearchItemDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  statistics?: SearchStatisticsDto;
}

export interface SearchFilterCriteriaDto {
  fieldId: string;
  filterId: string;
  filterType: string;
  filterValue: any;
  filterOptions?: Record<string, any>;
}

export interface SearchStatisticsDto {
  searchDurationMs: number;
  appliedFiltersCount: number;
  totalResultsBeforePaging: number;
  suggestions: string[];
  priceRange?: PriceRangeDto;
}

/**
 * استعلام بحث عن الكيانات بناءً على معايير متعددة
 */
export interface SearchPropertiesQuery {
    location?: string;
    minPrice?: number;
    maxPrice?: number;
    propertyTypeId?: string;
    checkInDate?: string;
    checkOutDate?: string;
    numberOfGuests?: number;
    pageNumber: number;
    pageSize: number;
    amenityIds?: string[];
    starRatings?: number[];
    minAverageRating?: number;
    isApproved?: boolean;
    hasActiveBookings?: boolean;
    latitude?: number;
    longitude?: number;
    radiusKm?: number;
    sortBy?: string;
}

/**
 * أمر بحث عن الكيانات بناءً على معايير متعددة
 */
export interface SearchPropertiesCommand {
    city?: string;
    checkIn?: string;
    checkOut?: string;
    guestCount?: number;
    propertyTypeId?: string;
    pageNumber: number;
    pageSize: number;
    sortBy?: string;
    sortDirection: string;
    minPrice?: number;
    maxPrice?: number;
    amenityIds?: string[];
    starRatings?: number[];
    minAverageRating?: number;
    searchTerm?: string;
    latitude?: number;
    longitude?: number;
    radiusKm?: number;
}
