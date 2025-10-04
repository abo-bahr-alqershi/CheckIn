// src/services/availability.service.ts

import apiClient from '../utils/api-client';
import { format, parseISO, isValid as isValidDate } from 'date-fns';
import {
  UnitAvailabilityDto,
  UpdateUnitAvailabilityCommand,
  BulkUpdateAvailabilityCommand,
  CloneAvailabilityCommand,
  DeleteAvailabilityCommand,
  CheckAvailabilityQuery,
  CheckAvailabilityResponse
} from '../types/availability.types';

class AvailabilityService {
  private basePath = '/api/admin/units';

  async getMonthlyAvailability(
    unitId: string,
    year: number,
    month: number
  ): Promise<UnitAvailabilityDto> {
    const response = await apiClient.get(
      `${this.basePath}/${unitId}/availability/${year}/${month}`
    );
    const dto = response.data.data as UnitAvailabilityDto;
    // Normalize calendar keys to 'yyyy-MM-dd' to match CalendarGrid expectations
    if (dto && dto.calendar) {
      const normalized: Record<string, typeof dto.calendar[keyof typeof dto.calendar]> = {};
      for (const [key, value] of Object.entries(dto.calendar as Record<string, any>)) {
        let date: Date;
        try {
          // Try ISO first; fallback to Date constructor
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

  async updateAvailability(
    command: UpdateUnitAvailabilityCommand
  ): Promise<void> {
    await apiClient.post(
      `${this.basePath}/${command.unitId}/availability`,
      command
    );
  }

  async bulkUpdateAvailability(
    command: BulkUpdateAvailabilityCommand
  ): Promise<void> {
    console.log('=== AVAILABILITY SERVICE DEBUG ===');
    console.log('Base Path:', this.basePath);
    console.log('Unit ID:', command.unitId);
    console.log('Full URL:', `${this.basePath}/${command.unitId}/availability/bulk`);
    console.log('Command Data:', command);
    console.log('Command JSON:', JSON.stringify(command, null, 2));
    console.log('=== END SERVICE DEBUG ===');
    
    await apiClient.post(
      `${this.basePath}/${command.unitId}/availability/bulk`,
      command
    );
  }

  async cloneAvailability(command: CloneAvailabilityCommand): Promise<void> {
    await apiClient.post(
      `${this.basePath}/${command.unitId}/availability/clone`,
      command
    );
  }

  async deleteAvailability(
    unitId: string,
    startDate: string,
    endDate: string
  ): Promise<void> {
    await apiClient.delete(
      `${this.basePath}/${unitId}/availability/${startDate}/${endDate}`
    );
  }

  async checkAvailability(
    query: CheckAvailabilityQuery
  ): Promise<CheckAvailabilityResponse> {
    const response = await apiClient.get(
      `${this.basePath}/${query.unitId}/availability/check`,
      { 
        params: { 
          checkIn: query.checkIn, 
          checkOut: query.checkOut,
          adults: query.adults,
          children: query.children,
          includePricing: query.includePricing
        } 
      }
    );
    return response.data.data;
  }
}

export default new AvailabilityService();