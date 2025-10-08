import 'dart:typed_data';
import 'dart:math' as math;
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

  // 🎨 Brand Colors - متوافقة مع AppTheme
  static const PdfColor primaryCyan = PdfColor.fromInt(0xFF00F2FE);
  static const PdfColor primaryBlue = PdfColor.fromInt(0xFF4FACFE);
  static const PdfColor primaryPurple = PdfColor.fromInt(0xFF667EEA);
  static const PdfColor primaryViolet = PdfColor.fromInt(0xFF764BA2);

  // 🌟 Neon & Glow Colors
  static const PdfColor neonBlue = PdfColor.fromInt(0xFF00D4FF);
  static const PdfColor neonPurple = PdfColor.fromInt(0xFF9D50FF);
  static const PdfColor neonGreen = PdfColor.fromInt(0xFF00FF88);

  // 🌙 Dark Theme Colors
  static const PdfColor darkBackground = PdfColor.fromInt(0xFF0A0E27);
  static const PdfColor darkBackground2 = PdfColor.fromInt(0xFF0F1629);
  static const PdfColor darkSurface = PdfColor.fromInt(0xFF151930);
  static const PdfColor darkCard = PdfColor.fromInt(0xFF1E2341);
  static const PdfColor darkBorder = PdfColor.fromInt(0xFF2A3050);

  // 📝 Text Colors
  static const PdfColor textWhite = PdfColors.white;
  static const PdfColor textLight = PdfColor.fromInt(0xFFB8C4E6);
  static const PdfColor textMuted = PdfColor.fromInt(0xFF8B95B7);
  static const PdfColor textDark = PdfColor.fromInt(0xFF1A1F36);

  // ✨ Glass & Effects
  static const PdfColor glassDark = PdfColor.fromInt(0x1A000000);
  static const PdfColor glassLight = PdfColor.fromInt(0x0DFFFFFF);
  static const PdfColor glassOverlay = PdfColor.fromInt(0x80151930);

  // 🚦 Status Colors
  static const PdfColor success = PdfColor.fromInt(0xFF00FF88);
  static const PdfColor warning = PdfColor.fromInt(0xFFFFB800);
  static const PdfColor error = PdfColor.fromInt(0xFFFF3366);
  static const PdfColor info = PdfColor.fromInt(0xFF00D4FF);

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

    // Professional Modern Header with Gradient
    pw.Widget buildProfessionalHeader() {
      return pw.Container(
        margin: const pw.EdgeInsets.only(bottom: 20),
        child: pw.Column(
          children: [
            // Top gradient bar
            pw.Container(
              height: 4,
              decoration: const pw.BoxDecoration(
                gradient: pw.LinearGradient(
                  colors: [
                    primaryCyan,
                    primaryBlue,
                    primaryPurple,
                    primaryViolet
                  ],
                  begin: pw.Alignment.centerLeft,
                  end: pw.Alignment.centerRight,
                ),
              ),
            ),
            pw.Container(
              decoration: const pw.BoxDecoration(
                color: darkBackground2,
                border: pw.Border(
                  bottom: pw.BorderSide(color: darkBorder, width: 1),
                ),
              ),
              padding: const pw.EdgeInsets.all(24),
              child: pw.Row(
                mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  // Company Info
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
                        pw.Container(
                          padding: const pw.EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 6,
                          ),
                          decoration: pw.BoxDecoration(
                            gradient: const pw.LinearGradient(
                              colors: [primaryBlue, primaryPurple],
                              begin: pw.Alignment.topLeft,
                              end: pw.Alignment.bottomRight,
                            ),
                            borderRadius: pw.BorderRadius.circular(8),
                          ),
                          child: pw.Text(
                            property?.name ?? 'YemenBooking',
                            style: pw.TextStyle(
                              fontSize: 18,
                              fontWeight: pw.FontWeight.bold,
                              color: textWhite,
                            ),
                          ),
                        ),
                      pw.SizedBox(height: 12),
                      if (property?.address != null)
                        _buildCompanyInfoRow(
                            'العنوان:', property?.address ?? '---'),
                      if (property?.phone != null)
                        _buildCompanyInfoRow(
                            'الهاتف:', property?.phone ?? '---'),
                      if (property?.email != null)
                        _buildCompanyInfoRow(
                            'البريد:', property?.email ?? '---'),
                      pw.SizedBox(height: 8),
                      pw.Text(
                        'الرقم الضريبي: 300123456789012',
                        style: const pw.TextStyle(
                          fontSize: 9,
                          color: textMuted,
                        ),
                      ),
                    ],
                  ),
                  // Invoice Details
                  pw.Column(
                    crossAxisAlignment: pw.CrossAxisAlignment.end,
                    children: [
                      pw.Container(
                        padding: const pw.EdgeInsets.symmetric(
                          horizontal: 16,
                          vertical: 8,
                        ),
                        decoration: pw.BoxDecoration(
                          color: darkCard,
                          borderRadius: pw.BorderRadius.circular(8),
                        ),
                        child: pw.Column(
                          crossAxisAlignment: pw.CrossAxisAlignment.end,
                          children: [
                            pw.Text(
                              'فاتورة ضريبية',
                              style: pw.TextStyle(
                                fontSize: 14,
                                fontWeight: pw.FontWeight.bold,
                                color: neonBlue,
                              ),
                            ),
                            pw.Text(
                              'Tax Invoice',
                              style: const pw.TextStyle(
                                fontSize: 10,
                                color: textLight,
                              ),
                            ),
                          ],
                        ),
                      ),
                      pw.SizedBox(height: 12),
                      _buildInvoiceDetailRow('رقم الفاتورة', invoiceNumber,
                          highlight: true),
                      _buildInvoiceDetailRow('رقم المرجع', bookingReference),
                      _buildInvoiceDetailRow('التاريخ',
                          DateFormat('yyyy/MM/dd').format(issueDate)),
                      _buildInvoiceDetailRow(
                          'الوقت', DateFormat('HH:mm').format(issueDate)),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    // Guest & Booking Information with Modern Cards
    pw.Widget buildModernInfoCards() {
      final guestCount = booking.guestsCount;

      return pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          // Guest Card
          pw.Expanded(
            child: pw.Container(
              decoration: pw.BoxDecoration(
                gradient: pw.LinearGradient(
                  colors: [
                    darkCard,
                    darkCard.shade(0.8),
                  ],
                  begin: pw.Alignment.topLeft,
                  end: pw.Alignment.bottomRight,
                ),
                borderRadius: pw.BorderRadius.circular(12),
              ),
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  pw.Container(
                    padding: const pw.EdgeInsets.symmetric(
                      horizontal: 16,
                      vertical: 10,
                    ),
                    decoration: pw.BoxDecoration(
                      gradient: pw.LinearGradient(
                        colors: [
                          primaryBlue.shade(0.2),
                          primaryPurple.shade(0.2)
                        ],
                        begin: pw.Alignment.centerLeft,
                        end: pw.Alignment.centerRight,
                      ),
                      borderRadius: const pw.BorderRadius.only(
                        topLeft: pw.Radius.circular(12),
                        topRight: pw.Radius.circular(12),
                      ),
                    ),
                    child: pw.Row(
                      children: [
                        pw.Container(
                          padding: const pw.EdgeInsets.all(4),
                          decoration: pw.BoxDecoration(
                            color: textWhite.shade(0.1),
                            shape: pw.BoxShape.circle,
                          ),
                          child: pw.Text(
                            '1',
                            style: pw.TextStyle(
                              fontSize: 10,
                              fontWeight: pw.FontWeight.bold,
                              color: textWhite,
                            ),
                          ),
                        ),
                        pw.SizedBox(width: 8),
                        pw.Text(
                          'معلومات العميل',
                          style: pw.TextStyle(
                            fontSize: 12,
                            fontWeight: pw.FontWeight.bold,
                            color: textWhite,
                          ),
                        ),
                      ],
                    ),
                  ),
                  pw.Container(
                    padding: const pw.EdgeInsets.all(16),
                    child: pw.Column(
                      crossAxisAlignment: pw.CrossAxisAlignment.start,
                      children: [
                        _buildInfoField('الاسم الكامل', guestContact.name),
                        if (guestContact.phone != null)
                          _buildInfoField('رقم الهاتف', guestContact.phone!),
                        if (guestContact.email != null)
                          _buildInfoField(
                              'البريد الإلكتروني', guestContact.email!),
                        _buildInfoField(
                            'الجنسية', guest?.nationality ?? 'غير محدد'),
                        _buildInfoField('نوع الهوية', 'جواز سفر'),
                        _buildInfoField('رقم الهوية', '********'),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
          pw.SizedBox(width: 16),
          // Booking Card
          pw.Expanded(
            child: pw.Container(
              decoration: pw.BoxDecoration(
                gradient: pw.LinearGradient(
                  colors: [
                    darkCard,
                    darkCard.shade(0.8),
                  ],
                  begin: pw.Alignment.topLeft,
                  end: pw.Alignment.bottomRight,
                ),
                borderRadius: pw.BorderRadius.circular(12),
              ),
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  pw.Container(
                    padding: const pw.EdgeInsets.symmetric(
                      horizontal: 16,
                      vertical: 10,
                    ),
                    decoration: pw.BoxDecoration(
                      gradient: pw.LinearGradient(
                        colors: [
                          primaryPurple.shade(0.2),
                          primaryViolet.shade(0.2)
                        ],
                        begin: pw.Alignment.centerLeft,
                        end: pw.Alignment.centerRight,
                      ),
                      borderRadius: const pw.BorderRadius.only(
                        topLeft: pw.Radius.circular(12),
                        topRight: pw.Radius.circular(12),
                      ),
                    ),
                    child: pw.Row(
                      children: [
                        pw.Container(
                          padding: const pw.EdgeInsets.all(4),
                          decoration: pw.BoxDecoration(
                            color: textWhite.shade(0.1),
                            shape: pw.BoxShape.circle,
                          ),
                          child: pw.Text(
                            '2',
                            style: pw.TextStyle(
                              fontSize: 10,
                              fontWeight: pw.FontWeight.bold,
                              color: textWhite,
                            ),
                          ),
                        ),
                        pw.SizedBox(width: 8),
                        pw.Text(
                          'تفاصيل الحجز',
                          style: pw.TextStyle(
                            fontSize: 12,
                            fontWeight: pw.FontWeight.bold,
                            color: textWhite,
                          ),
                        ),
                      ],
                    ),
                  ),
                  pw.Container(
                    padding: const pw.EdgeInsets.all(16),
                    child: pw.Column(
                      crossAxisAlignment: pw.CrossAxisAlignment.start,
                      children: [
                        _buildInfoField('الوحدة المحجوزة', booking.unitName),
                        _buildInfoField('تاريخ الوصول',
                            Formatters.formatDate(booking.checkIn)),
                        _buildInfoField('تاريخ المغادرة',
                            Formatters.formatDate(booking.checkOut)),
                        _buildInfoField(
                            'مدة الإقامة', '${booking.nights} ليلة'),
                        _buildInfoField('عدد النزلاء', '$guestCount شخص'),
                        _buildStatusField(booking.status.name),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      );
    }

    // Professional Pricing Table
    pw.Widget buildProfessionalPricingTable() {
      final services = details.services;
      final basePrice = booking.totalPrice.amount -
          services.fold(0.0, (sum, s) => sum + s.totalPrice.amount);

      // VAT calculation
      const vatRate = 0.15;
      final subtotal = booking.totalPrice.amount / (1 + vatRate);
      final vat = booking.totalPrice.amount - subtotal;

      return pw.Container(
        decoration: pw.BoxDecoration(
          color: darkCard,
          borderRadius: pw.BorderRadius.circular(12),
        ),
        child: pw.Column(
          children: [
            // Table Header
            pw.Container(
              padding: const pw.EdgeInsets.symmetric(
                horizontal: 16,
                vertical: 12,
              ),
              decoration: const pw.BoxDecoration(
                gradient: pw.LinearGradient(
                  colors: [darkSurface, darkCard],
                  begin: pw.Alignment.topCenter,
                  end: pw.Alignment.bottomCenter,
                ),
                borderRadius: pw.BorderRadius.only(
                  topLeft: pw.Radius.circular(12),
                  topRight: pw.Radius.circular(12),
                ),
                border: pw.Border(
                  bottom: pw.BorderSide(color: darkBorder, width: 1),
                ),
              ),
              child: pw.Row(
                children: [
                  pw.Container(
                    padding: const pw.EdgeInsets.symmetric(
                        horizontal: 8, vertical: 4),
                    decoration: pw.BoxDecoration(
                      color: primaryBlue.shade(0.2),
                      borderRadius: pw.BorderRadius.circular(4),
                    ),
                    child: pw.Text(
                      '3',
                      style: pw.TextStyle(
                        fontSize: 10,
                        fontWeight: pw.FontWeight.bold,
                        color: neonBlue,
                      ),
                    ),
                  ),
                  pw.SizedBox(width: 8),
                  pw.Text(
                    'بيان الأسعار',
                    style: pw.TextStyle(
                      fontSize: 13,
                      fontWeight: pw.FontWeight.bold,
                      color: textWhite,
                    ),
                  ),
                ],
              ),
            ),
            // Table Content
            pw.Table(
              columnWidths: const {
                0: pw.FlexColumnWidth(4),
                1: pw.FlexColumnWidth(1.5),
                2: pw.FlexColumnWidth(2),
                3: pw.FlexColumnWidth(2),
              },
              children: [
                // Header Row
                pw.TableRow(
                  decoration: pw.BoxDecoration(
                    color: darkSurface,
                    border: pw.Border(
                      bottom: pw.BorderSide(
                          color: primaryBlue.shade(0.2), width: 1),
                    ),
                  ),
                  children: [
                    _buildTableHeader('الوصف'),
                    _buildTableHeader('الكمية', align: pw.TextAlign.center),
                    _buildTableHeader('السعر', align: pw.TextAlign.center),
                    _buildTableHeader('الإجمالي', align: pw.TextAlign.left),
                  ],
                ),
                // Base Price Row
                pw.TableRow(
                  decoration: pw.BoxDecoration(
                    border: pw.Border(
                      bottom: pw.BorderSide(
                          color: darkBorder.shade(0.5), width: 0.5),
                    ),
                  ),
                  children: [
                    _buildTableCell(
                        'إيجار الوحدة السكنية\nالفترة: ${booking.nights} ليلة'),
                    _buildTableCell('1', align: pw.TextAlign.center),
                    _buildTableCell(
                      _formatMoney(basePrice / booking.nights, currency),
                      align: pw.TextAlign.center,
                    ),
                    _buildTableCell(
                      _formatMoney(basePrice, currency),
                      align: pw.TextAlign.left,
                      bold: true,
                    ),
                  ],
                ),
                // Services
                ...services.map((s) => pw.TableRow(
                      decoration: pw.BoxDecoration(
                        border: pw.Border(
                          bottom: pw.BorderSide(
                              color: darkBorder.shade(0.5), width: 0.5),
                        ),
                      ),
                      children: [
                        _buildTableCell(s.name),
                        _buildTableCell('${s.quantity}',
                            align: pw.TextAlign.center),
                        _buildTableCell(
                          _formatMoney(s.price.amount, s.price.currency),
                          align: pw.TextAlign.center,
                        ),
                        _buildTableCell(
                          _formatMoney(
                              s.totalPrice.amount, s.totalPrice.currency),
                          align: pw.TextAlign.left,
                          bold: true,
                        ),
                      ],
                    )),
              ],
            ),
            // Totals Section
            pw.Container(
              padding: const pw.EdgeInsets.all(16),
              decoration: const pw.BoxDecoration(
                color: darkSurface,
                borderRadius: pw.BorderRadius.only(
                  bottomLeft: pw.Radius.circular(12),
                  bottomRight: pw.Radius.circular(12),
                ),
              ),
              child: pw.Column(
                children: [
                  _buildTotalRow('المجموع الفرعي', subtotal, currency),
                  _buildTotalRow('ضريبة القيمة المضافة (15%)', vat, currency,
                      isVat: true),
                  pw.Container(
                    margin: const pw.EdgeInsets.symmetric(vertical: 8),
                    height: 1,
                    decoration: pw.BoxDecoration(
                      gradient: pw.LinearGradient(
                        colors: [
                          primaryBlue.shade(0.2),
                          primaryPurple.shade(0.2)
                        ],
                      ),
                    ),
                  ),
                  _buildTotalRow(
                    'المجموع الكلي',
                    booking.totalPrice.amount,
                    currency,
                    isTotal: true,
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    // Payment Summary Card
    pw.Widget buildPaymentSummary() {
      final paid = details.totalPaid;
      final remaining = details.remainingAmount;
      final payments = details.payments;

      return pw.Container(
        decoration: pw.BoxDecoration(
          gradient: pw.LinearGradient(
            colors: [
              primaryBlue.shade(0.1),
              primaryPurple.shade(0.1),
            ],
            begin: pw.Alignment.topLeft,
            end: pw.Alignment.bottomRight,
          ),
          borderRadius: pw.BorderRadius.circular(12),
        ),
        padding: const pw.EdgeInsets.all(20),
        child: pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Row(
              mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
              children: [
                pw.Row(
                  children: [
                    pw.Container(
                      padding: const pw.EdgeInsets.symmetric(
                          horizontal: 8, vertical: 4),
                      decoration: pw.BoxDecoration(
                        color: success.shade(0.2),
                        borderRadius: pw.BorderRadius.circular(4),
                      ),
                      child: pw.Text(
                        '4',
                        style: pw.TextStyle(
                          fontSize: 10,
                          fontWeight: pw.FontWeight.bold,
                          color: success,
                        ),
                      ),
                    ),
                    pw.SizedBox(width: 8),
                    pw.Text(
                      'ملخص المدفوعات',
                      style: pw.TextStyle(
                        fontSize: 13,
                        fontWeight: pw.FontWeight.bold,
                        color: textWhite,
                      ),
                    ),
                  ],
                ),
                _buildPaymentStatus(remaining.amount),
              ],
            ),
            pw.SizedBox(height: 16),
            pw.Row(
              mainAxisAlignment: pw.MainAxisAlignment.spaceAround,
              children: [
                _buildPaymentCard(
                  'المبلغ الإجمالي',
                  _formatMoney(booking.totalPrice.amount, currency),
                  primaryBlue,
                ),
                _buildPaymentCard(
                  'المدفوع',
                  _formatMoney(paid.amount, currency),
                  success,
                ),
                _buildPaymentCard(
                  'المتبقي',
                  _formatMoney(remaining.amount, currency),
                  remaining.amount > 0 ? warning : success,
                ),
              ],
            ),
            if (payments.isNotEmpty) ...[
              pw.SizedBox(height: 16),
              pw.Text(
                'سجل المدفوعات:',
                style: pw.TextStyle(
                  fontSize: 11,
                  fontWeight: pw.FontWeight.bold,
                  color: textLight,
                ),
              ),
              pw.SizedBox(height: 8),
              ...payments.map((payment) => pw.Container(
                    margin: const pw.EdgeInsets.only(bottom: 4),
                    padding: const pw.EdgeInsets.symmetric(
                        horizontal: 12, vertical: 6),
                    decoration: pw.BoxDecoration(
                      color: darkCard.shade(0.5),
                      borderRadius: pw.BorderRadius.circular(6),
                    ),
                    child: pw.Row(
                      mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                      children: [
                        pw.Text(
                          DateFormat('yyyy/MM/dd HH:mm')
                              .format(payment.paymentDate),
                          style:
                              const pw.TextStyle(fontSize: 9, color: textMuted),
                        ),
                        pw.Text(
                          _formatMoney(
                              payment.amount.amount, payment.amount.currency),
                          style: pw.TextStyle(
                            fontSize: 9,
                            fontWeight: pw.FontWeight.bold,
                            color: success,
                          ),
                        ),
                      ],
                    ),
                  )),
            ],
          ],
        ),
      );
    }

    // Terms and QR Code Section
    pw.Widget buildTermsAndQR() {
      final qrData =
          'REF:$bookingReference|AMT:${booking.totalPrice.amount}|DATE:${DateFormat('yyyyMMdd').format(issueDate)}|TAX:300123456789012';

      return pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Expanded(
            flex: 3,
            child: pw.Container(
              padding: const pw.EdgeInsets.all(16),
              decoration: pw.BoxDecoration(
                color: darkCard,
                borderRadius: pw.BorderRadius.circular(12),
              ),
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  pw.Container(
                    padding: const pw.EdgeInsets.symmetric(
                        horizontal: 8, vertical: 4),
                    decoration: pw.BoxDecoration(
                      gradient: pw.LinearGradient(
                        colors: [
                          primaryViolet.shade(0.2),
                          primaryPurple.shade(0.2)
                        ],
                      ),
                      borderRadius: pw.BorderRadius.circular(4),
                    ),
                    child: pw.Text(
                      'الشروط والأحكام',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: textWhite,
                      ),
                    ),
                  ),
                  pw.SizedBox(height: 12),
                  _buildTermItem(
                      'يجب سداد المبلغ المتبقي عند الوصول إلى مكان الإقامة'),
                  _buildTermItem(
                      'يخضع الإلغاء للشروط والأحكام المتفق عليها مسبقاً'),
                  _buildTermItem(
                      'يجب إبراز وثيقة إثبات الهوية عند تسجيل الدخول'),
                  _buildTermItem(
                      'أوقات تسجيل الدخول: 14:00 - تسجيل الخروج: 12:00'),
                  _buildTermItem(
                      'يحق للمنشأة إلغاء الحجز في حالة عدم الوصول خلال 24 ساعة'),
                  _buildTermItem('الأسعار شاملة ضريبة القيمة المضافة 15%'),
                ],
              ),
            ),
          ),
          pw.SizedBox(width: 16),
          pw.Container(
            padding: const pw.EdgeInsets.all(16),
            decoration: pw.BoxDecoration(
              gradient: const pw.LinearGradient(
                colors: [darkCard, darkSurface],
                begin: pw.Alignment.topCenter,
                end: pw.Alignment.bottomCenter,
              ),
              borderRadius: pw.BorderRadius.circular(12),
            ),
            child: pw.Column(
              children: [
                pw.Text(
                  'رمز التحقق السريع',
                  style: pw.TextStyle(
                    fontSize: 11,
                    fontWeight: pw.FontWeight.bold,
                    color: textWhite,
                  ),
                ),
                pw.SizedBox(height: 12),
                pw.Container(
                  padding: const pw.EdgeInsets.all(8),
                  decoration: pw.BoxDecoration(
                    color: PdfColors.white,
                    borderRadius: pw.BorderRadius.circular(8),
                  ),
                  child: pw.BarcodeWidget(
                    barcode: pw.Barcode.qrCode(),
                    data: qrData,
                    width: 100,
                    height: 100,
                  ),
                ),
                pw.SizedBox(height: 8),
                pw.Text(
                  'Scan for Verification',
                  style: const pw.TextStyle(
                    fontSize: 8,
                    color: textMuted,
                  ),
                ),
              ],
            ),
          ),
        ],
      );
    }

    // Professional Footer
    pw.Widget buildProfessionalFooter() {
      return pw.Container(
        margin: const pw.EdgeInsets.only(top: 20),
        child: pw.Column(
          children: [
            // Gradient separator
            pw.Container(
              height: 2,
              decoration: const pw.BoxDecoration(
                gradient: pw.LinearGradient(
                  colors: [
                    primaryCyan,
                    primaryBlue,
                    primaryPurple,
                    primaryViolet
                  ],
                  begin: pw.Alignment.centerLeft,
                  end: pw.Alignment.centerRight,
                ),
              ),
            ),
            pw.Container(
              padding: const pw.EdgeInsets.all(20),
              decoration: const pw.BoxDecoration(
                gradient: pw.LinearGradient(
                  colors: [darkBackground2, darkBackground],
                  begin: pw.Alignment.topCenter,
                  end: pw.Alignment.bottomCenter,
                ),
              ),
              child: pw.Column(
                children: [
                  pw.Row(
                    mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                    crossAxisAlignment: pw.CrossAxisAlignment.start,
                    children: [
                      // Contact Info
                      pw.Column(
                        crossAxisAlignment: pw.CrossAxisAlignment.start,
                        children: [
                          pw.Text(
                            'معلومات التواصل',
                            style: pw.TextStyle(
                              fontSize: 11,
                              fontWeight: pw.FontWeight.bold,
                              color: neonBlue,
                            ),
                          ),
                          pw.SizedBox(height: 8),
                          _buildContactInfo('الموقع:', 'www.yemenbooking.com'),
                          _buildContactInfo(
                              'البريد:', 'support@yemenbooking.com'),
                          _buildContactInfo('الجوال:', '+967 777 123 456'),
                          _buildContactInfo('الهاتف:', '+967 1 234 567'),
                        ],
                      ),
                      // Social Media
                      pw.Column(
                        crossAxisAlignment: pw.CrossAxisAlignment.center,
                        children: [
                          pw.Text(
                            'تابعنا على',
                            style: pw.TextStyle(
                              fontSize: 11,
                              fontWeight: pw.FontWeight.bold,
                              color: neonPurple,
                            ),
                          ),
                          pw.SizedBox(height: 8),
                          pw.Text(
                            '@YemenBooking',
                            style: const pw.TextStyle(
                              fontSize: 9,
                              color: textLight,
                            ),
                          ),
                        ],
                      ),
                      // Company Stamp Area
                      pw.Container(
                        width: 120,
                        height: 60,
                        decoration: pw.BoxDecoration(
                          borderRadius: pw.BorderRadius.circular(8),
                          border: const pw.Border(
                            top: pw.BorderSide(color: darkBorder, width: 1),
                            bottom: pw.BorderSide(color: darkBorder, width: 1),
                            left: pw.BorderSide(color: darkBorder, width: 1),
                            right: pw.BorderSide(color: darkBorder, width: 1),
                          ),
                        ),
                        child: pw.Center(
                          child: pw.Text(
                            'ختم المنشأة',
                            style: const pw.TextStyle(
                              fontSize: 10,
                              color: textMuted,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                  pw.SizedBox(height: 16),
                  // Thank you message
                  pw.Container(
                    padding: const pw.EdgeInsets.all(12),
                    decoration: pw.BoxDecoration(
                      gradient: pw.LinearGradient(
                        colors: [
                          primaryBlue.shade(0.1),
                          primaryPurple.shade(0.1),
                        ],
                        begin: pw.Alignment.centerLeft,
                        end: pw.Alignment.centerRight,
                      ),
                      borderRadius: pw.BorderRadius.circular(8),
                    ),
                    child: pw.Center(
                      child: pw.Text(
                        'نشكركم على ثقتكم بنا ونتطلع لخدمتكم بأفضل معايير الضيافة',
                        style: pw.TextStyle(
                          fontSize: 12,
                          fontWeight: pw.FontWeight.bold,
                          color: textWhite,
                        ),
                      ),
                    ),
                  ),
                  pw.SizedBox(height: 12),
                  // Copyright
                  pw.Row(
                    mainAxisAlignment: pw.MainAxisAlignment.center,
                    children: [
                      pw.Text(
                        '© ${DateTime.now().year} YemenBooking Platform - All Rights Reserved',
                        style: const pw.TextStyle(
                          fontSize: 8,
                          color: textMuted,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    // Build PDF Document
    doc.addPage(
      pw.MultiPage(
        pageTheme: pw.PageTheme(
          margin: const pw.EdgeInsets.all(24),
          theme: pw.ThemeData.withFont(
            base: arabicFont,
            bold: arabicBold,
          ),
          textDirection: pw.TextDirection.rtl,
          pageFormat: PdfPageFormat.a4,
        ),
        footer: (context) => pw.Container(
          padding: const pw.EdgeInsets.only(top: 10),
          decoration: const pw.BoxDecoration(
            border: pw.Border(
              top: pw.BorderSide(color: darkBorder, width: 0.5),
            ),
          ),
          child: pw.Row(
            mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
            children: [
              pw.Text(
                'صفحة ${context.pageNumber} من ${context.pagesCount}',
                style: const pw.TextStyle(fontSize: 9, color: textMuted),
              ),
              pw.Text(
                'الفاتورة الإلكترونية معتمدة من هيئة الزكاة والضريبة',
                style: const pw.TextStyle(fontSize: 9, color: textMuted),
              ),
              pw.Text(
                DateFormat('yyyy/MM/dd HH:mm').format(DateTime.now()),
                style: const pw.TextStyle(fontSize: 9, color: textMuted),
              ),
            ],
          ),
        ),
        build: (context) => [
          buildProfessionalHeader(),
          pw.SizedBox(height: 20),
          buildModernInfoCards(),
          pw.SizedBox(height: 20),
          buildProfessionalPricingTable(),
          pw.SizedBox(height: 20),
          buildPaymentSummary(),
          pw.SizedBox(height: 20),
          buildTermsAndQR(),
          buildProfessionalFooter(),
        ],
      ),
    );

    return doc.save();
  }

  // Helper Methods
  static pw.Widget _buildCompanyInfoRow(String label, String text) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(bottom: 4),
      child: pw.Row(
        children: [
          pw.Text(
            label,
            style: pw.TextStyle(
              fontSize: 9,
              fontWeight: pw.FontWeight.bold,
              color: textLight,
            ),
          ),
          pw.SizedBox(width: 6),
          pw.Text(
            text,
            style: const pw.TextStyle(fontSize: 10, color: textLight),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildInvoiceDetailRow(String label, String value,
      {bool highlight = false}) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(bottom: 4),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            label,
            style: const pw.TextStyle(
              fontSize: 10,
              color: textMuted,
            ),
          ),
          pw.SizedBox(width: 12),
          pw.Container(
            padding: highlight
                ? const pw.EdgeInsets.symmetric(horizontal: 8, vertical: 2)
                : pw.EdgeInsets.zero,
            decoration: highlight
                ? pw.BoxDecoration(
                    gradient: pw.LinearGradient(
                      colors: [
                        primaryBlue.shade(0.2),
                        primaryPurple.shade(0.2)
                      ],
                    ),
                    borderRadius: pw.BorderRadius.circular(4),
                  )
                : null,
            child: pw.Text(
              value,
              style: pw.TextStyle(
                fontSize: 10,
                fontWeight: pw.FontWeight.bold,
                color: highlight ? neonBlue : textWhite,
              ),
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildInfoField(String label, String value) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(bottom: 8),
      child: pw.Column(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Text(
            label,
            style: const pw.TextStyle(
              fontSize: 9,
              color: textMuted,
            ),
          ),
          pw.SizedBox(height: 2),
          pw.Text(
            value,
            style: pw.TextStyle(
              fontSize: 11,
              fontWeight: pw.FontWeight.bold,
              color: textWhite,
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildStatusField(String status) {
    final color = _getStatusColor(status);
    final text = _getStatusText(status);

    return pw.Container(
      margin: const pw.EdgeInsets.only(top: 4),
      child: pw.Column(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Text(
            'حالة الحجز',
            style: const pw.TextStyle(
              fontSize: 9,
              color: textMuted,
            ),
          ),
          pw.SizedBox(height: 2),
          pw.Container(
            padding: const pw.EdgeInsets.symmetric(horizontal: 8, vertical: 4),
            decoration: pw.BoxDecoration(
              color: color.shade(0.2),
              borderRadius: pw.BorderRadius.circular(6),
            ),
            child: pw.Text(
              text,
              style: pw.TextStyle(
                fontSize: 10,
                fontWeight: pw.FontWeight.bold,
                color: color,
              ),
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildTableHeader(String text,
      {pw.TextAlign align = pw.TextAlign.right}) {
    return pw.Container(
      padding: const pw.EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      child: pw.Text(
        text,
        style: pw.TextStyle(
          fontSize: 11,
          fontWeight: pw.FontWeight.bold,
          color: textLight,
        ),
        textAlign: align,
      ),
    );
  }

  static pw.Widget _buildTableCell(
    String text, {
    pw.TextAlign align = pw.TextAlign.right,
    bool bold = false,
  }) {
    return pw.Container(
      padding: const pw.EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      child: pw.Text(
        text,
        style: pw.TextStyle(
          fontSize: 10,
          fontWeight: bold ? pw.FontWeight.bold : pw.FontWeight.normal,
          color: bold ? textWhite : textLight,
        ),
        textAlign: align,
      ),
    );
  }

  static pw.Widget _buildTotalRow(
    String label,
    double amount,
    String currency, {
    bool isVat = false,
    bool isTotal = false,
  }) {
    return pw.Container(
      margin: const pw.EdgeInsets.symmetric(vertical: 3),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            label,
            style: pw.TextStyle(
              fontSize: isTotal ? 13 : 11,
              fontWeight: isTotal ? pw.FontWeight.bold : pw.FontWeight.normal,
              color: isTotal ? neonBlue : (isVat ? warning : textLight),
            ),
          ),
          pw.Text(
            _formatMoney(amount, currency),
            style: pw.TextStyle(
              fontSize: isTotal ? 13 : 11,
              fontWeight: isTotal ? pw.FontWeight.bold : pw.FontWeight.normal,
              color: isTotal ? neonBlue : (isVat ? warning : textWhite),
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildPaymentCard(
    String label,
    String amount,
    PdfColor color,
  ) {
    return pw.Container(
      padding: const pw.EdgeInsets.all(12),
      decoration: pw.BoxDecoration(
        color: color.shade(0.1),
        borderRadius: pw.BorderRadius.circular(8),
      ),
      child: pw.Column(
        children: [
          pw.Container(
            width: 24,
            height: 24,
            decoration: pw.BoxDecoration(
              color: color.shade(0.2),
              shape: pw.BoxShape.circle,
            ),
            child: pw.Center(
              child: pw.Text(
                label.substring(0, 1),
                style: pw.TextStyle(
                  fontSize: 12,
                  fontWeight: pw.FontWeight.bold,
                  color: color,
                ),
              ),
            ),
          ),
          pw.SizedBox(height: 4),
          pw.Text(
            label,
            style: const pw.TextStyle(
              fontSize: 9,
              color: textMuted,
            ),
          ),
          pw.SizedBox(height: 2),
          pw.Text(
            amount,
            style: pw.TextStyle(
              fontSize: 12,
              fontWeight: pw.FontWeight.bold,
              color: color,
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildPaymentStatus(double remaining) {
    final isPaid = remaining <= 0;
    return pw.Container(
      padding: const pw.EdgeInsets.symmetric(horizontal: 12, vertical: 4),
      decoration: pw.BoxDecoration(
        gradient: pw.LinearGradient(
          colors: isPaid
              ? [success.shade(0.2), success.shade(0.3)]
              : [warning.shade(0.2), warning.shade(0.3)],
        ),
        borderRadius: pw.BorderRadius.circular(12),
      ),
      child: pw.Text(
        isPaid ? 'مدفوع بالكامل' : 'دفعة جزئية',
        style: pw.TextStyle(
          fontSize: 10,
          fontWeight: pw.FontWeight.bold,
          color: isPaid ? success : warning,
        ),
      ),
    );
  }

  static pw.Widget _buildTermItem(String text) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(bottom: 6),
      child: pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Container(
            width: 4,
            height: 4,
            margin: const pw.EdgeInsets.only(top: 4),
            decoration: const pw.BoxDecoration(
              color: primaryBlue,
              shape: pw.BoxShape.circle,
            ),
          ),
          pw.SizedBox(width: 8),
          pw.Expanded(
            child: pw.Text(
              text,
              style: const pw.TextStyle(
                fontSize: 9,
                color: textLight,
                height: 1.3,
              ),
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildContactInfo(String label, String text) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(bottom: 4),
      child: pw.Row(
        children: [
          pw.Text(
            label,
            style: pw.TextStyle(
              fontSize: 9,
              fontWeight: pw.FontWeight.bold,
              color: textLight,
            ),
          ),
          pw.SizedBox(width: 6),
          pw.Text(
            text,
            style: const pw.TextStyle(fontSize: 9, color: textLight),
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
        return 'مؤكد';
      case 'pending':
        return 'قيد الانتظار';
      case 'cancelled':
        return 'ملغي';
      case 'completed':
        return 'مكتمل';
      default:
        return status;
    }
  }

  static PdfColor _getStatusColor(String status) {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return success;
      case 'pending':
        return warning;
      case 'cancelled':
        return error;
      case 'completed':
        return info;
      default:
        return textMuted;
    }
  }

  static String _generateInvoiceNumber(String bookingId) {
    final year = DateTime.now().year;
    final month = DateTime.now().month.toString().padLeft(2, '0');
    final day = DateTime.now().day.toString().padLeft(2, '0');
    final hash = bookingId.hashCode.abs() % 100000;
    return '$year$month$day${hash.toString().padLeft(5, '0')}';
  }

  static String _formatBookingReference(String bookingId) {
    if (bookingId.isEmpty) return 'N/A';
    const maxLength = 10;
    if (bookingId.length <= maxLength) {
      return bookingId.toUpperCase();
    }
    return bookingId.substring(0, maxLength).toUpperCase();
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
