// src/services/pricing.service.ts

import apiClient from '../utils/api-client';
import { format, parseISO, isValid as isValidDate } from 'date-fns';
import {
  UnitPricingDto,
  UpdateUnitPricingCommand,
  BulkUpdatePricingCommand,
  CopyPricingCommand,
  DeletePricingRuleCommand,
  ApplySeasonalPricingCommand,
  SeasonalPricingResponse,
  PricingBreakdownDto
} from '../types/pricing.types';

class PricingService {
  private basePath = '/api/admin/units';

  async getMonthlyPricing(
    unitId: string,
    year: number,
    month: number
  ): Promise<UnitPricingDto> {
    const response = await apiClient.get(
      `${this.basePath}/${unitId}/pricing/${year}/${month}`
    );
    const dto = response.data.data as UnitPricingDto;
    // Normalize calendar keys from Date strings to 'yyyy-MM-dd'
    if (dto && dto.calendar) {
      const normalized: Record<string, typeof dto.calendar[keyof typeof dto.calendar]> = {};
      for (const [key, value] of Object.entries(dto.calendar as Record<string, any>)) {
        let date: Date;
        try {
          const parsed = parseISO(key);
          date = isValidDate(parsed) ? parsed : new Date(key);
        } catch {
          date = new Date(key);
        }
        const normalizedKey = format(date, 'yyyy-MM-dd');
        normalized[normalizedKey] = value as any;
      }
      (dto as any).calendar = normalized as any;
    }
    return dto;
  }

  async updatePricing(command: UpdateUnitPricingCommand): Promise<void> {
    await apiClient.post(
      `${this.basePath}/${command.unitId}/pricing`,
      command
    );
  }

  async bulkUpdatePricing(
    command: BulkUpdatePricingCommand
  ): Promise<void> {
    await apiClient.post(
      `${this.basePath}/${command.unitId}/pricing/bulk`,
      command
    );
  }

  async copyPricing(command: CopyPricingCommand): Promise<void> {
    await apiClient.post(
      `${this.basePath}/${command.unitId}/pricing/copy`,
      command
    );
  }

  async deletePricing(unitId: string, pricingId: string): Promise<void> {
    await apiClient.delete(
      `${this.basePath}/${unitId}/pricing/${pricingId}`
    );
  }

  async getSeasonalPricing(unitId: string): Promise<SeasonalPricingResponse> {
    const response = await apiClient.get(
      `${this.basePath}/${unitId}/pricing/templates`
    );
    return response.data;
  }

  async applySeasonalPricing(
    command: ApplySeasonalPricingCommand
  ): Promise<void> {
    await apiClient.post(
      `${this.basePath}/${command.unitId}/pricing/apply-template`,
      command
    );
  }

  async getPricingBreakdown(
    unitId: string,
    checkIn: string,
    checkOut: string
  ): Promise<PricingBreakdownDto> {
    const response = await apiClient.get(
      `${this.basePath}/${unitId}/pricing/breakdown`,
      { params: { checkIn, checkOut } }
    );
    return response.data.data;
  }
}

export default new PricingService();