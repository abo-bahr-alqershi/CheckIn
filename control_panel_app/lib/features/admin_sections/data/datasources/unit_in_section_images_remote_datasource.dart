// lib/features/admin_sections/data/datasources/unit_in_section_images_remote_datasource.dart

import 'package:dio/dio.dart';
import 'package:bookn_cp_app/core/network/api_client.dart';
import 'package:bookn_cp_app/core/error/exceptions.dart';
import 'package:bookn_cp_app/core/constants/app_constants.dart';
import 'package:bookn_cp_app/core/utils/video_utils.dart';
import '../models/section_image_model.dart';

abstract class UnitInSectionImagesRemoteDataSource {
  Future<SectionImageModel> uploadImage({
    String? unitInSectionId,
    String? tempKey,
    required String filePath,
    String? category,
    String? alt,
    bool isPrimary = false,
    int? order,
    List<String>? tags,
    ProgressCallback? onSendProgress,
  });

  Future<List<SectionImageModel>> getUnitInSectionImages(
    String? unitInSectionId, {
    String? tempKey,
  });

  Future<bool> updateImage(String imageId, Map<String, dynamic> data);
  Future<bool> deleteImage(String imageId);
  Future<bool> reorderImages(
    String? unitInSectionId,
    String? tempKey,
    List<String> imageIds,
  );
  Future<bool> setAsPrimaryImage(
    String? unitInSectionId,
    String? tempKey,
    String imageId,
  );
}

class UnitInSectionImagesRemoteDataSourceImpl
    implements UnitInSectionImagesRemoteDataSource {
  final ApiClient apiClient;
  static const String _baseEndpoint = '/api/admin/unit-in-section-images';

  UnitInSectionImagesRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<SectionImageModel> uploadImage({
    String? unitInSectionId,
    String? tempKey,
    required String filePath,
    String? category,
    String? alt,
    bool isPrimary = false,
    int? order,
    List<String>? tags,
    ProgressCallback? onSendProgress,
  }) async {
    try {
      String? posterPath;
      if (AppConstants.isVideoFile(filePath)) {
        posterPath = await VideoUtils.generateVideoThumbnail(filePath);
      }

      final normalizedUnitInSectionId = (unitInSectionId != null && unitInSectionId.trim().isNotEmpty)
          ? unitInSectionId
          : null;
      final normalizedTempKey = (tempKey != null && tempKey.trim().isNotEmpty) ? tempKey : null;
      final formData = FormData.fromMap({
        'file': await MultipartFile.fromFile(filePath),
        if (normalizedUnitInSectionId != null) 'unitInSectionId': normalizedUnitInSectionId
        else if (normalizedTempKey != null) 'tempKey': normalizedTempKey,
        if (category != null && category.trim().isNotEmpty) 'category': category,
        if (alt != null && alt.trim().isNotEmpty) 'alt': alt,
        'isPrimary': isPrimary,
        if (order != null) 'order': order,
        if (tags != null && tags.isNotEmpty) 'tags': tags.join(','),
        if (posterPath != null)
          'videoThumbnail': await MultipartFile.fromFile(posterPath),
      });

      final response = await apiClient.post(
        '$_baseEndpoint/upload',
        data: formData,
        options: Options(
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        ),
        onSendProgress: onSendProgress,
      );

      // Accept different response shapes similar to section_images_remote_datasource
      if (response.data is Map<String, dynamic>) {
        final map = response.data as Map<String, dynamic>;
        final dynamic data =
            map['data'] ?? map['item'] ?? map['image'] ?? map['payload'];
        if (map['success'] == true && data != null) {
          return SectionImageModel.fromJson(data);
        }
        // Sometimes the image is directly returned
        if (map['success'] == true && data == null && map.containsKey('id')) {
          return SectionImageModel.fromJson(map);
        }
      }
      throw const ServerException('Failed to upload image');
    } on DioException catch (e) {
      // Better error messages and connectivity handling
      final msg = e.response?.data is Map<String, dynamic>
          ? (e.response?.data['message'] as String?)
          : e.message;
      if (e.type == DioExceptionType.unknown &&
          e.error.toString().contains('SocketException')) {
        throw const ServerException(
            'لا يمكن الاتصال بالخادم. تحقق من الاتصال بالإنترنت أو إعدادات الخادم');
      }
      throw ServerException(msg ?? 'Failed to upload image');
    }
  }

  @override
  Future<List<SectionImageModel>> getUnitInSectionImages(
    String? unitInSectionId, {
    String? tempKey,
  }) async {
    try {
      final normalizedUnitInSectionId = (unitInSectionId != null && unitInSectionId.trim().isNotEmpty)
          ? unitInSectionId
          : null;
      final normalizedTempKey = (tempKey != null && tempKey.trim().isNotEmpty) ? tempKey : null;
      final qp = <String, dynamic>{
        if (normalizedUnitInSectionId != null) 'unitInSectionId': normalizedUnitInSectionId
        else if (normalizedTempKey != null) 'tempKey': normalizedTempKey,
        'sortBy': 'order',
        'sortOrder': 'asc',
      };

      final response = await apiClient.get(_baseEndpoint, queryParameters: qp);

      // Handle different response formats like section images
      if (response.data is Map<String, dynamic>) {
        final map = response.data as Map<String, dynamic>;
        if (map['success'] == true) {
          final dynamic imagesData =
              map['images'] ?? map['items'] ?? map['data'];
          if (imagesData is List) {
            return imagesData
                .map((json) => SectionImageModel.fromJson(json))
                .toList();
          } else if (imagesData is Map && imagesData['items'] is List) {
            final List<dynamic> items = imagesData['items'];
            return items
                .map((json) => SectionImageModel.fromJson(json))
                .toList();
          }
        }
        if (map['items'] is List) {
          final List<dynamic> items = map['items'];
          return items.map((json) => SectionImageModel.fromJson(json)).toList();
        }
      } else if (response.data is List) {
        return (response.data as List)
            .map((json) => SectionImageModel.fromJson(json))
            .toList();
      }

      return [];
    } on DioException catch (e) {
      if (e.type == DioExceptionType.unknown &&
          e.error.toString().contains('SocketException')) {
        throw const ServerException(
            'لا يمكن الاتصال بالخادم. تحقق من الاتصال بالإنترنت أو إعدادات الخادم');
      }
      final msg = e.response?.data is Map<String, dynamic>
          ? (e.response?.data['message'] as String?)
          : e.message;
      throw ServerException(msg ?? 'Failed to fetch images');
    }
  }

  @override
  Future<bool> updateImage(String imageId, Map<String, dynamic> data) async {
    try {
      final response = await apiClient.put(
        '$_baseEndpoint/$imageId',
        data: data,
      );

      if (response.statusCode == 204) return true;
      if (response.data is Map<String, dynamic>) {
        return (response.data as Map<String, dynamic>)['success'] == true;
      }
      return response.statusCode == 200;
    } on DioException catch (e) {
      final msg = e.response?.data is Map<String, dynamic>
          ? (e.response?.data['message'] as String?)
          : e.message;
      throw ServerException(msg ?? 'Failed to update image');
    }
  }

  @override
  Future<bool> deleteImage(String imageId) async {
    try {
      final response = await apiClient.delete('$_baseEndpoint/$imageId');
      if (response.statusCode == 204) return true;
      if (response.data is Map<String, dynamic>) {
        return (response.data as Map<String, dynamic>)['success'] == true;
      }
      return response.statusCode == 200;
    } on DioException catch (e) {
      final msg = e.response?.data is Map<String, dynamic>
          ? (e.response?.data['message'] as String?)
          : e.message;
      throw ServerException(msg ?? 'Failed to delete image');
    }
  }

  @override
  Future<bool> reorderImages(
    String? unitInSectionId,
    String? tempKey,
    List<String> imageIds,
  ) async {
    try {
      final payload = {
        'imageIds': imageIds,
        if (unitInSectionId != null && unitInSectionId.trim().isNotEmpty) 'unitInSectionId': unitInSectionId
        else if (tempKey != null && tempKey.trim().isNotEmpty) 'tempKey': tempKey,
      };
      final response =
          await apiClient.post('$_baseEndpoint/reorder', data: payload);
      return response.statusCode == 204 ||
          (response.data is Map && response.data['success'] == true);
    } on DioException catch (e) {
      throw ServerException(
          e.response?.data['message'] ?? 'Failed to reorder images');
    }
  }

  @override
  Future<bool> setAsPrimaryImage(
    String? unitInSectionId,
    String? tempKey,
    String imageId,
  ) async {
    try {
      final body = {
        if (unitInSectionId != null && unitInSectionId.trim().isNotEmpty) 'unitInSectionId': unitInSectionId
        else if (tempKey != null && tempKey.trim().isNotEmpty) 'tempKey': tempKey,
      };
      final response = await apiClient
          .post('$_baseEndpoint/$imageId/set-primary', data: body);
      return response.statusCode == 204 ||
          (response.data is Map && response.data['success'] == true);
    } on DioException catch (e) {
      throw ServerException(
          e.response?.data['message'] ?? 'Failed to set primary image');
    }
  }
}
