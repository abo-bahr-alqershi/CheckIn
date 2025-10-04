# دليل التعامل مع الحقول الديناميكية (Dynamic Fields)

## نظرة عامة

النظام يدعم **24+ نوع مختلف من الحقول الديناميكية** التي يمكن إنشاؤها وإدارتها ديناميكياً لأنواع الوحدات المختلفة. هذا الدليل يوضح كيفية التعامل معها في تطبيق الموبايل.

---

## 🏗️ الهيكل الهرمي للحقول

```
Property Types (أنواع الكيانات)
└── Unit Types (أنواع الوحدات)
    └── Field Groups (مجموعات الحقول)
        └── Dynamic Fields (الحقول الديناميكية)
            └── Field Values (قيم الحقول)
```

---

## 📋 أنواع الحقول المدعومة

### 1. الحقول النصية
- **text**: نص قصير
- **textarea**: نص طويل متعدد الأسطر
- **email**: بريد إلكتروني
- **phone**: رقم هاتف
- **url**: رابط

### 2. الحقول الرقمية
- **number**: رقم عادي
- **currency**: مبلغ مالي (مع العملة)
- **percentage**: نسبة مئوية
- **range**: نطاق قيم (مع منزلق)
- **rating**: تقييم نجوم (1-5)

### 3. حقول التاريخ والوقت
- **date**: تاريخ
- **datetime**: تاريخ ووقت
- **time**: وقت فقط

### 4. حقول الاختيار
- **boolean**: صح/خطأ
- **checkbox**: خانة اختيار
- **select**: قائمة اختيار واحد
- **multiselect**: قائمة اختيار متعدد

### 5. حقول الملفات والوسائط
- **file**: ملف عام
- **image**: صورة
- **color**: لون

### 6. حقول متقدمة
- **tag**: علامات (tags)

---

## 🔧 APIs المطلوبة

### 1. جلب الحقول الديناميكية لنوع وحدة
```typescript
// GET /api/admin/UnitTypeFields/by-unit-type/{unitTypeId}
interface GetUnitTypeFieldsQuery {
  unitTypeId: string;
  isPublic?: boolean; // للعميل استخدم true
}

// Response
interface UnitTypeFieldDto {
  fieldId: string;
  unitTypeId: string;
  groupId: string;
  fieldTypeId: string; // نوع الحقل
  fieldName: string;
  displayName: string;
  description?: string;
  isRequired: boolean;
  isPublic: boolean;
  sortOrder: number;
  validationRules?: {
    minLength?: number;
    maxLength?: number;
    min?: number;
    max?: number;
    step?: number;
    pattern?: string;
    currency?: string;
    allowedTypes?: string[];
    minDate?: string;
    maxDate?: string;
  };
  fieldOptions?: {
    options?: string[];
    allowMultiple?: boolean;
  };
  category?: string;
}
```

### 2. حفظ قيم الحقول
```typescript
// عند إنشاء/تحديث وحدة
interface FieldValueDto {
  fieldId: string;
  fieldValue: string; // دائماً string، حتى للمصفوفات (JSON.stringify)
}

// في CreateUnitCommand أو UpdateUnitCommand
{
  // ... باقي البيانات
  fieldValues: FieldValueDto[];
}
```

### 3. جلب قيم الحقول الحالية
```typescript
// الوحدة تحتوي على fieldValues
interface UnitDto {
  // ... باقي البيانات
  fieldValues: UnitFieldValueDto[];
}

interface UnitFieldValueDto {
  valueId: string;
  unitId: string;
  fieldId: string;
  fieldName: string;
  displayName: string;
  fieldValue: string;
  field: UnitTypeFieldDto;
}
```

---

## 📱 تطبيق الفلاتر الديناميكية في الموبايل

### 1. جلب الحقول القابلة للفلترة
```typescript
// استعلام الحقول العامة فقط للفلترة
const getFilterableFields = async (unitTypeId: string) => {
  const response = await api.get(`/api/admin/UnitTypeFields/by-unit-type/${unitTypeId}`, {
    params: { isPublic: true }
  });
  
  // فلترة الحقول القابلة للفلترة
  return response.data.filter(field => 
    ['select', 'multiselect', 'boolean', 'range', 'currency', 'number', 'rating'].includes(field.fieldTypeId)
  );
};
```

### 2. بناء واجهة الفلاتر
```typescript
interface DynamicFilter {
  fieldId: string;
  fieldType: string;
  operator: 'eq' | 'gte' | 'lte' | 'in' | 'contains';
  value: any;
}

const buildFilterComponent = (field: UnitTypeFieldDto) => {
  switch (field.fieldTypeId) {
    case 'select':
      return <SelectFilter field={field} />;
    
    case 'multiselect':
      return <MultiSelectFilter field={field} />;
    
    case 'boolean':
      return <BooleanFilter field={field} />;
    
    case 'range':
    case 'currency':
    case 'number':
      return <RangeFilter field={field} />;
    
    case 'rating':
      return <RatingFilter field={field} />;
    
    default:
      return null;
  }
};
```

### 3. مكونات الفلاتر

#### Select Filter
```typescript
const SelectFilter = ({ field }: { field: UnitTypeFieldDto }) => {
  const [selectedValue, setSelectedValue] = useState('');
  
  return (
    <Picker
      selectedValue={selectedValue}
      onValueChange={setSelectedValue}
    >
      <Picker.Item label="الكل" value="" />
      {field.fieldOptions?.options?.map(option => (
        <Picker.Item key={option} label={option} value={option} />
      ))}
    </Picker>
  );
};
```

#### Multi Select Filter
```typescript
const MultiSelectFilter = ({ field }: { field: UnitTypeFieldDto }) => {
  const [selectedValues, setSelectedValues] = useState<string[]>([]);
  
  const toggleOption = (option: string) => {
    setSelectedValues(prev => 
      prev.includes(option)
        ? prev.filter(v => v !== option)
        : [...prev, option]
    );
  };
  
  return (
    <View>
      {field.fieldOptions?.options?.map(option => (
        <TouchableOpacity
          key={option}
          onPress={() => toggleOption(option)}
          style={[
            styles.optionButton,
            selectedValues.includes(option) && styles.selectedOption
          ]}
        >
          <Text>{option}</Text>
        </TouchableOpacity>
      ))}
    </View>
  );
};
```

#### Range Filter
```typescript
const RangeFilter = ({ field }: { field: UnitTypeFieldDto }) => {
  const [minValue, setMinValue] = useState(field.validationRules?.min || 0);
  const [maxValue, setMaxValue] = useState(field.validationRules?.max || 100);
  
  return (
    <View>
      <Text>من: {minValue}</Text>
      <Slider
        value={minValue}
        onValueChange={setMinValue}
        minimumValue={field.validationRules?.min || 0}
        maximumValue={maxValue}
      />
      
      <Text>إلى: {maxValue}</Text>
      <Slider
        value={maxValue}
        onValueChange={setMaxValue}
        minimumValue={minValue}
        maximumValue={field.validationRules?.max || 100}
      />
    </View>
  );
};
```

#### Boolean Filter
```typescript
const BooleanFilter = ({ field }: { field: UnitTypeFieldDto }) => {
  const [value, setValue] = useState<boolean | null>(null);
  
  return (
    <View style={styles.booleanContainer}>
      <TouchableOpacity
        onPress={() => setValue(null)}
        style={[styles.booleanOption, value === null && styles.selected]}
      >
        <Text>الكل</Text>
      </TouchableOpacity>
      
      <TouchableOpacity
        onPress={() => setValue(true)}
        style={[styles.booleanOption, value === true && styles.selected]}
      >
        <Text>نعم</Text>
      </TouchableOpacity>
      
      <TouchableOpacity
        onPress={() => setValue(false)}
        style={[styles.booleanOption, value === false && styles.selected]}
      >
        <Text>لا</Text>
      </TouchableOpacity>
    </View>
  );
};
```

#### Rating Filter
```typescript
const RatingFilter = ({ field }: { field: UnitTypeFieldDto }) => {
  const [minRating, setMinRating] = useState(0);
  
  return (
    <View style={styles.ratingContainer}>
      <Text>التقييم الأدنى:</Text>
      <View style={styles.starsContainer}>
        {[1, 2, 3, 4, 5].map(star => (
          <TouchableOpacity
            key={star}
            onPress={() => setMinRating(star)}
          >
            <Text style={[
              styles.star,
              star <= minRating && styles.selectedStar
            ]}>
              ⭐
            </Text>
          </TouchableOpacity>
        ))}
      </View>
    </View>
  );
};
```

---

## 🔍 تطبيق الفلاتر في البحث

### 1. بناء معايير البحث
```typescript
const buildSearchCriteria = (filters: DynamicFilter[]) => {
  const dynamicFieldFilters = filters.map(filter => ({
    fieldId: filter.fieldId,
    operator: filter.operator,
    value: Array.isArray(filter.value) 
      ? JSON.stringify(filter.value)
      : String(filter.value)
  }));
  
  return {
    // معايير البحث العادية
    propertyId: selectedPropertyId,
    unitTypeId: selectedUnitTypeId,
    // الفلاتر الديناميكية
    dynamicFieldFilters
  };
};
```

### 2. API للبحث مع الفلاتر الديناميكية
```typescript
// POST /api/search/units
interface SearchUnitsRequest {
  propertyId?: string;
  unitTypeId?: string;
  checkInDate?: string;
  checkOutDate?: string;
  guestsCount?: number;
  minPrice?: number;
  maxPrice?: number;
  // الفلاتر الديناميكية
  dynamicFieldFilters?: {
    fieldId: string;
    operator: 'eq' | 'gte' | 'lte' | 'in' | 'contains';
    value: string;
  }[];
  // ترتيب وصفحات
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}
```

---

## 🎨 مثال تطبيق كامل

```typescript
import React, { useState, useEffect } from 'react';
import { View, ScrollView, Text, TouchableOpacity } from 'react-native';

const DynamicFilters = ({ unitTypeId, onFiltersChange }) => {
  const [fields, setFields] = useState<UnitTypeFieldDto[]>([]);
  const [filters, setFilters] = useState<DynamicFilter[]>([]);
  
  // جلب الحقول
  useEffect(() => {
    if (unitTypeId) {
      fetchFilterableFields(unitTypeId);
    }
  }, [unitTypeId]);
  
  const fetchFilterableFields = async (unitTypeId: string) => {
    try {
      const response = await api.get(`/api/admin/UnitTypeFields/by-unit-type/${unitTypeId}`, {
        params: { isPublic: true }
      });
      
      const filterableFields = response.data.filter(field => 
        ['select', 'multiselect', 'boolean', 'range', 'currency', 'number', 'rating']
          .includes(field.fieldTypeId)
      );
      
      setFields(filterableFields);
    } catch (error) {
      console.error('Error fetching fields:', error);
    }
  };
  
  const updateFilter = (fieldId: string, value: any, operator: string = 'eq') => {
    setFilters(prev => {
      const newFilters = prev.filter(f => f.fieldId !== fieldId);
      if (value !== null && value !== '' && (!Array.isArray(value) || value.length > 0)) {
        newFilters.push({
          fieldId,
          fieldType: fields.find(f => f.fieldId === fieldId)?.fieldTypeId || '',
          operator,
          value
        });
      }
      onFiltersChange(newFilters);
      return newFilters;
    });
  };
  
  const clearFilters = () => {
    setFilters([]);
    onFiltersChange([]);
  };
  
  // تجميع الحقول حسب الفئة
  const groupedFields = fields.reduce((groups, field) => {
    const category = field.category || 'عام';
    if (!groups[category]) {
      groups[category] = [];
    }
    groups[category].push(field);
    return groups;
  }, {} as Record<string, UnitTypeFieldDto[]>);
  
  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>الفلاتر المتقدمة</Text>
        <TouchableOpacity onPress={clearFilters}>
          <Text style={styles.clearButton}>مسح الكل</Text>
        </TouchableOpacity>
      </View>
      
      {Object.entries(groupedFields).map(([category, categoryFields]) => (
        <View key={category} style={styles.categorySection}>
          <Text style={styles.categoryTitle}>📁 {category}</Text>
          
          {categoryFields.map(field => (
            <View key={field.fieldId} style={styles.fieldContainer}>
              <Text style={styles.fieldLabel}>
                {getFieldIcon(field.fieldTypeId)} {field.displayName}
              </Text>
              
              {renderFilterComponent(field, updateFilter)}
            </View>
          ))}
        </View>
      ))}
      
      {filters.length > 0 && (
        <View style={styles.activeFilters}>
          <Text style={styles.activeFiltersTitle}>الفلاتر النشطة:</Text>
          {filters.map((filter, index) => (
            <View key={index} style={styles.filterChip}>
              <Text style={styles.filterChipText}>
                {fields.find(f => f.fieldId === filter.fieldId)?.displayName}: {
                  Array.isArray(filter.value) 
                    ? filter.value.join(', ')
                    : String(filter.value)
                }
              </Text>
              <TouchableOpacity
                onPress={() => updateFilter(filter.fieldId, null)}
                style={styles.removeFilter}
              >
                <Text>×</Text>
              </TouchableOpacity>
            </View>
          ))}
        </View>
      )}
    </ScrollView>
  );
};

const getFieldIcon = (fieldType: string) => {
  const icons = {
    text: '📝', select: '📋', multiselect: '☑️', boolean: '✅',
    number: '🔢', currency: '💰', percentage: '📊', range: '🎚️',
    rating: '⭐', date: '📅', color: '🎨'
  };
  return icons[fieldType] || '📝';
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 20,
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  clearButton: {
    color: '#ef4444',
    fontSize: 14,
  },
  categorySection: {
    marginBottom: 20,
  },
  categoryTitle: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 10,
    color: '#374151',
  },
  fieldContainer: {
    marginBottom: 15,
    padding: 15,
    backgroundColor: '#f9fafb',
    borderRadius: 8,
  },
  fieldLabel: {
    fontSize: 14,
    fontWeight: '500',
    marginBottom: 8,
    color: '#374151',
  },
  activeFilters: {
    marginTop: 20,
    padding: 15,
    backgroundColor: '#eff6ff',
    borderRadius: 8,
  },
  activeFiltersTitle: {
    fontSize: 14,
    fontWeight: '600',
    marginBottom: 10,
  },
  filterChip: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#dbeafe',
    padding: 8,
    borderRadius: 16,
    marginRight: 8,
    marginBottom: 8,
  },
  filterChipText: {
    fontSize: 12,
    color: '#1e40af',
  },
  removeFilter: {
    marginLeft: 8,
    width: 20,
    height: 20,
    borderRadius: 10,
    backgroundColor: '#ef4444',
    alignItems: 'center',
    justifyContent: 'center',
  },
});
```

---

## 📝 ملاحظات مهمة

### 1. معالجة القيم
- **المصفوفات**: استخدم `JSON.stringify()` قبل الإرسال
- **التواريخ**: تأكد من التنسيق الصحيح (ISO 8601)
- **الأرقام**: تحويل إلى string للإرسال

### 2. الأداء
- احفظ الحقول في cache محلي
- استخدم debounce للبحث المباشر
- قم بتحميل الحقول مرة واحدة فقط

### 3. تجربة المستخدم
- أضف أيقونات للحقول المختلفة
- استخدم تجميع منطقي للحقول
- أضف خيار "مسح الكل" للفلاتر

### 4. التوافق
- تأكد من دعم RTL في التخطيط
- استخدم ألوان متسقة مع التطبيق
- اختبر على أحجام شاشات مختلفة

---

## 🔄 سير العمل الموصى به

1. **جلب أنواع الوحدات** للكيان المحدد
2. **عرض خيارات نوع الوحدة** للمستخدم
3. **جلب الحقول الديناميكية** لنوع الوحدة المختار
4. **بناء واجهة الفلاتر** ديناميكياً
5. **تطبيق الفلاتر** في استعلام البحث
6. **عرض النتائج** مع إمكانية تعديل الفلاتر

هذا الدليل يوفر أساساً قوياً لتطبيق الفلاتر الديناميكية في تطبيق الموبايل مع المرونة الكاملة لإضافة أنواع حقول جديدة مستقبلاً.