import { Outlet } from 'react-router-dom';
import ModernDashboardLayout from '../components/layout/ModernDashboardLayout';

const PropertyOwnerLayout = () => {
  return (
    <ModernDashboardLayout>
      <Outlet />
    </ModernDashboardLayout>
  );
};

export default PropertyOwnerLayout;