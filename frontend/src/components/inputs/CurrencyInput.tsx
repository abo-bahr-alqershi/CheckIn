import React, { useState, useEffect } from 'react';
import { useCurrencies } from '../../hooks/useCurrencies';

interface CurrencyInputProps {
  value?: number;
  currency?: string;
  onValueChange: (amount: number, currency: string) => void;
  className?: string;
  disabled?: boolean;
  placeholder?: string;
  required?: boolean;
  min?: number;
  max?: number;
  step?: number;
  showSymbol?: boolean;
  supportedCurrencies?: string[];
  direction?: 'rtl' | 'ltr';
  label?: string;
  error?: string;
  variant?: 'default' | 'modern' | 'minimal';
  size?: 'sm' | 'md' | 'lg';
}

const CurrencyInput: React.FC<CurrencyInputProps> = ({
  value = 0,
  currency = 'SAR',
  onValueChange,
  className = '',
  disabled = false,
  placeholder = '0.00',
  required = false,
  min = 0,
  max,
  step = 0.01,
  showSymbol = true,
  supportedCurrencies = ['SAR', 'USD', 'EUR', 'AED'],
  direction = 'rtl',
  label,
  error,
  variant = 'modern',
  size = 'md'
}) => {
  const [amount, setAmount] = useState(value);
  const [selectedCurrency, setSelectedCurrency] = useState(currency);
  const [focused, setFocused] = useState(false);

  // تحديث القيم عند تغيير المعاملات الخارجية
  useEffect(() => {
    setAmount(value);
  }, [value]);

  useEffect(() => {
    setSelectedCurrency(currency);
  }, [currency]);

  // معلومات العملات
  const currencyInfo = {
    YER: { symbol: 'ر.ي', name: 'ريال يمني', decimals: 2, flag: '🇾🇪' },
    SAR: { symbol: 'ر.س', name: 'ريال سعودي', decimals: 2, flag: '🇸🇦' },
    USD: { symbol: '$', name: 'دولار أمريكي', decimals: 2, flag: '🇺🇸' },
    EUR: { symbol: '€', name: 'يورو', decimals: 2, flag: '🇪🇺' },
    AED: { symbol: 'د.إ', name: 'درهم إماراتي', decimals: 2, flag: '🇦🇪' },
    QAR: { symbol: 'ر.ق', name: 'ريال قطري', decimals: 2, flag: '🇶🇦' },
    KWD: { symbol: 'د.ك', name: 'دينار كويتي', decimals: 3, flag: '🇰🇼' },
    BHD: { symbol: 'د.ب', name: 'دينار بحريني', decimals: 3, flag: '🇧🇭' },
    OMR: { symbol: 'ر.ع', name: 'ريال عماني', decimals: 3, flag: '🇴🇲' }
  };

  const getCurrentCurrencyInfo = () => currencyInfo[selectedCurrency] || currencyInfo.SAR;

  // أحجام المكونات
  const sizeStyles = {
    sm: {
      input: 'px-3 py-1.5 text-sm',
      button: 'px-1.5 py-0.5 text-xs',
      container: 'min-h-[36px]',
      icon: 'text-sm'
    },
    md: {
      input: 'px-4 py-2.5 text-base',
      button: 'px-2 py-1 text-sm',
      container: 'min-h-[42px]',
      icon: 'text-base'
    },
    lg: {
      input: 'px-5 py-3 text-lg',
      button: 'px-2.5 py-1.5 text-base',
      container: 'min-h-[48px]',
      icon: 'text-lg'
    }
  };

  // أنماط التصميم
  const variantStyles = {
    default: {
      container: 'border-gray-300 bg-white shadow-sm',
      focused: 'ring-2 ring-blue-500 border-blue-500',
      error: 'border-red-500 ring-2 ring-red-200',
      disabled: 'bg-gray-50 border-gray-200'
    },
    modern: {
      container: 'border-gray-200 bg-white shadow-lg rounded-xl',
      focused: 'ring-2 ring-blue-500/20 border-blue-400 shadow-xl',
      error: 'border-red-400 ring-2 ring-red-200 shadow-red-100',
      disabled: 'bg-gray-50 border-gray-100'
    },
    minimal: {
      container: 'border-gray-200 bg-transparent',
      focused: 'border-blue-400',
      error: 'border-red-400',
      disabled: 'bg-gray-50/50 border-gray-100'
    }
  };

  const currentSize = sizeStyles[size];
  const currentVariant = variantStyles[variant];

  // تنسيق المبلغ للعرض
  const formatAmount = (num: number) => {
    const info = getCurrentCurrencyInfo();
    return new Intl.NumberFormat('ar-SA', {
      minimumFractionDigits: focused ? 0 : info.decimals,
      maximumFractionDigits: info.decimals
    }).format(num);
  };

  // تنسيق المبلغ مع العملة
  const formatCurrency = (num: number, curr: string) => {
    const info = currencyInfo[curr] || currencyInfo.YER;
    const formatted = new Intl.NumberFormat('ar-SA', {
      style: 'currency',
      currency: curr,
      minimumFractionDigits: info.decimals,
      maximumFractionDigits: info.decimals
    }).format(num);
    
    return formatted;
  };

  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const inputValue = e.target.value;
    
    // السماح بالأرقام والفاصلة العشرية فقط
    if (inputValue === '' || /^\d*\.?\d*$/.test(inputValue)) {
      const numValue = inputValue === '' ? 0 : parseFloat(inputValue);
      
      if (!isNaN(numValue)) {
        // التحقق من الحد الأدنى والأقصى
        if (min !== undefined && numValue < min) return;
        if (max !== undefined && numValue > max) return;
        
        setAmount(numValue);
        onValueChange(numValue, selectedCurrency);
      }
    }
  };

  const handleCurrencyChange = (newCurrency: string) => {
    setSelectedCurrency(newCurrency);
    onValueChange(amount, newCurrency);
  };

  const incrementAmount = () => {
    const newAmount = amount + step;
    if (max === undefined || newAmount <= max) {
      setAmount(newAmount);
      onValueChange(newAmount, selectedCurrency);
    }
  };

  const decrementAmount = () => {
    const newAmount = amount - step;
    if (newAmount >= min) {
      setAmount(newAmount);
      onValueChange(newAmount, selectedCurrency);
    }
  };

  const handleFocus = () => {
    setFocused(true);
  };

  const handleBlur = () => {
    setFocused(false);
    // تنسيق المبلغ عند فقدان التركيز
    const info = getCurrentCurrencyInfo();
    const rounded = Math.round(amount * Math.pow(10, info.decimals)) / Math.pow(10, info.decimals);
    if (rounded !== amount) {
      setAmount(rounded);
      onValueChange(rounded, selectedCurrency);
    }
  };

  // Fetch currencies and determine records
  const { currencies, loading: currenciesLoading } = useCurrencies();
  const currentCurrencyRecord = currencies.find(c => c.code === selectedCurrency);
  const defaultCurrencyRecord = currencies.find(c => c.isDefault);

  const getContainerStyles = () => {
    let styles = `
      ${currentVariant.container} 
      ${currentSize.container}
      transition-all duration-200 ease-in-out
      ${variant === 'modern' ? 'rounded-xl' : 'rounded-lg'}
    `;
    
    if (error) {
      styles += ` ${currentVariant.error}`;
    } else if (focused) {
      styles += ` ${currentVariant.focused}`;
    } else if (disabled) {
      styles += ` ${currentVariant.disabled}`;
    }
    
    return styles;
  };

  return (
    <div className={`relative space-y-2 ${className}`}>
      {/* التسمية */}
      {label && (
        <label className="block text-sm font-medium text-gray-700 mb-1">
          {label}
          {required && <span className="text-red-500 mr-1">*</span>}
        </label>
      )}

      <div className={`flex items-stretch border ${getContainerStyles()}`}>
        {/* اختيار العملة مع العلم */}
        <div className="relative">
          <select 
            value={selectedCurrency}
            onChange={(e) => handleCurrencyChange(e.target.value)}
            disabled={disabled}
            className={`
              ${currentSize.input} 
              bg-gray-50 border-l border-gray-200 focus:outline-none
              ${variant === 'modern' ? 'rounded-r-xl' : 'rounded-r-lg'}
              cursor-pointer appearance-none pr-8 font-medium
              ${disabled ? 'opacity-50 cursor-not-allowed' : 'hover:bg-gray-100'}
              transition-colors duration-200
            `}
          >
            {supportedCurrencies.map((curr) => {
              const info = currencyInfo[curr];
              return (
                <option key={curr} value={curr}>
                  {info?.flag} {curr}
                </option>
              );
            })}
          </select>
          
          {/* سهم القائمة المنسدلة */}
          <div className="absolute left-2 top-1/2 transform -translate-y-1/2 pointer-events-none">
            <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
            </svg>
          </div>
        </div>

        {/* حقل إدخال المبلغ */}
        <div className="flex-1 relative">
          <input
            type="text"
            value={focused ? amount.toString() : formatAmount(amount)}
            onChange={handleAmountChange}
            onFocus={handleFocus}
            onBlur={handleBlur}
            disabled={disabled}
            placeholder={placeholder}
            className={`
              w-full ${currentSize.input} bg-transparent focus:outline-none font-mono
              ${direction === 'rtl' ? 'text-right pr-12' : 'text-left pl-12'}
              ${disabled ? 'cursor-not-allowed opacity-50' : ''}
              placeholder-gray-400
            `}
            dir={direction}
          />
          
          {/* رمز العملة */}
          {showSymbol && (
            <div className={`
              absolute top-1/2 transform -translate-y-1/2 pointer-events-none 
              text-gray-500 font-semibold ${currentSize.icon}
              ${direction === 'rtl' ? 'left-3' : 'right-3'}
            `}>
              {getCurrentCurrencyInfo().symbol}
            </div>
          )}
        </div>

        {/* أزرار الزيادة والنقصان */}
        <div className="flex flex-col border-l border-gray-200">
          <button
            type="button"
            onClick={incrementAmount}
            disabled={disabled || (max !== undefined && amount >= max)}
            className={`
              ${currentSize.button} flex-1 hover:bg-blue-50 hover:text-blue-600
              disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-transparent
              text-gray-400 transition-all duration-200 font-semibold
              ${variant === 'modern' ? 'rounded-tl-xl' : 'rounded-tl-lg'}
              border-b border-gray-200
            `}
          >
            <svg className="w-3 h-3 mx-auto" fill="currentColor" viewBox="0 0 24 24">
              <path d="M7 14l5-5 5 5z"/>
            </svg>
          </button>
          <button
            type="button"
            onClick={decrementAmount}
            disabled={disabled || amount <= min}
            className={`
              ${currentSize.button} flex-1 hover:bg-blue-50 hover:text-blue-600
              disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-transparent
              text-gray-400 transition-all duration-200 font-semibold
              ${variant === 'modern' ? 'rounded-bl-xl' : 'rounded-bl-lg'}
            `}
          >
            <svg className="w-3 h-3 mx-auto" fill="currentColor" viewBox="0 0 24 24">
              <path d="M7 10l5 5 5-5z"/>
            </svg>
          </button>
        </div>
      </div>

      {/* المعلومات الإضافية */}
      <div className="space-y-1">
        {/* عرض المبلغ المنسق */}
        {!focused && amount > 0 && variant !== 'minimal' && (
          <div className="flex items-center justify-center gap-2 p-2 bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg border border-blue-100">
            <span className="text-2xl">{getCurrentCurrencyInfo().flag}</span>
            <div className="text-center">
              <div className="text-lg font-semibold text-gray-800">
                {formatCurrency(amount, selectedCurrency)}
              </div>
              <div className="text-xs text-gray-500">
                {currentCurrencyRecord?.arabicName || getCurrentCurrencyInfo().name}
              </div>
            </div>
          </div>
        )}

        {/* سعر الصرف */}
        {!currenciesLoading && currentCurrencyRecord && !currentCurrencyRecord.isDefault && defaultCurrencyRecord && (
          <div className="flex items-center justify-center gap-2 p-2 bg-yellow-50 rounded-lg border border-yellow-200">
            <span className="text-yellow-600">💱</span>
            <div className="text-xs text-yellow-800">
              {`1 ${currentCurrencyRecord.code} = ${currentCurrencyRecord.exchangeRate} ${defaultCurrencyRecord.code}`}
            </div>
          </div>
        )}

        {/* الحد الأدنى والأقصى */}
        {(min > 0 || max !== undefined) && (
          <div className="flex items-center justify-center gap-2 p-1.5 bg-gray-50 rounded text-xs text-gray-600">
            <span>📊</span>
            <span>
              {min > 0 && `الحد الأدنى: ${formatCurrency(min, selectedCurrency)}`}
              {min > 0 && max !== undefined && ' • '}
              {max !== undefined && `الحد الأقصى: ${formatCurrency(max, selectedCurrency)}`}
            </span>
          </div>
        )}

        {/* رسالة الخطأ */}
        {error && (
          <div className="flex items-center gap-2 p-2 bg-red-50 border border-red-200 rounded-lg">
            <span className="text-red-500">⚠️</span>
            <span className="text-sm text-red-600">{error}</span>
          </div>
        )}

        {/* تحذير التحقق */}
        {required && amount === 0 && !error && (
          <div className="flex items-center gap-2 p-2 bg-orange-50 border border-orange-200 rounded-lg">
            <span className="text-orange-500">📝</span>
            <span className="text-sm text-orange-600">هذا الحقل مطلوب</span>
          </div>
        )}

        {/* نصائح للاستخدام */}
        {focused && variant !== 'minimal' && (
          <div className="p-3 bg-gradient-to-r from-blue-50 to-purple-50 border border-blue-200 rounded-lg">
            <div className="flex items-start gap-2">
              <span className="text-blue-500 mt-0.5">💡</span>
              <div className="text-sm text-blue-800">
                <div className="font-medium mb-1">نصائح الاستخدام:</div>
                <ul className="space-y-1 text-xs">
                  <li>• استخدم الأسهم ▲▼ للزيادة والنقصان</li>
                  <li>• اكتب المبلغ مباشرة</li>
                  <li>• غير العملة من القائمة المنسدلة</li>
                </ul>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default CurrencyInput;