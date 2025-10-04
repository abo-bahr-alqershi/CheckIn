// lib/features/admin_availability_pricing/data/models/pricing_rule_model.dart

import '../../domain/entities/pricing_rule.dart';
import '../../domain/entities/pricing.dart';
import 'package:intl/intl.dart';

class PricingRuleModel extends PricingRule {
  const PricingRuleModel({
    required super.id,
    required super.unitId,
    required super.startDate,
    required super.endDate,
    super.startTime,
    super.endTime,
    required super.priceAmount,
    required super.priceType,
    required super.pricingTier,
    super.percentageChange,
    super.minPrice,
    super.maxPrice,
    super.description,
    required super.currency,
  });

  factory PricingRuleModel.fromJson(Map<String, dynamic> json) {
    return PricingRuleModel(
      id: json['id'] as String,
      unitId: json['unitId'] as String,
      startDate: DateTime.parse(json['startDate'] as String),
      endDate: DateTime.parse(json['endDate'] as String),
      startTime: json['startTime'] as String?,
      endTime: json['endTime'] as String?,
      // Backend may send either "priceAmount" or "price"
      priceAmount: ((json['priceAmount'] ?? json['price']) as num).toDouble(),
      priceType: json['priceType'] as String,
      // Backend may send either "pricingTier" or compact "tier"; default to normal
      pricingTier: (json['pricingTier'] ?? json['tier'] ?? 'normal') as String,
      percentageChange: json['percentageChange'] != null
          ? (json['percentageChange'] as num).toDouble()
          : null,
      minPrice: json['minPrice'] != null
          ? (json['minPrice'] as num).toDouble()
          : null,
      maxPrice: json['maxPrice'] != null
          ? (json['maxPrice'] as num).toDouble()
          : null,
      description: json['description'] as String?,
      // If rule currency is missing, expect caller to inject parent currency; else fallback to empty string
      currency: (json['currency'] as String?) ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'unitId': unitId,
      'startDate': startDate.toIso8601String(),
      'endDate': endDate.toIso8601String(),
      if (startTime != null) 'startTime': startTime,
      if (endTime != null) 'endTime': endTime,
      'priceAmount': priceAmount,
      'priceType': priceType,
      'pricingTier': pricingTier,
      if (percentageChange != null) 'percentageChange': percentageChange,
      if (minPrice != null) 'minPrice': minPrice,
      if (maxPrice != null) 'maxPrice': maxPrice,
      if (description != null) 'description': description,
      'currency': currency,
    };
  }

  static PriceType _parsePriceType(String type) {
    switch (type.toLowerCase()) {
      case 'base':
        return PriceType.base;
      case 'weekend':
        return PriceType.weekend;
      case 'seasonal':
        return PriceType.seasonal;
      case 'holiday':
        return PriceType.holiday;
      case 'special_event':
      case 'specialevent':
        return PriceType.specialEvent;
      default:
        return PriceType.custom;
    }
  }
}

class UnitPricingModel extends UnitPricing {
  const UnitPricingModel({
    required super.unitId,
    required super.unitName,
    required super.basePrice,
    required super.currency,
    required super.calendar,
    required super.rules,
    required super.stats,
  });

  factory UnitPricingModel.fromJson(Map<String, dynamic> json) {
    final Map<String, PricingDay> calendar = {};
    if (json['calendar'] != null) {
      final dateFmt = DateFormat('yyyy-MM-dd');
      (json['calendar'] as Map<String, dynamic>).forEach((key, value) {
        String normalizedKey = key;
        try {
          final dt = DateTime.parse(key);
          normalizedKey = dateFmt.format(DateTime(dt.year, dt.month, dt.day));
        } catch (_) {
          if (key.length >= 10) normalizedKey = key.substring(0, 10);
        }
        calendar[normalizedKey] = PricingDayModel.fromJson(value);
      });
    }

    final List<PricingRule> rules = [];
    if (json['rules'] != null) {
      final String unitCurrency = json['currency'] as String;
      final String unitId = json['unitId'] as String;
      rules.addAll(
        (json['rules'] as List).map((e) {
          final map = Map<String, dynamic>.from(e as Map);
          // Ensure unitId and currency are present on each rule to satisfy domain entity
          map.putIfAbsent('unitId', () => unitId);
          map.putIfAbsent('currency', () => unitCurrency);
          // Normalize possible backend alias for pricing tier
          if (!map.containsKey('pricingTier') && map.containsKey('tier')) {
            map['pricingTier'] = map['tier'];
          }
          // Normalize price key if backend used "price"
          if (!map.containsKey('priceAmount') && map.containsKey('price')) {
            map['priceAmount'] = map['price'];
          }
          return PricingRuleModel.fromJson(map);
        }).toList(),
      );
    }

    return UnitPricingModel(
      unitId: json['unitId'] as String,
      unitName: json['unitName'] as String,
      basePrice: (json['basePrice'] as num).toDouble(),
      currency: json['currency'] as String,
      calendar: calendar,
      rules: rules,
      stats: PricingStatsModel.fromJson(json['stats']),
    );
  }
}

class PricingDayModel extends PricingDay {
  const PricingDayModel({
    required super.price,
    required super.priceType,
    required super.colorCode,
    super.percentageChange,
    super.pricingTier,
  });

  factory PricingDayModel.fromJson(Map<String, dynamic> json) {
    return PricingDayModel(
      price: (json['price'] as num).toDouble(),
      priceType: PricingRuleModel._parsePriceType(json['priceType'] as String),
      colorCode: json['colorCode'] as String,
      percentageChange: json['percentageChange'] != null
          ? (json['percentageChange'] as num).toDouble()
          : null,
      pricingTier: json['pricingTier'] as String?,
    );
  }
}

class PricingStatsModel extends PricingStats {
  const PricingStatsModel({
    required super.averagePrice,
    required super.minPrice,
    required super.maxPrice,
    required super.daysWithSpecialPricing,
    required super.potentialRevenue,
  });

  factory PricingStatsModel.fromJson(Map<String, dynamic> json) {
    return PricingStatsModel(
      averagePrice: (json['averagePrice'] as num).toDouble(),
      minPrice: (json['minPrice'] as num).toDouble(),
      maxPrice: (json['maxPrice'] as num).toDouble(),
      daysWithSpecialPricing: json['daysWithSpecialPricing'] as int,
      potentialRevenue: (json['potentialRevenue'] as num).toDouble(),
    );
  }
}
