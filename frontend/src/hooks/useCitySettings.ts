import { useState, useEffect } from 'react';
import { CitySettingsService } from '../services/city-settings.service';
import type { CityDto } from '../types/city.types';

export const useCitySettings = () => {
  const [cities, setCities] = useState<CityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    CitySettingsService.getCities()
      .then(data => setCities(data))
      .catch(err => setError(err))
      .finally(() => setLoading(false));
  }, []);

  return { cities, loading, error };
}; 