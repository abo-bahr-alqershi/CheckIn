import React, { useState, useEffect, useRef } from 'react';
import { useAdminBookings } from '../../hooks/useAdminBookings';
import type { BookingDto } from '../../types/booking.types';

interface BookingSelectorProps {
  value?: string;
  onChange: (bookingId: string, booking?: BookingDto) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  userId?: string;
  status?: string;
  required?: boolean;
}

const BookingSelector: React.FC<BookingSelectorProps> = ({
  value = '',
  onChange,
  placeholder = 'اختر الحجز',
  className = '',
  disabled = false,
  userId,
  status,
  required = false
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedBooking, setSelectedBooking] = useState<BookingDto | null>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // بناء معايير البحث
  const searchParams = {
    startDate: '2020-01-01', // نطاق زمني واسع للبحث
    endDate: '2030-12-31',
    pageNumber: 1,
    pageSize: 50,
    userId: userId || undefined,
    status: status || undefined,
    searchTerm: searchTerm || undefined,
  };

  const { bookingsData, isLoading } = useAdminBookings(searchParams);

  // العثور على الحجز المحدد
  useEffect(() => {
    if (value && bookingsData?.items) {
      const booking = bookingsData.items.find(b => b.id === value);
      setSelectedBooking(booking || null);
    } else {
      setSelectedBooking(null);
    }
  }, [value, bookingsData]);

  // إغلاق القائمة عند النقر خارجها
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleBookingSelect = (booking: BookingDto) => {
    setSelectedBooking(booking);
    onChange(booking.id, booking);
    setIsOpen(false);
    setSearchTerm('');
  };

  const handleClear = () => {
    setSelectedBooking(null);
    onChange('');
    setSearchTerm('');
  };

  const getStatusLabel = (status: string) => {
    const statusLabels = {
      'Pending': 'معلق',
      'Confirmed': 'مؤكد',
      'Cancelled': 'ملغي',
      'Completed': 'مكتمل',
      'CheckedIn': 'تم تسجيل الدخول',
      'CheckedOut': 'تم تسجيل الخروج'
    };
    return statusLabels[status] || status;
  };

  const getStatusColor = (status: string) => {
    const statusColors = {
      'Pending': 'bg-yellow-100 text-yellow-800',
      'Confirmed': 'bg-blue-100 text-blue-800',
      'Cancelled': 'bg-red-100 text-red-800',
      'Completed': 'bg-green-100 text-green-800',
      'CheckedIn': 'bg-purple-100 text-purple-800',
      'CheckedOut': 'bg-gray-100 text-gray-800'
    };
    return statusColors[status] || 'bg-gray-100 text-gray-800';
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ar-SA', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const formatPrice = (price: any) => {
    if (!price || typeof price !== 'object') return '';
    return `${price.amount} ${price.currency}`;
  };

  return (
    <div className={`relative ${className}`} ref={dropdownRef}>
      {/* زر الاختيار */}
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        disabled={disabled}
        className={`
          w-full flex items-center justify-between px-3 py-2 border rounded-md shadow-sm
          bg-white text-right focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
          ${disabled ? 'bg-gray-50 text-gray-400 cursor-not-allowed' : 'hover:border-gray-400'}
          ${!selectedBooking && required ? 'border-red-300' : 'border-gray-300'}
        `}
      >
        <div className="flex items-center space-x-3 space-x-reverse flex-1 min-w-0">
          {selectedBooking ? (
            <div className="flex-1 min-w-0">
              <div className="flex items-center space-x-2 space-x-reverse">
                <span className="font-medium text-gray-900 truncate font-mono">
                  #{selectedBooking.id.substring(0, 8)}
                </span>
                <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(selectedBooking.status)}`}>
                  {getStatusLabel(selectedBooking.status)}
                </span>
              </div>
              <div className="flex items-center space-x-4 space-x-reverse mt-1">
                <p className="text-sm text-gray-500 truncate">
                  {formatDate(selectedBooking.checkIn)} - {formatDate(selectedBooking.checkOut)}
                </p>
                <p className="text-xs text-gray-400">{formatPrice(selectedBooking.totalPrice)}</p>
              </div>
            </div>
          ) : (
            <span className="text-gray-500">{placeholder}</span>
          )}
        </div>
        
        <div className="flex items-center space-x-1 space-x-reverse">
          {selectedBooking && !disabled && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                handleClear();
              }}
              className="p-1 hover:bg-gray-100 rounded"
            >
              <span className="text-gray-400 hover:text-gray-600">×</span>
            </button>
          )}
          <span className="text-gray-400">
            {isOpen ? '▲' : '▼'}
          </span>
        </div>
      </button>

      {/* القائمة المنسدلة */}
      {isOpen && (
        <div className="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg">
          {/* شريط البحث */}
          <div className="p-3 border-b border-gray-200">
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="البحث في الحجوزات..."
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
              autoFocus
            />
          </div>

          {/* قائمة الحجوزات */}
          <div className="max-h-60 overflow-y-auto">
            {isLoading ? (
              <div className="p-4 text-center text-gray-500">
                <div className="animate-spin inline-block w-6 h-6 border-2 border-gray-300 border-t-blue-600 rounded-full"></div>
                <p className="mt-2">جارٍ التحميل...</p>
              </div>
            ) : bookingsData?.items && bookingsData.items.length > 0 ? (
              bookingsData.items.map((booking) => (
                <button
                  key={booking.id}
                  type="button"
                  onClick={() => handleBookingSelect(booking)}
                  className="w-full flex items-center px-3 py-3 hover:bg-gray-50 text-right border-b border-gray-100 last:border-b-0"
                >
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center space-x-2 space-x-reverse">
                      <span className="font-medium text-gray-900 truncate font-mono">
                        #{booking.id.substring(0, 8)}
                      </span>
                      <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(booking.status)}`}>
                        {getStatusLabel(booking.status)}
                      </span>
                    </div>
                    <div className="flex items-center space-x-4 space-x-reverse mt-1">
                      <p className="text-sm text-gray-500 truncate">
                        📅 {formatDate(booking.checkIn)} - {formatDate(booking.checkOut)}
                      </p>
                    </div>
                    <div className="flex items-center space-x-4 space-x-reverse mt-1">
                      <p className="text-xs text-gray-400 truncate">
                        👤 {booking.userName || 'عميل'}
                      </p>
                      <p className="text-xs text-gray-400">
                        🏠 {booking.unitName}
                      </p>
                    </div>
                  </div>
                  <div className="flex-shrink-0 text-left">
                    <p className="text-sm font-medium text-gray-900">{formatPrice(booking.totalPrice)}</p>
                    <p className="text-xs text-gray-500">المجموع</p>
                  </div>
                </button>
              ))
            ) : (
              <div className="p-4 text-center text-gray-500">
                {searchTerm ? `لا توجد نتائج للبحث "${searchTerm}"` : 'لا توجد حجوزات'}
              </div>
            )}
          </div>

          {/* إجمالي النتائج */}
          {bookingsData?.items && bookingsData.items.length > 0 && (
            <div className="p-2 bg-gray-50 border-t border-gray-200 text-xs text-gray-500 text-center">
              {bookingsData.items.length} من {bookingsData.totalCount || 0} حجز
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default BookingSelector;