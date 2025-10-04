import { Outlet } from 'react-router-dom';
import PublicRoute from '../components/auth/PublicRoute';

const AuthLayout = () => {
  return (
    <PublicRoute restricted={true}>
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-100" dir="rtl">
        <div className="w-full max-w-md relative">
          {/* تصميم خلفي مع دوائر متوهجة */}
          <div className="absolute top-0 left-0 w-72 h-72 bg-gradient-to-r from-blue-400 to-purple-600 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob"></div>
          <div className="absolute top-0 right-0 w-72 h-72 bg-gradient-to-r from-indigo-400 to-pink-600 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-2000"></div>
          <div className="absolute -bottom-8 left-20 w-72 h-72 bg-gradient-to-r from-yellow-400 to-red-600 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-4000"></div>

          <div className="relative">
            {/* الشعار والعنوان */}
            <div className="text-center mb-8">
              <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-r from-indigo-500 via-blue-500 to-purple-600 rounded-3xl shadow-2xl mb-6 relative">
                <span className="text-white text-3xl font-bold">B</span>
                <div className="absolute -top-1 -right-1 w-6 h-6 bg-green-400 rounded-full border-4 border-white animate-pulse"></div>
              </div>
              <h1 className="text-4xl font-bold bg-gradient-to-r from-gray-900 via-blue-900 to-purple-900 bg-clip-text text-transparent mb-3">
                BookN
              </h1>
              <p className="text-lg text-gray-600 font-medium">
                نظام إدارة الحجوزات المتطور
              </p>
              <p className="text-sm text-gray-500 mt-1">
                منصة شاملة لإدارة الكيانات والحجوزات
              </p>
            </div>

            {/* بطاقة النموذج */}
            <div className="glass-card shadow-2xl rounded-3xl px-8 py-8 border border-white/30">
              <Outlet />
            </div>

            {/* روابط إضافية */}
            <div className="text-center mt-6">
              <p className="text-xs text-gray-500">
                © 2024 BookN. جميع الحقوق محفوظة.
              </p>
            </div>
          </div>
        </div>
      </div>
    </PublicRoute>
  );
};

export default AuthLayout;