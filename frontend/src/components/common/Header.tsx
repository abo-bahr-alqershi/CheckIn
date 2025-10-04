import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface HeaderProps {
  userRole: string;
}

const Header = ({ userRole }: HeaderProps) => {
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const navigate = useNavigate();

  const getRoleDisplayName = (role: string) => {
    switch (role) {
      case 'admin':
        return 'مدير النظام';
      case 'property-owner':
        return 'مالك الكيان';
      default:
        return 'مستخدم';
    }
  };

  const handleLogout = () => {
    // TODO: Clear auth state
    navigate('/auth/login');
  };

  return (
    <header className="bg-white shadow-sm border-b border-gray-200 px-6 py-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center">
          <h1 className="text-xl font-semibold text-gray-800">
            مرحباً بك في النظام
          </h1>
        </div>

        <div className="flex items-center space-x-4 space-x-reverse">
          {/* Notifications */}
          <button className="relative p-2 text-gray-600 hover:text-gray-800 transition-colors">
            <span className="text-xl">🔔</span>
            <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
              3
            </span>
          </button>

          {/* User Profile Dropdown */}
          <div className="relative">
            <button
              onClick={() => setIsDropdownOpen(!isDropdownOpen)}
              className="flex items-center space-x-2 space-x-reverse text-gray-700 hover:text-gray-900 transition-colors"
            >
              <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center text-white font-medium">
                أ
              </div>
              <div className="text-right">
                <p className="text-sm font-medium">أحمد محمد</p>
                <p className="text-xs text-gray-500">{getRoleDisplayName(userRole)}</p>
              </div>
              <span className="text-gray-400">⬇️</span>
            </button>

            {isDropdownOpen && (
              <div className="absolute left-0 mt-2 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-50">
                <button
                  onClick={() => {/* TODO: Profile settings */}}
                  className="block w-full text-right px-4 py-2 text-sm text-gray-700 hover:bg-gray-50"
                >
                  الملف الشخصي
                </button>
                <button
                  onClick={() => {/* TODO: Settings */}}
                  className="block w-full text-right px-4 py-2 text-sm text-gray-700 hover:bg-gray-50"
                >
                  الإعدادات
                </button>
                <hr className="my-1" />
                <button
                  onClick={handleLogout}
                  className="block w-full text-right px-4 py-2 text-sm text-red-600 hover:bg-red-50"
                >
                  تسجيل الخروج
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;