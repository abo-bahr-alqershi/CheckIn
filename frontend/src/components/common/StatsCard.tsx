interface StatsCardProps {
  title: string;
  value: number;
  icon: string;
  color: 'blue' | 'green' | 'purple' | 'yellow' | 'indigo' | 'red';
  change?: string;
  changeType?: 'increase' | 'decrease';
  prefix?: string;
  suffix?: string;
}

const StatsCard = ({
  title,
  value,
  icon,
  color,
  change,
  changeType,
  prefix,
  suffix,
}: StatsCardProps) => {
  const getColorClasses = (color: string) => {
    const colors = {
      blue: 'bg-blue-50 text-blue-600 border-blue-200',
      green: 'bg-green-50 text-green-600 border-green-200',
      purple: 'bg-purple-50 text-purple-600 border-purple-200',
      yellow: 'bg-yellow-50 text-yellow-600 border-yellow-200',
      indigo: 'bg-indigo-50 text-indigo-600 border-indigo-200',
      red: 'bg-red-50 text-red-600 border-red-200',
    };
    return colors[color as keyof typeof colors];
  };

  const formatNumber = (num: number) => {
    return new Intl.NumberFormat('ar-SA').format(num);
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <div className="flex items-center justify-between">
        <div className="flex-1">
          <p className="text-sm font-medium text-gray-600 mb-1">{title}</p>
          <p className="text-2xl font-bold text-gray-900">
            {prefix && <span className="text-sm font-normal ml-1">{prefix}</span>}
            {formatNumber(value)}
            {suffix && <span className="text-sm font-normal mr-1">{suffix}</span>}
          </p>
          {change && (
            <p className={`text-sm mt-1 ${
              changeType === 'increase' ? 'text-green-600' : 'text-red-600'
            }`}>
              <span className="inline-flex items-center">
                {changeType === 'increase' ? '↗️' : '↘️'}
                <span className="mr-1">{change}</span>
              </span>
            </p>
          )}
        </div>
        <div className={`w-12 h-12 rounded-lg flex items-center justify-center ${getColorClasses(color)}`}>
          <span className="text-xl">{icon}</span>
        </div>
      </div>
    </div>
  );
};

export default StatsCard;