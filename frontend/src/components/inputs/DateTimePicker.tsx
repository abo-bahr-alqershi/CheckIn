import React, { useState, useRef, useEffect } from 'react';

interface DateTimePickerProps {
  value?: string;
  onChange: (value: string) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  required?: boolean;
  showTime?: boolean;
  minDate?: string;
  maxDate?: string;
  quickSelections?: boolean;
}

const DateTimePicker: React.FC<DateTimePickerProps> = ({
  value = '',
  onChange,
  placeholder = 'اختر التاريخ والوقت',
  className = '',
  disabled = false,
  required = false,
  showTime = true,
  minDate,
  maxDate,
  quickSelections = true
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedDate, setSelectedDate] = useState<Date | null>(value ? new Date(value) : null);
  const [currentMonth, setCurrentMonth] = useState(new Date());
  const [timeValue, setTimeValue] = useState({ hours: '12', minutes: '00' });
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (value) {
      const date = new Date(value);
      setSelectedDate(date);
      setCurrentMonth(date);
      setTimeValue({
        hours: date.getHours().toString().padStart(2, '0'),
        minutes: date.getMinutes().toString().padStart(2, '0')
      });
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

  const formatDisplayValue = (date: Date) => {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    
    if (showTime) {
      const hours = date.getHours().toString().padStart(2, '0');
      const minutes = date.getMinutes().toString().padStart(2, '0');
      return `${year}/${month}/${day} ${hours}:${minutes}`;
    }
    return `${year}/${month}/${day}`;
  };

  const handleDateSelect = (date: Date) => {
    if (showTime && selectedDate) {
      const newDate = new Date(date);
      newDate.setHours(selectedDate.getHours());
      newDate.setMinutes(selectedDate.getMinutes());
      setSelectedDate(newDate);
      onChange(newDate.toISOString());
    } else {
      setSelectedDate(date);
      onChange(date.toISOString());
      if (!showTime) {
        setIsOpen(false);
      }
    }
  };

  const handleTimeChange = (hours: string, minutes: string) => {
    if (selectedDate) {
      const newDate = new Date(selectedDate);
      newDate.setHours(parseInt(hours));
      newDate.setMinutes(parseInt(minutes));
      setSelectedDate(newDate);
      setTimeValue({ hours, minutes });
      onChange(newDate.toISOString());
    }
  };

  const handleQuickSelection = (type: string) => {
    const now = new Date();
    let newDate: Date;

    switch (type) {
      case 'today':
        newDate = now;
        break;
      case 'tomorrow':
        newDate = new Date(now.getTime() + 24 * 60 * 60 * 1000);
        break;
      case 'yesterday':
        newDate = new Date(now.getTime() - 24 * 60 * 60 * 1000);
        break;
      case 'weekStart':
        const dayOfWeek = now.getDay();
        const daysToSaturday = (6 - dayOfWeek) % 7;
        newDate = new Date(now.getTime() - daysToSaturday * 24 * 60 * 60 * 1000);
        break;
      case 'weekEnd':
        const dayOfWeekEnd = now.getDay();
        const daysToFriday = (5 + 7 - dayOfWeekEnd) % 7;
        newDate = new Date(now.getTime() + daysToFriday * 24 * 60 * 60 * 1000);
        break;
      default:
        return;
    }

    handleDateSelect(newDate);
  };

  const generateCalendarDays = () => {
    const year = currentMonth.getFullYear();
    const month = currentMonth.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const firstDayOfWeek = (firstDay.getDay() + 1) % 7; // Adjust for Saturday start
    const daysInMonth = lastDay.getDate();

    const days: Date[] = [];
    
    // Previous month days
    const prevMonth = new Date(year, month - 1, 0);
    for (let i = firstDayOfWeek - 1; i >= 0; i--) {
      days.push(new Date(year, month - 1, prevMonth.getDate() - i));
    }

    // Current month days
    for (let day = 1; day <= daysInMonth; day++) {
      days.push(new Date(year, month, day));
    }

    // Next month days
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
          ${!selectedDate && required ? 'border-red-300' : 'border-gray-300'}
        `}
      >
        <span className={selectedDate ? 'text-gray-900' : 'text-gray-500'}>
          {selectedDate ? formatDisplayValue(selectedDate) : placeholder}
        </span>
        <span className="text-gray-400">📅</span>
      </button>

      {isOpen && (
        <div className="absolute z-50 w-80 mt-1 bg-white border border-gray-300 rounded-md shadow-lg">
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
                  onClick={() => handleQuickSelection('tomorrow')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  غداً
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('weekStart')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  بداية الأسبوع
                </button>
                <button
                  type="button"
                  onClick={() => handleQuickSelection('weekEnd')}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  نهاية الأسبوع
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
                const isSelected = selectedDate && 
                  day.getDate() === selectedDate.getDate() &&
                  day.getMonth() === selectedDate.getMonth() &&
                  day.getFullYear() === selectedDate.getFullYear();
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
                    disabled={isDisabled}
                    className={`
                      p-1 text-xs rounded text-center h-8 w-8 mx-auto
                      ${!isCurrentMonth ? 'text-gray-300' : 'text-gray-900'}
                      ${isSelected ? 'bg-blue-600 text-white' : ''}
                      ${isToday && !isSelected ? 'bg-blue-100 text-blue-600' : ''}
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

          {showTime && (
            <div className="p-3 border-t border-gray-200">
              <div className="flex items-center justify-center space-x-2 space-x-reverse">
                <select
                  value={timeValue.hours}
                  onChange={(e) => handleTimeChange(e.target.value, timeValue.minutes)}
                  className="px-2 py-1 border border-gray-300 rounded text-center text-sm"
                >
                  {Array.from({ length: 24 }, (_, i) => {
                    const hour = i.toString().padStart(2, '0');
                    return (
                      <option key={hour} value={hour}>
                        {hour}
                      </option>
                    );
                  })}
                </select>
                <span className="text-gray-500">:</span>
                <select
                  value={timeValue.minutes}
                  onChange={(e) => handleTimeChange(timeValue.hours, e.target.value)}
                  className="px-2 py-1 border border-gray-300 rounded text-center text-sm"
                >
                  {Array.from({ length: 60 }, (_, i) => {
                    const minute = i.toString().padStart(2, '0');
                    return (
                      <option key={minute} value={minute}>
                        {minute}
                      </option>
                    );
                  })}
                </select>
              </div>
            </div>
          )}

          <div className="flex justify-end space-x-2 space-x-reverse p-3 border-t border-gray-200">
            <button
              type="button"
              onClick={() => setIsOpen(false)}
              className="px-3 py-1 text-sm text-gray-600 hover:text-gray-800"
            >
              إلغاء
            </button>
            <button
              type="button"
              onClick={() => {
                if (selectedDate) {
                  onChange(selectedDate.toISOString());
                }
                setIsOpen(false);
              }}
              className="px-3 py-1 text-sm bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              تأكيد
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default DateTimePicker;