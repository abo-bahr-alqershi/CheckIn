import 'package:bookn_cp_app/features/admin_bookings/presentation/bloc/booking_analytics/booking_analytics_event.dart'
    hide ExportFormat;
import 'package:bookn_cp_app/features/admin_payments/presentation/bloc/payments_list/payments_list_bloc.dart';
import 'package:bookn_cp_app/features/admin_payments/presentation/bloc/payments_list/payments_list_event.dart';
import 'package:bookn_cp_app/features/admin_payments/presentation/bloc/payments_list/payments_list_state.dart';
import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'dart:ui';
import '../widgets/futuristic_payments_table.dart';
import '../widgets/payment_filters_widget.dart';
import '../widgets/payment_stats_cards.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/widgets/loading_widget.dart';
import '../../../../../../core/widgets/error_widget.dart';
import 'payment_details_page.dart';

class PaymentsListPage extends StatefulWidget {
  const PaymentsListPage({super.key});

  @override
  State<PaymentsListPage> createState() => _PaymentsListPageState();
}

class _PaymentsListPageState extends State<PaymentsListPage>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;
  final ScrollController _scrollController = ScrollController();
  bool _showFilters = false;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );
    _fadeAnimation = CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
    );
    _animationController.forward();

    // Load payments
    context.read<PaymentsListBloc>().add(const LoadPaymentsEvent());
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
      body: Container(
        decoration: BoxDecoration(
          gradient: AppTheme.darkGradient,
        ),
        child: SafeArea(
          child: Column(
            children: [
              _buildHeader(),
              Expanded(
                child: BlocBuilder<PaymentsListBloc, PaymentsListState>(
                  builder: (context, state) {
                    if (state is PaymentsListLoading) {
                      return const LoadingWidget(
                        type: LoadingType.futuristic,
                        message: 'جاري تحميل المدفوعات...',
                      );
                    }

                    if (state is PaymentsListError) {
                      return CustomErrorWidget(
                        message: state.message,
                        type: ErrorType.general,
                        onRetry: () {
                          context.read<PaymentsListBloc>().add(
                                const RefreshPaymentsEvent(),
                              );
                        },
                      );
                    }

                    if (state is PaymentsListLoaded) {
                      return _buildLoadedContent(state);
                    }

                    return const SizedBox();
                  },
                ),
              ),
            ],
          ),
        ),
      ),
      floatingActionButton: _buildFloatingActionButton(),
    );
  }

  Widget _buildHeader() {
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
                    color: AppTheme.darkCard,
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
              const SizedBox(width: 12),
              // Title
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    ShaderMask(
                      shaderCallback: (bounds) =>
                          AppTheme.primaryGradient.createShader(bounds),
                      child: Text(
                        'إدارة المدفوعات',
                        style: AppTextStyles.heading1.copyWith(
                          color: AppTheme.textWhite,
                        ),
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'متابعة وإدارة جميع المعاملات المالية',
                      style: AppTextStyles.caption.copyWith(
                        color: AppTheme.textMuted,
                      ),
                    ),
                  ],
                ),
              ),
              // Filter Button
              IconButton(
                onPressed: () {
                  setState(() {
                    _showFilters = !_showFilters;
                  });
                },
                icon: Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    gradient: _showFilters ? AppTheme.primaryGradient : null,
                    color: _showFilters ? null : AppTheme.darkCard,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color: _showFilters
                          ? Colors.transparent
                          : AppTheme.darkBorder,
                      width: 1,
                    ),
                  ),
                  child: Icon(
                    CupertinoIcons.slider_horizontal_3,
                    color: AppTheme.textWhite,
                    size: 20,
                  ),
                ),
              ),
            ],
          ),
          // Filters
          AnimatedContainer(
            duration: const Duration(milliseconds: 300),
            height: _showFilters ? 120 : 0,
            child: _showFilters
                ? FadeTransition(
                    opacity: _fadeAnimation,
                    child: const PaymentFiltersWidget(),
                  )
                : const SizedBox(),
          ),
        ],
      ),
    );
  }

  Widget _buildLoadedContent(PaymentsListLoaded state) {
    // الحصول على أبعاد الشاشة
    final screenSize = MediaQuery.of(context).size;
    final isSmallScreen = screenSize.width < 600;
    final isMediumScreen = screenSize.width >= 600 && screenSize.width < 900;

    // حساب الارتفاع المناسب للجدول
    final tableHeight = isSmallScreen
        ? null // للموبايل، نترك الارتفاع مرن
        : isMediumScreen
            ? screenSize.height * 0.6
            : screenSize.height * 0.7;

    return FadeTransition(
      opacity: _fadeAnimation,
      child: LayoutBuilder(
        builder: (context, constraints) {
          // للشاشات الصغيرة، نستخدم SingleChildScrollView
          if (isSmallScreen) {
            return SingleChildScrollView(
              controller: _scrollController,
              physics: const BouncingScrollPhysics(),
              child: Column(
                children: [
                  // Stats Cards
                  Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppDimensions.paddingLarge,
                    ),
                    child: PaymentStatsCards(
                      statistics: state.statistics,
                    ),
                  ),
                  const SizedBox(height: AppDimensions.spaceMedium),

                  // Payments Table
                  Padding(
                    padding: const EdgeInsets.all(AppDimensions.paddingLarge),
                    child: FuturisticPaymentsTable(
                      payments: state.payments.items,
                      onPaymentTap: (payment) {
                        Navigator.push(
                          context,
                          CupertinoPageRoute(
                            builder: (_) => PaymentDetailsPage(
                              paymentId: payment.id,
                            ),
                          ),
                        );
                      },
                      onRefundTap: (payment) {
                        _showRefundDialog(payment);
                      },
                      onVoidTap: (payment) {
                        _showVoidConfirmation(payment);
                      },
                      // لا نحدد الارتفاع للموبايل
                    ),
                  ),

                  // Pagination
                  if (state.payments.hasNextPage)
                    Padding(
                      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
                      child: _buildLoadMoreButton(),
                    ),
                ],
              ),
            );
          }

          // للشاشات الأكبر، نستخدم CustomScrollView
          return CustomScrollView(
            controller: _scrollController,
            physics: const BouncingScrollPhysics(),
            slivers: [
              // Stats Cards
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingLarge,
                  ),
                  child: PaymentStatsCards(
                    statistics: state.statistics,
                  ),
                ),
              ),

              const SliverToBoxAdapter(
                child: SizedBox(height: AppDimensions.spaceMedium),
              ),

              // Payments Table - استخدام SliverFillRemaining
              SliverFillRemaining(
                hasScrollBody: false,
                fillOverscroll: false,
                child: Padding(
                  padding: const EdgeInsets.all(AppDimensions.paddingLarge),
                  child: FuturisticPaymentsTable(
                    payments: state.payments.items,
                    height: tableHeight, // تحديد الارتفاع
                    onPaymentTap: (payment) {
                      Navigator.push(
                        context,
                        CupertinoPageRoute(
                          builder: (_) => PaymentDetailsPage(
                            paymentId: payment.id,
                          ),
                        ),
                      );
                    },
                    onRefundTap: (payment) {
                      _showRefundDialog(payment);
                    },
                    onVoidTap: (payment) {
                      _showVoidConfirmation(payment);
                    },
                  ),
                ),
              ),

              // Pagination
              if (state.payments.hasNextPage)
                SliverToBoxAdapter(
                  child: Padding(
                    padding: const EdgeInsets.all(AppDimensions.paddingLarge),
                    child: Center(
                      child: _buildLoadMoreButton(),
                    ),
                  ),
                ),
            ],
          );
        },
      ),
    );
  }

  Widget _buildLoadMoreButton() {
    return Container(
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: AppTheme.primaryBlue.withValues(alpha: 0.1),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: CupertinoButton(
        padding: const EdgeInsets.symmetric(
          horizontal: 32,
          vertical: 14,
        ),
        color: AppTheme.darkCard,
        borderRadius: BorderRadius.circular(12),
        onPressed: () {
          final state = context.read<PaymentsListBloc>().state;
          if (state is PaymentsListLoaded) {
            context.read<PaymentsListBloc>().add(
                  ChangePageEvent(
                    pageNumber: state.payments.currentPage + 1,
                  ),
                );
          }
        },
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              CupertinoIcons.arrow_down_circle,
              color: AppTheme.primaryBlue,
              size: 20,
            ),
            const SizedBox(width: 8),
            Text(
              'تحميل المزيد',
              style: AppTextStyles.buttonMedium.copyWith(
                color: AppTheme.primaryBlue,
              ),
            ),
          ],
        ),
      ),
    );
  }

  // إضافة دالة لإظهار نافذة تأكيد الاسترداد
  void _showRefundDialog(payment) {
    showCupertinoDialog(
      context: context,
      builder: (context) => CupertinoAlertDialog(
        title: const Text(
          'تأكيد الاسترداد',
          style: AppTextStyles.heading3,
        ),
        content: Padding(
          padding: const EdgeInsets.only(top: 8.0),
          child: Text(
            'هل تريد استرداد المبلغ ${payment.amount.amount} ${payment.amount.currency}؟',
            style: AppTextStyles.bodyMedium,
          ),
        ),
        actions: [
          CupertinoDialogAction(
            isDestructiveAction: true,
            onPressed: () {
              Navigator.pop(context);
              context.read<PaymentsListBloc>().add(
                    RefundPaymentEvent(
                      paymentId: payment.id,
                      refundAmount: payment.amount,
                      refundReason: 'طلب العميل',
                    ),
                  );
            },
            child: const Text('استرداد'),
          ),
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () => Navigator.pop(context),
            child: const Text('إلغاء'),
          ),
        ],
      ),
    );
  }

  // إضافة دالة لإظهار نافذة تأكيد الإلغاء
  void _showVoidConfirmation(payment) {
    showCupertinoDialog(
      context: context,
      builder: (context) => CupertinoAlertDialog(
        title: const Text(
          'تأكيد إلغاء المعاملة',
          style: AppTextStyles.heading3,
        ),
        content: Padding(
          padding: const EdgeInsets.only(top: 8.0),
          child: Text(
            'هل تريد إلغاء المعاملة #${payment.transactionId}؟\nهذا الإجراء لا يمكن التراجع عنه.',
            style: AppTextStyles.bodyMedium,
            textAlign: TextAlign.center,
          ),
        ),
        actions: [
          CupertinoDialogAction(
            isDestructiveAction: true,
            onPressed: () {
              Navigator.pop(context);
              context.read<PaymentsListBloc>().add(
                    VoidPaymentEvent(paymentId: payment.id),
                  );
            },
            child: const Text('إلغاء المعاملة'),
          ),
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () => Navigator.pop(context),
            child: const Text('رجوع'),
          ),
        ],
      ),
    );
  }

  Widget _buildFloatingActionButton() {
    return Container(
      decoration: BoxDecoration(
        gradient: AppTheme.primaryGradient,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: AppTheme.primaryBlue.withValues(alpha: 0.3),
            blurRadius: 12,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: FloatingActionButton.extended(
        onPressed: () {
          context.read<PaymentsListBloc>().add(
                const ExportPaymentsEvent(format: ExportFormat.excel),
              );
        },
        backgroundColor: Colors.transparent,
        elevation: 0,
        icon: const Icon(CupertinoIcons.arrow_down_doc, size: 20),
        label: const Text(
          'تصدير',
          style: AppTextStyles.buttonMedium,
        ),
      ),
    );
  }
}
