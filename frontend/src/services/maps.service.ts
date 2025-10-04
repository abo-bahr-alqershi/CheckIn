import type {
  Coordinates,
  GeocodingOptions,
  GeocodingResult,
  RouteOptions,
  RouteResult,
  GeoBounds
} from '../types/map.types';

/**
 * خدمة الخرائط والمواقع الجغرافية باستخدام OpenStreetMap
 * تتوافق مع طبقة الخدمات الموجودة في التطبيق
 */
export class MapsService {
  private static readonly NOMINATIM_BASE_URL = 'https://nominatim.openstreetmap.org';
  private static readonly ROUTING_BASE_URL = 'https://router.project-osrm.org';

  /**
   * البحث الجغرافي - تحويل النص إلى إحداثيات
   * @param options خيارات البحث
   * @returns نتائج البحث
   */
  static async geocodeAddress(options: GeocodingOptions): Promise<GeocodingResult[]> {
    try {
      const params = new URLSearchParams({
        q: options.query,
        format: 'jsonv2',
        addressdetails: '1',
        limit: (options.limit || 5).toString(),
        'accept-language': options.language || 'ar,en'
      });

      // إضافة حدود البحث إن وجدت
      if (options.bounds) {
        const { northeast, southwest } = options.bounds;
        params.append('viewbox', `${southwest.longitude},${southwest.latitude},${northeast.longitude},${northeast.latitude}`);
        params.append('bounded', '1');
      }

      // إضافة البلد إن وجد
      if (options.country) {
        params.append('countrycodes', options.country);
      }

      const response = await fetch(`${this.NOMINATIM_BASE_URL}/search?${params}`);
      
      if (!response.ok) {
        throw new Error(`خطأ في البحث الجغرافي: ${response.status}`);
      }

      const data = await response.json();

      return data.map((item: any): GeocodingResult => ({
        displayName: item.display_name,
        coordinates: {
          latitude: parseFloat(item.lat),
          longitude: parseFloat(item.lon)
        },
        bounds: item.boundingbox ? {
          northeast: {
            latitude: parseFloat(item.boundingbox[1]),
            longitude: parseFloat(item.boundingbox[3])
          },
          southwest: {
            latitude: parseFloat(item.boundingbox[0]),
            longitude: parseFloat(item.boundingbox[2])
          }
        } : undefined,
        type: item.type || 'unknown',
        importance: parseFloat(item.importance || '0'),
        details: {
          country: item.address?.country,
          state: item.address?.state,
          city: item.address?.city || item.address?.town || item.address?.village,
          district: item.address?.suburb || item.address?.neighbourhood,
          street: item.address?.road,
          houseNumber: item.address?.house_number,
          postcode: item.address?.postcode
        }
      }));
    } catch (error) {
      console.error('خطأ في البحث الجغرافي:', error);
      throw new Error('فشل في البحث الجغرافي. تحقق من الاتصال بالإنترنت.');
    }
  }

  /**
   * البحث العكسي - تحويل الإحداثيات إلى عنوان
   * @param coordinates الإحداثيات
   * @param language اللغة المطلوبة
   * @returns العنوان
   */
  static async reverseGeocode(coordinates: Coordinates, language: string = 'ar,en'): Promise<GeocodingResult | null> {
    try {
      const params = new URLSearchParams({
        lat: coordinates.latitude.toString(),
        lon: coordinates.longitude.toString(),
        format: 'jsonv2',
        addressdetails: '1',
        'accept-language': language
      });

      const response = await fetch(`${this.NOMINATIM_BASE_URL}/reverse?${params}`);
      
      if (!response.ok) {
        throw new Error(`خطأ في البحث العكسي: ${response.status}`);
      }

      const data = await response.json();

      if (!data || data.error) {
        return null;
      }

      return {
        displayName: data.display_name,
        coordinates: {
          latitude: parseFloat(data.lat),
          longitude: parseFloat(data.lon)
        },
        bounds: data.boundingbox ? {
          northeast: {
            latitude: parseFloat(data.boundingbox[1]),
            longitude: parseFloat(data.boundingbox[3])
          },
          southwest: {
            latitude: parseFloat(data.boundingbox[0]),
            longitude: parseFloat(data.boundingbox[2])
          }
        } : undefined,
        type: data.type || 'unknown',
        importance: parseFloat(data.importance || '0'),
        details: {
          country: data.address?.country,
          state: data.address?.state,
          city: data.address?.city || data.address?.town || data.address?.village,
          district: data.address?.suburb || data.address?.neighbourhood,
          street: data.address?.road,
          houseNumber: data.address?.house_number,
          postcode: data.address?.postcode
        }
      };
    } catch (error) {
      console.error('خطأ في البحث العكسي:', error);
      throw new Error('فشل في تحديد العنوان. تحقق من الاتصال بالإنترنت.');
    }
  }

  /**
   * حساب المسار بين نقطتين
   * @param options خيارات المسار
   * @returns تفاصيل المسار
   */
  static async calculateRoute(options: RouteOptions): Promise<RouteResult> {
    try {
      const { start, end, transportMode = 'driving' } = options;
      
      // تحويل وسائل النقل إلى تنسيق OSRM
      const profile = transportMode === 'walking' ? 'foot' : 
                     transportMode === 'cycling' ? 'bike' : 'car';

      const url = `${this.ROUTING_BASE_URL}/route/v1/${profile}/${start.longitude},${start.latitude};${end.longitude},${end.latitude}?overview=full&steps=true&geometries=geojson`;

      const response = await fetch(url);
      
      if (!response.ok) {
        throw new Error(`خطأ في حساب المسار: ${response.status}`);
      }

      const data = await response.json();

      if (data.code !== 'Ok' || !data.routes || data.routes.length === 0) {
        throw new Error('لم يتم العثور على مسار مناسب');
      }

      const route = data.routes[0];
      const leg = route.legs[0];

      return {
        distance: Math.round(route.distance / 1000 * 100) / 100, // كيلومتر
        duration: Math.round(route.duration / 60), // دقيقة
        geometry: route.geometry.coordinates.map(([lng, lat]: [number, number]) => ({
          latitude: lat,
          longitude: lng
        })),
        steps: leg.steps.map((step: any) => ({
          instruction: this.translateInstruction(step.maneuver.type, options.language),
          distance: Math.round(step.distance),
          duration: Math.round(step.duration / 60),
          startLocation: {
            latitude: step.maneuver.location[1],
            longitude: step.maneuver.location[0]
          },
          endLocation: {
            latitude: step.geometry.coordinates[step.geometry.coordinates.length - 1][1],
            longitude: step.geometry.coordinates[step.geometry.coordinates.length - 1][0]
          },
          maneuver: step.maneuver.type
        })),
        summary: {
          totalDistance: `${Math.round(route.distance / 1000 * 100) / 100} كم`,
          totalDuration: this.formatDuration(Math.round(route.duration / 60)),
          transportMode: this.translateTransportMode(transportMode)
        }
      };
    } catch (error) {
      console.error('خطأ في حساب المسار:', error);
      throw new Error('فشل في حساب المسار. تحقق من الاتصال بالإنترنت.');
    }
  }

  /**
   * حساب المسافة المباشرة بين نقطتين (Haversine formula)
   * @param start نقطة البداية
   * @param end نقطة النهاية
   * @returns المسافة بالكيلومترات
   */
  static calculateDistance(start: Coordinates, end: Coordinates): number {
    const R = 6371; // نصف قطر الأرض بالكيلومترات
    const dLat = this.toRadians(end.latitude - start.latitude);
    const dLng = this.toRadians(end.longitude - start.longitude);
    
    const a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
              Math.cos(this.toRadians(start.latitude)) * Math.cos(this.toRadians(end.latitude)) *
              Math.sin(dLng / 2) * Math.sin(dLng / 2);
    
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    
    return Math.round(R * c * 100) / 100;
  }

  /**
   * الحصول على الموقع الحالي للمستخدم
   * @param highAccuracy هل نريد دقة عالية
   * @returns الإحداثيات الحالية
   */
  static async getCurrentLocation(highAccuracy: boolean = true): Promise<Coordinates> {
    return new Promise((resolve, reject) => {
      if (!navigator.geolocation) {
        reject(new Error('خدمة تحديد الموقع غير مدعومة في هذا المتصفح'));
        return;
      }

      navigator.geolocation.getCurrentPosition(
        (position) => {
          resolve({
            latitude: position.coords.latitude,
            longitude: position.coords.longitude
          });
        },
        (error) => {
          let message = 'فشل في تحديد الموقع الحالي';
          switch (error.code) {
            case error.PERMISSION_DENIED:
              message = 'تم رفض الإذن للوصول إلى الموقع';
              break;
            case error.POSITION_UNAVAILABLE:
              message = 'معلومات الموقع غير متاحة';
              break;
            case error.TIMEOUT:
              message = 'انتهت مهلة طلب الموقع';
              break;
          }
          reject(new Error(message));
        },
        {
          enableHighAccuracy: highAccuracy,
          timeout: 10000,
          maximumAge: 300000 // 5 دقائق
        }
      );
    });
  }

  /**
   * مراقبة الموقع الحالي
   * @param callback دالة استدعاء عند تغيير الموقع
   * @param options خيارات المراقبة
   * @returns معرف المراقبة لإلغائها لاحقاً
   */
  static watchLocation(
    callback: (coordinates: Coordinates) => void,
    options: {
      highAccuracy?: boolean;
      timeout?: number;
      maximumAge?: number;
    } = {}
  ): number {
    if (!navigator.geolocation) {
      throw new Error('خدمة تحديد الموقع غير مدعومة في هذا المتصفح');
    }

    return navigator.geolocation.watchPosition(
      (position) => {
        callback({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude
        });
      },
      (error) => {
        console.error('خطأ في مراقبة الموقع:', error);
      },
      {
        enableHighAccuracy: options.highAccuracy ?? true,
        timeout: options.timeout ?? 10000,
        maximumAge: options.maximumAge ?? 60000
      }
    );
  }

  /**
   * إيقاف مراقبة الموقع
   * @param watchId معرف المراقبة
   */
  static clearLocationWatch(watchId: number): void {
    navigator.geolocation.clearWatch(watchId);
  }

  /**
   * حساب حدود المنطقة التي تحتوي على جميع النقاط
   * @param coordinates مصفوفة الإحداثيات
   * @returns حدود المنطقة
   */
  static calculateBounds(coordinates: Coordinates[]): GeoBounds | null {
    if (coordinates.length === 0) return null;

    let minLat = coordinates[0].latitude;
    let maxLat = coordinates[0].latitude;
    let minLng = coordinates[0].longitude;
    let maxLng = coordinates[0].longitude;

    coordinates.forEach(coord => {
      minLat = Math.min(minLat, coord.latitude);
      maxLat = Math.max(maxLat, coord.latitude);
      minLng = Math.min(minLng, coord.longitude);
      maxLng = Math.max(maxLng, coord.longitude);
    });

    // إضافة هامش صغير
    const latPadding = (maxLat - minLat) * 0.1;
    const lngPadding = (maxLng - minLng) * 0.1;

    return {
      northeast: {
        latitude: maxLat + latPadding,
        longitude: maxLng + lngPadding
      },
      southwest: {
        latitude: minLat - latPadding,
        longitude: minLng - lngPadding
      }
    };
  }

  // الدوال المساعدة الخاصة

  private static toRadians(degrees: number): number {
    return degrees * (Math.PI / 180);
  }

  private static translateInstruction(maneuverType: string, language?: string): string {
    const translations: Record<string, string> = {
      'turn-straight': 'استمر مباشرة',
      'turn-slight-right': 'انعطف يميناً قليلاً',
      'turn-right': 'انعطف يميناً',
      'turn-sharp-right': 'انعطف يميناً بشدة',
      'turn-slight-left': 'انعطف يساراً قليلاً',
      'turn-left': 'انعطف يساراً',
      'turn-sharp-left': 'انعطف يساراً بشدة',
      'uturn': 'اعكس الاتجاه',
      'arrive': 'وصلت إلى الوجهة',
      'depart': 'ابدأ الرحلة',
      'merge': 'ادمج مع الطريق الرئيسي',
      'on-ramp': 'ادخل إلى الطريق السريع',
      'off-ramp': 'اخرج من الطريق السريع',
      'fork': 'خذ التفرع',
      'roundabout': 'ادخل الدوار'
    };

    return translations[maneuverType] || 'تابع المسار';
  }

  private static translateTransportMode(mode: string): string {
    const translations: Record<string, string> = {
      'driving': 'بالسيارة',
      'walking': 'سيراً على الأقدام',
      'cycling': 'بالدراجة',
      'transit': 'بالمواصلات العامة'
    };

    return translations[mode] || mode;
  }

  private static formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes} دقيقة`;
    }
    
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    
    if (remainingMinutes === 0) {
      return `${hours} ساعة`;
    }
    
    return `${hours} ساعة و ${remainingMinutes} دقيقة`;
  }
}