import React, { useState, useRef, useEffect } from 'react';

interface TagInputProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  label?: string;
  disabled?: boolean;
  required?: boolean;
  error?: string;
  maxTags?: number;
  className?: string;
  variant?: 'default' | 'modern' | 'minimal';
  size?: 'sm' | 'md' | 'lg';
  direction?: 'rtl' | 'ltr';
  separator?: string;
  allowDuplicates?: boolean;
  suggestions?: string[];
}

const TagInput: React.FC<TagInputProps> = ({
  value = '',
  onChange,
  placeholder = 'أدخل الميزات واضغط Enter أو الفاصلة للإضافة...',
  label,
  disabled = false,
  required = false,
  error,
  maxTags = 20,
  className = '',
  variant = 'modern',
  size = 'md',
  direction = 'rtl',
  separator = ',',
  allowDuplicates = false,
  suggestions = []
}) => {
  const [inputValue, setInputValue] = useState('');
  const [focused, setFocused] = useState(false);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  // تحويل القيمة النصية إلى مصفوفة من العلامات
  const tags = value ? value.split(separator).map(tag => tag.trim()).filter(tag => tag.length > 0) : [];

  // تصفية الاقتراحات
  const filteredSuggestions = suggestions.filter(suggestion => 
    suggestion.toLowerCase().includes(inputValue.toLowerCase()) &&
    (allowDuplicates || !tags.includes(suggestion))
  );

  // أحجام المكونات
  const sizeStyles = {
    sm: {
      container: 'min-h-[32px] text-sm',
      input: 'px-2 py-1 text-sm',
      tag: 'px-2 py-0.5 text-xs',
      icon: 'w-3 h-3'
    },
    md: {
      container: 'min-h-[40px] text-base',
      input: 'px-3 py-2 text-base',
      tag: 'px-2.5 py-1 text-sm',
      icon: 'w-4 h-4'
    },
    lg: {
      container: 'min-h-[48px] text-lg',
      input: 'px-4 py-3 text-lg',
      tag: 'px-3 py-1.5 text-base',
      icon: 'w-5 h-5'
    }
  };

  // أنماط التصميم
  const variantStyles = {
    default: {
      container: 'border-gray-300 bg-white shadow-sm rounded-lg',
      focused: 'ring-2 ring-blue-500 border-blue-500',
      error: 'border-red-500 ring-2 ring-red-200',
      tag: 'bg-blue-100 text-blue-800 border border-blue-200',
      tagHover: 'hover:bg-blue-200'
    },
    modern: {
      container: 'border-gray-200 bg-white shadow-lg rounded-xl',
      focused: 'ring-2 ring-blue-500/20 border-blue-400 shadow-xl',
      error: 'border-red-400 ring-2 ring-red-200 shadow-red-100',
      tag: 'bg-gradient-to-r from-blue-50 to-indigo-50 text-blue-700 border border-blue-200 rounded-full',
      tagHover: 'hover:from-blue-100 hover:to-indigo-100'
    },
    minimal: {
      container: 'border-gray-200 bg-transparent rounded-md',
      focused: 'border-blue-400',
      error: 'border-red-400',
      tag: 'bg-gray-100 text-gray-700 border border-gray-200 rounded-md',
      tagHover: 'hover:bg-gray-200'
    }
  };

  const currentSize = sizeStyles[size];
  const currentVariant = variantStyles[variant];

  // إضافة علامة جديدة
  const addTag = (newTag: string) => {
    const trimmedTag = newTag.trim();
    if (!trimmedTag) return;

    // التحقق من عدم تكرار العلامات
    if (!allowDuplicates && tags.includes(trimmedTag)) {
      setInputValue('');
      return;
    }

    // التحقق من الحد الأقصى للعلامات
    if (tags.length >= maxTags) {
      setInputValue('');
      return;
    }

    const newTags = [...tags, trimmedTag];
    onChange(newTags.join(separator));
    setInputValue('');
    setShowSuggestions(false);
  };

  // حذف علامة
  const removeTag = (indexToRemove: number) => {
    const newTags = tags.filter((_, index) => index !== indexToRemove);
    onChange(newTags.join(separator));
  };

  // التعامل مع ضغط المفاتيح
  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' || e.key === separator) {
      e.preventDefault();
      addTag(inputValue);
    } else if (e.key === 'Backspace' && !inputValue && tags.length > 0) {
      // حذف آخر علامة عند الضغط على Backspace
      removeTag(tags.length - 1);
    } else if (e.key === 'Escape') {
      setShowSuggestions(false);
      inputRef.current?.blur();
    }
  };

  // التعامل مع تغيير النص
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    
    // التحقق من وجود فاصلة في النص
    if (newValue.includes(separator)) {
      const parts = newValue.split(separator);
      const tagsToAdd = parts.slice(0, -1);
      
      // إضافة جميع العلامات الجديدة
      tagsToAdd.forEach(tag => addTag(tag));
      
      // الاحتفاظ بالجزء الأخير كنص إدخال
      setInputValue(parts[parts.length - 1]);
    } else {
      setInputValue(newValue);
      setShowSuggestions(newValue.length > 0 && filteredSuggestions.length > 0);
    }
  };

  // التعامل مع التركيز
  const handleFocus = () => {
    setFocused(true);
    if (inputValue && filteredSuggestions.length > 0) {
      setShowSuggestions(true);
    }
  };

  const handleBlur = () => {
    setFocused(false);
    // تأخير إخفاء الاقتراحات للسماح بالنقر عليها
    setTimeout(() => {
      setShowSuggestions(false);
      // إضافة النص المتبقي كعلامة عند فقدان التركيز
      if (inputValue.trim()) {
        addTag(inputValue);
      }
    }, 200);
  };

  // النقر على الحاوية لتركيز الإدخال
  const handleContainerClick = () => {
    inputRef.current?.focus();
  };

  // اختيار اقتراح
  const selectSuggestion = (suggestion: string) => {
    addTag(suggestion);
    inputRef.current?.focus();
  };

  // تحديد أنماط الحاوية
  const getContainerStyles = () => {
    let styles = `
      ${currentVariant.container} 
      ${currentSize.container}
      transition-all duration-200 ease-in-out
      cursor-text
    `;
    
    if (error) {
      styles += ` ${currentVariant.error}`;
    } else if (focused) {
      styles += ` ${currentVariant.focused}`;
    }
    
    if (disabled) {
      styles += ' opacity-50 cursor-not-allowed bg-gray-50';
    }
    
    return styles;
  };

  return (
    <div className={`relative space-y-2 ${className}`}>
      {/* التسمية */}
      {label && (
        <label className="block text-sm font-medium text-gray-700">
          {label}
          {required && <span className="text-red-500 mr-1">*</span>}
        </label>
      )}

      {/* الحاوية الرئيسية */}
      <div className="relative">
        <div
          className={`border flex flex-wrap items-center gap-1 p-2 ${getContainerStyles()}`}
          onClick={handleContainerClick}
          dir={direction}
        >
          {/* عرض العلامات */}
          {tags.map((tag, index) => (
            <span
              key={index}
              className={`
                inline-flex items-center gap-1 
                ${currentVariant.tag} ${currentVariant.tagHover}
                ${currentSize.tag}
                transition-all duration-200
                transform hover:scale-105 animate-in slide-in-from-left-2
              `}
            >
              <span>{tag}</span>
              {!disabled && (
                <button
                  type="button"
                  onClick={(e) => {
                    e.stopPropagation();
                    removeTag(index);
                  }}
                  className={`
                    ${currentSize.icon} 
                    hover:text-red-600 transition-colors duration-200
                    rounded-full hover:bg-red-100 p-0.5
                  `}
                >
                  <svg fill="currentColor" viewBox="0 0 20 20">
                    <path
                      fillRule="evenodd"
                      d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                      clipRule="evenodd"
                    />
                  </svg>
                </button>
              )}
            </span>
          ))}

          {/* حقل الإدخال */}
          <input
            ref={inputRef}
            type="text"
            value={inputValue}
            onChange={handleInputChange}
            onKeyDown={handleKeyDown}
            onFocus={handleFocus}
            onBlur={handleBlur}
            disabled={disabled}
            placeholder={tags.length === 0 ? placeholder : 'إضافة ميزة...'}
            className={`
              flex-1 min-w-[120px] bg-transparent border-none outline-none
              ${currentSize.input.replace('px-3 py-2', 'p-0')}
              ${disabled ? 'cursor-not-allowed' : ''}
              placeholder-gray-400
            `}
            dir={direction}
          />
        </div>

        {/* قائمة الاقتراحات */}
        {showSuggestions && filteredSuggestions.length > 0 && (
          <div className="absolute z-50 w-full mt-1 bg-white border border-gray-200 rounded-lg shadow-lg max-h-40 overflow-y-auto">
            {filteredSuggestions.map((suggestion, index) => (
              <button
                key={index}
                type="button"
                onClick={() => selectSuggestion(suggestion)}
                className="w-full px-3 py-2 text-right hover:bg-gray-50 focus:bg-gray-50 focus:outline-none border-b border-gray-100 last:border-b-0"
              >
                <span className="text-sm text-gray-700">{suggestion}</span>
              </button>
            ))}
          </div>
        )}
      </div>

      {/* معلومات إضافية */}
      <div className="space-y-1">
        {/* عداد العلامات */}
        {tags.length > 0 && (
          <div className="flex items-center justify-between text-xs text-gray-500">
            <span>
              {tags.length} من {maxTags} ميزة
            </span>
            {tags.length >= maxTags && (
              <span className="text-orange-600">تم الوصول للحد الأقصى</span>
            )}
          </div>
        )}

        {/* رسالة الخطأ */}
        {error && (
          <div className="flex items-center gap-2 p-2 bg-red-50 border border-red-200 rounded-lg">
            <span className="text-red-500">⚠️</span>
            <span className="text-sm text-red-600">{error}</span>
          </div>
        )}

        {/* نصائح الاستخدام */}
        {focused && !error && (
          <div className="p-2 bg-blue-50 border border-blue-200 rounded-lg">
            <div className="flex items-start gap-2">
              <span className="text-blue-500 mt-0.5">💡</span>
              <div className="text-xs text-blue-800">
                <div className="font-medium mb-1">نصائح:</div>
                <ul className="space-y-0.5">
                  <li>• اضغط Enter أو الفاصلة ({separator}) لإضافة ميزة</li>
                  <li>• اضغط Backspace لحذف آخر ميزة</li>
                  <li>• انقر على ✕ لحذف ميزة محددة</li>
                </ul>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default TagInput;