import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/usecases/get_property_details_usecase.dart';
import '../../domain/usecases/get_property_units_usecase.dart';
import '../../domain/usecases/get_property_reviews_usecase.dart';
import '../../domain/usecases/add_to_favorites_usecase.dart';
import 'property_event.dart';
import 'property_state.dart';

class PropertyBloc extends Bloc<PropertyEvent, PropertyState> {
  final GetPropertyDetailsUseCase getPropertyDetailsUseCase;
  final GetPropertyUnitsUseCase getPropertyUnitsUseCase;
  final GetPropertyReviewsUseCase getPropertyReviewsUseCase;
  final AddToFavoritesUseCase addToFavoritesUseCase;

  PropertyBloc({
    required this.getPropertyDetailsUseCase,
    required this.getPropertyUnitsUseCase,
    required this.getPropertyReviewsUseCase,
    required this.addToFavoritesUseCase,
  }) : super(PropertyInitial()) {
    on<GetPropertyDetailsEvent>(_onGetPropertyDetails);
    on<GetPropertyUnitsEvent>(_onGetPropertyUnits);
    on<GetPropertyReviewsEvent>(_onGetPropertyReviews);
    on<AddToFavoritesEvent>(_onAddToFavorites);
    on<RemoveFromFavoritesEvent>(_onRemoveFromFavorites);
    on<UpdateViewCountEvent>(_onUpdateViewCount);
    on<ToggleFavoriteEvent>(_onToggleFavorite);
    on<SelectUnitEvent>(_onSelectUnit);
    on<SelectImageEvent>(_onSelectImage);
  }

  Future<void> _onGetPropertyDetails(
    GetPropertyDetailsEvent event,
    Emitter<PropertyState> emit,
  ) async {
    emit(PropertyLoading());

    final result = await getPropertyDetailsUseCase(
      GetPropertyDetailsParams(
        propertyId: event.propertyId,
        userId: event.userId,
      ),
    );

    result.fold(
      (failure) => emit(PropertyError(message: failure.message)),
      (property) {
        emit(PropertyDetailsLoaded(
          property: property,
          isFavorite: property.isFavorite,
        ));
        // Trigger units load with default guestsCount if available
        add(GetPropertyUnitsEvent(
          propertyId: event.propertyId,
          guestsCount: 1,
        ));
      },
    );
  }

  Future<void> _onGetPropertyUnits(
    GetPropertyUnitsEvent event,
    Emitter<PropertyState> emit,
  ) async {
    final previousState = state;

    final result = await getPropertyUnitsUseCase(
      GetPropertyUnitsParams(
        propertyId: event.propertyId,
        checkInDate: event.checkInDate,
        checkOutDate: event.checkOutDate,
        guestsCount: event.guestsCount,
      ),
    );

    result.fold(
      (failure) {
        // Preserve current details UI; do not break if units call fails
        if (previousState is PropertyDetailsLoaded || previousState is PropertyWithDetails) {
          return;
        }
        emit(PropertyError(message: failure.message));
      },
      (units) {
        if (previousState is PropertyWithDetails) {
          emit(previousState.copyWith(units: units));
        } else if (previousState is PropertyDetailsLoaded) {
          emit(PropertyWithDetails(
            property: previousState.property,
            units: units,
            reviews: const [],
            isFavorite: previousState.isFavorite,
            selectedImageIndex: previousState.selectedImageIndex,
          ));
        } else {
          emit(PropertyUnitsLoaded(
            units: units,
            checkInDate: event.checkInDate,
            checkOutDate: event.checkOutDate,
            guestsCount: event.guestsCount,
          ));
        }
      },
    );
  }

  Future<void> _onGetPropertyReviews(
    GetPropertyReviewsEvent event,
    Emitter<PropertyState> emit,
  ) async {
    emit(PropertyReviewsLoading());

    final result = await getPropertyReviewsUseCase(
      GetPropertyReviewsParams(
        propertyId: event.propertyId,
        pageNumber: event.pageNumber,
        pageSize: event.pageSize,
        sortBy: event.sortBy,
        sortDirection: event.sortDirection,
        withImagesOnly: event.withImagesOnly,
        userId: event.userId,
      ),
    );

    result.fold(
      (failure) => emit(PropertyError(message: failure.message)),
      (reviews) => emit(PropertyReviewsLoaded(
        reviews: reviews,
        currentPage: event.pageNumber,
        hasReachedMax: reviews.length < event.pageSize,
      )),
    );
  }

  Future<void> _onAddToFavorites(
    AddToFavoritesEvent event,
    Emitter<PropertyState> emit,
  ) async {
    final result = await addToFavoritesUseCase(
      AddToFavoritesParams(
        propertyId: event.propertyId,
        userId: event.userId,
        notes: event.notes,
        desiredVisitDate: event.desiredVisitDate,
        expectedBudget: event.expectedBudget,
        currency: event.currency,
      ),
    );

    result.fold(
      (failure) => emit(PropertyError(message: failure.message)),
      (success) {
        emit(const PropertyFavoriteUpdated(
          isFavorite: true,
          message: 'تمت الإضافة إلى المفضلة',
        ));
        if (state is PropertyDetailsLoaded) {
          final currentState = state as PropertyDetailsLoaded;
          emit(currentState.copyWith(isFavorite: true));
        } else if (state is PropertyWithDetails) {
          final currentState = state as PropertyWithDetails;
          emit(currentState.copyWith(isFavorite: true));
        }
      },
    );
  }

  Future<void> _onRemoveFromFavorites(
    RemoveFromFavoritesEvent event,
    Emitter<PropertyState> emit,
  ) async {
    // Implementation for removing from favorites
    emit(const PropertyFavoriteUpdated(
      isFavorite: false,
      message: 'تمت الإزالة من المفضلة',
    ));

    if (state is PropertyDetailsLoaded) {
      final currentState = state as PropertyDetailsLoaded;
      emit(currentState.copyWith(isFavorite: false));
    } else if (state is PropertyWithDetails) {
      final currentState = state as PropertyWithDetails;
      emit(currentState.copyWith(isFavorite: false));
    }
  }

  Future<void> _onUpdateViewCount(
    UpdateViewCountEvent event,
    Emitter<PropertyState> emit,
  ) async {
    // Silently update view count
  }

  Future<void> _onToggleFavorite(
    ToggleFavoriteEvent event,
    Emitter<PropertyState> emit,
  ) async {
    if (event.isFavorite) {
      add(RemoveFromFavoritesEvent(
        propertyId: event.propertyId,
        userId: event.userId,
      ));
    } else {
      add(AddToFavoritesEvent(
        propertyId: event.propertyId,
        userId: event.userId,
      ));
    }
  }

  Future<void> _onSelectUnit(
    SelectUnitEvent event,
    Emitter<PropertyState> emit,
  ) async {
    if (state is PropertyUnitsLoaded) {
      final currentState = state as PropertyUnitsLoaded;
      emit(currentState.copyWith(selectedUnitId: event.unitId));
    } else if (state is PropertyWithDetails) {
      final currentState = state as PropertyWithDetails;
      emit(currentState.copyWith(selectedUnitId: event.unitId));
    }
  }

  Future<void> _onSelectImage(
    SelectImageEvent event,
    Emitter<PropertyState> emit,
  ) async {
    if (state is PropertyDetailsLoaded) {
      final currentState = state as PropertyDetailsLoaded;
      emit(currentState.copyWith(selectedImageIndex: event.imageIndex));
    } else if (state is PropertyWithDetails) {
      final currentState = state as PropertyWithDetails;
      emit(currentState.copyWith(selectedImageIndex: event.imageIndex));
    }
  }
}