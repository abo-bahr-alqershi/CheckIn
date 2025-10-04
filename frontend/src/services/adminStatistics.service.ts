import apiClient from './api.service';
import type { DashboardStatsDto } from '../types/dashboard.types';

/**
 * جلب إحصائيات لوحة التحكم من الخادم
 */
export async function fetchDashboardStats(): Promise<DashboardStatsDto> {
  const { data } = await apiClient.get<DashboardStatsDto>('/api/admin/dashboard/stats');
  return data;
} 