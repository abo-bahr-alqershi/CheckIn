// lib/features/home/domain/entities/section.dart

import 'package:equatable/equatable.dart';
import '../../../../core/enums/section_type_enum.dart';
import '../../../../core/enums/section_target_enum.dart';

class Section extends Equatable {
  final String id;
  final SectionType type;
  final int displayOrder;
  final SectionTarget target;
  final bool isActive;

  const Section({
    required this.id,
    required this.type,
    required this.displayOrder,
    required this.target,
    required this.isActive,
  });

  @override
  List<Object?> get props => [id, type, displayOrder, target, isActive];
}

class SectionItem extends Equatable {
  final String id;
  final String sectionId;
  final String? propertyId;
  final String? unitId;
  final int sortOrder;

  const SectionItem({
    required this.id,
    required this.sectionId,
    this.propertyId,
    this.unitId,
    required this.sortOrder,
  });

  @override
  List<Object?> get props => [id, sectionId, propertyId, unitId, sortOrder];
}