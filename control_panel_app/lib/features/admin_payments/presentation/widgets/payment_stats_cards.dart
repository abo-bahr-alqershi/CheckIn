import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';
import 'dart:ui';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/theme/app_dimensions.dart';

class PaymentStatsCards extends StatefulWidget {
  final Map<String, dynamic> statistics;

  const PaymentStatsCards({
    super.key,
    required this.statistics,
  });

  @override
  State<PaymentStatsCards> createState() => _PaymentStatsCardsState();
}

class _PaymentStatsCardsState extends State<PaymentStatsCards>
    with TickerProviderStateMixin {
  late List<AnimationController> _cardControllers;
  late List<Animation<double>> _scaleAnimations;
  late List<Animation<double>> _fadeAnimations;
  late AnimationController _countController;

  @override
  void initState() {
    super.initState();
    _initAnimations();
  }

  void _initAnimations() {
    _cardControllers = List.generate(
      4,
      (index) => AnimationController(
        duration: Duration(milliseconds: 500 + (index * 100)),
        vsync: this,
      ),
    );

    _scaleAnimations = _cardControllers.map((controller) {
      return Tween<double>(
        begin: 0.8,
        end: 1.0,
      ).animate(CurvedAnimation(
        parent: controller,
        curve: Curves.elasticOut,
      ));
    }).toList();

    _fadeAnimations = _cardControllers.map((controller) {
      return Tween<double>(
        begin: 0.0,
        end: 1.0,
      ).animate(CurvedAnimation(
        parent: controller,
        curve: Curves.easeIn,
      ));
    }).toList();

    _countController = AnimationController(
      duration: const Duration(milliseconds: 1500),
      vsync: this,
    );

    // Start animations
    for (var controller in _cardControllers) {
      controller.forward();
    }
    _countController.forward();
  }

  @override
  void dispose() {
    for (var controller in _cardControllers) {
      controller.dispose();
    }
    _countController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 180,
      child: ListView(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(vertical: 8),
        children: [
          _buildStatCard(
            index: 0,
            title: 'إجمالي المدفوعات',
            value: widget.statistics['totalPayments'] ?? 0,
            icon: CupertinoIcons.creditcard_fill,
            gradient: LinearGradient(
              colors: [
                AppTheme.primaryBlue,
                AppTheme.primaryBlue.withValues(alpha: 0.7),
              ],
            ),
            suffix: 'معاملة',
            trend: 12.5,
          ),
          _buildStatCard(
            index: 1,
            title: 'إجمالي المبلغ',
            value: widget.statistics['totalAmount'] ?? 0,
            icon: CupertinoIcons.money_dollar_circle_fill,
            gradient: LinearGradient(
              colors: [
                AppTheme.success,
                AppTheme.success.withValues(alpha: 0.7),
              ],
            ),
            isCurrency: true,
            trend: 8.3,
          ),
          _buildStatCard(
            index: 2,
            title: 'المدفوعات الناجحة',
            value: widget.statistics['successfulPayments'] ?? 0,
            icon: CupertinoIcons.checkmark_seal_fill,
            gradient: LinearGradient(
              colors: [
                AppTheme.primaryPurple,
                AppTheme.primaryPurple.withValues(alpha: 0.7),
              ],
            ),
            suffix: 'معاملة',
            trend: 5.2,
          ),
          _buildStatCard(
            index: 3,
            title: 'المستردات',
            value: widget.statistics['refundedPayments'] ?? 0,
            icon: CupertinoIcons.arrow_counterclockwise_circle_fill,
            gradient: LinearGradient(
              colors: [
                AppTheme.warning,
                AppTheme.warning.withValues(alpha: 0.7),
              ],
            ),
            suffix: 'معاملة',
            trend: -2.1,
          ),
        ],
      ),
    );
  }

  Widget _buildStatCard({
    required int index,
    required String title,
    required dynamic value,
    required IconData icon,
    required Gradient gradient,
    String? suffix,
    bool isCurrency = false,
    double? trend,
  }) {
    return AnimatedBuilder(
      animation: _scaleAnimations[index],
      builder: (context, child) {
        return Transform.scale(
          scale: _scaleAnimations[index].value,
          child: FadeTransition(
            opacity: _fadeAnimations[index],
            child: Container(
              width: 200,
              margin: const EdgeInsets.only(right: 16),
              decoration: BoxDecoration(
                gradient: gradient,
                borderRadius: BorderRadius.circular(20),
                boxShadow: [
                  BoxShadow(
                    color: gradient.colors.first.withValues(alpha: 0.3),
                    blurRadius: 12,
                    offset: const Offset(0, 6),
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
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Container(
                              padding: const EdgeInsets.all(10),
                              decoration: BoxDecoration(
                                color:
                                    AppTheme.textWhite.withValues(alpha: 0.2),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Icon(
                                icon,
                                color: AppTheme.textWhite,
                                size: 24,
                              ),
                            ),
                            if (trend != null)
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 8,
                                  vertical: 4,
                                ),
                                decoration: BoxDecoration(
                                  color:
                                      AppTheme.textWhite.withValues(alpha: 0.2),
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                child: Row(
                                  children: [
                                    Icon(
                                      trend > 0
                                          ? CupertinoIcons.arrow_up_right
                                          : CupertinoIcons.arrow_down_right,
                                      color: AppTheme.textWhite,
                                      size: 12,
                                    ),
                                    const SizedBox(width: 4),
                                    Text(
                                      '${trend.abs().toStringAsFixed(1)}%',
                                      style: AppTextStyles.caption.copyWith(
                                        color: AppTheme.textWhite,
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                          ],
                        ),
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              title,
                              style: AppTextStyles.caption.copyWith(
                                color:
                                    AppTheme.textWhite.withValues(alpha: 0.8),
                              ),
                            ),
                            const SizedBox(height: 8),
                            TweenAnimationBuilder<double>(
                              tween: Tween(
                                begin: 0,
                                end: value.toDouble(),
                              ),
                              duration: const Duration(milliseconds: 1500),
                              builder: (context, animatedValue, child) {
                                return Row(
                                  crossAxisAlignment: CrossAxisAlignment.end,
                                  children: [
                                    Text(
                                      isCurrency
                                          ? _formatCurrency(animatedValue)
                                          : _formatNumber(animatedValue),
                                      style: AppTextStyles.heading1.copyWith(
                                        color: AppTheme.textWhite,
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                                    if (suffix != null) ...[
                                      const SizedBox(width: 6),
                                      Text(
                                        suffix,
                                        style: AppTextStyles.caption.copyWith(
                                          color: AppTheme.textWhite
                                              .withValues(alpha: 0.8),
                                        ),
                                      ),
                                    ],
                                  ],
                                );
                              },
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        );
      },
    );
  }

  String _formatNumber(double value) {
    if (value >= 1000000) {
      return '${(value / 1000000).toStringAsFixed(1)}M';
    } else if (value >= 1000) {
      return '${(value / 1000).toStringAsFixed(1)}K';
    }
    return value.toStringAsFixed(0);
  }

  String _formatCurrency(double value) {
    if (value >= 1000000) {
      return '${(value / 1000000).toStringAsFixed(1)}M ر.ي';
    } else if (value >= 1000) {
      return '${(value / 1000).toStringAsFixed(1)}K ر.ي';
    }
    return '${value.toStringAsFixed(0)} ر.ي';
  }
}
