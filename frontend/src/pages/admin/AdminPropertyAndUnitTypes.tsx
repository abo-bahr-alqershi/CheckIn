import React, { useState, useEffect } from 'react';
import { useAdminPropertyTypes } from '../../hooks/useAdminPropertyTypes';
import { useAdminUnitTypesByPropertyType } from '../../hooks/useAdminUnitTypesByPropertyType';
import { useAdminFieldGroupsByUnitType } from '../../hooks/useAdminFieldGroupsByUnitType';
import { useAdminUnitTypeFieldsByUnitType } from '../../hooks/useAdminUnitTypeFieldsByUnitType';
import type { PropertyTypeDto, CreatePropertyTypeCommand, UpdatePropertyTypeCommand } from '../../types/property-type.types';
import type { UnitTypeDto, CreateUnitTypeCommand, UpdateUnitTypeCommand } from '../../types/unit-type.types';
import type { UnitTypeFieldDto } from '../../types/unit-type-field.types';
import type { FieldGroupDto, CreateFieldGroupCommand, UpdateFieldGroupCommand } from '../../types/field-group.types';
import type { CreateUnitTypeFieldCommand, UpdateUnitTypeFieldCommand } from '../../types/unit-type-field.types';
import TagInput from '../../components/inputs/TagInput';
import { useNotificationContext } from '../../components/ui/NotificationProvider';

// قائمة شاملة بأيقونات Material Design المتوافقة مع Flutter
const MATERIAL_ICONS = [
  // أيقونات المباني والعقارات
  { name: 'home', label: 'منزل', icon: '🏠', category: 'عقارات' },
  { name: 'apartment', label: 'شقة', icon: '🏢', category: 'عقارات' },
  { name: 'villa', label: 'فيلا', icon: '🏡', category: 'عقارات' },
  { name: 'business', label: 'مكتب', icon: '🏛️', category: 'عقارات' },
  { name: 'store', label: 'محل', icon: '🏪', category: 'عقارات' },
  { name: 'hotel', label: 'فندق', icon: '🏨', category: 'عقارات' },
  { name: 'house', label: 'بيت', icon: '🏘️', category: 'عقارات' },
  { name: 'cabin', label: 'شاليه', icon: '🛖', category: 'عقارات' },
  { name: 'meeting_room', label: 'استوديو', icon: '🚪', category: 'عقارات' },
  { name: 'stairs', label: 'دوبلكس', icon: '🪜', category: 'عقارات' },
  { name: 'roofing', label: 'بنتهاوس', icon: '🏗️', category: 'عقارات' },
  { name: 'warehouse', label: 'مستودع', icon: '🏭', category: 'عقارات' },
  { name: 'terrain', label: 'أرض', icon: '🏞️', category: 'عقارات' },
  { name: 'grass', label: 'مزرعة', icon: '🌾', category: 'عقارات' },
  { name: 'location_city', label: 'مدينة', icon: '🌆', category: 'عقارات' },
  { name: 'cottage', label: 'كوخ', icon: '🏚️', category: 'عقارات' },
  { name: 'holiday_village', label: 'قرية سياحية', icon: '🏘️', category: 'عقارات' },
  { name: 'gite', label: 'نزل', icon: '🏠', category: 'عقارات' },
  { name: 'domain', label: 'نطاق', icon: '🌐', category: 'عقارات' },
  { name: 'foundation', label: 'أساس', icon: '🏗️', category: 'عقارات' },
  
  // أيقونات الغرف والمساحات
  { name: 'bed', label: 'سرير', icon: '🛏️', category: 'غرف' },
  { name: 'king_bed', label: 'سرير كبير', icon: '🛏️', category: 'غرف' },
  { name: 'single_bed', label: 'سرير مفرد', icon: '🛏️', category: 'غرف' },
  { name: 'bedroom_parent', label: 'غرفة نوم رئيسية', icon: '🛏️', category: 'غرف' },
  { name: 'bedroom_child', label: 'غرفة أطفال', icon: '🛏️', category: 'غرف' },
  { name: 'living_room', label: 'غرفة معيشة', icon: '🛋️', category: 'غرف' },
  { name: 'dining_room', label: 'غرفة طعام', icon: '🍽️', category: 'غرف' },
  { name: 'kitchen', label: 'مطبخ', icon: '🍳', category: 'غرف' },
  { name: 'bathroom', label: 'حمام', icon: '🚿', category: 'غرف' },
  { name: 'bathtub', label: 'حوض استحمام', icon: '🛁', category: 'غرف' },
  { name: 'shower', label: 'دش', icon: '🚿', category: 'غرف' },
  { name: 'garage', label: 'كراج', icon: '🚗', category: 'غرف' },
  { name: 'balcony', label: 'شرفة', icon: '🌅', category: 'غرف' },
  { name: 'deck', label: 'سطح', icon: '☀️', category: 'غرف' },
  { name: 'yard', label: 'فناء', icon: '🏡', category: 'غرف' },
  
  // أيقونات المرافق
  { name: 'pool', label: 'مسبح', icon: '🏊', category: 'مرافق' },
  { name: 'hot_tub', label: 'جاكوزي', icon: '♨️', category: 'مرافق' },
  { name: 'fitness_center', label: 'صالة رياضية', icon: '💪', category: 'مرافق' },
  { name: 'sports_tennis', label: 'ملعب تنس', icon: '🎾', category: 'مرافق' },
  { name: 'sports_soccer', label: 'ملعب كرة قدم', icon: '⚽', category: 'مرافق' },
  { name: 'sports_basketball', label: 'ملعب كرة سلة', icon: '🏀', category: 'مرافق' },
  { name: 'spa', label: 'سبا', icon: '💆', category: 'مرافق' },
  { name: 'local_parking', label: 'موقف سيارات', icon: '🅿️', category: 'مرافق' },
  { name: 'elevator', label: 'مصعد', icon: '🛗', category: 'مرافق' },
  { name: 'stairs', label: 'درج', icon: '📶', category: 'مرافق' },
  { name: 'wifi', label: 'واي فاي', icon: '📶', category: 'مرافق' },
  { name: 'ac_unit', label: 'تكييف', icon: '❄️', category: 'مرافق' },
  { name: 'fireplace', label: 'مدفأة', icon: '🔥', category: 'مرافق' },
  { name: 'water_drop', label: 'ماء', icon: '💧', category: 'مرافق' },
  { name: 'electric_bolt', label: 'كهرباء', icon: '⚡', category: 'مرافق' },
  
  // أيقونات الخدمات
  { name: 'cleaning_services', label: 'خدمة تنظيف', icon: '🧹', category: 'خدمات' },
  { name: 'room_service', label: 'خدمة الغرف', icon: '🛎️', category: 'خدمات' },
  { name: 'local_laundry_service', label: 'غسيل', icon: '👔', category: 'خدمات' },
  { name: 'dry_cleaning', label: 'تنظيف جاف', icon: '👗', category: 'خدمات' },
  { name: 'iron', label: 'كوي', icon: '👔', category: 'خدمات' },
  { name: 'breakfast_dining', label: 'إفطار', icon: '🍳', category: 'خدمات' },
  { name: 'lunch_dining', label: 'غداء', icon: '🍽️', category: 'خدمات' },
  { name: 'dinner_dining', label: 'عشاء', icon: '🍽️', category: 'خدمات' },
  { name: 'restaurant', label: 'مطعم', icon: '🍴', category: 'خدمات' },
  { name: 'local_cafe', label: 'مقهى', icon: '☕', category: 'خدمات' },
  { name: 'local_bar', label: 'بار', icon: '🍺', category: 'خدمات' },
  
  // أيقونات الأمان
  { name: 'security', label: 'أمن', icon: '🔒', category: 'أمان' },
  { name: 'lock', label: 'قفل', icon: '🔒', category: 'أمان' },
  { name: 'key', label: 'مفتاح', icon: '🔑', category: 'أمان' },
  { name: 'shield', label: 'درع', icon: '🛡️', category: 'أمان' },
  { name: 'verified_user', label: 'مستخدم موثق', icon: '✅', category: 'أمان' },
  { name: 'safety_check', label: 'فحص أمان', icon: '✅', category: 'أمان' },
  { name: 'emergency', label: 'طوارئ', icon: '🚨', category: 'أمان' },
  { name: 'local_police', label: 'شرطة', icon: '👮', category: 'أمان' },
  { name: 'local_fire_department', label: 'إطفاء', icon: '🚒', category: 'أمان' },
  { name: 'medical_services', label: 'خدمات طبية', icon: '🏥', category: 'أمان' },
  
  // أيقونات المواقع
  { name: 'location_on', label: 'موقع', icon: '📍', category: 'مواقع' },
  { name: 'map', label: 'خريطة', icon: '🗺️', category: 'مواقع' },
  { name: 'place', label: 'مكان', icon: '📍', category: 'مواقع' },
  { name: 'near_me', label: 'قريب مني', icon: '📍', category: 'مواقع' },
  { name: 'my_location', label: 'موقعي', icon: '🎯', category: 'مواقع' },
  { name: 'directions', label: 'اتجاهات', icon: '🧭', category: 'مواقع' },
  { name: 'navigation', label: 'ملاحة', icon: '🧭', category: 'مواقع' },
  
  // أيقونات المواصلات
  { name: 'directions_car', label: 'سيارة', icon: '🚗', category: 'مواصلات' },
  { name: 'directions_bus', label: 'حافلة', icon: '🚌', category: 'مواصلات' },
  { name: 'directions_subway', label: 'مترو', icon: '🚇', category: 'مواصلات' },
  { name: 'directions_train', label: 'قطار', icon: '🚂', category: 'مواصلات' },
  { name: 'directions_boat', label: 'قارب', icon: '⛵', category: 'مواصلات' },
  { name: 'flight', label: 'طيران', icon: '✈️', category: 'مواصلات' },
  { name: 'directions_walk', label: 'مشي', icon: '🚶', category: 'مواصلات' },
  { name: 'directions_bike', label: 'دراجة', icon: '🚴', category: 'مواصلات' },
  
  // أيقونات عامة
  { name: 'star', label: 'نجمة', icon: '⭐', category: 'عام' },
  { name: 'favorite', label: 'مفضل', icon: '❤️', category: 'عام' },
  { name: 'bookmark', label: 'علامة', icon: '🔖', category: 'عام' },
  { name: 'share', label: 'مشاركة', icon: '📤', category: 'عام' },
  { name: 'info', label: 'معلومات', icon: 'ℹ️', category: 'عام' },
  { name: 'help', label: 'مساعدة', icon: '❓', category: 'عام' },
  { name: 'settings', label: 'إعدادات', icon: '⚙️', category: 'عام' },
  { name: 'phone', label: 'هاتف', icon: '📞', category: 'عام' },
  { name: 'email', label: 'بريد', icon: '📧', category: 'عام' },
  { name: 'message', label: 'رسالة', icon: '💬', category: 'عام' },
  { name: 'notifications', label: 'إشعارات', icon: '🔔', category: 'عام' },
];

// مكون اختيار الأيقونة
const IconPicker = ({ 
  selectedIcon, 
  onSelectIcon, 
  onClose 
}: { 
  selectedIcon: string; 
  onSelectIcon: (icon: string) => void; 
  onClose: () => void;
}) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('الكل');
  
  const categories = ['الكل', ...new Set(MATERIAL_ICONS.map(icon => icon.category))];
  
  const filteredIcons = MATERIAL_ICONS.filter(icon => {
    const matchesSearch = icon.label.toLowerCase().includes(searchTerm.toLowerCase()) ||
                          icon.name.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory = selectedCategory === 'الكل' || icon.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });
  
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold">اختر أيقونة</h3>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700"
          >
            ✕
          </button>
        </div>
        
        {/* البحث والفلترة */}
        <div className="mb-4 space-y-3">
          <input
            type="text"
            placeholder="ابحث عن أيقونة..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          
          <div className="flex flex-wrap gap-2">
            {categories.map(category => (
              <button
                key={category}
                onClick={() => setSelectedCategory(category)}
                className={`px-3 py-1 rounded-full text-sm transition-colors ${
                  selectedCategory === category
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {category}
              </button>
            ))}
          </div>
        </div>
        
        {/* قائمة الأيقونات */}
        <div className="flex-1 overflow-y-auto">
          <div className="grid grid-cols-8 gap-2">
            {filteredIcons.map(icon => (
              <button
                key={icon.name}
                onClick={() => onSelectIcon(icon.name)}
                className={`p-3 rounded-lg border-2 transition-all hover:scale-105 ${
                  selectedIcon === icon.name
                    ? 'border-blue-500 bg-blue-50'
                    : 'border-gray-200 hover:border-gray-300'
                }`}
                title={`${icon.label} (${icon.name})`}
              >
                <div className="flex flex-col items-center">
                  <span className="text-2xl mb-1">{icon.icon}</span>
                  <span className="text-xs text-gray-600">{icon.label}</span>
                  <code className="text-xs text-gray-400 mt-1">{icon.name}</code>
                </div>
              </button>
            ))}
          </div>
          
          {filteredIcons.length === 0 && (
            <div className="text-center py-8 text-gray-500">
              <p>لا توجد أيقونات مطابقة للبحث</p>
            </div>
          )}
        </div>
        
        <div className="mt-4 p-3 bg-gray-50 rounded-lg">
          <p className="text-sm text-gray-600">
            <span className="font-semibold">ملاحظة:</span> هذه الأيقونات متوافقة مع Material Icons في Flutter.
            استخدم اسم الأيقونة (مثل: Icons.{selectedIcon || 'home'}) في تطبيق Flutter.
          </p>
        </div>
      </div>
    </div>
  );
};

const AdminPropertyAndUnitTypes = () => {
  const { showSuccess, showError } = useNotificationContext();
  
  // State for UI management
  const [selectedPropertyType, setSelectedPropertyType] = useState<PropertyTypeDto | null>(null);
  const [selectedUnitType, setSelectedUnitType] = useState<UnitTypeDto | null>(null);
  const [selectedFieldGroup, setSelectedFieldGroup] = useState<FieldGroupDto | null>(null);
  const [selectedField, setSelectedField] = useState<UnitTypeFieldDto | null>(null);
  
  // Modal states
  const [showPropertyTypeModal, setShowPropertyTypeModal] = useState(false);
  const [showUnitTypeModal, setShowUnitTypeModal] = useState(false);
  const [showFieldGroupModal, setShowFieldGroupModal] = useState(false);
  const [showFieldModal, setShowFieldModal] = useState(false);
  const [showIconPicker, setShowIconPicker] = useState(false);
  const [iconPickerTarget, setIconPickerTarget] = useState<'property' | 'unit' | null>(null);
  
  // Form states
  const [propertyTypeForm, setPropertyTypeForm] = useState<CreatePropertyTypeCommand>({
    name: '',
    description: '',
    defaultAmenities: '',
    icon: 'home', // أيقونة افتراضية
  });
  
  const [unitTypeForm, setUnitTypeForm] = useState<CreateUnitTypeCommand>({
    propertyTypeId: '',
    name: '',
    maxCapacity: 1,
    icon: 'apartment', // أيقونة افتراضية
    isHasAdults: false,
    isHasChildren: false,
    isMultiDays: false,
    isRequiredToDetermineTheHour: false,
  });
  
  const [fieldGroupForm, setFieldGroupForm] = useState<CreateFieldGroupCommand>({
    unitTypeId: '',
    groupName: '',
    displayName: '',
    description: '',
    sortOrder: 0,
    isCollapsible: true,
    isExpandedByDefault: true
  });
  
  const [fieldForm, setFieldForm] = useState<CreateUnitTypeFieldCommand>({
    unitTypeId: '',
    fieldTypeId: '',
    fieldName: '',
    displayName: '',
    description: '',
    fieldOptions: { options: [] },
    validationRules: {},
    isRequired: false,
    isSearchable: false,
    isPublic: true,
    sortOrder: 0,
    category: '',
    isForUnits: true,
    showInCards: false,
    isPrimaryFilter: false,
    priority: 0
  });

  // Search and filter states
  const [searchTerm, setSearchTerm] = useState('');
  const [filterByRequired, setFilterByRequired] = useState<boolean | null>(null);
  const [filterByPublic, setFilterByPublic] = useState<boolean | null>(null);
  const [filterByFieldType, setFilterByFieldType] = useState<string>('');

  // استخدام الهوك لإدارة أنواع الكيانات
  const PAGE_SIZE = 1000;
  const {
    propertyTypesData,
    isLoading: propertyTypesLoading,
    error: propertyTypesError,
    createPropertyType,
    updatePropertyType,
    deletePropertyType,
  } = useAdminPropertyTypes({ pageNumber: 1, pageSize: PAGE_SIZE });
  const propertyTypes = propertyTypesData?.items || [];

  // استخدام الهوك لإدارة أنواع الوحدات لنوع الكيان المحدد
  const {
    unitTypesData,
    isLoading: unitTypesLoading,
    error: unitTypesError,
    createUnitType,
    updateUnitType,
    deleteUnitType,
  } = useAdminUnitTypesByPropertyType({ propertyTypeId: selectedPropertyType?.id || '', pageNumber: 1, pageSize: PAGE_SIZE });
  const unitTypes = unitTypesData?.items || [];

  // استخدام الهوك لإدارة مجموعات الحقول لنوع الوحدة المحدد
  const {
    fieldGroupsData,
    isLoading: fieldGroupsLoading,
    error: fieldGroupsError,
    createFieldGroup,
    updateFieldGroup,
    deleteFieldGroup,
  } = useAdminFieldGroupsByUnitType({ unitTypeId: selectedUnitType?.id || '' });
  const fieldGroups = fieldGroupsData || [];

  // استخدام الهوك لإدارة الحقول الديناميكية لنوع الوحدة المحدد
  const {
    unitTypeFieldsData,
    isLoading: fieldsLoading,
    error: fieldsError,
    createUnitTypeField,
    updateUnitTypeField,
    deleteUnitTypeField,
  } = useAdminUnitTypeFieldsByUnitType({
    unitTypeId: selectedUnitType?.id || '',
    searchTerm,
  });
  const unitTypeFields = unitTypeFieldsData || [];

  // Helper functions
  const resetPropertyTypeForm = () => {
    setPropertyTypeForm({
      name: '',
      description: '',
      defaultAmenities: '',
      icon: 'home',
    });
  };

  const resetUnitTypeForm = () => {
    setUnitTypeForm({
      propertyTypeId: selectedPropertyType?.id || '',
      name: '',
      maxCapacity: 1,
      icon: 'apartment',
      isHasAdults: false,
      isHasChildren: false,
      isMultiDays: false,
      isRequiredToDetermineTheHour: false,
    });
  };

  const resetFieldGroupForm = () => {
    setFieldGroupForm({
      unitTypeId: selectedUnitType?.id || '',
      groupName: '',
      displayName: '',
      description: '',
      sortOrder: fieldGroups.length,
      isCollapsible: true,
      isExpandedByDefault: true
    });
  };

  const resetFieldForm = () => {
    setFieldForm({
      unitTypeId: selectedUnitType?.id || '',
      fieldTypeId: '',
      fieldName: '',
      displayName: '',
      description: '',
      fieldOptions: { options: [] },
      validationRules: {},
      isRequired: false,
      isSearchable: false,
      isPublic: true,
      sortOrder: unitTypeFields.length,
      category: '',
      isForUnits: true,
      showInCards: false,
      isPrimaryFilter: false,
      priority: 0
    });
  };

  // Filter fields based on search and filters
  const filteredFields = unitTypeFields.filter(field => {
    const matchesSearch = field.displayName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         field.fieldName.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesRequired = filterByRequired === null || field.isRequired === filterByRequired;
    const matchesPublic = filterByPublic === null || field.isPublic === filterByPublic;
    const matchesFieldType = filterByFieldType === '' || field.fieldTypeId === filterByFieldType;
    
    return matchesSearch && matchesRequired && matchesPublic && matchesFieldType;
  });

  // Get ungrouped fields
  const ungroupedFields = filteredFields.filter(field => 
    !fieldGroups.some(group => group.fields.some(f => f.fieldId === field.fieldId))
  );

  // Field type options with comprehensive details
  const fieldTypeOptions = [
    { 
      value: "text", 
      label: "نص قصير", 
      icon: "📝",
      description: "حقل نص مفرد للمعلومات القصيرة",
      defaultValidation: { minLength: 1, maxLength: 255 },
      allowedValidations: ["minLength", "maxLength", "pattern", "required"]
    },
    { 
      value: "textarea", 
      label: "نص طويل", 
      icon: "📄",
      description: "حقل نص متعدد الأسطر للأوصاف والتفاصيل",
      defaultValidation: { minLength: 1, maxLength: 2000 },
      allowedValidations: ["minLength", "maxLength", "required"]
    },
    { 
      value: "number", 
      label: "رقم", 
      icon: "🔢",
      description: "حقل رقمي للأعداد الصحيحة والعشرية",
      defaultValidation: { min: 0, max: 999999 },
      allowedValidations: ["min", "max", "step", "required"]
    },
    { 
      value: "currency", 
      label: "مبلغ مالي", 
      icon: "💰",
      description: "حقل خاص للمبالغ المالية مع تنسيق العملة",
      defaultValidation: { min: 0, max: 999999999 },
      allowedValidations: ["min", "max", "currency", "required"]
    },
    { 
      value: "boolean", 
      label: "منطقي (نعم/لا)", 
      icon: "☑️",
      description: "حقل اختيار ثنائي (صواب/خطأ)",
      defaultValidation: {},
      allowedValidations: ["required"]
    },
    { 
      value: "select", 
      label: "قائمة منسدلة", 
      icon: "📋",
      description: "قائمة منسدلة لاختيار عنصر واحد",
      defaultValidation: {},
      allowedValidations: ["required"],
      requiresOptions: true
    },
    { 
      value: "multiselect", 
      label: "تحديد متعدد", 
      icon: "📝",
      description: "قائمة لاختيار عدة عناصر",
      defaultValidation: { minItems: 0, maxItems: 10 },
      allowedValidations: ["minItems", "maxItems", "required"],
      requiresOptions: true
    },
    { 
      value: "date", 
      label: "تاريخ", 
      icon: "📅",
      description: "حقل اختيار التاريخ",
      defaultValidation: {},
      allowedValidations: ["minDate", "maxDate", "required"]
    },
    { 
      value: "email", 
      label: "بريد إلكتروني", 
      icon: "📧",
      description: "حقل البريد الإلكتروني مع التحقق التلقائي",
      defaultValidation: { pattern: "^[^@]+@[^@]+\.[^@]+$" },
      allowedValidations: ["pattern", "required"]
    },
    { 
      value: "phone", 
      label: "رقم هاتف", 
      icon: "📞",
      description: "حقل رقم الهاتف مع التنسيق التلقائي",
      defaultValidation: { pattern: "^[+]?[0-9\s\-KATEX_INLINE_OPENKATEX_INLINE_CLOSE]+$" },
      allowedValidations: ["pattern", "required"]
    },
    { 
      value: "file", 
      label: "ملف", 
      icon: "📎",
      description: "رفع ملف من أي نوع",
      defaultValidation: { maxSize: 10, allowedTypes: ["pdf", "doc", "docx"] },
      allowedValidations: ["maxSize", "allowedTypes", "required"]
    },
    { 
      value: "image", 
      label: "صورة", 
      icon: "🖼️",
      description: "رفع الصور مع معاينة",
      defaultValidation: { maxSize: 5, allowedTypes: ["jpg", "png", "gif"] },
      allowedValidations: ["maxSize", "allowedTypes", "maxWidth", "maxHeight", "required"]
    }
  ];

  const getFieldTypeIcon = (type: string) => {
    const fieldType = fieldTypeOptions.find(option => option.value === type);
    return fieldType?.icon || "📝";
  };

  // دالة للحصول على أيقونة من الاسم
  const getIconDisplay = (iconName: string) => {
    const icon = MATERIAL_ICONS.find(i => i.name === iconName);
    return icon ? icon.icon : '🏠';
  };

  // Handler for icon selection
  const handleIconSelect = (iconName: string) => {
    if (iconPickerTarget === 'property') {
      setPropertyTypeForm({ ...propertyTypeForm, icon: iconName });
    } else if (iconPickerTarget === 'unit') {
      setUnitTypeForm({ ...unitTypeForm, icon: iconName });
    }
    setShowIconPicker(false);
    setIconPickerTarget(null);
  };

  // Handler for creating or updating a dynamic field
  const handleSaveField = () => {
    if (selectedField) {
      updateUnitTypeField.mutate({ fieldId: selectedField.fieldId, data: { fieldId: selectedField.fieldId, ...fieldForm, groupId: selectedFieldGroup?.groupId || '' } }, {
        onSuccess: () => {
          showSuccess('تم تحديث الحقل بنجاح');
          setShowFieldModal(false);
          setSelectedField(null);
          resetFieldForm();
        },
        onError: (error: any) => {
          showError(error.response?.data?.message || 'فشل في تحديث الحقل');
        }
      });
    } else {
      createUnitTypeField.mutate({ ...fieldForm, unitTypeId: selectedUnitType!.id, groupId: selectedFieldGroup?.groupId || '' }, {
        onSuccess: () => {
          showSuccess('تم إضافة الحقل بنجاح');
          setShowFieldModal(false);
          resetFieldForm();
        },
        onError: (error: any) => {
          showError(error.response?.data?.message || 'فشل في إضافة الحقل');
        }
      });
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6" dir="rtl">
      {/* Header */}
      <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
        <div className="flex justify-between items-center mb-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة أنواع الكيانات والوحدات والحقول الديناميكية</h1>
            <p className="text-gray-600">إدارة شاملة متدرجة لأنواع الكيانات وأنواع الوحدات ومجموعات الحقول والحقول الديناميكية مع دعم الأيقونات</p>
          </div>
        </div>

        {/* Statistics */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="bg-blue-50 p-4 rounded-lg">
            <div className="flex items-center">
              <div className="text-blue-600 text-2xl ml-3">🏢</div>
              <div>
                <p className="text-sm text-blue-600">أنواع الكيانات</p>
                <p className="text-2xl font-bold text-blue-900">{propertyTypes.length}</p>
              </div>
            </div>
          </div>
          <div className="bg-green-50 p-4 rounded-lg">
            <div className="flex items-center">
              <div className="text-green-600 text-2xl ml-3">🏠</div>
              <div>
                <p className="text-sm text-green-600">أنواع الوحدات</p>
                <p className="text-2xl font-bold text-green-900">{unitTypes.length}</p>
              </div>
            </div>
          </div>
          <div className="bg-yellow-50 p-4 rounded-lg">
            <div className="flex items-center">
              <div className="text-yellow-600 text-2xl ml-3">📁</div>
              <div>
                <p className="text-sm text-yellow-600">مجموعات الحقول</p>
                <p className="text-2xl font-bold text-yellow-900">{fieldGroups.length}</p>
              </div>
            </div>
          </div>
          <div className="bg-purple-50 p-4 rounded-lg">
            <div className="flex items-center">
              <div className="text-purple-600 text-2xl ml-3">📝</div>
              <div>
                <p className="text-sm text-purple-600">الحقول الديناميكية</p>
                <p className="text-2xl font-bold text-purple-900">{unitTypeFields.length}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content with 4 Columns Hierarchy */}
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Column 1: Property Types */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center">
              🏢 أنواع الكيانات
            </h2>
            <button
              onClick={() => {
                resetPropertyTypeForm();
                setShowPropertyTypeModal(true);
              }}
              className="bg-blue-600 text-white px-3 py-2 rounded-lg hover:bg-blue-700 transition-colors text-sm"
            >
              + إضافة
            </button>
          </div>
          
          {propertyTypesLoading ? (
            <div className="flex justify-center py-8">
              <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
            </div>
          ) : (
            <div className="space-y-2">
              {propertyTypes.map(propertyType => (
                <div
                  key={propertyType.id}
                  className={`p-3 rounded-lg border cursor-pointer transition-colors ${
                    selectedPropertyType?.id === propertyType.id
                      ? "border-blue-500 bg-blue-50"
                      : "border-gray-200 hover:border-gray-300"
                  }`}
                  onClick={() => {
                    setSelectedPropertyType(propertyType);
                    setSelectedUnitType(null);
                    setSelectedFieldGroup(null);
                  }}
                >
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <div className="flex items-center space-x-2 space-x-reverse">
                        <span className="text-lg">{getIconDisplay(propertyType.icon || 'home')}</span>
                        <h3 className="font-medium text-gray-900 text-sm">{propertyType.name}</h3>
                      </div>
                      <p className="text-xs text-gray-600">{propertyType.description}</p>
                      {propertyType.defaultAmenities && (
                        <span className="inline-block bg-gray-100 text-gray-700 text-xs px-2 py-1 rounded mt-1">
                          {propertyType.defaultAmenities}
                        </span>
                      )}
                      {propertyType.icon && (
                        <div className="mt-1">
                          <code className="text-xs text-gray-500">Icons.{propertyType.icon}</code>
                        </div>
                      )}
                    </div>
                    <div className="flex space-x-1 space-x-reverse">
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          setPropertyTypeForm({
                            name: propertyType.name,
                            description: propertyType.description,
                            defaultAmenities: propertyType.defaultAmenities,
                            icon: propertyType.icon || 'home',
                          });
                          setSelectedPropertyType(propertyType);
                          setShowPropertyTypeModal(true);
                        }}
                        className="text-blue-600 hover:text-blue-800 text-xs"
                      >
                        ✏️
                      </button>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          if (confirm("هل أنت متأكد من حذف هذا النوع؟")) {
                            deletePropertyType.mutate(propertyType.id);
                          }
                        }}
                        className="text-red-600 hover:text-red-800 text-xs"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                </div>
              ))}
              
              {propertyTypes.length === 0 && (
                <div className="text-center py-8 text-gray-500">
                  <div className="text-3xl mb-2">🏢</div>
                  <p className="text-sm">لا توجد أنواع كيانات</p>
                  <p className="text-xs">قم بإضافة نوع كيان للبدء</p>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Column 2: Unit Types */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center">
              🏠 أنواع الوحدات
            </h2>
            <button
              onClick={() => {
                if (!selectedPropertyType) {
                  alert("يرجى اختيار نوع كيان أولاً");
                  return;
                }
                resetUnitTypeForm();
                setShowUnitTypeModal(true);
              }}
              disabled={!selectedPropertyType}
              className="bg-green-600 text-white px-3 py-2 rounded-lg hover:bg-green-700 transition-colors disabled:bg-gray-400 text-sm"
            >
              + إضافة
            </button>
          </div>

          {!selectedPropertyType ? (
            <div className="text-center py-8 text-gray-500">
              <div className="text-3xl mb-2">🏠</div>
              <p className="text-sm">اختر نوع كيان لعرض الوحدات</p>
            </div>
          ) : unitTypesLoading ? (
            <div className="flex justify-center py-8">
              <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-green-600"></div>
            </div>
          ) : (
            <div className="space-y-2">
              {unitTypes.map(unitType => (
                <div
                  key={unitType.id}
                  className={`p-3 rounded-lg border cursor-pointer transition-colors ${
                    selectedUnitType?.id === unitType.id
                      ? "border-green-500 bg-green-50"
                      : "border-gray-200 hover:border-gray-300"
                  }`}
                  onClick={() => {
                    setSelectedUnitType(unitType);
                    setSelectedFieldGroup(null);
                  }}
                >
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <div className="flex items-center space-x-2 space-x-reverse">
                        <span className="text-lg">{getIconDisplay(unitType.icon || 'apartment')}</span>
                        <h3 className="font-medium text-gray-900 text-sm">{unitType.name}</h3>
                      </div>
                      <p className="text-xs text-gray-600">{unitType.description || "لا يوجد وصف"}</p>
                      
                      {/* عرض الخصائص الجديدة */}
                      <div className="flex flex-wrap gap-1 mt-1">
                        {unitType.isHasAdults && (
                          <span className="inline-block bg-blue-100 text-blue-700 text-xs px-2 py-0.5 rounded">
                            👨 بالغين
                          </span>
                        )}
                        {unitType.isHasChildren && (
                          <span className="inline-block bg-green-100 text-green-700 text-xs px-2 py-0.5 rounded">
                            👶 أطفال
                          </span>
                        )}
                        {unitType.isMultiDays && (
                          <span className="inline-block bg-yellow-100 text-yellow-700 text-xs px-2 py-0.5 rounded">
                            📅 متعدد الأيام
                          </span>
                        )}
                        {unitType.isRequiredToDetermineTheHour && (
                          <span className="inline-block bg-purple-100 text-purple-700 text-xs px-2 py-0.5 rounded">
                            ⏰ تحديد الساعة
                          </span>
                        )}
                      </div>
                      
                      {unitType.icon && (
                        <div className="mt-1">
                          <code className="text-xs text-gray-500">Icons.{unitType.icon}</code>
                        </div>
                      )}
                    </div>
                    <div className="flex space-x-1 space-x-reverse">
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          setUnitTypeForm({
                            propertyTypeId: selectedPropertyType.id,
                            name: unitType.name,
                            maxCapacity: 1,
                            icon: unitType.icon || 'apartment',
                            isHasAdults: unitType.isHasAdults || false,
                            isHasChildren: unitType.isHasChildren || false,
                            isMultiDays: unitType.isMultiDays || false,
                            isRequiredToDetermineTheHour: unitType.isRequiredToDetermineTheHour || false,
                          });
                          setSelectedUnitType(unitType);
                          setShowUnitTypeModal(true);
                        }}
                        className="text-green-600 hover:text-green-800 text-xs"
                      >
                        ✏️
                      </button>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          if (confirm("هل أنت متأكد من حذف هذا النوع؟")) {
                            deleteUnitType.mutate(unitType.id);
                          }
                        }}
                        className="text-red-600 hover:text-red-800 text-xs"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                </div>
              ))}
              
              {unitTypes.length === 0 && (
                <div className="text-center py-8 text-gray-500">
                  <div className="text-3xl mb-2">🏠</div>
                  <p className="text-sm">لا توجد أنواع وحدات</p>
                  <p className="text-xs">قم بإضافة نوع وحدة للبدء</p>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Column 3: Field Groups */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center">
              📁 مجموعات الحقول
            </h2>
            <button
              onClick={() => {
                if (!selectedUnitType) {
                  alert("يرجى اختيار نوع وحدة أولاً");
                  return;
                }
                resetFieldGroupForm();
                setShowFieldGroupModal(true);
              }}
              disabled={!selectedUnitType}
              className="bg-yellow-600 text-white px-3 py-2 rounded-lg hover:bg-yellow-700 transition-colors disabled:bg-gray-400 text-sm"
            >
              + إضافة
            </button>
          </div>

          {!selectedUnitType ? (
            <div className="text-center py-8 text-gray-500">
              <div className="text-3xl mb-2">📁</div>
              <p className="text-sm">اختر نوع وحدة لعرض المجموعات</p>
            </div>
          ) : fieldGroupsLoading ? (
            <div className="flex justify-center py-8">
              <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-yellow-600"></div>
            </div>
          ) : (
            <div className="space-y-2">
              {fieldGroups.map(group => (
                <div
                  key={group.groupId}
                  className={`p-3 rounded-lg border cursor-pointer transition-colors ${
                    selectedFieldGroup?.groupId === group.groupId
                      ? "border-yellow-500 bg-yellow-50"
                      : "border-gray-200 hover:border-gray-300"
                  }`}
                  onClick={() => setSelectedFieldGroup(group)}
                >
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <h3 className="font-medium text-gray-900 text-sm">{group.displayName || group.groupName}</h3>
                      <p className="text-xs text-gray-600">{group.description}</p>
                      <div className="flex items-center space-x-2 space-x-reverse mt-1">
                        <span className="text-xs text-gray-500">{group.fields.length} حقل</span>
                        {group.isCollapsible && (
                          <span className="text-xs text-gray-500">
                            {group.isExpandedByDefault ? "🔽" : "▶️"} قابل للطي
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="flex space-x-1 space-x-reverse">
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          setFieldGroupForm({
                            unitTypeId: selectedUnitType.id,
                            groupName: group.groupName,
                            displayName: group.displayName,
                            description: group.description,
                            sortOrder: group.sortOrder,
                            isCollapsible: group.isCollapsible,
                            isExpandedByDefault: group.isExpandedByDefault
                          });
                          setSelectedFieldGroup(group);
                          setShowFieldGroupModal(true);
                        }}
                        className="text-yellow-600 hover:text-yellow-800 text-xs"
                      >
                        ✏️
                      </button>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          if (confirm("هل أنت متأكد من حذف هذه المجموعة؟")) {
                            deleteFieldGroup.mutate(group.groupId);
                          }
                        }}
                        className="text-red-600 hover:text-red-800 text-xs"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                </div>
              ))}
              
              {fieldGroups.length === 0 && (
                <div className="text-center py-8 text-gray-500">
                  <div className="text-3xl mb-2">📁</div>
                  <p className="text-sm">لا توجد مجموعات حقول</p>
                  <p className="text-xs">قم بإضافة مجموعة للبدء</p>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Column 4: Dynamic Fields */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center">
              📝 الحقول الديناميكية
            </h2>
            <button
              onClick={() => {
                if (!selectedUnitType) {
                  alert("يرجى اختيار نوع وحدة أولاً");
                  return;
                }
                setSelectedField(null);
                resetFieldForm();
                setShowFieldModal(true);
              }}
              disabled={!selectedUnitType}
              className="bg-purple-600 text-white px-3 py-2 rounded-lg hover:bg-purple-700 transition-colors disabled:bg-gray-400 text-sm"
            >
              + إضافة
            </button>
          </div>

          {!selectedUnitType ? (
            <div className="text-center py-8 text-gray-500">
              <div className="text-3xl mb-2">📝</div>
              <p className="text-sm">اختر نوع وحدة لعرض الحقول</p>
            </div>
          ) : (
            <>
              {/* Search and Filters */}
              <div className="mb-4 space-y-2">
                <input
                  type="text"
                  placeholder="البحث في الحقول..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500 text-sm"
                />
                
                <div className="flex flex-wrap gap-1">
                  <select
                    value={filterByRequired === null ? "" : filterByRequired.toString()}
                    onChange={(e) => setFilterByRequired(e.target.value === "" ? null : e.target.value === "true")}
                    className="px-2 py-1 border border-gray-300 rounded text-xs"
                  >
                    <option value="">الكل</option>
                    <option value="true">إلزامي</option>
                    <option value="false">اختياري</option>
                  </select>
                  
                  <select
                    value={filterByPublic === null ? "" : filterByPublic.toString()}
                    onChange={(e) => setFilterByPublic(e.target.value === "" ? null : e.target.value === "true")}
                    className="px-2 py-1 border border-gray-300 rounded text-xs"
                  >
                    <option value="">المستوى</option>
                    <option value="true">عام</option>
                    <option value="false">خاص</option>
                  </select>
                  
                  <select
                    value={filterByFieldType}
                    onChange={(e) => setFilterByFieldType(e.target.value)}
                    className="px-2 py-1 border border-gray-300 rounded text-xs"
                  >
                    <option value="">النوع</option>
                    {fieldTypeOptions.map(option => (
                      <option key={option.value} value={option.value}>
                        {option.icon} {option.label}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              {fieldsLoading ? (
                <div className="flex justify-center py-8">
                  <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-purple-600"></div>
                </div>
              ) : (
                <div className="space-y-2 max-h-96 overflow-y-auto">
                  {/* Ungrouped Fields */}
                  {ungroupedFields.length > 0 && (
                    <div>
                      <h4 className="text-xs font-medium text-gray-700 mb-2 flex items-center">
                        🔧 حقول غير مجمعة ({ungroupedFields.length})
                      </h4>
                      {ungroupedFields.map(field => (
                        <div
                          key={field.fieldId}
                          className="p-2 rounded border border-gray-200 hover:border-gray-300 mb-1"
                        >
                          <div className="flex justify-between items-start">
                            <div className="flex-1">
                              <div className="flex items-center space-x-1 space-x-reverse">
                                <span className="text-sm">{getFieldTypeIcon(field.fieldTypeId)}</span>
                                <h4 className="font-medium text-gray-900 text-xs">{field.displayName}</h4>
                                {field.isRequired && <span className="text-red-500 text-xs">*</span>}
                                {!field.isPublic && <span className="text-gray-500 text-xs">🔒</span>}
                              </div>
                              <p className="text-xs text-gray-600">{field.fieldName}</p>
                            </div>
                            <div className="flex space-x-1 space-x-reverse">
                              <button
                                onClick={() => {
                                  setFieldForm({
                                    unitTypeId: selectedUnitType.id,
                                    fieldTypeId: field.fieldTypeId,
                                    fieldName: field.fieldName,
                                    displayName: field.displayName,
                                    description: field.description,
                                    fieldOptions: field.fieldOptions,
                                    validationRules: field.validationRules,
                                    isRequired: field.isRequired,
                                    isSearchable: field.isSearchable,
                                    isPublic: field.isPublic,
                                    sortOrder: field.sortOrder,
                                    category: field.category,
                                    isForUnits: field.isForUnits,
                                    showInCards: field.showInCards,
                                    isPrimaryFilter: field.isPrimaryFilter,
                                    priority: field.priority,
                                    groupId: field.groupId
                                  });
                                  setSelectedField(field);
                                  setShowFieldModal(true);
                                }}
                                className="text-purple-600 hover:text-purple-800 text-xs"
                              >
                                ✏️
                              </button>
                              <button
                                onClick={() => {
                                  if (confirm("هل أنت متأكد من حذف هذا الحقل؟")) {
                                    deleteUnitTypeField.mutate(field.fieldId);
                                  }
                                }}
                                className="text-red-600 hover:text-red-800 text-xs"
                              >
                                🗑️
                              </button>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}

                  {/* Grouped Fields */}
                  {fieldGroups.map(group => {
                    const groupFields = filteredFields.filter(field => 
                      group.fields.some(f => f.fieldId === field.fieldId)
                    );
                    
                    if (groupFields.length === 0) return null;
                    
                    return (
                      <div key={group.groupId} className="mb-3">
                        <h4 className="text-xs font-medium text-gray-700 mb-2 flex items-center">
                          📁 {group.displayName || group.groupName} ({groupFields.length})
                        </h4>
                        {groupFields.map(field => (
                          <div
                            key={field.fieldId}
                            className="p-2 rounded border border-gray-200 hover:border-gray-300 mb-1 mr-3"
                          >
                            <div className="flex justify-between items-start">
                              <div className="flex-1">
                                <div className="flex items-center space-x-1 space-x-reverse">
                                  <span className="text-sm">{getFieldTypeIcon(field.fieldTypeId)}</span>
                                  <h4 className="font-medium text-gray-900 text-xs">{field.displayName}</h4>
                                  {field.isRequired && <span className="text-red-500 text-xs">*</span>}
                                  {!field.isPublic && <span className="text-gray-500 text-xs">🔒</span>}
                                </div>
                                <p className="text-xs text-gray-600">{field.fieldName}</p>
                              </div>
                              <div className="flex space-x-1 space-x-reverse">
                                <button
                                  onClick={() => {
                                    setFieldForm({
                                      unitTypeId: selectedUnitType.id,
                                      fieldTypeId: field.fieldTypeId,
                                      fieldName: field.fieldName,
                                      displayName: field.displayName,
                                      description: field.description,
                                      fieldOptions: field.fieldOptions,
                                      validationRules: field.validationRules,
                                      isRequired: field.isRequired,
                                      isSearchable: field.isSearchable,
                                      isPublic: field.isPublic,
                                      sortOrder: field.sortOrder,
                                      category: field.category,
                                      isForUnits: field.isForUnits,
                                      showInCards: field.showInCards,
                                      isPrimaryFilter: field.isPrimaryFilter,
                                      priority: field.priority,
                                      groupId: field.groupId
                                    });
                                    setSelectedField(field);
                                    setShowFieldModal(true);
                                  }}
                                  className="text-purple-600 hover:text-purple-800 text-xs"
                                >
                                  ✏️
                                </button>
                                <button
                                  onClick={() => {
                                    if (confirm("هل أنت متأكد من حذف هذا الحقل؟")) {
                                      deleteUnitTypeField.mutate(field.fieldId);
                                    }
                                  }}
                                  className="text-red-600 hover:text-red-800 text-xs"
                                >
                                  🗑️
                                </button>
                              </div>
                            </div>
                          </div>
                        ))}
                      </div>
                    );
                  })}
                  
                  {filteredFields.length === 0 && (
                    <div className="text-center py-8 text-gray-500">
                      <div className="text-3xl mb-2">📝</div>
                      <p className="text-sm">لا توجد حقول</p>
                      <p className="text-xs">قم بإضافة حقل جديد للبدء</p>
                    </div>
                  )}
                </div>
              )}
            </>
          )}
        </div>
      </div>

      {/* Property Type Modal */}
      {showPropertyTypeModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h3 className="text-lg font-semibold mb-4">
              {selectedPropertyType ? 'تعديل نوع الكيان' : 'إضافة نوع كيان جديد'}
            </h3>
            <form onSubmit={(e) => {
              e.preventDefault();
              if (selectedPropertyType) {
                updatePropertyType.mutate({
                  propertyTypeId: selectedPropertyType.id,
                  data: {
                    propertyTypeId: selectedPropertyType.id,
                    ...propertyTypeForm
                  }
                }, {
                  onSuccess: () => {
                    showSuccess('تم تحديث نوع الكيان بنجاح');
                    setShowPropertyTypeModal(false);
                    setSelectedPropertyType(null);
                    resetPropertyTypeForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في تحديث نوع الكيان');
                  }
                });
              } else {
                createPropertyType.mutate(propertyTypeForm, {
                  onSuccess: () => {
                    showSuccess('تم إضافة نوع الكيان بنجاح');
                    setShowPropertyTypeModal(false);
                    resetPropertyTypeForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في إضافة نوع الكيان');
                  }
                });
              }
            }}>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    اسم النوع
                  </label>
                  <input
                    type="text"
                    value={propertyTypeForm.name}
                    onChange={(e) => setPropertyTypeForm({...propertyTypeForm, name: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الأيقونة
                  </label>
                  <div className="flex items-center space-x-2 space-x-reverse">
                    <button
                      type="button"
                      onClick={() => {
                        setIconPickerTarget('property');
                        setShowIconPicker(true);
                      }}
                      className="flex-1 px-3 py-2 border border-gray-300 rounded-md hover:bg-gray-50 flex items-center justify-between"
                    >
                      <span className="flex items-center space-x-2 space-x-reverse">
                        <span className="text-xl">{getIconDisplay(propertyTypeForm.icon)}</span>
                        <span className="text-sm">{propertyTypeForm.icon}</span>
                      </span>
                      <span className="text-gray-400">▼</span>
                    </button>
                  </div>
                  <p className="text-xs text-gray-500 mt-1">
                    استخدم Icons.{propertyTypeForm.icon} في Flutter
                  </p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الوصف
                  </label>
                  <textarea
                    value={propertyTypeForm.description}
                    onChange={(e) => setPropertyTypeForm({...propertyTypeForm, description: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    rows={3}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    المرافق الافتراضية
                  </label>
                  <textarea
                    value={propertyTypeForm.defaultAmenities}
                    onChange={(e) => setPropertyTypeForm({...propertyTypeForm, defaultAmenities: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    rows={2}
                  />
                </div>
              </div>
              <div className="flex justify-end space-x-2 space-x-reverse mt-6">
                <button
                  type="button"
                  onClick={() => {
                    setShowPropertyTypeModal(false);
                    setSelectedPropertyType(null);
                  }}
                  className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  إلغاء
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
                >
                  {selectedPropertyType ? 'تحديث' : 'إضافة'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Unit Type Modal مع الحقول الجديدة */}
      {showUnitTypeModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md max-h-[90vh] overflow-y-auto">
            <h3 className="text-lg font-semibold mb-4">
              {selectedUnitType ? 'تعديل نوع الوحدة' : 'إضافة نوع وحدة جديد'}
            </h3>
            <form onSubmit={(e) => {
              e.preventDefault();
              if (selectedUnitType) {
                updateUnitType.mutate({
                  unitTypeId: selectedUnitType.id,
                  data: {
                    unitTypeId: selectedUnitType.id,
                    name: unitTypeForm.name,
                    maxCapacity: unitTypeForm.maxCapacity,
                    icon: unitTypeForm.icon,
                    isHasAdults: unitTypeForm.isHasAdults,
                    isHasChildren: unitTypeForm.isHasChildren,
                    isMultiDays: unitTypeForm.isMultiDays,
                    isRequiredToDetermineTheHour: unitTypeForm.isRequiredToDetermineTheHour,
                  }
                }, {
                  onSuccess: () => {
                    showSuccess('تم تحديث نوع الوحدة بنجاح');
                    setShowUnitTypeModal(false);
                    setSelectedUnitType(null);
                    resetUnitTypeForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في تحديث نوع الوحدة');
                  }
                });
              } else {
                createUnitType.mutate(unitTypeForm, {
                  onSuccess: () => {
                    showSuccess('تم إضافة نوع الوحدة بنجاح');
                    setShowUnitTypeModal(false);
                    resetUnitTypeForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في إضافة نوع الوحدة');
                  }
                });
              }
            }}>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    اسم النوع
                  </label>
                  <input
                    type="text"
                    value={unitTypeForm.name}
                    onChange={(e) => setUnitTypeForm({...unitTypeForm, name: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
                    required
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الأيقونة
                  </label>
                  <div className="flex items-center space-x-2 space-x-reverse">
                    <button
                      type="button"
                      onClick={() => {
                        setIconPickerTarget('unit');
                        setShowIconPicker(true);
                      }}
                      className="flex-1 px-3 py-2 border border-gray-300 rounded-md hover:bg-gray-50 flex items-center justify-between"
                    >
                      <span className="flex items-center space-x-2 space-x-reverse">
                        <span className="text-xl">{getIconDisplay(unitTypeForm.icon)}</span>
                        <span className="text-sm">{unitTypeForm.icon}</span>
                      </span>
                      <span className="text-gray-400">▼</span>
                    </button>
                  </div>
                  <p className="text-xs text-gray-500 mt-1">
                    استخدم Icons.{unitTypeForm.icon} في Flutter
                  </p>
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    السعة القصوى
                  </label>
                  <input
                    type="number"
                    min="1"
                    value={unitTypeForm.maxCapacity}
                    onChange={(e) => setUnitTypeForm({...unitTypeForm, maxCapacity: parseInt(e.target.value)})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
                    required
                  />
                </div>

                {/* الحقول الجديدة */}
                <div className="border-t pt-4">
                  <h4 className="text-sm font-medium text-gray-700 mb-3">خصائص نوع الوحدة</h4>
                  
                  <div className="space-y-3">
                    <label className="flex items-center">
                      <input
                        type="checkbox"
                        checked={unitTypeForm.isHasAdults}
                        onChange={(e) => setUnitTypeForm({...unitTypeForm, isHasAdults: e.target.checked})}
                        className="ml-2 rounded text-green-600 focus:ring-green-500"
                      />
                      <div className="flex items-center">
                        <span className="text-sm ml-1">👨</span>
                        <span className="text-sm font-medium text-gray-700">يحتوي على بالغين</span>
                      </div>
                      <span className="text-xs text-gray-500 mr-2">(تفعيل حقل عدد البالغين في الحجز)</span>
                    </label>
                    
                    <label className="flex items-center">
                      <input
                        type="checkbox"
                        checked={unitTypeForm.isHasChildren}
                        onChange={(e) => setUnitTypeForm({...unitTypeForm, isHasChildren: e.target.checked})}
                        className="ml-2 rounded text-green-600 focus:ring-green-500"
                      />
                      <div className="flex items-center">
                        <span className="text-sm ml-1">👶</span>
                        <span className="text-sm font-medium text-gray-700">يحتوي على أطفال</span>
                      </div>
                      <span className="text-xs text-gray-500 mr-2">(تفعيل حقل عدد الأطفال في الحجز)</span>
                    </label>
                    
                    <label className="flex items-center">
                      <input
                        type="checkbox"
                        checked={unitTypeForm.isMultiDays}
                        onChange={(e) => setUnitTypeForm({...unitTypeForm, isMultiDays: e.target.checked})}
                        className="ml-2 rounded text-green-600 focus:ring-green-500"
                      />
                      <div className="flex items-center">
                        <span className="text-sm ml-1">📅</span>
                        <span className="text-sm font-medium text-gray-700">متعدد الأيام</span>
                      </div>
                      <span className="text-xs text-gray-500 mr-2">(السماح بالحجز لعدة أيام)</span>
                    </label>
                    
                    <label className="flex items-center">
                      <input
                        type="checkbox"
                        checked={unitTypeForm.isRequiredToDetermineTheHour}
                        onChange={(e) => setUnitTypeForm({...unitTypeForm, isRequiredToDetermineTheHour: e.target.checked})}
                        className="ml-2 rounded text-green-600 focus:ring-green-500"
                      />
                      <div className="flex items-center">
                        <span className="text-sm ml-1">⏰</span>
                        <span className="text-sm font-medium text-gray-700">يتطلب تحديد الساعة</span>
                      </div>
                      <span className="text-xs text-gray-500 mr-2">(إلزام تحديد الوقت عند الحجز)</span>
                    </label>
                  </div>
                </div>
              </div>
              
              <div className="flex justify-end space-x-2 space-x-reverse mt-6">
                <button
                  type="button"
                  onClick={() => {
                    setShowUnitTypeModal(false);
                    setSelectedUnitType(null);
                  }}
                  className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  إلغاء
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700"
                >
                  {selectedUnitType ? 'تحديث' : 'إضافة'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Field Group Modal */}
      {showFieldGroupModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h3 className="text-lg font-semibold mb-4">
              {selectedFieldGroup ? 'تعديل مجموعة الحقول' : 'إضافة مجموعة حقول جديدة'}
            </h3>
            <form onSubmit={(e) => {
              e.preventDefault();
              if (selectedFieldGroup) {
                updateFieldGroup.mutate({
                  groupId: selectedFieldGroup.groupId,
                  ...fieldGroupForm
                }, {
                  onSuccess: () => {
                    showSuccess('تم تحديث مجموعة الحقول بنجاح');
                    setShowFieldGroupModal(false);
                    setSelectedFieldGroup(null);
                    resetFieldGroupForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في تحديث مجموعة الحقول');
                  }
                });
              } else {
                createFieldGroup.mutate(fieldGroupForm, {
                  onSuccess: () => {
                    showSuccess('تم إضافة مجموعة الحقول بنجاح');
                    setShowFieldGroupModal(false);
                    resetFieldGroupForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في إضافة مجموعة الحقول');
                  }
                });
              }
            }}>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    اسم المجموعة
                  </label>
                  <input
                    type="text"
                    value={fieldGroupForm.groupName}
                    onChange={(e) => setFieldGroupForm({...fieldGroupForm, groupName: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-yellow-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الاسم المعروض
                  </label>
                  <input
                    type="text"
                    value={fieldGroupForm.displayName}
                    onChange={(e) => setFieldGroupForm({...fieldGroupForm, displayName: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-yellow-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الوصف
                  </label>
                  <textarea
                    value={fieldGroupForm.description}
                    onChange={(e) => setFieldGroupForm({...fieldGroupForm, description: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-yellow-500"
                    rows={3}
                  />
                </div>
                <div className="flex items-center space-x-4 space-x-reverse">
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      checked={fieldGroupForm.isCollapsible}
                      onChange={(e) => setFieldGroupForm({...fieldGroupForm, isCollapsible: e.target.checked})}
                      className="mr-2"
                    />
                    قابل للطي
                  </label>
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      checked={fieldGroupForm.isExpandedByDefault}
                      onChange={(e) => setFieldGroupForm({...fieldGroupForm, isExpandedByDefault: e.target.checked})}
                      className="mr-2"
                    />
                    مفتوح افتراضياً
                  </label>
                </div>
              </div>
              <div className="flex justify-end space-x-2 space-x-reverse mt-6">
                <button
                  type="button"
                  onClick={() => {
                    setShowFieldGroupModal(false);
                    setSelectedFieldGroup(null);
                  }}
                  className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  إلغاء
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-yellow-600 text-white rounded-md hover:bg-yellow-700"
                >
                  {selectedFieldGroup ? 'تحديث' : 'إضافة'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Field Modal */}
      {showFieldModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <h3 className="text-lg font-semibold mb-4">
              {selectedField ? 'تعديل الحقل' : 'إضافة حقل جديد'}
            </h3>
            <form onSubmit={(e) => {
              e.preventDefault();
              if (selectedField) {
                updateUnitTypeField.mutate({
                  fieldId: selectedField.fieldId,
                  data: {
                    fieldId: selectedField.fieldId,
                    ...fieldForm,
                    groupId: selectedFieldGroup?.groupId || ''
                  }
                }, {
                  onSuccess: () => {
                    showSuccess('تم تحديث الحقل بنجاح');
                    setShowFieldModal(false);
                    setSelectedField(null);
                    resetFieldForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في تحديث الحقل');
                  }
                });
              } else {
                createUnitTypeField.mutate(fieldForm, {
                  onSuccess: () => {
                    showSuccess('تم إضافة الحقل بنجاح');
                    setShowFieldModal(false);
                    resetFieldForm();
                  },
                  onError: (error: any) => {
                    showError(error.response?.data?.message || 'فشل في إضافة الحقل');
                  }
                });
              }
            }}>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    نوع الحقل
                  </label>
                  <select
                    value={fieldForm.fieldTypeId}
                    onChange={(e) => setFieldForm({...fieldForm, fieldTypeId: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
                    required
                  >
                    <option value="">اختر نوع الحقل</option>
                    {fieldTypeOptions.map(option => (
                      <option key={option.value} value={option.value}>
                        {option.icon} {option.label}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    اسم الحقل
                  </label>
                  <input
                    type="text"
                    value={fieldForm.fieldName}
                    onChange={(e) => setFieldForm({...fieldForm, fieldName: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الاسم المعروض
                  </label>
                  <input
                    type="text"
                    value={fieldForm.displayName}
                    onChange={(e) => setFieldForm({...fieldForm, displayName: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الوصف
                  </label>
                  <textarea
                    value={fieldForm.description}
                    onChange={(e) => setFieldForm({...fieldForm, description: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
                    rows={3}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الفئة
                  </label>
                  <input
                    type="text"
                    value={fieldForm.category}
                    onChange={(e) => setFieldForm({...fieldForm, category: e.target.value})}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
                  />
                </div>
                {/* Input for options when field type is select or multiselect */}
                {(fieldForm.fieldTypeId === 'select' || fieldForm.fieldTypeId === 'multiselect') && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      خيارات {fieldForm.fieldTypeId === 'select' ? 'القائمة المنسدلة' : 'التحديد المتعدد'}
                    </label>
                    <TagInput
                      value={((fieldForm.fieldOptions as any).options || []).join(',')}
                      onChange={(val) => setFieldForm(prev => ({
                        ...prev,
                        fieldOptions: { options: val.split(',').map(s => s.trim()).filter(Boolean) }
                      }))}
                      placeholder="أدخل خيار واضغط Enter أو استخدم الفاصلة للفصل"
                      className="w-full"
                    />
                  </div>
                )}
                {/* Checkbox grid for field settings */}
                <div className="grid grid-cols-2 gap-4">
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      checked={fieldForm.isRequired}
                      onChange={(e) => setFieldForm({...fieldForm, isRequired: e.target.checked})}
                      className="mr-2"
                    />
                    مطلوب
                  </label>
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      checked={fieldForm.isSearchable}
                      onChange={(e) => setFieldForm({...fieldForm, isSearchable: e.target.checked})}
                      className="mr-2"
                    />
                    قابل للبحث
                  </label>
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      checked={fieldForm.isPublic}
                      onChange={(e) => setFieldForm({...fieldForm, isPublic: e.target.checked})}
                      className="mr-2"
                    />
                    عام
                  </label>
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      checked={fieldForm.isForUnits}
                      onChange={(e) => setFieldForm({...fieldForm, isForUnits: e.target.checked})}
                      className="mr-2"
                    />
                    للوحدات
                  </label>
                </div>
              </div>
              <div className="flex justify-end space-x-2 space-x-reverse mt-6">
                <button
                  type="button"
                  onClick={() => {
                    setShowFieldModal(false);
                    setSelectedField(null);
                  }}
                  className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  إلغاء
                </button>
                <button
                  type="button"
                  onClick={handleSaveField}
                  className="px-4 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700"
                >
                  {selectedField ? 'تحديث' : 'إضافة'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Icon Picker Modal */}
      {showIconPicker && (
        <IconPicker
          selectedIcon={iconPickerTarget === 'property' ? propertyTypeForm.icon : unitTypeForm.icon}
          onSelectIcon={handleIconSelect}
          onClose={() => {
            setShowIconPicker(false);
            setIconPickerTarget(null);
          }}
        />
      )}

    </div>
  );
};

export default AdminPropertyAndUnitTypes;