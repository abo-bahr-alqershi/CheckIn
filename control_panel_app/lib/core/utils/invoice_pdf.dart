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

  // Professional Colors - Similar to Booking.com
  static const PdfColor primaryBlue =
      PdfColor.fromInt(0xFF003580); // Booking.com blue
  static const PdfColor darkBlue = PdfColor.fromInt(0xFF00224F);
  static const PdfColor lightBlue = PdfColor.fromInt(0xFF0077CC);
  static const PdfColor green = PdfColor.fromInt(0xFF008009);
  static const PdfColor orange = PdfColor.fromInt(0xFFFF8000);
  static const PdfColor red = PdfColor.fromInt(0xFFCC0000);

  // Neutral Colors
  static const PdfColor black = PdfColor.fromInt(0xFF262626);
  static const PdfColor darkGray = PdfColor.fromInt(0xFF333333);
  static const PdfColor gray = PdfColor.fromInt(0xFF6B6B6B);
  static const PdfColor lightGray = PdfColor.fromInt(0xFFE7E7E7);
  static const PdfColor veryLightGray = PdfColor.fromInt(0xFFF5F5F5);
  static const PdfColor white = PdfColors.white;

  static Future<Uint8List> generate(BookingDetails details) async {
    final doc = pw.Document();
    final booking = details.booking;
    final property = details.propertyDetails;
    final guest = details.guestInfo;
    final guestContact = resolveGuestContact(booking, guest);

    final currency = booking.totalPrice.currency;
    final bookingReference = _formatBookingReference(booking.id);
    final invoiceNumber = _generateInvoiceNumber(booking.id);
    final issueDate = DateTime.now();

    // Load fonts
    pw.Font arabicFont;
    pw.Font arabicBold;
    Uint8List? logoData;

    try {
      final regularData =
          await rootBundle.load('assets/fonts/Amiri-Regular.ttf');
      final boldData = await rootBundle.load('assets/fonts/Amiri-Bold.ttf');
      arabicFont = pw.Font.ttf(regularData);
      arabicBold = pw.Font.ttf(boldData);
    } catch (_) {
      arabicFont = pw.Font.helvetica();
      arabicBold = pw.Font.helveticaBold();
    }

    try {
      logoData = (await rootBundle.load('assets/images/logo.png'))
          .buffer
          .asUint8List();
    } catch (_) {
      logoData = null;
    }

    // Professional Header - Clean and Simple
    pw.Widget buildHeader() {
      return pw.Column(
        children: [
          pw.Row(
            mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
            crossAxisAlignment: pw.CrossAxisAlignment.start,
            children: [
              // Logo and Company Info
              pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  if (logoData != null)
                    pw.Image(
                      pw.MemoryImage(logoData),
                      height: 40,
                      width: 120,
                      fit: pw.BoxFit.contain,
                    )
                  else
                    pw.Text(
                      'YemenBooking',
                      style: pw.TextStyle(
                        fontSize: 24,
                        fontWeight: pw.FontWeight.bold,
                        color: primaryBlue,
                      ),
                    ),
                  pw.SizedBox(height: 4),
                  pw.Text(
                    'منصة الحجوزات الإلكترونية',
                    style: const pw.TextStyle(
                      fontSize: 10,
                      color: gray,
                    ),
                  ),
                ],
              ),
              // Invoice Title
              pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.end,
                children: [
                  pw.Container(
                    padding: const pw.EdgeInsets.symmetric(
                        horizontal: 12, vertical: 6),
                    decoration: pw.BoxDecoration(
                      color: primaryBlue,
                      borderRadius: pw.BorderRadius.circular(4),
                    ),
                    child: pw.Text(
                      'INVOICE / فاتورة',
                      style: pw.TextStyle(
                        fontSize: 14,
                        fontWeight: pw.FontWeight.bold,
                        color: white,
                      ),
                    ),
                  ),
                  pw.SizedBox(height: 8),
                  pw.Text(
                    'Invoice #: $invoiceNumber',
                    style: pw.TextStyle(
                      fontSize: 10,
                      fontWeight: pw.FontWeight.bold,
                      color: black,
                    ),
                  ),
                  pw.Text(
                    'Date: ${DateFormat('dd MMM yyyy').format(issueDate)}',
                    style: const pw.TextStyle(
                      fontSize: 10,
                      color: gray,
                    ),
                  ),
                ],
              ),
            ],
          ),
          pw.SizedBox(height: 20),
          pw.Divider(color: lightGray, thickness: 1),
        ],
      );
    }

    // Booking Confirmation Section
    pw.Widget buildBookingConfirmation() {
      return pw.Container(
        padding: const pw.EdgeInsets.all(16),
        decoration: pw.BoxDecoration(
          color: veryLightGray,
          borderRadius: pw.BorderRadius.circular(8),
          border: pw.Border.all(color: lightGray, width: 1),
        ),
        child: pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Row(
              mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
              children: [
                pw.Text(
                  'Booking Confirmation',
                  style: pw.TextStyle(
                    fontSize: 16,
                    fontWeight: pw.FontWeight.bold,
                    color: darkBlue,
                  ),
                ),
                pw.Container(
                  padding: const pw.EdgeInsets.symmetric(
                      horizontal: 10, vertical: 4),
                  decoration: pw.BoxDecoration(
                    color: _getStatusColor(booking.status.name),
                    borderRadius: pw.BorderRadius.circular(12),
                  ),
                  child: pw.Text(
                    _getStatusText(booking.status.name),
                    style: pw.TextStyle(
                      fontSize: 10,
                      fontWeight: pw.FontWeight.bold,
                      color: white,
                    ),
                  ),
                ),
              ],
            ),
            pw.SizedBox(height: 8),
            pw.Text(
              'Confirmation Number: $bookingReference',
              style: pw.TextStyle(
                fontSize: 12,
                fontWeight: pw.FontWeight.bold,
                color: primaryBlue,
              ),
            ),
          ],
        ),
      );
    }

    // Guest and Property Information
    pw.Widget buildGuestPropertyInfo() {
      return pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          // Guest Information
          pw.Expanded(
            child: pw.Column(
              crossAxisAlignment: pw.CrossAxisAlignment.start,
              children: [
                pw.Text(
                  'Guest Information',
                  style: pw.TextStyle(
                    fontSize: 12,
                    fontWeight: pw.FontWeight.bold,
                    color: darkBlue,
                  ),
                ),
                pw.SizedBox(height: 8),
                pw.Container(
                  padding: const pw.EdgeInsets.all(12),
                  decoration: pw.BoxDecoration(
                    border: pw.Border.all(color: lightGray, width: 1),
                    borderRadius: pw.BorderRadius.circular(4),
                  ),
                  child: pw.Column(
                    crossAxisAlignment: pw.CrossAxisAlignment.start,
                    children: [
                      _buildInfoRow('Name:', guestContact.name),
                      if (guestContact.phone != null)
                        _buildInfoRow('Phone:', guestContact.phone!),
                      if (guestContact.email != null)
                        _buildInfoRow('Email:', guestContact.email!),
                      _buildInfoRow('Nationality:',
                          guest?.nationality ?? 'Not specified'),
                    ],
                  ),
                ),
              ],
            ),
          ),
          pw.SizedBox(width: 20),
          // Property Information
          pw.Expanded(
            child: pw.Column(
              crossAxisAlignment: pw.CrossAxisAlignment.start,
              children: [
                pw.Text(
                  'Property Information',
                  style: pw.TextStyle(
                    fontSize: 12,
                    fontWeight: pw.FontWeight.bold,
                    color: darkBlue,
                  ),
                ),
                pw.SizedBox(height: 8),
                pw.Container(
                  padding: const pw.EdgeInsets.all(12),
                  decoration: pw.BoxDecoration(
                    border: pw.Border.all(color: lightGray, width: 1),
                    borderRadius: pw.BorderRadius.circular(4),
                  ),
                  child: pw.Column(
                    crossAxisAlignment: pw.CrossAxisAlignment.start,
                    children: [
                      _buildInfoRow(
                          'Property:', property?.name ?? 'YemenBooking'),
                      if (property?.address != null)
                        _buildInfoRow('Address:', property!.address),
                      if (property?.phone != null)
                        _buildInfoRow('Contact:', property?.phone ?? '---'),
                      _buildInfoRow('Tax ID:', '300123456789012'),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      );
    }

    // Booking Details Section
    pw.Widget buildBookingDetails() {
      final guestCount = booking.guestsCount;

      return pw.Container(
        margin: const pw.EdgeInsets.symmetric(vertical: 16),
        child: pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Text(
              'Booking Details',
              style: pw.TextStyle(
                fontSize: 14,
                fontWeight: pw.FontWeight.bold,
                color: darkBlue,
              ),
            ),
            pw.SizedBox(height: 8),
            pw.Container(
              decoration: pw.BoxDecoration(
                border: pw.Border.all(color: lightGray, width: 1),
                borderRadius: pw.BorderRadius.circular(4),
              ),
              child: pw.Column(
                children: [
                  // Header Row
                  pw.Container(
                    padding: const pw.EdgeInsets.all(12),
                    decoration: const pw.BoxDecoration(
                      color: veryLightGray,
                      border: pw.Border(
                        bottom: pw.BorderSide(color: lightGray, width: 1),
                      ),
                    ),
                    child: pw.Row(
                      children: [
                        pw.Expanded(
                          flex: 2,
                          child: pw.Text(
                            'Accommodation',
                            style: pw.TextStyle(
                              fontSize: 11,
                              fontWeight: pw.FontWeight.bold,
                              color: darkGray,
                            ),
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            'Check-in',
                            style: pw.TextStyle(
                              fontSize: 11,
                              fontWeight: pw.FontWeight.bold,
                              color: darkGray,
                            ),
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            'Check-out',
                            style: pw.TextStyle(
                              fontSize: 11,
                              fontWeight: pw.FontWeight.bold,
                              color: darkGray,
                            ),
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            'Nights',
                            style: pw.TextStyle(
                              fontSize: 11,
                              fontWeight: pw.FontWeight.bold,
                              color: darkGray,
                            ),
                            textAlign: pw.TextAlign.center,
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            'Guests',
                            style: pw.TextStyle(
                              fontSize: 11,
                              fontWeight: pw.FontWeight.bold,
                              color: darkGray,
                            ),
                            textAlign: pw.TextAlign.center,
                          ),
                        ),
                      ],
                    ),
                  ),
                  // Data Row
                  pw.Container(
                    padding: const pw.EdgeInsets.all(12),
                    child: pw.Row(
                      children: [
                        pw.Expanded(
                          flex: 2,
                          child: pw.Text(
                            booking.unitName,
                            style: const pw.TextStyle(
                              fontSize: 11,
                              color: black,
                            ),
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            DateFormat('dd MMM\nyyyy').format(booking.checkIn),
                            style: const pw.TextStyle(
                              fontSize: 10,
                              color: black,
                            ),
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            DateFormat('dd MMM\nyyyy').format(booking.checkOut),
                            style: const pw.TextStyle(
                              fontSize: 10,
                              color: black,
                            ),
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            '${booking.nights}',
                            style: const pw.TextStyle(
                              fontSize: 11,
                              color: black,
                            ),
                            textAlign: pw.TextAlign.center,
                          ),
                        ),
                        pw.Expanded(
                          child: pw.Text(
                            '$guestCount',
                            style: const pw.TextStyle(
                              fontSize: 11,
                              color: black,
                            ),
                            textAlign: pw.TextAlign.center,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    // Pricing Breakdown
    pw.Widget buildPricingBreakdown() {
      final services = details.services;
      final basePrice = booking.totalPrice.amount -
          services.fold(0.0, (sum, s) => sum + s.totalPrice.amount);

      // VAT calculation
      const vatRate = 0.15;
      final subtotal = booking.totalPrice.amount / (1 + vatRate);
      final vat = booking.totalPrice.amount - subtotal;

      return pw.Column(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Text(
            'Price Breakdown',
            style: pw.TextStyle(
              fontSize: 14,
              fontWeight: pw.FontWeight.bold,
              color: darkBlue,
            ),
          ),
          pw.SizedBox(height: 8),
          pw.Container(
            decoration: pw.BoxDecoration(
              border: pw.Border.all(color: lightGray, width: 1),
              borderRadius: pw.BorderRadius.circular(4),
            ),
            child: pw.Column(
              children: [
                // Room charges
                _buildPriceRow(
                  'Room charges (${booking.nights} nights)',
                  _formatMoney(basePrice, currency),
                  isHeader: false,
                ),
                // Additional services
                ...services.map((service) => _buildPriceRow(
                      '${service.name} (x${service.quantity})',
                      _formatMoney(service.totalPrice.amount,
                          service.totalPrice.currency),
                    )),
                // Divider
                pw.Container(
                  margin: const pw.EdgeInsets.symmetric(horizontal: 12),
                  height: 1,
                  color: lightGray,
                ),
                // Subtotal
                _buildPriceRow(
                  'Subtotal',
                  _formatMoney(subtotal, currency),
                ),
                // VAT
                _buildPriceRow(
                  'VAT (15%)',
                  _formatMoney(vat, currency),
                ),
                // Total
                pw.Container(
                  padding: const pw.EdgeInsets.all(12),
                  decoration: const pw.BoxDecoration(
                    color: veryLightGray,
                  ),
                  child: pw.Row(
                    mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                    children: [
                      pw.Text(
                        'Total Amount',
                        style: pw.TextStyle(
                          fontSize: 14,
                          fontWeight: pw.FontWeight.bold,
                          color: darkBlue,
                        ),
                      ),
                      pw.Text(
                        _formatMoney(booking.totalPrice.amount, currency),
                        style: pw.TextStyle(
                          fontSize: 14,
                          fontWeight: pw.FontWeight.bold,
                          color: darkBlue,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      );
    }

    // Payment Information
    pw.Widget buildPaymentInfo() {
      final paid = details.totalPaid;
      final remaining = details.remainingAmount;
      final payments = details.payments;

      return pw.Container(
        margin: const pw.EdgeInsets.symmetric(vertical: 16),
        child: pw.Row(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            // Payment Summary
            pw.Expanded(
              child: pw.Container(
                padding: const pw.EdgeInsets.all(12),
                decoration: pw.BoxDecoration(
                  color: remaining.amount > 0
                      ? PdfColors.orange50
                      : PdfColors.green50,
                  borderRadius: pw.BorderRadius.circular(4),
                  border: pw.Border.all(
                    color: remaining.amount > 0 ? orange : green,
                    width: 1,
                  ),
                ),
                child: pw.Column(
                  crossAxisAlignment: pw.CrossAxisAlignment.start,
                  children: [
                    pw.Text(
                      'Payment Summary',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: darkBlue,
                      ),
                    ),
                    pw.SizedBox(height: 8),
                    _buildPaymentRow('Total Amount:',
                        _formatMoney(booking.totalPrice.amount, currency)),
                    _buildPaymentRow(
                        'Amount Paid:', _formatMoney(paid.amount, currency),
                        color: green),
                    if (remaining.amount > 0)
                      _buildPaymentRow('Balance Due:',
                          _formatMoney(remaining.amount, currency),
                          color: orange),
                    pw.SizedBox(height: 8),
                    pw.Container(
                      padding: const pw.EdgeInsets.symmetric(
                          horizontal: 8, vertical: 4),
                      decoration: pw.BoxDecoration(
                        color: remaining.amount > 0 ? orange : green,
                        borderRadius: pw.BorderRadius.circular(4),
                      ),
                      child: pw.Text(
                        remaining.amount > 0
                            ? 'PAYMENT DUE AT PROPERTY'
                            : 'FULLY PAID',
                        style: pw.TextStyle(
                          fontSize: 10,
                          fontWeight: pw.FontWeight.bold,
                          color: white,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
            pw.SizedBox(width: 20),
            // Payment History
            if (payments.isNotEmpty)
              pw.Expanded(
                child: pw.Column(
                  crossAxisAlignment: pw.CrossAxisAlignment.start,
                  children: [
                    pw.Text(
                      'Payment History',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: darkBlue,
                      ),
                    ),
                    pw.SizedBox(height: 8),
                    pw.Container(
                      padding: const pw.EdgeInsets.all(12),
                      decoration: pw.BoxDecoration(
                        border: pw.Border.all(color: lightGray, width: 1),
                        borderRadius: pw.BorderRadius.circular(4),
                      ),
                      child: pw.Column(
                        children: payments
                            .map((payment) => pw.Container(
                                  padding: const pw.EdgeInsets.symmetric(
                                      vertical: 4),
                                  decoration: const pw.BoxDecoration(
                                    border: pw.Border(
                                      bottom: pw.BorderSide(
                                          color: lightGray, width: 0.5),
                                    ),
                                  ),
                                  child: pw.Row(
                                    mainAxisAlignment:
                                        pw.MainAxisAlignment.spaceBetween,
                                    children: [
                                      pw.Text(
                                        DateFormat('dd MMM yyyy')
                                            .format(payment.paymentDate),
                                        style: const pw.TextStyle(
                                            fontSize: 10, color: gray),
                                      ),
                                      pw.Text(
                                        _formatMoney(payment.amount.amount,
                                            payment.amount.currency),
                                        style: pw.TextStyle(
                                          fontSize: 10,
                                          fontWeight: pw.FontWeight.bold,
                                          color: green,
                                        ),
                                      ),
                                    ],
                                  ),
                                ))
                            .toList(),
                      ),
                    ),
                  ],
                ),
              ),
          ],
        ),
      );
    }

    // Important Information
    pw.Widget buildImportantInfo() {
      return pw.Container(
        padding: const pw.EdgeInsets.all(12),
        decoration: pw.BoxDecoration(
          color: PdfColors.blue50,
          borderRadius: pw.BorderRadius.circular(4),
          border: pw.Border.all(color: lightBlue, width: 1),
        ),
        child: pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Row(
              children: [
                pw.Container(
                  width: 20,
                  height: 20,
                  decoration: const pw.BoxDecoration(
                    color: lightBlue,
                    shape: pw.BoxShape.circle,
                  ),
                  child: pw.Center(
                    child: pw.Text(
                      'i',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: white,
                      ),
                    ),
                  ),
                ),
                pw.SizedBox(width: 8),
                pw.Text(
                  'Important Information',
                  style: pw.TextStyle(
                    fontSize: 12,
                    fontWeight: pw.FontWeight.bold,
                    color: darkBlue,
                  ),
                ),
              ],
            ),
            pw.SizedBox(height: 8),
            _buildBulletPoint('Check-in time: 14:00 - Check-out time: 12:00'),
            _buildBulletPoint('Valid ID required at check-in'),
            _buildBulletPoint('This invoice includes 15% VAT'),
            _buildBulletPoint(
                'Cancellation policy applies as per booking terms'),
            _buildBulletPoint(
                'For assistance, contact our 24/7 customer service'),
          ],
        ),
      );
    }

    // Footer
    pw.Widget buildFooter() {
      return pw.Container(
        margin: const pw.EdgeInsets.only(top: 20),
        padding: const pw.EdgeInsets.only(top: 16),
        decoration: const pw.BoxDecoration(
          border: pw.Border(
            top: pw.BorderSide(color: lightGray, width: 1),
          ),
        ),
        child: pw.Column(
          children: [
            pw.Row(
              mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
              children: [
                pw.Column(
                  crossAxisAlignment: pw.CrossAxisAlignment.start,
                  children: [
                    pw.Text(
                      'YemenBooking Platform',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: primaryBlue,
                      ),
                    ),
                    pw.SizedBox(height: 4),
                    pw.Text(
                      'www.yemenbooking.com',
                      style: const pw.TextStyle(fontSize: 10, color: gray),
                    ),
                    pw.Text(
                      'support@yemenbooking.com',
                      style: const pw.TextStyle(fontSize: 10, color: gray),
                    ),
                  ],
                ),
                pw.Column(
                  crossAxisAlignment: pw.CrossAxisAlignment.center,
                  children: [
                    pw.Text(
                      'Customer Service',
                      style: pw.TextStyle(
                        fontSize: 11,
                        fontWeight: pw.FontWeight.bold,
                        color: darkGray,
                      ),
                    ),
                    pw.SizedBox(height: 4),
                    pw.Text(
                      '+967 777 123 456',
                      style: const pw.TextStyle(fontSize: 10, color: gray),
                    ),
                    pw.Text(
                      'Available 24/7',
                      style: const pw.TextStyle(fontSize: 9, color: gray),
                    ),
                  ],
                ),
                pw.BarcodeWidget(
                  barcode: pw.Barcode.qrCode(),
                  data:
                      'BOOKING:$bookingReference|AMOUNT:${booking.totalPrice.amount}|INV:$invoiceNumber',
                  width: 60,
                  height: 60,
                ),
              ],
            ),
            pw.SizedBox(height: 12),
            pw.Container(
              padding: const pw.EdgeInsets.all(8),
              decoration: pw.BoxDecoration(
                color: veryLightGray,
                borderRadius: pw.BorderRadius.circular(4),
              ),
              child: pw.Center(
                child: pw.Text(
                  'Thank you for choosing YemenBooking. We wish you a pleasant stay!',
                  style: pw.TextStyle(
                    fontSize: 11,
                    fontWeight: pw.FontWeight.bold,
                    color: darkBlue,
                  ),
                ),
              ),
            ),
            pw.SizedBox(height: 8),
            pw.Text(
              '© ${DateTime.now().year} YemenBooking. All rights reserved. This is a computer-generated invoice.',
              style: const pw.TextStyle(fontSize: 8, color: gray),
              textAlign: pw.TextAlign.center,
            ),
          ],
        ),
      );
    }

    // Build PDF Document
    doc.addPage(
      pw.MultiPage(
        pageTheme: pw.PageTheme(
          margin: const pw.EdgeInsets.all(40),
          theme: pw.ThemeData.withFont(
            base: arabicFont,
            bold: arabicBold,
          ),
          pageFormat: PdfPageFormat.a4,
        ),
        build: (context) => [
          buildHeader(),
          buildBookingConfirmation(),
          pw.SizedBox(height: 16),
          buildGuestPropertyInfo(),
          buildBookingDetails(),
          buildPricingBreakdown(),
          buildPaymentInfo(),
          buildImportantInfo(),
          buildFooter(),
        ],
      ),
    );

    return doc.save();
  }

  // Helper Methods
  static pw.Widget _buildInfoRow(String label, String value) {
    return pw.Container(
      padding: const pw.EdgeInsets.symmetric(vertical: 2),
      child: pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.SizedBox(
            width: 80,
            child: pw.Text(
              label,
              style: const pw.TextStyle(
                fontSize: 10,
                color: gray,
              ),
            ),
          ),
          pw.Expanded(
            child: pw.Text(
              value,
              style: pw.TextStyle(
                fontSize: 10,
                fontWeight: pw.FontWeight.bold,
                color: black,
              ),
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildPriceRow(String label, String amount,
      {bool isHeader = false}) {
    return pw.Container(
      padding: const pw.EdgeInsets.all(12),
      decoration: pw.BoxDecoration(
        color: isHeader ? veryLightGray : white,
        border: const pw.Border(
          bottom: pw.BorderSide(color: lightGray, width: 0.5),
        ),
      ),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            label,
            style: pw.TextStyle(
              fontSize: 11,
              fontWeight: isHeader ? pw.FontWeight.bold : pw.FontWeight.normal,
              color: isHeader ? darkGray : black,
            ),
          ),
          pw.Text(
            amount,
            style: pw.TextStyle(
              fontSize: 11,
              fontWeight: isHeader ? pw.FontWeight.bold : pw.FontWeight.normal,
              color: isHeader ? darkGray : black,
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildPaymentRow(String label, String amount,
      {PdfColor? color}) {
    return pw.Container(
      padding: const pw.EdgeInsets.symmetric(vertical: 2),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            label,
            style: const pw.TextStyle(
              fontSize: 10,
              color: gray,
            ),
          ),
          pw.Text(
            amount,
            style: pw.TextStyle(
              fontSize: 10,
              fontWeight: pw.FontWeight.bold,
              color: color ?? black,
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildBulletPoint(String text) {
    return pw.Container(
      padding: const pw.EdgeInsets.symmetric(vertical: 2),
      child: pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Container(
            width: 4,
            height: 4,
            margin: const pw.EdgeInsets.only(top: 4, right: 8),
            decoration: const pw.BoxDecoration(
              color: gray,
              shape: pw.BoxShape.circle,
            ),
          ),
          pw.Expanded(
            child: pw.Text(
              text,
              style: const pw.TextStyle(
                fontSize: 10,
                color: darkGray,
              ),
            ),
          ),
        ],
      ),
    );
  }

  static String _formatMoney(double amount, String currency) {
    final formatter = NumberFormat('#,##0.00');
    return '$currency ${formatter.format(amount)}';
  }

  static String _getStatusText(String status) {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'CONFIRMED';
      case 'pending':
        return 'PENDING';
      case 'cancelled':
        return 'CANCELLED';
      case 'completed':
        return 'COMPLETED';
      default:
        return status.toUpperCase();
    }
  }

  static PdfColor _getStatusColor(String status) {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return green;
      case 'pending':
        return orange;
      case 'cancelled':
        return red;
      case 'completed':
        return primaryBlue;
      default:
        return gray;
    }
  }

  static String _generateInvoiceNumber(String bookingId) {
    final year = DateTime.now().year;
    final month = DateTime.now().month.toString().padLeft(2, '0');
    final day = DateTime.now().day.toString().padLeft(2, '0');
    final hash = bookingId.hashCode.abs() % 100000;
    return 'INV$year$month$day${hash.toString().padLeft(5, '0')}';
  }

  static String _formatBookingReference(String bookingId) {
    if (bookingId.isEmpty) return 'N/A';
    // Format like Booking.com: XXX-XXX-XXXX
    final hash = bookingId.hashCode.abs().toString();
    if (hash.length >= 10) {
      return '${hash.substring(0, 3)}-${hash.substring(3, 6)}-${hash.substring(6, 10)}';
    }
    return hash.padRight(10, '0');
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
        'Guest';

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
