
// src/types/availability.types.ts

import type { PricingSummaryDto } from './pricing.types';

export enum AvailabilityStatus {
  Available = 'Available',
  Booked = 'Booked',
  Blocked = 'Blocked',
  Maintenance = 'Maintenance',
  Hold = 'Hold'
}

// Lightweight string-based status type for UI components
export type AvailabilityStatusString = 'available' | 'unavailable' | 'maintenance' | 'blocked' | 'booked';

export type PricingTier = 'normal' | 'high' | 'peak' | 'discount' | 'custom' | 'custom';
export type PriceType = 'base' | 'weekend' | 'seasonal' | 'holiday' | 'special_event' | 'custom';
export type UnavailabilityReason = 'maintenance' | 'vacation' | 'private_booking' | 'renovation' | 'other';
export type ConflictResolutionAction = 'availability' | 'pricing';

export interface BaseUnit {
  unitId: string;
  property_id?: string;
  unitName: string;
  unitType: string;
  capacity: number;
  basePrice: number;
  isActive?: boolean;
}

export interface UnitAvailability {
  availabilityId?: string;
  unitId: string;
  startDate: string | Date;
  endDate: string | Date;
  startTime?: string;
  endTime?: string;
  status: AvailabilityStatusString;
  reason?: UnavailabilityReason | string;
  notes?: string;
  createdBy?: string;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface PricingRule {
  pricingId?: string;
  unitId?: string;
  priceType: PriceType | string;
  startDate: string | Date;
  endDate: string | Date;
  priceAmount: number;
  pricingTier?: PricingTier | string;
  percentageChange?: number;
  minPrice?: number;
  maxPrice?: number;
  description?: string;
  currency?: string;
  isActive?: boolean;
  createdBy?: string;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface BookingConflict {
  conflictId?: string;
  unitId: string;
  bookingId: string;
  bookingStatus?: string;
  totalAmount?: number;
  paymentStatus?: string;
  conflictType?: 'availability' | 'pricing';
  impactLevel?: 'low' | 'medium' | 'high' | 'critical';
  suggestedActions?: ConflictResolutionAction[];
}

export interface CreateAvailabilityRequest {
  unitId: string;
  startDate: string;
  endDate: string;
  startTime?: string;
  endTime?: string;
  status: AvailabilityStatusString;
  reason?: UnavailabilityReason | string;
  notes?: string;
  overrideConflicts?: boolean;
}

export interface UpdateAvailabilityRequest extends CreateAvailabilityRequest {
  availabilityId?: string;
}

export interface CreatePricingRequest {
  unitId: string;
  startDate: string;
  endDate: string;
  startTime?: string;
  endTime?: string;
  priceAmount: number;
  pricingTier: PricingTier;
  percentageChange?: number;
  minPrice?: number;
  maxPrice?: number;
  description?: string;
  currency: string;
  overrideConflicts?: boolean;
}

export interface UpdatePricingRequest extends CreatePricingRequest {
  pricingId?: string;
}

export interface AvailabilitySearchRequest {
  unitIds?: string[];
  propertyId?: string;
  startDate?: string;
  endDate?: string;
  status?: AvailabilityStatusString[];
  includeConflicts?: boolean;
}

export interface AvailabilitySearchResponse {
  availabilities: UnitAvailability[];
  conflicts: BookingConflict[];
  total_count: number;
  has_more: boolean;
}

export interface PricingSearchRequest {
  unitIds?: string[];
  propertyId?: string;
  startDate?: string;
  endDate?: string;
  priceTypes?: PriceType[];
  pricingTiers?: PricingTier[];
  includeConflicts?: boolean;
}

export interface PricingSearchResponse {
  pricing_rules: PricingRule[];
  conflicts: BookingConflict[];
  total_count: number;
  has_more: boolean;
}

export interface ConflictCheckRequest {
  unitId: string;
  startDate: string;
  endDate: string;
  startTime?: string;
  endTime?: string;
  checkType: 'availability' | 'pricing' | 'both';
}

export interface ConflictCheckResponse {
  hasConflicts: boolean;
  conflicts: BookingConflict[];
  recommendations: {
    action: string;
    description: string;
    feasible: boolean;
    estimated_cost?: number;
  }[];
}

export interface UnitManagementData {
  unit: BaseUnit;
  currentAvailability?: AvailabilityStatusString;
  activePricingRules?: PricingRule[];
  upcomingBookings?: Array<{
    bookingId: string;
    guestName?: string;
    startDate: Date | string;
    endDate: Date | string;
    status?: string;
    totalAmount?: number;
  }>;
  availabilityCalendar?: Array<{
    date: string;
    status: AvailabilityStatusString;
    reason?: string;
    pricingTier?: PricingTier;
    currentPrice?: number;
    currency?: string;
    startDate?: string;
    endDate?: string;
  }>;
  stats?: {
    totalUnits?: number;
    availableUnits?: number;
    unavailableUnits?: number;
    maintenanceUnits?: number;
    bookedUnits?: number;
    totalRevenueToday?: number;
    avgOccupancyRate?: number;
  };
}

export interface AvailabilityError {
  error_code: string;
  error_type: 'validation' | 'conflict' | 'permission' | 'system';
  message: string;
  details?: any;
  suggested_action?: string;
}

export interface PricingError {
  error_code: string;
  error_type: 'validation' | 'conflict' | 'permission' | 'system';
  message: string;
  details?: any;
  suggested_action?: string;
}

// Keep the existing DTOs below
export interface UnitAvailabilityDto {
  unitId: string;
  unitName: string;
  calendar: Record<string, AvailabilityStatusDto>;
  periods: AvailabilityPeriodDto[];
  stats: AvailabilityStatsDto;
}

export interface AvailabilityStatusDto {
  status: string;
  reason?: string;
  bookingId?: string;
  colorCode: string;
}

export interface AvailabilityPeriodDto {
  startDate: string;
  endDate: string;
  status: string;
  reason?: string;
  notes?: string;
  overwriteExisting: boolean;
}

export interface AvailabilityStatsDto {
  totalDays: number;
  availableDays: number;
  bookedDays: number;
  blockedDays: number;
  occupancyRate: number;
}

export interface CheckAvailabilityResponse {
  isAvailable: boolean;
  status: string;
  blockedPeriods: BlockedPeriodDto[];
  availablePeriods: AvailablePeriodDto[];
  details: AvailabilityDetailsDto;
  pricingSummary?: PricingSummaryDto;
  messages: string[];
}

export interface BlockedPeriodDto {
  startDate: string;
  endDate: string;
  status: string;
  reason: string;
  notes: string;
}

export interface AvailablePeriodDto {
  startDate: string;
  endDate: string;
  price?: number;
  currency?: string;
}

export interface AvailabilityDetailsDto {
  unitId: string;
  unitName: string;
  unitType: string;
  maxAdults: number;
  maxChildren: number;
  totalNights: number;
  isMultiDays: boolean;
  isRequiredToDetermineTheHour: boolean;
}

export interface UpdateUnitAvailabilityCommand {
  unitId: string;
  startDate: string;
  endDate: string;
  status: string;
  reason?: string;
  notes?: string;
  overwriteExisting: boolean;
}

export interface BulkUpdateAvailabilityCommand {
  unitId: string;
  periods: AvailabilityPeriodDto[];
  overwriteExisting: boolean;
}

export interface CloneAvailabilityCommand {
  unitId: string;
  sourceStartDate: string;
  sourceEndDate: string;
  targetStartDate: string;
  repeatCount: number;
}

export interface DeleteAvailabilityCommand {
  unitId: string;
  availabilityId?: string;
  startDate?: string;
  endDate?: string;
  forceDelete?: boolean;
}

export interface CheckAvailabilityQuery {
  unitId: string;
  checkIn: string;
  checkOut: string;
  adults?: number;
  children?: number;
  includePricing?: boolean;
}