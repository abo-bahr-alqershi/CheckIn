import React from 'react';
import ActionButton from './ActionButton';

interface EmptyStateProps {
  icon?: string;
  title: string;
  description?: string;
  actionLabel?: string;
  onAction?: () => void;
  className?: string;
  size?: 'sm' | 'md' | 'lg';
}

const EmptyState: React.FC<EmptyStateProps> = ({
  icon = '📋',
  title,
  description,
  actionLabel,
  onAction,
  className = '',
  size = 'md'
}) => {
  const getSizeClasses = () => {
    switch (size) {
      case 'sm':
        return {
          container: 'py-8',
          icon: 'text-4xl',
          title: 'text-lg',
          description: 'text-sm'
        };
      case 'lg':
        return {
          container: 'py-16',
          icon: 'text-8xl',
          title: 'text-2xl',
          description: 'text-lg'
        };
      default:
        return {
          container: 'py-12',
          icon: 'text-6xl',
          title: 'text-xl',
          description: 'text-base'
        };
    }
  };

  const sizeClasses = getSizeClasses();

  return (
    <div className={`text-center ${sizeClasses.container} ${className}`}>
      <div className={`${sizeClasses.icon} mb-4`}>{icon}</div>
      <h3 className={`${sizeClasses.title} font-semibold text-gray-900 mb-2`}>
        {title}
      </h3>
      {description && (
        <p className={`${sizeClasses.description} text-gray-600 mb-6 max-w-md mx-auto`}>
          {description}
        </p>
      )}
      {actionLabel && onAction && (
        <ActionButton
          onClick={onAction}
          label={actionLabel}
          variant="primary"
          size={size === 'lg' ? 'lg' : 'md'}
        />
      )}
    </div>
  );
};

export default EmptyState;