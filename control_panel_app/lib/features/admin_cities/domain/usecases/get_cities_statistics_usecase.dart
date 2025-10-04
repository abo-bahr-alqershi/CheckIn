import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/cities_repository.dart';

class GetCitiesStatisticsUseCase implements UseCase<Map<String, dynamic>, NoParams> {
  final CitiesRepository repository;

  GetCitiesStatisticsUseCase(this.repository);

  @override
  Future<Either<Failure, Map<String, dynamic>>> call(NoParams params) async {
    return await repository.getCitiesStatistics();
  }
}