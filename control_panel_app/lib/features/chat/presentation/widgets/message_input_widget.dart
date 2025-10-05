import 'dart:io';
import 'dart:async';
import 'package:bookn_cp_app/features/chat/presentation/widgets/multi_image_picker_modal.dart';
import 'package:bookn_cp_app/features/chat/presentation/widgets/image_preview_screen.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:ui';
import 'package:image_picker/image_picker.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:record/record.dart';
import 'package:photo_manager/photo_manager.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/message.dart';
import '../bloc/chat_bloc.dart';
import '../models/image_upload_info.dart';

class MessageInputWidget extends StatefulWidget {
  final TextEditingController controller;
  final FocusNode focusNode;
  final String conversationId;
  final String? replyToMessageId;
  final Message? editingMessage;
  final Function(String) onSend;
  final VoidCallback? onAttachment;
  final VoidCallback? onLocation;
  final VoidCallback? onCancelReply;
  final VoidCallback? onCancelEdit;

  const MessageInputWidget({
    super.key,
    required this.controller,
    required this.focusNode,
    required this.conversationId,
    this.replyToMessageId,
    this.editingMessage,
    required this.onSend,
    this.onAttachment,
    this.onLocation,
    this.onCancelReply,
    this.onCancelEdit,
  });

  @override
  State<MessageInputWidget> createState() => _MessageInputWidgetState();
}

class _MessageInputWidgetState extends State<MessageInputWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _sendButtonAnimation;
  late Animation<double> _recordAnimation;

  final AudioRecorder _audioRecorder = AudioRecorder();
  bool _isRecording = false;
  bool _showAttachmentOptions = false;
  bool _showEmojiPicker = false;
  String _recordingPath = '';

  // Smooth progress tracking (UI-only) — kept minimal and driven by true bytes progress
  Timer? _progressTimer;
  double _currentDisplayedProgress = 0.0;
  double _targetProgress = 0.0;
  String? _currentUploadId;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 250),
      vsync: this,
    );
    _sendButtonAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOutBack,
    ));
    _recordAnimation = Tween<double>(
      begin: 0.9,
      end: 1.1,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
    ));

    widget.controller.addListener(_onTextChanged);
  }

  @override
  void dispose() {
    _progressTimer?.cancel();
    _animationController.dispose();
    _audioRecorder.dispose();
    super.dispose();
  }

  void _startSmoothProgress(String baseUploadId) {
    _currentUploadId = baseUploadId;
    _currentDisplayedProgress = 0.0;
    _targetProgress = 0.0;

    _progressTimer?.cancel();
    _progressTimer = Timer.periodic(const Duration(milliseconds: 50), (timer) {
      if (!mounted || _currentUploadId == null) {
        timer.cancel();
        return;
      }

      // Smoothly interpolate towards target progress using easing towards target
      if (_currentDisplayedProgress < _targetProgress) {
        // Move 10% of remaining gap per tick for responsive smoothing
        final gap = _targetProgress - _currentDisplayedProgress;
        _currentDisplayedProgress += gap * 0.1;
        if (_currentDisplayedProgress > _targetProgress) {
          _currentDisplayedProgress = _targetProgress;
        }

        // Update UI with smooth progress
        final bloc = context.read<ChatBloc>();

        // Extract base ID (everything before last underscore)
        final baseId =
            _currentUploadId!.substring(0, _currentUploadId!.lastIndexOf('_'));

        // Update all current uploads for this conversation with same displayed progress
        final state = bloc.state;
        if (state is ChatLoaded) {
          final currentUploads = state.uploadingImages[widget.conversationId] ??
              const <ImageUploadInfo>[];
          for (final u in currentUploads) {
            bloc.add(UpdateImageUploadProgressEvent(
              conversationId: widget.conversationId,
              uploadId: u.id,
              progress: _currentDisplayedProgress,
            ));
          }
        }
      }

      // Stop timer when complete
      if (_currentDisplayedProgress >= 1.0) {
        timer.cancel();
      }
    });
  }

  void _updateTargetProgress(double progress) {
    _targetProgress = progress;
  }

  void _stopSmoothProgress() {
    _progressTimer?.cancel();
    _currentDisplayedProgress = 0.0;
    _targetProgress = 0.0;
    _currentUploadId = null;
  }

  void _onTextChanged() {
    if (widget.controller.text.isNotEmpty && _sendButtonAnimation.value == 0) {
      _animationController.forward();
    } else if (widget.controller.text.isEmpty &&
        _sendButtonAnimation.value == 1) {
      _animationController.reverse();
    }
  }

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 15, sigmaY: 15),
        child: Container(
          padding: EdgeInsets.only(
            left: 8,
            right: 8,
            top: 4,
            bottom: MediaQuery.of(context).viewInsets.bottom > 0 ? 4 : 8,
          ),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [
                AppTheme.darkCard.withValues(alpha: 0.85),
                AppTheme.darkCard.withValues(alpha: 0.8),
              ],
            ),
            border: Border(
              top: BorderSide(
                color: AppTheme.darkBorder.withValues(alpha: 0.03),
                width: 0.5,
              ),
            ),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              if (_showAttachmentOptions) _buildMinimalAttachmentOptions(),
              if (_showEmojiPicker) _buildEmojiPicker(),
              Row(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  _buildMinimalAttachmentButton(),
                  const SizedBox(width: 5),
                  Expanded(child: _buildMinimalInputField()),
                  const SizedBox(width: 5),
                  _buildMinimalActionButton(),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildMinimalAttachmentOptions() {
    return Container(
      height: 64,
      margin: const EdgeInsets.only(bottom: 6),
      child: ListView(
        scrollDirection: Axis.horizontal,
        physics: const BouncingScrollPhysics(),
        children: [
          _MinimalAttachmentOption(
            icon: Icons.image_rounded,
            label: 'صورة',
            gradient: [
              AppTheme.primaryBlue.withValues(alpha: 0.8),
              AppTheme.primaryBlue.withValues(alpha: 0.6),
            ],
            onTap: () => _pickImage(ImageSource.gallery),
          ),
          _MinimalAttachmentOption(
            icon: Icons.camera_alt_rounded,
            label: 'كاميرا',
            gradient: [
              AppTheme.neonGreen.withValues(alpha: 0.8),
              AppTheme.neonGreen.withValues(alpha: 0.6),
            ],
            onTap: () => _pickImage(ImageSource.camera),
          ),
          _MinimalAttachmentOption(
            icon: Icons.videocam_rounded,
            label: 'فيديو',
            gradient: [
              AppTheme.error.withValues(alpha: 0.8),
              AppTheme.error.withValues(alpha: 0.6),
            ],
            onTap: _pickVideo,
          ),
          _MinimalAttachmentOption(
            icon: Icons.attach_file_rounded,
            label: 'ملف',
            gradient: [
              AppTheme.warning.withValues(alpha: 0.8),
              AppTheme.warning.withValues(alpha: 0.6),
            ],
            onTap: _pickFile,
          ),
          _MinimalAttachmentOption(
            icon: Icons.location_on_rounded,
            label: 'موقع',
            gradient: [
              AppTheme.primaryPurple.withValues(alpha: 0.8),
              AppTheme.primaryPurple.withValues(alpha: 0.6),
            ],
            onTap: () {
              setState(() {
                _showAttachmentOptions = false;
              });
              widget.onLocation?.call();
            },
          ),
        ],
      ),
    );
  }

  Widget _buildMinimalAttachmentButton() {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        setState(() {
          _showAttachmentOptions = !_showAttachmentOptions;
        });
      },
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        width: 32,
        height: 32,
        decoration: BoxDecoration(
          gradient: _showAttachmentOptions
              ? LinearGradient(
                  colors: [
                    AppTheme.primaryBlue.withValues(alpha: 0.8),
                    AppTheme.primaryPurple.withValues(alpha: 0.7),
                  ],
                )
              : null,
          color: !_showAttachmentOptions
              ? AppTheme.darkCard.withValues(alpha: 0.4)
              : null,
          borderRadius: BorderRadius.circular(8),
          border: Border.all(
            color: _showAttachmentOptions
                ? Colors.white.withValues(alpha: 0.15)
                : AppTheme.darkBorder.withValues(alpha: 0.15),
            width: 0.5,
          ),
        ),
        child: AnimatedRotation(
          duration: const Duration(milliseconds: 200),
          turns: _showAttachmentOptions ? 0.125 : 0,
          child: Icon(
            Icons.add_rounded,
            color: _showAttachmentOptions
                ? Colors.white
                : AppTheme.textMuted.withValues(alpha: 0.5),
            size: 18,
          ),
        ),
      ),
    );
  }

  Widget _buildMinimalInputField() {
    return Container(
      constraints: const BoxConstraints(
        minHeight: 32,
        maxHeight: 80,
      ),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.darkCard.withValues(alpha: 0.5),
            AppTheme.darkCard.withValues(alpha: 0.3),
          ],
        ),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: widget.focusNode.hasFocus
              ? AppTheme.primaryBlue.withValues(alpha: 0.2)
              : AppTheme.darkBorder.withValues(alpha: 0.08),
          width: 0.5,
        ),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          Expanded(
            child: TextField(
              controller: widget.controller,
              focusNode: widget.focusNode,
              maxLines: null,
              textInputAction: TextInputAction.newline,
              style: AppTextStyles.bodySmall.copyWith(
                fontSize: 12,
                color: AppTheme.textWhite.withValues(alpha: 0.9),
                height: 1.3,
              ),
              decoration: InputDecoration(
                hintText: widget.editingMessage != null
                    ? 'تعديل الرسالة...'
                    : widget.replyToMessageId != null
                        ? 'اكتب ردك...'
                        : 'اكتب رسالة...',
                hintStyle: AppTextStyles.bodySmall.copyWith(
                  color: AppTheme.textMuted.withValues(alpha: 0.35),
                  fontSize: 12,
                ),
                border: InputBorder.none,
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: 8,
                  vertical: 6,
                ),
                isDense: true,
              ),
              onSubmitted: (text) {
                if (text.trim().isNotEmpty) {
                  widget.onSend(text);
                }
              },
            ),
          ),
          GestureDetector(
            onTap: () {
              HapticFeedback.lightImpact();
              setState(() {
                _showEmojiPicker = !_showEmojiPicker;
              });
              if (_showEmojiPicker) {
                FocusScope.of(context).unfocus();
              } else {
                widget.focusNode.requestFocus();
              }
            },
            child: Container(
              padding: const EdgeInsets.all(6),
              child: Icon(
                Icons.emoji_emotions_outlined,
                color: AppTheme.textMuted.withValues(alpha: 0.35),
                size: 16,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildMinimalActionButton() {
    return AnimatedBuilder(
      animation: _sendButtonAnimation,
      builder: (context, child) {
        final showSend = _sendButtonAnimation.value > 0.5;

        return GestureDetector(
          onTap: showSend ? _sendMessage : null,
          onLongPress: !showSend ? _startRecording : null,
          onLongPressEnd: !showSend ? (_) => _stopRecording() : null,
          child: AnimatedContainer(
            duration: const Duration(milliseconds: 200),
            width: 32,
            height: 32,
            decoration: BoxDecoration(
              gradient: showSend || _isRecording
                  ? LinearGradient(
                      colors: [
                        AppTheme.primaryBlue.withValues(alpha: 0.8),
                        AppTheme.primaryPurple.withValues(alpha: 0.7),
                      ],
                    )
                  : null,
              color: !showSend && !_isRecording
                  ? AppTheme.darkCard.withValues(alpha: 0.4)
                  : null,
              shape: BoxShape.circle,
              border: Border.all(
                color: showSend || _isRecording
                    ? Colors.white.withValues(alpha: 0.15)
                    : AppTheme.darkBorder.withValues(alpha: 0.15),
                width: 0.5,
              ),
              boxShadow: showSend || _isRecording
                  ? [
                      BoxShadow(
                        color: AppTheme.primaryBlue.withValues(alpha: 0.2),
                        blurRadius: 6,
                        offset: const Offset(0, 1),
                      ),
                    ]
                  : null,
            ),
            child: Center(
              child: AnimatedSwitcher(
                duration: const Duration(milliseconds: 200),
                transitionBuilder: (child, animation) {
                  return ScaleTransition(
                    scale: animation,
                    child: FadeTransition(
                      opacity: animation,
                      child: child,
                    ),
                  );
                },
                child: _isRecording
                    ? _buildMinimalRecordingIndicator()
                    : Icon(
                        showSend ? Icons.send_rounded : Icons.mic_rounded,
                        color: showSend || _isRecording
                            ? Colors.white
                            : AppTheme.textMuted.withValues(alpha: 0.5),
                        size: 16,
                        key: ValueKey(showSend),
                      ),
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildMinimalRecordingIndicator() {
    return AnimatedBuilder(
      animation: _animationController,
      builder: (context, child) {
        return Transform.scale(
          scale: _recordAnimation.value,
          child: Container(
            width: 10,
            height: 10,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  AppTheme.error.withValues(alpha: 0.8),
                  AppTheme.error.withValues(alpha: 0.6),
                ],
              ),
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: AppTheme.error.withValues(alpha: 0.4),
                  blurRadius: 6,
                  spreadRadius: 1,
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  void _sendMessage() {
    final text = widget.controller.text.trim();
    if (text.isNotEmpty) {
      HapticFeedback.lightImpact();
      widget.onSend(text);
    }
  }

  Future<void> _startRecording() async {
    HapticFeedback.mediumImpact();

    final permission = await Permission.microphone.request();
    if (!permission.isGranted) return;

    if (await _audioRecorder.hasPermission()) {
      final directory = Directory.systemTemp;
      _recordingPath =
          '${directory.path}/audio_${DateTime.now().millisecondsSinceEpoch}.m4a';

      await _audioRecorder.start(
        const RecordConfig(),
        path: _recordingPath,
      );

      setState(() {
        _isRecording = true;
      });

      _animationController.repeat(reverse: true);
    }
  }

  Future<void> _stopRecording() async {
    if (!_isRecording) return;

    HapticFeedback.mediumImpact();
    _animationController.stop();

    final path = await _audioRecorder.stop();

    setState(() {
      _isRecording = false;
    });

    if (path != null) {
      _sendAudioMessage(path);
    }
  }

  void _sendAudioMessage(String path) {
    // Implement audio message sending
  }

  Future<void> _pickImage(ImageSource source) async {
    if (source == ImageSource.gallery) {
      // تحقق من صلاحية الصور قبل فتح نافذة الاختيار
      final state = await PhotoManager.requestPermissionExtend();
      if (state.isAuth || state == PermissionState.limited) {
        _showMultiImagePickerBottomSheet();
      } else {
        // جرّب منتقي النظام كبديل فوري (Android 13+/iOS لا يحتاج إذن قراءة)
        final systemPicked = await _pickImagesWithSystemPicker();
        if (systemPicked) return;

        // كخيار إضافي، افتح الإعدادات ثم أعد المحاولة تلقائياً عند العودة
        try {
          await PhotoManager.openSetting();
        } catch (_) {
          await openAppSettings();
        }
        await Future.delayed(const Duration(milliseconds: 300));
        if (!mounted) return;
        final retry = await PhotoManager.requestPermissionExtend();
        if (retry.isAuth || retry == PermissionState.limited) {
          _showMultiImagePickerBottomSheet();
        } else {
          // محاولة أخيرة عبر منتقي النظام
          final picked = await _pickImagesWithSystemPicker();
          if (!picked && mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(
                  'لا يزال الوصول إلى الصور مرفوضًا. يرجى السماح من الإعدادات.',
                  style: AppTextStyles.bodySmall.copyWith(color: Colors.white),
                ),
                backgroundColor: AppTheme.error.withValues(alpha: 0.9),
              ),
            );
          }
        }
      }
    } else {
      // التقاط صورة واحدة من الكاميرا
      final picker = ImagePicker();
      final image = await picker.pickImage(
        source: source,
        maxWidth: 1920,
        maxHeight: 1920,
        imageQuality: 85,
      );

      if (image != null) {
        _showImagePreviewScreen([File(image.path)]);
      }
    }
  }

  void _showMultiImagePickerBottomSheet() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) => MultiImagePickerModal(
        onImagesSelected: (images) {
          _sendMultipleImages(images);
        },
        maxImages: 10,
      ),
    );
  }

  Future<bool> _pickImagesWithSystemPicker() async {
    try {
      final picker = ImagePicker();
      final picks = await picker.pickMultiImage(
        maxWidth: 1920,
        maxHeight: 1920,
        imageQuality: 85,
      );
      if (picks.isNotEmpty) {
        if (!mounted) return true;
        final images = picks.map((x) => File(x.path)).toList();
        _showImagePreviewScreen(images);
        return true;
      }
    } catch (_) {}
    return false;
  }

  void _showImagePreviewScreen(List<File> images) {
    Navigator.push(
      context,
      PageRouteBuilder(
        pageBuilder: (context, animation, secondaryAnimation) =>
            ImagePreviewScreen(
          images: images,
          onSend: (editedImages) {
            Navigator.pop(context); // Close preview
            _sendMultipleImages(editedImages);
          },
        ),
        transitionDuration: const Duration(milliseconds: 300),
        transitionsBuilder: (context, animation, secondaryAnimation, child) {
          return FadeTransition(
            opacity: animation,
            child: ScaleTransition(
              scale: Tween<double>(
                begin: 0.95,
                end: 1.0,
              ).animate(animation),
              child: child,
            ),
          );
        },
      ),
    );
  }

  void _sendMultipleImages(List<File> images) {
    if (images.isEmpty) return;

    // إنشاء رسالة مؤقتة مع الصور
    final tempMessageId = DateTime.now().millisecondsSinceEpoch.toString();
    final uploadInfos = <ImageUploadInfo>[];
    for (int i = 0; i < images.length; i++) {
      uploadInfos.add(ImageUploadInfo(
        id: '${tempMessageId}_$i',
        file: images[i],
        progress: 0.0,
      ));
    }

    // إعلام الـ Bloc ببدء عملية رفع الصور لإظهار فقاعة الرفع داخل الشات
    context.read<ChatBloc>().add(StartImageUploadsEvent(
          conversationId: widget.conversationId,
          uploads: uploadInfos,
        ));

    // بدء رفع الصور بالتتابع مع تقدم حقيقي بالبايتات
    _uploadImagesWithProgress(images, tempMessageId, uploadInfos);
  }

  Future<void> _uploadImagesWithProgress(
    List<File> images,
    String tempMessageId,
    List<ImageUploadInfo> uploadInfos,
  ) async {
    final bloc = context.read<ChatBloc>();
    final filePaths = images.map((f) => f.path).toList();
    final totalImages = filePaths.length;
    // We will upload and send each image as a separate message to satisfy the requirement

    try {
      for (int i = 0; i < images.length; i++) {
        // Start smooth progress animation for this image
        _startSmoothProgress(uploadInfos[i].id);
        final filePath = images[i].path;
        final uploadId = uploadInfos[i].id;
        await bloc
            .uploadAttachmentWithProgress(
              conversationId: widget.conversationId,
              filePath: filePath,
              messageType: 'image',
              onProgress: (sent, total) {
                final t = total > 0 ? total : images[i].lengthSync();
                final p = t > 0 ? sent / t : 0.0;
                bloc.add(UpdateImageUploadProgressEvent(
                  conversationId: widget.conversationId,
                  uploadId: uploadId,
                  progress: p,
                ));
                _updateTargetProgress(p);
              },
            )
            .then((_) async {
          bloc.add(UpdateImageUploadProgressEvent(
            conversationId: widget.conversationId,
            uploadId: uploadId,
            progress: 1.0,
            isCompleted: true,
          ));
        }).whenComplete(() {
          _stopSmoothProgress();
        });
      }

      // Clear the uploading bubble once all are done
      if (mounted) {
        bloc.add(FinishImageUploadsEvent(conversationId: widget.conversationId));
      }
    } catch (e) {
      for (int i = 0; i < images.length; i++) {
        final uploadId = '${tempMessageId}_$i';
        bloc.add(UpdateImageUploadProgressEvent(
          conversationId: widget.conversationId,
          uploadId: uploadId,
          isFailed: true,
          error: e.toString(),
        ));
      }
    } finally {
      // Ensure smooth animator stopped
      _stopSmoothProgress();
    }
  }

  Future<void> _pickVideo() async {
    final picker = ImagePicker();
    final video = await picker.pickVideo(
      source: ImageSource.gallery,
      maxDuration: const Duration(minutes: 5),
    );

    if (video != null) {
      setState(() {
        _showAttachmentOptions = false;
      });
      if (!mounted) return;
      context.read<ChatBloc>().add(
            UploadAttachmentEvent(
              conversationId: widget.conversationId,
              filePath: video.path,
              messageType: 'video',
            ),
          );
    }
  }

  void _pickFile() {
    setState(() {
      _showAttachmentOptions = false;
    });
    // Implement file picker
  }

  Widget _buildEmojiPicker() {
    // Lightweight custom emoji grid to avoid external deps; can be replaced with emoji_picker_flutter.
    const emojis = [
      '😀','😁','😂','🤣','😊','😍','😘','😜','😎','😢','😭','😡','👍','👎','🙏','👏','🔥','🎉','💯','❤️','💔','😮','🤔','🤗','😴','🤯','😇','😉','😅','😏'
    ];
    return Container(
      height: 220,
      margin: const EdgeInsets.only(bottom: 6),
      decoration: BoxDecoration(
        gradient: LinearGradient(colors: [
          AppTheme.darkCard.withValues(alpha: 0.8),
          AppTheme.darkCard.withValues(alpha: 0.7),
        ]),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppTheme.darkBorder.withValues(alpha: 0.08),
          width: 0.5,
        ),
      ),
      child: GridView.builder(
        padding: const EdgeInsets.all(8),
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: 8,
          mainAxisSpacing: 6,
          crossAxisSpacing: 6,
        ),
        itemCount: emojis.length,
        itemBuilder: (context, index) {
          final emoji = emojis[index];
          return GestureDetector(
            onTap: () {
              HapticFeedback.selectionClick();
              final text = widget.controller.text;
              final selection = widget.controller.selection;
              final base = selection.baseOffset;
              final extent = selection.extentOffset;
              if (base >= 0 && extent >= 0 && base <= text.length && extent <= text.length) {
                final start = text.substring(0, base);
                final end = text.substring(extent);
                widget.controller.text = '$start$emoji$end';
                final newPos = base + emoji.length;
                widget.controller.selection = TextSelection.collapsed(offset: newPos);
              } else {
                widget.controller.text = '$text$emoji';
                widget.controller.selection = TextSelection.collapsed(offset: widget.controller.text.length);
              }
            },
            child: Center(
              child: Text(
                emoji,
                style: const TextStyle(fontSize: 22),
              ),
            ),
          );
        },
      ),
    );
  }
}

class _MinimalAttachmentOption extends StatelessWidget {
  final IconData icon;
  final String label;
  final List<Color> gradient;
  final VoidCallback onTap;

  const _MinimalAttachmentOption({
    required this.icon,
    required this.label,
    required this.gradient,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        onTap();
      },
      child: Container(
        width: 56,
        margin: const EdgeInsets.symmetric(horizontal: 3),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              width: 38,
              height: 38,
              decoration: BoxDecoration(
                gradient: LinearGradient(colors: gradient),
                borderRadius: BorderRadius.circular(10),
                boxShadow: [
                  BoxShadow(
                    color: gradient.first.withValues(alpha: 0.2),
                    blurRadius: 6,
                    offset: const Offset(0, 2),
                  ),
                ],
              ),
              child: Icon(
                icon,
                color: Colors.white,
                size: 20,
              ),
            ),
            const SizedBox(height: 3),
            Text(
              label,
              style: AppTextStyles.caption.copyWith(
                fontSize: 9,
                color: AppTheme.textMuted.withValues(alpha: 0.6),
              ),
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
      ),
    );
  }
}
