// src/components/ui/Button.tsx

import React, { forwardRef } from 'react';
import { cn } from '../../utils/cn';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger' | 'success' | 'warning';
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl';
  loading?: boolean;
  icon?: React.ReactNode;
  iconPosition?: 'left' | 'right';
  fullWidth?: boolean;
  rounded?: 'sm' | 'md' | 'lg' | 'full';
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(({
  variant = 'primary',
  size = 'md',
  loading = false,
  icon,
  iconPosition = 'left',
  fullWidth = false,
  rounded = 'md',
  className = '',
  children,
  disabled,
  ...props
}, ref) => {
  const base = cn(
    'inline-flex items-center justify-center font-medium transition-all duration-200',
    'focus:outline-none focus:ring-2 focus:ring-offset-2',
    'disabled:opacity-60 disabled:cursor-not-allowed',
    'active:scale-[0.98]'
  );

  const variants = {
    primary: cn(
      'bg-blue-600 text-white border border-transparent',
      'hover:bg-blue-700 focus:ring-blue-500',
      'shadow-sm hover:shadow'
    ),
    secondary: cn(
      'bg-gray-600 text-white border border-transparent',
      'hover:bg-gray-700 focus:ring-gray-500',
      'shadow-sm hover:shadow'
    ),
    outline: cn(
      'bg-white text-gray-700 border border-gray-300',
      'hover:bg-gray-50 focus:ring-gray-500'
    ),
    ghost: cn(
      'bg-transparent text-gray-700 border border-transparent',
      'hover:bg-gray-100 focus:ring-gray-500'
    ),
    danger: cn(
      'bg-red-600 text-white border border-transparent',
      'hover:bg-red-700 focus:ring-red-500',
      'shadow-sm hover:shadow'
    ),
    success: cn(
      'bg-green-600 text-white border border-transparent',
      'hover:bg-green-700 focus:ring-green-500',
      'shadow-sm hover:shadow'
    ),
    warning: cn(
      'bg-amber-600 text-white border border-transparent',
      'hover:bg-amber-700 focus:ring-amber-500',
      'shadow-sm hover:shadow'
    )
  };

  const sizes = {
    xs: 'text-xs px-2 py-1',
    sm: 'text-sm px-3 py-1.5',
    md: 'text-sm px-4 py-2',
    lg: 'text-base px-5 py-2.5',
    xl: 'text-base px-6 py-3'
  };

  const roundedSizes = {
    sm: 'rounded',
    md: 'rounded-md',
    lg: 'rounded-lg',
    full: 'rounded-full'
  };

  return (
    <button
      ref={ref}
      className={cn(
        base,
        variants[variant],
        sizes[size],
        roundedSizes[rounded],
        fullWidth && 'w-full',
        className
      )}
      disabled={disabled || loading}
      {...props}
    >
      {loading && (
        <svg 
          className={cn(
            'animate-spin h-4 w-4',
            !!children && (iconPosition === 'left' ? 'mr-2' : 'ml-2')
          )}
          fill="none" 
          viewBox="0 0 24 24"
        >
          <circle 
            className="opacity-25" 
            cx="12" 
            cy="12" 
            r="10" 
            stroke="currentColor" 
            strokeWidth="4"
          />
          <path 
            className="opacity-75" 
            fill="currentColor" 
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
          />
        </svg>
      )}
      {!loading && icon && iconPosition === 'left' && (
        <span className={cn(!!children && 'mr-2')}>
          {icon}
        </span>
      )}
      {children}
      {!loading && icon && iconPosition === 'right' && (
        <span className={cn(!!children && 'ml-2')}>
          {icon}
        </span>
      )}
    </button>
  );
});

Button.displayName = 'Button';