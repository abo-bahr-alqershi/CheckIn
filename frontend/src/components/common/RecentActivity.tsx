interface Activity {
  id: number;
  type: string;
  message: string;
  time: string;
  user: string;
}

interface RecentActivityProps {
  activities: Activity[];
}

const RecentActivity = ({ activities }: RecentActivityProps) => {
  const getActivityIcon = (type: string) => {
    const icons = {
      booking: '📝',
      property: '🏢',
      payment: '💰',
      review: '⭐',
      user: '👤',
      default: '📋',
    };
    return icons[type as keyof typeof icons] || icons.default;
  };

  const getActivityColor = (type: string) => {
    const colors = {
      booking: 'bg-blue-50 text-blue-600',
      property: 'bg-green-50 text-green-600',
      payment: 'bg-yellow-50 text-yellow-600',
      review: 'bg-purple-50 text-purple-600',
      user: 'bg-indigo-50 text-indigo-600',
      default: 'bg-gray-50 text-gray-600',
    };
    return colors[type as keyof typeof colors] || colors.default;
  };

  return (
    <div className="space-y-4">
      {activities.length === 0 ? (
        <div className="text-center py-8 text-gray-500">
          <p>لا توجد أنشطة حديثة</p>
        </div>
      ) : (
        activities.map((activity) => (
          <div key={activity.id} className="flex items-start space-x-3 space-x-reverse">
            <div className={`w-8 h-8 rounded-full flex items-center justify-center ${getActivityColor(activity.type)}`}>
              <span className="text-sm">{getActivityIcon(activity.type)}</span>
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-gray-900">{activity.message}</p>
              <div className="flex items-center text-xs text-gray-500 mt-1">
                <span>{activity.user}</span>
                <span className="mx-2">•</span>
                <span>{activity.time}</span>
              </div>
            </div>
          </div>
        ))
      )}
      
      {activities.length > 0 && (
        <div className="text-center pt-4 border-t border-gray-200">
          <button className="text-sm text-blue-600 hover:text-blue-700 font-medium">
            عرض جميع الأنشطة
          </button>
        </div>
      )}
    </div>
  );
};

export default RecentActivity;