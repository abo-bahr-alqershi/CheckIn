import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRole?: 'Admin' | 'Owner';
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, requiredRole }) => {
  const location = useLocation();
  const token = localStorage.getItem('token');
  const userData = localStorage.getItem('user');

  // التحقق من وجود التوكن
  if (!token) {
    return <Navigate to="/auth/login" state={{ from: location }} replace />;
  }

  // التحقق من الدور إذا كان مطلوب
  if (requiredRole && userData) {
    try {
      const user = JSON.parse(userData);
      if (user.role !== requiredRole) {
        // إعادة توجيه حسب دور المستخدم
        if (user.role === 'Admin') {
          return <Navigate to="/admin/dashboard" replace />;
        } else if (user.role === 'Owner') {
          return <Navigate to="/owner/dashboard" replace />;
        } else {
          return <Navigate to="/dashboard" replace />;
        }
      }
    } catch (error) {
      console.error('خطأ في تحليل بيانات المستخدم:', error);
      localStorage.removeItem('user');
      return <Navigate to="/auth/login" replace />;
    }
  }

  return <>{children}</>;
};

export default ProtectedRoute;