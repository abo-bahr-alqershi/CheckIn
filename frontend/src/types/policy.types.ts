// أنواع بيانات السياسات (Policies)
// جميع الحقول موثقة بالعربي لضمان الوضوح والتوافق مع الباك اند

/**
 * بيانات سياسة الكيان الأساسية
 */
export interface PolicyDto {
  id: string;
  propertyId: string;
  policyType: PolicyType;
  description: string;
  rules: string;
}

export type PolicyType = {
  Cancellation: 0,
  CheckIn: 1,
  CheckOut: 2,
  Payment: 3,
  Smoking: 4,
  Pets: 5,
  Damage: 6,
  Other: 7,
}

/**
 * نوع بيانات تفاصيل السياسة
 */
export interface PolicyDetailsDto {
  id: string;
  propertyId: string;
  policyType: PolicyType;
  description: string;
  rules: string;
}

/**
 * أمر إنشاء سياسة جديدة للكيان
 */
export interface CreatePropertyPolicyCommand {
  /** معرف الكيان */
  propertyId: string;
  /** نوع السياسة */
  policyType: string;
  /** وصف السياسة */
  description: string;
  /** قواعد السياسة (JSON) */
  rules: string;
}

/**
 * أمر لتحديث سياسة الكيان
 */
export interface UpdatePropertyPolicyCommand {
  /** معرف السياسة */
  policyId: string;
  /** نوع السياسة (اختياري) */
  policyType?: string;
  /** وصف السياسة (اختياري) */
  description?: string;
  /** قواعد السياسة (اختياري) */
  rules?: string;
}

/**
 * أمر حذف سياسة الكيان
 */
export interface DeletePropertyPolicyCommand {
  /** معرف السياسة */
  policyId: string;
}

/**
 * استعلام الحصول على سياسة بواسطة المعرف
 */
export interface GetPolicyByIdQuery {
  /** معرف السياسة */
  policyId: string;
}

/**
 * استعلام جلب جميع سياسات لكيان معين
 */
export interface GetPropertyPoliciesQuery {
  /** معرف الكيان */
  propertyId: string;
}

/**
 * استعلام جلب السياسات حسب النوع
 */
export interface GetPoliciesByTypeQuery {
  /** نوع السياسة */
  policyType: string;
  /** رقم الصفحة */
  pageNumber?: number;
  /** حجم الصفحة */
  pageSize?: number;
}
