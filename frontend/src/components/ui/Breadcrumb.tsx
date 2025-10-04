// src/components/ui/Breadcrumb.tsx

import React from 'react';
import { ChevronRight, Home } from 'lucide-react';
import { cn } from '../../utils/cn';

interface BreadcrumbProps {
  children: React.ReactNode;
  separator?: React.ReactNode;
  className?: string;
}

const BreadcrumbRoot: React.FC<BreadcrumbProps> = ({ 
  children, 
  separator,
  className = '' 
}) => {
  const childrenArray = React.Children.toArray(children);
  
  return (
    <nav aria-label="Breadcrumb" className={cn('py-2', className)}>
      <ol className="flex items-center flex-wrap gap-2 text-sm">
        {childrenArray.map((child, index) => (
          <React.Fragment key={index}>
            {child}
            {index < childrenArray.length - 1 && (
              <span className="text-gray-400 mx-1">
                {separator || <ChevronRight className="h-4 w-4" />}
              </span>
            )}
          </React.Fragment>
        ))}
      </ol>
    </nav>
  );
};

interface BreadcrumbItemProps {
  href?: string;
  active?: boolean;
  icon?: React.ReactNode;
  children: React.ReactNode;
  onClick?: () => void;
  className?: string;
}

const BreadcrumbItem: React.FC<BreadcrumbItemProps> = ({ 
  href, 
  active, 
  icon, 
  children,
  onClick,
  className = ''
}) => {
  const content = (
    <>
      {icon && (
        <span className="flex-shrink-0">
          {icon}
        </span>
      )}
      <span>{children}</span>
    </>
  );

  const baseClasses = cn(
    'flex items-center gap-1.5 transition-colors duration-200',
    'hover:text-blue-600 focus:outline-none focus:text-blue-600',
    active && 'text-gray-900 font-semibold pointer-events-none',
    !active && 'text-gray-600',
    className
  );

  return (
    <li className="flex items-center">
      {href && !active ? (
        <a href={href} onClick={onClick} className={baseClasses}>
          {content}
        </a>
      ) : (
        <span className={baseClasses}>
          {content}
        </span>
      )}
    </li>
  );
};

export const Breadcrumb = Object.assign(BreadcrumbRoot, { 
  Item: BreadcrumbItem 
});