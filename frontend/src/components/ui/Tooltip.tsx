// src/components/ui/Tooltip.tsx

import React, { useState, useRef, useEffect } from 'react';
import { cn } from '../../utils/cn';

interface TooltipProps {
  content: React.ReactNode;
  children: React.ReactNode;
  position?: 'top' | 'bottom' | 'left' | 'right';
  delay?: number;
  className?: string;
}

const Tooltip: React.FC<TooltipProps> = ({
  content,
  children,
  position = 'top',
  delay = 200,
  className = ''
}) => {
  const [isVisible, setIsVisible] = useState(false);
  const [coords, setCoords] = useState({ x: 0, y: 0 });
  const triggerRef = useRef<HTMLDivElement>(null);
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const showTooltip = () => {
    timeoutRef.current = setTimeout(() => {
      setIsVisible(true);
      updatePosition();
    }, delay);
  };

  const hideTooltip = () => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
    }
    setIsVisible(false);
  };

  const updatePosition = () => {
    if (!triggerRef.current) return;
    
    const rect = triggerRef.current.getBoundingClientRect();
    const positions = {
      top: { x: rect.left + rect.width / 2, y: rect.top },
      bottom: { x: rect.left + rect.width / 2, y: rect.bottom },
      left: { x: rect.left, y: rect.top + rect.height / 2 },
      right: { x: rect.right, y: rect.top + rect.height / 2 }
    };
    
    setCoords(positions[position]);
  };

  useEffect(() => {
    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, []);

  const positionClasses = {
    top: 'bottom-full left-1/2 -translate-x-1/2 mb-2',
    bottom: 'top-full left-1/2 -translate-x-1/2 mt-2',
    left: 'right-full top-1/2 -translate-y-1/2 mr-2',
    right: 'left-full top-1/2 -translate-y-1/2 ml-2'
  };

  return (
    <>
      <div
        ref={triggerRef}
        onMouseEnter={showTooltip}
        onMouseLeave={hideTooltip}
        className="inline-block"
      >
        {children}
      </div>
      
      {isVisible && (
        <div
          className={cn(
            'fixed z-50 px-2 py-1 text-xs text-white bg-gray-900 rounded shadow-lg',
            'animate-in fade-in zoom-in-95 duration-100',
            'pointer-events-none',
            positionClasses[position],
            className
          )}
          style={{
            left: position === 'left' || position === 'right' ? coords.x : undefined,
            top: position === 'top' || position === 'bottom' ? coords.y : undefined
          }}
        >
          {content}
        </div>
      )}
    </>
  );
};

export default Tooltip;
export { Tooltip };