import { useState } from 'react';
import { useAdminReports, useReportStats } from '../../hooks/useAdminReports';
import DataTable, { type Column } from '../../components/common/DataTable';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import Modal from '../../components/common/Modal';
import UserSelector from '../../components/selectors/UserSelector';
import PropertySelector from '../../components/selectors/PropertySelector';
import { 
  LoadingSpinner, 
  StatusBadge, 
  ActionButton, 
  ConfirmDialog, 
  EmptyState, 
  Tooltip 
} from '../../components/ui';
import { useUXHelpers } from '../../hooks/useUXHelpers';
import type { ReportDto } from '../../types/report.types';

const AdminReports = () => {
  // UX Helpers
  const { loading, executeWithFeedback, showConfirmDialog, confirmDialog, hideConfirmDialog, copyToClipboard } = useUXHelpers();
  
  // State for search and filters
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({
    reporterUserId: '',
    reportedUserId: '',
    reportedPropertyId: '',
    reason: '',
    status: '',
    fromDate: '',
    toDate: '',
  });

  // State for modals
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showActionModal, setShowActionModal] = useState(false);
  const [selectedReport, setSelectedReport] = useState<ReportDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);

  // State for action form
  const [actionForm, setActionForm] = useState({
    action: 'review',
    actionNote: '',
    adminId: 'admin-user-id' // This should come from auth context
  });

  // Build query params
  const queryParams = {
    pageNumber: currentPage,
    pageSize,
    searchTerm: searchTerm || undefined,
    reporterUserId: filterValues.reporterUserId || undefined,
    reportedUserId: filterValues.reportedUserId || undefined,
    reportedPropertyId: filterValues.reportedPropertyId || undefined,
    reason: filterValues.reason || undefined,
    status: filterValues.status || undefined,
    fromDate: filterValues.fromDate || undefined,
    toDate: filterValues.toDate || undefined,
  };

  // Hooks
  const {
    reportsData,
    isLoading: isLoadingReports,
    error: reportsError,
    createReport,
    updateReport,
    deleteReport,
    takeReportAction,
    bulkActionReports,
    refetch
  } = useAdminReports(queryParams);

  const { data: stats, isLoading: isLoadingStats } = useReportStats();

  // Helper functions
  const handleViewDetails = (report: ReportDto) => {
    setSelectedReport(report);
    setShowDetailsModal(true);
  };

  const handleTakeAction = (report: ReportDto) => {
    setSelectedReport(report);
    setActionForm({
      action: 'review',
      actionNote: '',
      adminId: 'admin-user-id'
    });
    setShowActionModal(true);
  };

  const handleDeleteReport = (report: ReportDto) => {
    showConfirmDialog({
      title: 'حذف البلاغ',
      message: `هل أنت متأكد من حذف البلاغ حول "${report.reason}"؟ هذا الإجراء لا يمكن التراجع عنه.`,
      type: 'danger',
      onConfirm: () => {
        executeWithFeedback(
          () => deleteReport.mutateAsync({ id: report.id, reason: 'تم الحذف من قبل الإدارة' }),
          {
            loadingKey: 'deleteReport',
            successMessage: 'تم حذف البلاغ بنجاح',
            errorMessage: 'فشل في حذف البلاغ'
          }
        );
      }
    });
  };

  const handleActionSubmit = async () => {
    if (!selectedReport || !actionForm.actionNote.trim()) {
      return;
    }

    await executeWithFeedback(
      () => takeReportAction.mutateAsync({
        id: selectedReport.id,
        action: actionForm.action as any,
        actionNote: actionForm.actionNote,
        adminId: actionForm.adminId
      }),
      {
        loadingKey: 'takeReportAction',
        successMessage: 'تم تنفيذ الإجراء بنجاح',
        errorMessage: 'فشل في تنفيذ الإجراء',
        onSuccess: () => {
          setShowActionModal(false);
          setSelectedReport(null);
          setActionForm({
            action: 'review',
            actionNote: '',
            adminId: 'admin-user-id'
          });
        }
      }
    );
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      reporterUserId: '',
      reportedUserId: '',
      reportedPropertyId: '',
      reason: '',
      status: '',
      fromDate: '',
      toDate: '',
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  // Get report severity color
  const getReportSeverityColor = (reason: string) => {
    const severityMap = {
      'انتهاك الشروط': 'text-red-600',
      'سلوك غير لائق': 'text-orange-600',
      'محتوى مضلل': 'text-yellow-600',
      'مشكلة فنية': 'text-blue-600',
      'أخرى': 'text-gray-600'
    };
    
    for (const [key, color] of Object.entries(severityMap)) {
      if (reason.includes(key)) return color;
    }
    return 'text-gray-600';
  };

  // Filter options
  const filterOptions: FilterOption[] = [
    {
      key: 'reporterUserId',
      label: 'المبلغ',
      type: 'custom',
      render: (value: string, onChange: (value: any) => void) => (
        <UserSelector
          value={value}
          onChange={(userId) => onChange(userId)}
          placeholder="اختر المستخدم المبلغ"
          className="w-full"
        />
      ),
    },
    {
      key: 'reportedUserId',
      label: 'المبلغ عنه',
      type: 'custom',
      render: (value: string, onChange: (value: any) => void) => (
        <UserSelector
          value={value}
          onChange={(userId) => onChange(userId)}
          placeholder="اختر المستخدم المبلغ عنه"
          className="w-full"
        />
      ),
    },
    {
      key: 'reportedPropertyId',
      label: 'الكيان المبلغ عنه',
      type: 'custom',
      render: (value: string, onChange: (value: any) => void) => (
        <PropertySelector
          value={value}
          onChange={(propertyId) => onChange(propertyId)}
          placeholder="اختر الكيان"
          className="w-full"
        />
      ),
    },
    {
      key: 'reason',
      label: 'سبب البلاغ',
      type: 'select',
      options: [
        { value: 'انتهاك الشروط', label: 'انتهاك الشروط' },
        { value: 'سلوك غير لائق', label: 'سلوك غير لائق' },
        { value: 'محتوى مضلل', label: 'محتوى مضلل' },
        { value: 'مشكلة فنية', label: 'مشكلة فنية' },
        { value: 'أخرى', label: 'أخرى' },
      ],
    },
    {
      key: 'status',
      label: 'حالة البلاغ',
      type: 'select',
      options: [
        { value: 'pending', label: 'معلق' },
        { value: 'reviewed', label: 'تمت المراجعة' },
        { value: 'resolved', label: 'تم الحل' },
        { value: 'dismissed', label: 'تم الرفض' },
        { value: 'escalated', label: 'تم التصعيد' },
      ],
    },
    {
      key: 'fromDate',
      label: 'من تاريخ',
      type: 'date',
    },
    {
      key: 'toDate',
      label: 'إلى تاريخ',
      type: 'date',
    },
  ];

  // Table columns
  const columns: Column<ReportDto>[] = [
    {
      key: 'id',
      title: 'معرف البلاغ',
      render: (value: string) => (
        <Tooltip content={`انقر للنسخ: ${value}`}>
          <button
            onClick={() => copyToClipboard(value, 'تم نسخ معرف البلاغ')}
            className="font-mono text-sm text-gray-600 hover:text-blue-600 transition-colors"
          >
            {value.substring(0, 8)}...
          </button>
        </Tooltip>
      ),
    },
    {
      key: 'reason',
      title: 'سبب البلاغ',
      render: (value: string) => (
        <div className="flex items-center space-x-2 space-x-reverse">
          <span className={`text-sm font-medium ${getReportSeverityColor(value)}`}>
            {value}
          </span>
        </div>
      ),
    },
    {
      key: 'description',
      title: 'الوصف',
      render: (value: string) => (
        <div className="max-w-md">
          <p className="text-sm text-gray-600 truncate">{value}</p>
        </div>
      ),
    },
    {
      key: 'reporterUserName',
      title: 'المبلغ',
      render: (value: string) => (
        <span className="text-sm text-gray-900 font-medium">{value}</span>
      ),
    },
    {
      key: 'reportedUserName',
      title: 'المبلغ عنه',
      render: (value: string) => (
        <span className="text-sm text-gray-900">{value || '-'}</span>
      ),
    },
    {
      key: 'reportedPropertyName',
      title: 'الكيان المبلغ عنه',
      render: (value: string) => (
        <span className="text-sm text-gray-900">{value || '-'}</span>
      ),
    },
    {
      key: 'createdAt',
      title: 'تاريخ البلاغ',
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
      label: 'اتخاذ إجراء',
      icon: '⚖️',
      color: 'green' as const,
      onClick: handleTakeAction,
    },
    {
      label: 'حذف البلاغ',
      icon: '🗑️',
      color: 'red' as const,
      onClick: handleDeleteReport,
    },
  ];

  if (reportsError) {
    return (
      <div className="bg-white rounded-lg shadow-sm">
        <EmptyState
          icon="⚠️"
          title="خطأ في تحميل البيانات"
          description="حدث خطأ أثناء تحميل بيانات البلاغات. يرجى المحاولة مرة أخرى."
          actionLabel="إعادة المحاولة"
          onAction={() => refetch()}
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
            <h1 className="text-2xl font-bold text-gray-900">إدارة البلاغات</h1>
            <p className="text-gray-600 mt-1">
              مراجعة ومعالجة البلاغات المقدمة من المستخدمين حول المحتوى والسلوك
            </p>
          </div>
        </div>
      </div>

      {/* Statistics Cards */}
      {isLoadingStats ? (
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          {[1, 2, 3, 4, 5].map((i) => (
            <div key={i} className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
              <LoadingSpinner size="md" text="جاري التحميل..." />
            </div>
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <div className="bg-blue-100 p-2 rounded-lg">
                <span className="text-2xl">📋</span>
              </div>
              <div className="mr-3">
                <p className="text-sm font-medium text-gray-600">إجمالي البلاغات</p>
                <p className="text-2xl font-bold text-gray-900">{stats?.totalReports || 0}</p>
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
                <p className="text-2xl font-bold text-yellow-600">{stats?.pendingReports || 0}</p>
              </div>
            </div>
          </div>

          <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <div className="bg-green-100 p-2 rounded-lg">
                <span className="text-2xl">✅</span>
              </div>
              <div className="mr-3">
                <p className="text-sm font-medium text-gray-600">محلولة</p>
                <p className="text-2xl font-bold text-green-600">{stats?.resolvedReports || 0}</p>
              </div>
            </div>
          </div>

          <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <div className="bg-gray-100 p-2 rounded-lg">
                <span className="text-2xl">❌</span>
              </div>
              <div className="mr-3">
                <p className="text-sm font-medium text-gray-600">مرفوضة</p>
                <p className="text-2xl font-bold text-gray-600">{stats?.dismissedReports || 0}</p>
              </div>
            </div>
          </div>

          <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
            <div className="flex items-center">
              <div className="bg-red-100 p-2 rounded-lg">
                <span className="text-2xl">🚨</span>
              </div>
              <div className="mr-3">
                <p className="text-sm font-medium text-gray-600">مصعدة</p>
                <p className="text-2xl font-bold text-red-600">{stats?.escalatedReports || 0}</p>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Reports by Category Chart */}
      {stats?.reportsByCategory && (
        <div className="bg-white rounded-lg shadow-sm p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">البلاغات حسب الفئة</h3>
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
            {Object.entries(stats.reportsByCategory).map(([category, count]) => (
              <div key={category} className="text-center">
                <div className="bg-gray-100 rounded-lg p-4 mb-2">
                  <p className="text-2xl font-bold text-gray-900">{count}</p>
                </div>
                <p className="text-sm text-gray-600">{category}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في البلاغات (السبب، الوصف)..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Reports Table */}
      {!isLoadingReports && (reportsData?.items?.length || 0) === 0 ? (
        <div className="bg-white rounded-lg shadow-sm">
          <EmptyState
            icon="📋"
            title="لا توجد بلاغات"
            description="لم يتم العثور على أي بلاغات تطابق معايير البحث والفلترة الحالية."
            actionLabel="إعادة تعيين الفلاتر"
            onAction={handleResetFilters}
          />
        </div>
      ) : (
        <DataTable
          data={reportsData?.items || []}
          columns={columns}
          loading={isLoadingReports}
          pagination={{
            current: currentPage,
            total: reportsData?.totalCount || 0,
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

      {/* Report Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedReport(null);
        }}
        title="تفاصيل البلاغ"
        size="lg"
      >
        {selectedReport && (
          <div className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف البلاغ</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedReport.id}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">سبب البلاغ</label>
                <div className="mt-1">
                  <span className={`text-sm font-medium ${getReportSeverityColor(selectedReport.reason)}`}>
                    {selectedReport.reason}
                  </span>
                </div>
              </div>
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700">وصف البلاغ</label>
                <p className="mt-1 text-sm text-gray-900 bg-gray-50 p-3 rounded-md">{selectedReport.description}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">المستخدم المبلغ</label>
                <p className="mt-1 text-sm text-gray-900 font-medium">{selectedReport.reporterUserName}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">تاريخ البلاغ</label>
                <p className="mt-1 text-sm text-gray-900">
                  {new Date(selectedReport.createdAt).toLocaleString('ar-SA')}
                </p>
              </div>
              {selectedReport.reportedUserName && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">المستخدم المبلغ عنه</label>
                  <p className="mt-1 text-sm text-gray-900">{selectedReport.reportedUserName}</p>
                </div>
              )}
              {selectedReport.reportedPropertyName && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">الكيان المبلغ عنه</label>
                  <p className="mt-1 text-sm text-gray-900">{selectedReport.reportedPropertyName}</p>
                </div>
              )}
            </div>
          </div>
        )}
      </Modal>

      {/* Take Action Modal */}
      <Modal
        isOpen={showActionModal}
        onClose={() => {
          setShowActionModal(false);
          setSelectedReport(null);
        }}
        title="اتخاذ إجراء على البلاغ"
        size="md"
        footer={
          <div className="flex justify-end gap-3">
            <ActionButton
              onClick={() => {
                setShowActionModal(false);
                setSelectedReport(null);
              }}
              label="إلغاء"
              variant="secondary"
            />
            <ActionButton
              onClick={handleActionSubmit}
              label="تنفيذ الإجراء"
              variant="primary"
              loading={loading.takeReportAction}
              disabled={!actionForm.actionNote.trim()}
              icon="⚖️"
            />
          </div>
        }
      >
        {selectedReport && (
          <div className="space-y-4">
            <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-blue-400 text-xl">ℹ️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-blue-800">
                    معلومات البلاغ
                  </h3>
                  <p className="mt-2 text-sm text-blue-700">
                    <strong>السبب:</strong> {selectedReport.reason}<br/>
                    <strong>المبلغ:</strong> {selectedReport.reporterUserName}
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                نوع الإجراء
              </label>
              <select
                value={actionForm.action}
                onChange={(e) => setActionForm(prev => ({ ...prev, action: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
                <option value="review">مراجعة البلاغ</option>
                <option value="resolve">حل البلاغ</option>
                <option value="dismiss">رفض البلاغ</option>
                <option value="escalate">تصعيد البلاغ</option>
                <option value="investigate">فتح تحقيق</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                ملاحظات الإجراء *
              </label>
              <textarea
                rows={4}
                value={actionForm.actionNote}
                onChange={(e) => setActionForm(prev => ({ ...prev, actionNote: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                placeholder="أدخل تفاصيل الإجراء المتخذ وأي ملاحظات إضافية..."
              />
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
          loading={loading.deleteReport}
        />
      )}
    </div>
  );
};

export default AdminReports;