// src/components/ui/Switch.tsx

import React from 'react';
import { cn } from '../../utils/cn';

interface SwitchProps {
  checked?: boolean;
  onChange?: (checked: boolean) => void;
  disabled?: boolean;
  size?: 'sm' | 'md' | 'lg';
  label?: string;
  labelPosition?: 'left' | 'right';
  color?: 'primary' | 'success' | 'danger';
  className?: string;
}

export const Switch: React.FC<SwitchProps> = ({ 
  checked = false, 
  onChange,
  disabled = false,
  size = 'md',
  label,
  labelPosition = 'right',
  color = 'primary',
  className = ''
}) => {
  const sizes = {
    sm: { container: 'h-5 w-9', thumb: 'h-4 w-4', translate: 'translate-x-4' },
    md: { container: 'h-6 w-11', thumb: 'h-5 w-5', translate: 'translate-x-5' },
    lg: { container: 'h-7 w-14', thumb: 'h-6 w-6', translate: 'translate-x-7' }
  };

  const colors = {
    primary: 'bg-blue-600',
    success: 'bg-green-600',
    danger: 'bg-red-600'
  };

  const currentSize = sizes[size];

  const switchElement = (
    <button
      type="button"
      role="switch"
      aria-checked={checked}
      onClick={() => !disabled && onChange?.(!checked)}
      disabled={disabled}
      className={cn(
        'relative inline-flex items-center rounded-full transition-colors duration-200',
        'focus:outline-none focus:ring-2 focus:ring-offset-2',
        currentSize.container,
        checked ? colors[color] : 'bg-gray-300',
        disabled && 'opacity-50 cursor-not-allowed',
        color === 'primary' && 'focus:ring-blue-500',
        color === 'success' && 'focus:ring-green-500',
        color === 'danger' && 'focus:ring-red-500'
      )}
    >
      <span
        className={cn(
          'inline-block transform rounded-full bg-white shadow-lg transition-transform duration-200',
          currentSize.thumb,
          checked ? currentSize.translate : 'translate-x-1'
        )}
      />
    </button>
  );

  if (!label) {
    return switchElement;
  }

  return (
    <label className={cn(
      'inline-flex items-center gap-3 cursor-pointer',
      disabled && 'cursor-not-allowed opacity-60',
      className
    )}>
      {labelPosition === 'left' && (
        <span className="text-sm text-gray-700">{label}</span>
      )}
      {switchElement}
      {labelPosition === 'right' && (
        <span className="text-sm text-gray-700">{label}</span>
      )}
    </label>
  );
};