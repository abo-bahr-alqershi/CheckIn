// src/components/availability-pricing/pricing/PricingLegend.tsx

import React from 'react';

export const PricingLegend: React.FC = () => {
  const legends = [
    {
      type: 'Base',
      label: 'سعر أساسي',
      color: 'bg-gray-100 border-gray-200'
    },
    {
      type: 'Custom',
      label: 'سعر مخصص',
      color: 'bg-blue-100 border-blue-200'
    },
    {
      type: 'Weekend',
      label: 'نهاية الأسبوع',
      color: 'bg-purple-100 border-purple-200'
    },
    {
      type: 'Holiday',
      label: 'عطلة رسمية',
      color: 'bg-red-100 border-red-200'
    },
    {
      type: 'Seasonal',
      label: 'موسمي',
      color: 'bg-green-100 border-green-200'
    },
    {
      type: 'Special',
      label: 'عرض خاص',
      color: 'bg-yellow-100 border-yellow-200'
    }
  ];

  return (
    <div className="bg-white rounded-lg border border-gray-200 p-3">
      <div className="flex items-center gap-6 flex-wrap">
        <span className="text-sm font-medium text-gray-700">أنواع التسعير:</span>
        {legends.map((legend) => (
          <div key={legend.type} className="flex items-center gap-2">
            <div className={`w-6 h-6 rounded border ${legend.color}`} />
            <span className="text-sm text-gray-600">{legend.label}</span>
          </div>
        ))}
      </div>
    </div>
  );
};