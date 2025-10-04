import React from 'react';
import { Link } from 'react-router-dom';

interface MainLayoutProps {
  children: React.ReactNode;
  showHeader?: boolean;
  showFooter?: boolean;
}

const MainLayout: React.FC<MainLayoutProps> = ({ 
  children, 
  showHeader = true, 
  showFooter = true 
}) => {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col" dir="rtl">
      {showHeader && (
        <header className="bg-white shadow-sm">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between items-center h-16">
              {/* Logo */}
              <Link to="/" className="flex items-center">
                <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                  <span className="text-white text-sm font-bold">B</span>
                </div>
                <span className="mr-2 text-xl font-bold text-gray-900">BookN</span>
              </Link>

              {/* Navigation */}
              <nav className="hidden md:flex space-x-8 space-x-reverse">
                <Link to="/" className="text-gray-600 hover:text-gray-900 px-3 py-2 text-sm font-medium">
                  الرئيسية
                </Link>
                <Link to="/properties" className="text-gray-600 hover:text-gray-900 px-3 py-2 text-sm font-medium">
                  الكيانات
                </Link>
                <Link to="/about" className="text-gray-600 hover:text-gray-900 px-3 py-2 text-sm font-medium">
                  من نحن
                </Link>
                <Link to="/contact" className="text-gray-600 hover:text-gray-900 px-3 py-2 text-sm font-medium">
                  تواصل معنا
                </Link>
              </nav>

              {/* Auth Links */}
              <div className="flex items-center space-x-4 space-x-reverse">
                <Link 
                  to="/auth/login" 
                  className="text-gray-600 hover:text-gray-900 px-3 py-2 text-sm font-medium"
                >
                  تسجيل الدخول
                </Link>
                <Link 
                  to="/auth/register" 
                  className="bg-blue-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-blue-700 transition-colors"
                >
                  سجل الآن
                </Link>
              </div>
            </div>
          </div>
        </header>
      )}

      {/* Main Content */}
      <main className="flex-1">
        {children}
      </main>

      {showFooter && (
        <footer className="bg-gray-800 text-white">
          <div className="max-w-7xl mx-auto py-12 px-4 sm:px-6 lg:px-8">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
              {/* Company Info */}
              <div className="col-span-1 md:col-span-2">
                <div className="flex items-center mb-4">
                  <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                    <span className="text-white text-sm font-bold">B</span>
                  </div>
                  <span className="mr-2 text-xl font-bold">BookN</span>
                </div>
                <p className="text-gray-300 mb-4">
                  نظام إدارة الحجوزات الشامل للفنادق والمنتجعات والشقق المفروشة. 
                  نساعدك في إدارة كيانك بكفاءة وزيادة إيراداتك.
                </p>
                <div className="flex space-x-4 space-x-reverse">
                  <a href="#" className="text-gray-300 hover:text-white">
                    <span className="sr-only">Facebook</span>
                    <svg className="h-6 w-6" fill="currentColor" viewBox="0 0 24 24">
                      <path fillRule="evenodd" d="M22 12c0-5.523-4.477-10-10-10S2 6.477 2 12c0 4.991 3.657 9.128 8.438 9.878v-6.987h-2.54V12h2.54V9.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V12h2.773l-.443 2.89h-2.33v6.988C18.343 21.128 22 16.991 22 12z" clipRule="evenodd" />
                    </svg>
                  </a>
                  <a href="#" className="text-gray-300 hover:text-white">
                    <span className="sr-only">Twitter</span>
                    <svg className="h-6 w-6" fill="currentColor" viewBox="0 0 24 24">
                      <path d="M8.29 20.251c7.547 0 11.675-6.253 11.675-11.675 0-.178 0-.355-.012-.53A8.348 8.348 0 0022 5.92a8.19 8.19 0 01-2.357.646 4.118 4.118 0 001.804-2.27 8.224 8.224 0 01-2.605.996 4.107 4.107 0 00-6.993 3.743 11.65 11.65 0 01-8.457-4.287 4.106 4.106 0 001.27 5.477A4.072 4.072 0 012.8 9.713v.052a4.105 4.105 0 003.292 4.022 4.095 4.095 0 01-1.853.07 4.108 4.108 0 003.834 2.85A8.233 8.233 0 012 18.407a11.616 11.616 0 006.29 1.84" />
                    </svg>
                  </a>
                </div>
              </div>

              {/* Quick Links */}
              <div>
                <h3 className="text-sm font-semibold uppercase tracking-wider mb-4">روابط سريعة</h3>
                <ul className="space-y-2">
                  <li><Link to="/about" className="text-gray-300 hover:text-white">من نحن</Link></li>
                  <li><Link to="/contact" className="text-gray-300 hover:text-white">تواصل معنا</Link></li>
                  <li><Link to="/terms" className="text-gray-300 hover:text-white">الشروط والأحكام</Link></li>
                  <li><Link to="/privacy" className="text-gray-300 hover:text-white">سياسة الخصوصية</Link></li>
                </ul>
              </div>

              {/* Contact Info */}
              <div>
                <h3 className="text-sm font-semibold uppercase tracking-wider mb-4">تواصل معنا</h3>
                <ul className="space-y-2">
                  <li className="text-gray-300">
                    <svg className="h-4 w-4 inline ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                    </svg>
                    info@bookn.com
                  </li>
                  <li className="text-gray-300">
                    <svg className="h-4 w-4 inline ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                    </svg>
                    +967 781 272 968
                  </li>
                </ul>
              </div>
            </div>

            <div className="mt-8 pt-8 border-t border-gray-700">
              <p className="text-center text-gray-300 text-sm">
                © 2024 BookN. جميع الحقوق محفوظة.
              </p>
            </div>
          </div>
        </footer>
      )}
    </div>
  );
};

export default MainLayout;