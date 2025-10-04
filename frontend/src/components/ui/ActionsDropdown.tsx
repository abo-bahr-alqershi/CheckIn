import React, { useState, useRef, useEffect } from 'react';

interface Action {
  label: string;
  icon: string;
  onClick: () => void;
  variant?: 'default' | 'danger';
  disabled?: boolean;
}

interface ActionsDropdownProps {
  actions: Action[];
  size?: 'sm' | 'md';
}

export const ActionsDropdown: React.FC<ActionsDropdownProps> = ({ 
  actions, 
  size = 'md' 
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  const handleActionClick = (action: Action) => {
    if (!action.disabled) {
      action.onClick();
      setIsOpen(false);
    }
  };

  const buttonSizeClasses = size === 'sm' 
    ? 'p-1.5 text-xs' 
    : 'p-2 text-sm';

  const menuSizeClasses = size === 'sm' 
    ? 'text-xs py-1' 
    : 'text-sm py-2';

  return (
    <div className="relative inline-block" ref={dropdownRef}>
      {/* Trigger Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className={`${buttonSizeClasses} text-gray-500 hover:text-gray-700 hover:bg-gray-100 rounded-md transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-1`}
        aria-label="إجراءات"
      >
        <svg 
          className={`${size === 'sm' ? 'w-4 h-4' : 'w-5 h-5'}`} 
          fill="currentColor" 
          viewBox="0 0 20 20"
        >
          <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z" />
        </svg>
      </button>

      {/* Dropdown Menu */}
      {isOpen && (
        <div className={`absolute left-0 mt-1 ${size === 'sm' ? 'w-32' : 'w-40'} bg-white rounded-md shadow-lg border border-gray-200 z-50`}>
          <div className={`${menuSizeClasses}`}>
            {actions.map((action, index) => (
              <button
                key={index}
                onClick={() => handleActionClick(action)}
                disabled={action.disabled}
                className={`
                  w-full text-right px-3 py-2 flex items-center gap-2 transition-colors
                  ${action.disabled 
                    ? 'text-gray-400 cursor-not-allowed' 
                    : action.variant === 'danger'
                      ? 'text-red-600 hover:bg-red-50 hover:text-red-700'
                      : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                  }
                  ${index !== actions.length - 1 ? 'border-b border-gray-100' : ''}
                `}
              >
                <span className="text-base">{action.icon}</span>
                <span className="flex-1">{action.label}</span>
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default ActionsDropdown;