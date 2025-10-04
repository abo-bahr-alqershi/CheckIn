import 'dart:async';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:yemen_booking_app/services/websocket_service.dart';
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
import '../../domain/usecases/upload_attachment_usecase.dart';
import '../../domain/usecases/search_chats_usecase.dart';
import '../../domain/usecases/get_available_users_usecase.dart';
import '../../domain/usecases/get_admin_users_usecase.dart';
import '../../domain/usecases/update_user_status_usecase.dart';
import '../../domain/usecases/get_chat_settings_usecase.dart';
import '../../domain/usecases/update_chat_settings_usecase.dart';
import 'package:collection/collection.dart';

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

    _initializeWebSocket();
  }

  void _initializeWebSocket() {
    webSocketService.connect();

    _messageSubscription = webSocketService.messageEvents.listen((event) {
      add(WebSocketMessageReceivedEvent(event));
    });

    _conversationSubscription =
        webSocketService.conversationUpdates.listen((conversation) {
      add(WebSocketConversationUpdatedEvent(conversation));
    });

    _typingSubscription = webSocketService.typingEvents.listen((event) {
      add(WebSocketTypingIndicatorEvent(
        conversationId: event.conversationId,
        typingUserIds: event.typingUserIds,
      ));
    });

    _presenceSubscription = webSocketService.presenceEvents.listen((event) {
      add(WebSocketPresenceUpdateEvent(
        userId: event.userId,
        status: event.status,
        lastSeen: event.lastSeen,
      ));
    });
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
      senderId: 'current_user', // Should get from auth
      messageType: event.messageType,
      content: event.content,
      location: event.location,
      replyToMessageId: event.replyToMessageId,
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
      status: 'sending',
    );

    final currentMessages = currentState.messages[event.conversationId] ?? [];
    emit(currentState.copyWith(
      messages: {
        ...currentState.messages,
        event.conversationId: [tempMessage, ...currentMessages],
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
          emit(currentState.copyWith(
            messages: {
              ...currentState.messages,
              messageEvent.conversationId: [
                messageEvent.message!,
                ...currentMessages
              ],
            },
          ));
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

      default:
        break;
    }
  }

  Future<void> _onWebSocketConversationUpdated(
    WebSocketConversationUpdatedEvent event,
    Emitter<ChatState> emit,
  ) async {
    if (state is! ChatLoaded) return;

    final currentState = state as ChatLoaded;
    final updatedConversations = currentState.conversations.map((c) {
      return c.id == event.conversation.id ? event.conversation : c;
    }).toList();

    emit(currentState.copyWith(conversations: updatedConversations));
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
    // Implementation
  }

  Future<void> _onArchiveConversation(
      ArchiveConversationEvent event, Emitter<ChatState> emit) async {
    // Implementation
  }

  Future<void> _onUnarchiveConversation(
      UnarchiveConversationEvent event, Emitter<ChatState> emit) async {
    // Implementation
  }

  Future<void> _onDeleteMessage(
      DeleteMessageEvent event, Emitter<ChatState> emit) async {
    // Implementation
  }

  Future<void> _onEditMessage(
      EditMessageEvent event, Emitter<ChatState> emit) async {
    // Implementation
  }

  Future<void> _onAddReaction(
      AddReactionEvent event, Emitter<ChatState> emit) async {
    // Implementation
  }

  Future<void> _onRemoveReaction(
      RemoveReactionEvent event, Emitter<ChatState> emit) async {
    // Implementation
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
        // No-op; optimistic UI already shows as read
      },
    );
  }

  Future<void> _onUploadAttachment(
      UploadAttachmentEvent event, Emitter<ChatState> emit) async {
    // Implementation
  }

  Future<void> _onSearchChats(
      SearchChatsEvent event, Emitter<ChatState> emit) async {
    // Implementation
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
    // Implementation
  }

  Future<void> _onLoadChatSettings(
      LoadChatSettingsEvent event, Emitter<ChatState> emit) async {
    // Implementation
  }

  Future<void> _onUpdateChatSettings(
      UpdateChatSettingsEvent event, Emitter<ChatState> emit) async {
    // Implementation
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
}
