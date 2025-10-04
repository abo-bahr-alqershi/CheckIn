// src/components/ui/DateRangePicker.tsx

import React, { useState } from 'react';
import { Calendar, ArrowRight } from 'lucide-react';
import { cn } from '../../utils/cn';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';

interface DateRangePickerProps {
  startDate?: string;
  endDate?: string;
  onChange: (start: string, end: string) => void;
  minDate?: string;
  maxDate?: string;
  disabled?: boolean;
  className?: string;
  placeholder?: { start?: string; end?: string };
}

export const DateRangePicker: React.FC<DateRangePickerProps> = ({ 
  startDate = '', 
  endDate = '', 
  onChange,
  minDate,
  maxDate,
  disabled = false,
  className = '',
  placeholder = { start: 'من تاريخ', end: 'إلى تاريخ' }
}) => {
  const [focused, setFocused] = useState<'start' | 'end' | null>(null);

  const handleStartChange = (date: string) => {
    onChange(date, endDate);
    if (date && endDate && date > endDate) {
      onChange(date, date);
    }
  };

  const handleEndChange = (date: string) => {
    onChange(startDate, date);
    if (startDate && date && date < startDate) {
      onChange(date, date);
    }
  };

  return (
    <div className={cn('flex items-center gap-2', className)}>
      <div className="relative flex-1">
        <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400 pointer-events-none" />
        <input
          type="date"
          value={startDate}
          onChange={(e) => handleStartChange(e.target.value)}
          onFocus={() => setFocused('start')}
          onBlur={() => setFocused(null)}
          min={minDate}
          max={endDate || maxDate}
          disabled={disabled}
          placeholder={placeholder.start}
          className={cn(
            'w-full pl-10 pr-3 py-2 text-sm border rounded-md transition-all duration-200',
            'hover:border-gray-400',
            focused === 'start' ? 'border-blue-500 ring-2 ring-blue-500/20' : 'border-gray-300',
            disabled && 'bg-gray-50 cursor-not-allowed opacity-60'
          )}
        />
      </div>
      
      <ArrowRight className="h-4 w-4 text-gray-400 flex-shrink-0" />
      
      <div className="relative flex-1">
        <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400 pointer-events-none" />
        <input
          type="date"
          value={endDate}
          onChange={(e) => handleEndChange(e.target.value)}
          onFocus={() => setFocused('end')}
          onBlur={() => setFocused(null)}
          min={startDate || minDate}
          max={maxDate}
          disabled={disabled || !startDate}
          placeholder={placeholder.end}
          className={cn(
            'w-full pl-10 pr-3 py-2 text-sm border rounded-md transition-all duration-200',
            'hover:border-gray-400',
            focused === 'end' ? 'border-blue-500 ring-2 ring-blue-500/20' : 'border-gray-300',
            (disabled || !startDate) && 'bg-gray-50 cursor-not-allowed opacity-60'
          )}
        />
      </div>
    </div>
  );
};