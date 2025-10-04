import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useUserDetails } from '../../hooks/useAdminUsers';
import LoadingSpinner from '../../components/ui/LoadingSpinner';
import { useNotificationContext } from '../../components/ui/NotificationProvider';

const UserDetails: React.FC = () => {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const { showError } = useNotificationContext();
  
  const { userDetails, isLoading: loading, error, isSuccess, message } = useUserDetails(userId || '');

  const isPropertyUser = () => {
    return userDetails?.role === 'Owner' || userDetails?.role === 'Staff';
  };

  const isClient = () => {
    return !isPropertyUser();
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return 'غير محدد';
    return new Date(dateString).toLocaleDateString('ar-SA');
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('ar-SA', {
      style: 'currency',
      currency: 'YER'
    }).format(amount);
  };

  // Navigation functions
  const navigateToBookings = () => {
    if (isPropertyUser()) {
      navigate(`/admin/bookings?propertyId=${userDetails?.propertyId}`);
    } else {
      navigate(`/admin/bookings?userId=${userId}`);
    }
  };

  const navigateToReviews = () => {
    if (isPropertyUser()) {
      navigate(`/admin/reviews?propertyId=${userDetails?.propertyId}`);
    } else {
      navigate(`/admin/reviews?userId=${userId}`);
    }
  };

  const navigateToReports = () => {
    if (isPropertyUser()) {
      navigate(`/admin/reports?propertyId=${userDetails?.propertyId}`);
    } else {
      navigate(`/admin/reports?userId=${userId}`);
    }
  };

  const navigateToReportsAgainst = () => {
    if (isPropertyUser()) {
      navigate(`/admin/reports?againstPropertyId=${userDetails?.propertyId}`);
    } else {
      navigate(`/admin/reports?againstUserId=${userId}`);
    }
  };

  const navigateToActivity = () => {
    if (isPropertyUser()) {
      navigate(`/admin/audit-logs?propertyId=${userDetails?.propertyId}`);
    } else {
      navigate(`/admin/audit-logs?userId=${userId}`);
    }
  };

  const navigateToPayments = () => {
    if (isPropertyUser()) {
      navigate(`/admin/payments?propertyId=${userDetails?.propertyId}`);
    } else {
      navigate(`/admin/payments?userId=${userId}`);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  if (!userDetails || error || (!loading && (!isSuccess || !userDetails))) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-8 text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">خطأ في تحميل البيانات</h2>
        <p className="text-gray-600 mb-4">{message || error?.message || 'لم يتم العثور على بيانات المستخدم'}</p>
        <button
          onClick={() => navigate('/admin/users')}
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
        >
          العودة لقائمة المستخدمين
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-4 space-x-reverse">
            <button
              onClick={() => navigate('/admin/users')}
              className="text-gray-400 hover:text-gray-600 transition-colors"
            >
              →
            </button>
            <div className="flex items-center space-x-4 space-x-reverse">
              <div className="w-16 h-16 rounded-full bg-gray-200 flex items-center justify-center overflow-hidden">
                {userDetails?.avatarUrl ? (
                  <img 
                    src={userDetails.avatarUrl} 
                    alt={userDetails.userName} 
                    className="w-full h-full object-cover" 
                  />
                ) : (
                  <span className="text-gray-500 text-2xl">
                    {userDetails?.userName.charAt(0).toUpperCase()}
                  </span>
                )}
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900">{userDetails?.userName}</h1>
                <p className="text-gray-600">{userDetails?.email}</p>
                <div className="flex items-center mt-1">
                  <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
                    userDetails?.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                  }`}>
                    {userDetails?.isActive ? 'نشط' : 'غير نشط'}
                  </span>
                  {isPropertyUser() && (
                    <span className="mr-2 inline-flex px-2 py-1 text-xs font-medium rounded-full bg-blue-100 text-blue-800">
                      {userDetails?.role === 'Owner' ? 'مالك كيان' : 'موظف'}
                    </span>
                  )}
                </div>
              </div>
            </div>
          </div>
          <div className="text-right">
            <p className="text-sm text-gray-500">عضو منذ</p>
            <p className="text-lg font-medium text-gray-900">{formatDate(userDetails?.createdAt || '')}</p>
          </div>
        </div>
      </div>

      {/* User Information */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
          👤 معلومات المستخدم
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div>
            <label className="block text-sm font-medium text-gray-500 mb-1">البريد الإلكتروني</label>
            <p className="text-gray-900">{userDetails?.email}</p>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-500 mb-1">رقم الهاتف</label>
            <p className="text-gray-900">{userDetails?.phoneNumber || 'غير محدد'}</p>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-500 mb-1">تاريخ التسجيل</label>
            <p className="text-gray-900">{formatDate(userDetails?.createdAt || '')}</p>
          </div>
        </div>
      </div>

      {/* Property Information (for Owner/Staff) */}
      {isPropertyUser() && (
        <div className="bg-white rounded-lg shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
            🏨 معلومات الكيان
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-500 mb-1">اسم الكيان</label>
              <p className="text-gray-900">{userDetails?.propertyName || 'غير محدد'}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500 mb-1">عدد الوحدات</label>
              <p className="text-gray-900">{userDetails?.unitsCount || 0}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500 mb-1">صور الكيان</label>
              <p className="text-gray-900">{userDetails?.propertyImagesCount || 0}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500 mb-1">صور الوحدات</label>
              <p className="text-gray-900">{userDetails?.unitImagesCount || 0}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500 mb-1">صافي الإيراد</label>
              <p className="text-gray-900 font-medium text-green-600">
                {formatCurrency(userDetails?.netRevenue || 0)}
              </p>
            </div>
          </div>
        </div>
      )}

      {/* Statistics */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {/* Bookings Stats */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-500">إجمالي الحجوزات</p>
              <p className="text-2xl font-bold text-gray-900">{userDetails?.bookingsCount || 0}</p>
              {userDetails?.pendingBookingsCount || 0 > 0 && (
                <p className="text-xs text-yellow-600">{userDetails?.pendingBookingsCount || 0} معلق</p>
              )}
              {userDetails?.canceledBookingsCount || 0 > 0 && (
                <p className="text-xs text-red-600">{userDetails?.canceledBookingsCount || 0} ملغى</p>
              )}
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <span className="text-2xl">📅</span>
            </div>
          </div>
          <div className="mt-3 text-xs text-gray-500">
            <p>أول حجز: {formatDate(userDetails?.firstBookingDate)}</p>
            <p>آخر حجز: {formatDate(userDetails?.lastBookingDate)}</p>
          </div>
        </div>

        {/* Reviews Stats */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-500">
                {isPropertyUser() ? 'تقييمات الكيان' : 'المراجعات'}
              </p>
              <p className="text-2xl font-bold text-gray-900">{userDetails?.reviewsCount || 0}</p>
            </div>
            <div className="w-12 h-12 bg-yellow-100 rounded-lg flex items-center justify-center">
              <span className="text-2xl">⭐</span>
            </div>
          </div>
        </div>

        {/* Reports Stats */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-500">البلاغات</p>
              <p className="text-2xl font-bold text-gray-900">{userDetails?.reportsCreatedCount || 0}</p>
              {userDetails?.reportsAgainstCount || 0 > 0 && (
                <p className="text-xs text-red-600">{userDetails?.reportsAgainstCount || 0} ضده</p>
              )}
            </div>
            <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
              <span className="text-2xl">⚠️</span>
            </div>
          </div>
        </div>

        {/* Payments Stats */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-500">المدفوعات</p>
              <p className="text-2xl font-bold text-green-600">{formatCurrency(userDetails?.totalPayments || 0)}</p>
              {userDetails?.totalRefunds || 0 > 0 && (
                <p className="text-xs text-red-600">مردودات: {formatCurrency(userDetails?.totalRefunds || 0)}</p>
              )}
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <span className="text-2xl">💰</span>
            </div>
          </div>
        </div>
      </div>

      {/* Action Buttons */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">الإجراءات السريعة</h2>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
          {/* Bookings */}
          <button
            onClick={navigateToBookings}
            className="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
          >
            <span className="text-2xl mb-2">📅</span>
            <span className="text-sm font-medium text-gray-900">
              {isPropertyUser() ? 'حجوزات الكيان' : 'حجوزات العميل'}
            </span>
            <span className="text-xs text-gray-500">{userDetails?.bookingsCount || 0}</span>
          </button>

          {/* Reviews */}
          <button
            onClick={navigateToReviews}
            className="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
          >
            <span className="text-2xl mb-2">⭐</span>
            <span className="text-sm font-medium text-gray-900">
              {isPropertyUser() ? 'تقييمات الكيان' : 'مراجعات العميل'}
            </span>
            <span className="text-xs text-gray-500">{userDetails?.reviewsCount || 0}</span>
          </button>

          {/* Reports Created */}
          <button
            onClick={navigateToReports}
            className="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
          >
            <span className="text-2xl mb-2">📝</span>
            <span className="text-sm font-medium text-gray-900">البلاغات المُنشأة</span>
            <span className="text-xs text-gray-500">{userDetails?.reportsCreatedCount || 0}</span>
          </button>

          {/* Reports Against */}
          {userDetails?.reportsAgainstCount || 0 > 0 && (
            <button
              onClick={navigateToReportsAgainst}
              className="flex flex-col items-center p-4 border border-red-200 rounded-lg hover:bg-red-50 transition-colors"
            >
              <span className="text-2xl mb-2">⚠️</span>
              <span className="text-sm font-medium text-red-600">
                {isPropertyUser() ? 'بلاغات ضد الكيان' : 'بلاغات ضد العميل'}
              </span>
              <span className="text-xs text-red-500">{userDetails?.reportsAgainstCount || 0}</span>
            </button>
          )}

          {/* Activity */}
          <button
            onClick={navigateToActivity}
            className="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
          >
            <span className="text-2xl mb-2">📊</span>
            <span className="text-sm font-medium text-gray-900">
              {isPropertyUser() ? 'نشاط الكيان' : 'نشاط العميل'}
            </span>
          </button>

          {/* Payments */}
          <button
            onClick={navigateToPayments}
            className="flex flex-col items-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
          >
            <span className="text-2xl mb-2">💰</span>
            <span className="text-sm font-medium text-gray-900">المدفوعات</span>
            <span className="text-xs text-gray-500">{formatCurrency(userDetails?.totalPayments || 0)}</span>
          </button>
        </div>
      </div>
    </div>
  );
};

export default UserDetails;