// src/components/ui/Textarea.tsx

import React, { forwardRef } from 'react';
import { cn } from '../../utils/cn';

interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  error?: boolean;
  resize?: 'none' | 'vertical' | 'horizontal' | 'both';
  showCount?: boolean;
  maxLength?: number;
}

export const Textarea = forwardRef<HTMLTextAreaElement, TextareaProps>(({
  error = false,
  resize = 'vertical',
  showCount = false,
  maxLength,
  className = '',
  disabled,
  value,
  ...props
}, ref) => {
  const resizeClasses = {
    none: 'resize-none',
    vertical: 'resize-y',
    horizontal: 'resize-x',
    both: 'resize'
  };

  const currentLength = value ? String(value).length : 0;

  return (
    <div className="relative">
      <textarea
        ref={ref}
        value={value}
        maxLength={maxLength}
        className={cn(
          'w-full px-3 py-2 text-sm border rounded-md transition-all duration-200',
          'placeholder:text-gray-400',
          'focus:outline-none focus:ring-2 focus:ring-offset-0',
          error 
            ? 'border-red-300 focus:border-red-500 focus:ring-red-500/20' 
            : 'border-gray-300 hover:border-gray-400 focus:border-blue-500 focus:ring-blue-500/20',
          disabled && 'bg-gray-50 cursor-not-allowed opacity-60',
          resizeClasses[resize],
          showCount && maxLength !== undefined && 'pb-6',
          className
        )}
        disabled={disabled}
        {...props}
      />
      
      {showCount && maxLength !== undefined && (
        <div className={cn(
          'absolute bottom-2 right-2 text-xs',
          currentLength === maxLength ? 'text-red-500' : 'text-gray-400'
        )}>
          {currentLength}/{maxLength}
        </div>
      )}
    </div>
  );
});

Textarea.displayName = 'Textarea';