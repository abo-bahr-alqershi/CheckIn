import 'package:dartz/dartz.dart';
import 'package:yemen_booking_app/core/error/error_handler.dart';
import 'package:yemen_booking_app/core/error/failures.dart';
import 'package:yemen_booking_app/core/network/network_info.dart';
import 'package:yemen_booking_app/core/network/api_client.dart';
import '../../../favorites/domain/repositories/favorites_repository.dart';
import '../../../favorites/domain/entities/favorite.dart';

/// Temporary implementation of FavoritesRepository.
/// NOTE: Replace endpoints and mapping once backend contract is finalized.
class FavoritesRepositoryImpl implements FavoritesRepository {
  final ApiClient apiClient;
  final NetworkInfo networkInfo;

  FavoritesRepositoryImpl({
    required this.apiClient,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, List<Favorite>>> getFavorites() async {
    if (await networkInfo.isConnected) {
      try {
        final response = await apiClient.get('/favorites');
        final data = response.data;
        final list = (data is List)
            ? data
            : (data is Map && data['data'] is List) ? data['data'] as List : <dynamic>[];
        final favorites = list
            .map((e) => _mapFavorite(e as Map<String, dynamic>? ?? const {}))
            .toList();
        return Right(favorites);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    }
    return const Left(NetworkFailure('لا يوجد اتصال بالإنترنت'));
  }

  @override
  Future<Either<Failure, Favorite>> addToFavorites({
    required String propertyId,
    required String userId,
  }) async {
    if (await networkInfo.isConnected) {
      try {
        final response = await apiClient.post('/favorites', data: {
          'property_id': propertyId,
          'user_id': userId,
        });
        final data = response.data;
        final fav = _mapFavorite(data is Map<String, dynamic> ? data : {});
        return Right(fav);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    }
    return const Left(NetworkFailure('لا يوجد اتصال بالإنترنت'));
  }

  @override
  Future<Either<Failure, void>> removeFromFavorites({
    required String propertyId,
    required String userId,
  }) async {
    if (await networkInfo.isConnected) {
      try {
        await apiClient.delete('/favorites/$propertyId', queryParameters: {
          'user_id': userId,
        });
        return const Right(null);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    }
    return const Left(NetworkFailure('لا يوجد اتصال بالإنترنت'));
  }

  @override
  Future<Either<Failure, bool>> checkFavoriteStatus({
    required String propertyId,
    required String userId,
  }) async {
    if (await networkInfo.isConnected) {
      try {
        final response = await apiClient.get('/favorites/$propertyId/status', queryParameters: {
          'user_id': userId,
        });
        final data = response.data;
        if (data is Map && data['is_favorite'] is bool) {
          return Right(data['is_favorite'] as bool);
        }
        return const Right(false);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    }
    return const Left(NetworkFailure('لا يوجد اتصال بالإنترنت'));
  }

  Favorite _mapFavorite(Map<String, dynamic> json) {
    // Provide safe defaults for missing fields
    DateTime parseDate(dynamic v) {
      if (v is String) {
        final d = DateTime.tryParse(v);
        if (d != null) return d;
      }
      return DateTime.now();
    }

    List<PropertyImage> images = [];
    final imagesRaw = json['images'];
    if (imagesRaw is List) {
      images = imagesRaw.map((i) {
        final m = i as Map<String, dynamic>? ?? {};
        return PropertyImage(
          id: (m['id'] ?? '').toString(),
          propertyId: (m['property_id'] ?? m['propertyId'])?.toString(),
          unitId: (m['unit_id'] ?? m['unitId'])?.toString(),
          name: (m['name'] ?? 'image').toString(),
          url: (m['url'] ?? '').toString(),
          sizeBytes: (m['size_bytes'] ?? m['sizeBytes'] ?? 0) is int
              ? (m['size_bytes'] ?? m['sizeBytes'] ?? 0) as int
              : int.tryParse((m['size_bytes'] ?? m['sizeBytes'] ?? '0').toString()) ?? 0,
          type: (m['type'] ?? '').toString(),
            category: (m['category'] ?? '').toString(),
          caption: (m['caption'] ?? '').toString(),
          altText: (m['alt_text'] ?? m['altText'] ?? '').toString(),
          tags: (m['tags'] ?? '').toString(),
          sizes: (m['sizes'] ?? '').toString(),
          isMain: (m['is_main'] ?? m['isMain'] ?? false) as bool,
          displayOrder: (m['display_order'] ?? m['displayOrder'] ?? 0) is int
              ? (m['display_order'] ?? m['displayOrder'] ?? 0) as int
              : int.tryParse((m['display_order'] ?? m['displayOrder'] ?? '0').toString()) ?? 0,
          uploadedAt: parseDate(m['uploaded_at'] ?? m['uploadedAt']),
          status: (m['status'] ?? '').toString(),
          associationType: (m['association_type'] ?? m['associationType'] ?? '').toString(),
        );
      }).toList();
    }

    List<Amenity> amenities = [];
    final amenitiesRaw = json['amenities'];
    if (amenitiesRaw is List) {
      amenities = amenitiesRaw.map((a) {
        final m = a as Map<String, dynamic>? ?? {};
        return Amenity(
          id: (m['id'] ?? '').toString(),
          name: (m['name'] ?? '').toString(),
          description: (m['description'] ?? '').toString(),
          iconUrl: (m['icon_url'] ?? m['iconUrl'] ?? '').toString(),
          category: (m['category'] ?? '').toString(),
          isActive: (m['is_active'] ?? m['isActive'] ?? true) as bool,
          displayOrder: (m['display_order'] ?? m['displayOrder'] ?? 0) is int
              ? (m['display_order'] ?? m['displayOrder'] ?? 0) as int
              : int.tryParse((m['display_order'] ?? m['displayOrder'] ?? '0').toString()) ?? 0,
          createdAt: parseDate(m['created_at'] ?? m['createdAt']),
        );
      }).toList();
    }

    double toDouble(dynamic v) {
      if (v is num) return v.toDouble();
      return double.tryParse(v?.toString() ?? '') ?? 0.0;
    }

    int toInt(dynamic v) {
      if (v is int) return v;
      if (v is num) return v.toInt();
      return int.tryParse(v?.toString() ?? '') ?? 0;
    }

    return Favorite(
      id: (json['id'] ?? '').toString(),
      userId: (json['user_id'] ?? json['userId'] ?? '').toString(),
      propertyId: (json['property_id'] ?? json['propertyId'] ?? '').toString(),
      propertyName: (json['property_name'] ?? json['propertyName'] ?? 'عقار').toString(),
      propertyImage: (json['property_image'] ?? json['propertyImage'] ?? '').toString(),
      propertyLocation: (json['property_location'] ?? json['propertyLocation'] ?? '').toString(),
      typeId: (json['type_id'] ?? json['typeId'] ?? '').toString(),
      typeName: (json['type_name'] ?? json['typeName'] ?? 'نوع').toString(),
      ownerName: (json['owner_name'] ?? json['ownerName'] ?? 'المالك').toString(),
      address: (json['address'] ?? '').toString(),
      city: (json['city'] ?? '').toString(),
      latitude: toDouble(json['latitude']),
      longitude: toDouble(json['longitude']),
      starRating: toInt(json['star_rating'] ?? json['starRating']),
      averageRating: toDouble(json['average_rating'] ?? json['averageRating']),
      reviewsCount: toInt(json['reviews_count'] ?? json['reviewsCount']),
      images: images,
      amenities: amenities,
      createdAt: parseDate(json['created_at'] ?? json['createdAt']),
    );
  }
}
