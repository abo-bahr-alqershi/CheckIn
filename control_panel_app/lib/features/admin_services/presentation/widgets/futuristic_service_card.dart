import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/service.dart';
import '../../domain/entities/pricing_model.dart';
import '../utils/service_icons.dart';

/// 🎴 Premium Service Card - Professional Version
class FuturisticServiceCard extends StatefulWidget {
  final Service service;
  final VoidCallback? onTap;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;
  final bool isSelected;

  const FuturisticServiceCard({
    super.key,
    required this.service,
    this.onTap,
    this.onEdit,
    this.onDelete,
    this.isSelected = false,
  });

  @override
  State<FuturisticServiceCard> createState() => _FuturisticServiceCardState();
}

class _FuturisticServiceCardState extends State<FuturisticServiceCard>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  
  bool _isHovered = false;
  bool _isPressed = false;

  @override
  void initState() {
    super.initState();
    _initAnimations();
  }

  void _initAnimations() {
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 200),
      vsync: this,
    );
    
    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 1.02,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOutCubic,
    ));
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final screenWidth = size.width;
    
    // تحديد نوع الجهاز بدقة
    final isMobile = screenWidth < 400;
    final isSmallTablet = screenWidth >= 400 && screenWidth < 600;
    final isTablet = screenWidth >= 600 && screenWidth < 900;
    final isDesktop = screenWidth >= 900;
    
    final icon = ServiceIcons.getIconByName(widget.service.icon);
    
    return MouseRegion(
      onEnter: isDesktop ? (_) {
        setState(() => _isHovered = true);
        _animationController.forward();
      } : null,
      onExit: isDesktop ? (_) {
        setState(() => _isHovered = false);
        _animationController.reverse();
      } : null,
      child: GestureDetector(
        onTapDown: (_) => setState(() => _isPressed = true),
        onTapUp: (_) => setState(() => _isPressed = false),
        onTapCancel: () => setState(() => _isPressed = false),
        onTap: () {
          HapticFeedback.selectionClick();
          widget.onTap?.call();
        },
        child: AnimatedBuilder(
          animation: _scaleAnimation,
          builder: (context, child) {
            return Transform.scale(
              scale: _isPressed ? 0.98 : (_isHovered ? _scaleAnimation.value : 1.0),
              child: AnimatedContainer(
                duration: const Duration(milliseconds: 200),
                margin: EdgeInsets.symmetric(
                  vertical: isMobile ? 3 : 5,
                ),
                child: Material(
                  color: AppTheme.isDark 
                    ? AppTheme.darkCard.withOpacity(0.6)
                    : Colors.white,
                  borderRadius: BorderRadius.circular(
                    isMobile ? 10 : 12,
                  ),
                  elevation: _isHovered ? 3 : 1,
                  shadowColor: widget.isSelected
                    ? AppTheme.primaryBlue.withOpacity(0.15)
                    : Colors.black.withOpacity(0.08),
                  child: InkWell(
                    onTap: widget.onTap,
                    borderRadius: BorderRadius.circular(
                      isMobile ? 10 : 12,
                    ),
                    child: Container(
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(
                          isMobile ? 10 : 12,
                        ),
                        border: Border.all(
                          color: widget.isSelected
                            ? AppTheme.primaryBlue.withOpacity(0.25)
                            : AppTheme.darkBorder.withOpacity(0.08),
                          width: widget.isSelected ? 1.2 : 0.8,
                        ),
                      ),
                      child: _buildOptimizedLayout(
                        icon: icon,
                        isMobile: isMobile,
                        isSmallTablet: isSmallTablet,
                        isTablet: isTablet,
                        isDesktop: isDesktop,
                      ),
                    ),
                  ),
                ),
              ),
            );
          },
        ),
      ),
    );
  }

  Widget _buildOptimizedLayout({
    required IconData icon,
    required bool isMobile,
    required bool isSmallTablet,
    required bool isTablet,
    required bool isDesktop,
  }) {
    if (isMobile) {
      return _buildMobileLayout(icon);
    } else if (isSmallTablet) {
      return _buildSmallTabletLayout(icon);
    } else if (isTablet) {
      return _buildTabletLayout(icon);
    } else {
      return _buildDesktopLayout(icon);
    }
  }

  // تصميم محسّن للموبايل
  Widget _buildMobileLayout(IconData icon) {
    return Container(
      padding: const EdgeInsets.all(10),
      child: Row(
        children: [
          // Icon
          Container(
            width: 36,
            height: 36,
            decoration: BoxDecoration(
              color: AppTheme.primaryBlue.withOpacity(0.08),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Icon(
              icon,
              color: AppTheme.primaryBlue,
              size: 18,
            ),
          ),
          
          const SizedBox(width: 8),
          
          // Content
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                // Service Name
                Text(
                  widget.service.name,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                    color: AppTheme.textWhite,
                  ),
                ),
                
                const SizedBox(height: 2),
                
                // Property & Price في سطر واحد محسّن
                _buildCompactInfo(),
              ],
            ),
          ),
          
          // Actions
          if (widget.onEdit != null || widget.onDelete != null)
            _buildMobileActions(),
        ],
      ),
    );
  }

  // معلومات مضغوطة محسّنة لتجنب overflow
  Widget _buildCompactInfo() {
    return LayoutBuilder(
      builder: (context, constraints) {
        return Row(
          children: [
            // Property - مرن
            Flexible(
              flex: 3,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 4,
                  vertical: 1,
                ),
                decoration: BoxDecoration(
                  color: AppTheme.primaryBlue.withOpacity(0.06),
                  borderRadius: BorderRadius.circular(3),
                ),
                child: Text(
                  widget.service.propertyName,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    fontSize: 9,
                    color: AppTheme.primaryBlue,
                  ),
                ),
              ),
            ),
            
            const SizedBox(width: 6),
            
            // Price - مرن أيضاً
            Flexible(
              flex: 2,
              child: Row(
                mainAxisSize: MainAxisSize.max,
                children: [
                  Flexible(
                    child: Text(
                      '${widget.service.price.amount}',
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(
                        fontSize: 11,
                        color: AppTheme.success,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  const SizedBox(width: 2),
                  Flexible(
                    child: Text(
                      widget.service.price.currency,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(
                        fontSize: 8,
                        color: AppTheme.success.withOpacity(0.7),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        );
      },
    );
  }

  // تصميم للتابلت الصغير
  Widget _buildSmallTabletLayout(IconData icon) {
    return Container(
      padding: const EdgeInsets.all(12),
      child: Row(
        children: [
          // Icon
          Container(
            width: 42,
            height: 42,
            decoration: BoxDecoration(
              color: AppTheme.primaryBlue.withOpacity(0.1),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(
              icon,
              color: AppTheme.primaryBlue,
              size: 20,
            ),
          ),
          
          const SizedBox(width: 10),
          
          // Info
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  widget.service.name,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                    color: AppTheme.textWhite,
                  ),
                ),
                const SizedBox(height: 3),
                Row(
                  children: [
                    Flexible(
                      flex: 2,
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 6,
                          vertical: 1.5,
                        ),
                        decoration: BoxDecoration(
                          color: AppTheme.primaryBlue.withOpacity(0.08),
                          borderRadius: BorderRadius.circular(4),
                        ),
                        child: Text(
                          widget.service.propertyName,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: TextStyle(
                            fontSize: 10,
                            color: AppTheme.primaryBlue,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Flexible(
                      child: Row(
                        mainAxisSize: MainAxisSize.max,
                        children: [
                          Flexible(
                            child: Text(
                              '${widget.service.price.amount}',
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: TextStyle(
                                fontSize: 13,
                                color: AppTheme.success,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                          ),
                          const SizedBox(width: 3),
                          Flexible(
                            child: Text(
                              widget.service.price.currency,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: TextStyle(
                                fontSize: 10,
                                color: AppTheme.success.withOpacity(0.7),
                              ),
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
          
          // Actions
          if (widget.onEdit != null || widget.onDelete != null)
            _buildTabletActions(),
        ],
      ),
    );
  }

  // تصميم للتابلت
  Widget _buildTabletLayout(IconData icon) {
    return Container(
      padding: const EdgeInsets.all(14),
      child: Row(
        children: [
          // Icon
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  AppTheme.primaryBlue.withOpacity(0.15),
                  AppTheme.primaryBlue.withOpacity(0.08),
                ],
              ),
              borderRadius: BorderRadius.circular(11),
            ),
            child: Icon(
              icon,
              color: AppTheme.primaryBlue,
              size: 22,
            ),
          ),
          
          const SizedBox(width: 12),
          
          // Info
          Expanded(
            flex: 3,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  widget.service.name,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: AppTheme.textWhite,
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const SizedBox(height: 4),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: 2,
                  ),
                  decoration: BoxDecoration(
                    color: AppTheme.primaryBlue.withOpacity(0.08),
                    borderRadius: BorderRadius.circular(5),
                  ),
                  child: Text(
                    widget.service.propertyName,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: AppTextStyles.caption.copyWith(
                      color: AppTheme.primaryBlue,
                    ),
                  ),
                ),
              ],
            ),
          ),
          
          // Price
          Expanded(
            flex: 2,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              mainAxisSize: MainAxisSize.max,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  mainAxisSize: MainAxisSize.max,
                  children: [
                    Flexible(
                      child: Text(
                        '${widget.service.price.amount}',
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: AppTextStyles.bodyLarge.copyWith(
                          color: AppTheme.success,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                    const SizedBox(width: 4),
                    Flexible(
                      child: Text(
                        widget.service.price.currency,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: AppTextStyles.caption.copyWith(
                          color: AppTheme.success.withOpacity(0.7),
                        ),
                      ),
                    ),
                  ],
                ),
                Flexible(
                  child: Text(
                    widget.service.pricingModel.label,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: AppTextStyles.caption.copyWith(
                      color: AppTheme.textMuted,
                      fontSize: 10,
                    ),
                  ),
                ),
              ],
            ),
          ),
          
          // Actions
          if (widget.onEdit != null || widget.onDelete != null) ...[
            const SizedBox(width: 8),
            _buildDesktopActions(),
          ],
        ],
      ),
    );
  }

  // تصميم للديسكتوب
  Widget _buildDesktopLayout(IconData icon) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Row(
        children: [
          // Icon with gradient
          Container(
            width: 52,
            height: 52,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  AppTheme.primaryBlue.withOpacity(0.15),
                  AppTheme.primaryPurple.withOpacity(0.08),
                ],
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
              ),
              borderRadius: BorderRadius.circular(12),
              border: _isHovered
                ? Border.all(
                    color: AppTheme.primaryBlue.withOpacity(0.2),
                    width: 1,
                  )
                : null,
            ),
            child: Icon(
              icon,
              color: AppTheme.primaryBlue,
              size: 24,
            ),
          ),
          
          const SizedBox(width: 14),
          
          // Info Section
          Expanded(
            flex: 3,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  widget.service.name,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: AppTextStyles.bodyLarge.copyWith(
                    color: AppTheme.textWhite,
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const SizedBox(height: 5),
                Row(
                  children: [
                    Container(
                      constraints: const BoxConstraints(maxWidth: 160),
                      padding: const EdgeInsets.symmetric(
                        horizontal: 10,
                        vertical: 3,
                      ),
                      decoration: BoxDecoration(
                        color: AppTheme.primaryBlue.withOpacity(0.08),
                        borderRadius: BorderRadius.circular(6),
                        border: Border.all(
                          color: AppTheme.primaryBlue.withOpacity(0.15),
                          width: 0.5,
                        ),
                      ),
                      child: Text(
                        widget.service.propertyName,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: AppTextStyles.caption.copyWith(
                          color: AppTheme.primaryBlue,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ),
                    const SizedBox(width: 10),
                    if (_isHovered)
                      Flexible(
                        child: Text(
                          'Icons.${widget.service.icon}',
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: AppTextStyles.caption.copyWith(
                            color: AppTheme.textMuted.withOpacity(0.5),
                            fontFamily: 'monospace',
                            fontSize: 10,
                          ),
                        ),
                      ),
                  ],
                ),
              ],
            ),
          ),
          
          // Price Section
          Expanded(
            flex: 2,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              mainAxisSize: MainAxisSize.max,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  mainAxisSize: MainAxisSize.max,
                  children: [
                    Flexible(
                      child: Text(
                        '${widget.service.price.amount}',
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: AppTextStyles.heading3.copyWith(
                          color: AppTheme.success,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                    const SizedBox(width: 5),
                    Flexible(
                      child: Text(
                        widget.service.price.currency,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: AppTextStyles.bodySmall.copyWith(
                          color: AppTheme.success.withOpacity(0.8),
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 2),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: 2,
                  ),
                  decoration: BoxDecoration(
                    color: AppTheme.success.withOpacity(0.08),
                    borderRadius: BorderRadius.circular(4),
                  ),
                  child: Text(
                    widget.service.pricingModel.label,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: AppTextStyles.caption.copyWith(
                      color: AppTheme.success.withOpacity(0.9),
                      fontSize: 10,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ),
              ],
            ),
          ),
          
          // Actions
          if (widget.onEdit != null || widget.onDelete != null) ...[
            const SizedBox(width: 12),
            _buildDesktopActions(),
          ],
        ],
      ),
    );
  }

  // أزرار محسّنة للموبايل
  Widget _buildMobileActions() {
    return SizedBox(
      width: 24,
      height: 24,
      child: PopupMenuButton<String>(
        padding: EdgeInsets.zero,
        iconSize: 16,
        icon: Icon(
          Icons.more_vert_rounded,
          color: AppTheme.textMuted.withOpacity(0.7),
        ),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        color: AppTheme.isDark ? AppTheme.darkCard : Colors.white,
        elevation: 3,
        onSelected: _handleAction,
        itemBuilder: (context) => _buildMenuItems(isCompact: true),
      ),
    );
  }

  // أزرار للتابلت
  Widget _buildTabletActions() {
    return PopupMenuButton<String>(
      iconSize: 18,
      icon: Icon(
        Icons.more_vert_rounded,
        color: AppTheme.textMuted,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(10),
      ),
      color: AppTheme.isDark ? AppTheme.darkCard : Colors.white,
      elevation: 4,
      onSelected: _handleAction,
      itemBuilder: (context) => _buildMenuItems(isCompact: false),
    );
  }

  // أزرار للديسكتوب
  Widget _buildDesktopActions() {
    return PopupMenuButton<String>(
      icon: Icon(
        Icons.more_vert_rounded,
        color: AppTheme.textMuted,
        size: 20,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
      ),
      color: AppTheme.isDark ? AppTheme.darkCard : Colors.white,
      elevation: 6,
      onSelected: _handleAction,
      itemBuilder: (context) => _buildMenuItems(isCompact: false),
    );
  }

  // بناء عناصر القائمة
  List<PopupMenuEntry<String>> _buildMenuItems({required bool isCompact}) {
    return [
      if (widget.onEdit != null)
        PopupMenuItem(
          value: 'edit',
          height: isCompact ? 32 : 40,
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                Icons.edit_outlined,
                color: AppTheme.textMuted,
                size: isCompact ? 14 : 16,
              ),
              SizedBox(width: isCompact ? 6 : 10),
              Text(
                'تعديل',
                style: (isCompact ? AppTextStyles.caption : AppTextStyles.bodySmall).copyWith(
                  color: AppTheme.textWhite,
                ),
              ),
            ],
          ),
        ),
      if (widget.onDelete != null)
        PopupMenuItem(
          value: 'delete',
          height: isCompact ? 32 : 40,
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                Icons.delete_outline,
                color: AppTheme.error,
                size: isCompact ? 14 : 16,
              ),
              SizedBox(width: isCompact ? 6 : 10),
              Text(
                'حذف',
                style: (isCompact ? AppTextStyles.caption : AppTextStyles.bodySmall).copyWith(
                  color: AppTheme.error,
                ),
              ),
            ],
          ),
        ),
    ];
  }

  // معالج الأحداث
  void _handleAction(String value) {
    HapticFeedback.selectionClick();
    switch (value) {
      case 'edit':
        widget.onEdit?.call();
        break;
      case 'delete':
        widget.onDelete?.call();
        break;
    }
  }
}