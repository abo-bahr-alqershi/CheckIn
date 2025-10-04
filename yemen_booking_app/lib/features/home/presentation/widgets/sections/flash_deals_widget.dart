// lib/features/home/presentation/widgets/sections/offers/flash_deals_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'dart:math' as math;
import 'dart:async';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../search/data/models/search_result_model.dart';

class FlashDealsWidget extends StatefulWidget {
  final List<SearchResultModel> deals;
  final Function(String)? onItemTap;

  const FlashDealsWidget({
    super.key,
    required this.deals,
    this.onItemTap,
  });

  @override
  State<FlashDealsWidget> createState() => _FlashDealsWidgetState();
}

class _FlashDealsWidgetState extends State<FlashDealsWidget>
    with TickerProviderStateMixin {
  late AnimationController _flashController;
  late AnimationController _lightningController;
  late AnimationController _shakeController;
  
  Timer? _lightningTimer;
  
  @override
  void initState() {
    super.initState();
    _initializeAnimations();
    _startLightningEffect();
  }

  void _initializeAnimations() {
    _flashController = AnimationController(
      duration: const Duration(milliseconds: 500),
      vsync: this,
    );
    
    _lightningController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );
    
    _shakeController = AnimationController(
      duration: const Duration(milliseconds: 100),
      vsync: this,
    );
  }

  void _startLightningEffect() {
    _lightningTimer = Timer.periodic(const Duration(seconds: 3), (timer) {
      if (mounted) {
        _lightningController.forward().then((_) {
          _lightningController.reverse();
        });
        _shakeController.forward().then((_) {
          _shakeController.reverse();
        });
      }
    });
  }

  @override
  void dispose() {
    _lightningTimer?.cancel();
    _flashController.dispose();
    _lightningController.dispose();
    _shakeController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 280,
      child: Column(
        children: [
          // Flash deals header with lightning effect
          _buildFlashHeader(),
          
          const SizedBox(height: 16),
          
          // Deals list
          Expanded(
            child: ListView.builder(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 20),
              itemCount: widget.deals.length,
              itemBuilder: (context, index) {
                return _buildFlashDealCard(widget.deals[index], index);
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFlashHeader() {
    return AnimatedBuilder(
      animation: _shakeController,
      builder: (context, child) {
        return Transform.translate(
          offset: Offset(
            math.sin(_shakeController.value * math.pi * 4) * 2,
            0,
          ),
          child: Container(
            margin: const EdgeInsets.symmetric(horizontal: 20),
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  AppTheme.error,
                  AppTheme.warning,
                  AppTheme.error,
                ],
              ),
              borderRadius: BorderRadius.circular(16),
              boxShadow: [
                BoxShadow(
                  color: AppTheme.error.withOpacity(0.5),
                  blurRadius: 20,
                  spreadRadius: 5,
                ),
              ],
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                AnimatedBuilder(
                  animation: _lightningController,
                  builder: (context, child) {
                    return Icon(
                      Icons.flash_on,
                      color: Colors.white.withOpacity(
                        0.7 + (_lightningController.value * 0.3),
                      ),
                      size: 24,
                    );
                  },
                ),
                const SizedBox(width: 8),
                Text(
                  'عروض البرق',
                  style: AppTextStyles.heading2.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(width: 8),
                AnimatedBuilder(
                  animation: _lightningController,
                  builder: (context, child) {
                    return Icon(
                      Icons.flash_on,
                      color: Colors.white.withOpacity(
                        0.7 + (_lightningController.value * 0.3),
                      ),
                      size: 24,
                    );
                  },
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildFlashDealCard(SearchResultModel deal, int index) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.heavyImpact();
        widget.onItemTap?.call(deal.id);
        _flashController.forward().then((_) {
          _flashController.reverse();
        });
      },
      child: AnimatedBuilder(
        animation: _flashController,
        builder: (context, child) {
          return Container(
            width: 200,
            margin: const EdgeInsets.only(left: 16),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                colors: [
                  AppTheme.darkCard.withOpacity(0.9),
                  AppTheme.darkCard.withOpacity(0.7),
                ],
              ),
              borderRadius: BorderRadius.circular(20),
              border: Border.all(
                color: AppTheme.error.withOpacity(0.5),
                width: 2,
              ),
              boxShadow: [
                BoxShadow(
                  color: AppTheme.error.withOpacity(
                    0.3 + (_flashController.value * 0.3),
                  ),
                  blurRadius: 20 + (_flashController.value * 10),
                  spreadRadius: 2,
                ),
              ],
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(20),
              child: Stack(
                children: [
                  // Background image
                  Positioned.fill(
                    child: CachedImageWidget(
                      imageUrl: deal.imageUrl ?? '',
                      fit: BoxFit.cover,
                    ),
                  ),
                  
                  // Flash overlay effect
                  if (_flashController.value > 0)
                    Positioned.fill(
                      child: Container(
                        color: Colors.white.withOpacity(
                          _flashController.value * 0.3,
                        ),
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
                          stops: const [0.4, 1.0],
                        ),
                      ),
                    ),
                  ),
                  
                  // Content
                  Positioned.fill(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          // Flash badge
                          _buildFlashBadge(deal.discount ?? 50),
                          
                          const Spacer(),
                          
                          // Deal info
                          _buildDealInfo(deal),
                        ],
                      ),
                    ),
                  ),
                  
                  // Lightning strike effect
                  if (_lightningController.value > 0.5)
                    Positioned.fill(
                      child: CustomPaint(
                        painter: _LightningPainter(
                          progress: _lightningController.value,
                        ),
                      ),
                    ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildFlashBadge(int discount) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.error,
            AppTheme.warning,
          ],
        ),
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: AppTheme.error.withOpacity(0.5),
            blurRadius: 10,
            spreadRadius: 2,
          ),
        ],
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(
            Icons.flash_on,
            color: Colors.white,
            size: 16,
          ),
          const SizedBox(width: 4),
          Text(
            '-$discount%',
            style: AppTextStyles.bodyMedium.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDealInfo(SearchResultModel deal) {
    final originalPrice = deal.price ?? 0;
    final flashPrice = originalPrice * 0.5; // 50% off for flash deals
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          deal.name,
          style: AppTextStyles.bodyMedium.copyWith(
            color: Colors.white,
            fontWeight: FontWeight.bold,
          ),
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
        ),
        
        const SizedBox(height: 8),
        
        Row(
          children: [
            Text(
              '${originalPrice.toStringAsFixed(0)}',
              style: AppTextStyles.bodySmall.copyWith(
                color: AppTheme.textMuted,
                decoration: TextDecoration.lineThrough,
              ),
            ),
            const SizedBox(width: 8),
            ShaderMask(
              shaderCallback: (bounds) =>
                  LinearGradient(
                    colors: [AppTheme.error, AppTheme.warning],
                  ).createShader(bounds),
              child: Text(
                '${flashPrice.toStringAsFixed(0)} ريال',
                style: AppTextStyles.heading3.copyWith(
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
}

// Lightning strike painter
class _LightningPainter extends CustomPainter {
  final double progress;
  
  _LightningPainter({required this.progress});
  
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.white.withOpacity(0.8 * progress)
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke
      ..strokeCap = StrokeCap.round;
    
    final path = Path();
    path.moveTo(size.width * 0.6, 0);
    path.lineTo(size.width * 0.4, size.height * 0.4);
    path.lineTo(size.width * 0.5, size.height * 0.4);
    path.lineTo(size.width * 0.3, size.height);
    
    canvas.drawPath(path, paint);
  }
  
  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => true;
}