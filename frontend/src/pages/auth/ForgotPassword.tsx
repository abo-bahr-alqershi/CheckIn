import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { CommonAuthService } from '../../services/common-auth.service';
import type { ForgotPasswordCommand, ResendPasswordResetLinkCommand } from '../../types/auth.types';
import ActionButton from '../../components/ui/ActionButton';
import { useNotificationContext } from '../../components/ui/NotificationProvider';
import { Card } from '../../components/ui';

const ForgotPassword: React.FC = () => {
  const { showSuccess, showError } = useNotificationContext();
  
  const [email, setEmail] = useState('');
  const [emailError, setEmailError] = useState('');
  const [isEmailSent, setIsEmailSent] = useState(false);

  const forgotPasswordMutation = useMutation({
    mutationFn: CommonAuthService.forgotPassword,
    onSuccess: (result) => {
      if (result.success) {
        setIsEmailSent(true);
        showSuccess('تم إرسال رابط إعادة تعيين كلمة المرور إلى بريدك الإلكتروني');
      } else {
        showError(result.message || 'فشل في إرسال البريد الإلكتروني');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء إرسال البريد الإلكتروني');
    }
  });

  const resendMutation = useMutation({
    mutationFn: CommonAuthService.resendPasswordResetLink,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم إعادة إرسال الرابط بنجاح');
      } else {
        showError(result.message || 'فشل في إعادة الإرسال');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء إعادة الإرسال');
    }
  });

  const validateEmail = (): boolean => {
    if (!email.trim()) {
      setEmailError('البريد الإلكتروني مطلوب');
      return false;
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      setEmailError('البريد الإلكتروني غير صحيح');
      return false;
    }
    
    setEmailError('');
    return true;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateEmail()) {
      const data: ForgotPasswordCommand = { email };
      forgotPasswordMutation.mutate(data);
    }
  };

  const handleResend = () => {
    const data: ResendPasswordResetLinkCommand = { email };
    resendMutation.mutate(data);
  };

  const handleEmailChange = (value: string) => {
    setEmail(value);
    if (emailError) {
      setEmailError('');
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <Card className="w-full max-w-md p-8 shadow-xl">
        <div className="text-center mb-8">
          <div className="mx-auto w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center mb-4">
            <span className="text-white text-2xl font-bold">B</span>
          </div>
          <h1 className="text-2xl font-bold text-gray-900 mb-2">BookN</h1>
          <p className="text-gray-600">
            {isEmailSent ? 'تم إرسال البريد الإلكتروني' : 'نسيت كلمة المرور؟'}
          </p>
        </div>

        {!isEmailSent ? (
          <div>
            <div className="mb-6 text-center">
              <p className="text-gray-600 text-sm">
                أدخل بريدك الإلكتروني وسنرسل لك رابط لإعادة تعيين كلمة المرور
              </p>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  البريد الإلكتروني
                </label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => handleEmailChange(e.target.value)}
                  placeholder="admin@example.com"
                  disabled={forgotPasswordMutation.isPending}
                  className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left ${
                    emailError 
                      ? 'border-red-300 focus:ring-red-500' 
                      : 'border-gray-300'
                  }`}
                  dir="ltr"
                />
                {emailError && (
                  <p className="mt-1 text-sm text-red-600">{emailError}</p>
                )}
              </div>

              <ActionButton
                type="submit"
                variant="primary"
                label={forgotPasswordMutation.isPending ? 'جاري الإرسال...' : 'إرسال رابط إعادة التعيين'}
                onClick={() => {}}
                className="w-full py-3"
                disabled={forgotPasswordMutation.isPending}
              />
            </form>
          </div>
        ) : (
          <div className="text-center space-y-6">
            <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-2">
                تم إرسال البريد الإلكتروني
              </h3>
              <p className="text-gray-600 text-sm mb-4">
                تم إرسال رابط إعادة تعيين كلمة المرور إلى:
              </p>
              <p className="font-medium text-gray-900 text-sm mb-6">
                {email}
              </p>
              <p className="text-gray-500 text-xs mb-6">
                تحقق من صندوق الوارد أو مجلد الرسائل غير المرغوب فيها
              </p>
            </div>

            <div className="space-y-3">
              <ActionButton
                variant="secondary"
                label={resendMutation.isPending ? 'جاري الإرسال...' : 'إعادة إرسال البريد'}
                onClick={handleResend}
                className="w-full"
                disabled={resendMutation.isPending}
              />
              
              <ActionButton
                variant="secondary"
                label="تغيير البريد الإلكتروني"
                onClick={() => {
                  setIsEmailSent(false);
                  setEmail('');
                }}
                className="w-full"
              />
            </div>
          </div>
        )}

        <div className="mt-8 text-center space-y-4">
          <div className="border-t border-gray-200 pt-6">
            <p className="text-sm text-gray-600">
              تذكرت كلمة المرور؟{' '}
              <Link
                to="/auth/login"
                className="font-medium text-blue-600 hover:text-blue-500"
              >
                تسجيل الدخول
              </Link>
            </p>
          </div>
          
          <div className="text-xs text-gray-500">
            <p>هل تحتاج مساعدة؟</p>
            <Link to="/contact" className="text-blue-600 hover:text-blue-500">
              تواصل مع الدعم الفني
            </Link>
          </div>
        </div>
      </Card>
    </div>
  );
};

export default ForgotPassword;