/**
 * مكون معرض الصور - مكون شامل لعرض وإدارة معرض الصور
 * Image Gallery Component - Comprehensive component for displaying and managing image gallery
 */

import React, { useState, useCallback, useMemo, useEffect } from 'react';
import { DndContext, closestCenter, KeyboardSensor, PointerSensor, useSensor, useSensors } from '@dnd-kit/core';
import { SortableContext, arrayMove, sortableKeyboardCoordinates, rectSortingStrategy } from '@dnd-kit/sortable';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { useImages } from '../../hooks/useImages';
import type { Image, ImageCategory } from '../../types/image.types';
import ActionButton from '../ui/ActionButton';
import LoadingSpinner from '../ui/LoadingSpinner';
import StatusBadge from '../ui/StatusBadge';
import { useUXHelpers } from '../../hooks/useUXHelpers';

interface ImageGalleryProps {
  /** معرف الكيان - Property ID */
  propertyId?: string;
  /** معرف الوحدة - Unit ID */
  unitId?: string;
  /** فئة الصور المطلوب عرضها - Category of images to display */
  category?: ImageCategory;
  /** هل يمكن التعديل - Is editable */
  editable?: boolean;
  /** هل يمكن إعادة الترتيب - Is reorderable */
  reorderable?: boolean;
  /** عدد الأعمدة - Number of columns */
  columns?: number;
  /** حجم الصور المصغرة - Thumbnail size */
  thumbnailSize?: 'small' | 'medium' | 'large';
  /** دالة عند اختيار صورة - Callback when image is selected */
  onImageSelect?: (image: Image) => void;
  /** دالة عند فتح محرر الصور - Callback when opening image editor */
  onEditImage?: (image: Image) => void;
  /** الصور المحددة - Selected images */
  selectedImages?: string[];
  /** دالة تغيير الصور المحددة - Callback for changing selected images */
  onSelectionChange?: (selectedIds: string[]) => void;
  /** وضع الاختيار المتعدد - Multiple selection mode */
  multiSelect?: boolean;
  /** عرض المعلومات التفصيلية - Show detailed info */
  showDetails?: boolean;
  /** عرض شريط الأدوات - Show toolbar */
  showToolbar?: boolean;
}

/**
 * مكون صورة قابلة للسحب
 * Draggable Image Item Component
 */
const SortableImageItem: React.FC<{
  image: Image;
  isSelected: boolean;
  thumbnailSize: 'small' | 'medium' | 'large';
  showDetails: boolean;
  onSelect: () => void;
  onEdit: () => void;
  onSetPrimary: () => void;
  onDelete: () => void;
  isDragging?: boolean;
}> = ({
  image,
  isSelected,
  thumbnailSize,
  showDetails,
  onSelect,
  onEdit,
  onSetPrimary,
  onDelete,
  isDragging
}) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging: isSortableDragging,
  } = useSortable({ id: image.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isSortableDragging ? 0.5 : 1,
  };

  const sizeClasses = {
    small: 'w-24 h-24',
    medium: 'w-32 h-32',
    large: 'w-48 h-48',
    xlarge: 'w-64 h-64',
  };

  const thumbnailUrl = image.thumbnails[thumbnailSize] || image.url;

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...attributes}
      className={`
        relative group cursor-pointer rounded-lg overflow-hidden border-2 transition-all duration-200
        ${isSelected ? 'border-blue-500 ring-2 ring-blue-200' : 'border-gray-200 hover:border-gray-300'}
        ${isSortableDragging ? 'shadow-2xl z-50' : 'hover:shadow-md'}
      `}
      onClick={onSelect}
    >
      {/* صورة أساسية - Main Image */}
      {/* <div className={`${sizeClasses[thumbnailSize]} relative`}> */}
      <div className={`relative`}>
        <img
          src={thumbnailUrl}
          alt={image.alt || image.filename}
          className="w-full h-full object-cover"
          loading="lazy"
        />
        
        {/* طبقة التحديد - Selection Overlay */}
        {isSelected && (
          <div className="absolute inset-0 bg-blue-500 bg-opacity-20 flex items-center justify-center">
            <div className="w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center">
              <svg className="w-4 h-4 text-white" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
              </svg>
            </div>
          </div>
        )}

        {/* مؤشر الصورة الرئيسية - Primary Image Indicator */}
        {image.isPrimary && (
          <div className="absolute top-1 right-1">
            <StatusBadge status="رئيسية" size="sm" />
          </div>
        )}

        {/* مؤشر حالة المعالجة - Processing Status Indicator */}
        {image.processingStatus !== 'ready' && (
          <div className="absolute top-1 left-1">
            <StatusBadge 
              status={image.processingStatus === 'processing' ? 'معالجة' : 'خطأ'} 
              size="sm" 
            />
          </div>
        )}

        {/* مقبض السحب - Drag Handle */}
        <div
          {...listeners}
          className="absolute top-1 left-1 opacity-0 group-hover:opacity-100 transition-opacity cursor-grab active:cursor-grabbing"
        >
          <div className="w-6 h-6 bg-black bg-opacity-50 rounded flex items-center justify-center">
            <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 8h16M4 16h16" />
            </svg>
          </div>
        </div>

        {/* شريط الأدوات - Toolbar */}
        <div className="absolute bottom-0 left-0 right-0 bg-black bg-opacity-50 p-1 transform translate-y-full group-hover:translate-y-0 transition-transform">
          <div className="flex justify-center space-x-1 rtl:space-x-reverse">
            <ActionButton
              variant="secondary"
              size="sm"
              onClick={(e) => { e.stopPropagation(); onEdit(); }}
              className="text-white hover:bg-white hover:bg-opacity-20"
              label="تحرير"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
            </ActionButton>
            
            {!image.isPrimary && (
              <ActionButton
                variant="secondary"
                size="sm"
                onClick={(e) => { e.stopPropagation(); onSetPrimary(); }}
                className="text-white hover:bg-white hover:bg-opacity-20"
                label="تعيين رئيسية"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
                </svg>
              </ActionButton>
            )}
            
            <ActionButton
              variant="danger"
              size="sm"
              onClick={(e) => { e.stopPropagation(); onDelete(); }}
              className="text-white hover:bg-red-500 hover:bg-opacity-50"
              label="حذف"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </ActionButton>
          </div>
        </div>
      </div>

      {/* معلومات تفصيلية - Detailed Info */}
      {showDetails && (
        <div className="p-2 bg-white">
          <p className="text-xs text-gray-600 truncate" title={image.filename}>
            {image.filename}
          </p>
          <p className="text-xs text-gray-500">
            {Math.round(image.size / 1024)} كب • {image.width}×{image.height}
          </p>
          {image.tags.length > 0 && (
            <div className="flex flex-wrap gap-1 mt-1">
              {image.tags.slice(0, 2).map(tag => (
                <span
                  key={tag}
                  className="px-1 py-0.5 bg-gray-100 text-gray-600 text-xs rounded"
                >
                  {tag}
                </span>
              ))}
              {image.tags.length > 2 && (
                <span className="text-xs text-gray-400">+{image.tags.length - 2}</span>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

/**
 * مكون معرض الصور الرئيسي
 * Main Image Gallery Component
 */
export const ImageGallery: React.FC<ImageGalleryProps> = ({
  propertyId,
  unitId,
  category,
  editable = true,
  reorderable = true,
  columns = 4,
  thumbnailSize = 'medium',
  onImageSelect,
  onEditImage,
  selectedImages = [],
  onSelectionChange,
  multiSelect = false,
  showDetails = false,
  showToolbar = true
}) => {
  const { loading, executeWithFeedback, showConfirmDialog, confirmDialog, hideConfirmDialog } = useUXHelpers();
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [filterCategory, setFilterCategory] = useState<ImageCategory | 'all'>('all');

  // استعلام الصور - Images query
  const {
    images,
    isLoading,
    error,
    reorderImagesAsync,
    setPrimaryImageAsync,
    deleteImageAsync,
    refetch
  } = useImages({
    propertyId,
    unitId,
    category: filterCategory === 'all' ? undefined : filterCategory,
    sortBy: 'order',
    sortOrder: 'asc'
  });

  // أجهزة الاستشعار للسحب والإفلات - Drag and drop sensors
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  // تصفية الصور المعروضة - Filter displayed images
  const filteredImages = useMemo(() => {
    let filtered = images;
    
    if (category) {
      filtered = filtered.filter(img => img.category === category);
    }
    
    return filtered;
  }, [images, category]);
  // Optimistic ordering state
  const [orderedImages, setOrderedImages] = useState<Image[]>([]);
  useEffect(() => {
    setOrderedImages(filteredImages);
  }, [filteredImages]);

  // معالجة إعادة ترتيب الصور - Handle image reordering
  const handleDragEnd = useCallback(async (event: any) => {
    const { active, over } = event;
    
    if (active.id !== over?.id) {
      // Local optimistic reorder
      const oldIndex = orderedImages.findIndex(img => img.id === active.id);
      const newIndex = orderedImages.findIndex(img => img.id === over.id);
      const newOrder = arrayMove(orderedImages, oldIndex, newIndex);
      setOrderedImages(newOrder);
      const imageIds = newOrder.map(img => img.id);
      try {
        await reorderImagesAsync({ imageIds, propertyId, unitId });
      } catch (error) {
        // Revert on failure
        setOrderedImages(filteredImages);
        console.error('فشل في إعادة ترتيب الصور:', error);
      }
    }
  }, [orderedImages, filteredImages, reorderImagesAsync, propertyId, unitId]);

  // معالجة اختيار الصورة - Handle image selection
  const handleImageSelect = useCallback((image: Image) => {
    if (multiSelect) {
      const newSelection = selectedImages.includes(image.id)
        ? selectedImages.filter(id => id !== image.id)
        : [...selectedImages, image.id];
      onSelectionChange?.(newSelection);
    } else {
      onSelectionChange?.([image.id]);
    }
    onImageSelect?.(image);
  }, [multiSelect, selectedImages, onSelectionChange, onImageSelect]);

  // معالجة تعيين صورة رئيسية - Handle set primary image
  const handleSetPrimary = useCallback(async (image: Image) => {
    try {
      await executeWithFeedback(
        () => setPrimaryImageAsync({ imageId: image.id, propertyId, unitId }),
        {
          loadingKey: 'set-primary-image',
          successMessage: 'تم تعيين الصورة الرئيسية بنجاح',
          errorMessage: 'فشل في تعيين الصورة الرئيسية'
        }
      );
    } catch (error) {
      console.error('فشل في تعيين الصورة الرئيسية:', error);
    }
  }, [executeWithFeedback, setPrimaryImageAsync, propertyId, unitId]);

  // معالجة حذف الصورة - Handle delete image
  const handleDeleteImage = useCallback((image: Image) => {
    showConfirmDialog({
      title: 'حذف الصورة',
      message: `هل أنت متأكد من حذف الصورة "${image.filename}"؟`,
      type: 'danger',
      onConfirm: async () => {
        try {
          await executeWithFeedback(
            () => deleteImageAsync({ imageId: image.id }),
            {
              loadingKey: 'delete-image',
              successMessage: 'تم حذف الصورة بنجاح',
              errorMessage: 'فشل في حذف الصورة'
            }
          );
        } catch (error) {
          console.error('فشل في حذف الصورة:', error);
        } finally {
          hideConfirmDialog();
        }
      }
    });
  }, [showConfirmDialog, hideConfirmDialog, executeWithFeedback, deleteImageAsync]);

  // فئات الصور للتصفية - Image categories for filtering
  const categories: { value: ImageCategory | 'all'; label: string }[] = [
    { value: 'all', label: 'جميع الصور' },
    { value: 'exterior', label: 'خارجية' },
    { value: 'interior', label: 'داخلية' },
    { value: 'amenity', label: 'المرافق' },
    { value: 'floor_plan', label: 'مخطط الطابق' },
    { value: 'documents', label: 'المستندات' }
  ];

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-8">
        <p className="text-red-600 mb-4">حدث خطأ في تحميل الصور</p>
        <ActionButton onClick={() => refetch()} variant="secondary" label="إعادة المحاولة">
          إعادة المحاولة
        </ActionButton>
      </div>
    );
  }

  return (
    <div className="w-full">
      {/* شريط الأدوات - Toolbar */}
      {showToolbar && (
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6 p-4 bg-gray-50 rounded-lg">
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
              <label className="text-sm font-medium text-gray-700">عرض:</label>
              <select
                value={viewMode}
                onChange={(e) => setViewMode(e.target.value as 'grid' | 'list')}
                className="px-3 py-1 border border-gray-300 rounded text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="grid">شبكة</option>
                <option value="list">قائمة</option>
              </select>
            </div>

            <div className="flex items-center gap-2">
              <label className="text-sm font-medium text-gray-700">الفئة:</label>
              <select
                value={filterCategory}
                onChange={(e) => setFilterCategory(e.target.value as ImageCategory | 'all')}
                className="px-3 py-1 border border-gray-300 rounded text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                {categories.map(cat => (
                  <option key={cat.value} value={cat.value}>
                    {cat.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <span className="text-sm text-gray-600">
              {filteredImages.length} صورة
            </span>
            {selectedImages.length > 0 && (
              <span className="text-sm text-blue-600">
                ({selectedImages.length} محددة)
              </span>
            )}
          </div>
        </div>
      )}

      {/* معرض الصور - Image Gallery */}
      {filteredImages.length === 0 ? (
        <div className="text-center py-12">
          <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          <p className="mt-2 text-sm text-gray-600">لا توجد صور للعرض</p>
        </div>
      ) : (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <SortableContext items={orderedImages.map(img => img.id)} strategy={rectSortingStrategy}>
            <div className={`
              ${viewMode === 'grid' 
                ? `grid grid-cols-2 md:grid-cols-${Math.min(columns, 6)} gap-4`
                : 'flex flex-col gap-2'
              }
            `}>
              {orderedImages.map((image) => (
                <SortableImageItem
                  key={image.id}
                  image={image}
                  isSelected={selectedImages.includes(image.id)}
                  thumbnailSize={thumbnailSize}
                  showDetails={showDetails || viewMode === 'list'}
                  onSelect={() => handleImageSelect(image)}
                  onEdit={() => onEditImage?.(image)}
                  onSetPrimary={() => handleSetPrimary(image)}
                  onDelete={() => handleDeleteImage(image)}
                />
              ))}
            </div>
          </SortableContext>
        </DndContext>
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