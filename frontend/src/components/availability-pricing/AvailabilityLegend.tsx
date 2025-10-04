// src/components/availability-pricing/availability/AvailabilityLegend.tsx

import React from 'react';
import { AvailabilityStatus } from '../../types/availability.types';

export const AvailabilityLegend: React.FC = () => {
  const legends = [
    {
      status: AvailabilityStatus.Available,
      label: 'متاح',
      color: 'bg-green-100 border-green-200',
      icon: '✓'
    },
    {
      status: AvailabilityStatus.Booked,
      label: 'محجوز',
      color: 'bg-red-100 border-red-200',
      icon: '●'
    },
    {
      status: AvailabilityStatus.Blocked,
      label: 'محظور',
      color: 'bg-gray-200 border-gray-300',
      icon: '✕'
    },
    {
      status: AvailabilityStatus.Maintenance,
      label: 'صيانة',
      color: 'bg-yellow-100 border-yellow-200',
      icon: '🔧'
    },
    {
      status: AvailabilityStatus.Hold,
      label: 'معلق',
      color: 'bg-blue-100 border-blue-200',
      icon: '⏸'
    }
  ];

  return (
    <div className="bg-white rounded-lg border border-gray-200 p-3">
      <div className="flex items-center gap-6 flex-wrap">
        <span className="text-sm font-medium text-gray-700">دليل الألوان:</span>
        {legends.map((legend) => (
          <div key={legend.status} className="flex items-center gap-2">
            <div className={`w-8 h-8 rounded border ${legend.color} flex items-center justify-center`}>
              <span className="text-xs">{legend.icon}</span>
            </div>
            <span className="text-sm text-gray-600">{legend.label}</span>
          </div>
        ))}
      </div>
    </div>
  );
};