import React, { useState, useRef, useEffect } from 'react';

interface DateRange {
  startDate: string;
  endDate: string;
}

interface DateRangePickerProps {
  value?: DateRange;
  onChange: (value: DateRange) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  required?: boolean;
  minDate?: string;
  maxDate?: string;
  quickSelections?: boolean;
}

const DateRangePicker: React.FC<DateRangePickerProps> = ({
  value,
  onChange,
  placeholder = 'اختر فترة زمنية',
  className = '',
  disabled = false,
  required = false,
  minDate,
  maxDate,
  quickSelections = true
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [startDate, setStartDate] = useState<Date | null>(value?.startDate ? new Date(value.startDate) : null);
  const [endDate, setEndDate] = useState<Date | null>(value?.endDate ? new Date(value.endDate) : null);
  const [hoverDate, setHoverDate] = useState<Date | null>(null);
  const [selectingStart, setSelectingStart] = useState(true);
  const [currentMonth, setCurrentMonth] = useState(new Date());
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (value?.startDate && value?.endDate) {
      setStartDate(new Date(value.startDate));
      setEndDate(new Date(value.endDate));
    }
  }, [value]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const formatDisplayValue = () => {
    if (!startDate || !endDate) return '';
    
    const formatDate = (date: Date) => {
      const year = date.getFullYear();
      const month = (date.getMonth() + 1).toString().padStart(2, '0');
      const day = date.getDate().toString().padStart(2, '0');
      return `${year}/${month}/${day}`;
    };

    return `${formatDate(startDate)} - ${formatDate(endDate)}`;
  };

  const handleDateSelect = (date: Date) => {
    if (selectingStart || !startDate) {
      setStartDate(date);
      setEndDate(null);
      setSelectingStart(false);
    } else {
      if (date < startDate) {
        setStartDate(date);
        setEndDate(startDate);
      } else {
        setEndDate(date);
      }
      
      onChange({
        startDate: (date < startDate ? date : startDate).toISOString(),
        endDate: (date < startDate ? startDate : date).toISOString()
      });
      
      setSelectingStart(true);
      setIsOpen(false);
    }
  };

  const handleQuickSelection = (type: string) => {
    const now = new Date();
    let start: Date, end: Date;

    switch (type) {
      case 'today':
        start = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        end = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);
        break;
      case 'yesterday':
        start = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 1);
        end = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        break;
      case 'thisWeek':
        const dayOfWeek = (now.getDay() + 1) % 7; // Saturday = 0
        start = new Date(now.getTime() - dayOfWeek * 24 * 60 * 60 * 1000);
        end = new Date(start.getTime() + 7 * 24 * 60 * 60 * 1000);
        break;
      case 'lastWeek':
        const lastWeekDay = (now.getDay() + 1) % 7;
        const lastWeekStart = new Date(now.getTime() - (lastWeekDay + 7) * 24 * 60 * 60 * 1000);
        start = lastWeekStart;
        end = new Date(lastWeekStart.getTime() + 7 * 24 * 60 * 60 * 1000);
        break;
      case 'thisMonth':
        start = new Date(now.getFullYear(), now.getMonth(), 1);
        end = new Date(now.getFullYear(), now.getMonth() + 1, 1);
        break;
      case 'lastMonth':
        start = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        end = new Date(now.getFullYear(), now.getMonth(), 1);
        break;
      case 'last7Days':
        start = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
        end = now;
        break;
      case 'last30Days':
        start = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
        end = now;
        break;
      default:
        return;
    }

    setStartDate(start);
    setEndDate(end);
    onChange({
      startDate: start.toISOString(),
      endDate: end.toISOString()
    });
    setIsOpen(false);
  };

  const generateCalendarDays = () => {
    const year = currentMonth.getFullYear();
    const month = currentMonth.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const firstDayOfWeek = (firstDay.getDay() + 1) % 7;
    const daysInMonth = lastDay.getDate();

    const days: Date[] = [];
    
    const prevMonth = new Date(year, month - 1, 0);
    for (let i = firstDayOfWeek - 1; i >= 0; i--) {
      days.push(new Date(year, month - 1, prevMonth.getDate() - i));
    }

    for (let day = 1; day <= daysInMonth; day++) {
      days.push(new Date(year, month, day));
    }

    const totalCells = 42;
    const remainingCells = totalCells - days.length;
    for (let day = 1; day <= remainingCells; day++) {
      days.push(new Date(year, month + 1, day));
    }

    return days;
  };

  const isDateDisabled = (date: Date) => {
    if (minDate && date < new Date(minDate)) return true;
    if (maxDate && date > new Date(maxDate)) return true;
    return false;
  };

  const isDateInRange = (date: Date) => {
    if (!startDate || !endDate) {
      if (startDate && hoverDate && !selectingStart) {
        const rangeStart = startDate < hoverDate ? startDate : hoverDate;
        const rangeEnd = startDate < hoverDate ? hoverDate : startDate;
        return date >= rangeStart && date <= rangeEnd;
      }
      return false;
    }
    return date >= startDate && date <= endDate;
  };

  const isDateSelected = (date: Date) => {
    if (startDate && 
        date.getDate() === startDate.getDate() &&
        date.getMonth() === startDate.getMonth() &&
        date.getFullYear() === startDate.getFullYear()) {
      return true;
    }
    if (endDate && 
        date.getDate() === endDate.getDate() &&
        date.getMonth() === endDate.getMonth() &&
        date.getFullYear() === endDate.getFullYear()) {
      return true;
    }
    return false;
  };

  const calendarDays = generateCalendarDays();
  const weekDays = ['س', 'ح', 'ن', 'ث', 'ر', 'خ', 'ج'];
  const monthNames = [
    'يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو',
    'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'
  ];

  return (
    <div className={`relative ${className}`} ref={dropdownRef}>
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        disabled={disabled}
        className={`
          w-full flex items-center justify-between px-3 py-2 border rounded-md shadow-sm
          bg-white text-right focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
          ${disabled ? 'bg-gray-50 text-gray-400 cursor-not-allowed' : 'hover:border-gray-400'}
          ${(!startDate || !endDate) && required ? 'border-red-300' : 'border-gray-300'}
        `}
      >
        <span className={startDate && endDate ? 'text-gray-900' : 'text-gray-500'}>
          {formatDisplayValue() || placeholder}
        </span>
        <span className="text-gray-400">📅</span>
      </button>

      {isOpen && (
        <div className="absolute z-50 w-96 mt-1 bg-white border border-gray-300 rounded-md shadow-lg">
          {quickSelections && (
            <div className="p-3 border-b border-gray-200">
              <div className="grid grid-cols-2 gap-2 text-xs">
                <button
                  type="button"
                  onClick={() => handleQuickSelection('today')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  اليوم
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('yesterday')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  أمس
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('thisWeek')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  هذا الأسبوع
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('lastWeek')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  الأسبوع الماضي
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('thisMonth')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  هذا الشهر
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('lastMonth')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  الشهر الماضي
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('last7Days')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  آخر 7 أيام
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('last30Days')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  آخر 30 يوم
                </button>
              </div>
            </div>
          )}

          <div className="flex items-center justify-between p-3 border-b border-gray-200">
            <button
              type="button"
              onClick={() => setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() - 1))}
              className="p-1 hover:bg-gray-100 rounded"
            >
              →
            </button>
            <h3 className="text-sm font-medium">
              {monthNames[currentMonth.getMonth()]} {currentMonth.getFullYear()}
            </h3>
            <button
              type="button"
              onClick={() => setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1))}
              className="p-1 hover:bg-gray-100 rounded"
            >
              ←
            </button>
          </div>

          <div className="p-3">
            <div className="grid grid-cols-7 gap-1 mb-2">
              {weekDays.map((day) => (
                <div key={day} className="text-xs font-medium text-gray-500 text-center p-1">
                  {day}
                </div>
              ))}
            </div>

            <div className="grid grid-cols-7 gap-1">
              {calendarDays.map((day, index) => {
                const isCurrentMonth = day.getMonth() === currentMonth.getMonth();
                const isSelected = isDateSelected(day);
                const isInRange = isDateInRange(day);
                const isToday = 
                  day.getDate() === new Date().getDate() &&
                  day.getMonth() === new Date().getMonth() &&
                  day.getFullYear() === new Date().getFullYear();
                const isDisabled = isDateDisabled(day);

                return (
                  <button
                    key={index}
                    type="button"
                    onClick={() => !isDisabled && handleDateSelect(day)}
                    onMouseEnter={() => setHoverDate(day)}
                    onMouseLeave={() => setHoverDate(null)}
                    disabled={isDisabled}
                    className={`
                      p-1 text-xs rounded text-center h-8 w-8 mx-auto
                      ${!isCurrentMonth ? 'text-gray-300' : 'text-gray-900'}
                      ${isSelected ? 'bg-blue-600 text-white' : ''}
                      ${isInRange && !isSelected ? 'bg-blue-100 text-blue-600' : ''}
                      ${isToday && !isSelected && !isInRange ? 'bg-gray-100 text-gray-900' : ''}
                      ${isDisabled ? 'cursor-not-allowed opacity-50' : 'hover:bg-gray-100'}
                      ${isSelected ? 'hover:bg-blue-700' : ''}
                    `}
                  >
                    {day.getDate()}
                  </button>
                );
              })}
            </div>
          </div>

          <div className="flex justify-between items-center p-3 border-t border-gray-200 text-xs text-gray-600">
            <span>
              {selectingStart ? 'اختر تاريخ البداية' : 'اختر تاريخ النهاية'}
            </span>
            <div className="flex space-x-2 space-x-reverse">
              <button
                type="button"
                onClick={() => {
                  setStartDate(null);
                  setEndDate(null);
                  setSelectingStart(true);
                }}
                className="text-gray-600 hover:text-gray-800"
              >
                مسح
              </button>
              <button
                type="button"
                onClick={() => setIsOpen(false)}
                className="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700"
              >
                تأكيد
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default DateRangePicker;