/**
 * مكون رفع الصور - مكون شامل لرفع الصور مع المعاينة والتحقق
 * Image Uploader Component - Comprehensive component for uploading images with preview and validation
 */

import React, { useCallback, useState, useRef } from 'react';
import { useDropzone } from 'react-dropzone';
import { useImages } from '../../hooks/useImages';
import type { UploadImageRequest, ImageCategory, UploadProgress } from '../../types/image.types';
import ActionButton from '../ui/ActionButton';
import LoadingSpinner from '../ui/LoadingSpinner';
import { useUXHelpers } from '../../hooks/useUXHelpers';
import { useNotifications } from '../../stores/appStore';

interface ImageUploaderProps {
  /** معرف الكيان - Property ID */
  propertyId?: string;
  /** معرف الوحدة - Unit ID */
  unitId?: string;
  /** فئة الصور الافتراضية - Default image category */
  defaultCategory?: ImageCategory;
  /** السماح بملفات متعددة - Allow multiple files */
  multiple?: boolean;
  /** الحد الأقصى لعدد الملفات - Maximum number of files */
  maxFiles?: number;
  /** الحد الأقصى لحجم الملف (بالميجابايت) - Maximum file size in MB */
  maxSize?: number;
  /** الأنواع المسموحة - Allowed file types */
  acceptedTypes?: string[];
  /** عرض المعاينة - Show preview */
  showPreview?: boolean;
  /** دالة عند اكتمال الرفع - Callback on upload completion */
  onUploadComplete?: (images: any[]) => void;
  /** دالة عند حدوث خطأ - Callback on error */
  onError?: (error: string) => void;
  /** النمط المضغوط - Compact mode */
  compact?: boolean;
  /** عرض تقدم الرفع - Show upload progress */
  showProgress?: boolean;
}

/**
 * مكون عنصر الصورة في المعاينة
 * Preview Image Item Component
 */
const PreviewImageItem: React.FC<{
  file: File;
  progress?: UploadProgress;
  onRemove: () => void;
  onCategoryChange: (category: ImageCategory) => void;
  category: ImageCategory;
}> = ({ file, progress, onRemove, onCategoryChange, category }) => {
  const [preview, setPreview] = useState<string>('');

  // إنشاء معاينة الصورة - Create image preview
  React.useEffect(() => {
    const objectUrl = URL.createObjectURL(file);
    setPreview(objectUrl);

    return () => URL.revokeObjectURL(objectUrl);
  }, [file]);

  const categories: { value: ImageCategory; label: string }[] = [
    { value: 'exterior', label: 'خارجية' },
    { value: 'interior', label: 'داخلية' },
    { value: 'amenity', label: 'المرافق' },
    { value: 'floor_plan', label: 'مخطط الطابق' },
    { value: 'documents', label: 'المستندات' },
    { value: 'gallery', label: 'معرض عام' }
  ];

  return (
    <div className="relative bg-white border border-gray-200 rounded-lg p-3">
      {/* معاينة الصورة - Image preview */}
      <div className="relative w-full h-24 mb-2 rounded overflow-hidden bg-gray-100">
        <img
          src={preview}
          alt={file.name}
          className="w-full h-full object-cover"
        />
        
        {/* زر الحذف - Remove button */}
        <button
          onClick={onRemove}
          className="absolute top-1 left-1 w-6 h-6 bg-red-500 text-white rounded-full flex items-center justify-center text-xs hover:bg-red-600 transition-colors"
        >
          ✕
        </button>

        {/* مؤشر التقدم - Progress indicator */}
        {progress && (
          <div className="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center">
            {progress.status === 'uploading' && (
              <div className="text-center">
                <LoadingSpinner size="sm" className="text-white" />
                <div className="text-white text-xs mt-1">{progress.progress}%</div>
              </div>
            )}
            {progress.status === 'completed' && (
              <div className="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center">
                <svg className="w-4 h-4 text-white" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                </svg>
              </div>
            )}
            {progress.status === 'failed' && (
              <div className="w-8 h-8 bg-red-500 rounded-full flex items-center justify-center">
                <svg className="w-4 h-4 text-white" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
                </svg>
              </div>
            )}
          </div>
        )}
      </div>

      {/* معلومات الملف - File info */}
      <div className="space-y-2">
        <div>
          <p className="text-xs font-medium text-gray-700 truncate" title={file.name}>
            {file.name}
          </p>
          <p className="text-xs text-gray-500">
            {(file.size / 1024 / 1024).toFixed(2)} MB
          </p>
        </div>

        {/* اختيار الفئة - Category selection */}
        <select
          value={category}
          onChange={(e) => onCategoryChange(e.target.value as ImageCategory)}
          className="w-full px-2 py-1 text-xs border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          {categories.map(cat => (
            <option key={cat.value} value={cat.value}>
              {cat.label}
            </option>
          ))}
        </select>

        {/* رسالة الخطأ - Error message */}
        {progress?.status === 'failed' && progress.error && (
          <p className="text-xs text-red-600 mt-1">{progress.error}</p>
        )}
      </div>
    </div>
  );
};

/**
 * مكون رفع الصور الرئيسي
 * Main Image Uploader Component
 */
export const ImageUploader: React.FC<ImageUploaderProps> = ({
  propertyId,
  unitId,
  defaultCategory = 'gallery',
  multiple = true,
  maxFiles = 20,
  maxSize = 10, // 10MB
  acceptedTypes = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'],
  showPreview = true,
  onUploadComplete,
  onError,
  compact = false,
  showProgress = true
}) => {
  const { loading, executeWithFeedback } = useUXHelpers();
  const { showSuccess, showError } = useNotifications();
  const fileInputRef = useRef<HTMLInputElement>(null);
  
  // الحالات المحلية - Local states
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [fileCategories, setFileCategories] = useState<Record<string, ImageCategory>>({});
  const [isUploading, setIsUploading] = useState(false);

  // هوك الصور - Images hook
  const {
    uploadImageWithProgress,
    uploadProgress,
    clearUploadProgress,
    clearAllUploadProgress
  } = useImages();

  // معالجة إسقاط الملفات - Handle file drop
  const onDrop = useCallback((acceptedFiles: File[], rejectedFiles: any[]) => {
    // معالجة الملفات المرفوضة - Handle rejected files
    if (rejectedFiles.length > 0) {
      const errors = rejectedFiles.map(({ file, errors }) => 
        `${file.name}: ${errors.map((e: any) => e.message).join(', ')}`
      );
      onError?.(errors.join('\n'));
      showError('بعض الملفات لم يتم قبولها');
      return;
    }

    // التحقق من عدد الملفات - Check file count
    const totalFiles = selectedFiles.length + acceptedFiles.length;
    if (totalFiles > maxFiles) {
      onError?.(`لا يمكن رفع أكثر من ${maxFiles} ملف`);
      showError(`لا يمكن رفع أكثر من ${maxFiles} ملف`);
      return;
    }

    // إضافة الملفات الجديدة - Add new files
    setSelectedFiles(prev => [...prev, ...acceptedFiles]);
    
    // تعيين الفئة الافتراضية - Set default category
    const newCategories: Record<string, ImageCategory> = {};
    acceptedFiles.forEach(file => {
      newCategories[file.name] = defaultCategory;
    });
    setFileCategories(prev => ({ ...prev, ...newCategories }));
  }, [selectedFiles, maxFiles, defaultCategory, onError, showError]);

  // إعدادات Dropzone - Dropzone configuration
  const {
    getRootProps,
    getInputProps,
    isDragActive,
    isDragReject
  } = useDropzone({
    onDrop,
    accept: acceptedTypes.reduce((acc, type) => ({ ...acc, [type]: [] }), {}),
    multiple,
    maxSize: maxSize * 1024 * 1024, // تحويل إلى بايت - Convert to bytes
    disabled: isUploading
  });

  // إزالة ملف - Remove file
  const removeFile = useCallback((index: number) => {
    const file = selectedFiles[index];
    setSelectedFiles(prev => prev.filter((_, i) => i !== index));
    setFileCategories(prev => {
      const { [file.name]: removed, ...rest } = prev;
      return rest;
    });
    
    // مسح تقدم الرفع إن وجد - Clear upload progress if exists
    const progressEntries = Object.entries(uploadProgress);
    const progressEntry = progressEntries.find(([_, progress]) => 
      progress.filename === file.name
    );
    if (progressEntry) {
      clearUploadProgress(progressEntry[0]);
    }
  }, [selectedFiles, uploadProgress, clearUploadProgress]);

  // تغيير فئة الملف - Change file category
  const changeFileCategory = useCallback((fileName: string, category: ImageCategory) => {
    setFileCategories(prev => ({ ...prev, [fileName]: category }));
  }, []);

  // رفع جميع الملفات - Upload all files
  const uploadAllFiles = useCallback(async () => {
    if (selectedFiles.length === 0) return;

    setIsUploading(true);
    const uploadedImages: any[] = [];
    const errors: string[] = [];

    try {
      for (const file of selectedFiles) {
        try {
          const uploadRequest: UploadImageRequest = {
            file,
            propertyId,
            unitId,
            category: fileCategories[file.name] || defaultCategory,
            alt: file.name.split('.')[0] // استخدام اسم الملف كنص بديل
          };

          const result = await uploadImageWithProgress(uploadRequest);
          if (result.success && result.image) {
            uploadedImages.push(result.image);
          }
        } catch (error) {
          const errorMessage = error instanceof Error ? error.message : 'حدث خطأ غير متوقع';
          errors.push(`${file.name}: ${errorMessage}`);
        }
      }

      if (uploadedImages.length > 0) {
        showSuccess(`تم رفع ${uploadedImages.length} صورة بنجاح`);
        onUploadComplete?.(uploadedImages);
        
        // مسح الملفات المحددة - Clear selected files
        setSelectedFiles([]);
        setFileCategories({});
        clearAllUploadProgress();
      }

      if (errors.length > 0) {
        onError?.(errors.join('\n'));
        showError(`فشل في رفع ${errors.length} صورة`);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'حدث خطأ غير متوقع';
      onError?.(errorMessage);
      showError('فشل في رفع الصور');
    } finally {
      setIsUploading(false);
    }
  }, [
    selectedFiles, 
    fileCategories, 
    propertyId, 
    unitId, 
    defaultCategory, 
    uploadImageWithProgress,
    onUploadComplete,
    onError,
    showSuccess,
    showError,
    clearAllUploadProgress
  ]);

  // مسح جميع الملفات - Clear all files
  const clearAllFiles = useCallback(() => {
    setSelectedFiles([]);
    setFileCategories({});
    clearAllUploadProgress();
  }, [clearAllUploadProgress]);

  // فتح نافذة اختيار الملفات - Open file picker
  const openFilePicker = useCallback(() => {
    fileInputRef.current?.click();
  }, []);

  if (compact) {
    return (
      <div className="w-full">
        <div
          {...getRootProps()}
          className={`
            border-2 border-dashed rounded-lg p-4 text-center cursor-pointer transition-colors
            ${isDragActive 
              ? 'border-blue-500 bg-blue-50' 
              : isDragReject 
              ? 'border-red-500 bg-red-50'
              : 'border-gray-300 hover:border-gray-400'
            }
            ${isUploading ? 'pointer-events-none opacity-50' : ''}
          `}
        >
          <input {...getInputProps()} ref={fileInputRef} />
          <div className="flex items-center justify-center gap-2">
            <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
            </svg>
            <span className="text-sm text-gray-600">
              {isDragActive ? 'اسقط الصور هنا' : 'اختر أو اسقط الصور'}
            </span>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full space-y-4">
      {/* منطقة الإسقاط - Drop zone */}
      <div
        {...getRootProps()}
        className={`
          border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition-all duration-200
          ${isDragActive 
            ? 'border-blue-500 bg-blue-50 scale-105' 
            : isDragReject 
            ? 'border-red-500 bg-red-50'
            : 'border-gray-300 hover:border-gray-400 hover:bg-gray-50'
          }
          ${isUploading ? 'pointer-events-none opacity-50' : ''}
        `}
      >
        <input {...getInputProps()} ref={fileInputRef} />
        
        <div className="space-y-4">
          <div className="flex justify-center">
            <svg className="w-12 h-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
            </svg>
          </div>
          
          <div>
            <p className="text-lg font-medium text-gray-700">
              {isDragActive 
                ? 'اسقط الصور هنا' 
                : 'اختر الصور أو اسقطها هنا'
              }
            </p>
            <p className="text-sm text-gray-500 mt-1">
              PNG، JPG، WebP، GIF حتى {maxSize} ميجابايت
            </p>
            {multiple && (
              <p className="text-xs text-gray-400 mt-1">
                يمكن رفع حتى {maxFiles} صورة
              </p>
            )}
          </div>
          
          <ActionButton
            variant="secondary"
            onClick={openFilePicker}
            disabled={isUploading}
            label="اختيار الملفات"
          >
            اختيار الملفات
          </ActionButton>
        </div>
      </div>

      {/* معاينة الملفات المحددة - Selected files preview */}
      {showPreview && selectedFiles.length > 0 && (
        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <h4 className="text-sm font-medium text-gray-700">
              الصور المحددة ({selectedFiles.length})
            </h4>
            <div className="flex gap-2">
              <ActionButton
                variant="secondary"
                size="sm"
                onClick={clearAllFiles}
                disabled={isUploading}
                label="مسح الكل"
              >
                مسح الكل
              </ActionButton>
              <ActionButton
                variant="primary"
                size="sm"
                onClick={uploadAllFiles}
                disabled={isUploading || selectedFiles.length === 0}
                loading={isUploading}
                label="رفع الصور"
              >
                رفع الصور
              </ActionButton>
            </div>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-3">
            {selectedFiles.map((file, index) => {
              const progressEntries = Object.entries(uploadProgress);
              const progressEntry = progressEntries.find(([_, progress]) => 
                progress.filename === file.name
              );
              
              return (
                <PreviewImageItem
                  key={`${file.name}-${index}`}
                  file={file}
                  progress={progressEntry?.[1]}
                  onRemove={() => removeFile(index)}
                  onCategoryChange={(category) => changeFileCategory(file.name, category)}
                  category={fileCategories[file.name] || defaultCategory}
                />
              );
            })}
          </div>
        </div>
      )}

      {/* تقدم الرفع الإجمالي - Overall upload progress */}
      {showProgress && isUploading && (
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <div className="flex items-center gap-3">
            <LoadingSpinner size="sm" />
            <div className="flex-1">
              <p className="text-sm font-medium text-blue-800">جاري رفع الصور...</p>
              <p className="text-xs text-blue-600 mt-1">
                {Object.values(uploadProgress).filter(p => p.status === 'completed').length} / {selectedFiles.length} مكتملة
              </p>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};