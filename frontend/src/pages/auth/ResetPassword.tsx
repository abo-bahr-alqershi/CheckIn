import React, { useState } from 'react';
import { useSearchParams, useNavigate, Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { CommonAuthService } from '../../services/common-auth.service';
import type { ResetPasswordCommand } from '../../types/auth.types';
import ActionButton from '../../components/ui/ActionButton';
import { Card } from '../../components/ui/Card';
import { useNotifications } from '../../hooks/useNotifications';

const ResetPassword: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { showSuccess, showError } = useNotifications();
  
  const [formData, setFormData] = useState({
    newPassword: '',
    confirmPassword: ''
  });
  
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [errors, setErrors] = useState<{ newPassword?: string; confirmPassword?: string }>({});

  const token = searchParams.get('token');

  const resetPasswordMutation = useMutation({
    mutationFn: CommonAuthService.resetPassword,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم تغيير كلمة المرور بنجاح. يمكنك الآن تسجيل الدخول');
        navigate('/auth/login');
      } else {
        showError(result.message || 'فشل في تغيير كلمة المرور');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء تغيير كلمة المرور');
    }
  });

  const validateForm = (): boolean => {
    const newErrors: { newPassword?: string; confirmPassword?: string } = {};
    
    if (!formData.newPassword.trim()) {
      newErrors.newPassword = 'كلمة المرور الجديدة مطلوبة';
    } else if (formData.newPassword.length < 6) {
      newErrors.newPassword = 'كلمة المرور يجب أن تكون 6 أحرف على الأقل';
    }
    
    if (!formData.confirmPassword.trim()) {
      newErrors.confirmPassword = 'تأكيد كلمة المرور مطلوب';
    } else if (formData.newPassword !== formData.confirmPassword) {
      newErrors.confirmPassword = 'كلمة المرور غير متطابقة';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!token) {
      showError('رابط إعادة تعيين كلمة المرور غير صحيح');
      return;
    }
    
    if (validateForm()) {
      const resetCommand: ResetPasswordCommand = {
        token,
        newPassword: formData.newPassword
      };
      resetPasswordMutation.mutate(resetCommand);
    }
  };

  const handleInputChange = (field: keyof typeof formData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  if (!token) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
        <Card className="w-full max-w-md p-8 shadow-xl">
          <div className="text-center">
            <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </div>
            <h2 className="text-xl font-bold text-gray-900 mb-2">رابط غير صحيح</h2>
            <p className="text-gray-600 mb-6">
              رابط إعادة تعيين كلمة المرور غير صحيح أو منتهي الصلاحية
            </p>
            <Link
              to="/auth/forgot-password"
              className="text-blue-600 hover:text-blue-500 font-medium"
            >
              طلب رابط جديد
            </Link>
          </div>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <Card className="w-full max-w-md p-8 shadow-xl">
        <div className="text-center mb-8">
          <div className="mx-auto w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center mb-4">
            <span className="text-white text-2xl font-bold">B</span>
          </div>
          <h1 className="text-2xl font-bold text-gray-900 mb-2">BookN</h1>
          <p className="text-gray-600">إعادة تعيين كلمة المرور</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              كلمة المرور الجديدة
            </label>
            <div className="relative">
              <input
                type={showNewPassword ? 'text' : 'password'}
                value={formData.newPassword}
                onChange={(e) => handleInputChange('newPassword', e.target.value)}
                placeholder="كلمة المرور الجديدة"
                disabled={resetPasswordMutation.isPending}
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left pr-10 ${
                  errors.newPassword 
                    ? 'border-red-300 focus:ring-red-500' 
                    : 'border-gray-300'
                }`}
                dir="ltr"
              />
              <button
                type="button"
                onClick={() => setShowNewPassword(!showNewPassword)}
                className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                disabled={resetPasswordMutation.isPending}
              >
                {showNewPassword ? 'إخفاء' : 'إظهار'}
              </button>
            </div>
            {errors.newPassword && (
              <p className="mt-1 text-sm text-red-600">{errors.newPassword}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              تأكيد كلمة المرور الجديدة
            </label>
            <div className="relative">
              <input
                type={showConfirmPassword ? 'text' : 'password'}
                value={formData.confirmPassword}
                onChange={(e) => handleInputChange('confirmPassword', e.target.value)}
                placeholder="تأكيد كلمة المرور الجديدة"
                disabled={resetPasswordMutation.isPending}
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left pr-10 ${
                  errors.confirmPassword 
                    ? 'border-red-300 focus:ring-red-500' 
                    : 'border-gray-300'
                }`}
                dir="ltr"
              />
              <button
                type="button"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                disabled={resetPasswordMutation.isPending}
              >
                {showConfirmPassword ? 'إخفاء' : 'إظهار'}
              </button>
            </div>
            {errors.confirmPassword && (
              <p className="mt-1 text-sm text-red-600">{errors.confirmPassword}</p>
            )}
          </div>

          {/* نصائح الأمان */}
          <div className="bg-yellow-50 border border-yellow-200 rounded-md p-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg className="h-5 w-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                </svg>
              </div>
              <div className="mr-3">
                <h3 className="text-sm font-medium text-yellow-800">
                  نصائح لكلمة مرور قوية
                </h3>
                <div className="mt-2 text-sm text-yellow-700">
                  <ul className="list-disc list-inside space-y-1">
                    <li>استخدم على الأقل 8 أحرف</li>
                    <li>امزج بين الأحرف الكبيرة والصغيرة</li>
                    <li>أضف أرقام ورموز خاصة</li>
                    <li>تجنب استخدام معلومات شخصية</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>

          <ActionButton
            type="submit"
            variant="primary"
            label={resetPasswordMutation.isPending ? 'جاري التحديث...' : 'تحديث كلمة المرور'}
            onClick={() => {}}
            className="w-full py-3"
            disabled={resetPasswordMutation.isPending}
          />
        </form>

        <div className="mt-8 text-center">
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
      </Card>
    </div>
  );
};

export default ResetPassword;