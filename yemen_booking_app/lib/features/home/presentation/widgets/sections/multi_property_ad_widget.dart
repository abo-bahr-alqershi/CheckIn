// lib/features/home/presentation/widgets/sections/ads/multi_property_ad_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'dart:math' as math;
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../search/data/models/search_result_model.dart';

class MultiPropertyAdWidget extends StatefulWidget {
  final List<SearchResultModel> properties;
  final Function(String)? onItemTap;

  const MultiPropertyAdWidget({
    super.key,
    required this.properties,
    this.onItemTap,
  });

  @override
  State<MultiPropertyAdWidget> createState() => _MultiPropertyAdWidgetState();
}

class _MultiPropertyAdWidgetState extends State<MultiPropertyAdWidget>
    with TickerProviderStateMixin {
  late PageController _pageController;
  late AnimationController _autoScrollController;
  late AnimationController _indicatorController;
  
  int _currentIndex = 0;

  @override
  void initState() {
    super.initState();
    _pageController = PageController(viewportFraction: 0.85);
    
    _autoScrollController = AnimationController(
      duration: const Duration(seconds: 5),
      vsync: this,
    )..repeat();
    
    _indicatorController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );
    
    _startAutoScroll();
  }

  void _startAutoScroll() {
    _autoScrollController.addStatusListener((status) {
      if (status == AnimationStatus.completed) {
        if (_pageController.hasClients) {
          final nextIndex = (_currentIndex + 1) % widget.properties.length;
          _pageController.animateToPage(
            nextIndex,
            duration: const Duration(milliseconds: 800),
            curve: Curves.easeInOut,
          );
        }
      }
    });
  }

  @override
  void dispose() {
    _pageController.dispose();
    _autoScrollController.dispose();
    _indicatorController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 400,
      margin: const EdgeInsets.symmetric(vertical: 16),
      child: Stack(
        children: [
          // Main carousel
          PageView.builder(
            controller: _pageController,
            onPageChanged: (index) {
              setState(() {
                _currentIndex = index;
              });
            },
            itemCount: widget.properties.length,
            itemBuilder: (context, index) {
              final property = widget.properties[index];
              return _buildAdCard(property, index);
            },
          ),
          
          // Page indicators
          Positioned(
            bottom: 20,
            left: 0,
            right: 0,
            child: _buildIndicators(),
          ),
          
          // "AD" Badge
          Positioned(
            top: 20,
            left: 20,
            child: _buildAdBadge(),
          ),
        ],
      ),
    );
  }

  Widget _buildAdCard(SearchResultModel property, int index) {
    final isActive = index == _currentIndex;
    
    return AnimatedScale(
      scale: isActive ? 1.0 : 0.9,
      duration: const Duration(milliseconds: 300),
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 8),
        child: GestureDetector(
          onTap: () {
            HapticFeedback.lightImpact();
            widget.onItemTap?.call(property.id);
          },
          child: Stack(
            children: [
              // Main container
              Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(24),
                  boxShadow: [
                    BoxShadow(
                      color: AppTheme.primaryPurple.withOpacity(0.3),
                      blurRadius: 30,
                      spreadRadius: 5,
                      offset: const Offset(0, 10),
                    ),
                  ],
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(24),
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      // Background image
                      CachedImageWidget(
                        imageUrl: property.imageUrl ?? '',
                        fit: BoxFit.cover,
                      ),
                      
                      // Gradient overlay
                      Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.topCenter,
                            end: Alignment.bottomCenter,
                            colors: [
                              Colors.transparent,
                              Colors.black.withOpacity(0.8),
                            ],
                            stops: const [0.4, 1.0],
                          ),
                        ),
                      ),
                      
                      // Content
                      Positioned(
                        bottom: 24,
                        left: 24,
                        right: 24,
                        child: _buildCardContent(property),
                      ),
                    ],
                  ),
                ),
              ),
              
              // Premium border effect
              if (isActive)
                Positioned.fill(
                  child: IgnorePointer(
                    child: Container(
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(24),
                        border: Border.all(
                          width: 2,
                          color: AppTheme.primaryPurple.withOpacity(0.5),
                        ),
                      ),
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildCardContent(SearchResultModel property) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Title
        Text(
          property.name,
          style: AppTextStyles.heading1.copyWith(
            color: Colors.white,
            fontWeight: FontWeight.bold,
          ),
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
        ),
        
        const SizedBox(height: 12),
        
        // Location
        Row(
          children: [
            Icon(
              Icons.location_on,
              size: 16,
              color: AppTheme.primaryCyan,
            ),
            const SizedBox(width: 4),
            Expanded(
              child: Text(
                property.location ?? '',
                style: AppTextStyles.bodySmall.copyWith(
                  color: Colors.white.withOpacity(0.9),
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
          ],
        ),
        
        const SizedBox(height: 16),
        
        // Price and features
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            // Price
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              decoration: BoxDecoration(
                gradient: AppTheme.primaryGradient,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    '${property.price?.toStringAsFixed(0) ?? '0'}',
                    style: AppTextStyles.heading3.copyWith(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(width: 4),
                  Text(
                    'ريال/ليلة',
                    style: AppTextStyles.caption.copyWith(
                      color: Colors.white.withOpacity(0.9),
                    ),
                  ),
                ],
              ),
            ),
            
            // Features
            Row(
              children: [
                _buildFeature(Icons.king_bed, '${property.bedrooms ?? 0}'),
                const SizedBox(width: 12),
                _buildFeature(Icons.bathtub, '${property.bathrooms ?? 0}'),
              ],
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildFeature(IconData icon, String value) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: AppTheme.darkCard.withOpacity(0.6),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: AppTheme.primaryCyan.withOpacity(0.3),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            icon,
            size: 14,
            color: AppTheme.primaryCyan,
          ),
          const SizedBox(width: 4),
          Text(
            value,
            style: AppTextStyles.caption.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildIndicators() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(widget.properties.length, (index) {
        final isActive = index == _currentIndex;
        
        return AnimatedContainer(
          duration: const Duration(milliseconds: 300),
          margin: const EdgeInsets.symmetric(horizontal: 4),
          width: isActive ? 32 : 8,
          height: 8,
          decoration: BoxDecoration(
            gradient: isActive ? AppTheme.primaryGradient : null,
            color: !isActive ? AppTheme.darkBorder.withOpacity(0.5) : null,
            borderRadius: BorderRadius.circular(4),
            boxShadow: isActive
                ? [
                    BoxShadow(
                      color: AppTheme.primaryPurple.withOpacity(0.5),
                      blurRadius: 8,
                      spreadRadius: 2,
                    ),
                  ]
                : [],
          ),
        );
      }),
    );
  }

  Widget _buildAdBadge() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.warning,
            AppTheme.warning.withOpacity(0.8),
          ],
        ),
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: AppTheme.warning.withOpacity(0.5),
            blurRadius: 10,
            spreadRadius: 2,
          ),
        ],
      ),
      child: Text(
        'إعلان',
        style: AppTextStyles.caption.copyWith(
          color: Colors.white,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }
}