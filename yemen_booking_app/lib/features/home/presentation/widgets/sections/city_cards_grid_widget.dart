// lib/features/home/presentation/widgets/sections/destinations/city_cards_grid_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'dart:math' as math;
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/widgets/cached_image_widget.dart';
import '../../../../search/data/models/search_result_model.dart';

class CityCardsGridWidget extends StatefulWidget {
  final List<SearchResultModel> cities;
  final Function(String)? onItemTap;

  const CityCardsGridWidget({
    super.key,
    required this.cities,
    this.onItemTap,
  });

  @override
  State<CityCardsGridWidget> createState() => _CityCardsGridWidgetState();
}

class _CityCardsGridWidgetState extends State<CityCardsGridWidget>
    with TickerProviderStateMixin {
  late AnimationController _waveController;
  late AnimationController _floatController;
  
  final Map<int, AnimationController> _itemControllers = {};
  final Map<int, bool> _hoveredItems = {};

  @override
  void initState() {
    super.initState();
    _waveController = AnimationController(
      duration: const Duration(seconds: 3),
      vsync: this,
    )..repeat();
    
    _floatController = AnimationController(
      duration: const Duration(seconds: 4),
      vsync: this,
    )..repeat(reverse: true);
    
    // Initialize item controllers
    for (int i = 0; i < widget.cities.length; i++) {
      _itemControllers[i] = AnimationController(
        duration: const Duration(milliseconds: 600),
        vsync: this,
      );
      _hoveredItems[i] = false;
    }
  }

  @override
  void dispose() {
    _waveController.dispose();
    _floatController.dispose();
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
          childAspectRatio: 0.85,
          crossAxisSpacing: 16,
          mainAxisSpacing: 16,
        ),
        itemCount: math.min(widget.cities.length, 4), // Show max 4 cities
        itemBuilder: (context, index) {
          return _buildCityCard(widget.cities[index], index);
        },
      ),
    );
  }

  Widget _buildCityCard(SearchResultModel city, int index) {
    return GestureDetector(
      onTapDown: (_) => _onItemPressed(index),
      onTapUp: (_) => _onItemReleased(index),
      onTapCancel: () => _onItemReleased(index),
      onTap: () {
        HapticFeedback.lightImpact();
        widget.onItemTap?.call(city.id);
      },
      child: AnimatedBuilder(
        animation: Listenable.merge([
          _itemControllers[index]!,
          _floatController,
        ]),
        builder: (context, child) {
          final scale = 1.0 - (_itemControllers[index]!.value * 0.05);
          final floatOffset = math.sin(_floatController.value * math.pi * 2) * 5;
          
          return Transform.translate(
            offset: Offset(0, index.isEven ? floatOffset : -floatOffset),
            child: Transform.scale(
              scale: scale,
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(20),
                  boxShadow: [
                    BoxShadow(
                      color: AppTheme.primaryCyan.withOpacity(0.3),
                      blurRadius: _hoveredItems[index]! ? 30 : 20,
                      spreadRadius: _hoveredItems[index]! ? 5 : 2,
                      offset: const Offset(0, 10),
                    ),
                  ],
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(20),
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      // City image
                      Hero(
                        tag: 'city_${city.id}_$index',
                        child: CachedImageWidget(
                          imageUrl: city.imageUrl ?? '',
                          fit: BoxFit.cover,
                        ),
                      ),
                      
                      // Animated gradient overlay
                      AnimatedBuilder(
                        animation: _waveController,
                        builder: (context, child) {
                          return Container(
                            decoration: BoxDecoration(
                              gradient: LinearGradient(
                                begin: Alignment(
                                  -1 + (_waveController.value * 2),
                                  -1,
                                ),
                                end: Alignment(
                                  1,
                                  1 - (_waveController.value * 2),
                                ),
                                colors: [
                                  Colors.transparent,
                                  AppTheme.primaryCyan.withOpacity(0.2),
                                  AppTheme.primaryBlue.withOpacity(0.3),
                                  Colors.black.withOpacity(0.7),
                                ],
                                stops: const [0.0, 0.3, 0.6, 1.0],
                              ),
                            ),
                          );
                        },
                      ),
                      
                      // Content
                      Positioned(
                        bottom: 16,
                        left: 16,
                        right: 16,
                        child: _buildCityInfo(city),
                      ),
                      
                      // Explore icon
                      Positioned(
                        top: 16,
                        right: 16,
                        child: Container(
                          padding: const EdgeInsets.all(8),
                          decoration: BoxDecoration(
                            color: AppTheme.darkCard.withOpacity(0.7),
                            shape: BoxShape.circle,
                            border: Border.all(
                              color: AppTheme.primaryCyan.withOpacity(0.3),
                              width: 1,
                            ),
                          ),
                          child: Icon(
                            Icons.explore,
                            color: AppTheme.primaryCyan,
                            size: 20,
                          ),
                        ),
                      ),
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

  Widget _buildCityInfo(SearchResultModel city) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          city.name,
          style: AppTextStyles.heading3.copyWith(
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
              color: AppTheme.primaryCyan,
            ),
            const SizedBox(width: 4),
            Expanded(
              child: Text(
                city.location ?? '',
                style: AppTextStyles.caption.copyWith(
                  color: Colors.white.withOpacity(0.9),
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                AppTheme.primaryCyan.withOpacity(0.8),
                AppTheme.primaryBlue.withOpacity(0.8),
              ],
            ),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Text(
            '${city.propertiesCount ?? 0} عقار',
            style: AppTextStyles.caption.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
      ],
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