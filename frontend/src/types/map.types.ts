/**
 * أنواع البيانات المتعلقة بالخرائط ونظام تحديد المواقع الجغرافية
 */

/**
 * إحداثيات النقطة الجغرافية
 */
export interface Coordinates {
  /** خط العرض */
  latitude: number;
  /** خط الطول */
  longitude: number;
}

/**
 * حدود منطقة جغرافية (Bounding Box)
 */
export interface GeoBounds {
  /** الإحداثيات الشمالية الشرقية */
  northeast: Coordinates;
  /** الإحداثيات الجنوبية الغربية */
  southwest: Coordinates;
}

/**
 * عنصر قابل لعرضه على الخريطة
 */
export interface MapItem {
  /** معرف العنصر */
  id: string;
  /** الإحداثيات الجغرافية */
  coordinates?: Coordinates;
  /** اسم العنصر */
  name: string;
  /** العنوان */
  address?: string;
  /** وصف إضافي */
  description?: string;
  /** أيقونة مخصصة */
  icon?: string;
  /** لون العلامة */
  color?: string;
}

/**
 * علامة خريطة (Marker) مع معلومات إضافية
 */
export interface MapMarker extends MapItem {
  /** نوع العلامة */
  type: 'property' | 'unit' | 'amenity' | 'custom';
  /** هل العلامة مختارة */
  isSelected?: boolean;
  /** هل العلامة مميزة */
  isFeatured?: boolean;
  /** السعر (للكيانات) */
  price?: {
    amount: number;
    currency: string;
  };
  /** التقييم (للكيانات) */
  rating?: number;
  /** حالة التوفر */
  isAvailable?: boolean;
}

/**
 * خصائص مكون الخريطة
 */
export interface MapViewProps {
  /** العلامات لعرضها على الخريطة */
  markers: MapMarker[];
  /** الإحداثيات المركزية للخريطة */
  center?: Coordinates;
  /** مستوى التكبير */
  zoom?: number;
  /** ارتفاع الخريطة */
  height?: string | number;
  /** عرض الخريطة */
  width?: string | number;
  /** هل الخريطة قابلة للتفاعل */
  interactive?: boolean;
  /** دالة عند النقر على علامة */
  onMarkerClick?: (marker: MapMarker) => void;
  /** دالة عند تغيير الموضع */
  onLocationChange?: (coordinates: Coordinates) => void;
  /** دالة عند تغيير التكبير */
  onZoomChange?: (zoom: number) => void;
  /** هل يظهر موقع المستخدم الحالي */
  showCurrentLocation?: boolean;
  /** هل يظهر عناصر التحكم */
  showControls?: boolean;
  /** رسالة عند عدم وجود علامات */
  emptyMessage?: string;
  /** هل يجمع العلامات المتقاربة */
  enableClustering?: boolean;
  /** نمط الخريطة */
  mapStyle?: 'default' | 'satellite' | 'terrain';
}

/**
 * إعدادات البحث الجغرافي
 */
export interface GeocodingOptions {
  /** النص المراد البحث عنه */
  query: string;
  /** حدود المنطقة للبحث داخلها */
  bounds?: GeoBounds;
  /** البلد للبحث فيه */
  country?: string;
  /** اللغة المفضلة للنتائج */
  language?: string;
  /** عدد النتائج المطلوبة */
  limit?: number;
}

/**
 * نتيجة البحث الجغرافي
 */
export interface GeocodingResult {
  /** النص الكامل للعنوان */
  displayName: string;
  /** الإحداثيات */
  coordinates: Coordinates;
  /** حدود المنطقة */
  bounds?: GeoBounds;
  /** نوع المكان */
  type: string;
  /** أهمية المكان (0-1) */
  importance: number;
  /** معلومات إضافية */
  details: {
    country?: string;
    state?: string;
    city?: string;
    district?: string;
    street?: string;
    houseNumber?: string;
    postcode?: string;
  };
}

/**
 * خيارات حساب المسافة والاتجاهات
 */
export interface RouteOptions {
  /** نقطة البداية */
  start: Coordinates;
  /** نقطة النهاية */
  end: Coordinates;
  /** وسيلة النقل */
  transportMode?: 'driving' | 'walking' | 'cycling' | 'transit';
  /** تجنب أنواع معينة من الطرق */
  avoid?: ('tolls' | 'highways' | 'ferries')[];
  /** اللغة للتعليمات */
  language?: string;
}

/**
 * نتيجة حساب المسار
 */
export interface RouteResult {
  /** المسافة بالكيلومترات */
  distance: number;
  /** المدة بالدقائق */
  duration: number;
  /** النقاط الجغرافية للمسار */
  geometry: Coordinates[];
  /** التعليمات خطوة بخطوة */
  steps: RouteStep[];
  /** معلومات إضافية */
  summary: {
    totalDistance: string;
    totalDuration: string;
    transportMode: string;
  };
}

/**
 * خطوة في المسار
 */
export interface RouteStep {
  /** الوصف */
  instruction: string;
  /** المسافة لهذه الخطوة */
  distance: number;
  /** المدة لهذه الخطوة */
  duration: number;
  /** نقطة البداية للخطوة */
  startLocation: Coordinates;
  /** نقطة النهاية للخطوة */
  endLocation: Coordinates;
  /** نوع المناورة */
  maneuver?: string;
}

/**
 * إعدادات تخصيص علامة الخريطة
 */
export interface MarkerStyle {
  /** لون الخلفية */
  backgroundColor?: string;
  /** لون النص */
  textColor?: string;
  /** الحجم */
  size?: 'small' | 'medium' | 'large';
  /** الشكل */
  shape?: 'circle' | 'square' | 'pin';
  /** الأيقونة */
  icon?: string;
  /** التأثيرات */
  effects?: {
    shadow?: boolean;
    glow?: boolean;
    pulse?: boolean;
  };
}

/**
 * حدث الخريطة
 */
export interface MapEvent {
  /** نوع الحدث */
  type: 'click' | 'drag' | 'zoom' | 'bounds_changed';
  /** الإحداثيات */
  coordinates?: Coordinates;
  /** مستوى التكبير */
  zoom?: number;
  /** حدود الرؤية الحالية */
  bounds?: GeoBounds;
  /** العلامة المرتبطة (إن وجدت) */
  marker?: MapMarker;
}

/**
 * إعدادات الخريطة المتقدمة
 */
export interface MapConfiguration {
  /** المزود (OpenStreetMap, Google, Mapbox...) */
  provider: 'osm' | 'google' | 'mapbox';
  /** مفتاح API (للخدمات المدفوعة) */
  apiKey?: string;
  /** رابط خادم البلاطات المخصص */
  tileServer?: string;
  /** إعدادات التجميع */
  clustering?: {
    enabled: boolean;
    maxClusterRadius: number;
    gridSize: number;
  };
  /** إعدادات التحكم */
  controls?: {
    zoom: boolean;
    fullscreen: boolean;
    layers: boolean;
    scale: boolean;
    attribution: boolean;
  };
  /** الحد الأقصى والأدنى للتكبير */
  zoomLimits?: {
    min: number;
    max: number;
  };
  /** قيود الحركة */
  restrictions?: {
    bounds?: GeoBounds;
    centerOnly?: boolean;
  };
}

/**
 * إحصائيات الخريطة
 */
export interface MapStats {
  /** عدد العلامات المعروضة */
  markersCount: number;
  /** عدد العلامات المختارة */
  selectedMarkersCount: number;
  /** متوسط الإحداثيات */
  center: Coordinates;
  /** مدى الرؤية الحالي */
  currentBounds: GeoBounds;
  /** مستوى التكبير الحالي */
  currentZoom: number;
}