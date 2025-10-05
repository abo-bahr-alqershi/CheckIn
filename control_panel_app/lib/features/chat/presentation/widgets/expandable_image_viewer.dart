import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:photo_view/photo_view.dart';
import 'package:photo_view/photo_view_gallery.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/attachment.dart';

class ExpandableImageViewer extends StatefulWidget {
  final List<Attachment> images;
  final int initialIndex;
  final Function(String)? onReaction;
  final VoidCallback? onReply;

  const ExpandableImageViewer({
    super.key,
    required this.images,
    this.initialIndex = 0,
    this.onReaction,
    this.onReply,
  });

  @override
  State<ExpandableImageViewer> createState() => _ExpandableImageViewerState();
}

class _ExpandableImageViewerState extends State<ExpandableImageViewer> {
  late int _currentIndex;
  late final PageController _pageController;

  @override
  void initState() {
    super.initState();
    _currentIndex = widget.initialIndex.clamp(0, widget.images.length - 1);
    _pageController = PageController(initialPage: _currentIndex);
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.black,
      body: Stack(
        children: [
          GestureDetector(
            onDoubleTap: () {
              widget.onReaction?.call('like');
            },
            onLongPress: () {
              HapticFeedback.lightImpact();
              _showImageOptions();
            },
            child: PhotoViewGallery.builder(
              pageController: _pageController,
              itemCount: widget.images.length,
              onPageChanged: (index) => setState(() => _currentIndex = index),
              builder: (context, index) {
                final image = widget.images[index];
                return PhotoViewGalleryPageOptions.customChild(
                  child: CachedImageWidget(
                    imageUrl: image.fileUrl,
                    fit: BoxFit.contain,
                    removeContainer: true,
                  ),
                  initialScale: PhotoViewComputedScale.contained,
                  minScale: PhotoViewComputedScale.contained,
                  maxScale: PhotoViewComputedScale.covered * 3.0,
                  heroAttributes: PhotoViewHeroAttributes(tag: image.id),
                );
              },
              loadingBuilder: (context, event) => const Center(
                child: CircularProgressIndicator(strokeWidth: 2),
              ),
              backgroundDecoration: const BoxDecoration(color: Colors.black),
            ),
          ),
          Positioned(
            top: MediaQuery.of(context).padding.top + 8,
            left: 8,
            child: IconButton(
              icon: const Icon(Icons.close, color: Colors.white),
              onPressed: () => Navigator.of(context).pop(),
            ),
          ),
          Positioned(
            top: MediaQuery.of(context).padding.top + 12,
            right: 16,
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.15),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Text(
                '${_currentIndex + 1}/${widget.images.length}',
                style: const TextStyle(color: Colors.white),
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _showImageOptions() {
    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      builder: (context) {
        return _ImageViewerOptionsSheet(
          onReact: (type) => widget.onReaction?.call(type),
          onReply: widget.onReply,
        );
      },
    );
  }
}

class _ImageViewerOptionsSheet extends StatelessWidget {
  final void Function(String) onReact;
  final VoidCallback? onReply;
  const _ImageViewerOptionsSheet({required this.onReact, this.onReply});

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              _reactionChip(context, '👍', 'like', onReact),
              _reactionChip(context, '❤️', 'love', onReact),
              _reactionChip(context, '😂', 'laugh', onReact),
              _reactionChip(context, '😮', 'wow', onReact),
              _reactionChip(context, '😢', 'sad', onReact),
              _reactionChip(context, '😠', 'angry', onReact),
            ],
          ),
          const SizedBox(height: 12),
          Container(
            margin: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.black.withOpacity(0.6),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (onReply != null)
                  _actionTile(context, Icons.reply_rounded, 'رد', () {
                    Navigator.pop(context);
                    onReply!();
                  }),
                _actionTile(context, Icons.download_rounded, 'حفظ', () {
                  Navigator.pop(context);
                }),
                _actionTile(context, Icons.share_rounded, 'مشاركة', () {
                  Navigator.pop(context);
                }),
              ],
            ),
          ),
          const SizedBox(height: 8),
        ],
      ),
    );
  }

  Widget _reactionChip(
    BuildContext context,
    String emoji,
    String type,
    void Function(String) onReact,
  ) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 6),
      child: GestureDetector(
        onTap: () {
          HapticFeedback.selectionClick();
          Navigator.pop(context);
          onReact(type);
        },
        child: Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: Colors.white.withOpacity(0.1),
            shape: BoxShape.circle,
          ),
          child: Text(emoji, style: const TextStyle(fontSize: 22)),
        ),
      ),
    );
  }

  Widget _actionTile(
    BuildContext context,
    IconData icon,
    String title,
    VoidCallback onTap,
  ) {
    return ListTile(
      onTap: onTap,
      leading: Icon(icon, color: Colors.white.withOpacity(0.9)),
      title: Text(title, style: const TextStyle(color: Colors.white)),
    );
  }
}

// No global key needed; we use the bottom sheet context to pop
