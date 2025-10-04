import 'package:equatable/equatable.dart';

class User extends Equatable {
  final String id;
  final String name;
  final String role;
  final String email;
  final String phone;
  final String? profileImage;
  final DateTime createdAt;
  final bool isActive;
  final Map<String, dynamic>? settings;
  final List<String>? favorites;

  const User({
    required this.id,
    required this.name,
    required this.role,
    required this.email,
    required this.phone,
    this.profileImage,
    required this.createdAt,
    required this.isActive,
    this.settings,
    this.favorites,
  });

  User copyWith({
    String? id,
    String? name,
    String? role,
    String? email,
    String? phone,
    String? profileImage,
    DateTime? createdAt,
    bool? isActive,
    Map<String, dynamic>? settings,
    List<String>? favorites,
  }) {
    return User(
      id: id ?? this.id,
      name: name ?? this.name,
      role: role ?? this.role,
      email: email ?? this.email,
      phone: phone ?? this.phone,
      profileImage: profileImage ?? this.profileImage,
      createdAt: createdAt ?? this.createdAt,
      isActive: isActive ?? this.isActive,
      settings: settings ?? this.settings,
      favorites: favorites ?? this.favorites,
    );
  }

  @override
  List<Object?> get props => [
        id,
        name,
        role,
        email,
        phone,
        profileImage,
        createdAt,
        isActive,
        settings,
        favorites,
      ];
}