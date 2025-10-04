import React from 'react';
import Tooltip from './Tooltip';
import LoadingSpinner from './LoadingSpinner';

interface ActionButtonProps {
  onClick: React.MouseEventHandler<HTMLButtonElement>;
  icon?: string;
  label: string;
  tooltip?: string;
  variant?: 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'info';
  size?: 'sm' | 'md' | 'lg';
  loading?: boolean;
  disabled?: boolean;
  className?: string;
  children?: React.ReactNode;
  type?: 'button' | 'submit' | 'reset';
}

const ActionButton: React.FC<ActionButtonProps> = ({
  onClick,
  icon,
  label,
  tooltip,
  variant = 'primary',
  size = 'md',
  loading = false,
  disabled = false,
  className = '',
  children,
  type = 'button'
}) => {
  const getVariantClasses = () => {
    const variants = {
      primary: 'bg-blue-600 hover:bg-blue-700 text-white border-blue-600',
      secondary: 'bg-gray-600 hover:bg-gray-700 text-white border-gray-600',
      success: 'bg-green-600 hover:bg-green-700 text-white border-green-600',
      warning: 'bg-yellow-600 hover:bg-yellow-700 text-white border-yellow-600',
      danger: 'bg-red-600 hover:bg-red-700 text-white border-red-600',
      info: 'bg-cyan-600 hover:bg-cyan-700 text-white border-cyan-600'
    };
    return variants[variant];
  };

  const getSizeClasses = () => {
    const sizes = {
      sm: 'px-3 py-1.5 text-xs',
      md: 'px-4 py-2 text-sm',
      lg: 'px-6 py-3 text-base'
    };
    return sizes[size];
  };

  const buttonContent = (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled || loading}
      className={`
        inline-flex items-center justify-center space-x-2 space-x-reverse
        border rounded-md font-medium transition-all duration-200
        focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
        disabled:opacity-50 disabled:cursor-not-allowed
        ${getVariantClasses()}
        ${getSizeClasses()}
        ${className}
      `}
    >
      {loading ? (
        <LoadingSpinner size="sm" />
      ) : (
        <>
          {icon && <span className="text-base">{icon}</span>}
          <span>{label}</span>
          {children}
        </>
      )}
    </button>
  );

  if (tooltip) {
    return (
      <Tooltip content={tooltip}>
        {buttonContent}
      </Tooltip>
    );
  }

  return buttonContent;
};

export default ActionButton;