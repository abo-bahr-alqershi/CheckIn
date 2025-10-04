import { NavLink } from 'react-router-dom';

interface MenuItem {
  title: string;
  path: string;
  icon: string;
}

interface SidebarProps {
  menuItems: MenuItem[];
  userRole: string;
}

const Sidebar = ({ menuItems, userRole }: SidebarProps) => {
  const getRoleTitle = (role: string) => {
    switch (role) {
      case 'admin':
        return 'لوحة الإدارة';
      case 'property-owner':
        return 'لوحة مالك الكيان';
      default:
        return 'لوحة التحكم';
    }
  };

  return (
    <div className="bg-white w-64 min-h-screen shadow-lg border-l border-gray-200">
      <div className="p-6 border-b border-gray-200">
        <h2 className="text-xl font-bold text-gray-800">
          {getRoleTitle(userRole)}
        </h2>
        <p className="text-sm text-gray-600 mt-1">
          نظام إدارة الحجوزات
        </p>
      </div>
      
      <nav className="mt-6">
        <div className="px-3">
          {menuItems.map((item) => (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                `flex items-center px-4 py-3 mb-2 rounded-lg transition-colors duration-200 ${
                  isActive
                    ? 'bg-blue-50 text-blue-700 border-l-4 border-blue-500'
                    : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                }`
              }
            >
              <span className="text-xl ml-3">{item.icon}</span>
              <span className="font-medium">{item.title}</span>
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  );
};

export default Sidebar;