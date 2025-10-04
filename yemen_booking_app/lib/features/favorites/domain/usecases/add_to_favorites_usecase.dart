import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/favorite.dart';
import '../repositories/favorites_repository.dart';

class AddToFavoritesParams {
  final String propertyId;
  final String userId;

  AddToFavoritesParams({
    required this.propertyId,
    required this.userId,
  });
}

class AddToFavoritesUseCase implements UseCase<Favorite, AddToFavoritesParams> {
  final FavoritesRepository repository;

  AddToFavoritesUseCase(this.repository);

  @override
  Future<Either<Failure, Favorite>> call(AddToFavoritesParams params) async {
    return await repository.addToFavorites(
      propertyId: params.propertyId,
      userId: params.userId,
    );
  }
}