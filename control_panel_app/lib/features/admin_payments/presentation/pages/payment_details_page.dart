import 'package:bookn_cp_app/features/admin_payments/presentation/bloc/payment_details/payment_details_bloc.dart';
import 'package:bookn_cp_app/features/admin_payments/presentation/bloc/payment_details/payment_details_event.dart';
import 'package:bookn_cp_app/features/admin_payments/presentation/bloc/payment_details/payment_details_state.dart';
import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'dart:ui';
import '../widgets/transaction_details_card.dart';
import '../widgets/payment_timeline_widget.dart';
import '../widgets/refund_dialog.dart';
import '../widgets/void_payment_dialog.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/widgets/loading_widget.dart';
import '../../../../../../core/widgets/error_widget.dart';

class PaymentDetailsPage extends StatefulWidget {
  final String paymentId;

  const PaymentDetailsPage({
    super.key,
    required this.paymentId,
  });

  @override
  State<PaymentDetailsPage> createState() => _PaymentDetailsPageState();
}

class _PaymentDetailsPageState extends State<PaymentDetailsPage>
    with TickerProviderStateMixin {
  late AnimationController _animationController;
  late AnimationController _pulseController;
  late Animation<double> _fadeAnimation;
  late Animation<double> _slideAnimation;
  late Animation<double> _pulseAnimation;

  @override
  void initState() {
    super.initState();
    _setupAnimations();

    // Load payment details
    context.read<PaymentDetailsBloc>().add(
          LoadPaymentDetailsEvent(paymentId: widget.paymentId),
        );
  }

  void _setupAnimations() {
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );

    _pulseController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat(reverse: true);

    _fadeAnimation = CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeIn,
    );

    _slideAnimation = Tween<double>(
      begin: 50,
      end: 0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOutCubic,
    ));

    _pulseAnimation = Tween<double>(
      begin: 1.0,
      end: 1.05,
    ).animate(CurvedAnimation(
      parent: _pulseController,
      curve: Curves.easeInOut,
    ));

    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    _pulseController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.darkBackground,
      body: Container(
        decoration: BoxDecoration(
          gradient: AppTheme.darkGradient,
        ),
        child: BlocBuilder<PaymentDetailsBloc, PaymentDetailsState>(
          builder: (context, state) {
            if (state is PaymentDetailsLoading) {
              return const Center(
                child: LoadingWidget(
                  type: LoadingType.pulse,
                  message: 'جاري تحميل تفاصيل الدفعة...',
                ),
              );
            }

            if (state is PaymentDetailsError) {
              return Center(
                child: CustomErrorWidget(
                  message: state.message,
                  type: ErrorType.general,
                  onRetry: () {
                    context.read<PaymentDetailsBloc>().add(
                          LoadPaymentDetailsEvent(paymentId: widget.paymentId),
                        );
                  },
                ),
              );
            }

            if (state is PaymentDetailsLoaded) {
              return _buildLoadedContent(state);
            }

            return const SizedBox();
          },
        ),
      ),
    );
  }

  Widget _buildLoadedContent(PaymentDetailsLoaded state) {
    return SafeArea(
      child: CustomScrollView(
        physics: const BouncingScrollPhysics(),
        slivers: [
          // App Bar
          SliverToBoxAdapter(
            child: _buildHeader(state),
          ),

          // Transaction Card
          SliverToBoxAdapter(
            child: FadeTransition(
              opacity: _fadeAnimation,
              child: AnimatedBuilder(
                animation: _slideAnimation,
                builder: (context, child) {
                  return Transform.translate(
                    offset: Offset(0, _slideAnimation.value),
                    child: Padding(
                      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
                      child: TransactionDetailsCard(
                        payment: state.payment,
                        paymentDetails: state.paymentDetails,
                      ),
                    ),
                  );
                },
              ),
            ),
          ),

          // Timeline
          if (state.activities.isNotEmpty)
            SliverToBoxAdapter(
              child: FadeTransition(
                opacity: _fadeAnimation,
                child: Padding(
                  padding: const EdgeInsets.all(AppDimensions.paddingLarge),
                  child: PaymentTimelineWidget(
                    activities: state.activities,
                    refunds: state.refunds,
                  ),
                ),
              ),
            ),

          // Actions
          SliverToBoxAdapter(
            child: _buildActions(state),
          ),

          const SliverToBoxAdapter(
            child: SizedBox(height: 100),
          ),
        ],
      ),
    );
  }

  Widget _buildHeader(PaymentDetailsLoaded state) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      child: Column(
        children: [
          Row(
            children: [
              // Back Button
              IconButton(
                onPressed: () => Navigator.pop(context),
                icon: Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: AppTheme.darkCard.withValues(alpha: 0.8),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color: AppTheme.darkBorder,
                      width: 1,
                    ),
                  ),
                  child: Icon(
                    CupertinoIcons.arrow_left,
                    color: AppTheme.textWhite,
                    size: 20,
                  ),
                ),
              ),

              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Text(
                      'معاملة #${state.payment.transactionId}',
                      style: AppTextStyles.heading2.copyWith(
                        color: AppTheme.textWhite,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 12,
                        vertical: 6,
                      ),
                      decoration: BoxDecoration(
                        color: _getStatusColor(state.payment.status)
                            .withValues(alpha: 0.2),
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(
                          color: _getStatusColor(state.payment.status)
                              .withValues(alpha: 0.5),
                          width: 1,
                        ),
                      ),
                      child: Text(
                        _getStatusText(state.payment.status),
                        style: AppTextStyles.caption.copyWith(
                          color: _getStatusColor(state.payment.status),
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              // More Button
              IconButton(
                onPressed: () => _showMoreOptions(state),
                icon: Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: AppTheme.darkCard.withValues(alpha: 0.8),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color: AppTheme.darkBorder,
                      width: 1,
                    ),
                  ),
                  child: Icon(
                    CupertinoIcons.ellipsis_vertical,
                    color: AppTheme.textWhite,
                    size: 20,
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildActions(PaymentDetailsLoaded state) {
    return Padding(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'الإجراءات',
            style: AppTextStyles.heading3.copyWith(
              color: AppTheme.textWhite,
            ),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              if (state.payment.canRefund) ...[
                Expanded(
                  child: _buildActionButton(
                    icon: CupertinoIcons.arrow_counterclockwise,
                    label: 'استرداد',
                    color: AppTheme.warning,
                    onTap: () => _showRefundDialog(state),
                  ),
                ),
                const SizedBox(width: 12),
              ],
              if (state.payment.canVoid) ...[
                Expanded(
                  child: _buildActionButton(
                    icon: CupertinoIcons.xmark_circle,
                    label: 'إلغاء',
                    color: AppTheme.error,
                    onTap: () => _showVoidDialog(state),
                  ),
                ),
                const SizedBox(width: 12),
              ],
              Expanded(
                child: _buildActionButton(
                  icon: CupertinoIcons.printer,
                  label: 'طباعة',
                  color: AppTheme.primaryBlue,
                  onTap: () => _printReceipt(state),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required String label,
    required Color color,
    required VoidCallback onTap,
  }) {
    return AnimatedBuilder(
      animation: _pulseAnimation,
      builder: (context, child) {
        return Transform.scale(
          scale: _pulseAnimation.value,
          child: Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  color.withValues(alpha: 0.2),
                  color.withValues(alpha: 0.1),
                ],
              ),
              borderRadius: BorderRadius.circular(16),
              border: Border.all(
                color: color.withValues(alpha: 0.3),
                width: 1,
              ),
            ),
            child: Material(
              color: Colors.transparent,
              child: InkWell(
                onTap: onTap,
                borderRadius: BorderRadius.circular(16),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 12,
                  ),
                  child: Column(
                    children: [
                      Icon(
                        icon,
                        color: color,
                        size: 24,
                      ),
                      const SizedBox(height: 8),
                      Text(
                        label,
                        style: AppTextStyles.caption.copyWith(
                          color: color,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        );
      },
    );
  }

  void _showRefundDialog(PaymentDetailsLoaded state) {
    showCupertinoModalPopup(
      context: context,
      builder: (_) => RefundDialog(
        payment: state.payment,
        onRefund: (amount, reason) {
          context.read<PaymentDetailsBloc>().add(
                RefundPaymentDetailsEvent(
                  paymentId: widget.paymentId,
                  refundAmount: amount,
                  refundReason: reason,
                ),
              );
        },
      ),
    );
  }

  void _showVoidDialog(PaymentDetailsLoaded state) {
    showCupertinoModalPopup(
      context: context,
      builder: (_) => VoidPaymentDialog(
        payment: state.payment,
        onVoid: () {
          context.read<PaymentDetailsBloc>().add(
                VoidPaymentDetailsEvent(paymentId: widget.paymentId),
              );
        },
      ),
    );
  }

  void _printReceipt(PaymentDetailsLoaded state) {
    context.read<PaymentDetailsBloc>().add(
          PrintReceiptEvent(paymentId: widget.paymentId),
        );
  }

  void _showMoreOptions(PaymentDetailsLoaded state) {
    showCupertinoModalPopup(
      context: context,
      builder: (context) => CupertinoActionSheet(
        title: const Text('خيارات إضافية'),
        actions: [
          CupertinoActionSheetAction(
            onPressed: () {
              Navigator.pop(context);
              context.read<PaymentDetailsBloc>().add(
                    SendReceiptEvent(
                      paymentId: widget.paymentId,
                      email: state.payment.userEmail,
                    ),
                  );
            },
            child: const Text('إرسال الإيصال بالبريد'),
          ),
          CupertinoActionSheetAction(
            onPressed: () {
              Navigator.pop(context);
              context.read<PaymentDetailsBloc>().add(
                    DownloadInvoiceEvent(paymentId: widget.paymentId),
                  );
            },
            child: const Text('تحميل الفاتورة'),
          ),
          CupertinoActionSheetAction(
            onPressed: () {
              Navigator.pop(context);
              // Add note dialog
            },
            child: const Text('إضافة ملاحظة'),
          ),
        ],
        cancelButton: CupertinoActionSheetAction(
          isDefaultAction: true,
          onPressed: () => Navigator.pop(context),
          child: const Text('إلغاء'),
        ),
      ),
    );
  }

  Color _getStatusColor(dynamic status) {
    // Implement based on PaymentStatus enum
    return AppTheme.success;
  }

  String _getStatusText(dynamic status) {
    // Implement based on PaymentStatus enum
    return 'مكتمل';
  }
}
