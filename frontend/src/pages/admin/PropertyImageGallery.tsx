/**
 * صفحة إدارة معرض صور الكيانات - صفحة شاملة لإدارة صور الكيانات
 * Property Image Gallery Management Page - Comprehensive property images management page
 */

import React, { useState, useCallback } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { useImages, useImageStatistics } from '../../hooks/useImages';
import { ImageGallery } from '../../components/images/ImageGallery';
import { ImageUploader } from '../../components/images/ImageUploader';
import { AdvancedImageEditor } from '../../components/images/AdvancedImageEditor';
import ActionButton from '../../components/ui/ActionButton';
import LoadingSpinner from '../../components/ui/LoadingSpinner';
import StatusBadge from '../../components/ui/StatusBadge';
import DataTable from '../../components/common/DataTable';
import { useUXHelpers } from '../../hooks/useUXHelpers';
import type { Image as ImageType, ImageCategory } from '../../types/image.types';

/**
 * صفحة إدارة معرض صور الكيان
 * Property Image Gallery Management Page
 */
const PropertyImageGallery: React.FC = () => {
  const { propertyId } = useParams<{ propertyId: string }>();
  const location = useLocation();
  const navigate = useNavigate();
  const { loading, executeWithFeedback, showConfirmDialog, confirmDialog, hideConfirmDialog } = useUXHelpers();

  // الحالات المحلية - Local states
  const [activeTab, setActiveTab] = useState<'gallery' | 'upload' | 'statistics'>('gallery');
  const [selectedImages, setSelectedImages] = useState<string[]>([]);
  const [editingImage, setEditingImage] = useState<ImageType | null>(null);
  const [filterCategory, setFilterCategory] = useState<ImageCategory | 'all'>('all');
  const [viewMode, setViewMode] = useState<'grid' | 'table'>('grid');

  // Default property data - get name from navigation state if available
  const propertyName = (location.state as { propertyName?: string })?.propertyName;
  const property = { title: propertyName || `الكيان ${propertyId}` };
  const propertyLoading = false;
  const propertyError = null;

  // بيانات الصور - Images data
  const {
    images,
    isLoading: imagesLoading,
    error: imagesError,
    deleteImagesAsync,
    reorderImagesAsync,
    refetch: refetchImages
  } = useImages({
    propertyId,
    category: filterCategory === 'all' ? undefined : filterCategory,
    sortBy: 'order',
    sortOrder: 'asc'
  });

  // إحصائيات الصور - Image statistics
  const {
    data: statistics,
    isLoading: statisticsLoading,
    refetch: refetchStatistics
  } = useImageStatistics(propertyId);

  // معالجة اختيار الصور - Handle image selection
  const handleSelectionChange = useCallback((selectedIds: string[]) => {
    setSelectedImages(selectedIds);
  }, []);

  // معالجة حذف الصور المحددة - Handle delete selected images
  const handleDeleteSelected = useCallback(() => {
    if (selectedImages.length === 0) return;

    showConfirmDialog({
      title: 'حذف الصور المحددة',
      message: `هل أنت متأكد من حذف ${selectedImages.length} صورة؟`,
      type: 'danger',
      onConfirm: async () => {
        try {
          await executeWithFeedback(
            () => deleteImagesAsync({ imageIds: selectedImages }),
            {
              loadingKey: 'delete-images',
              successMessage: `تم حذف ${selectedImages.length} صورة بنجاح`,
              errorMessage: 'فشل في حذف الصور'
            }
          );
          setSelectedImages([]);
        } catch (error) {
          console.error('فشل في حذف الصور:', error);
        } finally {
          hideConfirmDialog();
        }
      }
    });
  }, [selectedImages, showConfirmDialog, hideConfirmDialog, executeWithFeedback, deleteImagesAsync]);

  // معالجة اكتمال الرفع - Handle upload completion
  const handleUploadComplete = useCallback((newImages: ImageType[]) => {
    refetchImages();
    refetchStatistics();
    setActiveTab('gallery');
  }, [refetchImages, refetchStatistics]);

  // معالجة تحرير الصورة - Handle image editing
  const handleEditImage = useCallback((image: ImageType) => {
    setEditingImage(image);
  }, []);

  // معالجة إغلاق محرر الصور - Handle close image editor
  const handleCloseEditor = useCallback(() => {
    setEditingImage(null);
  }, []);

  // معالجة حفظ تعديلات الصورة - Handle save image edits
  const handleSaveImageEdits = useCallback((editedImage: ImageType) => {
    refetchImages();
    setEditingImage(null);
  }, [refetchImages]);

  // أعمدة جدول الصور - Images table columns
  const imageTableColumns = [
    {
      key: 'thumbnail',
      title: 'الصورة',
      render: (_: any, image: ImageType) => (
        <div className="w-16 h-16 rounded overflow-hidden">
          <img
            src={image.thumbnails.small || image.url}
            alt={image.alt || image.filename}
            className="w-full h-full object-cover"
          />
        </div>
      )
    },
    {
      key: 'filename',
      title: 'اسم الملف',
      render: (_: any, image: ImageType) => (
        <div>
          <p className="font-medium">{image.filename}</p>
          <p className="text-sm text-gray-500">{image.alt}</p>
        </div>
      )
    },
    {
      key: 'category',
      title: 'الفئة',
      render: (_: any, image: ImageType) => (
        <StatusBadge
          status={
            image.category === 'exterior' ? 'خارجية' :
            image.category === 'interior' ? 'داخلية' :
            image.category === 'amenity' ? 'المرافق' :
            image.category === 'floor_plan' ? 'مخطط الطابق' :
            image.category === 'documents' ? 'المستندات' :
            'معرض عام'
          }
        />
      )
    },
    {
      key: 'size',
      title: 'الحجم / الأبعاد',
      render: (_: any, image: ImageType) => (
        <div className="text-sm">
          <p>{Math.round(image.size / 1024)} كب</p>
          <p className="text-gray-500">{image.width}×{image.height}</p>
        </div>
      )
    },
    {
      key: 'status',
      title: 'الحالة',
      render: (_: any, image: ImageType) => (
        <div className="space-y-1">
          <StatusBadge
            status={image.processingStatus === 'ready' ? 'جاهزة' : 'معالجة'}
          />
          {image.isPrimary && (
            <StatusBadge status="رئيسية" size="sm" />
          )}
        </div>
      )
    },
    {
      key: 'uploadedAt',
      title: 'تاريخ الرفع',
      render: (_: any, image: ImageType) => (
        <div className="text-sm">
          <p>{new Date(image.uploadedAt).toLocaleDateString('ar-SA')}</p>
          <p className="text-gray-500">{new Date(image.uploadedAt).toLocaleTimeString('ar-SA')}</p>
        </div>
      )
    },
    {
      key: 'actions',
      title: 'الإجراءات',
      render: (_: any, image: ImageType) => (
        <div className="flex gap-1">
          <ActionButton
            variant="secondary"
            size="sm"
            onClick={() => handleEditImage(image)}
            label="تحرير">
          </ActionButton>
          <ActionButton
            variant="secondary"
            size="sm"
            onClick={() => setSelectedImages([image.id])}
            label="حذف">
          </ActionButton>
        </div>
      )
    }
  ];

  // فئات الصور للتصفية - Image categories for filtering
  const categories = [
    { value: 'all' as const, label: 'جميع الصور', count: statistics?.totalImages || 0 },
    { value: 'exterior' as const, label: 'خارجية', count: statistics?.byCategory.exterior || 0 },
    { value: 'interior' as const, label: 'داخلية', count: statistics?.byCategory.interior || 0 },
    { value: 'amenity' as const, label: 'المرافق', count: statistics?.byCategory.amenity || 0 },
    { value: 'floor_plan' as const, label: 'مخطط الطابق', count: statistics?.byCategory.floor_plan || 0 },
    { value: 'documents' as const, label: 'المستندات', count: statistics?.byCategory.documents || 0 },
    { value: 'gallery' as const, label: 'معرض عام', count: statistics?.byCategory.gallery || 0 }
  ];

  if (propertyLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  if (propertyError || !property) {
    return (
      <div className="text-center py-8">
        <p className="text-red-600 mb-4">حدث خطأ في تحميل بيانات الكيان</p>
        <ActionButton onClick={() => navigate('/admin/properties')} variant="secondary" label="العودة للكيانات">
        </ActionButton>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* رأس الصفحة - Page Header */}
      <div className="bg-white rounded-lg shadow-sm border p-6">
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <div className="flex items-center gap-2 text-sm text-gray-500 mb-2">
              <button
                onClick={() => navigate('/admin/properties')}
                className="hover:text-blue-600 transition-colors"
              >
                الكيانات
              </button>
              <span>/</span>
              <button
                onClick={() => navigate(`/admin/properties/${propertyId}`)}
                className="hover:text-blue-600 transition-colors"
              >
                {property.title}
              </button>
              <span>/</span>
              <span>معرض الصور</span>
            </div>
            <h1 className="text-2xl font-bold text-gray-900">معرض صور الكيان</h1>
            <p className="text-gray-600 mt-1">{property.title}</p>
          </div>

          <div className="flex items-center gap-3">
            <ActionButton
              variant="secondary"
              onClick={() => navigate(`/admin/properties/${propertyId}`)}
              label="العودة للكيان">
            </ActionButton>
          </div>
        </div>
      </div>

      {/* التبويبات - Tabs */}
      <div className="bg-white rounded-lg shadow-sm border">
        <div className="border-b border-gray-200">
          <nav className="flex space-x-8 rtl:space-x-reverse px-6">
            {[
              { id: 'gallery', label: 'معرض الصور', count: images.length },
              { id: 'upload', label: 'رفع جديدة', icon: '📤' },
              { id: 'statistics', label: 'الإحصائيات', icon: '📊' }
            ].map((tab) => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id as any)}
                className={`
                  py-4 px-1 border-b-2 font-medium text-sm transition-colors
                  ${activeTab === tab.id
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }
                `}
              >
                <div className="flex items-center gap-2">
                  {tab.icon && <span>{tab.icon}</span>}
                  <span>{tab.label}</span>
                  {tab.count !== undefined && (
                    <span className="bg-gray-100 text-gray-600 px-2 py-0.5 rounded-full text-xs">
                      {tab.count}
                    </span>
                  )}
                </div>
              </button>
            ))}
          </nav>
        </div>

        <div className="p-6">
          {/* تبويب معرض الصور - Gallery Tab */}
          {activeTab === 'gallery' && (
            <div className="space-y-6">
              {/* أدوات التحكم - Control Tools */}
              <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
                <div className="flex items-center gap-4">
                  {/* تصفية حسب الفئة - Filter by category */}
                  <div className="flex items-center gap-2">
                    <label className="text-sm font-medium text-gray-700">الفئة:</label>
                    <select
                      value={filterCategory}
                      onChange={(e) => setFilterCategory(e.target.value as ImageCategory | 'all')}
                      className="px-3 py-2 border border-gray-300 rounded text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      {categories.map(cat => (
                        <option key={cat.value} value={cat.value}>
                          {cat.label} ({cat.count})
                        </option>
                      ))}
                    </select>
                  </div>

                  {/* تبديل وضع العرض - Toggle view mode */}
                  <div className="flex items-center gap-2">
                    <label className="text-sm font-medium text-gray-700">العرض:</label>
                    <div className="flex border border-gray-300 rounded overflow-hidden">
                      <button
                        onClick={() => setViewMode('grid')}
                        className={`px-3 py-1 text-sm ${
                          viewMode === 'grid'
                            ? 'bg-blue-500 text-white'
                            : 'bg-white text-gray-700 hover:bg-gray-50'
                        }`}
                      >
                        شبكة
                      </button>
                      <button
                        onClick={() => setViewMode('table')}
                        className={`px-3 py-1 text-sm ${
                          viewMode === 'table'
                            ? 'bg-blue-500 text-white'
                            : 'bg-white text-gray-700 hover:bg-gray-50'
                        }`}
                      >
                        جدول
                      </button>
                    </div>
                  </div>
                </div>

                <div className="flex items-center gap-2">
                  {selectedImages.length > 0 && (
                    <>
                      <span className="text-sm text-gray-600">
                        {selectedImages.length} محددة
                      </span>
                      <ActionButton
                        variant="danger"
                        size="sm"
                        onClick={handleDeleteSelected}
                        label="حذف المحددة"
                      >
                        حذف المحددة
                      </ActionButton>
                    </>
                  )}
                  <ActionButton
                    variant="primary"
                    onClick={() => setActiveTab('upload')}
                    label="رفع صور جديدة">
                  </ActionButton>
                </div>
              </div>

              {/* معرض الصور أو الجدول - Gallery or Table */}
              {viewMode === 'grid' ? (
                <ImageGallery
                  propertyId={propertyId}
                  category={filterCategory === 'all' ? undefined : filterCategory}
                  editable={true}
                  reorderable={true}
                  multiSelect={true}
                  selectedImages={selectedImages}
                  onSelectionChange={handleSelectionChange}
                  onEditImage={handleEditImage}
                  showDetails={true}
                  showToolbar={false}
                />
              ) : (
                <div>
                  {images.length === 0 ? (
                    <div className="text-center py-8 text-gray-500">
                      لا توجد صور للعرض
                    </div>
                  ) : (
                    <DataTable
                      data={images}
                      columns={imageTableColumns}
                      loading={imagesLoading}
                      rowSelection={{
                        selectedRowKeys: selectedImages,
                        onChange: (selectedRowKeys: string[]) => setSelectedImages(selectedRowKeys)
                      }}
                    />
                  )}
                </div>
              )}
            </div>
          )}

          {/* تبويب رفع الصور - Upload Tab */}
          {activeTab === 'upload' && (
            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold mb-2">رفع صور جديدة</h3>
                <p className="text-gray-600">
                  اختر الصور لرفعها وربطها بهذا الكيان. يمكنك تحديد فئة كل صورة والتحكم في ترتيبها.
                </p>
              </div>

              <ImageUploader
                propertyId={propertyId}
                defaultCategory="gallery"
                multiple={true}
                maxFiles={50}
                maxSize={10}
                showPreview={true}
                showProgress={true}
                onUploadComplete={handleUploadComplete}
              />
            </div>
          )}

          {/* تبويب الإحصائيات - Statistics Tab */}
          {activeTab === 'statistics' && (
            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold mb-2">إحصائيات الصور</h3>
                <p className="text-gray-600">
                  نظرة عامة على صور هذا الكيان والمساحة المستخدمة.
                </p>
              </div>

              {statisticsLoading ? (
                <div className="flex justify-center py-8">
                  <LoadingSpinner size="lg" />
                </div>
              ) : statistics ? (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                  {/* إجمالي الصور - Total Images */}
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-6">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
                          <svg className="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clipRule="evenodd" />
                          </svg>
                        </div>
                      </div>
                      <div className="mr-4">
                        <p className="text-sm font-medium text-blue-600">إجمالي الصور</p>
                        <p className="text-2xl font-bold text-blue-900">{statistics.totalImages}</p>
                      </div>
                    </div>
                  </div>

                  {/* الحجم الكلي - Total Size */}
                  <div className="bg-green-50 border border-green-200 rounded-lg p-6">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-green-500 rounded-lg flex items-center justify-center">
                          <svg className="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z" />
                          </svg>
                        </div>
                      </div>
                      <div className="mr-4">
                        <p className="text-sm font-medium text-green-600">الحجم الكلي</p>
                        <p className="text-2xl font-bold text-green-900">
                          {(statistics.totalSize / 1024 / 1024).toFixed(1)} MB
                        </p>
                      </div>
                    </div>
                  </div>

                  {/* الصور الرئيسية - Primary Images */}
                  <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-yellow-500 rounded-lg flex items-center justify-center">
                          <svg className="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
                          </svg>
                        </div>
                      </div>
                      <div className="mr-4">
                        <p className="text-sm font-medium text-yellow-600">الصور الرئيسية</p>
                        <p className="text-2xl font-bold text-yellow-900">{statistics.primaryImages}</p>
                      </div>
                    </div>
                  </div>

                  {/* متوسط الحجم - Average Size */}
                  <div className="bg-purple-50 border border-purple-200 rounded-lg p-6">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-purple-500 rounded-lg flex items-center justify-center">
                          <svg className="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M3 3a1 1 0 000 2v8a2 2 0 002 2h2.586l-1.293 1.293a1 1 0 101.414 1.414L10 15.414l2.293 2.293a1 1 0 001.414-1.414L12.414 15H15a2 2 0 002-2V5a1 1 0 100-2H3zm11.707 4.707a1 1 0 00-1.414-1.414L10 9.586 8.707 8.293a1 1 0 00-1.414 0l-2 2a1 1 0 101.414 1.414L8 10.414l1.293 1.293a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                          </svg>
                        </div>
                      </div>
                      <div className="mr-4">
                        <p className="text-sm font-medium text-purple-600">متوسط الحجم</p>
                        <p className="text-2xl font-bold text-purple-900">
                          {(statistics.averageSize / 1024 / 1024).toFixed(1)} MB
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-8">
                  <p className="text-gray-500">لا توجد إحصائيات متاحة</p>
                </div>
              )}
            </div>
          )}
        </div>
      </div>

      {/* محرر الصور المتقدم - Advanced Image Editor */}
      {editingImage && (
        <AdvancedImageEditor
          image={editingImage}
          isOpen={!!editingImage}
          onClose={handleCloseEditor}
          onSave={handleSaveImageEdits}
        />
      )}

      {/* مربع حوار التأكيد - Confirmation Dialog */}
      {confirmDialog && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-md mx-4">
            <h3 className="text-lg font-semibold mb-2">{confirmDialog.title}</h3>
            <p className="text-gray-600 mb-4">{confirmDialog.message}</p>
            <div className="flex gap-2 justify-end">
              <ActionButton
                variant="secondary"
                onClick={hideConfirmDialog}
                label="إلغاء"
              >
                إلغاء
              </ActionButton>
              <ActionButton
                variant="danger"
                onClick={confirmDialog.onConfirm}
                label="تأكيد"
              >
                تأكيد
              </ActionButton>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default PropertyImageGallery;