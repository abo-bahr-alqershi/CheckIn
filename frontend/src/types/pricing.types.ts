// src/types/pricing.types.ts

export interface UnitPricingDto {
  unitId: string;
  unitName: string;
  basePrice: number;
  currency: string;
  calendar: Record<string, PricingDayDto>;
  rules: PricingRuleDto[];
  stats: PricingStatsDto;
}

export interface PricingDayDto {
  price: number;
  priceType: string;
  colorCode: string;
  percentageChange?: number;
}

export interface PricingRuleDto {
  id: string;
  startDate: string;
  endDate: string;
  price: number;
  priceType: string;
  description: string;
}

export interface PricingStatsDto {
  averagePrice: number;
  minPrice: number;
  maxPrice: number;
  daysWithSpecialPricing: number;
  potentialRevenue: number;
}

export interface SeasonalPricingResponse {
  unitId: string;
  unitName: string;
  seasons: SeasonalPricingDto[];
  statistics: SeasonalPricingStatsDto;
}

export interface SeasonalPricingDto {
  id: string;
  name: string;
  type: string;
  startDate: string;
  endDate: string;
  price: number;
  percentageChange?: number;
  currency: string;
  pricingTier: string;
  priority: number;
  description: string;
  isActive: boolean;
  isRecurring: boolean;
  daysCount: number;
  totalRevenuePotential: number;
}

export interface SeasonalPricingStatsDto {
  totalSeasons: number;
  activeSeasons: number;
  upcomingSeasons: number;
  expiredSeasons: number;
  averageSeasonalPrice: number;
  maxSeasonalPrice: number;
  minSeasonalPrice: number;
  totalDaysCovered: number;
}

export interface PricingBreakdownDto {
  checkIn: string;
  checkOut: string;
  currency: string;
  days: DayPriceDto[];
  totalNights: number;
  subTotal: number;
  discount?: number;
  taxes?: number;
  total: number;
}

export interface DayPriceDto {
  date: string;
  price: number;
  priceType: string;
  description?: string;
}

export interface UpdateUnitPricingCommand {
  unitId: string;
  startDate: string;
  endDate: string;
  startTime?: string;
  endTime?: string;
  priceType: string;
  price: number;
  currency: string;
  pricingTier: string;
  percentageChange?: number;
  minPrice?: number;
  maxPrice?: number;
  description?: string;
  overwriteExisting: boolean;
}

export interface BulkUpdatePricingCommand {
  unitId: string;
  periods: PricingPeriodDto[];
  overwriteExisting: boolean;
}

export interface PricingPeriodDto {
  startDate: string;
  endDate: string;
  startTime?: string;
  endTime?: string;
  priceType: string;
  price: number;
  currency?: string;
  tier: string;
  percentageChange?: number;
  minPrice?: number;
  maxPrice?: number;
  description?: string;
  overwriteExisting: boolean;
}

export interface CopyPricingCommand {
  unitId: string;
  sourceStartDate: string;
  sourceEndDate: string;
  targetStartDate: string;
  repeatCount: number;
  adjustmentType: string;
  adjustmentValue: number;
  overwriteExisting: boolean;
}

export interface DeletePricingRuleCommand {
  unitId: string;
  pricingRuleId?: string;
  startDate?: string;
  endDate?: string;
}

export interface ApplySeasonalPricingCommand {
  unitId: string;
  seasons: SeasonDto[];
  currency: string;
}

export interface SeasonDto {
  name: string;
  type: string;
  startDate: string;
  endDate: string;
  price: number;
  percentageChange?: number;
  priority: number;
  minPrice?: number;
  maxPrice?: number;
  description?: string;
}

export interface PricingSummaryDto {
  totalPrice: number;
  averageNightlyPrice: number;
  currency: string;
  dailyPrices: DailyPriceDto[];
}

export interface DailyPriceDto {
  date: string;
  price: number;
  priceType: string;
}