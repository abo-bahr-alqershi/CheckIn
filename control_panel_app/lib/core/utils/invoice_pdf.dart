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

  // تعريف الألوان الاحترافية
  static const PdfColor primaryColor = PdfColor.fromInt(0xFF1E40AF);
  static const PdfColor secondaryColor = PdfColor.fromInt(0xFF3B82F6);
  static const PdfColor accentColor = PdfColor.fromInt(0xFFF59E0B);
  static const PdfColor lightGray = PdfColor.fromInt(0xFFF3F4F6);
  static const PdfColor mediumGray = PdfColor.fromInt(0xFF9CA3AF);
  static const PdfColor darkGray = PdfColor.fromInt(0xFF374151);
  static const PdfColor successColor = PdfColor.fromInt(0xFF10B981);
  static const PdfColor warningColor = PdfColor.fromInt(0xFFF59E0B);
  static const PdfColor dangerColor = PdfColor.fromInt(0xFFEF4444);

  // ألوان مخصصة للنصوص الفاتحة
  static final PdfColor whiteLight =
      const PdfColor.fromInt(0xFFFFFFFF).shade(0.7);
  static final PdfColor whiteMedium =
      const PdfColor.fromInt(0xFFFFFFFF).shade(0.5);
  static final PdfColor whiteDark =
      const PdfColor.fromInt(0xFFFFFFFF).shade(0.3);

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

    // تحميل الخطوط
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

    // محاولة تحميل الشعار
    try {
      logoData = (await rootBundle.load('assets/images/logo.png'))
          .buffer
          .asUint8List();
    } catch (_) {
      logoData = null;
    }

    // بناء الهيدر الاحترافي
    pw.Widget buildProfessionalHeader() {
      return pw.Container(
        decoration: pw.BoxDecoration(
          gradient: const pw.LinearGradient(
            colors: [primaryColor, secondaryColor],
            begin: pw.Alignment.topLeft,
            end: pw.Alignment.bottomRight,
          ),
          borderRadius: pw.BorderRadius.circular(8),
        ),
        padding: const pw.EdgeInsets.all(20),
        child: pw.Row(
          mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Expanded(
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  if (logoData != null)
                    pw.Image(
                      pw.MemoryImage(logoData),
                      height: 50,
                      width: 150,
                      fit: pw.BoxFit.contain,
                    )
                  else
                    pw.Text(
                      property?.name ?? 'YemenBooking',
                      style: pw.TextStyle(
                        fontSize: 24,
                        fontWeight: pw.FontWeight.bold,
                        color: PdfColors.white,
                      ),
                    ),
                  pw.SizedBox(height: 8),
                  if (property?.address != null)
                    _buildInfoRow(
                      const pw.IconData(0xe0c8),
                      property.address, // تم إزالة ! لأن التحقق تم بالفعل
                      color: PdfColors.white,
                    ),
                  if (property?.phone != null)
                    _buildInfoRow(
                      const pw.IconData(0xe0cd),
                      property.phone, // تم إزالة ! لأن التحقق تم بالفعل
                      color: PdfColors.white,
                    ),
                  if (property?.email != null)
                    _buildInfoRow(
                      const pw.IconData(0xe0be),
                      property.email, // تم إزالة ! لأن التحقق تم بالفعل
                      color: PdfColors.white,
                    ),
                ],
              ),
            ),
            pw.Container(
              padding: const pw.EdgeInsets.all(12),
              decoration: pw.BoxDecoration(
                color: PdfColors.white.shade(0.95),
                borderRadius: pw.BorderRadius.circular(6),
              ),
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.end,
                children: [
                  pw.Text(
                    'فاتورة ضريبية',
                    style: pw.TextStyle(
                      fontSize: 16,
                      fontWeight: pw.FontWeight.bold,
                      color: primaryColor,
                    ),
                  ),
                  pw.Text(
                    'TAX INVOICE',
                    style: const pw.TextStyle(
                      fontSize: 12,
                      color: mediumGray,
                    ),
                  ),
                  pw.SizedBox(height: 8),
                  _buildLabelValue('رقم الفاتورة:', invoiceNumber),
                  _buildLabelValue('رقم الحجز:', bookingReference),
                  _buildLabelValue(
                    'التاريخ:',
                    DateFormat('dd/MM/yyyy').format(issueDate),
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    // بناء معلومات الضيف والحجز بتصميم محسّن
    pw.Widget buildEnhancedGuestPropertyInfo() {
      // حساب عدد الضيوف - استخدام قيمة افتراضية إذا لم تكن موجودة
      final guestCount = booking.adults + booking.children;

      return pw.Row(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Expanded(
            child: pw.Container(
              decoration: pw.BoxDecoration(
                border: pw.Border.all(color: lightGray, width: 1),
                borderRadius: pw.BorderRadius.circular(6),
              ),
              padding: const pw.EdgeInsets.all(12),
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  pw.Container(
                    padding: const pw.EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 4,
                    ),
                    decoration: pw.BoxDecoration(
                      color: primaryColor,
                      borderRadius: pw.BorderRadius.circular(4),
                    ),
                    child: pw.Text(
                      'معلومات الضيف',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: PdfColors.white,
                      ),
                    ),
                  ),
                  pw.SizedBox(height: 10),
                  _buildDetailRow('الاسم', guestContact.name),
                  if (guestContact.phone != null)
                    _buildDetailRow('الهاتف', guestContact.phone!),
                  if (guestContact.email != null)
                    _buildDetailRow('البريد الإلكتروني', guestContact.email!),
                  _buildDetailRow('الجنسية', guest?.nationality ?? 'غير محدد'),
                ],
              ),
            ),
          ),
          pw.SizedBox(width: 12),
          pw.Expanded(
            child: pw.Container(
              decoration: pw.BoxDecoration(
                border: pw.Border.all(color: lightGray, width: 1),
                borderRadius: pw.BorderRadius.circular(6),
              ),
              padding: const pw.EdgeInsets.all(12),
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  pw.Container(
                    padding: const pw.EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 4,
                    ),
                    decoration: pw.BoxDecoration(
                      color: secondaryColor,
                      borderRadius: pw.BorderRadius.circular(4),
                    ),
                    child: pw.Text(
                      'تفاصيل الحجز',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: PdfColors.white,
                      ),
                    ),
                  ),
                  pw.SizedBox(height: 10),
                  _buildDetailRow('الوحدة', booking.unitName),
                  _buildDetailRow(
                      'تاريخ الوصول', Formatters.formatDate(booking.checkIn)),
                  _buildDetailRow('تاريخ المغادرة',
                      Formatters.formatDate(booking.checkOut)),
                  _buildDetailRow('عدد الليالي', '${booking.nights} ليلة'),
                  _buildDetailRow('عدد الضيوف', '$guestCount ضيف'),
                  _buildDetailRow(
                      'حالة الحجز', _getStatusText(booking.status.name),
                      statusColor: _getStatusColor(booking.status.name)),
                ],
              ),
            ),
          ),
        ],
      );
    }

    // بناء جدول الخدمات المحسّن
    pw.Widget buildEnhancedServicesTable() {
      final services = details.services;

      // حساب سعر الوحدة الأساسي
      final basePrice = booking.totalPrice.amount -
          services.fold(0.0, (sum, s) => sum + s.totalPrice.amount);

      return pw.Container(
        decoration: pw.BoxDecoration(
          border: pw.Border.all(color: lightGray, width: 1),
          borderRadius: pw.BorderRadius.circular(6),
        ),
        child: pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Container(
              padding: const pw.EdgeInsets.all(10),
              decoration: const pw.BoxDecoration(
                color: lightGray,
                borderRadius: pw.BorderRadius.only(
                  topLeft: pw.Radius.circular(6),
                  topRight: pw.Radius.circular(6),
                ),
              ),
              child: pw.Text(
                'تفاصيل الأسعار',
                style: pw.TextStyle(
                  fontSize: 14,
                  fontWeight: pw.FontWeight.bold,
                  color: darkGray,
                ),
              ),
            ),
            pw.Table(
              border: const pw.TableBorder(
                horizontalInside: pw.BorderSide(
                  color: lightGray,
                  width: 0.5,
                ),
              ),
              columnWidths: const {
                0: pw.FlexColumnWidth(4),
                1: pw.FlexColumnWidth(1.5),
                2: pw.FlexColumnWidth(2),
                3: pw.FlexColumnWidth(2),
              },
              children: [
                // Header
                pw.TableRow(
                  decoration: const pw.BoxDecoration(color: lightGray),
                  children: [
                    _enhancedCell('البند',
                        bold: true, align: pw.TextAlign.right),
                    _enhancedCell('الكمية',
                        bold: true, align: pw.TextAlign.center),
                    _enhancedCell('السعر',
                        bold: true, align: pw.TextAlign.center),
                    _enhancedCell('المجموع',
                        bold: true, align: pw.TextAlign.left),
                  ],
                ),
                // سعر الوحدة الأساسي
                pw.TableRow(
                  children: [
                    _enhancedCell('إيجار الوحدة (${booking.nights} ليلة)'),
                    _enhancedCell('1', align: pw.TextAlign.center),
                    _enhancedCell(
                      _formatMoney(basePrice, currency),
                      align: pw.TextAlign.center,
                    ),
                    _enhancedCell(
                      _formatMoney(basePrice, currency),
                      align: pw.TextAlign.left,
                    ),
                  ],
                ),
                // الخدمات الإضافية
                ...services.map((s) => pw.TableRow(
                      children: [
                        _enhancedCell(s.name),
                        _enhancedCell('${s.quantity}',
                            align: pw.TextAlign.center),
                        _enhancedCell(
                          _formatMoney(s.price.amount, s.price.currency),
                          align: pw.TextAlign.center,
                        ),
                        _enhancedCell(
                          _formatMoney(
                              s.totalPrice.amount, s.totalPrice.currency),
                          align: pw.TextAlign.left,
                        ),
                      ],
                    )),
              ],
            ),
          ],
        ),
      );
    }

    // بناء ملخص المبالغ المحسّن
    pw.Widget buildEnhancedTotals() {
      final total = booking.totalPrice;
      final paid = details.totalPaid;
      final remaining = details.remainingAmount;

      // حساب الضريبة (مثال: 15%)
      const taxRate = 0.15;
      final subtotal = total.amount / (1 + taxRate);
      final tax = total.amount - subtotal;

      return pw.Container(
        decoration: pw.BoxDecoration(
          gradient: const pw.LinearGradient(
            colors: [lightGray, PdfColors.white],
            begin: pw.Alignment.topCenter,
            end: pw.Alignment.bottomCenter,
          ),
          border: pw.Border.all(color: primaryColor, width: 1),
          borderRadius: pw.BorderRadius.circular(6),
        ),
        padding: const pw.EdgeInsets.all(12),
        child: pw.Column(
          children: [
            _buildTotalRow('المجموع الفرعي', subtotal, currency),
            _buildTotalRow('ضريبة القيمة المضافة (15%)', tax, currency),
            pw.Divider(color: mediumGray, thickness: 1),
            _buildTotalRow(
              'المجموع الكلي',
              total.amount,
              currency,
              bold: true,
              color: primaryColor,
            ),
            pw.SizedBox(height: 8),
            _buildTotalRow(
              'المبلغ المدفوع',
              paid.amount,
              currency,
              color: successColor,
            ),
            _buildTotalRow(
              'المبلغ المتبقي',
              remaining.amount,
              currency,
              color: remaining.amount > 0 ? dangerColor : successColor,
              bold: remaining.amount > 0,
            ),
          ],
        ),
      );
    }

    // بناء معلومات الدفع
    pw.Widget buildPaymentInfo() {
      final payments = details.payments;
      if (payments.isEmpty) return pw.Container();

      return pw.Container(
        decoration: pw.BoxDecoration(
          border: pw.Border.all(color: lightGray, width: 1),
          borderRadius: pw.BorderRadius.circular(6),
        ),
        padding: const pw.EdgeInsets.all(12),
        child: pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Row(
              children: [
                pw.Container(
                  padding: const pw.EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: 4,
                  ),
                  decoration: pw.BoxDecoration(
                    color: successColor,
                    borderRadius: pw.BorderRadius.circular(4),
                  ),
                  child: pw.Text(
                    'سجل المدفوعات',
                    style: pw.TextStyle(
                      fontSize: 12,
                      fontWeight: pw.FontWeight.bold,
                      color: PdfColors.white,
                    ),
                  ),
                ),
              ],
            ),
            pw.SizedBox(height: 10),
            ...payments.map((payment) => pw.Container(
                  margin: const pw.EdgeInsets.only(bottom: 6),
                  padding: const pw.EdgeInsets.all(8),
                  decoration: pw.BoxDecoration(
                    color: lightGray,
                    borderRadius: pw.BorderRadius.circular(4),
                  ),
                  child: pw.Row(
                    mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                    children: [
                      pw.Text(
                        DateFormat('dd/MM/yyyy').format(payment.paymentDate),
                        style: const pw.TextStyle(fontSize: 10),
                      ),
                      pw.Text(
                        payment.status
                            .name, // استخدام status بدلاً من paymentMethod
                        style: const pw.TextStyle(
                          fontSize: 10,
                          color: darkGray,
                        ),
                      ),
                      pw.Text(
                        _formatMoney(
                            payment.amount.amount, payment.amount.currency),
                        style: pw.TextStyle(
                          fontSize: 10,
                          fontWeight: pw.FontWeight.bold,
                          color: successColor,
                        ),
                      ),
                    ],
                  ),
                )),
          ],
        ),
      );
    }

    // بناء الفوتر
    pw.Widget buildFooter() {
      return pw.Container(
        margin: const pw.EdgeInsets.only(top: 20),
        padding: const pw.EdgeInsets.all(16),
        decoration: pw.BoxDecoration(
          color: darkGray,
          borderRadius: pw.BorderRadius.circular(6),
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
                      'الشروط والأحكام',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: PdfColors.white,
                      ),
                    ),
                    pw.SizedBox(height: 4),
                    pw.Text(
                      '• يجب دفع المبلغ المتبقي عند الوصول',
                      style: pw.TextStyle(fontSize: 9, color: whiteLight),
                    ),
                    pw.Text(
                      '• تطبق سياسة الإلغاء حسب الشروط المتفق عليها',
                      style: pw.TextStyle(fontSize: 9, color: whiteLight),
                    ),
                    pw.Text(
                      '• يجب إبراز وثيقة الهوية عند تسجيل الدخول',
                      style: pw.TextStyle(fontSize: 9, color: whiteLight),
                    ),
                  ],
                ),
                pw.Column(
                  crossAxisAlignment: pw.CrossAxisAlignment.end,
                  children: [
                    pw.Text(
                      'تواصل معنا',
                      style: pw.TextStyle(
                        fontSize: 12,
                        fontWeight: pw.FontWeight.bold,
                        color: PdfColors.white,
                      ),
                    ),
                    pw.SizedBox(height: 4),
                    // إزالة website لأنه غير موجود في PropertyDetails
                    pw.Text(
                      'www.yemenbooking.com',
                      style: pw.TextStyle(fontSize: 9, color: whiteLight),
                    ),
                    pw.Text(
                      'support@yemenbooking.com',
                      style: pw.TextStyle(fontSize: 9, color: whiteLight),
                    ),
                    pw.Text(
                      '+967 777 123 456',
                      style: pw.TextStyle(fontSize: 9, color: whiteLight),
                    ),
                  ],
                ),
              ],
            ),
            pw.SizedBox(height: 12),
            pw.Divider(color: whiteDark),
            pw.SizedBox(height: 8),
            pw.Text(
              'شكراً لاختياركم خدماتنا - نتطلع لاستضافتكم',
              style: pw.TextStyle(
                fontSize: 11,
                fontWeight: pw.FontWeight.bold,
                color: accentColor,
              ),
            ),
            pw.SizedBox(height: 4),
            pw.Text(
              '© ${DateTime.now().year} YemenBooking. جميع الحقوق محفوظة',
              style: pw.TextStyle(fontSize: 8, color: whiteMedium),
            ),
          ],
        ),
      );
    }

    // إضافة QR Code (اختياري)
    pw.Widget buildQRCode() {
      final qrData =
          'BOOKING:$bookingReference|AMOUNT:${booking.totalPrice.amount}|DATE:${DateFormat('yyyy-MM-dd').format(issueDate)}';

      return pw.Container(
        padding: const pw.EdgeInsets.all(8),
        decoration: pw.BoxDecoration(
          border: pw.Border.all(color: lightGray),
          borderRadius: pw.BorderRadius.circular(4),
        ),
        child: pw.Column(
          children: [
            pw.BarcodeWidget(
              barcode: pw.Barcode.qrCode(),
              data: qrData,
              width: 80,
              height: 80,
            ),
            pw.SizedBox(height: 4),
            pw.Text(
              'امسح للتحقق',
              style: const pw.TextStyle(fontSize: 8),
            ),
          ],
        ),
      );
    }

    // بناء الصفحة
    doc.addPage(
      pw.MultiPage(
        pageTheme: pw.PageTheme(
          margin: const pw.EdgeInsets.all(20),
          theme: pw.ThemeData.withFont(
            base: arabicFont,
            bold: arabicBold,
          ),
          textDirection: pw.TextDirection.rtl,
        ),
        header: (context) => buildProfessionalHeader(),
        footer: (context) => pw.Container(
          alignment: pw.Alignment.center,
          child: pw.Text(
            'صفحة ${context.pageNumber} من ${context.pagesCount}',
            style: const pw.TextStyle(fontSize: 10, color: mediumGray),
          ),
        ),
        build: (context) => [
          pw.SizedBox(height: 16),
          buildEnhancedGuestPropertyInfo(),
          pw.SizedBox(height: 16),
          pw.Row(
            crossAxisAlignment: pw.CrossAxisAlignment.start,
            children: [
              pw.Expanded(flex: 3, child: buildEnhancedServicesTable()),
              pw.SizedBox(width: 12),
              pw.Expanded(
                flex: 2,
                child: pw.Column(
                  children: [
                    buildEnhancedTotals(),
                    pw.SizedBox(height: 12),
                    buildQRCode(),
                  ],
                ),
              ),
            ],
          ),
          pw.SizedBox(height: 16),
          buildPaymentInfo(),
          pw.SizedBox(height: 16),
          buildFooter(),
        ],
      ),
    );

    return doc.save();
  }

  // Helper Methods
  static pw.Widget _buildInfoRow(pw.IconData icon, String text,
      {PdfColor color = darkGray}) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(top: 4),
      child: pw.Row(
        children: [
          pw.Icon(icon, size: 12, color: color),
          pw.SizedBox(width: 6),
          pw.Text(
            text,
            style: pw.TextStyle(fontSize: 10, color: color),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildLabelValue(String label, String value) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(top: 2),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            label,
            style: const pw.TextStyle(fontSize: 10, color: mediumGray),
          ),
          pw.SizedBox(width: 8),
          pw.Text(
            value,
            style: pw.TextStyle(
              fontSize: 10,
              fontWeight: pw.FontWeight.bold,
              color: darkGray,
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _buildDetailRow(String label, String value,
      {PdfColor? statusColor}) {
    return pw.Container(
      margin: const pw.EdgeInsets.only(bottom: 6),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            label,
            style: const pw.TextStyle(
              fontSize: 10,
              color: mediumGray,
            ),
          ),
          pw.Container(
            padding: statusColor != null
                ? const pw.EdgeInsets.symmetric(horizontal: 6, vertical: 2)
                : pw.EdgeInsets.zero,
            decoration: statusColor != null
                ? pw.BoxDecoration(
                    color: statusColor.shade(0.1),
                    borderRadius: pw.BorderRadius.circular(3),
                  )
                : null,
            child: pw.Text(
              value,
              style: pw.TextStyle(
                fontSize: 10,
                fontWeight: pw.FontWeight.bold,
                color: statusColor ?? darkGray,
              ),
            ),
          ),
        ],
      ),
    );
  }

  static pw.Widget _enhancedCell(
    String text, {
    bool bold = false,
    pw.TextAlign align = pw.TextAlign.right,
  }) {
    return pw.Container(
      padding: const pw.EdgeInsets.all(8),
      child: pw.Text(
        text,
        style: pw.TextStyle(
          fontSize: 10,
          fontWeight: bold ? pw.FontWeight.bold : pw.FontWeight.normal,
          color: bold ? darkGray : mediumGray,
        ),
        textAlign: align,
      ),
    );
  }

  static pw.Widget _buildTotalRow(
    String label,
    double amount,
    String currency, {
    bool bold = false,
    PdfColor? color,
  }) {
    return pw.Container(
      margin: const pw.EdgeInsets.symmetric(vertical: 2),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(
            label,
            style: pw.TextStyle(
              fontSize: bold ? 12 : 11,
              fontWeight: bold ? pw.FontWeight.bold : pw.FontWeight.normal,
              color: color ?? darkGray,
            ),
          ),
          pw.Text(
            _formatMoney(amount, currency),
            style: pw.TextStyle(
              fontSize: bold ? 12 : 11,
              fontWeight: bold ? pw.FontWeight.bold : pw.FontWeight.normal,
              color: color ?? darkGray,
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
        return successColor;
      case 'pending':
        return warningColor;
      case 'cancelled':
        return dangerColor;
      case 'completed':
        return primaryColor;
      default:
        return mediumGray;
    }
  }

  static String _generateInvoiceNumber(String bookingId) {
    final year = DateTime.now().year;
    final month = DateTime.now().month.toString().padLeft(2, '0');
    final hash = bookingId.hashCode.abs() % 10000;
    return 'INV-$year$month-${hash.toString().padLeft(4, '0')}';
  }

  static String _formatBookingReference(String bookingId) {
    if (bookingId.isEmpty) {
      return 'غير متوفر';
    }
    const maxLength = 8;
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
