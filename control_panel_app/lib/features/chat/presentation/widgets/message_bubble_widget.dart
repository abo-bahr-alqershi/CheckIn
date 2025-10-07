import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/message.dart';
import '../../domain/entities/attachment.dart';
import '../bloc/chat_bloc.dart';
import 'message_status_indicator.dart';
import 'reaction_picker_widget.dart';
import 'attachment_preview_widget.dart';

class MessageBubbleWidget extends StatefulWidget {
  final Message message;
  final bool isMe;
  final Message? previousMessage;
  final Message? nextMessage;
  final VoidCallback? onReply;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;
  final Function(String)? onReaction;
  final VoidCallback? onReplyTap;

  const MessageBubbleWidget({
    super.key,
    required this.message,
    required this.isMe,
    this.previousMessage,
    this.nextMessage,
    this.onReply,
    this.onEdit,
    this.onDelete,
    this.onReaction,
    this.onReplyTap,
  });

  @override
  State<MessageBubbleWidget> createState() => _MessageBubbleWidgetState();
}

class _MessageBubbleWidgetState extends State<MessageBubbleWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  late Animation<double> _fadeAnimation;
  bool _showReactions = false;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );

    _scaleAnimation = Tween<double>(
      begin: 0.95,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOutBack,
    ));

    _fadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOut,
    ));

    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  bool get _isFirstInGroup {
    if (widget.previousMessage == null) return true;
    return widget.previousMessage!.senderId != widget.message.senderId ||
        widget.message.createdAt
                .difference(widget.previousMessage!.createdAt)
                .inMinutes >
            5;
  }

  bool get _isLastInGroup {
    if (widget.nextMessage == null) return true;
    return widget.nextMessage!.senderId != widget.message.senderId ||
        widget.nextMessage!.createdAt
                .difference(widget.message.createdAt)
                .inMinutes >
            5;
  }

  Message? _findReplyMessage() {
    final replyId = widget.message.replyToMessageId;
    if (replyId == null) return null;

    final chatBloc = context.read<ChatBloc>();
    final chatState = chatBloc.state;
    if (chatState is! ChatLoaded) return null;

    final List<Message> messages =
        (chatState.messages[widget.message.conversationId] ?? [])
            .cast<Message>();

    for (final m in messages) {
      if (m.id == replyId) return m;
    }
    return null;
  }

  // Helper لاستخراج معرف المرفق المستهدف
  String? _extractAttachmentId(String? content) {
    if (content == null || !content.startsWith('::attref=')) return null;
    final endIdx = content.indexOf('::', '::attref='.length);
    if (endIdx > '::attref='.length) {
      return content.substring('::attref='.length, endIdx);
    }
    return null;
  }

  // Helper لتنظيف المحتوى من token
  String _cleanContent(String? content) {
    if (content == null) return '';
    if (content.startsWith('::attref=')) {
      final endIdx = content.indexOf('::', '::attref='.length);
      if (endIdx > '::attref='.length) {
        return content.substring(endIdx + 2).trim();
      }
    }
    return content.trim();
  }

  @override
  Widget build(BuildContext context) {
    return FadeTransition(
      opacity: _fadeAnimation,
      child: ScaleTransition(
        scale: _scaleAnimation,
        child: Padding(
          padding: EdgeInsets.only(
            top: _isFirstInGroup ? 8 : 2,
            bottom: _isLastInGroup ? 8 : 2,
            left: widget.isMe ? MediaQuery.of(context).size.width * 0.2 : 8,
            right: widget.isMe ? 8 : MediaQuery.of(context).size.width * 0.2,
          ),
          child: Column(
            crossAxisAlignment:
                widget.isMe ? CrossAxisAlignment.end : CrossAxisAlignment.start,
            children: [
              GestureDetector(
                onLongPress: _showOptions,
                onDoubleTap: _handleDoubleTap,
                child: Container(
                  constraints: BoxConstraints(
                    maxWidth: MediaQuery.of(context).size.width * 0.75,
                  ),
                  decoration: BoxDecoration(
                    gradient: widget.isMe
                        ? LinearGradient(
                            colors: [
                              AppTheme.primaryBlue.withValues(alpha: 0.15),
                              AppTheme.primaryPurple.withValues(alpha: 0.08),
                            ],
                          )
                        : LinearGradient(
                            colors: [
                              AppTheme.darkCard.withValues(alpha: 0.4),
                              AppTheme.darkCard.withValues(alpha: 0.25),
                            ],
                          ),
                    borderRadius: _getBorderRadius(),
                    border: Border.all(
                      color: widget.isMe
                          ? AppTheme.primaryBlue.withValues(alpha: 0.15)
                          : AppTheme.darkBorder.withValues(alpha: 0.08),
                      width: 0.5,
                    ),
                  ),
                  child: ClipRRect(
                    borderRadius: _getBorderRadius(),
                    child: BackdropFilter(
                      filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
                      child: Padding(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 10,
                          vertical: 6,
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            if (widget.message.replyToMessageId != null)
                              _buildReplyPreview(),
                            _buildMessageContent(),
                            const SizedBox(height: 2),
                            _buildMessageFooter(),
                          ],
                        ),
                      ),
                    ),
                  ),
                ),
              ),
              if (widget.message.reactions.isNotEmpty || _showReactions)
                Padding(
                  padding: const EdgeInsets.only(top: 4),
                  child: _showReactions
                      ? ReactionPickerWidget(
                          onReaction: (reaction) {
                            widget.onReaction?.call(reaction);
                            setState(() => _showReactions = false);
                          },
                        )
                      : _buildMinimalReactions(),
                ),
            ],
          ),
        ),
      ),
    );
  }

  BorderRadius _getBorderRadius() {
    const radius = 12.0;
    const smallRadius = 4.0;

    if (widget.isMe) {
      return BorderRadius.only(
        topLeft: const Radius.circular(radius),
        topRight: Radius.circular(_isFirstInGroup ? radius : smallRadius),
        bottomLeft: const Radius.circular(radius),
        bottomRight: Radius.circular(_isLastInGroup ? radius : smallRadius),
      );
    } else {
      return BorderRadius.only(
        topLeft: Radius.circular(_isFirstInGroup ? radius : smallRadius),
        topRight: const Radius.circular(radius),
        bottomLeft: Radius.circular(_isLastInGroup ? radius : smallRadius),
        bottomRight: const Radius.circular(radius),
      );
    }
  }

  // ✅ دالة محسنة للرد مع دعم الصور الصحيح
  Widget _buildReplyPreview() {
    final replyMessage = _findReplyMessage();

    // استخراج معرف المرفق المستهدف إن وجد
    final targetAttachmentId = _extractAttachmentId(widget.message.content);

    print('🔍 DEBUG: Building reply preview');
    print('🔍 DEBUG: Reply to message ID: ${widget.message.replyToMessageId}');
    print('🔍 DEBUG: Found reply message: ${replyMessage != null}');
    print('🔍 DEBUG: Target attachment ID: $targetAttachmentId');
    print('🔍 DEBUG: onReplyTap is null? ${widget.onReplyTap == null}');

    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: GestureDetector(
        onTap: () {
          print('🔥 REPLY CARD TAPPED!');
          print('🔥 Reply to ID: ${widget.message.replyToMessageId}');
          print('🔥 onReplyTap exists: ${widget.onReplyTap != null}');

          if (widget.onReplyTap != null) {
            HapticFeedback.selectionClick();

            // إذا كانت الرسالة غير موجودة، قد نحتاج لتحميل المزيد
            if (replyMessage == null) {
              print('⚠️ Reply message not found in current messages');
              print('📜 Attempting to load more messages...');

              // إرسال حدث لتحميل المزيد من الرسائل
              context.read<ChatBloc>().add(
                    LoadMoreMessagesEvent(
                      conversationId: widget.message.conversationId,
                      targetMessageId: widget.message.replyToMessageId,
                    ),
                  );
            }

            widget.onReplyTap!();
          } else {
            print('⚠️ WARNING: onReplyTap is null!');
          }
        },
        child: Container(
          padding: const EdgeInsets.all(6),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: widget.isMe
                  ? [
                      Colors.white.withValues(alpha: 0.12),
                      Colors.white.withValues(alpha: 0.06),
                    ]
                  : [
                      AppTheme.primaryBlue.withValues(alpha: 0.06),
                      AppTheme.primaryBlue.withValues(alpha: 0.03),
                    ],
            ),
            borderRadius: BorderRadius.circular(5),
            border: Border(
              left: BorderSide(
                color: widget.isMe
                    ? Colors.white.withValues(alpha: 0.5)
                    : AppTheme.primaryBlue.withValues(alpha: 0.8),
                width: 2,
              ),
            ),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              // عرض الصورة إن وجدت
              if (replyMessage != null &&
                  replyMessage.attachments.isNotEmpty) ...[
                _buildReplyImage(replyMessage, targetAttachmentId),
                const SizedBox(width: 6),
              ],
              // عرض المحتوى النصي
              Flexible(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      'رد على رسالة',
                      style: AppTextStyles.caption.copyWith(
                        color: widget.isMe
                            ? Colors.white.withValues(alpha: 0.6)
                            : AppTheme.primaryBlue.withValues(alpha: 0.7),
                        fontWeight: FontWeight.w600,
                        fontSize: 9,
                      ),
                    ),
                    const SizedBox(height: 2),
                    _buildReplyContent(replyMessage),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  // ✅ دالة لبناء صورة الرد باستخدام CachedImageWidget
  Widget _buildReplyImage(Message replyMessage, String? targetAttachmentId) {
    Attachment? targetAttachment;

    // البحث عن المرفق المستهدف
    if (targetAttachmentId != null) {
      for (final a in replyMessage.attachments) {
        if (a.id == targetAttachmentId) {
          targetAttachment = a;
          break;
        }
      }
    }

    // إذا لم نجد المرفق المحدد، ابحث عن أول صورة
    if (targetAttachment == null) {
      for (final a in replyMessage.attachments) {
        if (a.isImage || _isImageUrl(a.fileUrl)) {
          targetAttachment = a;
          break;
        }
      }
    }

    // استخدم أول مرفق كملاذ أخير
    targetAttachment ??= replyMessage.attachments.first;

    final imageUrl = targetAttachment.thumbnailUrl ??
        targetAttachment.fileUrl ??
        targetAttachment.url;

    return ClipRRect(
      borderRadius: BorderRadius.circular(4),
      child: Container(
        width: 32,
        height: 32,
        decoration: BoxDecoration(
          border: Border.all(
            color: widget.isMe
                ? Colors.white.withValues(alpha: 0.2)
                : AppTheme.darkBorder.withValues(alpha: 0.15),
            width: 0.5,
          ),
          borderRadius: BorderRadius.circular(4),
        ),
        child: CachedImageWidget(
          imageUrl: imageUrl,
          fit: BoxFit.cover,
          width: 32,
          height: 32,
          removeContainer: true,
        ),
      ),
    );
  }

  // ✅ دالة لبناء محتوى الرد
  Widget _buildReplyContent(Message? replyMessage) {
    if (replyMessage == null) {
      return Text(
        'رسالة محذوفة',
        style: AppTextStyles.caption.copyWith(
          color: widget.isMe
              ? Colors.white.withValues(alpha: 0.4)
              : AppTheme.textMuted.withValues(alpha: 0.5),
          fontSize: 10,
          fontStyle: FontStyle.italic,
        ),
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
      );
    }

    // تنظيف المحتوى
    final cleanContent = _cleanContent(replyMessage.content);

    // إذا كانت رسالة صورة فقط
    if (replyMessage.attachments.isNotEmpty && cleanContent.isEmpty) {
      final hasImage = replyMessage.attachments
          .any((a) => a.isImage || _isImageUrl(a.fileUrl));

      if (hasImage) {
        return Text(
          'صورة',
          style: AppTextStyles.caption.copyWith(
            color: widget.isMe
                ? Colors.white.withValues(alpha: 0.6)
                : AppTheme.textWhite.withValues(alpha: 0.7),
            fontSize: 10,
          ),
        );
      }

      return Text(
        'مرفق',
        style: AppTextStyles.caption.copyWith(
          color: widget.isMe
              ? Colors.white.withValues(alpha: 0.6)
              : AppTheme.textWhite.withValues(alpha: 0.7),
          fontSize: 10,
        ),
      );
    }

    // رسالة نصية
    return Text(
      cleanContent.isEmpty ? '[محتوى غير نصي]' : cleanContent,
      style: AppTextStyles.caption.copyWith(
        color: widget.isMe
            ? Colors.white.withValues(alpha: 0.6)
            : AppTheme.textWhite.withValues(alpha: 0.7),
        fontSize: 10,
      ),
      maxLines: 1,
      overflow: TextOverflow.ellipsis,
    );
  }

  // Helper لفحص إذا كان URL صورة
  bool _isImageUrl(String? url) {
    if (url == null) return false;
    final lower = url.toLowerCase();
    return lower.endsWith('.jpg') ||
        lower.endsWith('.jpeg') ||
        lower.endsWith('.png') ||
        lower.endsWith('.gif') ||
        lower.endsWith('.webp') ||
        lower.contains('/api/common/chat/attachments/');
  }

  Widget _buildMessageContent() {
    if (widget.message.isDeleted) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            Icons.block,
            size: 12,
            color: AppTheme.textMuted.withValues(alpha: 0.3),
          ),
          const SizedBox(width: 4),
          Text(
            'تم حذف هذه الرسالة',
            style: AppTextStyles.caption.copyWith(
              color: AppTheme.textMuted.withValues(alpha: 0.5),
              fontStyle: FontStyle.italic,
              fontSize: 11,
            ),
          ),
        ],
      );
    }

    // تنظيف المحتوى من token
    final displayContent = _cleanContent(widget.message.content);

    final nonImageAttachments = widget.message.attachments
        .where((a) => !a.isImage)
        .toList();

    if (displayContent.isEmpty && widget.message.attachments.isEmpty) {
      return const SizedBox.shrink();
    }

    // Build rich content: text + non-image attachments (audio/video/docs)
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (displayContent.isNotEmpty)
          Text(
            displayContent,
            style: AppTextStyles.bodyMedium.copyWith(
              color: widget.isMe
                  ? AppTheme.textWhite.withValues(alpha: 0.95)
                  : AppTheme.textWhite.withValues(alpha: 0.9),
              fontSize: 13,
              height: 1.3,
            ),
          ),
        if (displayContent.isNotEmpty && nonImageAttachments.isNotEmpty)
          const SizedBox(height: 6),
        ...nonImageAttachments.map((att) => Padding(
              padding: const EdgeInsets.only(top: 4),
              child: AttachmentPreviewWidget(
                attachment: att,
                isMe: widget.isMe,
                onTap: () {},
              ),
            )),
      ],
    );
  }

  Widget _buildMessageFooter() {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (widget.message.isEdited) ...[
          Text(
            'تم التعديل',
            style: AppTextStyles.caption.copyWith(
              color: widget.isMe
                  ? AppTheme.textWhite.withValues(alpha: 0.4)
                  : AppTheme.textMuted.withValues(alpha: 0.3),
              fontSize: 9,
            ),
          ),
          const SizedBox(width: 4),
        ],
        Text(
          _formatTime(widget.message.createdAt),
          style: AppTextStyles.caption.copyWith(
            color: widget.isMe
                ? AppTheme.textWhite.withValues(alpha: 0.5)
                : AppTheme.textMuted.withValues(alpha: 0.4),
            fontSize: 9,
          ),
        ),
        if (widget.isMe) ...[
          const SizedBox(width: 3),
          MessageStatusIndicator(
            status: widget.message.status,
            color: AppTheme.textWhite.withValues(alpha: 0.5),
            size: 11,
          ),
        ],
      ],
    );
  }

  void _showOptions() {
    HapticFeedback.lightImpact();
    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      builder: (context) {
        return _MessageOptionsSheet(
          isMe: widget.isMe,
          onReply: widget.onReply,
          onEdit: widget.onEdit,
          onDelete: widget.onDelete,
        );
      },
    );
  }

  void _handleDoubleTap() {
    HapticFeedback.lightImpact();
    setState(() {
      _showReactions = !_showReactions;
    });
  }

  Widget _buildMinimalReactions() {
    final groupedReactions = <String, int>{};
    for (final reaction in widget.message.reactions) {
      groupedReactions[reaction.reactionType] =
          (groupedReactions[reaction.reactionType] ?? 0) + 1;
    }

    return Wrap(
      spacing: 2,
      runSpacing: 2,
      children: groupedReactions.entries.map((entry) {
        return Container(
          padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 2),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                AppTheme.darkCard.withValues(alpha: 0.5),
                AppTheme.darkCard.withValues(alpha: 0.3),
              ],
            ),
            borderRadius: BorderRadius.circular(8),
            border: Border.all(
              color: AppTheme.darkBorder.withValues(alpha: 0.15),
              width: 0.5,
            ),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(_getEmojiForReaction(entry.key),
                  style: const TextStyle(fontSize: 10)),
              if (entry.value > 1) ...[
                const SizedBox(width: 2),
                Text(
                  entry.value.toString(),
                  style: AppTextStyles.caption.copyWith(
                    fontSize: 8,
                    fontWeight: FontWeight.w600,
                    color: AppTheme.textWhite.withValues(alpha: 0.6),
                  ),
                ),
              ],
            ],
          ),
        );
      }).toList(),
    );
  }

  String _getEmojiForReaction(String reactionType) {
    switch (reactionType) {
      case 'like':
        return '👍';
      case 'love':
        return '❤️';
      case 'laugh':
        return '😂';
      case 'sad':
        return '😢';
      case 'angry':
        return '😠';
      case 'wow':
        return '😮';
      default:
        return '👍';
    }
  }

  String _formatTime(DateTime dateTime) {
    return '${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }
}

// Bottom Sheet للخيارات
class _MessageOptionsSheet extends StatelessWidget {
  final bool isMe;
  final VoidCallback? onReply;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;

  const _MessageOptionsSheet({
    required this.isMe,
    this.onReply,
    this.onEdit,
    this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: const BorderRadius.vertical(top: Radius.circular(16)),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 20, sigmaY: 20),
        child: Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [
                AppTheme.darkCard.withValues(alpha: 0.85),
                AppTheme.darkCard.withValues(alpha: 0.9),
              ],
            ),
            borderRadius: const BorderRadius.vertical(top: Radius.circular(16)),
            border: Border.all(
              color: AppTheme.darkBorder.withValues(alpha: 0.08),
              width: 0.5,
            ),
          ),
          child: SafeArea(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                // مؤشر السحب
                Container(
                  width: 28,
                  height: 3,
                  margin: const EdgeInsets.symmetric(vertical: 8),
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      colors: [
                        AppTheme.darkBorder.withValues(alpha: 0.2),
                        AppTheme.darkBorder.withValues(alpha: 0.08),
                      ],
                    ),
                    borderRadius: BorderRadius.circular(1.5),
                  ),
                ),
                // خيار الرد
                if (onReply != null)
                  _buildOption(
                    context,
                    icon: Icons.reply_rounded,
                    title: 'رد',
                    onTap: () {
                      Navigator.pop(context);
                      onReply!.call();
                    },
                  ),
                // خيار التعديل (للمرسل فقط)
                if (isMe && onEdit != null)
                  _buildOption(
                    context,
                    icon: Icons.edit_rounded,
                    title: 'تعديل',
                    onTap: () {
                      Navigator.pop(context);
                      onEdit!.call();
                    },
                  ),
                // خيار النسخ
                _buildOption(
                  context,
                  icon: Icons.copy_rounded,
                  title: 'نسخ',
                  onTap: () {
                    Navigator.pop(context);
                    // نسخ النص إلى الحافظة
                    final message = context.read<ChatBloc>().state;
                    if (message is ChatLoaded) {
                      // يمكنك الوصول إلى نص الرسالة هنا
                      // Clipboard.setData(ClipboardData(text: messageText));
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text(
                            'تم نسخ الرسالة',
                            style: AppTextStyles.caption.copyWith(
                              color: AppTheme.textWhite,
                            ),
                          ),
                          backgroundColor: AppTheme.darkCard,
                          duration: const Duration(seconds: 1),
                          behavior: SnackBarBehavior.floating,
                          margin: const EdgeInsets.all(12),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                        ),
                      );
                    }
                  },
                ),
                // خيار الحذف (للمرسل فقط)
                if (isMe && onDelete != null)
                  _buildOption(
                    context,
                    icon: Icons.delete_rounded,
                    title: 'حذف',
                    onTap: () {
                      Navigator.pop(context);
                      // إظهار تأكيد الحذف
                      _showDeleteConfirmation(context);
                    },
                    isDestructive: true,
                  ),
                const SizedBox(height: 6),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildOption(
    BuildContext context, {
    required IconData icon,
    required String title,
    required VoidCallback onTap,
    bool isDestructive = false,
  }) {
    return InkWell(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        child: Row(
          children: [
            // أيقونة الخيار
            Container(
              width: 28,
              height: 28,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: isDestructive
                      ? [
                          AppTheme.error.withValues(alpha: 0.12),
                          AppTheme.error.withValues(alpha: 0.06),
                        ]
                      : [
                          AppTheme.primaryBlue.withValues(alpha: 0.08),
                          AppTheme.primaryPurple.withValues(alpha: 0.04),
                        ],
                ),
                borderRadius: BorderRadius.circular(6),
              ),
              child: Icon(
                icon,
                color: isDestructive
                    ? AppTheme.error.withValues(alpha: 0.8)
                    : AppTheme.primaryBlue.withValues(alpha: 0.8),
                size: 16,
              ),
            ),
            const SizedBox(width: 10),
            // نص الخيار
            Text(
              title,
              style: AppTextStyles.bodyMedium.copyWith(
                color: isDestructive
                    ? AppTheme.error.withValues(alpha: 0.8)
                    : AppTheme.textWhite.withValues(alpha: 0.8),
                fontSize: 13,
              ),
            ),
            const Spacer(),
            // سهم للأمام
            Icon(
              Icons.arrow_forward_ios_rounded,
              size: 12,
              color: isDestructive
                  ? AppTheme.error.withValues(alpha: 0.4)
                  : AppTheme.textMuted.withValues(alpha: 0.3),
            ),
          ],
        ),
      ),
    );
  }

  void _showDeleteConfirmation(BuildContext context) {
    showDialog(
      context: context,
      barrierDismissible: true,
      barrierColor: Colors.black.withValues(alpha: 0.5),
      builder: (BuildContext context) {
        return Dialog(
          backgroundColor: Colors.transparent,
          child: ClipRRect(
            borderRadius: BorderRadius.circular(12),
            child: BackdropFilter(
              filter: ImageFilter.blur(sigmaX: 20, sigmaY: 20),
              child: Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                    colors: [
                      AppTheme.darkCard.withValues(alpha: 0.85),
                      AppTheme.darkCard.withValues(alpha: 0.9),
                    ],
                  ),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(
                    color: AppTheme.darkBorder.withValues(alpha: 0.08),
                    width: 0.5,
                  ),
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    // أيقونة التحذير
                    Container(
                      width: 40,
                      height: 40,
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          colors: [
                            AppTheme.error.withValues(alpha: 0.12),
                            AppTheme.error.withValues(alpha: 0.06),
                          ],
                        ),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Icon(
                        Icons.delete_outline_rounded,
                        color: AppTheme.error.withValues(alpha: 0.8),
                        size: 20,
                      ),
                    ),
                    const SizedBox(height: 12),
                    // عنوان التحذير
                    Text(
                      'حذف الرسالة',
                      style: AppTextStyles.heading3.copyWith(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(height: 6),
                    // رسالة التحذير
                    Text(
                      'هل أنت متأكد من حذف هذه الرسالة؟',
                      style: AppTextStyles.bodySmall.copyWith(
                        color: AppTheme.textMuted.withValues(alpha: 0.6),
                        fontSize: 11,
                      ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 16),
                    // أزرار الإجراءات
                    Row(
                      children: [
                        // زر الإلغاء
                        Expanded(
                          child: GestureDetector(
                            onTap: () => Navigator.pop(context),
                            child: Container(
                              padding: const EdgeInsets.symmetric(vertical: 8),
                              decoration: BoxDecoration(
                                border: Border.all(
                                  color: AppTheme.darkBorder
                                      .withValues(alpha: 0.15),
                                  width: 0.5,
                                ),
                                borderRadius: BorderRadius.circular(8),
                              ),
                              child: Text(
                                'إلغاء',
                                style: AppTextStyles.bodySmall.copyWith(
                                  fontSize: 12,
                                  color:
                                      AppTheme.textWhite.withValues(alpha: 0.7),
                                ),
                                textAlign: TextAlign.center,
                              ),
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        // زر الحذف
                        Expanded(
                          child: GestureDetector(
                            onTap: () {
                              Navigator.pop(context);
                              onDelete?.call();
                            },
                            child: Container(
                              padding: const EdgeInsets.symmetric(vertical: 8),
                              decoration: BoxDecoration(
                                gradient: LinearGradient(
                                  colors: [
                                    AppTheme.error.withValues(alpha: 0.7),
                                    AppTheme.error.withValues(alpha: 0.5),
                                  ],
                                ),
                                borderRadius: BorderRadius.circular(8),
                              ),
                              child: Text(
                                'حذف',
                                style: AppTextStyles.bodySmall.copyWith(
                                  color: AppTheme.textWhite,
                                  fontSize: 12,
                                  fontWeight: FontWeight.w600,
                                ),
                                textAlign: TextAlign.center,
                              ),
                            ),
                          ),
                        ),
                      ],
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
}
