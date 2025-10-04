import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';
import 'package:fl_chart/fl_chart.dart';
import 'dart:ui';
import 'dart:math' as math;
import '../../domain/entities/payment.dart';
import '../../domain/entities/payment_analytics.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/enums/payment_method_enum.dart';

class PaymentBreakdownPieChart extends StatefulWidget {
  final Map<dynamic, MethodAnalytics> methodAnalytics;
  final double height;

  const PaymentBreakdownPieChart({
    super.key,
    required this.methodAnalytics,
    this.height = 300,
  });

  @override
  State<PaymentBreakdownPieChart> createState() =>
      _PaymentBreakdownPieChartState();
}

class _PaymentBreakdownPieChartState extends State<PaymentBreakdownPieChart>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _animation;
  int _touchedIndex = -1;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 1500),
      vsync: this,
    );

    _animation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.elasticOut,
    ));

    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: widget.height,
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            AppTheme.darkCard.withValues(alpha: 0.8),
            AppTheme.darkCard.withValues(alpha: 0.6),
          ],
        ),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: AppTheme.darkBorder.withValues(alpha: 0.3),
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            'توزيع طرق الدفع',
            style: AppTextStyles.heading3.copyWith(
              color: AppTheme.textWhite,
            ),
          ),
          const SizedBox(height: 20),
          Expanded(
            child: LayoutBuilder(
              builder: (context, constraints) {
                // للشاشات الصغيرة، عرض التخطيط بشكل عمودي
                if (constraints.maxWidth < 350) {
                  return _buildVerticalLayout();
                }
                // للشاشات الكبيرة، عرض التخطيط بشكل أفقي
                return _buildHorizontalLayout();
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildHorizontalLayout() {
    return Row(
      children: [
        Expanded(
          flex: 3,
          child: AnimatedBuilder(
            animation: _animation,
            builder: (context, child) {
              return Stack(
                alignment: Alignment.center,
                children: [
                  PieChart(
                    PieChartData(
                      pieTouchData: PieTouchData(
                        touchCallback: (FlTouchEvent event, pieTouchResponse) {
                          setState(() {
                            if (!event.isInterestedForInteractions ||
                                pieTouchResponse == null ||
                                pieTouchResponse.touchedSection == null) {
                              _touchedIndex = -1;
                              return;
                            }
                            _touchedIndex = pieTouchResponse
                                .touchedSection!.touchedSectionIndex;
                          });
                        },
                      ),
                      borderData: FlBorderData(show: false),
                      sectionsSpace: 2,
                      centerSpaceRadius: 40,
                      sections: _buildSections(),
                    ),
                  ),
                  _buildCenterInfo(),
                ],
              );
            },
          ),
        ),
        const SizedBox(width: 20),
        Expanded(
          flex: 2,
          child: _buildLegend(),
        ),
      ],
    );
  }

  Widget _buildVerticalLayout() {
    return Column(
      children: [
        Expanded(
          flex: 2,
          child: AnimatedBuilder(
            animation: _animation,
            builder: (context, child) {
              return Stack(
                alignment: Alignment.center,
                children: [
                  PieChart(
                    PieChartData(
                      pieTouchData: PieTouchData(
                        touchCallback: (FlTouchEvent event, pieTouchResponse) {
                          setState(() {
                            if (!event.isInterestedForInteractions ||
                                pieTouchResponse == null ||
                                pieTouchResponse.touchedSection == null) {
                              _touchedIndex = -1;
                              return;
                            }
                            _touchedIndex = pieTouchResponse
                                .touchedSection!.touchedSectionIndex;
                          });
                        },
                      ),
                      borderData: FlBorderData(show: false),
                      sectionsSpace: 2,
                      centerSpaceRadius: 30,
                      sections: _buildSections(isCompact: true),
                    ),
                  ),
                  _buildCenterInfo(isCompact: true),
                ],
              );
            },
          ),
        ),
        const SizedBox(height: 16),
        SizedBox(
          height: 100,
          child: _buildCompactLegend(),
        ),
      ],
    );
  }

  Widget _buildCenterInfo({bool isCompact = false}) {
    return Container(
      padding: EdgeInsets.all(isCompact ? 8 : 16),
      decoration: BoxDecoration(
        color: AppTheme.darkBackground.withValues(alpha: 0.8),
        shape: BoxShape.circle,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            'الإجمالي',
            style: AppTextStyles.caption.copyWith(
              color: AppTheme.textMuted,
              fontSize: isCompact ? 10 : null,
            ),
          ),
          SizedBox(height: isCompact ? 2 : 4),
          FittedBox(
            fit: BoxFit.scaleDown,
            child: Text(
              _calculateTotal(),
              style: AppTextStyles.bodyLarge.copyWith(
                color: AppTheme.textWhite,
                fontWeight: FontWeight.bold,
                fontSize: isCompact ? 14 : null,
              ),
            ),
          ),
        ],
      ),
    );
  }

  List<PieChartSectionData> _buildSections({bool isCompact = false}) {
    // Create dummy data if empty
    final data = widget.methodAnalytics.isEmpty
        ? _createDummyData()
        : widget.methodAnalytics;

    final sections = <PieChartSectionData>[];
    int index = 0;

    data.forEach((method, analytics) {
      final isTouched = index == _touchedIndex;
      final fontSize =
          isCompact ? (isTouched ? 12.0 : 10.0) : (isTouched ? 16.0 : 12.0);
      final radius =
          isCompact ? (isTouched ? 50.0 : 45.0) : (isTouched ? 70.0 : 60.0);

      sections.add(
        PieChartSectionData(
          color: _getMethodColor(index),
          value: analytics.percentage * _animation.value,
          title:
              '${(analytics.percentage * _animation.value).toStringAsFixed(1)}%',
          radius: radius,
          titleStyle: AppTextStyles.caption.copyWith(
            fontSize: fontSize,
            color: AppTheme.textWhite,
            fontWeight: FontWeight.bold,
          ),
          titlePositionPercentageOffset: 0.55,
        ),
      );
      index++;
    });

    return sections;
  }

  // إصلاح مشكلة overflow في _buildLegend
  Widget _buildLegend() {
    final data = widget.methodAnalytics.isEmpty
        ? _createDummyData()
        : widget.methodAnalytics;

    return SingleChildScrollView(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: data.entries.map((entry) {
          final index = data.keys.toList().indexOf(entry.key);
          return Container(
            margin: const EdgeInsets.only(bottom: 8),
            child: Row(
              children: [
                Container(
                  width: 10,
                  height: 10,
                  decoration: BoxDecoration(
                    color: _getMethodColor(index),
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
                const SizedBox(width: 6),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        _getMethodName(entry.key),
                        style: AppTextStyles.caption.copyWith(
                          color: AppTheme.textWhite,
                          fontSize: 11,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      Text(
                        '${entry.value.transactionCount} معاملة',
                        style: AppTextStyles.caption.copyWith(
                          color: AppTheme.textMuted,
                          fontSize: 9,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
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

  Widget _buildCompactLegend() {
    final data = widget.methodAnalytics.isEmpty
        ? _createDummyData()
        : widget.methodAnalytics;

    return ListView(
      scrollDirection: Axis.horizontal,
      children: data.entries.map((entry) {
        final index = data.keys.toList().indexOf(entry.key);
        return Container(
          margin: const EdgeInsets.only(right: 12),
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          decoration: BoxDecoration(
            color: AppTheme.darkCard.withValues(alpha: 0.5),
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: _getMethodColor(index).withValues(alpha: 0.3),
            ),
          ),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Container(
                    width: 8,
                    height: 8,
                    decoration: BoxDecoration(
                      color: _getMethodColor(index),
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                  const SizedBox(width: 6),
                  Text(
                    _getMethodName(entry.key),
                    style: AppTextStyles.caption.copyWith(
                      color: AppTheme.textWhite,
                      fontSize: 11,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 4),
              Text(
                '${entry.value.percentage.toStringAsFixed(1)}%',
                style: AppTextStyles.bodyMedium.copyWith(
                  color: _getMethodColor(index),
                  fontWeight: FontWeight.bold,
                  fontSize: 14,
                ),
              ),
              Text(
                '${entry.value.transactionCount} معاملة',
                style: AppTextStyles.caption.copyWith(
                  color: AppTheme.textMuted,
                  fontSize: 9,
                ),
              ),
            ],
          ),
        );
      }).toList(),
    );
  }

  Map<dynamic, MethodAnalytics> _createDummyData() {
    return {
      PaymentMethod.creditCard: const MethodAnalytics(
        method: PaymentMethod.creditCard,
        transactionCount: 145,
        totalAmount: Money(
          amount: 250000,
          currency: 'YER',
          formattedAmount: '250,000 YER',
        ),
        percentage: 35,
        successRate: 95,
        averageAmount: Money(
          amount: 1724,
          currency: 'YER',
          formattedAmount: '1,724 YER',
        ),
      ),
      PaymentMethod.jwaliWallet: const MethodAnalytics(
        method: PaymentMethod.jwaliWallet,
        transactionCount: 98,
        totalAmount: Money(
          amount: 180000,
          currency: 'YER',
          formattedAmount: '180,000 YER',
        ),
        percentage: 25,
        successRate: 98,
        averageAmount: Money(
          amount: 1837,
          currency: 'YER',
          formattedAmount: '1,837 YER',
        ),
      ),
      PaymentMethod.cashWallet: const MethodAnalytics(
        method: PaymentMethod.cashWallet,
        transactionCount: 87,
        totalAmount: Money(
          amount: 120000,
          currency: 'YER',
          formattedAmount: '120,000 YER',
        ),
        percentage: 20,
        successRate: 92,
        averageAmount: Money(
          amount: 1379,
          currency: 'YER',
          formattedAmount: '1,379 YER',
        ),
      ),
      PaymentMethod.cash: const MethodAnalytics(
        method: PaymentMethod.cash,
        transactionCount: 65,
        totalAmount: Money(
          amount: 95000,
          currency: 'YER',
          formattedAmount: '95,000 YER',
        ),
        percentage: 15,
        successRate: 100,
        averageAmount: Money(
          amount: 1462,
          currency: 'YER',
          formattedAmount: '1,462 YER',
        ),
      ),
      PaymentMethod.oneCashWallet: const MethodAnalytics(
        method: PaymentMethod.oneCashWallet,
        transactionCount: 25,
        totalAmount: Money(
          amount: 35000,
          currency: 'YER',
          formattedAmount: '35,000 YER',
        ),
        percentage: 5,
        successRate: 88,
        averageAmount: Money(
          amount: 1400,
          currency: 'YER',
          formattedAmount: '1,400 YER',
        ),
      ),
    };
  }

  Color _getMethodColor(int index) {
    final colors = [
      AppTheme.primaryBlue,
      AppTheme.primaryPurple,
      AppTheme.success,
      AppTheme.warning,
      AppTheme.primaryViolet,
      AppTheme.info,
      AppTheme.error,
    ];
    return colors[index % colors.length];
  }

  String _getMethodName(dynamic method) {
    if (method is PaymentMethod) {
      switch (method) {
        case PaymentMethod.creditCard:
          return 'بطاقة ائتمان';
        case PaymentMethod.jwaliWallet:
          return 'محفظة جوالي';
        case PaymentMethod.cash:
          return 'نقدي';
        case PaymentMethod.cashWallet:
          return 'كاش محفظة';
        case PaymentMethod.oneCashWallet:
          return 'ون كاش';
        case PaymentMethod.floskWallet:
          return 'فلوسك';
        case PaymentMethod.jaibWallet:
          return 'جيب';
        default:
          return 'أخرى';
      }
    }
    return method.toString();
  }

  String _calculateTotal() {
    final data = widget.methodAnalytics.isEmpty
        ? _createDummyData()
        : widget.methodAnalytics;

    final total = data.values.fold(
      0.0,
      (sum, analytics) => sum + analytics.totalAmount.amount,
    );

    if (total >= 1000000) {
      return '${(total / 1000000).toStringAsFixed(1)}M ر.ي';
    } else if (total >= 1000) {
      return '${(total / 1000).toStringAsFixed(1)}K ر.ي';
    }
    return '${total.toStringAsFixed(0)} ر.ي';
  }
}
