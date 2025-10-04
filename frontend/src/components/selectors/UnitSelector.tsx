import React, { useState, useEffect, useRef } from 'react';
import { useAdminUnits } from '../../hooks/useAdminUnits';
import type { UnitDto } from '../../types/unit.types';

interface UnitSelectorProps {
  value?: string;
  onChange: (unitId: string, unit?: UnitDto) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  propertyId?: string;
  unitTypeId?: string;
  isAvailable?: boolean;
  required?: boolean;
}

const UnitSelector: React.FC<UnitSelectorProps> = ({
  value = '',
  onChange,
  placeholder = 'اختر الوحدة',
  className = '',
  disabled = false,
  propertyId,
  unitTypeId,
  isAvailable,
  required = false
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedUnit, setSelectedUnit] = useState<UnitDto | null>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // بناء معايير البحث
  const searchParams = {
    pageNumber: 1,
    pageSize: 50,
    nameContains: searchTerm || undefined,
    propertyId: propertyId || undefined,
    unitTypeId: unitTypeId || undefined,
    isAvailable: isAvailable || undefined,
  };

  const { unitsData, isLoading } = useAdminUnits(searchParams);

  // العثور على الوحدة المحددة
  useEffect(() => {
    if (value && unitsData?.items) {
      const unit = unitsData.items.find(u => u.id === value);
      setSelectedUnit(unit || null);
    } else {
      setSelectedUnit(null);
    }
  }, [value, unitsData]);

  // إغلاق القائمة عند النقر خارجها
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleUnitSelect = (unit: UnitDto) => {
    setSelectedUnit(unit);
    onChange(unit.id, unit);
    setIsOpen(false);
    setSearchTerm('');
  };

  const handleClear = () => {
    setSelectedUnit(null);
    onChange('');
    setSearchTerm('');
  };

  const getPricingMethodLabel = (method: string) => {
    const methodLabels = {
      'Hourly': 'بالساعة',
      'Daily': 'يومي',
      'Weekly': 'أسبوعي',
      'Monthly': 'شهري'
    };
    return methodLabels[method] || method;
  };

  const formatPrice = (price: any) => {
    if (!price || typeof price !== 'object') return '';
    return `${price.amount} ${price.currency}`;
  };

  return (
    <div className={`relative ${className}`} ref={dropdownRef}>
      {/* زر الاختيار */}
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        disabled={disabled}
        className={`
          w-full flex items-center justify-between px-3 py-2 border rounded-md shadow-sm
          bg-white text-right focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
          ${disabled ? 'bg-gray-50 text-gray-400 cursor-not-allowed' : 'hover:border-gray-400'}
          ${!selectedUnit && required ? 'border-red-300' : 'border-gray-300'}
        `}
      >
        <div className="flex items-center space-x-3 space-x-reverse flex-1 min-w-0">
          {selectedUnit ? (
            <div className="flex-1 min-w-0">
              <div className="flex items-center space-x-2 space-x-reverse">
                <span className="font-medium text-gray-900 truncate">
                  {selectedUnit.name}
                </span>
                <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
                  selectedUnit.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                }`}>
                  {selectedUnit.isAvailable ? 'متاحة' : 'غير متاحة'}
                </span>
              </div>
              <div className="flex items-center space-x-4 space-x-reverse mt-1">
                <p className="text-sm text-gray-500 truncate">{selectedUnit.propertyName}</p>
                <p className="text-xs text-gray-400">{formatPrice(selectedUnit.basePrice)}</p>
              </div>
            </div>
          ) : (
            <span className="text-gray-500">{placeholder}</span>
          )}
        </div>
        
        <div className="flex items-center space-x-1 space-x-reverse">
          {selectedUnit && !disabled && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                handleClear();
              }}
              className="p-1 hover:bg-gray-100 rounded"
            >
              <span className="text-gray-400 hover:text-gray-600">×</span>
            </button>
          )}
          <span className="text-gray-400">
            {isOpen ? '▲' : '▼'}
          </span>
        </div>
      </button>

      {/* القائمة المنسدلة */}
      {isOpen && (
        <div className="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg">
          {/* شريط البحث */}
          <div className="p-3 border-b border-gray-200">
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="البحث في الوحدات..."
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
              autoFocus
            />
          </div>

          {/* قائمة الوحدات */}
          <div className="max-h-60 overflow-y-auto">
            {isLoading ? (
              <div className="p-4 text-center text-gray-500">
                <div className="animate-spin inline-block w-6 h-6 border-2 border-gray-300 border-t-blue-600 rounded-full"></div>
                <p className="mt-2">جارٍ التحميل...</p>
              </div>
            ) : unitsData?.items && unitsData.items.length > 0 ? (
              unitsData.items.map((unit) => (
                <button
                  key={unit.id}
                  type="button"
                  onClick={() => handleUnitSelect(unit)}
                  className="w-full flex items-center px-3 py-3 hover:bg-gray-50 text-right border-b border-gray-100 last:border-b-0"
                >
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center space-x-2 space-x-reverse">
                      <span className="font-medium text-gray-900 truncate">
                        {unit.name}
                      </span>
                      <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
                        unit.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                      }`}>
                        {unit.isAvailable ? 'متاحة' : 'غير متاحة'}
                      </span>
                    </div>
                    <div className="flex items-center space-x-4 space-x-reverse mt-1">
                      <p className="text-sm text-gray-500 truncate">📍 {unit.propertyName}</p>
                      <p className="text-xs text-gray-400">🏠 {unit.unitTypeName}</p>
                    </div>
                    <div className="flex items-center space-x-4 space-x-reverse mt-1">
                      <p className="text-xs text-gray-400">{getPricingMethodLabel(unit.pricingMethod)}</p>
                    </div>
                  </div>
                  <div className="flex-shrink-0 text-left">
                    <p className="text-sm font-medium text-gray-900">{formatPrice(unit.basePrice)}</p>
                    <p className="text-xs text-gray-500">السعر الأساسي</p>
                  </div>
                </button>
              ))
            ) : (
              <div className="p-4 text-center text-gray-500">
                {searchTerm ? `لا توجد نتائج للبحث "${searchTerm}"` : 'لا توجد وحدات'}
              </div>
            )}
          </div>

          {/* إجمالي النتائج */}
          {unitsData?.items && unitsData.items.length > 0 && (
            <div className="p-2 bg-gray-50 border-t border-gray-200 text-xs text-gray-500 text-center">
              {unitsData.items.length} من {unitsData.totalCount || 0} وحدة
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default UnitSelector;