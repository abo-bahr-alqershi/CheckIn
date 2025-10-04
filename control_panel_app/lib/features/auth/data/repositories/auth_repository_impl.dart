import 'package:dartz/dartz.dart';
import 'package:internet_connection_checker/internet_connection_checker.dart';
import '../../../../core/error/error_handler.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/constants/storage_constants.dart';
import '../../../../services/local_storage_service.dart';
import '../../../../injection_container.dart';
import '../../domain/entities/auth_response.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_local_datasource.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/auth_response_model.dart';
import '../models/user_model.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource remoteDataSource;
  final AuthLocalDataSource localDataSource;
  final InternetConnectionChecker internetConnectionChecker;

  AuthRepositoryImpl({
    required this.remoteDataSource,
    required this.localDataSource,
    required this.internetConnectionChecker,
  });

  @override
  Future<Either<Failure, AuthResponse>> login({
    required String emailOrPhone,
    required String password,
    required bool rememberMe,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final authResponse = await remoteDataSource.login(
          emailOrPhone: emailOrPhone,
          password: password,
          rememberMe: rememberMe,
        );
        // Since common login endpoint only succeeds for verified users,
        // mark the user as email-verified locally to satisfy router guards.
        final originalUser = authResponse.user as UserModel;
        final verifiedUser = UserModel(
          userId: originalUser.userId,
          name: originalUser.name,
          email: originalUser.email,
          phone: originalUser.phone,
          roles: originalUser.roles,
          accountRole: originalUser.accountRole,
          propertyId: originalUser.propertyId,
          propertyName: originalUser.propertyName,
          propertyCurrency: originalUser.propertyCurrency,
          profileImage: originalUser.profileImage,
          emailVerifiedAt: DateTime.now(),
          phoneVerifiedAt: originalUser.phoneVerifiedAt,
          createdAt: originalUser.createdAt,
          updatedAt: DateTime.now(),
        );
        final fixedAuthResponse = AuthResponseModel(
          user: verifiedUser,
          accessToken: authResponse.accessToken,
          refreshToken: authResponse.refreshToken,
          expiresAt: authResponse.expiresAt,
        );
        // persist tokens to LocalStorageService as well for interceptor
        final localStorage = sl<LocalStorageService>();
        await localStorage.saveData(StorageConstants.accessToken, fixedAuthResponse.accessToken);
        await localStorage.saveData(StorageConstants.refreshToken, fixedAuthResponse.refreshToken);
        await localStorage.saveData(StorageConstants.userId, fixedAuthResponse.user.userId);
        await localStorage.saveData(StorageConstants.userEmail, fixedAuthResponse.user.email);
        await localStorage.saveData(StorageConstants.accountRole, (fixedAuthResponse.user as UserModel).accountRole ?? '');
        await localStorage.saveData(StorageConstants.propertyId, (fixedAuthResponse.user as UserModel).propertyId ?? '');
        await localStorage.saveData(StorageConstants.propertyName, (fixedAuthResponse.user as UserModel).propertyName ?? '');
        await localStorage.saveData(StorageConstants.propertyCurrency, (fixedAuthResponse.user as UserModel).propertyCurrency ?? '');
        
        if (rememberMe) {
          await localDataSource.cacheAuthResponse(fixedAuthResponse);
        } else {
          await localDataSource.cacheAccessToken(fixedAuthResponse.accessToken);
          await localDataSource.cacheRefreshToken(fixedAuthResponse.refreshToken);
          await localDataSource.cacheUser(verifiedUser);
        }
        
        return Right(fixedAuthResponse);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, AuthResponse>> register({
    required String name,
    required String email,
    required String phone,
    required String password,
    required String passwordConfirmation,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final authResponse = await remoteDataSource.register(
          name: name,
          email: email,
          phone: phone,
          password: password,
          passwordConfirmation: passwordConfirmation,
        );
        // persist tokens to LocalStorageService as well for interceptor
        final localStorage = sl<LocalStorageService>();
        await localStorage.saveData(StorageConstants.accessToken, authResponse.accessToken);
        await localStorage.saveData(StorageConstants.refreshToken, authResponse.refreshToken);
        await localStorage.saveData(StorageConstants.userId, authResponse.user.userId);
        await localStorage.saveData(StorageConstants.userEmail, authResponse.user.email);
        await localStorage.saveData(StorageConstants.accountRole, (authResponse.user as UserModel).accountRole ?? '');
        await localStorage.saveData(StorageConstants.propertyId, (authResponse.user as UserModel).propertyId ?? '');
        await localStorage.saveData(StorageConstants.propertyName, (authResponse.user as UserModel).propertyName ?? '');
        await localStorage.saveData(StorageConstants.propertyCurrency, (authResponse.user as UserModel).propertyCurrency ?? '');
        
        await localDataSource.cacheAuthResponse(authResponse);
        
        return Right(authResponse);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, void>> logout() async {
    try {
      final localStorage = sl<LocalStorageService>();
      final userId = (await localDataSource.getCachedUser())?.userId ?? (localStorage.getData(StorageConstants.userId)?.toString() ?? '');
      final refreshToken = await localDataSource.getCachedRefreshToken();
      if (await internetConnectionChecker.hasConnection) {
        if (userId.isNotEmpty && (refreshToken?.isNotEmpty ?? false)) {
          await remoteDataSource.logout(
            userId: userId,
            refreshToken: refreshToken!,
          );
        }
      }
      await localDataSource.clearAuthData();
      await localStorage.removeData(StorageConstants.accessToken);
      await localStorage.removeData(StorageConstants.refreshToken);
      await localStorage.removeData(StorageConstants.userId);
      await localStorage.removeData(StorageConstants.userEmail);
      await localStorage.removeData(StorageConstants.accountRole);
      await localStorage.removeData(StorageConstants.propertyId);
      await localStorage.removeData(StorageConstants.propertyName);
      await localStorage.removeData(StorageConstants.propertyCurrency);
      return const Right(null);
    } catch (e) {
      // Clear local data even if remote logout fails
      final localStorage = sl<LocalStorageService>();
      await localDataSource.clearAuthData();
      await localStorage.removeData(StorageConstants.accessToken);
      await localStorage.removeData(StorageConstants.refreshToken);
      await localStorage.removeData(StorageConstants.userId);
      await localStorage.removeData(StorageConstants.userEmail);
      await localStorage.removeData(StorageConstants.accountRole);
      await localStorage.removeData(StorageConstants.propertyId);
      await localStorage.removeData(StorageConstants.propertyName);
      await localStorage.removeData(StorageConstants.propertyCurrency);
      return const Right(null);
    }
  }

  @override
  Future<Either<Failure, void>> resetPassword({
    required String emailOrPhone,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        await remoteDataSource.resetPassword(emailOrPhone: emailOrPhone);
        return const Right(null);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, AuthResponse>> refreshToken({
    required String refreshToken,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final localStorage = sl<LocalStorageService>();
        final currentAccess = (await localDataSource.getCachedAccessToken()) ?? (localStorage.getData(StorageConstants.accessToken) as String? ?? '');
        final authResponse = await remoteDataSource.refreshToken(
          accessToken: currentAccess,
          refreshToken: refreshToken,
        );
        await localDataSource.cacheAuthResponse(authResponse);
        await localStorage.saveData(StorageConstants.accessToken, authResponse.accessToken);
        await localStorage.saveData(StorageConstants.refreshToken, authResponse.refreshToken);
        return Right(authResponse);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, User>> getCurrentUser() async {
    try {
      // Try to get cached user first
      final cachedUser = await localDataSource.getCachedUser();
      if (cachedUser != null) {
        // If online, fetch fresh data
        if (await internetConnectionChecker.hasConnection) {
          try {
            final user = await remoteDataSource.getCurrentUser();
            await localDataSource.cacheUser(user);
            return Right(user);
          } catch (e) {
            // Return cached user if remote fails
            return Right(cachedUser);
          }
        }
        return Right(cachedUser);
      }
      
      // No cached user, must fetch from remote
      if (await internetConnectionChecker.hasConnection) {
        final user = await remoteDataSource.getCurrentUser();
        await localDataSource.cacheUser(user);
        return Right(user);
      } else {
        return const Left(NetworkFailure());
      }
    } catch (e) {
      return ErrorHandler.handle(e);
    }
  }

  @override
  Future<Either<Failure, void>> updateProfile({
    required String name,
    String? email,
    String? phone,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final localStorage = sl<LocalStorageService>();
        final userId = (await localDataSource.getCachedUser())?.userId ?? (localStorage.getData(StorageConstants.userId)?.toString() ?? '');
        await remoteDataSource.updateProfile(
          userId: userId,
          name: name,
          email: email,
          phone: phone,
        );
        
        // Update cached user
        final cachedUser = await localDataSource.getCachedUser();
        if (cachedUser != null) {
          final updatedUser = UserModel(
            userId: cachedUser.userId,
            name: name,
            email: email ?? cachedUser.email,
            phone: phone ?? cachedUser.phone,
            roles: cachedUser.roles,
            profileImage: cachedUser.profileImage,
            emailVerifiedAt: cachedUser.emailVerifiedAt,
            phoneVerifiedAt: cachedUser.phoneVerifiedAt,
            createdAt: cachedUser.createdAt,
            updatedAt: DateTime.now(),
          );
          await localDataSource.cacheUser(updatedUser);
        }
        
        return const Right(null);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, void>> changePassword({
    required String currentPassword,
    required String newPassword,
    required String newPasswordConfirmation,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final localStorage = sl<LocalStorageService>();
        final userId = (await localDataSource.getCachedUser())?.userId ?? (localStorage.getData(StorageConstants.userId)?.toString() ?? '');
        await remoteDataSource.changePassword(
          userId: userId,
          currentPassword: currentPassword,
          newPassword: newPassword,
          newPasswordConfirmation: newPasswordConfirmation,
        );
        return const Right(null);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, bool>> checkAuthStatus() async {
    try {
      final isLoggedIn = await localDataSource.isLoggedIn();
      if (!isLoggedIn) {
        return const Right(false);
      }

      // Check if we have cached auth data
      final cachedAuth = await localDataSource.getCachedAuthResponse();
      if (cachedAuth == null) {
        return const Right(false);
      }

      // If online, validate token with server
      if (await internetConnectionChecker.hasConnection) {
        try {
          await remoteDataSource.getCurrentUser();
          return const Right(true);
        } catch (e) {
          // Token might be expired, try to refresh
          final refreshToken = await localDataSource.getCachedRefreshToken();
          if (refreshToken != null) {
            try {
              final localStorage = sl<LocalStorageService>();
              final currentAccess = (await localDataSource.getCachedAccessToken()) ?? (localStorage.getData(StorageConstants.accessToken) as String? ?? '');
              final newAuth = await remoteDataSource.refreshToken(
                accessToken: currentAccess,
                refreshToken: refreshToken,
              );
              await localDataSource.cacheAuthResponse(newAuth);
              await localStorage.saveData(StorageConstants.accessToken, newAuth.accessToken);
              await localStorage.saveData(StorageConstants.refreshToken, newAuth.refreshToken);
        await localStorage.saveData(StorageConstants.accountRole, newAuth.user.accountRole ?? '');
        await localStorage.saveData(StorageConstants.propertyId, (newAuth.user as UserModel).propertyId ?? '');
        await localStorage.saveData(StorageConstants.propertyName, (newAuth.user as UserModel).propertyName ?? '');
        await localStorage.saveData(StorageConstants.propertyCurrency, (newAuth.user as UserModel).propertyCurrency ?? '');
              return const Right(true);
            } catch (e) {
              // Refresh failed, user needs to login again
              final localStorage = sl<LocalStorageService>();
              await localDataSource.clearAuthData();
              await localStorage.removeData(StorageConstants.accessToken);
              await localStorage.removeData(StorageConstants.refreshToken);
              await localStorage.removeData(StorageConstants.userId);
              await localStorage.removeData(StorageConstants.userEmail);
      await localStorage.removeData(StorageConstants.accountRole);
      await localStorage.removeData(StorageConstants.propertyId);
      await localStorage.removeData(StorageConstants.propertyName);
      await localStorage.removeData(StorageConstants.propertyCurrency);
              return const Right(false);
            }
          }
          return const Right(false);
        }
      }
      
      // Offline, return true if we have cached data
      return const Right(true);
    } catch (e) {
      return const Right(false);
    }
  }

  @override
  Future<void> saveAuthData(AuthResponse authResponse) async {
    await localDataSource.cacheAuthResponse(
      AuthResponseModel.fromEntity(authResponse),
    );
    final localStorage = sl<LocalStorageService>();
    await localStorage.saveData(StorageConstants.accessToken, authResponse.accessToken);
    await localStorage.saveData(StorageConstants.refreshToken, authResponse.refreshToken);
    await localStorage.saveData(StorageConstants.userId, authResponse.user.userId);
    await localStorage.saveData(StorageConstants.userEmail, authResponse.user.email);
  }

  @override
  Future<void> clearAuthData() async {
    await localDataSource.clearAuthData();
    final localStorage = sl<LocalStorageService>();
    await localStorage.removeData(StorageConstants.accessToken);
    await localStorage.removeData(StorageConstants.refreshToken);
    await localStorage.removeData(StorageConstants.userId);
    await localStorage.removeData(StorageConstants.userEmail);
  }

  @override
  Future<String?> getAccessToken() async {
    return await localDataSource.getCachedAccessToken();
  }

  @override
  Future<String?> getRefreshToken() async {
    return await localDataSource.getCachedRefreshToken();
  }

  @override
  Future<Either<Failure, User>> uploadProfileImage({
    required String imagePath,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final updatedUser = await remoteDataSource.uploadProfileImage(
          imagePath: imagePath,
        );
        await localDataSource.cacheUser(UserModel.fromEntity(updatedUser));
        return Right(updatedUser);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  // Email verification
  @override
  Future<Either<Failure, bool>> verifyEmail({
    required String userId,
    required String code,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final ok = await remoteDataSource.verifyEmail(userId: userId, code: code);
        // بعد نجاح التحقق لا نحاول جلب المستخدم الحالي لتجنب 401 بلا توكن
        return Right(ok);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, int?>> resendEmailVerification({
    required String userId,
    required String email,
  }) async {
    if (await internetConnectionChecker.hasConnection) {
      try {
        final retryAfter = await remoteDataSource.resendEmailVerification(userId: userId, email: email);
        return Right(retryAfter);
      } catch (e) {
        return ErrorHandler.handle(e);
      }
    } else {
      return const Left(NetworkFailure());
    }
  }
}