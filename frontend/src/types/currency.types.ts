export interface CurrencyDto {
  /** Currency code (e.g. USD) */
  code: string;
  /** Arabic currency code */
  arabicCode: string;
  /** Currency name (e.g. US Dollar) */
  name: string;
  /** Currency name in Arabic */
  arabicName: string;
  /** Whether this is the default currency */
  isDefault: boolean;
  /** Exchange rate relative to default currency; null for default currency */
  exchangeRate?: number;
  /** Last update timestamp for the exchange rate */
  lastUpdated?: string;
} 