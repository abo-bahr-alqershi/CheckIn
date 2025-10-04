import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate, Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { CommonAuthService } from '../../services/common-auth.service';
import type { VerifyEmailCommand, ResendEmailVerificationLinkCommand } from '../../types/auth.types';
import ActionButton from '../../components/ui/ActionButton';
import { Card } from '../../components/ui/Card';
import { useNotifications } from '../../hooks/useNotifications';

const VerifyEmail: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { showSuccess, showError } = useNotifications();
  
  const [verificationStatus, setVerificationStatus] = useState<'pending' | 'success' | 'error'>('pending');
  const [email, setEmail] = useState('');
  const [emailError, setEmailError] = useState('');

  const token = searchParams.get('token');

  const verifyEmailMutation = useMutation({
    mutationFn: CommonAuthService.verifyEmail,
    onSuccess: (result) => {
      if (result.success) {
        setVerificationStatus('success');
        showSuccess('تم تأكيد البريد الإلكتروني بنجاح');
        setTimeout(() => {
          navigate('/auth/login');
        }, 3000);
      } else {
        setVerificationStatus('error');
        showError(result.message || 'فشل في تأكيد البريد الإلكتروني');
      }
    },
    onError: (error: any) => {
      setVerificationStatus('error');
      showError(error.response?.data?.message || 'حدث خطأ أثناء تأكيد البريد الإلكتروني');
    }
  });

  const resendVerificationMutation = useMutation({
    mutationFn: CommonAuthService.resendEmailVerification,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم إرسال رابط التأكيد مرة أخرى إلى بريدك الإلكتروني');
      } else {
        showError(result.message || 'فشل في إرسال رابط التأكيد');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء إرسال رابط التأكيد');
    }
  });

  useEffect(() => {
    if (token) {
      const verifyCommand: VerifyEmailCommand = { token };
      verifyEmailMutation.mutate(verifyCommand);
    } else {
      setVerificationStatus('error');
    }
  }, [token]);

  const handleResendVerification = () => {
    if (!email.trim()) {
      setEmailError('البريد الإلكتروني مطلوب');
      return;
    }
    
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      setEmailError('البريد الإلكتروني غير صحيح');
      return;
    }

    setEmailError('');
    const resendCommand: ResendEmailVerificationLinkCommand = { email };
    resendVerificationMutation.mutate(resendCommand);
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
          <p className="text-gray-600">تأكيد البريد الإلكتروني</p>
        </div>

        {verificationStatus === 'pending' && (
          <div className="text-center space-y-4">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
            <p className="text-gray-600">جارٍ تأكيد بريدك الإلكتروني...</p>
          </div>
        )}

        {verificationStatus === 'success' && (
          <div className="text-center space-y-6">
            <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-2">
                تم تأكيد البريد الإلكتروني بنجاح
              </h3>
              <p className="text-gray-600 text-sm mb-4">
                يمكنك الآن تسجيل الدخول إلى حسابك
              </p>
              <p className="text-gray-500 text-xs mb-6">
                سيتم توجيهك تلقائياً لصفحة تسجيل الدخول خلال 3 ثوانِ
              </p>
            </div>

            <ActionButton
              variant="primary"
              label="تسجيل الدخول الآن"
              onClick={() => navigate('/auth/login')}
              className="w-full"
            />
          </div>
        )}

        {verificationStatus === 'error' && (
          <div className="text-center space-y-6">
            <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </div>
            
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-2">
                فشل في تأكيد البريد الإلكتروني
              </h3>
              <p className="text-gray-600 text-sm mb-6">
                الرابط غير صحيح أو منتهي الصلاحية. يمكنك طلب رابط جديد أدناه.
              </p>
            </div>

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  البريد الإلكتروني
                </label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => handleEmailChange(e.target.value)}
                  placeholder="أدخل بريدك الإلكتروني"
                  disabled={resendVerificationMutation.isPending}
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
                variant="primary"
                label={resendVerificationMutation.isPending ? 'جاري الإرسال...' : 'إرسال رابط التأكيد'}
                onClick={handleResendVerification}
                className="w-full"
                disabled={resendVerificationMutation.isPending}
              />
            </div>
          </div>
        )}

        <div className="mt-8 text-center">
          <p className="text-sm text-gray-600">
            <Link
              to="/auth/login"
              className="font-medium text-blue-600 hover:text-blue-500"
            >
              العودة لتسجيل الدخول
            </Link>
          </p>
        </div>
      </Card>
    </div>
  );
};

export default VerifyEmail;