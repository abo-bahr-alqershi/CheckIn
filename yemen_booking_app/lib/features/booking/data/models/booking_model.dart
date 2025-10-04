import '../../../../core/enums/booking_status.dart';
import '../../domain/entities/booking.dart';
import 'payment_model.dart';

class BookingModel extends Booking {
  const BookingModel({
    required super.id,
    required super.bookingNumber,
    required super.userId,
    required super.userName,
    required super.propertyId,
    required super.propertyName,
    super.propertyAddress,
    super.unitId,
    super.unitName,
    required super.checkInDate,
    required super.checkOutDate,
    required super.numberOfNights,
    required super.adultGuests,
    required super.childGuests,
    required super.totalGuests,
    required super.totalAmount,
    super.platformCommissionAmount,
    super.finalAmount,
    required super.currency,
    required super.status,
    required super.bookingDate,
    super.actualCheckInDate,
    super.actualCheckOutDate,
    super.specialRequests,
    super.cancellationReason,
    super.bookingSource,
    super.isWalkIn,
    super.customerRating,
    super.completionNotes,
    super.services,
    super.payments,
    super.unitImages,
    required super.contactInfo,
    super.canCancel,
    super.canReview,
    super.canModify,
  });

  factory BookingModel.fromJson(Map<String, dynamic> json) {
    return BookingModel(
      id: json['id'] ?? '',
      bookingNumber: json['bookingNumber'] ?? '',
      userId: json['userId'] ?? '',
      userName: json['userName'] ?? '',
      propertyId: json['propertyId'] ?? '',
      propertyName: json['propertyName'] ?? '',
      propertyAddress: json['propertyAddress'],
      unitId: json['unitId'],
      unitName: json['unitName'],
      checkInDate: DateTime.parse(json['checkInDate'] ?? json['checkIn'] ?? DateTime.now().toIso8601String()),
      checkOutDate: DateTime.parse(json['checkOutDate'] ?? json['checkOut'] ?? DateTime.now().toIso8601String()),
      numberOfNights: json['numberOfNights'] ?? _calculateNights(json),
      adultGuests: json['adultGuests'] ?? json['guestsCount'] ?? 1,
      childGuests: json['childGuests'] ?? 0,
      totalGuests: json['totalGuests'] ?? json['guestsCount'] ?? 1,
      totalAmount: (json['totalAmount'] ?? json['totalPrice'] ?? 0).toDouble(),
      platformCommissionAmount: json['platformCommissionAmount']?.toDouble(),
      finalAmount: json['finalAmount']?.toDouble(),
      currency: json['currency'] ?? 'YER',
      status: parseBookingStatus(json['status']),
      bookingDate: DateTime.parse(json['bookingDate'] ?? json['bookedAt'] ?? DateTime.now().toIso8601String()),
      actualCheckInDate: json['actualCheckInDate'] != null 
          ? DateTime.parse(json['actualCheckInDate']) 
          : null,
      actualCheckOutDate: json['actualCheckOutDate'] != null 
          ? DateTime.parse(json['actualCheckOutDate']) 
          : null,
      specialRequests: json['specialRequests'] ?? json['specialNotes'],
      cancellationReason: json['cancellationReason'],
      bookingSource: json['bookingSource'],
      isWalkIn: json['isWalkIn'] ?? false,
      customerRating: json['customerRating'],
      completionNotes: json['completionNotes'],
      services: (json['services'] as List?)
              ?.map((e) => BookingServiceModel.fromJson(e))
              .toList() ??
          [],
      payments: (json['payments'] as List?)
              ?.map((e) => PaymentModel.fromJson(e))
              .toList() ??
          [],
      unitImages: (json['unitImages'] as List?)?.cast<String>() ?? [],
      contactInfo: ContactInfoModel.fromJson(json['contactInfo'] ?? {}),
      canCancel: json['canCancel'] ?? false,
      canReview: json['canReview'] ?? false,
      canModify: json['canModify'] ?? false,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'bookingNumber': bookingNumber,
      'userId': userId,
      'userName': userName,
      'propertyId': propertyId,
      'propertyName': propertyName,
      'propertyAddress': propertyAddress,
      'unitId': unitId,
      'unitName': unitName,
      'checkInDate': checkInDate.toIso8601String(),
      'checkOutDate': checkOutDate.toIso8601String(),
      'numberOfNights': numberOfNights,
      'adultGuests': adultGuests,
      'childGuests': childGuests,
      'totalGuests': totalGuests,
      'totalAmount': totalAmount,
      'platformCommissionAmount': platformCommissionAmount,
      'finalAmount': finalAmount,
      'currency': currency,
      'status': status.toString().split('.').last,
      'bookingDate': bookingDate.toIso8601String(),
      'actualCheckInDate': actualCheckInDate?.toIso8601String(),
      'actualCheckOutDate': actualCheckOutDate?.toIso8601String(),
      'specialRequests': specialRequests,
      'cancellationReason': cancellationReason,
      'bookingSource': bookingSource,
      'isWalkIn': isWalkIn,
      'customerRating': customerRating,
      'completionNotes': completionNotes,
      'services': services.map((e) => (e as BookingServiceModel).toJson()).toList(),
      'payments': payments.map((e) => (e as PaymentModel).toJson()).toList(),
      'unitImages': unitImages,
      'contactInfo': (contactInfo as ContactInfoModel).toJson(),
      'canCancel': canCancel,
      'canReview': canReview,
      'canModify': canModify,
    };
  }

  static int _calculateNights(Map<String, dynamic> json) {
    try {
      final checkIn = DateTime.parse(json['checkInDate'] ?? json['checkIn']);
      final checkOut = DateTime.parse(json['checkOutDate'] ?? json['checkOut']);
      return checkOut.difference(checkIn).inDays;
    } catch (e) {
      return 1;
    }
  }

  static BookingStatus parseBookingStatus(dynamic status) {
    if (status == null) return BookingStatus.pending;
    
    final statusStr = status.toString().toLowerCase();
    switch (statusStr) {
      case 'confirmed':
        return BookingStatus.confirmed;
      case 'pending':
        return BookingStatus.pending;
      case 'cancelled':
        return BookingStatus.cancelled;
      case 'completed':
        return BookingStatus.completed;
      case 'checkedin':
      case 'checked_in':
        return BookingStatus.checkedIn;
      default:
        return BookingStatus.pending;
    }
  }
}

class BookingServiceModel extends BookingService {
  const BookingServiceModel({
    required super.id,
    required super.serviceId,
    required super.serviceName,
    required super.quantity,
    required super.unitPrice,
    required super.totalPrice,
    required super.currency,
  });

  factory BookingServiceModel.fromJson(Map<String, dynamic> json) {
    return BookingServiceModel(
      id: json['id'] ?? '',
      serviceId: json['serviceId'] ?? '',
      serviceName: json['serviceName'] ?? '',
      quantity: json['quantity'] ?? 1,
      unitPrice: (json['unitPrice'] ?? 0).toDouble(),
      totalPrice: (json['totalPrice'] ?? 0).toDouble(),
      currency: json['currency'] ?? 'YER',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'serviceId': serviceId,
      'serviceName': serviceName,
      'quantity': quantity,
      'unitPrice': unitPrice,
      'totalPrice': totalPrice,
      'currency': currency,
    };
  }
}

class ContactInfoModel extends ContactInfo {
  const ContactInfoModel({
    required super.phoneNumber,
    required super.email,
    super.alternativePhone,
  });

  factory ContactInfoModel.fromJson(Map<String, dynamic> json) {
    return ContactInfoModel(
      phoneNumber: json['phoneNumber'] ?? '',
      email: json['email'] ?? '',
      alternativePhone: json['alternativePhone'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'phoneNumber': phoneNumber,
      'email': email,
      'alternativePhone': alternativePhone,
    };
  }
}