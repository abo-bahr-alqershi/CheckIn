import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../../../core/models/paginated_result.dart';
import '../../../../../../core/enums/booking_status.dart';
import '../../../domain/entities/booking.dart';
import '../../../domain/usecases/bookings/cancel_booking_usecase.dart';
import '../../../domain/usecases/bookings/update_booking_usecase.dart';
import '../../../domain/usecases/bookings/confirm_booking_usecase.dart';
import '../../../domain/usecases/bookings/get_bookings_by_date_range_usecase.dart';
import '../../../domain/usecases/bookings/check_in_usecase.dart';
import '../../../domain/usecases/bookings/check_out_usecase.dart';
import 'bookings_list_event.dart';
import 'bookings_list_state.dart';

class BookingsListBloc extends Bloc<BookingsListEvent, BookingsListState> {
  final CancelBookingUseCase cancelBookingUseCase;
  final UpdateBookingUseCase updateBookingUseCase;
  final ConfirmBookingUseCase confirmBookingUseCase;
  final GetBookingsByDateRangeUseCase getBookingsByDateRangeUseCase;
  final CheckInUseCase checkInUseCase;
  final CheckOutUseCase checkOutUseCase;

  // متغيرات لحفظ حالة البحث والفلاتر
  DateTime? _currentStartDate;
  DateTime? _currentEndDate;
  String? _currentUserId;
  String? _currentGuestNameOrEmail;
  String? _currentUnitId;
  String? _currentBookingSource;
  int _currentPageNumber = 1;
  int _currentPageSize = 10;

  BookingsListBloc({
    required this.cancelBookingUseCase,
    required this.updateBookingUseCase,
    required this.confirmBookingUseCase,
    required this.getBookingsByDateRangeUseCase,
    required this.checkInUseCase,
    required this.checkOutUseCase,
  }) : super(BookingsListInitial()) {
    on<LoadBookingsEvent>(_onLoadBookings);
    on<RefreshBookingsEvent>(_onRefreshBookings);
    on<CancelBookingEvent>(_onCancelBooking);
    on<UpdateBookingEvent>(_onUpdateBooking);
    on<ConfirmBookingEvent>(_onConfirmBooking);
    on<CheckInBookingEvent>(_onCheckInBooking);
    on<CheckOutBookingEvent>(_onCheckOutBooking);
    on<FilterBookingsEvent>(_onFilterBookings);
    on<SearchBookingsEvent>(_onSearchBookings);
    on<ChangePageEvent>(_onChangePage);
    on<ChangePageSizeEvent>(_onChangePageSize);
    on<SelectBookingEvent>(_onSelectBooking);
    on<DeselectBookingEvent>(_onDeselectBooking);
    on<SelectMultipleBookingsEvent>(_onSelectMultipleBookings);
    on<ClearSelectionEvent>(_onClearSelection);
  }

  Future<void> _onLoadBookings(
    LoadBookingsEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    emit(BookingsListLoading());

    // حفظ قيم الفلاتر
    _currentStartDate = event.startDate;
    _currentEndDate = event.endDate;
    _currentUserId = event.userId;
    _currentGuestNameOrEmail = event.guestNameOrEmail;
    _currentUnitId = event.unitId;
    _currentBookingSource = event.bookingSource;
    _currentPageNumber = event.pageNumber;
    _currentPageSize = event.pageSize;

    final result = await getBookingsByDateRangeUseCase(
      GetBookingsByDateRangeParams(
        startDate: event.startDate,
        endDate: event.endDate,
        pageNumber: event.pageNumber,
        pageSize: event.pageSize,
        userId: event.userId,
        guestNameOrEmail: event.guestNameOrEmail,
        unitId: event.unitId,
        bookingSource: event.bookingSource,
      ),
    );

    result.fold(
      (failure) => emit(BookingsListError(message: failure.message)),
      (bookings) => emit(BookingsListLoaded(
        bookings: bookings,
        selectedBookings: const [],
        filters: BookingFilters(
          startDate: event.startDate,
          endDate: event.endDate,
          userId: event.userId,
          guestNameOrEmail: event.guestNameOrEmail,
          unitId: event.unitId,
          bookingSource: event.bookingSource,
        ),
      )),
    );
  }

  Future<void> _onRefreshBookings(
    RefreshBookingsEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (_currentStartDate == null || _currentEndDate == null) {
      // Set default date range if not set
      _currentEndDate = DateTime.now();
      _currentStartDate = DateTime.now().subtract(const Duration(days: 30));
    }

    add(LoadBookingsEvent(
      startDate: _currentStartDate!,
      endDate: _currentEndDate!,
      pageNumber: _currentPageNumber,
      pageSize: _currentPageSize,
      userId: _currentUserId,
      guestNameOrEmail: _currentGuestNameOrEmail,
      unitId: _currentUnitId,
      bookingSource: _currentBookingSource,
    ));
  }

  Future<void> _onCancelBooking(
    CancelBookingEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;

      emit(BookingOperationInProgress(
        bookings: currentState.bookings,
        selectedBookings: currentState.selectedBookings,
        operation: 'cancel',
        bookingId: event.bookingId,
      ));

      final result = await cancelBookingUseCase(
        CancelBookingParams(
          bookingId: event.bookingId,
          cancellationReason: event.cancellationReason,
        ),
      );

      result.fold(
        (failure) => emit(BookingOperationFailure(
          bookings: currentState.bookings,
          selectedBookings: currentState.selectedBookings,
          message: failure.message,
          bookingId: event.bookingId,
        )),
        (_) {
          emit(BookingOperationSuccess(
            bookings: currentState.bookings,
            selectedBookings: currentState.selectedBookings,
            message: 'تم إلغاء الحجز بنجاح',
            bookingId: event.bookingId,
          ));
          // إعادة تحميل القائمة
          add(RefreshBookingsEvent());
        },
      );
    }
  }

  Future<void> _onUpdateBooking(
    UpdateBookingEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;

      emit(BookingOperationInProgress(
        bookings: currentState.bookings,
        selectedBookings: currentState.selectedBookings,
        operation: 'update',
        bookingId: event.bookingId,
      ));

      final result = await updateBookingUseCase(
        UpdateBookingParams(
          bookingId: event.bookingId,
          checkIn: event.checkIn,
          checkOut: event.checkOut,
          guestsCount: event.guestsCount,
        ),
      );

      result.fold(
        (failure) => emit(BookingOperationFailure(
          bookings: currentState.bookings,
          selectedBookings: currentState.selectedBookings,
          message: failure.message,
          bookingId: event.bookingId,
        )),
        (_) {
          emit(BookingOperationSuccess(
            bookings: currentState.bookings,
            selectedBookings: currentState.selectedBookings,
            message: 'تم تحديث الحجز بنجاح',
            bookingId: event.bookingId,
          ));
          // إعادة تحميل القائمة
          add(RefreshBookingsEvent());
        },
      );
    }
  }

  Future<void> _onConfirmBooking(
    ConfirmBookingEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;

      emit(BookingOperationInProgress(
        bookings: currentState.bookings,
        selectedBookings: currentState.selectedBookings,
        operation: 'confirm',
        bookingId: event.bookingId,
      ));

      final result = await confirmBookingUseCase(
        ConfirmBookingParams(bookingId: event.bookingId),
      );

      result.fold(
        (failure) => emit(BookingOperationFailure(
          bookings: currentState.bookings,
          selectedBookings: currentState.selectedBookings,
          message: failure.message,
          bookingId: event.bookingId,
        )),
        (_) {
          emit(BookingOperationSuccess(
            bookings: currentState.bookings,
            selectedBookings: currentState.selectedBookings,
            message: 'تم تأكيد الحجز بنجاح',
            bookingId: event.bookingId,
          ));
          // إعادة تحميل القائمة
          add(RefreshBookingsEvent());
        },
      );
    }
  }

  Future<void> _onCheckInBooking(
    CheckInBookingEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;

      emit(BookingOperationInProgress(
        bookings: currentState.bookings,
        selectedBookings: currentState.selectedBookings,
        operation: 'checkIn',
        bookingId: event.bookingId,
      ));

      final result = await checkInUseCase(
        CheckInParams(bookingId: event.bookingId),
      );

      result.fold(
        (failure) => emit(BookingOperationFailure(
          bookings: currentState.bookings,
          selectedBookings: currentState.selectedBookings,
          message: failure.message,
          bookingId: event.bookingId,
        )),
        (_) {
          emit(BookingOperationSuccess(
            bookings: currentState.bookings,
            selectedBookings: currentState.selectedBookings,
            message: 'تم تسجيل الوصول بنجاح',
            bookingId: event.bookingId,
          ));
          // إعادة تحميل القائمة
          add(RefreshBookingsEvent());
        },
      );
    }
  }

  Future<void> _onCheckOutBooking(
    CheckOutBookingEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;

      emit(BookingOperationInProgress(
        bookings: currentState.bookings,
        selectedBookings: currentState.selectedBookings,
        operation: 'checkOut',
        bookingId: event.bookingId,
      ));

      final result = await checkOutUseCase(
        CheckOutParams(bookingId: event.bookingId),
      );

      result.fold(
        (failure) => emit(BookingOperationFailure(
          bookings: currentState.bookings,
          selectedBookings: currentState.selectedBookings,
          message: failure.message,
          bookingId: event.bookingId,
        )),
        (_) {
          emit(BookingOperationSuccess(
            bookings: currentState.bookings,
            selectedBookings: currentState.selectedBookings,
            message: 'تم تسجيل المغادرة بنجاح',
            bookingId: event.bookingId,
          ));
          // إعادة تحميل القائمة
          add(RefreshBookingsEvent());
        },
      );
    }
  }

  Future<void> _onFilterBookings(
    FilterBookingsEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    add(LoadBookingsEvent(
      startDate: event.startDate ??
          _currentStartDate ??
          DateTime.now().subtract(const Duration(days: 30)),
      endDate: event.endDate ?? _currentEndDate ?? DateTime.now(),
      pageNumber: 1, // Reset to first page when filtering
      pageSize: _currentPageSize,
      userId: event.userId,
      guestNameOrEmail: event.guestNameOrEmail,
      unitId: event.unitId,
      bookingSource: event.bookingSource,
    ));
  }

  Future<void> _onSearchBookings(
    SearchBookingsEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    add(LoadBookingsEvent(
      startDate: _currentStartDate ??
          DateTime.now().subtract(const Duration(days: 30)),
      endDate: _currentEndDate ?? DateTime.now(),
      pageNumber: 1, // Reset to first page when searching
      pageSize: _currentPageSize,
      userId: _currentUserId,
      guestNameOrEmail: event.searchTerm,
      unitId: _currentUnitId,
      bookingSource: _currentBookingSource,
    ));
  }

  Future<void> _onChangePage(
    ChangePageEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    add(LoadBookingsEvent(
      startDate: _currentStartDate ??
          DateTime.now().subtract(const Duration(days: 30)),
      endDate: _currentEndDate ?? DateTime.now(),
      pageNumber: event.pageNumber,
      pageSize: _currentPageSize,
      userId: _currentUserId,
      guestNameOrEmail: _currentGuestNameOrEmail,
      unitId: _currentUnitId,
      bookingSource: _currentBookingSource,
    ));
  }

  Future<void> _onChangePageSize(
    ChangePageSizeEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    add(LoadBookingsEvent(
      startDate: _currentStartDate ??
          DateTime.now().subtract(const Duration(days: 30)),
      endDate: _currentEndDate ?? DateTime.now(),
      pageNumber: 1, // Reset to first page when changing page size
      pageSize: event.pageSize,
      userId: _currentUserId,
      guestNameOrEmail: _currentGuestNameOrEmail,
      unitId: _currentUnitId,
      bookingSource: _currentBookingSource,
    ));
  }

  Future<void> _onSelectBooking(
    SelectBookingEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;
      final updatedSelection =
          List<Booking>.from(currentState.selectedBookings);

      final booking = currentState.bookings.items.firstWhere(
        (b) => b.id == event.bookingId,
      );

      if (!updatedSelection.contains(booking)) {
        updatedSelection.add(booking);
      }

      emit(currentState.copyWith(selectedBookings: updatedSelection));
    }
  }

  Future<void> _onDeselectBooking(
    DeselectBookingEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;
      final updatedSelection = List<Booking>.from(currentState.selectedBookings)
        ..removeWhere((b) => b.id == event.bookingId);

      emit(currentState.copyWith(selectedBookings: updatedSelection));
    }
  }

  Future<void> _onSelectMultipleBookings(
    SelectMultipleBookingsEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;
      final bookings = currentState.bookings.items
          .where((b) => event.bookingIds.contains(b.id))
          .toList();

      emit(currentState.copyWith(selectedBookings: bookings));
    }
  }

  Future<void> _onClearSelection(
    ClearSelectionEvent event,
    Emitter<BookingsListState> emit,
  ) async {
    if (state is BookingsListLoaded) {
      final currentState = state as BookingsListLoaded;
      emit(currentState.copyWith(selectedBookings: []));
    }
  }
}
