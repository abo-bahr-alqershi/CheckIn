import { Routes, Route, Navigate } from 'react-router-dom';
import AdminLayout from '../layouts/AdminLayout';
import PropertyOwnerLayout from '../layouts/PropertyOwnerLayout';
import AuthLayout from '../layouts/AuthLayout';
import Login from '../pages/auth/Login';
import Register from '../pages/auth/Register';
import ForgotPassword from '../pages/auth/ForgotPassword';
import TestPage from '../pages/TestPage';
import ModernDashboardLayout from '../components/layout/ModernDashboardLayout';
import Profile from '../pages/profile/Profile';
import Settings from '../pages/settings/Settings';

// Admin Pages
import AdminDashboard from '../pages/admin/AdminDashboard';
import AdminUsers from '../pages/admin/AdminUsers';
import AdminProperties from '../pages/admin/AdminProperties';
import AdminPropertyAndUnitTypes from '../pages/admin/AdminPropertyAndUnitTypes';
import AdminUnits from '../pages/admin/AdminUnits';
import AdminBookings from '../pages/admin/AdminBookings';
import AdminPayments from '../pages/admin/AdminPayments';
import AdminAmenities from '../pages/admin/AdminAmenities';
import AdminReviews from '../pages/admin/AdminReviews';
import AdminNotifications from '../pages/admin/AdminNotifications';
import AdminReports from '../pages/admin/AdminReports';
import AdminAuditLogs from '../pages/admin/AdminAuditLogs';
import AdminSettings from '../pages/admin/AdminSettings';
import PropertyImageGallery from '../pages/admin/PropertyImageGallery';
import UnitImageGallery from '../pages/admin/UnitImageGallery';
import UserDetails from '../pages/admin/UserDetails';
import AdminPropertyServices from '../pages/admin/AdminPropertyServices';
import AvailabilityPricingManagement from '../pages/admin/AvailabilityPricingManagement';
import InputManagementPage from '../pages/admin/InputManagementPage';
import AdminSections from '../pages/admin/AdminSections';

// Property Owner Pages
import PropertyOwnerDashboard from '../pages/property/PropertyOwnerDashboard';
import PropertyOwnerProperties from '../pages/property/PropertyOwnerProperties';
import PropertyOwnerUnits from '../pages/property/PropertyOwnerUnits';
import PropertyOwnerBookings from '../pages/property/PropertyOwnerBookings';
import PropertyOwnerStaff from '../pages/property/PropertyOwnerStaff';

const AppRoutes = () => {
  return (
    <Routes>
      {/* Auth Routes */}
      <Route path="/auth" element={<AuthLayout />}>
        <Route path="login" element={<Login />} />
        <Route path="forgot-password" element={<ForgotPassword />} />
        <Route index element={<Navigate to="login" replace />} />
      </Route>

      {/* Register Route - Full Width */}
      <Route path="/auth/register" element={<Register />} />

      {/* Admin Routes */}
      <Route path="/admin" element={<AdminLayout />}>
        <Route index element={<AdminDashboard />} />
        <Route path="inputs" element={<InputManagementPage />} />
        <Route path="dashboard" element={<AdminDashboard />} />
        <Route path="users" element={<AdminUsers />} />
        <Route path="users/:userId" element={<UserDetails />} />
        <Route path="properties" element={<AdminProperties />} />
        <Route path="property-types" element={<AdminPropertyAndUnitTypes />} />
        <Route path="units" element={<AdminUnits />} />
        <Route path="property-images/:propertyId" element={<PropertyImageGallery />} />
        <Route path="unit-images/:propertyId/:unitId" element={<UnitImageGallery />} />
        <Route path="bookings" element={<AdminBookings />} />
        <Route path="payments" element={<AdminPayments />} />
        <Route path="amenities" element={<AdminAmenities />} />
        <Route path="property-services" element={<AdminPropertyServices />} />
        {/* ربط إدارة الإتاحة والتسعير على مستوى الوحدة */}
        <Route path="units/:unitId/availability-pricing" element={<AvailabilityPricingManagement />} />
        <Route path="reviews" element={<AdminReviews />} />
        <Route path="notifications" element={<AdminNotifications />} />
        <Route path="reports" element={<AdminReports />} />
        <Route path="audit-logs" element={<AdminAuditLogs />} />
        <Route path="settings" element={<AdminSettings />} />
        <Route path="sections" element={<AdminSections />} />
      </Route>

      {/* Property Owner Routes */}
      <Route path="/property-owner" element={<PropertyOwnerLayout />}>
        <Route index element={<PropertyOwnerDashboard />} />
        <Route path="dashboard" element={<PropertyOwnerDashboard />} />
        <Route path="properties" element={<PropertyOwnerProperties />} />
        <Route path="units" element={<PropertyOwnerUnits />} />
        <Route path="bookings" element={<PropertyOwnerBookings />} />
        <Route path="staff" element={<PropertyOwnerStaff />} />
      </Route>

      {/* Test page */}
      <Route path="/test" element={<TestPage />} />

      {/* صفحات الملف الشخصي والإعدادات للمستخدم */}
      <Route
        path="/profile"
        element={
          <ModernDashboardLayout>
            <Profile />
          </ModernDashboardLayout>
        }
      />
      <Route
        path="/settings"
        element={
          <ModernDashboardLayout>
            <Settings />
          </ModernDashboardLayout>
        }
      />

      {/* Default redirect */}
      <Route path="/" element={<Navigate to="/auth/login" replace />} />
      <Route path="*" element={<Navigate to="/auth/login" replace />} />
    </Routes>
  );
};

export default AppRoutes;