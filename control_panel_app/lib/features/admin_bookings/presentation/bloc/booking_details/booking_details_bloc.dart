import 'package:bookn_cp_app/features/admin_reviews/domain/repositories/reviews_repository.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../domain/entities/booking_details.dart';
import '../../../domain/usecases/bookings/get_booking_by_id_usecase.dart';
import '../../../domain/usecases/bookings/cancel_booking_usecase.dart';
import '../../../domain/usecases/bookings/update_booking_usecase.dart';
import '../../../domain/usecases/bookings/confirm_booking_usecase.dart';
import '../../../domain/usecases/bookings/check_in_usecase.dart';
import '../../../domain/usecases/bookings/check_out_usecase.dart';
import '../../../domain/usecases/services/add_service_to_booking_usecase.dart';
import '../../../domain/usecases/services/remove_service_from_booking_usecase.dart';
import '../../../domain/usecases/services/get_booking_services_usecase.dart';
import '../../../domain/repositories/bookings_repository.dart';
import 'booking_details_event.dart';
import 'booking_details_state.dart';

class BookingDetailsBloc
    extends Bloc<BookingDetailsEvent, BookingDetailsState> {
  final GetBookingByIdUseCase getBookingByIdUseCase;
  final CancelBookingUseCase cancelBookingUseCase;
  final UpdateBookingUseCase updateBookingUseCase;
  final ConfirmBookingUseCase confirmBookingUseCase;
  final CheckInUseCase checkInUseCase;
  final CheckOutUseCase checkOutUseCase;
  final AddServiceToBookingUseCase addServiceToBookingUseCase;
  final RemoveServiceFromBookingUseCase removeServiceFromBookingUseCase;
  final GetBookingServicesUseCase getBookingServicesUseCase;
  final BookingsRepository repository;
  final ReviewsRepository reviewsRepository;

  String? _currentBookingId;

  BookingDetailsBloc({
    required this.getBookingByIdUseCase,
    required this.cancelBookingUseCase,
    required this.updateBookingUseCase,
    required this.confirmBookingUseCase,
    required this.checkInUseCase,
    required this.checkOutUseCase,
    required this.addServiceToBookingUseCase,
    required this.removeServiceFromBookingUseCase,
    required this.getBookingServicesUseCase,
    required this.repository,
    required this.reviewsRepository,
  }) : super(BookingDetailsInitial()) {
    on<LoadBookingDetailsEvent>(_onLoadBookingDetails);
    on<RefreshBookingDetailsEvent>(_onRefreshBookingDetails);
    on<UpdateBookingDetailsEvent>(_onUpdateBookingDetails);
    on<CancelBookingDetailsEvent>(_onCancelBookingDetails);
    on<ConfirmBookingDetailsEvent>(_onConfirmBookingDetails);
    on<CheckInBookingDetailsEvent>(_onCheckInBookingDetails);
    on<CheckOutBookingDetailsEvent>(_onCheckOutBookingDetails);
    on<AddServiceEvent>(_onAddService);
    on<RemoveServiceEvent>(_onRemoveService);
    on<LoadBookingServicesEvent>(_onLoadBookingServices);
    on<LoadBookingActivitiesEvent>(_onLoadBookingActivities);
    on<LoadBookingPaymentsEvent>(_onLoadBookingPayments);
    on<PrintBookingDetailsEvent>(_onPrintBookingDetails);
    on<ShareBookingDetailsEvent>(_onShareBookingDetails);
    on<SendBookingConfirmationEvent>(_onSendBookingConfirmation);
  }

  Future<void> _onLoadBookingDetails(
    LoadBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    emit(BookingDetailsLoading());
    _currentBookingId = event.bookingId;

    // جلب البيانات الأساسية
    final bookingResult = await getBookingByIdUseCase(
      GetBookingByIdParams(bookingId: event.bookingId),
    );

    await bookingResult.fold(
      (failure) async {
        emit(BookingDetailsError(message: failure.message));
      },
      (booking) async {
        // جلب التفاصيل الإضافية
        final detailsResult = await repository.getBookingDetails(
          bookingId: event.bookingId,
        );

        await detailsResult.fold(
          (failure) async {
            // إذا فشل جلب التفاصيل، عرض البيانات الأساسية فقط
            emit(BookingDetailsLoaded(
              booking: booking,
              bookingDetails: null,
              services: const [],
              isRefreshing: false,
            ));
          },
          (details) async {
            // جلب الخدمات
            final servicesResult = await getBookingServicesUseCase(
              GetBookingServicesParams(bookingId: event.bookingId),
            );

            final services = servicesResult.fold(
              (_) => <Service>[],
              (services) => services,
            );

            // جلب التقييم المرتبط بالحجز (إن وجد)
            final reviewResult =
                await reviewsRepository.getReviewByBooking(event.bookingId);
            final review = reviewResult.fold((_) => null, (r) => r);

            emit(BookingDetailsLoaded(
              booking: booking,
              bookingDetails: details,
              services: services,
              isRefreshing: false,
              review: review,
            ));
          },
        );
      },
    );
  }

  Future<void> _onRefreshBookingDetails(
    RefreshBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded && _currentBookingId != null) {
      final currentState = state as BookingDetailsLoaded;
      emit(currentState.copyWith(isRefreshing: true));

      add(LoadBookingDetailsEvent(bookingId: _currentBookingId!));
    }
  }

  Future<void> _onUpdateBookingDetails(
    UpdateBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      emit(BookingDetailsOperationInProgress(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        operation: 'update',
      ));

      final result = await updateBookingUseCase(
        UpdateBookingParams(
          bookingId: event.bookingId,
          checkIn: event.checkIn,
          checkOut: event.checkOut,
          guestsCount: event.guestsCount,
        ),
      );

      await result.fold(
        (failure) async {
          emit(BookingDetailsOperationFailure(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: failure.message,
          ));
        },
        (_) async {
          emit(BookingDetailsOperationSuccess(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: 'تم تحديث الحجز بنجاح',
          ));
          // إعادة تحميل التفاصيل
          add(const RefreshBookingDetailsEvent());
        },
      );
    }
  }

  Future<void> _onCancelBookingDetails(
    CancelBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      emit(BookingDetailsOperationInProgress(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        operation: 'cancel',
      ));

      final result = await cancelBookingUseCase(
        CancelBookingParams(
          bookingId: event.bookingId,
          cancellationReason: event.cancellationReason,
        ),
      );

      await result.fold(
        (failure) async {
          emit(BookingDetailsOperationFailure(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: failure.message,
          ));
        },
        (_) async {
          emit(BookingDetailsOperationSuccess(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: 'تم إلغاء الحجز بنجاح',
          ));
          // إعادة تحميل التفاصيل
          add(const RefreshBookingDetailsEvent());
        },
      );
    }
  }

  Future<void> _onConfirmBookingDetails(
    ConfirmBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      emit(BookingDetailsOperationInProgress(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        operation: 'confirm',
      ));

      final result = await confirmBookingUseCase(
        ConfirmBookingParams(bookingId: event.bookingId),
      );

      await result.fold(
        (failure) async {
          emit(BookingDetailsOperationFailure(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: failure.message,
          ));
        },
        (_) async {
          emit(BookingDetailsOperationSuccess(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: 'تم تأكيد الحجز بنجاح',
          ));
          // إعادة تحميل التفاصيل
          add(const RefreshBookingDetailsEvent());
        },
      );
    }
  }

  Future<void> _onCheckInBookingDetails(
    CheckInBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      emit(BookingDetailsOperationInProgress(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        operation: 'checkIn',
      ));

      final result = await checkInUseCase(
        CheckInParams(bookingId: event.bookingId),
      );

      await result.fold(
        (failure) async {
          emit(BookingDetailsOperationFailure(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: failure.message,
          ));
        },
        (_) async {
          emit(BookingDetailsOperationSuccess(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: 'تم تسجيل الوصول بنجاح',
          ));
          // إعادة تحميل التفاصيل
          add(const RefreshBookingDetailsEvent());
        },
      );
    }
  }

  Future<void> _onCheckOutBookingDetails(
    CheckOutBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      emit(BookingDetailsOperationInProgress(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        operation: 'checkOut',
      ));

      final result = await checkOutUseCase(
        CheckOutParams(bookingId: event.bookingId),
      );

      await result.fold(
        (failure) async {
          emit(BookingDetailsOperationFailure(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: failure.message,
          ));
        },
        (_) async {
          emit(BookingDetailsOperationSuccess(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: 'تم تسجيل المغادرة بنجاح',
          ));
          // إعادة تحميل التفاصيل
          add(const RefreshBookingDetailsEvent());
        },
      );
    }
  }

  Future<void> _onAddService(
    AddServiceEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      emit(BookingDetailsOperationInProgress(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        operation: 'addService',
      ));

      final result = await addServiceToBookingUseCase(
        AddServiceToBookingParams(
          bookingId: event.bookingId,
          serviceId: event.serviceId,
        ),
      );

      await result.fold(
        (failure) async {
          emit(BookingDetailsOperationFailure(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: failure.message,
          ));
        },
        (_) async {
          emit(BookingDetailsOperationSuccess(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: 'تمت إضافة الخدمة بنجاح',
          ));
          // إعادة تحميل الخدمات
          add(LoadBookingServicesEvent(bookingId: event.bookingId));
        },
      );
    }
  }

  Future<void> _onRemoveService(
    RemoveServiceEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      emit(BookingDetailsOperationInProgress(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        operation: 'removeService',
      ));

      final result = await removeServiceFromBookingUseCase(
        RemoveServiceFromBookingParams(
          bookingId: event.bookingId,
          serviceId: event.serviceId,
        ),
      );

      await result.fold(
        (failure) async {
          emit(BookingDetailsOperationFailure(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: failure.message,
          ));
        },
        (_) async {
          emit(BookingDetailsOperationSuccess(
            booking: currentState.booking,
            bookingDetails: currentState.bookingDetails,
            services: currentState.services,
            message: 'تمت إزالة الخدمة بنجاح',
          ));
          // إعادة تحميل الخدمات
          add(LoadBookingServicesEvent(bookingId: event.bookingId));
        },
      );
    }
  }

  Future<void> _onLoadBookingServices(
    LoadBookingServicesEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;

      final result = await getBookingServicesUseCase(
        GetBookingServicesParams(bookingId: event.bookingId),
      );

      result.fold(
        (_) {}, // تجاهل الأخطاء
        (services) {
          emit(currentState.copyWith(services: services));
        },
      );
    }
  }

  Future<void> _onLoadBookingActivities(
    LoadBookingActivitiesEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    // يمكن تنفيذها لاحقاً إذا توفر endpoint للأنشطة
  }

  Future<void> _onLoadBookingPayments(
    LoadBookingPaymentsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    // يمكن تنفيذها لاحقاً إذا توفر endpoint للمدفوعات
  }

  Future<void> _onPrintBookingDetails(
    PrintBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;
      emit(BookingDetailsPrinting(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
      ));
      // تنفيذ منطق الطباعة
      await Future.delayed(const Duration(seconds: 2));
      emit(currentState);
    }
  }

  Future<void> _onShareBookingDetails(
    ShareBookingDetailsEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;
      emit(BookingDetailsSharing(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
      ));
      // تنفيذ منطق المشاركة
      await Future.delayed(const Duration(seconds: 1));
      emit(currentState);
    }
  }

  Future<void> _onSendBookingConfirmation(
    SendBookingConfirmationEvent event,
    Emitter<BookingDetailsState> emit,
  ) async {
    if (state is BookingDetailsLoaded) {
      final currentState = state as BookingDetailsLoaded;
      emit(BookingDetailsSendingConfirmation(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
      ));
      // تنفيذ منطق إرسال التأكيد
      await Future.delayed(const Duration(seconds: 2));
      emit(BookingDetailsOperationSuccess(
        booking: currentState.booking,
        bookingDetails: currentState.bookingDetails,
        services: currentState.services,
        message: 'تم إرسال تأكيد الحجز بنجاح',
      ));
      await Future.delayed(const Duration(seconds: 2));
      emit(currentState);
    }
  }
}
