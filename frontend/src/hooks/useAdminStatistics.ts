import { useQuery } from '@tanstack/react-query';
import { fetchDashboardStats } from '../services/adminStatistics.service';
import type { DashboardStatsDto } from '../types/dashboard.types';

export function useAdminStatistics() {
  return useQuery<DashboardStatsDto>({
    queryKey: ['adminStats'],
    queryFn: fetchDashboardStats,
    staleTime: 60 * 1000, // cache for 1 minute
    refetchOnWindowFocus: false,
    refetchOnMount: true,
  });
} 