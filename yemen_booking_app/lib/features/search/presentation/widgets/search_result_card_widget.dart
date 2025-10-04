// import 'package:flutter/material.dart';
// import '../../../../core/utils/image_utils.dart';
// import 'dart:ui';
// import '../../../../core/theme/app_theme.dart';
// import '../../../../core/theme/app_text_styles.dart';
// import '../../../../core/widgets/cached_image_widget.dart';
// import '../../domain/entities/search_result.dart';

// enum CardDisplayType { list, grid, compact }

// class SearchResultCardWidget extends StatefulWidget {
//   final SearchResult result;
//   final VoidCallback? onTap;
//   final VoidCallback? onFavoriteToggle;
//   final CardDisplayType displayType;
//   final bool showDistance;

//   const SearchResultCardWidget({
//     super.key,
//     required this.result,
//     this.onTap,
//     this.onFavoriteToggle,
//     this.displayType = CardDisplayType.list,
//     this.showDistance = true,
//   });

//   @override
//   State<SearchResultCardWidget> createState() => _SearchResultCardWidgetState();
// }

// class _SearchResultCardWidgetState extends State<SearchResultCardWidget>
//     with TickerProviderStateMixin {
//   late AnimationController _scaleController;
//   late AnimationController _shimmerController;
//   late AnimationController _heartController;
//   late Animation<double> _scaleAnimation;
//   late Animation<double> _shimmerAnimation;
//   late Animation<double> _heartAnimation;
  
//   bool _isPressed = false;
//   bool _isFavorite = false;

//   @override
//   void initState() {
//     super.initState();
    
//     _scaleController = AnimationController(
//       duration: const Duration(milliseconds: 150),
//       vsync: this,
//     );
    
//     _shimmerController = AnimationController(
//       duration: const Duration(seconds: 2),
//       vsync: this,
//     )..repeat();
    
//     _heartController = AnimationController(
//       duration: const Duration(milliseconds: 300),
//       vsync: this,
//     );
    
//     _scaleAnimation = Tween<double>(
//       begin: 1.0,
//       end: 0.95,
//     ).animate(CurvedAnimation(
//       parent: _scaleController,
//       curve: Curves.easeInOut,
//     ));
    
//     _shimmerAnimation = Tween<double>(
//       begin: 0.0,
//       end: 1.0,
//     ).animate(_shimmerController);
    
//     _heartAnimation = Tween<double>(
//       begin: 1.0,
//       end: 1.3,
//     ).animate(CurvedAnimation(
//       parent: _heartController,
//       curve: Curves.elasticOut,
//     ));
//   }

//   @override
//   void dispose() {
//     _scaleController.dispose();
//     _shimmerController.dispose();
//     _heartController.dispose();
//     super.dispose();
//   }

//   @override
//   Widget build(BuildContext context) {
//     switch (widget.displayType) {
//       case CardDisplayType.list:
//         return _buildListCard(context);
//       case CardDisplayType.grid:
//         return _buildGridCard(context);
//       case CardDisplayType.compact:
//         return _buildCompactCard(context);
//     }
//   }

//   Widget _buildListCard(BuildContext context) {
//     return AnimatedBuilder(
//       animation: _scaleAnimation,
//       builder: (context, child) {
//         return Transform.scale(
//           scale: _scaleAnimation.value,
//           child: GestureDetector(
//             onTapDown: (_) {
//               setState(() => _isPressed = true);
//               _scaleController.forward();
//             },
//             onTapUp: (_) {
//               setState(() => _isPressed = false);
//               _scaleController.reverse();
//               widget.onTap?.call();
//             },
//             onTapCancel: () {
//               setState(() => _isPressed = false);
//               _scaleController.reverse();
//             },
//             child: Container(
//               decoration: BoxDecoration(
//                 gradient: LinearGradient(
//                   begin: Alignment.topLeft,
//                   end: Alignment.bottomRight,
//                   colors: [
//                     AppTheme.darkCard.withOpacity(0.9),
//                     AppTheme.darkCard.withOpacity(0.7),
//                   ],
//                 ),
//                 borderRadius: BorderRadius.circular(24),
//                 border: Border.all(
//                   color: _isPressed
//                       ? AppTheme.primaryBlue.withOpacity(0.5)
//                       : AppTheme.darkBorder.withOpacity(0.3),
//                   width: _isPressed ? 2 : 1,
//                 ),
//                 boxShadow: [
//                   BoxShadow(
//                     color: _isPressed
//                         ? AppTheme.primaryBlue.withOpacity(0.3)
//                         : AppTheme.shadowDark.withOpacity(0.5),
//                     blurRadius: _isPressed ? 30 : 20,
//                     offset: const Offset(0, 10),
//                   ),
//                 ],
//               ),
//               child: ClipRRect(
//                 borderRadius: BorderRadius.circular(24),
//                 child: BackdropFilter(
//                   filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
//                   child: Column(
//                     crossAxisAlignment: CrossAxisAlignment.start,
//                     children: [
//                       _buildImageSection(height: 200),
//                       Padding(
//                         padding: const EdgeInsets.all(20),
//                         child: Column(
//                           crossAxisAlignment: CrossAxisAlignment.start,
//                           children: [
//                             _buildHeader(),
//                             const SizedBox(height: 12),
//                             _buildLocation(),
//                             const SizedBox(height: 12),
//                             _buildRatingAndReviews(),
//                             const SizedBox(height: 16),
//                             _buildAmenities(),
//                             const SizedBox(height: 20),
//                             _buildFooter(),
//                           ],
//                         ),
//                       ),
//                     ],
//                   ),
//                 ),
//               ),
//             ),
//           ),
//         );
//       },
//     );
//   }

//   Widget _buildGridCard(BuildContext context) {
//     return AnimatedBuilder(
//       animation: _scaleAnimation,
//       builder: (context, child) {
//         return Transform.scale(
//           scale: _scaleAnimation.value,
//           child: GestureDetector(
//             onTapDown: (_) {
//               setState(() => _isPressed = true);
//               _scaleController.forward();
//             },
//             onTapUp: (_) {
//               setState(() => _isPressed = false);
//               _scaleController.reverse();
//               widget.onTap?.call();
//             },
//             onTapCancel: () {
//               setState(() => _isPressed = false);
//               _scaleController.reverse();
//             },
//             child: Container(
//               decoration: BoxDecoration(
//                 gradient: LinearGradient(
//                   begin: Alignment.topLeft,
//                   end: Alignment.bottomRight,
//                   colors: [
//                     AppTheme.darkCard.withOpacity(0.9),
//                     AppTheme.darkCard.withOpacity(0.7),
//                   ],
//                 ),
//                 borderRadius: BorderRadius.circular(20),
//                 border: Border.all(
//                   color: _isPressed
//                       ? AppTheme.primaryBlue.withOpacity(0.5)
//                       : AppTheme.darkBorder.withOpacity(0.3),
//                   width: _isPressed ? 2 : 1,
//                 ),
//                 boxShadow: [
//                   BoxShadow(
//                     color: _isPressed
//                         ? AppTheme.primaryBlue.withOpacity(0.3)
//                         : AppTheme.shadowDark.withOpacity(0.3),
//                     blurRadius: _isPressed ? 20 : 15,
//                     offset: const Offset(0, 5),
//                   ),
//                 ],
//               ),
//               child: ClipRRect(
//                 borderRadius: BorderRadius.circular(20),
//                 child: Column(
//                   crossAxisAlignment: CrossAxisAlignment.start,
//                   children: [
//                     Expanded(
//                       flex: 3,
//                       child: _buildImageSection(),
//                     ),
//                     Expanded(
//                       flex: 2,
//                       child: Padding(
//                         padding: const EdgeInsets.all(12),
//                         child: Column(
//                           crossAxisAlignment: CrossAxisAlignment.start,
//                           mainAxisSize: MainAxisSize.max,
//                           mainAxisAlignment: MainAxisAlignment.start,
//                           children: [
//                             Flexible(
//                               child: Text(
//                                 widget.result.name,
//                                 style: AppTextStyles.bodyMedium.copyWith(
//                                   fontWeight: FontWeight.bold,
//                                 ),
//                                 maxLines: 2,
//                                 overflow: TextOverflow.ellipsis,
//                               ),
//                             ),
//                             const SizedBox(height: 4),
//                             Flexible(
//                               child: Row(
//                                 children: [
//                                   Icon(
//                                     Icons.location_on_outlined,
//                                     size: 14,
//                                     color: AppTheme.textMuted.withOpacity(0.7),
//                                   ),
//                                   const SizedBox(width: 4),
//                                   Expanded(
//                                     child: Text(
//                                       widget.result.city,
//                                       style: AppTextStyles.caption.copyWith(
//                                         color: AppTheme.textMuted,
//                                       ),
//                                       maxLines: 1,
//                                       overflow: TextOverflow.ellipsis,
//                                     ),
//                                   ),
//                                 ],
//                               ),
//                             ),
//                             const SizedBox(height: 4),
//                             Row(
//                               children: [
//                                 const Icon(
//                                   Icons.star_rounded,
//                                   size: 16,
//                                   color: AppTheme.warning,
//                                 ),
//                                 const SizedBox(width: 4),
//                                 Text(
//                                   widget.result.averageRating.toStringAsFixed(1),
//                                   style: AppTextStyles.caption.copyWith(
//                                     fontWeight: FontWeight.bold,
//                                   ),
//                                 ),
//                               ],
//                             ),
//                             const SizedBox(height: 4),
//                             Row(
//                               children: [
//                                 Text(
//                                   widget.result.discountedPrice.toStringAsFixed(0),
//                                   style: AppTextStyles.bodyLarge.copyWith(
//                                     color: AppTheme.primaryBlue,
//                                     fontWeight: FontWeight.bold,
//                                   ),
//                                 ),
//                                 const SizedBox(width: 4),
//                                 Text(
//                                   widget.result.currency,
//                                   style: AppTextStyles.caption.copyWith(
//                                     color: AppTheme.textMuted,
//                                   ),
//                                 ),
//                               ],
//                             ),
//                           ],
//                         ),
//                       ),
//                     ),
//                   ],
//                 ),
//               ),
//             ),
//           ),
//         );
//       },
//     );
//   }

//   Widget _buildCompactCard(BuildContext context) {
//     return AnimatedBuilder(
//       animation: _scaleAnimation,
//       builder: (context, child) {
//         return Transform.scale(
//           scale: _scaleAnimation.value,
//           child: GestureDetector(
//             onTapDown: (_) {
//               setState(() => _isPressed = true);
//               _scaleController.forward();
//             },
//             onTapUp: (_) {
//               setState(() => _isPressed = false);
//               _scaleController.reverse();
//               widget.onTap?.call();
//             },
//             onTapCancel: () {
//               setState(() => _isPressed = false);
//               _scaleController.reverse();
//             },
//             child: Container(
//               height: 120,
//               decoration: BoxDecoration(
//                 gradient: LinearGradient(
//                   colors: [
//                     AppTheme.darkCard.withOpacity(0.9),
//                     AppTheme.darkCard.withOpacity(0.7),
//                   ],
//                 ),
//                 borderRadius: BorderRadius.circular(16),
//                 border: Border.all(
//                   color: _isPressed
//                       ? AppTheme.primaryBlue.withOpacity(0.5)
//                       : AppTheme.darkBorder.withOpacity(0.3),
//                   width: _isPressed ? 2 : 1,
//                 ),
//                 boxShadow: [
//                   BoxShadow(
//                     color: _isPressed
//                         ? AppTheme.primaryBlue.withOpacity(0.2)
//                         : AppTheme.shadowDark.withOpacity(0.2),
//                     blurRadius: 15,
//                     offset: const Offset(0, 5),
//                   ),
//                 ],
//               ),
//               child: Row(
//                 children: [
//                   Container(
//                     width: 120,
//                     decoration: BoxDecoration(
//                       borderRadius: const BorderRadius.horizontal(
//                         right: Radius.circular(16),
//                       ),
//                       image: DecorationImage(
//                         image: NetworkImage(ImageUtils.resolveUrl(widget.result.mainImageUrl ?? '')),
//                         fit: BoxFit.cover,
//                       ),
//                     ),
//                     child: Stack(
//                       children: [
//                         Positioned(
//                           top: 8,
//                           right: 8,
//                           child: _buildFavoriteButton(size: 28),
//                         ),
//                       ],
//                     ),
//                   ),
//                   Expanded(
//                     child: Padding(
//                       padding: const EdgeInsets.all(12),
//                       child: Column(
//                         crossAxisAlignment: CrossAxisAlignment.start,
//                         mainAxisAlignment: MainAxisAlignment.spaceBetween,
//                         children: [
//                           Text(
//                             widget.result.name,
//                             style: AppTextStyles.bodyMedium.copyWith(
//                               fontWeight: FontWeight.bold,
//                               color: AppTheme.textLight,
//                             ),
//                             maxLines: 2,
//                             overflow: TextOverflow.ellipsis,
//                           ),
//                           Row(
//                             children: [
//                               Icon(
//                                 Icons.location_on_outlined,
//                                 size: 14,
//                                 color: AppTheme.textMuted.withOpacity(0.7),
//                               ),
//                               const SizedBox(width: 4),
//                               Expanded(
//                                 child: Text(
//                                   widget.result.city,
//                                   style: AppTextStyles.caption.copyWith(
//                                     color: AppTheme.textMuted,
//                                   ),
//                                   maxLines: 1,
//                                   overflow: TextOverflow.ellipsis,
//                                 ),
//                               ),
//                             ],
//                           ),
//                           Row(
//                             children: [
//                               const Icon(
//                                 Icons.star_rounded,
//                                 size: 16,
//                                 color: AppTheme.warning,
//                               ),
//                               const SizedBox(width: 4),
//                               Text(
//                                 widget.result.averageRating.toStringAsFixed(1),
//                                 style: AppTextStyles.caption.copyWith(
//                                   fontWeight: FontWeight.bold,
//                                   color: AppTheme.textMuted,
//                                 ),
//                               ),
//                               const Spacer(),
//                               ShaderMask(
//                                 shaderCallback: (bounds) => 
//                                     AppTheme.primaryGradient.createShader(bounds),
//                                 child: Text(
//                                   '${widget.result.discountedPrice.toStringAsFixed(0)} ${widget.result.currency}',
//                                   style: AppTextStyles.bodyMedium.copyWith(
//                                     fontWeight: FontWeight.bold,
//                                     color: Colors.white,
//                                   ),
//                                 ),
//                               ),
//                             ],
//                           ),
//                         ],
//                       ),
//                     ),
//                   ),
//                 ],
//               ),
//             ),
//           ),
//         );
//       },
//     );
//   }

//   Widget _buildImageSection({double? height}) {
//     return Stack(
//       children: [
//         // Main Image with Shimmer
//         AnimatedBuilder(
//           animation: _shimmerAnimation,
//           builder: (context, child) {
//             return Stack(
//               children: [
//                 CachedImageWidget(
//                   imageUrl: widget.result.mainImageUrl ?? '',
//                   height: height,
//                   width: double.infinity,
//                   fit: BoxFit.cover,
//                   borderRadius: BorderRadius.vertical(
//                     top: Radius.circular(
//                       widget.displayType == CardDisplayType.grid ? 20 : 24,
//                     ),
//                   ),
//                 ),
                
//                 // Shimmer overlay
//                 if (widget.result.isFeatured)
//                   Positioned.fill(
//                     child: Container(
//                       decoration: BoxDecoration(
//                         gradient: LinearGradient(
//                           begin: Alignment(-1 + (_shimmerAnimation.value * 2), -1),
//                           end: Alignment(1 + (_shimmerAnimation.value * 2), 1),
//                           colors: [
//                             Colors.transparent,
//                             AppTheme.primaryBlue.withOpacity(0.1),
//                             AppTheme.primaryPurple.withOpacity(0.1),
//                             Colors.transparent,
//                           ],
//                         ),
//                       ),
//                     ),
//                   ),
//               ],
//             );
//           },
//         ),
        
//         // Gradient Overlay
//         Positioned.fill(
//           child: Container(
//             decoration: BoxDecoration(
//               gradient: LinearGradient(
//                 begin: Alignment.topCenter,
//                 end: Alignment.bottomCenter,
//                 colors: [
//                   Colors.transparent,
//                   AppTheme.darkBackground.withOpacity(0.3),
//                 ],
//               ),
//             ),
//           ),
//         ),
        
//         // Top Badges
//         Positioned(
//           top: 12,
//           left: 12,
//           right: 12,
//           child: Row(
//             mainAxisAlignment: MainAxisAlignment.spaceBetween,
//             children: [
//               Row(
//                 children: [
//                   if (widget.result.isFeatured)
//                     _buildBadge(
//                       'مميز',
//                       AppTheme.primaryViolet,
//                       Icons.star_rounded,
//                     ),
//                   if (widget.result.isRecommended) ...[
//                     const SizedBox(width: 8),
//                     _buildBadge(
//                       'موصى به',
//                       AppTheme.success,
//                       Icons.thumb_up_rounded,
//                     ),
//                   ],
//                 ],
//               ),
//               _buildFavoriteButton(),
//             ],
//           ),
//         ),
        
//         // Discount Badge
//         if (widget.result.minPrice != widget.result.discountedPrice)
//           Positioned(
//             bottom: 12,
//             right: 12,
//             child: Container(
//               padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
//               decoration: BoxDecoration(
//                 gradient: const LinearGradient(
//                   colors: [
//                     AppTheme.error,
//                     AppTheme.warning,
//                   ],
//                 ),
//                 borderRadius: BorderRadius.circular(20),
//                 boxShadow: [
//                   BoxShadow(
//                     color: AppTheme.error.withOpacity(0.5),
//                     blurRadius: 10,
//                     spreadRadius: 2,
//                   ),
//                 ],
//               ),
//               child: Row(
//                 mainAxisSize: MainAxisSize.min,
//                 children: [
//                   const Icon(
//                     Icons.local_offer_rounded,
//                     size: 14,
//                     color: Colors.white,
//                   ),
//                   const SizedBox(width: 4),
//                   Text(
//                     '${((1 - widget.result.discountedPrice / widget.result.minPrice) * 100).toStringAsFixed(0)}% خصم',
//                     style: AppTextStyles.caption.copyWith(
//                       color: Colors.white,
//                       fontWeight: FontWeight.bold,
//                     ),
//                   ),
//                 ],
//               ),
//             ),
//           ),
//       ],
//     );
//   }

//   Widget _buildBadge(String label, Color color, IconData icon) {
//     return Container(
//       padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
//       decoration: BoxDecoration(
//         gradient: LinearGradient(
//           colors: [
//             color,
//             color.withOpacity(0.7),
//           ],
//         ),
//         borderRadius: BorderRadius.circular(20),
//         boxShadow: [
//           BoxShadow(
//             color: color.withOpacity(0.5),
//             blurRadius: 8,
//             spreadRadius: 1,
//           ),
//         ],
//       ),
//       child: Row(
//         mainAxisSize: MainAxisSize.min,
//         children: [
//           Icon(
//             icon,
//             size: 12,
//             color: Colors.white,
//           ),
//           const SizedBox(width: 4),
//           Text(
//             label,
//             style: AppTextStyles.overline.copyWith(
//               color: Colors.white,
//               fontWeight: FontWeight.bold,
//             ),
//           ),
//         ],
//       ),
//     );
//   }

//   Widget _buildFavoriteButton({double size = 36}) {
//     return AnimatedBuilder(
//       animation: _heartAnimation,
//       builder: (context, child) {
//         return GestureDetector(
//           onTap: () {
//             setState(() {
//               _isFavorite = !_isFavorite;
//             });
//             _heartController.forward().then((_) {
//               _heartController.reverse();
//             });
//             widget.onFavoriteToggle?.call();
//           },
//           child: Container(
//             width: size,
//             height: size,
//             decoration: BoxDecoration(
//               gradient: LinearGradient(
//                 colors: [
//                   Colors.white.withOpacity(0.9),
//                   Colors.white.withOpacity(0.7),
//                 ],
//               ),
//               shape: BoxShape.circle,
//               boxShadow: [
//                 BoxShadow(
//                   color: AppTheme.shadowDark.withOpacity(0.3),
//                   blurRadius: 8,
//                   spreadRadius: 1,
//                 ),
//               ],
//             ),
//             child: Transform.scale(
//               scale: _heartAnimation.value,
//               child: Icon(
//                 _isFavorite ? Icons.favorite_rounded : Icons.favorite_border_rounded,
//                 color: _isFavorite ? AppTheme.error : AppTheme.textDark,
//                 size: size * 0.5,
//               ),
//             ),
//           ),
//         );
//       },
//     );
//   }

//   Widget _buildHeader() {
//     return Row(
//       children: [
//         Expanded(
//           child: Column(
//             crossAxisAlignment: CrossAxisAlignment.start,
//             children: [
//               Container(
//                 padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
//                 decoration: BoxDecoration(
//                   gradient: LinearGradient(
//                     colors: [
//                       AppTheme.primaryBlue.withOpacity(0.2),
//                       AppTheme.primaryPurple.withOpacity(0.1),
//                     ],
//                   ),
//                   borderRadius: BorderRadius.circular(8),
//                 ),
//                 child: Text(
//                   widget.result.propertyType,
//                   style: AppTextStyles.caption.copyWith(
//                     color: AppTheme.primaryBlue,
//                     fontWeight: FontWeight.w600,
//                   ),
//                 ),
//               ),
//               const SizedBox(height: 8),
//               Text(
//                 widget.result.name,
//                 style: AppTextStyles.heading2.copyWith(
//                   fontWeight: FontWeight.bold,
//                 ),
//                 maxLines: 2,
//                 overflow: TextOverflow.ellipsis,
//               ),
//             ],
//           ),
//         ),
//         if (widget.result.starRating > 0)
//           Container(
//             padding: const EdgeInsets.all(12),
//             decoration: BoxDecoration(
//               gradient: LinearGradient(
//                 colors: [
//                   AppTheme.warning.withOpacity(0.2),
//                   AppTheme.warning.withOpacity(0.1),
//                 ],
//               ),
//               borderRadius: BorderRadius.circular(12),
//             ),
//             child: Row(
//               children: List.generate(
//                 widget.result.starRating,
//                 (index) => const Icon(
//                   Icons.star_rounded,
//                   size: 16,
//                   color: AppTheme.warning,
//                 ),
//               ),
//             ),
//           ),
//       ],
//     );
//   }

//   Widget _buildLocation() {
//     return Row(
//       children: [
//         Container(
//           padding: const EdgeInsets.all(6),
//           decoration: BoxDecoration(
//             gradient: LinearGradient(
//               colors: [
//                 AppTheme.info.withOpacity(0.2),
//                 AppTheme.info.withOpacity(0.1),
//               ],
//             ),
//             borderRadius: BorderRadius.circular(8),
//           ),
//           child: const Icon(
//             Icons.location_on_rounded,
//             size: 14,
//             color: AppTheme.info,
//           ),
//         ),
//         const SizedBox(width: 8),
//         Expanded(
//           child: Text(
//             '${widget.result.address}, ${widget.result.city}',
//             style: AppTextStyles.bodySmall.copyWith(
//               color: AppTheme.textLight,
//             ),
//             maxLines: 1,
//             overflow: TextOverflow.ellipsis,
//           ),
//         ),
//         if (widget.showDistance && widget.result.distanceKm != null) ...[
//           const SizedBox(width: 8),
//           Container(
//             padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
//             decoration: BoxDecoration(
//               gradient: LinearGradient(
//                 colors: [
//                   AppTheme.info.withOpacity(0.2),
//                   AppTheme.info.withOpacity(0.1),
//                 ],
//               ),
//               borderRadius: BorderRadius.circular(8),
//             ),
//             child: Row(
//               mainAxisSize: MainAxisSize.min,
//               children: [
//                 const Icon(
//                   Icons.near_me_rounded,
//                   size: 12,
//                   color: AppTheme.info,
//                 ),
//                 const SizedBox(width: 4),
//                 Text(
//                   '${widget.result.distanceKm!.toStringAsFixed(1)} كم',
//                   style: AppTextStyles.caption.copyWith(
//                     color: AppTheme.info,
//                     fontWeight: FontWeight.bold,
//                   ),
//                 ),
//               ],
//             ),
//           ),
//         ],
//       ],
//     );
//   }

//   Widget _buildRatingAndReviews() {
//     return Row(
//       children: [
//         Container(
//           padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
//           decoration: BoxDecoration(
//             gradient: LinearGradient(
//               colors: [
//                 AppTheme.warning.withOpacity(0.2),
//                 AppTheme.warning.withOpacity(0.1),
//               ],
//             ),
//             borderRadius: BorderRadius.circular(12),
//           ),
//           child: Row(
//             children: [
//               const Icon(
//                 Icons.star_rounded,
//                 size: 18,
//                 color: AppTheme.warning,
//               ),
//               const SizedBox(width: 6),
//               Text(
//                 widget.result.averageRating.toStringAsFixed(1),
//                 style: AppTextStyles.bodyMedium.copyWith(
//                   fontWeight: FontWeight.bold,
//                   color: AppTheme.warning,
//                 ),
//               ),
//               const SizedBox(width: 6),
//               Text(
//                 '(${widget.result.reviewsCount})',
//                 style: AppTextStyles.caption.copyWith(
//                   color: AppTheme.textMuted,
//                 ),
//               ),
//             ],
//           ),
//         ),
//         const Spacer(),
//         if (widget.result.isAvailable)
//           Container(
//             padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
//             decoration: BoxDecoration(
//               gradient: LinearGradient(
//                 colors: [
//                   AppTheme.success.withOpacity(0.2),
//                   AppTheme.success.withOpacity(0.1),
//                 ],
//               ),
//               borderRadius: BorderRadius.circular(12),
//             ),
//             child: Row(
//               mainAxisSize: MainAxisSize.min,
//               children: [
//                 Container(
//                   width: 6,
//                   height: 6,
//                   decoration: const BoxDecoration(
//                     color: AppTheme.success,
//                     shape: BoxShape.circle,
//                   ),
//                 ),
//                 const SizedBox(width: 6),
//                 Text(
//                   'متاح',
//                   style: AppTextStyles.caption.copyWith(
//                     color: AppTheme.success,
//                     fontWeight: FontWeight.bold,
//                   ),
//                 ),
//               ],
//             ),
//           )
//         else
//           Container(
//             padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
//             decoration: BoxDecoration(
//               gradient: LinearGradient(
//                 colors: [
//                   AppTheme.error.withOpacity(0.2),
//                   AppTheme.error.withOpacity(0.1),
//                 ],
//               ),
//               borderRadius: BorderRadius.circular(12),
//             ),
//             child: Row(
//               mainAxisSize: MainAxisSize.min,
//               children: [
//                 Container(
//                   width: 6,
//                   height: 6,
//                   decoration: const BoxDecoration(
//                     color: AppTheme.error,
//                     shape: BoxShape.circle,
//                   ),
//                 ),
//                 const SizedBox(width: 6),
//                 Text(
//                   'محجوز',
//                   style: AppTextStyles.caption.copyWith(
//                     color: AppTheme.error,
//                     fontWeight: FontWeight.bold,
//                   ),
//                 ),
//               ],
//             ),
//           ),
//       ],
//     );
//   }

//   Widget _buildAmenities() {
//     if (widget.result.mainAmenities.isEmpty) {
//       return const SizedBox.shrink();
//     }

//     return Wrap(
//       spacing: 8,
//       runSpacing: 8,
//       children: widget.result.mainAmenities.take(4).map((amenity) {
//         return Container(
//           padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
//           decoration: BoxDecoration(
//             gradient: LinearGradient(
//               colors: [
//                 AppTheme.darkCard.withOpacity(0.8),
//                 AppTheme.darkCard.withOpacity(0.5),
//               ],
//             ),
//             borderRadius: BorderRadius.circular(10),
//             border: Border.all(
//               color: AppTheme.darkBorder.withOpacity(0.3),
//               width: 1,
//             ),
//           ),
//           child: Row(
//             mainAxisSize: MainAxisSize.min,
//             children: [
//               Icon(
//                 _getAmenityIcon(amenity),
//                 size: 14,
//                 color: AppTheme.textMuted,
//               ),
//               const SizedBox(width: 4),
//               Text(
//                 amenity,
//                 style: AppTextStyles.caption.copyWith(
//                   color: AppTheme.textLight,
//                 ),
//               ),
//             ],
//           ),
//         );
//       }).toList(),
//     );
//   }

//   Widget _buildFooter() {
//     return Row(
//       mainAxisAlignment: MainAxisAlignment.spaceBetween,
//       children: [
//         Column(
//           crossAxisAlignment: CrossAxisAlignment.start,
//           children: [
//             if (widget.result.minPrice != widget.result.discountedPrice)
//               Text(
//                 '${widget.result.minPrice.toStringAsFixed(0)} ${widget.result.currency}',
//                 style: AppTextStyles.bodySmall.copyWith(
//                   decoration: TextDecoration.lineThrough,
//                   color: AppTheme.textMuted.withOpacity(0.7),
//                 ),
//               ),
//             Row(
//               crossAxisAlignment: CrossAxisAlignment.baseline,
//               textBaseline: TextBaseline.alphabetic,
//               children: [
//                 ShaderMask(
//                   shaderCallback: (bounds) => 
//                       AppTheme.primaryGradient.createShader(bounds),
//                   child: Text(
//                     widget.result.discountedPrice.toStringAsFixed(0),
//                     style: AppTextStyles.heading1.copyWith(
//                       fontWeight: FontWeight.bold,
//                       color: Colors.white,
//                     ),
//                   ),
//                 ),
//                 const SizedBox(width: 6),
//                 Text(
//                   widget.result.currency,
//                   style: AppTextStyles.bodyMedium.copyWith(
//                     color: AppTheme.textMuted,
//                   ),
//                 ),
//                 const SizedBox(width: 6),
//                 Text(
//                   '/ الليلة',
//                   style: AppTextStyles.caption.copyWith(
//                     color: AppTheme.textMuted.withOpacity(0.7),
//                   ),
//                 ),
//               ],
//             ),
//           ],
//         ),
//         Container(
//           decoration: BoxDecoration(
//             gradient: AppTheme.primaryGradient,
//             borderRadius: BorderRadius.circular(16),
//             boxShadow: [
//               BoxShadow(
//                 color: AppTheme.primaryBlue.withOpacity(0.4),
//                 blurRadius: 15,
//                 spreadRadius: 2,
//               ),
//             ],
//           ),
//           child: Material(
//             color: Colors.transparent,
//             child: InkWell(
//               onTap: widget.onTap,
//               borderRadius: BorderRadius.circular(16),
//               child: Padding(
//                 padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
//                 child: Row(
//                   children: [
//                     Text(
//                       'عرض التفاصيل',
//                       style: AppTextStyles.buttonMedium.copyWith(
//                         color: Colors.white,
//                       ),
//                     ),
//                     const SizedBox(width: 8),
//                     const Icon(
//                       Icons.arrow_forward_rounded,
//                       size: 16,
//                       color: Colors.white,
//                     ),
//                   ],
//                 ),
//               ),
//             ),
//           ),
//         ),
//       ],
//     );
//   }

//   IconData _getAmenityIcon(String amenity) {
//     switch (amenity.toLowerCase()) {
//       case 'واي فاي':
//       case 'wifi':
//         return Icons.wifi_rounded;
//       case 'موقف سيارات':
//       case 'parking':
//         return Icons.local_parking_rounded;
//       case 'مسبح':
//       case 'pool':
//         return Icons.pool_rounded;
//       case 'مطعم':
//       case 'restaurant':
//         return Icons.restaurant_rounded;
//       case 'صالة رياضية':
//       case 'gym':
//         return Icons.fitness_center_rounded;
//       case 'مكيف':
//       case 'ac':
//         return Icons.ac_unit_rounded;
//       case 'سبا':
//       case 'spa':
//         return Icons.spa_rounded;
//       default:
//         return Icons.check_circle_outline_rounded;
//     }
//   }
// }
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/utils/image_utils.dart';
import 'dart:ui';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/search_result.dart';

enum CardDisplayType { list, grid, compact }

class SearchResultCardWidget extends StatefulWidget {
  final SearchResult result;
  final VoidCallback? onTap;
  final VoidCallback? onFavoriteToggle;
  final CardDisplayType displayType;
  final bool showDistance;

  const SearchResultCardWidget({
    super.key,
    required this.result,
    this.onTap,
    this.onFavoriteToggle,
    this.displayType = CardDisplayType.list,
    this.showDistance = true,
  });

  @override
  State<SearchResultCardWidget> createState() => _SearchResultCardWidgetState();
}

class _SearchResultCardWidgetState extends State<SearchResultCardWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _scaleAnimation;
  bool _isPressed = false;
  bool _isFavorite = false;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: const Duration(milliseconds: 100),
      vsync: this,
    );
    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 0.98,
    ).animate(CurvedAnimation(
      parent: _controller,
      curve: Curves.easeOut,
    ));
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    switch (widget.displayType) {
      case CardDisplayType.list:
        return _buildMinimalListCard();
      case CardDisplayType.grid:
        return _buildMinimalGridCard();
      case CardDisplayType.compact:
        return _buildMinimalCompactCard();
    }
  }

  Widget _buildMinimalListCard() {
    return AnimatedBuilder(
      animation: _scaleAnimation,
      builder: (context, child) {
        return Transform.scale(
          scale: _scaleAnimation.value,
          child: GestureDetector(
            onTapDown: (_) {
              setState(() => _isPressed = true);
              _controller.forward();
            },
            onTapUp: (_) {
              setState(() => _isPressed = false);
              _controller.reverse();
              widget.onTap?.call();
            },
            onTapCancel: () {
              setState(() => _isPressed = false);
              _controller.reverse();
            },
            child: Container(
              margin: const EdgeInsets.only(bottom: 12),
              decoration: BoxDecoration(
                color: AppTheme.darkCard.withOpacity(0.5),
                borderRadius: BorderRadius.circular(16),
                border: Border.all(
                  color: _isPressed
                      ? AppTheme.primaryBlue.withOpacity(0.2)
                      : AppTheme.darkBorder.withOpacity(0.1),
                  width: 0.5,
                ),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildMinimalImageSection(height: 160),
                  Padding(
                    padding: const EdgeInsets.all(16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        _buildTitle(),
                        const SizedBox(height: 8),
                        _buildLocationRow(),
                        const SizedBox(height: 12),
                        _buildInfoRow(),
                        const SizedBox(height: 12),
                        _buildPriceRow(),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildMinimalGridCard() {
    return AnimatedBuilder(
      animation: _scaleAnimation,
      builder: (context, child) {
        return Transform.scale(
          scale: _scaleAnimation.value,
          child: GestureDetector(
            onTapDown: (_) {
              setState(() => _isPressed = true);
              _controller.forward();
            },
            onTapUp: (_) {
              setState(() => _isPressed = false);
              _controller.reverse();
              widget.onTap?.call();
            },
            onTapCancel: () {
              setState(() => _isPressed = false);
              _controller.reverse();
            },
            child: Container(
              decoration: BoxDecoration(
                color: AppTheme.darkCard.withOpacity(0.5),
                borderRadius: BorderRadius.circular(14),
                border: Border.all(
                  color: _isPressed
                      ? AppTheme.primaryBlue.withOpacity(0.2)
                      : AppTheme.darkBorder.withOpacity(0.1),
                  width: 0.5,
                ),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    flex: 3,
                    child: _buildMinimalImageSection(),
                  ),
                  Expanded(
                    flex: 2,
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            widget.result.name,
                            style: AppTextStyles.bodySmall.copyWith(
                              fontWeight: FontWeight.w600,
                              color: AppTheme.textWhite,
                            ),
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                          ),
                          Row(
                            children: [
                              Icon(
                                Icons.location_on,
                                size: 12,
                                color: AppTheme.textMuted.withOpacity(0.6),
                              ),
                              const SizedBox(width: 4),
                              Expanded(
                                child: Text(
                                  widget.result.city,
                                  style: AppTextStyles.caption.copyWith(
                                    color: AppTheme.textMuted,
                                    fontSize: 10,
                                  ),
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                ),
                              ),
                            ],
                          ),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Row(
                                children: [
                                  Icon(
                                    Icons.star,
                                    size: 12,
                                    color: AppTheme.warning.withOpacity(0.8),
                                  ),
                                  const SizedBox(width: 2),
                                  Text(
                                    widget.result.averageRating.toStringAsFixed(1),
                                    style: AppTextStyles.caption.copyWith(
                                      fontWeight: FontWeight.w500,
                                      fontSize: 10,
                                    ),
                                  ),
                                ],
                              ),
                              Text(
                                '${widget.result.discountedPrice.toStringAsFixed(0)} ${widget.result.currency}',
                                style: AppTextStyles.bodySmall.copyWith(
                                  color: AppTheme.primaryBlue,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildMinimalCompactCard() {
    return AnimatedBuilder(
      animation: _scaleAnimation,
      builder: (context, child) {
        return Transform.scale(
          scale: _scaleAnimation.value,
          child: GestureDetector(
            onTapDown: (_) {
              setState(() => _isPressed = true);
              _controller.forward();
              HapticFeedback.lightImpact();
            },
            onTapUp: (_) {
              setState(() => _isPressed = false);
              _controller.reverse();
              widget.onTap?.call();
            },
            onTapCancel: () {
              setState(() => _isPressed = false);
              _controller.reverse();
            },
            child: Container(
              margin: const EdgeInsets.only(bottom: 10),
              decoration: BoxDecoration(
                color: AppTheme.darkCard.withOpacity(0.4),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(
                  color: _isPressed
                      ? AppTheme.primaryBlue.withOpacity(0.2)
                      : AppTheme.darkBorder.withOpacity(0.1),
                  width: 0.5,
                ),
              ),
              child: IntrinsicHeight(
                child: Row(
                  children: [
                    // Fixed width image with dynamic height
                    SizedBox(
                      width: 100,
                      child: AspectRatio(
                        aspectRatio: 3/4, // Portrait aspect ratio
                        child: Container(
                          decoration: BoxDecoration(
                            borderRadius: const BorderRadius.horizontal(
                              right: Radius.circular(12),
                            ),
                            image: DecorationImage(
                              image: NetworkImage(
                                ImageUtils.resolveUrl(widget.result.mainImageUrl ?? ''),
                              ),
                              fit: BoxFit.cover,
                            ),
                          ),
                          child: Stack(
                            children: [
                              // Subtle gradient overlay
                              Container(
                                decoration: BoxDecoration(
                                  borderRadius: const BorderRadius.horizontal(
                                    right: Radius.circular(12),
                                  ),
                                  gradient: LinearGradient(
                                    begin: Alignment.topCenter,
                                    end: Alignment.bottomCenter,
                                    colors: [
                                      Colors.transparent,
                                      Colors.black.withOpacity(0.3),
                                    ],
                                  ),
                                ),
                              ),
                              // Favorite button
                              Positioned(
                                top: 8,
                                right: 8,
                                child: _buildCompactFavoriteButton(),
                              ),
                              // Discount badge
                              if (widget.result.minPrice != widget.result.discountedPrice)
                                Positioned(
                                  bottom: 8,
                                  right: 8,
                                  child: Container(
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 6,
                                      vertical: 2,
                                    ),
                                    decoration: BoxDecoration(
                                      color: AppTheme.error.withOpacity(0.9),
                                      borderRadius: BorderRadius.circular(4),
                                    ),
                                    child: Text(
                                      '-${((1 - widget.result.discountedPrice / widget.result.minPrice) * 100).toStringAsFixed(0)}%',
                                      style: const TextStyle(
                                        color: Colors.white,
                                        fontSize: 10,
                                        fontWeight: FontWeight.w600,
                                      ),
                                    ),
                                  ),
                                ),
                            ],
                          ),
                        ),
                      ),
                    ),
                    // Content
                    Expanded(
                      child: Padding(
                        padding: const EdgeInsets.all(12),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            // Property type badge
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 6,
                                vertical: 2,
                              ),
                              decoration: BoxDecoration(
                                color: AppTheme.primaryBlue.withOpacity(0.1),
                                borderRadius: BorderRadius.circular(4),
                              ),
                              child: Text(
                                widget.result.propertyType,
                                style: TextStyle(
                                  color: AppTheme.primaryBlue,
                                  fontSize: 9,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ),
                            const SizedBox(height: 6),
                            // Name
                            Text(
                              widget.result.name,
                              style: AppTextStyles.bodySmall.copyWith(
                                fontWeight: FontWeight.w600,
                                color: AppTheme.textWhite,
                              ),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                            const SizedBox(height: 6),
                            // Location
                            Row(
                              children: [
                                Icon(
                                  Icons.location_on,
                                  size: 10,
                                  color: AppTheme.textMuted.withOpacity(0.6),
                                ),
                                const SizedBox(width: 3),
                                Expanded(
                                  child: Text(
                                    widget.result.city,
                                    style: AppTextStyles.caption.copyWith(
                                      color: AppTheme.textMuted,
                                      fontSize: 10,
                                    ),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 8),
                            // Bottom row with rating and price
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                // Rating
                                Container(
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 4,
                                    vertical: 2,
                                  ),
                                  decoration: BoxDecoration(
                                    color: AppTheme.warning.withOpacity(0.1),
                                    borderRadius: BorderRadius.circular(4),
                                  ),
                                  child: Row(
                                    mainAxisSize: MainAxisSize.min,
                                    children: [
                                      Icon(
                                        Icons.star,
                                        size: 10,
                                        color: AppTheme.warning,
                                      ),
                                      const SizedBox(width: 2),
                                      Text(
                                        widget.result.averageRating.toStringAsFixed(1),
                                        style: TextStyle(
                                          color: AppTheme.warning,
                                          fontSize: 10,
                                          fontWeight: FontWeight.w600,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                // Price
                                Column(
                                  crossAxisAlignment: CrossAxisAlignment.end,
                                  children: [
                                    if (widget.result.minPrice != widget.result.discountedPrice)
                                      Text(
                                        '${widget.result.minPrice.toStringAsFixed(0)}',
                                        style: TextStyle(
                                          color: AppTheme.textMuted.withOpacity(0.5),
                                          fontSize: 9,
                                          decoration: TextDecoration.lineThrough,
                                        ),
                                      ),
                                    Text(
                                      '${widget.result.discountedPrice.toStringAsFixed(0)} ${widget.result.currency}',
                                      style: AppTextStyles.bodySmall.copyWith(
                                        color: AppTheme.primaryBlue,
                                        fontWeight: FontWeight.w600,
                                      ),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ],
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
    );
  }

  Widget _buildMinimalImageSection({double? height}) {
    return Container(
      height: height,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(
            widget.displayType == CardDisplayType.grid ? 14 : 16,
          ),
        ),
        image: DecorationImage(
          image: NetworkImage(
            ImageUtils.resolveUrl(widget.result.mainImageUrl ?? ''),
          ),
          fit: BoxFit.cover,
        ),
      ),
      child: Stack(
        children: [
          // Subtle gradient
          Container(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.vertical(
                top: Radius.circular(
                  widget.displayType == CardDisplayType.grid ? 14 : 16,
                ),
              ),
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  Colors.transparent,
                  Colors.black.withOpacity(0.2),
                ],
              ),
            ),
          ),
          
          // Top row badges
          Positioned(
            top: 10,
            left: 10,
            right: 10,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (widget.result.isFeatured)
                  Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 3,
                    ),
                    decoration: BoxDecoration(
                      color: AppTheme.primaryBlue.withOpacity(0.9),
                      borderRadius: BorderRadius.circular(6),
                    ),
                    child: Text(
                      'مميز',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 10,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                _buildMinimalFavoriteButton(),
              ],
            ),
          ),
          
          // Discount badge
          if (widget.result.minPrice != widget.result.discountedPrice)
            Positioned(
              bottom: 10,
              right: 10,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 8,
                  vertical: 4,
                ),
                decoration: BoxDecoration(
                  color: AppTheme.error.withOpacity(0.9),
                  borderRadius: BorderRadius.circular(6),
                ),
                child: Text(
                  '${((1 - widget.result.discountedPrice / widget.result.minPrice) * 100).toStringAsFixed(0)}% خصم',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 11,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildMinimalFavoriteButton() {
    return GestureDetector(
      onTap: () {
        setState(() {
          _isFavorite = !_isFavorite;
        });
        HapticFeedback.lightImpact();
        widget.onFavoriteToggle?.call();
      },
      child: Container(
        width: 32,
        height: 32,
        decoration: BoxDecoration(
          color: Colors.white.withOpacity(0.9),
          shape: BoxShape.circle,
        ),
        child: Icon(
          _isFavorite ? Icons.favorite : Icons.favorite_border,
          color: _isFavorite ? AppTheme.error : AppTheme.textDark,
          size: 16,
        ),
      ),
    );
  }

  Widget _buildCompactFavoriteButton() {
    return GestureDetector(
      onTap: () {
        setState(() {
          _isFavorite = !_isFavorite;
        });
        HapticFeedback.lightImpact();
        widget.onFavoriteToggle?.call();
      },
      child: Container(
        width: 26,
        height: 26,
        decoration: BoxDecoration(
          color: Colors.white.withOpacity(0.9),
          shape: BoxShape.circle,
        ),
        child: Icon(
          _isFavorite ? Icons.favorite : Icons.favorite_border,
          color: _isFavorite ? AppTheme.error : AppTheme.textDark,
          size: 14,
        ),
      ),
    );
  }

  Widget _buildTitle() {
    return Row(
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 6,
                  vertical: 2,
                ),
                decoration: BoxDecoration(
                  color: AppTheme.primaryBlue.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(4),
                ),
                child: Text(
                  widget.result.propertyType,
                  style: TextStyle(
                    color: AppTheme.primaryBlue,
                    fontSize: 10,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
              const SizedBox(height: 6),
              Text(
                widget.result.name,
                style: AppTextStyles.bodyMedium.copyWith(
                  fontWeight: FontWeight.w600,
                  color: AppTheme.textWhite,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],
          ),
        ),
        if (widget.result.starRating > 0)
          Row(
            children: List.generate(
              widget.result.starRating,
              (index) => Icon(
                Icons.star,
                size: 14,
                color: AppTheme.warning.withOpacity(0.8),
              ),
            ),
          ),
      ],
    );
  }

  Widget _buildLocationRow() {
    return Row(
      children: [
        Icon(
          Icons.location_on,
          size: 12,
          color: AppTheme.textMuted.withOpacity(0.6),
        ),
        const SizedBox(width: 4),
        Expanded(
          child: Text(
            '${widget.result.address}, ${widget.result.city}',
            style: AppTextStyles.caption.copyWith(
              color: AppTheme.textMuted,
            ),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ),
        if (widget.showDistance && widget.result.distanceKm != null) ...[
          const SizedBox(width: 8),
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: 6,
              vertical: 2,
            ),
            decoration: BoxDecoration(
              color: AppTheme.darkSurface.withOpacity(0.5),
              borderRadius: BorderRadius.circular(4),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(
                  Icons.near_me,
                  size: 10,
                  color: AppTheme.textMuted,
                ),
                const SizedBox(width: 3),
                Text(
                  '${widget.result.distanceKm!.toStringAsFixed(1)} كم',
                  style: TextStyle(
                    color: AppTheme.textMuted,
                    fontSize: 10,
                  ),
                ),
              ],
            ),
          ),
        ],
      ],
    );
  }

  Widget _buildInfoRow() {
    return Row(
      children: [
        Container(
          padding: const EdgeInsets.symmetric(
            horizontal: 8,
            vertical: 4,
          ),
          decoration: BoxDecoration(
            color: AppTheme.warning.withOpacity(0.1),
            borderRadius: BorderRadius.circular(6),
          ),
          child: Row(
            children: [
              Icon(
                Icons.star,
                size: 14,
                color: AppTheme.warning,
              ),
              const SizedBox(width: 4),
              Text(
                widget.result.averageRating.toStringAsFixed(1),
                style: AppTextStyles.caption.copyWith(
                  fontWeight: FontWeight.w600,
                  color: AppTheme.warning,
                ),
              ),
              const SizedBox(width: 4),
              Text(
                '(${widget.result.reviewsCount})',
                style: AppTextStyles.caption.copyWith(
                  color: AppTheme.textMuted,
                  fontSize: 10,
                ),
              ),
            ],
          ),
        ),
        const Spacer(),
        if (widget.result.isAvailable)
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: 8,
              vertical: 4,
            ),
            decoration: BoxDecoration(
              color: AppTheme.success.withOpacity(0.1),
              borderRadius: BorderRadius.circular(6),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: 4,
                  height: 4,
                  decoration: BoxDecoration(
                    color: AppTheme.success,
                    shape: BoxShape.circle,
                  ),
                ),
                const SizedBox(width: 4),
                Text(
                  'متاح',
                  style: TextStyle(
                    color: AppTheme.success,
                    fontSize: 10,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
          )
        else
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: 8,
              vertical: 4,
            ),
            decoration: BoxDecoration(
              color: AppTheme.error.withOpacity(0.1),
              borderRadius: BorderRadius.circular(6),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: 4,
                  height: 4,
                  decoration: BoxDecoration(
                    color: AppTheme.error,
                    shape: BoxShape.circle,
                  ),
                ),
                const SizedBox(width: 4),
                Text(
                  'محجوز',
                  style: TextStyle(
                    color: AppTheme.error,
                    fontSize: 10,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
          ),
      ],
    );
  }

  Widget _buildPriceRow() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (widget.result.minPrice != widget.result.discountedPrice)
              Text(
                '${widget.result.minPrice.toStringAsFixed(0)} ${widget.result.currency}',
                style: AppTextStyles.caption.copyWith(
                  decoration: TextDecoration.lineThrough,
                  color: AppTheme.textMuted.withOpacity(0.5),
                ),
              ),
            Row(
              crossAxisAlignment: CrossAxisAlignment.baseline,
              textBaseline: TextBaseline.alphabetic,
              children: [
                Text(
                  widget.result.discountedPrice.toStringAsFixed(0),
                  style: AppTextStyles.bodyLarge.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppTheme.primaryBlue,
                  ),
                ),
                const SizedBox(width: 4),
                Text(
                  widget.result.currency,
                  style: AppTextStyles.caption.copyWith(
                    color: AppTheme.textMuted,
                  ),
                ),
                const SizedBox(width: 4),
                Text(
                  '/ الليلة',
                  style: AppTextStyles.caption.copyWith(
                    color: AppTheme.textMuted.withOpacity(0.6),
                    fontSize: 10,
                  ),
                ),
              ],
            ),
          ],
        ),
        GestureDetector(
          onTap: widget.onTap,
          child: Container(
            padding: const EdgeInsets.symmetric(
              horizontal: 16,
              vertical: 8,
            ),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  AppTheme.primaryBlue.withOpacity(0.9),
                  AppTheme.primaryPurple.withOpacity(0.9),
                ],
              ),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  'عرض',
                  style: AppTextStyles.caption.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(width: 4),
                Icon(
                  Icons.arrow_forward,
                  size: 12,
                  color: Colors.white,
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}