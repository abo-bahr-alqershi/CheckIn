import 'package:bookn_cp_app/features/admin_payments/data/models/money_model.dart';

import '../../../../../core/enums/payment_method_enum.dart';
import '../../domain/entities/payment.dart';

class PaymentModel extends Payment {
  const PaymentModel({
    required super.id,
    required super.bookingId,
    required super.amount,
    required super.transactionId,
    required super.method,
    required super.status,
    required super.paymentDate,
    super.userId,
    super.userName,
    super.userEmail,
    super.unitId,
    super.unitName,
    super.propertyId,
    super.propertyName,
    super.description,
    super.notes,
    super.receiptUrl,
    super.invoiceNumber,
    super.metadata,
    super.isRefundable,
    super.refundDeadline,
    super.refundedAmount,
    super.refundedAt,
    super.refundReason,
    super.refundTransactionId,
    super.isVoided,
    super.voidedAt,
    super.voidReason,
  });

  factory PaymentModel.fromJson(Map<String, dynamic> json) {
    return PaymentModel(
      id: json['id']?.toString() ?? '',
      bookingId: json['bookingId']?.toString() ?? '',
      amount: MoneyModel.fromJson(json['amount']),
      transactionId: json['transactionId'] ?? '',
      method: _parsePaymentMethod(json['method']),
      status: _parsePaymentStatus(json['status']),
      paymentDate: DateTime.parse(json['paymentDate']),
      userId: json['userId']?.toString(),
      userName: json['userName'],
      userEmail: json['userEmail'],
      unitId: json['unitId']?.toString(),
      unitName: json['unitName'],
      propertyId: json['propertyId']?.toString(),
      propertyName: json['propertyName'],
      description: json['description'],
      notes: json['notes'],
      receiptUrl: json['receiptUrl'],
      invoiceNumber: json['invoiceNumber'],
      metadata: json['metadata'] as Map<String, dynamic>?,
      isRefundable: json['isRefundable'],
      refundDeadline: json['refundDeadline'] != null
          ? DateTime.parse(json['refundDeadline'])
          : null,
      refundedAmount: (json['refundedAmount'] ?? 0).toDouble(),
      refundedAt: json['refundedAt'] != null
          ? DateTime.parse(json['refundedAt'])
          : null,
      refundReason: json['refundReason'],
      refundTransactionId: json['refundTransactionId'],
      isVoided: json['isVoided'],
      voidedAt:
          json['voidedAt'] != null ? DateTime.parse(json['voidedAt']) : null,
      voidReason: json['voidReason'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'bookingId': bookingId,
      'amount': (amount as MoneyModel).toJson(),
      'transactionId': transactionId,
      'method': method.backendValue,
      'status': status.backendKey,
      'paymentDate': paymentDate.toIso8601String(),
      if (userId != null) 'userId': userId,
      if (userName != null) 'userName': userName,
      if (userEmail != null) 'userEmail': userEmail,
      if (unitId != null) 'unitId': unitId,
      if (unitName != null) 'unitName': unitName,
      if (propertyId != null) 'propertyId': propertyId,
      if (propertyName != null) 'propertyName': propertyName,
      if (description != null) 'description': description,
      if (notes != null) 'notes': notes,
      if (receiptUrl != null) 'receiptUrl': receiptUrl,
      if (invoiceNumber != null) 'invoiceNumber': invoiceNumber,
      if (metadata != null) 'metadata': metadata,
      if (isRefundable != null) 'isRefundable': isRefundable,
      if (refundDeadline != null)
        'refundDeadline': refundDeadline!.toIso8601String(),
      if (refundedAmount != null) 'refundedAmount': refundedAmount,
      if (refundedAt != null) 'refundedAt': refundedAt!.toIso8601String(),
      if (refundReason != null) 'refundReason': refundReason,
      if (refundTransactionId != null)
        'refundTransactionId': refundTransactionId,
      if (isVoided != null) 'isVoided': isVoided,
      if (voidedAt != null) 'voidedAt': voidedAt!.toIso8601String(),
      if (voidReason != null) 'voidReason': voidReason,
    };
  }

  static PaymentMethod _parsePaymentMethod(dynamic method) {
    if (method == null) return PaymentMethod.cash;

    if (method is int) {
      return PaymentMethodExtension.fromBackendValue(method);
    }

    if (method is String) {
      return PaymentMethodExtension.fromString(method);
    }

    return PaymentMethod.cash;
  }

  static PaymentStatus _parsePaymentStatus(dynamic status) {
    if (status == null) return PaymentStatus.pending;

    final statusStr = status.toString().toLowerCase();
    switch (statusStr) {
      case 'successful':
      case 'success':
        return PaymentStatus.successful;
      case 'failed':
      case 'failure':
        return PaymentStatus.failed;
      case 'pending':
        return PaymentStatus.pending;
      case 'refunded':
        return PaymentStatus.refunded;
      case 'voided':
        return PaymentStatus.voided;
      case 'partiallyrefunded':
      case 'partially_refunded':
        return PaymentStatus.partiallyRefunded;
      default:
        return PaymentStatus.pending;
    }
  }

  factory PaymentModel.fromEntity(Payment entity) {
    return PaymentModel(
      id: entity.id,
      bookingId: entity.bookingId,
      amount: entity.amount,
      transactionId: entity.transactionId,
      method: entity.method,
      status: entity.status,
      paymentDate: entity.paymentDate,
      userId: entity.userId,
      userName: entity.userName,
      userEmail: entity.userEmail,
      unitId: entity.unitId,
      unitName: entity.unitName,
      propertyId: entity.propertyId,
      propertyName: entity.propertyName,
      description: entity.description,
      notes: entity.notes,
      receiptUrl: entity.receiptUrl,
      invoiceNumber: entity.invoiceNumber,
      metadata: entity.metadata,
      isRefundable: entity.isRefundable,
      refundDeadline: entity.refundDeadline,
      refundedAmount: entity.refundedAmount,
      refundedAt: entity.refundedAt,
      refundReason: entity.refundReason,
      refundTransactionId: entity.refundTransactionId,
      isVoided: entity.isVoided,
      voidedAt: entity.voidedAt,
      voidReason: entity.voidReason,
    );
  }
}
