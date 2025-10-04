// lib/features/home/presentation/widgets/sections/base_section_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:yemen_booking_app/features/home/presentation/widgets/sections/horizontal_property_list_widget.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../../../core/enums/section_type_enum.dart';
import '../../../../../core/models/paginated_result.dart';
import '../../../../search/data/models/search_result_model.dart';
import '../../../domain/entities/section.dart';
import 'section_header_widget.dart';
import 'section_loading_widget.dart';
import 'section_empty_widget.dart';
import 'single_property_ad_widget.dart';
import 'multi_property_ad_widget.dart';
import 'single_property_offer_widget.dart';
import 'offers_carousel_widget.dart';
import 'flash_deals_widget.dart';
import 'horizontal_property_list_widget.dart';
import 'vertical_property_grid_widget.dart';
import 'city_cards_grid_widget.dart';
import 'premium_carousel_widget.dart';
import 'section_visibility_detector.dart';

class BaseSectionWidget extends StatefulWidget {
  final Section section;
  final PaginatedResult<SearchResultModel>? data;
  final bool isLoadingMore;
  final VoidCallback? onLoadMore;
  final Function(String)? onItemTap;
  final VoidCallback? onViewAll;

  const BaseSectionWidget({
    super.key,
    required this.section,
    this.data,
    this.isLoadingMore = false,
    this.onLoadMore,
    this.onItemTap,
    this.onViewAll,
  });

  @override
  State<BaseSectionWidget> createState() => _BaseSectionWidgetState();
}

class _BaseSectionWidgetState extends State<BaseSectionWidget>
    with TickerProviderStateMixin {
  late AnimationController _entranceController;
  late AnimationController _shimmerController;
  late AnimationController _glowController;
  
  late Animation<double> _fadeAnimation;
  late Animation<Offset> _slideAnimation;
  late Animation<double> _scaleAnimation;
  
  bool _isVisible = false;
  bool _hasAnimated = false;

  @override
  void initState() {
    super.initState();
    _initializeAnimations();
  }

  void _initializeAnimations() {
    _entranceController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );
    
    _shimmerController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat();
    
    _glowController = AnimationController(
      duration: const Duration(seconds: 3),
      vsync: this,
    )..repeat(reverse: true);
    
    _fadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _entranceController,
      curve: const Interval(0.0, 0.6, curve: Curves.easeOut),
    ));
    
    _slideAnimation = Tween<Offset>(
      begin: const Offset(0, 0.2),
      end: Offset.zero,
    ).animate(CurvedAnimation(
      parent: _entranceController,
      curve: const Interval(0.2, 1.0, curve: Curves.easeOutBack),
    ));
    
    _scaleAnimation = Tween<double>(
      begin: 0.95,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _entranceController,
      curve: const Interval(0.0, 0.8, curve: Curves.elasticOut),
    ));
  }

  @override
  void dispose() {
    _entranceController.dispose();
    _shimmerController.dispose();
    _glowController.dispose();
    super.dispose();
  }

  void _onVisibilityChanged(bool isVisible) {
    if (isVisible && !_hasAnimated) {
      setState(() {
        _isVisible = true;
        _hasAnimated = true;
      });
      _entranceController.forward();
    }
  }

  @override
  Widget build(BuildContext context) {
    // Don't render inactive sections
    if (!widget.section.isActive) {
      return const SizedBox.shrink();
    }

    return SectionVisibilityDetector(
      sectionId: widget.section.id,
      onVisibilityChanged: _onVisibilityChanged,
      child: AnimatedBuilder(
        animation: _entranceController,
        builder: (context, child) {
          return FadeTransition(
            opacity: _fadeAnimation,
            child: SlideTransition(
              position: _slideAnimation,
              child: Transform.scale(
                scale: _scaleAnimation.value,
                child: Container(
                  margin: const EdgeInsets.only(bottom: 24),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Section Header
                      if (_shouldShowHeader())
                        Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 20),
                          child: SectionHeaderWidget(
                            title: _getSectionTitle(),
                            subtitle: _getSectionSubtitle(),
                            icon: _getSectionIcon(),
                            gradientColors: _getSectionGradient(),
                            onViewAll: widget.onViewAll ?? () => _handleViewAll(),
                            isGlowing: _isSpecialSection(),
                          ),
                        ),
                      
                      const SizedBox(height: 16),
                      
                      // Section Content
                      _buildSectionContent(),
                      
                      // Load More Indicator
                      if (widget.isLoadingMore)
                        _buildLoadMoreIndicator(),
                    ],
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildSectionContent() {
    // Check for loading state
    if (widget.data == null) {
      return const SectionLoadingWidget();
    }
    
    // Check for empty state
    if (widget.data!.items.isEmpty) {
      return SectionEmptyWidget(
        message: _getEmptyMessage(),
        icon: _getSectionIcon(),
      );
    }
    
    // Build content based on section type
    return _buildContentByType();
  }

  Widget _buildContentByType() {
    switch (widget.section.type) {
      // Ad Sections
      case SectionType.singlePropertyAd:
        return SinglePropertyAdWidget(
          property: widget.data!.items.first,
          onTap: (id) => widget.onItemTap?.call(id),
        );
        
      case SectionType.multiPropertyAd:
        return MultiPropertyAdWidget(
          properties: widget.data!.items,
          onItemTap: widget.onItemTap,
        );
        
      // Offer Sections
      case SectionType.singlePropertyOffer:
        return SinglePropertyOfferWidget(
          property: widget.data!.items.first,
          onTap: () => widget.onItemTap?.call(widget.data!.items.first.id),
        );
        
      case SectionType.offersCarousel:
        return OffersCarouselWidget(
          offers: widget.data!.items,
          onItemTap: widget.onItemTap,
        );
        
      case SectionType.flashDeals:
        return FlashDealsWidget(
          deals: widget.data!.items,
          onItemTap: widget.onItemTap,
        );
        
      // Property Sections
      case SectionType.horizontalPropertyList:
        return HorizontalPropertyListWidget(
          properties: widget.data!.items,
          onItemTap: widget.onItemTap,
          onLoadMore: widget.onLoadMore,
        );
        
      case SectionType.verticalPropertyGrid:
        return VerticalPropertyGridWidget(
          properties: widget.data!.items,
          onItemTap: widget.onItemTap,
        );
        
      // Destination Sections
      case SectionType.cityCardsGrid:
        return CityCardsGridWidget(
          cities: widget.data!.items,
          onItemTap: widget.onItemTap,
        );
        
      // Premium Sections
      case SectionType.premiumCarousel:
        return PremiumCarouselWidget(
          properties: widget.data!.items,
          onItemTap: widget.onItemTap,
        );
        
      default:
        return HorizontalPropertyListWidget(
          properties: widget.data!.items,
          onItemTap: widget.onItemTap,
        );
    }
  }

  Widget _buildLoadMoreIndicator() {
    return Container(
      padding: const EdgeInsets.all(20),
      alignment: Alignment.center,
      child: AnimatedBuilder(
        animation: _shimmerController,
        builder: (context, child) {
          return Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              gradient: RadialGradient(
                colors: [
                  AppTheme.primaryBlue.withOpacity(0.3),
                  AppTheme.primaryBlue.withOpacity(0.1),
                ],
              ),
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: AppTheme.primaryBlue.withOpacity(
                    0.3 + (_shimmerController.value * 0.2),
                  ),
                  blurRadius: 20,
                  spreadRadius: 5,
                ),
              ],
            ),
            child: CircularProgressIndicator(
              valueColor: AlwaysStoppedAnimation<Color>(AppTheme.primaryBlue),
              strokeWidth: 2,
            ),
          );
        },
      ),
    );
  }

  bool _shouldShowHeader() {
    // Some sections might not need headers
    return true;
  }

  String _getSectionTitle() {
    // Return title based on section type
    switch (widget.section.type) {
      case SectionType.singlePropertyAd:
        return 'عرض مميز';
      case SectionType.multiPropertyAd:
        return 'إعلانات مميزة';
      case SectionType.singlePropertyOffer:
        return 'عرض خاص';
      case SectionType.offersCarousel:
        return 'عروض حصرية';
      case SectionType.flashDeals:
        return 'عروض سريعة';
      case SectionType.horizontalPropertyList:
        return 'عقارات مميزة';
      case SectionType.verticalPropertyGrid:
        return 'استكشف العقارات';
      case SectionType.cityCardsGrid:
        return 'وجهات رائجة';
      case SectionType.premiumCarousel:
        return 'الباقة المميزة';
      default:
        return 'استكشف';
    }
  }

  String? _getSectionSubtitle() {
    switch (widget.section.type) {
      case SectionType.flashDeals:
        return 'عروض محدودة المدة';
      case SectionType.premiumCarousel:
        return 'أفخم العقارات';
      default:
        return null;
    }
  }

  IconData _getSectionIcon() {
    switch (widget.section.type) {
      case SectionType.singlePropertyAd:
      case SectionType.multiPropertyAd:
        return Icons.campaign;
      case SectionType.singlePropertyOffer:
      case SectionType.offersCarousel:
      case SectionType.flashDeals:
        return Icons.local_offer;
      case SectionType.horizontalPropertyList:
      case SectionType.verticalPropertyGrid:
        return Icons.home_work;
      case SectionType.cityCardsGrid:
        return Icons.explore;
      case SectionType.premiumCarousel:
        return Icons.workspace_premium;
      default:
        return Icons.widgets;
    }
  }

  List<Color> _getSectionGradient() {
    switch (widget.section.type) {
      case SectionType.flashDeals:
        return [AppTheme.error, AppTheme.warning];
      case SectionType.premiumCarousel:
        return [AppTheme.warning, const Color(0xFFFFD700)];
      case SectionType.offersCarousel:
        return [AppTheme.success, AppTheme.primaryCyan];
      default:
        return [AppTheme.primaryBlue, AppTheme.primaryPurple];
    }
  }

  bool _isSpecialSection() {
    return widget.section.type == SectionType.flashDeals ||
           widget.section.type == SectionType.premiumCarousel;
  }

  String _getEmptyMessage() {
    switch (widget.section.type) {
      case SectionType.flashDeals:
        return 'لا توجد عروض سريعة حالياً';
      case SectionType.offersCarousel:
        return 'لا توجد عروض متاحة';
      default:
        return 'لا توجد عناصر للعرض';
    }
  }

  void _handleViewAll() {
    HapticFeedback.lightImpact();
    // Navigate to section details or filtered results
  }
}