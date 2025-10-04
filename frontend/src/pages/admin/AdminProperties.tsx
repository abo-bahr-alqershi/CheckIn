import { useState } from 'react';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import { useNavigate } from 'react-router-dom';
import { useAdminPropertyTypes } from '../../hooks/useAdminPropertyTypes';
import DataTable, { type Column } from '../../components/common/DataTable';
import CardView from '../../components/common/CardView';
import MapView from '../../components/common/MapView';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import ViewToggle, { type ViewType } from '../../components/common/ViewToggle';
import Modal from '../../components/common/Modal';
import UserSelector from '../../components/selectors/UserSelector';
import LocationSelector from '../../components/selectors/LocationSelector';
import ImageUpload from '../../components/inputs/ImageUpload';
import type { 
  PropertyDto, 
  CreatePropertyCommand, 
  UpdatePropertyCommand, 
  GetAllPropertiesQuery 
} from '../../types/property.types';
import type { PropertyImageDto } from '../../types/property-image.types';
import AdminSectionsService from '../../services/admin-sections.service';
import { AdminPropertiesService } from '../../services/admin-properties.service';
import { SectionTarget } from '../../types/enums';

// Extend PropertyDto to include coordinates for map view
interface PropertyWithLocation extends Omit<PropertyDto, 'latitude' | 'longitude'> {
  latitude?: number;
  longitude?: number;
}

const AdminProperties = () => {
  
  // State for view and search
  const [currentView, setCurrentView] = useState<ViewType>('table');
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({
    propertyTypeId: '',
    isApproved: undefined,
    starRatings: [],
    minPrice: '',
    maxPrice: '',
    hasActiveBookings: undefined,
  });

  // State for modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [selectedProperty, setSelectedProperty] = useState<PropertyDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);

  // State for assign to sections
  const [showAssignSectionsModal, setShowAssignSectionsModal] = useState(false);
  const [availablePropertySections, setAvailablePropertySections] = useState<any[]>([]);
  const [selectedSectionIds, setSelectedSectionIds] = useState<string[]>([]);

  // State for forms
  const [createForm, setCreateForm] = useState<CreatePropertyCommand>({
    name: '',
    address: '',
    propertyTypeId: '',
    ownerId: '',
    description: '',
    latitude: 0,
    longitude: 0,
    city: '',
    starRating: 3,
    images: [],
  });

  const [editForm, setEditForm] = useState<UpdatePropertyCommand>({
    propertyId: '',
    name: '',
    address: '',
    description: '',
    latitude: 0,
    longitude: 0,
    city: '',
    starRating: 3,
    images: [],
  });

  // Build query params
  const queryParams: GetAllPropertiesQuery = {
    pageNumber: currentPage,
    pageSize,
    searchTerm: searchTerm || undefined,
    propertyTypeId: filterValues.propertyTypeId || undefined,
    isApproved: filterValues.isApproved,
    starRatings: filterValues.starRatings.length > 0 ? filterValues.starRatings : undefined,
    minPrice: filterValues.minPrice || undefined,
    maxPrice: filterValues.maxPrice || undefined,
    hasActiveBookings: filterValues.hasActiveBookings,
  };

  // استخدام الهوك لإدارة بيانات الكيانات والعمليات
  const {
    propertiesData,
    pendingPropertiesData,
    isLoading: isLoadingProperties,
    error: propertiesError,
    createProperty,
    updateProperty,
    approveProperty,
    rejectProperty,
    deleteProperty,
  } = useAdminProperties(queryParams);
  const navigate = useNavigate();
  const { propertyTypesData, isLoading: typesLoading, error: typesError } = useAdminPropertyTypes({ pageNumber: 1, pageSize: 100 });

  const handleOpenGallery = (property: PropertyDto) => {
    navigate(`/admin/property-images/${property.id}`, { state: { propertyName: property.name } });
  };

  // تم حذف تعريفات الـ mutations المباشرة لاستخدام الهوك

  // Helper functions
  const resetCreateForm = () => {
    setCreateForm({
      name: '',
      address: '',
      propertyTypeId: '',
      ownerId: '',
      description: '',
      latitude: 0,
      longitude: 0,
      city: '',
      starRating: 3,
      images: [],
    });
  };

  const openAssignSections = async (property: PropertyDto) => {
    setSelectedProperty(property);
    setSelectedSectionIds([]);
    const res = await AdminSectionsService.getSections({ pageNumber: 1, pageSize: 100, target: SectionTarget.PROPERTIES });
    setAvailablePropertySections(res.items as any);
    setShowAssignSectionsModal(true);
  };

  const confirmAssignSections = async () => {
    if (!selectedProperty) return;
    await AdminPropertiesService.addPropertyToSections(selectedProperty.id, selectedSectionIds);
    setShowAssignSectionsModal(false);
  };

  const handleEdit = (property: PropertyDto) => {
    setSelectedProperty(property);
    setEditForm({
      propertyId: property.id,
      name: property.name,
      address: property.address,
      description: property.description,
      latitude: property.latitude,
      longitude: property.longitude,
      city: property.city,
      starRating: property.starRating,
      images: property.images?.map(img => img.url) || [],
    });
    setShowEditModal(true);
  };

  const handleViewDetails = (property: PropertyDto) => {
    setSelectedProperty(property);
    setShowDetailsModal(true);
  };

  const handleApprove = (property: PropertyDto) => {
    if (confirm(`هل أنت متأكد من الموافقة على الكيان "${property.name}"؟`)) {
      approveProperty.mutate(property.id);
    }
  };

  const handleReject = (property: PropertyDto) => {
    if (confirm(`هل أنت متأكد من رفض الكيان "${property.name}"؟`)) {
      rejectProperty.mutate(property.id);
    }
  };

  const handleDelete = (property: PropertyDto) => {
    if (confirm(`هل أنت متأكد من حذف الكيان "${property.name}"؟ هذا الإجراء لا يمكن التراجع عنه.`)) {
      deleteProperty.mutate(property.id);
    }
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      propertyTypeId: '',
      isApproved: undefined,
      starRatings: [],
      minPrice: '',
      maxPrice: '',
      hasActiveBookings: undefined,
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  // Filter options
  const filterOptions: FilterOption[] = [
    {
      key: 'propertyTypeId',
      label: 'نوع الكيان',
      type: 'select',
      options: [
        { value: 'hotel', label: 'فندق' },
        { value: 'resort', label: 'منتجع' },
        { value: 'apartment', label: 'شقة مفروشة' },
        { value: 'villa', label: 'فيلا' },
      ],
    },
    {
      key: 'isApproved',
      label: 'حالة الموافقة',
      type: 'boolean',
    },
    {
      key: 'starRatings',
      label: 'تقييم النجوم',
      type: 'select',
      options: [
        { value: '5', label: '5 نجوم' },
        { value: '4', label: '4 نجوم' },
        { value: '3', label: '3 نجوم' },
        { value: '2', label: '2 نجوم' },
        { value: '1', label: '1 نجمة' },
      ],
    },
    {
      key: 'minPrice',
      label: 'الحد الأدنى للسعر',
      type: 'number',
      placeholder: 'أدخل الحد الأدنى',
    },
    {
      key: 'maxPrice',
      label: 'الحد الأقصى للسعر',
      type: 'number',
      placeholder: 'أدخل الحد الأقصى',
    },
    {
      key: 'hasActiveBookings',
      label: 'يحتوي على حجوزات نشطة',
      type: 'boolean',
    },
  ];

  // Navigation handlers for availability and pricing management

  // Table columns
  const columns: Column<PropertyDto>[] = [
    {
      key: 'name',
      title: 'اسم الكيان',
      sortable: true,
      priority: 'high',
      mobileLabel: 'الاسم',
      render: (value: string, record: PropertyDto) => (
        <div className="flex flex-col">
          <span className="font-medium text-gray-900">{value}</span>
          <span className="text-sm text-gray-500">{record.typeName}</span>
        </div>
      ),
    },
    {
      key: 'ownerName',
      title: 'المالك',
      sortable: true,
      priority: 'medium',
      mobileLabel: 'المالك',
    },
    {
      key: 'city',
      title: 'المدينة',
      sortable: true,
      priority: 'medium',
      mobileLabel: 'المدينة',
    },
    {
      key: 'starRating',
      title: 'تقييم النجوم',
      priority: 'high',
      mobileLabel: 'التقييم',
      render: (value: number) => (
        <div className="flex items-center">
          <span className="ml-1">{value}</span>
          <span className="text-yellow-400">{'★'.repeat(value)}{'☆'.repeat(5 - value)}</span>
        </div>
      ),
    },
    {
      key: 'isApproved',
      title: 'حالة الموافقة',
      priority: 'high',
      mobileLabel: 'الحالة',
      render: (value: boolean) => (
        <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
          value ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'
        }`}>
          {value ? 'معتمد' : 'في انتظار الموافقة'}
        </span>
      ),
    },
    {
      key: 'createdAt',
      title: 'تاريخ الإنشاء',
      sortable: true,
      priority: 'low',
      mobileLabel: 'التاريخ',
      hideOnMobile: true,
      render: (value: string) => new Date(value).toLocaleDateString('ar-SA'),
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
      label: 'معرض الصور',
      icon: '🖼️',
      color: 'orange' as const,
      onClick: handleOpenGallery,
    },
    {
      label: 'تعديل',
      icon: '✏️',
      color: 'blue' as const,
      onClick: handleEdit,
    },
    {
      label: 'موافقة',
      icon: '✅',
      color: 'green' as const,
      onClick: handleApprove,
      show: (record: PropertyDto) => !record.isApproved,
    },
    {
      label: 'رفض',
      icon: '❌',
      color: 'red' as const,
      onClick: handleReject,
      show: (record: PropertyDto) => !record.isApproved,
    },
    {
      label: 'حذف',
      icon: '🗑️',
      color: 'red' as const,
      onClick: handleDelete,
    },
    {
      label: 'إسناد للأقسام',
      icon: '📚',
      color: 'blue' as const,
      onClick: (record) => openAssignSections(record),
    },
  ];

  // Helper function to get main image
  const getMainImage = (images?: PropertyImageDto[]) => {
    if (!images || images.length === 0) return null;
    // First try to find the main image
    const mainImage = images.find(img => img.isMain);
    // If no main image, use the first one
    return mainImage || images[0];
  };

  // Card renderer for card view
  const renderPropertyCard = (property: PropertyDto) => {
    const mainImage = getMainImage(property.images);
    
    return (
      <div className="bg-white border border-gray-200 rounded-lg shadow-sm hover:shadow-md transition-shadow overflow-hidden">
        {/* Property Image */}
        <div className="relative h-48 bg-gray-200">
          {mainImage ? (
            <img
              src={mainImage.url}
              alt={mainImage.altText || property.name}
              className="w-full h-full object-cover"
              onError={(e) => {
                // Fallback to placeholder if image fails to load
                const target = e.target as HTMLImageElement;
                target.style.display = 'none';
                target.nextElementSibling?.classList.remove('hidden');
              }}
            />
          ) : null}
          {/* Fallback placeholder */}
          <div className={`w-full h-full flex items-center justify-center bg-gray-100 ${mainImage ? 'hidden' : ''}`}>
            <div className="text-center">
              <span className="text-4xl text-gray-400">🏢</span>
              <p className="text-sm text-gray-500 mt-2">لا توجد صورة</p>
            </div>
          </div>
          
          {/* Status badge overlay */}
          <div className="absolute top-3 right-3">
            <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
              property.isApproved ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'
            }`}>
              {property.isApproved ? 'معتمد' : 'في انتظار الموافقة'}
            </span>
          </div>
          
          {/* Image count indicator */}
          {property.images && property.images.length > 0 && (
            <div className="absolute bottom-3 left-3 bg-black bg-opacity-60 text-white px-2 py-1 rounded text-xs">
              📸 {property.images.length} صورة
            </div>
          )}
        </div>

        <div className="p-6">
          <div className="mb-4">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">{property.name}</h3>
            <p className="text-sm text-gray-600">{property.address}</p>
          </div>
          
          <div className="space-y-2 mb-4">
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">المالك:</span>
              <span className="text-sm text-gray-900">{property.ownerName}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">المدينة:</span>
              <span className="text-sm text-gray-900">{property.city}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">التقييم:</span>
              <div className="flex items-center">
                <span className="text-sm text-gray-900 ml-1">{property.starRating}</span>
                <span className="text-yellow-400 text-sm">{'★'.repeat(property.starRating)}{'☆'.repeat(5 - property.starRating)}</span>
              </div>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">تاريخ الإنشاء:</span>
              <span className="text-sm text-gray-900">{new Date(property.createdAt).toLocaleDateString('ar-SA')}</span>
            </div>
          </div>

          <div className="space-y-2 mt-4">
            <div className="flex gap-2">
              <button
                onClick={() => handleViewDetails(property)}
                className="flex-1 px-3 py-2 text-sm bg-blue-50 text-blue-600 rounded-md hover:bg-blue-100 transition-colors"
              >
                👁️ عرض التفاصيل
              </button>
              <button
                onClick={() => handleEdit(property)}
                className="flex-1 px-3 py-2 text-sm bg-gray-50 text-gray-600 rounded-md hover:bg-gray-100 transition-colors"
              >
                ✏️ تعديل
              </button>
              {!property.isApproved && (
                <button
                  onClick={() => handleApprove(property)}
                  className="flex-1 px-3 py-2 text-sm bg-green-50 text-green-600 rounded-md hover:bg-green-100 transition-colors"
                >
                  ✅ موافقة
                </button>
              )}
            </div>
            
          </div>
        </div>
      </div>
    );
  };

  // Prepare properties with location data for map view
  const propertiesWithLocation: PropertyWithLocation[] = (propertiesData?.items || []).map(property => ({
    ...property,
    latitude: property.latitude,
    longitude: property.longitude
  }));

  if (propertiesError) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-8 text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">خطأ في تحميل البيانات</h2>
        <p className="text-gray-600">حدث خطأ أثناء تحميل بيانات الكيانات. يرجى المحاولة مرة أخرى.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة الكيانات</h1>
            <p className="text-gray-600 mt-1">
              مراجعة وموافقة الكيانات الجديدة وإدارة الكيانات المسجلة مع 3 طرق عرض مختلفة
            </p>
          </div>
          <div className="flex gap-3">
            <ViewToggle
              currentView={currentView}
              onViewChange={setCurrentView}
              availableViews={['table', 'cards', 'map']}
            />
            <button
              onClick={() => setShowCreateModal(true)}
              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              ➕ إضافة كيان جديد
            </button>
          </div>
        </div>
      </div>

      {/* Pending Properties Alert */}
      {pendingPropertiesData && pendingPropertiesData.totalCount > 0 && (
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
          <div className="flex items-center">
            <span className="text-yellow-600 text-xl ml-3">⚠️</span>
            <div>
              <h3 className="text-sm font-medium text-yellow-800">
                كيانات في انتظار الموافقة
              </h3>
              <p className="text-sm text-yellow-700 mt-1">
                يوجد {pendingPropertiesData.totalCount} كيان في انتظار المراجعة والموافقة
              </p>
            </div>
          </div>
        </div>
      )}

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في الكيانات..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Data Views */}
      {currentView === 'table' && (
        <DataTable
          data={propertiesData?.items || []}
          columns={columns}
          loading={isLoadingProperties}
          pagination={{
            current: currentPage,
            total: propertiesData?.totalCount || 0,
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
          mobileCardTitle={(record) => record.name}
          mobileCardSubtitle={(record) => `${record.typeName} - ${record.city}`}
          mobileCardImage={(record) => {
            const mainImage = record.images?.find(img => img.isMain);
            return mainImage ? mainImage.url : (record.images?.[0]?.url || '');
          }}
        />
      )}

      {currentView === 'cards' && (
        <CardView
          data={propertiesData?.items || []}
          loading={isLoadingProperties}
          renderCard={renderPropertyCard}
          emptyMessage="لا توجد كيانات للعرض"
          emptyIcon="🏢"
          columns={3}
          pagination={{
            current: currentPage,
            total: propertiesData?.totalCount || 0,
            pageSize,
            onChange: (page, size) => {
              setCurrentPage(page);
              setPageSize(size);
            },
          }}
        />
      )}

      {currentView === 'map' && (
        <MapView
          markers={propertiesWithLocation.map(property => ({
            id: property.id,
            name: property.name,
            address: property.address,
            description: `${property.city} - تقييم ${property.starRating} نجوم`,
            coordinates: property.latitude && property.longitude ? {
              latitude: property.latitude,
              longitude: property.longitude
            } : undefined,
            type: 'property' as const,
            color: property.isApproved ? '#10B981' : '#F59E0B',
            isApproved: property.isApproved,
            rating: property.starRating
          })).filter(marker => marker.coordinates)}
          onMarkerClick={(marker) => {
            const property = (propertiesData?.items || []).find(p => p.id === marker.id);
            if (property) handleViewDetails(property);
          }}
          emptyMessage="لا توجد كيانات بمواقع محددة لعرضها على الخريطة"
          height="600px"
          pagination={{
            current: currentPage,
            total: propertiesData?.totalCount || 0,
            pageSize,
            onChange: (page, size) => {
              setCurrentPage(page);
              setPageSize(size);
            },
          }}
        />
      )}

      {/* Create Property Modal */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="إضافة كيان جديد"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => setShowCreateModal(false)}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => createProperty.mutate(createForm, { onSuccess: () => { setShowCreateModal(false); resetCreateForm(); } })}
              disabled={createProperty.status === 'pending'}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {createProperty.status === 'pending' ? 'جارٍ الإضافة...' : 'إضافة'}
            </button>
          </div>
        }
      >
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              اسم الكيان *
            </label>
            <input
              type="text"
              value={createForm.name}
              onChange={(e) => setCreateForm(prev => ({ ...prev, name: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل اسم الكيان"
            />
          </div>
          
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              نوع الكيان *
            </label>
            <select
              value={createForm.propertyTypeId}
              onChange={(e) => setCreateForm(prev => ({ ...prev, propertyTypeId: e.target.value }))}
              disabled={typesLoading}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">اختر نوع الكيان</option>
              {propertyTypesData?.items.map(type => (
                <option key={type.id} value={type.id}>{type.name}</option>
              ))}
            </select>
            {typesError && <p className="mt-1 text-sm text-red-500">خطأ في تحميل أنواع الكيانات</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              مالك الكيان *
            </label>
            <UserSelector
              value={createForm.ownerId}
              onChange={(userId) => setCreateForm(prev => ({ ...prev, ownerId: userId }))}
              placeholder="اختر مالك الكيان"
              allowedRoles={['Owner']}
              required={true}
              className=""
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              المدينة *
            </label>
            <input
              type="text"
              value={createForm.city}
              onChange={(e) => setCreateForm(prev => ({ ...prev, city: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل اسم المدينة"
            />
          </div>

          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-1">
              العنوان *
            </label>
            <input
              type="text"
              value={createForm.address}
              onChange={(e) => setCreateForm(prev => ({ ...prev, address: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل العنوان الكامل"
            />
          </div>

          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              الموقع الجغرافي *
            </label>
            <LocationSelector
              latitude={createForm.latitude}
              longitude={createForm.longitude}
              onChange={(lat, lng, address) => {
                setCreateForm(prev => ({
                  ...prev,
                  latitude: lat,
                  longitude: lng
                }));
              }}
              placeholder="حدد موقع الكيان"
              required={true}
              showMap={true}
              allowManualInput={true}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              تقييم النجوم
            </label>
            <select
              value={createForm.starRating}
              onChange={(e) => setCreateForm(prev => ({ ...prev, starRating: Number(e.target.value) }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value={1}>1 نجمة</option>
              <option value={2}>2 نجمة</option>
              <option value={3}>3 نجوم</option>
              <option value={4}>4 نجوم</option>
              <option value={5}>5 نجوم</option>
            </select>
          </div>

          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-1">
              وصف الكيان
            </label>
            <textarea
              rows={3}
              value={createForm.description}
              onChange={(e) => setCreateForm(prev => ({ ...prev, description: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل وصف مفصل للكيان"
            />
          </div>

          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              صور الكيان
            </label>
            <ImageUpload
              value={createForm.images || []}
              onChange={(urls) => setCreateForm(prev => ({ ...prev, images: Array.isArray(urls) ? urls : [urls] }))}
              multiple={true}
              maxFiles={10}
              maxSize={5}
              showPreview={true}
              placeholder="اضغط لرفع صور الكيان أو اسحبها هنا"
              uploadEndpoint="/api/images/upload"
            />
          </div>
        </div>
      </Modal>

      {/* Edit Property Modal */}
      <Modal
        isOpen={showEditModal}
        onClose={() => {
          setShowEditModal(false);
          setSelectedProperty(null);
        }}
        title="تعديل بيانات الكيان"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowEditModal(false);
                setSelectedProperty(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => updateProperty.mutate({ 
                propertyId: editForm.propertyId, 
                data: editForm 
              })}
              disabled={updateProperty.status === 'pending'}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {updateProperty.status === 'pending' ? 'جارٍ التحديث...' : 'تحديث'}
            </button>
          </div>
        }
      >
        {selectedProperty && (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                اسم الكيان
              </label>
              <input
                type="text"
                value={editForm.name}
                onChange={(e) => setEditForm(prev => ({ ...prev, name: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                المدينة
              </label>
              <input
                type="text"
                value={editForm.city}
                onChange={(e) => setEditForm(prev => ({ ...prev, city: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                العنوان
              </label>
              <input
                type="text"
                value={editForm.address}
                onChange={(e) => setEditForm(prev => ({ ...prev, address: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                الموقع الجغرافي
              </label>
              <LocationSelector
                latitude={editForm.latitude || 0}
                longitude={editForm.longitude || 0}
                onChange={(lat, lng, address) => {
                  setEditForm(prev => ({
                    ...prev,
                    latitude: lat,
                    longitude: lng
                  }));
                }}
                placeholder="حدث موقع الكيان"
                showMap={true}
                allowManualInput={true}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                تقييم النجوم
              </label>
              <select
                value={editForm.starRating}
                onChange={(e) => setEditForm(prev => ({ ...prev, starRating: Number(e.target.value) }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
                <option value={1}>1 نجمة</option>
                <option value={2}>2 نجمة</option>
                <option value={3}>3 نجوم</option>
                <option value={4}>4 نجوم</option>
                <option value={5}>5 نجوم</option>
              </select>
            </div>

            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                وصف الكيان
              </label>
              <textarea
                rows={3}
                value={editForm.description}
                onChange={(e) => setEditForm(prev => ({ ...prev, description: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                صور الكيان
              </label>
              <ImageUpload
                value={editForm.images || selectedProperty?.images?.map(img => img.url) || []}
                onChange={(urls) => setEditForm(prev => ({ ...prev, images: Array.isArray(urls) ? urls : [urls] }))}
                multiple={true}
                maxFiles={10}
                maxSize={5}
                showPreview={true}
                placeholder="اضغط لرفع صور جديدة أو اسحبها هنا"
                uploadEndpoint="/api/upload/property-images"
              />
            </div>
          </div>
        )}
      </Modal>

      {/* Property Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedProperty(null);
        }}
        title="تفاصيل الكيان"
        size="xl"
      >
        {selectedProperty && (
          <div className="space-y-6">
            {/* Basic Info */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">اسم الكيان</label>
                <p className="mt-1 text-sm text-gray-900">{selectedProperty.name}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">نوع الكيان</label>
                <p className="mt-1 text-sm text-gray-900">{selectedProperty.typeName}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">المالك</label>
                <p className="mt-1 text-sm text-gray-900">{selectedProperty.ownerName}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">المدينة</label>
                <p className="mt-1 text-sm text-gray-900">{selectedProperty.city}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">تقييم النجوم</label>
                <div className="mt-1 flex items-center">
                  <span className="text-yellow-400">
                    {'★'.repeat(selectedProperty.starRating)}
                    {'☆'.repeat(5 - selectedProperty.starRating)}
                  </span>
                  <span className="mr-2 text-sm text-gray-600">
                    ({selectedProperty.starRating}/5)
                  </span>
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">حالة الموافقة</label>
                <span className={`mt-1 inline-flex px-2 py-1 text-xs font-medium rounded-full ${
                  selectedProperty.isApproved ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'
                }`}>
                  {selectedProperty.isApproved ? 'معتمد' : 'في انتظار الموافقة'}
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">العنوان</label>
              <p className="mt-1 text-sm text-gray-900">{selectedProperty.address}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">الوصف</label>
              <p className="mt-1 text-sm text-gray-900">{selectedProperty.description}</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">خط العرض</label>
                <p className="mt-1 text-sm text-gray-900">{selectedProperty.latitude}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">خط الطول</label>
                <p className="mt-1 text-sm text-gray-900">{selectedProperty.longitude}</p>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">تاريخ الإنشاء</label>
              <p className="mt-1 text-sm text-gray-900">
                {new Date(selectedProperty.createdAt).toLocaleString('ar-SA')}
              </p>
            </div>
          </div>
        )}
      </Modal>

      {/* Assign Sections Modal */}
      <Modal
        isOpen={showAssignSectionsModal}
        onClose={() => setShowAssignSectionsModal(false)}
        title="إضافة الكيان للأقسام"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => setShowAssignSectionsModal(false)}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={confirmAssignSections}
              disabled={selectedSectionIds.length === 0}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {selectedSectionIds.length === 0 ? 'لا يوجد أقسام متاحة' : 'حفظ'}
            </button>
          </div>
        }
      >
        <div className="space-y-3">
          <div className="grid grid-cols-2 gap-2 max-h-80 overflow-auto">
            {availablePropertySections.map((s: any) => (
              <label key={s.id} className="flex items-center gap-2">
                <input type="checkbox" checked={selectedSectionIds.includes(s.id)} onChange={(e) => {
                  setSelectedSectionIds(prev => e.target.checked ? [...prev, s.id] : prev.filter(x => x !== s.id));
                }} />
                <span>#{s.displayOrder} - {s.type}</span>
              </label>
            ))}
          </div>
          <div className="flex justify-end gap-2">
            <button className="btn" onClick={() => setShowAssignSectionsModal(false)}>إلغاء</button>
            <button className="btn btn-primary" onClick={confirmAssignSections}>حفظ</button>
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default AdminProperties;