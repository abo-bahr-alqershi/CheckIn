import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../../domain/entities/unit.dart';
import '../../../domain/usecases/get_units_usecase.dart';
import '../../../domain/usecases/delete_unit_usecase.dart';

part 'units_list_event.dart';
part 'units_list_state.dart';

class UnitsListBloc extends Bloc<UnitsListEvent, UnitsListState> {
  final GetUnitsUseCase getUnitsUseCase;
  final DeleteUnitUseCase deleteUnitUseCase;

  UnitsListBloc({
    required this.getUnitsUseCase,
    required this.deleteUnitUseCase,
  }) : super(UnitsListInitial()) {
    on<LoadUnitsEvent>(_onLoadUnits);
    on<SearchUnitsEvent>(_onSearchUnits);
    on<FilterUnitsEvent>(_onFilterUnits);
    on<DeleteUnitEvent>(_onDeleteUnit);
    on<RefreshUnitsEvent>(_onRefreshUnits);
  }

  Future<void> _onLoadUnits(
    LoadUnitsEvent event,
    Emitter<UnitsListState> emit,
  ) async {
    emit(UnitsListLoading());

    final result = await getUnitsUseCase(
      GetUnitsParams(
        pageNumber: event.pageNumber,
        pageSize: event.pageSize,
      ),
    );

    result.fold(
      (failure) => emit(UnitsListError(message: failure.message)),
      (units) => emit(UnitsListLoaded(
        units: units,
        totalCount: units.length,
        currentPage: event.pageNumber ?? 1,
        pageSize: event.pageSize ?? 20,
      )),
    );
  }

  Future<void> _onSearchUnits(
    SearchUnitsEvent event,
    Emitter<UnitsListState> emit,
  ) async {
    if (state is UnitsListLoaded) {
      final currentState = state as UnitsListLoaded;
      emit(UnitsListLoading());

      final result = await getUnitsUseCase(
        GetUnitsParams(
          searchQuery: event.query,
          pageNumber: 1,
          pageSize: currentState.pageSize,
        ),
      );

      result.fold(
        (failure) => emit(UnitsListError(message: failure.message)),
        (units) => emit(UnitsListLoaded(
          units: units,
          totalCount: units.length,
          currentPage: 1,
          pageSize: currentState.pageSize,
          searchQuery: event.query,
        )),
      );
    }
  }

  Future<void> _onFilterUnits(
    FilterUnitsEvent event,
    Emitter<UnitsListState> emit,
  ) async {
    if (state is UnitsListLoaded) {
      final currentState = state as UnitsListLoaded;
      emit(UnitsListLoading());

      final result = await getUnitsUseCase(
        GetUnitsParams(
          propertyId: event.filters['propertyId'],
          unitTypeId: event.filters['unitTypeId'],
          isAvailable: event.filters['isAvailable'],
          minPrice: event.filters['minPrice']?.toDouble(),
          maxPrice: event.filters['maxPrice']?.toDouble(),
          pricingMethod: event.filters['pricingMethod'],
          checkInDate: event.filters['checkInDate'],
          checkOutDate: event.filters['checkOutDate'],
          numberOfGuests: event.filters['numberOfGuests'],
          hasActiveBookings: event.filters['hasActiveBookings'],
          location: event.filters['location'],
          sortBy: event.filters['sortBy'],
          latitude: event.filters['latitude'],
          longitude: event.filters['longitude'],
          radiusKm: event.filters['radiusKm'],
          pageNumber: 1,
          pageSize: currentState.pageSize,
        ),
      );

      result.fold(
        (failure) => emit(UnitsListError(message: failure.message)),
        (units) => emit(UnitsListLoaded(
          units: units,
          totalCount: units.length,
          currentPage: 1,
          pageSize: currentState.pageSize,
          filters: event.filters,
        )),
      );
    }
  }

  Future<void> _onDeleteUnit(
    DeleteUnitEvent event,
    Emitter<UnitsListState> emit,
  ) async {
    if (state is UnitsListLoaded) {
      final currentState = state as UnitsListLoaded;
      
      final result = await deleteUnitUseCase(event.unitId);

      result.fold(
        (failure) => emit(UnitsListError(message: failure.message)),
        (_) {
          final updatedUnits = currentState.units
              .where((unit) => unit.id != event.unitId)
              .toList();
          
          emit(UnitsListLoaded(
            units: updatedUnits,
            totalCount: updatedUnits.length,
            currentPage: currentState.currentPage,
            pageSize: currentState.pageSize,
            searchQuery: currentState.searchQuery,
            filters: currentState.filters,
          ));
        },
      );
    }
  }

  Future<void> _onRefreshUnits(
    RefreshUnitsEvent event,
    Emitter<UnitsListState> emit,
  ) async {
    add(LoadUnitsEvent());
  }
}