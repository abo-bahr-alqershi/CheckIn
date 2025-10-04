import React, { useState, useRef, useEffect } from 'react';

interface TimePickerProps {
  value?: string;
  onChange: (value: string) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  required?: boolean;
  format24Hours?: boolean;
  minuteStep?: number;
  minTime?: string;
  maxTime?: string;
}

const TimePicker: React.FC<TimePickerProps> = ({
  value = '',
  onChange,
  placeholder = 'اختر الوقت',
  className = '',
  disabled = false,
  required = false,
  format24Hours = true,
  minuteStep = 5,
  minTime,
  maxTime
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [hours, setHours] = useState('12');
  const [minutes, setMinutes] = useState('00');
  const [period, setPeriod] = useState<'AM' | 'PM'>('AM');
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (value) {
      const [time] = value.split('T');
      if (time) {
        const [h, m] = time.split(':');
        const hour24 = parseInt(h);
        
        if (format24Hours) {
          setHours(h.padStart(2, '0'));
        } else {
          const hour12 = hour24 === 0 ? 12 : hour24 > 12 ? hour24 - 12 : hour24;
          setHours(hour12.toString().padStart(2, '0'));
          setPeriod(hour24 >= 12 ? 'PM' : 'AM');
        }
        setMinutes(m.padStart(2, '0'));
      }
    }
  }, [value, format24Hours]);

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
    if (!hours || !minutes) return '';
    
    if (format24Hours) {
      return `${hours}:${minutes}`;
    } else {
      return `${hours}:${minutes} ${period === 'AM' ? 'ص' : 'م'}`;
    }
  };

  const handleTimeChange = (newHours: string, newMinutes: string, newPeriod?: 'AM' | 'PM') => {
    setHours(newHours);
    setMinutes(newMinutes);
    if (newPeriod) setPeriod(newPeriod);

    let hour24 = parseInt(newHours);
    if (!format24Hours) {
      const currentPeriod = newPeriod || period;
      if (currentPeriod === 'PM' && hour24 !== 12) {
        hour24 += 12;
      } else if (currentPeriod === 'AM' && hour24 === 12) {
        hour24 = 0;
      }
    }

    const timeString = `${hour24.toString().padStart(2, '0')}:${newMinutes}`;
    
    // Check time constraints
    if (isTimeDisabled(timeString)) {
      return;
    }

    onChange(timeString);
  };

  const isTimeDisabled = (timeString: string) => {
    if (minTime && timeString < minTime) return true;
    if (maxTime && timeString > maxTime) return true;
    return false;
  };

  const generateHours = () => {
    if (format24Hours) {
      return Array.from({ length: 24 }, (_, i) => i.toString().padStart(2, '0'));
    } else {
      return Array.from({ length: 12 }, (_, i) => (i + 1).toString().padStart(2, '0'));
    }
  };

  const generateMinutes = () => {
    const minutes: string[] = [];
    for (let i = 0; i < 60; i += minuteStep) {
      minutes.push(i.toString().padStart(2, '0'));
    }
    return minutes;
  };

  const quickTimes = [
    { label: 'الآن', value: () => {
      const now = new Date();
      return `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}`;
    }},
    { label: '9:00 ص', value: () => '09:00' },
    { label: '12:00 م', value: () => '12:00' },
    { label: '6:00 م', value: () => '18:00' },
    { label: '9:00 م', value: () => '21:00' }
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
          ${!value && required ? 'border-red-300' : 'border-gray-300'}
        `}
      >
        <span className={value ? 'text-gray-900' : 'text-gray-500'}>
          {formatDisplayValue() || placeholder}
        </span>
        <span className="text-gray-400">🕐</span>
      </button>

      {isOpen && (
        <div className="absolute z-50 w-72 mt-1 bg-white border border-gray-300 rounded-md shadow-lg">
          {/* Quick Times */}
          <div className="p-3 border-b border-gray-200">
            <div className="grid grid-cols-3 gap-2 text-xs">
              {quickTimes.map((time) => (
                <button
                  key={time.label}
                  type="button"
                  onClick={() => {
                    const timeValue = time.value();
                    onChange(timeValue);
                    setIsOpen(false);
                  }}
                  className="px-2 py-1 text-blue-600 hover:bg-blue-50 rounded text-center"
                >
                  {time.label}
                </button>
              ))}
            </div>
          </div>

          {/* Time Selectors */}
          <div className="p-4">
            <div className="flex items-center justify-center space-x-4 space-x-reverse">
              {/* Hours */}
              <div className="text-center">
                <label className="block text-xs text-gray-600 mb-2">الساعة</label>
                <select
                  value={hours}
                  onChange={(e) => handleTimeChange(e.target.value, minutes, period)}
                  className="w-16 px-2 py-1 border border-gray-300 rounded text-center text-sm"
                >
                  {generateHours().map((hour) => (
                    <option key={hour} value={hour}>
                      {hour}
                    </option>
                  ))}
                </select>
              </div>

              <span className="text-gray-500 text-lg mt-6">:</span>

              {/* Minutes */}
              <div className="text-center">
                <label className="block text-xs text-gray-600 mb-2">الدقيقة</label>
                <select
                  value={minutes}
                  onChange={(e) => handleTimeChange(hours, e.target.value, period)}
                  className="w-16 px-2 py-1 border border-gray-300 rounded text-center text-sm"
                >
                  {generateMinutes().map((minute) => (
                    <option key={minute} value={minute}>
                      {minute}
                    </option>
                  ))}
                </select>
              </div>

              {/* AM/PM */}
              {!format24Hours && (
                <div className="text-center">
                  <label className="block text-xs text-gray-600 mb-2">الفترة</label>
                  <select
                    value={period}
                    onChange={(e) => handleTimeChange(hours, minutes, e.target.value as 'AM' | 'PM')}
                    className="w-16 px-2 py-1 border border-gray-300 rounded text-center text-sm"
                  >
                    <option value="AM">ص</option>
                    <option value="PM">م</option>
                  </select>
                </div>
              )}
            </div>

            {/* Manual Input */}
            <div className="mt-4 text-center">
              <input
                type="time"
                value={`${format24Hours ? hours : (period === 'PM' && hours !== '12' ? (parseInt(hours) + 12).toString().padStart(2, '0') : (period === 'AM' && hours === '12' ? '00' : hours))}:${minutes}`}
                onChange={(e) => {
                  const [h, m] = e.target.value.split(':');
                  const hour24 = parseInt(h);
                  
                  if (format24Hours) {
                    handleTimeChange(h, m);
                  } else {
                    const hour12 = hour24 === 0 ? 12 : hour24 > 12 ? hour24 - 12 : hour24;
                    const newPeriod = hour24 >= 12 ? 'PM' : 'AM';
                    handleTimeChange(hour12.toString().padStart(2, '0'), m, newPeriod);
                  }
                }}
                className="px-3 py-1 border border-gray-300 rounded text-sm"
              />
            </div>
          </div>

          {/* Action Buttons */}
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
                if (hours && minutes) {
                  handleTimeChange(hours, minutes, period);
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

export default TimePicker;