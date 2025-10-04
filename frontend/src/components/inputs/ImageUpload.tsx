import React, { useState, useRef, useCallback } from 'react';
import { ImagesService } from '../../services/images.service';

interface ImageUploadProps {
  value?: string | string[];
  onChange: (urls: string | string[]) => void;
  multiple?: boolean;
  maxFiles?: number;
  maxSize?: number; // بالميجابايت
  acceptedFormats?: string[];
  showPreview?: boolean;
  className?: string;
  disabled?: boolean;
  required?: boolean;
  placeholder?: string;
  uploadEndpoint?: string;
}

interface ImageFile {
  file: File;
  url: string;
  uploading?: boolean;
  uploaded?: boolean;
  error?: string;
  progress?: number;
}

const ImageUpload: React.FC<ImageUploadProps> = ({
  value = '',
  onChange,
  multiple = false,
  maxFiles = 10,
  maxSize = 5,
  acceptedFormats = ['jpg', 'jpeg', 'png', 'gif', 'webp'],
  showPreview = true,
  className = '',
  disabled = false,
  required = false,
  placeholder = 'اضغط لاختيار الصور أو اسحبها هنا',
  uploadEndpoint = '/api/upload/images'
}) => {
  const [imageFiles, setImageFiles] = useState<ImageFile[]>([]);
  const [dragOver, setDragOver] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // تحويل القيم الحالية إلى مصفوفة
  const currentUrls = Array.isArray(value) ? value : value ? [value] : [];

  // التحقق من صحة الملف
  const validateFile = (file: File): string | null => {
    // التحقق من الحجم
    if (file.size > maxSize * 1024 * 1024) {
      return `حجم الملف كبير جداً (الحد الأقصى ${maxSize}MB)`;
    }

    // التحقق من النوع
    const extension = file.name.split('.').pop()?.toLowerCase();
    if (!extension || !acceptedFormats.includes(extension)) {
      return `نوع الملف غير مدعوم (المدعوم: ${acceptedFormats.join(', ')})`;
    }

    // التحقق من عدد الملفات
    if (!multiple && (currentUrls.length + imageFiles.length) >= 1) {
      return 'يمكن رفع صورة واحدة فقط';
    }

    if ((currentUrls.length + imageFiles.length) >= maxFiles) {
      return `تم الوصول للحد الأقصى (${maxFiles} صور)`;
    }

    return null;
  };

  // معالجة اختيار الملفات
  const handleFiles = useCallback((files: FileList) => {
    const newFiles: ImageFile[] = [];

    Array.from(files).forEach((file) => {
      const error = validateFile(file);
      
      const imageFile: ImageFile = {
        file,
        url: URL.createObjectURL(file),
        uploading: false,
        uploaded: false,
        error: error || undefined,
        progress: 0
      };

      newFiles.push(imageFile);
    });

    setImageFiles(prev => [...prev, ...newFiles]);
  }, [currentUrls.length, imageFiles.length, maxFiles, multiple]);

  // رفع صورة واحدة باستخدام خدمة الصور
  const uploadImage = async (imageFile: ImageFile): Promise<string> => {
    try {
      // استخدام ImagesService لرفع الصورة
      const response = await ImagesService.uploadImage({
        file: imageFile.file,
        category: 'gallery'
      });
      if (!response.success) {
        throw new Error(response.error || 'فشل في رفع الصورة');
      }else{
        console.log(response.image?.url);
        console.log(response.image?.url);
      }

      return response.image?.url || '';
    } catch (error) {
      console.error('خطأ في رفع الصورة:', error);
      throw error;
    }
  };

  // رفع جميع الصور
  const uploadAllImages = async () => {
    if (imageFiles.length === 0) return;

    setIsUploading(true);
    const uploadedUrls: string[] = [...currentUrls];

    // تحديث حالة الرفع
    setImageFiles(prev => prev.map(img => ({ ...img, uploading: true, error: undefined })));

    for (let i = 0; i < imageFiles.length; i++) {
      const imageFile = imageFiles[i];
      
      if (imageFile.error) {
        continue;
      }

      try {
        // رفع ومتابعة التقدم
        const uploadedUrl = await ImagesService.uploadImage({ file: imageFile.file, category: 'gallery' }, (percent) => {
          setImageFiles(prev => prev.map((img, idx) =>
            idx === i ? { ...img, progress: percent } : img
          ));
        });
        if (uploadedUrl) {
          uploadedUrls.push(uploadedUrl.image?.url || '');
          
          // تحديث حالة الصورة المكتملة
          setImageFiles(prev => prev.map((img, index) =>
            index === i ? { ...img, uploading: false, uploaded: true, progress: 100 } : img
          ));
        }
      } catch (error) {
        // تحديث حالة الخطأ
        setImageFiles(prev => prev.map((img, index) =>
          index === i ? {
            ...img,
            uploading: false,
            error: 'فشل في رفع الصورة'
          } : img
        ));
      }
    }

    // تحديث القيم
    const finalUrls = multiple ? uploadedUrls : uploadedUrls[uploadedUrls.length - 1] || '';
    onChange(finalUrls);

    // مسح الملفات المرفوعة بنجاح
    setImageFiles(prev => prev.filter(img => !img.uploaded));
    setIsUploading(false);
  };

  // حذف صورة من المعاينة
  const removeImageFile = (index: number) => {
    setImageFiles(prev => {
      const newFiles = [...prev];
      URL.revokeObjectURL(newFiles[index].url);
      newFiles.splice(index, 1);
      return newFiles;
    });
  };

  // حذف صورة مرفوعة
  const removeUploadedImage = (index: number) => {
    const newUrls = [...currentUrls];
    newUrls.splice(index, 1);
    const finalUrls = multiple ? newUrls : newUrls[0] || '';
    onChange(finalUrls);
  };

  // معالجة السحب والإفلات
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
    
    if (disabled) return;

    const files = e.dataTransfer.files;
    if (files.length > 0) {
      handleFiles(files);
    }
  };

  // معالجة اختيار الملفات من المتصفح
  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      handleFiles(files);
    }
    // مسح قيمة الإدخال للسماح بإعادة اختيار نفس الملف
    e.target.value = '';
  };

  // فتح متصفح الملفات
  const openFileDialog = () => {
    if (!disabled && fileInputRef.current) {
      fileInputRef.current.click();
    }
  };

  return (
    <div className={`space-y-4 ${className}`}>
      {/* منطقة الرفع */}
      <div
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        onClick={openFileDialog}
        className={`
          relative border-2 border-dashed rounded-lg p-6 text-center cursor-pointer transition-colors
          ${dragOver ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-gray-400'}
          ${disabled ? 'opacity-50 cursor-not-allowed' : ''}
          ${required && currentUrls.length === 0 && imageFiles.length === 0 ? 'border-red-300 bg-red-50' : ''}
        `}
      >
        <input
          ref={fileInputRef}
          type="file"
          multiple={multiple}
          accept={acceptedFormats.map(format => `.${format}`).join(',')}
          onChange={handleFileInputChange}
          disabled={disabled}
          className="hidden"
        />

        <div className="space-y-2">
          <div className="text-4xl">📸</div>
          <div>
            <p className="text-sm font-medium text-gray-900">{placeholder}</p>
            <p className="text-xs text-gray-500 mt-1">
              الأنواع المدعومة: {acceptedFormats.join(', ')} • الحد الأقصى: {maxSize}MB
            </p>
            {multiple && (
              <p className="text-xs text-gray-400">
                يمكن رفع حتى {maxFiles} صور
              </p>
            )}
          </div>
        </div>
      </div>

      {/* معاينة الصور الحالية */}
      {showPreview && currentUrls.length > 0 && (
        <div className="space-y-2">
          <h4 className="text-sm font-medium text-gray-900">الصور الحالية:</h4>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {currentUrls.map((url, index) => (
              <div key={index} className="relative group">
                <img
                  src={url}
                  alt={`صورة ${index + 1}`}
                  className="w-full h-24 object-cover rounded-lg border border-gray-200"
                />
                {!disabled && (
                  <button
                    type="button"
                    onClick={() => removeUploadedImage(index)}
                    className="absolute top-1 left-1 w-6 h-6 bg-red-500 text-white rounded-full text-xs hover:bg-red-600 opacity-0 group-hover:opacity-100 transition-opacity"
                  >
                    ×
                  </button>
                )}
              </div>
            ))}
          </div>
        </div>
      )}

      {/* معاينة الصور الجديدة */}
      {showPreview && imageFiles.length > 0 && (
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <h4 className="text-sm font-medium text-gray-900">صور جديدة:</h4>
            <button
              type="button"
              onClick={uploadAllImages}
              disabled={disabled || isUploading || imageFiles.every(img => img.error)}
              className="px-3 py-1 bg-blue-600 text-white text-sm rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isUploading ? 'جارٍ الرفع...' : 'رفع الصور'}
            </button>
          </div>
          
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {imageFiles.map((imageFile, index) => (
              <div key={index} className="relative group">
                <img
                  src={imageFile.url}
                  alt={`معاينة ${index + 1}`}
                  className={`
                    w-full h-24 object-cover rounded-lg border-2
                    ${imageFile.error ? 'border-red-300' : 'border-gray-200'}
                    ${imageFile.uploading ? 'opacity-50' : ''}
                  `}
                />
                
                {/* مؤشر الحالة */}
                <div className="absolute inset-0 flex flex-col items-center justify-center space-y-1">
                  {imageFile.uploading && (
                    <>
                      <progress value={imageFile.progress || 0} max={100} className="w-10 h-2" />
                      <span className="text-white text-xs">{imageFile.progress || 0}%</span>
                    </>
                  )}
                  {imageFile.uploaded && (
                    <div className="w-6 h-6 bg-green-500 text-white rounded-full flex items-center justify-center text-xs">
                      ✓
                    </div>
                  )}
                  {imageFile.error && (
                    <div className="w-6 h-6 bg-red-500 text-white rounded-full flex items-center justify-center text-xs">
                      ✗
                    </div>
                  )}
                </div>

                {/* زر الحذف */}
                {!disabled && !imageFile.uploading && (
                  <button
                    type="button"
                    onClick={() => removeImageFile(index)}
                    className="absolute top-1 left-1 w-6 h-6 bg-red-500 text-white rounded-full text-xs hover:bg-red-600 opacity-0 group-hover:opacity-100 transition-opacity"
                  >
                    ×
                  </button>
                )}

                {/* رسالة الخطأ */}
                {imageFile.error && (
                  <div className="absolute bottom-0 left-0 right-0 bg-red-500 text-white text-xs p-1 rounded-b-lg">
                    {imageFile.error}
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      )}

      {/* تحذير التحقق */}
      {required && currentUrls.length === 0 && imageFiles.length === 0 && (
        <div className="text-sm text-red-600">
          ⚠️ رفع صورة واحدة على الأقل مطلوب
        </div>
      )}

      {/* إحصائيات */}
      {(currentUrls.length > 0 || imageFiles.length > 0) && (
        <div className="text-xs text-gray-500 text-center">
          الصور الحالية: {currentUrls.length} • صور جديدة: {imageFiles.length} • المجموع: {currentUrls.length + imageFiles.length}/{maxFiles}
        </div>
      )}
    </div>
  );
};

export default ImageUpload;