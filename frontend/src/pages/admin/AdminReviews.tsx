import { useState } from 'react';
import { useAdminReviews } from '../../hooks/useAdminReviews';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import { useAdminUsers } from '../../hooks/useAdminUsers';
import DataTable, { type Column } from '../../components/common/DataTable';
import { isProfane, getProfanityScore, PROFANITY_THRESHOLD } from '../../utils/profanityFilter';
import { timeAgo } from '../../utils/timeAgo';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import Modal from '../../components/common/Modal';
import PropertySelector from '../../components/selectors/PropertySelector';
import UnitSelector from '../../components/selectors/UnitSelector';
import UserSelector from '../../components/selectors/UserSelector';
import type {
  ReviewDto,
  GetReviewsByPropertyQuery,
  GetReviewsByUserQuery,
  GetPendingReviewsQuery,
  ReviewImageDto,
  ApproveReviewCommand,
  DeleteReviewCommand,
  RespondToReviewCommand
} from '../../types/review.types';
import type { GetAllPropertiesQuery } from '../../types/property.types';

const AdminReviews = () => {
  // removed erroneous hook invocation that returned queryClient
  
  // State for search and filters
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({
    status: 'pending',
    minRating: '',
    maxRating: '',
    hasImages: undefined,
    propertyId: '',
    userId: '',
    startDate: '',
    endDate: '',
    unitId: '',
  });

  // State for modals
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showApprovalModal, setShowApprovalModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [showImagesModal, setShowImagesModal] = useState(false);
  const [selectedReview, setSelectedReview] = useState<ReviewDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);
  const [selectedImages, setSelectedImages] = useState<ReviewImageDto[]>([]);

  // State for response form
  const [responseForm, setResponseForm] = useState({
    responseText: '',
    ownerId: '',
  });

  // Build query params
  const queryParams: Record<string, any> = {
    status: filterValues.status,
    minRating: filterValues.minRating ? parseFloat(filterValues.minRating) : undefined,
    maxRating: filterValues.maxRating ? parseFloat(filterValues.maxRating) : undefined,
    hasImages: filterValues.hasImages,
    propertyId: filterValues.propertyId || undefined,
    unitId: filterValues.unitId || undefined,
    userId: filterValues.userId || undefined,
    reviewedAfter: filterValues.startDate || undefined,
    reviewedBefore: filterValues.endDate || undefined,
    pageNumber: currentPage,
    pageSize,
  };

  // استخدام الهوك لإدارة استعلامات وعمليات التقييمات
  const {
    reviewsData,
    isLoading: isLoadingReviews,
    error: reviewsError,
    approveReview,
    deleteReview,
  } = useAdminReviews(queryParams);
  // جلب بيانات الكيانات لفلتر الاختيار
  const {
    propertiesData,
    isLoading: isLoadingProperties,
  } = useAdminProperties({ pageNumber: 1, pageSize: 100 } as GetAllPropertiesQuery);
  const { usersData, isLoading: isLoadingUsers } = useAdminUsers({});

  // Helper functions
  const handleViewDetails = (review: ReviewDto) => {
    setSelectedReview(review);
    setShowDetailsModal(true);
  };

  const handleApprove = (review: ReviewDto) => {
    setSelectedReview(review);
    setShowApprovalModal(true);
  };

  const handleDelete = (review: ReviewDto) => {
    setSelectedReview(review);
    setShowDeleteModal(true);
  };

  const handleViewImages = (review: ReviewDto) => {
    setSelectedReview(review);
    setSelectedImages(review.images || []);
    setShowImagesModal(true);
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      status: 'pending',
      minRating: '',
      maxRating: '',
      hasImages: undefined,
      propertyId: '',
      userId: '',
      startDate: '',
      endDate: '',
      unitId: '',
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  // Calculate average rating
  const calculateAverageRating = (review: ReviewDto) => {
    const ratings = [review.cleanliness, review.service, review.location, review.value];
    const sum = ratings.reduce((a, b) => a + b, 0);
    return (sum / ratings.length).toFixed(1);
  };

  // Get star display
  const getStarDisplay = (rating: number) => {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 !== 0;
    const emptyStars = 5 - Math.ceil(rating);
    
    return (
      <div className="flex items-center">
        {'⭐'.repeat(fullStars)}
        {hasHalfStar && '⭐'}
        {'☆'.repeat(emptyStars)}
        <span className="text-sm text-gray-600 mr-1">({rating})</span>
      </div>
    );
  };

  // Get rating color
  const getRatingColor = (rating: number) => {
    if (rating >= 4.5) return 'text-green-600';
    if (rating >= 3.5) return 'text-yellow-600';
    if (rating >= 2.5) return 'text-orange-600';
    return 'text-red-600';
  };

  // Statistics calculation
  const stats = {
    total: Array.isArray(reviewsData) ? reviewsData.length : 0,
    pending: Array.isArray(reviewsData) ? reviewsData.filter(r => filterValues.status === 'pending').length : 0,
    approved: 0, // We'll simulate this
    withImages: Array.isArray(reviewsData) ? reviewsData.filter(r => r.images && r.images.length > 0).length : 0,
    averageRating: Array.isArray(reviewsData) && reviewsData.length > 0 
      ? (reviewsData.reduce((sum, r) => sum + parseFloat(calculateAverageRating(r)), 0) / reviewsData.length).toFixed(1)
      : '0.0',
  };

  // Filter options
  const filterOptions: FilterOption[] = [
    {
      key: 'status',
      label: 'حالة التقييم',
      type: 'select',
      options: [
        { value: 'pending', label: 'معلق للموافقة' },
        { value: 'approved', label: 'معتمد' },
        { value: 'rejected', label: 'مرفوض' },
        { value: 'all', label: 'جميع التقييمات' },
      ],
    },
    {
      key: 'minRating',
      label: 'الحد الأدنى للتقييم',
      type: 'number',
      placeholder: '1.0',
    },
    {
      key: 'maxRating',
      label: 'الحد الأقصى للتقييم',
      type: 'number',
      placeholder: '5.0',
    },
    {
      key: 'hasImages',
      label: 'يحتوي على صور',
      type: 'boolean',
    },
    {
      key: 'propertyId',
      label: 'الكيان',
      type: 'custom',
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
      key: 'unitId',
      label: 'الوحدة',
      type: 'custom',
      render: (value, onChange) => (
        <UnitSelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر الوحدة"
          propertyId={filterValues.propertyId}
          className="w-full"
          disabled={!filterValues.propertyId}
        />
      ),
    },
    {
      key: 'userId',
      label: 'المستخدم',
      type: 'custom',
      render: (value, onChange) => (
        <UserSelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر المستخدم"
          className="w-full"
        />
      ),
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
  ];

  // Table columns
  const columns: Column<ReviewDto>[] = [
    {
      key: 'propertyName',
      title: 'الكيان',
      sortable: true,
      render: (value: string) => (
        <span className="text-sm text-gray-900">{value}</span>
      ),
    },
    {
      key: 'userName',
      title: 'المستخدم',
      sortable: true,
      render: (value: string) => (
        <span className="text-sm text-gray-900">{value}</span>
      ),
    },
    {
      key: 'cleanliness',
      title: 'التقييم الإجمالي',
      render: (value: number, record: ReviewDto) => {
        const avgRating = parseFloat(calculateAverageRating(record));
        return (
          <div className="flex flex-col">
            {getStarDisplay(avgRating)}
            <div className="text-xs text-gray-500 mt-1">
              <span>نظافة: {record.cleanliness}</span> | 
              <span> خدمة: {record.service}</span> | 
              <span> موقع: {record.location}</span> | 
              <span> قيمة: {record.value}</span>
            </div>
          </div>
        );
      },
    },
    {
      key: 'comment',
      title: 'التعليق',
      render: (value: string) => {
        const tokens = value.split(/(\s+)/);
        return (
          <div className="max-w-xs" title={value}>
            <p className="text-sm text-gray-900">
              {tokens.map((t, i) => {
                const score = getProfanityScore(t);
                if (score >= PROFANITY_THRESHOLD) {
                  // Normalize score between 0 and 1
                  const norm = (score - PROFANITY_THRESHOLD) / (1 - PROFANITY_THRESHOLD);
                  // Gradient from orange (#FDBA74) to red (#DC2626)
                  const r = Math.round(253 + (220 - 253) * norm);
                  const g = Math.round(186 + (38 - 186) * norm);
                  const b = Math.round(116 + (38 - 116) * norm);
                  return (
                    <span key={i} style={{ color: `rgb(${r},${g},${b})`, fontWeight: 'bold' }}>
                      {t}
                    </span>
                  );
                }
                return <span key={i}>{t}</span>;
              })}
            </p>
          </div>
        );
      },
    },
    {
      key: 'images',
      title: 'الصور',
      render: (value: ReviewImageDto[]) => (
        <div className="flex items-center">
          {value && value.length > 0 ? (
            <div className="flex items-center">
              <span className="text-green-600 text-lg">📷</span>
              <span className="text-sm text-gray-600 mr-1">({value.length})</span>
            </div>
          ) : (
            <span className="text-gray-400 text-sm">لا توجد صور</span>
          )}
        </div>
      ),
    },
    {
      key: 'createdAt',
      title: 'تاريخ التقييم',
      sortable: true,
      render: (value: string) => (
        <span className="text-sm text-gray-500">
          {new Date(value).toLocaleDateString('ar-SA')} <span className="text-xs text-gray-400">({timeAgo(value)})</span>
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
      label: 'عرض الصور',
      icon: '📷',
      color: 'green' as const,
      onClick: handleViewImages,
      show: (review: ReviewDto) => review.images && review.images.length > 0,
    },
    {
      label: 'الموافقة',
      icon: '✅',
      color: 'green' as const,
      onClick: handleApprove,
      show: (review: ReviewDto) => filterValues.status === 'pending',
    },
    {
      label: 'حذف',
      icon: '🗑️',
      color: 'red' as const,
      onClick: handleDelete,
    },
  ];

  if (reviewsError) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-8 text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">خطأ في تحميل البيانات</h2>
        <p className="text-gray-600">حدث خطأ أثناء تحميل بيانات التقييمات. يرجى المحاولة مرة أخرى.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة التقييمات</h1>
            <p className="text-gray-600 mt-1">
              مراجعة وإدارة تقييمات العملاء، الموافقة على التقييمات المعلقة، وإدارة الصور المرفقة
            </p>
          </div>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-blue-100 p-2 rounded-lg">
              <span className="text-2xl">📝</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">إجمالي التقييمات</p>
              <p className="text-2xl font-bold text-gray-900">{stats.total}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-yellow-100 p-2 rounded-lg">
              <span className="text-2xl">⏳</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">معلقة للموافقة</p>
              <p className="text-2xl font-bold text-yellow-600">{stats.pending}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-green-100 p-2 rounded-lg">
              <span className="text-2xl">✅</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">معتمدة</p>
              <p className="text-2xl font-bold text-green-600">{stats.approved}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-purple-100 p-2 rounded-lg">
              <span className="text-2xl">📷</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">تحتوي على صور</p>
              <p className="text-2xl font-bold text-purple-600">{stats.withImages}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-amber-100 p-2 rounded-lg">
              <span className="text-2xl">⭐</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">متوسط التقييم</p>
              <p className={`text-2xl font-bold ${getRatingColor(parseFloat(stats.averageRating))}`}>
                {stats.averageRating}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في التقييمات (التعليق أو معرف الحجز)..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Reviews Table */}
      <DataTable
        data={Array.isArray(reviewsData) ? reviewsData : []}
        columns={columns}
        loading={isLoadingReviews}
        pagination={{
          current: currentPage,
          total: Array.isArray(reviewsData) ? reviewsData.length : 0,
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

      {/* Review Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedReview(null);
        }}
        title="تفاصيل التقييم"
        size="xl"
      >
        {selectedReview && (
          <div className="space-y-6">
            {/* Header with overall rating */}
            <div className="bg-gray-50 p-4 rounded-lg">
              <div className="flex justify-between items-start">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">
                    التقييم الإجمالي: {calculateAverageRating(selectedReview)}
                  </h3>
                  {getStarDisplay(parseFloat(calculateAverageRating(selectedReview)))}
                </div>
                <div className="text-sm text-gray-500">
                  {new Date(selectedReview.createdAt).toLocaleString('ar-SA')}
                </div>
              </div>
            </div>

            {/* Individual ratings */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div className="text-center p-3 bg-blue-50 rounded-lg">
                <div className="text-2xl font-bold text-blue-600">{selectedReview.cleanliness}</div>
                <div className="text-sm text-gray-600">النظافة</div>
                {getStarDisplay(selectedReview.cleanliness)}
              </div>
              <div className="text-center p-3 bg-green-50 rounded-lg">
                <div className="text-2xl font-bold text-green-600">{selectedReview.service}</div>
                <div className="text-sm text-gray-600">الخدمة</div>
                {getStarDisplay(selectedReview.service)}
              </div>
              <div className="text-center p-3 bg-purple-50 rounded-lg">
                <div className="text-2xl font-bold text-purple-600">{selectedReview.location}</div>
                <div className="text-sm text-gray-600">الموقع</div>
                {getStarDisplay(selectedReview.location)}
              </div>
              <div className="text-center p-3 bg-orange-50 rounded-lg">
                <div className="text-2xl font-bold text-orange-600">{selectedReview.value}</div>
                <div className="text-sm text-gray-600">القيمة مقابل المال</div>
                {getStarDisplay(selectedReview.value)}
              </div>
            </div>

            {/* Comment */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">تعليق العميل</label>
              <div className="bg-gray-50 p-4 rounded-lg">
                <p className="text-gray-900">
                  {selectedReview.comment.split(/(\s+)/).map((t, i) => {
                    const score = getProfanityScore(t);
                    if (score >= PROFANITY_THRESHOLD) {
                      const norm = (score - PROFANITY_THRESHOLD) / (1 - PROFANITY_THRESHOLD);
                      const r = Math.round(253 + (220 - 253) * norm);
                      const g = Math.round(186 + (38 - 186) * norm);
                      const b = Math.round(116 + (38 - 116) * norm);
                      return (
                        <span key={i} style={{ color: `rgb(${r},${g},${b})`, fontWeight: 'bold' }}>
                          {t}
                        </span>
                      );
                    }
                    return <span key={i}>{t}</span>;
                  })}
                </p>
              </div>
            </div>

            {/* Review metadata */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف التقييم</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedReview.id}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف الحجز</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedReview.bookingId}</p>
              </div>
            </div>

            {/* Images preview */}
            {selectedReview.images && selectedReview.images.length > 0 && (
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  الصور المرفقة ({selectedReview.images.length})
                </label>
                <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
                  {selectedReview.images.slice(0, 4).map((image, index) => (
                    <div key={image.id} className="relative">
                      <img
                        src={image.url}
                        alt={image.altText}
                        className="w-full h-20 object-cover rounded-lg"
                      />
                      {index === 3 && selectedReview.images!.length > 4 && (
                        <div className="absolute inset-0 bg-black bg-opacity-50 rounded-lg flex items-center justify-center">
                          <span className="text-white font-medium">
                            +{selectedReview.images!.length - 4}
                          </span>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
                <button
                  onClick={() => handleViewImages(selectedReview)}
                  className="mt-2 text-blue-600 text-sm hover:text-blue-800"
                >
                  عرض جميع الصور
                </button>
              </div>
            )}
          </div>
        )}
      </Modal>

      {/* Images Gallery Modal */}
      <Modal
        isOpen={showImagesModal}
        onClose={() => {
          setShowImagesModal(false);
          setSelectedImages([]);
          setSelectedReview(null);
        }}
        title="صور التقييم"
        size="xl"
      >
        <div className="space-y-4">
          {selectedImages.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {selectedImages.map((image) => (
                <div key={image.id} className="space-y-2">
                  <img
                    src={image.url}
                    alt={image.altText}
                    className="w-full h-48 object-cover rounded-lg"
                  />
                  <div className="text-sm">
                    <p className="font-medium text-gray-900">{image.caption}</p>
                    <p className="text-gray-500">{image.name}</p>
                    <p className="text-gray-400">
                      {(image.sizeBytes / 1024 / 1024).toFixed(2)} MB
                    </p>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-8 text-gray-500">
              لا توجد صور لعرضها
            </div>
          )}
        </div>
      </Modal>

      {/* Approval Modal */}
      <Modal
        isOpen={showApprovalModal}
        onClose={() => {
          setShowApprovalModal(false);
          setSelectedReview(null);
        }}
        title="الموافقة على التقييم"
        size="md"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowApprovalModal(false);
                setSelectedReview(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => approveReview.mutate(selectedReview!.id)}
              disabled={approveReview.status === 'pending'}
              className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50"
            >
              {approveReview.status === 'pending' ? 'جارٍ الموافقة...' : 'الموافقة على التقييم'}
            </button>
          </div>
        }
      >
        {selectedReview && (
          <div className="space-y-4">
            <div className="bg-green-50 border border-green-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-green-400 text-xl">✅</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-green-800">
                    تأكيد الموافقة على التقييم
                  </h3>
                  <p className="mt-2 text-sm text-green-700">
                    سيتم الموافقة على التقييم وإتاحته للعرض العام. هل أنت متأكد من المتابعة؟
                  </p>
                </div>
              </div>
            </div>
            
            <div className="bg-gray-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">معاينة التقييم:</p>
              <div className="mt-2">
                <div className="flex items-center">
                  {getStarDisplay(parseFloat(calculateAverageRating(selectedReview)))}
                </div>
                <p className="text-gray-900 mt-2">{selectedReview.comment}</p>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Delete Modal */}
      <Modal
        isOpen={showDeleteModal}
        onClose={() => {
          setShowDeleteModal(false);
          setSelectedReview(null);
        }}
        title="حذف التقييم"
        size="md"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowDeleteModal(false);
                setSelectedReview(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => deleteReview.mutate(selectedReview!.id)}
              disabled={deleteReview.status === 'pending'}
              className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50"
            >
              {deleteReview.status === 'pending' ? 'جارٍ الحذف...' : 'حذف التقييم'}
            </button>
          </div>
        }
      >
        {selectedReview && (
          <div className="space-y-4">
            <div className="bg-red-50 border border-red-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-red-400 text-xl">⚠️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-red-800">
                    تأكيد حذف التقييم
                  </h3>
                  <p className="mt-2 text-sm text-red-700">
                    سيتم حذف التقييم نهائياً ولن يمكن استرداده. هذا الإجراء لا يمكن التراجع عنه.
                  </p>
                </div>
              </div>
            </div>
            
            <div className="bg-gray-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">التقييم المراد حذفه:</p>
              <div className="mt-2">
                <div className="flex items-center">
                  {getStarDisplay(parseFloat(calculateAverageRating(selectedReview)))}
                </div>
                <p className="text-gray-900 mt-2">{selectedReview.comment}</p>
              </div>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default AdminReviews;