// lib/features/admin_reviews/presentation/bloc/reviews_list/reviews_list_bloc.dart

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../../domain/entities/review.dart';
import '../../../domain/usecases/get_all_reviews_usecase.dart';
import '../../../domain/usecases/approve_review_usecase.dart';
import '../../../domain/usecases/delete_review_usecase.dart';

part 'reviews_list_event.dart';
part 'reviews_list_state.dart';

class ReviewsListBloc extends Bloc<ReviewsListEvent, ReviewsListState> {
  final GetAllReviewsUseCase getAllReviews;
  final ApproveReviewUseCase approveReview;
  final DeleteReviewUseCase deleteReview;
  
  List<Review> _allReviews = [];
  
  ReviewsListBloc({
    required this.getAllReviews,
    required this.approveReview,
    required this.deleteReview,
  }) : super(ReviewsListInitial()) {
    on<LoadReviewsEvent>(_onLoadReviews);
    on<FilterReviewsEvent>(_onFilterReviews);
    on<ApproveReviewEvent>(_onApproveReview);
    on<DeleteReviewEvent>(_onDeleteReview);
    on<RefreshReviewsEvent>(_onRefreshReviews);
  }
  
  Future<void> _onLoadReviews(
    LoadReviewsEvent event,
    Emitter<ReviewsListState> emit,
  ) async {
    emit(ReviewsListLoading());
    
    final result = await getAllReviews(GetAllReviewsParams(
      status: event.status,
      minRating: event.minRating,
      maxRating: event.maxRating,
      hasImages: event.hasImages,
      propertyId: event.propertyId,
      userId: event.userId,
      startDate: event.startDate,
      endDate: event.endDate,
      pageNumber: event.pageNumber,
      pageSize: event.pageSize,
    ));
    
    result.fold(
      (failure) => emit(ReviewsListError(failure.message)),
      (reviews) {
        _allReviews = reviews;
        emit(ReviewsListLoaded(
          reviews: reviews,
          filteredReviews: reviews,
          pendingCount: reviews.where((r) => r.isPending).length,
          averageRating: _calculateAverageRating(reviews),
          approvingReviewIds: const <String>{},
        ));
      },
    );
  }
  
  Future<void> _onFilterReviews(
    FilterReviewsEvent event,
    Emitter<ReviewsListState> emit,
  ) async {
    if (state is ReviewsListLoaded) {
      final currentState = state as ReviewsListLoaded;
      
      var filtered = _allReviews;
      
      // Apply filters
      if (event.searchQuery.isNotEmpty) {
        filtered = filtered.where((review) =>
          review.userName.toLowerCase().contains(event.searchQuery.toLowerCase()) ||
          review.propertyName.toLowerCase().contains(event.searchQuery.toLowerCase()) ||
          review.comment.toLowerCase().contains(event.searchQuery.toLowerCase())
        ).toList();
      }
      
      if (event.minRating != null) {
        filtered = filtered.where((r) => r.averageRating >= event.minRating!).toList();
      }
      
      if (event.isPending != null) {
        filtered = filtered.where((r) => r.isPending == event.isPending).toList();
      }
      
      if (event.hasResponse != null) {
        filtered = filtered.where((r) => r.hasResponse == event.hasResponse).toList();
      }
      
      emit(currentState.copyWith(filteredReviews: filtered));
    }
  }
  
  Future<void> _onApproveReview(
    ApproveReviewEvent event,
    Emitter<ReviewsListState> emit,
  ) async {
    if (state is ReviewsListLoaded) {
      final currentState = state as ReviewsListLoaded;
      // Mark this review as approving
      final Set<String> newApproving = Set<String>.from(currentState.approvingReviewIds)
        ..add(event.reviewId);
      emit(currentState.copyWith(approvingReviewIds: newApproving));

      final result = await approveReview(event.reviewId);
      
      result.fold(
        (failure) {
          // Remove from approving on failure and surface error
          final Set<String> cleaned = Set<String>.from(newApproving)..remove(event.reviewId);
          emit(currentState.copyWith(approvingReviewIds: cleaned));
          emit(ReviewsListError(failure.message));
        },
        (_) {
          final updatedReviews = currentState.reviews.map((review) {
            if (review.id == event.reviewId) {
              return Review(
                id: review.id,
                bookingId: review.bookingId,
                propertyName: review.propertyName,
                userName: review.userName,
                cleanliness: review.cleanliness,
                service: review.service,
                location: review.location,
                value: review.value,
                comment: review.comment,
                createdAt: review.createdAt,
                images: review.images,
                isApproved: true,
                isPending: false,
                responseText: review.responseText,
                responseDate: review.responseDate,
                respondedBy: review.respondedBy,
              );
            }
            return review;
          }).toList();
          
          _allReviews = updatedReviews;
          final Set<String> cleaned = Set<String>.from(newApproving)..remove(event.reviewId);
          emit(currentState.copyWith(
            reviews: updatedReviews,
            filteredReviews: updatedReviews,
            pendingCount: updatedReviews.where((r) => r.isPending).length,
            approvingReviewIds: cleaned,
          ));
        },
      );
    }
  }
  
  Future<void> _onDeleteReview(
    DeleteReviewEvent event,
    Emitter<ReviewsListState> emit,
  ) async {
    if (state is ReviewsListLoaded) {
      final currentState = state as ReviewsListLoaded;
      
      final result = await deleteReview(event.reviewId);
      
      result.fold(
        (failure) => emit(ReviewsListError(failure.message)),
        (_) {
          final updatedReviews = currentState.reviews
              .where((r) => r.id != event.reviewId)
              .toList();
          
          _allReviews = updatedReviews;
          emit(currentState.copyWith(
            reviews: updatedReviews,
            filteredReviews: updatedReviews,
            pendingCount: updatedReviews.where((r) => r.isPending).length,
            averageRating: _calculateAverageRating(updatedReviews),
          ));
        },
      );
    }
  }
  
  Future<void> _onRefreshReviews(
    RefreshReviewsEvent event,
    Emitter<ReviewsListState> emit,
  ) async {
    add(LoadReviewsEvent());
  }
  
  double _calculateAverageRating(List<Review> reviews) {
    if (reviews.isEmpty) return 0;
    final total = reviews.fold<double>(
      0,
      (sum, review) => sum + review.averageRating,
    );
    return total / reviews.length;
  }
}