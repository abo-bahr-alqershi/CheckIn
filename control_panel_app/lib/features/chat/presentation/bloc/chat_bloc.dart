import 'dart:async';
import 'dart:io';
import 'package:bloc/bloc.dart';
import 'package:bookn_cp_app/features/chat/presentation/models/image_upload_info.dart';
import 'package:equatable/equatable.dart';
import 'package:flutter/foundation.dart';
import 'package:collection/collection.dart';
// removed invalid package:/ imports
import '../../../../core/error/failures.dart';
import '../../../../services/websocket_service.dart';
import '../../domain/entities/attachment.dart';
import '../../domain/entities/conversation.dart';
import '../../domain/entities/message.dart';
import 'dart:async';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
// removed invalid package:/ import
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/conversation.dart';
import '../../domain/entities/message.dart';
import '../../domain/entities/attachment.dart';
import '../../domain/repositories/chat_repository.dart';
import '../../domain/usecases/get_conversations_usecase.dart';
import '../../domain/usecases/get_messages_usecase.dart';
import '../../domain/usecases/send_message_usecase.dart';
import '../../domain/usecases/create_conversation_usecase.dart';
import '../../domain/usecases/delete_conversation_usecase.dart';
import '../../domain/usecases/archive_conversation_usecase.dart';
import '../../domain/usecases/unarchive_conversation_usecase.dart';
import '../../domain/usecases/delete_message_usecase.dart';
import '../../domain/usecases/edit_message_usecase.dart';
import '../../domain/usecases/add_reaction_usecase.dart';
import '../../domain/usecases/remove_reaction_usecase.dart';
import '../../domain/usecases/mark_as_read_usecase.dart';
import '../../../auth/domain/usecases/get_current_user_usecase.dart';
import '../../domain/usecases/send_typing_indicator_usecase.dart';
import '../../domain/usecases/upload_attachment_usecase.dart';
import '../../domain/usecases/search_chats_usecase.dart';
import '../../domain/usecases/get_available_users_usecase.dart';
import '../../domain/usecases/get_admin_users_usecase.dart';
import '../../domain/usecases/update_user_status_usecase.dart';
import '../../domain/usecases/get_chat_settings_usecase.dart';
import '../../domain/usecases/update_chat_settings_usecase.dart';
part 'chat_event.dart';
part 'chat_state.dart';

/// BLoC لإدارة الشات بالكامل مع جميع المزايا المتقدمة
class ChatBloc extends Bloc<ChatEvent, ChatState> {
  // Use Cases
  final GetConversationsUseCase getConversationsUseCase;
  final GetMessagesUseCase getMessagesUseCase;
  final SendMessageUseCase sendMessageUseCase;
  final CreateConversationUseCase createConversationUseCase;
  final DeleteConversationUseCase deleteConversationUseCase;
  final ArchiveConversationUseCase archiveConversationUseCase;
  final UnarchiveConversationUseCase unarchiveConversationUseCase;
  final DeleteMessageUseCase deleteMessageUseCase;
  final EditMessageUseCase editMessageUseCase;
  final AddReactionUseCase addReactionUseCase;
  final RemoveReactionUseCase removeReactionUseCase;
  final MarkAsReadUseCase markMessagesAsReadUseCase;
  final UploadAttachmentUseCase uploadAttachmentUseCase;
  final SearchChatsUseCase searchChatsUseCase;
  final GetAvailableUsersUseCase getAvailableUsersUseCase;
  final GetAdminUsersUseCase getAdminUsersUseCase;
  final UpdateUserStatusUseCase updateUserStatusUseCase;
  final GetChatSettingsUseCase getChatSettingsUseCase;
  final UpdateChatSettingsUseCase updateChatSettingsUseCase;
  final SendTypingIndicatorUseCase sendTypingIndicatorUseCase;
  final GetCurrentUserUseCase getCurrentUserUseCase;

  // WebSocket Service
  final ChatWebSocketService webSocketService;

  // الاشتراكات
  StreamSubscription? _webSocketMessagesSubscription;
  StreamSubscription? _webSocketTypingSubscription;
  StreamSubscription? _webSocketPresenceSubscription;
  StreamSubscription? _webSocketConversationSubscription;

  // حالات التحميل لكل محادثة
  final Map<String, bool> _conversationLoadingStates = {};
  final Map<String, int> _messagePages = {};
  final Map<String, bool> _hasMoreMessages = {};

  // ذاكرة تخزين مؤقتة للرسائل المحملة
  final Map<String, Set<String>> _loadedMessageIds = {};

  // قائمة انتظار الرسائل المتفائلة
  final Map<String, List<Message>> _optimisticMessages = {};

  // تتبع عمليات الرفع النشطة
  final Map<String, UploadTask> _activeUploads = {};

  ChatBloc({
    required this.getConversationsUseCase,
    required this.getMessagesUseCase,
    required this.sendMessageUseCase,
    required this.createConversationUseCase,
    required this.deleteConversationUseCase,
    required this.archiveConversationUseCase,
    required this.unarchiveConversationUseCase,
    required this.deleteMessageUseCase,
    required this.editMessageUseCase,
    required this.addReactionUseCase,
    required this.removeReactionUseCase,
    required this.markMessagesAsReadUseCase,
    required this.uploadAttachmentUseCase,
    required this.searchChatsUseCase,
    required this.getAvailableUsersUseCase,
    required this.getAdminUsersUseCase,
    required this.updateUserStatusUseCase,
    required this.getChatSettingsUseCase,
    required this.updateChatSettingsUseCase,
    required this.sendTypingIndicatorUseCase,
    required this.getCurrentUserUseCase,
    required this.webSocketService,
  }) : super(const ChatInitial()) {
    // تسجيل معالجات الأحداث
    on<InitializeChatEvent>(_onInitializeChat);
    on<LoadConversationsEvent>(_onLoadConversations);
    on<LoadMessagesEvent>(_onLoadMessages);
    on<LoadMoreMessagesEvent>(_onLoadMoreMessages);
    on<SendMessageEvent>(_onSendMessage);
    on<CreateConversationEvent>(_onCreateConversation);
    on<DeleteConversationEvent>(_onDeleteConversation);
    on<ArchiveConversationEvent>(_onArchiveConversation);
    on<UnarchiveConversationEvent>(_onUnarchiveConversation);
    on<DeleteMessageEvent>(_onDeleteMessage);
    on<EditMessageEvent>(_onEditMessage);
    on<AddReactionEvent>(_onAddReaction);
    on<RemoveReactionEvent>(_onRemoveReaction);
    on<MarkMessagesAsReadEvent>(_onMarkMessagesAsRead);
    on<UploadAttachmentEvent>(_onUploadAttachment);
    on<UploadMultipleAttachmentsEvent>(_onUploadMultipleAttachments);
    on<SearchChatsEvent>(_onSearchChats);
    on<LoadAvailableUsersEvent>(_onLoadAvailableUsers);
    on<LoadAdminUsersEvent>(_onLoadAdminUsers);
    on<UpdateUserStatusEvent>(_onUpdateUserStatus);
    on<LoadChatSettingsEvent>(_onLoadChatSettings);
    on<UpdateChatSettingsEvent>(_onUpdateChatSettings);
    on<SendTypingIndicatorEvent>(_onSendTypingIndicator);
    on<WebSocketMessageReceivedEvent>(_onWebSocketMessageReceived);
    on<WebSocketConversationUpdatedEvent>(_onWebSocketConversationUpdated);
    on<WebSocketTypingIndicatorEvent>(_onWebSocketTypingIndicator);
    on<WebSocketPresenceUpdateEvent>(_onWebSocketPresenceUpdate);
    on<ClearConversationCacheEvent>(_onClearConversationCache);
    on<RefreshConversationEvent>(_onRefreshConversation);
    on<RetryFailedMessageEvent>(_onRetryFailedMessage);
    on<CancelUploadEvent>(_onCancelUpload);

    // تهيئة WebSocket
    _initializeWebSocket();
  }

  /// تهيئة اتصال WebSocket والاستماع للأحداث
  void _initializeWebSocket() {
    // الاستماع لرسائل WebSocket
    _webSocketMessagesSubscription =
        webSocketService.messageEvents.listen((event) {
      add(WebSocketMessageReceivedEvent(event));
    });

    // الاستماع لمؤشرات الكتابة
    _webSocketTypingSubscription =
        webSocketService.typingEvents.listen((event) {
      add(WebSocketTypingIndicatorEvent(
        conversationId: event.conversationId,
        typingUserIds: event.typingUserIds,
      ));
    });

    // الاستماع لتحديثات الحضور
    _webSocketPresenceSubscription =
        webSocketService.presenceEvents.listen((event) {
      add(WebSocketPresenceUpdateEvent(
        userId: event.userId,
        status: event.status,
        lastSeen: event.lastSeen,
      ));
    });

    // الاستماع لتحديثات المحادثات
    _webSocketConversationSubscription =
        webSocketService.conversationEvents.listen((event) {
      if (event.conversation != null) {
        add(WebSocketConversationUpdatedEvent(event.conversation!));
      }
    });

    // الاتصال بـ WebSocket
    webSocketService.connect();
  }

  /// تهيئة الشات
  Future<void> _onInitializeChat(
    InitializeChatEvent event,
    Emitter<ChatState> emit,
  ) async {
    emit(const ChatLoading());

    try {
      // جلب المحادثات
      final conversationsResult = await getConversationsUseCase(
        const GetConversationsParams(),
      );

      // جلب الإعدادات
      final settingsResult = await getChatSettingsUseCase(NoParams());

      // جلب المستخدم الحالي
      final currentUserResult = await getCurrentUserUseCase(NoParams());

      await conversationsResult.fold(
        (failure) async =>
            emit(ChatError(message: _mapFailureToMessage(failure))),
        (conversations) async {
          ChatSettings? settings;
          await settingsResult.fold(
            (_) async {}, // تجاهل فشل الإعدادات
            (s) async => settings = s,
          );

          String? currentUserId;
          await currentUserResult.fold(
            (_) async {},
            (user) async => currentUserId = user.userId,
          );

          emit(ChatLoaded(
            conversations: conversations,
            settings: settings,
            currentUserId: currentUserId,
          ));
        },
      );
    } catch (e) {
      emit(ChatError(message: 'حدث خطأ في تهيئة الشات: ${e.toString()}'));
    }
  }

  /// تحميل المحادثات
  Future<void> _onLoadConversations(
    LoadConversationsEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) {
      emit(const ChatLoading());
    }

    final result = await getConversationsUseCase(
      GetConversationsParams(
        pageNumber: event.pageNumber,
        pageSize: event.pageSize,
      ),
    );

    await result.fold(
      (failure) async {
        if (currentState is ChatLoaded) {
          emit(currentState.copyWith(
            error: _mapFailureToMessage(failure),
          ));
        } else {
          emit(ChatError(message: _mapFailureToMessage(failure)));
        }
      },
      (conversations) async {
        if (currentState is ChatLoaded) {
          // دمج المحادثات الجديدة مع القديمة
          final mergedConversations = _mergeConversations(
            currentState.conversations,
            conversations,
          );

          emit(currentState.copyWith(
            conversations: mergedConversations,
            error: null,
          ));
        } else {
          emit(ChatLoaded(conversations: conversations));
        }
      },
    );
  }

  /// تحميل الرسائل
  Future<void> _onLoadMessages(
    LoadMessagesEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    // تحقق من حالة التحميل
    if (_conversationLoadingStates[event.conversationId] == true) {
      return;
    }

    _conversationLoadingStates[event.conversationId] = true;

    emit(currentState.copyWith(
      isLoadingMessages: true,
      loadingConversationId: event.conversationId,
    ));

    final result = await getMessagesUseCase(
      GetMessagesParams(
        conversationId: event.conversationId,
        pageNumber: event.pageNumber,
        pageSize: event.pageSize,
        beforeMessageId: event.beforeMessageId,
      ),
    );

    await result.fold(
      (failure) async {
        _conversationLoadingStates[event.conversationId] = false;
        emit(currentState.copyWith(
          isLoadingMessages: false,
          error: _mapFailureToMessage(failure),
        ));
      },
      (messages) async {
        // تحديث معلومات الصفحات
        _messagePages[event.conversationId] = event.pageNumber;
        _hasMoreMessages[event.conversationId] =
            messages.length >= event.pageSize;

        // تتبع معرفات الرسائل المحملة
        _loadedMessageIds.putIfAbsent(event.conversationId, () => {});
        for (final message in messages) {
          _loadedMessageIds[event.conversationId]!.add(message.id);
        }

        // دمج الرسائل
        final updatedMessages =
            Map<String, List<Message>>.from(currentState.messages);
        final existingMessages = updatedMessages[event.conversationId] ?? [];

        final mergedMessages = _mergeMessages(existingMessages, messages);
        updatedMessages[event.conversationId] = mergedMessages;

        _conversationLoadingStates[event.conversationId] = false;

        emit(currentState.copyWith(
          messages: updatedMessages,
          isLoadingMessages: false,
          loadingConversationId: null,
          error: null,
        ));
      },
    );
  }

  /// تحميل المزيد من الرسائل (التمرير اللانهائي)
  Future<void> _onLoadMoreMessages(
    LoadMoreMessagesEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    // تحقق من وجود المزيد من الرسائل
    if (_hasMoreMessages[event.conversationId] == false) return;

    // تحقق من حالة التحميل
    if (_conversationLoadingStates[event.conversationId] == true) return;

    _conversationLoadingStates[event.conversationId] = true;

    emit(currentState.copyWith(isLoadingMore: true));

    final currentPage = _messagePages[event.conversationId] ?? 1;
    final nextPage = currentPage + 1;

    final result = await getMessagesUseCase(
      GetMessagesParams(
        conversationId: event.conversationId,
        pageNumber: nextPage,
        pageSize: event.pageSize,
        beforeMessageId: event.targetMessageId,
      ),
    );

    await result.fold(
      (failure) async {
        _conversationLoadingStates[event.conversationId] = false;
        emit(currentState.copyWith(
          isLoadingMore: false,
          error: _mapFailureToMessage(failure),
        ));
      },
      (messages) async {
        // تحديث معلومات الصفحات
        _messagePages[event.conversationId] = nextPage;
        _hasMoreMessages[event.conversationId] =
            messages.length >= event.pageSize;

        // تتبع معرفات الرسائل المحملة
        for (final message in messages) {
          _loadedMessageIds[event.conversationId]!.add(message.id);
        }

        // دمج الرسائل القديمة مع الجديدة
        final updatedMessages =
            Map<String, List<Message>>.from(currentState.messages);
        final existingMessages = updatedMessages[event.conversationId] ?? [];

        // إضافة الرسائل القديمة في النهاية (التمرير لأعلى)
        final mergedMessages = [...existingMessages, ...messages]
          ..sort((a, b) => b.createdAt.compareTo(a.createdAt));

        updatedMessages[event.conversationId] = mergedMessages;

        _conversationLoadingStates[event.conversationId] = false;

        emit(currentState.copyWith(
          messages: updatedMessages,
          isLoadingMore: false,
          error: null,
        ));
      },
    );
  }

  /// إرسال رسالة مع التحديث المتفائل
  Future<void> _onSendMessage(
    SendMessageEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    // إنشاء رسالة متفائلة
    final optimisticMessage = Message(
      id: 'temp_${DateTime.now().microsecondsSinceEpoch}',
      conversationId: event.conversationId,
      senderId: event.currentUserId ?? currentState.currentUserId ?? '',
      senderName: currentState.currentUserName,
      messageType: event.messageType,
      content: event.content,
      location: event.location,
      replyToMessageId: event.replyToMessageId,
      attachments: const [],
      reactions: const [],
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
      status: 'sending',
      isEdited: false,
      isDeleted: false,
      deliveryReceipt: null,
    );

    // إضافة الرسالة المتفائلة للقائمة
    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);
    final conversationMessages = List<Message>.from(
      updatedMessages[event.conversationId] ?? [],
    );
    conversationMessages.insert(0, optimisticMessage);
    updatedMessages[event.conversationId] = conversationMessages;

    // تتبع الرسالة المتفائلة
    _optimisticMessages.putIfAbsent(event.conversationId, () => []);
    _optimisticMessages[event.conversationId]!.add(optimisticMessage);

    emit(currentState.copyWith(
      messages: updatedMessages,
      isSendingMessage: true,
      sendingConversationId: event.conversationId,
    ));

    // إرسال الرسالة الفعلية
    final result = await sendMessageUseCase(
      SendMessageParams(
        conversationId: event.conversationId,
        messageType: event.messageType,
        content: event.content,
        location: event.location,
        replyToMessageId: event.replyToMessageId,
        attachmentIds: event.attachmentIds,
      ),
    );

    await result.fold(
      (failure) async {
        // فشل الإرسال - تحديث حالة الرسالة المتفائلة
        final failedMessages =
            Map<String, List<Message>>.from(currentState.messages);
        final messages = failedMessages[event.conversationId] ?? [];

        final updatedMessagesList = messages.map((msg) {
          if (msg.id == optimisticMessage.id) {
            return Message(
              id: msg.id,
              conversationId: msg.conversationId,
              senderId: msg.senderId,
              senderName: msg.senderName,
              messageType: msg.messageType,
              content: msg.content,
              location: msg.location,
              replyToMessageId: msg.replyToMessageId,
              attachments: msg.attachments,
              reactions: msg.reactions,
              createdAt: msg.createdAt,
              updatedAt: msg.updatedAt,
              status: 'failed',
              isEdited: msg.isEdited,
              isDeleted: msg.isDeleted,
              deliveryReceipt: msg.deliveryReceipt,
              failureReason: _mapFailureToMessage(failure),
            );
          }
          return msg;
        }).toList();

        failedMessages[event.conversationId] = updatedMessagesList;

        emit(currentState.copyWith(
          messages: failedMessages,
          isSendingMessage: false,
          error: 'فشل إرسال الرسالة',
        ));
      },
      (sentMessage) async {
        // نجح الإرسال - استبدال الرسالة المتفائلة بالرسالة الفعلية
        final successMessages =
            Map<String, List<Message>>.from(currentState.messages);
        final messages = successMessages[event.conversationId] ?? [];

        // إزالة الرسالة المتفائلة وإضافة الرسالة الفعلية
        final updatedMessagesList =
            messages.where((msg) => msg.id != optimisticMessage.id).toList();

        // إضافة الرسالة الفعلية في المقدمة
        updatedMessagesList.insert(0, sentMessage);
        successMessages[event.conversationId] = updatedMessagesList;

        // إزالة من قائمة الرسائل المتفائلة
        _optimisticMessages[event.conversationId]?.removeWhere(
          (msg) => msg.id == optimisticMessage.id,
        );

        // تحديث آخر رسالة في المحادثة
        final updatedConversations = currentState.conversations.map((conv) {
          if (conv.id == event.conversationId) {
            return Conversation(
              id: conv.id,
              conversationType: conv.conversationType,
              title: conv.title,
              description: conv.description,
              avatar: conv.avatar,
              createdAt: conv.createdAt,
              updatedAt: DateTime.now(),
              lastMessage: sentMessage,
              unreadCount: conv.unreadCount,
              isArchived: conv.isArchived,
              isMuted: conv.isMuted,
              propertyId: conv.propertyId,
              participants: conv.participants,
            );
          }
          return conv;
        }).toList();

        emit(currentState.copyWith(
          messages: successMessages,
          conversations: updatedConversations,
          isSendingMessage: false,
          sendingConversationId: null,
          error: null,
        ));
      },
    );
  }

  /// رفع المرفقات المتعددة
  Future<void> _onUploadMultipleAttachments(
    UploadMultipleAttachmentsEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final uploadTasks = <ImageUploadInfo>[];
    final attachmentIds = <String>[];

    // إنشاء مهام الرفع
    for (int i = 0; i < event.filePaths.length; i++) {
      final filePath = event.filePaths[i];
      final uploadId = 'upload_${DateTime.now().microsecondsSinceEpoch}_$i';

      uploadTasks.add(ImageUploadInfo(
        id: uploadId,
        file: File(filePath),
        progress: 0.0,
        isCompleted: false,
        isFailed: false,
      ));
    }

    // تحديث الحالة مع مهام الرفع الجديدة
    final updatedUploads = Map<String, List<ImageUploadInfo>>.from(
      currentState.uploadingImages,
    );
    updatedUploads[event.conversationId] = uploadTasks;

    emit(currentState.copyWith(uploadingImages: updatedUploads));

    // رفع كل ملف بشكل متتابع
    for (int i = 0; i < event.filePaths.length; i++) {
      final filePath = event.filePaths[i];
      final uploadTask = uploadTasks[i];

      try {
        // إنشاء مهمة رفع قابلة للإلغاء
        final uploadController = UploadController();
        _activeUploads[uploadTask.id] = UploadTask(
          controller: uploadController,
          filePath: filePath,
        );

        final result = await uploadAttachmentUseCase(
          UploadAttachmentParams(
            conversationId: event.conversationId,
            filePath: filePath,
            messageType: event.messageType,
            onSendProgress: (sent, total) {
              // تحديث تقدم الرفع
              final progress = total > 0 ? sent / total : 0.0;
              _updateUploadProgress(
                emit,
                currentState,
                event.conversationId,
                uploadTask.id,
                progress,
              );
              event.onProgress?.call(i, sent, total);
            },
            // CancelToken simulation removed from use case params; progress handled via onSendProgress
          ),
        );

        await result.fold(
          (failure) async {
            // فشل الرفع
            _updateUploadStatus(
              emit,
              currentState,
              event.conversationId,
              uploadTask.id,
              isFailed: true,
              error: _mapFailureToMessage(failure),
            );
          },
          (attachment) async {
            // نجح الرفع
            attachmentIds.add(attachment.id);
            _updateUploadStatus(
              emit,
              currentState,
              event.conversationId,
              uploadTask.id,
              isCompleted: true,
            );
          },
        );
      } catch (e) {
        // خطأ في الرفع
        _updateUploadStatus(
          emit,
          currentState,
          event.conversationId,
          uploadTask.id,
          isFailed: true,
          error: e.toString(),
        );
      } finally {
        _activeUploads.remove(uploadTask.id);
      }
    }

    // إرسال رسالة واحدة بجميع المرفقات
    if (attachmentIds.isNotEmpty) {
      add(SendMessageEvent(
        conversationId: event.conversationId,
        messageType: event.messageType,
        attachmentIds: attachmentIds,
        currentUserId: currentState.currentUserId,
      ));
    }

    // إزالة مهام الرفع من الحالة
    final finalUploads = Map<String, List<ImageUploadInfo>>.from(
      currentState.uploadingImages,
    );
    finalUploads.remove(event.conversationId);
    emit(currentState.copyWith(uploadingImages: finalUploads));
  }

  /// رفع مرفق واحد
  Future<void> _onUploadAttachment(
    UploadAttachmentEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final uploadId = 'upload_${DateTime.now().microsecondsSinceEpoch}';
    final task = ImageUploadInfo(
      id: uploadId,
      file: File(event.filePath),
      progress: 0.0,
      isCompleted: false,
      isFailed: false,
    );

    final updatedUploads = Map<String, List<ImageUploadInfo>>.from(
      currentState.uploadingImages,
    );
    final list = List<ImageUploadInfo>.from(
      updatedUploads[event.conversationId] ?? const [],
    )..add(task);
    updatedUploads[event.conversationId] = list;
    emit(currentState.copyWith(uploadingImages: updatedUploads));

    try {
      final controller = UploadController();
      _activeUploads[uploadId] = UploadTask(
        controller: controller,
        filePath: event.filePath,
      );

      final result = await uploadAttachmentUseCase(
        UploadAttachmentParams(
          conversationId: event.conversationId,
          filePath: event.filePath,
          messageType: event.messageType,
          onSendProgress: (sent, total) {
            final progress = total > 0 ? sent / total : 0.0;
            _updateUploadProgress(
              emit,
              currentState,
              event.conversationId,
              uploadId,
              progress,
            );
            event.onProgress?.call(sent, total);
          },
        ),
      );

      await result.fold(
        (failure) async {
          _updateUploadStatus(
            emit,
            currentState,
            event.conversationId,
            uploadId,
            isFailed: true,
            error: _mapFailureToMessage(failure),
          );
        },
        (attachment) async {
          _updateUploadStatus(
            emit,
            currentState,
            event.conversationId,
            uploadId,
            isCompleted: true,
          );

          // أرسل رسالة تحتوي هذا المرفق فقط
          add(SendMessageEvent(
            conversationId: event.conversationId,
            messageType: event.messageType,
            attachmentIds: [attachment.id],
            currentUserId: currentState.currentUserId,
          ));
        },
      );
    } catch (e) {
      _updateUploadStatus(
        emit,
        currentState,
        event.conversationId,
        uploadId,
        isFailed: true,
        error: e.toString(),
      );
    } finally {
      _activeUploads.remove(uploadId);
    }

    // إزالة من قائمة الرفع بعد الانتهاء/الفشل
    final finalUploads = Map<String, List<ImageUploadInfo>>.from(
      currentState.uploadingImages,
    );
    finalUploads.remove(event.conversationId);
    emit(currentState.copyWith(uploadingImages: finalUploads));
  }

  // واجهة مساعدة عامة حتى تتمكن الواجهة من تتبع التقدم بدقة
  Future<void> uploadAttachmentWithProgress({
    required String conversationId,
    required String filePath,
    required String messageType,
    String? replyToMessageId,
    String? replyToAttachmentId,
    void Function(int sent, int total)? onProgress,
  }) async {
    add(UploadAttachmentEvent(
      conversationId: conversationId,
      filePath: filePath,
      messageType: messageType,
      onProgress: onProgress,
    ));
  }

  /// تحديث تقدم الرفع
  void _updateUploadProgress(
    Emitter<ChatState> emit,
    ChatLoaded currentState,
    String conversationId,
    String uploadId,
    double progress,
  ) {
    final updatedUploads = Map<String, List<ImageUploadInfo>>.from(
      currentState.uploadingImages,
    );

    final uploads = updatedUploads[conversationId] ?? [];
    final updatedTasks = uploads.map((task) {
      if (task.id == uploadId) {
        return task.copyWith(progress: progress);
      }
      return task;
    }).toList();

    updatedUploads[conversationId] = updatedTasks;
    emit(currentState.copyWith(uploadingImages: updatedUploads));
  }

  /// تحديث حالة الرفع
  void _updateUploadStatus(
    Emitter<ChatState> emit,
    ChatLoaded currentState,
    String conversationId,
    String uploadId, {
    bool? isCompleted,
    bool? isFailed,
    String? error,
  }) {
    final updatedUploads = Map<String, List<ImageUploadInfo>>.from(
      currentState.uploadingImages,
    );

    final uploads = updatedUploads[conversationId] ?? [];
    final updatedTasks = uploads.map((task) {
      if (task.id == uploadId) {
        return task.copyWith(
          isCompleted: isCompleted ?? task.isCompleted,
          isFailed: isFailed ?? task.isFailed,
          error: error ?? task.error,
          progress: isCompleted == true ? 1.0 : task.progress,
        );
      }
      return task;
    }).toList();

    updatedUploads[conversationId] = updatedTasks;
    emit(currentState.copyWith(uploadingImages: updatedUploads));
  }

  /// إلغاء عملية الرفع
  Future<void> _onCancelUpload(
    CancelUploadEvent event,
    Emitter<ChatState> emit,
  ) async {
    final uploadTask = _activeUploads[event.uploadId];
    if (uploadTask != null) {
      uploadTask.controller.cancel();
      _activeUploads.remove(event.uploadId);

      // تحديث حالة الرفع
      final currentState = state;
      if (currentState is ChatLoaded) {
        _updateUploadStatus(
          emit,
          currentState,
          event.conversationId,
          event.uploadId,
          isFailed: true,
          error: 'تم إلغاء الرفع',
        );
      }
    }
  }

  /// إعادة محاولة إرسال رسالة فاشلة
  // Duplicate older implementation removed to avoid duplicate definition

  /// إنشاء محادثة جديدة
  Future<void> _onCreateConversation(
    CreateConversationEvent event,
    Emitter<ChatState> emit,
  ) async {
    emit(const ConversationCreating());

    final result = await createConversationUseCase(
      CreateConversationParams(
        participantIds: event.participantIds,
        conversationType: event.conversationType,
        title: event.title,
        description: event.description,
        propertyId: event.propertyId,
      ),
    );

    await result.fold(
      (failure) async =>
          emit(ChatError(message: _mapFailureToMessage(failure))),
      (conversation) async {
        emit(ConversationCreated(conversation: conversation));

        // تحديث قائمة المحادثات
        final currentState = state;
        if (currentState is ChatLoaded) {
          final updatedConversations = [
            conversation,
            ...currentState.conversations
          ];
          emit(currentState.copyWith(conversations: updatedConversations));
        } else {
          add(const LoadConversationsEvent());
        }
      },
    );
  }

  /// حذف محادثة
  Future<void> _onDeleteConversation(
    DeleteConversationEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await deleteConversationUseCase(
      DeleteConversationParams(conversationId: event.conversationId),
    );

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (_) async {
        // إزالة المحادثة من القائمة
        final updatedConversations = currentState.conversations
            .where((conv) => conv.id != event.conversationId)
            .toList();

        // إزالة الرسائل المرتبطة
        final updatedMessages =
            Map<String, List<Message>>.from(currentState.messages);
        updatedMessages.remove(event.conversationId);

        // تنظيف الذاكرة المؤقتة
        add(ClearConversationCacheEvent(conversationId: event.conversationId));

        emit(currentState.copyWith(
          conversations: updatedConversations,
          messages: updatedMessages,
          error: null,
        ));
      },
    );
  }

  /// أرشفة محادثة
  Future<void> _onArchiveConversation(
    ArchiveConversationEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await archiveConversationUseCase(
      ArchiveConversationParams(conversationId: event.conversationId),
    );

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (_) async {
        // تحديث حالة الأرشفة للمحادثة
        final updatedConversations = currentState.conversations.map((conv) {
          if (conv.id == event.conversationId) {
            return Conversation(
              id: conv.id,
              conversationType: conv.conversationType,
              title: conv.title,
              description: conv.description,
              avatar: conv.avatar,
              createdAt: conv.createdAt,
              updatedAt: conv.updatedAt,
              lastMessage: conv.lastMessage,
              unreadCount: conv.unreadCount,
              isArchived: true,
              isMuted: conv.isMuted,
              propertyId: conv.propertyId,
              participants: conv.participants,
            );
          }
          return conv;
        }).toList();

        emit(currentState.copyWith(
          conversations: updatedConversations,
          error: null,
        ));
      },
    );
  }

  /// إلغاء أرشفة محادثة
  Future<void> _onUnarchiveConversation(
    UnarchiveConversationEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await unarchiveConversationUseCase(
      UnarchiveConversationParams(conversationId: event.conversationId),
    );

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (_) async {
        // تحديث حالة الأرشفة للمحادثة
        final updatedConversations = currentState.conversations.map((conv) {
          if (conv.id == event.conversationId) {
            return Conversation(
              id: conv.id,
              conversationType: conv.conversationType,
              title: conv.title,
              description: conv.description,
              avatar: conv.avatar,
              createdAt: conv.createdAt,
              updatedAt: conv.updatedAt,
              lastMessage: conv.lastMessage,
              unreadCount: conv.unreadCount,
              isArchived: false,
              isMuted: conv.isMuted,
              propertyId: conv.propertyId,
              participants: conv.participants,
            );
          }
          return conv;
        }).toList();

        emit(currentState.copyWith(
          conversations: updatedConversations,
          error: null,
        ));
      },
    );
  }

  /// حذف رسالة
  Future<void> _onDeleteMessage(
    DeleteMessageEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await deleteMessageUseCase(
      DeleteMessageParams(messageId: event.messageId),
    );

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (_) async {
        // تحديث الرسالة كمحذوفة
        final updatedMessages =
            Map<String, List<Message>>.from(currentState.messages);

        updatedMessages.forEach((conversationId, messages) {
          final updatedList = messages.map((msg) {
            if (msg.id == event.messageId) {
              return Message(
                id: msg.id,
                conversationId: msg.conversationId,
                senderId: msg.senderId,
                senderName: msg.senderName,
                messageType: msg.messageType,
                content: '[رسالة محذوفة]',
                location: msg.location,
                replyToMessageId: msg.replyToMessageId,
                attachments: const [],
                reactions: msg.reactions,
                createdAt: msg.createdAt,
                updatedAt: DateTime.now(),
                status: msg.status,
                isEdited: msg.isEdited,
                isDeleted: true,
                deliveryReceipt: msg.deliveryReceipt,
              );
            }
            return msg;
          }).toList();

          updatedMessages[conversationId] = updatedList;
        });

        emit(currentState.copyWith(
          messages: updatedMessages,
          error: null,
        ));
      },
    );
  }

  /// تعديل رسالة
  Future<void> _onEditMessage(
    EditMessageEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    // تحديث متفائل
    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);
    String? targetConversationId;

    updatedMessages.forEach((conversationId, messages) {
      final messageIndex =
          messages.indexWhere((msg) => msg.id == event.messageId);
      if (messageIndex != -1) {
        targetConversationId = conversationId;
        final oldMessage = messages[messageIndex];
        messages[messageIndex] = Message(
          id: oldMessage.id,
          conversationId: oldMessage.conversationId,
          senderId: oldMessage.senderId,
          senderName: oldMessage.senderName,
          messageType: oldMessage.messageType,
          content: event.content,
          location: oldMessage.location,
          replyToMessageId: oldMessage.replyToMessageId,
          attachments: oldMessage.attachments,
          reactions: oldMessage.reactions,
          createdAt: oldMessage.createdAt,
          updatedAt: DateTime.now(),
          status: 'editing',
          isEdited: true,
          editedAt: DateTime.now(),
          isDeleted: oldMessage.isDeleted,
          deliveryReceipt: oldMessage.deliveryReceipt,
        );
      }
    });

    emit(currentState.copyWith(messages: updatedMessages));

    final result = await editMessageUseCase(
      EditMessageParams(
        messageId: event.messageId,
        content: event.content,
      ),
    );

    await result.fold(
      (failure) async {
        // فشل التعديل - إرجاع الرسالة الأصلية
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (editedMessage) async {
        // نجح التعديل - تحديث بالرسالة المعدلة من الخادم
        final finalMessages =
            Map<String, List<Message>>.from(currentState.messages);

        if (targetConversationId != null) {
          final messages = finalMessages[targetConversationId]!;
          final messageIndex =
              messages.indexWhere((msg) => msg.id == event.messageId);
          if (messageIndex != -1) {
            messages[messageIndex] = editedMessage;
          }
        }

        emit(currentState.copyWith(
          messages: finalMessages,
          error: null,
        ));
      },
    );
  }

  /// إضافة تفاعل
  Future<void> _onAddReaction(
    AddReactionEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final userId = event.currentUserId ?? currentState.currentUserId ?? '';
    if (userId.isEmpty) return;

    // تحديث متفائل
    final optimisticReaction = MessageReaction(
      id: 'temp_reaction_${DateTime.now().microsecondsSinceEpoch}',
      messageId: event.messageId,
      userId: userId,
      reactionType: event.reactionType,
    );

    final updatedMessages = _updateMessageReaction(
      currentState.messages,
      event.messageId,
      optimisticReaction,
      isAdding: true,
    );

    emit(currentState.copyWith(messages: updatedMessages));

    final result = await addReactionUseCase(
      AddReactionParams(
        messageId: event.messageId,
        reactionType: event.reactionType,
      ),
    );

    await result.fold(
      (failure) async {
        // فشل - إزالة التفاعل المتفائل
        final revertedMessages = _updateMessageReaction(
          currentState.messages,
          event.messageId,
          optimisticReaction,
          isAdding: false,
        );
        emit(currentState.copyWith(
          messages: revertedMessages,
          error: 'فشل في إضافة التفاعل',
        ));
      },
      (reaction) async {
        // نجح - استبدال التفاعل المتفائل بالفعلي
        final finalMessages = _replaceOptimisticReaction(
          currentState.messages,
          event.messageId,
          optimisticReaction.id,
          reaction,
        );
        emit(currentState.copyWith(
          messages: finalMessages,
          error: null,
        ));
      },
    );
  }

  /// إزالة تفاعل
  Future<void> _onRemoveReaction(
    RemoveReactionEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final userId = event.currentUserId ?? currentState.currentUserId ?? '';
    if (userId.isEmpty) return;

    // البحث عن التفاعل الحالي
    MessageReaction? reactionToRemove;
    currentState.messages.forEach((_, messages) {
      for (final message in messages) {
        if (message.id == event.messageId) {
          reactionToRemove = message.reactions.firstWhereOrNull(
            (r) => r.userId == userId && r.reactionType == event.reactionType,
          );
          break;
        }
      }
    });

    if (reactionToRemove == null) return;

    // تحديث متفائل - إزالة التفاعل
    final updatedMessages = _updateMessageReaction(
      currentState.messages,
      event.messageId,
      reactionToRemove!,
      isAdding: false,
    );

    emit(currentState.copyWith(messages: updatedMessages));

    final result = await removeReactionUseCase(
      RemoveReactionParams(
        messageId: event.messageId,
        reactionType: event.reactionType,
      ),
    );

    await result.fold(
      (failure) async {
        // فشل - إعادة التفاعل
        final revertedMessages = _updateMessageReaction(
          currentState.messages,
          event.messageId,
          reactionToRemove!,
          isAdding: true,
        );
        emit(currentState.copyWith(
          messages: revertedMessages,
          error: 'فشل في إزالة التفاعل',
        ));
      },
      (_) async {
        // نجح - التحديث نهائي
        emit(currentState.copyWith(error: null));
      },
    );
  }

  /// وضع علامة قراءة على الرسائل
  Future<void> _onMarkMessagesAsRead(
    MarkMessagesAsReadEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    // تحديث متفائل - وضع علامة قراءة محلياً
    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);
    final messages = updatedMessages[event.conversationId] ?? [];

    final updatedList = messages.map((msg) {
      if (event.messageIds.contains(msg.id)) {
        final receipt = msg.deliveryReceipt ?? const DeliveryReceipt();
        return Message(
          id: msg.id,
          conversationId: msg.conversationId,
          senderId: msg.senderId,
          senderName: msg.senderName,
          messageType: msg.messageType,
          content: msg.content,
          location: msg.location,
          replyToMessageId: msg.replyToMessageId,
          attachments: msg.attachments,
          reactions: msg.reactions,
          createdAt: msg.createdAt,
          updatedAt: msg.updatedAt,
          status: 'read',
          isEdited: msg.isEdited,
          editedAt: msg.editedAt,
          isDeleted: msg.isDeleted,
          deliveryReceipt: DeliveryReceipt(
            deliveredAt: receipt.deliveredAt ?? DateTime.now(),
            readAt: DateTime.now(),
            readBy: [...receipt.readBy, currentState.currentUserId ?? '']
                .where((id) => id.isNotEmpty)
                .toSet()
                .toList(),
          ),
        );
      }
      return msg;
    }).toList();

    updatedMessages[event.conversationId] = updatedList;

    // تحديث عداد الرسائل غير المقروءة
    final updatedConversations = currentState.conversations.map((conv) {
      if (conv.id == event.conversationId) {
        return Conversation(
          id: conv.id,
          conversationType: conv.conversationType,
          title: conv.title,
          description: conv.description,
          avatar: conv.avatar,
          createdAt: conv.createdAt,
          updatedAt: conv.updatedAt,
          lastMessage: conv.lastMessage,
          unreadCount: 0,
          isArchived: conv.isArchived,
          isMuted: conv.isMuted,
          propertyId: conv.propertyId,
          participants: conv.participants,
        );
      }
      return conv;
    }).toList();

    emit(currentState.copyWith(
      messages: updatedMessages,
      conversations: updatedConversations,
    ));

    // إرسال طلب القراءة للخادم
    await markMessagesAsReadUseCase(
      const MarkAsReadParams(
        conversationId: '',
        messageIds: [],
      ),
    );
  }

  /// البحث في الرسائل
  Future<void> _onSearchChats(
    SearchChatsEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    emit(currentState.copyWith(isSearching: true));

    final result = await searchChatsUseCase(
      SearchChatsParams(
        query: event.query,
        conversationId: event.conversationId,
        messageType: event.messageType,
        senderId: event.senderId,
        dateFrom: event.dateFrom,
        dateTo: event.dateTo,
        page: event.page,
        limit: event.limit,
      ),
    );

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(
          isSearching: false,
          error: _mapFailureToMessage(failure),
        ));
      },
      (searchResult) async {
        emit(currentState.copyWith(
          searchResult: searchResult,
          isSearching: false,
          error: null,
        ));
      },
    );
  }

  /// تحميل المستخدمين المتاحين
  Future<void> _onLoadAvailableUsers(
    LoadAvailableUsersEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await getAvailableUsersUseCase(
      GetAvailableUsersParams(
        userType: event.userType,
        propertyId: event.propertyId,
      ),
    );

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (users) async {
        emit(currentState.copyWith(
          availableUsers: users,
          error: null,
        ));
      },
    );
  }

  /// تحميل المشرفين
  Future<void> _onLoadAdminUsers(
    LoadAdminUsersEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await getAdminUsersUseCase(NoParams());

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (admins) async {
        emit(currentState.copyWith(
          adminUsers: admins,
          error: null,
        ));
      },
    );
  }

  /// تحديث حالة المستخدم
  Future<void> _onUpdateUserStatus(
    UpdateUserStatusEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await updateUserStatusUseCase(
      UpdateUserStatusParams(status: event.status),
    );

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (_) async {
        // تحديث حالة المستخدم الحالي
        final updatedPresence =
            Map<String, UserPresence>.from(currentState.userPresence);
        updatedPresence[currentState.currentUserId ?? ''] = UserPresence(
          status: event.status,
          lastSeen: event.status == 'offline' ? DateTime.now() : null,
        );

        emit(currentState.copyWith(
          userPresence: updatedPresence,
          error: null,
        ));
      },
    );
  }

  /// تحميل إعدادات الشات
  Future<void> _onLoadChatSettings(
    LoadChatSettingsEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final result = await getChatSettingsUseCase(NoParams());

    await result.fold(
      (failure) async {
        emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
      },
      (settings) async {
        emit(currentState.copyWith(
          settings: settings,
          error: null,
        ));
      },
    );
  }

  /// تحديث إعدادات الشات
  Future<void> _onUpdateChatSettings(
    UpdateChatSettingsEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final currentSettings = currentState.settings;
    if (currentSettings == null) return;

    final updatedSettings = ChatSettings(
      id: currentSettings.id,
      userId: currentSettings.userId,
      notificationsEnabled:
          event.notificationsEnabled ?? currentSettings.notificationsEnabled,
      soundEnabled: event.soundEnabled ?? currentSettings.soundEnabled,
      showReadReceipts:
          event.showReadReceipts ?? currentSettings.showReadReceipts,
      showTypingIndicator:
          event.showTypingIndicator ?? currentSettings.showTypingIndicator,
      theme: event.theme ?? currentSettings.theme,
      fontSize: event.fontSize ?? currentSettings.fontSize,
      autoDownloadMedia:
          event.autoDownloadMedia ?? currentSettings.autoDownloadMedia,
      backupMessages: event.backupMessages ?? currentSettings.backupMessages,
    );

    // تحديث متفائل
    emit(currentState.copyWith(settings: updatedSettings));

    final result = await updateChatSettingsUseCase(
      UpdateChatSettingsParams(settings: updatedSettings),
    );

    await result.fold(
      (failure) async {
        // فشل - إرجاع الإعدادات السابقة
        emit(currentState.copyWith(
          settings: currentSettings,
          error: _mapFailureToMessage(failure),
        ));
      },
      (settings) async {
        // نجح - تحديث بالإعدادات من الخادم
        emit(currentState.copyWith(
          settings: settings,
          error: null,
        ));
      },
    );
  }

  /// إرسال مؤشر الكتابة
  Future<void> _onSendTypingIndicator(
    SendTypingIndicatorEvent event,
    Emitter<ChatState> emit,
  ) async {
    await sendTypingIndicatorUseCase(
      SendTypingIndicatorParams(
        conversationId: event.conversationId,
        isTyping: event.isTyping,
      ),
    );
  }

  /// معالجة رسالة WebSocket
  Future<void> _onWebSocketMessageReceived(
    WebSocketMessageReceivedEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final messageEvent = event.messageEvent;

    switch (messageEvent.type) {
      case MessageEventType.newMessage:
        await _handleNewMessage(messageEvent, currentState, emit);
        break;
      case MessageEventType.messageUpdated:
        await _handleMessageUpdated(messageEvent, currentState, emit);
        break;
      case MessageEventType.messageDeleted:
        await _handleMessageDeleted(messageEvent, currentState, emit);
        break;
      case MessageEventType.reactionAdded:
        await _handleReactionAdded(messageEvent, currentState, emit);
        break;
      case MessageEventType.reactionRemoved:
        await _handleReactionRemoved(messageEvent, currentState, emit);
        break;
      case MessageEventType.statusUpdated:
        await _handleStatusUpdated(messageEvent, currentState, emit);
        break;
    }
  }

  /// معالجة رسالة جديدة من WebSocket
  Future<void> _handleNewMessage(
    MessageEvent event,
    ChatLoaded currentState,
    Emitter<ChatState> emit,
  ) async {
    if (event.message == null) return;

    final newMessage = event.message!;
    final conversationId = event.conversationId;

    // تحديث قائمة الرسائل
    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);
    final messages = List<Message>.from(updatedMessages[conversationId] ?? []);

    // تحقق من عدم وجود الرسالة بالفعل
    if (!messages.any((msg) => msg.id == newMessage.id)) {
      messages.insert(0, newMessage);
      updatedMessages[conversationId] = messages;
    }

    // تحديث آخر رسالة في المحادثة وزيادة العداد
    final updatedConversations = currentState.conversations.map((conv) {
      if (conv.id == conversationId) {
        final isMyMessage = newMessage.senderId == currentState.currentUserId;
        return Conversation(
          id: conv.id,
          conversationType: conv.conversationType,
          title: conv.title,
          description: conv.description,
          avatar: conv.avatar,
          createdAt: conv.createdAt,
          updatedAt: DateTime.now(),
          lastMessage: newMessage,
          unreadCount: isMyMessage ? 0 : conv.unreadCount + 1,
          isArchived: conv.isArchived,
          isMuted: conv.isMuted,
          propertyId: conv.propertyId,
          participants: conv.participants,
        );
      }
      return conv;
    }).toList();

    emit(currentState.copyWith(
      messages: updatedMessages,
      conversations: updatedConversations,
    ));
  }

  /// معالجة تحديث رسالة من WebSocket
  Future<void> _handleMessageUpdated(
    MessageEvent event,
    ChatLoaded currentState,
    Emitter<ChatState> emit,
  ) async {
    if (event.message == null) return;

    final updatedMessage = event.message!;
    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);

    updatedMessages.forEach((conversationId, messages) {
      final messageIndex =
          messages.indexWhere((msg) => msg.id == updatedMessage.id);
      if (messageIndex != -1) {
        messages[messageIndex] = updatedMessage;
      }
    });

    emit(currentState.copyWith(messages: updatedMessages));
  }

  /// معالجة حذف رسالة من WebSocket
  Future<void> _handleMessageDeleted(
    MessageEvent event,
    ChatLoaded currentState,
    Emitter<ChatState> emit,
  ) async {
    final messageId = event.messageId;
    if (messageId == null) return;

    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);

    updatedMessages.forEach((conversationId, messages) {
      final messageIndex = messages.indexWhere((msg) => msg.id == messageId);
      if (messageIndex != -1) {
        final deletedMessage = messages[messageIndex];
        messages[messageIndex] = Message(
          id: deletedMessage.id,
          conversationId: deletedMessage.conversationId,
          senderId: deletedMessage.senderId,
          senderName: deletedMessage.senderName,
          messageType: deletedMessage.messageType,
          content: '[رسالة محذوفة]',
          location: deletedMessage.location,
          replyToMessageId: deletedMessage.replyToMessageId,
          attachments: const [],
          reactions: deletedMessage.reactions,
          createdAt: deletedMessage.createdAt,
          updatedAt: DateTime.now(),
          status: deletedMessage.status,
          isEdited: deletedMessage.isEdited,
          isDeleted: true,
          deliveryReceipt: deletedMessage.deliveryReceipt,
        );
      }
    });

    emit(currentState.copyWith(messages: updatedMessages));
  }

  /// معالجة إضافة تفاعل من WebSocket
  Future<void> _handleReactionAdded(
    MessageEvent event,
    ChatLoaded currentState,
    Emitter<ChatState> emit,
  ) async {
    final reaction = event.reaction;
    if (reaction == null) return;

    final updatedMessages = _updateMessageReaction(
      currentState.messages,
      reaction.messageId,
      reaction,
      isAdding: true,
    );

    emit(currentState.copyWith(messages: updatedMessages));
  }

  /// معالجة إزالة تفاعل من WebSocket
  Future<void> _handleReactionRemoved(
    MessageEvent event,
    ChatLoaded currentState,
    Emitter<ChatState> emit,
  ) async {
    final reaction = event.reaction;
    if (reaction == null) return;

    final updatedMessages = _updateMessageReaction(
      currentState.messages,
      reaction.messageId,
      reaction,
      isAdding: false,
    );

    emit(currentState.copyWith(messages: updatedMessages));
  }

  /// معالجة تحديث حالة الرسالة من WebSocket
  Future<void> _handleStatusUpdated(
    MessageEvent event,
    ChatLoaded currentState,
    Emitter<ChatState> emit,
  ) async {
    final messageId = event.messageId;
    final status = event.status;
    if (messageId == null || status == null) return;

    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);

    updatedMessages.forEach((conversationId, messages) {
      final messageIndex = messages.indexWhere((msg) => msg.id == messageId);
      if (messageIndex != -1) {
        final message = messages[messageIndex];
        messages[messageIndex] = Message(
          id: message.id,
          conversationId: message.conversationId,
          senderId: message.senderId,
          senderName: message.senderName,
          messageType: message.messageType,
          content: message.content,
          location: message.location,
          replyToMessageId: message.replyToMessageId,
          attachments: message.attachments,
          reactions: message.reactions,
          createdAt: message.createdAt,
          updatedAt: message.updatedAt,
          status: status,
          isEdited: message.isEdited,
          editedAt: message.editedAt,
          isDeleted: message.isDeleted,
          deliveryReceipt: message.deliveryReceipt,
        );
      }
    });

    emit(currentState.copyWith(messages: updatedMessages));
  }

  /// معالجة تحديث المحادثة من WebSocket
  /// معالجة تحديث المحادثة من WebSocket
  Future<void> _onWebSocketConversationUpdated(
    WebSocketConversationUpdatedEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final updatedConversation = event.conversation;

    // تحديث المحادثة في القائمة
    final updatedConversations = currentState.conversations.map((conv) {
      if (conv.id == updatedConversation.id) {
        return updatedConversation;
      }
      return conv;
    }).toList();

    emit(currentState.copyWith(conversations: updatedConversations));
  }

  /// معالجة مؤشر الكتابة من WebSocket
  Future<void> _onWebSocketTypingIndicator(
    WebSocketTypingIndicatorEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final updatedTypingUsers =
        Map<String, List<String>>.from(currentState.typingUsers);

    if (event.typingUserIds.isEmpty) {
      updatedTypingUsers.remove(event.conversationId);
    } else {
      // فلترة معرف المستخدم الحالي من قائمة الكاتبين
      final filteredUsers = event.typingUserIds
          .where((id) => id != currentState.currentUserId)
          .toList();

      if (filteredUsers.isNotEmpty) {
        updatedTypingUsers[event.conversationId] = filteredUsers;
      } else {
        updatedTypingUsers.remove(event.conversationId);
      }
    }

    emit(currentState.copyWith(typingUsers: updatedTypingUsers));
  }

  /// معالجة تحديث الحضور من WebSocket
  Future<void> _onWebSocketPresenceUpdate(
    WebSocketPresenceUpdateEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    final updatedPresence =
        Map<String, UserPresence>.from(currentState.userPresence);
    updatedPresence[event.userId] = UserPresence(
      status: event.status,
      lastSeen: event.lastSeen,
    );

    // تحديث حالة المستخدم في قائمة المستخدمين المتاحين
    final updatedAvailableUsers = currentState.availableUsers.map((user) {
      if (user.id == event.userId) {
        return ChatUser(
          id: user.id,
          name: user.name,
          email: user.email,
          phone: user.phone,
          profileImage: user.profileImage,
          userType: user.userType,
          status: event.status,
          lastSeen: event.lastSeen,
          propertyId: user.propertyId,
          isOnline: event.status == 'online',
        );
      }
      return user;
    }).toList();

    // تحديث حالة المستخدم في المحادثات
    final updatedConversations = currentState.conversations.map((conv) {
      final updatedParticipants = conv.participants.map((participant) {
        if (participant.id == event.userId) {
          return ChatUser(
            id: participant.id,
            name: participant.name,
            email: participant.email,
            phone: participant.phone,
            profileImage: participant.profileImage,
            userType: participant.userType,
            status: event.status,
            lastSeen: event.lastSeen,
            propertyId: participant.propertyId,
            isOnline: event.status == 'online',
          );
        }
        return participant;
      }).toList();

      return Conversation(
        id: conv.id,
        conversationType: conv.conversationType,
        title: conv.title,
        description: conv.description,
        avatar: conv.avatar,
        createdAt: conv.createdAt,
        updatedAt: conv.updatedAt,
        lastMessage: conv.lastMessage,
        unreadCount: conv.unreadCount,
        isArchived: conv.isArchived,
        isMuted: conv.isMuted,
        propertyId: conv.propertyId,
        participants: updatedParticipants,
      );
    }).toList();

    emit(currentState.copyWith(
      userPresence: updatedPresence,
      availableUsers: updatedAvailableUsers,
      conversations: updatedConversations,
    ));
  }

  /// مسح ذاكرة التخزين المؤقت للمحادثة
  Future<void> _onClearConversationCache(
    ClearConversationCacheEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    // مسح الرسائل المحفوظة
    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);
    updatedMessages.remove(event.conversationId);

    // مسح معلومات التحميل
    _conversationLoadingStates.remove(event.conversationId);
    _messagePages.remove(event.conversationId);
    _hasMoreMessages.remove(event.conversationId);
    _loadedMessageIds.remove(event.conversationId);
    _optimisticMessages.remove(event.conversationId);

    // مسح مؤشرات الكتابة
    final updatedTypingUsers =
        Map<String, List<String>>.from(currentState.typingUsers);
    updatedTypingUsers.remove(event.conversationId);

    emit(currentState.copyWith(
      messages: updatedMessages,
      typingUsers: updatedTypingUsers,
    ));
  }

  /// تحديث المحادثة
  Future<void> _onRefreshConversation(
    RefreshConversationEvent event,
    Emitter<ChatState> emit,
  ) async {
    // مسح الذاكرة المؤقتة أولاً
    add(ClearConversationCacheEvent(conversationId: event.conversationId));

    // إعادة تحميل الرسائل
    add(LoadMessagesEvent(
      conversationId: event.conversationId,
      pageNumber: 1,
      pageSize: 50,
    ));
  }

  /// إعادة محاولة إرسال رسالة فاشلة
  Future<void> _onRetryFailedMessage(
    RetryFailedMessageEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;
    if (currentState is! ChatLoaded) return;

    // البحث عن الرسالة الفاشلة
    Message? failedMessage;
    currentState.messages[event.conversationId]?.forEach((msg) {
      if (msg.id == event.messageId && msg.status == 'failed') {
        failedMessage = msg;
      }
    });

    if (failedMessage == null) return;

    // تحديث حالة الرسالة إلى "إرسال"
    final updatedMessages =
        Map<String, List<Message>>.from(currentState.messages);
    final messages = updatedMessages[event.conversationId] ?? [];

    final updatedList = messages.map((msg) {
      if (msg.id == event.messageId) {
        return Message(
          id: msg.id,
          conversationId: msg.conversationId,
          senderId: msg.senderId,
          senderName: msg.senderName,
          messageType: msg.messageType,
          content: msg.content,
          location: msg.location,
          replyToMessageId: msg.replyToMessageId,
          attachments: msg.attachments,
          reactions: msg.reactions,
          createdAt: msg.createdAt,
          updatedAt: DateTime.now(),
          status: 'sending',
          isEdited: msg.isEdited,
          isDeleted: msg.isDeleted,
          deliveryReceipt: msg.deliveryReceipt,
        );
      }
      return msg;
    }).toList();

    updatedMessages[event.conversationId] = updatedList;
    emit(currentState.copyWith(messages: updatedMessages));

    // إعادة المحاولة
    add(SendMessageEvent(
      conversationId: failedMessage!.conversationId,
      messageType: failedMessage!.messageType,
      content: failedMessage!.content,
      location: failedMessage!.location,
      replyToMessageId: failedMessage!.replyToMessageId,
      attachmentIds: failedMessage!.attachments.map((a) => a.id).toList(),
      currentUserId: currentState.currentUserId,
    ));
  }

  /// دمج المحادثات الجديدة مع القديمة
  List<Conversation> _mergeConversations(
    List<Conversation> existing,
    List<Conversation> newConversations,
  ) {
    final conversationMap = <String, Conversation>{};

    // إضافة المحادثات الموجودة
    for (final conv in existing) {
      conversationMap[conv.id] = conv;
    }

    // تحديث أو إضافة المحادثات الجديدة
    for (final conv in newConversations) {
      conversationMap[conv.id] = conv;
    }

    // ترتيب حسب آخر تحديث
    final merged = conversationMap.values.toList()
      ..sort((a, b) => b.updatedAt.compareTo(a.updatedAt));

    return merged;
  }

  /// دمج الرسائل الجديدة مع القديمة
  List<Message> _mergeMessages(
    List<Message> existing,
    List<Message> newMessages,
  ) {
    final messageMap = <String, Message>{};

    // إضافة الرسائل الموجودة
    for (final msg in existing) {
      // تخطي الرسائل المتفائلة المؤقتة
      if (!msg.id.startsWith('temp_')) {
        messageMap[msg.id] = msg;
      }
    }

    // إضافة الرسائل الجديدة
    for (final msg in newMessages) {
      messageMap[msg.id] = msg;
    }

    // ترتيب حسب وقت الإنشاء (الأحدث أولاً)
    final merged = messageMap.values.toList()
      ..sort((a, b) => b.createdAt.compareTo(a.createdAt));

    return merged;
  }

  /// تحديث تفاعل الرسالة
  Map<String, List<Message>> _updateMessageReaction(
      Map<String, List<Message>> messages,
      String messageId,
      MessageReaction reaction,
      {required bool isAdding}) {
    final updatedMessages = Map<String, List<Message>>.from(messages);

    updatedMessages.forEach((conversationId, messageList) {
      final messageIndex = messageList.indexWhere((msg) => msg.id == messageId);
      if (messageIndex != -1) {
        final message = messageList[messageIndex];
        final updatedReactions = List<MessageReaction>.from(message.reactions);

        if (isAdding) {
          // إضافة التفاعل إذا لم يكن موجوداً
          if (!updatedReactions.any((r) =>
              r.userId == reaction.userId &&
              r.reactionType == reaction.reactionType)) {
            updatedReactions.add(reaction);
          }
        } else {
          // إزالة التفاعل
          updatedReactions.removeWhere((r) =>
              r.userId == reaction.userId &&
              r.reactionType == reaction.reactionType);
        }

        messageList[messageIndex] = Message(
          id: message.id,
          conversationId: message.conversationId,
          senderId: message.senderId,
          senderName: message.senderName,
          messageType: message.messageType,
          content: message.content,
          location: message.location,
          replyToMessageId: message.replyToMessageId,
          attachments: message.attachments,
          reactions: updatedReactions,
          createdAt: message.createdAt,
          updatedAt: message.updatedAt,
          status: message.status,
          isEdited: message.isEdited,
          editedAt: message.editedAt,
          isDeleted: message.isDeleted,
          deliveryReceipt: message.deliveryReceipt,
        );
      }
    });

    return updatedMessages;
  }

  /// استبدال التفاعل المتفائل بالفعلي
  Map<String, List<Message>> _replaceOptimisticReaction(
    Map<String, List<Message>> messages,
    String messageId,
    String optimisticReactionId,
    MessageReaction actualReaction,
  ) {
    final updatedMessages = Map<String, List<Message>>.from(messages);

    updatedMessages.forEach((conversationId, messageList) {
      final messageIndex = messageList.indexWhere((msg) => msg.id == messageId);
      if (messageIndex != -1) {
        final message = messageList[messageIndex];
        final updatedReactions = message.reactions.map((r) {
          if (r.id == optimisticReactionId) {
            return actualReaction;
          }
          return r;
        }).toList();

        messageList[messageIndex] = Message(
          id: message.id,
          conversationId: message.conversationId,
          senderId: message.senderId,
          senderName: message.senderName,
          messageType: message.messageType,
          content: message.content,
          location: message.location,
          replyToMessageId: message.replyToMessageId,
          attachments: message.attachments,
          reactions: updatedReactions,
          createdAt: message.createdAt,
          updatedAt: message.updatedAt,
          status: message.status,
          isEdited: message.isEdited,
          editedAt: message.editedAt,
          isDeleted: message.isDeleted,
          deliveryReceipt: message.deliveryReceipt,
        );
      }
    });

    return updatedMessages;
  }

  /// تحويل الفشل إلى رسالة
  String _mapFailureToMessage(Failure failure) {
    switch (failure.runtimeType) {
      case ServerFailure:
        return 'حدث خطأ في الخادم. يرجى المحاولة مرة أخرى.';
      case NetworkFailure:
        return 'لا يوجد اتصال بالإنترنت. تحقق من الاتصال وحاول مرة أخرى.';
      case AuthenticationFailure:
        return 'فشل التحقق من الهوية. يرجى تسجيل الدخول مرة أخرى.';
      case ValidationFailure:
        return 'البيانات المدخلة غير صحيحة. تحقق وحاول مرة أخرى.';
      case NotFoundFailure:
        return 'المحتوى المطلوب غير موجود.';
      case PermissionDeniedFailure:
        return 'ليس لديك صلاحيات لتنفيذ هذا الإجراء.';
      case TimeoutFailure:
        return 'انتهت مهلة الاتصال. حاول مرة أخرى.';
      case CacheFailure:
        return 'حدث خطأ في الذاكرة المؤقتة.';
      default:
        return 'حدث خطأ غير متوقع. يرجى المحاولة لاحقاً.';
    }
  }

  @override
  Future<void> close() {
    // إلغاء جميع الاشتراكات
    _webSocketMessagesSubscription?.cancel();
    _webSocketTypingSubscription?.cancel();
    _webSocketPresenceSubscription?.cancel();
    _webSocketConversationSubscription?.cancel();

    // إلغاء جميع عمليات الرفع النشطة
    for (final upload in _activeUploads.values) {
      upload.controller.cancel();
    }
    _activeUploads.clear();

    // قطع اتصال WebSocket
    webSocketService.disconnect();

    return super.close();
  }
}

/// فئة لتتبع مهام الرفع
class UploadTask {
  final UploadController controller;
  final String filePath;

  UploadTask({
    required this.controller,
    required this.filePath,
  });
}

/// وحدة تحكم في عملية الرفع
class UploadController {
  final cancelToken = CancelToken();
  bool _isCancelled = false;

  bool get isCancelled => _isCancelled;

  void cancel() {
    _isCancelled = true;
    cancelToken.cancel('Upload cancelled by user');
  }
}

/// رمز إلغاء (محاكاة Dio CancelToken)
class CancelToken {
  bool _isCancelled = false;
  String? _cancelReason;

  bool get isCancelled => _isCancelled;
  String? get cancelReason => _cancelReason;

  void cancel([String? reason]) {
    _isCancelled = true;
    _cancelReason = reason;
  }
}
