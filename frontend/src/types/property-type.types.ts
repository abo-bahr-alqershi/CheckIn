
export interface CreatePropertyTypeCommand {
    name: string;
    description: string;
    defaultAmenities: string;
    icon: string;
}

export interface UpdatePropertyTypeCommand {
    propertyTypeId: string;
    name: string;
    description: string;
    defaultAmenities: string;
    icon: string;
}

export interface DeletePropertyTypeCommand {
    propertyTypeId: string;
}

export interface PropertyTypeDto {
    id: string;
    name: string;
    description: string;
    defaultAmenities: string;
    icon: string;
}

/**
 * استعلام جلب جميع أنواع الكيانات مع الترتيب والصفحة
 */
export interface GetAllPropertyTypesQuery {
    pageNumber: number;
    pageSize: number;
}

/**
 * استعلام جلب نوع كيان بالمعرف
 */
export interface GetPropertyTypeByIdQuery {
    propertyTypeId: string;
}
