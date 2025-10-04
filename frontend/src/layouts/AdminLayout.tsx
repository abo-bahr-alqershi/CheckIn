import { Outlet } from 'react-router-dom';
import ModernDashboardLayout from '../components/layout/ModernDashboardLayout';

const AdminLayout = () => {
  return (
    <ModernDashboardLayout>
      <Outlet />
    </ModernDashboardLayout>
  );
};

export default AdminLayout;