import React, { useState, useEffect } from 'react';
import type { UnitTypeFieldDto } from '../../types/unit-type-field.types';
import type { FieldValueDto } from '../../types/unit-field-value.types';
import TagInput from '../inputs/TagInput';
import CurrencyInput from '../inputs/CurrencyInput';

interface DynamicFieldsFormProps {
  fields: UnitTypeFieldDto[];
  values?: FieldValueDto[];
  onChange: (fieldValues: Record<string, any>) => void;
  className?: string;
  variant?: 'default' | 'modern' | 'minimal';
  collapsible?: boolean;
  showGroupIcons?: boolean;
}

const DynamicFieldsForm: React.FC<DynamicFieldsFormProps> = ({
  fields,
  values = [],
  onChange,
  className = '',
  variant = 'modern',
  collapsible = true,
  showGroupIcons = true
}) => {
  const [fieldValues, setFieldValues] = useState<Record<string, any>>({});
  const [collapsedGroups, setCollapsedGroups] = useState<Record<string, boolean>>({});

  // Initialize field values from existing values
  useEffect(() => {
    const initialValues: Record<string, any> = {};
    
    // Set values from existing data
    values.forEach((value: FieldValueDto) => {
      initialValues[value.fieldId] = value.fieldValue;
    });
    
    // Set default values for fields without values
    fields.forEach(field => {
      if (!(field.fieldId in initialValues)) {
        initialValues[field.fieldId] = getDefaultValue(field);
      }
    });
    
    setFieldValues(initialValues);
    onChange(initialValues);
  }, [fields]);

  const getDefaultValue = (field: UnitTypeFieldDto) => {
    switch (field.fieldTypeId) {
      case 'boolean':
      case 'checkbox':
        return false;
      case 'multiselect':
        return [];
      case 'number':
      case 'currency':
      case 'percentage':
      case 'range':
        return field.validationRules?.min || 0;
      default:
        return '';
    }
  };

  const updateFieldValue = (fieldId: string, value: any) => {
    const newValues = { ...fieldValues, [fieldId]: value };
    setFieldValues(newValues);
    onChange(newValues);
  };

  const renderField = (field: UnitTypeFieldDto) => {
    const value = fieldValues[field.fieldId] || getDefaultValue(field);
    const isRequired = field.isRequired;

    switch (field.fieldTypeId) {
      case 'text':
        return (
          <input
            type="text"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            minLength={field.validationRules?.minLength}
            maxLength={field.validationRules?.maxLength}
            pattern={field.validationRules?.pattern}
          />
        );

      case 'textarea':
        return (
          <textarea
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            rows={3}
            className={getInputStyles()}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            minLength={field.validationRules?.minLength}
            maxLength={field.validationRules?.maxLength}
          />
        );

      case 'number':
        return (
          <input
            type="number"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, parseFloat(e.target.value) || 0)}
            className={getInputStyles()}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            min={field.validationRules?.min}
            max={field.validationRules?.max}
            step={field.validationRules?.step || 1}
          />
        );

      case 'currency':
        return (
          <CurrencyInput
            value={value || 0}
            currency={field.validationRules?.currency || 'SAR'}
            onValueChange={(amount, currency) => updateFieldValue(field.fieldId, amount)}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            min={field.validationRules?.min || 0}
            max={field.validationRules?.max}
            variant="modern"
            size="md"
          />
        );

      case 'percentage':
        return (
          <div className="relative">
            <input
              type="number"
              value={value}
              onChange={(e) => updateFieldValue(field.fieldId, parseFloat(e.target.value) || 0)}
              className={`${getInputStyles()} pr-8`}
              placeholder={field.description || `أدخل ${field.displayName}`}
              required={isRequired}
              min={0}
              max={100}
              step="0.1"
            />
            <span className="absolute left-3 top-2 text-gray-500">%</span>
          </div>
        );

      case 'boolean':
        return (
          <div className="flex items-center p-3 bg-gray-50 rounded-lg border border-gray-200 hover:bg-gray-100 transition-colors duration-200">
            <input
              type="checkbox"
              checked={value}
              onChange={(e) => updateFieldValue(field.fieldId, e.target.checked)}
              className="h-5 w-5 text-blue-600 focus:ring-blue-500 border-gray-300 rounded transition-colors duration-200"
              required={isRequired && !value}
            />
            <label className="mr-3 text-sm text-gray-700 font-medium cursor-pointer">
              {field.description || 'نعم'}
            </label>
          </div>
        );

      case 'select':
        const selectOptions = field.fieldOptions?.options || [];
        return (
          <select
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            required={isRequired}
          >
            <option value="">اختر {field.displayName}</option>
            {selectOptions.map((option: string, index: number) => (
              <option key={index} value={option}>
                {option}
              </option>
            ))}
          </select>
        );

      case 'multiselect':
        const multiselectOptions = field.fieldOptions?.options || [];
        const selectedValues = Array.isArray(value) ? value : [];
        return (
          <div className="space-y-3">
            {multiselectOptions.map((option: string, index: number) => (
              <label key={index} className="flex items-center p-3 bg-gray-50 rounded-lg border border-gray-200 hover:bg-gray-100 transition-colors duration-200 cursor-pointer">
                <input
                  type="checkbox"
                  checked={selectedValues.includes(option)}
                  onChange={(e) => {
                    const newValues = e.target.checked
                      ? [...selectedValues, option]
                      : selectedValues.filter(v => v !== option);
                    updateFieldValue(field.fieldId, newValues);
                  }}
                  className="h-5 w-5 text-blue-600 focus:ring-blue-500 border-gray-300 rounded transition-colors duration-200"
                />
                <span className="mr-3 text-sm text-gray-700 font-medium">{option}</span>
              </label>
            ))}
          </div>
        );

      case 'date':
        return (
          <input
            type="date"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            required={isRequired}
            min={field.validationRules?.minDate}
            max={field.validationRules?.maxDate}
          />
        );

      case 'datetime':
        return (
          <input
            type="datetime-local"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            required={isRequired}
            min={field.validationRules?.minDate}
            max={field.validationRules?.maxDate}
          />
        );

      case 'time':
        return (
          <input
            type="time"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            required={isRequired}
          />
        );

      case 'email':
        return (
          <input
            type="email"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            pattern={field.validationRules?.pattern}
          />
        );

      case 'phone':
        return (
          <input
            type="tel"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            pattern={field.validationRules?.pattern}
          />
        );

      case 'url':
        return (
          <input
            type="url"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            pattern={field.validationRules?.pattern}
          />
        );

      case 'color':
        return (
          <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200">
            <input
              type="color"
              value={value}
              onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
              className="w-12 h-12 border-2 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 cursor-pointer"
              required={isRequired}
            />
            <div className="flex-1">
              <div className="text-sm font-medium text-gray-700">{value || '#000000'}</div>
              <div className="text-xs text-gray-500">انقر لاختيار لون</div>
            </div>
          </div>
        );

      case 'range':
        return (
          <div className="space-y-4 p-4 bg-gray-50 rounded-lg border border-gray-200">
            <input
              type="range"
              value={value}
              onChange={(e) => updateFieldValue(field.fieldId, parseFloat(e.target.value))}
              className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer slider"
              min={field.validationRules?.min || 0}
              max={field.validationRules?.max || 100}
              step={field.validationRules?.step || 1}
              required={isRequired}
            />
            <div className="flex justify-between items-center">
              <span className="text-xs text-gray-500">{field.validationRules?.min || 0}</span>
              <div className="flex items-center gap-2 px-3 py-1 bg-blue-100 text-blue-800 rounded-full">
                <span className="text-sm font-semibold">القيمة: {value}</span>
              </div>
              <span className="text-xs text-gray-500">{field.validationRules?.max || 100}</span>
            </div>
          </div>
        );

      case 'rating':
        return (
          <div className="p-4 bg-gray-50 rounded-lg border border-gray-200">
            <div className="flex items-center justify-center space-x-1 space-x-reverse mb-3">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  onClick={() => updateFieldValue(field.fieldId, star)}
                  className={`text-3xl transition-all duration-200 transform hover:scale-110 ${
                    star <= value ? 'text-yellow-400 drop-shadow-sm' : 'text-gray-300'
                  } hover:text-yellow-400`}
                >
                  ⭐
                </button>
              ))}
            </div>
            <div className="text-center">
              <span className="inline-flex items-center gap-2 px-3 py-1 bg-yellow-100 text-yellow-800 rounded-full text-sm font-medium">
                <span>التقييم: {value}/5</span>
                {value > 0 && <span className="text-yellow-600">{'★'.repeat(value)}</span>}
              </span>
            </div>
          </div>
        );

      case 'file':
        return (
          <div className="p-4 border-2 border-dashed border-gray-300 rounded-lg text-center hover:border-blue-400 transition-colors duration-200">
            <input
              type="file"
              onChange={(e) => {
                const file = e.target.files?.[0];
                if (file) {
                  updateFieldValue(field.fieldId, file.name);
                }
              }}
              className="hidden"
              id={`file-${field.fieldId}`}
              required={isRequired}
              accept={field.validationRules?.allowedTypes?.map(type => `.${type}`).join(',')}
            />
            <label htmlFor={`file-${field.fieldId}`} className="cursor-pointer">
              <div className="text-4xl mb-2">📎</div>
              <div className="text-sm text-gray-600">
                <span className="text-blue-600 hover:text-blue-800 font-medium">اختر ملف</span>
                <div className="text-xs mt-1">
                  {field.validationRules?.allowedTypes ? 
                    `الصيغ المسموحة: ${field.validationRules.allowedTypes.join(', ')}` : 
                    'جميع الصيغ مسموحة'
                  }
                </div>
              </div>
              {value && (
                <div className="mt-2 text-xs text-green-600 bg-green-50 p-2 rounded">
                  ✓ تم اختيار: {value}
                </div>
              )}
            </label>
          </div>
        );

      case 'image':
        return (
          <div className="p-6 border-2 border-dashed border-gray-300 rounded-lg text-center hover:border-blue-400 transition-colors duration-200">
            <input
              type="file"
              onChange={(e) => {
                const file = e.target.files?.[0];
                if (file) {
                  updateFieldValue(field.fieldId, file.name);
                }
              }}
              className="hidden"
              id={`image-${field.fieldId}`}
              required={isRequired}
              accept="image/*"
            />
            <label htmlFor={`image-${field.fieldId}`} className="cursor-pointer">
              <div className="text-5xl mb-3">🖼️</div>
              <div className="text-sm text-gray-600">
                <span className="text-blue-600 hover:text-blue-800 font-medium">اختر صورة</span>
                <div className="text-xs mt-1">JPG, PNG, GIF</div>
              </div>
              {value && (
                <div className="mt-3 text-xs text-green-600 bg-green-50 p-2 rounded">
                  ✓ تم اختيار: {value}
                </div>
              )}
            </label>
          </div>
        );

      case 'tag':
        const tags = Array.isArray(value) ? value : (value ? value.split(',') : []);
        const tagsString = tags.join(',');
        return (
          <TagInput
            value={tagsString}
            onChange={(newValue) => {
              const newTags = newValue ? newValue.split(',').map(tag => tag.trim()).filter(tag => tag.length > 0) : [];
              updateFieldValue(field.fieldId, newTags);
            }}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
            variant="modern"
            size="md"
            maxTags={field.validationRules?.maxItems || 20}
            allowDuplicates={false}
            suggestions={field.fieldOptions?.options || []}
          />
        );

      default:
        return (
          <input
            type="text"
            value={value}
            onChange={(e) => updateFieldValue(field.fieldId, e.target.value)}
            className={getInputStyles()}
            placeholder={field.description || `أدخل ${field.displayName}`}
            required={isRequired}
          />
        );
    }
  };

  const getFieldIcon = (fieldType: string) => {
    const icons: Record<string, string> = {
      text: '📝', textarea: '📄', number: '🔢', currency: '💰',
      percentage: '📊', boolean: '☑️', checkbox: '✅', select: '📋',
      multiselect: '📝', date: '📅', datetime: '⏰', time: '🕐',
      email: '📧', phone: '📞', url: '🔗', color: '🎨',
      range: '🎚️', rating: '⭐', file: '📎', image: '🖼️',
      tag: '🏷️'
    };
    return icons[fieldType] || '📝';
  };

  const getGroupIcon = (category: string) => {
    const icons: Record<string, string> = {
      'معلومات أساسية': '📋',
      'التفاصيل': '📄',
      'المواصفات': '🔧',
      'الأسعار': '💰',
      'الميزات': '⭐',
      'المعلومات الإضافية': '📝',
      'الوسائط': '🖼️',
      'الاتصال': '📞',
      'التقييم': '⭐',
      'عام': '📁'
    };
    return icons[category] || '📁';
  };

  const toggleGroup = (category: string) => {
    setCollapsedGroups(prev => ({
      ...prev,
      [category]: !prev[category]
    }));
  };

  const getFieldWrapperStyles = () => {
    switch (variant) {
      case 'modern':
        return 'bg-white border border-gray-100 rounded-xl p-4 shadow-sm hover:shadow-md transition-all duration-200';
      case 'minimal':
        return 'bg-transparent border-b border-gray-100 pb-4 last:border-b-0';
      default:
        return 'bg-gray-50 border border-gray-200 rounded-lg p-3';
    }
  };

  const getGroupHeaderStyles = () => {
    switch (variant) {
      case 'modern':
        return 'bg-gradient-to-r from-blue-50 to-indigo-50 border border-blue-100 rounded-xl p-4 shadow-sm';
      case 'minimal':
        return 'border-b border-gray-200 pb-3 mb-4';
      default:
        return 'bg-gray-100 border border-gray-200 rounded-lg p-3';
    }
  };

  const getInputStyles = () => {
    switch (variant) {
      case 'modern':
        return 'w-full px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-400 transition-all duration-200 bg-white shadow-sm';
      case 'minimal':
        return 'w-full px-3 py-2 border-b border-gray-300 focus:outline-none focus:border-blue-500 bg-transparent transition-colors duration-200';
      default:
        return 'w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500';
    }
  };

  // Group fields by category or group
  const groupedFields = fields.reduce((groups, field) => {
    const category = field.category || field.groupId || 'عام';
    if (!groups[category]) {
      groups[category] = [];
    }
    groups[category].push(field);
    return groups;
  }, {} as Record<string, UnitTypeFieldDto[]>);

  if (fields.length === 0) {
    return (
      <div className={`text-center py-12 text-gray-500 ${className}`}>
        <div className="text-6xl mb-4 opacity-50">📝</div>
        <h3 className="text-lg font-medium text-gray-600 mb-2">لا توجد حقول ديناميكية</h3>
        <p className="text-sm text-gray-500">لا توجد حقول ديناميكية محددة لهذا النوع من الوحدات</p>
      </div>
    );
  }

  return (
    <div className={`space-y-8 ${className}`} dir="rtl">
      {Object.entries(groupedFields).map(([category, categoryFields]) => {
        const isCollapsed = collapsedGroups[category];
        return (
          <div key={category} className="space-y-4">
            {/* Group Header */}
            <div className={getGroupHeaderStyles()}>
              <div 
                className={`flex items-center justify-between ${
                  collapsible ? 'cursor-pointer' : ''
                }`}
                onClick={() => collapsible && toggleGroup(category)}
              >
                <div className="flex items-center gap-3">
                  {showGroupIcons && (
                    <span className="text-2xl">{getGroupIcon(category)}</span>
                  )}
                  <h3 className="text-lg font-semibold text-gray-800">
                    {category}
                  </h3>
                  <span className="text-xs text-gray-500 bg-gray-200 px-2 py-1 rounded-full">
                    {categoryFields.length} حقل
                  </span>
                </div>
                {collapsible && (
                  <button className="text-gray-400 hover:text-gray-600 transition-colors duration-200">
                    <svg 
                      className={`w-5 h-5 transform transition-transform duration-200 ${
                        isCollapsed ? 'rotate-180' : ''
                      }`} 
                      fill="none" 
                      stroke="currentColor" 
                      viewBox="0 0 24 24"
                    >
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                  </button>
                )}
              </div>
            </div>

            {/* Group Fields */}
            {(!collapsible || !isCollapsed) && (
              <div className={`grid grid-cols-1 lg:grid-cols-2 gap-6 transition-all duration-300 ${
                isCollapsed ? 'opacity-0 max-h-0 overflow-hidden' : 'opacity-100'
              }`}>
                {categoryFields.map((field) => (
                  <div key={field.fieldId} className={getFieldWrapperStyles()}>
                    <label className="block space-y-3">
                      <div className="flex items-center gap-3">
                        <span className="text-xl">{getFieldIcon(field.fieldTypeId)}</span>
                        <div className="flex-1">
                          <div className="flex items-center gap-2">
                            <span className="text-sm font-semibold text-gray-800">
                              {field.displayName}
                            </span>
                            {field.isRequired && (
                              <span className="text-red-500 text-sm font-bold">*</span>
                            )}
                          </div>
                          {field.description && (
                            <span className="text-xs text-gray-500 block">
                              {field.description}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="pt-2">
                        {renderField(field)}
                      </div>
                    </label>
                  </div>
                ))}
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
};

export default DynamicFieldsForm;