import React from 'react';

type PriceDisplayProps = {
  amount: number;
  currency?: string;
  showPerNight?: boolean;
  variant?: 'default' | 'card' | 'minimal' | 'large';
};

const variantClasses: Record<NonNullable<PriceDisplayProps['variant']>, string> = {
  default: 'text-gray-900',
  card: 'text-primary-700 bg-primary-50 px-2 py-1 rounded-md',
  minimal: 'text-gray-800',
  large: 'text-primary-800 text-2xl font-bold',
};

export const PriceDisplay: React.FC<PriceDisplayProps> = ({
  amount,
  currency = 'YER',
  showPerNight = false,
  variant = 'default',
}) => {
  const formatted = new Intl.NumberFormat('ar-YE').format(amount);

  return (
    <span className={variantClasses[variant]}>
      {formatted} {currency}
      {showPerNight ? ' / ليلة' : ''}
    </span>
  );
};