// src/components/ui/ButtonGroup.tsx

import React from 'react';
import { cn } from '../../utils/cn';

interface ButtonGroupProps extends React.HTMLAttributes<HTMLDivElement> {
  variant?: 'default' | 'pills' | 'tabs';
  size?: 'sm' | 'md' | 'lg';
  fullWidth?: boolean;
}

export const ButtonGroup: React.FC<ButtonGroupProps> = ({ 
  variant = 'default',
  size = 'md',
  fullWidth = false,
  className = '', 
  children, 
  ...props 
}) => {
  const variants = {
    default: 'gap-0.5',
    pills: 'gap-2',
    tabs: 'gap-0 border-b border-gray-200'
  };

  return (
    <div 
      className={cn(
        'inline-flex items-center',
        variants[variant],
        fullWidth && 'w-full',
        variant === 'default' && 'bg-gray-100 p-0.5 rounded-lg',
        className
      )} 
      {...props}
    >
      {React.Children.map(children, (child, index) => {
        if (React.isValidElement(child)) {
          return React.cloneElement(child as React.ReactElement<any>, {
            className: cn(
              (child as React.ReactElement<any>).props.className,
              variant === 'default' && index === 0 && 'rounded-l-md',
              variant === 'default' && index === React.Children.count(children) - 1 && 'rounded-r-md',
              variant === 'default' && 'rounded-none',
              fullWidth && 'flex-1'
            )
          });
        }
        return child;
      })}
    </div>
  );
};