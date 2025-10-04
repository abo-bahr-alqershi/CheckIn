import { useState, useEffect } from 'react';
import { CurrencySettingsService } from '../services/currency-settings.service';
import type { CurrencyDto } from '../types/currency.types';

/**
 * Custom hook to fetch currency settings from back-end.
 */
export const useCurrencies = () => {
  const [currencies, setCurrencies] = useState<CurrencyDto[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    CurrencySettingsService.getCurrencies()
      .then((data) => {
        setCurrencies(data);
      })
      .catch((err) => {
        console.error('Failed to load currencies', err);
        setError(err);
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  return { currencies, loading, error };
}; 