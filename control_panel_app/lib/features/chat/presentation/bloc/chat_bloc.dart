import 'dart:async';
import 'package:bookn_cp_app/features/chat/presentation/models/image_upload_info.dart';
import 'package:bookn_cp_app/services/notification_service.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:bookn_cp_app/services/websocket_service.dart';
import 'package:get_it/get_it.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/conversation.dart';
import '../../domain/entities/message.dart';
import '../../data/models/message_model.dart';
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
import '../../domain/usecases/upload_attachment_usecase.dart';
import '../../domain/usecases/search_chats_usecase.dart';
import '../../domain/usecases/get_available_users_usecase.dart';
import '../../domain/usecases/get_admin_users_usecase.dart';
import '../../domain/usecases/update_user_status_usecase.dart';
import '../../domain/usecases/get_chat_settings_usecase.dart';
import '../../domain/usecases/update_chat_settings_usecase.dart';

part 'chat_event.dart';
part 'chat_state.dart';

class ChatBloc extends Bloc<ChatEvent, ChatState> {
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
  final MarkAsReadUseCase markAsReadUseCase;
  final UploadAttachmentUseCase uploadAttachmentUseCase;
  final SearchChatsUseCase searchChatsUseCase;
  final GetAvailableUsersUseCase getAvailableUsersUseCase;
  final GetAdminUsersUseCase getAdminUsersUseCase;
  final UpdateUserStatusUseCase updateUserStatusUseCase;
  final GetChatSettingsUseCase getChatSettingsUseCase;
  final UpdateChatSettingsUseCase updateChatSettingsUseCase;
  final ChatWebSocketService webSocketService;

  StreamSubscription? _messageSubscription;
  StreamSubscription? _conversationSubscription;
  StreamSubscription? _typingSubscription;
  StreamSubscription? _presenceSubscription;

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
    required this.markAsReadUseCase,
    required this.uploadAttachmentUseCase,
    required this.searchChatsUseCase,
    required this.getAvailableUsersUseCase,
    required this.getAdminUsersUseCase,
    required this.updateUserStatusUseCase,
    required this.getChatSettingsUseCase,
    required this.updateChatSettingsUseCase,
    required this.webSocketService,
  }) : super(const ChatInitial()) {
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
    on<StartImageUploadsEvent>(_onStartImageUploads);
    on<UpdateImageUploadProgressEvent>(_onUpdateImageUploadProgress);
    on<FinishImageUploadsEvent>(_onFinishImageUploads);
    // Removed legacy image upload temp-message handlers
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
    on<_UploadProgressInternal>(_onUploadProgressInternal);

    _initializeWebSocket();
    _bindRealtimeStreams();
  }

  void _initializeWebSocket() {
    // WebSocket disabled; bind NotificationService to dispatch events directly
    try {
      final notif = GetIt.instance<NotificationService>();
      notif.bindChatEventSink((evt) => add(evt));
    } catch (_) {}
  }

  // Bind message/conversation realtime streams (fed by NotificationService)
  void _bindRealtimeStreams() {
    // No WebSocket realtime streams; FCM dispatches via NotificationService -> bindChatEventSink
  }

  Future<void> _onInitializeChat(
    InitializeChatEvent event,
    Emitter<ChatState> emit,
  ) async {
    emit(const ChatLoading());

    // Load conversations and settings in parallel
    final conversationsResult = await getConversationsUseCase(
      const GetConversationsParams(),
    );

    final settingsResult = await getChatSettingsUseCase(NoParams());

    await conversationsResult.fold(
      (failure) async =>
          emit(ChatError(message: _mapFailureToMessage(failure))),
      (conversations) async {
        // Ensure conversations are sorted by updatedAt desc for list stability
        conversations.sort((a, b) => b.updatedAt.compareTo(a.updatedAt));
        await settingsResult.fold(
          (failure) async =>
              emit(ChatError(message: _mapFailureToMessage(failure))),
          (settings) async => emit(ChatLoaded(
            conversations: conversations,
            settings: settings,
          )),
        );
      },
    );
  }

  Future<void> _onLoadConversations(
    LoadConversationsEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) {
      emit(const ChatLoading());
    }

    final result = await getConversationsUseCase(
      GetConversationsParams(
        pageNumber: event.pageNumber,
        pageSize: event.pageSize,
      ),
    );

    await result.fold(
      (failure) async =>
          emit(ChatError(message: _mapFailureToMessage(failure))),
      (conversations) async {
        // Ensure stable ordering and preserve unread counts coming from API
        conversations.sort((a, b) => b.updatedAt.compareTo(a.updatedAt));
        if (state is ChatLoaded) {
          final currentState = state as ChatLoaded;
          emit(currentState.copyWith(
            conversations: event.pageNumber == 1
                ? conversations
                : [...currentState.conversations, ...conversations],
          ));
        } else {
          emit(ChatLoaded(conversations: conversations));
        }
      },
    );
  }

  Future<void> _onLoadMessages(
    LoadMessagesEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;
    emit(currentState.copyWith(isLoadingMessages: true));

    final result = await getMessagesUseCase(
      GetMessagesParams(
        conversationId: event.conversationId,
        pageNumber: event.pageNumber,
        pageSize: event.pageSize,
        beforeMessageId: event.beforeMessageId,
      ),
    );

    await result.fold(
      (failure) async => emit(currentState.copyWith(
        isLoadingMessages: false,
        error: _mapFailureToMessage(failure),
      )),
      (messages) async {
        final currentMessages =
            currentState.messages[event.conversationId] ?? [];
        final updatedMessages = event.pageNumber == 1
            ? messages
            : [...currentMessages, ...messages];

        emit(currentState.copyWith(
          messages: {
            ...currentState.messages,
            event.conversationId: updatedMessages,
          },
          isLoadingMessages: false,
        ));
      },
    );
  }

  Future<void> _onLoadMoreMessages(
    LoadMoreMessagesEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;
    if (currentState.isLoadingMore) return;

    final currentMessages =
        List<Message>.from(currentState.messages[event.conversationId] ?? []);

    if (currentMessages.isEmpty) {
      add(LoadMessagesEvent(
        conversationId: event.conversationId,
        pageNumber: 1,
        pageSize: event.pageSize,
      ));
      return;
    }

    final oldestMessageId = currentMessages.last.id;

    emit(currentState.copyWith(
      isLoadingMore: true,
      error: null,
    ));

    final result = await getMessagesUseCase(
      GetMessagesParams(
        conversationId: event.conversationId,
        pageNumber: 1,
        pageSize: event.pageSize,
        beforeMessageId: oldestMessageId,
      ),
    );

    await result.fold(
      (failure) async => emit(currentState.copyWith(
        isLoadingMore: false,
        error: _mapFailureToMessage(failure),
      )),
      (messages) async {
        if (messages.isEmpty) {
          emit(currentState.copyWith(isLoadingMore: false));
          return;
        }

        final merged = [
          ...currentMessages,
          ...messages.where(
            (msg) => currentMessages.every((existing) => existing.id != msg.id),
          ),
        ];

        emit(currentState.copyWith(
          messages: {
            ...currentState.messages,
            event.conversationId: merged,
          },
          isLoadingMore: false,
        ));

        if (event.targetMessageId != null &&
            merged.every((m) => m.id != event.targetMessageId)) {
          add(LoadMoreMessagesEvent(
            conversationId: event.conversationId,
            targetMessageId: event.targetMessageId,
            pageSize: event.pageSize,
          ));
        }
      },
    );
  }

  Future<void> _onSendMessage(
    SendMessageEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;

    // Optimistic update
    final tempMessage = Message(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      conversationId: event.conversationId,
      // Use actual current user id if provided to ensure correct alignment
      senderId: (event.currentUserId != null && event.currentUserId!.isNotEmpty)
          ? event.currentUserId!
          : 'current_user',
      messageType: event.messageType,
      content: event.content,
      location: event.location,
      replyToMessageId: event.replyToMessageId,
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
      status: 'sending',
      isDeleted: false,
      senderName: null,
    );

    final currentMessages = currentState.messages[event.conversationId] ?? [];
    final tempModel = MessageModel.fromEntity(tempMessage);
    emit(currentState.copyWith(
      messages: {
        ...currentState.messages,
        event.conversationId: [tempModel, ...currentMessages],
      },
    ));

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
        // Remove optimistic message and show error
        emit(currentState.copyWith(
          messages: {
            ...currentState.messages,
            event.conversationId: currentMessages,
          },
          error: _mapFailureToMessage(failure),
        ));
      },
      (message) async {
        // Replace temp message with real one
        final updatedMessages = [
          message,
          ...currentMessages.where((m) => m.id != tempMessage.id),
        ];
        emit(currentState.copyWith(
          messages: {
            ...currentState.messages,
            event.conversationId: updatedMessages,
          },
        ));
      },
    );
  }

  // Legacy _onSendImages removed; UI handles local progress now.

  // Legacy _onUpdateImageUploadProgress removed; UI handles local progress now.

  // Public helper retained: UI may call uploadAttachmentWithProgress directly.

  // ... باقي event handlers ...

  Future<void> _onSendTypingIndicator(
    SendTypingIndicatorEvent event,
    Emitter<ChatState> emit,
  ) async {
    webSocketService.sendTypingIndicator(
      event.conversationId,
      event.isTyping,
    );
  }

  Future<void> _onWebSocketMessageReceived(
    WebSocketMessageReceivedEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;
    final messageEvent = event.messageEvent;

    switch (messageEvent.type) {
      case MessageEventType.newMessage:
        if (messageEvent.message != null) {
          final currentMessages =
              currentState.messages[messageEvent.conversationId] ?? [];
          // Insert message at top (reverse list), update conversation ordering by updatedAt
          final List<Message> newList = [
            messageEvent.message!,
            ...currentMessages
          ];
          // Also bump the conversation's updatedAt if present
          final conversations = currentState.conversations.map((c) {
            if (c.id == messageEvent.conversationId) {
              return Conversation(
                id: c.id,
                conversationType: c.conversationType,
                title: c.title,
                description: c.description,
                avatar: c.avatar,
                createdAt: c.createdAt,
                updatedAt: messageEvent.message!.updatedAt,
                lastMessage: messageEvent.message!,
                unreadCount: (c.unreadCount + 1),
                isArchived: c.isArchived,
                isMuted: c.isMuted,
                propertyId: c.propertyId,
                participants: c.participants,
              );
            }
            return c;
          }).toList();
          conversations.sort((a, b) => b.updatedAt.compareTo(a.updatedAt));
          emit(currentState.copyWith(
            messages: {
              ...currentState.messages,
              messageEvent.conversationId: newList,
            },
            conversations: conversations,
          ));
        } else {
          // في حال لم يصل جسم الرسالة (FCM data فقط)، اجلب آخر الرسائل لهذا الحوار
          final result = await getMessagesUseCase(
            GetMessagesParams(
              conversationId: messageEvent.conversationId,
              pageNumber: 1,
              pageSize: 50,
            ),
          );
          await result.fold(
            (failure) async {
              emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
            },
            (messages) async {
              // Update messages in memory
              final updatedMessagesMap = {
                ...currentState.messages,
                messageEvent.conversationId: messages,
              };

              // Also update the conversation's lastMessage and reorder list
              List<Conversation> updatedConversations =
                  currentState.conversations.map((c) {
                if (c.id == messageEvent.conversationId) {
                  final last =
                      messages.isNotEmpty ? messages.first : c.lastMessage;
                  return Conversation(
                    id: c.id,
                    conversationType: c.conversationType,
                    title: c.title,
                    description: c.description,
                    avatar: c.avatar,
                    createdAt: c.createdAt,
                    updatedAt: (last?.updatedAt ?? DateTime.now()),
                    lastMessage: last ?? c.lastMessage,
                    unreadCount: c.unreadCount + 1,
                    isArchived: c.isArchived,
                    isMuted: c.isMuted,
                    propertyId: c.propertyId,
                    participants: c.participants,
                  );
                }
                return c;
              }).toList();
              updatedConversations
                  .sort((a, b) => b.updatedAt.compareTo(a.updatedAt));

              emit(currentState.copyWith(
                messages: updatedMessagesMap,
                conversations: updatedConversations,
              ));
            },
          );
        }
        break;
      case MessageEventType.statusUpdated:
        {
          // Update status locally for the specific messageId if present
          if (messageEvent.messageId != null && messageEvent.status != null) {
            final currentMessages =
                currentState.messages[messageEvent.conversationId] ?? [];
            bool found = false;
            final updatedMessages = currentMessages.map((m) {
              if (m.id == messageEvent.messageId) {
                found = true;
                return Message(
                  id: m.id,
                  conversationId: m.conversationId,
                  senderId: m.senderId,
                  messageType: m.messageType,
                  content: m.content,
                  location: m.location,
                  replyToMessageId: m.replyToMessageId,
                  reactions: m.reactions,
                  attachments: m.attachments,
                  createdAt: m.createdAt,
                  updatedAt: DateTime.now(),
                  status: messageEvent.status!,
                  isEdited: m.isEdited,
                  editedAt: m.editedAt,
                  deliveryReceipt: m.deliveryReceipt,
                  isDeleted: m.isDeleted,
                  senderName: m.senderName,
                );
              }
              return m;
            }).toList();

            if (found) {
              // Decrement unread count if message transitioned to read and it's from other user
              final conversations = currentState.conversations.map((c) {
                if (c.id == messageEvent.conversationId &&
                    messageEvent.status == 'read') {
                  final dec = (c.unreadCount > 0) ? c.unreadCount - 1 : 0;
                  return Conversation(
                    id: c.id,
                    conversationType: c.conversationType,
                    title: c.title,
                    description: c.description,
                    avatar: c.avatar,
                    createdAt: c.createdAt,
                    updatedAt: DateTime.now(),
                    lastMessage: c.lastMessage,
                    unreadCount: dec,
                    isArchived: c.isArchived,
                    isMuted: c.isMuted,
                    propertyId: c.propertyId,
                    participants: c.participants,
                  );
                }
                return c;
              }).toList();
              emit(currentState.copyWith(
                messages: {
                  ...currentState.messages,
                  messageEvent.conversationId: updatedMessages,
                },
                conversations: conversations,
              ));
            } else {
              // If message not found in memory (e.g., user in list view), fetch latest page
              final result = await getMessagesUseCase(
                GetMessagesParams(
                  conversationId: messageEvent.conversationId,
                  pageNumber: 1,
                  pageSize: 50,
                ),
              );
              await result.fold(
                (failure) async => emit(currentState.copyWith(
                    error: _mapFailureToMessage(failure))),
                (messages) async => emit(currentState.copyWith(messages: {
                  ...currentState.messages,
                  messageEvent.conversationId: messages,
                })),
              );
            }
          } else {
            // Fallback: fetch latest messages if we lack ids
            final result = await getMessagesUseCase(
              GetMessagesParams(
                conversationId: messageEvent.conversationId,
                pageNumber: 1,
                pageSize: 50,
              ),
            );
            await result.fold(
              (failure) async => emit(
                  currentState.copyWith(error: _mapFailureToMessage(failure))),
              (messages) async => emit(currentState.copyWith(messages: {
                ...currentState.messages,
                messageEvent.conversationId: messages,
              })),
            );
          }
        }
        break;
      case MessageEventType.reactionAdded:
      case MessageEventType.reactionRemoved:
        {
          if (messageEvent.messageId != null && messageEvent.reaction != null) {
            final currentMessages =
                currentState.messages[messageEvent.conversationId] ?? [];
            final updatedMessages = currentMessages.map((m) {
              if (m.id == messageEvent.messageId) {
                if (messageEvent.type == MessageEventType.reactionAdded) {
                  final reactions = List<MessageReaction>.from(m.reactions);
                  final exists = reactions.any((r) =>
                      r.userId == messageEvent.reaction!.userId &&
                      r.reactionType == messageEvent.reaction!.reactionType);
                  if (!exists) {
                    reactions.add(messageEvent.reaction!);
                  }
                  final updatedMessage = Message(
                    id: m.id,
                    conversationId: m.conversationId,
                    senderId: m.senderId,
                    messageType: m.messageType,
                    content: m.content,
                    location: m.location,
                    replyToMessageId: m.replyToMessageId,
                    reactions: reactions,
                    attachments: m.attachments,
                    createdAt: m.createdAt,
                    updatedAt: DateTime.now(),
                    status: m.status,
                    isEdited: m.isEdited,
                    editedAt: m.editedAt,
                    deliveryReceipt: m.deliveryReceipt,
                    isDeleted: m.isDeleted,
                    senderName: m.senderName,
                  );
                  // Update lastMessage in conversation if this is the last one
                  _bumpConversationForMessage(currentState,
                      messageEvent.conversationId, updatedMessage);
                  return updatedMessage;
                } else {
                  final reactions = m.reactions
                      .where((r) =>
                          !(r.userId == messageEvent.reaction!.userId &&
                              r.reactionType ==
                                  messageEvent.reaction!.reactionType))
                      .toList();
                  final updatedMessage = Message(
                    id: m.id,
                    conversationId: m.conversationId,
                    senderId: m.senderId,
                    messageType: m.messageType,
                    content: m.content,
                    location: m.location,
                    replyToMessageId: m.replyToMessageId,
                    reactions: reactions,
                    attachments: m.attachments,
                    createdAt: m.createdAt,
                    updatedAt: DateTime.now(),
                    status: m.status,
                    isEdited: m.isEdited,
                    editedAt: m.editedAt,
                    deliveryReceipt: m.deliveryReceipt,
                    isDeleted: m.isDeleted,
                    senderName: m.senderName,
                  );
                  _bumpConversationForMessage(currentState,
                      messageEvent.conversationId, updatedMessage);
                  return updatedMessage;
                }
              }
              return m;
            }).toList();

            emit(currentState.copyWith(
              messages: {
                ...currentState.messages,
                messageEvent.conversationId: updatedMessages,
              },
            ));
          } else {
            final result = await getMessagesUseCase(
              GetMessagesParams(
                conversationId: messageEvent.conversationId,
                pageNumber: 1,
                pageSize: 50,
              ),
            );
            await result.fold(
              (failure) async => emit(
                  currentState.copyWith(error: _mapFailureToMessage(failure))),
              (messages) async => emit(currentState.copyWith(messages: {
                ...currentState.messages,
                messageEvent.conversationId: messages,
              })),
            );
          }
        }
        break;
      case MessageEventType.edited:
        if (messageEvent.message != null) {
          final currentMessages =
              currentState.messages[messageEvent.conversationId] ?? [];
          final updatedMessages = currentMessages.map((m) {
            return m.id == messageEvent.message!.id ? messageEvent.message! : m;
          }).toList();
          emit(currentState.copyWith(
            messages: {
              ...currentState.messages,
              messageEvent.conversationId: updatedMessages,
            },
          ));
        }
        break;

      case MessageEventType.deleted:
        if (messageEvent.messageId != null) {
          final currentMessages =
              currentState.messages[messageEvent.conversationId] ?? [];
          final updatedMessages = currentMessages
              .where((m) => m.id != messageEvent.messageId)
              .toList();
          emit(currentState.copyWith(
            messages: {
              ...currentState.messages,
              messageEvent.conversationId: updatedMessages,
            },
          ));
        }
        break;
    }
  }

  Future<void> _onWebSocketConversationUpdated(
    WebSocketConversationUpdatedEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;
    // Merge or insert, then sort by updatedAt desc to keep list stable
    final List<Conversation> merged = [];
    bool inserted = false;
    for (int i = 0; i < currentState.conversations.length; i++) {
      final c = currentState.conversations[i];
      if (c.id == event.conversation.id) {
        if (!inserted) {
          merged.add(event.conversation);
          inserted = true;
        }
      } else {
        merged.add(c);
      }
    }
    if (!inserted) merged.add(event.conversation);
    merged.sort((a, b) => b.updatedAt.compareTo(a.updatedAt));
    emit(currentState.copyWith(conversations: merged));
  }

  // Ensure conversation list stays sorted and lastMessage reflects latest mutations (e.g., reactions)
  void _bumpConversationForMessage(
      ChatLoaded currentState, String conversationId, Message updatedMessage) {
    try {
      final conversations = currentState.conversations.map((c) {
        if (c.id == conversationId &&
            (c.lastMessage?.id == updatedMessage.id)) {
          return Conversation(
            id: c.id,
            conversationType: c.conversationType,
            title: c.title,
            description: c.description,
            avatar: c.avatar,
            createdAt: c.createdAt,
            updatedAt: DateTime.now(),
            lastMessage: updatedMessage,
            unreadCount: c.unreadCount,
            isArchived: c.isArchived,
            isMuted: c.isMuted,
            propertyId: c.propertyId,
            participants: c.participants,
          );
        }
        return c;
      }).toList();
      conversations.sort((a, b) => b.updatedAt.compareTo(a.updatedAt));
      // ignore: invalid_use_of_visible_for_testing_member
      emit(currentState.copyWith(conversations: conversations));
    } catch (_) {}
  }

  Future<void> _onWebSocketTypingIndicator(
    WebSocketTypingIndicatorEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;
    emit(currentState.copyWith(
      typingUsers: {
        ...currentState.typingUsers,
        event.conversationId: event.typingUserIds,
      },
    ));
  }

  Future<void> _onWebSocketPresenceUpdate(
    WebSocketPresenceUpdateEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;
    emit(currentState.copyWith(
      userPresence: {
        ...currentState.userPresence,
        event.userId: UserPresence(
          status: event.status,
          lastSeen: event.lastSeen,
        ),
      },
    ));
  }

  String _mapFailureToMessage(Failure failure) {
    switch (failure.runtimeType) {
      case ServerFailure:
        return 'حدث خطأ في الخادم';
      case NetworkFailure:
        return 'لا يوجد اتصال بالإنترنت';
      case CacheFailure:
        return 'حدث خطأ في التخزين المحلي';
      default:
        return 'حدث خطأ غير متوقع';
    }
  }

  @override
  Future<void> close() {
    _messageSubscription?.cancel();
    _conversationSubscription?.cancel();
    _typingSubscription?.cancel();
    _presenceSubscription?.cancel();
    webSocketService.dispose();
    return super.close();
  }

  Future<void> _onCreateConversation(
    CreateConversationEvent event,
    Emitter<ChatState> emit,
  ) async {
    final currentState = state;

    emit(const ConversationCreating());

    // منع أي محادثة ليست direct أو تتجاوز شخصًا واحدًا (غير المستخدم الحالي)
    if (event.conversationType != 'direct' ||
        event.participantIds.length != 1) {
      if (currentState is ChatLoaded) {
        emit(currentState.copyWith(error: 'يُسمح فقط بمحادثات ثنائية مباشرة'));
      } else {
        emit(const ChatError(message: 'يُسمح فقط بمحادثات ثنائية مباشرة'));
      }
      return;
    }

    // للمحادثات الفردية، تحقق من وجود محادثة سابقة
    if (event.conversationType == 'direct' && currentState is ChatLoaded) {
      Conversation? existingConversation;

      for (final conversation in currentState.conversations) {
        if (conversation.conversationType != 'direct') continue;
        if (conversation.participants.length != 2) continue;

        final participantIds = conversation.participants
            .map((participant) => participant.id)
            .toList();

        bool hasTargetParticipant = false;
        for (final targetId in event.participantIds) {
          if (participantIds.contains(targetId)) {
            hasTargetParticipant = true;
            break;
          }
        }

        if (hasTargetParticipant) {
          existingConversation = conversation;
          break;
        }
      }

      if (existingConversation != null) {
        emit(ConversationCreated(
          conversation: existingConversation,
          message: 'المحادثة موجودة بالفعل',
        ));

        await Future.delayed(const Duration(milliseconds: 100));
        emit(currentState);
        return;
      }
    }

    // إنشاء محادثة جديدة
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
      (failure) async {
        if (currentState is ChatLoaded) {
          emit(currentState.copyWith(error: _mapFailureToMessage(failure)));
        } else {
          emit(ChatError(message: _mapFailureToMessage(failure)));
        }
      },
      (conversation) async {
        emit(ConversationCreated(conversation: conversation));

        if (currentState is ChatLoaded) {
          final List<Conversation> updatedConversations = [
            conversation,
          ];

          for (final conv in currentState.conversations) {
            if (conv.id != conversation.id) {
              updatedConversations.add(conv);
            }
          }

          await Future.delayed(const Duration(milliseconds: 100));

          emit(currentState.copyWith(
            conversations: updatedConversations,
          ));
        } else {
          add(const LoadConversationsEvent());
        }
      },
    );
  }

  Future<void> _onDeleteConversation(
      DeleteConversationEvent event, Emitter<ChatState> emit) async {
    final result = await deleteConversationUseCase(
      DeleteConversationParams(conversationId: event.conversationId),
    );

    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          emit(current.copyWith(error: _mapFailureToMessage(failure)));
        }
      },
      (_) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          emit(current.copyWith(
            conversations: current.conversations
                .where((c) => c.id != event.conversationId)
                .toList(),
          ));
        }
      },
    );
  }

  Future<void> _onArchiveConversation(
      ArchiveConversationEvent event, Emitter<ChatState> emit) async {
    final result = await archiveConversationUseCase(
      ArchiveConversationParams(conversationId: event.conversationId),
    );

    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          emit(current.copyWith(error: _mapFailureToMessage(failure)));
        }
      },
      (_) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          final updated = current.conversations.map((c) {
            if (c.id == event.conversationId) {
              return Conversation(
                id: c.id,
                conversationType: c.conversationType,
                title: c.title,
                description: c.description,
                avatar: c.avatar,
                createdAt: c.createdAt,
                updatedAt: DateTime.now(),
                lastMessage: c.lastMessage,
                unreadCount: c.unreadCount,
                isArchived: true,
                isMuted: c.isMuted,
                propertyId: c.propertyId,
                participants: c.participants,
              );
            }
            return c;
          }).toList();
          emit(current.copyWith(conversations: updated));
        }
      },
    );
  }

  Future<void> _onUnarchiveConversation(
      UnarchiveConversationEvent event, Emitter<ChatState> emit) async {
    final result = await unarchiveConversationUseCase(
      UnarchiveConversationParams(conversationId: event.conversationId),
    );

    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          emit(current.copyWith(error: _mapFailureToMessage(failure)));
        }
      },
      (_) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          final updated = current.conversations.map((c) {
            if (c.id == event.conversationId) {
              return Conversation(
                id: c.id,
                conversationType: c.conversationType,
                title: c.title,
                description: c.description,
                avatar: c.avatar,
                createdAt: c.createdAt,
                updatedAt: DateTime.now(),
                lastMessage: c.lastMessage,
                unreadCount: c.unreadCount,
                isArchived: false,
                isMuted: c.isMuted,
                propertyId: c.propertyId,
                participants: c.participants,
              );
            }
            return c;
          }).toList();
          emit(current.copyWith(conversations: updated));
        }
      },
    );
  }

  Future<void> _onDeleteMessage(
      DeleteMessageEvent event, Emitter<ChatState> emit) async {
    final result = await deleteMessageUseCase(
      DeleteMessageParams(messageId: event.messageId),
    );

    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          emit(current.copyWith(error: _mapFailureToMessage(failure)));
        }
      },
      (_) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          // ابحث عن المحادثة التي تحتوي الرسالة واحذفها من قائمتها إن وُجدت
          final updatedMessages = <String, List<Message>>{};
          current.messages.forEach((convId, msgs) {
            updatedMessages[convId] =
                msgs.where((m) => m.id != event.messageId).toList();
          });
          emit(current.copyWith(messages: updatedMessages));
        }
      },
    );
  }

  Future<void> _onEditMessage(
      EditMessageEvent event, Emitter<ChatState> emit) async {
    final result = await editMessageUseCase(
      EditMessageParams(messageId: event.messageId, content: event.content),
    );

    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          emit(current.copyWith(error: _mapFailureToMessage(failure)));
        }
      },
      (message) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          final convId = message.conversationId;
          final msgs = current.messages[convId] ?? [];
          final updated =
              msgs.map((m) => m.id == message.id ? message : m).toList();
          emit(current.copyWith(messages: {
            ...current.messages,
            convId: updated,
          }));
        }
      },
    );
  }

  Future<void> _onAddReaction(
      AddReactionEvent event, Emitter<ChatState> emit) async {
    // Optimistic toggle/switch: ensure only one reaction per user
    if (state is ChatLoaded) {
      final current = state as ChatLoaded;
      final Map<String, List<Message>> updated = {...current.messages};
      updated.forEach((convId, msgs) {
        bool changed = false;
        final newMsgs = msgs.map((m) {
          if (m.id == event.messageId) {
            final String currentUserId =
                (event.currentUserId != null && event.currentUserId!.isNotEmpty)
                    ? event.currentUserId!
                    : 'current_user';

            final List<MessageReaction> reactions =
                List<MessageReaction>.from(m.reactions);

            // Remove any existing reaction by this user
            reactions.removeWhere((r) => r.userId == currentUserId);

            // If the same type wasn't already set, add the new one
            reactions.add(MessageReaction(
              id: 'temp_${DateTime.now().microsecondsSinceEpoch}',
              messageId: m.id,
              userId: currentUserId,
              reactionType: event.reactionType,
            ));

            changed = true;
            return MessageModel(
              id: m.id,
              conversationId: m.conversationId,
              senderId: m.senderId,
              messageType: m.messageType,
              content: m.content,
              location: m.location,
              replyToMessageId: m.replyToMessageId,
              reactions: reactions,
              attachments: m.attachments,
              createdAt: m.createdAt,
              updatedAt: DateTime.now(),
              status: m.status,
              isEdited: m.isEdited,
              editedAt: m.editedAt,
              deliveryReceipt: m.deliveryReceipt,
              isDeleted: m.isDeleted,
              senderName: m.senderName,
            );
          }
          return m;
        }).toList();
        if (changed) updated[convId] = newMsgs;
      });
      emit(current.copyWith(messages: updated));
    }

    final result = await addReactionUseCase(
      AddReactionParams(
          messageId: event.messageId, reactionType: event.reactionType),
    );
    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          // rollback optimistic add
          final Map<String, List<Message>> rolled = {...current.messages};
          rolled.forEach((convId, msgs) {
            for (int i = 0; i < msgs.length; i++) {
              final m = msgs[i];
              if (m.id == event.messageId) {
                final reactions = m.reactions
                    .where((r) => !(r.id.startsWith('temp_') &&
                        r.reactionType == event.reactionType))
                    .toList();
                msgs[i] = Message(
                  id: m.id,
                  conversationId: m.conversationId,
                  senderId: m.senderId,
                  messageType: m.messageType,
                  content: m.content,
                  location: m.location,
                  replyToMessageId: m.replyToMessageId,
                  reactions: reactions,
                  attachments: m.attachments,
                  createdAt: m.createdAt,
                  updatedAt: m.updatedAt,
                  status: m.status,
                  isEdited: m.isEdited,
                  editedAt: m.editedAt,
                  deliveryReceipt: m.deliveryReceipt,
                  isDeleted: m.isDeleted,
                  senderName: m.senderName,
                );
                break;
              }
            }
          });
          emit(current.copyWith(
              messages: rolled, error: _mapFailureToMessage(failure)));
        }
      },
      (_) async {},
    );
  }

  Future<void> _onRemoveReaction(
      RemoveReactionEvent event, Emitter<ChatState> emit) async {
    // Optimistic removal: remove only this user's reaction of given type
    if (state is ChatLoaded) {
      final current = state as ChatLoaded;
      final Map<String, List<Message>> updated = {...current.messages};
      updated.forEach((convId, msgs) {
        bool changed = false;
        final newMsgs = msgs.map((m) {
          if (m.id == event.messageId) {
            final reactions = m.reactions
                .where((r) => !(r.reactionType == event.reactionType &&
                    (event.currentUserId == null ||
                        r.userId == event.currentUserId)))
                .toList();
            changed = true;
            return MessageModel(
              id: m.id,
              conversationId: m.conversationId,
              senderId: m.senderId,
              messageType: m.messageType,
              content: m.content,
              location: m.location,
              replyToMessageId: m.replyToMessageId,
              reactions: reactions,
              attachments: m.attachments,
              createdAt: m.createdAt,
              updatedAt: DateTime.now(),
              status: m.status,
              isEdited: m.isEdited,
              editedAt: m.editedAt,
              deliveryReceipt: m.deliveryReceipt,
              isDeleted: m.isDeleted,
              senderName: m.senderName,
            );
          }
          return m;
        }).toList();
        if (changed) updated[convId] = newMsgs;
      });
      emit(current.copyWith(messages: updated));
    }

    final result = await removeReactionUseCase(
      RemoveReactionParams(
          messageId: event.messageId, reactionType: event.reactionType),
    );
    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          // rollback removal by refetching this message list (fallback)
          emit(current.copyWith(error: _mapFailureToMessage(failure)));
        }
      },
      (_) async {},
    );
  }

  Future<void> _onMarkMessagesAsRead(
      MarkMessagesAsReadEvent event, Emitter<ChatState> emit) async {
    final result = await markAsReadUseCase(
      MarkAsReadParams(
        conversationId: event.conversationId,
        messageIds: event.messageIds,
      ),
    );
    await result.fold(
      (failure) async {
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          emit(current.copyWith(error: _mapFailureToMessage(failure)));
        }
      },
      (_) async {
        // After marking as read, update unreadCount to 0 for that conversation
        if (state is ChatLoaded) {
          final current = state as ChatLoaded;
          final conversations = current.conversations.map((c) {
            if (c.id == event.conversationId) {
              return Conversation(
                id: c.id,
                conversationType: c.conversationType,
                title: c.title,
                description: c.description,
                avatar: c.avatar,
                createdAt: c.createdAt,
                updatedAt: DateTime.now(),
                lastMessage: c.lastMessage,
                unreadCount: 0,
                isArchived: c.isArchived,
                isMuted: c.isMuted,
                propertyId: c.propertyId,
                participants: c.participants,
              );
            }
            return c;
          }).toList();
          conversations.sort((a, b) => b.updatedAt.compareTo(a.updatedAt));
          emit(current.copyWith(conversations: conversations));
        }
      },
    );
  }

  Future<void> _onUploadAttachment(
      UploadAttachmentEvent event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
    // Bootstrap a synthetic uploading bubble immediately for non-image files (e.g., audio)
    try {
      final fileName = event.filePath.split('/')..removeWhere((s) => s.isEmpty);
      final name = (fileName.isNotEmpty) ? fileName.last : 'file';
      String lower = name.toLowerCase();
      String contentType = 'application/octet-stream';
      if (lower.endsWith('.m4a') || lower.endsWith('.aac'))
        contentType = 'audio/mp4';
      else if (lower.endsWith('.mp3'))
        contentType = 'audio/mpeg';
      else if (lower.endsWith('.wav'))
        contentType = 'audio/wav';
      else if (lower.endsWith('.ogg') || lower.endsWith('.opus'))
        contentType = 'audio/ogg';
      else if (lower.endsWith('.mp4') ||
          lower.contains('.mov') ||
          lower.contains('.mkv') ||
          lower.contains('.webm'))
        contentType = 'video/mp4';
      else if (lower.endsWith('.jpg') || lower.endsWith('.jpeg'))
        contentType = 'image/jpeg';
      else if (lower.endsWith('.png')) contentType = 'image/png';

      // فقط أظهر الفقاعة المباشرة لغير الصور (الصوت/الفيديو/المستندات)
      if (!contentType.startsWith('image/')) {
        final tempAttachment = Attachment(
          id: 'temp_${DateTime.now().microsecondsSinceEpoch}',
          conversationId: event.conversationId,
          fileName: name,
          contentType: contentType,
          fileSize: 0,
          filePath: event.filePath,
          fileUrl: '',
          url: '',
          uploadedBy: 'current_user',
          createdAt: DateTime.now(),
          duration: null,
          downloadProgress: 0.0,
        );
        emit(current.copyWith(
            uploadingAttachment: tempAttachment, uploadProgress: 0.0));
      } else {
        // الصور تُدار عبر مسار الرفع الجماعي الحالي
        emit(current.copyWith(uploadingAttachment: null, uploadProgress: 0));
      }
    } catch (_) {
      emit(current.copyWith(uploadingAttachment: null, uploadProgress: 0));
    }

    final result = await uploadAttachmentUseCase(
      UploadAttachmentParams(
        conversationId: event.conversationId,
        filePath: event.filePath,
        messageType: event.messageType,
        onSendProgress: (sent, total) {
          add(_UploadProgressInternal(sent: sent, total: total));
        },
      ),
    );

    await result.fold(
      (failure) async {
        emit(current.copyWith(
          uploadingAttachment: null,
          uploadProgress: null,
          error: _mapFailureToMessage(failure),
        ));
      },
      (attachment) async {
        // Show progress bubble immediately for non-image attachments
        if (attachment.contentType.startsWith('audio/') ||
            attachment.contentType.startsWith('video/') ||
            (!attachment.contentType.startsWith('image/'))) {
          emit(current.copyWith(
            uploadingAttachment: attachment,
            uploadProgress: 1.0,
          ));
        }

        // Immediately send a message referencing this attachment
        final sendResult = await sendMessageUseCase(
          SendMessageParams(
            conversationId: event.conversationId,
            messageType: event.messageType,
            content: null, // لا نعرض الرابط داخل الفقاعة
            location: null,
            replyToMessageId: null,
            attachmentIds: [attachment.id],
          ),
        );

        await sendResult.fold(
          (failure) async {
            // Reset uploading state and surface error
            final latest = state is ChatLoaded ? state as ChatLoaded : current;
            emit(latest.copyWith(
              error: _mapFailureToMessage(failure),
            ));
          },
          (message) async {
            // Insert message at top and bump conversation ordering
            final latest = state is ChatLoaded ? state as ChatLoaded : current;
            final currentMessages = latest.messages[event.conversationId] ?? [];
            final updatedMessages = [
              message,
              ...currentMessages,
            ];

            var conversations = latest.conversations.map((c) {
              if (c.id == event.conversationId) {
                return Conversation(
                  id: c.id,
                  conversationType: c.conversationType,
                  title: c.title,
                  description: c.description,
                  avatar: c.avatar,
                  createdAt: c.createdAt,
                  updatedAt: message.updatedAt,
                  lastMessage: message,
                  unreadCount: c.unreadCount,
                  isArchived: c.isArchived,
                  isMuted: c.isMuted,
                  propertyId: c.propertyId,
                  participants: c.participants,
                );
              }
              return c;
            }).toList();
            conversations.sort((a, b) => b.updatedAt.compareTo(a.updatedAt));

            emit(latest.copyWith(
              messages: {
                ...latest.messages,
                event.conversationId: updatedMessages,
              },
              conversations: conversations,
              uploadingAttachment: null,
              uploadProgress: null,
            ));
          },
        );
      },
    );
  }

  // Track per-conversation in-bubble image upload progress and render via state
  Future<void> _onStartImageUploads(
      StartImageUploadsEvent event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
    final updated =
        Map<String, List<ImageUploadInfo>>.from(current.uploadingImages);
    updated[event.conversationId] = event.uploads;
    emit(current.copyWith(uploadingImages: updated));
  }

  Future<void> _onUpdateImageUploadProgress(
      UpdateImageUploadProgressEvent event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
    final convoUploads = List<ImageUploadInfo>.from(
        current.uploadingImages[event.conversationId] ?? const []);
    final index = convoUploads.indexWhere((u) => u.id == event.uploadId);
    if (index >= 0) {
      final prev = convoUploads[index];
      final next = prev.copyWith(
        progress: event.progress ?? prev.progress,
        isCompleted: event.isCompleted ?? prev.isCompleted,
        isFailed: event.isFailed ?? prev.isFailed,
        error: event.error ?? prev.error,
      );
      convoUploads[index] = next;
      final updated =
          Map<String, List<ImageUploadInfo>>.from(current.uploadingImages);
      updated[event.conversationId] = convoUploads;
      emit(current.copyWith(uploadingImages: updated));
    }
  }

  Future<void> _onFinishImageUploads(
      FinishImageUploadsEvent event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
    final updated =
        Map<String, List<ImageUploadInfo>>.from(current.uploadingImages);
    updated.remove(event.conversationId);
    emit(current.copyWith(uploadingImages: updated));
  }

  Future<void> _onSearchChats(
      SearchChatsEvent event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
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
      (failure) async =>
          emit(current.copyWith(error: _mapFailureToMessage(failure))),
      (res) async => emit(current.copyWith(searchResult: res)),
    );
  }

  Future<void> _onLoadAvailableUsers(
      LoadAvailableUsersEvent event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
    final result = await getAvailableUsersUseCase(
      GetAvailableUsersParams(
          userType: event.userType, propertyId: event.propertyId),
    );
    await result.fold(
      (failure) async =>
          emit(current.copyWith(error: _mapFailureToMessage(failure))),
      (users) async => emit(current.copyWith(availableUsers: users)),
    );
  }

  Future<void> _onUpdateUserStatus(
      UpdateUserStatusEvent event, Emitter<ChatState> emit) async {
    final result = await updateUserStatusUseCase(
        UpdateUserStatusParams(status: event.status));
    await result.fold((failure) async {
      if (state is ChatLoaded) {
        final current = state as ChatLoaded;
        emit(current.copyWith(error: _mapFailureToMessage(failure)));
      }
    }, (_) async {});
  }

  Future<void> _onLoadChatSettings(
      LoadChatSettingsEvent event, Emitter<ChatState> emit) async {
    final result = await getChatSettingsUseCase(NoParams());
    await result.fold((failure) async {
      if (state is ChatLoaded) {
        final current = state as ChatLoaded;
        emit(current.copyWith(error: _mapFailureToMessage(failure)));
      }
    }, (settings) async {
      if (state is ChatLoaded) {
        final current = state as ChatLoaded;
        emit(current.copyWith(settings: settings));
      }
    });
  }

  Future<void> _onUpdateChatSettings(
      UpdateChatSettingsEvent event, Emitter<ChatState> emit) async {
    final result = await updateChatSettingsUseCase(UpdateChatSettingsParams(
      notificationsEnabled: event.notificationsEnabled,
      soundEnabled: event.soundEnabled,
      showReadReceipts: event.showReadReceipts,
      showTypingIndicator: event.showTypingIndicator,
      theme: event.theme,
      fontSize: event.fontSize,
      autoDownloadMedia: event.autoDownloadMedia,
      backupMessages: event.backupMessages,
    ));
    await result.fold((failure) async {
      if (state is ChatLoaded) {
        final current = state as ChatLoaded;
        emit(current.copyWith(error: _mapFailureToMessage(failure)));
      }
    }, (settings) async {
      if (state is ChatLoaded) {
        final current = state as ChatLoaded;
        emit(current.copyWith(settings: settings));
      }
    });
  }

  // Internal event for updating upload progress (not exposed)
  Future<void> _onUploadProgressInternal(
      _UploadProgressInternal event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
    final progress = event.total > 0 ? event.sent / event.total : 0.0;
    emit(current.copyWith(uploadProgress: progress));
  }

  Future<void> _onLoadAdminUsers(
      LoadAdminUsersEvent event, Emitter<ChatState> emit) async {
    if (state is! ChatLoaded) return;
    final current = state as ChatLoaded;
    final result = await getAdminUsersUseCase(NoParams());
    await result.fold(
      (failure) async =>
          emit(current.copyWith(error: _mapFailureToMessage(failure))),
      (users) async => emit(current.copyWith(adminUsers: users)),
    );
  }

  // Public helper for uploading an attachment with progress and sending a message.
  // Used by UI to run sequential uploads while reflecting progress in an overlay.
  Future<void> uploadAttachmentWithProgress({
    required String conversationId,
    required String filePath,
    required String messageType,
    required void Function(int sent, int total) onProgress,
    String? replyToMessageId,
    String? replyToAttachmentId,
  }) async {
    final result = await uploadAttachmentUseCase(
      UploadAttachmentParams(
        conversationId: conversationId,
        filePath: filePath,
        messageType: messageType,
        onSendProgress: (sent, total) {
          // Update Bloc state and surface progress to caller
          add(_UploadProgressInternal(sent: sent, total: total));
          try {
            onProgress(sent, total);
          } catch (_) {}
        },
      ),
    );

    await result.fold(
      (failure) async {
        throw Exception(_mapFailureToMessage(failure));
      },
      (attachment) async {
        // Compose content with optional reply attachment token for precise reply previews
        String content = attachment.fileUrl;
        if (replyToAttachmentId != null && replyToAttachmentId.isNotEmpty) {
          content = '::attref=$replyToAttachmentId::$content';
        }

        final sendResult = await sendMessageUseCase(
          SendMessageParams(
            conversationId: conversationId,
            messageType: messageType,
            content: content,
            location: null,
            replyToMessageId: replyToMessageId,
            attachmentIds: [attachment.id],
          ),
        );

        await sendResult.fold(
          (failure) async => throw Exception(_mapFailureToMessage(failure)),
          (message) async {
            // Propagate the new message via existing handler to update state
            add(WebSocketMessageReceivedEvent(MessageEvent(
              type: MessageEventType.newMessage,
              message: message,
              conversationId: conversationId,
            )));
          },
        );
      },
    );
  }

  // Upload a single attachment and return it without sending any message
  Future<Attachment> uploadAttachmentOnly({
    required String conversationId,
    required String filePath,
    required String messageType,
    required void Function(int sent, int total) onProgress,
  }) async {
    final result = await uploadAttachmentUseCase(
      UploadAttachmentParams(
        conversationId: conversationId,
        filePath: filePath,
        messageType: messageType,
        onSendProgress: (sent, total) {
          add(_UploadProgressInternal(sent: sent, total: total));
          try {
            onProgress(sent, total);
          } catch (_) {}
        },
      ),
    );

    return await result.fold(
      (failure) async => throw Exception(_mapFailureToMessage(failure)),
      (attachment) async => attachment,
    );
  }

  // Upload multiple images, then send a single message that contains all attachments
  Future<void> uploadImagesAndSendSingleMessage({
    required String conversationId,
    required List<String> filePaths,
    required void Function(int index, int sent, int total) onProgress,
  }) async {
    final List<Attachment> uploaded = [];
    for (int i = 0; i < filePaths.length; i++) {
      final path = filePaths[i];
      final att = await uploadAttachmentOnly(
        conversationId: conversationId,
        filePath: path,
        messageType: 'image',
        onProgress: (s, t) => onProgress(i, s, t),
      );
      uploaded.add(att);
    }

    // send one message with all attachment ids
    final sendResult = await sendMessageUseCase(
      SendMessageParams(
        conversationId: conversationId,
        messageType: 'image',
        content: uploaded.isNotEmpty ? uploaded.first.fileUrl : null,
        location: null,
        replyToMessageId: null,
        attachmentIds: uploaded.map((a) => a.id).toList(),
      ),
    );

    await sendResult.fold(
      (failure) async => throw Exception(_mapFailureToMessage(failure)),
      (message) async {
        add(WebSocketMessageReceivedEvent(MessageEvent(
          type: MessageEventType.newMessage,
          message: message,
          conversationId: conversationId,
        )));
      },
    );
  }
}
