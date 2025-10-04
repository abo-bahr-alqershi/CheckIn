/**
 * هوك إدارة الصور - هوك شامل لإدارة رفع وتحرير ومعرض الصور
 * Images Management Hook - Comprehensive hook for managing image upload, editing and gallery
 */

import { useState, useCallback } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ImagesService } from '../services/images.service';
import type {
  Image,
  UploadImageRequest,
  UpdateImageRequest,
  GetImagesRequest,
  DeleteImagesRequest,
  ReorderImagesRequest,
  ImageStatistics,
  UploadProgress,
  ImageSearchFilter,
  ImageProcessingSettings
} from '../types/image.types';

/**
 * هوك إدارة الصور الرئيسي
 * Main images management hook
 */
export const useImages = (query: GetImagesRequest = {}) => {
  const queryClient = useQueryClient();
  const [uploadProgress, setUploadProgress] = useState<Record<string, UploadProgress>>({});

  // الحصول على قائمة الصور - Get images list
  const {
    data: imagesData,
    isLoading,
    error,
    refetch
  } = useQuery({
    queryKey: ['images', query],
    queryFn: () => ImagesService.getImages(query),
    staleTime: 5 * 60 * 1000, // 5 دقائق - 5 minutes
  });

  // رفع صورة واحدة - Upload single image
  const uploadImageMutation = useMutation({
    mutationFn: async (request: UploadImageRequest) => {
      // التحقق من صحة الصورة - Validate image
      const validation = ImagesService.validateImageFile(request.file);
      if (!validation.valid) {
        throw new Error(validation.error);
      }

      // ضغط الصورة قبل الرفع - Compress image before upload
      const compressedFile = await ImagesService.compressImage(request.file);
      const compressedRequest = { ...request, file: compressedFile };

      return ImagesService.uploadImage(compressedRequest);
    },
    onSuccess: (data) => {
      // تحديث الكاش - Update cache
      queryClient.invalidateQueries({ queryKey: ['images'] });
      queryClient.invalidateQueries({ queryKey: ['image-statistics'] });
      
      // إزالة تقدم الرفع - Remove upload progress
      if (data.taskId) {
        setUploadProgress(prev => {
          const { [data.taskId!]: removed, ...rest } = prev;
          return rest;
        });
      }
    },
    onError: (error: any, variables) => {
      console.error('فشل في رفع الصورة:', error);
      // إزالة تقدم الرفع عند الفشل - Remove upload progress on failure
      const taskId = `upload_${variables.file.name}_${Date.now()}`;
      setUploadProgress(prev => {
        const { [taskId]: removed, ...rest } = prev;
        return rest;
      });
    }
  });

  // رفع صور متعددة - Upload multiple images
  const uploadMultipleImagesMutation = useMutation({
    mutationFn: async ({ files, options }: { files: File[], options: Omit<UploadImageRequest, 'file'> }) => {
      const results = await ImagesService.uploadMultipleImages(files, options);
      return results;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
      queryClient.invalidateQueries({ queryKey: ['image-statistics'] });
    }
  });

  // تحديث بيانات الصورة - Update image data
  const updateImageMutation = useMutation({
    mutationFn: ImagesService.updateImage,
    onSuccess: (updatedImage) => {
      // تحديث الكاش المحلي - Update local cache
      queryClient.setQueryData(['images', query], (oldData: any) => {
        if (!oldData) return oldData;
        
        return {
          ...oldData,
          images: oldData.images.map((img: Image) => 
            img.id === updatedImage.id ? updatedImage : img
          )
        };
      });
      
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // حذف صورة واحدة - Delete single image
  const deleteImageMutation = useMutation({
    mutationFn: ({ imageId, permanent = false }: { imageId: string, permanent?: boolean }) =>
      ImagesService.deleteImage(imageId, permanent),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
      queryClient.invalidateQueries({ queryKey: ['image-statistics'] });
    }
  });

  // حذف صور متعددة - Delete multiple images
  const deleteImagesMutation = useMutation({
    mutationFn: ImagesService.deleteImages,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
      queryClient.invalidateQueries({ queryKey: ['image-statistics'] });
    }
  });

  // إعادة ترتيب الصور - Reorder images
  const reorderImagesMutation = useMutation({
    mutationFn: ImagesService.reorderImages,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // تعيين صورة كرئيسية - Set image as primary
  const setPrimaryImageMutation = useMutation({
    mutationFn: ({ imageId, propertyId, unitId }: { 
      imageId: string, 
      propertyId?: string, 
      unitId?: string 
    }) => ImagesService.setPrimaryImage(imageId, propertyId, unitId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // تحسين الصورة - Optimize image
  const optimizeImageMutation = useMutation({
    mutationFn: ({ imageId, settings }: { imageId: string, settings: ImageProcessingSettings }) =>
      ImagesService.optimizeImage(imageId, settings),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // اقتصاص الصورة - Crop image
  const cropImageMutation = useMutation({
    mutationFn: ({ 
      imageId, 
      cropData 
    }: { 
      imageId: string, 
      cropData: { x: number; y: number; width: number; height: number } 
    }) => ImagesService.cropImage(imageId, cropData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // تطبيق فلتر - Apply filter
  const applyFilterMutation = useMutation({
    mutationFn: ({ 
      imageId, 
      filter, 
      intensity 
    }: { 
      imageId: string, 
      filter: string, 
      intensity?: number 
    }) => ImagesService.applyFilter(imageId, filter as any, intensity),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // تدوير الصورة - Rotate image
  const rotateImageMutation = useMutation({
    mutationFn: ({ imageId, degrees }: { imageId: string, degrees: 90 | 180 | 270 }) =>
      ImagesService.rotateImage(imageId, degrees),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // قلب الصورة - Flip image
  const flipImageMutation = useMutation({
    mutationFn: ({ imageId, direction }: { imageId: string, direction: 'horizontal' | 'vertical' }) =>
      ImagesService.flipImage(imageId, direction),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // إضافة علامة مائية - Add watermark
  const addWatermarkMutation = useMutation({
    mutationFn: ({ 
      imageId, 
      text, 
      position 
    }: { 
      imageId: string, 
      text: string, 
      position?: string 
    }) => ImagesService.addWatermark(imageId, text, position as any),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // نسخ الصورة - Copy image
  const copyImageMutation = useMutation({
    mutationFn: ({ 
      imageId, 
      targetPropertyId, 
      targetUnitId 
    }: { 
      imageId: string, 
      targetPropertyId?: string, 
      targetUnitId?: string 
    }) => ImagesService.copyImage(imageId, targetPropertyId, targetUnitId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // استعادة النسخة الأصلية - Restore original
  const restoreOriginalMutation = useMutation({
    mutationFn: ImagesService.restoreOriginal,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images'] });
    }
  });

  // دالة رفع صورة مع تتبع التقدم - Upload image with progress tracking
  const uploadImageWithProgress = useCallback(async (request: UploadImageRequest) => {
    const taskId = `upload_${request.file.name}_${Date.now()}`;
    
    // تهيئة تقدم الرفع - Initialize upload progress
    setUploadProgress(prev => ({
      ...prev,
      [taskId]: {
        taskId,
        filename: request.file.name,
        progress: 0,
        status: 'uploading'
      }
    }));

    try {
      // محاكاة تقدم الرفع - Simulate upload progress
      const progressInterval = setInterval(() => {
        setUploadProgress(prev => {
          const current = prev[taskId];
          if (current && current.progress < 90) {
            return {
              ...prev,
              [taskId]: {
                ...current,
                progress: current.progress + 10
              }
            };
          }
          return prev;
        });
      }, 200);

      const result = await uploadImageMutation.mutateAsync(request);

      clearInterval(progressInterval);

      // إكمال التقدم - Complete progress
      setUploadProgress(prev => ({
        ...prev,
        [taskId]: {
          taskId,
          filename: request.file.name,
          progress: 100,
          status: 'completed',
          imageId: result.image?.id
        }
      }));

      return result;
    } catch (error) {
      // تحديث حالة الخطأ - Update error status
      setUploadProgress(prev => ({
        ...prev,
        [taskId]: {
          taskId,
          filename: request.file.name,
          progress: 0,
          status: 'failed',
          error: error instanceof Error ? error.message : 'حدث خطأ غير متوقع'
        }
      }));
      throw error;
    }
  }, [uploadImageMutation]);

  // مسح تقدم الرفع - Clear upload progress
  const clearUploadProgress = useCallback((taskId: string) => {
    setUploadProgress(prev => {
      const { [taskId]: removed, ...rest } = prev;
      return rest;
    });
  }, []);

  // مسح جميع تقدم الرفع - Clear all upload progress
  const clearAllUploadProgress = useCallback(() => {
    setUploadProgress({});
  }, []);

  return {
    // البيانات - Data
    images: imagesData?.images || [],
    total: imagesData?.total || 0,
    page: imagesData?.page || 1,
    totalPages: imagesData?.totalPages || 1,
    isLoading,
    error,
    uploadProgress,

    // العمليات - Operations
    refetch,
    uploadImage: uploadImageMutation.mutate,
    uploadImageAsync: uploadImageMutation.mutateAsync,
    uploadImageWithProgress,
    uploadMultipleImages: uploadMultipleImagesMutation.mutate,
    uploadMultipleImagesAsync: uploadMultipleImagesMutation.mutateAsync,
    updateImage: updateImageMutation.mutate,
    updateImageAsync: updateImageMutation.mutateAsync,
    deleteImage: deleteImageMutation.mutate,
    deleteImageAsync: deleteImageMutation.mutateAsync,
    deleteImages: deleteImagesMutation.mutate,
    deleteImagesAsync: deleteImagesMutation.mutateAsync,
    reorderImages: reorderImagesMutation.mutate,
    reorderImagesAsync: reorderImagesMutation.mutateAsync,
    setPrimaryImage: setPrimaryImageMutation.mutate,
    setPrimaryImageAsync: setPrimaryImageMutation.mutateAsync,
    optimizeImage: optimizeImageMutation.mutate,
    optimizeImageAsync: optimizeImageMutation.mutateAsync,
    cropImage: cropImageMutation.mutate,
    cropImageAsync: cropImageMutation.mutateAsync,
    applyFilter: applyFilterMutation.mutate,
    applyFilterAsync: applyFilterMutation.mutateAsync,
    rotateImage: rotateImageMutation.mutate,
    rotateImageAsync: rotateImageMutation.mutateAsync,
    flipImage: flipImageMutation.mutate,
    flipImageAsync: flipImageMutation.mutateAsync,
    addWatermark: addWatermarkMutation.mutate,
    addWatermarkAsync: addWatermarkMutation.mutateAsync,
    copyImage: copyImageMutation.mutate,
    copyImageAsync: copyImageMutation.mutateAsync,
    restoreOriginal: restoreOriginalMutation.mutate,
    restoreOriginalAsync: restoreOriginalMutation.mutateAsync,

    // حالات التحميل - Loading states
    isUploading: uploadImageMutation.isPending || uploadMultipleImagesMutation.isPending,
    isUpdating: updateImageMutation.isPending,
    isDeleting: deleteImageMutation.isPending || deleteImagesMutation.isPending,
    isReordering: reorderImagesMutation.isPending,
    isOptimizing: optimizeImageMutation.isPending,
    isEditing: cropImageMutation.isPending || applyFilterMutation.isPending || 
               rotateImageMutation.isPending || flipImageMutation.isPending ||
               addWatermarkMutation.isPending,
    isProcessing: restoreOriginalMutation.isPending || copyImageMutation.isPending,

    // مساعدات - Helpers
    clearUploadProgress,
    clearAllUploadProgress
  };
};

/**
 * هوك إحصائيات الصور
 * Images statistics hook
 */
export const useImageStatistics = (propertyId?: string, unitId?: string) => {
  return useQuery({
    queryKey: ['image-statistics', propertyId, unitId],
    queryFn: () => ImagesService.getImageStatistics(propertyId, unitId),
    staleTime: 10 * 60 * 1000, // 10 دقائق - 10 minutes
  });
};

/**
 * هوك البحث المتقدم في الصور
 * Advanced image search hook
 */
export const useImageSearch = () => {
  const [searchFilter, setSearchFilter] = useState<ImageSearchFilter>({});
  
  const {
    data: searchResults,
    isLoading: isSearching,
    error: searchError,
    refetch: search
  } = useQuery({
    queryKey: ['image-search', searchFilter],
    queryFn: () => ImagesService.searchImages(searchFilter),
    enabled: Object.keys(searchFilter).length > 0,
    staleTime: 2 * 60 * 1000, // دقيقتان - 2 minutes
  });

  return {
    searchResults: searchResults?.images || [],
    total: searchResults?.total || 0,
    isSearching,
    searchError,
    searchFilter,
    setSearchFilter,
    search
  };
};

/**
 * هوك تحميل صورة واحدة
 * Single image hook
 */
export const useImage = (imageId: string) => {
  return useQuery({
    queryKey: ['image', imageId],
    queryFn: () => ImagesService.getImage(imageId),
    enabled: !!imageId,
    staleTime: 5 * 60 * 1000, // 5 دقائق - 5 minutes
  });
};

/**
 * هوك تاريخ تعديلات الصورة
 * Image edit history hook
 */
export const useImageHistory = (imageId: string) => {
  return useQuery({
    queryKey: ['image-history', imageId],
    queryFn: () => ImagesService.getEditHistory(imageId),
    enabled: !!imageId,
    staleTime: 5 * 60 * 1000, // 5 دقائق - 5 minutes
  });
};