import '../../domain/entities/payment.dart';

class MoneyModel extends Money {
  const MoneyModel({
    required super.amount,
    required super.currency,
    required super.formattedAmount,
  });

  factory MoneyModel.fromJson(Map<String, dynamic> json) {
    final amount = (json['amount'] ?? 0).toDouble();
    final currency = json['currency'] ?? 'LYD';

    return MoneyModel(
      amount: amount,
      currency: currency,
      formattedAmount:
          json['formattedAmount'] ?? '$currency ${amount.toStringAsFixed(2)}',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'amount': amount,
      'currency': currency,
      'formattedAmount': formattedAmount,
    };
  }

  factory MoneyModel.fromEntity(Money entity) {
    return MoneyModel(
      amount: entity.amount,
      currency: entity.currency,
      formattedAmount: entity.formattedAmount,
    );
  }

  factory MoneyModel.zero(String currency) {
    return MoneyModel(
      amount: 0,
      currency: currency,
      formattedAmount: '$currency 0.00',
    );
  }
}
