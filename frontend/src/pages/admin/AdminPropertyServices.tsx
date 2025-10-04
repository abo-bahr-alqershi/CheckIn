import React, { useState } from 'react';
// أيقونات بسيطة بدلاً من lucide-react
const PlusIcon = () => <span>➕</span>;
const EditIcon = () => <span>✏️</span>;
const TrashIcon = () => <span>🗑️</span>;
const SearchIcon = () => <span>🔍</span>;
const BuildingIcon = () => <span>🏢</span>;
const DollarIcon = () => <span>💰</span>;
const TagIcon = () => <span>🏷️</span>;
const EyeIcon = () => <span>👁️</span>;
import {
  usePropertyServices,
  useServiceDetails,
  useServicesByType,
  useCreatePropertyService,
  useUpdatePropertyService,
  useDeletePropertyService,
} from '../../hooks/useAdminPropertyServices';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import type {
  ServiceDto,
  ServiceDetailsDto,
  CreatePropertyServiceCommand,
  UpdatePropertyServiceCommand,
  PricingModel,
} from '../../types/service.types';
import type { MoneyDto } from '../../types/amenity.types';
import DataTable from '../../components/common/DataTable';
import Modal from '../../components/common/Modal';
import PropertySelector from '../../components/selectors/PropertySelector';
import CurrencyInput from '../../components/inputs/CurrencyInput';
import ActionsDropdown from '../../components/ui/ActionsDropdown';
import { useCurrencies } from '../../hooks/useCurrencies';

// قائمة أيقونات Material Icons المتخصصة للخدمات
const SERVICE_MATERIAL_ICONS = [
  // خدمات التنظيف
  { name: 'cleaning_services', label: 'خدمة تنظيف', icon: '🧹', category: 'تنظيف' },
  { name: 'dry_cleaning', label: 'تنظيف جاف', icon: '👔', category: 'تنظيف' },
  { name: 'local_laundry_service', label: 'خدمة غسيل', icon: '🧺', category: 'تنظيف' },
  { name: 'iron', label: 'كوي الملابس', icon: '👔', category: 'تنظيف' },
  { name: 'wash', label: 'غسيل', icon: '🧴', category: 'تنظيف' },
  { name: 'soap', label: 'صابون', icon: '🧼', category: 'تنظيف' },
  { name: 'sanitizer', label: 'معقم', icon: '🧴', category: 'تنظيف' },
  { name: 'plumbing', label: 'سباكة', icon: '🔧', category: 'تنظيف' },
  
  // خدمات الطعام والضيافة
  { name: 'room_service', label: 'خدمة الغرف', icon: '🛎️', category: 'ضيافة' },
  { name: 'restaurant', label: 'مطعم', icon: '🍴', category: 'ضيافة' },
  { name: 'local_cafe', label: 'مقهى', icon: '☕', category: 'ضيافة' },
  { name: 'local_bar', label: 'بار', icon: '🍺', category: 'ضيافة' },
  { name: 'breakfast_dining', label: 'إفطار', icon: '🍳', category: 'ضيافة' },
  { name: 'lunch_dining', label: 'غداء', icon: '🍽️', category: 'ضيافة' },
  { name: 'dinner_dining', label: 'عشاء', icon: '🍽️', category: 'ضيافة' },
  { name: 'delivery_dining', label: 'توصيل طعام', icon: '🚚', category: 'ضيافة' },
  { name: 'takeout_dining', label: 'طعام للخارج', icon: '🥡', category: 'ضيافة' },
  { name: 'ramen_dining', label: 'وجبات سريعة', icon: '🍜', category: 'ضيافة' },
  { name: 'icecream', label: 'آيس كريم', icon: '🍦', category: 'ضيافة' },
  { name: 'cake', label: 'كيك', icon: '🎂', category: 'ضيافة' },
  { name: 'local_pizza', label: 'بيتزا', icon: '🍕', category: 'ضيافة' },
  { name: 'fastfood', label: 'وجبات سريعة', icon: '🍔', category: 'ضيافة' },
  
  // خدمات النقل والمواصلات
  { name: 'airport_shuttle', label: 'نقل مطار', icon: '🚐', category: 'نقل' },
  { name: 'local_taxi', label: 'تاكسي', icon: '🚕', category: 'نقل' },
  { name: 'car_rental', label: 'تأجير سيارات', icon: '🚙', category: 'نقل' },
  { name: 'car_repair', label: 'صيانة سيارات', icon: '🔧', category: 'نقل' },
  { name: 'directions_car', label: 'سيارة خاصة', icon: '🚗', category: 'نقل' },
  { name: 'directions_bus', label: 'حافلة', icon: '🚌', category: 'نقل' },
  { name: 'directions_boat', label: 'قارب', icon: '⛵', category: 'نقل' },
  { name: 'directions_bike', label: 'دراجة', icon: '🚴', category: 'نقل' },
  { name: 'electric_bike', label: 'دراجة كهربائية', icon: '🚴', category: 'نقل' },
  { name: 'electric_scooter', label: 'سكوتر كهربائي', icon: '🛴', category: 'نقل' },
  { name: 'local_shipping', label: 'شحن محلي', icon: '🚚', category: 'نقل' },
  { name: 'local_parking', label: 'موقف سيارات', icon: '🅿️', category: 'نقل' },
  { name: 'valet_parking', label: 'خدمة صف السيارات', icon: '🚗', category: 'نقل' },
  
  // خدمات الاتصالات والإنترنت
  { name: 'wifi', label: 'واي فاي', icon: '📶', category: 'اتصالات' },
  { name: 'wifi_calling', label: 'مكالمات واي فاي', icon: '📞', category: 'اتصالات' },
  { name: 'router', label: 'راوتر', icon: '🔌', category: 'اتصالات' },
  { name: 'phone_in_talk', label: 'خدمة هاتف', icon: '📞', category: 'اتصالات' },
  { name: 'phone_callback', label: 'اتصال مجاني', icon: '📞', category: 'اتصالات' },
  { name: 'support_agent', label: 'دعم العملاء', icon: '🧑‍💼', category: 'اتصالات' },
  { name: 'headset_mic', label: 'خدمة عملاء', icon: '🎧', category: 'اتصالات' },
  { name: 'mail', label: 'بريد', icon: '📧', category: 'اتصالات' },
  { name: 'markunread_mailbox', label: 'صندوق بريد', icon: '📬', category: 'اتصالات' },
  { name: 'print', label: 'طباعة', icon: '🖨️', category: 'اتصالات' },
  { name: 'scanner', label: 'ماسح ضوئي', icon: '📄', category: 'اتصالات' },
  { name: 'fax', label: 'فاكس', icon: '📠', category: 'اتصالات' },
  
  // خدمات الترفيه والاستجمام
  { name: 'spa', label: 'سبا', icon: '💆', category: 'ترفيه' },
  { name: 'hot_tub', label: 'جاكوزي', icon: '♨️', category: 'ترفيه' },
  { name: 'pool', label: 'مسبح', icon: '🏊', category: 'ترفيه' },
  { name: 'fitness_center', label: 'صالة رياضية', icon: '💪', category: 'ترفيه' },
  { name: 'sports_tennis', label: 'تنس', icon: '🎾', category: 'ترفيه' },
  { name: 'sports_golf', label: 'جولف', icon: '⛳', category: 'ترفيه' },
  { name: 'sports_soccer', label: 'كرة قدم', icon: '⚽', category: 'ترفيه' },
  { name: 'sports_basketball', label: 'كرة سلة', icon: '🏀', category: 'ترفيه' },
  { name: 'casino', label: 'كازينو', icon: '🎰', category: 'ترفيه' },
  { name: 'theater_comedy', label: 'مسرح', icon: '🎭', category: 'ترفيه' },
  { name: 'movie', label: 'سينما', icon: '🎬', category: 'ترفيه' },
  { name: 'music_note', label: 'موسيقى', icon: '🎵', category: 'ترفيه' },
  { name: 'nightlife', label: 'حياة ليلية', icon: '🌃', category: 'ترفيه' },
  { name: 'celebration', label: 'احتفالات', icon: '🎉', category: 'ترفيه' },
  
  // خدمات الأعمال
  { name: 'business_center', label: 'مركز أعمال', icon: '💼', category: 'أعمال' },
  { name: 'meeting_room', label: 'قاعة اجتماعات', icon: '👥', category: 'أعمال' },
  { name: 'co_present', label: 'عرض تقديمي', icon: '📊', category: 'أعمال' },
  { name: 'groups', label: 'مجموعات', icon: '👥', category: 'أعمال' },
  { name: 'event', label: 'فعاليات', icon: '📅', category: 'أعمال' },
  { name: 'event_available', label: 'حجز فعاليات', icon: '✅', category: 'أعمال' },
  { name: 'event_seat', label: 'مقاعد فعاليات', icon: '🪑', category: 'أعمال' },
  { name: 'mic', label: 'ميكروفون', icon: '🎤', category: 'أعمال' },
  { name: 'videocam', label: 'كاميرا فيديو', icon: '📹', category: 'أعمال' },
  { name: 'desktop_windows', label: 'كمبيوتر', icon: '💻', category: 'أعمال' },
  { name: 'laptop', label: 'لابتوب', icon: '💻', category: 'أعمال' },
  
  // خدمات الصحة والعناية
  { name: 'medical_services', label: 'خدمات طبية', icon: '🏥', category: 'صحة' },
  { name: 'local_hospital', label: 'مستشفى', icon: '🏥', category: 'صحة' },
  { name: 'local_pharmacy', label: 'صيدلية', icon: '💊', category: 'صحة' },
  { name: 'emergency', label: 'طوارئ', icon: '🚨', category: 'صحة' },
  { name: 'vaccines', label: 'لقاحات', icon: '💉', category: 'صحة' },
  { name: 'healing', label: 'علاج', icon: '❤️‍🩹', category: 'صحة' },
  { name: 'monitor_heart', label: 'مراقبة صحية', icon: '❤️', category: 'صحة' },
  { name: 'health_and_safety', label: 'صحة وأمان', icon: '🏥', category: 'صحة' },
  { name: 'masks', label: 'كمامات', icon: '😷', category: 'صحة' },
  { name: 'sanitizer', label: 'معقم', icon: '🧴', category: 'صحة' },
  { name: 'psychology', label: 'استشارة نفسية', icon: '🧠', category: 'صحة' },
  { name: 'self_improvement', label: 'تطوير ذاتي', icon: '🧘', category: 'صحة' },
  
  // خدمات التسوق
  { name: 'shopping_cart', label: 'عربة تسوق', icon: '🛒', category: 'تسوق' },
  { name: 'shopping_bag', label: 'حقيبة تسوق', icon: '🛍️', category: 'تسوق' },
  { name: 'local_mall', label: 'مول', icon: '🛍️', category: 'تسوق' },
  { name: 'local_grocery_store', label: 'بقالة', icon: '🛒', category: 'تسوق' },
  { name: 'local_convenience_store', label: 'متجر صغير', icon: '🏪', category: 'تسوق' },
  { name: 'store', label: 'متجر', icon: '🏪', category: 'تسوق' },
  { name: 'storefront', label: 'واجهة متجر', icon: '🏪', category: 'تسوق' },
  { name: 'local_offer', label: 'عروض', icon: '🏷️', category: 'تسوق' },
  { name: 'loyalty', label: 'برنامج ولاء', icon: '🎁', category: 'تسوق' },
  { name: 'card_giftcard', label: 'بطاقة هدية', icon: '🎁', category: 'تسوق' },
  
  // خدمات الأطفال والعائلة
  { name: 'child_care', label: 'رعاية أطفال', icon: '👶', category: 'عائلة' },
  { name: 'baby_changing_station', label: 'غرفة تغيير', icon: '👶', category: 'عائلة' },
  { name: 'child_friendly', label: 'صديق للأطفال', icon: '👨‍👩‍👧‍👦', category: 'عائلة' },
  { name: 'toys', label: 'ألعاب', icon: '🧸', category: 'عائلة' },
  { name: 'stroller', label: 'عربة أطفال', icon: '👶', category: 'عائلة' },
  { name: 'family_restroom', label: 'حمام عائلي', icon: '👨‍👩‍👧‍👦', category: 'عائلة' },
  { name: 'escalator_warning', label: 'تحذير أطفال', icon: '⚠️', category: 'عائلة' },
  { name: 'pregnant_woman', label: 'خدمات حوامل', icon: '🤰', category: 'عائلة' },
  
  // خدمات الحيوانات الأليفة
  { name: 'pets', label: 'حيوانات أليفة', icon: '🐾', category: 'حيوانات' },
  { name: 'pet_supplies', label: 'مستلزمات حيوانات', icon: '🐕', category: 'حيوانات' },
  
  // خدمات الأمان
  { name: 'security', label: 'أمن', icon: '🔒', category: 'أمان' },
  { name: 'local_police', label: 'شرطة', icon: '👮', category: 'أمان' },
  { name: 'shield', label: 'حماية', icon: '🛡️', category: 'أمان' },
  { name: 'verified_user', label: 'مستخدم موثق', icon: '✅', category: 'أمان' },
  { name: 'lock', label: 'قفل', icon: '🔒', category: 'أمان' },
  { name: 'key', label: 'مفتاح', icon: '🔑', category: 'أمان' },
  { name: 'doorbell', label: 'جرس الباب', icon: '🔔', category: 'أمان' },
  { name: 'camera_alt', label: 'كاميرا مراقبة', icon: '📷', category: 'أمان' },
  
  // خدمات مالية
  { name: 'local_atm', label: 'صراف آلي', icon: '💳', category: 'مالية' },
  { name: 'account_balance', label: 'بنك', icon: '🏦', category: 'مالية' },
  { name: 'currency_exchange', label: 'صرافة', icon: '💱', category: 'مالية' },
  { name: 'payment', label: 'دفع', icon: '💳', category: 'مالية' },
  { name: 'credit_card', label: 'بطاقة ائتمان', icon: '💳', category: 'مالية' },
  { name: 'account_balance_wallet', label: 'محفظة', icon: '👛', category: 'مالية' },
  { name: 'savings', label: 'توفير', icon: '🏦', category: 'مالية' },
  
  // خدمات أخرى
  { name: 'handshake', label: 'استقبال', icon: '🤝', category: 'أخرى' },
  { name: 'concierge', label: 'كونسيرج', icon: '🧑‍💼', category: 'أخرى' },
  { name: 'bellhop', label: 'حمال', icon: '🧳', category: 'أخرى' },
  { name: 'luggage', label: 'أمتعة', icon: '🧳', category: 'أخرى' },
  { name: 'umbrella', label: 'مظلة', icon: '☂️', category: 'أخرى' },
  { name: 'interpreter', label: 'مترجم', icon: '🗣️', category: 'أخرى' },
  { name: 'translate', label: 'ترجمة', icon: '🌐', category: 'أخرى' },
  { name: 'tour', label: 'جولة سياحية', icon: '🗺️', category: 'أخرى' },
  { name: 'map', label: 'خريطة', icon: '🗺️', category: 'أخرى' },
  { name: 'info', label: 'معلومات', icon: 'ℹ️', category: 'أخرى' },
];

// مكون اختيار الأيقونة للخدمات
const ServiceIconPicker = ({ 
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
  
  const categories = ['الكل', ...new Set(SERVICE_MATERIAL_ICONS.map(icon => icon.category))];
  
  const filteredIcons = SERVICE_MATERIAL_ICONS.filter(icon => {
    const matchesSearch = icon.label.toLowerCase().includes(searchTerm.toLowerCase()) ||
                          icon.name.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory = selectedCategory === 'الكل' || icon.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });
  
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-[60]">
      <div className="bg-white rounded-lg p-6 w-full max-w-5xl max-h-[90vh] overflow-hidden flex flex-col">
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold">اختر أيقونة للخدمة</h3>
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
            استخدم اسم الأيقونة (مثل: Icons.{selectedIcon || 'room_service'}) في تطبيق Flutter.
          </p>
        </div>
      </div>
    </div>
  );
};

const AdminPropertyServices = () => {
  const [selectedPropertyId, setSelectedPropertyId] = useState<string>('');
  const [selectedServiceType, setSelectedServiceType] = useState<string>('');
  const [searchQuery, setSearchQuery] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [showIconPicker, setShowIconPicker] = useState(false);
  const [iconPickerTarget, setIconPickerTarget] = useState<'create' | 'edit' | null>(null);
  
  // Fetch currencies for price component
  const { currencies, loading: currenciesLoading } = useCurrencies();
  const currencyCodes = currenciesLoading ? [] : currencies.map(c => c.code);
  
  // حالات المودالات
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [selectedService, setSelectedService] = useState<ServiceDto | null>(null);

  // بيانات النماذج - بدون category
  const [createForm, setCreateForm] = useState<CreatePropertyServiceCommand>({
    propertyId: '',
    name: '',
    price: { amount: 0, currency: 'SAR' },
    pricingModel: 'PerBooking',
    icon: 'room_service', // أيقونة افتراضية
  });

  const [editForm, setEditForm] = useState<UpdatePropertyServiceCommand>({
    serviceId: '',
    name: '',
    price: { amount: 0, currency: 'SAR' },
    pricingModel: 'PerBooking',
    icon: 'room_service',
  });

  // دالة للحصول على أيقونة من الاسم
  const getIconDisplay = (iconName: string) => {
    const icon = SERVICE_MATERIAL_ICONS.find(i => i.name === iconName);
    return icon ? icon.icon : '🛎️';
  };

  // دالة للحصول على أيقونة الخدمة
  const getServiceIcon = (name: string, iconName?: string) => {
    // إذا كان هناك اسم أيقونة محدد، استخدمه
    if (iconName) {
      return getIconDisplay(iconName);
    }
    
    // وإلا ابحث عن أيقونة مناسبة بناءً على الاسم
    const lowerName = name.toLowerCase();
    const matchingIcon = SERVICE_MATERIAL_ICONS.find(icon => 
      lowerName.includes(icon.label.toLowerCase()) || 
      lowerName.includes(icon.name.toLowerCase())
    );
    
    return matchingIcon ? matchingIcon.icon : '🛎️';
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

  // جلب البيانات
  // Fetch properties with maximum allowed page size (API supports up to 100)
  const { propertiesData: properties } = useAdminProperties({
    pageNumber: 1,
    pageSize: 100,
  });

  const { data: propertyServices, isLoading: isLoadingPropertyServices } = usePropertyServices({
    propertyId: selectedPropertyId,
  });

  const { data: servicesByType, isLoading: isLoadingServicesByType } = useServicesByType({
    serviceType: selectedServiceType,
    pageNumber,
    pageSize,
  });

  const { data: serviceDetails } = useServiceDetails({
    serviceId: selectedService?.id || '',
  });

  // الطفرات
  const createMutation = useCreatePropertyService();
  const updateMutation = useUpdatePropertyService();
  const deleteMutation = useDeletePropertyService();

  // خيارات نماذج التسعير
  const pricingModelOptions = [
    { value: 'PerBooking', label: 'لكل حجز' },
    { value: 'PerDay', label: 'لكل يوم' },
    { value: 'PerPerson', label: 'لكل شخص' },
    { value: 'PerUnit', label: 'لكل وحدة' },
    { value: 'PerHour', label: 'لكل ساعة' },
    { value: 'Fixed', label: 'سعر ثابت' },
  ];

  // دالة لتحديد البيانات المعروضة
  const getDisplayData = () => {
    if (selectedPropertyId && propertyServices?.success && propertyServices.data) {
      return propertyServices.data;
    }
    if (selectedServiceType && servicesByType?.items) {
      return servicesByType.items;
    }
    return [];
  };

  const isLoading = selectedPropertyId ? isLoadingPropertyServices : isLoadingServicesByType;

  // التعامل مع الأحداث
  const handleCreate = () => {
    setCreateForm({
      propertyId: selectedPropertyId || '',
      name: '',
      price: { amount: 0, currency: 'SAR' },
      pricingModel: 'PerBooking',
      icon: 'room_service',
    });
    setIsCreateModalOpen(true);
  };

  const handleEdit = (service: ServiceDto) => {
    setSelectedService(service);
    setEditForm({
      serviceId: service.id,
      name: service.name,
      price: service.price,
      pricingModel: Object.keys(service.pricingModel)[0] as string,
      icon: service.icon || 'room_service',
    });
    setIsEditModalOpen(true);
  };

  const handleDelete = (service: ServiceDto) => {
    if (confirm('هل أنت متأكد من حذف هذه الخدمة؟')) {
      deleteMutation.mutate(service.id, {
        onSuccess: () => {
          // إشعار نجاح
        },
        onError: () => {
          // إشعار خطأ
        }
      });
    }
  };

  const handleViewDetails = (service: ServiceDto) => {
    setSelectedService(service);
    setIsDetailsModalOpen(true);
  };

  const handleCreateSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    createMutation.mutate(createForm, {
      onSuccess: () => {
        setIsCreateModalOpen(false);
      },
      onError: () => {
        // إشعار خطأ
      }
    });
  };

  const handleEditSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateMutation.mutate({
      serviceId: editForm.serviceId,
      data: editForm,
    }, {
      onSuccess: () => {
        setIsEditModalOpen(false);
      },
      onError: () => {
        // إشعار خطأ
      }
    });
  };

  // أعمدة الجدول
  const columns = [
    {
      header: 'الخدمة',
      title: 'الخدمة',
      key: 'name',
      render: (service: ServiceDto) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center text-xl">
            {getServiceIcon(service.name, service.icon)}
          </div>
          <div>
            <div className="font-medium text-gray-900">{service.name}</div>
            {service.icon && (
              <code className="text-xs text-gray-400">Icons.{service.icon}</code>
            )}
          </div>
        </div>
      ),
    },
    {
      header: 'العقار',
      title: 'العقار',
      key: 'property',
      render: (service: ServiceDto) => (
        <div className="flex items-center gap-2">
          <BuildingIcon />
          <div>
            <div className="font-medium">{service.propertyName}</div>
            <div className="text-sm text-gray-500">{service.propertyId.substring(0, 8)}...</div>
          </div>
        </div>
      ),
    },
    {
      header: 'السعر',
      title: 'السعر',
      key: 'price',
      render: (service: ServiceDto) => (
        <div className="flex items-center gap-2">
          <DollarIcon />
          <div>
            <div className="font-medium text-green-600">
              {service.price.amount} {service.price.currency}
            </div>
            <div className="text-sm text-gray-500">
              {pricingModelOptions.find(option => 
                option.value === Object.keys(service.pricingModel)[0]
              )?.label}
            </div>
          </div>
        </div>
      ),
    },
    {
      header: 'الإجراءات',
      title: 'الإجراءات',
      key: 'actions',
      render: (service: ServiceDto) => (
        <ActionsDropdown
          actions={[
            {
              label: 'عرض التفاصيل',
              icon: '👁️',
              onClick: () => handleViewDetails(service),
            },
            {
              label: 'تعديل',
              icon: '✏️',
              onClick: () => handleEdit(service),
            },
            {
              label: 'حذف',
              icon: '🗑️',
              onClick: () => handleDelete(service),
              variant: 'danger',
            },
          ]}
        />
      ),
    },
  ];

  const filteredData = getDisplayData().filter(service =>
    service.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    service.propertyName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  // حساب الإحصائيات
  const stats = {
    totalServices: filteredData.length,
    totalIcons: SERVICE_MATERIAL_ICONS.length,
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">إدارة الخدمات</h1>
          <p className="text-gray-600">إدارة خدمات العقارات والتحكم في الأسعار مع دعم الأيقونات الديناميكية</p>
        </div>
        <button
          onClick={handleCreate}
          disabled={!selectedPropertyId}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <PlusIcon />
          إضافة خدمة جديدة
        </button>
      </div>

      {/* بطاقات الإحصائيات */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-blue-100 p-2 rounded-lg">
              <span className="text-2xl">🛎️</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">إجمالي الخدمات</p>
              <p className="text-2xl font-bold text-gray-900">{stats.totalServices}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-green-100 p-2 rounded-lg">
              <span className="text-2xl">💰</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">خدمات مدفوعة</p>
              <p className="text-2xl font-bold text-green-600">
                {filteredData.filter(s => s.price.amount > 0).length}
              </p>
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
              <p className="text-2xl font-bold text-orange-600">{stats.totalIcons}</p>
            </div>
          </div>
        </div>
      </div>

      {/* الفلاتر */}
      <div className="bg-white rounded-lg shadow p-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              العقار
            </label>
            <PropertySelector
              value={selectedPropertyId}
              onChange={(id) => { setSelectedPropertyId(id); setSelectedServiceType(''); }}
              placeholder="اختر العقار"
              className="w-full"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              نوع الخدمة
            </label>
            <input
              type="text"
              value={selectedServiceType}
              onChange={(e) => {
                setSelectedServiceType(e.target.value);
                setSelectedPropertyId('');
              }}
              placeholder="ادخل نوع الخدمة"
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              البحث
            </label>
            <div className="relative">
              <div className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">
                <SearchIcon />
              </div>
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="البحث في الخدمات..."
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>
        </div>

        {!selectedPropertyId && !selectedServiceType && (
          <div className="text-center py-8 text-gray-500">
            الرجاء اختيار عقار أو إدخال نوع خدمة لعرض النتائج
          </div>
        )}
      </div>

      {/* الجدول */}
      {(selectedPropertyId || selectedServiceType) && (
        <div className="bg-white rounded-lg shadow">
          <DataTable
            data={filteredData}
            columns={columns}
            loading={isLoading}
            pagination={selectedServiceType ? {
              current: pageNumber,
              total: servicesByType?.totalPages || 1,
              pageSize,
              onChange: (page, size) => {
                setPageNumber(page);
                setPageSize(size);
              },
            } : undefined}
            onRowClick={() => {}}
          />
        </div>
      )}

      {/* مودال إنشاء خدمة */}
      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        title="إضافة خدمة جديدة"
        size="lg"
      >
        <form onSubmit={handleCreateSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              العقار
            </label>
            <PropertySelector
              value={createForm.propertyId}
              onChange={(id) => setCreateForm(prev => ({ ...prev, propertyId: id }))}
              required
              className="w-full"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              اسم الخدمة
            </label>
            <input
              type="text"
              value={createForm.name}
              onChange={(e) => setCreateForm(prev => ({ ...prev, name: e.target.value }))}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                السعر
              </label>
              <CurrencyInput
                value={createForm.price.amount}
                currency={createForm.price.currency}
                supportedCurrencies={currencyCodes}
                onValueChange={(amount, currency) => setCreateForm(prev => ({
                  ...prev,
                  price: { amount, currency }
                }))}
                required
                className="w-full"
                min={0}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                نموذج التسعير
              </label>
              <select
                value={createForm.pricingModel}
                onChange={(e) => setCreateForm(prev => ({ ...prev, pricingModel: e.target.value }))}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                {pricingModelOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* معاينة الأيقونة */}
          <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-md">
            <span className="text-3xl">{getIconDisplay(createForm.icon)}</span>
            <div>
              <p className="text-sm font-medium text-gray-700">معاينة الأيقونة</p>
              <p className="text-xs text-gray-500">
                الأيقونة المحددة: <code>Icons.{createForm.icon}</code>
              </p>
            </div>
          </div>

          <div className="flex gap-2 pt-4">
            <button
              type="submit"
              disabled={createMutation.isPending}
              className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
            >
              {createMutation.isPending ? 'جاري الإنشاء...' : 'إنشاء'}
            </button>
            <button
              type="button"
              onClick={() => setIsCreateModalOpen(false)}
              className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
            >
              إلغاء
            </button>
          </div>
        </form>
      </Modal>

      {/* مودال تعديل خدمة */}
      <Modal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        title="تعديل الخدمة"
        size="lg"
      >
        <form onSubmit={handleEditSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              اسم الخدمة
            </label>
            <input
              type="text"
              value={editForm.name}
              onChange={(e) => setEditForm(prev => ({ ...prev, name: e.target.value }))}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
                  <span className="text-xl">{getIconDisplay(editForm.icon || 'room_service')}</span>
                  <span className="text-sm">{editForm.icon}</span>
                </span>
                <span className="text-gray-400">▼</span>
              </button>
            </div>
            <p className="text-xs text-gray-500 mt-1">
              استخدم Icons.{editForm.icon} في Flutter
            </p>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                السعر
              </label>
              <CurrencyInput
                value={editForm.price?.amount ?? 0}
                currency={editForm.price?.currency ?? 'SAR'}
                supportedCurrencies={currencyCodes}
                onValueChange={(amount, currency) => setEditForm(prev => ({
                  ...prev,
                  price: { amount, currency }
                }))}
                required
                className="w-full"
                min={0}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                نموذج التسعير
              </label>
              <select
                value={editForm.pricingModel}
                onChange={(e) => setEditForm(prev => ({ ...prev, pricingModel: e.target.value }))}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                {pricingModelOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* معاينة الأيقونة */}
          <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-md">
            <span className="text-3xl">{getIconDisplay(editForm.icon || 'room_service')}</span>
            <div>
              <p className="text-sm font-medium text-gray-700">معاينة الأيقونة</p>
              <p className="text-xs text-gray-500">
                الأيقونة المحددة: <code>Icons.{editForm.icon}</code>
              </p>
            </div>
          </div>

          <div className="flex gap-2 pt-4">
            <button
              type="submit"
              disabled={updateMutation.isPending}
              className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
            >
              {updateMutation.isPending ? 'جاري التحديث...' : 'تحديث'}
            </button>
            <button
              type="button"
              onClick={() => setIsEditModalOpen(false)}
              className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
            >
              إلغاء
            </button>
          </div>
        </form>
      </Modal>

      {/* مودال تفاصيل الخدمة */}
      <Modal
        isOpen={isDetailsModalOpen}
        onClose={() => setIsDetailsModalOpen(false)}
        title="تفاصيل الخدمة"
        size="lg"
      >
        {selectedService && serviceDetails?.success && (
          <div className="space-y-6">
            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-lg">
              <span className="text-6xl">{getServiceIcon(serviceDetails.data.name, serviceDetails.data.icon)}</span>
              <div>
                <h3 className="text-2xl font-bold text-gray-900">{serviceDetails.data.name}</h3>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-4 bg-gray-50 rounded-lg">
              <div>
                <div className="text-sm text-gray-500">معرف الخدمة</div>
                <div className="font-medium">{serviceDetails.data.id}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">اسم الأيقونة</div>
                <div className="font-medium font-mono">
                  {serviceDetails.data.icon ? `Icons.${serviceDetails.data.icon}` : 'غير محدد'}
                </div>
              </div>
              <div>
                <div className="text-sm text-gray-500">العقار</div>
                <div className="font-medium">{serviceDetails.data.propertyName}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">معرف العقار</div>
                <div className="font-medium">{serviceDetails.data.propertyId}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">السعر</div>
                <div className="font-medium text-green-600">
                  {serviceDetails.data.price.amount} {serviceDetails.data.price.currency}
                </div>
              </div>
              <div>
                <div className="text-sm text-gray-500">نموذج التسعير</div>
                <div className="font-medium">
                  {pricingModelOptions.find(option => 
                    option.value === Object.keys(serviceDetails.data.pricingModel)[0]
                  )?.label}
                </div>
              </div>
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
                    الأيقونة المحددة متوافقة مع Material Icons في Flutter ويمكن استخدامها مباشرة في التطبيق.
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Icon Picker Modal */}
      {showIconPicker && (
        <ServiceIconPicker
          selectedIcon={iconPickerTarget === 'create' ? createForm.icon : editForm.icon || 'room_service'}
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

export default AdminPropertyServices;