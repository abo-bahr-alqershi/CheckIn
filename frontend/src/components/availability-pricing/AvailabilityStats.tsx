// src/components/availability-pricing/availability/AvailabilityStats.tsx

import React from 'react';
import { AvailabilityStatsDto } from '../../types/availability.types';
import { Calendar, TrendingUp, Clock, Ban } from 'lucide-react';
import { Card } from '../ui/Card';

interface AvailabilityStatsProps {
  stats: AvailabilityStatsDto;
}

export const AvailabilityStats: React.FC<AvailabilityStatsProps> = ({ stats }) => {
  // Ensure all values are numbers
  const totalDays = (() => {
    if (typeof stats.totalDays === 'object' && stats.totalDays !== null && 'amount' in stats.totalDays) {
      return (stats.totalDays as any).amount;
    }
    return Number(stats.totalDays) || 0;
  })();
  
  const availableDays = (() => {
    if (typeof stats.availableDays === 'object' && stats.availableDays !== null && 'amount' in stats.availableDays) {
      return (stats.availableDays as any).amount;
    }
    return Number(stats.availableDays) || 0;
  })();
  
  const bookedDays = (() => {
    if (typeof stats.bookedDays === 'object' && stats.bookedDays !== null && 'amount' in stats.bookedDays) {
      return (stats.bookedDays as any).amount;
    }
    return Number(stats.bookedDays) || 0;
  })();
  
  const occupancyRate = (() => {
    if (typeof stats.occupancyRate === 'object' && stats.occupancyRate !== null && 'amount' in stats.occupancyRate) {
      return (stats.occupancyRate as any).amount;
    }
    return Number(stats.occupancyRate) || 0;
  })();

  const statsCards = [
    {
      label: 'إجمالي الأيام',
      value: totalDays,
      icon: Calendar,
      color: 'blue',
      bgColor: 'bg-blue-50',
      textColor: 'text-blue-600'
    },
    {
      label: 'الأيام المتاحة',
      value: availableDays,
      icon: Clock,
      color: 'green',
      bgColor: 'bg-green-50',
      textColor: 'text-green-600',
      percentage: totalDays > 0 ? ((availableDays / totalDays) * 100).toFixed(1) : '0.0'
    },
    {
      label: 'الأيام المحجوزة',
      value: bookedDays,
      icon: TrendingUp,
      color: 'red',
      bgColor: 'bg-red-50',
      textColor: 'text-red-600',
      percentage: totalDays > 0 ? ((bookedDays / totalDays) * 100).toFixed(1) : '0.0'
    },
    {
      label: 'معدل الإشغال',
      value: `${occupancyRate.toFixed(1)}%`,
      icon: TrendingUp,
      color: 'purple',
      bgColor: 'bg-purple-50',
      textColor: 'text-purple-600'
    }
  ];

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      {statsCards.map((stat, index) => {
        const Icon = stat.icon;
        return (
          <Card key={index} className="p-4">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <p className="text-sm text-gray-600 mb-1">{stat.label}</p>
                <p className="text-2xl font-bold text-gray-900">{stat.value}</p>
                {stat.percentage && (
                  <p className={`text-sm mt-1 ${stat.textColor}`}>
                    {stat.percentage}% من الإجمالي
                  </p>
                )}
              </div>
              <div className={`p-3 rounded-lg ${stat.bgColor}`}>
                <Icon className={`h-5 w-5 ${stat.textColor}`} />
              </div>
            </div>
          </Card>
        );
      })}
    </div>
  );
};