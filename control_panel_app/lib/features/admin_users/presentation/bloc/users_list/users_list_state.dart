part of 'users_list_bloc.dart';

abstract class UsersListState extends Equatable {
  const UsersListState();

  @override
  List<Object?> get props => [];
}

class UsersListInitial extends UsersListState {}

class UsersListLoading extends UsersListState {}

class UsersListLoaded extends UsersListState {
  final List<User> users;
  final bool hasMore;
  final int totalCount;
  final bool isLoadingMore;

  const UsersListLoaded({
    required this.users,
    required this.hasMore,
    required this.totalCount,
    this.isLoadingMore = false,
  });

  UsersListLoaded copyWith({
    List<User>? users,
    bool? hasMore,
    int? totalCount,
    bool? isLoadingMore,
  }) {
    return UsersListLoaded(
      users: users ?? this.users,
      hasMore: hasMore ?? this.hasMore,
      totalCount: totalCount ?? this.totalCount,
      isLoadingMore: isLoadingMore ?? this.isLoadingMore,
    );
  }

  @override
  List<Object> get props => [users, hasMore, totalCount, isLoadingMore];
}

class UsersListError extends UsersListState {
  final String message;

  const UsersListError({required this.message});

  @override
  List<Object> get props => [message];
}

class UserOperationSuccess extends UsersListState {
  final String message;
  final List<User> users;
  final bool hasMore;
  final int totalCount;

  const UserOperationSuccess({
    required this.message,
    required this.users,
    required this.hasMore,
    required this.totalCount,
  });

  @override
  List<Object> get props => [message, users, hasMore, totalCount];
}
