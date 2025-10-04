// lib/features/admin_properties/presentation/widgets/property_filters_widget.dart

import 'package:bookn_cp_app/core/theme/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:bookn_cp_app/core/theme/app_text_styles.dart';
import '../bloc/property_types/property_types_bloc.dart';

class PropertyFiltersWidget extends StatefulWidget {
  final Function(Map<String, dynamic>) onFilterChanged;

  const PropertyFiltersWidget({
    super.key,
    required this.onFilterChanged,
  });

  @override
  State<PropertyFiltersWidget> createState() => _PropertyFiltersWidgetState();
}

class _PropertyFiltersWidgetState extends State<PropertyFiltersWidget> {
  String? _selectedPropertyTypeId;
  RangeValues _priceRange = const RangeValues(0, 1000);
  final List<int> _selectedStarRatings = [];
  bool? _isApproved;
  bool? _hasActiveBookings;
  final TextEditingController _searchController = TextEditingController();

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // عنوان الفلترات
          Padding(
            padding: const EdgeInsets.only(bottom: 16),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'فلترة العقارات',
                  style: AppTextStyles.displayMedium.copyWith(
                    color: AppTheme.textWhite,
                    fontWeight: FontWeight.w600,
                  ),
                ),
                // زر إعادة تعيين
                _buildResetButton(),
              ],
            ),
          ),

          Wrap(
            spacing: 12,
            runSpacing: 12,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [
              // حقل البحث
              SizedBox(width: 260, child: _buildSearchField()),

              // فلتر نوع العقار
              SizedBox(width: 220, child: _buildPropertyTypeFilter()),

              // فلتر نطاق السعر
              SizedBox(width: 280, child: _buildPriceRangeFilter()),

              // فلتر التقييم بالنجوم
              SizedBox(width: 220, child: _buildStarRatingFilter()),

              // فلتر حالة الاعتماد
              SizedBox(width: 200, child: _buildStatusFilter()),

              // فلتر الحجوزات النشطة
              SizedBox(width: 220, child: _buildHasActiveBookingsFilter()),

              // زر التطبيق
              _buildApplyButton(),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildSearchField() {
    return _buildFilterContainer(
      label: 'البحث',
      icon: Icons.search_rounded,
      child: TextField(
        controller: _searchController,
        style: AppTextStyles.bodyMedium.copyWith(
          color: AppTheme.textWhite,
          fontSize: 13,
        ),
        decoration: InputDecoration(
          hintText: 'اسم العقار، المدينة، أو العنوان...',
          hintStyle: AppTextStyles.bodyMedium.copyWith(
            color: AppTheme.textMuted.withValues(alpha: 0.6),
            fontSize: 13,
          ),
          border: InputBorder.none,
          contentPadding: const EdgeInsets.symmetric(horizontal: 8),
        ),
        onSubmitted: (_) => _applyFilters(),
      ),
    );
  }

  Widget _buildPropertyTypeFilter() {
    return BlocBuilder<PropertyTypesBloc, PropertyTypesState>(
      builder: (context, state) {
        if (state is PropertyTypesLoaded) {
          return _buildFilterContainer(
            label: 'نوع العقار',
            icon: Icons.home_work_rounded,
            child: DropdownButtonHideUnderline(
              child: DropdownButton<String>(
                value: _selectedPropertyTypeId,
                isExpanded: true,
                dropdownColor: AppTheme.darkCard,
                icon: Icon(
                  Icons.arrow_drop_down_rounded,
                  color: AppTheme.primaryBlue.withValues(alpha: 0.7),
                  size: 20,
                ),
                style: AppTextStyles.bodySmall.copyWith(
                  color: AppTheme.textWhite,
                ),
                hint: Text(
                  'اختر النوع',
                  style: AppTextStyles.bodySmall.copyWith(
                    color: AppTheme.textMuted.withValues(alpha: 0.6),
                  ),
                ),
                items: [
                  const DropdownMenuItem(
                    value: null,
                    child: Text('جميع الأنواع'),
                  ),
                  ...state.propertyTypes.map((type) {
                    return DropdownMenuItem(
                      value: type.id,
                      child: Text(type.name),
                    );
                  }),
                ],
                onChanged: (value) {
                  setState(() {
                    _selectedPropertyTypeId = value;
                  });
                },
              ),
            ),
          );
        }
        return const SizedBox.shrink();
      },
    );
  }

  Widget _buildPriceRangeFilter() {
    return _buildFilterContainer(
      label: 'نطاق السعر (ريال/ليلة)',
      icon: Icons.monetization_on_rounded,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'من: ${_priceRange.start.toInt()} ريال',
                  style: AppTextStyles.caption.copyWith(
                    color: AppTheme.textWhite.withValues(alpha: 0.8),
                    fontSize: 11,
                  ),
                ),
                Text(
                  'إلى: ${_priceRange.end.toInt()} ريال',
                  style: AppTextStyles.caption.copyWith(
                    color: AppTheme.textWhite.withValues(alpha: 0.8),
                    fontSize: 11,
                  ),
                ),
              ],
            ),
          ),
          SliderTheme(
            data: SliderTheme.of(context).copyWith(
              trackHeight: 4,
              thumbShape: const RoundSliderThumbShape(enabledThumbRadius: 6),
              overlayShape: const RoundSliderOverlayShape(overlayRadius: 14),
            ),
            child: RangeSlider(
              values: _priceRange,
              min: 0,
              max: 1000,
              divisions: 20,
              activeColor: AppTheme.primaryBlue,
              inactiveColor: AppTheme.darkBorder.withValues(alpha: 0.3),
              labels: RangeLabels(
                '${_priceRange.start.toInt()}',
                '${_priceRange.end.toInt()}',
              ),
              onChanged: (values) {
                setState(() {
                  _priceRange = values;
                });
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStarRatingFilter() {
    return _buildFilterContainer(
      label: 'التقييم',
      icon: Icons.star_rounded,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(bottom: 4),
            child: Text(
              _selectedStarRatings.isEmpty
                  ? 'اختر التقييمات المطلوبة'
                  : 'التقييمات: ${_selectedStarRatings.join(", ")} نجوم',
              style: AppTextStyles.caption.copyWith(
                color: AppTheme.textMuted.withValues(alpha: 0.8),
                fontSize: 11,
              ),
            ),
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: List.generate(5, (index) {
              final rating = index + 1;
              final isSelected = _selectedStarRatings.contains(rating);

              return Tooltip(
                message: '$rating نجوم',
                child: GestureDetector(
                  onTap: () {
                    setState(() {
                      if (isSelected) {
                        _selectedStarRatings.remove(rating);
                      } else {
                        _selectedStarRatings.add(rating);
                      }
                    });
                  },
                  child: Container(
                    padding: const EdgeInsets.all(4),
                    child: Column(
                      children: [
                        Icon(
                          Icons.star_rounded,
                          size: 20,
                          color: isSelected
                              ? AppTheme.warning
                              : AppTheme.textMuted.withValues(alpha: 0.3),
                        ),
                        Text(
                          '$rating',
                          style: AppTextStyles.caption.copyWith(
                            color: isSelected
                                ? AppTheme.warning
                                : AppTheme.textMuted.withValues(alpha: 0.5),
                            fontSize: 10,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              );
            }),
          ),
        ],
      ),
    );
  }

  Widget _buildStatusFilter() {
    return _buildFilterContainer(
      label: 'حالة الاعتماد',
      icon: Icons.verified_rounded,
      child: DropdownButtonHideUnderline(
        child: DropdownButton<bool?>(
          value: _isApproved,
          isExpanded: true,
          dropdownColor: AppTheme.darkCard,
          icon: Icon(
            Icons.arrow_drop_down_rounded,
            color: AppTheme.primaryBlue.withValues(alpha: 0.7),
            size: 20,
          ),
          style: AppTextStyles.bodySmall.copyWith(
            color: AppTheme.textWhite,
          ),
          hint: Text(
            'اختر الحالة',
            style: AppTextStyles.bodySmall.copyWith(
              color: AppTheme.textMuted.withValues(alpha: 0.6),
            ),
          ),
          items: [
            DropdownMenuItem(
              value: null,
              child: Row(
                children: [
                  Icon(Icons.all_inclusive,
                      size: 14, color: AppTheme.textMuted),
                  const SizedBox(width: 8),
                  const Text('جميع الحالات'),
                ],
              ),
            ),
            DropdownMenuItem(
              value: true,
              child: Row(
                children: [
                  Icon(Icons.check_circle, size: 14, color: AppTheme.success),
                  const SizedBox(width: 8),
                  const Text('معتمد'),
                ],
              ),
            ),
            DropdownMenuItem(
              value: false,
              child: Row(
                children: [
                  Icon(Icons.pending, size: 14, color: AppTheme.warning),
                  const SizedBox(width: 8),
                  const Text('قيد المراجعة'),
                ],
              ),
            ),
          ],
          onChanged: (value) {
            setState(() {
              _isApproved = value;
            });
          },
        ),
      ),
    );
  }

  Widget _buildHasActiveBookingsFilter() {
    return _buildFilterContainer(
      label: 'الحجوزات',
      icon: Icons.calendar_month_rounded,
      child: DropdownButtonHideUnderline(
        child: DropdownButton<bool?>(
          value: _hasActiveBookings,
          isExpanded: true,
          dropdownColor: AppTheme.darkCard,
          icon: Icon(
            Icons.arrow_drop_down_rounded,
            color: AppTheme.primaryBlue.withValues(alpha: 0.7),
            size: 20,
          ),
          style: AppTextStyles.bodySmall.copyWith(
            color: AppTheme.textWhite,
          ),
          hint: Text(
            'حالة الحجوزات',
            style: AppTextStyles.bodySmall.copyWith(
              color: AppTheme.textMuted.withValues(alpha: 0.6),
            ),
          ),
          items: [
            const DropdownMenuItem(
              value: null,
              child: Text('جميع العقارات'),
            ),
            DropdownMenuItem(
              value: true,
              child: Row(
                children: [
                  Container(
                    width: 8,
                    height: 8,
                    decoration: BoxDecoration(
                      color: AppTheme.success,
                      shape: BoxShape.circle,
                    ),
                  ),
                  const SizedBox(width: 8),
                  const Text('مع حجوزات نشطة'),
                ],
              ),
            ),
            DropdownMenuItem(
              value: false,
              child: Row(
                children: [
                  Container(
                    width: 8,
                    height: 8,
                    decoration: BoxDecoration(
                      color: AppTheme.textMuted,
                      shape: BoxShape.circle,
                    ),
                  ),
                  const SizedBox(width: 8),
                  const Text('بدون حجوزات'),
                ],
              ),
            ),
          ],
          onChanged: (value) {
            setState(() {
              _hasActiveBookings = value;
            });
          },
        ),
      ),
    );
  }

  // Container موحد للفلاتر مع label وicon
  Widget _buildFilterContainer({
    required String label,
    required IconData icon,
    required Widget child,
  }) {
    return Container(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppTheme.darkCard.withValues(alpha: 0.5),
            AppTheme.darkCard.withValues(alpha: 0.3),
          ],
        ),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppTheme.darkBorder.withValues(alpha: 0.3),
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header with label and icon
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  AppTheme.primaryBlue.withValues(alpha: 0.1),
                  AppTheme.primaryBlue.withValues(alpha: 0.05),
                ],
              ),
              borderRadius: const BorderRadius.only(
                topLeft: Radius.circular(11),
                topRight: Radius.circular(11),
              ),
            ),
            child: Row(
              children: [
                Icon(
                  icon,
                  size: 14,
                  color: AppTheme.primaryBlue.withValues(alpha: 0.8),
                ),
                const SizedBox(width: 6),
                Text(
                  label,
                  style: AppTextStyles.caption.copyWith(
                    color: AppTheme.textWhite.withValues(alpha: 0.9),
                    fontWeight: FontWeight.w500,
                    fontSize: 11,
                  ),
                ),
              ],
            ),
          ),
          // Content
          Padding(
            padding: const EdgeInsets.all(8),
            child: child,
          ),
        ],
      ),
    );
  }

  Widget _buildApplyButton() {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: _applyFilters,
        borderRadius: BorderRadius.circular(10),
        child: Ink(
          decoration: BoxDecoration(
            gradient: AppTheme.primaryGradient,
            borderRadius: BorderRadius.circular(10),
            boxShadow: [
              BoxShadow(
                color: AppTheme.primaryBlue.withValues(alpha: 0.3),
                blurRadius: 8,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Icon(
                  Icons.filter_list_rounded,
                  color: Colors.white,
                  size: 16,
                ),
                const SizedBox(width: 6),
                Text(
                  'تطبيق الفلاتر',
                  style: AppTextStyles.buttonSmall.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildResetButton() {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: _resetFilters,
        borderRadius: BorderRadius.circular(8),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
          decoration: BoxDecoration(
            border: Border.all(
              color: AppTheme.textMuted.withValues(alpha: 0.3),
            ),
            borderRadius: BorderRadius.circular(8),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                Icons.refresh_rounded,
                color: AppTheme.textMuted,
                size: 14,
              ),
              const SizedBox(width: 4),
              Text(
                'إعادة تعيين',
                style: AppTextStyles.caption.copyWith(
                  color: AppTheme.textMuted,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _resetFilters() {
    setState(() {
      _searchController.clear();
      _selectedPropertyTypeId = null;
      _priceRange = const RangeValues(0, 1000);
      _selectedStarRatings.clear();
      _isApproved = null;
      _hasActiveBookings = null;
    });

    // إرسال فلاتر فارغة
    widget.onFilterChanged({});
  }

  void _applyFilters() {
    widget.onFilterChanged({
      'searchTerm': _searchController.text.trim().isEmpty
          ? null
          : _searchController.text.trim(),
      'propertyTypeId': _selectedPropertyTypeId,
      'minPrice': _priceRange.start,
      'maxPrice': _priceRange.end,
      'starRatings': _selectedStarRatings,
      'isApproved': _isApproved,
      'hasActiveBookings': _hasActiveBookings,
    });
  }
}
