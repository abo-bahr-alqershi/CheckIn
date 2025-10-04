import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import { PropertyUsersService } from '../../services/property-users.service';
import { PropertyTypesService } from '../../services/property-types.service';
import type { RegisterPropertyOwnerCommand } from '../../types/user.types';
import { useNotificationContext } from '../../components/ui/NotificationProvider';
import LocationSelector from '../../components/selectors/LocationSelector';
import PublicRoute from '../../components/auth/PublicRoute';

const Register: React.FC = () => {
  const navigate = useNavigate();
  const { showSuccess, showError } = useNotificationContext();
  const [selectedPropertyType, setSelectedPropertyType] = useState('');
  const [isAnimating, setIsAnimating] = useState(false);
  
  const [formData, setFormData] = useState<RegisterPropertyOwnerCommand>({
    name: '',
    email: '',
    password: '',
    phone: '',
    propertyTypeId: '',
    propertyName: '',
    description: '',
    address: '',
    city: '',
    latitude: undefined,
    longitude: undefined,
    starRating: 3
  });
  
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [errors, setErrors] = useState<Partial<RegisterPropertyOwnerCommand & { confirmPassword: string }>>({});
  const [acceptTerms, setAcceptTerms] = useState(false);

  // Property type themes
  const propertyTypeThemes = {
    'hotel': {
      gradient: 'from-blue-500 via-purple-500 to-pink-500',
      bgImage: 'bg-gradient-to-br from-blue-50 via-purple-50 to-pink-50',
      icon: '🏨',
      title: 'انضم إلى شبكة الفنادق الرائدة',
      subtitle: 'اجعل فندقك وجهة مفضلة للمسافرين والزوار',
      benefits: ['إدارة ذكية للحجوزات', 'تحليلات متقدمة للإيرادات', 'نظام تقييم موثوق', 'دعم فني 24/7'],
      cta: 'ابدأ رحلتك في صناعة الضيافة',
      animation: 'animate-pulse'
    },
    'car-rental': {
      gradient: 'from-green-500 via-emerald-500 to-teal-500',
      bgImage: 'bg-gradient-to-br from-green-50 via-emerald-50 to-teal-50',
      icon: '🚗',
      title: 'قد أسطول سياراتك نحو النجاح',
      subtitle: 'منصة متطورة لإدارة تأجير السيارات بكفاءة عالية',
      benefits: ['تتبع دقيق للمركبات', 'إدارة عقود التأجير', 'تقارير مالية شاملة', 'صيانة مجدولة'],
      cta: 'احجز مكانك في سوق تأجير السيارات',
      animation: 'animate-bounce'
    },
    'chalet': {
      gradient: 'from-orange-500 via-amber-500 to-yellow-500',
      bgImage: 'bg-gradient-to-br from-orange-50 via-amber-50 to-yellow-50',
      icon: '🏡',
      title: 'اجعل شاليهك ومزرعتك واحة للاسترخاء',
      subtitle: 'منصة حصرية لإدارة الشاليهات والمزارع السياحية',
      benefits: ['حجوزات المناسبات الخاصة', 'إدارة المرافق والخدمات', 'تسويق موسمي ذكي', 'ضمان الجودة'],
      cta: 'حول ممتلكاتك إلى مصدر دخل مستدام',
      animation: 'animate-swing'
    },
    'sports-lounge': {
      gradient: 'from-red-500 via-rose-500 to-pink-500',
      bgImage: 'bg-gradient-to-br from-red-50 via-rose-50 to-pink-50',
      icon: '⚽',
      title: 'ناديك الرياضي.. مركز للإثارة والمتعة',
      subtitle: 'إدارة احترافية للنوادي الرياضية ومراكز الترفيه',
      benefits: ['جدولة الفعاليات الرياضية', 'إدارة الملاعب والمرافق', 'نظام العضويات', 'تنظيم البطولات'],
      cta: 'كن شريكنا وضاعف ربحك من ناديك الرياضي',
      animation: 'animate-ping'
    },
    'majlis': {
      gradient: 'from-green-500 via-emerald-500 to-teal-500',
      bgImage: 'bg-gradient-to-br from-green-50 via-emerald-50 to-teal-50',
      icon: '🍃',
      title: 'مجلسك التراثي.. أصالة وضيافة',
      subtitle: 'منصة مخصصة لإدارة حجوزات المجالس والطيرمانات',
      benefits: ['حجوزات جمعات الاصدقاء', 'إدارة الخدمات التقليدية', 'الحفاظ على الطابع الأصيل', 'ضيافة راقية'],
      cta: 'بوابتك الاوسع لجلب ضيوفك',
      animation: 'animate-sway'
    }
  };

  useEffect(() => {
    if (selectedPropertyType) {
      setIsAnimating(true);
      const timer = setTimeout(() => setIsAnimating(false), 800);
      return () => clearTimeout(timer);
    }
  }, [selectedPropertyType]);

  const { data: propertyTypesData, isLoading: typesLoading, error: typesError } = useQuery({
    queryKey: ['property-types'],
    queryFn: () => PropertyTypesService.getAll({ pageNumber: 1, pageSize: 100 }),
  });

  const registerMutation = useMutation({
    mutationFn: PropertyUsersService.registerPropertyOwner,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم إنشاء الحساب بنجاح! سيتم توجيهك لتسجيل الدخول');
        navigate('/auth/login');
      } else {
        showError(result.message || 'فشل في إنشاء الحساب');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء إنشاء الحساب');
    }
  });

  const validateForm = (): boolean => {
    const newErrors: Partial<RegisterPropertyOwnerCommand & { confirmPassword: string }> = {};
    
    if (!formData.name.trim()) {
      newErrors.name = 'الاسم مطلوب';
    }
    
    if (!formData.email.trim()) {
      newErrors.email = 'البريد الإلكتروني مطلوب';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'البريد الإلكتروني غير صحيح';
    }
    
    if (!formData.phone.trim()) {
      newErrors.phone = 'رقم الهاتف مطلوب';
    } else if (!/^[0-9+\-\s()]+$/.test(formData.phone)) {
      newErrors.phone = 'رقم الهاتف غير صحيح';
    }
    
    if (!formData.password.trim()) {
      newErrors.password = 'كلمة المرور مطلوبة';
    } else if (formData.password.length < 6) {
      newErrors.password = 'كلمة المرور يجب أن تكون 6 أحرف على الأقل';
    }
    
    if (!confirmPassword.trim()) {
      newErrors.confirmPassword = 'تأكيد كلمة المرور مطلوب';
    } else if (formData.password !== confirmPassword) {
      newErrors.confirmPassword = 'كلمة المرور غير متطابقة';
    }
    
    if (!formData.propertyTypeId) {
      newErrors.propertyTypeId = 'نوع الكيان مطلوب';
    }
    
    if (!formData.propertyName.trim()) {
      newErrors.propertyName = 'اسم الكيان مطلوب';
    }
    
    if (!formData.address.trim()) {
      newErrors.address = 'عنوان الكيان مطلوب';
    }
    
    if (!formData.city.trim()) {
      newErrors.city = 'المدينة مطلوبة';
    }
    
    if (!acceptTerms) {
      showError('يجب الموافقة على الشروط والأحكام');
      return false;
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateForm()) {
      registerMutation.mutate(formData);
    }
  };

  const handleInputChange = (field: keyof RegisterPropertyOwnerCommand, value: string | number) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    
    if (field === 'propertyTypeId') {
      const selectedType = propertyTypesData?.items.find(type => type.id === value);
      if (selectedType) {
        const typeKey = getPropertyTypeKey(selectedType.name);
        setSelectedPropertyType(typeKey);
      }
    }
    
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const getPropertyTypeKey = (typeName: string): string => {
    const lowerName = typeName.toLowerCase();
    if (lowerName.includes('فند') || lowerName.includes('hotel')) return 'hotel';
    if (lowerName.includes('سيار') || lowerName.includes('car')) return 'car-rental';
    if (lowerName.includes('شال') || lowerName.includes('مزرع') || lowerName.includes('chalet')) return 'chalet';
    if (lowerName.includes('رياض') || lowerName.includes('sport')) return 'sports-lounge';
    if (lowerName.includes('مجل') || lowerName.includes('قات') || lowerName.includes('majlis')) return 'majlis';
    return 'hotel'; // default
  };

  const getCurrentTheme = () => {
    return propertyTypeThemes[selectedPropertyType as keyof typeof propertyTypeThemes] || propertyTypeThemes.hotel;
  };

  const currentTheme = getCurrentTheme();

  return (
    <PublicRoute restricted={true}>
      <div className="min-h-screen bg-white flex flex-col lg:flex-row">
      {/* Mobile Header - Shows only on mobile */}
      <div className="lg:hidden w-full bg-gradient-to-br from-slate-900 via-blue-900 to-indigo-900 relative overflow-hidden py-6 px-4">
        <div className="relative z-10 text-center">
          {selectedPropertyType ? (
            <div className="space-y-3">
              <div className={`inline-flex items-center justify-center w-16 h-16 rounded-xl bg-gradient-to-r ${currentTheme.gradient} text-white text-2xl shadow-lg transform transition-all duration-500 ${currentTheme.animation}`}>
                {currentTheme.icon}
              </div>
              <div>
                <h2 className="text-lg font-bold text-white mb-1 leading-tight">
                  {currentTheme.title}
                </h2>
                <p className="text-sm text-blue-100 leading-relaxed">
                  {currentTheme.subtitle}
                </p>
              </div>
            </div>
          ) : (
            <div className="space-y-3">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-xl bg-gradient-to-r from-blue-600 to-purple-600 text-white text-2xl shadow-lg">
                🏢
              </div>
              <div>
                <h2 className="text-lg font-bold text-white mb-1">
                  منصة BookN
                </h2>
                <p className="text-sm text-blue-100">
                  حلول ذكية وشاملة لإدارة جميع أنواع خدمات الحجوزات والتأجير
                </p>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Left Side - Registration Form */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center px-4 sm:px-8 lg:px-12 py-4 lg:py-8 bg-white relative overflow-hidden">
        {/* Subtle background pattern */}
        <div className="absolute inset-0 opacity-5">
          <div className="absolute inset-0" style={{
            backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23000000' fill-opacity='0.1'%3E%3Ccircle cx='30' cy='30' r='2'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`
          }} />
        </div>

        <div className="relative z-10 max-w-md mx-auto w-full">
          {/* Logo and Header */}
          <div className="text-center mb-6 lg:mb-8">
            <div className="w-12 h-12 sm:w-16 sm:h-16 bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl flex items-center justify-center text-white text-lg sm:text-2xl font-bold mx-auto mb-3 sm:mb-4 shadow-lg">
              B
            </div>
            <h1 className="text-2xl sm:text-3xl font-bold text-gray-900 mb-2">إنشاء حساب جديد</h1>
            <p className="text-sm sm:text-base text-gray-600">انضم إلى منصة BookN واستمتع بإدارة احترافية</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-4 lg:space-y-6">
            {/* Personal Information Section */}
            <div className="space-y-3 lg:space-y-4">
              <h3 className="text-base lg:text-lg font-semibold text-gray-800 border-b border-gray-200 pb-2">البيانات الشخصية</h3>
              
              <div className="space-y-3 lg:space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    الاسم الكامل
                  </label>
                  <input
                    type="text"
                    value={formData.name}
                    onChange={(e) => handleInputChange('name', e.target.value)}
                    placeholder="أدخل اسمك الكامل"
                    disabled={registerMutation.isPending}
                    className={`w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                      errors.name ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                    }`}
                  />
                  {errors.name && (
                    <p className="mt-1 text-sm text-red-600">{errors.name}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    رقم الهاتف
                  </label>
                  <input
                    type="tel"
                    value={formData.phone}
                    onChange={(e) => handleInputChange('phone', e.target.value)}
                    placeholder="781272968"
                    disabled={registerMutation.isPending}
                    className={`w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors text-left ${
                      errors.phone ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                    }`}
                    dir="ltr"
                  />
                  {errors.phone && (
                    <p className="mt-1 text-sm text-red-600">{errors.phone}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    البريد الإلكتروني
                  </label>
                  <input
                    type="email"
                    value={formData.email}
                    onChange={(e) => handleInputChange('email', e.target.value)}
                    placeholder="owner@example.com"
                    disabled={registerMutation.isPending}
                    className={`w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors text-left ${
                      errors.email ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                    }`}
                    dir="ltr"
                  />
                  {errors.email && (
                    <p className="mt-1 text-sm text-red-600">{errors.email}</p>
                  )}
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 lg:gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      كلمة المرور
                    </label>
                    <div className="relative">
                      <input
                        type={showPassword ? 'text' : 'password'}
                        value={formData.password}
                        onChange={(e) => handleInputChange('password', e.target.value)}
                        placeholder="كلمة المرور"
                        disabled={registerMutation.isPending}
                        className={`w-full px-3 py-2 lg:px-4 lg:py-3 pr-10 lg:pr-12 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors text-left ${
                          errors.password ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                        }`}
                        dir="ltr"
                      />
                      <button
                        type="button"
                        onClick={() => setShowPassword(!showPassword)}
                        className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                        disabled={registerMutation.isPending}
                      >
                        {showPassword ? 'إخفاء' : 'إظهار'}
                      </button>
                    </div>
                    {errors.password && (
                      <p className="mt-1 text-sm text-red-600">{errors.password}</p>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      تأكيد كلمة المرور
                    </label>
                    <div className="relative">
                      <input
                        type={showConfirmPassword ? 'text' : 'password'}
                        value={confirmPassword}
                        onChange={(e) => {
                          setConfirmPassword(e.target.value);
                          if (errors.confirmPassword) {
                            setErrors(prev => ({ ...prev, confirmPassword: undefined }));
                          }
                        }}
                        placeholder="تأكيد كلمة المرور"
                        disabled={registerMutation.isPending}
                        className={`w-full px-3 py-2 lg:px-4 lg:py-3 pr-10 lg:pr-12 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors text-left ${
                          errors.confirmPassword ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                        }`}
                        dir="ltr"
                      />
                      <button
                        type="button"
                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                        className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                        disabled={registerMutation.isPending}
                      >
                        {showConfirmPassword ? 'إخفاء' : 'إظهار'}
                      </button>
                    </div>
                    {errors.confirmPassword && (
                      <p className="mt-1 text-sm text-red-600">{errors.confirmPassword}</p>
                    )}
                  </div>
                </div>
              </div>
            </div>

            {/* Property Information Section */}
            <div className="space-y-3 lg:space-y-4">
              <h3 className="text-base lg:text-lg font-semibold text-gray-800 border-b border-gray-200 pb-2">بيانات الكيان</h3>
              
              <div className="space-y-3 lg:space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    نوع الكيان
                  </label>
                  <select
                    value={formData.propertyTypeId}
                    onChange={(e) => handleInputChange('propertyTypeId', e.target.value)}
                    disabled={registerMutation.isPending || typesLoading}
                    className={`w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                      errors.propertyTypeId ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                    }`}
                  >
                    <option value="">اختر نوع الكيان</option>
                    {propertyTypesData?.items.map(type => (
                      <option key={type.id} value={type.id}>{type.name}</option>
                    ))}
                  </select>
                  {typesError && <p className="mt-1 text-sm text-red-600">خطأ في تحميل أنواع الكيانات</p>}
                  {errors.propertyTypeId && (
                    <p className="mt-1 text-sm text-red-600">{errors.propertyTypeId}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    اسم الكيان
                  </label>
                  <input
                    type="text"
                    value={formData.propertyName}
                    onChange={(e) => handleInputChange('propertyName', e.target.value)}
                    placeholder="فندق الراحة"
                    disabled={registerMutation.isPending}
                    className={`w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                      errors.propertyName ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                    }`}
                  />
                  {errors.propertyName && (
                    <p className="mt-1 text-sm text-red-600">{errors.propertyName}</p>
                  )}
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 lg:gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      المدينة
                    </label>
                    <input
                      type="text"
                      value={formData.city}
                      onChange={(e) => handleInputChange('city', e.target.value)}
                      placeholder="صنعاء"
                      disabled={registerMutation.isPending}
                      className={`w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                        errors.city ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                      }`}
                    />
                    {errors.city && (
                      <p className="mt-1 text-sm text-red-600">{errors.city}</p>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      التقييم الأولي
                    </label>
                    <select
                      value={formData.starRating}
                      onChange={(e) => handleInputChange('starRating', parseInt(e.target.value))}
                      disabled={registerMutation.isPending}
                      className="w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                    >
                      <option value={1}>نجمة واحدة</option>
                      <option value={2}>نجمتان</option>
                      <option value={3}>3 نجوم</option>
                      <option value={4}>4 نجوم</option>
                      <option value={5}>5 نجوم</option>
                    </select>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    عنوان الكيان
                  </label>
                  <input
                    type="text"
                    value={formData.address}
                    onChange={(e) => handleInputChange('address', e.target.value)}
                    placeholder="مثل : الاصبحي شارع المقالح"
                    disabled={registerMutation.isPending}
                    className={`w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                      errors.address ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : ''
                    }`}
                  />
                  {errors.address && (
                    <p className="mt-1 text-sm text-red-600">{errors.address}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    الموقع الجغرافي
                  </label>
                  <LocationSelector
                    latitude={formData.latitude}
                    longitude={formData.longitude}
                    onChange={(lat, lng) => {
                      handleInputChange('latitude', lat);
                      handleInputChange('longitude', lng);
                    }}
                    placeholder="حدد موقع الكيان"
                    disabled={registerMutation.isPending}
                    showMap={true}
                    allowManualInput={true}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    وصف الكيان (اختياري)
                  </label>
                  <textarea
                    value={formData.description || ''}
                    onChange={(e) => handleInputChange('description', e.target.value)}
                    placeholder="وصف مختصر عن الكيان وخدماته"
                    disabled={registerMutation.isPending}
                    rows={3}
                    className="w-full px-3 py-2 lg:px-4 lg:py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                  />
                </div>
              </div>
            </div>

            <div className="flex items-start space-x-3 space-x-reverse">
              <input
                type="checkbox"
                checked={acceptTerms}
                onChange={(e) => setAcceptTerms(e.target.checked)}
                className="mt-1 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                disabled={registerMutation.isPending}
              />
              <div className="text-sm">
                <p className="text-gray-700">
                  أوافق على{' '}
                  <Link to="/terms" className="text-blue-600 hover:text-blue-500 font-medium">
                    الشروط والأحكام
                  </Link>
                  {' '}و{' '}
                  <Link to="/privacy" className="text-blue-600 hover:text-blue-500 font-medium">
                    سياسة الخصوصية
                  </Link>
                </p>
              </div>
            </div>

            <button
              type="submit"
              disabled={registerMutation.isPending || !acceptTerms}
              className="w-full bg-gradient-to-r from-blue-600 to-purple-600 text-white py-2.5 sm:py-3 px-4 rounded-lg font-medium hover:from-blue-700 hover:to-purple-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {registerMutation.isPending ? (
                <div className="flex items-center justify-center">
                  <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin ml-2"></div>
                  جاري إنشاء الحساب...
                </div>
              ) : (
                'إنشاء الحساب'
              )}
            </button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-sm text-gray-600">
              لديك حساب بالفعل؟{' '}
              <Link
                to="/auth/login"
                className="text-blue-600 hover:text-blue-500 font-medium"
              >
                سجل الدخول
              </Link>
            </p>
          </div>
        </div>
      </div>

      {/* Right Side - Preview Section (Desktop Only) */}
      <div className="hidden lg:flex w-1/2 bg-gradient-to-br from-slate-900 via-blue-900 to-indigo-900 relative overflow-hidden items-center justify-center">
        {/* Animated Background */}
        <div className="absolute inset-0">
          {/* Floating Shapes */}
          <div className="absolute top-10 left-10 w-32 h-32 bg-blue-500/20 rounded-full blur-xl animate-pulse"></div>
          <div className="absolute top-1/3 right-20 w-48 h-48 bg-purple-500/20 rounded-full blur-xl animate-pulse" style={{animationDelay: '1s'}}></div>
          <div className="absolute bottom-20 left-1/3 w-40 h-40 bg-indigo-500/20 rounded-full blur-xl animate-pulse" style={{animationDelay: '2s'}}></div>
          
          {/* Grid Pattern */}
          <div className="absolute inset-0 opacity-10">
            <div className="absolute inset-0" style={{
              backgroundImage: `url("data:image/svg+xml,%3Csvg width='40' height='40' viewBox='0 0 40 40' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='white' fill-opacity='0.1'%3E%3Cpath d='M0 0h40v1H0zm0 20h40v1H0z'/%3E%3Cpath d='M0 0v40h1V0zm20 0v40h1V0z'/%3E%3C/g%3E%3C/svg%3E")`
            }} />
          </div>
        </div>

        <div className="relative z-10 text-center px-12 max-w-lg">
          {selectedPropertyType ? (
            <div className="space-y-8 animate-fadeIn">
              {/* Dynamic Icon */}
              <div className={`inline-flex items-center justify-center w-24 h-24 rounded-2xl bg-gradient-to-r ${currentTheme.gradient} text-white text-4xl shadow-2xl transform transition-all duration-500 ${currentTheme.animation}`}>
                {currentTheme.icon}
              </div>
              
              {/* Dynamic Title */}
              <div>
                <h2 className="text-4xl font-bold text-white mb-4 leading-tight">
                  {currentTheme.title}
                </h2>
                <p className="text-xl text-blue-100 leading-relaxed">
                  {currentTheme.subtitle}
                </p>
              </div>

              {/* Benefits Grid */}
              <div className="grid grid-cols-2 gap-4">
                {currentTheme.benefits.map((benefit, index) => (
                  <div
                    key={index}
                    className="bg-white/10 backdrop-blur-sm p-4 rounded-xl border border-white/20 transform hover:scale-105 transition-all duration-300"
                    style={{ animationDelay: `${index * 0.1}s` }}
                  >
                    <div className="text-center">
                      <div className={`w-8 h-8 mx-auto mb-2 bg-gradient-to-r ${currentTheme.gradient} rounded-full flex items-center justify-center`}>
                        <span className="text-white text-sm">✓</span>
                      </div>
                      <p className="text-sm font-medium text-white">{benefit}</p>
                    </div>
                  </div>
                ))}
              </div>

              {/* Call to Action */}
              <div className={`inline-block px-8 py-4 bg-gradient-to-r ${currentTheme.gradient} text-white rounded-xl shadow-xl text-lg font-semibold transform hover:scale-105 transition-all duration-300`}>
                {currentTheme.cta}
              </div>
            </div>
          ) : (
            <div className="space-y-8">
              {/* Default State */}
              <div className="inline-flex items-center justify-center w-24 h-24 rounded-2xl bg-gradient-to-r from-blue-600 to-purple-600 text-white text-4xl shadow-2xl">
                🏢
              </div>
              
              <div>
                <h2 className="text-4xl font-bold text-white mb-4 leading-tight">
                  منصة BookN
                </h2>
                <p className="text-xl text-blue-100 leading-relaxed">
                  حلول ذكية وشاملة لإدارة جميع أنواع خدمات الحجوزات والتأجير
                </p>
              </div>

              {/* Feature highlights */}
              <div className="grid grid-cols-2 gap-4">
                {[
                  'إدارة ذكية للحجوزات',
                  'تحليلات متقدمة',
                  'نظام مدفوعات آمن',
                  'دعم فني 24/7'
                ].map((feature, index) => (
                  <div
                    key={index}
                    className="bg-white/10 backdrop-blur-sm p-4 rounded-xl border border-white/20 transform hover:scale-105 transition-all duration-300"
                  >
                    <div className="text-center">
                      <div className="w-8 h-8 mx-auto mb-2 bg-gradient-to-r from-blue-600 to-purple-600 rounded-full flex items-center justify-center">
                        <span className="text-white text-sm">✓</span>
                      </div>
                      <p className="text-sm font-medium text-white">{feature}</p>
                    </div>
                  </div>
                ))}
              </div>

              <div className="text-blue-100 text-lg">
                اختر نوع الكيان لرؤية المزايا المخصصة
              </div>
            </div>
          )}
        </div>
      </div>

      <style>{`
        @keyframes fadeIn {
          from { opacity: 0; transform: translateY(20px); }
          to { opacity: 1; transform: translateY(0); }
        }
        .animate-fadeIn {
          animation: fadeIn 0.8s ease-out forwards;
        }
        @keyframes sway {
          0%, 100% { transform: rotate(-3deg) scale(1); }
          50% { transform: rotate(3deg) scale(1.05); }
        }
        .animate-sway {
          animation: sway 2s ease-in-out infinite;
        }
      `}</style>
      </div>
    </PublicRoute>
  );
};

export default Register;