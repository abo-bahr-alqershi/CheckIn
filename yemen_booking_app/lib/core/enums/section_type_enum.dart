// lib/core/enums/section_type_enum.dart

enum SectionType {
  singlePropertyAd,
  multiPropertyAd,
  unitShowcaseAd,
  singlePropertyOffer,
  limitedTimeOffer,
  seasonalOffer,
  multiPropertyOffersGrid,
  offersCarousel,
  flashDeals,
  horizontalPropertyList,
  verticalPropertyGrid,
  mixedLayoutList,
  compactPropertyList,
  cityCardsGrid,
  destinationCarousel,
  exploreCities,
  premiumCarousel,
  interactiveShowcase,
}

extension SectionTypeExtension on SectionType {
  String get value {
    switch (this) {
      case SectionType.singlePropertyAd:
        return 'singlePropertyAd';
      case SectionType.multiPropertyAd:
        return 'multiPropertyAd';
      case SectionType.unitShowcaseAd:
        return 'unitShowcaseAd';
      case SectionType.singlePropertyOffer:
        return 'singlePropertyOffer';
      case SectionType.limitedTimeOffer:
        return 'limitedTimeOffer';
      case SectionType.seasonalOffer:
        return 'seasonalOffer';
      case SectionType.multiPropertyOffersGrid:
        return 'multiPropertyOffersGrid';
      case SectionType.offersCarousel:
        return 'offersCarousel';
      case SectionType.flashDeals:
        return 'flashDeals';
      case SectionType.horizontalPropertyList:
        return 'horizontalPropertyList';
      case SectionType.verticalPropertyGrid:
        return 'verticalPropertyGrid';
      case SectionType.mixedLayoutList:
        return 'mixedLayoutList';
      case SectionType.compactPropertyList:
        return 'compactPropertyList';
      case SectionType.cityCardsGrid:
        return 'cityCardsGrid';
      case SectionType.destinationCarousel:
        return 'destinationCarousel';
      case SectionType.exploreCities:
        return 'exploreCities';
      case SectionType.premiumCarousel:
        return 'premiumCarousel';
      case SectionType.interactiveShowcase:
        return 'interactiveShowcase';
    }
  }

  static SectionType? tryFromString(String value) {
    for (final type in SectionType.values) {
      if (type.value == value) return type;
    }
    return null;
  }
}