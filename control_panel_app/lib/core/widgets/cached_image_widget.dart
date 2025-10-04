import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:bookn_cp_app/injection_container.dart';
import 'package:bookn_cp_app/services/local_storage_service.dart';
import '../constants/storage_constants.dart';
import '../theme/app_theme.dart';
import '../theme/app_dimensions.dart';
import '../utils/image_utils.dart';

class CachedImageWidget extends StatelessWidget {
  final String imageUrl;
  final double? width;
  final double? height;
  final BoxFit fit;
  final BorderRadius? borderRadius;
  final Widget? placeholder;
  final Widget? errorWidget;
  final bool showLoadingIndicator;
  final Color? backgroundColor;
  final List<BoxShadow>? boxShadow;
  final Gradient? gradient;
  final BlendMode? colorBlendMode;
  final Color? color;
  final bool removeContainer; // إضافة هذا المعامل للتحكم في Container

  const CachedImageWidget({
    super.key,
    required this.imageUrl,
    this.width,
    this.height,
    this.fit = BoxFit.cover,
    this.borderRadius,
    this.placeholder,
    this.errorWidget,
    this.showLoadingIndicator = true,
    this.backgroundColor,
    this.boxShadow,
    this.gradient,
    this.colorBlendMode,
    this.color,
    this.removeContainer = false, // القيمة الافتراضية false للحفاظ على التوافق
  });

  @override
  Widget build(BuildContext context) {
    // إذا كان removeContainer = true، لا نضع Container خارجي
    if (removeContainer) {
      return Stack(
        fit: StackFit.passthrough,
        children: [
          CachedNetworkImage(
            imageUrl: ImageUtils.resolveUrl(imageUrl),
            fit: fit,
            width: width,
            height: height,
            color: color,
            colorBlendMode: colorBlendMode,
            httpHeaders: _buildAuthHeaders(),
            placeholder: (context, url) => placeholder ?? _buildPlaceholder(),
            errorWidget: (context, url, error) =>
                errorWidget ?? _buildErrorWidget(),
          ),
          if (gradient != null)
            Positioned.fill(
              child: Container(
                decoration: BoxDecoration(
                  gradient: gradient,
                ),
              ),
            ),
        ],
      );
    }

    // الكود الأصلي للحفاظ على التوافق مع باقي التطبيق
    return Container(
      width: width,
      height: height,
      constraints: (width == null && height == null)
          ? const BoxConstraints(minHeight: 1, minWidth: 1)
          : null,
      decoration: BoxDecoration(
        color: backgroundColor ?? AppTheme.darkCard,
        borderRadius: borderRadius,
        boxShadow: boxShadow,
      ),
      child: ClipRRect(
        borderRadius: borderRadius ?? BorderRadius.zero,
        child: Stack(
          fit: StackFit.loose,
          children: [
            CachedNetworkImage(
              imageUrl: ImageUtils.resolveUrl(imageUrl),
              fit: fit,
              width: width,
              height: height,
              color: color,
              colorBlendMode: colorBlendMode,
              httpHeaders: _buildAuthHeaders(),
              placeholder: (context, url) => placeholder ?? _buildPlaceholder(),
              errorWidget: (context, url, error) =>
                  errorWidget ?? _buildErrorWidget(),
            ),
            if (gradient != null)
              Container(
                decoration: BoxDecoration(
                  gradient: gradient,
                ),
              ),
          ],
        ),
      ),
    );
  }

  Map<String, String>? _buildAuthHeaders() {
    try {
      final local = sl<LocalStorageService>();
      final token = local.getData(StorageConstants.accessToken) as String?;
      if (token != null && token.isNotEmpty) {
        return {'Authorization': 'Bearer $token'};
      }
    } catch (_) {}
    return null;
  }

  Widget _buildPlaceholder() {
    return Container(
      color: AppTheme.shimmer.withValues(alpha: 0.1),
      child: showLoadingIndicator
          ? Center(
              child: SizedBox(
                width: 24,
                height: 24,
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  valueColor: AlwaysStoppedAnimation<Color>(
                    AppTheme.primaryBlue.withValues(alpha: 0.5),
                  ),
                ),
              ),
            )
          : const SizedBox.shrink(),
    );
  }

  Widget _buildErrorWidget() {
    return Container(
      color: AppTheme.darkCard,
      child: Center(
        child: Icon(
          Icons.image_not_supported_outlined,
          color: AppTheme.textMuted.withValues(alpha: 0.5),
          size: AppDimensions.iconLarge,
        ),
      ),
    );
  }
}
