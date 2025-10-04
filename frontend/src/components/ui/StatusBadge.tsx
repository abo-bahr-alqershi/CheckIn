import React from 'react';

interface StatusBadgeProps {
  status: string;
  variant?: 'default' | 'booking' | 'payment' | 'user' | 'property';
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

const StatusBadge: React.FC<StatusBadgeProps> = ({
  status,
  variant = 'default',
  size = 'md',
  className = ''
}) => {
  const getSizeClasses = () => {
    const sizes = {
      sm: 'px-2 py-0.5 text-xs',
      md: 'px-2 py-1 text-xs',
      lg: 'px-3 py-1.5 text-sm'
    };
    return sizes[size];
  };

  const getStatusConfig = () => {
    const configs = {
      booking: {
        'Pending': { label: 'معلق', color: 'bg-yellow-100 text-yellow-800', icon: '⏳' },
        'Confirmed': { label: 'مؤكد', color: 'bg-blue-100 text-blue-800', icon: '✅' },
        'Cancelled': { label: 'ملغي', color: 'bg-red-100 text-red-800', icon: '❌' },
        'Completed': { label: 'مكتمل', color: 'bg-green-100 text-green-800', icon: '🏁' },
        'CheckedIn': { label: 'تم تسجيل الدخول', color: 'bg-purple-100 text-purple-800', icon: '🚪' },
        'CheckedOut': { label: 'تم تسجيل الخروج', color: 'bg-gray-100 text-gray-800', icon: '🚶‍♂️' }
      },
      payment: {
        'Successful': { label: 'ناجح', color: 'bg-green-100 text-green-800', icon: '✅' },
        'Failed': { label: 'فاشل', color: 'bg-red-100 text-red-800', icon: '❌' },
        'Pending': { label: 'معلق', color: 'bg-yellow-100 text-yellow-800', icon: '⏳' },
        'Refunded': { label: 'مسترد', color: 'bg-blue-100 text-blue-800', icon: '↩️' },
        'Voided': { label: 'ملغي', color: 'bg-gray-100 text-gray-800', icon: '🚫' },
        'PartiallyRefunded': { label: 'مسترد جزئياً', color: 'bg-purple-100 text-purple-800', icon: '↪️' }
      },
      user: {
        'Active': { label: 'نشط', color: 'bg-green-100 text-green-800', icon: '✅' },
        'Inactive': { label: 'غير نشط', color: 'bg-gray-100 text-gray-800', icon: '⭕' },
        'Suspended': { label: 'معلق', color: 'bg-red-100 text-red-800', icon: '🚫' },
        'Pending': { label: 'في الانتظار', color: 'bg-yellow-100 text-yellow-800', icon: '⏳' }
      },
      property: {
        'Available': { label: 'متاح', color: 'bg-green-100 text-green-800', icon: '✅' },
        'Occupied': { label: 'مشغول', color: 'bg-red-100 text-red-800', icon: '🔴' },
        'Maintenance': { label: 'صيانة', color: 'bg-yellow-100 text-yellow-800', icon: '🔧' },
        'Unavailable': { label: 'غير متاح', color: 'bg-gray-100 text-gray-800', icon: '⭕' }
      },
      default: {
        'active': { label: 'نشط', color: 'bg-green-100 text-green-800', icon: '✅' },
        'inactive': { label: 'غير نشط', color: 'bg-gray-100 text-gray-800', icon: '⭕' },
        'pending': { label: 'معلق', color: 'bg-yellow-100 text-yellow-800', icon: '⏳' },
        'success': { label: 'نجح', color: 'bg-green-100 text-green-800', icon: '✅' },
        'error': { label: 'خطأ', color: 'bg-red-100 text-red-800', icon: '❌' },
        'warning': { label: 'تحذير', color: 'bg-yellow-100 text-yellow-800', icon: '⚠️' },
        'info': { label: 'معلومات', color: 'bg-blue-100 text-blue-800', icon: 'ℹ️' }
      }
    };

    const variantConfig = configs[variant] || configs.default;
    return variantConfig[status] || { 
      label: status, 
      color: 'bg-gray-100 text-gray-800', 
      icon: '📍' 
    };
  };

  const config = getStatusConfig();

  return (
    <span 
      className={`
        inline-flex items-center space-x-1 space-x-reverse font-medium rounded-full
        ${getSizeClasses()}
        ${config.color}
        ${className}
      `}
    >
      <span>{config.icon}</span>
      <span>{config.label}</span>
    </span>
  );
};

export default StatusBadge;