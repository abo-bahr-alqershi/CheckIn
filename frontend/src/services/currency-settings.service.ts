import { apiClient } from './api.service';
import type { CurrencyDto } from '../types/currency.types';
import type { ResultDto } from '../types/common.types';

const API_BASE_COMMON = '/api/common/system-settings';
const API_BASE_ADMIN = '/api/admin/system-settings';

/**
 * Services for currency settings
 */
export const CurrencySettingsService = {
  /** Get currency list */
  getCurrencies: (): Promise<CurrencyDto[]> =>
    apiClient
      .get<ResultDto<CurrencyDto[]>>(`${API_BASE_COMMON}/currencies`)
      .then((res) => res.data.data || []),

  /** Save or update currency list (admin) */
  saveCurrencies: (currencies: CurrencyDto[]): Promise<boolean> =>
    apiClient
      .put<ResultDto<boolean>>(`${API_BASE_ADMIN}/currencies`, currencies)
      .then((res) => res.data.data || false),
}; 