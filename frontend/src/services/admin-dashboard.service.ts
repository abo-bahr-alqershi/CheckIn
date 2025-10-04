import { apiClient } from './api.service';
import type {
  AdminDashboardDto,
  GetAdminDashboardQuery,
  ExportDashboardReportCommand,
  CustomerReportDto,
  GetCustomerReportQuery,
  OccupancyReportDto,
  GetOccupancyRateQuery,
  GetOccupancyReportQuery,
  RevenueReportDto,
  GetRevenueReportQuery,
  ReviewSentimentDto,
  PropertyDto,
  GetTopPerformingPropertiesQuery
} from '../types/dashboard.types';
import type {
  FinancialSummaryDto,
  GetFinancialSummaryQuery,
  PropertyPerformanceDto,
  GetPropertyPerformanceQuery,
  PerformanceComparisonDto,
  GetPropertyPerformanceComparisonQuery,
  CancellationReasonDto,
  GetPlatformCancellationAnalysisQuery,
  RevenueBreakdownDto,
  GetPlatformRevenueBreakdownQuery,
  UserFunnelDto,
  GetUserAcquisitionFunnelQuery,
  CohortDto,
  GetCustomerCohortAnalysisQuery,
} from '../types/dashboard.types';

// Base URL for admin dashboard endpoints
const API_BASE = '/api/admin/dashboard';

export const AdminDashboardService = {
  /** Get admin dashboard data */
  getAdminDashboard: (query: GetAdminDashboardQuery) =>
    apiClient.post<AdminDashboardDto>(`${API_BASE}/dashboard`, query).then(res => res.data),

  /** Export dashboard report */
  exportReport: (command: ExportDashboardReportCommand) =>
    apiClient.post<ArrayBuffer>(`${API_BASE}/dashboard/export`, command, { responseType: 'arraybuffer' }).then(res => res.data),

  /** Get customer report */
  getCustomerReport: (query: GetCustomerReportQuery) =>
    apiClient.get<CustomerReportDto>(`${API_BASE}/dashboard/customer-report`, { params: query }).then(res => res.data),

  /** Get financial summary */
  getFinancialSummary: (query: GetFinancialSummaryQuery) =>
    apiClient.get<FinancialSummaryDto>(`${API_BASE}/dashboard/financial-summary`, { params: query }).then(res => res.data),

  /** Get property performance */
  getPropertyPerformance: (query: GetPropertyPerformanceQuery) =>
    apiClient.post<PropertyPerformanceDto>(`${API_BASE}/dashboard/performance`, query).then(res => res.data),

  /** Get property performance comparison */
  getPropertyPerformanceComparison: (query: GetPropertyPerformanceComparisonQuery) =>
    apiClient.post<PerformanceComparisonDto>(`${API_BASE}/dashboard/performance-comparison`, query).then(res => res.data),

  /** Get occupancy rate */
  getOccupancyRate: (query: GetOccupancyRateQuery) =>
    apiClient.post<number>(`${API_BASE}/dashboard/occupancy-rate`, query).then(res => res.data),

  /** Get occupancy report */
  getOccupancyReport: (query: GetOccupancyReportQuery) =>
    apiClient.post<OccupancyReportDto>(`${API_BASE}/dashboard/occupancy-report`, query).then(res => res.data),

  /** Get revenue report */
  getRevenueReport: (query: GetRevenueReportQuery) =>
    apiClient.post<RevenueReportDto>(`${API_BASE}/dashboard/revenue-report`, query).then(res => res.data),

  /** Get platform cancellation analysis */
  getPlatformCancellationAnalysis: (query: GetPlatformCancellationAnalysisQuery) =>
    apiClient.post<CancellationReasonDto[]>(`${API_BASE}/dashboard/cancellation-analysis`, query).then(res => res.data),

  /** Get platform revenue breakdown */
  getPlatformRevenueBreakdown: (query: GetPlatformRevenueBreakdownQuery) =>
    apiClient.post<RevenueBreakdownDto>(`${API_BASE}/dashboard/revenue-breakdown`, query).then(res => res.data),

  /** Get user acquisition funnel */
  getUserAcquisitionFunnel: (query: GetUserAcquisitionFunnelQuery) =>
    apiClient.post<UserFunnelDto>(`${API_BASE}/dashboard/user-acquisition-funnel`, query).then(res => res.data),

  /** Get customer cohort analysis */
  getCustomerCohortAnalysis: (query: GetCustomerCohortAnalysisQuery) =>
    apiClient.post<CohortDto[]>(`${API_BASE}/dashboard/customer-cohort-analysis`, query).then(res => res.data),

  /** Get review sentiment analysis */
  getReviewSentimentAnalysis: (propertyId: string) =>
    apiClient.get<ReviewSentimentDto>(`${API_BASE}/review-sentiment-analysis/${propertyId}`).then(res => res.data),

  /** Get top performing properties */
  getTopPerformingProperties: (query: GetTopPerformingPropertiesQuery) =>
    apiClient.get<PropertyDto[]>(`${API_BASE}/top-properties/${query.count}`).then(res => res.data),
}; 