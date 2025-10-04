/**
 * مكون إضافة النص والملصقات على الصور - مكون متخصص لإضافة النصوص والملصقات والعلامات المائية
 * Image Text Overlay Component - Specialized component for adding text, stickers and watermarks to images
 */

import React, { useState, useRef, useEffect, useCallback } from 'react';
import ActionButton from '../ui/ActionButton';
import { useUXHelpers } from '../../hooks/useUXHelpers';
import { useNotifications } from '../../stores/appStore';

interface ImageTextOverlayProps {
  /** رابط الصورة - Image URL */
  imageUrl: string;
  /** عرض الصورة - Image width */
  imageWidth?: number;
  /** ارتفاع الصورة - Image height */
  imageHeight?: number;
  /** هل المكون مفتوح - Is component open */
  isOpen: boolean;
  /** دالة الإغلاق - Close callback */
  onClose: () => void;
  /** دالة الحفظ - Save callback */
  onSave: (imageDataUrl: string) => void;
  /** النصوص المحفوظة مسبقاً - Pre-saved texts */
  existingTexts?: TextElement[];
  /** الملصقات المتاحة - Available stickers */
  availableStickers?: string[];
}

/**
 * عنصر النص
 * Text Element
 */
interface TextElement {
  id: string;
  text: string;
  x: number;
  y: number;
  fontSize: number;
  fontFamily: string;
  color: string;
  bold: boolean;
  italic: boolean;
  underline: boolean;
  shadow: boolean;
  strokeWidth: number;
  strokeColor: string;
  backgroundColor: string;
  angle: number;
  opacity: number;
}

/**
 * إعدادات النص
 * Text Settings
 */
interface TextSettings {
  text: string;
  fontSize: number;
  fontFamily: string;
  color: string;
  bold: boolean;
  italic: boolean;
  underline: boolean;
  shadow: boolean;
  strokeWidth: number;
  strokeColor: string;
  backgroundColor: string;
  opacity: number;
}

/**
 * مكون إضافة النص والملصقات على الصور
 * Image Text Overlay Component
 */
export const ImageTextOverlay: React.FC<ImageTextOverlayProps> = ({
  imageUrl,
  imageWidth = 800,
  imageHeight = 600,
  isOpen,
  onClose,
  onSave,
  existingTexts = [],
  availableStickers = []
}) => {
  const { loading, executeWithFeedback } = useUXHelpers();
  const { showSuccess, showError } = useNotifications();
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const textCanvasRef = useRef<HTMLCanvasElement>(null);
  const backgroundImageRef = useRef<HTMLImageElement | null>(null);

  // إعدادات النص الحالية - Current text settings
  const [textSettings, setTextSettings] = useState<TextSettings>({
    text: 'النص الجديد',
    fontSize: 24,
    fontFamily: 'Arial',
    color: '#000000',
    bold: false,
    italic: false,
    underline: false,
    shadow: false,
    strokeWidth: 0,
    strokeColor: '#000000',
    backgroundColor: 'transparent',
    opacity: 1
  });

  // الوضع النشط - Active mode
  const [activeMode, setActiveMode] = useState<'text' | 'sticker' | 'watermark' | 'select'>('text');

  // النص المحدد حالياً - Currently selected text
  const [selectedTextId, setSelectedTextId] = useState<string | null>(null);

  // الخطوط المتاحة - Available fonts
  const availableFonts = [
    'Arial',
    'Times New Roman',
    'Helvetica',
    'Georgia',
    'Verdana',
    'Courier New',
    'Impact',
    'Comic Sans MS',
    'Trebuchet MS',
    'Lucida Console'
  ];

  // النصوص المحفوظة مسبقاً - Pre-saved texts
  const predefinedTexts = [
    'للبيع',
    'للإيجار',
    'جديد',
    'عرض خاص',
    'تخفيضات',
    'محجوز',
    'متاح الآن',
    'اتصل بنا',
    'زيارة مجانية',
    'بدون عمولة'
  ];

  // الملصقات الافتراضية - Default stickers (emoji)
  const defaultStickers = [
    '🏠', '🏡', '🏢', '🏬', '🏭', '🏘️',
    '⭐', '🌟', '✨', '💎', '🔥', '💯',
    '📍', '📌', '🗺️', '🎯', '✅', '❌',
    '💰', '💵', '💴', '💶', '💷', '💳',
    '📞', '📱', '📧', '💬', '📩', '🔔',
    '🎉', '🎊', '🎈', '🎁', '🏆', '🥇'
  ];

  const allStickers = [...defaultStickers, ...availableStickers];

  // تهيئة Canvas - Initialize Canvas
  useEffect(() => {
    if (!isOpen || !canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    canvas.width = imageWidth;
    canvas.height = imageHeight;

    // تحميل الصورة الخلفية - Load background image
    const img = new Image();
    img.crossOrigin = 'anonymous';
    img.onload = () => {
      backgroundImageRef.current = img;
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      ctx.drawImage(img, 0, 0, imageWidth, imageHeight);
    };
    img.src = imageUrl;

  }, [isOpen, imageUrl, imageWidth, imageHeight]);

  // إضافة نص إلى Canvas - Add text to Canvas
  const addTextToCanvas = useCallback((textElement: Partial<TextElement>) => {
    if (!canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const x = textElement.x || 100;
    const y = textElement.y || 100;
    const text = textElement.text || textSettings.text;
    const fontSize = textElement.fontSize || textSettings.fontSize;
    const fontFamily = textElement.fontFamily || textSettings.fontFamily;
    const color = textElement.color || textSettings.color;
    const bold = textElement.bold !== undefined ? textElement.bold : textSettings.bold;

    ctx.font = `${bold ? 'bold' : 'normal'} ${fontSize}px ${fontFamily}`;
    ctx.fillStyle = color;
    ctx.fillText(text, x, y);
  }, [textSettings]);


  // إضافة نص جديد - Add new text
  const handleAddText = useCallback(() => {
    addTextToCanvas({});
    setActiveMode('select');
  }, [addTextToCanvas]);

  // إضافة نص محفوظ مسبقاً - Add predefined text
  const handleAddPredefinedText = useCallback((text: string) => {
    addTextToCanvas({ text });
    setActiveMode('select');
  }, [addTextToCanvas]);

  // إضافة ملصق - Add sticker
  const handleAddSticker = useCallback((sticker: string) => {
    if (!canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    ctx.font = '48px Arial';
    ctx.fillStyle = '#000000';
    ctx.fillText(sticker, 100, 100);
  }, []);

  // إضافة علامة مائية - Add watermark
  const handleAddWatermark = useCallback(() => {
    if (!canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    ctx.font = '16px Arial';
    ctx.fillStyle = 'rgba(255,255,255,0.7)';
    ctx.save();
    ctx.translate(imageWidth - 100, imageHeight - 25);
    ctx.rotate(-15 * Math.PI / 180);
    ctx.fillText('العلامة المائية', 0, 0);
    ctx.restore();
  }, [imageWidth, imageHeight]);

  // تطبيق التغييرات على النص المحدد - Apply changes to selected text
  const applyTextChanges = useCallback(() => {
    // يمكن إضافة منطق إضافي هنا
  }, []);

  // تطبيق التغييرات عند تغيير الإعدادات - Apply changes when settings change
  useEffect(() => {
    applyTextChanges();
  }, [textSettings]);

  // حذف العنصر المحدد - Delete selected object
  const handleDeleteSelected = useCallback(() => {
    // يمكن إضافة منطق حذف النص هنا
    setSelectedTextId(null);
  }, []);

  // حفظ الصورة - Save image
  const handleSave = useCallback(() => {
    if (!canvasRef.current) return;

    const canvas = canvasRef.current;
    const dataURL = canvas.toDataURL('image/png', 1.0);
    
    onSave(dataURL);
    showSuccess('تم حفظ التعديلات بنجاح');
  }, [onSave, showSuccess]);

  // إعادة تعيين جميع التعديلات - Reset all changes
  const handleReset = useCallback(() => {
    if (!canvasRef.current || !backgroundImageRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    
    // إعادة رسم الصورة الخلفية فقط - Redraw only background image
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(backgroundImageRef.current, 0, 0, imageWidth, imageHeight);
  }, [imageWidth, imageHeight]);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg w-full max-w-7xl h-full max-h-[95vh] flex flex-col">
        {/* رأس المحرر - Editor Header */}
        <div className="flex items-center justify-between p-4 border-b">
          <h3 className="text-lg font-semibold">إضافة النص والملصقات</h3>
          <div className="flex items-center gap-2">
            <ActionButton
              variant="secondary"
              onClick={handleReset}
              label="إعادة تعيين"
            >
              إعادة تعيين
            </ActionButton>
            <ActionButton
              variant="primary"
              onClick={handleSave}
              label="حفظ التعديلات"
            >
              حفظ التعديلات
            </ActionButton>
            <ActionButton
              variant="secondary"
              onClick={onClose}
              label="إغلاق"
            >
              ✕
            </ActionButton>
          </div>
        </div>

        <div className="flex flex-1 overflow-hidden">
          {/* شريط الأدوات الجانبي - Sidebar Tools */}
          <div className="w-80 border-l p-4 overflow-y-auto">
            <div className="space-y-6">
              {/* أوضاع التحرير - Editing Modes */}
              <div>
                <h4 className="font-medium mb-3">الأدوات</h4>
                <div className="grid grid-cols-2 gap-2">
                  {[
                    { id: 'text', label: 'نص', icon: '📝' },
                    { id: 'sticker', label: 'ملصق', icon: '🎭' },
                    { id: 'watermark', label: 'علامة مائية', icon: '💧' },
                    { id: 'select', label: 'تحديد', icon: '👆' }
                  ].map(mode => (
                    <button
                      key={mode.id}
                      onClick={() => setActiveMode(mode.id as any)}
                      className={`
                        p-3 rounded-lg border transition-all text-sm
                        ${activeMode === mode.id 
                          ? 'border-blue-500 bg-blue-50 text-blue-700' 
                          : 'border-gray-200 hover:border-gray-300'
                        }
                      `}
                    >
                      <div className="text-lg mb-1">{mode.icon}</div>
                      {mode.label}
                    </button>
                  ))}
                </div>
              </div>

              {/* أدوات النص - Text Tools */}
              {activeMode === 'text' && (
                <div className="space-y-4">
                  <div>
                    <h4 className="font-medium mb-3">إضافة نص</h4>
                    <ActionButton
                      onClick={handleAddText}
                      className="w-full mb-3"
                      label="إضافة نص جديد"
                    >
                      إضافة نص جديد
                    </ActionButton>
                    
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        النصوص المحفوظة مسبقاً
                      </label>
                      <div className="grid grid-cols-2 gap-1">
                        {predefinedTexts.map(text => (
                          <button
                            key={text}
                            onClick={() => handleAddPredefinedText(text)}
                            className="px-2 py-1 text-xs border border-gray-300 rounded hover:bg-gray-50 transition-colors"
                          >
                            {text}
                          </button>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              )}

              {/* أدوات الملصقات - Sticker Tools */}
              {activeMode === 'sticker' && (
                <div>
                  <h4 className="font-medium mb-3">الملصقات</h4>
                  <div className="grid grid-cols-6 gap-2">
                    {allStickers.map((sticker, index) => (
                      <button
                        key={index}
                        onClick={() => handleAddSticker(sticker)}
                        className="w-10 h-10 text-xl border border-gray-300 rounded hover:bg-gray-50 transition-colors flex items-center justify-center"
                      >
                        {sticker}
                      </button>
                    ))}
                  </div>
                </div>
              )}

              {/* أدوات العلامة المائية - Watermark Tools */}
              {activeMode === 'watermark' && (
                <div>
                  <h4 className="font-medium mb-3">العلامة المائية</h4>
                  <ActionButton
                    onClick={handleAddWatermark}
                    className="w-full"
                    label="إضافة علامة مائية"
                  >
                    إضافة علامة مائية
                  </ActionButton>
                </div>
              )}

              {/* إعدادات النص المحدد - Selected Text Settings */}
              {selectedTextId && (
                <div className="border-t pt-4">
                  <h4 className="font-medium mb-3">إعدادات النص</h4>
                  <div className="space-y-3">
                    {/* النص - Text */}
                    <label className="block">
                      <span className="text-sm text-gray-600">النص:</span>
                      <input
                        type="text"
                        value={textSettings.text}
                        onChange={(e) => setTextSettings(prev => ({ ...prev, text: e.target.value }))}
                        className="w-full mt-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                      />
                    </label>

                    {/* الخط - Font */}
                    <label className="block">
                      <span className="text-sm text-gray-600">الخط:</span>
                      <select
                        value={textSettings.fontFamily}
                        onChange={(e) => setTextSettings(prev => ({ ...prev, fontFamily: e.target.value }))}
                        className="w-full mt-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                      >
                        {availableFonts.map(font => (
                          <option key={font} value={font}>{font}</option>
                        ))}
                      </select>
                    </label>

                    {/* حجم الخط - Font Size */}
                    <label className="block">
                      <span className="text-sm text-gray-600">حجم الخط:</span>
                      <input
                        type="range"
                        min="8"
                        max="120"
                        value={textSettings.fontSize}
                        onChange={(e) => setTextSettings(prev => ({ ...prev, fontSize: parseInt(e.target.value) }))}
                        className="w-full mt-1"
                      />
                      <span className="text-xs text-gray-500">{textSettings.fontSize}px</span>
                    </label>

                    {/* لون النص - Text Color */}
                    <label className="block">
                      <span className="text-sm text-gray-600">لون النص:</span>
                      <input
                        type="color"
                        value={textSettings.color}
                        onChange={(e) => setTextSettings(prev => ({ ...prev, color: e.target.value }))}
                        className="w-full mt-1 h-10 border border-gray-300 rounded"
                      />
                    </label>

                    {/* خيارات التنسيق - Formatting Options */}
                    <div>
                      <span className="text-sm text-gray-600 block mb-2">التنسيق:</span>
                      <div className="grid grid-cols-2 gap-2">
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
                        <label className="flex items-center">
                          <input
                            type="checkbox"
                            checked={textSettings.underline}
                            onChange={(e) => setTextSettings(prev => ({ ...prev, underline: e.target.checked }))}
                            className="ml-2"
                          />
                          <span className="text-sm">تحته خط</span>
                        </label>
                        <label className="flex items-center">
                          <input
                            type="checkbox"
                            checked={textSettings.shadow}
                            onChange={(e) => setTextSettings(prev => ({ ...prev, shadow: e.target.checked }))}
                            className="ml-2"
                          />
                          <span className="text-sm">ظل</span>
                        </label>
                      </div>
                    </div>

                    {/* الشفافية - Opacity */}
                    <label className="block">
                      <span className="text-sm text-gray-600">الشفافية:</span>
                      <input
                        type="range"
                        min="0"
                        max="1"
                        step="0.1"
                        value={textSettings.opacity}
                        onChange={(e) => setTextSettings(prev => ({ ...prev, opacity: parseFloat(e.target.value) }))}
                        className="w-full mt-1"
                      />
                      <span className="text-xs text-gray-500">{Math.round(textSettings.opacity * 100)}%</span>
                    </label>

                    {/* حذف النص - Delete Text */}
                    <ActionButton
                      variant="danger"
                      onClick={handleDeleteSelected}
                      className="w-full"
                      label="حذف النص المحدد"
                    >
                      حذف النص المحدد
                    </ActionButton>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* منطقة التحرير الرئيسية - Main Editing Area */}
          <div className="flex-1 flex items-center justify-center bg-gray-100 p-4 overflow-auto">
            <div className="relative">
              <canvas
                ref={canvasRef}
                className="border border-gray-300 rounded shadow-lg bg-white"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};