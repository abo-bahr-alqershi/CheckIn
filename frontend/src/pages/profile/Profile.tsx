import React, { useState, useEffect } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { CommonUsersService } from '../../services/common-users.service';
import { CommonAuthService } from '../../services/common-auth.service';
import type { UpdateUserCommand, UpdateUserProfilePictureCommand, UpdateUserSettingsCommand } from '../../types/user.types';
import type { ChangePasswordCommand } from '../../types/auth.types';
import ActionButton from '../../components/ui/ActionButton';
import ImageUpload from '../../components/ui/ImageUpload';
import { useNotifications } from '../../hooks/useNotifications';

const Profile: React.FC = () => {
  const { showSuccess, showError } = useNotifications();
  
  const [activeTab, setActiveTab] = useState<'profile' | 'password'>('profile');
  const [profileData, setProfileData] = useState({
    name: '',
    email: '',
    phone: '',
    profileImage: ''
  });
  
  const [passwordData, setPasswordData] = useState<ChangePasswordCommand>({
    userId: '',
    currentPassword: '',
    newPassword: ''
  });
  
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showCurrentPassword, setShowCurrentPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [profileErrors, setProfileErrors] = useState<Partial<UpdateUserCommand>>({});
  const [passwordErrors, setPasswordErrors] = useState<Partial<ChangePasswordCommand & { confirmPassword: string }>>({});

  // جلب بيانات المستخدم الحالي
  const { data: currentUser, isLoading, refetch } = useQuery({
    queryKey: ['current-user'],
    queryFn: () => CommonUsersService.getCurrentUser({})
  });

  // تحديث صورة الملف الشخصي
  const updateProfilePictureMutation = useMutation({
    mutationFn: CommonUsersService.updateProfilePicture,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم تحديث الصورة الشخصية بنجاح');
        refetch();
      } else {
        showError(result.message || 'فشل في تحديث الصورة الشخصية');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء تحديث الصورة الشخصية');
    }
  });

  // تحديث بيانات الملف الشخصي
  const updateProfileMutation = useMutation({
    mutationFn: CommonUsersService.updateUserSettings,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم تحديث الملف الشخصي بنجاح');
        refetch();
      } else {
        showError(result.message || 'فشل في تحديث الملف الشخصي');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء تحديث الملف الشخصي');
    }
  });

  // تغيير كلمة المرور
  const changePasswordMutation = useMutation({
    mutationFn: CommonAuthService.changePassword,
    onSuccess: (result) => {
      if (result.success) {
        showSuccess('تم تغيير كلمة المرور بنجاح');
        setPasswordData({
          userId: passwordData.userId,
          currentPassword: '',
          newPassword: ''
        });
        setConfirmPassword('');
      } else {
        showError(result.message || 'فشل في تغيير كلمة المرور');
      }
    },
    onError: (error: any) => {
      showError(error.response?.data?.message || 'حدث خطأ أثناء تغيير كلمة المرور');
    }
  });

  // تحديث البيانات عند جلب المستخدم
  useEffect(() => {
    const user = currentUser?.data;
    if (user) {
      setProfileData({
        name: user.name || '',
        email: user.email || '',
        phone: user.phone || '',
        profileImage: user.profileImage || ''
      });
      
      setPasswordData(prev => ({
        ...prev,
        userId: user.id
      }));
    }
  }, [currentUser]);

  // التحقق من صحة بيانات الملف الشخصي
  const validateProfile = (): boolean => {
    const newErrors: Partial<UpdateUserCommand> = {};
    
    if (!profileData.name.trim()) {
      newErrors.name = 'الاسم مطلوب';
    }
    
    if (!profileData.email.trim()) {
      newErrors.email = 'البريد الإلكتروني مطلوب';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(profileData.email)) {
      newErrors.email = 'البريد الإلكتروني غير صحيح';
    }
    
    if (!profileData.phone.trim()) {
      newErrors.phone = 'رقم الهاتف مطلوب';
    } else if (!/^[0-9+\-\s()]+$/.test(profileData.phone)) {
      newErrors.phone = 'رقم الهاتف غير صحيح';
    }
    
    setProfileErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // التحقق من صحة بيانات كلمة المرور
  const validatePassword = (): boolean => {
    const newErrors: Partial<ChangePasswordCommand & { confirmPassword: string }> = {};
    
    if (!passwordData.currentPassword.trim()) {
      newErrors.currentPassword = 'كلمة المرور الحالية مطلوبة';
    }
    
    if (!passwordData.newPassword.trim()) {
      newErrors.newPassword = 'كلمة المرور الجديدة مطلوبة';
    } else if (passwordData.newPassword.length < 6) {
      newErrors.newPassword = 'كلمة المرور يجب أن تكون 6 أحرف على الأقل';
    }
    
    if (!confirmPassword.trim()) {
      newErrors.confirmPassword = 'تأكيد كلمة المرور مطلوب';
    } else if (passwordData.newPassword !== confirmPassword) {
      newErrors.confirmPassword = 'كلمة المرور غير متطابقة';
    }
    
    if (passwordData.currentPassword === passwordData.newPassword) {
      newErrors.newPassword = 'كلمة المرور الجديدة يجب أن تكون مختلفة عن الحالية';
    }
    
    setPasswordErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // معالجة تحديث الملف الشخصي
  const handleProfileSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateProfile()) {
      const updateData: UpdateUserSettingsCommand = {
        settingsJson: JSON.stringify(profileData)
      };
      updateProfileMutation.mutate(updateData);
    }
  };

  // معالجة تغيير كلمة المرور
  const handlePasswordSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validatePassword()) {
      changePasswordMutation.mutate(passwordData);
    }
  };

  // تحديث بيانات الملف الشخصي
  const handleProfileChange = (field: keyof typeof profileData, value: string) => {
    setProfileData(prev => ({ ...prev, [field]: value }));
    
    if (profileErrors[field as keyof UpdateUserCommand]) {
      setProfileErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  // تحديث بيانات كلمة المرور
  const handlePasswordChange = (field: keyof ChangePasswordCommand, value: string) => {
    setPasswordData(prev => ({ ...prev, [field]: value }));
    
    if (passwordErrors[field]) {
      setPasswordErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  // معالجة رفع الصورة
  const handleImageUpload = async (file: File) => {
    try {
      // تحويل الصورة إلى base64
      const imageUrl = await convertFileToUrl(file);
      
      const user = currentUser?.data;
      if (user?.id) {
        const updateCommand: UpdateUserProfilePictureCommand = {
          userId: user.id,
          profileImageUrl: imageUrl
        };
        updateProfilePictureMutation.mutate(updateCommand);
      }
    } catch (error) {
      showError('حدث خطأ أثناء معالجة الصورة');
    }
  };

  // معالجة حذف الصورة
  const handleImageDelete = () => {
    const user = currentUser?.data;
    if (user?.id) {
      const updateCommand: UpdateUserProfilePictureCommand = {
        userId: user.id,
        profileImageUrl: ''
      };
      updateProfilePictureMutation.mutate(updateCommand);
    }
  };

  // تحويل الملف إلى base64
  const convertFileToUrl = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        if (reader.result) {
          resolve(reader.result as string);
        } else {
          reject(new Error('فشل في قراءة الملف'));
        }
      };
      reader.onerror = () => reject(new Error('فشل في قراءة الملف'));
      reader.readAsDataURL(file);
    });
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">جارٍ تحميل بيانات الملف الشخصي...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4">
      <div className="max-w-4xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">الملف الشخصي</h1>
          <p className="text-gray-600 mt-2">إدارة بياناتك الشخصية وإعدادات الحساب</p>
        </div>

        {/* التبويبات */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 mb-6">
          <div className="border-b border-gray-200">
            <nav className="flex space-x-8 space-x-reverse px-6">
              <button
                onClick={() => setActiveTab('profile')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'profile'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700'
                }`}
              >
                البيانات الشخصية
              </button>
              <button
                onClick={() => setActiveTab('password')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'password'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700'
                }`}
              >
                تغيير كلمة المرور
              </button>
            </nav>
          </div>

          <div className="p-6">
            {activeTab === 'profile' && (
              <form onSubmit={handleProfileSubmit} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  {/* صورة الملف الشخصي */}
                  <div className="md:col-span-2">
                    <label className="block text-sm font-medium text-gray-700 mb-4">
                      صورة الملف الشخصي
                    </label>
                    <ImageUpload
                      currentImage={profileData.profileImage}
                      onImageUpload={handleImageUpload}
                      onImageDelete={handleImageDelete}
                      isUploading={updateProfilePictureMutation.isPending}
                      disabled={updateProfileMutation.isPending}
                      size="md"
                      placeholder={profileData.name.charAt(0).toUpperCase()}
                      maxFileSize={2}
                      acceptedTypes={['image/jpeg', 'image/png', 'image/jpg']}
                    />
                  </div>

                  {/* الاسم */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      الاسم الكامل
                    </label>
                    <input
                      type="text"
                      value={profileData.name}
                      onChange={(e) => handleProfileChange('name', e.target.value)}
                      disabled={updateProfileMutation.isPending}
                      className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 ${
                        profileErrors.name
                          ? 'border-red-300 focus:ring-red-500'
                          : 'border-gray-300'
                      }`}
                    />
                    {profileErrors.name && (
                      <p className="mt-1 text-sm text-red-600">{profileErrors.name}</p>
                    )}
                  </div>

                  {/* البريد الإلكتروني */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      البريد الإلكتروني
                    </label>
                    <input
                      type="email"
                      value={profileData.email}
                      onChange={(e) => handleProfileChange('email', e.target.value)}
                      disabled={updateProfileMutation.isPending}
                      className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left ${
                        profileErrors.email
                          ? 'border-red-300 focus:ring-red-500'
                          : 'border-gray-300'
                      }`}
                      dir="ltr"
                    />
                    {profileErrors.email && (
                      <p className="mt-1 text-sm text-red-600">{profileErrors.email}</p>
                    )}
                  </div>

                  {/* رقم الهاتف */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      رقم الهاتف
                    </label>
                    <input
                      type="tel"
                      value={profileData.phone}
                      onChange={(e) => handleProfileChange('phone', e.target.value)}
                      disabled={updateProfileMutation.isPending}
                      className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left ${
                        profileErrors.phone
                          ? 'border-red-300 focus:ring-red-500'
                          : 'border-gray-300'
                      }`}
                      dir="ltr"
                    />
                    {profileErrors.phone && (
                      <p className="mt-1 text-sm text-red-600">{profileErrors.phone}</p>
                    )}
                  </div>

                  {/* معلومات إضافية */}
                  <div className="md:col-span-2">
                    <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
                      <div className="flex">
                        <div className="flex-shrink-0">
                          <svg className="h-5 w-5 text-blue-400" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
                          </svg>
                        </div>
                        <div className="mr-3">
                          <h3 className="text-sm font-medium text-blue-800">
                            ملاحظة مهمة
                          </h3>
                          <div className="mt-2 text-sm text-blue-700">
                            <p>
                              تغيير البريد الإلكتروني قد يتطلب التحقق من العنوان الجديد.
                              ستتلقى رسالة تأكيد على البريد الجديد.
                            </p>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="flex justify-end">
                  <ActionButton
                    type="submit"
                    variant="primary"
                    label={updateProfileMutation.isPending ? 'جارٍ الحفظ...' : 'حفظ التغييرات'}
                    onClick={() => {}}
                    disabled={updateProfileMutation.isPending}
                  />
                </div>
              </form>
            )}

            {activeTab === 'password' && (
              <form onSubmit={handlePasswordSubmit} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  {/* كلمة المرور الحالية */}
                  <div className="md:col-span-2">
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      كلمة المرور الحالية
                    </label>
                    <div className="relative">
                      <input
                        type={showCurrentPassword ? 'text' : 'password'}
                        value={passwordData.currentPassword}
                        onChange={(e) => handlePasswordChange('currentPassword', e.target.value)}
                        disabled={changePasswordMutation.isPending}
                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left pr-10 ${
                          passwordErrors.currentPassword
                            ? 'border-red-300 focus:ring-red-500'
                            : 'border-gray-300'
                        }`}
                        dir="ltr"
                      />
                      <button
                        type="button"
                        onClick={() => setShowCurrentPassword(!showCurrentPassword)}
                        className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                        disabled={changePasswordMutation.isPending}
                      >
                        {showCurrentPassword ? 'إخفاء' : 'إظهار'}
                      </button>
                    </div>
                    {passwordErrors.currentPassword && (
                      <p className="mt-1 text-sm text-red-600">{passwordErrors.currentPassword}</p>
                    )}
                  </div>

                  {/* كلمة المرور الجديدة */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      كلمة المرور الجديدة
                    </label>
                    <div className="relative">
                      <input
                        type={showNewPassword ? 'text' : 'password'}
                        value={passwordData.newPassword}
                        onChange={(e) => handlePasswordChange('newPassword', e.target.value)}
                        disabled={changePasswordMutation.isPending}
                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left pr-10 ${
                          passwordErrors.newPassword
                            ? 'border-red-300 focus:ring-red-500'
                            : 'border-gray-300'
                        }`}
                        dir="ltr"
                      />
                      <button
                        type="button"
                        onClick={() => setShowNewPassword(!showNewPassword)}
                        className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                        disabled={changePasswordMutation.isPending}
                      >
                        {showNewPassword ? 'إخفاء' : 'إظهار'}
                      </button>
                    </div>
                    {passwordErrors.newPassword && (
                      <p className="mt-1 text-sm text-red-600">{passwordErrors.newPassword}</p>
                    )}
                  </div>

                  {/* تأكيد كلمة المرور الجديدة */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      تأكيد كلمة المرور الجديدة
                    </label>
                    <div className="relative">
                      <input
                        type={showConfirmPassword ? 'text' : 'password'}
                        value={confirmPassword}
                        onChange={(e) => {
                          setConfirmPassword(e.target.value);
                          if (passwordErrors.confirmPassword) {
                            setPasswordErrors(prev => ({ ...prev, confirmPassword: undefined }));
                          }
                        }}
                        disabled={changePasswordMutation.isPending}
                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500 text-left pr-10 ${
                          passwordErrors.confirmPassword
                            ? 'border-red-300 focus:ring-red-500'
                            : 'border-gray-300'
                        }`}
                        dir="ltr"
                      />
                      <button
                        type="button"
                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                        className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                        disabled={changePasswordMutation.isPending}
                      >
                        {showConfirmPassword ? 'إخفاء' : 'إظهار'}
                      </button>
                    </div>
                    {passwordErrors.confirmPassword && (
                      <p className="mt-1 text-sm text-red-600">{passwordErrors.confirmPassword}</p>
                    )}
                  </div>

                  {/* نصائح الأمان */}
                  <div className="md:col-span-2">
                    <div className="bg-yellow-50 border border-yellow-200 rounded-md p-4">
                      <div className="flex">
                        <div className="flex-shrink-0">
                          <svg className="h-5 w-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                          </svg>
                        </div>
                        <div className="mr-3">
                          <h3 className="text-sm font-medium text-yellow-800">
                            نصائح لكلمة مرور قوية
                          </h3>
                          <div className="mt-2 text-sm text-yellow-700">
                            <ul className="list-disc list-inside space-y-1">
                              <li>استخدم على الأقل 8 أحرف</li>
                              <li>امزج بين الأحرف الكبيرة والصغيرة</li>
                              <li>أضف أرقام ورموز خاصة</li>
                              <li>تجنب استخدام معلومات شخصية</li>
                            </ul>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="flex justify-end">
                  <ActionButton
                    type="submit"
                    variant="primary"
                    label={changePasswordMutation.isPending ? 'جارٍ التغيير...' : 'تغيير كلمة المرور'}
                    onClick={() => {}}
                    disabled={changePasswordMutation.isPending}
                  />
                </div>
              </form>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile;