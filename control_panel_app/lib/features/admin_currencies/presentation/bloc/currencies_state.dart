import 'package:equatable/equatable.dart';
import '../../domain/entities/currency.dart';

abstract class CurrenciesState extends Equatable {
  const CurrenciesState();

  @override
  List<Object?> get props => [];
}

class CurrenciesInitial extends CurrenciesState {}

class CurrenciesLoading extends CurrenciesState {}

class CurrenciesLoaded extends CurrenciesState {
  final List<Currency> currencies;
  final List<Currency> filteredCurrencies;
  final String searchQuery;
  final bool isSaving;

  const CurrenciesLoaded({
    required this.currencies,
    required this.filteredCurrencies,
    this.searchQuery = '',
    this.isSaving = false,
  });

  CurrenciesLoaded copyWith({
    List<Currency>? currencies,
    List<Currency>? filteredCurrencies,
    String? searchQuery,
    bool? isSaving,
  }) {
    return CurrenciesLoaded(
      currencies: currencies ?? this.currencies,
      filteredCurrencies: filteredCurrencies ?? this.filteredCurrencies,
      searchQuery: searchQuery ?? this.searchQuery,
      isSaving: isSaving ?? this.isSaving,
    );
  }

  @override
  List<Object?> get props => [currencies, filteredCurrencies, searchQuery, isSaving];
}

class CurrenciesError extends CurrenciesState {
  final String message;

  const CurrenciesError({required this.message});

  @override
  List<Object> get props => [message];
}

class CurrencyOperationSuccess extends CurrenciesState {
  final String message;

  const CurrencyOperationSuccess({required this.message});

  @override
  List<Object> get props => [message];
}

class CurrencySaving extends CurrenciesState {}