// lib/features/admin_properties/presentation/pages/edit_property_page.dart

import 'package:bookn_cp_app/core/theme/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'dart:ui';
import 'package:bookn_cp_app/core/theme/app_colors.dart';
import 'package:bookn_cp_app/core/theme/app_text_styles.dart';
import '../bloc/properties/properties_bloc.dart';
import '../bloc/property_types/property_types_bloc.dart';
import '../bloc/amenities/amenities_bloc.dart';
import '../bloc/property_images/property_images_bloc.dart'; // إضافة استيراد
import '../widgets/property_image_gallery.dart';
import '../widgets/amenity_selector_widget.dart';
import '../widgets/property_map_view.dart';
import 'package:bookn_cp_app/injection_container.dart' as di;
import 'package:bookn_cp_app/core/usecases/usecase.dart';
import 'package:bookn_cp_app/features/admin_currencies/domain/usecases/get_currencies_usecase.dart';
import 'package:bookn_cp_app/features/admin_cities/domain/usecases/get_cities_usecase.dart'
    as ci_uc;
import 'package:bookn_cp_app/core/widgets/loading_widget.dart';
import '../../domain/entities/property.dart';
import '../../domain/entities/property_type.dart';
import '../../domain/entities/property_image.dart'; // إضافة استيراد

class EditPropertyPage extends StatelessWidget {
  final String propertyId;

  const EditPropertyPage({
    super.key,
    required this.propertyId,
  });

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(
          create: (_) => di.sl<PropertiesBloc>()
            ..add(LoadPropertyDetailsEvent(propertyId: propertyId)),
        ),
        BlocProvider(
          create: (_) => di.sl<PropertyTypesBloc>()
            ..add(const LoadPropertyTypesEvent(pageSize: 100)),
        ),
        BlocProvider(
          create: (_) =>
              di.sl<AmenitiesBloc>()..add(const LoadAmenitiesEvent()),
        ),
        // إضافة PropertyImagesBloc
        BlocProvider(
          create: (_) => di.sl<PropertyImagesBloc>(),
        ),
      ],
      child: _EditPropertyPageContent(propertyId: propertyId),
    );
  }
}

class _EditPropertyPageContent extends StatefulWidget {
  final String propertyId;

  const _EditPropertyPageContent({
    required this.propertyId,
  });

  @override
  State<_EditPropertyPageContent> createState() =>
      _EditPropertyPageContentState();
}

class _EditPropertyPageContentState extends State<_EditPropertyPageContent>
    with TickerProviderStateMixin {
  // Animation Controllers
  late AnimationController _animationController;
  late AnimationController _shimmerController;
  late Animation<double> _fadeAnimation;
  late Animation<Offset> _slideAnimation;
  late AnimationController _glowController;

  // Form Controllers
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _addressController = TextEditingController();
  final _cityController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _latitudeController = TextEditingController();
  final _longitudeController = TextEditingController();
  final _shortDescriptionController = TextEditingController();
  final _basePriceController = TextEditingController();

  // State
  String? _selectedPropertyTypeId;
  int _starRating = 3;
  List<PropertyImage> _selectedImages = []; // تغيير نوع البيانات
  List<String> _selectedAmenities = [];
  bool _isFeatured = false;
  String _currency = 'YER';
  String? _selectedCity;
  final _currencyDropdownKey = GlobalKey();
  Property? _currentProperty;
  bool _isDataLoaded = false;
  bool _isNavigating = false;
  int _currentStep = 0;
  final GlobalKey<PropertyImageGalleryState> _galleryKey = GlobalKey();
  bool _isDeleting = false;

  void _showDeletingDialog({String message = 'جاري حذف العقار...'}) {
    if (_isDeleting) return;
    _isDeleting = true;
    showDialog(
      fullscreenDialog: true,
      context: context,
      barrierDismissible: false,
      barrierColor: Colors.transparent,
      builder: (dialogContext) => Dialog(
        backgroundColor: Colors.transparent,
        insetPadding: EdgeInsets.zero,
        elevation: 0,
        child: Center(
          child: LoadingWidget(
            type: LoadingType.futuristic,
            message: message,
          ),
        ),
      ),
    );
  }

  void _dismissDeletingDialog() {
    if (_isDeleting) {
      _isDeleting = false;
      if (mounted) {
        Navigator.of(context, rootNavigator: true).pop();
      }
    }
  }

  @override
  void initState() {
    super.initState();
    _initializeAnimations();
  }

  void _initializeAnimations() {
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );

    _shimmerController = AnimationController(
      duration: const Duration(seconds: 1),
      vsync: this,
    )..repeat();
    _glowController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat(reverse: true);

    _fadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOut,
    ));

    _slideAnimation = Tween<Offset>(
      begin: const Offset(0, 0.1),
      end: Offset.zero,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOutQuart,
    ));

    _animationController.forward();
  }

  void _loadPropertyDataToForm(Property property) {
    if (!_isDataLoaded) {
      setState(() {
        _currentProperty = property;
        _nameController.text = property.name;
        _shortDescriptionController.text = property.shortDescription ?? '';
        _addressController.text = property.address;
        _cityController.text = property.city;
        _selectedCity = property.city.isNotEmpty ? property.city : null;
        _descriptionController.text = property.description;
        _latitudeController.text = property.latitude?.toString() ?? '';
        _longitudeController.text = property.longitude?.toString() ?? '';
        _basePriceController.text = property.basePricePerNight.toString();
        _selectedPropertyTypeId = property.typeId;
        _starRating = property.starRating;
        _isFeatured = property.isFeatured;
        _currency = property.currency;
        _selectedImages = property.images; // تخزين كائنات PropertyImage
        _selectedAmenities =
            property.amenities.map((amenity) => amenity.id).toList();
        _isDataLoaded = true;
      });
    }
  }

  @override
  void dispose() {
    _animationController.dispose();
    _shimmerController.dispose();
    _glowController.dispose();
    _nameController.dispose();
    _addressController.dispose();
    _cityController.dispose();
    _descriptionController.dispose();
    _latitudeController.dispose();
    _longitudeController.dispose();
    _shortDescriptionController.dispose();
    _basePriceController.dispose();
    super.dispose();
  }

  void _showSnackBar(String message,
      {Color? backgroundColor, bool isError = false}) {
    if (!mounted) return;

    Future.microtask(() {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(message),
          backgroundColor:
              backgroundColor ?? (isError ? AppTheme.error : AppTheme.success),
          behavior: SnackBarBehavior.floating,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          duration: const Duration(seconds: 2),
        ),
      );
    });
  }

  void _navigateBack() {
    if (!_isNavigating && mounted) {
      _isNavigating = true;
      Future.delayed(const Duration(milliseconds: 100), () {
        if (mounted) {
          context.pop();
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return WillPopScope(
      onWillPop: () async {
        return !_isNavigating;
      },
      child: BlocListener<PropertiesBloc, PropertiesState>(
        listener: (context, state) {
          if (state is PropertyDeleting) {
            _showDeletingDialog();
          } else if (state is PropertyDeleted) {
            _dismissDeletingDialog();
            _showSnackBar('تم حذف العقار بنجاح');
            _navigateBack();
          } else if (state is PropertiesError && _isDeleting) {
            _dismissDeletingDialog();
            _showSnackBar('فشل الحذف: ${state.message}', isError: true);
          }
        },
        child: Scaffold(
        backgroundColor: AppTheme.darkBackground,
        body: Stack(
          children: [
            _buildAnimatedBackground(),
            SafeArea(
              child: Column(
                children: [
                  _buildHeader(),
                  _buildProgressIndicator(),
                  Expanded(
                    child: BlocConsumer<PropertiesBloc, PropertiesState>(
                      listenWhen: (previous, current) {
                        return !_isNavigating;
                      },
                      listener: (context, state) {
                        if (state is PropertyDetailsLoaded) {
                          _loadPropertyDataToForm(state.property);
                        } else if (state is PropertyDetailsError) {
                          _showSnackBar(
                              'خطأ في تحميل البيانات: ${state.message}',
                              isError: true);
                        } else if (state is PropertyUpdated) {
                          _showSnackBar('تم حفظ التغييرات بنجاح');
                          _navigateBack();
                        } else if (state is PropertyDeleted) {
                          _showSnackBar('تم حذف العقار بنجاح');
                          _navigateBack();
                        } else if (state is PropertiesError) {
                          _showSnackBar('خطأ: ${state.message}', isError: true);
                        }
                      },
                      builder: (context, state) {
                        if (state is PropertyDetailsLoading) {
                          return _buildLoadingState();
                        } else if (state is PropertyDetailsError) {
                          return _buildErrorState(state.message);
                        } else if (state is PropertyDetailsLoaded) {
                return FadeTransition(
                            opacity: _fadeAnimation,
                            child: SlideTransition(
                              position: _slideAnimation,
                    child: _buildFormContent(),
                            ),
                          );
                        } else if (state is PropertyUpdating) {
                          return _buildUpdatingState();
                        } else {
                          return _buildLoadingState();
                        }
                      },
                    ),
                  ),
                  BlocBuilder<PropertiesBloc, PropertiesState>(
                    builder: (context, state) {
                      if (state is PropertyDetailsLoaded ||
                          state is PropertyUpdating) {
                        return _buildActionButtons(state is PropertyUpdating);
                      }
                      return const SizedBox.shrink();
                    },
                  ),
                ],
              ),
            ),
          ],
        ),
        ),
      ),
    );
  }

  Widget _buildAnimatedBackground() {
    return AnimatedBuilder(
      animation: _glowController,
      builder: (context, child) {
        return Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: [
                AppTheme.darkBackground,
                AppTheme.darkBackground2.withOpacity(0.8),
                AppTheme.darkBackground3.withOpacity(0.6),
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildHeader() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.darkCard.withOpacity(0.7),
            AppTheme.darkCard.withOpacity(0.3),
          ],
        ),
        border: Border(
          bottom: BorderSide(
            color: AppTheme.primaryBlue.withOpacity(0.3),
            width: 1,
          ),
        ),
      ),
      child: Row(
        children: [
          GestureDetector(
            onTap: _isNavigating ? null : () => _navigateBack(),
            child: Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    AppTheme.darkSurface.withOpacity(0.5),
                    AppTheme.darkSurface.withOpacity(0.3),
                  ],
                ),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(
                  color: AppTheme.darkBorder.withOpacity(0.3),
                  width: 1,
                ),
              ),
              child: Icon(
                Icons.arrow_back_rounded,
                color: AppTheme.textWhite,
                size: 20,
              ),
            ),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                ShaderMask(
                  shaderCallback: (bounds) =>
                      AppTheme.primaryGradient.createShader(bounds),
                  child: Text(
                    'تعديل العقار',
                    style: AppTextStyles.heading2.copyWith(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(height: 4),
                BlocBuilder<PropertiesBloc, PropertiesState>(
                  builder: (context, state) {
                    String propertyName = 'جاري التحميل...';
                    if (state is PropertyDetailsLoaded) {
                      propertyName = state.property.name;
                    }
                    return Text(
                      propertyName,
                      style: AppTextStyles.bodySmall.copyWith(
                        color: AppTheme.textMuted,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    );
                  },
                ),
              ],
            ),
          ),
          GestureDetector(
            onTap: _isNavigating ? null : _showDeleteConfirmation,
            child: Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    AppTheme.error.withOpacity(0.2),
                    AppTheme.error.withOpacity(0.1),
                  ],
                ),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(
                  color: AppTheme.error.withOpacity(0.3),
                  width: 1,
                ),
              ),
              child: Icon(
                Icons.delete_rounded,
                color: AppTheme.error,
                size: 20,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFormContent() {
    return Form(
      key: _formKey,
      child: IndexedStack(
        index: _currentStep,
        children: [
          _buildBasicInfoStep(),
          _buildLocationStep(),
          _buildImagesAmenitiesStep(),
          _buildReviewStep(),
        ],
      ),
    );
  }

  Widget _buildBasicInfoStep() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildInputField(
            controller: _nameController,
            label: 'اسم العقار',
            icon: Icons.business_rounded,
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'يرجى إدخال اسم العقار';
              }
              return null;
            },
          ),
          const SizedBox(height: 20),
          _buildPropertyTypeDropdown(),
          const SizedBox(height: 20),
          Row(
            children: [
              Expanded(
                child: _buildInputField(
                  controller: _basePriceController,
                  label: 'السعر الأساسي',
                  icon: Icons.attach_money_rounded,
                  keyboardType: TextInputType.number,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'يرجى إدخال السعر';
                    }
                    if (double.tryParse(value) == null) {
                      return 'يرجى إدخال رقم صحيح';
                    }
                    return null;
                  },
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                  child: _CurrencyDropdown(
                      value: _currency,
                      onChanged: (v) => setState(() => _currency = v))),
            ],
          ),
          const SizedBox(height: 20),
          _buildStarRatingSelector(),
          const SizedBox(height: 20),
          _buildFeaturedSwitch(),
          const SizedBox(height: 20),
          _buildInputField(
            controller: _shortDescriptionController,
            label: 'وصف مختصر',
            icon: Icons.short_text_rounded,
          ),
          const SizedBox(height: 20),
          _buildInputField(
            controller: _descriptionController,
            label: 'الوصف التفصيلي',
            icon: Icons.description_rounded,
            maxLines: 5,
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'يرجى إدخال الوصف';
              }
              return null;
            },
          ),
        ],
      ),
    );
  }

  Widget _buildLocationStep() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            height: 300,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  AppTheme.darkCard.withOpacity(0.7),
                  AppTheme.darkCard.withOpacity(0.5),
                ],
              ),
              borderRadius: BorderRadius.circular(16),
              border: Border.all(
                color: AppTheme.darkBorder.withOpacity(0.3),
                width: 1,
              ),
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(16),
              child: PropertyMapView(
                onLocationSelected: (latLng) {
                  setState(() {
                    _latitudeController.text = latLng.latitude.toString();
                    _longitudeController.text = latLng.longitude.toString();
                  });
                },
              ),
            ),
          ),
          const SizedBox(height: 20),
          Row(
            children: [
              Expanded(
                child: _buildInputField(
                  controller: _latitudeController,
                  label: 'خط العرض',
                  icon: Icons.my_location_rounded,
                  keyboardType:
                      const TextInputType.numberWithOptions(decimal: true),
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'يرجى إدخال خط العرض';
                    }
                    final v = double.tryParse(value);
                    if (v == null || v < -90 || v > 90) {
                      return 'خط العرض غير صحيح';
                    }
                    return null;
                  },
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: _buildInputField(
                  controller: _longitudeController,
                  label: 'خط الطول',
                  icon: Icons.my_location_rounded,
                  keyboardType:
                      const TextInputType.numberWithOptions(decimal: true),
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'يرجى إدخال خط الطول';
                    }
                    final v = double.tryParse(value);
                    if (v == null || v < -180 || v > 180) {
                      return 'خط الطول غير صحيح';
                    }
                    return null;
                  },
                ),
              ),
            ],
          ),
          const SizedBox(height: 20),
          _buildInputField(
            controller: _addressController,
            label: 'العنوان',
            icon: Icons.location_on_rounded,
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'يرجى إدخال العنوان';
              }
              return null;
            },
          ),
          const SizedBox(height: 20),
          _CityDropdown(
            value: _selectedCity,
            onChanged: (v) {
              setState(() {
                _selectedCity = v;
                _cityController.text = v ?? '';
              });
            },
            requiredField: true,
          ),
        ],
      ),
    );
  }

  Widget _buildImagesAmenitiesStep() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'صور العقار',
            style: AppTextStyles.heading3.copyWith(
              color: AppTheme.textWhite,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          PropertyImageGallery(
            key: _galleryKey,
            propertyId: widget.propertyId,
            initialImages: _selectedImages,
            onImagesChanged: (images) {
              setState(() {
                _selectedImages = images;
              });
            },
            maxImages: 10,
          ),
          const SizedBox(height: 30),
          Text(
            'المرافق المتاحة',
            style: AppTextStyles.heading3.copyWith(
              color: AppTheme.textWhite,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          AmenitySelectorWidget(
            selectedAmenities: _selectedAmenities,
            onAmenitiesChanged: (amenities) {
              setState(() {
                _selectedAmenities = amenities;
              });
            },
            propertyTypeId: _selectedPropertyTypeId ?? _currentProperty?.typeId,
          ),
        ],
      ),
    );
  }

  Widget _buildReviewStep() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'مراجعة البيانات',
            style: AppTextStyles.heading2.copyWith(
              color: AppTheme.textWhite,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 20),
          _buildReviewCard(
            title: 'المعلومات الأساسية',
            items: [
              {'label': 'الاسم', 'value': _nameController.text},
              {'label': 'النوع', 'value': _currentProperty?.typeName ?? ''},
              {'label': 'التقييم', 'value': '$_starRating نجوم'},
              {'label': 'السعر', 'value': _basePriceController.text},
              {'label': 'العملة', 'value': _currency},
              {'label': 'مميز', 'value': _isFeatured ? 'نعم' : 'لا'},
            ],
          ),
          const SizedBox(height: 16),
          _buildReviewCard(
            title: 'الموقع',
            items: [
              {'label': 'العنوان', 'value': _addressController.text},
              {'label': 'المدينة', 'value': _cityController.text},
              {'label': 'خط العرض', 'value': _latitudeController.text},
              {'label': 'خط الطول', 'value': _longitudeController.text},
            ],
          ),
          const SizedBox(height: 16),
          _buildReviewCard(
            title: 'الصور والمرافق',
            items: [
              {'label': 'عدد الصور', 'value': '${_selectedImages.length}'},
              {'label': 'عدد المرافق', 'value': '${_selectedAmenities.length}'},
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildProgressIndicator() {
    final steps = [
      'المعلومات الأساسية',
      'الموقع',
      'الصور والمرافق',
      'المراجعة'
    ];
    return Container(
      height: 80,
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
      child: Row(
        children: List.generate(steps.length, (index) {
          final isActive = index <= _currentStep;
          final isCompleted = index < _currentStep;
          return Expanded(
            child: Row(
              children: [
                AnimatedContainer(
                  duration: const Duration(milliseconds: 300),
                  width: 32,
                  height: 32,
                  decoration: BoxDecoration(
                    gradient: isActive ? AppTheme.primaryGradient : null,
                    color: !isActive
                        ? AppTheme.darkSurface.withOpacity(0.5)
                        : null,
                    shape: BoxShape.circle,
                    border: Border.all(
                      color: isActive
                          ? AppTheme.primaryBlue.withOpacity(0.5)
                          : AppTheme.darkBorder.withOpacity(0.3),
                      width: 1,
                    ),
                    boxShadow: isActive
                        ? [
                            BoxShadow(
                              color: AppTheme.primaryBlue.withOpacity(0.3),
                              blurRadius: 10,
                            ),
                          ]
                        : null,
                  ),
                  child: Center(
                    child: isCompleted
                        ? const Icon(Icons.check_rounded,
                            size: 16, color: Colors.white)
                        : Text(
                            '${index + 1}',
                            style: AppTextStyles.caption.copyWith(
                              color:
                                  isActive ? Colors.white : AppTheme.textMuted,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                  ),
                ),
                if (index < steps.length - 1)
                  Expanded(
                    child: Container(
                      height: 2,
                      margin: const EdgeInsets.symmetric(horizontal: 8),
                      decoration: BoxDecoration(
                        gradient: isCompleted ? AppTheme.primaryGradient : null,
                        color: !isCompleted
                            ? AppTheme.darkBorder.withOpacity(0.2)
                            : null,
                        borderRadius: BorderRadius.circular(1),
                      ),
                    ),
                  ),
              ],
            ),
          );
        }),
      ),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Row(
      children: [
        Container(
          width: 4,
          height: 20,
          decoration: BoxDecoration(
            gradient: AppTheme.primaryGradient,
            borderRadius: BorderRadius.circular(2),
          ),
        ),
        const SizedBox(width: 8),
        Text(
          title,
          style: AppTextStyles.heading3.copyWith(
            color: AppTheme.textWhite,
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildInputField({
    required TextEditingController controller,
    required String label,
    required IconData icon,
    TextInputType keyboardType = TextInputType.text,
    int maxLines = 1,
    String? Function(String?)? validator,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: AppTextStyles.bodySmall.copyWith(
            color: AppTheme.textMuted,
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                AppTheme.darkCard.withOpacity(0.5),
                AppTheme.darkCard.withOpacity(0.3),
              ],
            ),
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: AppTheme.darkBorder.withOpacity(0.3),
              width: 1,
            ),
          ),
          child: TextFormField(
            controller: controller,
            keyboardType: keyboardType,
            maxLines: maxLines,
            validator: validator,
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppTheme.textWhite,
            ),
            decoration: InputDecoration(
              prefixIcon: maxLines == 1
                  ? Icon(
                      icon,
                      color: AppTheme.primaryBlue.withOpacity(0.7),
                      size: 20,
                    )
                  : null,
              border: InputBorder.none,
              contentPadding: const EdgeInsets.all(16),
              errorStyle: AppTextStyles.bodySmall.copyWith(
                color: AppTheme.error,
              ),
            ),
          ),
        ),
      ],
    );
  }

  // باقي الدوال (_buildPropertyTypeDropdown, _buildCurrencyDropdown, etc.) تبقى كما هي...

  Widget _buildLoadingState() {
    return AnimatedBuilder(
      animation: _shimmerController,
      builder: (context, child) {
        return SingleChildScrollView(
          padding: const EdgeInsets.all(20),
          child: Column(
            children: List.generate(5, (index) => _buildShimmerBox()),
          ),
        );
      },
    );
  }

  Widget _buildUpdatingState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          CircularProgressIndicator(
            valueColor: AlwaysStoppedAnimation<Color>(AppTheme.primaryBlue),
          ),
          const SizedBox(height: 16),
          Text(
            'جاري حفظ التغييرات...',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppTheme.textWhite,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildErrorState(String message) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              width: 80,
              height: 80,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    AppTheme.error.withOpacity(0.2),
                    AppTheme.error.withOpacity(0.1),
                  ],
                ),
                shape: BoxShape.circle,
              ),
              child: Icon(
                Icons.error_rounded,
                size: 48,
                color: AppTheme.error,
              ),
            ),
            const SizedBox(height: 20),
            Text(
              'خطأ في تحميل البيانات',
              style: AppTextStyles.heading2.copyWith(
                color: AppTheme.textWhite,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),
            Text(
              message,
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppTheme.textLight,
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 30),
            GestureDetector(
              onTap: () {
                context.read<PropertiesBloc>().add(
                      LoadPropertyDetailsEvent(propertyId: widget.propertyId),
                    );
              },
              child: Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                decoration: BoxDecoration(
                  gradient: AppTheme.primaryGradient,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Text(
                  'إعادة المحاولة',
                  style: AppTextStyles.buttonMedium.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildShimmerBox() {
    return Container(
      height: 60,
      margin: const EdgeInsets.only(bottom: 16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            AppTheme.darkCard.withOpacity(0.3),
            AppTheme.darkCard.withOpacity(0.5),
            AppTheme.darkCard.withOpacity(0.3),
          ],
          stops: [
            0.0,
            _shimmerController.value,
            1.0,
          ],
        ),
        borderRadius: BorderRadius.circular(12),
      ),
    );
  }

  // باقي الدوال كما هي...
  Widget _buildPropertyTypeDropdown() {
    // نفس الكود السابق
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'نوع العقار',
          style: AppTextStyles.bodySmall.copyWith(
            color: AppTheme.textMuted,
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                AppTheme.darkCard.withOpacity(0.5),
                AppTheme.darkCard.withOpacity(0.3),
              ],
            ),
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: AppTheme.darkBorder.withOpacity(0.3),
              width: 1,
            ),
          ),
          child: BlocBuilder<PropertyTypesBloc, PropertyTypesState>(
            builder: (context, state) {
              List<PropertyType> propertyTypes = [];
              bool isLoading = false;

              if (state is PropertyTypesLoading) {
                isLoading = true;
              } else if (state is PropertyTypesLoaded) {
                propertyTypes = state.propertyTypes;
              }

              String? validSelectedValue = _selectedPropertyTypeId;
              if (validSelectedValue != null &&
                  propertyTypes.isNotEmpty &&
                  !propertyTypes.any((type) => type.id == validSelectedValue)) {
                if (_currentProperty != null) {
                  propertyTypes.insert(
                      0,
                      PropertyType(
                        id: _currentProperty!.typeId,
                        name: _currentProperty!.typeName,
                        description: '',
                        defaultAmenities: const [],
                        icon: '',
                        propertiesCount: 0,
                      ));
                } else {
                  validSelectedValue = null;
                }
              }

              if (isLoading) {
                return Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
                  child: Row(
                    children: [
                      SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          valueColor: AlwaysStoppedAnimation<Color>(
                            AppTheme.primaryBlue.withOpacity(0.7),
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Text(
                        'جاري تحميل أنواع العقارات...',
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: AppTheme.textMuted,
                        ),
                      ),
                    ],
                  ),
                );
              }

              if (propertyTypes.isEmpty) {
                return Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
                  child: Text(
                    'لا توجد أنواع عقارات متاحة',
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: AppTheme.textMuted,
                    ),
                  ),
                );
              }

              return DropdownButtonFormField<String>(
                initialValue: validSelectedValue,
                decoration: const InputDecoration(
                  border: InputBorder.none,
                  contentPadding:
                      EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                ),
                dropdownColor: AppTheme.darkCard,
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppTheme.textWhite,
                ),
                hint: Text(
                  'اختر نوع العقار',
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: AppTheme.textMuted,
                  ),
                ),
                items: propertyTypes.map((type) {
                  return DropdownMenuItem(
                    value: type.id,
                    child: Text(type.name),
                  );
                }).toList(),
                onChanged: (value) {
                  setState(() {
                    _selectedPropertyTypeId = value;
                  });
                  if (value != null && value.isNotEmpty) {
                    context.read<AmenitiesBloc>().add(
                          LoadAmenitiesEventWithType(
                            propertyTypeId: value,
                            pageSize: 100,
                          ),
                        );
                  }
                },
                validator: (value) {
                  if (value == null) {
                    return 'يرجى اختيار نوع العقار';
                  }
                  return null;
                },
              );
            },
          ),
        ),
      ],
    );
  }

  // Removed old hardcoded currency dropdown

  Widget _buildStarRatingSelector() {
    // نفس الكود السابق
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'التقييم',
          style: AppTextStyles.bodySmall.copyWith(
            color: AppTheme.textMuted,
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 8),
        Row(
          children: List.generate(5, (index) {
            return GestureDetector(
              onTap: () {
                setState(() {
                  _starRating = index + 1;
                });
              },
              child: Padding(
                padding: const EdgeInsets.only(right: 4),
                child: Icon(
                  index < _starRating
                      ? Icons.star_rounded
                      : Icons.star_border_rounded,
                  color: index < _starRating
                      ? AppTheme.warning
                      : AppTheme.textMuted,
                  size: 32,
                ),
              ),
            );
          }),
        ),
      ],
    );
  }

  Widget _buildFeaturedSwitch() {
    // نفس الكود السابق
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.darkCard.withOpacity(0.3),
            AppTheme.darkCard.withOpacity(0.2),
          ],
        ),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: _isFeatured
              ? AppTheme.warning.withOpacity(0.3)
              : AppTheme.darkBorder.withOpacity(0.3),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            children: [
              Icon(
                Icons.star_rounded,
                color: _isFeatured ? AppTheme.warning : AppTheme.textMuted,
                size: 24,
              ),
              const SizedBox(width: 8),
              Text(
                'عقار مميز',
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppTheme.textWhite,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
          Switch(
            value: _isFeatured,
            onChanged: (value) {
              setState(() {
                _isFeatured = value;
              });
            },
            activeThumbColor: AppTheme.warning,
            inactiveThumbColor: AppTheme.textMuted,
            inactiveTrackColor: AppTheme.darkBorder.withOpacity(0.3),
          ),
        ],
      ),
    );
  }

  Widget _buildMapButton() {
    // نفس الكود السابق
    return GestureDetector(
      onTap: () {
        // TODO: Open map selector
      },
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          gradient: LinearGradient(
            colors: [
              AppTheme.primaryBlue.withOpacity(0.1),
              AppTheme.primaryPurple.withOpacity(0.1),
            ],
          ),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: AppTheme.primaryBlue.withOpacity(0.3),
            width: 1,
          ),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.map_rounded,
              color: AppTheme.primaryBlue,
              size: 24,
            ),
            const SizedBox(width: 8),
            Text(
              'اختر الموقع من الخريطة',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppTheme.primaryBlue,
                fontWeight: FontWeight.w600,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActionButtons(bool isUpdating) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.darkCard.withOpacity(0.7),
            AppTheme.darkCard.withOpacity(0.5),
          ],
        ),
        border: Border(
          top: BorderSide(
            color: AppTheme.darkBorder.withOpacity(0.3),
            width: 1,
          ),
        ),
      ),
      child: Row(
        children: [
          if (_currentStep > 0)
            Expanded(
              child: GestureDetector(
                onTap: (isUpdating || _isNavigating)
                    ? null
                    : () {
                        setState(() {
                          _currentStep = (_currentStep - 1).clamp(0, 3);
                        });
                      },
                child: Container(
                  height: 48,
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      colors: [
                        AppTheme.darkSurface
                            .withOpacity(isUpdating ? 0.3 : 0.5),
                        AppTheme.darkSurface
                            .withOpacity(isUpdating ? 0.2 : 0.3),
                      ],
                    ),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color: AppTheme.darkBorder.withOpacity(0.3),
                      width: 1,
                    ),
                  ),
                  child: Center(
                    child: Text(
                      'السابق',
                      style: AppTextStyles.buttonMedium.copyWith(
                        color: isUpdating
                            ? AppTheme.textMuted
                            : AppTheme.textWhite,
                      ),
                    ),
                  ),
                ),
              ),
            ),
          if (_currentStep > 0) const SizedBox(width: 12),
          Expanded(
            child: GestureDetector(
              onTap: (isUpdating || _isNavigating)
                  ? null
                  : () {
                      if (_currentStep < 3) {
                        if (_validateCurrentStep()) {
                          setState(() {
                            _currentStep = (_currentStep + 1).clamp(0, 3);
                          });
                        }
                      } else {
                        _saveChanges();
                      }
                    },
              child: Container(
                height: 48,
                decoration: BoxDecoration(
                  gradient: isUpdating
                      ? LinearGradient(colors: [
                          AppTheme.primaryBlue.withOpacity(0.3),
                          AppTheme.primaryPurple.withOpacity(0.3),
                        ])
                      : AppTheme.primaryGradient,
                  borderRadius: BorderRadius.circular(12),
                  boxShadow: isUpdating
                      ? []
                      : [
                          BoxShadow(
                            color: AppTheme.primaryBlue.withOpacity(0.3),
                            blurRadius: 12,
                            offset: const Offset(0, 4),
                          ),
                        ],
                ),
                child: Center(
                  child: isUpdating
                      ? const SizedBox(
                          width: 20,
                          height: 20,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            valueColor:
                                AlwaysStoppedAnimation<Color>(Colors.white),
                          ),
                        )
                      : Text(
                          _currentStep < 3 ? 'التالي' : 'حفظ التغييرات',
                          style: AppTextStyles.buttonMedium.copyWith(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _saveChanges() {
    if (_formKey.currentState!.validate() && !_isNavigating) {
      // استخراج URLs من PropertyImage objects
      final List<String> imageUrls =
          _selectedImages.map((img) => img.url).toList();

      context.read<PropertiesBloc>().add(
            UpdatePropertyEvent(
              propertyId: widget.propertyId,
              name: _nameController.text,
              address: _addressController.text,
              city: _cityController.text.isEmpty ? null : _cityController.text,
              description: _descriptionController.text,
              latitude: double.tryParse(_latitudeController.text),
              longitude: double.tryParse(_longitudeController.text),
              starRating: _starRating,
              images: imageUrls, // تمرير URLs فقط
              shortDescription: _shortDescriptionController.text.isNotEmpty
                  ? _shortDescriptionController.text
                  : null,
              basePricePerNight: double.tryParse(_basePriceController.text),
              currency: _currency,
              isFeatured: _isFeatured,
            ),
          );
    }
  }

  bool _validateCurrentStep() {
    switch (_currentStep) {
      case 0:
        return _nameController.text.isNotEmpty &&
            _selectedPropertyTypeId != null &&
            _descriptionController.text.isNotEmpty &&
            _basePriceController.text.isNotEmpty &&
            double.tryParse(_basePriceController.text) != null;
      case 1:
        final lat = double.tryParse(_latitudeController.text);
        final lng = double.tryParse(_longitudeController.text);
        if (_addressController.text.isEmpty) return false;
        if (lat == null || lat < -90 || lat > 90) return false;
        if (lng == null || lng < -180 || lng > 180) return false;
        return true;
      case 2:
        return true;
      default:
        return true;
    }
  }

  Widget _buildReviewCard({
    required String title,
    required List<Map<String, String>> items,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.darkCard.withOpacity(0.5),
            AppTheme.darkCard.withOpacity(0.3),
          ],
        ),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppTheme.darkBorder.withOpacity(0.3),
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppTheme.primaryBlue,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          ...items.map((item) => Padding(
                padding: const EdgeInsets.only(bottom: 8),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      item['label'] ?? '',
                      style: AppTextStyles.bodySmall.copyWith(
                        color: AppTheme.textMuted,
                      ),
                    ),
                    Expanded(
                      child: Text(
                        item['value'] ?? '',
                        style: AppTextStyles.bodySmall.copyWith(
                          color: AppTheme.textWhite,
                          fontWeight: FontWeight.w600,
                        ),
                        textAlign: TextAlign.end,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ],
                ),
              )),
        ],
      ),
    );
  }

  void _showDeleteConfirmation() {
    if (_isNavigating) return;

    showDialog(
      fullscreenDialog: true,
      context: context,
      barrierDismissible: false,
      builder: (dialogContext) => _DeleteConfirmationDialog(
        onConfirm: () {
          Navigator.pop(dialogContext);
          if (mounted) {
            context.read<PropertiesBloc>().add(
                  DeletePropertyEvent(widget.propertyId),
                );
          }
        },
      ),
    );
  }
}

class _CurrencyDropdown extends StatefulWidget {
  final String value;
  final ValueChanged<String> onChanged;
  const _CurrencyDropdown({required this.value, required this.onChanged});

  @override
  State<_CurrencyDropdown> createState() => _CurrencyDropdownState();
}

class _CurrencyDropdownState extends State<_CurrencyDropdown> {
  List<String> _codes = const [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final usecase = di.sl<GetCurrenciesUseCase>();
      final result = await usecase(NoParams());
      result.fold(
        (f) => setState(() {
          _error = f.message;
          _loading = false;
        }),
        (list) => setState(() {
          _codes = list.map((c) => c.code).toList();
          _loading = false;
          if (_codes.isNotEmpty && !_codes.contains(widget.value)) {
            widget.onChanged(_codes.first);
          }
        }),
      );
    } catch (e) {
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final decoration = InputDecoration(
      labelText: 'العملة',
      labelStyle: AppTextStyles.bodySmall.copyWith(color: AppTheme.textMuted),
      filled: true,
      fillColor: AppTheme.darkSurface.withOpacity(0.3),
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide.none,
      ),
    );
    if (_loading) {
      return InputDecorator(
        decoration: decoration,
        child: Row(children: [
          const SizedBox(width: 4, height: 4),
          SizedBox(
            width: 18,
            height: 18,
            child: CircularProgressIndicator(
                strokeWidth: 2, color: AppTheme.textMuted),
          ),
          const SizedBox(width: 8),
          Text('جاري تحميل العملات...',
              style: AppTextStyles.caption.copyWith(color: AppTheme.textMuted)),
        ]),
      );
    }
    if (_error != null) {
      return DropdownButtonFormField<String>(
        initialValue: _codes.contains(widget.value) ? widget.value : null,
        decoration: decoration.copyWith(errorText: _error),
        items: _codes
            .map((c) => DropdownMenuItem(value: c, child: Text(c)))
            .toList(),
        onChanged: (v) {
          if (v != null) widget.onChanged(v);
        },
      );
    }
    return DropdownButtonFormField<String>(
      initialValue: _codes.contains(widget.value) ? widget.value : null,
      decoration: decoration,
      dropdownColor: AppTheme.darkCard,
      style: AppTextStyles.bodyMedium.copyWith(color: AppTheme.textWhite),
      items: _codes
          .map((c) => DropdownMenuItem(value: c, child: Text(c)))
          .toList(),
      onChanged: (v) {
        if (v != null) widget.onChanged(v);
      },
    );
  }
}

class _CityDropdown extends StatefulWidget {
  final String? value;
  final ValueChanged<String?> onChanged;
  final bool requiredField;
  const _CityDropdown(
      {required this.value,
      required this.onChanged,
      this.requiredField = false});

  @override
  State<_CityDropdown> createState() => _CityDropdownState();
}

class _CityDropdownState extends State<_CityDropdown> {
  List<String> _cities = const [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final usecase = di.sl<ci_uc.GetCitiesUseCase>();
      final result = await usecase(const ci_uc.GetCitiesParams());
      result.fold(
        (f) => setState(() {
          _error = f.message;
          _loading = false;
        }),
        (list) => setState(() {
          _cities = list.map((c) => c.name).toList();
          _loading = false;
          if (_cities.isNotEmpty &&
              (widget.value == null || !_cities.contains(widget.value))) {
            widget.onChanged(_cities.first);
          }
        }),
      );
    } catch (e) {
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    const decoration = InputDecoration(
      border: InputBorder.none,
      contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 12),
    );

    if (_loading) {
      return Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
        child: const Row(children: [
          SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(strokeWidth: 2)),
          SizedBox(width: 12),
          Text('جاري تحميل المدن...'),
        ]),
      );
    }

    final items =
        _cities.map((c) => DropdownMenuItem(value: c, child: Text(c))).toList();

    if (_error != null) {
      return DropdownButtonFormField<String?>(
        initialValue: _cities.contains(widget.value) ? widget.value : null,
        decoration: decoration,
        dropdownColor: AppTheme.darkCard,
        style: AppTextStyles.bodyMedium.copyWith(color: AppTheme.textWhite),
        items: items,
        onChanged: (v) => widget.onChanged(v),
        validator: widget.requiredField
            ? (v) => (v == null || (v).isEmpty) ? 'المدينة مطلوبة' : null
            : null,
      );
    }

    return DropdownButtonFormField<String?>(
      initialValue: _cities.contains(widget.value) ? widget.value : null,
      decoration: decoration,
      dropdownColor: AppTheme.darkCard,
      style: AppTextStyles.bodyMedium.copyWith(color: AppTheme.textWhite),
      hint: Text('اختر المدينة',
          style: AppTextStyles.bodyMedium.copyWith(color: AppTheme.textMuted)),
      items: items,
      onChanged: (v) => widget.onChanged(v),
      validator: widget.requiredField
          ? (v) => (v == null || (v).isEmpty) ? 'المدينة مطلوبة' : null
          : null,
    );
  }
}

// _DeleteConfirmationDialog يبقى كما هو...
class _DeleteConfirmationDialog extends StatelessWidget {
  final VoidCallback onConfirm;

  const _DeleteConfirmationDialog({required this.onConfirm});

  @override
  Widget build(BuildContext context) {
    // نفس الكود السابق
    return BackdropFilter(
      filter: ImageFilter.blur(sigmaX: 5, sigmaY: 5),
      child: Dialog(
        backgroundColor: Colors.transparent,
        child: Container(
          padding: const EdgeInsets.all(24),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: [
                AppTheme.darkCard.withOpacity(0.95),
                AppTheme.darkCard.withOpacity(0.85),
              ],
            ),
            borderRadius: BorderRadius.circular(20),
            border: Border.all(
              color: AppTheme.error.withOpacity(0.3),
              width: 1,
            ),
            boxShadow: [
              BoxShadow(
                color: AppTheme.error.withOpacity(0.2),
                blurRadius: 20,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Container(
                width: 80,
                height: 80,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [
                      AppTheme.error.withOpacity(0.2),
                      AppTheme.error.withOpacity(0.1),
                    ],
                  ),
                  shape: BoxShape.circle,
                ),
                child: Icon(
                  Icons.warning_rounded,
                  size: 48,
                  color: AppTheme.error,
                ),
              ),
              const SizedBox(height: 20),
              Text(
                'تأكيد الحذف',
                style: AppTextStyles.heading2.copyWith(
                  color: AppTheme.textWhite,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 12),
              Text(
                'هل أنت متأكد من حذف هذا العقار؟',
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppTheme.textLight,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 8),
              Text(
                'لا يمكن التراجع عن هذا الإجراء',
                style: AppTextStyles.bodySmall.copyWith(
                  color: AppTheme.error.withOpacity(0.8),
                  fontStyle: FontStyle.italic,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 30),
              Row(
                children: [
                  Expanded(
                    child: GestureDetector(
                      onTap: () => Navigator.pop(context),
                      child: Container(
                        padding: const EdgeInsets.symmetric(vertical: 14),
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            colors: [
                              AppTheme.darkSurface.withOpacity(0.5),
                              AppTheme.darkSurface.withOpacity(0.3),
                            ],
                          ),
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: AppTheme.darkBorder.withOpacity(0.3),
                            width: 1,
                          ),
                        ),
                        child: Center(
                          child: Text(
                            'إلغاء',
                            style: AppTextStyles.buttonMedium.copyWith(
                              color: AppTheme.textLight,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: GestureDetector(
                      onTap: onConfirm,
                      child: Container(
                        padding: const EdgeInsets.symmetric(vertical: 14),
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            colors: [
                              AppTheme.error,
                              AppTheme.error.withOpacity(0.8),
                            ],
                          ),
                          borderRadius: BorderRadius.circular(12),
                          boxShadow: [
                            BoxShadow(
                              color: AppTheme.error.withOpacity(0.3),
                              blurRadius: 8,
                              offset: const Offset(0, 4),
                            ),
                          ],
                        ),
                        child: Center(
                          child: Text(
                            'حذف',
                            style: AppTextStyles.buttonMedium.copyWith(
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
