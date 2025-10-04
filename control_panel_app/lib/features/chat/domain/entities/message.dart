import 'package:equatable/equatable.dart';
import 'attachment.dart';

class Message extends Equatable {
  final String id;
  final String conversationId;
  final String senderId;
  final String messageType; // "text", "image", "audio", "video", "document", "location"
  final String? content;
  final Location? location;
  final String? replyToMessageId;
  final List<MessageReaction> reactions;
  final List<Attachment> attachments;
  final DateTime createdAt;
  final DateTime updatedAt;
  final String status; // "sent", "delivered", "read", "failed"
  final bool isEdited;
  final DateTime? editedAt;
  final DeliveryReceipt? deliveryReceipt;

  const Message({
    required this.id,
    required this.conversationId,
    required this.senderId,
    required this.messageType,
    this.content,
    this.location,
    this.replyToMessageId,
    this.reactions = const [],
    this.attachments = const [],
    required this.createdAt,
    required this.updatedAt,
    required this.status,
    this.isEdited = false,
    this.editedAt,
    this.deliveryReceipt,
  });

  @override
  List<Object?> get props => [
    id,
    conversationId,
    senderId,
    messageType,
    content,
    location,
    replyToMessageId,
    reactions,
    attachments,
    createdAt,
    updatedAt,
    status,
    isEdited,
    editedAt,
    deliveryReceipt,
  ];

  // Helper methods
  bool get isTextMessage => messageType == 'text';
  bool get isMediaMessage => ['image', 'video', 'audio'].contains(messageType);
  bool get hasAttachments => attachments.isNotEmpty;
  bool get isDelivered => status == 'delivered' || status == 'read';
  bool get isRead => status == 'read';
  bool get isFailed => status == 'failed';
}

class Location extends Equatable {
  final double latitude;
  final double longitude;
  final String? address;

  const Location({
    required this.latitude,
    required this.longitude,
    this.address,
  });

  @override
  List<Object?> get props => [latitude, longitude, address];
}

class MessageReaction extends Equatable {
  final String id;
  final String messageId;
  final String userId;
  final String reactionType; // "like", "love", "laugh", "sad", "angry", "wow"

  const MessageReaction({
    required this.id,
    required this.messageId,
    required this.userId,
    required this.reactionType,
  });

  @override
  List<Object?> get props => [id, messageId, userId, reactionType];
}

class DeliveryReceipt extends Equatable {
  final DateTime? deliveredAt;
  final DateTime? readAt;
  final List<String> readBy;

  const DeliveryReceipt({
    this.deliveredAt,
    this.readAt,
    this.readBy = const [],
  });

  @override
  List<Object?> get props => [deliveredAt, readAt, readBy];
}
extension LocationExtension on Location {
  Map<String, dynamic> toJson() {
    return {
      'latitude': latitude,
      'longitude': longitude,
      if (address != null) 'address': address,
    };
  }
}