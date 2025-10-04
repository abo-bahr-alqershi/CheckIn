import { useState } from 'react';
import { useAdminBookings } from '../../hooks/useAdminBookings';
import DataTable, { type Column } from '../../components/common/DataTable';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import Modal from '../../components/common/Modal';
import UserSelector from '../../components/selectors/UserSelector';
import UnitSelector from '../../components/selectors/UnitSelector';
// تم حذف استدعاء خدمة الحجوزات المباشر لاستخدام الهوك
import type {
  BookingDto,
  BookingStatus,
  UpdateBookingCommand,
  CancelBookingCommand,
  ConfirmBookingCommand,
  GetBookingsByDateRangeQuery
} from '../../types/booking.types';
import type { MoneyDto } from '../../types/payment.types';

const AdminBookings = () => {
  
  // State for search and filters
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({
    status: '',
    startDate: '',
    endDate: '',
    userId: '',
    unitId: '',
    minTotalPrice: '',
    maxTotalPrice: '',
    guestNameOrEmail: '',
    bookingSource: '',
    isWalkIn: undefined,
  });

  // State for modals
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showCancelModal, setShowCancelModal] = useState(false);
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [selectedBooking, setSelectedBooking] = useState<BookingDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);

  // State for forms
  const [editForm, setEditForm] = useState<UpdateBookingCommand>({
    bookingId: '',
    checkIn: '',
    checkOut: '',
    guestsCount: 1,
  });

  const [cancelForm, setCancelForm] = useState({
    cancellationReason: '',
  });

  // Build query params
  const queryParams: GetBookingsByDateRangeQuery = {
    startDate: filterValues.startDate || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    endDate: filterValues.endDate || new Date().toISOString().split('T')[0],
    pageNumber: currentPage,
    pageSize,
    userId: filterValues.userId || undefined,
    unitId: filterValues.unitId || undefined,
    guestNameOrEmail: searchTerm || undefined,
    bookingSource: filterValues.bookingSource || undefined,
  };

  // استخدام الهوك لإدارة بيانات الحجوزات والعمليات
  const {
    bookingsData,
    isLoading: isLoadingBookings,
    error: bookingsError,
    updateBooking,
    cancelBooking,
    confirmBooking
  } = useAdminBookings(queryParams);

  // Filter bookings by status on client side if status filter is applied
  const filteredBookings = bookingsData?.items?.filter(booking => 
    !filterValues.status || booking.status === filterValues.status
  ) || [];

  // تم حذف التعريفات المباشرة للـ mutations لاستخدام الهوك الجديد

  // Helper functions
  const handleViewDetails = (booking: BookingDto) => {
    setSelectedBooking(booking);
    setShowDetailsModal(true);
  };

  const handleEdit = (booking: BookingDto) => {
    setSelectedBooking(booking);
    setEditForm({
      bookingId: booking.id,
      checkIn: booking.checkIn.split('T')[0],
      checkOut: booking.checkOut.split('T')[0],
      guestsCount: booking.guestsCount,
    });
    setShowEditModal(true);
  };

  const handleCancel = (booking: BookingDto) => {
    setSelectedBooking(booking);
    setShowCancelModal(true);
  };

  /**
   * دالة لتأكيد الحجز باستخدام الهوك
   * @param booking بيانات الحجز
   */
  const handleConfirm = (booking: BookingDto) => {
    const command: ConfirmBookingCommand = { bookingId: booking.id };
    confirmBooking.mutate(command);
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      status: '',
      startDate: '',
      endDate: '',
      userId: '',
      unitId: '',
      minTotalPrice: '',
      maxTotalPrice: '',
      guestNameOrEmail: '',
      bookingSource: '',
      isWalkIn: undefined,
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  // Get status color
  const getStatusColor = (status: BookingStatus) => {
    const statusColors = {
      Confirmed: 'bg-green-100 text-green-800',
      Pending: 'bg-yellow-100 text-yellow-800',
      Cancelled: 'bg-red-100 text-red-800',
      Completed: 'bg-blue-100 text-blue-800',
      CheckedIn: 'bg-purple-100 text-purple-800',
    };
    return statusColors[status] || 'bg-gray-100 text-gray-800';
  };

  const getStatusLabel = (status: BookingStatus) => {
    const statusLabels = {
      Confirmed: 'مؤكد',
      Pending: 'معلق',
      Cancelled: 'ملغي',
      Completed: 'مكتمل',
      CheckedIn: 'تم الوصول',
    };
    return statusLabels[status] || status;
  };

  // Statistics calculation
  const stats = {
    total: filteredBookings.length,
    confirmed: filteredBookings.filter(b => b.status === 'Confirmed').length,
    pending: filteredBookings.filter(b => b.status === 'Pending').length,
    cancelled: filteredBookings.filter(b => b.status === 'Cancelled').length,
    completed: filteredBookings.filter(b => b.status === 'Completed').length,
  };

  // Filter options
  const filterOptions: FilterOption[] = [
    {
      key: 'status',
      label: 'حالة الحجز',
      type: 'select',
      options: [
        { value: 'Confirmed', label: 'مؤكد' },
        { value: 'Pending', label: 'معلق' },
        { value: 'Cancelled', label: 'ملغي' },
        { value: 'Completed', label: 'مكتمل' },
        { value: 'CheckedIn', label: 'تم الوصول' },
      ],
    },
    {
      key: 'startDate',
      label: 'تاريخ البداية',
      type: 'date',
    },
    {
      key: 'endDate',
      label: 'تاريخ النهاية',
      type: 'date',
    },
    {
      key: 'userId',
      label: 'المستخدم',
      type: 'custom',
      render: (value: string, onChange: (value: any) => void) => (
        <UserSelector
          value={value}
          onChange={(userId) => onChange(userId)}
          placeholder="اختر المستخدم"
          allowedRoles={['Customer']}
          className="w-full"
        />
      ),
    },
    {
      key: 'unitId',
      label: 'الوحدة',
      type: 'custom',
      render: (value: string, onChange: (value: any) => void) => (
        <UnitSelector
          value={value}
          onChange={(unitId) => onChange(unitId)}
          placeholder="اختر الوحدة"
          className="w-full"
        />
      ),
    },
    {
      key: 'minTotalPrice',
      label: 'الحد الأدنى للسعر',
      type: 'number',
      placeholder: 'أدخل الحد الأدنى',
    },
    {
      key: 'maxTotalPrice',
      label: 'الحد الأقصى للسعر',
      type: 'number',
      placeholder: 'أدخل الحد الأقصى',
    },
    {
      key: 'bookingSource',
      label: 'مصدر الحجز',
      type: 'select',
      options: [
        { value: 'web', label: 'موقع الويب' },
        { value: 'mobile', label: 'التطبيق' },
        { value: 'admin', label: 'لوحة الإدارة' },
        { value: 'phone', label: 'الهاتف' },
      ],
    },
    {
      key: 'isWalkIn',
      label: 'حجز مباشر',
      type: 'boolean',
    },
  ];

  // Table columns
  const columns: Column<BookingDto>[] = [
    {
      key: 'id',
      title: 'معرف الحجز',
      sortable: true,
      render: (value: string) => (
        <span className="font-mono text-sm text-gray-600">
          {value.substring(0, 8)}...
        </span>
      ),
    },
    {
      key: 'userName',
      title: 'اسم العميل',
      sortable: true,
      render: (value: string, record: BookingDto) => (
        <div className="flex flex-col">
          <span className="font-medium text-gray-900">{value}</span>
          <span className="text-sm text-gray-500">{record.guestsCount} ضيف</span>
        </div>
      ),
    },
    {
      key: 'unitName',
      title: 'الوحدة',
      sortable: true,
    },
    {
      key: 'checkIn',
      title: 'تاريخ الوصول',
      sortable: true,
      render: (value: string) => (
        <span className="text-sm">
          {new Date(value).toLocaleDateString('ar-SA')}
        </span>
      ),
    },
    {
      key: 'checkOut',
      title: 'تاريخ المغادرة',
      sortable: true,
      render: (value: string) => (
        <span className="text-sm">
          {new Date(value).toLocaleDateString('ar-SA')}
        </span>
      ),
    },
    {
      key: 'totalPrice',
      title: 'السعر الإجمالي',
      render: (value: MoneyDto) => (
        <div className="text-left">
          <span className="font-medium">{value.amount}</span>
          <span className="text-sm text-gray-500 mr-1">{value.currency}</span>
        </div>
      ),
    },
    {
      key: 'status',
      title: 'الحالة',
      render: (value: BookingStatus) => (
        <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(value)}`}>
          {getStatusLabel(value)}
        </span>
      ),
    },
    {
      key: 'bookedAt',
      title: 'تاريخ الحجز',
      sortable: true,
      render: (value: string) => (
        <span className="text-sm text-gray-500">
          {new Date(value).toLocaleDateString('ar-SA')}
        </span>
      ),
    },
  ];

  // Table actions
  const tableActions = [
    {
      label: 'عرض التفاصيل',
      icon: '👁️',
      color: 'blue' as const,
      onClick: handleViewDetails,
    },
    {
      label: 'تعديل',
      icon: '✏️',
      color: 'blue' as const,
      onClick: handleEdit,
      show: (booking: BookingDto) => booking.status === 'Pending' || booking.status === 'Confirmed',
    },
    {
      label: 'إلغاء',
      icon: '❌',
      color: 'red' as const,
      onClick: handleCancel,
      show: (booking: BookingDto) => booking.status === 'Pending' || booking.status === 'Confirmed',
    },
    {
      label: 'تأكيد',
      icon: '✅',
      color: 'green' as const,
      onClick: handleConfirm,
      show: (booking: BookingDto) => booking.status === 'Pending',
    },
  ];

  if (bookingsError) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-8 text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">خطأ في تحميل البيانات</h2>
        <p className="text-gray-600">حدث خطأ أثناء تحميل بيانات الحجوزات. يرجى المحاولة مرة أخرى.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة الحجوزات</h1>
            <p className="text-gray-600 mt-1">
              إدارة جميع حجوزات النظام مع فلترة متقدمة وإدارة الحالات
            </p>
          </div>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-blue-100 p-2 rounded-lg">
              <span className="text-2xl">📊</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">إجمالي الحجوزات</p>
              <p className="text-2xl font-bold text-gray-900">{stats.total}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-green-100 p-2 rounded-lg">
              <span className="text-2xl">✅</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">مؤكدة</p>
              <p className="text-2xl font-bold text-green-600">{stats.confirmed}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-yellow-100 p-2 rounded-lg">
              <span className="text-2xl">⏳</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">معلقة</p>
              <p className="text-2xl font-bold text-yellow-600">{stats.pending}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-red-100 p-2 rounded-lg">
              <span className="text-2xl">❌</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">ملغية</p>
              <p className="text-2xl font-bold text-red-600">{stats.cancelled}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-blue-100 p-2 rounded-lg">
              <span className="text-2xl">🏁</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">مكتملة</p>
              <p className="text-2xl font-bold text-blue-600">{stats.completed}</p>
            </div>
          </div>
        </div>
      </div>

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في الحجوزات (اسم العميل أو البريد الإلكتروني)..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Bookings Table */}
      <DataTable
        data={filteredBookings}
        columns={columns}
        loading={isLoadingBookings}
        pagination={{
          current: currentPage,
          total: bookingsData?.totalCount || 0,
          pageSize,
          onChange: (page, size) => {
            setCurrentPage(page);
            setPageSize(size);
          },
        }}
        rowSelection={{
          selectedRowKeys: selectedRows,
          onChange: setSelectedRows,
        }}
        actions={tableActions}
        onRowClick={handleViewDetails}
      />

      {/* Booking Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedBooking(null);
        }}
        title="تفاصيل الحجز"
        size="xl"
      >
        {selectedBooking && (
          <div className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف الحجز</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedBooking.id}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">اسم العميل</label>
                <p className="mt-1 text-sm text-gray-900">{selectedBooking.userName}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">الوحدة</label>
                <p className="mt-1 text-sm text-gray-900">{selectedBooking.unitName}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">عدد الضيوف</label>
                <p className="mt-1 text-sm text-gray-900">{selectedBooking.guestsCount}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">تاريخ الوصول</label>
                <p className="mt-1 text-sm text-gray-900">
                  {new Date(selectedBooking.checkIn).toLocaleDateString('ar-SA')}
                </p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">تاريخ المغادرة</label>
                <p className="mt-1 text-sm text-gray-900">
                  {new Date(selectedBooking.checkOut).toLocaleDateString('ar-SA')}
                </p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">السعر الإجمالي</label>
                <p className="mt-1 text-sm text-gray-900 font-medium">
                  {selectedBooking.totalPrice.amount} {selectedBooking.totalPrice.currency}
                </p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">حالة الحجز</label>
                <span className={`mt-1 inline-flex px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(selectedBooking.status)}`}>
                  {getStatusLabel(selectedBooking.status)}
                </span>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">تاريخ الحجز</label>
                <p className="mt-1 text-sm text-gray-900">
                  {new Date(selectedBooking.bookedAt).toLocaleDateString('ar-SA')}
                </p>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Edit Booking Modal */}
      <Modal
        isOpen={showEditModal}
        onClose={() => {
          setShowEditModal(false);
          setSelectedBooking(null);
        }}
        title="تعديل بيانات الحجز"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowEditModal(false);
                setSelectedBooking(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => updateBooking.mutate(editForm)}
              disabled={updateBooking.status === 'pending'}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {updateBooking.status === 'pending' ? 'جارٍ التحديث...' : 'تحديث'}
            </button>
          </div>
        }
      >
        {selectedBooking && (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                تاريخ الوصول
              </label>
              <input
                type="date"
                value={editForm.checkIn}
                onChange={(e) => setEditForm(prev => ({ ...prev, checkIn: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                تاريخ المغادرة
              </label>
              <input
                type="date"
                value={editForm.checkOut}
                onChange={(e) => setEditForm(prev => ({ ...prev, checkOut: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                عدد الضيوف
              </label>
              <input
                type="number"
                min="1"
                value={editForm.guestsCount}
                onChange={(e) => setEditForm(prev => ({ ...prev, guestsCount: Number(e.target.value) }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
          </div>
        )}
      </Modal>

      {/* Cancel Booking Modal */}
      <Modal
        isOpen={showCancelModal}
        onClose={() => {
          setShowCancelModal(false);
          setSelectedBooking(null);
          setCancelForm({ cancellationReason: '' });
        }}
        title="إلغاء الحجز"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowCancelModal(false);
                setSelectedBooking(null);
                setCancelForm({ cancellationReason: '' });
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => cancelBooking.mutate({
                bookingId: selectedBooking!.id,
                cancellationReason: cancelForm.cancellationReason,
              })}
              disabled={cancelBooking.status === 'pending' || !cancelForm.cancellationReason.trim()}
              className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50"
            >
              {cancelBooking.status === 'pending' ? 'جارٍ الإلغاء...' : 'إلغاء الحجز'}
            </button>
          </div>
        }
      >
        {selectedBooking && (
          <div className="space-y-4">
            <div className="bg-red-50 border border-red-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-red-400 text-xl">⚠️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-red-800">
                    تأكيد إلغاء الحجز
                  </h3>
                  <p className="mt-2 text-sm text-red-700">
                    هل أنت متأكد من إلغاء حجز <strong>{selectedBooking.userName}</strong> للوحدة <strong>{selectedBooking.unitName}</strong>؟
                    هذا الإجراء لا يمكن التراجع عنه.
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                سبب الإلغاء *
              </label>
              <textarea
                rows={3}
                value={cancelForm.cancellationReason}
                onChange={(e) => setCancelForm(prev => ({ ...prev, cancellationReason: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                placeholder="أدخل سبب إلغاء الحجز..."
              />
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default AdminBookings;