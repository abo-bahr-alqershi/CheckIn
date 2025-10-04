import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/models/paginated_result.dart' as core;

import '../entities/property_type.dart' as domain;
import '../entities/unit_type.dart' as domain;
import '../entities/section.dart' as section_domain;
import '../../../search/data/models/search_result_model.dart';

abstract class HomeRepository {
  Future<Either<Failure, void>> recordSectionImpression({required String sectionId});
  Future<Either<Failure, void>> recordSectionInteraction({required String sectionId, required String interactionType, String? itemId, Map<String, dynamic>? metadata});

  // Sections
  Future<Either<Failure, core.PaginatedResult<section_domain.Section>>> getSections({int pageNumber, int pageSize, String? target, String? type, bool forceRefresh});
  Future<Either<Failure, core.PaginatedResult<SearchResultModel>>> getSectionPropertyItems({required String sectionId, int pageNumber, int pageSize, bool forceRefresh});

  // Stage 2 additions
  Future<Either<Failure, List<domain.PropertyType>>> getPropertyTypes();
  Future<Either<Failure, List<domain.UnitType>>> getUnitTypes({required String propertyTypeId});
}
