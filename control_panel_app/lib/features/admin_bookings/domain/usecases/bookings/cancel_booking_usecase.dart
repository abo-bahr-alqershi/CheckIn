import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';
import '../../../../../../core/error/failures.dart';
import '../../../../../../core/usecases/usecase.dart';
import '../../repositories/bookings_repository.dart';

class CancelBookingUseCase implements UseCase<bool, CancelBookingParams> {
  final BookingsRepository repository;

  CancelBookingUseCase(this.repository);

  @override
  Future<Either<Failure, bool>> call(CancelBookingParams params) async {
    return await repository.cancelBooking(
      bookingId: params.bookingId,
      cancellationReason: params.cancellationReason,
    );
  }
}

class CancelBookingParams extends Equatable {
  final String bookingId;
  final String cancellationReason;

  const CancelBookingParams({
    required this.bookingId,
    required this.cancellationReason,
  });

  @override
  List<Object> get props => [bookingId, cancellationReason];
}
