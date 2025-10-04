import React, { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import { CommonUsersService } from '../../services/common-users.service';

interface AuthGuardProps {
  children: React.ReactNode;
  requiredRole?: 'Admin' | 'Owner';
  redirectTo?: string;
}

const AuthGuard: React.FC<AuthGuardProps> = ({ 
  children, 
  requiredRole, 
  redirectTo = '/auth/login' 
}) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const [userRole, setUserRole] = useState<string | null>(null);

  useEffect(() => {
    const checkAuth = async () => {
      const token = localStorage.getItem('token');
      const userData = localStorage.getItem('user');

      if (!token) {
        setIsAuthenticated(false);
        return;
      }

      try {
        // التحقق من صحة التوكن
        const result = await CommonUsersService.getCurrentUser({});
        
        if (result.success && result.data) {
          setIsAuthenticated(true);
          setUserRole(result.data.role);
        } else {
          setIsAuthenticated(false);
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');
        }
      } catch (error) {
        setIsAuthenticated(false);
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
      }
    };

    checkAuth();
  }, []);

  // عرض مؤشر التحميل أثناء التحقق
  if (isAuthenticated === null) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">جارٍ التحقق من الهوية...</p>
        </div>
      </div>
    );
  }

  // إعادة توجيه إذا لم يكن مصادق عليه
  if (!isAuthenticated) {
    return <Navigate to={redirectTo} replace />;
  }

  // التحقق من الدور المطلوب
  if (requiredRole && userRole !== requiredRole) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-lg shadow-lg p-8 max-w-md w-full text-center">
          <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.998 16.5c-.77.833.192 2.5 1.732 2.5z" />
            </svg>
          </div>
          <h2 className="text-xl font-bold text-gray-900 mb-2">غير مصرح لك</h2>
          <p className="text-gray-600 mb-4">
            ليس لديك صلاحية للوصول إلى هذه الصفحة
          </p>
          <button
            onClick={() => window.history.back()}
            className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 transition-colors"
          >
            العودة للخلف
          </button>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};

export default AuthGuard;