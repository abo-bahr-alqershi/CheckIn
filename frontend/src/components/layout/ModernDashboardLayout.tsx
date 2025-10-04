import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { Navigate } from 'react-router-dom';
import { useAuthContext } from '../../contexts/AuthContext';
import { useAdminStatistics } from '../../hooks/useAdminStatistics';
import { AttachMoney as AttachMoneyIcon } from '@mui/icons-material';

interface DashboardLayoutProps {
  children: React.ReactNode;
}

interface MenuItem {
  path: string;
  label: string;
  icon: React.ReactNode;
  badge?: string;
  children?: MenuItem[];
}

const ModernDashboardLayout: React.FC<DashboardLayoutProps> = ({ children }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  
  // استخدام AuthContext للحصول على بيانات المستخدم الحقيقية
  const { user, isAuthenticated, isLoading, logout } = useAuthContext();
  // جلب احصائيات لوحة التحكم
  const { data: stats } = useAdminStatistics();

  // انتظر انتهاء فحص المصادقة قبل المتابعة
  if (isLoading) {
    return null; // أو عرض مؤشر تحميل
  }
  // إعادة توجيه إلى صفحة تسجيل الدخول إذا لم يكن المستخدم مصدقًا عليه
  if (!isAuthenticated || !user) {
    return <Navigate to="/auth/login" replace />;
  }

  const isAdmin = user.role === 'Admin';
  const isPropertyOwner = user.role === 'Owner';

  const adminMenuItems: MenuItem[] = [
    // الرئيسية
    {
      path: '/admin/dashboard',
      label: 'لوحة التحكم',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
        </svg>
      )
    },
    
    // إدارة المستخدمين
    {
      path: '/admin/users',
      label: 'المستخدمون',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
        </svg>
      ),
      badge: stats ? stats.unverifiedUsers.toString() : '0'
    },
    // إدارة العملات
    {
      path: '/admin/inputs',
      label: 'إدارة المدخلات',
      icon: <AttachMoneyIcon className="w-5 h-5" />
    },
    // إدارة الكيانات
    {
      path: '/admin/properties',
      label: 'الكيانات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
        </svg>
      ),
      badge: stats ? stats.unapprovedProperties.toString() : '0'
    },
    {
      path: '/admin/property-types',
      label: 'أنواع الكيانات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
        </svg>
      )
    },
    {
      path: '/admin/units',
      label: 'الوحدات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 5a1 1 0 011-1h14a1 1 0 011 1v2a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM4 13a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H5a1 1 0 01-1-1v-6zM16 13a1 1 0 011-1h2a1 1 0 011 1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-6z" />
        </svg>
      )
    },
    {
      path: '/admin/property-images',
      label: 'صور الكيانات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7h4l1-2h8l1 2h4v13H3V7z" />
          <circle cx="12" cy="13" r="4" />
        </svg>
      )
    },
    {
      path: '/admin/unit-images',
      label: 'صور الوحدات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7h4l1-2h8l1 2h4v13H3V7z" />
          <circle cx="12" cy="13" r="4" />
        </svg>
      )
    },
    {
      path: '/admin/amenities',
      label: 'المرافق',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
        </svg>
      )
    },
    {
      path: '/admin/property-services',
      label: 'الخدمات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7.5L12 12l9-4.5v9L12 21l-9-4.5v-9z" />
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 12V3.5" />
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7.5l9 4.5 9-4.5" />
        </svg>
      )
    },
    
    // إدارة الحجوزات والمدفوعات
    {
      path: '/admin/bookings',
      label: 'الحجوزات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      ),
      badge: stats ? stats.unconfirmedBookings.toString() : '0'
    },
    {
      path: '/admin/payments',
      label: 'المدفوعات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
        </svg>
      )
    },
    
    // التقييمات والإشعارات
    {
      path: '/admin/reviews',
      label: 'التقييمات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
        </svg>
      )
    },
    {
      path: '/admin/notifications',
      label: 'الإشعارات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-5-5V9a6 6 0 10-12 0v3l-5 5h5m7 0v1a3 3 0 01-6 0v-1m6 0H9" />
        </svg>
      ),
      badge: stats ? stats.unreadNotifications.toString() : '0'
    },
    
    // ادارة البلاغات
    {
      path: '/admin/reports',
      label: 'البلاغات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
        </svg>
      )
    },
    {
      path: '/admin/audit-logs',
      label: 'سجلات النشاط',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
      )
    },
    
    // الإعدادات
    {
      path: '/admin/settings',
      label: 'الإعدادات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
        </svg>
      )
    },
    // إدارة الصفحة الرئيسية الديناميكية
    {
      path: '/admin/home-sections',
      label: 'إدارة أقسام الصفحة الرئيسية',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h7v7H3V3zm11 0h7v7h-7V3zM3 14h7v7H3v-7zm11 0h7v7h-7v-7z" />
        </svg>
      )
    }
  ];

  const ownerMenuItems: MenuItem[] = [
    {
      path: '/property-owner/dashboard',
      label: 'لوحة التحكم',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
        </svg>
      )
    },
    {
      path: '/property-owner/properties',
      label: 'كياناتي',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
        </svg>
      )
    },
    {
      path: '/property-owner/units',
      label: 'الوحدات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 5a1 1 0 011-1h14a1 1 0 011 1v2a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM4 13a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H5a1 1 0 01-1-1v-6zM16 13a1 1 0 011-1h2a1 1 0 011 1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-6z" />
        </svg>
      )
    },
    {
      path: '/property-owner/bookings',
      label: 'الحجوزات',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      )
    },
    {
      path: '/property-owner/staff',
      label: 'الموظفون',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
        </svg>
      )
    }
  ];

  // Dynamically configure sidebar menu: hide image links for Admin, show for non-Admin with user's propertyId
  let menuItems: MenuItem[];
  // Admin sees all except image galleries
  if (isAdmin) {
    menuItems = adminMenuItems.filter(
      item => item.label !== 'صور الكيانات' && item.label !== 'صور الوحدات'
    );
  } else if (isPropertyOwner || user?.role === 'Staff') {
    // Property owner or staff sees owner menu plus image galleries for their property
    menuItems = [...ownerMenuItems];
    if (user?.propertyId) {
      // Property Images
      menuItems.push({
        path: `/admin/property-images/${user.propertyId}`,
        label: 'صور الكيانات',
        icon: (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7h4l1-2h8l1 2h4v13H3V7z" />
            <circle cx="12" cy="13" r="4" />
          </svg>
        )
      });
    }
  } else {
    menuItems = [];
  }

  const MenuItem: React.FC<{ item: MenuItem; isActive: boolean }> = ({ item, isActive }) => (
    <Link
      to={item.path}
      onClick={() => setIsMobileMenuOpen(false)}
      className={`
        group relative flex items-center px-4 py-3 text-sm font-medium rounded-xl
        transition-all duration-300 ease-in-out transform
        ${isActive
          ? 'bg-gradient-to-r from-indigo-500 via-blue-500 to-purple-600 text-white shadow-lg scale-105'
          : 'text-gray-700 hover:bg-gradient-to-r hover:from-blue-50 hover:to-indigo-50 hover:text-indigo-600 hover:scale-102'
        }
        ${isSidebarCollapsed ? 'justify-center' : ''}
      `}
    >
      {/* أيقونة مع تأثير متوهج */}
      <div className={`
        relative flex items-center justify-center w-6 h-6
        ${isActive ? 'text-white' : 'text-gray-400 group-hover:text-indigo-500'}
        transition-colors duration-300
      `}>
        {item.icon}
        {isActive && (
          <div className="absolute inset-0 bg-white/20 rounded-full animate-pulse"></div>
        )}
      </div>

      {/* النص والشارة */}
      {!isSidebarCollapsed && (
        <>
          <span className="mr-3 font-medium">{item.label}</span>
          {item.badge && (
            <span className={`
              ml-auto px-2 py-1 text-xs font-bold rounded-full
              ${isActive 
                ? 'bg-white/20 text-white' 
                : 'bg-indigo-100 text-indigo-600 group-hover:bg-indigo-200'
              }
              transition-colors duration-300
            `}>
              {item.badge}
            </span>
          )}
        </>
      )}

      {/* خط متوهج للعنصر النشط */}
      {isActive && (
        <div className="absolute right-0 top-0 bottom-0 w-1 bg-gradient-to-b from-white/50 to-white/20 rounded-l-full"></div>
      )}
    </Link>
  );

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-100" dir="rtl">
      {/* شريط علوي حديث */}
      <nav className="fixed top-0 left-0 right-0 z-30 bg-white/80 backdrop-blur-xl border-b border-white/20 shadow-lg">
        <div className="px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            {/* زر القائمة والشعار */}
            <div className="flex items-center">
              <button
                onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                className="md:hidden p-2 rounded-xl text-gray-500 hover:text-gray-900 hover:bg-gray-100 transition-colors duration-200"
              >
                <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                </svg>
              </button>

              <button
                onClick={() => setIsSidebarCollapsed(!isSidebarCollapsed)}
                className="hidden lg:block p-2 rounded-xl text-gray-500 hover:text-gray-900 hover:bg-gray-100 transition-colors duration-200 ml-3"
              >
                <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                </svg>
              </button>

              <div className="flex items-center ml-4">
                <div className="relative">
                  <div className="w-10 h-10 bg-gradient-to-r from-indigo-500 via-blue-500 to-purple-600 rounded-2xl flex items-center justify-center shadow-lg">
                    <span className="text-white text-lg font-bold">B</span>
                  </div>
                  <div className="absolute -top-1 -left-1 w-3 h-3 bg-green-400 rounded-full border-2 border-white animate-pulse"></div>
                </div>
                <div className="ml-3">
                  <h1 className="text-2xl font-bold bg-gradient-to-r from-indigo-600 via-blue-600 to-purple-600 bg-clip-text text-transparent">
                    BookN
                  </h1>
                  <p className="text-xs text-gray-500">نظام إدارة الحجوزات</p>
                </div>
              </div>
            </div>

            {/* شريط البحث */}
            <div className="hidden md:flex flex-1 max-w-lg mx-8">
              <div className="relative w-full">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <svg className="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                </div>
                <input
                  type="text"
                  placeholder="بحث..."
                  className="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-xl bg-white/50 backdrop-blur-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all duration-200"
                />
              </div>
            </div>

            {/* قائمة المستخدم */}
            <div className="flex items-center space-x-4 space-x-reverse">
              {/* الإشعارات */}
              <button className="relative p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-xl transition-colors duration-200">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-5-5V9a6 6 0 10-12 0v3l-5 5h5m7 0v1a3 3 0 01-6 0v-1m6 0H9" />
                </svg>
                <span className="absolute top-1 left-1 w-3 h-3 bg-red-400 rounded-full animate-pulse"></span>
              </button>

              {/* معلومات المستخدم */}
              <button
                type="button"
                onClick={() => navigate('/profile')}
                className="flex items-center p-2 rounded-xl hover:bg-gray-100 transition-colors duration-200"
              >
                {user?.profileImage
                  ? (
                    <img
                      src={user.profileImage}
                      alt={user.name}
                      className="w-8 h-8 rounded-xl object-cover shadow-md"
                    />
                  ) : (
                    <div className="w-8 h-8 bg-gradient-to-r from-indigo-400 to-purple-500 rounded-xl flex items-center justify-center shadow-md">
                      <span className="text-white text-sm font-bold">
                        {user?.name?.charAt(0).toUpperCase()}
                      </span>
                    </div>
                  )}
                <div className="ml-3 hidden sm:block">
                  <div className="text-sm font-medium text-gray-900">{user?.name}</div>
                  <div className="text-xs text-gray-500">
                    {user?.role === 'Admin' ? 'مدير النظام' : 'مالك كيان'}
                  </div>
                </div>
              </button>

              {/* قائمة الإعدادات */}
              <div className="relative">
                <button
                  type="button"
                  onClick={() => navigate('/settings')}
                  className="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-xl transition-colors duration-200"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                </button>
              </div>

              {/* تسجيل الخروج */}
              <button
                onClick={logout}
                className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-xl transition-colors duration-200"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </nav>

      <div className="pt-16 min-h-screen relative lg:flex">
        {/* القائمة الجانبية الحديثة */}
        <aside className={`
          fixed right-0 top-16 bottom-0 z-20
          ${isSidebarCollapsed ? 'w-20' : 'w-64'}
          bg-white/90 backdrop-blur-xl border-l border-white/20 shadow-xl
          transition-all duration-300 ease-in-out
          ${isMobileMenuOpen ? 'translate-x-0' : 'translate-x-full lg:translate-x-0'}
          lg:static lg:z-auto
        `}>
          <div className="h-full px-3 py-6 overflow-y-auto">
            <nav className="space-y-2">
              {menuItems.map((item) => {
                const isActive = location.pathname === item.path;
                return <MenuItem key={item.path} item={item} isActive={isActive} />;
              })}
            </nav>

            {/* قسم الاعدادات السفلي */}
            <div className="absolute bottom-6 left-0 right-0 px-3">
              <div className="border-t border-gray-200/50 pt-4">
                <Link
                  to="/profile"
                  className="flex items-center px-4 py-3 text-sm text-gray-600 rounded-xl hover:bg-gradient-to-r hover:from-blue-50 hover:to-indigo-50 hover:text-indigo-600 transition-all duration-300"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  {!isSidebarCollapsed && <span className="mr-3">الملف الشخصي</span>}
                </Link>
              </div>
            </div>
          </div>
        </aside>

        {/* طبقة تغطية للهاتف المحمول */}
        {isMobileMenuOpen && (
          <div 
            className="fixed inset-0 z-10 bg-black/40 backdrop-blur-sm md:hidden"
            onClick={() => setIsMobileMenuOpen(false)}
          />
        )}

        {/* المحتوى الرئيسي */}
        <main className="w-full min-h-screen transition-all duration-300 ease-in-out lg:flex-1">
          <div className="w-full h-full p-4 md:p-6">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
};

export default ModernDashboardLayout;