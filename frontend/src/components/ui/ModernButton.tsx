import React from 'react';
import LoadingSpinner from './LoadingSpinner';

interface ModernButtonProps {
  onClick: () => void;
  icon?: React.ReactNode;
  label: string;
  variant?: 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'ghost' | 'gradient';
  size?: 'sm' | 'md' | 'lg' | 'xl';
  loading?: boolean;
  disabled?: boolean;
  className?: string;
  type?: 'button' | 'submit' | 'reset';
  fullWidth?: boolean;
  glowing?: boolean;
}

const ModernButton: React.FC<ModernButtonProps> = ({
  onClick,
  icon,
  label,
  variant = 'primary',
  size = 'md',
  loading = false,
  disabled = false,
  className = '',
  type = 'button',
  fullWidth = false,
  glowing = false
}) => {
  const getVariantClasses = () => {
    const baseClasses = 'font-medium transition-all duration-300 ease-in-out transform hover:scale-105 active:scale-95';
    
    const variants = {
      primary: `${baseClasses} bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white shadow-lg hover:shadow-xl`,
      secondary: `${baseClasses} bg-gradient-to-r from-gray-100 to-gray-200 hover:from-gray-200 hover:to-gray-300 text-gray-700 border border-gray-300 shadow-md hover:shadow-lg`,
      success: `${baseClasses} bg-gradient-to-r from-green-500 to-green-600 hover:from-green-600 hover:to-green-700 text-white shadow-lg hover:shadow-xl`,
      warning: `${baseClasses} bg-gradient-to-r from-yellow-500 to-yellow-600 hover:from-yellow-600 hover:to-yellow-700 text-white shadow-lg hover:shadow-xl`,
      danger: `${baseClasses} bg-gradient-to-r from-red-500 to-red-600 hover:from-red-600 hover:to-red-700 text-white shadow-lg hover:shadow-xl`,
      ghost: `${baseClasses} bg-transparent hover:bg-gray-100 text-gray-600 hover:text-gray-900 border border-gray-200 hover:border-gray-300`,
      gradient: `${baseClasses} bg-gradient-to-r from-indigo-500 via-blue-500 to-purple-600 hover:from-indigo-600 hover:via-blue-600 hover:to-purple-700 text-white shadow-lg hover:shadow-2xl`
    };
    
    return variants[variant];
  };

  const getSizeClasses = () => {
    const sizes = {
      sm: 'px-3 py-1.5 text-sm rounded-lg',
      md: 'px-4 py-2 text-sm rounded-xl',
      lg: 'px-6 py-3 text-base rounded-xl',
      xl: 'px-8 py-4 text-lg rounded-2xl'
    };
    return sizes[size];
  };

  const getGlowEffect = () => {
    if (!glowing) return '';
    
    const glowColors = {
      primary: 'hover:shadow-blue-500/50',
      secondary: 'hover:shadow-gray-500/30',
      success: 'hover:shadow-green-500/50',
      warning: 'hover:shadow-yellow-500/50',
      danger: 'hover:shadow-red-500/50',
      ghost: 'hover:shadow-gray-500/20',
      gradient: 'hover:shadow-purple-500/50'
    };
    
    return `hover:shadow-2xl ${glowColors[variant]} transition-shadow duration-300`;
  };

  const buttonClasses = `
    relative overflow-hidden
    inline-flex items-center justify-center space-x-2 space-x-reverse
    focus:outline-none focus:ring-4 focus:ring-opacity-50
    disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none
    ${getVariantClasses()}
    ${getSizeClasses()}
    ${getGlowEffect()}
    ${fullWidth ? 'w-full' : ''}
    ${className}
  `;

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled || loading}
      className={buttonClasses}
    >
      {/* تأثير موجة عند الضغط */}
      <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent -translate-x-full group-hover:translate-x-full transition-transform duration-1000 ease-in-out opacity-0 hover:opacity-100"></div>
      
      {/* المحتوى */}
      <div className="relative flex items-center space-x-2 space-x-reverse">
        {loading ? (
          <LoadingSpinner size="sm" color={variant === 'secondary' || variant === 'ghost' ? 'dark' : 'light'} />
        ) : (
          <>
            {icon && (
              <span className="flex-shrink-0">
                {icon}
              </span>
            )}
            <span className="font-medium">
              {label}
            </span>
          </>
        )}
      </div>

      {/* تأثير التوهج الداخلي */}
      {glowing && !disabled && (
        <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent animate-pulse rounded-full"></div>
      )}
    </button>
  );
};

export default ModernButton;