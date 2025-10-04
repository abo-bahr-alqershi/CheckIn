// أنواع بيانات المدفوعات (Payments)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * بيانات المبلغ المالي
 */
export interface MoneyDto {
  /** المبلغ المالي */
  amount: number;
  /** رمز العملة */
  currency: string;
  /** المبلغ المنسق للعرض */
  formattedAmount: string;
}


/**
 * حالات الدفع
 * (تم التحويل إلى نوع union string لضمان التوافق مع إعدادات TypeScript الحديثة)
 */
export type PaymentStatus =
  | 'Successful'
  | 'Failed'
  | 'Pending'
  | 'Refunded'
  | 'Voided'
  | 'PartiallyRefunded';

/**
 * بيانات الدفعة الأساسية
 */
export interface PaymentDto {
  id: string;
  bookingId: string;
  amount: MoneyDto;
  transactionId: string;
  method: PaymentMethod;
  status: PaymentStatus;
  paymentDate: string;
}

export type PaymentMethod = {
  CreditCard: 0,
  PayPal: 1,
  BankTransfer: 2,
  Cash: 3,
  Other: 4,
}

/**
 * أمر لاسترداد الدفعة
 */
export interface RefundPaymentCommand {
  /** معرف الدفعة */
  paymentId: string;
  /** المبلغ المسترد */
  refundAmount: MoneyDto;
  /** سبب الاسترداد */
  refundReason: string;
}

/**
 * أمر لإبطال الدفعة
 */
export interface VoidPaymentCommand {
  /** معرف الدفعة */
  paymentId: string;
}

/**
 * أمر لتحديث حالة الدفعة
 */
export interface UpdatePaymentStatusCommand {
  /** معرف الدفعة */
  paymentId: string;
  /** الحالة الجديدة للدفع */
  newStatus: PaymentStatus;
}

/**
 * أمر لمعالجة الدفع
 */
export interface ProcessPaymentCommand {
  /** معرف الحجز */
  bookingId: string;
  /** مبلغ الدفع */
  amount: MoneyDto;
  /** طريقة الدفع */
  method: PaymentMethod;
}

/**
 * استعلام جلب دفعة بواسطة المعرف
 */
export interface GetPaymentByIdQuery {
  /** معرف الدفعة */
  paymentId: string;
}

/**
 * استعلام جلب المدفوعات حسب الحجز
 */
export interface GetPaymentsByBookingQuery {
  /** معرف الحجز */
  bookingId: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام جلب المدفوعات حسب الحالة
 */
export interface GetPaymentsByStatusQuery {
  /** حالة الدفع */
  status: PaymentStatus;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام جلب المدفوعات حسب المستخدم
 */
export interface GetPaymentsByUserQuery {
  /** معرف المستخدم */
  userId: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام جلب المدفوعات حسب طريقة الدفع
 */
export interface GetPaymentsByMethodQuery {
  /** طريقة الدفع */
  paymentMethod: PaymentMethod;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}

/**
 * استعلام جلب جميع المدفوعات مع دعم الفلاتر
 */
export interface GetAllPaymentsQuery {
  status?: string;
  method?: string;
  bookingId?: string;
  userId?: string;
  propertyId?: string;
  unitId?: string;
  minAmount?: number;
  maxAmount?: number;
  startDate?: string;
  endDate?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface PaymentDetailsDto {
  payment: PaymentDto;
}
