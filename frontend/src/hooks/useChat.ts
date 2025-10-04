import { useState, useEffect, useCallback, useRef } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { 
  ChatService, 
  ChatApiService, 
  FCMService, 
  ChatWebSocketService 
} from '../services/chat.services';
import type {
  ChatConversation,
  ChatMessage,
  ChatUser,
  SendMessageRequest,
  MessageType,
  ReactionType,
  MessageStatus,
  TypingIndicator,
  ChatSettings,
  ConversationsResponse,
  MessagesResponse,
  SearchResponse,
  CreateConversationRequest,
  FileUploadResponse,
  UserType,
  GetConversationsRequest,
  GetMessagesRequest,
  SearchChatsRequest,
  UpdateMessageStatusRequest,
  AddReactionRequest,
  ChatEvent,
  ChatEventType
} from '../types/chat.types';

// ===== Hook للشات الرئيسي =====
/**
 * Hook رئيسي لإدارة الشات
 * يوفر جميع الوظائف الأساسية للشات
 */
export function useChat(chatService: ChatService) {
  const queryClient = useQueryClient();
  const [conversations, setConversations] = useState<ChatConversation[]>([]);
  const [currentConversation, setCurrentConversation] = useState<ChatConversation | null>(null);
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [typingUsers, setTypingUsers] = useState<TypingIndicator[]>([]);
  const [onlineUsers, setOnlineUsers] = useState<Set<string>>(new Set());
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [hasMoreMessages, setHasMoreMessages] = useState(true);
  const [messagesPage, setMessagesPage] = useState(1);

  // ===== تحميل المحادثات =====
  const loadConversations = useCallback(async (filters?: GetConversationsRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await chatService.api.getConversations(filters || {});
      setConversations(response.conversations);
      return response;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تحميل المحادثات';
      setError(errorMessage);
      console.error('خطأ في تحميل المحادثات:', err);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, [chatService]);

  // ===== تحميل الرسائل =====
  const loadMessages = useCallback(async (conversationId: string, page = 1, limit = 50) => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await chatService.api.getMessages({
        conversation_id: conversationId,
        page,
        limit
      });

      if (page === 1) {
        setMessages(response.messages);
      } else {
        setMessages(prev => [...response.messages, ...prev]);
      }

      setHasMoreMessages(response.has_more);
      setMessagesPage(page);
      return response;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تحميل الرسائل';
      setError(errorMessage);
      console.error('خطأ في تحميل الرسائل:', err);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, [chatService]);

  // ===== تحميل المزيد من الرسائل =====
  const loadMoreMessages = useCallback(async () => {
    if (!currentConversation || !hasMoreMessages || isLoading) return;
    
    await loadMessages(currentConversation.conversation_id, messagesPage + 1);
  }, [currentConversation, hasMoreMessages, isLoading, messagesPage, loadMessages]);

  // ===== إرسال رسالة =====
  const sendMessage = useCallback(async (messageData: SendMessageRequest) => {
    try {
      setError(null);
      
      const message = await chatService.api.sendMessage(messageData);
      setMessages(prev => [...prev, message]);

      // تحديث آخر رسالة في المحادثة
      setConversations(prev => 
        prev.map(conv => 
          conv.conversation_id === messageData.conversation_id
            ? { ...conv, last_message: message, updated_at: new Date() }
            : conv
        )
      );

      // تحديث cache
      queryClient.invalidateQueries({ queryKey: ['conversations'] });
      queryClient.invalidateQueries({ queryKey: ['messages', messageData.conversation_id] });

      return message;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في إرسال الرسالة';
      setError(errorMessage);
      console.error('خطأ في إرسال الرسالة:', err);
      throw err;
    }
  }, [chatService, queryClient]);

  // ===== إضافة تفاعل =====
  const addReaction = useCallback(async (messageId: string, reactionType: ReactionType) => {
    try {
      await chatService.api.addReaction({ message_id: messageId, reaction_type: reactionType });
      
      // تحديث الرسالة محلياً
      setMessages(prev => 
        prev.map(msg => {
          if (msg.message_id === messageId) {
            const existingReaction = msg.reactions?.find(r => 
              r.user_id === chatService.user?.user_id && r.reaction_type === reactionType
            );
            
            if (existingReaction) {
              return msg; // التفاعل موجود بالفعل
            }

            const newReaction = {
              reaction_id: `temp-${Date.now()}`,
              user_id: chatService.user?.user_id || '',
              user_name: chatService.user?.name || '',
              user_avatar: chatService.user?.profile_image,
              reaction_type: reactionType,
              created_at: new Date()
            };

            return {
              ...msg,
              reactions: [...(msg.reactions || []), newReaction]
            };
          }
          return msg;
        })
      );
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في إضافة التفاعل';
      setError(errorMessage);
      console.error('خطأ في إضافة التفاعل:', err);
      throw err;
    }
  }, [chatService]);

  // ===== إزالة تفاعل =====
  const removeReaction = useCallback(async (messageId: string, reactionType: ReactionType) => {
    try {
      await chatService.api.removeReaction(messageId, reactionType);
      
      // تحديث الرسالة محلياً
      setMessages(prev => 
        prev.map(msg => {
          if (msg.message_id === messageId) {
            return {
              ...msg,
              reactions: msg.reactions?.filter(r => 
                !(r.user_id === chatService.user?.user_id && r.reaction_type === reactionType)
              )
            };
          }
          return msg;
        })
      );
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في إزالة التفاعل';
      setError(errorMessage);
      console.error('خطأ في إزالة التفاعل:', err);
      throw err;
    }
  }, [chatService]);

  // ===== إنشاء محادثة جديدة =====
  const createConversation = useCallback(async (request: CreateConversationRequest) => {
    try {
      setError(null);
      const conversation = await chatService.api.createConversation(request);
      setConversations(prev => [conversation, ...prev]);
      queryClient.invalidateQueries({ queryKey: ['conversations'] });
      return conversation;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في إنشاء المحادثة';
      setError(errorMessage);
      console.error('خطأ في إنشاء المحادثة:', err);
      throw err;
    }
  }, [chatService, queryClient]);

  // ===== حذف محادثة =====
  const deleteConversation = useCallback(async (conversationId: string) => {
    try {
      setError(null);
      await chatService.api.deleteConversation(conversationId);
      setConversations(prev => prev.filter(conv => conv.conversation_id !== conversationId));
      
      // إذا كانت المحادثة المحذوفة هي المحادثة الحالية، قم بإلغاء تحديدها
      if (currentConversation?.conversation_id === conversationId) {
        setCurrentConversation(null);
        setMessages([]);
      }

      queryClient.invalidateQueries({ queryKey: ['conversations'] });
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في حذف المحادثة';
      setError(errorMessage);
      console.error('خطأ في حذف المحادثة:', err);
      throw err;
    }
  }, [chatService, currentConversation, queryClient]);

  // ===== أرشفة محادثة =====
  const archiveConversation = useCallback(async (conversationId: string) => {
    try {
      setError(null);
      await chatService.api.archiveConversation(conversationId);
      setConversations(prev => 
        prev.map(conv => 
          conv.conversation_id === conversationId
            ? { ...conv, is_archived: true }
            : conv
        )
      );
      queryClient.invalidateQueries({ queryKey: ['conversations'] });
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في أرشفة المحادثة';
      setError(errorMessage);
      console.error('خطأ في أرشفة المحادثة:', err);
      throw err;
    }
  }, [chatService, queryClient]);

  // ===== إلغاء أرشفة محادثة =====
  const unarchiveConversation = useCallback(async (conversationId: string) => {
    try {
      setError(null);
      await chatService.api.unarchiveConversation(conversationId);
      setConversations(prev => 
        prev.map(conv => 
          conv.conversation_id === conversationId
            ? { ...conv, is_archived: false }
            : conv
        )
      );
      queryClient.invalidateQueries({ queryKey: ['conversations'] });
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في إلغاء أرشفة المحادثة';
      setError(errorMessage);
      console.error('خطأ في إلغاء أرشفة المحادثة:', err);
      throw err;
    }
  }, [chatService, queryClient]);

  // ===== تحديث حالة الرسالة =====
  const updateMessageStatus = useCallback(async (messageId: string, status: MessageStatus) => {
    try {
      await chatService.api.updateMessageStatus({
        message_id: messageId,
        status,
        read_by: chatService.user?.user_id
      });
      
      // تحديث الرسالة محلياً
      setMessages(prev => 
        prev.map(msg => 
          msg.message_id === messageId
            ? { ...msg, status }
            : msg
        )
      );
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تحديث حالة الرسالة';
      setError(errorMessage);
      console.error('خطأ في تحديث حالة الرسالة:', err);
      throw err;
    }
  }, [chatService]);

  // ===== وضع علامة "مقروء" على جميع الرسائل =====
  const markAllAsRead = useCallback(async (conversationId: string) => {
    try {
      const unreadMessages = messages.filter(msg => 
        msg.conversation_id === conversationId && 
        msg.status !== 'read' &&
        msg.sender_id !== chatService.user?.user_id
      );

      await Promise.all(
        unreadMessages.map(msg => 
          chatService.api.updateMessageStatus({
            message_id: msg.message_id,
            status: 'read',
            read_by: chatService.user?.user_id
          })
        )
      );

      // تحديث الرسائل محلياً
      setMessages(prev => 
        prev.map(msg => 
          msg.conversation_id === conversationId && msg.status !== 'read'
            ? { ...msg, status: 'read' }
            : msg
        )
      );

      // تحديث عدد الرسائل غير المقروءة في المحادثة
      setConversations(prev => 
        prev.map(conv => 
          conv.conversation_id === conversationId
            ? { ...conv, unread_count: 0 }
            : conv
        )
      );
    } catch (err) {
      console.error('خطأ في وضع علامة "مقروء" على الرسائل:', err);
    }
  }, [messages, chatService]);

  // ===== حذف رسالة =====
  const deleteMessage = useCallback(async (messageId: string) => {
    try {
      setError(null);
      await chatService.api.deleteMessage(messageId);
      setMessages(prev => prev.filter(msg => msg.message_id !== messageId));
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في حذف الرسالة';
      setError(errorMessage);
      console.error('خطأ في حذف الرسالة:', err);
      throw err;
    }
  }, [chatService]);

  // ===== تعديل رسالة =====
  const editMessage = useCallback(async (messageId: string, newContent: string) => {
    try {
      setError(null);
      const updatedMessage = await chatService.api.editMessage(messageId, newContent);
      setMessages(prev => 
        prev.map(msg => 
          msg.message_id === messageId ? updatedMessage : msg
        )
      );
      return updatedMessage;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تعديل الرسالة';
      setError(errorMessage);
      console.error('خطأ في تعديل الرسالة:', err);
      throw err;
    }
  }, [chatService]);

  // ===== تحديد المحادثة الحالية =====
  const selectConversation = useCallback(async (conversation: ChatConversation) => {
    setCurrentConversation(conversation);
    setMessages([]);
    setMessagesPage(1);
    setHasMoreMessages(true);
    
    await loadMessages(conversation.conversation_id);
    await markAllAsRead(conversation.conversation_id);
  }, [chatService, loadMessages, markAllAsRead]);

  // ===== مغادرة المحادثة الحالية =====
  const leaveCurrentConversation = useCallback(() => {
    if (currentConversation) {
      setCurrentConversation(null);
      setMessages([]);
      setMessagesPage(1);
      setHasMoreMessages(true);
    }
  }, [currentConversation]);

  // ===== مستمعي أحداث FCM (ممر عبر wsService event bus) =====
  useEffect(() => {
    const handleNewMessage = (message: ChatMessage) => {
      setMessages(prev => [...prev, message]);
      
      // تحديث المحادثة
      setConversations(prev => 
        prev.map(conv => 
          conv.conversation_id === message.conversation_id
            ? { 
                ...conv, 
                last_message: message, 
                updated_at: new Date(), 
                unread_count: message.sender_id !== chatService.user?.user_id 
                  ? conv.unread_count + 1 
                  : conv.unread_count
              }
            : conv
        )
      );

      // إذا كانت الرسالة في المحادثة الحالية ومن مستخدم آخر، وضع علامة "مقروء"
      if (message.conversation_id === currentConversation?.conversation_id && 
          message.sender_id !== chatService.user?.user_id) {
        updateMessageStatus(message.message_id, 'read');
      }
    };

    const handleMessageUpdated = (message: ChatMessage) => {
      setMessages(prev => 
        prev.map(msg => 
          msg.message_id === message.message_id ? message : msg
        )
      );
    };

    const handleMessageDeleted = (data: { message_id: string }) => {
      setMessages(prev => prev.filter(msg => msg.message_id !== data.message_id));
    };

    const handleTypingIndicator = (indicator: TypingIndicator) => {
      setTypingUsers(prev => {
        const filtered = prev.filter(t => t.user_id !== indicator.user_id);
        return indicator.is_typing ? [...filtered, indicator] : filtered;
      });
    };

    const handleUserStatusChanged = (data: { user_id: string, status: string }) => {
      if (data.status === 'online') {
        setOnlineUsers(prev => new Set([...prev, data.user_id]));
      } else {
        setOnlineUsers(prev => {
          const newSet = new Set(prev);
          newSet.delete(data.user_id);
          return newSet;
        });
      }
    };

    const handleConversationCreated = (conversation: ChatConversation) => {
      setConversations(prev => [conversation, ...prev]);
    };
    // Handle conversation updated (e.g., archived/unarchived)
    const handleConversationUpdated = (updatedConversation: ChatConversation) => {
      setConversations(prev =>
        prev.map(conv =>
          conv.conversation_id === updatedConversation.conversation_id ? updatedConversation : conv
        )
      );
    };

    const handleReactionAdded = (data: { message_id: string, reaction: any }) => {
      setMessages(prev => 
        prev.map(msg => 
          msg.message_id === data.message_id
            ? { ...msg, reactions: [...(msg.reactions || []), data.reaction] }
            : msg
        )
      );
    };

    const handleReactionRemoved = (data: { message_id: string, reaction_id: string }) => {
      setMessages(prev => 
        prev.map(msg => 
          msg.message_id === data.message_id
            ? { ...msg, reactions: msg.reactions?.filter(r => r.reaction_id !== data.reaction_id) }
            : msg
        )
      );
    };

    // إضافة المستمعين
    chatService.ws.on('new_message', handleNewMessage);
    chatService.ws.on('message_updated', handleMessageUpdated);
    chatService.ws.on('message_deleted', handleMessageDeleted);
    chatService.ws.on('typing_indicator', handleTypingIndicator);
    chatService.ws.on('user_status_changed', handleUserStatusChanged);
    chatService.ws.on('conversation_created', handleConversationCreated);
    chatService.ws.on('conversation_updated', handleConversationUpdated);
    chatService.ws.on('reaction_added', handleReactionAdded);
    chatService.ws.on('reaction_removed', handleReactionRemoved);

    // تنظيف المستمعين
    return () => {
      chatService.ws.off('new_message', handleNewMessage);
      chatService.ws.off('message_updated', handleMessageUpdated);
      chatService.ws.off('message_deleted', handleMessageDeleted);
      chatService.ws.off('typing_indicator', handleTypingIndicator);
      chatService.ws.off('user_status_changed', handleUserStatusChanged);
      chatService.ws.off('conversation_created', handleConversationCreated);
      chatService.ws.off('conversation_updated', handleConversationUpdated);
      chatService.ws.off('reaction_added', handleReactionAdded);
      chatService.ws.off('reaction_removed', handleReactionRemoved);
    };
  }, [chatService, currentConversation, updateMessageStatus]);

  // ===== تنظيف مؤشرات الكتابة =====
  useEffect(() => {
    const interval = setInterval(() => {
      setTypingUsers(prev => 
        prev.filter(indicator => 
          Date.now() - indicator.timestamp.getTime() < 5000 // إزالة المؤشرات الأقدم من 5 ثوان
        )
      );
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  // ===== حساب إجمالي الرسائل غير المقروءة =====
  const totalUnreadCount = conversations.reduce((total, conv) => total + conv.unread_count, 0);

  return {
    // البيانات
    conversations,
    currentConversation,
    messages,
    typingUsers,
    onlineUsers,
    isLoading,
    error,
    hasMoreMessages,
    totalUnreadCount,
    
    // الوظائف
    loadConversations,
    loadMessages,
    loadMoreMessages,
    sendMessage,
    addReaction,
    removeReaction,
    selectConversation,
    leaveCurrentConversation,
    createConversation,
    deleteConversation,
    archiveConversation,
    unarchiveConversation,
    updateMessageStatus,
    markAllAsRead,
    deleteMessage,
    editMessage,
    
    // تنظيف الأخطاء
    clearError: () => setError(null)
  };
}

// ===== Hook للكتابة =====
/**
 * Hook لإدارة مؤشر الكتابة
 */
export function useTypingIndicator(chatService: ChatService, conversationId: string | null) {
  const [isTyping, setIsTyping] = useState(false);
  const typingTimeoutRef = useRef<NodeJS.Timeout | null>(null);

  const startTyping = useCallback(() => {
    if (!conversationId) return;
    
    if (!isTyping) {
      setIsTyping(true);
    }

    // إعادة تعيين المؤقت
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    typingTimeoutRef.current = setTimeout(() => {
      setIsTyping(false);
    }, 3000);
  }, [chatService, conversationId, isTyping]);

  const stopTyping = useCallback(() => {
    if (!conversationId) return;
    
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    if (isTyping) {
      setIsTyping(false);
    }
  }, [chatService, conversationId, isTyping]);

  // تنظيف عند إلغاء التركيب
  useEffect(() => {
    return () => {
      if (typingTimeoutRef.current) {
        clearTimeout(typingTimeoutRef.current);
      }
    };
  }, []);

  return {
    isTyping,
    startTyping,
    stopTyping
  };
}

// ===== Hook للبحث =====
/**
 * Hook للبحث في المحادثات والرسائل
 */
export function useChatSearch(chatService: ChatService) {
  const [searchResults, setSearchResults] = useState<SearchResponse | null>(null);
  const [isSearching, setIsSearching] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);

  const search = useCallback(async (query: string, filters?: Partial<SearchChatsRequest>) => {
    if (!query.trim()) {
      setSearchResults(null);
      return;
    }

    try {
      setIsSearching(true);
      setSearchError(null);

      const response = await chatService.api.searchChats({
        query,
        ...filters
      });

      setSearchResults(response);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في البحث';
      setSearchError(errorMessage);
      console.error('خطأ في البحث:', err);
    } finally {
      setIsSearching(false);
    }
  }, [chatService]);

  const clearSearch = useCallback(() => {
    setSearchResults(null);
    setSearchError(null);
  }, []);

  return {
    searchResults,
    isSearching,
    searchError,
    search,
    clearSearch
  };
}

// ===== Hook للمستخدمين =====
/**
 * Hook لإدارة المستخدمين المتاحين
 */
export function useChatUsers(chatService: ChatService) {
  const [availableUsers, setAvailableUsers] = useState<ChatUser[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadAvailableUsers = useCallback(async (userType?: UserType, propertyId?: string) => {
    try {
      setIsLoading(true);
      setError(null);
      
      const users = await chatService.api.getAvailableUsers(userType, propertyId);
      setAvailableUsers(users);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تحميل المستخدمين';
      setError(errorMessage);
      console.error('خطأ في تحميل المستخدمين:', err);
    } finally {
      setIsLoading(false);
    }
  }, [chatService]);

  const updateUserStatus = useCallback(async (status: string) => {
    try {
      await chatService.api.updateUserStatus(status);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تحديث حالة المستخدم';
      setError(errorMessage);
      console.error('خطأ في تحديث حالة المستخدم:', err);
    }
  }, [chatService]);

  return {
    availableUsers,
    isLoading,
    error,
    loadAvailableUsers,
    updateUserStatus,
    clearError: () => setError(null)
  };
}

// ===== Hook للإعدادات =====
/**
 * Hook لإدارة إعدادات الشات
 */
export function useChatSettings(chatService: ChatService) {
  const [settings, setSettings] = useState<ChatSettings | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadSettings = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      
      const chatSettings = await chatService.api.getChatSettings();
      setSettings(chatSettings);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تحميل الإعدادات';
      setError(errorMessage);
      console.error('خطأ في تحميل الإعدادات:', err);
    } finally {
      setIsLoading(false);
    }
  }, [chatService]);

  const updateSettings = useCallback(async (newSettings: Partial<ChatSettings>) => {
    try {
      setIsLoading(true);
      setError(null);
      
      const updatedSettings = await chatService.api.updateChatSettings(newSettings);
      setSettings(updatedSettings);
      
      return updatedSettings;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في تحديث الإعدادات';
      setError(errorMessage);
      console.error('خطأ في تحديث الإعدادات:', err);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, [chatService]);

  return {
    settings,
    isLoading,
    error,
    loadSettings,
    updateSettings,
    clearError: () => setError(null)
  };
}

// ===== Hook لرفع الملفات =====
/**
 * Hook لإدارة رفع الملفات
 */
export function useFileUpload(chatService: ChatService) {
  const [uploadProgress, setUploadProgress] = useState<{ [key: string]: number }>({});
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const uploadFile = useCallback(async (file: File, messageType: MessageType): Promise<FileUploadResponse> => {
    // التحقق من نوع الملف
    if (!chatService.isFileTypeAllowed(file, messageType)) {
      throw new Error(`نوع الملف ${file.type} غير مسموح لهذا النوع من الرسائل`);
    }

    // التحقق من حجم الملف
    const maxSize = chatService.getMaxFileSize(messageType);
    if (file.size > maxSize) {
      throw new Error(`حجم الملف ${chatService.formatFileSize(file.size)} يتجاوز الحد المسموح ${chatService.formatFileSize(maxSize)}`);
    }

    try {
      setIsUploading(true);
      setError(null);
      
      const fileId = `${Date.now()}-${file.name}`;
      setUploadProgress(prev => ({ ...prev, [fileId]: 0 }));

      // محاكاة تقدم الرفع
      const progressInterval = setInterval(() => {
        setUploadProgress(prev => {
          const currentProgress = prev[fileId] || 0;
          const newProgress = Math.min(currentProgress + 10, 90);
          return { ...prev, [fileId]: newProgress };
        });
      }, 200);

      const response = await chatService.api.uploadFile(file, messageType);
      
      clearInterval(progressInterval);
      setUploadProgress(prev => ({ ...prev, [fileId]: 100 }));
      
      // إزالة تقدم الرفع بعد ثانيتين
      setTimeout(() => {
        setUploadProgress(prev => {
          const { [fileId]: _, ...rest } = prev;
          return rest;
        });
      }, 2000);

      return response;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطأ في رفع الملف';
      setError(errorMessage);
      console.error('خطأ في رفع الملف:', err);
      throw err;
    } finally {
      setIsUploading(false);
    }
  }, [chatService]);

  const uploadMultipleFiles = useCallback(async (files: File[], messageType: MessageType): Promise<FileUploadResponse[]> => {
    const results: FileUploadResponse[] = [];
    const errors: string[] = [];
    
    for (const file of files) {
      try {
        const result = await uploadFile(file, messageType);
        results.push(result);
      } catch (error) {
        const errorMessage = error instanceof Error ? error.message : 'خطأ في رفع الملف';
        errors.push(`${file.name}: ${errorMessage}`);
        console.error(`خطأ في رفع الملف ${file.name}:`, error);
      }
    }
    
    if (errors.length > 0) {
      setError(errors.join('\n'));
    }
    
    return results;
  }, [uploadFile]);

  return {
    uploadProgress,
    isUploading,
    error,
    uploadFile,
    uploadMultipleFiles,
    clearError: () => setError(null)
  };
}

// ===== Hook لحالة الاتصال =====
/**
 * Hook لمراقبة حالة الاتصال
 */
export function useConnectionStatus(chatService: ChatService) {
  type WSConnectionStatus = 'connected' | 'disconnected' | 'reconnecting' | 'error';

  const [isConnected, setIsConnected] = useState<boolean>(false);
  const [connectionStatus, setConnectionStatus] = useState<WSConnectionStatus>('disconnected');

  useEffect(() => {
    const handleConnected = () => {
      setIsConnected(true);
      setConnectionStatus('connected');
    };

    const handleDisconnected = () => {
      setIsConnected(false);
      setConnectionStatus('disconnected');
    };

    const handleReconnecting = () => {
      setConnectionStatus('reconnecting');
    };

    const handleError = (error: any) => {
      console.error('خطأ في الاتصال:', error);
      setConnectionStatus('error');
    };

    // اعتبار الاتصال دائماً متاح لكوننا نعتمد على FCM
    setIsConnected(true);
    setConnectionStatus('connected');

    return () => {};
  }, [chatService]);

  return {
    isConnected,
    connectionStatus
  };
}

// ===== Hook لإدارة الرسائل غير المقروءة =====
/**
 * Hook لإدارة عدد الرسائل غير المقروءة
 */
export function useUnreadMessages(chatService: ChatService) {
  const [unreadCounts, setUnreadCounts] = useState<{ [conversationId: string]: number }>({});
  const [totalUnreadCount, setTotalUnreadCount] = useState(0);

  const updateUnreadCount = useCallback((conversationId: string, count: number) => {
    setUnreadCounts(prev => {
      const newCounts = { ...prev, [conversationId]: count };
      const newTotal = Object.values(newCounts).reduce((sum, c) => sum + c, 0);
      setTotalUnreadCount(newTotal);
      return newCounts;
    });
  }, []);

  const incrementUnreadCount = useCallback((conversationId: string) => {
    setUnreadCounts(prev => {
      const currentCount = prev[conversationId] || 0;
      const newCounts = { ...prev, [conversationId]: currentCount + 1 };
      const newTotal = Object.values(newCounts).reduce((sum, c) => sum + c, 0);
      setTotalUnreadCount(newTotal);
      return newCounts;
    });
  }, []);

  const clearUnreadCount = useCallback((conversationId: string) => {
    setUnreadCounts(prev => {
      const newCounts = { ...prev, [conversationId]: 0 };
      const newTotal = Object.values(newCounts).reduce((sum, c) => sum + c, 0);
      setTotalUnreadCount(newTotal);
      return newCounts;
    });
  }, []);

  const resetAllUnreadCounts = useCallback(() => {
    setUnreadCounts({});
    setTotalUnreadCount(0);
  }, []);

  return {
    unreadCounts,
    totalUnreadCount,
    updateUnreadCount,
    incrementUnreadCount,
    clearUnreadCount,
    resetAllUnreadCounts
  };
}

// ===== Hook لإحصائيات الشات =====
/**
 * Hook لإحصائيات الشات
 */
export function useChatStatistics(chatService: ChatService) {
  const [statistics, setStatistics] = useState({
    totalConversations: 0,
    totalMessages: 0,
    totalUnreadMessages: 0,
    activeUsers: 0,
    messagesPerDay: 0,
    mostActiveConversation: null as ChatConversation | null,
    messagesByType: {} as Record<MessageType, number>
  });

  const updateStatistics = useCallback((conversations: ChatConversation[], messages: ChatMessage[]) => {
    const totalConversations = conversations.length;
    const totalMessages = messages.length;
    const totalUnreadMessages = conversations.reduce((sum, conv) => sum + conv.unread_count, 0);
    
    // حساب الرسائل حسب النوع
    const messagesByType = messages.reduce((acc, msg) => {
      acc[msg.message_type] = (acc[msg.message_type] || 0) + 1;
      return acc;
    }, {} as Record<MessageType, number>);

    // العثور على المحادثة الأكثر نشاطاً
    const mostActiveConversation = conversations.reduce((most, conv) => {
      const convMessages = messages.filter(msg => msg.conversation_id === conv.conversation_id);
      const mostMessages = messages.filter(msg => msg.conversation_id === most?.conversation_id);
      return convMessages.length > mostMessages.length ? conv : most;
    }, conversations[0] || null);

    // حساب الرسائل لكل يوم (آخر 7 أيام)
    const sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7);
    const recentMessages = messages.filter(msg => new Date(msg.created_at) >= sevenDaysAgo);
    const messagesPerDay = recentMessages.length / 7;

    setStatistics({
      totalConversations,
      totalMessages,
      totalUnreadMessages,
      activeUsers: 0, // يجب تحديثه من خلال WebSocket
      messagesPerDay: Math.round(messagesPerDay * 10) / 10,
      mostActiveConversation,
      messagesByType
    });
  }, []);

  return {
    statistics,
    updateStatistics
  };
}

// ===== Hook لإدارة المسودات =====
/**
 * Hook لإدارة مسودات الرسائل
 */
export function useMessageDrafts() {
  const [drafts, setDrafts] = useState<{ [conversationId: string]: string }>({});

  const saveDraft = useCallback((conversationId: string, content: string) => {
    setDrafts(prev => ({
      ...prev,
      [conversationId]: content
    }));
  }, []);

  const getDraft = useCallback((conversationId: string) => {
    return drafts[conversationId] || '';
  }, [drafts]);

  const clearDraft = useCallback((conversationId: string) => {
    setDrafts(prev => {
      const { [conversationId]: _, ...rest } = prev;
      return rest;
    });
  }, []);

  const clearAllDrafts = useCallback(() => {
    setDrafts({});
  }, []);

  return {
    drafts,
    saveDraft,
    getDraft,
    clearDraft,
    clearAllDrafts
  };
}

// ===== Hook لإدارة الإشعارات =====
/**
 * Hook لإدارة إشعارات الشات
 */
export function useChatNotifications(chatService: ChatService) {
  const [notificationsEnabled, setNotificationsEnabled] = useState(false);
  const [soundEnabled, setSoundEnabled] = useState(true);
  const [vibrationEnabled, setVibrationEnabled] = useState(true);

  const requestNotificationPermission = useCallback(async () => {
    if ('Notification' in window) {
      const permission = await Notification.requestPermission();
      setNotificationsEnabled(permission === 'granted');
      return permission === 'granted';
    }
    return false;
  }, []);

  const showNotification = useCallback((title: string, message: string, conversationId?: string) => {
    if (!notificationsEnabled) return;

    const notification = new Notification(title, {
      body: message,
      icon: '/icons/chat-icon.png',
      badge: '/icons/badge-icon.png',
      tag: conversationId,
      requireInteraction: true
    });

    notification.onclick = () => {
      window.focus();
      notification.close();
      // يمكن إضافة منطق للانتقال إلى المحادثة
    };

    // إغلاق الإشعار تلقائياً بعد 5 ثوان
    setTimeout(() => {
      notification.close();
    }, 5000);
  }, [notificationsEnabled]);

  const playNotificationSound = useCallback(() => {
    if (!soundEnabled) return;

    const audio = new Audio('/sounds/notification.mp3');
    audio.play().catch(console.error);
  }, [soundEnabled]);

  const triggerVibration = useCallback((pattern: number[] = [200, 100, 200]) => {
    if (!vibrationEnabled || !('vibrate' in navigator)) return;

    navigator.vibrate(pattern);
  }, [vibrationEnabled]);

  return {
    notificationsEnabled,
    soundEnabled,
    vibrationEnabled,
    setNotificationsEnabled,
    setSoundEnabled,
    setVibrationEnabled,
    requestNotificationPermission,
    showNotification,
    playNotificationSound,
    triggerVibration
  };
}

// ===== Hook مجمع للشات =====
/**
 * Hook مجمع يوفر جميع وظائف الشات
 */
export function useChatComplete(chatService: ChatService) {
  const chat = useChat(chatService);
  const search = useChatSearch(chatService);
  const users = useChatUsers(chatService);
  const settings = useChatSettings(chatService);
  const fileUpload = useFileUpload(chatService);
  const connection = useConnectionStatus(chatService);
  const unreadMessages = useUnreadMessages(chatService);
  const statistics = useChatStatistics(chatService);
  const drafts = useMessageDrafts();
  const notifications = useChatNotifications(chatService);
  const typing = useTypingIndicator(chatService, chat.currentConversation?.conversation_id || null);

  return {
    chat,
    search,
    users,
    settings,
    fileUpload,
    connection,
    unreadMessages,
    statistics,
    drafts,
    notifications,
    typing
  };
}