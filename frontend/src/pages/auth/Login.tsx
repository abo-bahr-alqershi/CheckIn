import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { CommonAuthService } from '../../services/common-auth.service';
import { useAuthContext } from '../../contexts/AuthContext';
import type { LoginCommand } from '../../types/auth.types';
import ActionButton from '../../components/ui/ActionButton';
import { useNotificationContext } from '../../components/ui/NotificationProvider';

const Login: React.FC = () => {
  const navigate = useNavigate();
  const { showSuccess, showError } = useNotificationContext();
  const { login } = useAuthContext();
  
  const [formData, setFormData] = useState<LoginCommand>({
    email: '',
    password: '',
    rememberMe: false
  });
  
  const [showPassword, setShowPassword] = useState(false);
  const [errors, setErrors] = useState<Partial<LoginCommand>>({});

  const loginMutation = useMutation({
    mutationFn: CommonAuthService.login,
    onSuccess: (result) => {
      if (result.success && result.data) {
        const auth = result.data;
        console.log("سسسسسسسسسسسسسسسسس");

        // إنشاء كائن المستخدم
        const userData = {
          id: auth.userId,
          name: auth.userName,
          email: auth.email,
          role: auth.role,
          profileImage: auth.profileImage || undefined,
          propertyId: auth.propertyId,
          propertyName: auth.propertyName,
          staffId: auth.staffId,
          settingsJson: auth.settingsJson,
          favoritesJson: auth.favoritesJson
        };
        console.log("hhhhhhhhhhhhhhhhhhhhhhhhhhhhh");

        // استخدام AuthContext للتسجيل
        login(auth.accessToken, auth.refreshToken, userData);
        console.log("hhhhhhhhhhhhhhhhhhhhhhhhhhhhh");

        showSuccess('تم تسجيل الدخول بنجاح');
        
        // التوجيه بناءً على الدور
        if (auth.role === 'Owner') {
          navigate('/owner/dashboard');
        } else if (auth.role === 'Admin') {
          navigate('/admin/dashboard');
        } else {
          navigate('/dashboard');
        }
      } else {
        showError(result.message || 'فشل في تسجيل الدخول');
      }
    },
    onError: (error: any) => {
      console.log('Login error:', error);
      console.log('Error response:', error.response?.data);
      showError(error.response?.data?.message || 'حدث خطأ أثناء تسجيل الدخول');
    }
  });

  const validateForm = (): boolean => {
    const newErrors: Partial<LoginCommand> = {};
    
    if (!formData.email.trim()) {
      newErrors.email = 'البريد الإلكتروني مطلوب';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'البريد الإلكتروني غير صحيح';
    }
    
    if (!formData.password.trim()) {
      newErrors.password = 'كلمة المرور مطلوبة';
    } else if (formData.password.length < 6) {
      newErrors.password = 'كلمة المرور يجب أن تكون 6 أحرف على الأقل';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log('handleSubmit called with:', formData);
    
    if (validateForm()) {
      console.log('Form is valid, submitting...');
      loginMutation.mutate(formData);
    } else {
      console.log('Form validation failed');
    }
  };

  const handleInputChange = (field: keyof LoginCommand, value: string | boolean) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  return (
    <div>
      <div className="text-center mb-8">
        <h2 className="text-2xl font-bold text-gray-900 mb-2">تسجيل الدخول</h2>
        <p className="text-gray-600">أدخل بياناتك للوصول إلى النظام</p>
      </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              البريد الإلكتروني
            </label>
            <input
              type="email"
              value={formData.email}
              onChange={(e) => handleInputChange('email', e.target.value)}
              placeholder="admin@example.com"
              disabled={loginMutation.isPending}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left ${
                errors.email 
                  ? 'border-red-300 focus:ring-red-500' 
                  : 'border-gray-300'
              }`}
              dir="ltr"
            />
            {errors.email && (
              <p className="mt-1 text-sm text-red-600">{errors.email}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              كلمة المرور
            </label>
            <div className="relative">
              <input
                type={showPassword ? 'text' : 'password'}
                value={formData.password}
                onChange={(e) => handleInputChange('password', e.target.value)}
                placeholder="كلمة المرور"
                disabled={loginMutation.isPending}
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left pr-10 ${
                  errors.password 
                    ? 'border-red-300 focus:ring-red-500' 
                    : 'border-gray-300'
                }`}
                dir="ltr"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                disabled={loginMutation.isPending}
              >
                {showPassword ? 'إخفاء' : 'إظهار'}
              </button>
            </div>
            {errors.password && (
              <p className="mt-1 text-sm text-red-600">{errors.password}</p>
            )}
          </div>

          <div className="flex items-center justify-between">
            <label className="flex items-center">
              <input
                type="checkbox"
                checked={formData.rememberMe}
                onChange={(e) => handleInputChange('rememberMe', e.target.checked)}
                className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                disabled={loginMutation.isPending}
              />
              <span className="mr-2 text-sm text-gray-600">تذكرني</span>
            </label>
            
            <Link
              to="/auth/forgot-password"
              className="text-sm text-blue-600 hover:text-blue-500"
            >
              نسيت كلمة المرور؟
            </Link>
          </div>

          <ActionButton
            type="submit"
            variant="primary"
            label={loginMutation.isPending ? 'جاري تسجيل الدخول...' : 'تسجيل الدخول'}
            onClick={() => {}}
            className="w-full py-3"
            disabled={loginMutation.isPending}
          />
        </form>

        <div className="mt-8 text-center space-y-4">
          <div className="border-t border-gray-200 pt-6">
            <p className="text-sm text-gray-600">
              مالك كيان جديد؟{' '}
              <Link
                to="/auth/register"
                className="font-medium text-blue-600 hover:text-blue-500"
              >
                سجل الآن
              </Link>
            </p>
          </div>
          
          <div className="text-xs text-gray-500">
            <p>للحصول على حساب إدارة، تواصل مع الدعم الفني</p>
          </div>
        </div>
    </div>
  );
};

export default Login;