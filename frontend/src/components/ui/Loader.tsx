// src/components/ui/Loader.tsx

import React from 'react';
import { cn } from '../../utils/cn';

interface LoaderProps {
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl';
  variant?: 'spinner' | 'dots' | 'pulse' | 'ring';
  color?: 'primary' | 'secondary' | 'white' | 'dark';
  text?: string;
  fullScreen?: boolean;
  className?: string;
}

export const Loader: React.FC<LoaderProps> = ({ 
  size = 'md', 
  variant = 'spinner',
  color = 'primary',
  text,
  fullScreen = false,
  className = ''
}) => {
  const sizes = {
    xs: 'h-3 w-3',
    sm: 'h-4 w-4',
    md: 'h-6 w-6',
    lg: 'h-8 w-8',
    xl: 'h-12 w-12'
  };

  const colors = {
    primary: 'border-blue-600',
    secondary: 'border-gray-600',
    white: 'border-white',
    dark: 'border-gray-900'
  };

  const spinnerElement = variant === 'spinner' && (
    <div
      className={cn(
        'animate-spin rounded-full border-2 border-t-transparent',
        sizes[size],
        colors[color]
      )}
      aria-label="Loading"
    />
  );

  const dotsElement = variant === 'dots' && (
    <div className="flex gap-1">
      {[0, 1, 2].map((i) => (
        <div
          key={i}
          className={cn(
            'rounded-full bg-current animate-pulse',
            size === 'xs' && 'h-1 w-1',
            size === 'sm' && 'h-1.5 w-1.5',
            size === 'md' && 'h-2 w-2',
            size === 'lg' && 'h-3 w-3',
            size === 'xl' && 'h-4 w-4',
            color === 'primary' && 'text-blue-600',
            color === 'secondary' && 'text-gray-600',
            color === 'white' && 'text-white',
            color === 'dark' && 'text-gray-900'
          )}
          style={{ animationDelay: `${i * 150}ms` }}
        />
      ))}
    </div>
  );

  const pulseElement = variant === 'pulse' && (
    <div className={cn('space-y-2', sizes[size])}>
      <div className="bg-gray-300 rounded h-2 animate-pulse" />
      <div className="bg-gray-300 rounded h-2 animate-pulse" style={{ width: '75%' }} />
    </div>
  );

  const ringElement = variant === 'ring' && (
    <div className={cn('relative', sizes[size])}>
      <div className={cn(
        'absolute inset-0 rounded-full border-2 opacity-25',
        colors[color]
      )} />
      <div className={cn(
        'absolute inset-0 rounded-full border-2 border-t-transparent animate-spin',
        colors[color]
      )} />
    </div>
  );

  const content = (
    <div className={cn('flex flex-col items-center justify-center gap-3', className)}>
      {spinnerElement}
      {dotsElement}
      {pulseElement}
      {ringElement}
      {text && (
        <p className={cn(
          'text-sm font-medium',
          color === 'primary' && 'text-blue-600',
          color === 'secondary' && 'text-gray-600',
          color === 'white' && 'text-white',
          color === 'dark' && 'text-gray-900'
        )}>
          {text}
        </p>
      )}
    </div>
  );

  if (fullScreen) {
    return (
      <div className="fixed inset-0 bg-white/80 backdrop-blur-sm flex items-center justify-center z-50">
        {content}
      </div>
    );
  }

  return content;
};