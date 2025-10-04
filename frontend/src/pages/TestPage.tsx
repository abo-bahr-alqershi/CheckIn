import React from 'react';

const TestPage: React.FC = () => {
  return (
    <div className="w-full bg-gradient-to-br from-blue-50 to-indigo-100" dir="rtl">
      <div className="w-full space-y-6">
        <h1 className="text-4xl font-bold text-center text-gray-800 mb-8">
          اختبار التصاميم
        </h1>

        {/* بطاقات اختبار */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-white rounded-2xl shadow-lg p-6 hover:shadow-xl transition-shadow duration-300">
            <h3 className="text-xl font-bold text-gray-800 mb-3">بطاقة بسيطة</h3>
            <p className="text-gray-600">هذا نص تجريبي للتأكد من أن التصاميم تعمل بشكل صحيح</p>
          </div>

          <div className="bg-gradient-to-r from-blue-500 to-purple-600 rounded-2xl shadow-lg p-6 text-white">
            <h3 className="text-xl font-bold mb-3">بطاقة متدرجة</h3>
            <p>تصميم بخلفية متدرجة ونص أبيض</p>
          </div>

          <div className="glass-card rounded-2xl shadow-lg p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-3">بطاقة زجاجية</h3>
            <p className="text-gray-600">تأثير الزجاج المطلوب</p>
          </div>
        </div>

        {/* أزرار اختبار */}
        <div className="flex flex-wrap gap-4 justify-center">
          <button className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-3 rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:scale-105">
            زر أزرق
          </button>
          
          <button className="bg-gradient-to-r from-green-500 to-green-600 hover:from-green-600 hover:to-green-700 text-white px-6 py-3 rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:scale-105">
            زر أخضر متدرج
          </button>
          
          <button className="bg-transparent border-2 border-gray-300 hover:border-blue-500 text-gray-700 hover:text-blue-500 px-6 py-3 rounded-xl transition-all duration-300">
            زر شفاف
          </button>
        </div>

        {/* اختبار الأيقونات */}
        <div className="bg-white rounded-2xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">أيقونات واختبارات</h3>
          <div className="flex items-center space-x-4 space-x-reverse">
            <div className="w-12 h-12 bg-blue-500 rounded-full flex items-center justify-center">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <div className="w-12 h-12 bg-red-500 rounded-full flex items-center justify-center">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </div>
            <div className="w-12 h-12 bg-yellow-500 rounded-full flex items-center justify-center">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.998 16.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
          </div>
        </div>

        {/* اختبار النماذج */}
        <div className="bg-white rounded-2xl shadow-lg p-6">
          <h3 className="text-xl font-bold text-gray-800 mb-4">نموذج اختبار</h3>
          <form className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">الاسم</label>
              <input
                type="text"
                placeholder="أدخل اسمك"
                className="w-full px-4 py-2 border border-gray-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">البريد الإلكتروني</label>
              <input
                type="email"
                placeholder="example@domain.com"
                className="w-full px-4 py-2 border border-gray-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                dir="ltr"
              />
            </div>
            <button
              type="submit"
              className="w-full bg-blue-500 hover:bg-blue-600 text-white py-3 rounded-xl font-medium transition-colors duration-300"
            >
              إرسال
            </button>
          </form>
        </div>

        {/* اختبار الشبكة */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {[1, 2, 3, 4, 5, 6, 7, 8].map((num) => (
            <div key={num} className="bg-gray-100 hover:bg-gray-200 rounded-lg p-4 text-center transition-colors duration-300">
              <div className="text-2xl font-bold text-gray-600">{num}</div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default TestPage;