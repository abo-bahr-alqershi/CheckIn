// lib/core/enums/section_content_type.dart

/// ContentType aligned with backend YemenBooking.Core.Enums.ContentType
/// Values: Properties, Units, Mixed
enum SectionContentType {
  properties,
  units,
  mixed,
}

extension SectionContentTypeApi on SectionContentType {
  String get apiValue {
    switch (this) {
      case SectionContentType.properties:
        return 'Properties';
      case SectionContentType.units:
        return 'Units';
      case SectionContentType.mixed:
        return 'Mixed';
    }
  }

  static SectionContentType? tryParse(String? value) {
    if (value == null) return null;
    final v = value.trim();
    switch (v) {
      case 'Properties':
        return SectionContentType.properties;
      case 'Units':
        return SectionContentType.units;
      case 'Mixed':
        return SectionContentType.mixed;
      default:
        return null;
    }
  }
}


