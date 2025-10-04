// src/components/ui/DatePicker.tsx

import React, { useState, useRef, useEffect } from 'react';
import { Calendar, ChevronLeft, ChevronRight } from 'lucide-react';
import { cn } from '../../utils/cn';
import { format, parse, isValid } from 'date-fns';
import { ar } from 'date-fns/locale';

interface DatePickerProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  value?: string;
  onChange?: (date: string) => void;
  format?: string;
  locale?: any;
  minDate?: string;
  maxDate?: string;
  showIcon?: boolean;
}

export const DatePicker: React.FC<DatePickerProps> = ({ 
  value = '',
  onChange,
  format: dateFormat = 'yyyy-MM-dd',
  locale = ar,
  minDate,
  maxDate,
  showIcon = true,
  className = '',
  disabled,
  ...props 
}) => {
  const [showCalendar, setShowCalendar] = useState(false);
  const [inputValue, setInputValue] = useState(value);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setShowCalendar(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleDateSelect = (date: string) => {
    setInputValue(date);
    onChange?.(date);
    setShowCalendar(false);
  };

  return (
    <div className="relative" ref={containerRef}>
      <div className="relative">
        {showIcon && (
          <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400 pointer-events-none" />
        )}
        <input
          type="date"
          value={inputValue}
          onChange={(e) => handleDateSelect(e.target.value)}
          className={cn(
            'w-full px-3 py-2 text-sm border border-gray-300 rounded-md',
            'focus:ring-2 focus:ring-blue-500 focus:border-blue-500',
            'hover:border-gray-400 transition-colors duration-200',
            showIcon && 'pl-10',
            disabled && 'bg-gray-50 cursor-not-allowed opacity-60',
            className
          )}
          min={minDate}
          max={maxDate}
          disabled={disabled}
          {...props}
        />
      </div>
    </div>
  );
};