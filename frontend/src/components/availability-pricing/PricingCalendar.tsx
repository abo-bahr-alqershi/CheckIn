// src/components/availability-pricing/pricing/PricingCalendar.tsx

import React, { useState, useMemo, useCallback, useEffect } from 'react';
import { format } from 'date-fns';
import { CalendarGrid } from './CalendarGrid';
import { CalendarToolbar } from './CalendarToolbar';
import { PricingLegend } from './PricingLegend';
import { PricingStats } from './PricingStats';
import { PricingQuickActions } from './PricingQuickActions';
import { UpdatePricingModal } from './UpdatePricingModal';
import { SeasonalPricingModal } from './SeasonalPricingModal';
import { PricingBulkOperations } from './PricingBulkOperations';
import { usePricing } from '../../hooks/usePricing';
import { cn } from '../../utils/cn';
import { Loader } from '../ui/Loader';
import { Alert } from '../ui/Alert';
import { Banknote } from 'lucide-react';

interface PricingCalendarProps {
  unitId: string;
  unitName: string;
  initialYear?: number;
  initialMonth?: number;
}

export const PricingCalendar: React.FC<PricingCalendarProps> = ({
  unitId,
  unitName,
  initialYear = new Date().getFullYear(),
  initialMonth = new Date().getMonth() + 1
}) => {
  const [year, setYear] = useState(initialYear);
  const [month, setMonth] = useState(initialMonth);
  const [selectedDates, setSelectedDates] = useState<string[]>([]);
  const [showUpdateModal, setShowUpdateModal] = useState(false);
  const [showSeasonalModal, setShowSeasonalModal] = useState(false);
  const [showBulkOperations, setShowBulkOperations] = useState(false);
  const [viewMode, setViewMode] = useState<'month' | 'year' | 'list'>('month');

  const {
    data,
    isLoading,
    error,
    updatePricing,
    applyPercentageChange,
    applyWeekendPricing,
    refetch
  } = usePricing(unitId, year, month);

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
    setShowBulkOperations(true);
  }, []);

  const handleQuickIncrease = useCallback(() => {
    if (selectedDates.length > 0) {
      applyPercentageChange(selectedDates, 10);
      setSelectedDates([]);
    }
  }, [selectedDates, applyPercentageChange]);

  const handleQuickDecrease = useCallback(() => {
    if (selectedDates.length > 0) {
      applyPercentageChange(selectedDates, -10);
      setSelectedDates([]);
    }
  }, [selectedDates, applyPercentageChange]);

  const renderDay = useCallback((day: any) => {
    const dayData = data?.calendar[day.dateString];
    
    // Handle both number and MoneyDto object for price
    const getPriceValue = (priceObj: any) => {
      if (typeof priceObj === 'object' && priceObj !== null && 'amount' in priceObj) {
        return (priceObj as any).amount;
      }
      return typeof priceObj === 'number' ? priceObj : 0;
    };
    
    const price = getPriceValue(dayData?.price) || getPriceValue(data?.basePrice) || 0;
    const priceType = dayData?.priceType || 'Base';
    const percentageChange = dayData?.percentageChange;
    const currencyCode = data?.currency || 'YER';
    const isSelected = selectedDates.includes(day.dateString);

    const priceTypeColors = {
      'Base': 'bg-gray-100',
      'Custom': 'bg-blue-100',
      'Weekend': 'bg-purple-100',
      'Holiday': 'bg-red-100',
      'Seasonal': 'bg-green-100',
      'Special': 'bg-yellow-100'
    } as Record<string, string>;

    const formatPrice = (price: number) => {
      let numeric = price;
      let numStr: string;
      if (numeric >= 1000000) {
        numStr = `${(numeric / 1000000).toFixed(1)}M`;
      } else if (numeric >= 1000) {
        numStr = `${(numeric / 1000).toFixed(0)}K`;
      } else {
        numStr = numeric.toString();
      }
      return `${numStr} ${currencyCode}`;
    };

    return (
      <div className={cn(
        "flex flex-col h-full p-1 rounded transition-all",
        day.isCurrentMonth && priceTypeColors[priceType],
        isSelected && "ring-2 ring-blue-500",
        day.isPast && "opacity-60"
      )}>
        <div className="flex items-center justify-between mb-1">
          <span className={cn(
            "text-xs font-medium",
            day.isToday && "text-blue-600"
          )}>
            {format(day.date, 'd')}
          </span>
          {percentageChange !== null && percentageChange !== undefined && (
            <span className={cn(
              "text-xs font-medium",
              percentageChange > 0 ? "text-green-600" : "text-red-600"
            )}>
              {percentageChange > 0 ? '+' : ''}{Math.round(Number(percentageChange))}%
            </span>
          )}
        </div>
        
        <div className="flex items-center gap-1 mt-auto">
          <Banknote className="h-3 w-3 text-gray-500" />
          <span className="text-sm font-semibold text-gray-900">
            {formatPrice(price)}
          </span>
        </div>
        
        {priceType !== 'Base' && (
          <div className="mt-1">
            <span className="text-xs text-gray-600">
              {priceType}
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
        حدث خطأ في تحميل بيانات التسعير
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
        title={`تسعير ${unitName}`}
        onSettings={() => setShowSeasonalModal(true)}
        actions={
          <PricingQuickActions
            selectedCount={selectedDates.length}
            onIncrease={handleQuickIncrease}
            onDecrease={handleQuickDecrease}
            onUpdate={() => setShowUpdateModal(true)}
            onBulkOperations={() => setShowBulkOperations(true)}
          />
        }
      />

      {/* Stats */}
      {data?.stats && (
        <PricingStats stats={data.stats} currency={data.currency} />
      )}

      {/* Legend */}
      <PricingLegend />

      {/* Calendar Grid */}
      {viewMode === 'month' && (
        <CalendarGrid
          year={year}
          month={month}
          data={data?.calendar}
          selectedDates={selectedDates}
          onDayClick={handleDayClick}
          onRangeSelect={handleRangeSelect}
          renderDay={renderDay}
          multiSelect
        />
      )}

      {/* Modals */}
      {showUpdateModal && selectedDates.length > 0 && (
        <UpdatePricingModal
          unitId={unitId}
          selectedDates={selectedDates}
          basePrice={data?.basePrice || 0}
          currency={data?.currency || 'YER'}
          year={year}
          month={month}
          onClose={() => setShowUpdateModal(false)}
          onSuccess={() => {
            setShowUpdateModal(false);
            setSelectedDates([]);
          }}
        />
      )}

      {showSeasonalModal && (
        <SeasonalPricingModal
          unitId={unitId}
          onClose={() => setShowSeasonalModal(false)}
          onSuccess={() => setShowSeasonalModal(false)}
        />
      )}

      {showBulkOperations && (
        <PricingBulkOperations
          unitId={unitId}
          selectedDates={selectedDates}
          basePrice={data?.basePrice || 0}
          currency={data?.currency || 'YER'}
          year={year}
          month={month}
          onClose={() => setShowBulkOperations(false)}
          onSuccess={() => {
            setShowBulkOperations(false);
            setSelectedDates([]);
          }}
        />
      )}
    </div>
  );
};