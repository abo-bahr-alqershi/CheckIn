export type ViewType = 'table' | 'cards' | 'map';

interface ViewOption {
  type: ViewType;
  label: string;
  icon: string;
}

interface ViewToggleProps {
  currentView: ViewType;
  onViewChange: (view: ViewType) => void;
  availableViews?: ViewType[];
  className?: string;
}

const ViewToggle = ({
  currentView,
  onViewChange,
  availableViews = ['table', 'cards', 'map'],
  className = '',
}: ViewToggleProps) => {
  const viewOptions: ViewOption[] = [
    {
      type: 'table',
      label: 'جدول',
      icon: '📋',
    },
    {
      type: 'cards',
      label: 'كروت',
      icon: '🎴',
    },
    {
      type: 'map',
      label: 'خريطة',
      icon: '🗺️',
    },
  ];

  const filteredOptions = viewOptions.filter(option => 
    availableViews.includes(option.type)
  );

  return (
    <div className={`inline-flex rounded-lg border border-gray-300 bg-white ${className}`}>
      {filteredOptions.map((option, index) => (
        <button
          key={option.type}
          onClick={() => onViewChange(option.type)}
          className={`px-4 py-2 text-sm font-medium transition-colors ${
            currentView === option.type
              ? 'bg-blue-50 text-blue-700 border-blue-200'
              : 'text-gray-700 hover:text-gray-900 hover:bg-gray-50'
          } ${
            index === 0 
              ? 'rounded-r-md border-l-0' 
              : index === filteredOptions.length - 1 
                ? 'rounded-l-md border-r-0' 
                : 'border-l-0 border-r-0'
          }`}
          title={option.label}
        >
          <div className="flex items-center space-x-2 space-x-reverse">
            <span>{option.icon}</span>
            <span>{option.label}</span>
          </div>
        </button>
      ))}
    </div>
  );
};

export default ViewToggle;