import 'package:dio/dio.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exceptions.dart';
import '../../../../core/models/paginated_result.dart';
import '../models/city_model.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/utils/video_utils.dart';

/// 🌐 Remote Data Source للمدن
abstract class CitiesRemoteDataSource {
  /// الحصول على جميع المدن
  Future<List<CityModel>> getCities();
  
  /// حفظ قائمة المدن (إضافة أو تحديث)
  Future<bool> saveCities(List<CityModel> cities);
  
  /// إضافة مدينة جديدة
  Future<String> createCity(CityModel city);
  
  /// تحديث مدينة موجودة
  Future<bool> updateCity(String oldName, CityModel city);
  
  /// حذف مدينة
  Future<bool> deleteCity(String name);
  
  /// البحث في المدن
  Future<List<CityModel>> searchCities(String query);
  
  /// الحصول على إحصائيات المدن
  Future<Map<String, dynamic>> getCitiesStatistics();
  
  /// رفع صورة للمدينة
  Future<String> uploadCityImage(String cityName, String imagePath, {ProgressCallback? onSendProgress});
  
  /// حذف صورة من المدينة
  Future<bool> deleteCityImage(String imageUrl);
  
  /// الحصول على المدن بصفحات
  Future<PaginatedResult<CityModel>> getCitiesPaginated({
    int? pageNumber,
    int? pageSize,
    String? search,
    String? country,
    bool? isActive,
  });
}

class CitiesRemoteDataSourceImpl implements CitiesRemoteDataSource {
  final ApiClient apiClient;
  
  /// 🔗 المسار الأساسي لـ API المدن
  static const String _basePath = '/api/admin/system-settings/cities';
  // لا يوجد CitiesController على الـ backend؛ نُبقي فقط على مسارات system-settings
  static const String _adminPath = '/api/admin/cities';
  static const String _imagesPath = '/api/images';

  CitiesRemoteDataSourceImpl({required this.apiClient});

  /// 📋 الحصول على جميع المدن
  @override
  Future<List<CityModel>> getCities() async {
    try {
      final response = await apiClient.get(_basePath);
      
      if (response.data['success'] == true) {
        final List<dynamic> data = response.data['data'] ?? [];
        return data.map((json) => CityModel.fromJson(json)).toList();
      }
      
      throw ApiException(
        message: response.data['message'] ?? 'Failed to fetch cities',
      );
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// 💾 حفظ قائمة المدن
  @override
  Future<bool> saveCities(List<CityModel> cities) async {
    try {
      final citiesJson = cities.map((city) => city.toJson()).toList();
      
      final response = await apiClient.put(
        _basePath,
        data: citiesJson,
      );
      
      if (response.data['success'] == true) {
        return response.data['data'] ?? false;
      }
      
      throw ApiException(
        message: response.data['message'] ?? 'Failed to save cities',
      );
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// ➕ إضافة مدينة جديدة
  @override
  Future<String> createCity(CityModel city) async {
    try {
      // لا يوجد مسار لإنشاء مدينة منفردة؛ ندمجها ضمن القائمة ونحفظ عبر PUT
      final existing = await getCities();
      final updated = [...existing, city];
      final ok = await saveCities(updated);
      if (ok) {
        return city.name;
        }
      throw ApiException(message: 'Failed to create city');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// ✏️ تحديث مدينة موجودة
  @override
  Future<bool> updateCity(String oldName, CityModel city) async {
    try {
      // تحديث عبر جلب القائمة وتعديل العنصر ثم PUT للقائمة كاملة
      final existing = await getCities();
      final idx = existing.indexWhere((c) => c.name == oldName);
      if (idx == -1) throw ApiException(message: 'City not found');
      final List<CityModel> updated = List.of(existing);
      updated[idx] = city;
      return await saveCities(updated);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// 🗑️ حذف مدينة
  @override
  Future<bool> deleteCity(String name) async {
    try {
      final response = await apiClient.delete('$_basePath/${Uri.encodeComponent(name)}');
      if (response.data is Map<String, dynamic>) {
        final map = response.data as Map<String, dynamic>;
        if (map['success'] == true || map['isSuccess'] == true) return true;
        // Surface backend reason if conflict
        if (response.statusCode == 409 || map['errorCode'] == 'CITY_DELETE_CONFLICT') {
          throw ApiException(message: map['message'] ?? 'Deletion conflict');
        }
      }
      return response.statusCode == 200 || response.statusCode == 204;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// 🔍 البحث في المدن
  @override
  Future<List<CityModel>> searchCities(String query) async {
    try {
      final all = await getCities();
      final q = query.toLowerCase();
      return all.where((c) => c.name.toLowerCase().contains(q) || c.country.toLowerCase().contains(q)).toList();
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// 📊 الحصول على إحصائيات المدن
  @override
  Future<Map<String, dynamic>> getCitiesStatistics() async {
    try {
      final all = await getCities();
      final total = all.length;
      final active = all.where((c) => c.isActive ?? true).length;
      final byCountry = <String, int>{};
      for (final c in all) {
        byCountry[c.country] = (byCountry[c.country] ?? 0) + 1;
      }
      return {
        'total': total,
        'active': active,
        'byCountry': byCountry,
      };
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// 📤 رفع صورة للمدينة
  @override
  Future<String> uploadCityImage(String cityName, String imagePath, {ProgressCallback? onSendProgress}) async {
    try {
      // Build multipart form data explicitly to avoid filename inference issues
      final formData = FormData();
      formData.files.add(MapEntry(
        'file', await MultipartFile.fromFile(imagePath),
      ));
      formData.fields
        ..add(const MapEntry('category', 'Gallery'))
        ..add(MapEntry('cityName', cityName));

      // If uploading a video, attach a generated poster too
      if (AppConstants.isVideoFile(imagePath)) {
        final posterPath = await VideoUtils.generateVideoThumbnail(imagePath);
        if (posterPath != null) {
          formData.files.add(MapEntry(
            'videoThumbnail', await MultipartFile.fromFile(posterPath),
          ));
        }
      }

      final response = await apiClient.post(
        '$_imagesPath/upload',
        data: formData,
        options: Options(
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        ),
        onSendProgress: onSendProgress,
      );
      
      if (response.data is Map<String, dynamic>) {
        final map = response.data as Map<String, dynamic>;
        if (map['success'] == true) {
          final data = map['image'] ?? map['data'];
          if (data is Map && data['url'] != null) {
            return data['url'] as String;
          }
        }
      }
      
      throw ApiException(
        message: response.data['message'] ?? 'Failed to upload image',
      );
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// 🗑️ حذف صورة من المدينة
  @override
  Future<bool> deleteCityImage(String imageUrl) async {
    try {
      // محاولة إيجاد صورة عبر GET /api/images ثم حذفها عبر ID
      final listResponse = await apiClient.get(
        _imagesPath,
        queryParameters: {
          'search': imageUrl,
          'page': 1,
          'limit': 100,
        },
      );
      if (listResponse.data is Map<String, dynamic>) {
        final map = listResponse.data as Map<String, dynamic>;
        final List<dynamic> images = (map['images'] as List?) ?? (map['items'] as List?) ?? const [];
        final match = images.cast<Map<String, dynamic>?>().firstWhere(
          (m) => m != null && (m!['url'] == imageUrl),
          orElse: () => null,
        );
        if (match != null && match['id'] != null) {
          final id = match['id'].toString();
          final del = await apiClient.delete('$_imagesPath/$id');
          return del.data is Map && del.data['success'] == true || del.statusCode == 204;
        }
      }
      return false;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// 📑 الحصول على المدن بصفحات
  @override
  Future<PaginatedResult<CityModel>> getCitiesPaginated({
    int? pageNumber,
    int? pageSize,
    String? search,
    String? country,
    bool? isActive,
  }) async {
    try {
      final all = await getCities();
      List<CityModel> filtered = all;
      if (search != null && search.isNotEmpty) {
        final s = search.toLowerCase();
        filtered = filtered.where((c) => c.name.toLowerCase().contains(s) || c.country.toLowerCase().contains(s)).toList();
      }
      if (country != null && country.isNotEmpty) {
        final c = country.toLowerCase();
        filtered = filtered.where((x) => x.country.toLowerCase() == c).toList();
      }
      if (isActive != null) {
        filtered = filtered.where((x) => (x.isActive ?? true) == isActive).toList();
      }
      final pn = (pageNumber ?? 1) < 1 ? 1 : (pageNumber ?? 1);
      final ps = (pageSize ?? 20) <= 0 ? 20 : (pageSize ?? 20);
      final start = (pn - 1) * ps;
      final end = (start + ps) > filtered.length ? filtered.length : (start + ps);
      final pageItems = start < filtered.length ? filtered.sublist(start, end) : <CityModel>[];
      return PaginatedResult(
        items: pageItems,
        pageNumber: pn,
        pageSize: ps,
        totalCount: filtered.length,
      );
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }
}