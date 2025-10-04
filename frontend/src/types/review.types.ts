// أنواع بيانات التقييمات (Reviews)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * صورة التقييم
 */
export interface ReviewImageDto {
  id: string;
  reviewId: string;
  name: string;
  url: string;
  sizeBytes: number;
  type: string;
  category: ImageCategory;
  caption: string;
  altText: string;
  uploadedAt: string;
}

/**
 * فئة الصورة
 */
export type ImageCategory = {
    Exterior: 0,
    Interior: 1,
    Room: 2,
    Facility: 3,
}

/**
 * رد على التقييم
 */
export interface ReviewResponseDto {
  id: string;
  reviewId: string;
  responseText: string;
  respondedBy: string;
  respondedByName: string;
  createdAt: string;
  updatedAt?: string;
}

/**
 * بيانات التقييم الأساسية
 */
export interface ReviewDto {
  id: string;
  bookingId: string;
  /** اسم الكيان */
  propertyName: string;
  /** اسم الوحدة */
  unitName?: string;
  /** اسم المستخدم الذي قام بالتقييم */
  userName: string;
  cleanliness: number;
  service: number;
  location: number;
  value: number;
  /** متوسط التقييم */
  averageRating: number;
  comment: string;
  createdAt: string;
  images: ReviewImageDto[];
  /** نص رد الإدارة */
  responseText?: string;
  /** تاريخ الرد */
  responseDate?: string;
  /** هل المراجعة معتمدة */
  isApproved: boolean;
  /** هل المراجعة بانتظار الموافقة */
  isPending: boolean;
  /** معرف من قام بالرد */
  respondedBy?: string;
  /** ردود التقييم */
  responses?: ReviewResponseDto[];
  /** مدينة الكيان */
  propertyCity?: string;
  /** عنوان الكيان */
  propertyAddress?: string;
  /** بريد العميل */
  userEmail?: string;
  /** هاتف العميل */
  userPhone?: string;
  /** تواريخ الحجز */
  bookingCheckIn?: string;
  bookingCheckOut?: string;
  guestsCount?: number;
  bookingStatus?: string;
  bookingSource?: string;
}

/**
 * أمر إنشاء تقييم جديد
 */
export interface CreateReviewCommand {
  bookingId: string;
  cleanliness: number;
  service: number;
  location: number;
  value: number;
  comment: string;
}

/**
 * استعلام جلب تقييم حسب الحجز
 */
export interface GetReviewByBookingQuery {
  bookingId: string;
}

/**
 * استعلام جلب تقييمات كيان مع التصفية والصفحات
 */
export interface GetReviewsByPropertyQuery {
  propertyId: string;
  pageNumber?: number;
  pageSize?: number;
  minRating?: number;
  maxRating?: number;
  isPendingApproval?: boolean;
  hasResponse?: boolean;
  reviewedAfter?: string;
  sortBy?: string;
}

/**
 * استعلام جلب تقييمات مستخدم مع التصفية والصفحات
 */
export interface GetReviewsByUserQuery {
  userId: string;
  pageNumber?: number;
  pageSize?: number;
  isPendingApproval?: boolean;
  hasResponse?: boolean;
  reviewedAfter?: string;
  sortBy?: string;
}

/**
 * استعلام جلب التقييمات المعلقة للموافقة
 */
export interface GetPendingReviewsQuery {}

/**
 * استعلام جلب ردود التقييم
 */
export interface GetReviewResponsesQuery {
  reviewId: string;
}

// إضافة استعلام جلب جميع التقييمات مع دعم التصفية والصفحات
/**
 * استعلام جلب جميع التقييمات مع دعم التصفية والصفحات
 */
export interface GetAllReviewsQuery {
  status?: string;
  minRating?: number;
  maxRating?: number;
  hasImages?: boolean;
  propertyId?: string;
  unitId?: string;
  userId?: string;
  startDate?: string;
  endDate?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface ApproveReviewCommand {
    reviewId: string;
    adminId: string;
}

export interface DeleteReviewCommand {
    reviewId: string;
}

export interface RespondToReviewCommand {
    reviewId: string;
    responseText: string;
    ownerId: string;
}

export interface DeleteReviewResponseCommand {
    responseId: string;
}