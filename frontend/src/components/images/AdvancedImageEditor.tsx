/**
 * محرر الصور المتقدم - محرر شامل للصور مع جميع الأدوات المطلوبة
 * Advanced Image Editor - Comprehensive image editor with all required tools
 */

import React, { useState, useRef, useEffect, useCallback } from 'react';
import ReactCrop, { type Crop, type PixelCrop, centerCrop, makeAspectCrop } from 'react-image-crop';
import { useImages } from '../../hooks/useImages';
import type { Image as ImageType } from '../../types/image.types';
import ActionButton from '../ui/ActionButton';
import LoadingSpinner from '../ui/LoadingSpinner';
import { useUXHelpers } from '../../hooks/useUXHelpers';
import 'react-image-crop/dist/ReactCrop.css';

interface AdvancedImageEditorProps {
  /** الصورة المراد تحريرها - Image to edit */
  image: ImageType;
  /** هل المحرر مفتوح - Is editor open */
  isOpen: boolean;
  /** دالة الإغلاق - Close callback */
  onClose: () => void;
  /** دالة الحفظ - Save callback */
  onSave?: (editedImage: ImageType) => void;
}

/**
 * أنواع أدوات التحرير
 * Edit tool types
 */
type EditTool = 
  | 'crop' // اقتصاص
  | 'rotate' // تدوير
  | 'flip' // قلب
  | 'filter' // فلاتر
  | 'text' // نص
  | 'draw' // رسم
  | 'watermark' // علامة مائية
  | 'adjust'; // تعديلات

/**
 * إعدادات الفلاتر
 * Filter settings
 */
interface FilterSettings {
  brightness: number;
  contrast: number;
  saturation: number;
  blur: number;
  sepia: number;
  hue: number;
}

/**
 * إعدادات النص
 * Text settings
 */
interface TextSettings {
  text: string;
  fontSize: number;
  fontFamily: string;
  color: string;
  bold: boolean;
  italic: boolean;
  shadow: boolean;
}

/**
 * محرر الصور المتقدم
 * Advanced Image Editor Component
 */
export const AdvancedImageEditor: React.FC<AdvancedImageEditorProps> = ({
  image,
  isOpen,
  onClose,
  onSave
}) => {
  const { loading, executeWithFeedback } = useUXHelpers();
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const imgRef = useRef<HTMLImageElement>(null);
  
  // الحالات - States
  const [activeTool, setActiveTool] = useState<EditTool>('crop');
  const [originalImageData, setOriginalImageData] = useState<string>('');
  const [currentImageData, setCurrentImageData] = useState<string>('');
  const [isProcessing, setIsProcessing] = useState(false);
  
  // إعدادات الاقتصاص - Crop settings
  const [crop, setCrop] = useState<Crop>();
  const [completedCrop, setCompletedCrop] = useState<PixelCrop>();
  const [aspect, setAspect] = useState<number | undefined>(undefined);
  
  // إعدادات الفلاتر - Filter settings
  const [filters, setFilters] = useState<FilterSettings>({
    brightness: 100,
    contrast: 100,
    saturation: 100,
    blur: 0,
    sepia: 0,
    hue: 0
  });
  
  // إعدادات النص - Text settings
  const [textSettings, setTextSettings] = useState<TextSettings>({
    text: 'النص الجديد',
    fontSize: 24,
    fontFamily: 'Arial',
    color: '#000000',
    bold: false,
    italic: false,
    shadow: false
  });
  
  // الزاوية للتدوير - Rotation angle
  const [rotationAngle, setRotationAngle] = useState(0);
  
  // هوكس الخدمات - Service hooks
  const {
    cropImageAsync,
    applyFilterAsync,
    rotateImageAsync,
    flipImageAsync,
    addWatermarkAsync,
    restoreOriginalAsync
  } = useImages();

  // تهيئة المحرر - Initialize editor
  useEffect(() => {
    if (isOpen && image) {
      setOriginalImageData(image.url);
      setCurrentImageData(image.url);
      
      // إعادة تعيين الإعدادات - Reset settings
      setFilters({
        brightness: 100,
        contrast: 100,
        saturation: 100,
        blur: 0,
        sepia: 0,
        hue: 0
      });
      setRotationAngle(0);
      setCrop(undefined);
      setCompletedCrop(undefined);
    }
  }, [isOpen, image]);

  // تهيئة Canvas للرسم والنص - Initialize Canvas for drawing and text
  useEffect(() => {
    if (isOpen && canvasRef.current) {
      const canvas = canvasRef.current;
      const ctx = canvas.getContext('2d');
      if (ctx) {
        canvas.width = 800;
        canvas.height = 600;
        ctx.clearRect(0, 0, canvas.width, canvas.height);
      }
    }
  }, [isOpen, activeTool]);

  // تطبيق الفلاتر محلياً - Apply filters locally
  const applyFiltersLocally = useCallback(() => {
    if (!imgRef.current || !canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const img = imgRef.current;
    canvas.width = img.naturalWidth;
    canvas.height = img.naturalHeight;

    // تطبيق الفلاتر - Apply filters
    ctx.filter = `
      brightness(${filters.brightness}%)
      contrast(${filters.contrast}%)
      saturate(${filters.saturation}%)
      blur(${filters.blur}px)
      sepia(${filters.sepia}%)
      hue-rotate(${filters.hue}deg)
    `;

    ctx.drawImage(img, 0, 0);
    
    // تحديث البيانات - Update image data
    const newImageData = canvas.toDataURL('image/jpeg', 0.9);
    setCurrentImageData(newImageData);
  }, [filters]);

  // تطبيق الفلاتر عند التغيير - Apply filters on change
  useEffect(() => {
    if (activeTool === 'filter' || activeTool === 'adjust') {
      applyFiltersLocally();
    }
  }, [filters, activeTool, applyFiltersLocally]);

  // معالجة الاقتصاص - Handle crop
  const handleCrop = useCallback(async () => {
    if (!completedCrop || !imgRef.current) return;

    setIsProcessing(true);
    try {
      const result = await cropImageAsync({
        imageId: image.id,
        cropData: {
          x: completedCrop.x,
          y: completedCrop.y,
          width: completedCrop.width,
          height: completedCrop.height
        }
      });
      
      if (result) {
        setCurrentImageData(result.url);
        onSave?.(result);
      }
    } catch (error) {
      console.error('فشل في اقتصاص الصورة:', error);
    } finally {
      setIsProcessing(false);
    }
  }, [completedCrop, image.id, cropImageAsync, onSave]);

  // معالجة التدوير - Handle rotation
  const handleRotate = useCallback(async (degrees: 90 | 180 | 270) => {
    setIsProcessing(true);
    try {
      const result = await executeWithFeedback(
        () => rotateImageAsync({ imageId: image.id, degrees }),
        {
          loadingKey: 'rotate-image',
          successMessage: 'تم تدوير الصورة بنجاح',
          errorMessage: 'فشل في تدوير الصورة'
        }
      );
      if (!result) return;
      setCurrentImageData(result.url);
      setRotationAngle(prev => (prev + degrees) % 360);
      onSave?.(result);
    } finally {
      setIsProcessing(false);
    }
  }, [image.id, rotateImageAsync, executeWithFeedback, onSave]);

  // معالجة القلب - Handle flip
  const handleFlip = useCallback(async (direction: 'horizontal' | 'vertical') => {
    setIsProcessing(true);
    try {
      const result = await executeWithFeedback(
        () => flipImageAsync({ imageId: image.id, direction }),
        {
          loadingKey: 'flip-image',
          successMessage: 'تم قلب الصورة بنجاح',
          errorMessage: 'فشل في قلب الصورة'
        }
      );
      if (!result) return;
      setCurrentImageData(result.url);
      onSave?.(result);
    } finally {
      setIsProcessing(false);
    }
  }, [image.id, flipImageAsync, executeWithFeedback, onSave]);

  // إضافة نص - Add text
  const handleAddText = useCallback(() => {
    if (!canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    // رسم النص على Canvas
    ctx.font = `${textSettings.bold ? 'bold' : 'normal'} ${textSettings.fontSize}px ${textSettings.fontFamily}`;
    ctx.fillStyle = textSettings.color;
    ctx.textAlign = 'center';
    
    if (textSettings.shadow) {
      ctx.shadowColor = 'rgba(0,0,0,0.5)';
      ctx.shadowBlur = 5;
      ctx.shadowOffsetX = 2;
      ctx.shadowOffsetY = 2;
    }
    
    ctx.fillText(textSettings.text, canvas.width / 2, canvas.height / 2);
    
    // إزالة الظل للنصوص التالية
    ctx.shadowColor = 'transparent';
  }, [textSettings]);

  // تطبيق التعديلات النهائية - Apply final edits
  const handleApplyEdits = useCallback(async () => {
    setIsProcessing(true);
    try {
      let result = image;
      
      // تطبيق الفلاتر إذا تم تغييرها - Apply filters if changed
      const hasFilterChanges = Object.values(filters).some((value, index) => {
        const defaultValues = [100, 100, 100, 0, 0, 0];
        return value !== defaultValues[index];
      });
      
      if (hasFilterChanges) {
        // هنا يمكن تطبيق الفلاتر عبر API
        // result = await applyFilterAsync(...);
      }
      
      // إضافة علامة مائية إذا كان هناك نص - Add watermark if there's text
      if (canvasRef.current) {
        const canvas = canvasRef.current;
        const dataURL = canvas.toDataURL('image/png');
        // هنا يمكن إرسال البيانات للخادم لدمج النص مع الصورة
      }
      
      onSave?.(result);
      onClose();
    } catch (error) {
      console.error('فشل في تطبيق التعديلات:', error);
    } finally {
      setIsProcessing(false);
    }
  }, [filters, image, onSave, onClose]);

  // استعادة النسخة الأصلية - Restore original
  const handleRestoreOriginal = useCallback(async () => {
    setIsProcessing(true);
    try {
      const result = await executeWithFeedback(
        () => restoreOriginalAsync(image.id),
        {
          loadingKey: 'restore-image',
          successMessage: 'تم استعادة النسخة الأصلية بنجاح',
          errorMessage: 'فشل في استعادة النسخة الأصلية'
        }
      );
      if (!result) return;
      setCurrentImageData(result.url);
      setFilters({
        brightness: 100,
        contrast: 100,
        saturation: 100,
        blur: 0,
        sepia: 0,
        hue: 0
      });
      onSave?.(result);
    } finally {
      setIsProcessing(false);
    }
  }, [image.id, restoreOriginalAsync, executeWithFeedback, onSave]);

  // معالجة تحميل الصورة - Handle image load
  const onImageLoad = useCallback((e: React.SyntheticEvent<HTMLImageElement>) => {
    const { width, height } = e.currentTarget;
    const crop = centerCrop(
      makeAspectCrop(
        {
          unit: '%',
          width: 90,
        },
        aspect || width / height,
        width,
        height,
      ),
      width,
      height,
    );
    setCrop(crop);
  }, [aspect]);

  // أدوات التحرير - Edit tools
  const tools = [
    { id: 'crop', name: 'اقتصاص', icon: '✂️' },
    { id: 'rotate', name: 'تدوير', icon: '🔄' },
    { id: 'flip', name: 'قلب', icon: '🔀' },
    { id: 'filter', name: 'فلاتر', icon: '🎨' },
    { id: 'adjust', name: 'تعديلات', icon: '⚙️' },
    { id: 'text', name: 'نص', icon: '📝' },
    { id: 'watermark', name: 'علامة مائية', icon: '💧' }
  ];

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg w-full max-w-7xl h-full max-h-[90vh] flex flex-col">
        {/* رأس المحرر - Editor Header */}
        <div className="flex items-center justify-between p-4 border-b">
          <h3 className="text-lg font-semibold">محرر الصور المتقدم</h3>
          <div className="flex items-center gap-2">
            <ActionButton
              variant="secondary"
              onClick={handleRestoreOriginal}
              disabled={isProcessing} label={'استعادة الأصلية'}>
            </ActionButton>
            <ActionButton
              variant="primary"
              onClick={handleApplyEdits}
              disabled={isProcessing}
              loading={isProcessing} label={'حفظ التعديلات'}            >
            </ActionButton>
            <ActionButton
              variant="secondary"
              onClick={onClose}
              disabled={isProcessing} label={'✕'}>
            </ActionButton>
          </div>
        </div>

        <div className="flex flex-1 overflow-hidden">
          {/* شريط الأدوات الجانبي - Sidebar Tools */}
          <div className="w-64 border-l p-4 overflow-y-auto">
            <div className="space-y-4">
              {/* اختيار الأداة - Tool Selection */}
              <div>
                <h4 className="font-medium mb-2">الأدوات</h4>
                <div className="grid grid-cols-2 gap-2">
                  {tools.map(tool => (
                    <button
                      key={tool.id}
                      onClick={() => setActiveTool(tool.id as EditTool)}
                      className={`
                        p-3 rounded-lg border transition-all text-sm
                        ${activeTool === tool.id 
                          ? 'border-blue-500 bg-blue-50 text-blue-700' 
                          : 'border-gray-200 hover:border-gray-300'
                        }
                      `}
                    >
                      <div className="text-lg mb-1">{tool.icon}</div>
                      {tool.name}
                    </button>
                  ))}
                </div>
              </div>

              {/* إعدادات الأداة النشطة - Active Tool Settings */}
              <div className="border-t pt-4">
                {activeTool === 'crop' && (
                  <div>
                    <h4 className="font-medium mb-2">إعدادات الاقتصاص</h4>
                    <div className="space-y-2">
                      <label className="block">
                        <span className="text-sm text-gray-600">النسبة:</span>
                        <select
                          value={aspect || ''}
                          onChange={(e) => setAspect(e.target.value ? parseFloat(e.target.value) : undefined)}
                          className="w-full mt-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                        >
                          <option value="">حر</option>
                          <option value="1">مربع (1:1)</option>
                          <option value="1.3333">4:3</option>
                          <option value="1.7778">16:9</option>
                          <option value="0.75">3:4</option>
                        </select>
                      </label>
                      <ActionButton
                        onClick={handleCrop}
                        disabled={!completedCrop || isProcessing}
                        loading={isProcessing}
                        className="w-full" label={'تطبيق الاقتصاص'}                      >
                        <span className="text-sm">تطبيق الاقتصاص</span>
                      </ActionButton>
                    </div>
                  </div>
                )}

                {activeTool === 'rotate' && (
                  <div>
                    <h4 className="font-medium mb-2">التدوير</h4>
                    <div className="space-y-2">
                      <div className="grid grid-cols-3 gap-2">
                        <ActionButton
                          onClick={() => handleRotate(90)}
                          disabled={isProcessing}
                          size="sm" label={'90°'}                        >
                        </ActionButton>
                        <ActionButton
                          onClick={() => handleRotate(180)}
                          disabled={isProcessing}
                          size="sm" label={'180°'}                        >
                        </ActionButton>
                        <ActionButton
                          onClick={() => handleRotate(270)}
                          disabled={isProcessing}
                          size="sm" label={'270°'}                        >
                        </ActionButton>
                      </div>
                    </div>
                  </div>
                )}

                {activeTool === 'flip' && (
                  <div>
                    <h4 className="font-medium mb-2">القلب</h4>
                    <div className="space-y-2">
                      <ActionButton
                        onClick={() => handleFlip('horizontal')}
                        disabled={isProcessing}
                        className="w-full" label={'قلب أفقي'}                      >
                      </ActionButton>
                      <ActionButton
                        onClick={() => handleFlip('vertical')}
                        disabled={isProcessing}
                        className="w-full" label={'قلب عمودي'}                      >
                      </ActionButton>
                    </div>
                  </div>
                )}

                {(activeTool === 'filter' || activeTool === 'adjust') && (
                  <div>
                    <h4 className="font-medium mb-2">التعديلات والفلاتر</h4>
                    <div className="space-y-3">
                      {Object.entries(filters).map(([key, value]) => (
                        <label key={key} className="block">
                          <span className="text-sm text-gray-600 capitalize">
                            {key === 'brightness' ? 'السطوع' :
                             key === 'contrast' ? 'التباين' :
                             key === 'saturation' ? 'التشبع' :
                             key === 'blur' ? 'الضبابية' :
                             key === 'sepia' ? 'بني داكن' :
                             key === 'hue' ? 'الصبغة' : key}
                          </span>
                          <input
                            type="range"
                            min={key === 'hue' ? -180 : 0}
                            max={key === 'hue' ? 180 : key === 'blur' ? 20 : 200}
                            value={value}
                            onChange={(e) => setFilters(prev => ({
                              ...prev,
                              [key]: parseInt(e.target.value)
                            }))}
                            className="w-full mt-1"
                          />
                          <span className="text-xs text-gray-500">{value}</span>
                        </label>
                      ))}
                    </div>
                  </div>
                )}

                {activeTool === 'text' && (
                  <div>
                    <h4 className="font-medium mb-2">إعدادات النص</h4>
                    <div className="space-y-3">
                      <label className="block">
                        <span className="text-sm text-gray-600">النص:</span>
                        <input
                          type="text"
                          value={textSettings.text}
                          onChange={(e) => setTextSettings(prev => ({ ...prev, text: e.target.value }))}
                          className="w-full mt-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                      </label>
                      
                      <label className="block">
                        <span className="text-sm text-gray-600">حجم الخط:</span>
                        <input
                          type="range"
                          min="12"
                          max="72"
                          value={textSettings.fontSize}
                          onChange={(e) => setTextSettings(prev => ({ ...prev, fontSize: parseInt(e.target.value) }))}
                          className="w-full mt-1"
                        />
                        <span className="text-xs text-gray-500">{textSettings.fontSize}px</span>
                      </label>
                      
                      <label className="block">
                        <span className="text-sm text-gray-600">اللون:</span>
                        <input
                          type="color"
                          value={textSettings.color}
                          onChange={(e) => setTextSettings(prev => ({ ...prev, color: e.target.value }))}
                          className="w-full mt-1 h-10 border border-gray-300 rounded"
                        />
                      </label>
                      
                      <div className="flex gap-2">
                        <label className="flex items-center">
                          <input
                            type="checkbox"
                            checked={textSettings.bold}
                            onChange={(e) => setTextSettings(prev => ({ ...prev, bold: e.target.checked }))}
                            className="ml-2"
                          />
                          <span className="text-sm">عريض</span>
                        </label>
                        <label className="flex items-center">
                          <input
                            type="checkbox"
                            checked={textSettings.italic}
                            onChange={(e) => setTextSettings(prev => ({ ...prev, italic: e.target.checked }))}
                            className="ml-2"
                          />
                          <span className="text-sm">مائل</span>
                        </label>
                      </div>
                      
                      <ActionButton
                        onClick={handleAddText}
                        className="w-full" label={'إضافة النص'}                      >
                        
                      </ActionButton>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* منطقة التحرير الرئيسية - Main Editing Area */}
          <div className="flex-1 flex items-center justify-center bg-gray-100 p-4 overflow-auto">
            {isProcessing && (
              <div className="absolute inset-0 bg-white bg-opacity-75 flex items-center justify-center z-10">
                <LoadingSpinner size="lg" />
              </div>
            )}
            
            <div className="relative max-w-full max-h-full">
              {activeTool === 'crop' ? (
                <ReactCrop
                  crop={crop}
                  onChange={(_, percentCrop) => setCrop(percentCrop)}
                  onComplete={(c) => setCompletedCrop(c)}
                  aspect={aspect}
                >
                  <img
                    ref={imgRef}
                    src={currentImageData}
                    alt="تحرير الصورة"
                    onLoad={onImageLoad}
                    className="max-w-full max-h-full object-contain"
                  />
                </ReactCrop>
              ) : (
                <div className="relative">
                  <img
                    ref={imgRef}
                    src={currentImageData}
                    alt="تحرير الصورة"
                    className="max-w-full max-h-full object-contain"
                    style={{
                      filter: activeTool === 'filter' || activeTool === 'adjust' ? `
                        brightness(${filters.brightness}%)
                        contrast(${filters.contrast}%)
                        saturate(${filters.saturation}%)
                        blur(${filters.blur}px)
                        sepia(${filters.sepia}%)
                        hue-rotate(${filters.hue}deg)
                      ` : 'none'
                    }}
                  />
                  
                  {/* Canvas للرسم والنص - Canvas for drawing and text */}
                  {activeTool === 'text' && (
                    <canvas
                      ref={canvasRef}
                      className="absolute top-0 left-0 w-full h-full"
                      style={{ pointerEvents: 'auto' }}
                    />
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};