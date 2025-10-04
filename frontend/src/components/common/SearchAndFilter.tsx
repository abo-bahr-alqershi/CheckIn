import { useState } from 'react';
import type { ReactNode } from 'react';

export interface FilterOption {
  key: string;
  label: string;
  type: 'select' | 'date' | 'dateRange' | 'number' | 'text' | 'boolean' | 'custom';
  options?: { value: string; label: string }[];
  placeholder?: string;
  /** Custom render function for custom filter types */
  render?: (value: any, onChange: (value: any) => void) => ReactNode;
}

interface SearchAndFilterProps {
  searchPlaceholder?: string;
  searchValue: string;
  onSearchChange: (value: string) => void;
  filters?: FilterOption[];
  filterValues: Record<string, any>;
  onFilterChange: (key: string, value: any) => void;
  onReset: () => void;
  showAdvanced?: boolean;
  onToggleAdvanced?: () => void;
}

const SearchAndFilter = ({
  searchPlaceholder = 'البحث...',
  searchValue,
  onSearchChange,
  filters = [],
  filterValues,
  onFilterChange,
  onReset,
  showAdvanced = false,
  onToggleAdvanced,
}: SearchAndFilterProps) => {
  const [localSearchValue, setLocalSearchValue] = useState(searchValue);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearchChange(localSearchValue);
  };

  const renderFilterInput = (filter: FilterOption) => {
    const value = filterValues[filter.key];

    switch (filter.type) {
      case 'select':
        return (
          <select
            value={value || ''}
            onChange={(e) => onFilterChange(filter.key, e.target.value)}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          >
            <option value="">جميع الخيارات</option>
            {filter.options?.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        );

      case 'boolean':
        return (
          <select
            value={value !== undefined ? value.toString() : ''}
            onChange={(e) => {
              const val = e.target.value;
              onFilterChange(filter.key, val === '' ? undefined : val === 'true');
            }}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          >
            <option value="">جميع الحالات</option>
            <option value="true">نعم</option>
            <option value="false">لا</option>
          </select>
        );

      case 'date':
        return (
          <input
            type="date"
            value={value || ''}
            onChange={(e) => onFilterChange(filter.key, e.target.value)}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          />
        );

      case 'number':
        return (
          <input
            type="number"
            value={value || ''}
            onChange={(e) => onFilterChange(filter.key, e.target.value ? Number(e.target.value) : '')}
            placeholder={filter.placeholder}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          />
        );

      case 'text':
        return (
          <input
            type="text"
            value={value || ''}
            onChange={(e) => onFilterChange(filter.key, e.target.value)}
            placeholder={filter.placeholder}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          />
        );

      case 'dateRange':
        return (
          <div className="grid grid-cols-2 gap-2">
            <input
              type="date"
              value={value?.from || ''}
              onChange={(e) =>
                onFilterChange(filter.key, { ...value, from: e.target.value })
              }
              placeholder="من"
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
            />
            <input
              type="date"
              value={value?.to || ''}
              onChange={(e) =>
                onFilterChange(filter.key, { ...value, to: e.target.value })
              }
              placeholder="إلى"
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
            />
          </div>
        );
      case 'custom':
        return filter.render ? filter.render(value, (val) => onFilterChange(filter.key, val)) : null;
      default:
        return null;
    }
  };

  const hasActiveFilters = Object.values(filterValues).some(
    (value) => value !== undefined && value !== '' && value !== null
  );

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      {/* Search Bar */}
      <form onSubmit={handleSearchSubmit} className="mb-4">
        <div className="flex gap-4">
          <div className="flex-1">
            <input
              type="text"
              value={localSearchValue}
              onChange={(e) => setLocalSearchValue(e.target.value)}
              placeholder={searchPlaceholder}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
            />
          </div>
          <button
            type="submit"
            className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            🔍 بحث
          </button>
          {filters.length > 0 && onToggleAdvanced && (
            <button
              type="button"
              onClick={onToggleAdvanced}
              className="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              ⚙️ {showAdvanced ? 'إخفاء الفلاتر' : 'فلاتر متقدمة'}
            </button>
          )}
        </div>
      </form>

      {/* Advanced Filters */}
      {showAdvanced && filters.length > 0 && (
        <div className="border-t pt-4">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-4">
            {filters.map((filter) => (
              <div key={filter.key}>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  {filter.label}
                </label>
                {renderFilterInput(filter)}
              </div>
            ))}
          </div>
          
          <div className="flex justify-between items-center">
            <div className="flex gap-2">
              <button
                onClick={() => onSearchChange(localSearchValue)}
                className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                ✓ تطبيق الفلاتر
              </button>
              <button
                onClick={onReset}
                className="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                🔄 إعادة تعيين
              </button>
            </div>
            {hasActiveFilters && (
              <span className="text-sm text-blue-600 font-medium">
                ✓ تم تطبيق فلاتر
              </span>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default SearchAndFilter;