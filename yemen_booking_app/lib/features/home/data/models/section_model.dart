// lib/features/home/data/models/section_model.dart

import '../../domain/entities/section.dart' as domain;
import '../../../../core/enums/section_type_enum.dart';
import '../../../../core/enums/section_target_enum.dart';

class SectionModel {
  final String id;
  final SectionType type;
  final int displayOrder;
  final SectionTarget target;
  final bool isActive;

  const SectionModel({
    required this.id,
    required this.type,
    required this.displayOrder,
    required this.target,
    required this.isActive,
  });

  factory SectionModel.fromJson(Map<String, dynamic> json) {
    final typeStr = (json['type'] ?? json['sectionType'] ?? '').toString();
    final targetStr = (json['target'] ?? '').toString();
    final dynamic rawIsActive = json['isActive'];
    final bool parsedIsActive = () {
      if (rawIsActive == null) return true;
      if (rawIsActive is bool) return rawIsActive;
      if (rawIsActive is num) return rawIsActive != 0;
      final String s = rawIsActive.toString().trim().toLowerCase();
      return s == 'true' || s == '1' || s == 'yes' || s == 'y' || s == 'on';
    }();
    return SectionModel(
      id: json['id']?.toString() ?? '',
      type: SectionTypeExtension.tryFromString(typeStr) ?? SectionType.horizontalPropertyList,
      displayOrder: (json['displayOrder'] ?? json['order'] ?? 0) is int
          ? (json['displayOrder'] ?? json['order'] ?? 0)
          : int.tryParse((json['displayOrder'] ?? json['order'] ?? '0').toString()) ?? 0,
      target: SectionTargetBackend.tryParse(targetStr) ?? SectionTarget.properties,
      isActive: parsedIsActive,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'type': type.value,
      'displayOrder': displayOrder,
      'target': target.backendName,
      'isActive': isActive,
    };
  }

  domain.Section toEntity() => domain.Section(
        id: id,
        type: type,
        displayOrder: displayOrder,
        target: target,
        isActive: isActive,
      );
}

class SectionItemModel {
  final String id;
  final String sectionId;
  final String? propertyId;
  final String? unitId;
  final int sortOrder;

  const SectionItemModel({
    required this.id,
    required this.sectionId,
    this.propertyId,
    this.unitId,
    required this.sortOrder,
  });

  factory SectionItemModel.fromJson(Map<String, dynamic> json) {
    return SectionItemModel(
      id: json['id']?.toString() ?? '',
      sectionId: json['sectionId']?.toString() ?? '',
      propertyId: json['propertyId']?.toString(),
      unitId: json['unitId']?.toString(),
      sortOrder: json['sortOrder'] is int
          ? (json['sortOrder'] as int)
          : int.tryParse((json['sortOrder'] ?? '0').toString()) ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'sectionId': sectionId,
      'propertyId': propertyId,
      'unitId': unitId,
      'sortOrder': sortOrder,
    };
  }

  domain.SectionItem toEntity() => domain.SectionItem(
        id: id,
        sectionId: sectionId,
        propertyId: propertyId,
        unitId: unitId,
        sortOrder: sortOrder,
      );
}