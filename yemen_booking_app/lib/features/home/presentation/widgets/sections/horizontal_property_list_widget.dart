// lib/features/home/presentation/widgets/sections/properties/horizontal_property_list_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';
import 'package:yemen_booking_app/features/home/presentation/widgets/sections/horizontal_property_card.dart';
import 'package:yemen_booking_app/features/search/data/models/search_result_model.dart';
import '../../../../../../core/theme/app_theme.dart';

class HorizontalPropertyListWidget extends StatefulWidget {
  final List<SearchResultModel> properties;
  final Function(String)? onItemTap;
  final VoidCallback? onLoadMore;

  const HorizontalPropertyListWidget({
    super.key,
    required this.properties,
    this.onItemTap,
    this.onLoadMore,
  });

  @override
  State<HorizontalPropertyListWidget> createState() => 
      _HorizontalPropertyListWidgetState();
}

class _HorizontalPropertyListWidgetState extends State<HorizontalPropertyListWidget>
    with TickerProviderStateMixin {
  late ScrollController _scrollController;
  late AnimationController _shimmerController;
  late AnimationController _parallaxController;
  
  double _scrollOffset = 0;
  bool _isLoadingMore = false;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController()
      ..addListener(_onScroll);
    
    _shimmerController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat();
    
    _parallaxController = AnimationController(
      duration: const Duration(seconds: 10),
      vsync: this,
    )..repeat();
  }

  void _onScroll() {
    setState(() {
      _scrollOffset = _scrollController.offset;
    });
    
    // Load more when reaching the end
    if (_scrollController.position.pixels >= 
        _scrollController.position.maxScrollExtent - 200) {
      if (!_isLoadingMore && widget.onLoadMore != null) {
        setState(() {
          _isLoadingMore = true;
        });
        widget.onLoadMore!();
        Future.delayed(const Duration(seconds: 2), () {
          if (mounted) {
            setState(() {
              _isLoadingMore = false;
            });
          }
        });
      }
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    _shimmerController.dispose();
    _parallaxController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 280,
      child: ListView.builder(
        controller: _scrollController,
        scrollDirection: Axis.horizontal,
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.symmetric(horizontal: 20),
        itemCount: widget.properties.length + (_isLoadingMore ? 1 : 0),
        itemBuilder: (context, index) {
          if (index == widget.properties.length) {
            return _buildLoadingIndicator();
          }
          
          final property = widget.properties[index];
          final parallaxOffset = _calculateParallaxOffset(index);
          
          return AnimatedBuilder(
            animation: _parallaxController,
            builder: (context, child) {
              return Transform.translate(
                offset: Offset(parallaxOffset, 0),
                child: HorizontalPropertyCard(
                  property: property,
                  onTap: () => _handlePropertyTap(property),
                  index: index,
                  scrollOffset: _scrollOffset,
                ),
              );
            },
          );
        },
      ),
    );
  }

  Widget _buildLoadingIndicator() {
    return Container(
      width: 100,
      margin: const EdgeInsets.symmetric(horizontal: 8),
      child: Center(
        child: AnimatedBuilder(
          animation: _shimmerController,
          builder: (context, child) {
            return Container(
              width: 50,
              height: 50,
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
      ),
    );
  }

  double _calculateParallaxOffset(int index) {
    final itemPosition = index * 320.0; // Approximate card width
    final viewportOffset = _scrollOffset;
    final relativeOffset = itemPosition - viewportOffset;
    return relativeOffset * 0.05; // Parallax factor
  }

  void _handlePropertyTap(SearchResultModel property) {
    HapticFeedback.lightImpact();
    widget.onItemTap?.call(property.id);
    
    // Navigate to property details
    context.push('/property/${property.id}');
  }
}