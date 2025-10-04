import React, { useEffect, useRef, useState, useCallback } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap, useMapEvents } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import type { MapViewProps, MapMarker, Coordinates } from '../../types/map.types';

interface MapViewWithPaginationProps extends MapViewProps {
  pagination?: {
    current: number;
    total: number;
    pageSize: number;
    onChange: (page: number, pageSize: number) => void;
  };
}
import { useMaps, useGeocoding, useMapMarkers } from '../../hooks/useMaps';
import ActionButton from '../ui/ActionButton';

// إعداد أيقونات Leaflet الافتراضية
delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

/**
 * مكون أيقونة العلامة المخصصة
 */
const createCustomIcon = (marker: MapMarker): L.Icon => {
  const iconColor = marker.color || '#3B82F6';
  const iconSize = marker.type === 'property' ? 32 : 24;
  
  const svgIcon = `
    <svg width="${iconSize}" height="${iconSize}" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z" fill="${iconColor}" stroke="white" stroke-width="2"/>
      <circle cx="12" cy="9" r="3" fill="white"/>
      ${marker.icon ? `<text x="12" y="13" text-anchor="middle" fill="${iconColor}" font-size="8">${marker.icon}</text>` : ''}
    </svg>
  `;
  
  return new L.Icon({
    iconUrl: `data:image/svg+xml;base64,${btoa(svgIcon)}`,
    iconSize: [iconSize, iconSize],
    iconAnchor: [iconSize / 2, iconSize],
    popupAnchor: [0, -iconSize],
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
    shadowSize: [iconSize * 1.5, iconSize],
    shadowAnchor: [iconSize * 0.75, iconSize]
  });
};

/**
 * مكون التحكم في أحداث الخريطة
 */
interface MapEventsHandlerProps {
  onMapClick?: (coordinates: Coordinates) => void;
  onZoomChange?: (zoom: number) => void;
  onMoveEnd?: (center: Coordinates, bounds: any) => void;
}

const MapEventsHandler: React.FC<MapEventsHandlerProps> = ({ 
  onMapClick, 
  onZoomChange, 
  onMoveEnd 
}) => {
  const map = useMapEvents({
    click: (e) => {
      if (onMapClick) {
        onMapClick({
          latitude: e.latlng.lat,
          longitude: e.latlng.lng
        });
      }
    },
    zoomend: () => {
      if (onZoomChange) {
        onZoomChange(map.getZoom());
      }
    },
    moveend: () => {
      if (onMoveEnd) {
        const center = map.getCenter();
        const bounds = map.getBounds();
        onMoveEnd(
          { latitude: center.lat, longitude: center.lng },
          bounds
        );
      }
    }
  });
  
  return null;
};

/**
 * مكون التحكم في موضع الخريطة
 */
interface MapControllerProps {
  center?: Coordinates;
  zoom?: number;
  bounds?: any;
}

const MapController: React.FC<MapControllerProps> = ({ center, zoom, bounds }) => {
  const map = useMap();
  
  useEffect(() => {
    if (bounds) {
      map.fitBounds(bounds, { padding: [20, 20] });
    } else if (center) {
      map.setView([center.latitude, center.longitude], zoom || map.getZoom());
    }
  }, [map, center, zoom, bounds]);
  
  return null;
};

/**
 * شريط البحث الجغرافي
 */
interface MapSearchProps {
  onLocationSelect: (coordinates: Coordinates) => void;
  placeholder?: string;
}

const MapSearch: React.FC<MapSearchProps> = ({ onLocationSelect, placeholder = 'ابحث عن موقع...' }) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<any[]>([]);
  const [showResults, setShowResults] = useState(false);
  const { searchByText, isSearching } = useGeocoding();
  
  const handleSearch = useCallback(async (query: string) => {
    if (query.length < 3) {
      setSearchResults([]);
      setShowResults(false);
      return;
    }
    
    try {
      const results = await searchByText(query, {
        language: 'ar,en',
        limit: 5
      });
      setSearchResults(results);
      setShowResults(true);
    } catch (error) {
      console.error('خطأ في البحث:', error);
      setSearchResults([]);
      setShowResults(false);
    }
  }, [searchByText]);
  
  const handleResultSelect = (result: any) => {
    onLocationSelect(result.coordinates);
    setSearchQuery(result.displayName);
    setShowResults(false);
  };
  
  return (
    <div className="relative">
      <input
        type="text"
        value={searchQuery}
        onChange={(e) => {
          setSearchQuery(e.target.value);
          handleSearch(e.target.value);
        }}
        placeholder={placeholder}
        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-blue-500"
      />
      
      {showResults && searchResults.length > 0 && (
        <div className="absolute z-[9999] w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-60 overflow-y-auto">
          {searchResults.map((result, index) => (
            <button
              key={index}
              onClick={() => handleResultSelect(result)}
              className="w-full px-3 py-2 text-right hover:bg-gray-50 border-b border-gray-100 last:border-b-0"
            >
              <p className="font-medium text-gray-900 truncate">{result.displayName}</p>
              <p className="text-xs text-gray-500">{result.type}</p>
            </button>
          ))}
        </div>
      )}
    </div>
  );
};

/**
 * مكون الخريطة الرئيسي
 */
const MapView: React.FC<MapViewWithPaginationProps> = ({
  markers = [],
  center = { latitude: 15.279272, longitude: 44.227494 }, // صنعاء كموقع افتراضي
  zoom = 10,
  height = '500px',
  width = '100%',
  interactive = true,
  onMarkerClick,
  onLocationChange,
  onZoomChange,
  showCurrentLocation = true,
  showControls = true,
  emptyMessage = 'لا توجد مواقع لعرضها على الخريطة',
  enableClustering = false,
  mapStyle = 'default',
  pagination
}) => {
  const [mapCenter, setMapCenter] = useState<Coordinates>(center);
  const [mapZoom, setMapZoom] = useState(zoom);
  const [selectedMarker, setSelectedMarker] = useState<MapMarker | null>(null);
  const [showSearch, setShowSearch] = useState(false);
  const { currentLocation, getCurrentLocation, isLocationLoading } = useMaps();
  const { calculateBounds } = useMaps();
  
  // فلترة العلامات الصحيحة
  const validMarkers = markers.filter(marker => 
    marker.coordinates && 
    marker.coordinates.latitude !== 0 && 
    marker.coordinates.longitude !== 0
  );
  
  // اختيار خادم البلاطات حسب النمط
  const getTileLayerUrl = () => {
    switch (mapStyle) {
      case 'satellite':
        return 'https://{s}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png';
      case 'terrain':
        return 'https://{s}.tile.opentopomap.org/{z}/{x}/{y}.png';
      default:
        return 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
    }
  };
  
  // التنقل إلى الموقع الحالي
  const goToCurrentLocation = useCallback(async () => {
    try {
      const location = await getCurrentLocation();
      setMapCenter(location);
      setMapZoom(15);
      onLocationChange?.(location);
    } catch (error) {
      console.error('خطأ في تحديد الموقع:', error);
    }
  }, [getCurrentLocation, onLocationChange]);
  
  // عرض جميع العلامات
  const showAllMarkers = useCallback(() => {
    if (validMarkers.length > 0) {
      const bounds = calculateBounds(validMarkers);
      if (bounds) {
        // تحديث الخريطة لتناسب جميع العلامات
        const centerLat = (bounds.northeast.latitude + bounds.southwest.latitude) / 2;
        const centerLng = (bounds.northeast.longitude + bounds.southwest.longitude) / 2;
        setMapCenter({ latitude: centerLat, longitude: centerLng });
      }
    }
  }, [validMarkers, calculateBounds]);
  
  // معالجة النقر على العلامة
  const handleMarkerClick = useCallback((marker: MapMarker) => {
    setSelectedMarker(marker);
    onMarkerClick?.(marker);
  }, [onMarkerClick]);
  
  // معالجة النقر على الخريطة
  const handleMapClick = useCallback((coordinates: Coordinates) => {
    setSelectedMarker(null);
    onLocationChange?.(coordinates);
  }, [onLocationChange]);
  
  // معالجة تغيير مستوى التكبير
  const handleZoomChange = useCallback((newZoom: number) => {
    setMapZoom(newZoom);
    onZoomChange?.(newZoom);
  }, [onZoomChange]);
  
  // معالجة اختيار موقع من البحث
  const handleSearchLocationSelect = useCallback((coordinates: Coordinates) => {
    setMapCenter(coordinates);
    setMapZoom(15);
    setShowSearch(false);
    onLocationChange?.(coordinates);
  }, [onLocationChange]);
  
  if (validMarkers.length === 0) {
    return (
      <div className="bg-white rounded-lg shadow-sm border border-gray-200" style={{ height, width }}>
        <div className="flex flex-col items-center justify-center h-full text-center p-8">
          <div className="text-6xl mb-4">🗺️</div>
          <h3 className="text-lg font-medium text-gray-900 mb-2">لا توجد مواقع للعرض</h3>
          <p className="text-gray-500">{emptyMessage}</p>
        </div>
      </div>
    );
  }
  
  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden" style={{ width }}>
      {/* شريط البحث والتحكم */}
      {showControls && (
        <div className="p-4 bg-gray-50 border-b border-gray-200">
          <div className="flex items-center gap-3">
            {showSearch ? (
              <div className="flex-1">
                <MapSearch onLocationSelect={handleSearchLocationSelect} />
              </div>
            ) : (
              <div className="flex-1">
                <span className="text-sm text-gray-600">عدد المواقع: {validMarkers.length}</span>
              </div>
            )}
            
            <div className="flex gap-2">
              <ActionButton
                label={showSearch ? 'إخفاء البحث' : 'بحث'}
                variant="secondary"
                onClick={() => setShowSearch(!showSearch)}
              />
              
              {showCurrentLocation && (
                <ActionButton
                  label="موقعي"
                  variant="secondary"
                  onClick={goToCurrentLocation}
                />
              )}
              
              <ActionButton
                label="عرض الكل"
                variant="secondary"
                onClick={showAllMarkers}
              />
            </div>
          </div>
        </div>
      )}
      
      {/* الخريطة */}
      <div style={{ height, position: 'relative' }}>
        <MapContainer
          center={[mapCenter.latitude, mapCenter.longitude]}
          zoom={mapZoom}
          style={{ height: '100%', width: '100%' }}
          scrollWheelZoom={interactive}
          dragging={interactive}
          touchZoom={interactive}
          doubleClickZoom={interactive}
          boxZoom={interactive}
          keyboard={interactive}
          zoomControl={false}
        >
          <TileLayer
            url={getTileLayerUrl()}
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          />
          
          <MapController center={mapCenter} zoom={mapZoom} />
          
          <MapEventsHandler
            onMapClick={handleMapClick}
            onZoomChange={handleZoomChange}
          />
          
          {/* عرض العلامات */}
          {validMarkers.map((marker) => (
            <Marker
              key={marker.id}
              position={[marker.coordinates!.latitude, marker.coordinates!.longitude]}
              icon={createCustomIcon(marker)}
              eventHandlers={{
                click: () => handleMarkerClick(marker)
              }}
            >
              <Popup>
                <div className="p-2 min-w-48 text-right">
                  <h4 className="font-medium text-gray-900 mb-1">{marker.name}</h4>
                  {marker.address && (
                    <p className="text-sm text-gray-600 mb-2">{marker.address}</p>
                  )}
                  {marker.description && (
                    <p className="text-xs text-gray-500 mb-2">{marker.description}</p>
                  )}
                  {marker.price && (
                    <p className="text-sm font-medium text-blue-600">
                      {marker.price.amount} {marker.price.currency}
                    </p>
                  )}
                  {marker.rating && (
                    <p className="text-xs text-yellow-600">
                      {marker.rating}/5 تقييم
                    </p>
                  )}
                </div>
              </Popup>
            </Marker>
          ))}
          
          {/* عرض الموقع الحالي */}
          {currentLocation && showCurrentLocation && (
            <Marker
              position={[currentLocation.latitude, currentLocation.longitude]}
              icon={new L.Icon({
                iconUrl: 'data:image/svg+xml;base64,' + btoa(`
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <circle cx="12" cy="12" r="8" fill="#3B82F6" stroke="white" stroke-width="3"/>
                    <circle cx="12" cy="12" r="3" fill="white"/>
                  </svg>
                `),
                iconSize: [16, 16],
                iconAnchor: [8, 8]
              })}
            >
              <Popup>
                <div className="p-2 text-right">
                  <h4 className="font-medium text-gray-900">موقعك الحالي</h4>
                  <p className="text-xs text-gray-500">
                    {currentLocation.latitude.toFixed(6)}, {currentLocation.longitude.toFixed(6)}
                  </p>
                </div>
              </Popup>
            </Marker>
          )}
        </MapContainer>
      </div>
      
      {/* Pagination */}
      {pagination && (
        <div className="p-4 bg-gray-50 border-t border-gray-200">
          <div className="flex items-center justify-between">
            <div className="flex-1 flex justify-between sm:hidden">
              <button
                onClick={() => pagination.onChange(pagination.current - 1, pagination.pageSize)}
                disabled={pagination.current <= 1}
                className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                السابق
              </button>
              <button
                onClick={() => pagination.onChange(pagination.current + 1, pagination.pageSize)}
                disabled={pagination.current * pagination.pageSize >= pagination.total}
                className="mr-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                التالي
              </button>
            </div>
            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
              <div>
                <p className="text-sm text-gray-700">
                  عرض{' '}
                  <span className="font-medium">
                    {(pagination.current - 1) * pagination.pageSize + 1}
                  </span>{' '}
                  إلى{' '}
                  <span className="font-medium">
                    {Math.min(pagination.current * pagination.pageSize, pagination.total)}
                  </span>{' '}
                  من{' '}
                  <span className="font-medium">{pagination.total}</span> موقع
                </p>
              </div>
              <div>
                <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                  <button
                    onClick={() => pagination.onChange(pagination.current - 1, pagination.pageSize)}
                    disabled={pagination.current <= 1}
                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    →
                  </button>
                  {Array.from({ length: Math.ceil(pagination.total / pagination.pageSize) }, (_, i) => i + 1)
                    .slice(
                      Math.max(0, pagination.current - 3),
                      Math.min(Math.ceil(pagination.total / pagination.pageSize), pagination.current + 2)
                    )
                    .map((page) => (
                      <button
                        key={page}
                        onClick={() => pagination.onChange(page, pagination.pageSize)}
                        className={`relative inline-flex items-center px-4 py-2 border text-sm font-medium ${
                          page === pagination.current
                            ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                            : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                        }`}
                      >
                        {page}
                      </button>
                    ))}
                  <button
                    onClick={() => pagination.onChange(pagination.current + 1, pagination.pageSize)}
                    disabled={pagination.current * pagination.pageSize >= pagination.total}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    ←
                  </button>
                </nav>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default MapView;