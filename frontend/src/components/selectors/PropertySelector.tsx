import React, { useState, useEffect, useRef } from 'react';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import type { PropertyDto } from '../../types/property.types';

interface PropertySelectorProps {
  value?: string;
  onChange: (propertyId: string, property?: PropertyDto) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  ownerId?: string;
  status?: string;
  showImage?: boolean;
  required?: boolean;
}

const PropertySelector: React.FC<PropertySelectorProps> = ({
  value = '',
  onChange,
  placeholder = 'اختر الكيان',
  className = '',
  disabled = false,
  ownerId,
  status,
  showImage = true,
  required = false
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedProperty, setSelectedProperty] = useState<PropertyDto | null>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // بناء معايير البحث
  const searchParams = {
    pageNumber: 1,
    pageSize: 50,
    nameContains: searchTerm || undefined,
    ownerId: ownerId || undefined,
    status: status || undefined,
  };

  const { propertiesData, isLoading } = useAdminProperties(searchParams);

  // العثور على الكيان المحدد
  useEffect(() => {
    if (value && propertiesData?.items) {
      const property = propertiesData.items.find(p => p.id === value);
      setSelectedProperty(property || null);
    } else {
      setSelectedProperty(null);
    }
  }, [value, propertiesData]);

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

  const handlePropertySelect = (property: PropertyDto) => {
    setSelectedProperty(property);
    onChange(property.id, property);
    setIsOpen(false);
    setSearchTerm('');
  };

  const handleClear = () => {
    setSelectedProperty(null);
    onChange('');
    setSearchTerm('');
  };

  const getStatusLabel = (status: string) => {
    const statusLabels = {
      'Active': 'نشط',
      'Pending': 'معلق',
      'Suspended': 'موقوف',
      'Rejected': 'مرفوض'
    };
    return statusLabels[status] || status;
  };

  const getStatusColor = (status: string) => {
    const statusColors = {
      'Active': 'bg-green-100 text-green-800',
      'Pending': 'bg-yellow-100 text-yellow-800',
      'Suspended': 'bg-orange-100 text-orange-800',
      'Rejected': 'bg-red-100 text-red-800'
    };
    return statusColors[status] || 'bg-gray-100 text-gray-800';
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
          ${!selectedProperty && required ? 'border-red-300' : 'border-gray-300'}
        `}
      >
        <div className="flex items-center space-x-3 space-x-reverse flex-1 min-w-0">
          {selectedProperty ? (
            <>
              {/* صور غير متاحة في PropertyDto */}
              <div className="flex-1 min-w-0">
                <div className="flex items-center space-x-2 space-x-reverse">
                  <span className="font-medium text-gray-900 truncate">
                    {selectedProperty.name}
                  </span>
                  <span className="inline-flex px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-800">
                    {selectedProperty.isApproved ? 'معتمد' : 'غير معتمد'}
                  </span>
                </div>
                <div className="flex items-center space-x-4 space-x-reverse">
                  <p className="text-sm text-gray-500 truncate">{selectedProperty.city}</p>
                  <p className="text-xs text-gray-400">تقييم: {selectedProperty.starRating}/5</p>
                </div>
              </div>
            </>
          ) : (
            <span className="text-gray-500">{placeholder}</span>
          )}
        </div>
        
        <div className="flex items-center space-x-1 space-x-reverse">
          {selectedProperty && !disabled && (
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
              placeholder="البحث في الكيانات..."
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
              autoFocus
            />
          </div>

          {/* قائمة الكيانات */}
          <div className="max-h-60 overflow-y-auto">
            {isLoading ? (
              <div className="p-4 text-center text-gray-500">
                <div className="animate-spin inline-block w-6 h-6 border-2 border-gray-300 border-t-blue-600 rounded-full"></div>
                <p className="mt-2">جارٍ التحميل...</p>
              </div>
            ) : propertiesData?.items && propertiesData.items.length > 0 ? (
              propertiesData.items.map((property) => (
                <button
                  key={property.id}
                  type="button"
                  onClick={() => handlePropertySelect(property)}
                  className="w-full flex items-center px-3 py-3 hover:bg-gray-50 text-right border-b border-gray-100 last:border-b-0"
                >
                  {/* صور غير متاحة في PropertyDto */}
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center space-x-2 space-x-reverse">
                      <span className="font-medium text-gray-900 truncate">
                        {property.name}
                      </span>
                      <span className="inline-flex px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-800">
                        {property.isApproved ? 'معتمد' : 'غير معتمد'}
                      </span>
                    </div>
                    <div className="flex items-center space-x-4 space-x-reverse mt-1">
                      <p className="text-sm text-gray-500 truncate">📍 {property.city}</p>
                      <p className="text-xs text-gray-400">⭐ {property.starRating}/5</p>
                    </div>
                    {property.description && (
                      <p className="text-xs text-gray-400 truncate mt-1">{property.description}</p>
                    )}
                  </div>
                  <div className="flex-shrink-0 text-left">
                    <p className="text-sm font-medium text-gray-900">{property.typeName}</p>
                    <p className="text-xs text-gray-500">نوع الكيان</p>
                  </div>
                </button>
              ))
            ) : (
              <div className="p-4 text-center text-gray-500">
                {searchTerm ? `لا توجد نتائج للبحث "${searchTerm}"` : 'لا توجد كيانات'}
              </div>
            )}
          </div>

          {/* إجمالي النتائج */}
          {propertiesData?.items && propertiesData.items.length > 0 && (
            <div className="p-2 bg-gray-50 border-t border-gray-200 text-xs text-gray-500 text-center">
              {propertiesData.items.length} من {propertiesData.totalCount || 0} كيان
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default PropertySelector;