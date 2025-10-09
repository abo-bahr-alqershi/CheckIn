import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/attachment.dart';
import 'expandable_image_viewer.dart';

class WhatsAppStyleImageGrid extends StatelessWidget {
  final List<Attachment> images;
  final bool isMe;
  final VoidCallback? onTap;
  // Optional callbacks to propagate actions to parent (message-level)
  final Function(String)? onReaction;
  final void Function(Attachment)? onReply;
  // Optional: per-attachment reaction state to reflect overlay in viewer
  final Map<String, String>? reactionsByAttachment;
  // Optional: callback when a reaction is set for a specific attachment
  final void Function(Attachment, String)? onReactForAttachment;

  const WhatsAppStyleImageGrid({
    super.key,
    required this.images,
    required this.isMe,
    this.onTap,
    this.onReaction,
    this.onReply,
    this.reactionsByAttachment,
    this.onReactForAttachment,
  });

  @override
  Widget build(BuildContext context) {
    if (images.isEmpty) return const SizedBox.shrink();

    final imageCount = images.length;

    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        _openImageViewer(context, 0);
      },
      onLongPress: () {
        HapticFeedback.lightImpact();
        _openImageViewer(context, 0);
      },
      child: _buildGrid(imageCount),
    );
  }

  Widget _buildGrid(int count) {
    switch (count) {
      case 1:
        return _buildSingleImage(images.first);
      case 2:
        return _buildTwoImages();
      case 3:
        return _buildThreeImages();
      case 4:
        return _buildFourImages();
      default:
        return _buildMoreImages();
    }
  }

  Widget _buildSingleImage(Attachment image) {
    // Keep a visually pleasant default ratio and ensure tap opens viewer.
    return AspectRatio(
      aspectRatio: 4 / 3,
      child: Builder(
        builder: (context) => GestureDetector(
          onTap: () {
            HapticFeedback.lightImpact();
            _openImageViewer(context, 0);
          },
          onLongPress: () {
            HapticFeedback.lightImpact();
            _openImageViewer(context, 0);
          },
          child: Stack(
            fit: StackFit.expand,
            children: [
              CachedImageWidget(
                imageUrl: image.fileUrl,
                fit: BoxFit.cover,
              ),
              if (_hasOverlayFor(image)) _buildReactionOverlay(image),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTwoImages() {
    return AspectRatio(
      aspectRatio: 4 / 3,
      child: Row(
        children: [
          Expanded(
            child: _buildImageTile(images[0], 0),
          ),
          const SizedBox(width: 2),
          Expanded(
            child: _buildImageTile(images[1], 1),
          ),
        ],
      ),
    );
  }

  Widget _buildThreeImages() {
    return AspectRatio(
      aspectRatio: 4 / 3,
      child: Row(
        children: [
          Expanded(
            flex: 2,
            child: _buildImageTile(images[0], 0),
          ),
          const SizedBox(width: 2),
          Expanded(
            child: Column(
              children: [
                Expanded(
                  child: _buildImageTile(images[1], 1),
                ),
                const SizedBox(height: 2),
                Expanded(
                  child: _buildImageTile(images[2], 2),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFourImages() {
    return AspectRatio(
      aspectRatio: 4 / 3,
      child: Column(
        children: [
          Expanded(
            child: Row(
              children: [
                Expanded(
                  child: _buildImageTile(images[0], 0),
                ),
                const SizedBox(width: 2),
                Expanded(
                  child: _buildImageTile(images[1], 1),
                ),
              ],
            ),
          ),
          const SizedBox(height: 2),
          Expanded(
            child: Row(
              children: [
                Expanded(
                  child: _buildImageTile(images[2], 2),
                ),
                const SizedBox(width: 2),
                Expanded(
                  child: _buildImageTile(images[3], 3),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildMoreImages() {
    final displayImages = images.take(4).toList();
    final remainingCount = images.length - 4;

    return AspectRatio(
      aspectRatio: 4 / 3,
      child: Column(
        children: [
          Expanded(
            child: Row(
              children: [
                Expanded(
                  child: _buildImageTile(displayImages[0], 0),
                ),
                const SizedBox(width: 2),
                Expanded(
                  child: _buildImageTile(displayImages[1], 1),
                ),
              ],
            ),
          ),
          const SizedBox(height: 2),
          Expanded(
            child: Row(
              children: [
                Expanded(
                  child: _buildImageTile(displayImages[2], 2),
                ),
                const SizedBox(width: 2),
                Expanded(
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      _buildImageTile(displayImages[3], 3),
                      if (remainingCount > 0)
                        Container(
                          decoration: BoxDecoration(
                            gradient: LinearGradient(
                              colors: [
                                Colors.black.withValues(alpha: 0.7),
                                Colors.black.withValues(alpha: 0.5),
                              ],
                            ),
                          ),
                          child: Center(
                            child: Text(
                              '+$remainingCount',
                              style: AppTextStyles.heading2.copyWith(
                                color: Colors.white,
                                fontWeight: FontWeight.bold,
                                fontSize: 24,
                              ),
                            ),
                          ),
                        ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildImageTile(Attachment image, int index) {
    return Builder(
      builder: (context) => GestureDetector(
        onTap: () {
          HapticFeedback.lightImpact();
          _openImageViewer(context, index);
        },
        onLongPress: () {
          HapticFeedback.lightImpact();
          _openImageViewer(context, index);
        },
        child: Stack(
          fit: StackFit.expand,
          children: [
            CachedImageWidget(
              imageUrl: image.fileUrl,
              fit: BoxFit.cover,
            ),
            if (_hasOverlayFor(image)) _buildReactionOverlay(image),
          ],
        ),
      ),
    );
  }

  bool _hasOverlayFor(Attachment image) {
    final map = reactionsByAttachment;
    if (map == null) return false;
    final r = map[image.id];
    return r != null && r.isNotEmpty;
  }

  Widget _buildReactionOverlay(Attachment image) {
    final type = reactionsByAttachment?[image.id] ?? '';
    final emoji = _emojiFor(type);
    return Positioned(
      right: 6,
      bottom: 6,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 3),
        decoration: BoxDecoration(
          color: Colors.black.withOpacity(0.45),
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: Colors.white.withOpacity(0.2), width: 0.5),
        ),
        child: Text(emoji, style: const TextStyle(fontSize: 12)),
      ),
    );
  }

  String _emojiFor(String type) {
    switch (type) {
      case 'like':
        return '👍';
      case 'love':
        return '❤️';
      case 'laugh':
        return '😂';
      case 'wow':
        return '😮';
      case 'sad':
        return '😢';
      case 'angry':
        return '😠';
      default:
        return '👍';
    }
  }

  void _openImageViewer(BuildContext context, int initialIndex) {
    Navigator.push(
      context,
      PageRouteBuilder(
        pageBuilder: (context, animation, secondaryAnimation) =>
            ExpandableImageViewer(
          images: images,
          initialIndex: initialIndex,
          onReaction: onReaction,
          onReply: onReply,
          initialReactionsByAttachment: reactionsByAttachment,
          onReactForAttachment: onReactForAttachment,
        ),
        transitionDuration: const Duration(milliseconds: 300),
        transitionsBuilder: (context, animation, secondaryAnimation, child) {
          return FadeTransition(
            opacity: animation,
            child: child,
          );
        },
      ),
    );
  }
}
