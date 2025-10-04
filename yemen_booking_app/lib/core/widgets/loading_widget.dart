import 'package:flutter/material.dart';
import 'package:shimmer/shimmer.dart';
import '../theme/app_theme.dart';
import '../theme/app_dimensions.dart';

class LoadingWidget extends StatelessWidget {
  final LoadingType type;
  final double? size;
  final Color? color;
  final String? message;
  final EdgeInsets? padding;

  const LoadingWidget({
    super.key,
    this.type = LoadingType.circular,
    this.size,
    this.color,
    this.message,
    this.padding,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final effectiveColor = color ?? theme.primaryColor;
    final effectiveSize = size ?? 40.0;

    Widget loadingIndicator;

    switch (type) {
      case LoadingType.circular:
        loadingIndicator = _buildCircularLoader(effectiveColor, effectiveSize);
        break;
      case LoadingType.linear:
        loadingIndicator = _buildLinearLoader(effectiveColor);
        break;
      case LoadingType.shimmer:
        loadingIndicator = _buildShimmerLoader();
        break;
      case LoadingType.pulse:
        loadingIndicator = _buildPulseLoader(effectiveColor, effectiveSize);
        break;
      case LoadingType.dots:
        loadingIndicator = _buildDotsLoader(effectiveColor);
        break;
    }

    if (message != null) {
      loadingIndicator = Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          loadingIndicator,
          const SizedBox(height: AppDimensions.spacingMd),
          Text(
            message!,
            style: theme.textTheme.bodyMedium?.copyWith(
              color: AppTheme.textMuted,
            ),
            textAlign: TextAlign.center,
          ),
        ],
      );
    }

    return Padding(
      padding: padding ?? EdgeInsets.zero,
      child: Center(child: loadingIndicator),
    );
  }

  Widget _buildCircularLoader(Color color, double size) {
    return SizedBox(
      width: size,
      height: size,
      child: CircularProgressIndicator(
        strokeWidth: 3.0,
        valueColor: AlwaysStoppedAnimation<Color>(color),
      ),
    );
  }

  Widget _buildLinearLoader(Color color) {
    return SizedBox(
      width: double.infinity,
      child: LinearProgressIndicator(
        valueColor: AlwaysStoppedAnimation<Color>(color),
        backgroundColor: color.withValues(alpha: 0.2),
      ),
    );
  }

  Widget _buildShimmerLoader() {
    return Shimmer.fromColors(
      baseColor: AppTheme.shimmer,
      highlightColor: AppTheme.shimmer.withValues(alpha: 0.5),
      child: Container(
        width: double.infinity,
        height: 200,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
      ),
    );
  }

  Widget _buildPulseLoader(Color color, double size) {
    return TweenAnimationBuilder<double>(
      tween: Tween(begin: 0.8, end: 1.2),
      duration: const Duration(milliseconds: 800),
      builder: (context, value, child) {
        return Transform.scale(
          scale: value,
          child: Container(
            width: size,
            height: size,
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.3),
              shape: BoxShape.circle,
            ),
            child: Center(
              child: Container(
                width: size * 0.6,
                height: size * 0.6,
                decoration: BoxDecoration(
                  color: color,
                  shape: BoxShape.circle,
                ),
              ),
            ),
          ),
        );
      },
      onEnd: () {},
    );
  }

  Widget _buildDotsLoader(Color color) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: List.generate(3, (index) {
        return TweenAnimationBuilder<double>(
          tween: Tween(begin: 0.0, end: 1.0),
          duration: Duration(milliseconds: 600 + (index * 200)),
          builder: (context, value, child) {
            return Container(
              margin: const EdgeInsets.symmetric(horizontal: 4),
              width: 12,
              height: 12,
              decoration: BoxDecoration(
                color: color.withValues(alpha: value),
                shape: BoxShape.circle,
              ),
            );
          },
        );
      }),
    );
  }
}

enum LoadingType {
  circular,
  linear,
  shimmer,
  pulse,
  dots,
}