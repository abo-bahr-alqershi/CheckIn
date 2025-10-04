import { useState, useEffect, useCallback, useRef } from 'react';
import { useQuery, useMutation } from '@tanstack/react-query';
import { MapsService } from '../services/maps.service';
import type {
  Coordinates,
  MapMarker,
  GeocodingOptions,
  GeocodingResult,
  RouteOptions,
  RouteResult,
  GeoBounds
} from '../types/map.types';

/**
 * هوك لإدارة حالة الخرائط ووظائف الموقع الجغرافي
 * يعزل التعامل مع خدمات الخرائط في مكان واحد
 */
export const useMaps = () => {
  const [currentLocation, setCurrentLocation] = useState<Coordinates | null>(null);
  const [locationError, setLocationError] = useState<string | null>(null);
  const [isLocationLoading, setIsLocationLoading] = useState(false);
  const locationWatchId = useRef<number | null>(null);

  /**
   * الحصول على الموقع الحالي للمستخدم
   */
  const getCurrentLocation = useCallback(async (highAccuracy: boolean = true) => {
    setIsLocationLoading(true);
    setLocationError(null);
    
    try {
      const location = await MapsService.getCurrentLocation(highAccuracy);
      setCurrentLocation(location);
      return location;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'خطأ في تحديد الموقع';
      setLocationError(errorMessage);
      throw error;
    } finally {
      setIsLocationLoading(false);
    }
  }, []);

  /**
   * بدء مراقبة الموقع الحالي
   */
  const startLocationWatch = useCallback((
    callback?: (coordinates: Coordinates) => void,
    options?: {
      highAccuracy?: boolean;
      timeout?: number;
      maximumAge?: number;
    }
  ) => {
    try {
      if (locationWatchId.current) {
        MapsService.clearLocationWatch(locationWatchId.current);
      }

      locationWatchId.current = MapsService.watchLocation(
        (coordinates) => {
          setCurrentLocation(coordinates);
          callback?.(coordinates);
        },
        options
      );
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'خطأ في مراقبة الموقع';
      setLocationError(errorMessage);
    }
  }, []);

  /**
   * إيقاف مراقبة الموقع
   */
  const stopLocationWatch = useCallback(() => {
    if (locationWatchId.current) {
      MapsService.clearLocationWatch(locationWatchId.current);
      locationWatchId.current = null;
    }
  }, []);

  /**
   * تنظيف الموارد عند إلغاء المكون
   */
  useEffect(() => {
    return () => {
      stopLocationWatch();
    };
  }, [stopLocationWatch]);

  /**
   * حساب المسافة بين نقطتين
   */
  const calculateDistance = useCallback((start: Coordinates, end: Coordinates): number => {
    return MapsService.calculateDistance(start, end);
  }, []);

  /**
   * حساب حدود المنطقة للعلامات
   */
  const calculateBounds = useCallback((markers: MapMarker[]): GeoBounds | null => {
    const coordinates = markers
      .filter(marker => marker.coordinates)
      .map(marker => marker.coordinates!);
    
    return MapsService.calculateBounds(coordinates);
  }, []);

  return {
    // حالة الموقع الحالي
    currentLocation,
    locationError,
    isLocationLoading,
    
    // وظائف الموقع
    getCurrentLocation,
    startLocationWatch,
    stopLocationWatch,
    
    // وظائف الحسابات
    calculateDistance,
    calculateBounds
  };
};

/**
 * هوك للبحث الجغرافي
 */
export const useGeocoding = () => {
  /**
   * البحث الجغرافي بالنص
   */
  const geocodeQuery = useMutation<GeocodingResult[], Error, GeocodingOptions>({
    mutationFn: (options) => MapsService.geocodeAddress(options),
  });

  /**
   * البحث العكسي بالإحداثيات
   */
  const reverseGeocodeQuery = useMutation<GeocodingResult | null, Error, {
    coordinates: Coordinates;
    language?: string;
  }>({
    mutationFn: ({ coordinates, language }) => MapsService.reverseGeocode(coordinates, language),
  });

  /**
   * البحث بالنص
   */
  const searchByText = useCallback(async (
    query: string,
    options?: Partial<GeocodingOptions>
  ): Promise<GeocodingResult[]> => {
    return geocodeQuery.mutateAsync({
      query,
      ...options
    });
  }, [geocodeQuery]);

  /**
   * البحث العكسي
   */
  const searchByCoordinates = useCallback(async (
    coordinates: Coordinates,
    language?: string
  ): Promise<GeocodingResult | null> => {
    return reverseGeocodeQuery.mutateAsync({ coordinates, language });
  }, [reverseGeocodeQuery]);

  return {
    // حالات البحث
    isSearching: geocodeQuery.isPending || reverseGeocodeQuery.isPending,
    searchError: geocodeQuery.error || reverseGeocodeQuery.error,
    
    // وظائف البحث
    searchByText,
    searchByCoordinates,
    
    // إعادة تعيين الحالة
    resetSearch: () => {
      geocodeQuery.reset();
      reverseGeocodeQuery.reset();
    }
  };
};

/**
 * هوك لحساب المسارات
 */
export const useRouting = () => {
  /**
   * حساب المسار
   */
  const routeQuery = useMutation<RouteResult, Error, RouteOptions>({
    mutationFn: (options) => MapsService.calculateRoute(options),
  });

  /**
   * حساب المسار بين نقطتين
   */
  const calculateRoute = useCallback(async (
    start: Coordinates,
    end: Coordinates,
    options?: Partial<RouteOptions>
  ): Promise<RouteResult> => {
    return routeQuery.mutateAsync({
      start,
      end,
      ...options
    });
  }, [routeQuery]);

  return {
    // حالة حساب المسار
    isCalculating: routeQuery.isPending,
    routeError: routeQuery.error,
    routeData: routeQuery.data,
    
    // وظائف المسار
    calculateRoute,
    
    // إعادة تعيين
    resetRoute: () => routeQuery.reset()
  };
};

/**
 * هوك لإدارة العلامات على الخريطة
 */
export const useMapMarkers = (initialMarkers: MapMarker[] = []) => {
  const [markers, setMarkers] = useState<MapMarker[]>(initialMarkers);
  const [selectedMarker, setSelectedMarker] = useState<MapMarker | null>(null);
  const [hoveredMarker, setHoveredMarker] = useState<MapMarker | null>(null);

  /**
   * إضافة علامة جديدة
   */
  const addMarker = useCallback((marker: MapMarker) => {
    setMarkers(prev => [...prev, marker]);
  }, []);

  /**
   * إزالة علامة
   */
  const removeMarker = useCallback((markerId: string) => {
    setMarkers(prev => prev.filter(m => m.id !== markerId));
    if (selectedMarker?.id === markerId) {
      setSelectedMarker(null);
    }
    if (hoveredMarker?.id === markerId) {
      setHoveredMarker(null);
    }
  }, [selectedMarker, hoveredMarker]);

  /**
   * تحديث علامة موجودة
   */
  const updateMarker = useCallback((markerId: string, updates: Partial<MapMarker>) => {
    setMarkers(prev => prev.map(marker => 
      marker.id === markerId ? { ...marker, ...updates } : marker
    ));
    
    if (selectedMarker?.id === markerId) {
      setSelectedMarker(prev => prev ? { ...prev, ...updates } : null);
    }
  }, [selectedMarker]);

  /**
   * مسح جميع العلامات
   */
  const clearMarkers = useCallback(() => {
    setMarkers([]);
    setSelectedMarker(null);
    setHoveredMarker(null);
  }, []);

  /**
   * تحديد علامة
   */
  const selectMarker = useCallback((marker: MapMarker | null) => {
    setSelectedMarker(marker);
  }, []);

  /**
   * تمرير الماوس على علامة
   */
  const hoverMarker = useCallback((marker: MapMarker | null) => {
    setHoveredMarker(marker);
  }, []);

  /**
   * فلترة العلامات
   */
  const filterMarkers = useCallback((predicate: (marker: MapMarker) => boolean): MapMarker[] => {
    return markers.filter(predicate);
  }, [markers]);

  /**
   * البحث في العلامات
   */
  const searchMarkers = useCallback((query: string): MapMarker[] => {
    const lowercaseQuery = query.toLowerCase();
    return markers.filter(marker => 
      marker.name.toLowerCase().includes(lowercaseQuery) ||
      marker.address?.toLowerCase().includes(lowercaseQuery) ||
      marker.description?.toLowerCase().includes(lowercaseQuery)
    );
  }, [markers]);

  return {
    // البيانات
    markers,
    selectedMarker,
    hoveredMarker,
    
    // وظائف الإدارة
    addMarker,
    removeMarker,
    updateMarker,
    clearMarkers,
    selectMarker,
    hoverMarker,
    
    // وظائف البحث والفلترة
    filterMarkers,
    searchMarkers,
    
    // إحصائيات
    markersCount: markers.length,
    selectedMarkersCount: markers.filter(m => m.isSelected).length
  };
};