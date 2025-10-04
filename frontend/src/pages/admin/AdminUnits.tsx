import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAdminUnits } from '../../hooks/useAdminUnits';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import { useAdminUnitTypes } from '../../hooks/useAdminUnitTypes';
import { useAdminUnitTypeFieldsByUnitType } from '../../hooks/useAdminUnitTypeFieldsByUnitType';
import { useAdminUnitTypesByPropertyType } from '../../hooks/useAdminUnitTypesByPropertyType';
import { useCurrencies } from '../../hooks/useCurrencies';
import DataTable, { type Column } from '../../components/common/DataTable';
import CardView from '../../components/common/CardView';
import MapView from '../../components/common/MapView';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import ViewToggle, { type ViewType } from '../../components/common/ViewToggle';
import Modal from '../../components/common/Modal';
import DynamicFieldsForm from '../../components/forms/DynamicFieldsForm';
import ImageUpload from '../../components/inputs/ImageUpload';
import CurrencyInput from '../../components/inputs/CurrencyInput';
import TagInput from '../../components/inputs/TagInput';
import type { 
  UnitDto, 
  CreateUnitCommand, 
  UpdateUnitCommand,
  MoneyDto,
  PricingMethod
} from '../../types/unit.types';
import type { UnitTypeDto } from '../../types/unit-type.types';
import type { UnitTypeFieldDto } from '../../types/unit-type-field.types';
import type { FieldValueDto, UnitFieldValueDto } from '../../types/unit-field-value.types';
import type { PropertyImageDto } from '../../types/property-image.types';
import AdminSectionsService from '../../services/admin-sections.service';
import { AdminUnitsService } from '../../services/admin-units.service';
import { SectionTarget } from '../../types/enums';

// Extend UnitDto to include coordinates for map view
interface UnitWithLocation extends UnitDto {
  latitude?: number;
  longitude?: number;
  address?: string;
}

const AdminUnits = () => {
  const navigate = useNavigate();
  const handleOpenGallery = (unit: UnitDto) => {
    navigate(
      `/admin/unit-images/${unit.propertyId}/${unit.id}`,
      { state: { propertyName: unit.propertyName, unitName: unit.name } }
    );
  };

  // State for view and search
  const [currentView, setCurrentView] = useState<ViewType>('table');
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({
    propertyId: '',
    unitTypeId: '',
    isAvailable: undefined,
    minPrice: '',
    maxPrice: '',
    pricingMethod: '',
    checkInDate: '',
    checkOutDate: ''
  });

  // State for modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [selectedUnit, setSelectedUnit] = useState<UnitDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);

  // Assign to sections modal state
  const [showAssignSectionsModal, setShowAssignSectionsModal] = useState(false);
  const [availableUnitSections, setAvailableUnitSections] = useState<any[]>([]);
  const [selectedSectionIds, setSelectedSectionIds] = useState<string[]>([]);

  // State for unit type settings
  const [selectedUnitTypeDetails, setSelectedUnitTypeDetails] = useState<UnitTypeDto | null>(null);
  const [editUnitTypeDetails, setEditUnitTypeDetails] = useState<UnitTypeDto | null>(null);

  // State for forms - تحديث لإضافة حقول السعة
  const [createForm, setCreateForm] = useState<CreateUnitCommand>({
    propertyId: '',
    unitTypeId: '',
    name: '',
    basePrice: { amount: 0, currency: 'YER' },
    customFeatures: '',
    pricingMethod: 'Daily' as PricingMethod,
    fieldValues: [],
    images: [],
    adultCapacity: undefined,
    childrenCapacity: undefined,
  });

  const [editForm, setEditForm] = useState<UpdateUnitCommand>({
    unitId: '',
    name: '',
    basePrice: { amount: 0, currency: 'YER' },
    customFeatures: '',
    pricingMethod: 'Dynamic' as PricingMethod,
    fieldValues: [],
    images: [],
    adultCapacity: undefined,
    childrenCapacity: undefined,
  });

  // State for dynamic fields
  const [createDynamicFields, setCreateDynamicFields] = useState<Record<string, any>>({});
  const [editDynamicFields, setEditDynamicFields] = useState<Record<string, any>>({});

  // Build query params
  const queryParams = {
    pageNumber: currentPage,
    pageSize,
    nameContains: searchTerm || undefined,
    propertyId: filterValues.propertyId || undefined,
    unitTypeId: filterValues.unitTypeId || undefined,
    isAvailable: filterValues.isAvailable,
    minPrice: filterValues.minPrice || undefined,
    maxPrice: filterValues.maxPrice || undefined,
    pricingMethod: filterValues.pricingMethod || undefined,
    checkInDate: filterValues.checkInDate || undefined,
    checkOutDate: filterValues.checkOutDate || undefined
  };

  // هوكات لإدارة البيانات
  const { unitsData, isLoading: isLoadingUnits, error: unitsError, createUnit, updateUnit, deleteUnit } = useAdminUnits(queryParams);
  const { propertiesData, isLoading: isLoadingProperties } = useAdminProperties({
    pageNumber: 1,
    pageSize: 100
  });
  const { data: unitTypesData, isLoading: isLoadingUnitTypes } = useAdminUnitTypes({
    pageNumber: 1,
    pageSize: 100
  });
  
  // Fetch unit types based on selected property's type
  const selectedCreateProperty = propertiesData?.items.find(p => p.id === createForm.propertyId);
  const { unitTypesData: createUnitTypesData, isLoading: isLoadingCreateUnitTypes } = useAdminUnitTypesByPropertyType({
    propertyTypeId: selectedCreateProperty?.typeId || '',
    pageNumber: 1,
    pageSize: 100,
  });
  
  // جلب الحقول الديناميكية للوحدة المحددة في النموذج
  const { unitTypeFieldsData: createFields } = useAdminUnitTypeFieldsByUnitType({
    unitTypeId: createForm.unitTypeId,
    isPublic: true
  });
  
  const { unitTypeFieldsData: editFields } = useAdminUnitTypeFieldsByUnitType({
    unitTypeId: selectedUnit?.unitTypeId || '',
    isPublic: true
  });
  
  // جلب قائمة العملات من الباك إند
  const { currencies, loading: currenciesLoading, error: currenciesError } = useCurrencies();
  const currencyCodes = currenciesLoading ? [] : currencies.map(c => c.code);

  // جلب تفاصيل نوع الوحدة المحدد عند تغيير نوع الوحدة في نموذج الإنشاء
  useEffect(() => {
    if (createForm.unitTypeId && createUnitTypesData?.items) {
      const unitType = createUnitTypesData.items.find(t => t.id === createForm.unitTypeId);
      setSelectedUnitTypeDetails(unitType || null);
      
      // إعادة تعيين قيم السعة إذا كانت غير مطلوبة
      if (unitType) {
        if (!unitType.isHasAdults) {
          setCreateForm(prev => ({ ...prev, adultCapacity: undefined }));
        }
        if (!unitType.isHasChildren) {
          setCreateForm(prev => ({ ...prev, childrenCapacity: undefined }));
        }
      }
    } else {
      setSelectedUnitTypeDetails(null);
    }
  }, [createForm.unitTypeId, createUnitTypesData]);

  // جلب تفاصيل نوع الوحدة للتعديل
  useEffect(() => {
    if (selectedUnit && unitTypesData?.items) {
      const unitType = unitTypesData.items.find(t => t.id === selectedUnit.unitTypeId);
      setEditUnitTypeDetails(unitType || null);
    } else {
      setEditUnitTypeDetails(null);
    }
  }, [selectedUnit, unitTypesData]);

  // تحديث القيم الديناميكية عند تغيير نوع الوحدة
  useEffect(() => {
    if (createForm.unitTypeId && createFields?.length) {
      setCreateDynamicFields({});
    }
  }, [createForm.unitTypeId, createFields]);

  useEffect(() => {
    if (selectedUnit && editFields?.length) {
      // تحديد القيم الحالية للحقول الديناميكية
      const currentValues: Record<string, any> = {};
      selectedUnit.fieldValues?.forEach(value => {
        currentValues[value.fieldId] = value.fieldValue;
      });
      setEditDynamicFields(currentValues);
    }
  }, [selectedUnit, editFields]);

  // Helper to validate dynamic fields before submitting
  const validateDynamicFields = (fields: UnitTypeFieldDto[], values: Record<string, any>) => {
    const errors: string[] = [];
    fields.forEach(field => {
      const rawValue = values[field.fieldId];
      const val = rawValue !== undefined && rawValue !== null ? rawValue : '';
      // Required check
      if (field.isRequired) {
        if (field.fieldTypeId === 'multiselect') {
          if (!Array.isArray(val) || val.length === 0) {
            errors.push(`${field.displayName} مطلوب.`);
          }
        } else if (String(val).trim() === '') {
          errors.push(`${field.displayName} مطلوب.`);
        }
      }
      // Type-specific checks
      switch (field.fieldTypeId) {
        case 'text':
        case 'textarea':
          const length = String(val).length;
          if (field.validationRules.minLength && length < field.validationRules.minLength) {
            errors.push(`${field.displayName} يجب أن يكون طوله على الأقل ${field.validationRules.minLength} أحرف.`);
          }
          if (field.validationRules.maxLength && length > field.validationRules.maxLength) {
            errors.push(`${field.displayName} يجب ألا يزيد طوله عن ${field.validationRules.maxLength} أحرف.`);
          }
          if (field.validationRules.pattern) {
            const regex = new RegExp(field.validationRules.pattern);
            if (!regex.test(String(val))) {
              errors.push(`${field.displayName} غير صالح.`);
            }
          }
          break;
        case 'number':
        case 'currency':
        case 'percentage':
        case 'range':
          const num = parseFloat(val) || 0;
          if (field.validationRules.min != null && num < field.validationRules.min) {
            errors.push(`${field.displayName} يجب أن يكون ≥ ${field.validationRules.min}.`);
          }
          if (field.validationRules.max != null && num > field.validationRules.max) {
            errors.push(`${field.displayName} يجب أن يكون ≤ ${field.validationRules.max}.`);
          }
          break;
        case 'select':
          const options = field.fieldOptions.options || [];
          if (val && !options.includes(val)) {
            errors.push(`${field.displayName} غير صالح.`);
          }
          break;
        case 'multiselect':
          const moptions = field.fieldOptions.options || [];
          if (Array.isArray(val)) {
            val.forEach((item: string) => {
              if (!moptions.includes(item)) {
                errors.push(`${field.displayName} يحتوي على قيمة غير صالحة.`);
              }
            });
          }
          break;
      }
    });
    return errors;
  };

  // Helper function to validate capacity fields
  const validateCapacityFields = (form: CreateUnitCommand | UpdateUnitCommand, unitType: UnitTypeDto | null) => {
    const errors: string[] = [];
    
    if (unitType?.isHasAdults) {
      if (!form.adultCapacity || form.adultCapacity <= 0) {
        errors.push('عدد البالغين مطلوب ويجب أن يكون أكبر من صفر');
      }
    }
    
    if (unitType?.isHasChildren) {
      if (!form.childrenCapacity || form.childrenCapacity < 0) {
        errors.push('عدد الأطفال مطلوب ويجب أن يكون صفر أو أكثر');
      }
    }
    
    return errors;
  };

  // Helper functions
  const resetCreateForm = () => {
    setCreateForm({
      propertyId: '',
      unitTypeId: '',
      name: '',
      basePrice: { amount: 0, currency: 'YER' },
      customFeatures: '',
      pricingMethod: 'Daily' as PricingMethod,
      fieldValues: [],
      images: [],
      adultCapacity: undefined,
      childrenCapacity: undefined,
    });
    setCreateDynamicFields({});
    setSelectedUnitTypeDetails(null);
  };

  const handleEdit = (unit: UnitDto) => {
    setSelectedUnit(unit);
    setEditForm({
      unitId: unit.id,
      name: unit.name,
      basePrice: unit.basePrice,
      customFeatures: unit.customFeatures,
      pricingMethod: unit.pricingMethod,
      fieldValues: unit.fieldValues?.map(fv => ({
        fieldId: fv.fieldId,
        fieldValue: fv.fieldValue
      })) || [],
      images: unit.images?.map(img => img.url) || [],
      adultCapacity: unit.adultCapacity,
      childrenCapacity: unit.childrenCapacity,
    });
    setShowEditModal(true);
  };

  const handleViewDetails = (unit: UnitDto) => {
    setSelectedUnit(unit);
    setShowDetailsModal(true);
  };

  const handleDelete = (unit: UnitDto) => {
    if (confirm(`هل أنت متأكد من حذف الوحدة "${unit.name}"؟ هذا الإجراء لا يمكن التراجع عنه.`)) {
      deleteUnit.mutate(unit.id);
    }
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      propertyId: '',
      unitTypeId: '',
      isAvailable: undefined,
      minPrice: '',
      maxPrice: '',
      pricingMethod: '',
      checkInDate: '',
      checkOutDate: ''
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  const openAssignSections = async (unit: UnitDto) => {
    setSelectedUnit(unit);
    setSelectedSectionIds([]);
    const res = await AdminSectionsService.getSections({ pageNumber: 1, pageSize: 100, target: SectionTarget.UNITS });
    setAvailableUnitSections(res.items as any);
    setShowAssignSectionsModal(true);
  };

  const confirmAssignSections = async () => {
    if (!selectedUnit) return;
    await AdminUnitsService.addUnitToSections(selectedUnit.id, selectedSectionIds);
    setShowAssignSectionsModal(false);
  };

  // Filter options
  const filterOptions: FilterOption[] = [
    {
      key: 'propertyId',
      label: 'الكيان',
      type: 'select',
      options: propertiesData?.items.map(p => ({ value: p.id, label: p.name })) ?? [],
    },
    {
      key: 'unitTypeId',
      label: 'نوع الوحدة',
      type: 'select',
      options: unitTypesData?.items.map(t => ({ value: t.id, label: t.name })) ?? [],
    },
    {
      key: 'isAvailable',
      label: 'متاحة',
      type: 'boolean',
    },
    {
      key: 'minPrice',
      label: 'الحد الأدنى للسعر',
      type: 'number',
      placeholder: 'أدخل الحد الأدنى',
    },
    {
      key: 'maxPrice',
      label: 'الحد الأقصى للسعر',
      type: 'number',
      placeholder: 'أدخل الحد الأقصى',
    },
    {
      key: 'pricingMethod',
      label: 'طريقة التسعير',
      type: 'select',
      options: [
        { value: 'Hourly', label: 'بالساعة' },
        { value: 'Daily', label: 'يومي' },
        { value: 'Weekly', label: 'أسبوعي' },
        { value: 'Monthly', label: 'شهري' },
      ],
    },
    {
      key: 'checkInDate',
      label: 'متاح من',
      type: 'custom',
      render: (value: string, onChange: (val: any) => void) => (
        <input
          type="datetime-local"
          value={value}
          onChange={e => onChange(e.target.value)}
          className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
        />
      )
    },
    {
      key: 'checkOutDate',
      label: 'متاح إلى',
      type: 'custom',
      render: (value: string, onChange: (val: any) => void) => (
        <input
          type="datetime-local"
          value={value}
          onChange={e => onChange(e.target.value)}
          className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
        />
      )
    }
  ];

  // Table columns - إضافة عمود السعة
  const columns: Column<UnitDto>[] = [
    {
      key: 'name',
      title: 'اسم الوحدة',
      sortable: true,
      render: (value: string, record: UnitDto) => (
        <div className="flex flex-col">
          <span className="font-medium text-gray-900">{value}</span>
          <span className="text-sm text-gray-500">{record.unitTypeName}</span>
        </div>
      ),
    },
    {
      key: 'propertyName',
      title: 'الكيان',
      sortable: true,
    },
    {
      key: 'capacity',
      title: 'السعة',
      render: (value: any, record: UnitDto) => {
        const capacities: string[] = [];
        if (record.adultCapacity) capacities.push(`👨 ${record.adultCapacity}`);
        if (record.childrenCapacity !== undefined) capacities.push(`👶 ${record.childrenCapacity}`);
        return capacities.length > 0 ? (
          <div className="flex gap-2">
            {capacities.map((cap, idx) => (
              <span key={idx} className="text-sm">{cap}</span>
            ))}
          </div>
        ) : '-';
      },
    },
    {
      key: 'basePrice',
      title: 'السعر الأساسي',
      render: (value: MoneyDto) => (
        <div className="text-right">
          <span className="font-medium">{value.amount}</span>
          <span className="text-sm text-gray-500 mr-1">{value.currency}</span>
        </div>
      ),
    },
    {
      key: 'pricingMethod',
      title: 'طريقة التسعير',
      render: (value: PricingMethod) => {
        const methodLabels = {
          Hourly: 'بالساعة',
          Daily: 'يومي',
          Weekly: 'أسبوعي',
          Monthly: 'شهري',
        };
        return methodLabels[value] || value;
      },
    },
    {
      key: 'isAvailable',
      title: 'متاحة',
      render: (value: boolean) => (
        <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
          value ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
        }`}>
          {value ? 'متاحة' : 'غير متاحة'}
        </span>
      ),
    },
  ];

  // Table actions
  const tableActions = [
    {
      label: 'إدارة الإتاحة والتسعير',
      icon: '📅',
      color: 'green' as const,
      onClick: (unit: UnitDto) => {
        navigate(`/admin/units/${unit.id}/availability-pricing`, { state: { unitName: unit.name } });
      }
    },
    {
      label: 'عرض التفاصيل',
      icon: '👁️',
      color: 'blue' as const,
      onClick: handleViewDetails,
    },
    {
      label: 'معرض الصور',
      icon: '🖼️',
      color: 'blue' as const,
      onClick: handleOpenGallery,
    },
    {
      label: 'تعديل',
      icon: '✏️',
      color: 'yellow' as const,
      onClick: handleEdit,
    },
    {
      label: 'إسناد للأقسام',
      icon: '📚',
      color: 'yellow' as const,
      onClick: (unit: UnitDto) => openAssignSections(unit),
    },
    {
      label: 'حذف',
      icon: '🗑️',
      color: 'red' as const,
      onClick: handleDelete,
    },
  ];

  // Helper function to get main image for unit
  const getMainUnitImage = (images?: PropertyImageDto[]) => {
    if (!images || images.length === 0) return null;
    const mainImage = images.find(img => img.isMain);
    return mainImage || images[0];
  };

  // Card renderer for card view
  const renderUnitCard = (unit: UnitDto) => {
    const mainImage = getMainUnitImage(unit.images);
    
    return (
      <div className="bg-white border border-gray-200 rounded-lg shadow-sm hover:shadow-md transition-shadow overflow-hidden">
        {/* Unit Image */}
        <div className="relative h-48 bg-gray-200">
          {mainImage ? (
            <img
              src={mainImage.url}
              alt={mainImage.altText || unit.name}
              className="w-full h-full object-cover"
              onError={(e) => {
                const target = e.target as HTMLImageElement;
                target.style.display = 'none';
                target.nextElementSibling?.classList.remove('hidden');
              }}
            />
          ) : null}
          {/* Fallback placeholder */}
          <div className={`w-full h-full flex items-center justify-center bg-gray-100 ${mainImage ? 'hidden' : ''}`}>
            <div className="text-center">
              <span className="text-4xl text-gray-400">🏠</span>
              <p className="text-sm text-gray-500 mt-2">لا توجد صورة</p>
            </div>
          </div>
          
          {/* Availability badge overlay */}
          <div className="absolute top-3 right-3">
            <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${
              unit.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
            }`}>
              {unit.isAvailable ? 'متاحة' : 'غير متاحة'}
            </span>
          </div>
          
          {/* Image count indicator */}
          {unit.images && unit.images.length > 0 && (
            <div className="absolute bottom-3 left-3 bg-black bg-opacity-60 text-white px-2 py-1 rounded text-xs">
              📸 {unit.images.length} صورة
            </div>
          )}
        </div>

        <div className="p-6">
          <div className="mb-4">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">{unit.name}</h3>
            <p className="text-sm text-gray-600">{unit.unitTypeName}</p>
          </div>
          
          <div className="space-y-2 mb-4">
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">الكيان:</span>
              <span className="text-sm text-gray-900">{unit.propertyName}</span>
            </div>
            
            {/* عرض السعة إذا كانت موجودة */}
            {(unit.adultCapacity || unit.childrenCapacity !== undefined) && (
              <div className="flex justify-between">
                <span className="text-sm text-gray-500">السعة:</span>
                <div className="flex gap-2">
                  {unit.adultCapacity && (
                    <span className="text-sm text-gray-900">👨 {unit.adultCapacity}</span>
                  )}
                  {unit.childrenCapacity !== undefined && (
                    <span className="text-sm text-gray-900">👶 {unit.childrenCapacity}</span>
                  )}
                </div>
              </div>
            )}
            
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">السعر:</span>
              <span className="text-sm text-gray-900 font-medium">
                {unit.basePrice.amount} {unit.basePrice.currency}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-sm text-gray-500">طريقة التسعير:</span>
              <span className="text-sm text-gray-900">
                {filterOptions.find(opt => opt.key === 'pricingMethod')?.options?.find(
                  option => option.value === unit.pricingMethod
                )?.label || unit.pricingMethod}
              </span>
            </div>
          </div>

          <div className="flex gap-2">
            <button
              onClick={() => handleViewDetails(unit)}
              className="flex-1 px-3 py-2 bg-blue-50 text-blue-700 text-sm font-medium rounded hover:bg-blue-100 transition-colors"
            >
              عرض التفاصيل
            </button>
            <button
              onClick={() => handleEdit(unit)}
              className="px-3 py-2 bg-gray-50 text-gray-700 text-sm font-medium rounded hover:bg-gray-100 transition-colors"
            >
              ✏️
            </button>
            <button
              onClick={() => openAssignSections(unit)}
              className="px-3 py-2 bg-indigo-50 text-indigo-700 text-sm font-medium rounded hover:bg-indigo-100 transition-colors"
            >
              📚
            </button>
            <button
              onClick={() => handleDelete(unit)}
              className="px-3 py-2 bg-red-50 text-red-700 text-sm font-medium rounded hover:bg-red-100 transition-colors"
            >
              🗑️
            </button>
          </div>
        </div>
      </div>
    );
  };

  // ربط الوحدات بإحداثيات الكيانات الحقيقية
  const unitsWithLocation: UnitWithLocation[] = (unitsData?.items || []).map(unit => {
    const property = propertiesData?.items.find(p => p.id === unit.propertyId);
    return {
      ...unit,
      latitude: property?.latitude,
      longitude: property?.longitude,
      address: property?.address,
    };
  });

  if (unitsError) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-8 text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">خطأ في تحميل البيانات</h2>
        <p className="text-gray-600">حدث خطأ أثناء تحميل بيانات الوحدات. يرجى المحاولة مرة أخرى.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة الوحدات</h1>
            <p className="text-gray-600 mt-1">
              إدارة جميع الوحدات في النظام مع 3 طرق عرض مختلفة
            </p>
          </div>
          <div className="flex gap-3">
            <ViewToggle
              currentView={currentView}
              onViewChange={setCurrentView}
              availableViews={['table', 'cards', 'map']}
            />
            <button
              onClick={() => setShowCreateModal(true)}
              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              ➕ إضافة وحدة جديدة
            </button>
          </div>
        </div>
      </div>

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في الوحدات..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Data Views */}
      {currentView === 'table' && (
        <DataTable
          data={unitsData?.items || []}
          columns={columns}
          loading={isLoadingUnits}
          pagination={{ 
            current: unitsData?.pageNumber || 1, 
            pageSize: unitsData?.pageSize || pageSize, 
            total: unitsData?.totalCount || 0, 
            onChange: (p, s) => { 
              setCurrentPage(p); 
              setPageSize(s); 
            } 
          }}
          actions={tableActions}
        />
      )}

      {currentView === 'cards' && (
        <CardView
          data={unitsData?.items || []}
          loading={isLoadingUnits}
          renderCard={renderUnitCard}
          emptyMessage="لا توجد وحدات للعرض"
          emptyIcon="🏠"
          columns={3}
          pagination={{
            current: currentPage,
            total: unitsData?.totalCount || 0,
            pageSize,
            onChange: (page, size) => {
              setCurrentPage(page);
              setPageSize(size);
            },
          }}
        />
      )}

      {currentView === 'map' && (
        <MapView
          markers={unitsWithLocation.map(unit => ({
            id: unit.id,
            name: unit.name,
            address: unit.propertyName,
            description: `${unit.unitTypeName} - ${unit.basePrice.amount} ${unit.basePrice.currency}`,
            coordinates: unit.latitude && unit.longitude ? {
              latitude: unit.latitude,
              longitude: unit.longitude
            } : undefined,
            type: 'unit' as const,
            color: unit.isAvailable ? '#10B981' : '#EF4444',
            isAvailable: unit.isAvailable,
            price: {
              amount: unit.basePrice.amount,
              currency: unit.basePrice.currency
            }
          })).filter(marker => marker.coordinates)}
          onMarkerClick={(marker) => {
            const unit = unitsWithLocation.find(u => u.id === marker.id);
            if (unit) handleViewDetails(unit);
          }}
          emptyMessage="لا توجد وحدات بمواقع محددة لعرضها على الخريطة"
          height="600px"
          pagination={{
            current: currentPage,
            total: unitsData?.totalCount || 0,
            pageSize,
            onChange: (page, size) => {
              setCurrentPage(page);
              setPageSize(size);
            },
          }}
        />
      )}

      {/* Create Unit Modal */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => {
          setShowCreateModal(false);
          resetCreateForm();
        }}
        title="إضافة وحدة جديدة"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowCreateModal(false);
                resetCreateForm();
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => {
                // التحقق من الحقول الإجبارية
                const capacityErrors = validateCapacityFields(createForm, selectedUnitTypeDetails);
                if (capacityErrors.length) {
                  alert(capacityErrors.join('\n'));
                  return;
                }
                
                const createErrors = validateDynamicFields(createFields || [], createDynamicFields);
                if (createErrors.length) {
                  alert(createErrors.join('\n'));
                  return;
                }
                const fieldValues: FieldValueDto[] = Object.entries(createDynamicFields).map(([fieldId, value]) => ({
                  fieldId,
                  fieldValue: Array.isArray(value) ? JSON.stringify(value) : String(value)
                }));
                
                const unitData = {
                  ...createForm,
                  fieldValues
                };
                
                createUnit.mutate(unitData, {
                  onSuccess: () => {
                    setShowCreateModal(false);
                    resetCreateForm();
                  },
                });
              }}
              disabled={createUnit.status === 'pending'}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {createUnit.status === 'pending' ? 'جارٍ الإضافة...' : 'إضافة'}
            </button>
          </div>
        }
      >
        <div className="space-y-6">
          {/* القسم الأساسي */}
          <div className="bg-gray-50 rounded-lg p-4">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <span className="ml-2">📋</span> المعلومات الأساسية
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  الكيان *
                </label>
                <select
                  value={createForm.propertyId}
                  onChange={(e) => setCreateForm(prev => ({ ...prev, propertyId: e.target.value, unitTypeId: '' }))}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                >
                  <option value="">اختر الكيان</option>
                  {propertiesData?.items?.map(property => (
                    <option key={property.id} value={property.id}>
                      {property.name}
                    </option>
                  ))}
                </select>
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  نوع الوحدة *
                </label>
                <select
                  value={createForm.unitTypeId}
                  onChange={(e) => setCreateForm(prev => ({ ...prev, unitTypeId: e.target.value }))}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  disabled={!createForm.propertyId}
                  required
                >
                  <option value="">اختر نوع الوحدة</option>
                  {isLoadingCreateUnitTypes ? (
                    <option disabled>جارٍ التحميل...</option>
                  ) : (
                    createUnitTypesData?.items.map(type => (
                      <option key={type.id} value={type.id}>
                        {type.name}
                      </option>
                    ))
                  )}
                </select>
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  اسم الوحدة *
                </label>
                <input
                  type="text"
                  value={createForm.name}
                  onChange={(e) => setCreateForm(prev => ({ ...prev, name: e.target.value }))}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  placeholder="أدخل اسم الوحدة"
                  required
                />
              </div>
            </div>
          </div>

          {/* قسم السعة - يظهر فقط إذا كان مطلوباً */}
          {(selectedUnitTypeDetails?.isHasAdults || selectedUnitTypeDetails?.isHasChildren) && (
            <div className="bg-blue-50 rounded-lg p-4">
              <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                <span className="ml-2">👥</span> السعة الاستيعابية
              </h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {selectedUnitTypeDetails?.isHasAdults && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      عدد البالغين * <span className="text-gray-500 text-xs">(الحد الأقصى)</span>
                    </label>
                    <input
                      type="number"
                      min="1"
                      value={createForm.adultCapacity || ''}
                      onChange={(e) => setCreateForm(prev => ({ ...prev, adultCapacity: parseInt(e.target.value) || undefined }))}
                      className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                      placeholder="أدخل عدد البالغين"
                      required
                    />
                  </div>
                )}
                
                {selectedUnitTypeDetails?.isHasChildren && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      عدد الأطفال * <span className="text-gray-500 text-xs">(الحد الأقصى)</span>
                    </label>
                    <input
                      type="number"
                      min="0"
                      value={createForm.childrenCapacity !== undefined ? createForm.childrenCapacity : ''}
                      onChange={(e) => setCreateForm(prev => ({ ...prev, childrenCapacity: parseInt(e.target.value) || 0 }))}
                      className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                      placeholder="أدخل عدد الأطفال"
                      required
                    />
                  </div>
                )}
              </div>
            </div>
          )}

          {/* قسم التسعير */}
          <div className="bg-green-50 rounded-lg p-4">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <span className="ml-2">💰</span> التسعير
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  السعر الأساسي *
                </label>
                <CurrencyInput
                  value={createForm.basePrice.amount}
                  currency={createForm.basePrice.currency}
                  onValueChange={(amount, currency) => 
                    setCreateForm(prev => ({ 
                      ...prev, 
                      basePrice: { amount, currency }
                    }))
                  }
                  placeholder="0.00"
                  required={true}
                  min={0}
                  showSymbol={true}
                  supportedCurrencies={currencyCodes}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  طريقة التسعير *
                </label>
                <select
                  value={createForm.pricingMethod}
                  onChange={(e) => setCreateForm(prev => ({ ...prev, pricingMethod: e.target.value as PricingMethod }))}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                >
                  <option value="Hourly">بالساعة</option>
                  <option value="Daily">يومي</option>
                  <option value="Weekly">أسبوعي</option>
                  <option value="Monthly">شهري</option>
                </select>
              </div>
            </div>
          </div>

          {/* قسم الميزات */}
          <div className="bg-yellow-50 rounded-lg p-4">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <span className="ml-2">✨</span> الميزات المخصصة
            </h3>
            <TagInput
              value={createForm.customFeatures}
              onChange={(value) => setCreateForm(prev => ({ ...prev, customFeatures: value }))}
              placeholder="أدخل الميزات المخصصة واضغط Enter أو الفاصلة للإضافة..."
              variant="modern"
              size="md"
              maxTags={15}
              suggestions={[
                'واي فاي مجاني',
                'مكيف هواء',
                'تلفزيون ذكي',
                'مطبخ مجهز',
                'شرفة خاصة',
                'موقف سيارة',
                'مسبح',
                'جيم',
                'خدمة تنظيف',
                'أمن وحراسة',
                'خدمة استقبال',
                'صالة ألعاب',
                'منطقة شواء',
                'حديقة خاصة',
                'جاكوزي'
              ]}
            />
          </div>

          {/* الحقول الديناميكية - مدمجة بشكل أفضل */}
          {createForm.unitTypeId && createFields && createFields.length > 0 && (
            <div className="bg-purple-50 rounded-lg p-4">
              <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                <span className="ml-2">📝</span> معلومات إضافية
              </h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <DynamicFieldsForm
                  fields={createFields}
                  values={[]}
                  onChange={setCreateDynamicFields}
                  className="contents"
                />
              </div>
            </div>
          )}
          
          {/* صور الوحدة */}
          <div className="bg-indigo-50 rounded-lg p-4">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <span className="ml-2">🖼️</span> صور الوحدة
            </h3>
            <ImageUpload
              value={createForm.images || []}
              onChange={(urls) => setCreateForm(prev => ({ ...prev, images: Array.isArray(urls) ? urls : [urls] }))}
              multiple={true}
              maxFiles={10}
              maxSize={5}
              showPreview={true}
              placeholder="اضغط لرفع صور الوحدة أو اسحبها هنا"
              uploadEndpoint="/api/images/upload"
            />
          </div>
        </div>
      </Modal>

      {/* Edit Unit Modal */}
      <Modal
        isOpen={showEditModal}
        onClose={() => {
          setShowEditModal(false);
          setSelectedUnit(null);
        }}
        title="تعديل بيانات الوحدة"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowEditModal(false);
                setSelectedUnit(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => {
                // التحقق من الحقول الإجبارية
                const capacityErrors = validateCapacityFields(editForm, editUnitTypeDetails);
                if (capacityErrors.length) {
                  alert(capacityErrors.join('\n'));
                  return;
                }
                
                const updateErrors = validateDynamicFields(editFields || [], editDynamicFields);
                if (updateErrors.length) {
                  alert(updateErrors.join('\n'));
                  return;
                }
                const fieldValues: FieldValueDto[] = Object.entries(editDynamicFields).map(([fieldId, value]) => ({
                  fieldId,
                  fieldValue: Array.isArray(value) ? JSON.stringify(value) : String(value)
                }));
                
                const unitData = {
                  ...editForm,
                  fieldValues
                };
                
                updateUnit.mutate({ 
                  unitId: editForm.unitId, 
                  data: unitData 
                }, {
                  onSuccess: () => {
                    setShowEditModal(false);
                    setSelectedUnit(null);
                  },
                });
              }}
              disabled={updateUnit.status === 'pending'}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {updateUnit.status === 'pending' ? 'جارٍ التحديث...' : 'تحديث'}
            </button>
          </div>
        }
      >
        {selectedUnit && (
          <div className="space-y-6">
            {/* القسم الأساسي */}
            <div className="bg-gray-50 rounded-lg p-4">
              <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                <span className="ml-2">📋</span> المعلومات الأساسية
              </h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    اسم الوحدة
                  </label>
                  <input
                    type="text"
                    value={editForm.name}
                    onChange={(e) => setEditForm(prev => ({ ...prev, name: e.target.value }))}
                    className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  />
                </div>
              </div>
            </div>

            {/* قسم السعة - يظهر فقط إذا كان مطلوباً */}
            {(editUnitTypeDetails?.isHasAdults || editUnitTypeDetails?.isHasChildren) && (
              <div className="bg-blue-50 rounded-lg p-4">
                <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                  <span className="ml-2">👥</span> السعة الاستيعابية
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {editUnitTypeDetails?.isHasAdults && (
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        عدد البالغين * <span className="text-gray-500 text-xs">(الحد الأقصى)</span>
                      </label>
                      <input
                        type="number"
                        min="1"
                        value={editForm.adultCapacity || ''}
                        onChange={(e) => setEditForm(prev => ({ ...prev, adultCapacity: parseInt(e.target.value) || undefined }))}
                        className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                        placeholder="أدخل عدد البالغين"
                        required
                      />
                    </div>
                  )}
                  
                  {editUnitTypeDetails?.isHasChildren && (
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        عدد الأطفال * <span className="text-gray-500 text-xs">(الحد الأقصى)</span>
                      </label>
                      <input
                        type="number"
                        min="0"
                        value={editForm.childrenCapacity !== undefined ? editForm.childrenCapacity : ''}
                        onChange={(e) => setEditForm(prev => ({ ...prev, childrenCapacity: parseInt(e.target.value) || 0 }))}
                        className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                        placeholder="أدخل عدد الأطفال"
                        required
                      />
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* قسم التسعير */}
            <div className="bg-green-50 rounded-lg p-4">
              <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                <span className="ml-2">💰</span> التسعير
              </h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    السعر الأساسي
                  </label>
                  <CurrencyInput
                    value={editForm.basePrice?.amount || 0}
                    currency={editForm.basePrice?.currency || 'YER'}
                    onValueChange={(amount, currency) => 
                      setEditForm(prev => ({ 
                        ...prev, 
                        basePrice: { amount, currency }
                      }))
                    }
                    placeholder="0.00"
                    min={0}
                    showSymbol={true}
                    supportedCurrencies={currencyCodes}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    طريقة التسعير
                  </label>
                  <select
                    value={editForm.pricingMethod}
                    onChange={(e) => setEditForm(prev => ({ ...prev, pricingMethod: e.target.value as PricingMethod }))}
                    className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  >
                    <option value="Hourly">بالساعة</option>
                    <option value="Daily">يومي</option>
                    <option value="Weekly">أسبوعي</option>
                    <option value="Monthly">شهري</option>
                  </select>
                </div>
              </div>
            </div>

            {/* قسم الميزات */}
            <div className="bg-yellow-50 rounded-lg p-4">
              <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                <span className="ml-2">✨</span> الميزات المخصصة
              </h3>
              <TagInput
                value={editForm.customFeatures || ''}
                onChange={(value) => setEditForm(prev => ({ ...prev, customFeatures: value }))}
                placeholder="أدخل الميزات المخصصة واضغط Enter أو الفاصلة للإضافة..."
                variant="modern"
                size="md"
                maxTags={15}
                suggestions={[
                  'واي فاي مجاني',
                  'مكيف هواء',
                  'تلفزيون ذكي',
                  'مطبخ مجهز',
                  'شرفة خاصة',
                  'موقف سيارة',
                  'مسبح',
                  'جيم',
                  'خدمة تنظيف',
                  'أمن وحراسة',
                  'خدمة استقبال',
                  'صالة ألعاب',
                  'منطقة شواء',
                  'حديقة خاصة',
                  'جاكوزي'
                ]}
              />
            </div>

            {/* الحقول الديناميكية - مدمجة بشكل أفضل */}
            {selectedUnit && editFields && editFields.length > 0 && (
              <div className="bg-purple-50 rounded-lg p-4">
                <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                  <span className="ml-2">📝</span> معلومات إضافية
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <DynamicFieldsForm
                    fields={editFields}
                    values={selectedUnit?.fieldValues?.map(fv => ({
                      fieldId: fv.fieldId,
                      fieldValue: fv.fieldValue
                    })) || []}
                    onChange={setEditDynamicFields}
                    className="contents"
                  />
                </div>
              </div>
            )}
            
            {/* صور الوحدة */}
            <div className="bg-indigo-50 rounded-lg p-4">
              <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                <span className="ml-2">🖼️</span> صور الوحدة
              </h3>
              <ImageUpload
                value={editForm.images || selectedUnit?.images?.map(img => img.url) || []}
                onChange={(urls) => setEditForm(prev => ({ ...prev, images: Array.isArray(urls) ? urls : [urls] }))}
                multiple={true}
                maxFiles={10}
                maxSize={5}
                showPreview={true}
                placeholder="اضغط لرفع صور جديدة للوحدة أو اسحبها هنا"
                uploadEndpoint="/api/images/upload"
              />
            </div>
          </div>
        )}
      </Modal>

      {/* Unit Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedUnit(null);
        }}
        title="تفاصيل الوحدة"
        size="xl"
      >
        {selectedUnit && (
          <div className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">اسم الوحدة</label>
                <p className="mt-1 text-sm text-gray-900">{selectedUnit.name}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">نوع الوحدة</label>
                <p className="mt-1 text-sm text-gray-900">{selectedUnit.unitTypeName}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">الكيان</label>
                <p className="mt-1 text-sm text-gray-900">{selectedUnit.propertyName}</p>
              </div>
              
              {/* عرض السعة */}
              {(selectedUnit.adultCapacity || selectedUnit.childrenCapacity !== undefined) && (
                <>
                  {selectedUnit.adultCapacity && (
                    <div>
                      <label className="block text-sm font-medium text-gray-700">عدد البالغين</label>
                      <p className="mt-1 text-sm text-gray-900">👨 {selectedUnit.adultCapacity} بالغ</p>
                    </div>
                  )}
                  {selectedUnit.childrenCapacity !== undefined && (
                    <div>
                      <label className="block text-sm font-medium text-gray-700">عدد الأطفال</label>
                      <p className="mt-1 text-sm text-gray-900">👶 {selectedUnit.childrenCapacity} طفل</p>
                    </div>
                  )}
                </>
              )}
              
              <div>
                <label className="block text-sm font-medium text-gray-700">السعر الأساسي</label>
                <p className="mt-1 text-sm text-gray-900 font-medium">
                  {selectedUnit.basePrice.amount} {selectedUnit.basePrice.currency}
                </p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">طريقة التسعير</label>
                <p className="mt-1 text-sm text-gray-900">
                  {filterOptions.find(opt => opt.key === 'pricingMethod')?.options?.find(
                    option => option.value === selectedUnit.pricingMethod
                  )?.label || selectedUnit.pricingMethod}
                </p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">حالة التوفر</label>
                <span className={`mt-1 inline-flex px-2 py-1 text-xs font-medium rounded-full ${
                  selectedUnit.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                }`}>
                  {selectedUnit.isAvailable ? 'متاحة' : 'غير متاحة'}
                </span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">الميزات المخصصة</label>
              {selectedUnit.customFeatures ? (
                <div className="flex flex-wrap gap-1">
                  {selectedUnit.customFeatures.split(',').map((feature, index) => (
                    <span
                      key={index}
                      className="inline-flex items-center px-2.5 py-1 bg-gradient-to-r from-blue-50 to-indigo-50 text-blue-700 border border-blue-200 rounded-full text-sm font-medium"
                    >
                      ✨ {feature.trim()}
                    </span>
                  ))}
                </div>
              ) : (
                <p className="text-sm text-gray-500 italic">لا توجد ميزات مخصصة</p>
              )}
            </div>

            {/* عرض الحقول الديناميكية */}
            {selectedUnit.fieldValues && selectedUnit.fieldValues.length > 0 && (
              <div className="border-t border-gray-200 pt-6">
                <h3 className="text-lg font-medium text-gray-900 mb-4">
                  📝 معلومات إضافية
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {selectedUnit.fieldValues.map((fieldValue) => (
                    <div key={fieldValue.fieldId} className="space-y-1">
                      <label className="block text-sm font-medium text-gray-700">
                        {fieldValue.displayName || fieldValue.fieldName}
                      </label>
                      <div className="text-sm text-gray-900 bg-gray-50 px-3 py-2 rounded-md">
                        {fieldValue.fieldValue || 'لا توجد قيمة'}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* عرض الصور */}
            {selectedUnit.images && selectedUnit.images.length > 0 && (
              <div className="border-t border-gray-200 pt-6">
                <h3 className="text-lg font-medium text-gray-900 mb-4">
                  🖼️ صور الوحدة
                </h3>
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                  {selectedUnit.images.map((image, index) => (
                    <div key={index} className="relative">
                      <img
                        src={image.url}
                        alt={image.altText || `صورة ${index + 1}`}
                        className="w-full h-32 object-cover rounded-lg"
                      />
                      {image.isMain && (
                        <span className="absolute top-2 right-2 bg-blue-500 text-white text-xs px-2 py-1 rounded">
                          رئيسية
                        </span>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        )}
      </Modal>

      {/* Assign Sections Modal */}
      <Modal
        isOpen={showAssignSectionsModal}
        onClose={() => {
          setShowAssignSectionsModal(false);
          setSelectedSectionIds([]);
        }}
        title="إضافة الوحدة للأقسام"
        size="md"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowAssignSectionsModal(false);
                setSelectedSectionIds([]);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={confirmAssignSections}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
            >
              حفظ
            </button>
          </div>
        }
      >
        <div className="space-y-3">
          <p className="text-sm text-gray-600">
            حدد الأقسام التي تريد إضافة الوحدة "{selectedUnit?.name}" إليها:
          </p>
          <div className="grid grid-cols-1 gap-2 max-h-80 overflow-auto border border-gray-200 rounded-lg p-3">
            {availableUnitSections.length > 0 ? (
              availableUnitSections.map((section: any) => (
                <label key={section.id} className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded cursor-pointer">
                  <input 
                    type="checkbox" 
                    checked={selectedSectionIds.includes(section.id)} 
                    onChange={(e) => {
                      setSelectedSectionIds(prev => 
                        e.target.checked 
                          ? [...prev, section.id] 
                          : prev.filter(x => x !== section.id)
                      );
                    }}
                    className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                  <div className="flex-1">
                    <span className="font-medium text-gray-900">
                      #{section.displayOrder} - {section.type}
                    </span>
                    {section.title && (
                      <p className="text-sm text-gray-500">{section.title}</p>
                    )}
                  </div>
                </label>
              ))
            ) : (
              <p className="text-center text-gray-500 py-4">لا توجد أقسام متاحة</p>
            )}
          </div>
          {selectedSectionIds.length > 0 && (
            <p className="text-sm text-blue-600">
              تم تحديد {selectedSectionIds.length} قسم/أقسام
            </p>
          )}
        </div>
      </Modal>
    </div>
  );
};

export default AdminUnits;