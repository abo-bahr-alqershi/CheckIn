// src/components/availability-pricing/pricing/PricingStats.tsx

import React from 'react';
import { PricingStatsDto } from '../../types/pricing.types';
import { TrendingUp, TrendingDown, DollarSign, BarChart } from 'lucide-react';
import { Card } from '../ui/Card';

interface PricingStatsProps {
  stats: PricingStatsDto;
  currency: string;
}

export const PricingStats: React.FC<PricingStatsProps> = ({ stats, currency }) => {
  const formatCurrency = (amount: any) => {
    // Handle both number and MoneyDto object
    let numericAmount: number;
    let currencyCode: string;
    
    if (typeof amount === 'object' && amount !== null && 'amount' in amount) {
      numericAmount = (amount as any).amount;
      currencyCode = (amount as any).currency || currency;
    } else {
      numericAmount = typeof amount === 'number' ? amount : 0;
      currencyCode = currency;
    }
    
    const formatted = new Intl.NumberFormat('ar-YE', {
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(numericAmount);
    return `${formatted} ${currencyCode}`;
  };

  const statsCards = [
    {
      label: 'متوسط السعر',
      value: formatCurrency(stats.averagePrice),
      icon: DollarSign,
      color: 'blue',
      bgColor: 'bg-blue-50',
      textColor: 'text-blue-600'
    },
    {
      label: 'أعلى سعر',
      value: formatCurrency(stats.maxPrice),
      icon: TrendingUp,
      color: 'green',
      bgColor: 'bg-green-50',
      textColor: 'text-green-600',
      change: stats.maxPrice > stats.averagePrice 
        ? `+${Math.round(((stats.maxPrice - stats.averagePrice) / stats.averagePrice * 100))}%`
        : null
    },
    {
      label: 'أقل سعر',
      value: formatCurrency(stats.minPrice),
      icon: TrendingDown,
      color: 'red',
      bgColor: 'bg-red-50',
      textColor: 'text-red-600',
      change: stats.minPrice < stats.averagePrice
        ? `-${Math.round(((stats.averagePrice - stats.minPrice) / stats.averagePrice * 100))}%`
        : null
    },
    {
      label: 'الإيرادات المحتملة',
      value: formatCurrency(stats.potentialRevenue),
      icon: BarChart,
      color: 'purple',
      bgColor: 'bg-purple-50',
      textColor: 'text-purple-600',
      subtitle: `${stats.daysWithSpecialPricing} يوم بتسعير خاص`
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
                <p className="text-xl font-bold text-gray-900">{stat.value}</p>
                {stat.change && (
                  <p className={`text-sm mt-1 ${stat.textColor}`}>
                    {stat.change} من المتوسط
                  </p>
                )}
                {stat.subtitle && (
                  <p className="text-xs text-gray-500 mt-1">
                    {stat.subtitle}
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