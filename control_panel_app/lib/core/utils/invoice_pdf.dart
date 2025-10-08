import 'dart:typed_data';
import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart' show rootBundle;
import 'package:intl/intl.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import '../../features/admin_bookings/domain/entities/booking.dart';
import '../../features/admin_bookings/domain/entities/booking_details.dart';
import '../utils/formatters.dart';

class InvoicePdfGenerator {
  InvoicePdfGenerator._();

  static Future<Uint8List> generate(BookingDetails details) async {
    final doc = pw.Document();
    final booking = details.booking;
    final property = details.propertyDetails;
    final guest = details.guestInfo;
    final guestContact = resolveGuestContact(booking, guest);

    final currency = booking.totalPrice.currency;
    final bookingReference = _formatBookingReference(booking.id);

    pw.Widget buildHeader() {
      return pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Column(
            crossAxisAlignment: pw.CrossAxisAlignment.start,
            children: [
              pw.Text(property?.name ?? 'YemenBooking',
                  style: pw.TextStyle(
                    fontSize: 18,
                    fontWeight: pw.FontWeight.bold,
                  )),
              if (property?.address != null)
                pw.Text(property!.address,
                    style: const pw.TextStyle(fontSize: 10)),
              if (property?.phone != null)
                pw.Text('هاتف: ${property!.phone}',
                    style: const pw.TextStyle(fontSize: 10)),
              if (property?.email != null)
                pw.Text('بريد: ${property!.email}',
                    style: const pw.TextStyle(fontSize: 10)),
            ],
          ),
          pw.Column(
            crossAxisAlignment: pw.CrossAxisAlignment.end,
            children: [
              pw.Text('فاتورة / Invoice',
                  style: pw.TextStyle(
                      fontSize: 18, fontWeight: pw.FontWeight.bold)),
              pw.SizedBox(height: 4),
              pw.Text('رقم الحجز: $bookingReference'),
              pw.Text(
                  'التاريخ: ${DateFormat('yyyy-MM-dd').format(DateTime.now())}'),
            ],
          )
        ],
      );
    }

    pw.Widget buildGuestPropertyInfo() {
      return pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Expanded(
            child: pw.Column(
              crossAxisAlignment: pw.CrossAxisAlignment.start,
              children: [
                pw.Text('بيانات الضيف',
                    style: pw.TextStyle(
                        fontSize: 12, fontWeight: pw.FontWeight.bold)),
                pw.SizedBox(height: 6),
                pw.Text('الاسم: ${guestContact.name}'),
                if (guestContact.phone != null)
                  pw.Text('الهاتف: ${guestContact.phone}'),
                if (guestContact.email != null)
                  pw.Text('البريد: ${guestContact.email}'),
              ],
            ),
          ),
          pw.SizedBox(width: 16),
          pw.Expanded(
            child: pw.Column(
              crossAxisAlignment: pw.CrossAxisAlignment.start,
              children: [
                pw.Text('بيانات الحجز',
                    style: pw.TextStyle(
                        fontSize: 12, fontWeight: pw.FontWeight.bold)),
                pw.SizedBox(height: 6),
                pw.Text('الوحدة: ${booking.unitName}'),
                pw.Text('الوصول: ${Formatters.formatDate(booking.checkIn)}'),
                pw.Text('المغادرة: ${Formatters.formatDate(booking.checkOut)}'),
                pw.Text('الليالي: ${booking.nights}')
              ],
            ),
          ),
        ],
      );
    }

    pw.Widget buildServicesTable() {
      final services = details.services;
      if (services.isEmpty) {
        return pw.Container();
      }
      return pw.Column(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Text('الخدمات الإضافية',
              style:
                  pw.TextStyle(fontSize: 12, fontWeight: pw.FontWeight.bold)),
          pw.SizedBox(height: 6),
          pw.Table(
            border: pw.TableBorder.all(color: PdfColors.grey300, width: 0.5),
            columnWidths: const {
              0: pw.FlexColumnWidth(5),
              1: pw.FlexColumnWidth(2),
              2: pw.FlexColumnWidth(2),
              3: pw.FlexColumnWidth(2),
            },
            children: [
              pw.TableRow(
                decoration:
                    const pw.BoxDecoration(color: PdfColor.fromInt(0xFFEFEFEF)),
                children: [
                  _cell('الخدمة', bold: true),
                  _cell('الكمية', bold: true),
                  _cell('السعر', bold: true),
                  _cell('الإجمالي', bold: true),
                ],
              ),
              ...services.map((s) => pw.TableRow(children: [
                    _cell(s.name),
                    _cell('${s.quantity}'),
                    _cell(
                        '${s.price.currency} ${s.price.amount.toStringAsFixed(2)}'),
                    _cell(
                        '${s.totalPrice.currency} ${s.totalPrice.amount.toStringAsFixed(2)}'),
                  ]))
            ],
          )
        ],
      );
    }

    pw.Widget buildTotals() {
      final total = booking.totalPrice;
      final paid = details.totalPaid;
      final remaining = details.remainingAmount;
      pw.Widget row(String label, String value, {bool bold = false}) => pw.Row(
            mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
            children: [
              pw.Text(label,
                  style: pw.TextStyle(
                      fontSize: 12,
                      fontWeight:
                          bold ? pw.FontWeight.bold : pw.FontWeight.normal)),
              pw.Text(value,
                  style: pw.TextStyle(
                      fontSize: 12,
                      fontWeight:
                          bold ? pw.FontWeight.bold : pw.FontWeight.normal)),
            ],
          );

      return pw.Container(
        padding: const pw.EdgeInsets.all(12),
        decoration: pw.BoxDecoration(
          border: pw.Border.all(color: PdfColors.grey300, width: 0.5),
          borderRadius: pw.BorderRadius.circular(6),
        ),
        child: pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            row('الإجمالي', '$currency ${total.amount.toStringAsFixed(2)}',
                bold: true),
            pw.SizedBox(height: 4),
            row('المدفوع', '$currency ${paid.amount.toStringAsFixed(2)}'),
            pw.SizedBox(height: 4),
            row('المتبقي', '$currency ${remaining.amount.toStringAsFixed(2)}'),
          ],
        ),
      );
    }

    // Load bundled fonts to avoid network calls (offline-safe)
    // Fallback gracefully to Helvetica if assets missing
    pw.Font arabicFont;
    pw.Font arabicBold;
    try {
      final regularData =
          await rootBundle.load('assets/fonts/Amiri-Regular.ttf');
      final boldData = await rootBundle.load('assets/fonts/Amiri-Bold.ttf');
      arabicFont = pw.Font.ttf(regularData);
      arabicBold = pw.Font.ttf(boldData);
    } catch (_) {
      // Fallback fonts
      arabicFont = pw.Font.helvetica();
      arabicBold = pw.Font.helveticaBold();
    }

    doc.addPage(
      pw.MultiPage(
        pageTheme: pw.PageTheme(
          margin: const pw.EdgeInsets.all(24),
          theme: pw.ThemeData.withFont(
            base: arabicFont,
            bold: arabicBold,
          ),
          textDirection: pw.TextDirection.rtl,
        ),
        build: (context) => [
          buildHeader(),
          pw.SizedBox(height: 16),
          buildGuestPropertyInfo(),
          pw.SizedBox(height: 16),
          pw.Divider(color: PdfColors.grey300),
          pw.SizedBox(height: 12),
          pw.Row(
            crossAxisAlignment: pw.CrossAxisAlignment.start,
            children: [
              pw.Expanded(child: buildServicesTable()),
              pw.SizedBox(width: 16),
              pw.SizedBox(width: 200, child: buildTotals()),
            ],
          ),
          pw.SizedBox(height: 24),
          pw.Text('شكرًا لاختياركم منصتنا لحجوزاتكم.',
              style: const pw.TextStyle(fontSize: 10)),
        ],
      ),
    );

    return doc.save();
  }

  static pw.Widget _cell(String text, {bool bold = false}) {
    return pw.Padding(
      padding: const pw.EdgeInsets.symmetric(horizontal: 8, vertical: 6),
      child: pw.Text(
        text,
        style: pw.TextStyle(
            fontSize: 10,
            fontWeight: bold ? pw.FontWeight.bold : pw.FontWeight.normal),
      ),
    );
  }

  static String _formatBookingReference(String bookingId) {
    if (bookingId.isEmpty) {
      return 'غير متوفر';
    }

    const maxLength = 8;
    if (bookingId.length <= maxLength) {
      return bookingId;
    }

    return bookingId.substring(0, maxLength);
  }

  @visibleForTesting
  static ({String name, String? phone, String? email}) resolveGuestContact(
    Booking booking,
    GuestInfo? guest,
  ) {
    final name = _coalesceNonEmpty([
          guest?.name,
          booking.userName,
        ]) ??
        'غير متوفر';

    final phone = _coalesceNonEmpty([
      guest?.phone,
      booking.userPhone,
    ]);

    final email = _coalesceNonEmpty([
      guest?.email,
      booking.userEmail,
    ]);

    return (
      name: name,
      phone: phone != null ? Formatters.formatPhoneNumber(phone) : null,
      email: email,
    );
  }

  static String? _coalesceNonEmpty(List<String?> values) {
    for (final value in values) {
      final normalized = _normalize(value);
      if (normalized != null) {
        return normalized;
      }
    }
    return null;
  }

  static String? _normalize(String? value) {
    if (value == null) return null;
    final trimmed = value.trim();
    if (trimmed.isEmpty) return null;
    return trimmed;
  }
}
