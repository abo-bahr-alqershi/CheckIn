import React from 'react';

interface LoadingSpinnerProps {
  size?: 'sm' | 'md' | 'lg' | 'xl';
  text?: string;
  center?: boolean;
  className?: string;
  /** optional spinner color (e.g. 'light' | 'dark') */
  color?: string;
}

const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
  size = 'md',
  text = '',
  center = false,
  className = ''
}) => {
  const sizeClasses = {
    sm: 'w-4 h-4',
    md: 'w-6 h-6',
    lg: 'w-8 h-8',
    xl: 'w-12 h-12'
  };

  const textSizeClasses = {
    sm: 'text-xs',
    md: 'text-sm',
    lg: 'text-base',
    xl: 'text-lg'
  };

  const containerClass = center 
    ? 'flex flex-col items-center justify-center min-h-[200px]'
    : 'flex items-center space-x-2 space-x-reverse';

  return (
    <div className={`${containerClass} ${className}`}>
      <div className={`animate-spin border-2 border-gray-300 border-t-blue-600 rounded-full ${sizeClasses[size]}`}></div>
      {text && (
        <span className={`text-gray-600 ${textSizeClasses[size]} ${center ? 'mt-2' : ''}`}>
          {text}
        </span>
      )}
    </div>
  );
};

export default LoadingSpinner;