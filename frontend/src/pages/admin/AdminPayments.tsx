import { useState } from 'react';
import { useAdminPayments } from '../../hooks/useAdminPayments';
import DataTable, { type Column } from '../../components/common/DataTable';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import Modal from '../../components/common/Modal';
import UserSelector from '../../components/selectors/UserSelector';
import BookingSelector from '../../components/selectors/BookingSelector';
import PropertySelector from '../../components/selectors/PropertySelector';
import UnitSelector from '../../components/selectors/UnitSelector';
import CurrencyInput from '../../components/inputs/CurrencyInput';
import { LoadingSpinner, StatusBadge, ActionButton, ConfirmDialog, EmptyState, Tooltip } from '../../components/ui';
import { useUXHelpers } from '../../hooks/useUXHelpers';
import { useCurrencies } from '../../hooks/useCurrencies';
// تم حذف استدعاء خدمة المدفوعات المباشر لاستخدام الهوك
import type {
  PaymentDto,
  PaymentStatus,
  PaymentMethod,
  RefundPaymentCommand,
  VoidPaymentCommand,
  UpdatePaymentStatusCommand,
  GetPaymentsByStatusQuery,
  MoneyDto,
  GetAllPaymentsQuery
} from '../../types/payment.types';

const AdminPayments = () => {
  // UX Helpers
  const { loading, executeWithFeedback, showConfirmDialog, confirmDialog, hideConfirmDialog, copyToClipboard } = useUXHelpers();
  
  const { currencies, loading: currenciesLoading, error: currenciesError } = useCurrencies();
  const currencyCodes = currenciesLoading ? [] : currencies.map((c) => c.code);

  // State for search and filters
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({
    status: undefined,
    method: undefined,
    bookingId: undefined,
    userId: undefined,
    propertyId: undefined,
    unitId: undefined,
    minAmount: undefined,
    maxAmount: undefined,
    startDate: undefined,
    endDate: undefined,
  });

  // State for modals
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showRefundModal, setShowRefundModal] = useState(false);
  const [showVoidModal, setShowVoidModal] = useState(false);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [selectedPayment, setSelectedPayment] = useState<PaymentDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);

  // State for forms
  const [refundForm, setRefundForm] = useState<RefundPaymentCommand>({
    paymentId: '',
    refundAmount: { amount: 0, currency: 'YER', formattedAmount: '' },
    refundReason: '',
  });

  const [statusForm, setStatusForm] = useState<UpdatePaymentStatusCommand>({
    paymentId: '',
    newStatus: 'Pending',
  });

  // Build query params
  const queryParams: GetAllPaymentsQuery = {
    status: filterValues.status,
    method: filterValues.method,
    bookingId: filterValues.bookingId,
    userId: filterValues.userId,
    propertyId: filterValues.propertyId,
    unitId: filterValues.unitId,
    minAmount: filterValues.minAmount ? parseFloat(filterValues.minAmount) : undefined,
    maxAmount: filterValues.maxAmount ? parseFloat(filterValues.maxAmount) : undefined,
    startDate: filterValues.startDate || undefined,
    endDate: filterValues.endDate || undefined,
    pageNumber: currentPage,
    pageSize,
  };

  // استخدام الهوك لإدارة استعلام المدفوعات والعمليات بعد تعريف المعاملات
  const {
    paymentsData,
    isLoading: isLoadingPayments,
    error: paymentsError,
    refundPayment,
    voidPayment,
    updatePaymentStatus
  } = useAdminPayments(queryParams);

  // Remove client-side filtering
  const dataToDisplay = paymentsData?.items ?? [];

  // تمت المعالجة عبر الهوك useAdminPayments

  // Filter payments on client side for additional filters
  const filteredPayments = dataToDisplay?.filter(payment => {
    if (filterValues.bookingId && !payment.bookingId.includes(filterValues.bookingId)) return false;
    if (filterValues.minAmount && payment.amount.amount < parseFloat(filterValues.minAmount)) return false;
    if (filterValues.maxAmount && payment.amount.amount > parseFloat(filterValues.maxAmount)) return false;
    if (searchTerm) {
      const lowerSearch = searchTerm.toLowerCase();
      const matchesTransaction = payment.transactionId.toLowerCase().includes(lowerSearch);
      const matchesAmount = payment.amount.amount.toString().includes(searchTerm)
        || payment.amount.formattedAmount.toLowerCase().includes(lowerSearch);
      if (!matchesTransaction && !matchesAmount) return false;
    }
    return true;
  }) || [];

  // تم حذف تعريفات الـ mutations لاستخدام الهوك الجديد

  // Helper functions
  const handleViewDetails = (payment: PaymentDto) => {
    setSelectedPayment(payment);
    setShowDetailsModal(true);
  };

  const handleRefund = (payment: PaymentDto) => {
    setSelectedPayment(payment);
    setRefundForm({
      paymentId: payment.id,
      refundAmount: { ...payment.amount },
      refundReason: '',
    });
    setShowRefundModal(true);
  };

  const handleVoid = (payment: PaymentDto) => {
    showConfirmDialog({
      title: 'إبطال الدفعة',
      message: `هل أنت متأكد من إبطال الدفعة ${payment.transactionId} بمبلغ ${payment.amount.amount} ${payment.amount.currency}؟ هذا الإجراء لا يمكن التراجع عنه.`,
      type: 'danger',
      onConfirm: () => {
        executeWithFeedback(
          () => voidPayment.mutateAsync({ paymentId: payment.id }),
          {
            loadingKey: 'voidPayment',
            successMessage: 'تم إبطال الدفعة بنجاح',
            errorMessage: 'فشل في إبطال الدفعة'
          }
        );
      }
    });
  };

  const handleUpdateStatus = (payment: PaymentDto) => {
    setSelectedPayment(payment);
    setStatusForm({
      paymentId: payment.id,
      newStatus: payment.status,
    });
    setShowStatusModal(true);
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      status: undefined,
      method: undefined,
      bookingId: undefined,
      userId: undefined,
      propertyId: undefined,
      unitId: undefined,
      minAmount: undefined,
      maxAmount: undefined,
      startDate: undefined,
      endDate: undefined,
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  // Get status color
  const getStatusColor = (status: PaymentStatus) => {
    const statusColors = {
      Successful: 'bg-green-100 text-green-800',
      Failed: 'bg-red-100 text-red-800',
      Pending: 'bg-yellow-100 text-yellow-800',
      Refunded: 'bg-blue-100 text-blue-800',
      Voided: 'bg-gray-100 text-gray-800',
      PartiallyRefunded: 'bg-purple-100 text-purple-800',
    };
    return statusColors[status] || 'bg-gray-100 text-gray-800';
  };

  const getStatusLabel = (status: PaymentStatus) => {
    const statusLabels = {
      Successful: 'ناجح',
      Failed: 'فاشل',
      Pending: 'معلق',
      Refunded: 'مسترد',
      Voided: 'ملغي',
      PartiallyRefunded: 'مسترد جزئياً',
    };
    return statusLabels[status] || status;
  };

  // Get method label
  const getMethodLabel = (method: PaymentMethod) => {
    const methodLabels: Record<number, string> = {
      0: 'بطاقة ائتمان',
      1: 'PayPal',
      2: 'تحويل بنكي',
      3: 'نقداً',
      4: 'أخرى',
    };
    const index = (method as unknown as number);
    return methodLabels[index] || 'غير محدد';
  };

  // Statistics calculation
  const stats = {
    total: filteredPayments.length,
    successful: filteredPayments.filter(p => p.status === 'Successful').length,
    pending: filteredPayments.filter(p => p.status === 'Pending').length,
    failed: filteredPayments.filter(p => p.status === 'Failed').length,
    refunded: filteredPayments.filter(p => p.status === 'Refunded' || p.status === 'PartiallyRefunded').length,
    totalAmount: filteredPayments
      .filter(p => p.status === 'Successful')
      .reduce((sum, p) => sum + p.amount.amount, 0),
  };

  // Filter options
  const filterOptions: FilterOption[] = [
    {
      key: 'status', label: 'حالة الدفع', type: 'select',
      options: [
        { value: 'Successful', label: 'ناجح' },
        { value: 'Failed', label: 'فاشل' },
        { value: 'Pending', label: 'معلق' },
        { value: 'Refunded', label: 'مسترد' },
        { value: 'Voided', label: 'ملغي' },
        { value: 'PartiallyRefunded', label: 'مسترد جزئياً' },
      ],
    },
    {
      key: 'method', label: 'طريقة الدفع', type: 'select',
      options: [
        { value: 'CreditCard', label: 'بطاقة ائتمان' },
        { value: 'PayPal', label: 'PayPal' },
        { value: 'BankTransfer', label: 'تحويل بنكي' },
        { value: 'Cash', label: 'نقداً' },
        { value: 'Other', label: 'أخرى' },
      ],
    },
    {
      key: 'propertyId', label: 'الكيان', type: 'custom',
      render: (value, onChange) => (
        <PropertySelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر الكيان"
          className="w-full"
        />
      ),
    },
    {
      key: 'unitId', label: 'الوحدة', type: 'custom',
      render: (value, onChange) => (
        <UnitSelector
          value={value}
          onChange={(id) => onChange(id)}
          propertyId={filterValues.propertyId}
          className="w-full"
          disabled={!filterValues.propertyId}
        />
      ),
    },
    {
      key: 'bookingId', label: 'الحجز', type: 'custom',
      render: (value, onChange) => (
        <BookingSelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر الحجز"
          className="w-full"
        />
      ),
    },
    {
      key: 'userId', label: 'المستخدم', type: 'custom',
      render: (value, onChange) => (
        <UserSelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر المستخدم"
          allowedRoles={['Customer']}
          className="w-full"
        />
      ),
    },
    { key: 'minAmount', label: 'الحد الأدنى للمبلغ', type: 'number', placeholder: 'أدخل الحد الأدنى' },
    { key: 'maxAmount', label: 'الحد الأقصى للمبلغ', type: 'number', placeholder: 'أدخل الحد الأقصى' },
    { key: 'startDate', label: 'تاريخ البداية', type: 'date' },
    { key: 'endDate', label: 'تاريخ النهاية', type: 'date' },
  ];

  // Table columns
  const columns: Column<PaymentDto>[] = [
    {
      key: 'id',
      title: 'معرف الدفعة',
      sortable: true,
      render: (value: string) => (
        <Tooltip content={`انقر للنسخ: ${value}`}>
          <button
            onClick={() => copyToClipboard(value, 'تم نسخ معرف الدفعة')}
            className="font-mono text-sm text-gray-600 hover:text-blue-600 transition-colors"
          >
            {value.substring(0, 8)}...
          </button>
        </Tooltip>
      ),
    },
    {
      key: 'transactionId',
      title: 'رقم المعاملة',
      sortable: true,
      render: (value: string) => (
        <Tooltip content={`انقر للنسخ: ${value}`}>
          <button
            onClick={() => copyToClipboard(value, 'تم نسخ رقم المعاملة')}
            className="font-mono text-sm text-gray-900 hover:text-blue-600 transition-colors"
          >
            {value}
          </button>
        </Tooltip>
      ),
    },
    {
      key: 'bookingId',
      title: 'معرف الحجز',
      render: (value: string) => (
        <span className="font-mono text-sm text-gray-600">
          {value.substring(0, 8)}...
        </span>
      ),
    },
    {
      key: 'amount',
      title: 'المبلغ',
      render: (value: MoneyDto) => (
        <div className="text-left">
          <span className="font-medium text-lg">{value.amount}</span>
          <span className="text-sm text-gray-500 mr-1">{value.currency}</span>
        </div>
      ),
    },
    {
      key: 'method',
      title: 'طريقة الدفع',
      render: (value: PaymentMethod) => (
        <span className="text-sm">{getMethodLabel(value)}</span>
      ),
    },
    {
      key: 'status',
      title: 'الحالة',
      render: (value: PaymentStatus) => (
        <StatusBadge status={value} variant="payment" />
      ),
    },
    {
      key: 'paymentDate',
      title: 'تاريخ الدفع',
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
      label: 'استرداد',
      icon: '↩️',
      color: 'orange' as const,
      onClick: handleRefund,
      show: (payment: PaymentDto) => payment.status === 'Successful',
    },
    {
      label: 'إبطال',
      icon: '❌',
      color: 'red' as const,
      onClick: handleVoid,
      show: (payment: PaymentDto) => payment.status === 'Successful' || payment.status === 'Pending',
    },
    {
      label: 'تحديث الحالة',
      icon: '🔄',
      color: 'blue' as const,
      onClick: handleUpdateStatus,
      show: (payment: PaymentDto) => payment.status === 'Pending',
    },
  ];

  if (paymentsError) {
    return (
      <div className="bg-white rounded-lg shadow-sm">
        <EmptyState
          icon="⚠️"
          title="خطأ في تحميل البيانات"
          description="حدث خطأ أثناء تحميل بيانات المدفوعات. يرجى المحاولة مرة أخرى."
          actionLabel="إعادة المحاولة"
          onAction={() => window.location.reload()}
          size="lg"
        />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة المدفوعات</h1>
            <p className="text-gray-600 mt-1">
              مراقبة وإدارة المدفوعات، الاستردادات، وإبطال المعاملات المالية
            </p>
          </div>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-6 gap-4">
        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-blue-100 p-2 rounded-lg">
              <span className="text-2xl">💰</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">إجمالي المدفوعات</p>
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
              <p className="text-sm font-medium text-gray-600">ناجحة</p>
              <p className="text-2xl font-bold text-green-600">{stats.successful}</p>
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
              <p className="text-sm font-medium text-gray-600">فاشلة</p>
              <p className="text-2xl font-bold text-red-600">{stats.failed}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-purple-100 p-2 rounded-lg">
              <span className="text-2xl">↩️</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">مستردة</p>
              <p className="text-2xl font-bold text-purple-600">{stats.refunded}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-emerald-100 p-2 rounded-lg">
              <span className="text-2xl">💵</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">إجمالي المبلغ</p>
              <p className="text-lg font-bold text-emerald-600">{stats.totalAmount.toLocaleString()} ر.ي</p>
            </div>
          </div>
        </div>
      </div>

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في المدفوعات (رقم المعاملة)..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Payments Table */}
      {!isLoadingPayments && filteredPayments.length === 0 ? (
        <div className="bg-white rounded-lg shadow-sm">
          <EmptyState
            icon="💳"
            title="لا توجد مدفوعات"
            description="لم يتم العثور على أي مدفوعات تطابق معايير البحث والفلترة الحالية."
            actionLabel="إعادة تعيين الفلاتر"
            onAction={handleResetFilters}
          />
        </div>
      ) : (
        <DataTable
          data={filteredPayments}
          columns={columns}
          loading={isLoadingPayments}
          pagination={{
            current: currentPage,
            total: paymentsData?.totalCount || 0,
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
      )}

      {/* Payment Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedPayment(null);
        }}
        title="تفاصيل الدفعة"
        size="lg"
      >
        {selectedPayment && (
          <div className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف الدفعة</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedPayment.id}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">رقم المعاملة</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedPayment.transactionId}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف الحجز</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedPayment.bookingId}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">المبلغ</label>
                <p className="mt-1 text-sm text-gray-900 font-medium">
                  {selectedPayment.amount.amount} {selectedPayment.amount.currency}
                </p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">طريقة الدفع</label>
                <p className="mt-1 text-sm text-gray-900">{getMethodLabel(selectedPayment.method)}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">حالة الدفع</label>
                <span className={`mt-1 inline-flex px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(selectedPayment.status)}`}>
                  {getStatusLabel(selectedPayment.status)}
                </span>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">تاريخ الدفع</label>
                <p className="mt-1 text-sm text-gray-900">
                  {new Date(selectedPayment.paymentDate).toLocaleString('ar-SA')}
                </p>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Refund Payment Modal */}
      <Modal
        isOpen={showRefundModal}
        onClose={() => {
          setShowRefundModal(false);
          setSelectedPayment(null);
        }}
        title="استرداد الدفعة"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowRefundModal(false);
                setSelectedPayment(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => refundPayment.mutate(refundForm)}
              disabled={refundPayment.status === 'pending' || !refundForm.refundReason.trim()}
              className="px-4 py-2 bg-orange-600 text-white rounded-md hover:bg-orange-700 disabled:opacity-50"
            >
              {loading.refundPayment ? 'جارٍ الاسترداد...' : 'استرداد'}
            </button>
          </div>
        }
      >
        {selectedPayment && (
          <div className="space-y-4">
            <div className="bg-orange-50 border border-orange-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-orange-400 text-xl">⚠️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-orange-800">
                    تأكيد استرداد الدفعة
                  </h3>
                  <p className="mt-2 text-sm text-orange-700">
                    سيتم استرداد <strong>{refundForm.refundAmount.amount} {refundForm.refundAmount.currency}</strong> من المعاملة <strong>{selectedPayment.transactionId}</strong>.
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                مبلغ الاسترداد
              </label>
              <CurrencyInput
                value={refundForm.refundAmount.amount}
                currency={refundForm.refundAmount.currency}
                onValueChange={(amount, currency) => 
                  setRefundForm(prev => ({ 
                    ...prev, 
                    refundAmount: { 
                      ...prev.refundAmount,
                      amount, 
                      currency 
                    }
                  }))
                }
                direction="rtl"
                placeholder="0.00"
                required={true}
                min={0}
                max={selectedPayment?.amount.amount}
                showSymbol={true}
                supportedCurrencies={currencyCodes}
                disabled={false}
              />
              {selectedPayment && (
                <p className="mt-1 text-xs text-gray-500">
                  الحد الأقصى للاسترداد: {selectedPayment.amount.amount} {selectedPayment.amount.currency}
                </p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                سبب الاسترداد *
              </label>
              <textarea
                rows={3}
                value={refundForm.refundReason}
                onChange={(e) => setRefundForm(prev => ({ ...prev, refundReason: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                placeholder="أدخل سبب الاسترداد..."
              />
            </div>
          </div>
        )}
      </Modal>

      {/* Void Payment Modal */}
      <Modal
        isOpen={showVoidModal}
        onClose={() => {
          setShowVoidModal(false);
          setSelectedPayment(null);
        }}
        title="إبطال الدفعة"
        size="md"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowVoidModal(false);
                setSelectedPayment(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => voidPayment.mutate({ paymentId: selectedPayment!.id })}
              disabled={voidPayment.status === 'pending'}
              className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50"
            >
              {voidPayment.status === 'pending' ? 'جارٍ الإبطال...' : 'إبطال الدفعة'}
            </button>
          </div>
        }
      >
        {selectedPayment && (
          <div className="space-y-4">
            <div className="bg-red-50 border border-red-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-red-400 text-xl">⚠️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-red-800">
                    تأكيد إبطال الدفعة
                  </h3>
                  <p className="mt-2 text-sm text-red-700">
                    هل أنت متأكد من إبطال الدفعة <strong>{selectedPayment.transactionId}</strong> بمبلغ <strong>{selectedPayment.amount.amount} {selectedPayment.amount.currency}</strong>؟
                    هذا الإجراء لا يمكن التراجع عنه.
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Update Status Modal */}
      <Modal
        isOpen={showStatusModal}
        onClose={() => {
          setShowStatusModal(false);
          setSelectedPayment(null);
        }}
        title="تحديث حالة الدفعة"
        size="md"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowStatusModal(false);
                setSelectedPayment(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => updatePaymentStatus.mutate({ paymentId: statusForm.paymentId, data: statusForm })}
              disabled={updatePaymentStatus.status === 'pending'}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {updatePaymentStatus.status === 'pending' ? 'جارٍ التحديث...' : 'تحديث'}
            </button>
          </div>
        }
      >
        {selectedPayment && (
          <div className="space-y-4">
            <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-blue-400 text-xl">ℹ️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-blue-800">
                    تحديث حالة الدفعة
                  </h3>
                  <p className="mt-2 text-sm text-blue-700">
                    المعاملة: <strong>{selectedPayment.transactionId}</strong><br/>
                    الحالة الحالية: <strong>{getStatusLabel(selectedPayment.status)}</strong>
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                الحالة الجديدة
              </label>
              <select
                value={statusForm.newStatus}
                onChange={(e) => setStatusForm(prev => ({ ...prev, newStatus: e.target.value as PaymentStatus }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
                <option value="Successful">ناجح</option>
                <option value="Failed">فاشل</option>
                <option value="Pending">معلق</option>
                <option value="Refunded">مسترد</option>
                <option value="Voided">ملغي</option>
                <option value="PartiallyRefunded">مسترد جزئياً</option>
              </select>
            </div>
          </div>
        )}
      </Modal>

      {/* Confirm Dialog */}
      {confirmDialog && (
        <ConfirmDialog
          isOpen={confirmDialog.isOpen}
          onClose={hideConfirmDialog}
          onConfirm={() => {
            confirmDialog.onConfirm();
            hideConfirmDialog();
          }}
          title={confirmDialog.title}
          message={confirmDialog.message}
          type={confirmDialog.type}
          loading={loading.voidPayment}
        />
      )}
    </div>
  );
};

export default AdminPayments;