import { apiClient } from './api.service';
// import { apiClient } from './api.service';
import type {
  LoginCommand,
  AuthResultDto,
  RefreshTokenCommand,
  ForgotPasswordCommand,
  ResetPasswordCommand,
  ResendEmailVerificationLinkCommand,
  ResendPasswordResetLinkCommand,
  VerifyEmailCommand,
  ChangePasswordCommand,
} from '../types/auth.types';
import type { ResultDto } from '../types/common.types';

// المسار الأساسي لتعاملات المصادقة
const API_BASE = '/api/common/auth';

export const CommonAuthService = {
  // تسجيل الدخول
  login: (data: LoginCommand) =>
    apiClient.post<ResultDto<AuthResultDto>>(`${API_BASE}/login`, data).then(res => res.data),

  // تحديث رمز المصادقة
  refreshToken: (data: RefreshTokenCommand) =>
    apiClient.post<ResultDto<AuthResultDto>>(`${API_BASE}/refresh-token`, data).then(res => res.data),

  // طلب إعادة تعيين كلمة المرور
  forgotPassword: (data: ForgotPasswordCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/forgot-password`, data).then(res => res.data),

  // إعادة تعيين كلمة المرور
  resetPassword: (data: ResetPasswordCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/reset-password`, data).then(res => res.data),

  // إعادة إرسال رابط التحقق من البريد الإلكتروني
  resendEmailVerification: (data: ResendEmailVerificationLinkCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/resend-email-verification`, data).then(res => res.data),

  // إعادة إرسال رابط استعادة كلمة المرور
  resendPasswordResetLink: (data: ResendPasswordResetLinkCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/resend-password-reset`, data).then(res => res.data),

  // التحقق من صحة عنوان البريد الإلكتروني
  verifyEmail: (data: VerifyEmailCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/verify-email`, data).then(res => res.data),

  // تغيير كلمة المرور
  changePassword: (data: ChangePasswordCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/change-password`, data).then(res => res.data),
};