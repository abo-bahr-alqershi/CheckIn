import React, { useState } from 'react';
import { useAdminAmenities } from '../../hooks/useAdminAmenities';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import DataTable, { type Column } from '../../components/common/DataTable';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import Modal from '../../components/common/Modal';
import PropertySelector from '../../components/selectors/PropertySelector';
import type {
  AmenityDto,
  CreateAmenityCommand,
  UpdateAmenityCommand,
  GetAllAmenitiesQuery,
  AssignAmenityToPropertyCommand,
  UpdatePropertyAmenityCommand,
  MoneyDto
} from '../../types/amenity.types';
import CurrencyInput from '../../components/inputs/CurrencyInput';
import { useCurrencies } from '../../hooks/useCurrencies';

// قائمة أيقونات Material Icons المتخصصة للمرافق والخدمات
const AMENITY_MATERIAL_ICONS = [
  // مرافق أساسية
  { name: 'wifi', label: 'واي فاي', icon: '📶', category: 'أساسيات' },
  { name: 'network_wifi', label: 'شبكة واي فاي', icon: '📡', category: 'أساسيات' },
  { name: 'signal_wifi_4_bar', label: 'واي فاي قوي', icon: '📶', category: 'أساسيات' },
  { name: 'router', label: 'راوتر', icon: '🔌', category: 'أساسيات' },
  { name: 'ac_unit', label: 'تكييف', icon: '❄️', category: 'أساسيات' },
  { name: 'thermostat', label: 'ثرموستات', icon: '🌡️', category: 'أساسيات' },
  { name: 'air', label: 'تهوية', icon: '💨', category: 'أساسيات' },
  { name: 'water_drop', label: 'ماء', icon: '💧', category: 'أساسيات' },
  { name: 'electric_bolt', label: 'كهرباء', icon: '⚡', category: 'أساسيات' },
  { name: 'gas_meter', label: 'غاز', icon: '🔥', category: 'أساسيات' },
  { name: 'heating', label: 'تدفئة', icon: '🔥', category: 'أساسيات' },
  { name: 'light', label: 'إضاءة', icon: '💡', category: 'أساسيات' },
  
  // مرافق المطبخ
  { name: 'kitchen', label: 'مطبخ', icon: '🍳', category: 'مطبخ' },
  { name: 'microwave', label: 'مايكروويف', icon: '📦', category: 'مطبخ' },
  { name: 'coffee_maker', label: 'صانع القهوة', icon: '☕', category: 'مطبخ' },
  { name: 'blender', label: 'خلاط', icon: '🥤', category: 'مطبخ' },
  { name: 'dining_room', label: 'غرفة طعام', icon: '🍽️', category: 'مطبخ' },
  { name: 'restaurant', label: 'مطعم', icon: '🍴', category: 'مطبخ' },
  { name: 'local_cafe', label: 'مقهى', icon: '☕', category: 'مطبخ' },
  { name: 'local_bar', label: 'بار', icon: '🍺', category: 'مطبخ' },
  { name: 'breakfast_dining', label: 'إفطار', icon: '🍳', category: 'مطبخ' },
  { name: 'lunch_dining', label: 'غداء', icon: '🍽️', category: 'مطبخ' },
  { name: 'dinner_dining', label: 'عشاء', icon: '🍽️', category: 'مطبخ' },
  { name: 'outdoor_grill', label: 'شواية خارجية', icon: '🍖', category: 'مطبخ' },
  { name: 'countertops', label: 'أسطح عمل', icon: '🔲', category: 'مطبخ' },
  { name: 'kitchen_appliances', label: 'أجهزة مطبخ', icon: '🍳', category: 'مطبخ' },
  
  // أجهزة كهربائية
  { name: 'tv', label: 'تلفزيون', icon: '📺', category: 'أجهزة' },
  { name: 'desktop_windows', label: 'كمبيوتر', icon: '💻', category: 'أجهزة' },
  { name: 'laptop', label: 'لابتوب', icon: '💻', category: 'أجهزة' },
  { name: 'phone_android', label: 'هاتف', icon: '📱', category: 'أجهزة' },
  { name: 'tablet', label: 'تابلت', icon: '📱', category: 'أجهزة' },
  { name: 'speaker', label: 'سماعات', icon: '🔊', category: 'أجهزة' },
  { name: 'radio', label: 'راديو', icon: '📻', category: 'أجهزة' },
  { name: 'videogame_asset', label: 'ألعاب فيديو', icon: '🎮', category: 'أجهزة' },
  { name: 'local_laundry_service', label: 'غسالة', icon: '🧺', category: 'أجهزة' },
  { name: 'dry_cleaning', label: 'تنظيف جاف', icon: '👔', category: 'أجهزة' },
  { name: 'iron', label: 'مكواة', icon: '👔', category: 'أجهزة' },
  { name: 'dishwasher', label: 'غسالة صحون', icon: '🍽️', category: 'أجهزة' },
  
  // مرافق الحمام
  { name: 'bathroom', label: 'حمام', icon: '🚿', category: 'حمام' },
  { name: 'bathtub', label: 'حوض استحمام', icon: '🛁', category: 'حمام' },
  { name: 'shower', label: 'دش', icon: '🚿', category: 'حمام' },
  { name: 'soap', label: 'صابون', icon: '🧼', category: 'حمام' },
  { name: 'dry', label: 'مجفف', icon: '💨', category: 'حمام' },
  { name: 'wash', label: 'غسيل', icon: '🧴', category: 'حمام' },
  
  // مرافق النوم والراحة
  { name: 'bed', label: 'سرير', icon: '🛏️', category: 'نوم' },
  { name: 'king_bed', label: 'سرير كبير', icon: '🛏️', category: 'نوم' },
  { name: 'single_bed', label: 'سرير مفرد', icon: '🛏️', category: 'نوم' },
  { name: 'bedroom_parent', label: 'غرفة نوم رئيسية', icon: '🛏️', category: 'نوم' },
  { name: 'bedroom_child', label: 'غرفة أطفال', icon: '🛏️', category: 'نوم' },
  { name: 'crib', label: 'سرير أطفال', icon: '👶', category: 'نوم' },
  { name: 'chair', label: 'كرسي', icon: '🪑', category: 'نوم' },
  { name: 'chair_alt', label: 'كرسي مريح', icon: '🪑', category: 'نوم' },
  { name: 'weekend', label: 'أريكة', icon: '🛋️', category: 'نوم' },
  { name: 'living', label: 'غرفة معيشة', icon: '🛋️', category: 'نوم' },
  
  // مرافق رياضية وترفيهية
  { name: 'pool', label: 'مسبح', icon: '🏊', category: 'رياضة' },
  { name: 'hot_tub', label: 'جاكوزي', icon: '♨️', category: 'رياضة' },
  { name: 'fitness_center', label: 'صالة رياضية', icon: '💪', category: 'رياضة' },
  { name: 'sports_tennis', label: 'ملعب تنس', icon: '🎾', category: 'رياضة' },
  { name: 'sports_soccer', label: 'ملعب كرة قدم', icon: '⚽', category: 'رياضة' },
  { name: 'sports_basketball', label: 'ملعب كرة سلة', icon: '🏀', category: 'رياضة' },
  { name: 'sports_volleyball', label: 'كرة طائرة', icon: '🏐', category: 'رياضة' },
  { name: 'sports_golf', label: 'جولف', icon: '⛳', category: 'رياضة' },
  { name: 'sports_handball', label: 'كرة يد', icon: '🤾', category: 'رياضة' },
  { name: 'sports_cricket', label: 'كريكيت', icon: '🏏', category: 'رياضة' },
  { name: 'sports_baseball', label: 'بيسبول', icon: '⚾', category: 'رياضة' },
  { name: 'sports_esports', label: 'ألعاب إلكترونية', icon: '🎮', category: 'رياضة' },
  { name: 'spa', label: 'سبا', icon: '💆', category: 'رياضة' },
  { name: 'sauna', label: 'ساونا', icon: '🧖', category: 'رياضة' },
  { name: 'self_improvement', label: 'يوغا', icon: '🧘', category: 'رياضة' },
  
  // مرافق المواصلات والمواقف
  { name: 'local_parking', label: 'موقف سيارات', icon: '🅿️', category: 'مواصلات' },
  { name: 'garage', label: 'كراج', icon: '🚗', category: 'مواصلات' },
  { name: 'ev_station', label: 'شحن سيارات كهربائية', icon: '🔌', category: 'مواصلات' },
  { name: 'local_gas_station', label: 'محطة وقود', icon: '⛽', category: 'مواصلات' },
  { name: 'car_rental', label: 'تأجير سيارات', icon: '🚙', category: 'مواصلات' },
  { name: 'car_repair', label: 'صيانة سيارات', icon: '🔧', category: 'مواصلات' },
  { name: 'directions_car', label: 'سيارة', icon: '🚗', category: 'مواصلات' },
  { name: 'directions_bus', label: 'حافلة', icon: '🚌', category: 'مواصلات' },
  { name: 'directions_bike', label: 'دراجة', icon: '🚴', category: 'مواصلات' },
  { name: 'electric_bike', label: 'دراجة كهربائية', icon: '🚴', category: 'مواصلات' },
  { name: 'electric_scooter', label: 'سكوتر كهربائي', icon: '🛴', category: 'مواصلات' },
  { name: 'moped', label: 'دراجة نارية', icon: '🏍️', category: 'مواصلات' },
  
  // مرافق المصاعد والسلالم
  { name: 'elevator', label: 'مصعد', icon: '🛗', category: 'وصول' },
  { name: 'stairs', label: 'درج', icon: '📶', category: 'وصول' },
  { name: 'escalator', label: 'سلم متحرك', icon: '🔼', category: 'وصول' },
  { name: 'escalator_warning', label: 'تحذير سلم متحرك', icon: '⚠️', category: 'وصول' },
  { name: 'accessible', label: 'ممر لذوي الاحتياجات', icon: '♿', category: 'وصول' },
  { name: 'wheelchair_pickup', label: 'كرسي متحرك', icon: '♿', category: 'وصول' },
  { name: 'elderly', label: 'كبار السن', icon: '👴', category: 'وصول' },
  
  // مرافق الأمان
  { name: 'security', label: 'أمن', icon: '🔒', category: 'أمان' },
  { name: 'lock', label: 'قفل', icon: '🔒', category: 'أمان' },
  { name: 'key', label: 'مفتاح', icon: '🔑', category: 'أمان' },
  { name: 'vpn_key', label: 'مفتاح رقمي', icon: '🔐', category: 'أمان' },
  { name: 'shield', label: 'درع', icon: '🛡️', category: 'أمان' },
  { name: 'admin_panel_settings', label: 'لوحة تحكم', icon: '⚙️', category: 'أمان' },
  { name: 'verified_user', label: 'مستخدم موثق', icon: '✅', category: 'أمان' },
  { name: 'safety_check', label: 'فحص أمان', icon: '✅', category: 'أمان' },
  { name: 'health_and_safety', label: 'صحة وأمان', icon: '🏥', category: 'أمان' },
  { name: 'local_police', label: 'شرطة', icon: '👮', category: 'أمان' },
  { name: 'local_fire_department', label: 'إطفاء', icon: '🚒', category: 'أمان' },
  { name: 'medical_services', label: 'خدمات طبية', icon: '🏥', category: 'أمان' },
  { name: 'emergency', label: 'طوارئ', icon: '🚨', category: 'أمان' },
  { name: 'camera_alt', label: 'كاميرا', icon: '📷', category: 'أمان' },
  { name: 'videocam', label: 'كاميرا فيديو', icon: '📹', category: 'أمان' },
  { name: 'sensor_door', label: 'حساس باب', icon: '🚪', category: 'أمان' },
  { name: 'sensor_window', label: 'حساس نافذة', icon: '🪟', category: 'أمان' },
  { name: 'doorbell', label: 'جرس الباب', icon: '🔔', category: 'أمان' },
  { name: 'smoke_detector', label: 'كاشف دخان', icon: '🚨', category: 'أمان' },
  { name: 'fire_extinguisher', label: 'طفاية حريق', icon: '🧯', category: 'أمان' },
  
  // خدمات إضافية
  { name: 'cleaning_services', label: 'خدمة تنظيف', icon: '🧹', category: 'خدمات' },
  { name: 'room_service', label: 'خدمة الغرف', icon: '🛎️', category: 'خدمات' },
  { name: 'concierge', label: 'كونسيرج', icon: '🧑‍💼', category: 'خدمات' },
  { name: 'luggage', label: 'أمتعة', icon: '🧳', category: 'خدمات' },
  { name: 'shopping_cart', label: 'عربة تسوق', icon: '🛒', category: 'خدمات' },
  { name: 'local_grocery_store', label: 'بقالة', icon: '🛒', category: 'خدمات' },
  { name: 'local_mall', label: 'مول', icon: '🛍️', category: 'خدمات' },
  { name: 'local_pharmacy', label: 'صيدلية', icon: '💊', category: 'خدمات' },
  { name: 'local_hospital', label: 'مستشفى', icon: '🏥', category: 'خدمات' },
  { name: 'local_atm', label: 'صراف آلي', icon: '💳', category: 'خدمات' },
  { name: 'local_library', label: 'مكتبة', icon: '📚', category: 'خدمات' },
  { name: 'local_post_office', label: 'بريد', icon: '📮', category: 'خدمات' },
  { name: 'print', label: 'طباعة', icon: '🖨️', category: 'خدمات' },
  { name: 'mail', label: 'بريد', icon: '📧', category: 'خدمات' },
  
  // مرافق خارجية
  { name: 'balcony', label: 'شرفة', icon: '🌅', category: 'خارجي' },
  { name: 'deck', label: 'سطح', icon: '☀️', category: 'خارجي' },
  { name: 'yard', label: 'فناء', icon: '🏡', category: 'خارجي' },
  { name: 'grass', label: 'حديقة', icon: '🌿', category: 'خارجي' },
  { name: 'park', label: 'منتزه', icon: '🌳', category: 'خارجي' },
  { name: 'forest', label: 'غابة', icon: '🌲', category: 'خارجي' },
  { name: 'beach_access', label: 'شاطئ', icon: '🏖️', category: 'خارجي' },
  { name: 'water', label: 'مياه', icon: '💧', category: 'خارجي' },
  { name: 'fence', label: 'سياج', icon: '🚧', category: 'خارجي' },
  { name: 'roofing', label: 'سقف', icon: '🏗️', category: 'خارجي' },
  
  // مرافق الأطفال
  { name: 'child_care', label: 'رعاية أطفال', icon: '👶', category: 'أطفال' },
  { name: 'child_friendly', label: 'صديق للأطفال', icon: '👨‍👩‍👧‍👦', category: 'أطفال' },
  { name: 'baby_changing_station', label: 'غرفة تغيير حفاضات', icon: '👶', category: 'أطفال' },
  { name: 'toys', label: 'ألعاب', icon: '🧸', category: 'أطفال' },
  { name: 'stroller', label: 'عربة أطفال', icon: '👶', category: 'أطفال' },
  
  // مرافق الحيوانات الأليفة
  { name: 'pets', label: 'حيوانات أليفة', icon: '🐾', category: 'حيوانات' },
  { name: 'pet_supplies', label: 'مستلزمات حيوانات', icon: '🐕', category: 'حيوانات' },
  
  // مرافق العمل والدراسة
  { name: 'desk', label: 'مكتب', icon: '🪑', category: 'عمل' },
  { name: 'meeting_room', label: 'قاعة اجتماعات', icon: '👥', category: 'عمل' },
  { name: 'business_center', label: 'مركز أعمال', icon: '💼', category: 'عمل' },
  { name: 'computer', label: 'كمبيوتر', icon: '💻', category: 'عمل' },
  { name: 'scanner', label: 'ماسح ضوئي', icon: '📄', category: 'عمل' },
  { name: 'fax', label: 'فاكس', icon: '📠', category: 'عمل' },
  
  // مرافق دينية
  { name: 'mosque', label: 'مسجد', icon: '🕌', category: 'ديني' },
  { name: 'church', label: 'كنيسة', icon: '⛪', category: 'ديني' },
  { name: 'synagogue', label: 'كنيس', icon: '🕍', category: 'ديني' },
  { name: 'temple_hindu', label: 'معبد هندوسي', icon: '🛕', category: 'ديني' },
  { name: 'temple_buddhist', label: 'معبد بوذي', icon: '🏛️', category: 'ديني' },
];

// مكون اختيار الأيقونة للمرافق
const AmenityIconPicker = ({ 
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
  
  const categories = ['الكل', ...new Set(AMENITY_MATERIAL_ICONS.map(icon => icon.category))];
  
  const filteredIcons = AMENITY_MATERIAL_ICONS.filter(icon => {
    const matchesSearch = icon.label.toLowerCase().includes(searchTerm.toLowerCase()) ||
                          icon.name.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory = selectedCategory === 'الكل' || icon.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });
  
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-[60]">
      <div className="bg-white rounded-lg p-6 w-full max-w-5xl max-h-[90vh] overflow-hidden flex flex-col">
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold">اختر أيقونة للمرفق</h3>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-xl"
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
            استخدم اسم الأيقونة (مثل: Icons.{selectedIcon || 'wifi'}) في تطبيق Flutter.
          </p>
        </div>
      </div>
    </div>
  );
};

const AdminAmenities = () => {
  // Fetch currencies for extra cost
  const { currencies, loading: currenciesLoading } = useCurrencies();
  const currencyCodes = currenciesLoading ? [] : currencies.map(c => c.code);

  // استخدام الهوكات لإدارة البيانات والعمليات
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({ isAssigned: undefined, propertyId: undefined, isFree: undefined });

  // بناء معايير الاستعلام
  const queryParams: GetAllAmenitiesQuery = {
    pageNumber: currentPage,
    pageSize,
    searchTerm: searchTerm || undefined,
    propertyId: filterValues.propertyId || undefined,
    isAssigned: filterValues.isAssigned,
    isFree: filterValues.isFree
  };
  
  // استعلام المرافق عبر هوك مخصص
  const {
    amenitiesData,
    isLoading: isLoadingAmenities,
    error: amenitiesError,
    createAmenity,
    updateAmenity,
    deleteAmenity,
    assignAmenityToProperty,
  } = useAdminAmenities(queryParams);
  // جلب قائمة الكيانات للربط
  const { propertiesData } = useAdminProperties({});

  // State for modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [showIconPicker, setShowIconPicker] = useState(false);
  const [iconPickerTarget, setIconPickerTarget] = useState<'create' | 'edit' | null>(null);
  const [selectedAmenity, setSelectedAmenity] = useState<AmenityDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);

  // State for forms - بدون category
  const [createForm, setCreateForm] = useState<CreateAmenityCommand>({
    name: '',
    description: '',
    icon: 'wifi', // أيقونة افتراضية
  });

  const [editForm, setEditForm] = useState<UpdateAmenityCommand>({
    amenityId: '',
    name: '',
    description: '',
    icon: 'wifi',
  });

  const [assignForm, setAssignForm] = useState({
    propertyId: '',
    extraCost: { amount: 0, currency: 'YER', formattedAmount: '' } as MoneyDto,
    isAvailable: true,
    description: '',
    icon: '',
  });

  // Helper functions
  const resetCreateForm = () => {
    setCreateForm({
      name: '',
      description: '',
      icon: 'wifi',
    });
  };

  // دالة للحصول على أيقونة من الاسم
  const getIconDisplay = (iconName: string) => {
    const icon = AMENITY_MATERIAL_ICONS.find(i => i.name === iconName);
    return icon ? icon.icon : '🏠';
  };

  // دالة للحصول على أيقونة حسب الاسم (للتوافق مع الكود القديم)
  const getAmenityIcon = (name: string, iconName?: string) => {
    // إذا كان هناك اسم أيقونة محدد، استخدمه
    if (iconName) {
      return getIconDisplay(iconName);
    }
    
    // وإلا ابحث عن أيقونة مناسبة بناءً على الاسم
    const lowerName = name.toLowerCase();
    const matchingIcon = AMENITY_MATERIAL_ICONS.find(icon => 
      lowerName.includes(icon.label.toLowerCase()) || 
      lowerName.includes(icon.name.toLowerCase())
    );
    
    return matchingIcon ? matchingIcon.icon : '🏠';
  };

  const handleViewDetails = (amenity: AmenityDto) => {
    setSelectedAmenity(amenity);
    setShowDetailsModal(true);
  };

  const handleEdit = (amenity: AmenityDto) => {
    setSelectedAmenity(amenity);
    setEditForm({
      amenityId: amenity.id,
      name: amenity.name,
      description: amenity.description,
      icon: amenity.icon || 'wifi',
    });
    setShowEditModal(true);
  };

  const handleDelete = (amenity: AmenityDto) => {
    if (confirm(`هل أنت متأكد من حذف المرفق "${amenity.name}"؟ هذا الإجراء لا يمكن التراجع عنه.`)) {
      deleteAmenity.mutate(amenity.id, {
        onSuccess: () => {
          setShowEditModal(false);
          setSelectedAmenity(null);
        },
      });
    }
  };

  const handleAssignToProperty = (amenity: AmenityDto) => {
    setSelectedAmenity(amenity);
    setShowAssignModal(true);
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      isAssigned: undefined,
      propertyId: undefined,
      isFree: undefined
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  // Handler for icon selection
  const handleIconSelect = (iconName: string) => {
    if (iconPickerTarget === 'create') {
      setCreateForm({ ...createForm, icon: iconName });
    } else if (iconPickerTarget === 'edit') {
      setEditForm({ ...editForm, icon: iconName });
    }
    setShowIconPicker(false);
    setIconPickerTarget(null);
  };

  // Statistics calculation
  const stats = {
    total: amenitiesData?.items?.length || 0,
    totalCount: amenitiesData?.totalCount || 0,
  };

  // Filter options - بدون فلتر الفئة
  const filterOptions: FilterOption[] = [
    {
      key: 'propertyId',
      label: 'الكيان',
      type: 'custom',
      render: (value, onChange) => (
        <PropertySelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر الكيان"
          className="w-full"
        />
      ),
    },
    {
      key: 'isAssigned',
      label: 'مربوط بكيانات',
      type: 'boolean',
    },
    {
      key: 'isFree',
      label: 'مجاني',
      type: 'boolean',
    },
  ];

  // Table columns - بدون عمود الفئة
  const columns: Column<AmenityDto>[] = [
    {
      key: 'name',
      title: 'المرفق',
      sortable: true,
      render: (value: string, record: AmenityDto) => (
        <div className="flex items-center">
          <span className="text-2xl ml-3">{getAmenityIcon(value, record.icon)}</span>
          <div>
            <span className="font-medium text-gray-900">{value}</span>
            <p className="text-sm text-gray-500 mt-1">{record.description}</p>
            {record.icon && (
              <code className="text-xs text-gray-400">Icons.{record.icon}</code>
            )}
          </div>
        </div>
      ),
    },
    {
      key: 'id',
      title: 'المعرف',
      render: (value: string) => (
        <span className="font-mono text-sm text-gray-600">
          {value.substring(0, 8)}...
        </span>
      ),
    },
  ];

  // Table actions
  const tableActions = [
    {
      label: 'عرض التفاصيل',
      icon: '👁️',
      color: 'blue' as const,
      onClick: handleViewDetails,
    },
    {
      label: 'تعديل',
      icon: '✏️',
      color: 'blue' as const,
      onClick: handleEdit,
    },
    {
      label: 'ربط بكيان',
      icon: '🔗',
      color: 'green' as const,
      onClick: handleAssignToProperty,
    },
    {
      label: 'حذف',
      icon: '🗑️',
      color: 'red' as const,
      onClick: handleDelete,
    },
  ];

  if (amenitiesError) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-8 text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">خطأ في تحميل البيانات</h2>
        <p className="text-gray-600">حدث خطأ أثناء تحميل بيانات المرافق. يرجى المحاولة مرة أخرى.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة المرافق</h1>
            <p className="text-gray-600 mt-1">
              إنشاء وتحديث المرافق المتاحة في النظام وربطها بالكيانات المختلفة مع دعم الأيقونات الديناميكية
            </p>
          </div>
          <button
            onClick={() => setShowCreateModal(true)}
            className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            ➕ إضافة مرفق جديد
          </button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-blue-100 p-2 rounded-lg">
              <span className="text-2xl">🏠</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">إجمالي المرافق</p>
              <p className="text-2xl font-bold text-gray-900">{stats.totalCount}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-green-100 p-2 rounded-lg">
              <span className="text-2xl">🔗</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">مربوطة بكيانات</p>
              <p className="text-2xl font-bold text-green-600">-</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-orange-100 p-2 rounded-lg">
              <span className="text-2xl">🎨</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">أيقونات متاحة</p>
              <p className="text-2xl font-bold text-orange-600">{AMENITY_MATERIAL_ICONS.length}</p>
            </div>
          </div>
        </div>
      </div>

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في المرافق (الاسم أو الوصف)..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Amenities Table */}
      <DataTable
        data={amenitiesData?.items || []}
        columns={columns}
        loading={isLoadingAmenities}
        pagination={{
          current: currentPage,
          total: amenitiesData?.totalCount || 0,
          pageSize,
          onChange: (page, size) => {
            setCurrentPage(page);
            setPageSize(size);
          },
        }}
        rowSelection={{
          selectedRowKeys: selectedRows,
          onChange: setSelectedRows,
        }}
        actions={tableActions}
        onRowClick={handleViewDetails}
      />

      {/* Create Amenity Modal */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="إضافة مرفق جديد"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => setShowCreateModal(false)}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => createAmenity.mutate(createForm, {
                onSuccess: () => {
                  setShowCreateModal(false);
                  resetCreateForm();
                },
              })}
              disabled={createAmenity.status === 'pending' || !createForm.name.trim()}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {createAmenity.status === 'pending' ? 'جارٍ الإضافة...' : 'إضافة'}
            </button>
          </div>
        }
      >
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              اسم المرفق *
            </label>
            <input
              type="text"
              value={createForm.name}
              onChange={(e) => setCreateForm(prev => ({ ...prev, name: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل اسم المرفق"
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
                  setIconPickerTarget('create');
                  setShowIconPicker(true);
                }}
                className="flex-1 px-3 py-2 border border-gray-300 rounded-md hover:bg-gray-50 flex items-center justify-between"
              >
                <span className="flex items-center space-x-2 space-x-reverse">
                  <span className="text-xl">{getIconDisplay(createForm.icon)}</span>
                  <span className="text-sm">{createForm.icon}</span>
                </span>
                <span className="text-gray-400">▼</span>
              </button>
            </div>
            <p className="text-xs text-gray-500 mt-1">
              استخدم Icons.{createForm.icon} في Flutter
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              وصف المرفق *
            </label>
            <textarea
              rows={3}
              value={createForm.description}
              onChange={(e) => setCreateForm(prev => ({ ...prev, description: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل وصف تفصيلي للمرفق"
            />
          </div>

          {/* Icon Preview */}
          <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-md">
            <span className="text-3xl">{getIconDisplay(createForm.icon)}</span>
            <div>
              <p className="text-sm font-medium text-gray-700">معاينة الأيقونة</p>
              <p className="text-xs text-gray-500">
                الأيقونة المحددة: <code>Icons.{createForm.icon}</code>
              </p>
            </div>
          </div>
        </div>
      </Modal>

      {/* Edit Amenity Modal */}
      <Modal
        isOpen={showEditModal}
        onClose={() => {
          setShowEditModal(false);
          setSelectedAmenity(null);
        }}
        title="تعديل بيانات المرفق"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowEditModal(false);
                setSelectedAmenity(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => updateAmenity.mutate({ amenityId: editForm.amenityId, data: editForm }, {
                onSuccess: () => {
                  setShowEditModal(false);
                  setSelectedAmenity(null);
                },
              })}
              disabled={updateAmenity.status === 'pending' || !editForm.name?.trim()}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {updateAmenity.status === 'pending' ? 'جارٍ التحديث...' : 'تحديث'}
            </button>
          </div>
        }
      >
        {selectedAmenity && (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                اسم المرفق
              </label>
              <input
                type="text"
                value={editForm.name || ''}
                onChange={(e) => setEditForm(prev => ({ ...prev, name: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
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
                    setIconPickerTarget('edit');
                    setShowIconPicker(true);
                  }}
                  className="flex-1 px-3 py-2 border border-gray-300 rounded-md hover:bg-gray-50 flex items-center justify-between"
                >
                  <span className="flex items-center space-x-2 space-x-reverse">
                    <span className="text-xl">{getIconDisplay(editForm.icon || 'wifi')}</span>
                    <span className="text-sm">{editForm.icon}</span>
                  </span>
                  <span className="text-gray-400">▼</span>
                </button>
              </div>
              <p className="text-xs text-gray-500 mt-1">
                استخدم Icons.{editForm.icon} في Flutter
              </p>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                وصف المرفق
              </label>
              <textarea
                rows={3}
                value={editForm.description || ''}
                onChange={(e) => setEditForm(prev => ({ ...prev, description: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            {/* Icon Preview */}
            <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-md">
              <span className="text-3xl">{getIconDisplay(editForm.icon || 'wifi')}</span>
              <div>
                <p className="text-sm font-medium text-gray-700">معاينة الأيقونة</p>
                <p className="text-xs text-gray-500">
                  الأيقونة المحددة: <code>Icons.{editForm.icon}</code>
                </p>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Amenity Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedAmenity(null);
        }}
        title="تفاصيل المرفق"
        size="lg"
      >
        {selectedAmenity && (
          <div className="space-y-6">
            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-lg">
              <span className="text-6xl">{getAmenityIcon(selectedAmenity.name, selectedAmenity.icon)}</span>
              <div>
                <h3 className="text-2xl font-bold text-gray-900">{selectedAmenity.name}</h3>
                <p className="text-gray-600 mt-1">{selectedAmenity.description}</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف المرفق</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedAmenity.id}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">اسم الأيقونة</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">
                  {selectedAmenity.icon ? `Icons.${selectedAmenity.icon}` : 'غير محدد'}
                </p>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">الوصف</label>
              <p className="mt-1 text-sm text-gray-900">{selectedAmenity.description}</p>
            </div>

            <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-blue-400 text-xl">ℹ️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-blue-800">
                    معلومات إضافية
                  </h3>
                  <p className="mt-2 text-sm text-blue-700">
                    يمكن ربط هذا المرفق بالكيانات مع تحديد تكلفة إضافية وحالة التوفر لكل كيان.
                    الأيقونة المحددة متوافقة مع Material Icons في Flutter.
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Assign to Property Modal */}
      <Modal
        isOpen={showAssignModal}
        onClose={() => {
          setShowAssignModal(false);
          setSelectedAmenity(null);
        }}
        title="ربط المرفق بكيان"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowAssignModal(false);
                setSelectedAmenity(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => assignAmenityToProperty.mutate({ 
                amenityId: selectedAmenity!.id, 
                propertyId: assignForm.propertyId, 
                data: { 
                  amenityId: selectedAmenity!.id, 
                  propertyId: assignForm.propertyId 
                } 
              }, {
                onSuccess: () => {
                  setShowAssignModal(false);
                  setSelectedAmenity(null);
                },
              })}
              disabled={assignAmenityToProperty.status === 'pending' || !assignForm.propertyId}
              className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50"
            >
              {assignAmenityToProperty.status === 'pending' ? 'جارٍ الربط...' : 'ربط المرفق'}
            </button>
          </div>
        }
      >
        {selectedAmenity && (
          <div className="space-y-4">
            <div className="bg-green-50 border border-green-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-green-400 text-xl">🔗</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-green-800">
                    ربط المرفق بكيان
                  </h3>
                  <p className="mt-2 text-sm text-green-700">
                    سيتم ربط المرفق "<strong>{selectedAmenity.name}</strong>" بالكيان المحدد مع إمكانية تحديد تكلفة إضافية.
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                معرف الكيان *
              </label>
              <select
                value={assignForm.propertyId}
                onChange={(e) => setAssignForm(prev => ({ ...prev, propertyId: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
                <option value="">اختر كيان</option>
                {propertiesData?.items.map(p => (
                  <option key={p.id} value={p.id}>{p.name}</option>
                ))}
              </select>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  التكلفة الإضافية
                </label>
                <CurrencyInput
                  value={assignForm.extraCost.amount}
                  currency={assignForm.extraCost.currency}
                  onValueChange={(amount, currency) => setAssignForm(prev => ({
                    ...prev,
                    extraCost: { amount, currency, formattedAmount: '' }
                  }))}
                  placeholder="0"
                  required={false}
                  showSymbol={true}
                  supportedCurrencies={currencyCodes}
                  direction="ltr"
                />
              </div>
            </div>

            <div className="flex items-center">
              <input
                type="checkbox"
                id="isAvailable"
                checked={assignForm.isAvailable}
                onChange={(e) => setAssignForm(prev => ({ ...prev, isAvailable: e.target.checked }))}
                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <label htmlFor="isAvailable" className="mr-2 block text-sm text-gray-900">
                متاح في الكيان
              </label>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                وصف إضافي
              </label>
              <textarea
                rows={2}
                value={assignForm.description}
                onChange={(e) => setAssignForm(prev => ({ ...prev, description: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                placeholder="أدخل وصف إضافي للمرفق في هذا الكيان (اختياري)"
              />
            </div>
          </div>
        )}
      </Modal>

      {/* Icon Picker Modal */}
      {showIconPicker && (
        <AmenityIconPicker
          selectedIcon={iconPickerTarget === 'create' ? createForm.icon : editForm.icon || 'wifi'}
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

export default AdminAmenities;