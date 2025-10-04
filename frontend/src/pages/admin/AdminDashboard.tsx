import { useEffect, useState } from 'react';
import StatsCard from '../../components/common/StatsCard';
import RecentActivity from '../../components/common/RecentActivity';

interface DashboardStats {
  totalUsers: number;
  totalProperties: number;
  totalBookings: number;
  totalRevenue: number;
  activeBookings: number;
  pendingApprovals: number;
}

const AdminDashboard = () => {
  const [stats, setStats] = useState<DashboardStats>({
    totalUsers: 0,
    totalProperties: 0,
    totalBookings: 0,
    totalRevenue: 0,
    activeBookings: 0,
    pendingApprovals: 0,
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // TODO: Fetch real data from admin dashboard service
    const fetchDashboardData = async () => {
      setIsLoading(true);
      // Simulate API call
      setTimeout(() => {
        setStats({
          totalUsers: 1284,
          totalProperties: 456,
          totalBookings: 2341,
          totalRevenue: 847650,
          activeBookings: 89,
          pendingApprovals: 12,
        });
        setIsLoading(false);
      }, 1000);
    };

    fetchDashboardData();
  }, []);

  const recentActivities = [
    {
      id: 1,
      type: 'booking',
      message: 'حجز جديد في فندق الريتز',
      time: 'منذ 5 دقائق',
      user: 'أحمد محمد',
    },
    {
      id: 2,
      type: 'property',
      message: 'تم إضافة كيان جديد للمراجعة',
      time: 'منذ 15 دقيقة',
      user: 'سارة أحمد',
    },
    {
      id: 3,
      type: 'payment',
      message: 'تم استلام دفعة بقيمة 2,500 ريال',
      time: 'منذ 30 دقيقة',
      user: 'محمد عبدالله',
    },
    {
      id: 4,
      type: 'review',
      message: 'تقييم جديد يحتاج للمراجعة',
      time: 'منذ ساعة',
      user: 'فاطمة علي',
    },
  ];

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <h1 className="text-2xl font-bold text-gray-900 mb-2">
          لوحة المعلومات الإدارية
        </h1>
        <p className="text-gray-600">
          مرحباً بك في لوحة التحكم الرئيسية لإدارة نظام الحجوزات
        </p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-6">
        <StatsCard
          title="إجمالي المستخدمين"
          value={stats.totalUsers}
          icon="👥"
          color="blue"
          change="+12%"
          changeType="increase"
        />
        <StatsCard
          title="الكيانات المسجلة"
          value={stats.totalProperties}
          icon="🏢"
          color="green"
          change="+8%"
          changeType="increase"
        />
        <StatsCard
          title="إجمالي الحجوزات"
          value={stats.totalBookings}
          icon="📝"
          color="purple"
          change="+15%"
          changeType="increase"
        />
        <StatsCard
          title="الإيرادات الإجمالية"
          value={stats.totalRevenue}
          icon="💰"
          color="yellow"
          change="+23%"
          changeType="increase"
          prefix="ريال"
        />
        <StatsCard
          title="الحجوزات النشطة"
          value={stats.activeBookings}
          icon="📅"
          color="indigo"
          change="-3%"
          changeType="decrease"
        />
        <StatsCard
          title="في انتظار الموافقة"
          value={stats.pendingApprovals}
          icon="⏳"
          color="red"
          change="+2"
          changeType="increase"
        />
      </div>

      {/* Charts and Recent Activity */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Revenue Chart */}
        <div className="lg:col-span-2 bg-white rounded-lg shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            الإيرادات الشهرية
          </h2>
          <div className="h-64 flex items-center justify-center text-gray-500">
            {/* TODO: Add Recharts component */}
            <p>مخطط الإيرادات سيتم إضافته هنا</p>
          </div>
        </div>

        {/* Recent Activity */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            النشاطات الأخيرة
          </h2>
          <RecentActivity activities={recentActivities} />
        </div>
      </div>

      {/* Quick Actions */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          إجراءات سريعة
        </h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <button className="p-4 bg-blue-50 rounded-lg text-center hover:bg-blue-100 transition-colors">
            <div className="text-2xl mb-2">👤</div>
            <div className="text-sm font-medium text-blue-700">إضافة مستخدم</div>
          </button>
          <button className="p-4 bg-green-50 rounded-lg text-center hover:bg-green-100 transition-colors">
            <div className="text-2xl mb-2">🏢</div>
            <div className="text-sm font-medium text-green-700">موافقة على كيان</div>
          </button>
          <button className="p-4 bg-purple-50 rounded-lg text-center hover:bg-purple-100 transition-colors">
            <div className="text-2xl mb-2">📊</div>
            <div className="text-sm font-medium text-purple-700">عرض التقارير</div>
          </button>
          <button className="p-4 bg-orange-50 rounded-lg text-center hover:bg-orange-100 transition-colors">
            <div className="text-2xl mb-2">⚙️</div>
            <div className="text-sm font-medium text-orange-700">إعدادات النظام</div>
          </button>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;