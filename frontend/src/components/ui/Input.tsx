// src/components/ui/Input.tsx

import React, { forwardRef } from 'react';
import { cn } from '../../utils/cn';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: boolean;
  icon?: React.ReactNode;
  iconPosition?: 'left' | 'right';
  addon?: React.ReactNode;
  addonPosition?: 'left' | 'right';
}

export const Input = forwardRef<HTMLInputElement, InputProps>(({
  error = false,
  icon,
  iconPosition = 'left',
  addon,
  addonPosition = 'left',
  className = '',
  disabled,
  ...props
}, ref) => {
  const inputElement = (
    <input
      ref={ref}
      className={cn(
        'w-full px-3 py-2 text-sm border rounded-md transition-all duration-200',
        'placeholder:text-gray-400',
        'focus:outline-none focus:ring-2 focus:ring-offset-0',
        error 
          ? 'border-red-300 focus:border-red-500 focus:ring-red-500/20' 
          : 'border-gray-300 hover:border-gray-400 focus:border-blue-500 focus:ring-blue-500/20',
        disabled && 'bg-gray-50 cursor-not-allowed opacity-60',
        !!icon && iconPosition === 'left' && 'pl-10',
        !!icon && iconPosition === 'right' && 'pr-10',
        !!addon && addonPosition === 'left' && 'rounded-l-none',
        !!addon && addonPosition === 'right' && 'rounded-r-none',
        className
      )}
      disabled={disabled}
      {...props}
    />
  );

  if (!icon && !addon) {
    return inputElement;
  }

  return (
    <div className="relative flex">
      {addon && addonPosition === 'left' && (
        <span className="inline-flex items-center px-3 text-sm text-gray-600 bg-gray-50 border border-r-0 border-gray-300 rounded-l-md">
          {addon}
        </span>
      )}
      
      <div className="relative flex-1">
        {icon && (
          <div className={cn(
            'absolute top-1/2 transform -translate-y-1/2 text-gray-400 pointer-events-none',
            iconPosition === 'left' ? 'left-3' : 'right-3'
          )}>
            {icon}
          </div>
        )}
        {inputElement}
      </div>

      {addon && addonPosition === 'right' && (
        <span className="inline-flex items-center px-3 text-sm text-gray-600 bg-gray-50 border border-l-0 border-gray-300 rounded-r-md">
          {addon}
        </span>
      )}
    </div>
  );
});

Input.displayName = 'Input';