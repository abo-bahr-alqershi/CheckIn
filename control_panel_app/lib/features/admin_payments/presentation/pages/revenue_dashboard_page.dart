import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';
import 'dart:ui';
import '../widgets/revenue_chart_widget.dart';
import '../widgets/payment_stats_cards.dart';
import '../widgets/payment_breakdown_pie_chart.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../domain/entities/payment_analytics.dart';

class RevenueDashboardPage extends StatefulWidget {
  const RevenueDashboardPage({super.key});

  @override
  State<RevenueDashboardPage> createState() => _RevenueDashboardPageState();
}

class _RevenueDashboardPageState extends State<RevenueDashboardPage>
    with TickerProviderStateMixin {
  late AnimationController _animationController;
  late AnimationController _countAnimationController;
  late Animation<double> _fadeAnimation;
  late Animation<double> _slideAnimation;
  late Animation<double> _scaleAnimation;

  final PageController _pageController = PageController();
  int _currentPage = 0;

  // Mock data - Replace with actual data from BLoC
  final Map<String, dynamic> _mockStats = {
    'totalRevenue': 2500000.0,
    'monthlyRevenue': 450000.0,
    'weeklyRevenue': 120000.0,
    'dailyRevenue': 18000.0,
    'growthRate': 12.5,
    'transactionCount': 342,
    'averageTransaction': 7300.0,
    'pendingAmount': 85000.0,
  };

  @override
  void initState() {
    super.initState();
    _setupAnimations();
  }

  void _setupAnimations() {
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 1200),
      vsync: this,
    );

    _countAnimationController = AnimationController(
      duration: const Duration(milliseconds: 2000),
      vsync: this,
    );

    _fadeAnimation = CurvedAnimation(
      parent: _animationController,
      curve: const Interval(0.0, 0.5, curve: Curves.easeIn),
    );

    _slideAnimation = Tween<double>(
      begin: 100,
      end: 0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: const Interval(0.2, 0.7, curve: Curves.easeOutCubic),
    ));

    _scaleAnimation = Tween<double>(
      begin: 0.8,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: const Interval(0.3, 0.8, curve: Curves.elasticOut),
    ));

    _animationController.forward();
    _countAnimationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    _countAnimationController.dispose();
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.darkBackground,
      body: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [
              AppTheme.darkBackground,
              AppTheme.darkBackground2,
              AppTheme.darkBackground3,
            ],
          ),
        ),
        child: SafeArea(
          child: CustomScrollView(
            physics: const BouncingScrollPhysics(),
            slivers: [
              // Header
              SliverToBoxAdapter(
                child: _buildHeader(),
              ),

              // Main Revenue Card
              SliverToBoxAdapter(
                child: FadeTransition(
                  opacity: _fadeAnimation,
                  child: AnimatedBuilder(
                    animation: _scaleAnimation,
                    builder: (context, child) {
                      return Transform.scale(
                        scale: _scaleAnimation.value,
                        child: _buildMainRevenueCard(),
                      );
                    },
                  ),
                ),
              ),

              // Revenue Breakdown
              SliverToBoxAdapter(
                child: AnimatedBuilder(
                  animation: _slideAnimation,
                  builder: (context, child) {
                    return Transform.translate(
                      offset: Offset(0, _slideAnimation.value),
                      child: FadeTransition(
                        opacity: _fadeAnimation,
                        child: _buildRevenueBreakdown(),
                      ),
                    );
                  },
                ),
              ),

              // Charts Section
              SliverToBoxAdapter(
                child: FadeTransition(
                  opacity: _fadeAnimation,
                  child: _buildChartsSection(),
                ),
              ),

              // Quick Stats
              SliverToBoxAdapter(
                child: FadeTransition(
                  opacity: _fadeAnimation,
                  child: _buildQuickStats(),
                ),
              ),

              const SliverToBoxAdapter(
                child: SizedBox(height: 100),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingSmall),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              IconButton(
                onPressed: () => Navigator.pop(context),
                icon: Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: AppTheme.darkCard.withValues(alpha: 0.5),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Icon(
                    CupertinoIcons.arrow_right,
                    color: AppTheme.textWhite,
                    size: 20,
                  ),
                ),
              ),
              const Spacer(),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 6,
                ),
                decoration: BoxDecoration(
                  gradient: AppTheme.primaryGradient,
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Row(
                  children: [
                    Icon(
                      CupertinoIcons.calendar,
                      color: AppTheme.textWhite,
                      size: 16,
                    ),
                    const SizedBox(width: 6),
                    Text(
                      'هذا الشهر',
                      style: AppTextStyles.caption.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          ShaderMask(
            shaderCallback: (bounds) =>
                AppTheme.primaryGradient.createShader(bounds),
            child: Text(
              'لوحة الإيرادات',
              style: AppTextStyles.displaySmall.copyWith(
                color: AppTheme.textWhite,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'نظرة شاملة على الأداء المالي',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppTheme.textMuted,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildMainRevenueCard() {
    return Container(
      margin: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingSmall,
      ),
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        gradient: AppTheme.primaryGradient,
        borderRadius: BorderRadius.circular(24),
        boxShadow: [
          BoxShadow(
            color: AppTheme.primaryBlue.withValues(alpha: 0.3),
            blurRadius: 20,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppTheme.textWhite.withValues(alpha: 0.2),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(
                  CupertinoIcons.money_dollar_circle_fill,
                  color: Colors.white,
                  size: 28,
                ),
              ),
              const Spacer(),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 6,
                ),
                decoration: BoxDecoration(
                  color: _mockStats['growthRate'] > 0
                      ? AppTheme.success.withValues(alpha: 0.2)
                      : AppTheme.error.withValues(alpha: 0.2),
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Row(
                  children: [
                    Icon(
                      _mockStats['growthRate'] > 0
                          ? CupertinoIcons.arrow_up_right
                          : CupertinoIcons.arrow_down_right,
                      color: _mockStats['growthRate'] > 0
                          ? AppTheme.success
                          : AppTheme.error,
                      size: 16,
                    ),
                    const SizedBox(width: 4),
                    Text(
                      '${_mockStats['growthRate'].toStringAsFixed(1)}%',
                      style: AppTextStyles.caption.copyWith(
                        color: _mockStats['growthRate'] > 0
                            ? AppTheme.success
                            : AppTheme.error,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 20),
          Text(
            'إجمالي الإيرادات',
            style: AppTextStyles.bodyMedium.copyWith(
              color: Colors.white.withValues(alpha: 0.8),
            ),
          ),
          const SizedBox(height: 8),
          TweenAnimationBuilder<double>(
            tween: Tween(begin: 0, end: _mockStats['totalRevenue']),
            duration: const Duration(milliseconds: 1500),
            builder: (context, value, child) {
              return Text(
                '${value.toStringAsFixed(0)} ر.ي',
                style: AppTextStyles.displayMedium.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              );
            },
          ),
          const SizedBox(height: 24),
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: AppTheme.textWhite.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(16),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                _buildMiniStat(
                  'اليوم',
                  _mockStats['dailyRevenue'],
                  CupertinoIcons.sun_max_fill,
                ),
                _buildVerticalDivider(),
                _buildMiniStat(
                  'هذا الأسبوع',
                  _mockStats['weeklyRevenue'],
                  CupertinoIcons.calendar_badge_plus,
                ),
                _buildVerticalDivider(),
                _buildMiniStat(
                  'هذا الشهر',
                  _mockStats['monthlyRevenue'],
                  CupertinoIcons.calendar_circle_fill,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildMiniStat(String label, double value, IconData icon) {
    return Column(
      children: [
        Icon(
          icon,
          color: AppTheme.textWhite.withValues(alpha: 0.6),
          size: 20,
        ),
        const SizedBox(height: 8),
        Text(
          label,
          style: AppTextStyles.caption.copyWith(
            color: AppTheme.textWhite.withValues(alpha: 0.6),
          ),
        ),
        const SizedBox(height: 4),
        Text(
          '${(value / 1000).toStringAsFixed(0)}K',
          style: AppTextStyles.bodyLarge.copyWith(
            color: AppTheme.textWhite,
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildVerticalDivider() {
    return Container(
      height: 50,
      width: 1,
      color: AppTheme.textWhite.withValues(alpha: 0.2),
    );
  }

  Widget _buildRevenueBreakdown() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'تفصيل الإيرادات',
            style: AppTextStyles.heading2.copyWith(
              color: AppTheme.textWhite,
            ),
          ),
          const SizedBox(height: 16),
          SizedBox(
            height: 120,
            child: ListView(
              scrollDirection: Axis.horizontal,
              children: [
                _buildBreakdownCard(
                  'الحجوزات',
                  1850000,
                  74,
                  CupertinoIcons.bed_double_fill,
                  AppTheme.primaryBlue,
                ),
                const SizedBox(width: 12),
                _buildBreakdownCard(
                  'الخدمات',
                  450000,
                  18,
                  CupertinoIcons.wrench_fill,
                  AppTheme.primaryPurple,
                ),
                const SizedBox(width: 12),
                _buildBreakdownCard(
                  'أخرى',
                  200000,
                  8,
                  CupertinoIcons.ellipsis_circle_fill,
                  AppTheme.primaryViolet,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBreakdownCard(
    String title,
    double amount,
    int percentage,
    IconData icon,
    Color color,
  ) {
    return Container(
      width: 160,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppTheme.darkCard,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: color.withValues(alpha: 0.3),
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: color.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Icon(
                  icon,
                  color: color,
                  size: 16,
                ),
              ),
              const Spacer(),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 8,
                  vertical: 4,
                ),
                decoration: BoxDecoration(
                  color: color.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Text(
                  '$percentage%',
                  style: AppTextStyles.caption.copyWith(
                    color: color,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),
          const Spacer(),
          Text(
            title,
            style: AppTextStyles.caption.copyWith(
              color: AppTheme.textMuted,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            '${(amount / 1000).toStringAsFixed(0)}K ر.ي',
            style: AppTextStyles.bodyLarge.copyWith(
              color: AppTheme.textWhite,
              fontWeight: FontWeight.bold,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildChartsSection() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'الرسوم البيانية',
                style: AppTextStyles.heading2.copyWith(
                  color: AppTheme.textWhite,
                ),
              ),
              Row(
                children: [
                  _buildChartToggle(0, CupertinoIcons.chart_bar),
                  const SizedBox(width: 8),
                  _buildChartToggle(1, CupertinoIcons.chart_pie_fill),
                ],
              ),
            ],
          ),
          const SizedBox(height: 16),
          SizedBox(
            height: 300,
            child: PageView(
              controller: _pageController,
              onPageChanged: (index) {
                setState(() {
                  _currentPage = index;
                });
              },
              children: [
                RevenueChartWidget(
                  data: _generateMockTrends(),
                  height: 300,
                ),
                PaymentBreakdownPieChart(
                  methodAnalytics: _generateMockMethodAnalytics(),
                  height: 300,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildChartToggle(int index, IconData icon) {
    final isSelected = _currentPage == index;
    return GestureDetector(
      onTap: () {
        _pageController.animateToPage(
          index,
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeInOut,
        );
      },
      child: Container(
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: isSelected ? AppTheme.primaryBlue : AppTheme.darkCard,
          borderRadius: BorderRadius.circular(8),
        ),
        child: Icon(
          icon,
          color: AppTheme.textWhite,
          size: 20,
        ),
      ),
    );
  }

  Widget _buildQuickStats() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'إحصائيات سريعة',
            style: AppTextStyles.heading2.copyWith(
              color: AppTheme.textWhite,
            ),
          ),
          const SizedBox(height: 16),
          LayoutBuilder(
            builder: (context, constraints) {
              // حساب العرض المتاح وتحديد عدد الأعمدة
              final crossAxisCount = constraints.maxWidth > 600 ? 4 : 2;
              // تحديد aspect ratio بناءً على حجم الشاشة
              final aspectRatio = constraints.maxWidth > 600
                  ? 1.8
                  : constraints.maxWidth > 400
                      ? 1.6
                      : 1.3;

              return GridView.count(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                crossAxisCount: crossAxisCount,
                mainAxisSpacing: 12,
                crossAxisSpacing: 12,
                childAspectRatio: aspectRatio,
                children: [
                  _buildStatCard(
                    'المعاملات',
                    _mockStats['transactionCount'].toString(),
                    CupertinoIcons.arrow_right_arrow_left,
                    AppTheme.primaryBlue,
                    isCompact: constraints.maxWidth < 400,
                  ),
                  _buildStatCard(
                    'متوسط المعاملة',
                    '${(_mockStats['averageTransaction'] / 1000).toStringAsFixed(1)}K',
                    CupertinoIcons.chart_bar_fill,
                    AppTheme.primaryPurple,
                    isCompact: constraints.maxWidth < 400,
                  ),
                  _buildStatCard(
                    'قيد الانتظار',
                    '${(_mockStats['pendingAmount'] / 1000).toStringAsFixed(0)}K',
                    CupertinoIcons.clock_fill,
                    AppTheme.warning,
                    isCompact: constraints.maxWidth < 400,
                  ),
                  _buildStatCard(
                    'معدل النجاح',
                    '94.5%',
                    CupertinoIcons.checkmark_seal_fill,
                    AppTheme.success,
                    isCompact: constraints.maxWidth < 400,
                  ),
                ],
              );
            },
          ),
        ],
      ),
    );
  }

  // إصلاح مشكلة overflow في _buildStatCard
  Widget _buildStatCard(
    String title,
    String value,
    IconData icon,
    Color color, {
    bool isCompact = false,
  }) {
    return Container(
      padding: EdgeInsets.all(isCompact ? 12 : 16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            color.withValues(alpha: 0.1),
            color.withValues(alpha: 0.05),
          ],
        ),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: color.withValues(alpha: 0.2),
          width: 1,
        ),
      ),
      child: LayoutBuilder(
        builder: (context, constraints) {
          return Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Row(
                children: [
                  Container(
                    padding: EdgeInsets.all(isCompact ? 6 : 8),
                    decoration: BoxDecoration(
                      color: color.withValues(alpha: 0.2),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Icon(
                      icon,
                      color: color,
                      size: isCompact ? 16 : 20,
                    ),
                  ),
                  const Spacer(),
                ],
              ),
              const Spacer(),
              FittedBox(
                fit: BoxFit.scaleDown,
                alignment: Alignment.centerLeft,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      title,
                      style: AppTextStyles.caption.copyWith(
                        color: AppTheme.textMuted,
                        fontSize: isCompact ? 10 : null,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 2),
                    Text(
                      value,
                      style: AppTextStyles.heading2.copyWith(
                        color: AppTheme.textWhite,
                        fontWeight: FontWeight.bold,
                        fontSize: isCompact ? 18 : null,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ),
              ),
            ],
          );
        },
      ),
    );
  }

  List<PaymentTrend> _generateMockTrends() {
    // Generate mock data for chart
    return [];
  }

  Map<dynamic, MethodAnalytics> _generateMockMethodAnalytics() {
    // Generate mock data for pie chart
    return {};
  }
}
