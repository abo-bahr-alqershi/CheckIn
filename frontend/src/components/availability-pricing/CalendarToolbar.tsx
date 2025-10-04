// src/components/availability-pricing/shared/CalendarToolbar.tsx

import React from 'react';
import { ChevronLeft, ChevronRight, Calendar, Download, Upload, Copy, Settings } from 'lucide-react';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';
import { Button } from '../ui/Button';
import { Select } from '../ui/Select';
import { ButtonGroup } from '../ui/ButtonGroup';

interface CalendarToolbarProps {
  year: number;
  month: number;
  onMonthChange: (year: number, month: number) => void;
  onViewChange?: (view: 'month' | 'year' | 'list') => void;
  currentView?: 'month' | 'year' | 'list';
  onExport?: () => void;
  onImport?: () => void;
  onSettings?: () => void;
  onClone?: () => void;
  title?: string;
  actions?: React.ReactNode;
}

export const CalendarToolbar: React.FC<CalendarToolbarProps> = ({
  year,
  month,
  onMonthChange,
  onViewChange,
  currentView = 'month',
  onExport,
  onImport,
  onSettings,
  onClone,
  title,
  actions
}) => {
  const handlePreviousMonth = () => {
    if (month === 1) {
      onMonthChange(year - 1, 12);
    } else {
      onMonthChange(year, month - 1);
    }
  };

  const handleNextMonth = () => {
    if (month === 12) {
      onMonthChange(year + 1, 1);
    } else {
      onMonthChange(year, month + 1);
    }
  };

  const handleToday = () => {
    const today = new Date();
    onMonthChange(today.getFullYear(), today.getMonth() + 1);
  };

  const months = [
    'يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو',
    'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'
  ];

  const years = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - 2 + i);

  return (
    <div className="bg-white border-b border-gray-200 px-4 py-3">
      <div className="flex items-center justify-between flex-wrap gap-3">
        {/* Title and Navigation */}
        <div className="flex items-center gap-4">
          {title && (
            <h2 className="text-lg font-semibold text-gray-900">{title}</h2>
          )}
          
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handlePreviousMonth}
              icon={<ChevronRight className="h-4 w-4" />}
            />
            
            <div className="flex items-center gap-2">
              <Select
                value={month.toString()}
                onChange={(value) => onMonthChange(year, parseInt(value))}
                className="w-28"
              >
                {months.map((monthName, index) => (
                  <option key={index + 1} value={index + 1}>
                    {monthName}
                  </option>
                ))}
              </Select>
              
              <Select
                value={year.toString()}
                onChange={(value) => onMonthChange(parseInt(value), month)}
                className="w-20"
              >
                {years.map(y => (
                  <option key={y} value={y}>{y}</option>
                ))}
              </Select>
            </div>
            
            <Button
              variant="outline"
              size="sm"
              onClick={handleNextMonth}
              icon={<ChevronLeft className="h-4 w-4" />}
            />
            
            <Button
              variant="outline"
              size="sm"
              onClick={handleToday}
            >
              اليوم
            </Button>
          </div>
        </div>

        {/* View Toggle and Actions */}
        <div className="flex items-center gap-3">
          {onViewChange && (
            <ButtonGroup>
              <Button
                variant={currentView === 'month' ? 'primary' : 'outline'}
                size="sm"
                onClick={() => onViewChange('month')}
                icon={<Calendar className="h-4 w-4" />}
              >
                شهري
              </Button>
              <Button
                variant={currentView === 'year' ? 'primary' : 'outline'}
                size="sm"
                onClick={() => onViewChange('year')}
              >
                سنوي
              </Button>
              <Button
                variant={currentView === 'list' ? 'primary' : 'outline'}
                size="sm"
                onClick={() => onViewChange('list')}
              >
                قائمة
              </Button>
            </ButtonGroup>
          )}

          {(onClone || onImport || onExport || onSettings) && (
            <div className="flex items-center gap-2 border-r pr-3">
              {onClone && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onClone}
                  icon={<Copy className="h-4 w-4" />}
                >
                  نسخ
                </Button>
              )}
              {onImport && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onImport}
                  icon={<Upload className="h-4 w-4" />}
                >
                  استيراد
                </Button>
              )}
              {onExport && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onExport}
                  icon={<Download className="h-4 w-4" />}
                >
                  تصدير
                </Button>
              )}
              {onSettings && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onSettings}
                  icon={<Settings className="h-4 w-4" />}
                />
              )}
            </div>
          )}

          {actions}
        </div>
      </div>
    </div>
  );
};