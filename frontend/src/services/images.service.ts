/**
 * خدمة إدارة الصور - خدمة شاملة للتعامل مع رفع وإدارة وتحرير الصور
 * Images Management Service - Comprehensive service for uploading, managing and editing images
 */

import { apiClient } from './api.service';
import type {
  Image,
  UploadImageRequest,
  UploadImageResponse,
  UpdateImageRequest,
  GetImagesRequest,
  GetImagesResponse,
  DeleteImagesRequest,
  ReorderImagesRequest,
  ImageStatistics,
  UploadProgress,
  ImageSearchFilter,
  ImageProcessingSettings
} from '../types/image.types';

/**
 * خدمة إدارة الصور الرئيسية
 * Main images management service
 */
export class ImagesService {
  private static readonly BASE_URL = '/api/images';

  /**
   * رفع صورة واحدة مع تتبع التقدم
   * Upload single image with progress callback
   * @param request طلب رفع الصورة
   * @param onProgress دالة استدعاء لتعقب نسبة الرفع (0-100)
   */
  static async uploadImage(
    request: UploadImageRequest,
    onProgress?: (percent: number) => void
  ): Promise<UploadImageResponse> {
    // إعداد FormData لرفع الصورة
    const formData = new FormData();
    formData.append('file', request.file);
    
    if (request.propertyId) formData.append('propertyId', request.propertyId);
    if (request.unitId) formData.append('unitId', request.unitId);
    formData.append('category', request.category);
    if (request.alt) formData.append('alt', request.alt);
    if (request.isPrimary !== undefined) formData.append('isPrimary', String(request.isPrimary));
    if (request.order !== undefined) formData.append('order', String(request.order));
    if (request.tags) formData.append('tags', JSON.stringify(request.tags));

    const response = await apiClient.post<UploadImageResponse>(
      `${this.BASE_URL}/upload`,
      formData,
      {
        headers: { 'Content-Type': 'multipart/form-data' },
        onUploadProgress: (event) => {
          if (onProgress && event.total) {
            const percent = Math.round((event.loaded * 100) / event.total);
            onProgress(percent);
          }
        }
      }
    );
    return response.data;
  }

  /**
   * رفع صور متعددة وإرجاع مصفوفة استجابات
   * Upload multiple images and return array of responses
   */
  static async uploadMultipleImages(
    files: File[],
    options: Omit<UploadImageRequest, 'file'>
  ): Promise<UploadImageResponse[]> {
    // رفع كل ملف واستقبال UploadImageResponse لكل واحد
    const uploadPromises = files.map(file =>
      this.uploadImage({ ...options, file })
    );
    return Promise.all(uploadPromises);
  }

  /**
   * الحصول على قائمة الصور
   * Get images list
   */
  static async getImages(request: GetImagesRequest = {}): Promise<GetImagesResponse> {
    const params = new URLSearchParams();
    
    if (request.propertyId) params.append('propertyId', request.propertyId);
    if (request.unitId) params.append('unitId', request.unitId);
    if (request.category) params.append('category', request.category);
    if (request.page) params.append('page', String(request.page));
    if (request.limit) params.append('limit', String(request.limit));
    if (request.sortBy) params.append('sortBy', request.sortBy);
    if (request.sortOrder) params.append('sortOrder', request.sortOrder);
    if (request.search) params.append('search', request.search);

    const response = await apiClient.get<GetImagesResponse>(
      `${this.BASE_URL}?${params.toString()}`
    );

    return response.data;
  }

  /**
   * الحصول على صورة واحدة
   * Get single image
   */
  static async getImage(imageId: string): Promise<Image> {
    const response = await apiClient.get<Image>(`${this.BASE_URL}/${imageId}`);
    return response.data;
  }

  /**
   * تحديث بيانات الصورة
   * Update image data
   */
  static async updateImage(request: UpdateImageRequest): Promise<Image> {
    const { id, ...updateData } = request;
    const response = await apiClient.put<Image>(`${this.BASE_URL}/${id}`, updateData);
    return response.data;
  }

  /**
   * حذف صورة واحدة
   * Delete single image
   */
  static async deleteImage(imageId: string, permanent = false): Promise<void> {
    await apiClient.delete(`${this.BASE_URL}/${imageId}?permanent=${permanent}`);
  }

  /**
   * حذف صور متعددة
   * Delete multiple images
   */
  static async deleteImages(request: DeleteImagesRequest): Promise<void> {
    await apiClient.post(`${ImagesService.BASE_URL}/bulk-delete`, request);
  }

  /**
   * إعادة ترتيب الصور
   * Reorder images
   */
  static async reorderImages(request: ReorderImagesRequest): Promise<void> {
    await apiClient.post(`${ImagesService.BASE_URL}/reorder`, request);
  }

  /**
   * تعيين صورة كرئيسية
   * Set image as primary
   */
  static async setPrimaryImage(imageId: string, propertyId?: string, unitId?: string): Promise<void> {
    await apiClient.post(`${ImagesService.BASE_URL}/${imageId}/set-primary`, {
      propertyId,
      unitId
    });
  }

  /**
   * الحصول على إحصائيات الصور
   * Get images statistics
   */
  static async getImageStatistics(propertyId?: string, unitId?: string): Promise<ImageStatistics> {
    const params = new URLSearchParams();
    if (propertyId) params.append('propertyId', propertyId);
    if (unitId) params.append('unitId', unitId);

    const response = await apiClient.get<ImageStatistics>(
      `${this.BASE_URL}/statistics?${params.toString()}`
    );

    return response.data;
  }

  /**
   * البحث المتقدم في الصور
   * Advanced image search
   */
  static async searchImages(filter: ImageSearchFilter): Promise<GetImagesResponse> {
    const response = await apiClient.post<GetImagesResponse>(
      `${this.BASE_URL}/search`,
      filter
    );

    return response.data;
  }

  /**
   * تتبع تقدم رفع الصورة
   * Track upload progress
   */
  static async getUploadProgress(taskId: string): Promise<UploadProgress> {
    const response = await apiClient.get<UploadProgress>(
      `${this.BASE_URL}/upload-progress/${taskId}`
    );

    return response.data;
  }

  /**
   * الحصول على رابط تحميل مؤقت
   * Get temporary download URL
   */
  static async getDownloadUrl(imageId: string, size?: 'small' | 'medium' | 'large' | 'hd' | 'original'): Promise<string> {
    const params = size ? `?size=${size}` : '';
    const response = await apiClient.get<{ url: string }>(
      `${this.BASE_URL}/${imageId}/download${params}`
    );

    return response.data.url;
  }

  /**
   * تحسين الصور تلقائياً
   * Auto-optimize images
   */
  static async optimizeImage(imageId: string, settings: ImageProcessingSettings): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/optimize`,
      settings
    );

    return response.data;
  }

  /**
   * إنشاء صور مصغرة إضافية
   * Generate additional thumbnails
   */
  static async generateThumbnails(imageId: string, sizes: string[]): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/thumbnails`,
      { sizes }
    );

    return response.data;
  }

  /**
   * نسخ صورة لكيان أو وحدة أخرى
   * Copy image to another property or unit
   */
  static async copyImage(
    imageId: string, 
    targetPropertyId?: string, 
    targetUnitId?: string
  ): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/copy`,
      {
        targetPropertyId,
        targetUnitId
      }
    );

    return response.data;
  }

  /**
   * تحويل تنسيق الصورة
   * Convert image format
   */
  static async convertImageFormat(
    imageId: string, 
    format: 'jpeg' | 'png' | 'webp',
    quality?: number
  ): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/convert`,
      {
        format,
        quality
      }
    );

    return response.data;
  }

  /**
   * إضافة علامة مائية للصورة
   * Add watermark to image
   */
  static async addWatermark(
    imageId: string,
    watermarkText: string,
    position: 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' | 'center' = 'bottom-right'
  ): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/watermark`,
      {
        text: watermarkText,
        position
      }
    );

    return response.data;
  }

  /**
   * تدوير الصورة
   * Rotate image
   */
  static async rotateImage(imageId: string, degrees: 90 | 180 | 270): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/rotate`,
      { degrees }
    );

    return response.data;
  }

  /**
   * قلب الصورة
   * Flip image
   */
  static async flipImage(imageId: string, direction: 'horizontal' | 'vertical'): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/flip`,
      { direction }
    );

    return response.data;
  }

  /**
   * اقتصاص الصورة
   * Crop image
   */
  static async cropImage(
    imageId: string,
    cropData: {
      x: number;
      y: number;
      width: number;
      height: number;
    }
  ): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/crop`,
      cropData
    );

    return response.data;
  }

  /**
   * تطبيق فلتر على الصورة
   * Apply filter to image
   */
  static async applyFilter(
    imageId: string,
    filter: 'blur' | 'sharpen' | 'brightness' | 'contrast' | 'saturation' | 'vintage' | 'blackwhite',
    intensity: number = 50
  ): Promise<Image> {
    const response = await apiClient.post<Image>(
      `${this.BASE_URL}/${imageId}/filter`,
      {
        filter,
        intensity
      }
    );

    return response.data;
  }

  /**
   * استعادة النسخة الأصلية للصورة
   * Restore original image version
   */
  static async restoreOriginal(imageId: string): Promise<Image> {
    const response = await apiClient.post<Image>(`${this.BASE_URL}/${imageId}/restore`);
    return response.data;
  }

  /**
   * الحصول على تاريخ التعديلات
   * Get edit history
   */
  static async getEditHistory(imageId: string): Promise<any[]> {
    const response = await apiClient.get<any[]>(`${this.BASE_URL}/${imageId}/history`);
    return response.data;
  }

  /**
   * التحقق من صحة الصورة قبل الرفع
   * Validate image before upload
   */
  static validateImageFile(file: File): { valid: boolean; error?: string } {
    // التحقق من نوع الملف - Check file type
    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'];
    if (!allowedTypes.includes(file.type)) {
      return {
        valid: false,
        error: 'نوع الملف غير مدعوم. يرجى اختيار صورة بصيغة JPEG أو PNG أو WebP أو GIF'
      };
    }

    // التحقق من حجم الملف (10MB كحد أقصى) - Check file size (10MB max)
    const maxSize = 10 * 1024 * 1024; // 10MB
    if (file.size > maxSize) {
      return {
        valid: false,
        error: 'حجم الملف كبير جداً. الحد الأقصى هو 10 ميجابايت'
      };
    }

    return { valid: true };
  }

  /**
   * ضغط الصورة قبل الرفع
   * Compress image before upload
   */
  static async compressImage(file: File, quality: number = 0.8): Promise<File> {
    return new Promise((resolve) => {
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d');
      const img = new Image();

      img.onload = () => {
        // تحديد الأبعاد المناسبة - Set appropriate dimensions
        const maxWidth = 1920;
        const maxHeight = 1080;
        
        let { width, height } = img;
        
        if (width > maxWidth) {
          height = (height * maxWidth) / width;
          width = maxWidth;
        }
        
        if (height > maxHeight) {
          width = (width * maxHeight) / height;
          height = maxHeight;
        }

        canvas.width = width;
        canvas.height = height;

        // رسم الصورة المضغوطة - Draw compressed image
        ctx?.drawImage(img, 0, 0, width, height);

        canvas.toBlob(
          (blob) => {
            if (blob) {
              const compressedFile = new File([blob], file.name, {
                type: file.type,
                lastModified: Date.now()
              });
              resolve(compressedFile);
            } else {
              resolve(file);
            }
          },
          file.type,
          quality
        );
      };

      img.src = URL.createObjectURL(file);
    });
  }
}