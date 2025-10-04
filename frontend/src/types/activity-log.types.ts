// أنواع بيانات سجل نشاط (Activity Log)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * بيانات سجل نشاط (قد تختلف عن AuditLogDto في بعض الحقول)
 */
export interface ActivityLogDto {
  /** المعرف الفريد للنشاط */
  id: string;
  /** نوع النشاط (مثال: تسجيل دخول، تعديل، حذف، ...إلخ) */
  activityType: string;
  /** وصف النشاط */
  description: string;
  /** معرف المستخدم */
  userId: string;
  /** اسم المستخدم */
  userName: string;
  /** تاريخ ووقت النشاط */
  timestamp: string;
  /** بيانات إضافية للنشاط (اختياري) */
  details?: string;
}