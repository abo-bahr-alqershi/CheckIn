// lib/features/home/presentation/widgets/sections/properties/vertical_property_grid_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'dart:math' as math;
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../search/data/models/search_result_model.dart';

class VerticalPropertyGridWidget extends StatefulWidget {
  final List<SearchResultModel> properties;
  final Function(String)? onItemTap;

  const VerticalPropertyGridWidget({
    super.key,
    required this.properties,
    this.onItemTap,
  });

  @override
  State<VerticalPropertyGridWidget> createState() => _VerticalPropertyGridWidgetState();
}

class _VerticalPropertyGridWidgetState extends State<VerticalPropertyGridWidget>
    with TickerProviderStateMixin {
  late AnimationController _gridAnimationController;
  final Map<int, AnimationController> _itemControllers = {};
  final Map<int, bool> _hoveredItems = {};

  @override
  void initState() {
    super.initState();
    _gridAnimationController = AnimationController(
      duration: const Duration(milliseconds: 600),
      vsync: this,
    )..forward();
    
    // Initialize item controllers
    for (int i = 0; i < widget.properties.length; i++) {
      _itemControllers[i] = AnimationController(
        duration: const Duration(milliseconds: 300),
        vsync: this,
      );
      _hoveredItems[i] = false;
    }
  }

  @override
  void dispose() {
    _gridAnimationController.dispose();
    _itemControllers.forEach((_, controller) => controller.dispose());
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      child: GridView.builder(
        shrinkWrap: true,
        physics: const NeverScrollableScrollPhysics(),
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: 2,
          childAspectRatio: 0.75,
          crossAxisSpacing: 16,
          mainAxisSpacing: 16,
        ),
        itemCount: math.min(widget.properties.length, 6),
        itemBuilder: (context, index) {
          return AnimatedBuilder(
            animation: _gridAnimationController,
            builder: (context, child) {
              final delay = index * 0.1;
              final animation = Curves.easeOutBack.transform(
                (_gridAnimationController.value - delay).clamp(0.0, 1.0),
              );
              
              return Transform.scale(
                scale: animation,
                child: Transform.translate(
                  offset: Offset(0, 50 * (1 - animation)),
                  child: Opacity(
                    opacity: animation,
                    child: _buildPropertyCard(widget.properties[index], index),
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }

  Widget _buildPropertyCard(SearchResultModel property, int index) {
    return GestureDetector(
      onTapDown: (_) => _onItemPressed(index),
      onTapUp: (_) => _onItemReleased(index),
      onTapCancel: () => _onItemReleased(index),
      onTap: () {
        HapticFeedback.lightImpact();
        widget.onItemTap?.call(property.id);
      },
      child: AnimatedBuilder(
        animation: _itemControllers[index]!,
        builder: (context, child) {
          final scale = 1.0 - (_itemControllers[index]!.value * 0.05);
          
          return Transform.scale(
            scale: scale,
            child: Container(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(20),
                boxShadow: [
                  BoxShadow(
                    color: _hoveredItems[index]!
                        ? AppTheme.primaryBlue.withOpacity(0.3)
                        : AppTheme.shadowDark.withOpacity(0.2),
                    blurRadius: _hoveredItems[index]! ? 25 : 15,
                    spreadRadius: _hoveredItems[index]! ? 5 : 2,
                    offset: const Offset(0, 8),
                  ),
                ],
              ),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(20),
                child: Stack(
                  fit: StackFit.expand,
                  children: [
                    // Property image
                    Hero(
                      tag: 'property_grid_${property.id}_$index',
                      child: CachedImageWidget(
                        imageUrl: property.imageUrl ?? '',
                        fit: BoxFit.cover,
                      ),
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
                          stops: const [0.5, 1.0],
                        ),
                      ),
                    ),
                    
                    // Hover effect overlay
                    if (_hoveredItems[index]!)
                      Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.topLeft,
                            end: Alignment.bottomRight,
                            colors: [
                              AppTheme.primaryBlue.withOpacity(0.1),
                              AppTheme.primaryPurple.withOpacity(0.05),
                            ],
                          ),
                        ),
                      ),
                    
                    // Content
                    Positioned(
                      bottom: 12,
                      left: 12,
                      right: 12,
                      child: _buildCardContent(property),
                    ),
                    
                    // Top badges
                    Positioned(
                      top: 12,
                      left: 12,
                      right: 12,
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          if (property.discount != null)
                            _buildDiscountBadge(property.discount!),
                          _buildFavoriteButton(),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildCardContent(SearchResultModel property) {
    return ClipRRect(
      borderRadius: BorderRadius.circular(12),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
        child: Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                AppTheme.darkCard.withOpacity(0.8),
                AppTheme.darkCard.withOpacity(0.6),
              ],
            ),
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: AppTheme.primaryBlue.withOpacity(0.2),
              width: 1,
            ),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                property.name,
                style: AppTextStyles.bodyMedium.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              
              const SizedBox(height: 4),
              
              Row(
                children: [
                  Icon(
                    Icons.location_on,
                    size: 12,
                    color: AppTheme.primaryBlue,
                  ),
                  const SizedBox(width: 2),
                  Expanded(
                    child: Text(
                      property.location ?? '',
                      style: AppTextStyles.caption.copyWith(
                        color: AppTheme.textLight,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ],
              ),
              
              const SizedBox(height: 8),
              
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  // Price
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ShaderMask(
                        shaderCallback: (bounds) =>
                            AppTheme.primaryGradient.createShader(bounds),
                        child: Text(
                          '${property.price?.toStringAsFixed(0) ?? '0'}',
                          style: AppTextStyles.bodyLarge.copyWith(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      Text(
                        'ريال/ليلة',
                        style: AppTextStyles.caption.copyWith(
                          color: AppTheme.textMuted,
                          fontSize: 10,
                        ),
                      ),
                    ],
                  ),
                  
                  // Rating
                  if (property.rating != null)
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 6,
                        vertical: 3,
                      ),
                      decoration: BoxDecoration(
                        color: AppTheme.warning.withOpacity(0.2),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Icon(
                            Icons.star,
                            size: 12,
                            color: AppTheme.warning,
                          ),
                          const SizedBox(width: 2),
                          Text(
                            property.rating!.toStringAsFixed(1),
                            style: AppTextStyles.caption.copyWith(
                              color: AppTheme.warning,
                              fontWeight: FontWeight.bold,
                              fontSize: 11,
                            ),
                          ),
                        ],
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

  Widget _buildDiscountBadge(int discount) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [AppTheme.error, AppTheme.warning],
        ),
        borderRadius: BorderRadius.circular(8),
        boxShadow: [
          BoxShadow(
            color: AppTheme.error.withOpacity(0.5),
            blurRadius: 8,
            spreadRadius: 1,
          ),
        ],
      ),
      child: Text(
        '-$discount%',
        style: AppTextStyles.caption.copyWith(
          color: Colors.white,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }

  Widget _buildFavoriteButton() {
    return Container(
      width: 32,
      height: 32,
      decoration: BoxDecoration(
        color: AppTheme.darkCard.withOpacity(0.7),
        shape: BoxShape.circle,
        border: Border.all(
          color: Colors.white.withOpacity(0.2),
          width: 1,
        ),
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(16),
        child: BackdropFilter(
          filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
          child: const Icon(
            Icons.favorite_border,
            color: Colors.white,
            size: 16,
          ),
        ),
      ),
    );
  }

  void _onItemPressed(int index) {
    setState(() {
      _hoveredItems[index] = true;
    });
    _itemControllers[index]!.forward();
  }

  void _onItemReleased(int index) {
    setState(() {
      _hoveredItems[index] = false;
    });
    _itemControllers[index]!.reverse();
  }
}