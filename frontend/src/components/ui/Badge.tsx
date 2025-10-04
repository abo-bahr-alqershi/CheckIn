// src/components/ui/Badge.tsx

import React from 'react';
import { cn } from '../../utils/cn';

interface BadgeProps {
  children: React.ReactNode;
  variant?: 'default' | 'primary' | 'success' | 'warning' | 'danger' | 'info' | 'secondary';
  size?: 'xs' | 'sm' | 'md' | 'lg';
  rounded?: 'sm' | 'md' | 'lg' | 'full';
  icon?: React.ReactNode;
  removable?: boolean;
  onRemove?: () => void;
  className?: string;
  pulse?: boolean;
}

export const Badge: React.FC<BadgeProps> = ({ 
  children, 
  variant = 'default',
  size = 'sm',
  rounded = 'md',
  icon,
  removable = false,
  onRemove,
  className = '',
  pulse = false
}) => {
  const variants = {
    default: 'bg-gray-100 text-gray-800 border-gray-200',
    primary: 'bg-blue-100 text-blue-800 border-blue-200',
    success: 'bg-green-100 text-green-800 border-green-200',
    warning: 'bg-amber-100 text-amber-800 border-amber-200',
    danger: 'bg-red-100 text-red-800 border-red-200',
    info: 'bg-cyan-100 text-cyan-800 border-cyan-200',
    secondary: 'bg-purple-100 text-purple-800 border-purple-200'
  };

  const sizes = {
    xs: 'px-1.5 py-0.5 text-xs',
    sm: 'px-2 py-0.5 text-xs',
    md: 'px-2.5 py-1 text-sm',
    lg: 'px-3 py-1.5 text-sm'
  };

  const roundedSizes = {
    sm: 'rounded',
    md: 'rounded-md',
    lg: 'rounded-lg',
    full: 'rounded-full'
  };

  return (
    <span 
      className={cn(
        'inline-flex items-center font-medium border transition-all duration-200',
        'hover:shadow-sm',
        variants[variant],
        sizes[size],
        roundedSizes[rounded],
        pulse && 'animate-pulse',
        className
      )}
    >
      {icon && (
        <span className="mr-1.5 -ml-0.5">
          {icon}
        </span>
      )}
      {children}
      {removable && (
        <button
          onClick={onRemove}
          className={cn(
            'ml-1.5 -mr-0.5 hover:bg-black/10 rounded-full p-0.5 transition-colors',
            'focus:outline-none focus:ring-1 focus:ring-offset-1',
            variant === 'primary' && 'focus:ring-blue-500',
            variant === 'success' && 'focus:ring-green-500',
            variant === 'warning' && 'focus:ring-amber-500',
            variant === 'danger' && 'focus:ring-red-500'
          )}
        >
          <svg className="h-3 w-3" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
          </svg>
        </button>
      )}
    </span>
  );
};