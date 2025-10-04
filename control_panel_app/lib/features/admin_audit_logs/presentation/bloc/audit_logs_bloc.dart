// lib/features/admin_audit_logs/presentation/bloc/audit_logs_bloc.dart

import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/entities/audit_log.dart';
import '../../domain/usecases/get_audit_logs_usecase.dart';
import '../../domain/usecases/export_audit_logs_usecase.dart';
import 'audit_logs_event.dart';
import 'audit_logs_state.dart';

class AuditLogsBloc extends Bloc<AuditLogsEvent, AuditLogsState> {
  final GetAuditLogsUseCase getAuditLogsUseCase;
  final ExportAuditLogsUseCase exportAuditLogsUseCase;

  static const int _pageSize = 20;

  AuditLogsBloc({
    required this.getAuditLogsUseCase,
    required this.exportAuditLogsUseCase,
  }) : super(AuditLogsInitial()) {
    on<LoadAuditLogsEvent>(_onLoadAuditLogs);
    on<LoadMoreAuditLogsEvent>(_onLoadMoreAuditLogs);
    on<RefreshAuditLogsEvent>(_onRefreshAuditLogs);
    on<FilterAuditLogsEvent>(_onFilterAuditLogs);
    on<ExportAuditLogsEvent>(_onExportAuditLogs);
    on<SelectAuditLogEvent>(_onSelectAuditLog);
    on<ClearFiltersEvent>(_onClearFilters);
  }

  Future<void> _onLoadAuditLogs(
    LoadAuditLogsEvent event,
    Emitter<AuditLogsState> emit,
  ) async {
    emit(AuditLogsLoading());

    final query = event.query.copyWith(
      pageNumber: event.query.pageNumber ?? 1,
      pageSize: event.query.pageSize ?? _pageSize,
    );

    final result = await getAuditLogsUseCase(query);

    result.fold(
      (failure) => emit(AuditLogsError(message: failure.message)),
      (paginatedResult) {
        emit(AuditLogsLoaded(
          auditLogs: paginatedResult.items,
          totalCount: paginatedResult.totalCount,
          currentPage: paginatedResult.pageNumber,
          pageSize: paginatedResult.pageSize,
          hasReachedMax: paginatedResult.items.length < _pageSize,
          currentQuery: query,
        ));
      },
    );
  }

  Future<void> _onLoadMoreAuditLogs(
    LoadMoreAuditLogsEvent event,
    Emitter<AuditLogsState> emit,
  ) async {
    if (state is AuditLogsLoaded) {
      final currentState = state as AuditLogsLoaded;
      
      if (currentState.hasReachedMax) return;

      final nextPage = currentState.currentPage + 1;
      final query = currentState.currentQuery.copyWith(
        pageNumber: nextPage,
      );

      final result = await getAuditLogsUseCase(query);

      result.fold(
        (failure) => emit(AuditLogsError(message: failure.message)),
        (paginatedResult) {
          emit(currentState.copyWith(
            auditLogs: [...currentState.auditLogs, ...paginatedResult.items],
            currentPage: nextPage,
            hasReachedMax: paginatedResult.items.length < _pageSize,
          ));
        },
      );
    }
  }

  Future<void> _onRefreshAuditLogs(
    RefreshAuditLogsEvent event,
    Emitter<AuditLogsState> emit,
  ) async {
    if (state is AuditLogsLoaded) {
      final currentState = state as AuditLogsLoaded;
      add(LoadAuditLogsEvent(query: currentState.currentQuery));
    } else {
      add(LoadAuditLogsEvent(query: const AuditLogsQuery()));
    }
  }

  Future<void> _onFilterAuditLogs(
    FilterAuditLogsEvent event,
    Emitter<AuditLogsState> emit,
  ) async {
    final query = AuditLogsQuery(
      pageNumber: 1,
      pageSize: _pageSize,
      userId: event.userId,
      from: event.from,
      to: event.to,
      operationType: event.operationType,
      searchTerm: event.searchTerm,
    );

    add(LoadAuditLogsEvent(query: query));
  }

  Future<void> _onExportAuditLogs(
    ExportAuditLogsEvent event,
    Emitter<AuditLogsState> emit,
  ) async {
    emit(AuditLogsExporting());

    final result = await exportAuditLogsUseCase(event.query);

    result.fold(
      (failure) => emit(AuditLogsError(message: failure.message)),
      (logs) => emit(AuditLogsExported(exportedLogs: logs)),
    );
  }

  void _onSelectAuditLog(
    SelectAuditLogEvent event,
    Emitter<AuditLogsState> emit,
  ) {
    if (state is AuditLogsLoaded) {
      final currentState = state as AuditLogsLoaded;
      emit(currentState.copyWith(selectedLog: event.auditLog));
    }
  }

  void _onClearFilters(
    ClearFiltersEvent event,
    Emitter<AuditLogsState> emit,
  ) {
    add(LoadAuditLogsEvent(query: const AuditLogsQuery()));
  }

  // Extension method for AuditLogsQuery
}

extension on AuditLogsQuery {
  AuditLogsQuery copyWith({
    int? pageNumber,
    int? pageSize,
    String? userId,
    DateTime? from,
    DateTime? to,
    String? searchTerm,
    String? operationType,
  }) {
    return AuditLogsQuery(
      pageNumber: pageNumber ?? this.pageNumber,
      pageSize: pageSize ?? this.pageSize,
      userId: userId ?? this.userId,
      from: from ?? this.from,
      to: to ?? this.to,
      searchTerm: searchTerm ?? this.searchTerm,
      operationType: operationType ?? this.operationType,
    );
  }
}