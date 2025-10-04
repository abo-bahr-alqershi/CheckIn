import React, { useState, useRef } from 'react';
import ActionButton from './ActionButton';

interface ImageUploadProps {
  currentImage?: string;
  onImageUpload: (file: File) => void;
  onImageDelete?: () => void;
  isUploading?: boolean;
  disabled?: boolean;
  size?: 'sm' | 'md' | 'lg';
  showDeleteButton?: boolean;
  acceptedTypes?: string[];
  maxFileSize?: number; // بالميجابايت
  placeholder?: string;
}

const ImageUpload: React.FC<ImageUploadProps> = ({
  currentImage,
  onImageUpload,
  onImageDelete,
  isUploading = false,
  disabled = false,
  size = 'md',
  showDeleteButton = true,
  acceptedTypes = ['image/jpeg', 'image/png', 'image/jpg'],
  maxFileSize = 2, // 2MB
  placeholder
}) => {
  const [dragOver, setDragOver] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const sizeClasses = {
    sm: 'w-16 h-16',
    md: 'w-24 h-24',
    lg: 'w-32 h-32'
  };

  const handleFileSelect = (file: File | null) => {
    if (!file) return;

    // التحقق من نوع الملف
    if (!acceptedTypes.includes(file.type)) {
      alert(`نوع الملف غير مدعوم. الأنواع المدعومة: ${acceptedTypes.join(', ')}`);
      return;
    }

    // التحقق من حجم الملف
    const maxSizeInBytes = maxFileSize * 1024 * 1024;
    if (file.size > maxSizeInBytes) {
      alert(`حجم الملف كبير جداً. الحد الأقصى: ${maxFileSize}MB`);
      return;
    }

    onImageUpload(file);
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(false);
    
    const files = e.dataTransfer.files;
    if (files && files[0]) {
      handleFileSelect(files[0]);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files[0]) {
      handleFileSelect(files[0]);
    }
  };

  const openFileDialog = () => {
    if (!disabled && !isUploading && fileInputRef.current) {
      fileInputRef.current.click();
    }
  };

  return (
    <div className="space-y-4">
      {/* منطقة عرض الصورة */}
      <div className="flex items-center space-x-6 space-x-reverse">
        <div
          className={`${sizeClasses[size]} bg-gray-200 rounded-full flex items-center justify-center overflow-hidden relative cursor-pointer border-2 border-dashed transition-colors ${
            dragOver ? 'border-blue-500 bg-blue-50' : 'border-gray-300'
          } ${disabled || isUploading ? 'opacity-50 cursor-not-allowed' : 'hover:border-blue-400'}`}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          onClick={openFileDialog}
        >
          {currentImage ? (
            <img
              src={currentImage}
              alt="الصورة الحالية"
              className="w-full h-full object-cover"
            />
          ) : (
            <div className="text-center">
              {isUploading ? (
                <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600 mx-auto"></div>
              ) : (
                <div className="text-gray-400">
                  <svg className="w-6 h-6 mx-auto mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                  </svg>
                  <p className="text-xs">{placeholder || 'اختر صورة'}</p>
                </div>
              )}
            </div>
          )}
          
          {/* أيقونة التحديث */}
          {currentImage && !isUploading && (
            <div className="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center opacity-0 hover:opacity-100 transition-opacity">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 13a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            </div>
          )}
        </div>

        {/* أزرار التحكم */}
        <div className="space-y-2">
          <input
            ref={fileInputRef}
            type="file"
            accept={acceptedTypes.join(',')}
            onChange={handleFileChange}
            className="hidden"
            disabled={disabled || isUploading}
          />
          
          <div className="space-y-2">
            <ActionButton
              variant="secondary"
              label={isUploading ? 'جاري الرفع...' : currentImage ? 'تغيير الصورة' : 'اختيار صورة'}
              onClick={openFileDialog}
              disabled={disabled || isUploading}
              className="text-sm"
            />
            
            {currentImage && showDeleteButton && onImageDelete && (
              <ActionButton
                variant="danger"
                label="حذف الصورة"
                onClick={onImageDelete}
                disabled={disabled || isUploading}
                className="text-sm"
              />
            )}
          </div>
          
          <p className="text-xs text-gray-500">
            {acceptedTypes.includes('image/jpeg') ? 'JPG' : ''} 
            {acceptedTypes.includes('image/png') ? ' أو PNG' : ''}
            . حد أقصى {maxFileSize}MB
          </p>
        </div>
      </div>

      {/* منطقة السحب والإفلات */}
      {!currentImage && (
        <div
          className={`border-2 border-dashed rounded-lg p-6 text-center transition-colors ${
            dragOver ? 'border-blue-500 bg-blue-50' : 'border-gray-300'
          } ${disabled || isUploading ? 'opacity-50 cursor-not-allowed' : 'hover:border-blue-400 cursor-pointer'}`}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          onClick={openFileDialog}
        >
          <svg className="w-12 h-12 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
          </svg>
          <p className="text-gray-600 mb-2">اسحب وأفلت الصورة هنا</p>
          <p className="text-gray-500 text-sm">أو انقر للاختيار من الجهاز</p>
        </div>
      )}
    </div>
  );
};

export default ImageUpload;