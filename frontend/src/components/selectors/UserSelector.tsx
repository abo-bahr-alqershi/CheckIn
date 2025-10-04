import React, { useState, useEffect, useRef } from 'react';
import { useAdminUsers } from '../../hooks/useAdminUsers';
import type { UserDto } from '../../types/user.types';

interface UserSelectorProps {
  value?: string;
  onChange: (userId: string, user?: UserDto) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  activeOnly?: boolean;
  showAvatar?: boolean;
  required?: boolean;
  /** Roles allowed for selection */
  allowedRoles?: string[];
}

const UserSelector: React.FC<UserSelectorProps> = ({
  value = '',
  onChange,
  placeholder = 'اختر المستخدم',
  className = '',
  disabled = false,
  activeOnly = false,
  showAvatar = true,
  required = false
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // بناء معايير البحث
  const searchParams = {
    pageNumber: 1,
    pageSize: 50,
    nameContains: searchTerm || undefined,
  };

  const { usersData, isLoading } = useAdminUsers(searchParams);

  // فلترة المستخدمين حسب الحالة النشطة
  const filteredUsers = usersData?.items?.filter(user => 
    !activeOnly || user.isActive
  ) || [];

  // العثور على المستخدم المحدد
  useEffect(() => {
    if (value && usersData?.items) {
      const user = usersData.items.find(u => u.id === value);
      setSelectedUser(user || null);
    } else {
      setSelectedUser(null);
    }
  }, [value, usersData]);

  // إغلاق القائمة عند النقر خارجها
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleUserSelect = (user: UserDto) => {
    setSelectedUser(user);
    onChange(user.id, user);
    setIsOpen(false);
    setSearchTerm('');
  };

  const handleClear = () => {
    setSelectedUser(null);
    onChange('');
    setSearchTerm('');
  };

  const getStatusLabel = (isActive: boolean) => {
    return isActive ? 'نشط' : 'غير نشط';
  };

  const getStatusColor = (isActive: boolean) => {
    return isActive 
      ? 'bg-green-100 text-green-800'
      : 'bg-red-100 text-red-800';
  };

  return (
    <div className={`relative ${className}`} ref={dropdownRef}>
      {/* زر الاختيار */}
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        disabled={disabled}
        className={`
          w-full flex items-center justify-between px-3 py-2 border rounded-md shadow-sm
          bg-white text-right focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
          ${disabled ? 'bg-gray-50 text-gray-400 cursor-not-allowed' : 'hover:border-gray-400'}
          ${!selectedUser && required ? 'border-red-300' : 'border-gray-300'}
        `}
      >
        <div className="flex items-center space-x-3 space-x-reverse flex-1 min-w-0">
          {selectedUser ? (
            <>
              {showAvatar && (
                <div className="flex-shrink-0">
                  {selectedUser.profileImage ? (
                    <img
                      src={selectedUser.profileImage}
                      alt={selectedUser.name}
                      className="w-8 h-8 rounded-full object-cover"
                    />
                  ) : (
                    <div className="w-8 h-8 rounded-full bg-gray-300 flex items-center justify-center">
                      <span className="text-sm font-medium text-gray-600">
                        {selectedUser.name?.[0]?.toUpperCase()}
                      </span>
                    </div>
                  )}
                </div>
              )}
              <div className="flex-1 min-w-0">
                <div className="flex items-center space-x-2 space-x-reverse">
                  <span className="font-medium text-gray-900 truncate">
                    {selectedUser.name}
                  </span>
                  <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(selectedUser.isActive)}`}>
                    {getStatusLabel(selectedUser.isActive)}
                  </span>
                </div>
                <p className="text-sm text-gray-500 truncate">{selectedUser.email}</p>
              </div>
            </>
          ) : (
            <span className="text-gray-500">{placeholder}</span>
          )}
        </div>
        
        <div className="flex items-center space-x-1 space-x-reverse">
          {selectedUser && !disabled && (
            // تغيير الزر الداخلي إلى عنصر span لتجنب زر داخل زر
            <span
              role="button"
              tabIndex={0}
              onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();
                handleClear();
              }}
              onKeyDown={(e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                  e.preventDefault();
                  (e as unknown as React.MouseEvent<HTMLSpanElement>).stopPropagation();
                  handleClear();
                }
              }}
              className="p-1 hover:bg-gray-100 rounded cursor-pointer"
            >
              <span className="text-gray-400 hover:text-gray-600">×</span>
            </span>
          )}
          <span className="text-gray-400">
            {isOpen ? '▲' : '▼'}
          </span>
        </div>
      </button>

      {/* القائمة المنسدلة */}
      {isOpen && (
        <div className="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg">
          {/* شريط البحث */}
          <div className="p-3 border-b border-gray-200">
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="البحث في المستخدمين..."
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
              autoFocus
            />
          </div>

          {/* قائمة المستخدمين */}
          <div className="max-h-60 overflow-y-auto">
            {isLoading ? (
              <div className="p-4 text-center text-gray-500">
                <div className="animate-spin inline-block w-6 h-6 border-2 border-gray-300 border-t-blue-600 rounded-full"></div>
                <p className="mt-2">جارٍ التحميل...</p>
              </div>
            ) : filteredUsers.length > 0 ? (
              filteredUsers.map((user ) => (
                <button
                  key={user.id}
                  type="button"
                  onClick={() => handleUserSelect(user)}
                  className="w-full flex items-center px-3 py-3 hover:bg-gray-50 text-right border-b border-gray-100 last:border-b-0"
                >
                  {showAvatar && (
                    <div className="flex-shrink-0 ml-3">
                      {user.profileImage ? (
                        <img
                          src={user.profileImage}
                          alt={user.name}
                          className="w-10 h-10 rounded-full object-cover"
                        />
                      ) : (
                        <div className="w-10 h-10 rounded-full bg-gray-300 flex items-center justify-center">
                          <span className="text-sm font-medium text-gray-600">
                            {user.name?.[0]?.toUpperCase()}
                          </span>
                        </div>
                      )}
                    </div>
                  )}
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center space-x-2 space-x-reverse">
                      <span className="font-medium text-gray-900 truncate">
                        {user.name}
                      </span>
                      <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(user.isActive)}`}>
                        {getStatusLabel(user.isActive)}
                      </span>
                    </div>
                    <p className="text-sm text-gray-500 truncate">{user.email}</p>
                    {user.phone && (
                      <p className="text-xs text-gray-400 truncate">{user.phone}</p>
                    )}
                  </div>
                </button>
              ))
            ) : (
              <div className="p-4 text-center text-gray-500">
                {searchTerm ? `لا توجد نتائج للبحث "${searchTerm}"` : 'لا توجد مستخدمين'}
              </div>
            )}
          </div>

          {/* إجمالي النتائج */}
          {filteredUsers.length > 0 && (
            <div className="p-2 bg-gray-50 border-t border-gray-200 text-xs text-gray-500 text-center">
              {filteredUsers.length} من {usersData?.totalCount || 0} مستخدم
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default UserSelector;