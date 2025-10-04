// lib/features/home/presentation/widgets/sections/premium/premium_carousel_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'dart:math' as math;
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../search/data/models/search_result_model.dart';

class PremiumCarouselWidget extends StatefulWidget {
  final List<SearchResultModel> properties;
  final Function(String)? onItemTap;

  const PremiumCarouselWidget({
    super.key,
    required this.properties,
    this.onItemTap,
  });

  @override
  State<PremiumCarouselWidget> createState() => _PremiumCarouselWidgetState();
}

class _PremiumCarouselWidgetState extends State<PremiumCarouselWidget>
    with TickerProviderStateMixin {
  late PageController _pageController;
  late AnimationController _goldShineController;
  late AnimationController _crownRotationController;
  late AnimationController _sparkleController;
  
  int _currentIndex = 0;
  final List<_Sparkle> _sparkles = [];

  @override
  void initState() {
    super.initState();
    _pageController = PageController(viewportFraction: 0.85);
    
    _goldShineController = AnimationController(
      duration: const Duration(seconds: 3),
      vsync: this,
    )..repeat();
    
    _crownRotationController = AnimationController(
      duration: const Duration(seconds: 20),
      vsync: this,
    )..repeat();
    
    _sparkleController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat();
    
    _generateSparkles();
  }

  void _generateSparkles() {
    for (int i = 0; i < 20; i++) {
      _sparkles.add(_Sparkle());
    }
  }

  @override
  void dispose() {
    _pageController.dispose();
    _goldShineController.dispose();
    _crownRotationController.dispose();
    _sparkleController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 380,
      child: Stack(
        children: [
          // Gold sparkles background
          AnimatedBuilder(
            animation: _sparkleController,
            builder: (context, child) {
              return CustomPaint(
                painter: _SparklesPainter(
                  sparkles: _sparkles,
                  animationValue: _sparkleController.value,
                ),
                size: const Size(double.infinity, 380),
              );
            },
          ),
          
          // Premium carousel
          PageView.builder(
            controller: _pageController,
            onPageChanged: (index) {
              setState(() {
                _currentIndex = index;
              });
            },
            itemCount: widget.properties.length,
            itemBuilder: (context, index) {
              return _buildPremiumCard(widget.properties[index], index);
            },
          ),
          
          // Premium badge
          Positioned(
            top: 20,
            left: 0,
            right: 0,
            child: _buildPremiumBadge(),
          ),
        ],
      ),
    );
  }

  Widget _buildPremiumBadge() {
    return Center(
      child: AnimatedBuilder(
        animation: _crownRotationController,
        builder: (context, child) {
          return Container(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
            decoration: BoxDecoration(
              gradient: const LinearGradient(
                colors: [
                  Color(0xFFFFD700),
                  Color(0xFFFFA500),
                  Color(0xFFFFD700),
                ],
              ),
              borderRadius: BorderRadius.circular(20),
              boxShadow: [
                BoxShadow(
                  color: const Color(0xFFFFD700).withOpacity(0.5),
                  blurRadius: 20,
                  spreadRadius: 5,
                ),
              ],
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Transform.rotate(
                  angle: math.sin(_crownRotationController.value * 2 * math.pi) * 0.1,
                  child: const Icon(
                    Icons.workspace_premium,
                    color: Colors.white,
                    size: 24,
                  ),
                ),
                const SizedBox(width: 8),
                ShaderMask(
                  shaderCallback: (bounds) =>
                      LinearGradient(
                        colors: [Colors.white, Colors.white.withOpacity(0.9)],
                      ).createShader(bounds),
                  child: Text(
                    'عقارات مميزة',
                    style: AppTextStyles.bodyLarge.copyWith(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildPremiumCard(SearchResultModel property, int index) {
    final isActive = index == _currentIndex;
    
    return AnimatedScale(
      scale: isActive ? 1.0 : 0.9,
      duration: const Duration(milliseconds: 300),
      child: Container(
        margin: const EdgeInsets.only(
          left: 8,
          right: 8,
          top: 60,
          bottom: 20,
        ),
        child: GestureDetector(
          onTap: () {
            HapticFeedback.mediumImpact();
            widget.onItemTap?.call(property.id);
          },
          child: Stack(
            children: [
              // Main card
              Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(28),
                  gradient: const LinearGradient(
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                    colors: [
                      Color(0xFF1A1A2E),
                      Color(0xFF16213E),
                    ],
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: const Color(0xFFFFD700).withOpacity(0.3),
                      blurRadius: 30,
                      spreadRadius: 5,
                      offset: const Offset(0, 10),
                    ),
                  ],
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(28),
                  child: Stack(
                    children: [
                      // Background image
                      Positioned.fill(
                        child: CachedImageWidget(
                          imageUrl: property.imageUrl ?? '',
                          fit: BoxFit.cover,
                        ),
                      ),
                      
                      // Gold gradient overlay
                      Positioned.fill(
                        child: AnimatedBuilder(
                          animation: _goldShineController,
                          builder: (context, child) {
                            return Container(
                              decoration: BoxDecoration(
                                gradient: LinearGradient(
                                  begin: Alignment(
                                    -1 + (_goldShineController.value * 2),
                                    -1,
                                  ),
                                  end: Alignment(
                                    0 + (_goldShineController.value * 2),
                                    1,
                                  ),
                                  colors: [
                                    Colors.transparent,
                                    const Color(0xFFFFD700).withOpacity(0.1),
                                    Colors.transparent,
                                  ],
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                      
                      // Dark gradient for text
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
                              stops: const [0.5, 1.0],
                            ),
                          ),
                        ),
                      ),
                      
                      // Content
                      Positioned(
                        bottom: 24,
                        left: 24,
                        right: 24,
                        child: _buildPremiumContent(property),
                      ),
                      
                      // Premium crown
                      Positioned(
                        top: 20,
                        right: 20,
                        child: _buildPremiumCrown(),
                      ),
                    ],
                  ),
                ),
              ),
              
              // Gold border
              if (isActive)
                Positioned.fill(
                  child: IgnorePointer(
                    child: Container(
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(28),
                        border: Border.all(
                          width: 3,
                          color: const Color(0xFFFFD700).withOpacity(0.5),
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

  Widget _buildPremiumContent(SearchResultModel property) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Title with gold effect
        ShaderMask(
          shaderCallback: (bounds) =>
              const LinearGradient(
                colors: [
                  Color(0xFFFFD700),
                  Color(0xFFFFA500),
                  Colors.white,
                ],
              ).createShader(bounds),
          child: Text(
            property.name,
            style: AppTextStyles.heading1.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
              fontSize: 24,
            ),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
        ),
        
        const SizedBox(height: 12),
        
        // Location with premium icon
        Row(
          children: [
            Container(
              padding: const EdgeInsets.all(6),
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    const Color(0xFFFFD700).withOpacity(0.3),
                    const Color(0xFFFFA500).withOpacity(0.2),
                  ],
                ),
                borderRadius: BorderRadius.circular(8),
              ),
              child: const Icon(
                Icons.location_on,
                size: 14,
                color: Color(0xFFFFD700),
              ),
            ),
            const SizedBox(width: 8),
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
        
        // Premium features
        Row(
          children: [
            _buildPremiumFeature(Icons.king_bed, '${property.bedrooms ?? 0}'),
            const SizedBox(width: 12),
            _buildPremiumFeature(Icons.bathtub, '${property.bathrooms ?? 0}'),
            const SizedBox(width: 12),
            _buildPremiumFeature(Icons.square_foot, '${property.area ?? 0} م²'),
          ],
        ),
        
        const SizedBox(height: 16),
        
        // Price with premium styling
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'السعر الحصري',
                  style: AppTextStyles.caption.copyWith(
                    color: const Color(0xFFFFD700),
                  ),
                ),
                const SizedBox(height: 4),
                ShaderMask(
                  shaderCallback: (bounds) =>
                      const LinearGradient(
                        colors: [
                          Color(0xFFFFD700),
                          Color(0xFFFFA500),
                        ],
                      ).createShader(bounds),
                  child: Text(
                    '${property.price?.toStringAsFixed(0) ?? '0'} ريال',
                    style: AppTextStyles.heading1.copyWith(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: 20,
                vertical: 10,
              ),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [
                    Color(0xFFFFD700),
                    Color(0xFFFFA500),
                  ],
                ),
                borderRadius: BorderRadius.circular(14),
                boxShadow: [
                  BoxShadow(
                    color: const Color(0xFFFFD700).withOpacity(0.5),
                    blurRadius: 10,
                    spreadRadius: 2,
                  ),
                ],
              ),
              child: Text(
                'VIP',
                style: AppTextStyles.buttonMedium.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildPremiumFeature(IconData icon, String value) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            const Color(0xFFFFD700).withOpacity(0.2),
            const Color(0xFFFFA500).withOpacity(0.1),
          ],
        ),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: const Color(0xFFFFD700).withOpacity(0.3),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            icon,
            size: 14,
            color: const Color(0xFFFFD700),
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

  Widget _buildPremiumCrown() {
    return AnimatedBuilder(
      animation: _crownRotationController,
      builder: (context, child) {
        return Container(
          width: 50,
          height: 50,
          decoration: BoxDecoration(
            gradient: RadialGradient(
              colors: [
                const Color(0xFFFFD700),
                const Color(0xFFFFA500),
                const Color(0xFFFFD700).withOpacity(0.5),
              ],
            ),
            shape: BoxShape.circle,
            boxShadow: [
              BoxShadow(
                color: const Color(0xFFFFD700).withOpacity(0.6),
                blurRadius: 20,
                spreadRadius: 5,
              ),
            ],
          ),
          child: Transform.rotate(
            angle: math.sin(_crownRotationController.value * 2 * math.pi) * 0.2,
            child: const Icon(
              Icons.workspace_premium,
              color: Colors.white,
              size: 28,
            ),
          ),
        );
      },
    );
  }
}

// Sparkle model
class _Sparkle {
  late double x;
  late double y;
  late double size;
  late double speed;
  late double opacity;
  
  _Sparkle() {
    reset();
  }
  
  void reset() {
    x = math.Random().nextDouble();
    y = math.Random().nextDouble();
    size = math.Random().nextDouble() * 3 + 1;
    speed = math.Random().nextDouble() * 0.002 + 0.001;
    opacity = math.Random().nextDouble() * 0.5 + 0.2;
  }
  
  void update() {
    y -= speed;
    if (y < 0) {
      reset();
      y = 1.0;
    }
  }
}

// Sparkles painter
class _SparklesPainter extends CustomPainter {
  final List<_Sparkle> sparkles;
  final double animationValue;
  
  _SparklesPainter({
    required this.sparkles,
    required this.animationValue,
  });
  
  @override
  void paint(Canvas canvas, Size size) {
    for (var sparkle in sparkles) {
      sparkle.update();
      
      final paint = Paint()
        ..color = const Color(0xFFFFD700).withOpacity(sparkle.opacity)
        ..style = PaintingStyle.fill
        ..maskFilter = const MaskFilter.blur(BlurStyle.normal, 2);
      
      canvas.drawCircle(
        Offset(sparkle.x * size.width, sparkle.y * size.height),
        sparkle.size,
        paint,
      );
    }
  }
  
  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => true;
}