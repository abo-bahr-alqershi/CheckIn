// src/components/ui/Alert.tsx

import React from 'react';
import { 
  CheckCircle, 
  XCircle, 
  AlertCircle, 
  Info,
  X 
} from 'lucide-react';
import { cn } from '../../utils/cn';

interface AlertProps {
  variant?: 'info' | 'success' | 'warning' | 'error';
  title?: string;
  description?: string;
  children?: React.ReactNode;
  icon?: React.ReactNode;
  closable?: boolean;
  onClose?: () => void;
  className?: string;
  action?: React.ReactNode;
}

export const Alert: React.FC<AlertProps> = ({ 
  variant = 'info', 
  title,
  description,
  children,
  icon,
  closable = false,
  onClose,
  className = '',
  action
}) => {
  const variants = {
    info: {
      container: 'bg-blue-50 border-blue-200',
      icon: 'text-blue-600',
      title: 'text-blue-900',
      description: 'text-blue-800',
      defaultIcon: <Info className="h-5 w-5" />
    },
    success: {
      container: 'bg-green-50 border-green-200',
      icon: 'text-green-600',
      title: 'text-green-900',
      description: 'text-green-800',
      defaultIcon: <CheckCircle className="h-5 w-5" />
    },
    warning: {
      container: 'bg-amber-50 border-amber-200',
      icon: 'text-amber-600',
      title: 'text-amber-900',
      description: 'text-amber-800',
      defaultIcon: <AlertCircle className="h-5 w-5" />
    },
    error: {
      container: 'bg-red-50 border-red-200',
      icon: 'text-red-600',
      title: 'text-red-900',
      description: 'text-red-800',
      defaultIcon: <XCircle className="h-5 w-5" />
    }
  };

  const styles = variants[variant];

  return (
    <div 
      className={cn(
        'relative rounded-lg border px-4 py-3 transition-all duration-200',
        'animate-in fade-in slide-in-from-top-1',
        styles.container,
        className
      )}
      role="alert"
    >
      <div className="flex gap-3">
        {(icon || styles.defaultIcon) && (
          <div className={cn('flex-shrink-0 mt-0.5', styles.icon)}>
            {icon || styles.defaultIcon}
          </div>
        )}
        
        <div className="flex-1">
          {title && (
            <h3 className={cn('font-semibold text-sm mb-1', styles.title)}>
              {title}
            </h3>
          )}
          {description && (
            <p className={cn('text-sm', styles.description)}>
              {description}
            </p>
          )}
          {children && (
            <div className={cn('text-sm', styles.description)}>
              {children}
            </div>
          )}
          {action && (
            <div className="mt-3">
              {action}
            </div>
          )}
        </div>

        {closable && (
          <button
            onClick={onClose}
            className={cn(
              'flex-shrink-0 ml-auto -mr-1 -mt-1 p-1 rounded-md transition-colors',
              'hover:bg-black/5 focus:outline-none focus:ring-2 focus:ring-offset-2',
              variant === 'info' && 'focus:ring-blue-500',
              variant === 'success' && 'focus:ring-green-500',
              variant === 'warning' && 'focus:ring-amber-500',
              variant === 'error' && 'focus:ring-red-500'
            )}
          >
            <X className={cn('h-4 w-4', styles.icon)} />
          </button>
        )}
      </div>
    </div>
  );
};