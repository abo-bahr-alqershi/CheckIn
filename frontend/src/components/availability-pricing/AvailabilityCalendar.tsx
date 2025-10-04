// src/components/availability-pricing/availability/AvailabilityCalendar.tsx

import React, { useState, useMemo, useCallback, useEffect } from 'react';
import { format } from 'date-fns';
import { CalendarGrid } from './CalendarGrid';
import { CalendarToolbar } from './CalendarToolbar';
import { AvailabilityLegend } from './AvailabilityLegend';
import { AvailabilityStats } from './AvailabilityStats';
import { AvailabilityQuickActions } from './AvailabilityQuickActions';
import { UpdateAvailabilityModal } from './UpdateAvailabilityModal';
import { BulkOperationsPanel } from './BulkOperationsPanel';
import { useAvailability } from '../../hooks/useAvailability';
import { AvailabilityStatus, UnitAvailabilityDto } from '../../types/availability.types';
import { cn } from '../../utils/cn';
import { Loader } from '../ui/Loader';
import { Alert } from '../ui/Alert';

interface AvailabilityCalendarProps {
  unitId: string;
  unitName: string;
  initialYear?: number;
  initialMonth?: number;
}

export const AvailabilityCalendar: React.FC<AvailabilityCalendarProps> = ({
  unitId,
  unitName,
  initialYear = new Date().getFullYear(),
  initialMonth = new Date().getMonth() + 1
}) => {
  const [year, setYear] = useState(initialYear);
  const [month, setMonth] = useState(initialMonth);
  const [selectedDates, setSelectedDates] = useState<string[]>([]);
  const [showUpdateModal, setShowUpdateModal] = useState(false);
  const [showBulkPanel, setShowBulkPanel] = useState(false);
  const [hoveredDate, setHoveredDate] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<'month' | 'year' | 'list'>('month');

  const {
    data,
    isLoading,
    error,
    updateAvailability,
    bulkUpdateAvailability,
    quickBlock,
    quickAvailable,
    refetch
  } = useAvailability(unitId, year, month);

  // Refetch data when month/year changes
  useEffect(() => {
    refetch();
  }, [year, month, refetch]);

  const handleMonthChange = useCallback((newYear: number, newMonth: number) => {
    setYear(newYear);
    setMonth(newMonth);
    setSelectedDates([]);
  }, []);

  const handleDayClick = useCallback((date: string) => {
    setSelectedDates(prev => {
      if (prev.includes(date)) {
        return prev.filter(d => d !== date);
      }
      return [...prev, date];
    });
  }, []);

  const handleRangeSelect = useCallback((start: string, end: string) => {
    const startDate = new Date(start);
    const endDate = new Date(end);
    const dates: string[] = [];
    
    for (let d = new Date(startDate); d <= endDate; d.setDate(d.getDate() + 1)) {
      dates.push(format(new Date(d), 'yyyy-MM-dd'));
    }
    
    setSelectedDates(dates);
    setShowBulkPanel(true);
  }, []);

  const handleQuickBlock = useCallback(() => {
    if (selectedDates.length > 0) {
      quickBlock(selectedDates);
      setSelectedDates([]);
    }
  }, [selectedDates, quickBlock]);

  const handleQuickAvailable = useCallback(() => {
    if (selectedDates.length > 0) {
      quickAvailable(selectedDates);
      setSelectedDates([]);
    }
  }, [selectedDates, quickAvailable]);

  const renderDay = useCallback((day: any) => {
    const dayData = data?.calendar[day.dateString];
    
    // Ensure status is a string and use the correct values
    const status = typeof dayData?.status === 'string' 
      ? dayData.status 
      : 'Available';
    
    const isSelected = selectedDates.includes(day.dateString);

    const statusColors = {
      'Available': 'bg-green-100 text-green-800 border-green-200',
      'Booked': 'bg-red-100 text-red-800 border-red-200',
      'Blocked': 'bg-gray-200 text-gray-700 border-gray-300',
      'Maintenance': 'bg-yellow-100 text-yellow-800 border-yellow-200',
      'Hold': 'bg-blue-100 text-blue-800 border-blue-200'
    };

    const statusIcons = {
      'Available': '✓',
      'Booked': '●',
      'Blocked': '✕',
      'Maintenance': '🔧',
      'Hold': '⏸'
    };

    return (
      <div className={cn(
        "flex flex-col h-full p-1 rounded transition-all",
        day.isCurrentMonth && statusColors[status],
        isSelected && "ring-2 ring-blue-500",
        day.isPast && "opacity-60"
      )}>
        <div className="flex items-center justify-between">
          <span className={cn(
            "text-sm font-medium",
            day.isToday && "font-bold"
          )}>
            {format(day.date, 'd')}
          </span>
          <span className="text-xs">
            {statusIcons[status]}
          </span>
        </div>
        
        {dayData?.reason && (
          <div className="mt-1 text-xs truncate" title={dayData.reason}>
            {dayData.reason}
          </div>
        )}
        
        {dayData?.bookingId && (
          <div className="mt-auto">
            <span className="text-xs bg-white/50 px-1 rounded">
              #{dayData.bookingId.slice(0, 6)}
            </span>
          </div>
        )}
      </div>
    );
  }, [data, selectedDates]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <Loader size="lg" />
      </div>
    );
  }

  if (error) {
    return (
      <Alert variant="error">
        حدث خطأ في تحميل بيانات الإتاحة
      </Alert>
    );
  }

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <CalendarToolbar
        year={year}
        month={month}
        onMonthChange={handleMonthChange}
        onViewChange={setViewMode}
        currentView={viewMode}
        title={`إتاحة ${unitName}`}
        actions={
          <AvailabilityQuickActions
            selectedCount={selectedDates.length}
            onBlock={handleQuickBlock}
            onAvailable={handleQuickAvailable}
            onUpdate={() => setShowUpdateModal(true)}
            onBulkOperations={() => setShowBulkPanel(true)}
          />
        }
      />

      {/* Stats */}
      {data?.stats && (
        <AvailabilityStats stats={data.stats} />
      )}

      {/* Legend */}
      <AvailabilityLegend />

      {/* Calendar Grid */}
      {viewMode === 'month' && (
        <CalendarGrid
          year={year}
          month={month}
          data={data?.calendar}
          selectedDates={selectedDates}
          onDayClick={handleDayClick}
          onRangeSelect={handleRangeSelect}
          onDayHover={setHoveredDate}
          renderDay={renderDay}
          multiSelect
        />
      )}

      {/* Modals */}
      {showUpdateModal && selectedDates.length > 0 && (
        <UpdateAvailabilityModal
          unitId={unitId}
          year={year}
          month={month}
          selectedDates={selectedDates}
          onClose={() => setShowUpdateModal(false)}
          onSuccess={() => {
            setShowUpdateModal(false);
            setSelectedDates([]);
          }}
        />
      )}

      {showBulkPanel && (
        <BulkOperationsPanel
          unitId={unitId!}
          selectedDates={selectedDates}
          year={year}
          month={month}
          onClose={() => setShowBulkPanel(false)}
          onSuccess={() => {
            setShowBulkPanel(false);
            setSelectedDates([]);
          }}
        />
      )}
    </div>
  );
};