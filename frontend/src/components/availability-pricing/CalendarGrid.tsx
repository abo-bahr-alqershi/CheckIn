// src/components/availability-pricing/shared/CalendarGrid.tsx

import React, { useMemo, useState, useCallback, useRef, useEffect } from 'react';
import { format, startOfMonth, endOfMonth, eachDayOfInterval, isSameMonth, isToday, isBefore, startOfDay } from 'date-fns';
import { ar } from 'date-fns/locale';
import { cn } from '../../utils/cn';
import { Tooltip } from '../ui/Tooltip';
import { Badge } from '../ui/Badge';

interface CalendarDay {
  date: Date;
  dateString: string;
  isCurrentMonth: boolean;
  isToday: boolean;
  isPast: boolean;
  data?: any;
}

interface CalendarGridProps {
  year: number;
  month: number;
  data?: Record<string, any>;
  onDayClick?: (date: string) => void;
  onDayHover?: (date: string | null) => void;
  onRangeSelect?: (start: string, end: string) => void;
  renderDay?: (day: CalendarDay) => React.ReactNode;
  selectedDates?: string[];
  rangeStart?: string | null;
  rangeEnd?: string | null;
  multiSelect?: boolean;
  className?: string;
}

export const CalendarGrid: React.FC<CalendarGridProps> = ({
  year,
  month,
  data = {},
  onDayClick,
  onDayHover,
  onRangeSelect,
  renderDay,
  selectedDates = [],
  rangeStart,
  rangeEnd,
  multiSelect = false,
  className
}) => {
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState<string | null>(null);
  const [dragEnd, setDragEnd] = useState<string | null>(null);
  const [hoveredDate, setHoveredDate] = useState<string | null>(null);
  const gridRef = useRef<HTMLDivElement>(null);

  const days = useMemo(() => {
    const start = startOfMonth(new Date(year, month - 1));
    const end = endOfMonth(new Date(year, month - 1));
    const interval = eachDayOfInterval({ start, end });
    
    // Add padding days to complete weeks
    const startDay = start.getDay();
    const endDay = end.getDay();
    
    const paddingStart = Array.from({ length: startDay }, (_, i) => {
      const date = new Date(start);
      date.setDate(date.getDate() - (startDay - i));
      return date;
    });
    
    const paddingEnd = Array.from({ length: 6 - endDay }, (_, i) => {
      const date = new Date(end);
      date.setDate(date.getDate() + i + 1);
      return date;
    });
    
    return [...paddingStart, ...interval, ...paddingEnd].map(date => {
      const dateString = format(date, 'yyyy-MM-dd');
      // Ensure data is accessible even if undefined, and handle both normalized and non-normalized keys
      const dayData = data?.[dateString] || null;
      return {
        date,
        dateString,
        isCurrentMonth: isSameMonth(date, new Date(year, month - 1)),
        isToday: isToday(date),
        isPast: isBefore(date, startOfDay(new Date())),
        data: dayData
      };
    });
  }, [year, month, data]);

  const weekDays = useMemo(() => {
    return ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];
  }, []);

  const handleMouseDown = useCallback((date: string, e: React.MouseEvent) => {
    e.preventDefault();
    if (multiSelect) {
      setIsDragging(true);
      setDragStart(date);
      setDragEnd(date);
    }
  }, [multiSelect]);

  const handleMouseEnter = useCallback((date: string) => {
    setHoveredDate(date);
    onDayHover?.(date);
    
    if (isDragging && dragStart) {
      setDragEnd(date);
    }
  }, [isDragging, dragStart, onDayHover]);

  const handleMouseUp = useCallback(() => {
    if (isDragging && dragStart && dragEnd && onRangeSelect) {
      const start = dragStart < dragEnd ? dragStart : dragEnd;
      const end = dragStart < dragEnd ? dragEnd : dragStart;
      onRangeSelect(start, end);
    }
    setIsDragging(false);
    setDragStart(null);
    setDragEnd(null);
  }, [isDragging, dragStart, dragEnd, onRangeSelect]);

  const handleClick = useCallback((date: string, e: React.MouseEvent) => {
    if (!isDragging && onDayClick) {
      if (e.shiftKey && selectedDates.length > 0) {
        const lastSelected = selectedDates[selectedDates.length - 1];
        const start = lastSelected < date ? lastSelected : date;
        const end = lastSelected < date ? date : lastSelected;
        onRangeSelect?.(start, end);
      } else if (e.ctrlKey || e.metaKey) {
        onDayClick(date);
      } else {
        onDayClick(date);
      }
    }
  }, [isDragging, onDayClick, onRangeSelect, selectedDates]);

  useEffect(() => {
    const handleGlobalMouseUp = () => {
      if (isDragging) {
        handleMouseUp();
      }
    };

    document.addEventListener('mouseup', handleGlobalMouseUp);
    return () => document.removeEventListener('mouseup', handleGlobalMouseUp);
  }, [isDragging, handleMouseUp]);

  const isInRange = useCallback((dateString: string) => {
    if (!rangeStart || !rangeEnd) return false;
    return dateString >= rangeStart && dateString <= rangeEnd;
  }, [rangeStart, rangeEnd]);

  const isInDragRange = useCallback((dateString: string) => {
    if (!dragStart || !dragEnd) return false;
    const start = dragStart < dragEnd ? dragStart : dragEnd;
    const end = dragStart < dragEnd ? dragEnd : dragStart;
    return dateString >= start && dateString <= end;
  }, [dragStart, dragEnd]);

  return (
    <div 
      ref={gridRef}
      className={cn("bg-white rounded-lg shadow-sm border border-gray-200", className)}
      onMouseLeave={() => {
        setHoveredDate(null);
        onDayHover?.(null);
      }}
    >
      {/* Week days header */}
      <div className="grid grid-cols-7 border-b border-gray-200">
        {weekDays.map((day, index) => (
          <div 
            key={index}
            className="py-3 px-2 text-center text-sm font-medium text-gray-700 bg-gray-50"
          >
            {day}
          </div>
        ))}
      </div>

      {/* Calendar days */}
      <div className="grid grid-cols-7">
        {days.map((day) => {
          const isSelected = selectedDates.includes(day.dateString);
          const isRange = isInRange(day.dateString);
          const isDragRange = isInDragRange(day.dateString);
          const isRangeStart = day.dateString === rangeStart || day.dateString === dragStart;
          const isRangeEnd = day.dateString === rangeEnd || day.dateString === dragEnd;

          return (
            <div
              key={day.dateString}
              className={cn(
                "relative aspect-square border-r border-b border-gray-100 p-1",
                "transition-all duration-150 select-none",
                !day.isCurrentMonth && "bg-gray-50 opacity-50",
                day.isToday && "ring-2 ring-blue-400 ring-inset",
                day.isPast && "opacity-60",
                (isSelected || isRange || isDragRange) && day.isCurrentMonth && "bg-blue-50",
                isRangeStart && "bg-blue-100 rounded-l-lg",
                isRangeEnd && "bg-blue-100 rounded-r-lg",
                hoveredDate === day.dateString && "bg-gray-100",
                "cursor-pointer hover:bg-gray-50"
              )}
              onMouseDown={(e) => handleMouseDown(day.dateString, e)}
              onMouseEnter={() => handleMouseEnter(day.dateString)}
              onClick={(e) => handleClick(day.dateString, e)}
            >
              {renderDay ? (
                renderDay(day)
              ) : (
                <div className="flex flex-col h-full">
                  <div className={cn(
                    "text-sm font-medium",
                    day.isToday ? "text-blue-600" : "text-gray-900"
                  )}>
                    {format(day.date, 'd')}
                  </div>
                  {day.data && (
                    <div className="flex-1 mt-1">
                      {/* Default data rendering */}
                    </div>
                  )}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
};