// ===== أنواع المستخدمين =====
/**
 * نوع المستخدم في النظام
 * - ADMIN: الإدارة العامة
 * - PROPERTY_OWNER: مالك الفندق أو موظف الفندق
 * - CUSTOMER: العميل
 */
export const UserType = {
  ADMIN: 'admin',
  PROPERTY_OWNER: 'property_owner',
  CUSTOMER: 'customer'
} as const;

export type UserType = typeof UserType[keyof typeof UserType];

/**
 * حالة المستخدم
 * - ONLINE: متصل
 * - OFFLINE: غير متصل
 * - AWAY: بعيد
 * - BUSY: مشغول
 */
export const UserStatus = {
  ONLINE: 'online',
  OFFLINE: 'offline',
  AWAY: 'away',
  BUSY: 'busy'
} as const;

export type UserStatus = typeof UserStatus[keyof typeof UserStatus];

// ===== أنواع الرسائل =====
/**
 * نوع الرسالة
 * - TEXT: نص عادي
 * - IMAGE: صورة
 * - AUDIO: مقطع صوتي
 * - VIDEO: فيديو
 * - DOCUMENT: ملف PDF أو مستند
 * - LOCATION: موقع جغرافي
 * - REPLY: رد على رسالة
 */
export const MessageType = {
  TEXT: 'text',
  IMAGE: 'image',
  AUDIO: 'audio',
  VIDEO: 'video',
  DOCUMENT: 'document',
  LOCATION: 'location',
  REPLY: 'reply'
} as const;

export type MessageType = typeof MessageType[keyof typeof MessageType];

/**
 * حالة الرسالة
 * - SENT: مرسلة
 * - DELIVERED: تم توصيلها
 * - READ: تم قراءتها
 * - FAILED: فشل الإرسال
 */
export const MessageStatus = {
  SENT: 'sent',
  DELIVERED: 'delivered',
  READ: 'read',
  FAILED: 'failed'
} as const;

export type MessageStatus = typeof MessageStatus[keyof typeof MessageStatus];

// ===== أنواع التفاعل =====
/**
 * نوع التفاعل مع الرسالة
 * - LIKE: إعجاب
 * - LOVE: حب
 * - LAUGH: ضحك
 * - ANGRY: غضب
 * - SAD: حزن
 * - WOW: استغراب
 */
export const ReactionType = {
  LIKE: 'like',
  LOVE: 'love',
  LAUGH: 'laugh',
  ANGRY: 'angry',
  SAD: 'sad',
  WOW: 'wow'
} as const;

export type ReactionType = typeof ReactionType[keyof typeof ReactionType];

// ===== واجهات البيانات =====
/**
 * واجهة المستخدم في الشات
 */
export interface ChatUser {
  user_id: string;
  name: string;
  email: string;
  phone?: string;
  profile_image?: string;
  user_type: UserType;
  status: UserStatus;
  last_seen?: Date;
  property_id?: string; // للمالكين - معرف الفندق
  is_online: boolean;
}

/**
 * واجهة المحادثة
 */
export interface ChatConversation {
  conversation_id: string;
  participants: ChatUser[];
  conversation_type: 'direct' | 'group'; // مباشرة أو جماعية
  title?: string; // عنوان المحادثة (للمجموعات)
  description?: string;
  avatar?: string;
  created_at: Date;
  updated_at: Date;
  last_message?: ChatMessage;
  unread_count: number;
  is_archived: boolean;
  is_muted: boolean;
  property_id?: string; // معرف الفندق المرتبط بالمحادثة
}

/**
 * واجهة المرفق في الرسالة
 */
export interface MessageAttachment {
  attachment_id: string;
  file_name: string;
  file_url: string;
  file_size: number;
  file_type: string;
  mime_type: string;
  thumbnail_url?: string; // للصور والفيديوهات
  duration?: number; // للصوت والفيديو بالثواني
  width?: number; // للصور والفيديوهات
  height?: number; // للصور والفيديوهات
  caption?: string;
  uploaded_at: Date;
}

/**
 * واجهة الموقع الجغرافي
 */
export interface LocationData {
  latitude: number;
  longitude: number;
  address?: string;
  place_name?: string;
  country?: string;
  city?: string;
}

/**
 * واجهة التفاعل مع الرسالة
 */
export interface MessageReaction {
  reaction_id: string;
  user_id: string;
  user_name: string;
  user_avatar?: string;
  reaction_type: ReactionType;
  created_at: Date;
}

/**
 * واجهة الرسالة
 */
export interface ChatMessage {
  message_id: string;
  conversation_id: string;
  sender_id: string;
  sender_name: string;
  sender_avatar?: string;
  sender_type: UserType;
  message_type: MessageType;
  content?: string; // النص - اختياري للرسائل التي تحتوي على مرفقات فقط
  attachments?: MessageAttachment[]; // المرفقات
  location?: LocationData; // بيانات الموقع
  reply_to?: {
    message_id: string;
    sender_name: string;
    content_preview: string; // معاينة مختصرة للرسالة المرد عليها
    message_type: MessageType;
  };
  reactions?: MessageReaction[]; // التفاعلات
  status: MessageStatus;
  is_edited: boolean;
  edited_at?: Date;
  created_at: Date;
  updated_at: Date;
  is_forwarded: boolean;
  forwarded_from?: string; // معرف الرسالة الأصلية
  delivery_receipt?: {
    delivered_at?: Date;
    read_at?: Date;
    read_by?: string[]; // قائمة معرفات المستخدمين الذين قرأوا الرسالة
  };
}

/**
 * واجهة إشعار الكتابة
 */
export interface TypingIndicator {
  conversation_id: string;
  user_id: string;
  user_name: string;
  is_typing: boolean;
  timestamp: Date;
}

/**
 * واجهة حالة الاتصال
 */
export interface ConnectionStatus {
  user_id: string;
  is_connected: boolean;
  last_ping: Date;
  device_info?: {
    device_type: 'web' | 'mobile' | 'desktop';
    browser?: string;
    os?: string;
    app_version?: string;
  };
}

// ===== أنواع الطلبات =====
/**
 * طلب إنشاء محادثة جديدة
 */
export interface CreateConversationRequest {
  participant_ids: string[];
  conversation_type: 'direct' | 'group';
  title?: string;
  description?: string;
  property_id?: string;
}

/**
 * طلب إرسال رسالة
 */
export interface SendMessageRequest {
  conversation_id: string;
  message_type: MessageType;
  content?: string;
  attachments?: File[]; // للملفات المرفوعة
  location?: LocationData;
  reply_to_message_id?: string;
}

/**
 * طلب تحديث حالة الرسالة
 */
export interface UpdateMessageStatusRequest {
  message_id: string;
  status: MessageStatus;
  read_by?: string;
}

/**
 * طلب إضافة تفاعل
 */
export interface AddReactionRequest {
  message_id: string;
  reaction_type: ReactionType;
}

/**
 * طلب البحث في المحادثات
 */
export interface SearchChatsRequest {
  query: string;
  conversation_id?: string;
  message_type?: MessageType;
  sender_id?: string;
  date_from?: Date;
  date_to?: Date;
  page?: number;
  limit?: number;
}

/**
 * طلب الحصول على المحادثات
 */
export interface GetConversationsRequest {
  user_type?: UserType;
  property_id?: string;
  is_archived?: boolean;
  page?: number;
  limit?: number;
}

/**
 * طلب الحصول على الرسائل
 */
export interface GetMessagesRequest {
  conversation_id: string;
  page?: number;
  limit?: number;
  message_id_before?: string; // للتصفح التسلسلي
}

// ===== أنواع الاستجابات =====
/**
 * استجابة قائمة المحادثات
 */
export interface ConversationsResponse {
  conversations: ChatConversation[];
  total_count: number;
  has_more: boolean;
  next_page?: number;
}

/**
 * استجابة قائمة الرسائل
 */
export interface MessagesResponse {
  messages: ChatMessage[];
  total_count: number;
  has_more: boolean;
  next_page?: number;
}

/**
 * استجابة البحث
 */
export interface SearchResponse {
  messages: ChatMessage[];
  conversations: ChatConversation[];
  total_count: number;
  has_more: boolean;
  next_page?: number;
}

/**
 * استجابة رفع الملفات
 */
export interface FileUploadResponse {
  attachment: MessageAttachment;
  upload_url?: string; // في حالة الرفع المباشر
}

// ===== أنواع أحداث الوقت الفعلي =====
/**
 * أحداث الشات في الوقت الفعلي
 */
export const ChatEventType = {
  NEW_MESSAGE: 'new_message',
  MESSAGE_UPDATED: 'message_updated',
  MESSAGE_DELETED: 'message_deleted',
  MESSAGE_REACTION_ADDED: 'message_reaction_added',
  MESSAGE_REACTION_REMOVED: 'message_reaction_removed',
  TYPING_START: 'typing_start',
  TYPING_STOP: 'typing_stop',
  USER_ONLINE: 'user_online',
  USER_OFFLINE: 'user_offline',
  CONVERSATION_CREATED: 'conversation_created',
  CONVERSATION_UPDATED: 'conversation_updated',
  PARTICIPANT_ADDED: 'participant_added',
  PARTICIPANT_REMOVED: 'participant_removed'
} as const;

export type ChatEventType = typeof ChatEventType[keyof typeof ChatEventType];

/**
 * واجهة حدث الشات
 */
export interface ChatEvent {
  event_type: ChatEventType;
  conversation_id?: string;
  user_id?: string;
  message?: ChatMessage;
  typing_indicator?: TypingIndicator;
  connection_status?: ConnectionStatus;
  timestamp: Date;
  data?: any; // بيانات إضافية حسب نوع الحدث
}

// ===== أنواع الإعدادات =====
/**
 * إعدادات الشات للمستخدم
 */
export interface ChatSettings {
  user_id: string;
  notifications_enabled: boolean;
  sound_enabled: boolean;
  show_read_receipts: boolean;
  show_typing_indicator: boolean;
  theme: 'light' | 'dark' | 'auto';
  font_size: 'small' | 'medium' | 'large';
  auto_download_media: boolean;
  backup_messages: boolean;
  updated_at: Date;
}

// ===== أنواع الأخطاء =====
/**
 * أخطاء الشات
 */
export const ChatErrorType = {
  UNAUTHORIZED: 'unauthorized',
  CONVERSATION_NOT_FOUND: 'conversation_not_found',
  MESSAGE_NOT_FOUND: 'message_not_found',
  INVALID_PARTICIPANT: 'invalid_participant',
  FILE_TOO_LARGE: 'file_too_large',
  UNSUPPORTED_FILE_TYPE: 'unsupported_file_type',
  NETWORK_ERROR: 'network_error',
  SERVER_ERROR: 'server_error',
  PERMISSION_DENIED: 'permission_denied'
} as const;

export type ChatErrorType = typeof ChatErrorType[keyof typeof ChatErrorType];

/**
 * واجهة خطأ الشات
 */
export interface ChatError {
  error_type: ChatErrorType;
  message: string;
  details?: any;
  timestamp: Date;
}

// ===== أنواع مساعدة للمكونات =====
/**
 * خصائص مكون قائمة المحادثات
 */
export interface ConversationListProps {
  conversations: ChatConversation[];
  currentUserId: string;
  onConversationSelect: (conversation: ChatConversation) => void;
  onConversationDelete?: (conversationId: string) => void;
  onConversationArchive?: (conversationId: string) => void;
  isLoading?: boolean;
  hasMore?: boolean;
  onLoadMore?: () => void;
}

/**
 * خصائص مكون قائمة الرسائل
 */
export interface MessageListProps {
  messages: ChatMessage[];
  currentUserId: string;
  onMessageReact?: (messageId: string, reactionType: ReactionType) => void;
  onMessageReply?: (message: ChatMessage) => void;
  onMessageEdit?: (messageId: string, newContent: string) => void;
  onMessageDelete?: (messageId: string) => void;
  isLoading?: boolean;
  hasMore?: boolean;
  onLoadMore?: () => void;
}

/**
 * خصائص مكون إدخال الرسالة
 */
export interface MessageInputProps {
  onSendMessage: (message: SendMessageRequest) => void;
  onTyping?: (isTyping: boolean) => void;
  replyToMessage?: ChatMessage;
  onCancelReply?: () => void;
  disabled?: boolean;
  placeholder?: string;
  maxFileSize?: number;
  allowedFileTypes?: string[];
}