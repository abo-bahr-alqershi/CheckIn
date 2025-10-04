import type { MoneyDto, PaymentDto } from './payment.types';
import type { ServiceDto } from './service.types';

/**
 * حالات الحجز
 */
export type BookingStatus =
  | 'Confirmed'
  | 'Pending'
  | 'Cancelled'
  | 'Completed'
  | 'CheckedIn';

/**
 * بيانات الحجز الأساسية
 */
export interface BookingDto {
  /** معرف الحجز */
  id: string;
  /** معرف المستخدم */
  userId: string;
  /** معرف الوحدة */
  unitId: string;
  /** تاريخ الوصول */
  checkIn: string;
  /** تاريخ المغادرة */
  checkOut: string;
  /** عدد الضيوف */
  guestsCount: number;
  /** السعر الإجمالي */
  totalPrice: MoneyDto;
  /** حالة الحجز */
  status: BookingStatus;
  /** تاريخ إجراء الحجز */
  bookedAt: string;
  /** اسم المستخدم */
  userName: string;
  /** اسم الوحدة */
  unitName: string;
}
/**
 * تفاصيل الحجز (تشمل المدفوعات والخدمات)
 */
export interface BookingDetailsDto extends BookingDto {
  payments: PaymentDto[];
  services: ServiceDto[];
}

/**
 * الأمر لإنشاء حجز جديد
 */
export interface CreateBookingCommand {
  /** معرف المستخدم */
  userId: string;
  /** معرف الوحدة */
  unitId: string;
  /** تاريخ الوصول */
  checkIn: string;
  /** تاريخ المغادرة */
  checkOut: string;
  /** عدد الضيوف */
  guestsCount: number;
  /** قائمة معرفات الخدمات الإضافية (اختياري) */
  services?: string[];
}

/**
 * الأمر لتحديث بيانات الحجز
 */
export interface UpdateBookingCommand {
  /** معرف الحجز */
  bookingId: string;
  /** تاريخ الوصول المحدث (اختياري) */
  checkIn?: string;
  /** تاريخ المغادرة المحدث (اختياري) */
  checkOut?: string;
  /** عدد الضيوف المحدث (اختياري) */
  guestsCount?: number;
}

/**
 * الأمر لإلغاء الحجز
 */
export interface CancelBookingCommand {
  /** معرف الحجز */
  bookingId: string;
  /** سبب الإلغاء */
  cancellationReason: string;
}

/**
 * الأمر لتأكيد الحجز
 */
export interface ConfirmBookingCommand {
  /** معرف الحجز */
  bookingId: string;
}

/**
 * الأمر لإكمال الحجز (تسجيل الخروج)
 */
export interface CompleteBookingCommand {
  /** معرف الحجز */
  bookingId: string;
}

/**
 * الأمر لتسجيل الوصول
 */
export interface CheckInCommand {
  /** معرف الحجز */
  bookingId: string;
}

/**
 * الأمر لتسجيل المغادرة
 */
export interface CheckOutCommand {
  /** معرف الحجز */
  bookingId: string;
}

/**
 * الأمر لإضافة خدمة للحجز
 */
export interface AddServiceToBookingCommand {
  /** معرف الحجز */
  bookingId: string;
  /** معرف الخدمة */
  serviceId: string;
}

/**
 * الأمر لإزالة خدمة من الحجز
 */
export interface RemoveServiceFromBookingCommand {
  /** معرف الحجز */
  bookingId: string;
  /** معرف الخدمة */
  serviceId: string;
}

/**
 * استعلام جلب حجز بواسطة المعرف
 */
export interface GetBookingByIdQuery {
  /** معرف الحجز */
  bookingId: string;
}

/**
 * استعلام جلب الحجوزات حسب الكيان مع خيارات فلترة وتصفح
 */
export interface GetBookingsByPropertyQuery {
  /** معرف الكيان */
  propertyId: string;
  /** تاريخ البداية (اختياري) */
  startDate?: string;
  /** تاريخ النهاية (اختياري) */
  endDate?: string;
  /** رقم الصفحة (اختياري) */
  pageNumber?: number;
  /** حجم الصفحة (اختياري) */
  pageSize?: number;
  /** معرف المستخدم للفلترة (اختياري) */
  userId?: string;
  /** معرف نوع الكيان للفلترة (اختياري) */
  propertyTypeId?: string;
  /** قائمة معرفات المرافق للفلترة (اختياري) */
  amenityIds?: string[];
  /** حالة الحجز للفلترة (اختياري) */
  status?: BookingStatus;
  /** حالة الدفع للفلترة (اختياري) */
  paymentStatus?: string;
  /** بحث باسم الضيف أو البريد الإلكتروني (اختياري) */
  guestNameOrEmail?: string;
  /** مصدر الحجز (اختياري) */
  bookingSource?: string;
  /** فلترة بالحجوزات المباشرة (اختياري) */
  isWalkIn?: boolean;
  /** فلترة بالسعر الأدنى (اختياري) */
  minTotalPrice?: number;
  /** فلترة بعدد الضيوف (اختياري) */
  minGuestsCount?: number;
  /** خيارات الترتيب المتقدمة (اختياري) */
  sortBy?: string;
}

/**
 * استعلام جلب الحجوزات حسب الحالة
 */
export interface GetBookingsByStatusQuery {
  /** حالة الحجز */
  status: BookingStatus;
  /** رقم الصفحة (اختياري) */
  pageNumber?: number;
  /** حجم الصفحة (اختياري) */
  pageSize?: number;
}

/**
 * استعلام جلب الحجوزات حسب الوحدة
 */
export interface GetBookingsByUnitQuery {
  /** معرف الوحدة */
  unitId: string;
  /** تاريخ البداية (اختياري) */
  startDate?: string;
  /** تاريخ النهاية (اختياري) */
  endDate?: string;
  /** رقم الصفحة (اختياري) */
  pageNumber?: number;
  /** حجم الصفحة (اختياري) */
  pageSize?: number;
}

/**
 * استعلام جلب حجوزات المستخدم
 */
export interface GetBookingsByUserQuery {
  /** معرف المستخدم */
  userId: string;
  /** رقم الصفحة (اختياري) */
  pageNumber?: number;
  /** حجم الصفحة (اختياري) */
  pageSize?: number;
  /** حالة الحجز للفلترة (اختياري) */
  status?: BookingStatus;
  /** بحث باسم الضيف أو البريد الإلكتروني (اختياري) */
  guestNameOrEmail?: string;
  /** معرف الوحدة للفلترة (اختياري) */
  unitId?: string;
  /** مصدر الحجز (اختياري) */
  bookingSource?: string;
  /** فلترة بالحجوزات المباشرة (اختياري) */
  isWalkIn?: boolean;
  /** فلترة بالسعر الأدنى (اختياري) */
  minTotalPrice?: number;
  /** فلترة بعدد الضيوف (اختياري) */
  minGuestsCount?: number;
  /** خيارات الترتيب المتقدمة (اختياري) */
  sortBy?: string;
}

/**
 * استعلام الحجوزات في نطاق زمني
 */
export interface GetBookingsByDateRangeQuery {
  /** تاريخ البداية */
  startDate: string;
  /** تاريخ النهاية */
  endDate: string;
  /** رقم الصفحة (اختياري) */
  pageNumber?: number;
  /** حجم الصفحة (اختياري) */
  pageSize?: number;
  /** معرف المستخدم للفلترة (اختياري) */
  userId?: string;
  /** بحث باسم الضيف أو البريد الإلكتروني (اختياري) */
  guestNameOrEmail?: string;
  /** معرف الوحدة للفلترة (اختياري) */
  unitId?: string;
  /** مصدر الحجز (اختياري) */
  bookingSource?: string;
}

/**
 * استعلام جلب خدمات الحجز
 */
export interface GetBookingServicesQuery {
  /** معرف الحجز */
  bookingId: string;
}

/**
 * استعلام تقرير الحجوزات اليومية
 */
export interface GetBookingReportQuery {
  /** تاريخ البداية */
  startDate: string;
  /** تاريخ النهاية */
  endDate: string;
  /** معرف الكيان للفلترة (اختياري) */
  propertyId?: string;
}

/**
 * استعلام اتجاهات الحجوزات كسلسلة زمنية
 */
export interface GetBookingTrendsQuery {
  /** معرف الكيان (اختياري) */
  propertyId?: string;
  /** تاريخ البداية */
  startDate: string;
  /** تاريخ النهاية */
  endDate: string;
}

/**
 * استعلام تحليل نافذة الحجز لكيان معين
 */
export interface GetBookingWindowAnalysisQuery {
  /** معرف الكيان */
  propertyId: string;
}

/**
 * عنصر تقرير الحجوزات اليومية
 */
export interface BookingReportItemDto {
  date: string;
  count: number;
}

/**
 * تقرير الحجوزات اليومية
 */
export interface BookingReportDto {
  items: BookingReportItemDto[];
}

/**
 * بيانات نقطية زمنية (Time series)
 */
export interface TimeSeriesDataDto {
  /** التاريخ */
  date: string;
  /** القيمة */
  value: number;
}

/**
 * بيانات تحليل نافذة الحجز
 */
export interface BookingWindowDto {
  averageLeadTimeInDays: number;
  bookingsLastMinute: number;
}

