import '../../domain/entities/property_type.dart' as domain;

class PropertyTypeModel {
  final String id;
  final String name;
  final String description;
  final int propertiesCount;
  final String icon;
  final List<String> defaultAmenities;
  final List<String> unitTypeIds;

  const PropertyTypeModel({
    required this.id,
    required this.name,
    required this.description,
    required this.propertiesCount,
    required this.icon,
    this.defaultAmenities = const [],
    this.unitTypeIds = const [],
  });

  factory PropertyTypeModel.fromJson(Map<String, dynamic> json) {
    return PropertyTypeModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      propertiesCount: json['propertiesCount'] ?? 0,
      icon: json['icon'] ?? "",
      defaultAmenities: (json['defaultAmenities'] as List?)?.map((e) => e.toString()).toList() ?? const [],
      unitTypeIds: (json['unitTypeIds'] as List?)?.map((e) => e.toString()).toList() ?? const [],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'propertiesCount': propertiesCount,
      'icon': icon,
      'defaultAmenities': defaultAmenities,
      'unitTypeIds': unitTypeIds,
    };
  }

  domain.PropertyType toEntity() => domain.PropertyType(
        id: id,
        name: name,
        description: description,
        propertiesCount: propertiesCount,
        defaultAmenities: defaultAmenities,
        icon: icon,
        unitTypeIds: unitTypeIds,
      );
}