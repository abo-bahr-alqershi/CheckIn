// أنواع بيانات صور الكيانات (Property Images)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * فئة الصورة (مطابقة للـ enum في الباك اند)
 */
export type ImageCategory = 'Exterior' | 'Interior' | 'Room' | 'Facility';

/**
 * حالة الصورة (مطابقة للـ enum في الباك اند)
 */
export type ImageStatus = 'Pending' | 'Approved' | 'Rejected';

/**
 * بيانات صورة الكيان
 */
export interface PropertyImageDto {
  /** معرف الصورة */
  id: string;
  /** معرف الكيان (إن وجد) */
  propertyId?: string;
  /** معرف الوحدة (إن وجد) */
  unitId?: string;
  /** اسم الملف */
  name: string;
  /** مسار الصورة */
  url: string;
  /** حجم الملف بالبايت */
  sizeBytes: number;
  /** نوع المحتوى */
  type: string;
  /** فئة الصورة */
  category: ImageCategory;
  /** تعليق توضيحي للصورة */
  caption: string;
  /** نص بديل للصورة */
  altText: string;
  /** وسوم الصورة (JSON) */
  tags: string;

  /** أحجام الصورة (JSON) */
  sizes: string;
  /** هل هي الصورة الرئيسية */
  isMain: boolean;
  /** ترتيب العرض */
  displayOrder: number;
  /** تاريخ الرفع */
  uploadedAt: string;
  /** حالة الصورة */
  status: ImageStatus;
}

/**
 * أمر إنشاء صورة جديدة للكيان أو الوحدة
 */
export interface CreatePropertyImageCommand {
  /** معرف الكيان (اختياري) */
  propertyId?: string;
  /** معرف الوحدة (اختياري) */
  unitId?: string;
  /** رابط الملف أو المسار */
  url: string;
  /** تعليق توضيحي للصورة */
  caption: string;
  /** نص بديل للصورة */
  altText: string;
  /** فئة الصورة */
  category: ImageCategory;
  /** هل هذه الصورة الرئيسية */
  isMain: boolean;
}

/**
 * أمر لتحديث بيانات صورة موجودة
 */
export interface UpdatePropertyImageCommand {
  /** معرف الصورة */
  imageId: string;
  /** رابط الملف أو المسار (اختياري) */
  url?: string;
  /** تعليق توضيحي جديد (اختياري) */
  caption?: string;
  /** نص بديل جديد (اختياري) */
  altText?: string;
  /** فئة الصورة (اختياري) */
  category?: ImageCategory;
  /** تعيين كصورة رئيسية (اختياري) */
  isMain?: boolean;
}

/**
 * أمر لحذف صورة من المعرض
 */
export interface DeletePropertyImageCommand {
  /** معرف الصورة المراد حذفها */
  imageId: string;
}

/**
 * أمر لتعيين صورة إلى كيان
 */
export interface AssignPropertyImageToPropertyCommand {
  /** معرف الصورة */
  imageId: string;
  /** معرف الكيان */
  propertyId: string;
  /** تعيين كصورة رئيسية */
  setAsMain?: boolean;
}

/**
 * أمر لتعيين صورة إلى وحدة
 */
export interface AssignPropertyImageToUnitCommand {
  /** معرف الصورة */
  imageId: string;
  /** معرف الوحدة */
  unitId: string;
  /** تعيين كصورة رئيسية */
  setAsMain?: boolean;
}

/**
 * أمر لتعيين صور متعددة لكيانات
 */
export interface BulkAssignImageToPropertyCommand {
  /** قائمة التعيينات: معرف الكيان ومعرف الصورة */
  assignments: Array<{ propertyId: string; imageId: string }>;
}

/**
 * أمر لتعيين صور متعددة لوحدات
 */
export interface BulkAssignImageToUnitCommand {
  /** قائمة التعيينات: معرف الوحدة ومعرف الصورة */
  assignments: Array<{ unitId: string; imageId: string }>;
}

/**
 * أمر لإعادة ترتيب الصور
 * Command to reorder property images display order
 */
export interface ReorderPropertyImagesCommand {
  /** قائمة الترتيبات: معرف الصورة وترتيب العرض */
  assignments: Array<{ imageId: string; displayOrder: number }>;
}

/**
 * استعلام جلب صور الكيان/الوحدة مع الترقيم
 */
export interface GetPropertyImagesQuery {
  /** معرف الكيان (اختياري) */
  propertyId?: string;
  /** معرف الوحدة (اختياري) */
  unitId?: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * إحصائيات صور الكيان
 */
export interface PropertyImageStatsDto {
  /** معرف الكيان */
  propertyId: string;
  /** إجمالي عدد الصور */
  totalImages: number;
  /** عدد الصور المعلقة */
  pendingCount: number;
  /** عدد الصور الموافق عليها */
  approvedCount: number;
  /** عدد الصور المرفوضة */
  rejectedCount: number;
}
