import type { UnitDynamicFieldFilterDto } from "./unit.types";

export interface SearchUnitsQuery {
  location?: string;
  minPrice?: number;
  maxPrice?: number;
  propertyId?: string;
  unitTypeId?: string;
  checkInDate?: string;
  checkOutDate?: string;
  numberOfGuests?: number;
  isAvailable?: boolean;
  hasActiveBookings?: boolean;
  pageNumber?: number;
  pageSize?: number;
  dynamicFieldFilters?: UnitDynamicFieldFilterDto[];
  latitude?: number;
  longitude?: number;
  radiusKm?: number;
  sortBy?: string;
}