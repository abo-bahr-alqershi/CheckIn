import React, { useState, useEffect } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { CommonUsersService } from '../../services/common-users.service';
import ActionButton from '../../components/ui/ActionButton';
import { Card } from '../../components/ui/Card';
import { useNotifications } from '../../hooks/useNotifications';

interface UserSettings {
  language: 'ar' | 'en';
  theme: 'light' | 'dark' | 'system';
  notifications: {
    email: boolean;
    sms: boolean;
    push: boolean;
    marketing: boolean;
  };
  privacy: {
    profileVisibility: 'public' | 'private' | 'contacts';
    showEmail: boolean;
    showPhone: boolean;
  };
  preferences: {
    dateFormat: 'dd/mm/yyyy' | 'mm/dd/yyyy' | 'yyyy-mm-dd';
    currency: 'YER' | 'USD' | 'EUR';
    timezone: string;
  };
}

const Settings: React.FC = () => {
  const { showSuccess, showError } = useNotifications();
  
  const [settings, setSettings] = useState<UserSettings>({
    language: 'ar',
    theme: 'light',
    notifications: {
      email: true,
      sms: true,
      push: true,
      marketing: false
    },
    privacy: {
      profileVisibility: 'public',
      showEmail: false,
      showPhone: false
    },
    preferences: {
      dateFormat: 'dd/mm/yyyy',
      currency: 'YER',
      timezone: 'Asia/Riyadh'
    }
  });

  const [activeTab, setActiveTab] = useState<'general' | 'notifications' | 'privacy' | 'preferences'>('general');

  // جلب بيانات المستخدم الحالي
  const { data: currentUser, isLoading } = useQuery({
    queryKey: ['current-user'],
    queryFn: () => CommonUsersService.getCurrentUser({})
  });

  // تحديث الإعدادات
  const updateSettingsMutation = useMutation({
    mutationFn: CommonUsersService.updateUserSettings,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم حفظ الإعدادات بنجاح');
      } else {
        showError(result.message || 'فشل في حفظ الإعدادات');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء حفظ الإعدادات');
    }
  });

  // تحميل الإعدادات المحفوظة
  useEffect(() => {
    if (currentUser?.data?.settingsJson) {
      try {
        const savedSettings = JSON.parse(currentUser.data.settingsJson) as Partial<UserSettings>;
        setSettings(prev => Object.assign({}, prev, savedSettings));
      } catch (error) {
        console.error('خطأ في تحليل الإعدادات المحفوظة:', error);
      }
    }
  }, [currentUser]);

  // تحديث إعداد محدد
  const updateSetting = (category: keyof UserSettings, key: string, value: any) => {
    setSettings(prev => {
      // Handle nested object properties
      if (category === 'notifications' || category === 'privacy' || category === 'preferences') {
        return {
          ...prev,
          [category]: {
            ...(prev[category] as any),
            [key]: value
          }
        } as UserSettings;
      }
      // Handle top-level properties (language, theme)
      return {
        ...prev,
        [category]: value
      } as UserSettings;
    });
  };

  // حفظ الإعدادات
  const saveSettings = () => {
    if (currentUser?.data?.id) {
      const settingsData = {
        userId: currentUser.data.id,
        settingsJson: JSON.stringify(settings)
      };
      updateSettingsMutation.mutate(settingsData);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">جارٍ تحميل الإعدادات...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4">
      <div className="max-w-4xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">الإعدادات</h1>
          <p className="text-gray-600 mt-2">إدارة تفضيلاتك وإعدادات التطبيق</p>
        </div>

        {/* التبويبات */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 mb-6">
          <div className="border-b border-gray-200">
            <nav className="flex space-x-8 space-x-reverse px-6 overflow-x-auto">
              <button
                onClick={() => setActiveTab('general')}
                className={`py-4 px-1 border-b-2 font-medium text-sm whitespace-nowrap ${
                  activeTab === 'general'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700'
                }`}
              >
                الإعدادات العامة
              </button>
              <button
                onClick={() => setActiveTab('notifications')}
                className={`py-4 px-1 border-b-2 font-medium text-sm whitespace-nowrap ${
                  activeTab === 'notifications'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700'
                }`}
              >
                الإشعارات
              </button>
              <button
                onClick={() => setActiveTab('privacy')}
                className={`py-4 px-1 border-b-2 font-medium text-sm whitespace-nowrap ${
                  activeTab === 'privacy'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700'
                }`}
              >
                الخصوصية
              </button>
              <button
                onClick={() => setActiveTab('preferences')}
                className={`py-4 px-1 border-b-2 font-medium text-sm whitespace-nowrap ${
                  activeTab === 'preferences'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700'
                }`}
              >
                التفضيلات
              </button>
            </nav>
          </div>

          <div className="p-6">
            {/* الإعدادات العامة */}
            {activeTab === 'general' && (
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium text-gray-900 mb-4">الإعدادات العامة</h3>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    {/* اللغة */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        اللغة
                      </label>
                      <select
                        value={settings.language}
                        onChange={(e) => updateSetting('language', '', e.target.value as 'ar' | 'en')}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
                      >
                        <option value="ar">العربية</option>
                        <option value="en">English</option>
                      </select>
                    </div>

                    {/* الثيم */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        المظهر
                      </label>
                      <select
                        value={settings.theme}
                        onChange={(e) => updateSetting('theme', '', e.target.value as 'light' | 'dark' | 'system')}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
                      >
                        <option value="light">فاتح</option>
                        <option value="dark">داكن</option>
                        <option value="system">تلقائي (حسب النظام)</option>
                      </select>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* إعدادات الإشعارات */}
            {activeTab === 'notifications' && (
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium text-gray-900 mb-4">إعدادات الإشعارات</h3>
                  
                  <div className="space-y-4">
                    {/* إشعارات البريد الإلكتروني */}
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="text-sm font-medium text-gray-900">إشعارات البريد الإلكتروني</h4>
                        <p className="text-sm text-gray-500">تلقي الإشعارات عبر البريد الإلكتروني</p>
                      </div>
                      <label className="relative inline-flex items-center cursor-pointer">
                        <input
                          type="checkbox"
                          checked={settings.notifications.email}
                          onChange={(e) => updateSetting('notifications', 'email', e.target.checked)}
                          className="sr-only peer"
                        />
                        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                      </label>
                    </div>

                    {/* إشعارات الرسائل النصية */}
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="text-sm font-medium text-gray-900">إشعارات الرسائل النصية</h4>
                        <p className="text-sm text-gray-500">تلقي الإشعارات عبر الرسائل النصية</p>
                      </div>
                      <label className="relative inline-flex items-center cursor-pointer">
                        <input
                          type="checkbox"
                          checked={settings.notifications.sms}
                          onChange={(e) => updateSetting('notifications', 'sms', e.target.checked)}
                          className="sr-only peer"
                        />
                        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                      </label>
                    </div>

                    {/* الإشعارات الفورية */}
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="text-sm font-medium text-gray-900">الإشعارات الفورية</h4>
                        <p className="text-sm text-gray-500">تلقي الإشعارات الفورية على المتصفح</p>
                      </div>
                      <label className="relative inline-flex items-center cursor-pointer">
                        <input
                          type="checkbox"
                          checked={settings.notifications.push}
                          onChange={(e) => updateSetting('notifications', 'push', e.target.checked)}
                          className="sr-only peer"
                        />
                        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                      </label>
                    </div>

                    {/* الإشعارات التسويقية */}
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="text-sm font-medium text-gray-900">الإشعارات التسويقية</h4>
                        <p className="text-sm text-gray-500">تلقي إشعارات العروض والأخبار</p>
                      </div>
                      <label className="relative inline-flex items-center cursor-pointer">
                        <input
                          type="checkbox"
                          checked={settings.notifications.marketing}
                          onChange={(e) => updateSetting('notifications', 'marketing', e.target.checked)}
                          className="sr-only peer"
                        />
                        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                      </label>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* إعدادات الخصوصية */}
            {activeTab === 'privacy' && (
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium text-gray-900 mb-4">إعدادات الخصوصية</h3>
                  
                  <div className="space-y-6">
                    {/* مستوى الخصوصية */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        مستوى ظهور الملف الشخصي
                      </label>
                      <select
                        value={settings.privacy.profileVisibility}
                        onChange={(e) => updateSetting('privacy', 'profileVisibility', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
                      >
                        <option value="public">عام - يمكن للجميع رؤيته</option>
                        <option value="contacts">جهات الاتصال فقط</option>
                        <option value="private">خاص - لا يظهر للآخرين</option>
                      </select>
                    </div>

                    <div className="space-y-4">
                      {/* إظهار البريد الإلكتروني */}
                      <div className="flex items-center justify-between">
                        <div>
                          <h4 className="text-sm font-medium text-gray-900">إظهار البريد الإلكتروني</h4>
                          <p className="text-sm text-gray-500">السماح للآخرين برؤية بريدك الإلكتروني</p>
                        </div>
                        <label className="relative inline-flex items-center cursor-pointer">
                          <input
                            type="checkbox"
                            checked={settings.privacy.showEmail}
                            onChange={(e) => updateSetting('privacy', 'showEmail', e.target.checked)}
                            className="sr-only peer"
                          />
                          <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                        </label>
                      </div>

                      {/* إظهار رقم الهاتف */}
                      <div className="flex items-center justify-between">
                        <div>
                          <h4 className="text-sm font-medium text-gray-900">إظهار رقم الهاتف</h4>
                          <p className="text-sm text-gray-500">السماح للآخرين برؤية رقم هاتفك</p>
                        </div>
                        <label className="relative inline-flex items-center cursor-pointer">
                          <input
                            type="checkbox"
                            checked={settings.privacy.showPhone}
                            onChange={(e) => updateSetting('privacy', 'showPhone', e.target.checked)}
                            className="sr-only peer"
                          />
                          <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                        </label>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* التفضيلات */}
            {activeTab === 'preferences' && (
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium text-gray-900 mb-4">التفضيلات</h3>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    {/* صيغة التاريخ */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        صيغة التاريخ
                      </label>
                      <select
                        value={settings.preferences.dateFormat}
                        onChange={(e) => updateSetting('preferences', 'dateFormat', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
                      >
                        <option value="dd/mm/yyyy">يوم/شهر/سنة (31/12/2023)</option>
                        <option value="mm/dd/yyyy">شهر/يوم/سنة (12/31/2023)</option>
                        <option value="yyyy-mm-dd">سنة-شهر-يوم (2023-12-31)</option>
                      </select>
                    </div>

                    {/* العملة */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        العملة المفضلة
                      </label>
                      <select
                        value={settings.preferences.currency}
                        onChange={(e) => updateSetting('preferences', 'currency', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
                      >
                        <option value="YER">ريال يمني (YER)</option>
                        <option value="USD">دولار أمريكي (USD)</option>
                        <option value="EUR">يورو (EUR)</option>
                      </select>
                    </div>

                    {/* المنطقة الزمنية */}
                    <div className="md:col-span-2">
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        المنطقة الزمنية
                      </label>
                      <select
                        value={settings.preferences.timezone}
                        onChange={(e) => updateSetting('preferences', 'timezone', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
                      >
                        <option value="Asia/Riyadh">صنعاء (GMT+3)</option>
                        <option value="Asia/Dubai">دبي (GMT+4)</option>
                        <option value="Europe/London">لندن (GMT+0)</option>
                        <option value="America/New_York">نيويورك (GMT-5)</option>
                      </select>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* زر الحفظ */}
            <div className="mt-8 pt-6 border-t border-gray-200">
              <div className="flex justify-end">
                <ActionButton
                  variant="primary"
                  label={updateSettingsMutation.isPending ? 'جارٍ الحفظ...' : 'حفظ الإعدادات'}
                  onClick={saveSettings}
                  disabled={updateSettingsMutation.isPending}
                />
              </div>
            </div>
          </div>
        </div>

        {/* معلومات إضافية */}
        <Card className="p-6">
          <div className="flex items-start space-x-3 space-x-reverse">
            <div className="flex-shrink-0">
              <svg className="h-5 w-5 text-blue-400" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
              </svg>
            </div>
            <div>
              <h3 className="text-sm font-medium text-gray-900">ملاحظة حول الإعدادات</h3>
              <div className="mt-2 text-sm text-gray-600">
                <p>
                  يتم حفظ إعداداتك تلقائياً في حسابك. بعض التغييرات قد تتطلب إعادة تسجيل الدخول لتظهر بشكل كامل.
                  في حالة واجهت أي مشاكل، يمكنك التواصل مع الدعم الفني.
                </p>
              </div>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
};

export default Settings;