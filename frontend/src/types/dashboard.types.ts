/**
 * Date range DTO
 */
export interface DateRangeDto {
  startDate: string;
  endDate: string;
}

/**
 * Admin dashboard data
 */
export interface AdminDashboardDto {
  totalUsers: number;
  totalProperties: number;
  totalBookings: number;
  totalRevenue: number;
}

/**
 * Owner dashboard data
 */
export interface ExportResultDto {
  exportId: string;
  fileName: string;
  downloadUrl: string;
  fileFormat: string;
  fileSizeBytes: number;
  fileSizeReadable: string;
  contentType: string;
  createdAt: string;
  expiryDate: string;
  requestedBy: string;
  requestedByName: string;
  recordsCount: number;
  exportedFields: string[];
  appliedFilters?: Record<string, any>;
  status: string;
  statusMessage?: string;
  processingDuration: string;
  metadata?: ExportMetadataDto;
  warnings?: string[];
  canRetry: boolean;
  security?: ExportSecurityDto;
}

export interface ExportMetadataDto {
  exportSystemVersion: string;
  templateVersion?: string;
  timezone: string;
  encoding: string;
  language: string;
  currency?: string;
  dateFormat: string;
  timeFormat: string;
  decimalSeparator: string;
  thousandsSeparator: string;
  customMetadata?: Record<string, any>;
  dataSource: string;
  databaseVersion?: string;
  dataSnapshotAt: string;
}

export interface ExportSecurityDto {
  isPasswordProtected: boolean;
  encryptionLevel?: string;
  encryptionType?: string;
  containsSensitiveData: boolean;
  privacyLevel: string;
  accessRestrictions?: string[];
  redactedFields?: string[];
  retentionPolicy?: string;
  allowedDownloads?: number;
  currentDownloadCount: number;
}

export interface OwnerDashboardDto {
  ownerId: string;
  ownerName: string;
  propertyCount: number;
  bookingCount: number;
  totalRevenue: number;
}

export interface StatisticsPeriodDto {
  startDate: string;
  endDate: string;
  periodName: string;
  periodType: PeriodType;
  value: number;
  count: number;
  average?: number;
  changePercentage?: number;
  changeDirection?: ChangeDirection;
  additionalData?: Record<string, any>;
}

export type PeriodType = {
  DAILY: 'DAILY',
  WEEKLY: 'WEEKLY',
  MONTHLY: 'MONTHLY',
  QUARTERLY: 'QUARTERLY',
  YEARLY: 'YEARLY',
  CUSTOM: 'CUSTOM',
}

export type ChangeDirection = {
  UP: 'UP',
  DOWN: 'DOWN',
  STABLE: 'STABLE',
}

/**
 * Customer dashboard data
 */
export interface CustomerDashboardDto {
  customerId: string;
  customerName: string;
  upcomingBookings: number;
  pastBookings: number;
  totalSpent: number;
}

/**
 * Query to retrieve admin dashboard data
 */
export interface GetAdminDashboardQuery {
  range: DateRangeDto;
}

/**
 * Query to retrieve owner dashboard data
 */
export interface GetOwnerDashboardQuery {
  ownerId: string;
  range: DateRangeDto;
}

/**
 * Query to retrieve customer dashboard data
 */
export interface GetCustomerDashboardQuery {
  customerId: string;
}

import type { PropertyDto } from './property.types';

// أنواع إضافية للوحة التحكم

// أنواع لوحات التحكم
export type DashboardType = 'Admin' | 'Owner' | 'Customer';

// تنسيقات تقرير لوحة التحكم
export type ReportFormat = 'Pdf' | 'Excel' | 'Csv';

/**
 * أمر تصدير تقرير لوحة التحكم
 */
export interface ExportDashboardReportCommand {
  dashboardType: DashboardType;
  targetId: string;
  format: ReportFormat;
}

/**
 * استعلام تقرير العملاء لإدارة النظام
 */
export interface GetCustomerReportQuery {
  startDate: string;
  endDate: string;
  filters?: string;
}

/**
 * عنصر تقرير العملاء
 */
export interface CustomerReportItemDto {
  userId: string;
  customerName: string;
  bookingsCount: number;
  totalSpent: number;
}

/**
 * تقرير العملاء
 */
export interface CustomerReportDto {
  items: CustomerReportItemDto[];
}

/**
 * ملخص مالي
 */
export interface FinancialSummaryDto {
  totalRevenue: number;
  totalBookings: number;
  averageBookingValue: number;
}

/**
 * مؤشرات أداء الكيان
 */
export interface PropertyPerformanceDto {
  occupancyRate: number;
  totalRevenue: number;
  totalBookings: number;
  completedBookings: number;
  cancelledBookings: number;
  cancellationRate: number;
  averageStay: number;
  averageRevenuePerBooking: number;
  averageRating: number;
  totalReviews: number;
}

/**
 * مقارنة أداء الكيان
 */
export interface PerformanceComparisonDto {
  currentPeriodRevenue: number;
  previousPeriodRevenue: number;
  revenueChangePercentage: number;
}

/**
 * استعلام نسبة إشغال الكيان
 */
export interface GetOccupancyRateQuery {
  propertyId: string;
  range: DateRangeDto;
}

/**
 * استعلام تقرير الإشغال
 */
export interface GetOccupancyReportQuery {
  propertyId: string;
  startDate: string;
  endDate: string;
}

/**
 * عنصر تقرير الإشغال اليومي
 */
export interface OccupancyReportItemDto {
  date: string;
  occupiedUnits: number;
  totalUnits: number;
  occupancyRate: number;
}

/**
 * تقرير الإشغال
 */
export interface OccupancyReportDto {
  items: OccupancyReportItemDto[];
}

/**
 * استعلام تقرير الإيرادات
 */
export interface GetRevenueReportQuery {
  startDate: string;
  endDate: string;
  propertyId?: string;
}

/**
 * عنصر تقرير الإيرادات اليومية
 */
export interface RevenueReportItemDto {
  date: string;
  revenue: number;
}

/**
 * تقرير الإيرادات
 */
export interface RevenueReportDto {
  items: RevenueReportItemDto[];
}

/**
 * تحليل أسباب إلغاء الحجوزات للمنصة
 */
export interface CancellationReasonDto {
  reason: string;
  count: number;
  lostRevenue: number;
}

/**
 * تفصيل الإيرادات في المنصة
 */
export interface RevenueBreakdownDto {
  totalRevenue: number;
  revenueFromCommissions: number;
  revenueFromServices: number;
}

/**
 * بيانات قمع اكتساب العملاء
 */
export interface UserFunnelDto {
  totalVisitors: number;
  totalSearches: number;
  totalPropertyViews: number;
  totalBookingsCompleted: number;
  conversionRates: Record<string, number>;
}

/**
 * تحليل أفواج العملاء
 */
export interface CohortDto {
  cohortPeriod: string;
  totalNewUsers: number;
  monthlyRetention: number[];
}

/**
 * تحليل مشاعر التقييمات
 */
export interface ReviewSentimentDto {
  overallSentimentScore: number;
  positiveKeywords: string[];
  negativeKeywords: string[];
}

/**
 * استعلام أفضل الكيانات أداءً
 */
export interface GetTopPerformingPropertiesQuery {
  count: number;
}

/**
 * نوع بيانات الكيان لأفضل أداء
 */
export interface CategoricalDataDto {
  category: string;
  value: number;
}


export interface FinancialSummaryDto {
    totalRevenue: number;
    totalBookings: number;
    averageBookingValue: number;
}

export interface GetFinancialSummaryQuery {
    startDate: string;
    endDate: string;
    propertyId?: string;
}

export interface GetPropertyPerformanceQuery {
    propertyId: string;
    startDate: string;
    endDate: string;
}

/**
 * استعلام للحصول على مقارنة أداء الكيان بين فترتين زمنيتين
 */
export interface GetPropertyPerformanceComparisonQuery {
  propertyId: string;
  currentRange: DateRangeDto;
  previousRange: DateRangeDto;
}

/**
 * استعلام للحصول على تحليل أسباب إلغاء الحجوزات ضمن نطاق زمني
 */
export interface GetPlatformCancellationAnalysisQuery {
  range: DateRangeDto;
}

/**
 * استعلام للحصول على تفصيل الإيرادات الكلي لمنصة ضمن نطاق زمني
 */
export interface GetPlatformRevenueBreakdownQuery {
  range: DateRangeDto;
}

/**
 * استعلام للحصول على بيانات قمع اكتساب العملاء ضمن نطاق زمني
 */
export interface GetUserAcquisitionFunnelQuery {
  range: DateRangeDto;
}

/**
 * استعلام للحصول على تحليل أفواج العملاء ضمن نطاق زمني
 */
export interface GetCustomerCohortAnalysisQuery {
  range: DateRangeDto;
}


export type { PropertyDto };

export interface DashboardStatsDto {
  unverifiedUsers: number;
  unapprovedProperties: number;
  unconfirmedBookings: number;
  unreadNotifications: number;
} 