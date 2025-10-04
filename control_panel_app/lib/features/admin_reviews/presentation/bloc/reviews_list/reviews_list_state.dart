// lib/features/admin_reviews/presentation/bloc/reviews_list/reviews_list_state.dart

part of 'reviews_list_bloc.dart';

abstract class ReviewsListState extends Equatable {
  const ReviewsListState();
  
  @override
  List<Object> get props => [];
}

class ReviewsListInitial extends ReviewsListState {}

class ReviewsListLoading extends ReviewsListState {}

class ReviewsListLoaded extends ReviewsListState {
  final List<Review> reviews;
  final List<Review> filteredReviews;
  final int pendingCount;
  final double averageRating;
  final Set<String> approvingReviewIds;
  
  const ReviewsListLoaded({
    required this.reviews,
    required this.filteredReviews,
    required this.pendingCount,
    required this.averageRating,
    this.approvingReviewIds = const <String>{},
  });
  
  ReviewsListLoaded copyWith({
    List<Review>? reviews,
    List<Review>? filteredReviews,
    int? pendingCount,
    double? averageRating,
    Set<String>? approvingReviewIds,
  }) {
    return ReviewsListLoaded(
      reviews: reviews ?? this.reviews,
      filteredReviews: filteredReviews ?? this.filteredReviews,
      pendingCount: pendingCount ?? this.pendingCount,
      averageRating: averageRating ?? this.averageRating,
      approvingReviewIds: approvingReviewIds ?? this.approvingReviewIds,
    );
  }
  
  @override
  List<Object> get props => [reviews, filteredReviews, pendingCount, averageRating, approvingReviewIds];
}

class ReviewsListError extends ReviewsListState {
  final String message;
  
  const ReviewsListError(this.message);
  
  @override
  List<Object> get props => [message];
}