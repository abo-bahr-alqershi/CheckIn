// src/components/ui/Select.tsx

import React, { forwardRef } from 'react';
import { ChevronDown } from 'lucide-react';
import { cn } from '../../utils/cn';

interface SelectProps extends Omit<React.SelectHTMLAttributes<HTMLSelectElement>, 'onChange'> {
  onChange?: (value: string) => void;
  error?: boolean;
  icon?: React.ReactNode;
  placeholder?: string;
}

export const Select = forwardRef<HTMLSelectElement, SelectProps>(({ 
  children, 
  className = '', 
  onChange,
  error = false,
  icon,
  placeholder,
  disabled,
  ...props 
}, ref) => {
  return (
    <div className="relative">
      {icon && (
        <div className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 pointer-events-none">
          {icon}
        </div>
      )}
      
      <select
        ref={ref}
        className={cn(
          'w-full appearance-none px-3 py-2 pr-10 text-sm border rounded-md transition-all duration-200',
          'focus:outline-none focus:ring-2 focus:ring-offset-0',
          error 
            ? 'border-red-300 focus:border-red-500 focus:ring-red-500/20' 
            : 'border-gray-300 hover:border-gray-400 focus:border-blue-500 focus:ring-blue-500/20',
          disabled && 'bg-gray-50 cursor-not-allowed opacity-60',
          !!icon && 'pl-10',
          className
        )}
        onChange={(e) => onChange?.(e.target.value)}
        disabled={disabled}
        {...props}
      >
        {placeholder && (
          <option value="" disabled>
            {placeholder}
          </option>
        )}
        {children}
      </select>
      
      <ChevronDown className="absolute right-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400 pointer-events-none" />
    </div>
  );
});

Select.displayName = 'Select';