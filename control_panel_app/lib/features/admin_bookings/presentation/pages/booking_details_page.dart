// lib/features/admin_bookings/presentation/pages/booking_details_page.dart

import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_staggered_animations/flutter_staggered_animations.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/utils/formatters.dart';
import '../bloc/booking_details/booking_details_bloc.dart';
import '../bloc/booking_details/booking_details_event.dart';
import '../bloc/booking_details/booking_details_state.dart';
import '../widgets/booking_status_badge.dart';
import '../widgets/booking_payment_summary.dart';
import '../widgets/booking_services_widget.dart';
import '../widgets/booking_actions_dialog.dart';
import '../widgets/check_in_out_dialog.dart';

class BookingDetailsPage extends StatefulWidget {
  final String bookingId;

  const BookingDetailsPage({
    super.key,
    required this.bookingId,
  });

  @override
  State<BookingDetailsPage> createState() => _BookingDetailsPageState();
}

class _BookingDetailsPageState extends State<BookingDetailsPage>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late ScrollController _scrollController;
  double _scrollOffset = 0;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 600),
    );
    _scrollController = ScrollController()
      ..addListener(() {
        setState(() {
          _scrollOffset = _scrollController.offset;
        });
      });

    _loadBookingDetails();
    _animationController.forward();
  }

  void _loadBookingDetails() {
    context.read<BookingDetailsBloc>().add(
          LoadBookingDetailsEvent(bookingId: widget.bookingId),
        );
  }

  @override
  void dispose() {
    _animationController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.darkBackground,
      body: BlocBuilder<BookingDetailsBloc, BookingDetailsState>(
        builder: (context, state) {
          if (state is BookingDetailsLoading) {
            return const LoadingWidget(
              type: LoadingType.futuristic,
              message: 'جاري تحميل تفاصيل الحجز...',
            );
          }

          if (state is BookingDetailsError) {
            return CustomErrorWidget(
              message: state.message,
              onRetry: _loadBookingDetails,
            );
          }

          if (state is BookingDetailsLoaded) {
            return _buildContent(state);
          }

          return const SizedBox.shrink();
        },
      ),
    );
  }

  Widget _buildContent(BookingDetailsLoaded state) {
    final booking = state.booking;
    final details = state.bookingDetails;

    return Stack(
      children: [
        CustomScrollView(
          controller: _scrollController,
          physics: const BouncingScrollPhysics(),
          slivers: [
            _buildSliverAppBar(state),
            SliverToBoxAdapter(
              child: AnimationLimiter(
                child: Column(
                  children: AnimationConfiguration.toStaggeredList(
                    duration: const Duration(milliseconds: 375),
                    childAnimationBuilder: (widget) => SlideAnimation(
                      verticalOffset: 50.0,
                      child: FadeInAnimation(child: widget),
                    ),
                    children: [
                      _buildBookingInfoCard(state),
                      _buildGuestInfoCard(state),
                      _buildUnitInfoCard(state),
                      _buildPaymentSection(state),
                      _buildServicesSection(state),
                      _buildActivityTimeline(state),
                      _buildReviewSection(state),
                      const SizedBox(height: 100),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
        _buildBottomActions(state),
      ],
    );
  }

  Widget _buildSliverAppBar(BookingDetailsLoaded state) {
    final booking = state.booking;
    final parallaxOffset = _scrollOffset * 0.5;

    return SliverAppBar(
      expandedHeight: 280,
      pinned: true,
      backgroundColor: AppTheme.darkBackground,
      leading: _buildBackButton(),
      actions: [
        _buildActionButton(
          icon: CupertinoIcons.share,
          onPressed: () => _shareBooking(booking.id),
        ),
        _buildActionButton(
          icon: CupertinoIcons.time,
          onPressed: () => context.push('/admin/bookings/${booking.id}/audit'),
        ),
        _buildActionButton(
          icon: CupertinoIcons.printer,
          onPressed: () => _printBooking(booking.id),
        ),
        const SizedBox(width: 8),
      ],
      flexibleSpace: FlexibleSpaceBar(
        background: Stack(
          fit: StackFit.expand,
          children: [
            // Background image with parallax
            if (booking.unitImage != null)
              Transform.translate(
                offset: Offset(0, parallaxOffset),
                child: Container(
                  decoration: BoxDecoration(
                    image: DecorationImage(
                      image: NetworkImage(booking.unitImage!),
                      fit: BoxFit.cover,
                    ),
                  ),
                  child: Container(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                        colors: [
                          Colors.transparent,
                          AppTheme.darkBackground.withOpacity(0.7),
                          AppTheme.darkBackground,
                        ],
                        stops: const [0.3, 0.6, 1.0],
                      ),
                    ),
                  ),
                ),
              ),
            // Content
            Positioned(
              bottom: 0,
              left: 0,
              right: 0,
              child: Container(
                padding: const EdgeInsets.all(20),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                'حجز #${booking.id.substring(0, 8)}',
                                style: AppTextStyles.caption.copyWith(
                                  color: AppTheme.textMuted,
                                ),
                              ),
                              const SizedBox(height: 4),
                              Text(
                                booking.unitName,
                                style: AppTextStyles.heading1.copyWith(
                                  color: AppTheme.textWhite,
                                  shadows: [
                                    Shadow(
                                      color: Colors.black.withOpacity(0.3),
                                      blurRadius: 10,
                                    ),
                                  ],
                                ),
                              ),
                            ],
                          ),
                        ),
                        BookingStatusBadge(status: booking.status),
                      ],
                    ),
                    const SizedBox(height: 16),
                    _buildInfoRow(
                      icon: CupertinoIcons.calendar,
                      label: 'تاريخ الحجز',
                      value: Formatters.formatDate(booking.bookedAt),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBackButton() {
    return Container(
      margin: const EdgeInsets.all(8),
      decoration: BoxDecoration(
        color: AppTheme.darkCard.withOpacity(0.8),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppTheme.darkBorder.withOpacity(0.3),
        ),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: () => Navigator.of(context).pop(),
          borderRadius: BorderRadius.circular(12),
          child: Icon(
            CupertinoIcons.arrow_right,
            color: AppTheme.textWhite,
            size: 20,
          ),
        ),
      ),
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required VoidCallback onPressed,
  }) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 4, vertical: 8),
      decoration: BoxDecoration(
        color: AppTheme.darkCard.withOpacity(0.8),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppTheme.darkBorder.withOpacity(0.3),
        ),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onPressed,
          borderRadius: BorderRadius.circular(12),
          child: Container(
            padding: const EdgeInsets.all(8),
            child: Icon(
              icon,
              color: AppTheme.textWhite,
              size: 20,
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildBookingInfoCard(BookingDetailsLoaded state) {
    final booking = state.booking;
    final details = state.bookingDetails;

    return _buildGlassCard(
      title: 'معلومات الحجز',
      icon: CupertinoIcons.doc_text_fill,
      child: Column(
        children: [
          _buildDetailRow(
            label: 'تاريخ الوصول',
            value: Formatters.formatDate(booking.checkIn),
            icon: CupertinoIcons.arrow_down_circle,
          ),
          _buildDetailRow(
            label: 'تاريخ المغادرة',
            value: Formatters.formatDate(booking.checkOut),
            icon: CupertinoIcons.arrow_up_circle,
          ),
          _buildDetailRow(
            label: 'عدد الليالي',
            value: '${booking.nights} ليلة',
            icon: CupertinoIcons.moon_fill,
          ),
          _buildDetailRow(
            label: 'عدد الضيوف',
            value: '${booking.guestsCount} ضيف',
            icon: CupertinoIcons.person_2_fill,
          ),
          if (booking.bookingSource != null)
            _buildDetailRow(
              label: 'مصدر الحجز',
              value: booking.bookingSource!,
              icon: CupertinoIcons.link,
            ),
          if (booking.isWalkIn == true)
            _buildDetailRow(
              label: 'حجز مباشر (Walk-in)',
              value: 'نعم',
              icon: CupertinoIcons.person_crop_circle_badge_checkmark,
            ),
          if (booking.confirmedAt != null)
            _buildDetailRow(
              label: 'تاريخ التأكيد',
              value: Formatters.formatDateTime(booking.confirmedAt!),
              icon: CupertinoIcons.checkmark_seal_fill,
            ),
          if (booking.checkedInAt != null)
            _buildDetailRow(
              label: 'تسجيل الوصول الفعلي',
              value: Formatters.formatDateTime(booking.checkedInAt!),
              icon: CupertinoIcons.arrow_down_circle_fill,
            ),
          if (booking.checkedOutAt != null)
            _buildDetailRow(
              label: 'تسجيل المغادرة الفعلي',
              value: Formatters.formatDateTime(booking.checkedOutAt!),
              icon: CupertinoIcons.arrow_up_circle_fill,
            ),
          if (booking.cancellationReason != null)
            _buildDetailRow(
              label: 'سبب الإلغاء',
              value: booking.cancellationReason!,
              icon: CupertinoIcons.xmark_octagon_fill,
              isMultiline: true,
            ),
          if (booking.paymentStatus != null)
            _buildDetailRow(
              label: 'حالة الدفع',
              value: booking.paymentStatus!,
              icon: CupertinoIcons.creditcard_fill,
            ),
          if (booking.notes != null)
            _buildDetailRow(
              label: 'ملاحظات',
              value: booking.notes!,
              icon: CupertinoIcons.text_bubble,
              isMultiline: true,
            ),
          if (booking.specialRequests != null)
            _buildDetailRow(
              label: 'طلبات خاصة',
              value: booking.specialRequests!,
              icon: CupertinoIcons.square_list_fill,
              isMultiline: true,
            ),
        ],
      ),
    );
  }

  Widget _buildGuestInfoCard(BookingDetailsLoaded state) {
    final booking = state.booking;
    final guestInfo = state.bookingDetails?.guestInfo;

    return _buildGlassCard(
      title: 'معلومات الضيف',
      icon: CupertinoIcons.person_circle_fill,
      child: Column(
        children: [
          _buildDetailRow(
            label: 'الاسم',
            value: booking.userName,
            icon: CupertinoIcons.person,
          ),
          if (booking.userEmail != null)
            _buildDetailRow(
              label: 'البريد الإلكتروني',
              value: booking.userEmail!,
              icon: CupertinoIcons.mail,
            ),
          if (booking.userPhone != null)
            _buildDetailRow(
              label: 'رقم الهاتف',
              value: booking.userPhone!,
              icon: CupertinoIcons.phone,
            ),
          if (guestInfo?.nationality != null)
            _buildDetailRow(
              label: 'الجنسية',
              value: guestInfo!.nationality!,
              icon: CupertinoIcons.flag,
            ),
        ],
      ),
    );
  }

  Widget _buildUnitInfoCard(BookingDetailsLoaded state) {
    final booking = state.booking;
    final unitDetails = state.bookingDetails?.unitDetails;
    final propertyDetails = state.bookingDetails?.propertyDetails;

    return _buildGlassCard(
      title: 'معلومات الوحدة',
      icon: CupertinoIcons.home,
      child: Column(
        children: [
          _buildDetailRow(
            label: 'اسم الوحدة',
            value: booking.unitName,
            icon: CupertinoIcons.building_2_fill,
          ),
          if (booking.propertyName != null)
            _buildDetailRow(
              label: 'العقار',
              value: booking.propertyName!,
              icon: CupertinoIcons.location,
            ),
          if (propertyDetails?.address != null &&
              propertyDetails!.address.isNotEmpty)
            _buildDetailRow(
              label: 'عنوان العقار',
              value: propertyDetails.address,
              icon: CupertinoIcons.map_pin_ellipse,
              isMultiline: true,
            ),
          if (unitDetails?.type != null)
            _buildDetailRow(
              label: 'النوع',
              value: unitDetails!.type,
              icon: CupertinoIcons.square_grid_2x2,
            ),
          if (unitDetails?.capacity != null)
            _buildDetailRow(
              label: 'السعة',
              value: '${unitDetails!.capacity} شخص',
              icon: CupertinoIcons.person_3_fill,
            ),
        ],
      ),
    );
  }

  Widget _buildPaymentSection(BookingDetailsLoaded state) {
    return Container(
      margin: const EdgeInsets.all(16),
      child: BookingPaymentSummary(
        booking: state.booking,
        payments: state.bookingDetails?.payments ?? [],
      ),
    );
  }

  Widget _buildServicesSection(BookingDetailsLoaded state) {
    if (state.services.isEmpty) return const SizedBox.shrink();

    return Container(
      margin: const EdgeInsets.all(16),
      child: BookingServicesWidget(
        services: state.services,
        onAddService: () => _showAddServiceDialog(state.booking.id),
        onRemoveService: (serviceId) =>
            _removeService(serviceId, state.booking.id),
      ),
    );
  }

  Widget _buildActivityTimeline(BookingDetailsLoaded state) {
    final activities = state.bookingDetails?.activities ?? [];
    if (activities.isEmpty) return const SizedBox.shrink();

    return _buildGlassCard(
      title: 'سجل النشاطات',
      icon: CupertinoIcons.time,
      child: Column(
        children: activities.map((activity) {
          return Container(
            margin: const EdgeInsets.symmetric(vertical: 8),
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: AppTheme.darkBackground.withOpacity(0.5),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(
                color: AppTheme.darkBorder.withOpacity(0.2),
              ),
            ),
            child: Row(
              children: [
                Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: AppTheme.primaryBlue.withOpacity(0.1),
                    shape: BoxShape.circle,
                  ),
                  child: Icon(
                    CupertinoIcons.clock_fill,
                    color: AppTheme.primaryBlue,
                    size: 20,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        activity.description,
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: AppTheme.textWhite,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        Formatters.formatDateTime(activity.timestamp),
                        style: AppTextStyles.caption.copyWith(
                          color: AppTheme.textMuted,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          );
        }).toList(),
      ),
    );
  }

  Widget _buildReviewSection(BookingDetailsLoaded state) {
    final review = state.review;
    if (review == null) return const SizedBox.shrink();

    return _buildGlassCard(
      title: 'تقييم الضيف',
      icon: CupertinoIcons.star_fill,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: List.generate(5, (index) {
              final value = review.averageRating;
              final filled = index < value.floor();
              final half = index == value.floor() && (value % 1) != 0;
              return Padding(
                padding: const EdgeInsets.only(right: 4),
                child: Icon(
                  half
                      ? CupertinoIcons.star_lefthalf_fill
                      : (filled
                          ? CupertinoIcons.star_fill
                          : CupertinoIcons.star),
                  size: 18,
                  color: AppTheme.warning,
                ),
              );
            }),
          ),
          const SizedBox(height: 12),
          if (review.comment.isNotEmpty)
            _buildDetailRow(
              label: 'تعليق',
              value: review.comment,
              icon: CupertinoIcons.text_bubble,
              isMultiline: true,
            ),
          const SizedBox(height: 8),
          _buildDetailRow(
            label: 'النظافة',
            value: review.cleanliness.toStringAsFixed(1),
            icon: CupertinoIcons.sparkles,
          ),
          _buildDetailRow(
            label: 'الخدمة',
            value: review.service.toStringAsFixed(1),
            icon: CupertinoIcons.person_2,
          ),
          _buildDetailRow(
            label: 'الموقع',
            value: review.location.toStringAsFixed(1),
            icon: CupertinoIcons.location_solid,
          ),
          _buildDetailRow(
            label: 'القيمة',
            value: review.value.toStringAsFixed(1),
            icon: CupertinoIcons.money_dollar,
          ),
          const SizedBox(height: 8),
          _buildDetailRow(
            label: 'تاريخ التقييم',
            value: Formatters.formatDate(review.createdAt),
            icon: CupertinoIcons.calendar_today,
          ),
          if (review.responseText != null && review.responseText!.isNotEmpty)
            _buildDetailRow(
              label: 'رد الإدارة',
              value: review.responseText!,
              icon: CupertinoIcons.reply,
              isMultiline: true,
            ),
          if (review.images.isNotEmpty) ...[
            const SizedBox(height: 12),
            SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: review.images.map((img) {
                  return Container(
                    margin: const EdgeInsets.only(right: 8),
                    width: 72,
                    height: 72,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(10),
                      image: DecorationImage(
                        image: NetworkImage(img.url),
                        fit: BoxFit.cover,
                      ),
                    ),
                  );
                }).toList(),
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildBottomActions(BookingDetailsLoaded state) {
    final booking = state.booking;

    return Positioned(
      bottom: 0,
      left: 0,
      right: 0,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: AppTheme.darkCard,
          borderRadius: const BorderRadius.vertical(
            top: Radius.circular(24),
          ),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.1),
              blurRadius: 10,
              offset: const Offset(0, -5),
            ),
          ],
        ),
        child: Row(
          children: [
            if (booking.canCheckIn)
              Expanded(
                child: _buildActionButtonLarge(
                  label: 'تسجيل وصول',
                  icon: CupertinoIcons.arrow_down_circle_fill,
                  gradient: AppTheme.primaryGradient,
                  onPressed: () => _showCheckInDialog(booking.id),
                ),
              ),
            if (booking.canCheckOut)
              Expanded(
                child: _buildActionButtonLarge(
                  label: 'تسجيل مغادرة',
                  icon: CupertinoIcons.arrow_up_circle_fill,
                  gradient: AppTheme.primaryGradient,
                  onPressed: () => _showCheckOutDialog(booking.id),
                ),
              ),
            if (booking.canCancel) ...[
              const SizedBox(width: 12),
              Expanded(
                child: _buildActionButtonLarge(
                  label: 'إلغاء الحجز',
                  icon: CupertinoIcons.xmark_circle_fill,
                  gradient: LinearGradient(
                    colors: [
                      AppTheme.error.withOpacity(0.8),
                      AppTheme.error,
                    ],
                  ),
                  onPressed: () => _showCancelDialog(booking.id),
                ),
              ),
            ],
            if (booking.canConfirm) ...[
              const SizedBox(width: 12),
              Expanded(
                child: _buildActionButtonLarge(
                  label: 'تأكيد الحجز',
                  icon: CupertinoIcons.checkmark_circle_fill,
                  gradient: LinearGradient(
                    colors: [
                      AppTheme.success.withOpacity(0.8),
                      AppTheme.success,
                    ],
                  ),
                  onPressed: () => _confirmBooking(booking.id),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildActionButtonLarge({
    required String label,
    required IconData icon,
    required Gradient gradient,
    required VoidCallback onPressed,
  }) {
    return Container(
      height: 56,
      decoration: BoxDecoration(
        gradient: gradient,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: gradient.colors.first.withOpacity(0.3),
            blurRadius: 10,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onPressed,
          borderRadius: BorderRadius.circular(16),
          child: Center(
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(icon, color: Colors.white, size: 20),
                const SizedBox(width: 8),
                Text(
                  label,
                  style: AppTextStyles.buttonMedium.copyWith(
                    color: Colors.white,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildGlassCard({
    required String title,
    required IconData icon,
    required Widget child,
  }) {
    return Container(
      margin: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppTheme.darkCard.withOpacity(0.5),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: AppTheme.darkBorder.withOpacity(0.3),
        ),
        boxShadow: [
          BoxShadow(
            color: AppTheme.shadowDark.withOpacity(0.1),
            blurRadius: 20,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: BackdropFilter(
          filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
          child: Container(
            padding: const EdgeInsets.all(20),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: AppTheme.primaryBlue.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: Icon(
                        icon,
                        color: AppTheme.primaryBlue,
                        size: 20,
                      ),
                    ),
                    const SizedBox(width: 12),
                    Text(
                      title,
                      style: AppTextStyles.heading3.copyWith(
                        color: AppTheme.textWhite,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                child,
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildDetailRow({
    required String label,
    required String value,
    required IconData icon,
    bool isMultiline = false,
  }) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 12),
      decoration: BoxDecoration(
        border: Border(
          bottom: BorderSide(
            color: AppTheme.darkBorder.withOpacity(0.1),
          ),
        ),
      ),
      child: Row(
        crossAxisAlignment:
            isMultiline ? CrossAxisAlignment.start : CrossAxisAlignment.center,
        children: [
          Icon(
            icon,
            color: AppTheme.textMuted,
            size: 18,
          ),
          const SizedBox(width: 12),
          SizedBox(
            width: 100,
            child: Text(
              label,
              style: AppTextStyles.caption.copyWith(
                color: AppTheme.textMuted,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppTheme.textWhite,
                fontWeight: FontWeight.w500,
              ),
              textAlign: TextAlign.end,
              maxLines: isMultiline ? null : 1,
              overflow:
                  isMultiline ? TextOverflow.visible : TextOverflow.ellipsis,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildInfoRow({
    required IconData icon,
    required String label,
    required String value,
  }) {
    return Row(
      children: [
        Icon(
          icon,
          color: AppTheme.textMuted,
          size: 16,
        ),
        const SizedBox(width: 6),
        Text(
          label,
          style: AppTextStyles.caption.copyWith(
            color: AppTheme.textMuted,
          ),
        ),
        const SizedBox(width: 8),
        Text(
          value,
          style: AppTextStyles.bodyMedium.copyWith(
            color: AppTheme.textWhite,
            fontWeight: FontWeight.w500,
          ),
        ),
      ],
    );
  }

  void _showCheckInDialog(String bookingId) {
    showDialog(
      fullscreenDialog: true,
      context: context,
      builder: (context) => CheckInOutDialog(
        bookingId: bookingId,
        isCheckIn: true,
        onConfirm: () {
          context.read<BookingDetailsBloc>().add(
                CheckInBookingDetailsEvent(bookingId: bookingId),
              );
        },
      ),
    );
  }

  void _showCheckOutDialog(String bookingId) {
    showDialog(
      fullscreenDialog: true,
      context: context,
      builder: (context) => CheckInOutDialog(
        bookingId: bookingId,
        isCheckIn: false,
        onConfirm: () {
          context.read<BookingDetailsBloc>().add(
                CheckOutBookingDetailsEvent(bookingId: bookingId),
              );
        },
      ),
    );
  }

  void _showCancelDialog(String bookingId) {
    showDialog(
      fullscreenDialog: true,
      context: context,
      builder: (context) => BookingActionsDialog(
        bookingId: bookingId,
        action: BookingAction.cancel,
        onConfirm: (reason) {
          context.read<BookingDetailsBloc>().add(
                CancelBookingDetailsEvent(
                  bookingId: bookingId,
                  cancellationReason: reason ?? 'إلغاء بواسطة الإدارة',
                ),
              );
        },
      ),
    );
  }

  void _confirmBooking(String bookingId) {
    context.read<BookingDetailsBloc>().add(
          ConfirmBookingDetailsEvent(bookingId: bookingId),
        );
  }

  void _showAddServiceDialog(String bookingId) {
    // Implement add service dialog
  }

  void _removeService(String serviceId, String bookingId) {
    context.read<BookingDetailsBloc>().add(
          RemoveServiceEvent(
            bookingId: bookingId,
            serviceId: serviceId,
          ),
        );
  }

  void _shareBooking(String bookingId) {
    context.read<BookingDetailsBloc>().add(
          ShareBookingDetailsEvent(bookingId: bookingId),
        );
  }

  void _printBooking(String bookingId) {
    context.read<BookingDetailsBloc>().add(
          PrintBookingDetailsEvent(bookingId: bookingId),
        );
  }
}
