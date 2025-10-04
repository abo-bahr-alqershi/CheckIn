// أنواع البيانات الخاصة بالمرافق (Amenities)

export interface AmenityDto {
  id: string;
  name: string;
  description: string;
  icon: '',
}

export interface CreateAmenityCommand {
  name: string;
  description: string;
  icon: string;
}

export interface UpdateAmenityCommand {
  amenityId: string;
  name?: string;
  description?: string;
  icon?: string;
}

export interface DeleteAmenityCommand {
  amenityId: string;
}

export interface AssignAmenityToPropertyCommand {
  propertyId: string;
  amenityId: string;
}

export interface AssignAmenityToPropertyTypeCommand {
  propertyTypeId: string;
  amenityId: string;
  isDefault: boolean;
}

export interface UpdatePropertyAmenityCommand {
  propertyId: string;
  amenityId: string;
  isAvailable: boolean;
  extraCost: MoneyDto;
  description?: string;  // Added for full backend compatibility
}

/**
 * DTO للمبالغ المالية (مطابقة للباك اند)
 */
export interface MoneyDto {
  amount: number;
  currency: string;
  formattedAmount?: string;
}

export interface PropertyAmenityDto {
  amenityId: string;
  isAvailable: boolean;
  extraCost?: number;
  description?: string;
}


/**
 * استعلام جلب جميع المرافق مع صفحات
 */
export interface GetAllAmenitiesQuery {
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
  /** مصطلح البحث */
  searchTerm?: string;
  /** Filter by related property ID */
  propertyId?: string;
  /** Filter amenities assigned to property or globally */
  isAssigned?: boolean;
  /** Filter amenities free (extra cost == 0) */
  isFree?: boolean;
}

/**
 * استعلام جلب مرافق كيان معين
 */
export interface GetAmenitiesByPropertyQuery {
  /** معرف الكيان */
  propertyId: string;
}

/**
 * استعلام جلب مرافق حسب نوع الكيان
 */
export interface GetAmenitiesByPropertyTypeQuery {
  /** معرف نوع الكيان */
  propertyTypeId: string;
}
