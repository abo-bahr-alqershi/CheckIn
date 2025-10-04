// src/components/ui/Slider.tsx

import React, { useState } from 'react';
import { cn } from '../../utils/cn';

interface SliderProps {
  value: [number];
  onValueChange: (value: [number]) => void;
  min?: number;
  max?: number;
  step?: number;
  disabled?: boolean;
  showLabels?: boolean;
  showValue?: boolean;
  formatValue?: (value: number) => string;
  marks?: { value: number; label?: string }[];
  className?: string;
}

export const Slider: React.FC<SliderProps> = ({ 
  value, 
  onValueChange, 
  min = 0, 
  max = 100, 
  step = 1,
  disabled = false,
  showLabels = false,
  showValue = false,
  formatValue = (v) => v.toString(),
  marks,
  className = '' 
}) => {
  const [isDragging, setIsDragging] = useState(false);
  const percentage = ((value[0] - min) / (max - min)) * 100;

  return (
    <div className={cn('relative', className)}>
      {showValue && (
        <div 
          className="absolute -top-8 transform -translate-x-1/2 bg-gray-900 text-white text-xs px-2 py-1 rounded"
          style={{ left: `${percentage}%` }}
        >
          {formatValue(value[0])}
        </div>
      )}
      
      <div className="relative">
        <input
          type="range"
          min={min}
          max={max}
          step={step}
          value={value[0]}
          onChange={(e) => onValueChange([Number(e.target.value)])}
          onMouseDown={() => setIsDragging(true)}
          onMouseUp={() => setIsDragging(false)}
          disabled={disabled}
          className={cn(
            'w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer',
            'focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2',
            '[&::-webkit-slider-thumb]:appearance-none',
            '[&::-webkit-slider-thumb]:w-5 [&::-webkit-slider-thumb]:h-5',
            '[&::-webkit-slider-thumb]:bg-blue-600 [&::-webkit-slider-thumb]:rounded-full',
            '[&::-webkit-slider-thumb]:cursor-pointer [&::-webkit-slider-thumb]:transition-all',
            isDragging && '[&::-webkit-slider-thumb]:scale-125 [&::-webkit-slider-thumb]:shadow-lg',
            disabled && 'opacity-50 cursor-not-allowed'
          )}
        />
        
        {/* Progress bar */}
        <div 
          className="absolute top-0 h-2 bg-blue-600 rounded-lg pointer-events-none"
          style={{ width: `${percentage}%` }}
        />
      </div>
      
      {/* Labels */}
      {showLabels && (
        <div className="flex justify-between mt-1">
          <span className="text-xs text-gray-500">{min}</span>
          <span className="text-xs text-gray-500">{max}</span>
        </div>
      )}
      
      {/* Marks */}
      {marks && (
        <div className="relative mt-2">
          {marks.map((mark) => {
            const markPercentage = ((mark.value - min) / (max - min)) * 100;
            return (
              <div
                key={mark.value}
                className="absolute transform -translate-x-1/2"
                style={{ left: `${markPercentage}%` }}
              >
                <div className="w-0.5 h-2 bg-gray-300" />
                {mark.label && (
                  <span className="text-xs text-gray-500 mt-1 block">
                    {mark.label}
                  </span>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};