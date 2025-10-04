// أنواع بيانات المصادقة وتسجيل الدخول (Auth)
// جميع الحقول موثقة بالعربي لضمان التوافق التام مع الباك اند

/**
 * أمر تسجيل الدخول
 */
export interface LoginCommand {
  email: string;
  password: string;
  rememberMe?: boolean;
}

/**
 * نتيجة المصادقة (رموز الوصول والتحديث)
 */
export interface AuthResultDto {
  /** رمز الوصول */
  accessToken: string;
  /** رمز التحديث */
  refreshToken: string;
  /** تاريخ انتهاء صلاحية رمز الوصول */
  expiresAt: string;
  /** معرف المستخدم */
  userId: string;
  /** اسم المستخدم */
  userName: string;
  /** البريد الإلكتروني للمستخدم */
  email: string;
  /** دور المستخدم */
  role: string;
  /** رابط صورة الملف الشخصي للمستخدم */
  profileImage: string;
  /** معرف الكيان إذا كان مالكًا أو موظفًا */
  propertyId?: string;
  /** اسم الكيان إذا كان مالكًا أو موظفًا */
  propertyName?: string;
  /** إعدادات المستخدم بصيغة JSON */
  settingsJson: string;
  /** معرف الموظف إذا كان موظفًا */
  staffId?: string;
  /** قائمة المفضلة للمستخدم بصيغة JSON */
  favoritesJson: string;
}

/**
 * أمر لتأكيد البريد الإلكتروني باستخدام رمز
 * Command to verify email using token
 */
export interface VerifyEmailCommand {
  token: string;
}

export interface ResetPasswordCommand {
  token: string;
  newPassword: string;
}

export interface ResendPasswordResetLinkCommand {
  email: string;
}

export interface ResendEmailVerificationLinkCommand {
  email: string;
}

export interface ChangePasswordCommand {
    userId: string;
    currentPassword: string;
    newPassword: string;
}

export interface RefreshTokenCommand {
    refreshToken: string;
}

export interface ForgotPasswordCommand {
    email: string;
}