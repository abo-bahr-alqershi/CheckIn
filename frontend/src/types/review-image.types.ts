// أنواع بيانات صور التقييمات (Review Images)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند
import type { ImageCategory } from './property-image.types';

/**
 * بيانات صورة التقييم
 */
export interface ReviewImageDto {
  /** معرف الصورة */
  id: string;
  /** معرف التقييم */
  reviewId: string;
  /** اسم الملف */
  name: string;
  /** رابط الصورة */
  url: string;
  /** حجم الصورة بالبايت */
  sizeBytes: number;
  /** نوع المحتوى */
  type: string;
  /** فئة الصورة */
  category: ImageCategory;
  /** تعليق توضيحي للصورة */
  caption: string;
  /** نص بديل للصورة */
  altText: string;
  /** تاريخ الرفع */
  uploadedAt: string;
}
