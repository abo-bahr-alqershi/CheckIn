import 'package:bookn_cp_app/core/utils/invoice_pdf.dart';
import 'package:bookn_cp_app/core/enums/booking_status.dart';
import 'package:bookn_cp_app/features/admin_bookings/domain/entities/booking.dart';
import 'package:bookn_cp_app/features/admin_bookings/domain/entities/booking_details.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('InvoicePdfGenerator', () {
    test('generates invoice when booking id shorter than 8 chars', () async {
      const bookingId = 'ABC123';

      final booking = Booking(
        id: bookingId,
        userId: 'user-1',
        unitId: 'unit-1',
        checkIn: DateTime(2024, 1, 1),
        checkOut: DateTime(2024, 1, 5),
        guestsCount: 2,
        totalPrice: const Money(
          amount: 100.0,
          currency: 'USD',
          formattedAmount: 'USD 100.00',
        ),
        status: BookingStatus.confirmed,
        bookedAt: DateTime(2023, 12, 1),
        userName: 'Test User',
        unitName: 'Test Unit',
      );

      final details = BookingDetails(
        booking: booking,
        payments: const [],
        services: const [],
      );

      final pdfBytes = await InvoicePdfGenerator.generate(details);

      expect(pdfBytes, isNotEmpty);
    });
  });
}
