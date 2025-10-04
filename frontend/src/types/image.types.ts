/**
 * أنواع البيانات المتعلقة بإدارة الصور
 * Image management related data types
 */

/**
 * واجهة الصورة الأساسية
 * Basic image interface
 */
export interface Image {
  /** معرف فريد للصورة - Unique image identifier */
  id: string;
  /** رابط الصورة - Image URL */
  url: string;
  /** اسم الملف - File name */
  filename: string;
  /** حجم الملف بالبايت - File size in bytes */
  size: number;
  /** نوع الملف - File type */
  mimeType: string;
  /** الأبعاد - Dimensions */
  width: number;
  height: number;
  /** النص البديل - Alt text */
  alt?: string;
  /** تاريخ الرفع - Upload date */
  uploadedAt: string;
  /** معرف المستخدم الذي رفع الصورة - User ID who uploaded */
  uploadedBy: string;
  /** ترتيب الصورة - Image order */
  order: number;
  /** هل الصورة رئيسية - Is primary image */
  isPrimary: boolean;
  /** معرف الكيان المرتبط - Associated property ID */
  propertyId?: string;
  /** معرف الوحدة المرتبطة - Associated unit ID */
  unitId?: string;
  /** فئة الصورة - Image category */
  category: ImageCategory;
  /** العلامات - Tags */
  tags: string[];
  /** حالة المعالجة - Processing status */
  processingStatus: ProcessingStatus;
  /** الصور المختلفة الأحجام - Different size versions */
  thumbnails: ImageThumbnails;
}

/**
 * فئات الصور
 * Image categories
 */
export type ImageCategory = 
  | 'exterior' // خارجية
  | 'interior' // داخلية
  | 'amenity' // المرافق
  | 'floor_plan' // مخطط الطابق
  | 'documents' // المستندات
  | 'avatar' // الصورة الشخصية
  | 'cover' // صورة الغلاف
  | 'gallery'; // معرض عام

/**
 * حالة معالجة الصورة
 * Image processing status
 */
export type ProcessingStatus = 
  | 'uploading' // قيد الرفع
  | 'processing' // قيد المعالجة
  | 'ready' // جاهزة
  | 'failed' // فشلت
  | 'deleted'; // محذوفة

/**
 * الصور المصغرة بأحجام مختلفة
 * Thumbnails in different sizes
 */
export interface ImageThumbnails {
  /** صورة مصغرة صغيرة 150x150 - Small thumbnail */
  small: string;
  /** صورة مصغرة متوسطة 300x300 - Medium thumbnail */
  medium: string;
  /** صورة مصغرة كبيرة 600x600 - Large thumbnail */
  large: string;
  /** صورة عالية الجودة 1200x1200 - High quality */
  hd: string;
}

/**
 * طلب رفع الصورة
 * Image upload request
 */
export interface UploadImageRequest {
  /** الملف - File */
  file: File;
  /** معرف الكيان (اختياري) - Property ID (optional) */
  propertyId?: string;
  /** معرف الوحدة (اختياري) - Unit ID (optional) */
  unitId?: string;
  /** فئة الصورة - Image category */
  category: ImageCategory;
  /** النص البديل - Alt text */
  alt?: string;
  /** هل الصورة رئيسية - Is primary */
  isPrimary?: boolean;
  /** الترتيب - Order */
  order?: number;
  /** العلامات - Tags */
  tags?: string[];
}

/**
 * استجابة رفع الصورة
 * Image upload response
 */
export interface UploadImageResponse {
  /** نجح الرفع - Upload success */
  success: boolean;
  /** بيانات الصورة - Image data */
  image?: Image;
  /** رابط الصورة - Image data */
  url?: Image;
  /** رسالة الخطأ - Error message */
  error?: string;
  /** معرف المهمة لتتبع التقدم - Task ID for progress tracking */
  taskId?: string;
}

/**
 * طلب تحديث الصورة
 * Image update request
 */
export interface UpdateImageRequest {
  /** معرف الصورة - Image ID */
  id: string;
  /** النص البديل - Alt text */
  alt?: string;
  /** هل الصورة رئيسية - Is primary */
  isPrimary?: boolean;
  /** الترتيب - Order */
  order?: number;
  /** العلامات - Tags */
  tags?: string[];
  /** فئة الصورة - Image category */
  category?: ImageCategory;
}

/**
 * طلب الحصول على الصور
 * Get images request
 */
export interface GetImagesRequest {
  /** معرف الكيان - Property ID */
  propertyId?: string;
  /** معرف الوحدة - Unit ID */
  unitId?: string;
  /** فئة الصورة - Image category */
  category?: ImageCategory;
  /** الصفحة - Page */
  page?: number;
  /** عدد العناصر في الصفحة - Items per page */
  limit?: number;
  /** ترتيب النتائج - Sort order */
  sortBy?: 'uploadedAt' | 'order' | 'filename';
  /** اتجاه الترتيب - Sort direction */
  sortOrder?: 'asc' | 'desc';
  /** البحث في الاسم أو العلامات - Search in name or tags */
  search?: string;
}

/**
 * استجابة الحصول على الصور
 * Get images response
 */
export interface GetImagesResponse {
  /** قائمة الصور - Images list */
  images: Image[];
  /** العدد الكلي - Total count */
  total: number;
  /** الصفحة الحالية - Current page */
  page: number;
  /** العدد في الصفحة - Items per page */
  limit: number;
  /** عدد الصفحات الكلي - Total pages */
  totalPages: number;
}

/**
 * طلب حذف الصور
 * Delete images request
 */
export interface DeleteImagesRequest {
  /** معرفات الصور - Image IDs */
  imageIds: string[];
  /** حذف نهائي أم مؤقت - Permanent or soft delete */
  permanent?: boolean;
}

/**
 * طلب إعادة ترتيب الصور
 * Reorder images request
 */
export interface ReorderImagesRequest {
  /** قائمة معرفات الصور بالترتيب الجديد - Image IDs in new order */
  imageIds: string[];
  /** معرف الكيان - Property ID */
  propertyId?: string;
  /** معرف الوحدة - Unit ID */
  unitId?: string;
}

/**
 * إعدادات معالجة الصورة
 * Image processing settings
 */
export interface ImageProcessingSettings {
  /** جودة الضغط (1-100) - Compression quality */
  quality: number;
  /** الحد الأقصى للعرض - Max width */
  maxWidth: number;
  /** الحد الأقصى للارتفاع - Max height */
  maxHeight: number;
  /** تنسيق الإخراج - Output format */
  format: 'jpeg' | 'png' | 'webp';
  /** إنشاء صور مصغرة - Generate thumbnails */
  generateThumbnails: boolean;
  /** إزالة بيانات EXIF - Remove EXIF data */
  stripExif: boolean;
}

/**
 * إحصائيات الصور
 * Image statistics
 */
export interface ImageStatistics {
  /** العدد الكلي للصور - Total images count */
  totalImages: number;
  /** الحجم الكلي بالبايت - Total size in bytes */
  totalSize: number;
  /** عدد الصور حسب الفئة - Images count by category */
  byCategory: Record<ImageCategory, number>;
  /** عدد الصور حسب الحالة - Images count by status */
  byStatus: Record<ProcessingStatus, number>;
  /** متوسط حجم الصورة - Average image size */
  averageSize: number;
  /** الصور الرئيسية - Primary images count */
  primaryImages: number;
}

/**
 * تقدم رفع الصورة
 * Image upload progress
 */
export interface UploadProgress {
  /** معرف المهمة - Task ID */
  taskId: string;
  /** اسم الملف - Filename */
  filename: string;
  /** النسبة المئوية للتقدم - Progress percentage */
  progress: number;
  /** الحالة - Status */
  status: 'uploading' | 'processing' | 'completed' | 'failed';
  /** رسالة الخطأ - Error message */
  error?: string;
  /** معرف الصورة (عند الانتهاء) - Image ID (when completed) */
  imageId?: string;
}

/**
 * فلتر البحث المتقدم
 * Advanced search filter
 */
export interface ImageSearchFilter {
  /** تاريخ البداية - Start date */
  dateFrom?: string;
  /** تاريخ النهاية - End date */
  dateTo?: string;
  /** الحد الأدنى للحجم - Min size */
  minSize?: number;
  /** الحد الأقصى للحجم - Max size */
  maxSize?: number;
  /** الحد الأدنى للعرض - Min width */
  minWidth?: number;
  /** الحد الأقصى للعرض - Max width */
  maxWidth?: number;
  /** الحد الأدنى للارتفاع - Min height */
  minHeight?: number;
  /** الحد الأقصى للارتفاع - Max height */
  maxHeight?: number;
  /** نوع الملف - File type */
  mimeTypes?: string[];
  /** المستخدم الذي رفع - Uploaded by user */
  uploadedBy?: string[];
  /** العلامات - Tags */
  tags?: string[];
  /** الصور الرئيسية فقط - Primary images only */
  primaryOnly?: boolean;
}