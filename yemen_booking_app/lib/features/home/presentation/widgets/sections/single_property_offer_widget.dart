// lib/features/home/presentation/widgets/sections/offers/single_property_offer_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'dart:math' as math;
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../search/data/models/search_result_model.dart';

class SinglePropertyOfferWidget extends StatefulWidget {
  final SearchResultModel property;
  final VoidCallback onTap;

  const SinglePropertyOfferWidget({
    super.key,
    required this.property,
    required this.onTap,
  });

  @override
  State<SinglePropertyOfferWidget> createState() => _SinglePropertyOfferWidgetState();
}

class _SinglePropertyOfferWidgetState extends State<SinglePropertyOfferWidget>
    with TickerProviderStateMixin {
  late AnimationController _ribbonController;
  late AnimationController _glowController;
  late AnimationController _priceController;
  late AnimationController _confettiController;
  
  late Animation<double> _ribbonAnimation;
  late Animation<double> _glowAnimation;
  late Animation<double> _priceAnimation;
  
  final List<_Confetti> _confettiPieces = [];

  @override
  void initState() {
    super.initState();
    _initializeAnimations();
    _generateConfetti();
  }

  void _initializeAnimations() {
    _ribbonController = AnimationController(
      duration: const Duration(seconds: 3),
      vsync: this,
    )..repeat(reverse: true);
    
    _glowController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat(reverse: true);
    
    _priceController = AnimationController(
      duration: const Duration(seconds: 4),
      vsync: this,
    )..repeat();
    
    _confettiController = AnimationController(
      duration: const Duration(seconds: 5),
      vsync: this,
    )..repeat();
    
    _ribbonAnimation = Tween<double>(
      begin: -0.02,
      end: 0.02,
    ).animate(CurvedAnimation(
      parent: _ribbonController,
      curve: Curves.easeInOut,
    ));
    
    _glowAnimation = Tween<double>(
      begin: 0.5,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _glowController,
      curve: Curves.easeInOut,
    ));
    
    _priceAnimation = Tween<double>(
      begin: 0.95,
      end: 1.05,
    ).animate(CurvedAnimation(
      parent: _priceController,
      curve: Curves.elasticInOut,
    ));
  }

  void _generateConfetti() {
    for (int i = 0; i < 15; i++) {
      _confettiPieces.add(_Confetti());
    }
  }

  @override
  void dispose() {
    _ribbonController.dispose();
    _glowController.dispose();
    _priceController.dispose();
    _confettiController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.mediumImpact();
        widget.onTap();
      },
      child: Container(
        height: 380,
        margin: const EdgeInsets.symmetric(horizontal: 20),
        child: Stack(
          children: [
            // Confetti animation
            AnimatedBuilder(
              animation: _confettiController,
              builder: (context, child) {
                return CustomPaint(
                  painter: _ConfettiPainter(
                    confetti: _confettiPieces,
                    animationValue: _confettiController.value,
                  ),
                );
              },
            ),
            
            // Main offer card
            AnimatedBuilder(
              animation: _glowAnimation,
              builder: (context, child) {
                return Container(
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(28),
                    boxShadow: [
                      BoxShadow(
                        color: AppTheme.success.withOpacity(0.3 * _glowAnimation.value),
                        blurRadius: 40,
                        spreadRadius: 10,
                        offset: const Offset(0, 20),
                      ),
                    ],
                  ),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(28),
                    child: Stack(
                      fit: StackFit.expand,
                      children: [
                        // Background image
                        CachedImageWidget(
                          imageUrl: widget.property.imageUrl ?? '',
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
                              stops: const [0.3, 1.0],
                            ),
                          ),
                        ),
                        
                        // Content
                        _buildContent(),
                        
                        // Offer ribbon
                        _buildOfferRibbon(),
                        
                        // Limited time badge
                        _buildLimitedTimeBadge(),
                      ],
                    ),
                  ),
                );
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildContent() {
    final originalPrice = widget.property.price ?? 0;
    final discountedPrice = originalPrice * (1 - (widget.property.discount ?? 30) / 100);
    
    return Positioned(
      bottom: 0,
      left: 0,
      right: 0,
      child: Container(
        padding: const EdgeInsets.all(24),
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [
              Colors.transparent,
              Colors.black.withOpacity(0.9),
            ],
          ),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Title
            ShaderMask(
              shaderCallback: (bounds) =>
                  LinearGradient(
                    colors: [Colors.white, Colors.white.withOpacity(0.9)],
                  ).createShader(bounds),
              child: Text(
                widget.property.name,
                style: AppTextStyles.heading1.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                  fontSize: 26,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ),
            
            const SizedBox(height: 12),
            
            // Location
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(6),
                  decoration: BoxDecoration(
                    color: AppTheme.success.withOpacity(0.2),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Icon(
                    Icons.location_on,
                    size: 16,
                    color: AppTheme.success,
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    widget.property.location ?? '',
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: Colors.white.withOpacity(0.9),
                    ),
                  ),
                ),
              ],
            ),
            
            const SizedBox(height: 20),
            
            // Features
            Row(
              children: [
                _buildFeatureChip(Icons.king_bed, '${widget.property.bedrooms ?? 0} غرف'),
                const SizedBox(width: 12),
                _buildFeatureChip(Icons.bathtub, '${widget.property.bathrooms ?? 0} حمام'),
                const SizedBox(width: 12),
                _buildFeatureChip(Icons.square_foot, '${widget.property.area ?? 0} م²'),
              ],
            ),
            
            const SizedBox(height: 20),
            
            // Price section
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Original price
                    Text(
                      '${originalPrice.toStringAsFixed(0)} ريال',
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: AppTheme.textMuted,
                        decoration: TextDecoration.lineThrough,
                      ),
                    ),
                    const SizedBox(height: 4),
                    // Discounted price with animation
                    AnimatedBuilder(
                      animation: _priceAnimation,
                      builder: (context, child) {
                        return Transform.scale(
                          scale: _priceAnimation.value,
                          child: Row(
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
                                  style: AppTextStyles.displaySmall.copyWith(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),
                              const SizedBox(width: 4),
                              Text(
                                'ريال/ليلة',
                                style: AppTextStyles.bodySmall.copyWith(
                                  color: AppTheme.textMuted,
                                ),
                              ),
                            ],
                          ),
                        );
                      },
                    ),
                  ],
                ),
                
                // CTA Button
                _buildCTAButton(),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFeatureChip(IconData icon, String label) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: AppTheme.darkCard.withOpacity(0.5),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppTheme.success.withOpacity(0.3),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            icon,
            size: 16,
            color: AppTheme.success,
          ),
          const SizedBox(width: 6),
          Text(
            label,
            style: AppTextStyles.caption.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCTAButton() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [AppTheme.success, AppTheme.primaryCyan],
        ),
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: AppTheme.success.withOpacity(0.4),
            blurRadius: 20,
            spreadRadius: 2,
          ),
        ],
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            'احجز الآن',
            style: AppTextStyles.buttonMedium.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(width: 8),
          const Icon(
            Icons.arrow_forward,
            color: Colors.white,
            size: 18,
          ),
        ],
      ),
    );
  }

  Widget _buildOfferRibbon() {
    return Positioned(
      top: 30,
      right: -30,
      child: AnimatedBuilder(
        animation: _ribbonAnimation,
        builder: (context, child) {
          return Transform.rotate(
            angle: math.pi / 4 + _ribbonAnimation.value,
            child: Container(
              width: 150,
              padding: const EdgeInsets.symmetric(vertical: 8),
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    AppTheme.error,
                    AppTheme.warning,
                  ],
                ),
                boxShadow: [
                  BoxShadow(
                    color: AppTheme.error.withOpacity(0.5),
                    blurRadius: 10,
                    spreadRadius: 2,
                  ),
                ],
              ),
              child: Center(
                child: Text(
                  'عرض خاص',
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildLimitedTimeBadge() {
    return Positioned(
      top: 20,
      left: 20,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        decoration: BoxDecoration(
          gradient: LinearGradient(
            colors: [
              AppTheme.success.withOpacity(0.9),
              AppTheme.primaryCyan.withOpacity(0.9),
            ],
          ),
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: AppTheme.success.withOpacity(0.5),
              blurRadius: 15,
              spreadRadius: 2,
            ),
          ],
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.timer,
              color: Colors.white,
              size: 16,
            ),
            const SizedBox(width: 6),
            Text(
              'لفترة محدودة',
              style: AppTextStyles.caption.copyWith(
                color: Colors.white,
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// Confetti model
class _Confetti {
  late double x;
  late double y;
  late double vx;
  late double vy;
  late double size;
  late Color color;
  late double rotation;
  late double rotationSpeed;
  
  _Confetti() {
    reset();
  }
  
  void reset() {
    x = math.Random().nextDouble();
    y = -0.1;
    vx = (math.Random().nextDouble() - 0.5) * 0.002;
    vy = math.Random().nextDouble() * 0.003 + 0.002;
    size = math.Random().nextDouble() * 6 + 2;
    rotation = math.Random().nextDouble() * 2 * math.pi;
    rotationSpeed = (math.Random().nextDouble() - 0.5) * 0.1;
    
    final colors = [
      AppTheme.success,
      AppTheme.warning,
      AppTheme.primaryCyan,
      AppTheme.primaryBlue,
    ];
    color = colors[math.Random().nextInt(colors.length)];
  }
  
  void update() {
    x += vx;
    y += vy;
    rotation += rotationSpeed;
    
    if (y > 1.1) {
      reset();
    }
  }
}

// Confetti painter
class _ConfettiPainter extends CustomPainter {
  final List<_Confetti> confetti;
  final double animationValue;
  
  _ConfettiPainter({
    required this.confetti,
    required this.animationValue,
  });
  
  @override
  void paint(Canvas canvas, Size size) {
    for (var piece in confetti) {
      piece.update();
      
      canvas.save();
      canvas.translate(piece.x * size.width, piece.y * size.height);
      canvas.rotate(piece.rotation);
      
      final paint = Paint()
        ..color = piece.color.withOpacity(0.8)
        ..style = PaintingStyle.fill;
      
      canvas.drawRect(
        Rect.fromCenter(
          center: Offset.zero,
          width: piece.size,
          height: piece.size * 0.6,
        ),
        paint,
      );
      
      canvas.restore();
    }
  }
  
  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => true;
}