// lib/features/home/presentation/widgets/categories/ultra_futuristic_property_types_grid.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:yemen_booking_app/features/home/presentation/widgets/sections/futuristic_property_type_card_widget.dart';
import 'dart:ui';
import 'dart:math' as math;
import '../../../../../core/theme/app_theme.dart';
import '../../../../../core/theme/app_text_styles.dart';
import '../../../../../core/theme/app_dimensions.dart';
import '../../../domain/entities/property_type.dart';

class FuturisticPropertyTypesGrid extends StatefulWidget {
  final List<PropertyType> propertyTypes;
  final String? selectedTypeId;
  final Function(String?) onTypeSelected;
  final bool isCompact;

  const FuturisticPropertyTypesGrid({
    super.key,
    required this.propertyTypes,
    this.selectedTypeId,
    required this.onTypeSelected,
    this.isCompact = false,
  });

  @override
  State<FuturisticPropertyTypesGrid> createState() => 
      _FuturisticPropertyTypesGridState();
}

class _FuturisticPropertyTypesGridState extends State<FuturisticPropertyTypesGrid>
    with TickerProviderStateMixin {
  late AnimationController _expandController;
  late AnimationController _shimmerController;
  late AnimationController _pulseController;
  late AnimationController _headerGlowController;
  late AnimationController _floatingController;
  
  late Animation<double> _expandAnimation;
  late Animation<double> _pulseAnimation;
  late Animation<double> _floatingAnimation;
  
  bool _isExpanded = false;
  final int _initialDisplayCount = 6;
  final int _compactDisplayCount = 4;

  @override
  void initState() {
    super.initState();
    _initializeAnimations();
  }

  void _initializeAnimations() {
    _expandController = AnimationController(
      duration: const Duration(milliseconds: 600),
      vsync: this,
    );
    
    _shimmerController = AnimationController(
      duration: const Duration(seconds: 3),
      vsync: this,
    )..repeat();
    
    _pulseController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat(reverse: true);
    
    _headerGlowController = AnimationController(
      duration: const Duration(seconds: 4),
      vsync: this,
    )..repeat(reverse: true);
    
    _floatingController = AnimationController(
      duration: const Duration(seconds: 6),
      vsync: this,
    )..repeat(reverse: true);
    
    _expandAnimation = CurvedAnimation(
      parent: _expandController,
      curve: Curves.easeOutExpo,
    );
    
    _pulseAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _pulseController,
      curve: Curves.easeInOut,
    ));
    
    _floatingAnimation = Tween<double>(
      begin: -5.0,
      end: 5.0,
    ).animate(CurvedAnimation(
      parent: _floatingController,
      curve: Curves.easeInOut,
    ));
  }

  @override
  void dispose() {
    _expandController.dispose();
    _shimmerController.dispose();
    _pulseController.dispose();
    _headerGlowController.dispose();
    _floatingController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (widget.propertyTypes.isEmpty) {
      return _buildEmptyState();
    }

    final baseCount = widget.isCompact 
        ? _compactDisplayCount 
        : _initialDisplayCount;
    final displayCount = _isExpanded 
        ? widget.propertyTypes.length 
        : math.min(baseCount, widget.propertyTypes.length);

    return AnimatedBuilder(
      animation: _floatingAnimation,
      builder: (context, child) {
        return Transform.translate(
          offset: Offset(0, _floatingAnimation.value),
          child: Container(
            padding: EdgeInsets.symmetric(
              horizontal: widget.isCompact ? 8 : 12,
              vertical: widget.isCompact ? 6 : 10,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (!widget.isCompact) _buildUltraHeader(),
                if (!widget.isCompact) const SizedBox(height: 10),
                _buildUltraGrid(displayCount),
                if (widget.propertyTypes.length > baseCount)
                  _buildUltraExpandButton(),
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildEmptyState() {
    return Container(
      height: 180,
      margin: const EdgeInsets.all(12),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: BackdropFilter(
          filter: ImageFilter.blur(sigmaX: 20, sigmaY: 20),
          child: Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  Colors.white.withOpacity(0.05),
                  Colors.white.withOpacity(0.02),
                ],
              ),
              borderRadius: BorderRadius.circular(20),
              border: Border.all(
                color: Colors.white.withOpacity(0.1),
                width: 0.5,
              ),
            ),
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  AnimatedBuilder(
                    animation: _shimmerController,
                    builder: (context, child) {
                      return Container(
                        width: 50,
                        height: 50,
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          border: Border.all(
                            color: AppTheme.primaryBlue.withOpacity(0.3),
                            width: 2,
                          ),
                        ),
                        child: Center(
                          child: Transform.rotate(
                            angle: _shimmerController.value * 2 * math.pi,
                            child: Icon(
                              Icons.category_rounded,
                              color: AppTheme.primaryBlue.withOpacity(0.5),
                              size: 24,
                            ),
                          ),
                        ),
                      );
                    },
                  ),
                  const SizedBox(height: 10),
                  Text(
                    'لا توجد فئات',
                    style: AppTextStyles.bodySmall.copyWith(
                      color: AppTheme.textMuted.withOpacity(0.6),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildUltraHeader() {
    return AnimatedBuilder(
      animation: _headerGlowController,
      builder: (context, child) {
        return Container(
          padding: const EdgeInsets.symmetric(horizontal: 4),
          child: Row(
            children: [
              // Floating icon
              Container(
                width: 32,
                height: 32,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  border: Border.all(
                    color: AppTheme.primaryBlue.withOpacity(
                      0.3 + (0.3 * _headerGlowController.value),
                    ),
                    width: 1,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: AppTheme.primaryBlue.withOpacity(0.2),
                      blurRadius: 10 + (5 * _headerGlowController.value),
                    ),
                  ],
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(16),
                  child: BackdropFilter(
                    filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
                    child: Container(
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          colors: [
                            AppTheme.primaryBlue.withOpacity(0.2),
                            AppTheme.primaryPurple.withOpacity(0.1),
                          ],
                        ),
                      ),
                      child: const Icon(
                        Icons.apps_rounded,
                        color: Colors.white,
                        size: 16,
                      ),
                    ),
                  ),
                ),
              ),
              
              const SizedBox(width: 8),
              
              // Title
              Text(
                'الفئات',
                style: AppTextStyles.bodyLarge.copyWith(
                  fontWeight: FontWeight.w600,
                  color: AppTheme.textWhite.withOpacity(0.9),
                ),
              ),
              
              const SizedBox(width: 6),
              
              // Count
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                decoration: BoxDecoration(
                  color: AppTheme.primaryBlue.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(
                    color: AppTheme.primaryBlue.withOpacity(0.2),
                    width: 0.5,
                  ),
                ),
                child: Text(
                  '${widget.propertyTypes.length}',
                  style: AppTextStyles.caption.copyWith(
                    color: AppTheme.primaryBlue.withOpacity(0.8),
                    fontWeight: FontWeight.w600,
                    fontSize: 10,
                  ),
                ),
              ),
              
              const Spacer(),
              
              // Clear button
              if (widget.selectedTypeId != null)
                _buildClearButton(),
            ],
          ),
        );
      },
    );
  }

  Widget _buildClearButton() {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        widget.onTypeSelected(null);
      },
      child: Container(
        width: 28,
        height: 28,
        decoration: BoxDecoration(
          shape: BoxShape.circle,
          color: AppTheme.error.withOpacity(0.1),
          border: Border.all(
            color: AppTheme.error.withOpacity(0.2),
            width: 0.5,
          ),
        ),
        child: Icon(
          Icons.close_rounded,
          size: 14,
          color: AppTheme.error.withOpacity(0.7),
        ),
      ),
    );
  }

  Widget _buildUltraGrid(int displayCount) {
    final crossAxisCount = widget.isCompact ? 3 : 3;
    final aspectRatio = widget.isCompact ? 1.0 : 0.95;
    final spacing = widget.isCompact ? 6.0 : 8.0;
    
    return AnimatedContainer(
      duration: const Duration(milliseconds: 500),
      curve: Curves.easeOutExpo,
      height: _calculateGridHeight(displayCount, crossAxisCount, aspectRatio, spacing),
      child: GridView.builder(
        physics: const NeverScrollableScrollPhysics(),
        gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: crossAxisCount,
          childAspectRatio: aspectRatio,
          crossAxisSpacing: spacing,
          mainAxisSpacing: spacing,
        ),
        itemCount: displayCount,
        itemBuilder: (context, index) {
          final type = widget.propertyTypes[index];
          return FuturisticPropertyTypeCard(
            propertyType: type,
            isSelected: widget.selectedTypeId == type.id,
            onTap: () => _handleTypeSelection(type),
            animationDelay: Duration(milliseconds: index * 40),
            isCompact: widget.isCompact,
          );
        },
      ),
    );
  }

  Widget _buildUltraExpandButton() {
    return Center(
      child: AnimatedBuilder(
        animation: _pulseAnimation,
        builder: (context, child) {
          return GestureDetector(
            onTap: _toggleExpand,
            child: Container(
              margin: const EdgeInsets.only(top: 10),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(20),
                child: BackdropFilter(
                  filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
                  child: Container(
                    padding: EdgeInsets.symmetric(
                      horizontal: widget.isCompact ? 14 : 16,
                      vertical: widget.isCompact ? 6 : 8,
                    ),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [
                          Colors.white.withOpacity(0.05),
                          Colors.white.withOpacity(0.02),
                        ],
                      ),
                      borderRadius: BorderRadius.circular(20),
                      border: Border.all(
                        color: AppTheme.primaryBlue.withOpacity(
                          0.2 + (0.2 * _pulseAnimation.value),
                        ),
                        width: 0.5,
                      ),
                      boxShadow: [
                        BoxShadow(
                          color: AppTheme.primaryBlue.withOpacity(
                            0.1 * _pulseAnimation.value,
                          ),
                          blurRadius: 10 * _pulseAnimation.value,
                        ),
                      ],
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        AnimatedRotation(
                          turns: _isExpanded ? 0.5 : 0,
                          duration: const Duration(milliseconds: 300),
                          child: Icon(
                            Icons.expand_more_rounded,
                            color: AppTheme.primaryBlue.withOpacity(0.7),
                            size: widget.isCompact ? 16 : 18,
                          ),
                        ),
                        const SizedBox(width: 4),
                        Text(
                          _isExpanded 
                              ? 'إخفاء' 
                              : 'المزيد (${widget.propertyTypes.length - (_isExpanded ? widget.propertyTypes.length : math.min(widget.isCompact ? _compactDisplayCount : _initialDisplayCount, widget.propertyTypes.length))})',
                          style: AppTextStyles.caption.copyWith(
                            color: AppTheme.primaryBlue.withOpacity(0.8),
                            fontWeight: FontWeight.w500,
                            fontSize: widget.isCompact ? 11 : 12,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  double _calculateGridHeight(int itemCount, int crossAxisCount, 
                              double aspectRatio, double spacing) {
    final rows = (itemCount / crossAxisCount).ceil();
    final screenWidth = MediaQuery.of(context).size.width;
    final availableWidth = screenWidth - 24;
    final itemWidth = (availableWidth - (spacing * (crossAxisCount - 1))) / crossAxisCount;
    final itemHeight = itemWidth / aspectRatio;
    return rows * itemHeight + (rows - 1) * spacing;
  }

  void _toggleExpand() {
    HapticFeedback.lightImpact();
    setState(() {
      _isExpanded = !_isExpanded;
    });
    
    if (_isExpanded) {
      _expandController.forward();
    } else {
      _expandController.reverse();
    }
  }

  void _handleTypeSelection(PropertyType type) {
    HapticFeedback.lightImpact();
    if (widget.selectedTypeId == type.id) {
      widget.onTypeSelected(null);
    } else {
      widget.onTypeSelected(type.id);
    }
  }
}