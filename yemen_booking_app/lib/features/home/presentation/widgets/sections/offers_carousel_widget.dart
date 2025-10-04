// lib/features/home/presentation/widgets/sections/offers/offers_carousel_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'dart:math' as math;
import 'dart:async';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../search/data/models/search_result_model.dart';

class OffersCarouselWidget extends StatefulWidget {
  final List<SearchResultModel> offers;
  final Function(String)? onItemTap;

  const OffersCarouselWidget({
    super.key,
    required this.offers,
    this.onItemTap,
  });

  @override
  State<OffersCarouselWidget> createState() => _OffersCarouselWidgetState();
}

class _OffersCarouselWidgetState extends State<OffersCarouselWidget>
    with TickerProviderStateMixin {
  late PageController _pageController;
  late AnimationController _rotationController;
  late AnimationController _pulseController;
  late AnimationController _countdownController;
  
  Timer? _autoScrollTimer;
  int _currentIndex = 0;
  
  // Countdown timer
  Duration _remainingTime = const Duration(hours: 23, minutes: 59, seconds: 59);
  Timer? _countdownTimer;

  @override
  void initState() {
    super.initState();
    _initializeControllers();
    _startCountdown();
    _startAutoScroll();
  }

  void _initializeControllers() {
    _pageController = PageController(viewportFraction: 0.9);
    
    _rotationController = AnimationController(
      duration: const Duration(seconds: 20),
      vsync: this,
    )..repeat();
    
    _pulseController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat(reverse: true);
    
    _countdownController = AnimationController(
      duration: const Duration(seconds: 1),
      vsync: this,
    )..repeat();
  }

  void _startCountdown() {
    _countdownTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (mounted) {
        setState(() {
          if (_remainingTime.inSeconds > 0) {
            _remainingTime = Duration(seconds: _remainingTime.inSeconds - 1);
          } else {
            _remainingTime = const Duration(hours: 23, minutes: 59, seconds: 59);
          }
        });
      }
    });
  }

  void _startAutoScroll() {
    _autoScrollTimer = Timer.periodic(const Duration(seconds: 4), (timer) {
      if (_pageController.hasClients) {
        final nextIndex = (_currentIndex + 1) % widget.offers.length;
        _pageController.animateToPage(
          nextIndex,
          duration: const Duration(milliseconds: 800),
          curve: Curves.easeInOutCubic,
        );
      }
    });
  }

  @override
  void dispose() {
    _autoScrollTimer?.cancel();
    _countdownTimer?.cancel();
    _pageController.dispose();
    _rotationController.dispose();
    _pulseController.dispose();
    _countdownController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 320,
      child: Column(
        children: [
          // Countdown timer header
          _buildCountdownHeader(),
          
          const SizedBox(height: 16),
          
          // Offers carousel
          Expanded(
            child: PageView.builder(
              controller: _pageController,
              onPageChanged: (index) {
                setState(() {
                  _currentIndex = index;
                });
              },
              itemCount: widget.offers.length,
              itemBuilder: (context, index) {
                return _buildOfferCard(widget.offers[index], index);
              },
            ),
          ),
          
          const SizedBox(height: 16),
          
          // Page indicators
          _buildPageIndicators(),
        ],
      ),
    );
  }

  Widget _buildCountdownHeader() {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 20),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.error.withOpacity(0.2),
            AppTheme.warning.withOpacity(0.1),
          ],
        ),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(
          color: AppTheme.error.withOpacity(0.3),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            children: [
              AnimatedBuilder(
                animation: _pulseController,
                builder: (context, child) {
                  return Container(
                    padding: const EdgeInsets.all(8),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [
                          AppTheme.error.withOpacity(0.8),
                          AppTheme.warning.withOpacity(0.8),
                        ],
                      ),
                      shape: BoxShape.circle,
                      boxShadow: [
                        BoxShadow(
                          color: AppTheme.error.withOpacity(
                            0.4 + (_pulseController.value * 0.2),
                          ),
                          blurRadius: 10 + (_pulseController.value * 5),
                          spreadRadius: 2,
                        ),
                      ],
                    ),
                    child: const Icon(
                      Icons.local_fire_department,
                      color: Colors.white,
                      size: 20,
                    ),
                  );
                },
              ),
              const SizedBox(width: 12),
              Text(
                'عروض محدودة',
                style: AppTextStyles.bodyLarge.copyWith(
                  color: AppTheme.error,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          
          _buildCountdownTimer(),
        ],
      ),
    );
  }

  Widget _buildCountdownTimer() {
    final hours = _remainingTime.inHours;
    final minutes = _remainingTime.inMinutes % 60;
    final seconds = _remainingTime.inSeconds % 60;
    
    return Row(
      children: [
        _buildTimeUnit(hours.toString().padLeft(2, '0'), 'ساعة'),
        _buildTimeSeparator(),
        _buildTimeUnit(minutes.toString().padLeft(2, '0'), 'دقيقة'),
        _buildTimeSeparator(),
        _buildTimeUnit(seconds.toString().padLeft(2, '0'), 'ثانية'),
      ],
    );
  }

  Widget _buildTimeUnit(String value, String label) {
    return Column(
      children: [
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
          decoration: BoxDecoration(
            gradient: AppTheme.primaryGradient,
            borderRadius: BorderRadius.circular(8),
          ),
          child: Text(
            value,
            style: AppTextStyles.bodyLarge.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        const SizedBox(height: 2),
        Text(
          label,
          style: AppTextStyles.caption.copyWith(
            color: AppTheme.textMuted,
            fontSize: 9,
          ),
        ),
      ],
    );
  }

  Widget _buildTimeSeparator() {
    return AnimatedBuilder(
      animation: _countdownController,
      builder: (context, child) {
        return Padding(
          padding: const EdgeInsets.symmetric(horizontal: 4),
          child: Opacity(
            opacity: _countdownController.value > 0.5 ? 1.0 : 0.3,
            child: Text(
              ':',
              style: AppTextStyles.heading2.copyWith(
                color: AppTheme.primaryBlue,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildOfferCard(SearchResultModel offer, int index) {
    final isActive = index == _currentIndex;
    final discount = offer.discount ?? 20;
    
    return AnimatedScale(
      scale: isActive ? 1.0 : 0.95,
      duration: const Duration(milliseconds: 300),
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 8),
        child: GestureDetector(
          onTap: () {
            HapticFeedback.lightImpact();
            widget.onItemTap?.call(offer.id);
          },
          child: Stack(
            children: [
              // Main card
              Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(24),
                  gradient: LinearGradient(
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                    colors: [
                      AppTheme.darkCard.withOpacity(0.9),
                      AppTheme.darkCard.withOpacity(0.7),
                    ],
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: AppTheme.success.withOpacity(0.3),
                      blurRadius: 30,
                      spreadRadius: 5,
                      offset: const Offset(0, 10),
                    ),
                  ],
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(24),
                  child: Stack(
                    children: [
                      // Background image
                      Positioned.fill(
                        child: CachedImageWidget(
                          imageUrl: offer.imageUrl ?? '',
                          fit: BoxFit.cover,
                        ),
                      ),
                      
                      // Gradient overlay
                      Positioned.fill(
                        child: Container(
                          decoration: BoxDecoration(
                            gradient: LinearGradient(
                              begin: Alignment.topCenter,
                              end: Alignment.bottomCenter,
                              colors: [
                                Colors.transparent,
                                Colors.black.withOpacity(0.8),
                              ],
                              stops: const [0.3, 1.0],
                            ),
                          ),
                        ),
                      ),
                      
                      // Content
                      Positioned.fill(
                        child: Padding(
                          padding: const EdgeInsets.all(20),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              // Discount badge
                              _buildDiscountBadge(discount),
                              
                              const Spacer(),
                              
                              // Property info
                              _buildOfferInfo(offer),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              
              // Animated border
              if (isActive)
                AnimatedBuilder(
                  animation: _rotationController,
                  builder: (context, child) {
                    return CustomPaint(
                      painter: _RotatingBorderPainter(
                        rotation: _rotationController.value,
                        color: AppTheme.success,
                      ),
                      child: Container(),
                    );
                  },
                ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildDiscountBadge(int discount) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.error,
            AppTheme.warning,
          ],
        ),
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: AppTheme.error.withOpacity(0.5),
            blurRadius: 15,
            spreadRadius: 2,
          ),
        ],
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(
            Icons.discount,
            color: Colors.white,
            size: 18,
          ),
          const SizedBox(width: 6),
          Text(
            'خصم $discount%',
            style: AppTextStyles.bodyMedium.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildOfferInfo(SearchResultModel offer) {
    final originalPrice = offer.price ?? 0;
    final discountedPrice = originalPrice * (1 - (offer.discount ?? 20) / 100);
    
    return ClipRRect(
      borderRadius: BorderRadius.circular(16),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
        child: Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                AppTheme.darkCard.withOpacity(0.8),
                AppTheme.darkCard.withOpacity(0.6),
              ],
            ),
            borderRadius: BorderRadius.circular(16),
            border: Border.all(
              color: AppTheme.success.withOpacity(0.3),
              width: 1,
            ),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                offer.name,
                style: AppTextStyles.heading2.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              
              const SizedBox(height: 8),
              
              Row(
                children: [
                  Icon(
                    Icons.location_on,
                    size: 14,
                    color: AppTheme.success,
                  ),
                  const SizedBox(width: 4),
                  Expanded(
                    child: Text(
                      offer.location ?? '',
                      style: AppTextStyles.caption.copyWith(
                        color: AppTheme.textLight,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ],
              ),
              
              const SizedBox(height: 12),
              
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Original price
                      Text(
                        '${originalPrice.toStringAsFixed(0)} ريال',
                        style: AppTextStyles.bodySmall.copyWith(
                          color: AppTheme.textMuted,
                          decoration: TextDecoration.lineThrough,
                        ),
                      ),
                      // Discounted price
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.baseline,
                        textBaseline: TextBaseline.alphabetic,
                        children: [
                          ShaderMask(
                            shaderCallback: (bounds) =>
                                LinearGradient(
                                  colors: [AppTheme.success, AppTheme.primaryCyan],
                                ).createShader(bounds),
                            child: Text(
                              discountedPrice.toStringAsFixed(0),
                              style: AppTextStyles.heading1.copyWith(
                                color: Colors.white,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                          const SizedBox(width: 4),
                          Text(
                            'ريال/ليلة',
                            style: AppTextStyles.caption.copyWith(
                              color: AppTheme.textMuted,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                  
                  Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 16,
                      vertical: 8,
                    ),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [AppTheme.success, AppTheme.primaryCyan],
                      ),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      'احجز الآن',
                      style: AppTextStyles.buttonSmall.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildPageIndicators() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(widget.offers.length, (index) {
        final isActive = index == _currentIndex;
        
        return AnimatedContainer(
          duration: const Duration(milliseconds: 300),
          margin: const EdgeInsets.symmetric(horizontal: 4),
          width: isActive ? 24 : 8,
          height: 8,
          decoration: BoxDecoration(
            gradient: isActive
                ? LinearGradient(colors: [AppTheme.success, AppTheme.primaryCyan])
                : null,
            color: !isActive ? AppTheme.darkBorder.withOpacity(0.3) : null,
            borderRadius: BorderRadius.circular(4),
          ),
        );
      }),
    );
  }
}

// Rotating border painter
class _RotatingBorderPainter extends CustomPainter {
  final double rotation;
  final Color color;
  
  _RotatingBorderPainter({
    required this.rotation,
    required this.color,
  });
  
  @override
  void paint(Canvas canvas, Size size) {
    final rect = Rect.fromLTWH(8, 0, size.width - 16, size.height);
    final rrect = RRect.fromRectAndRadius(rect, const Radius.circular(24));
    
    final paint = Paint()
      ..style = PaintingStyle.stroke
      ..strokeWidth = 2
      ..shader = SweepGradient(
        colors: [
          color.withOpacity(0),
          color,
          color,
          color.withOpacity(0),
        ],
        stops: const [0.0, 0.25, 0.75, 1.0],
        transform: GradientRotation(rotation * 2 * math.pi),
      ).createShader(rect);
    
    canvas.drawRRect(rrect, paint);
  }
  
  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => true;
}