import apiClient from './api.service';
import type { CityDto } from '../types/city.types';
import type { ResultDto } from '../types/common.types';

/** Front-end service for city settings */
export const CitySettingsService = {
  /** Get list of cities */
  getCities: (): Promise<CityDto[]> =>
    apiClient
      .get<ResultDto<CityDto[]>>('/api/admin/system-settings/cities')
      .then(res => res.data.data || []),

  /** Save or update list of cities */
  saveCities: (cities: CityDto[]): Promise<boolean> =>
    apiClient
      .put<ResultDto<boolean>>('/api/admin/system-settings/cities', cities)
      .then(res => res.data.data || false)
}; 