import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/empty_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../admin_units/domain/entities/unit.dart';
import '../../domain/entities/search_result.dart';
import '../../domain/usecases/search_units_usecase.dart';
import 'package:bookn_cp_app/injection_container.dart' as di;
import '../widgets/search_header.dart';
import '../widgets/search_item_card.dart';
import '../widgets/simple_filter_bar.dart';

class UnitSearchPage extends StatefulWidget {
  final String? initialSearchTerm;
  final String? propertyId;
  final bool allowMultiSelect;
  final Function(List<Unit>)? onUnitsSelected;
  final Function(Unit)? onUnitSelected;

  const UnitSearchPage({
    super.key,
    this.initialSearchTerm,
    this.propertyId,
    this.allowMultiSelect = false,
    this.onUnitsSelected,
    this.onUnitSelected,
  });

  @override
  State<UnitSearchPage> createState() => _UnitSearchPageState();
}

class _UnitSearchPageState extends State<UnitSearchPage> {
  final SearchUnitsUseCase _searchUnitsUseCase = di.sl<SearchUnitsUseCase>();
  
  final TextEditingController _searchController = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  
  List<SearchResult> _units = [];
  List<String> _selectedIds = [];
  bool _isLoading = false;
  bool _hasError = false;
  String? _errorMessage;
  
  bool? _isAvailableFilter;
  String? _pricingMethodFilter;
  
  int _currentPage = 1;
  int _totalPages = 1;
  bool _isLoadingMore = false;

  @override
  void initState() {
    super.initState();
    _searchController.text = widget.initialSearchTerm ?? '';
    _scrollController.addListener(_onScroll);
    _loadUnits();
  }

  @override
  void dispose() {
    _searchController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.position.pixels >= 
        _scrollController.position.maxScrollExtent - 200) {
      if (!_isLoadingMore && _currentPage < _totalPages) {
        _loadMoreUnits();
      }
    }
  }

  Future<void> _loadUnits({bool isRefresh = false}) async {
    if (isRefresh) {
      _currentPage = 1;
      _units.clear();
    }

    setState(() {
      _isLoading = true;
      _hasError = false;
    });

    final result = await _searchUnitsUseCase(
      searchTerm: _searchController.text.isEmpty ? null : _searchController.text,
      propertyId: widget.propertyId,
      isAvailable: _isAvailableFilter,
      pageNumber: _currentPage,
      pageSize: 20,
    );

    result.fold(
      (failure) {
        setState(() {
          _hasError = true;
          _errorMessage = failure.message;
          _isLoading = false;
        });
      },
      (paginatedResult) {
        setState(() {
          _units = paginatedResult.items;
          _totalPages = paginatedResult.totalPages;
          _isLoading = false;
        });
      },
    );
  }

  Future<void> _loadMoreUnits() async {
    if (_isLoadingMore) return;

    setState(() {
      _isLoadingMore = true;
    });

    _currentPage++;
    
    final result = await _searchUnitsUseCase(
      searchTerm: _searchController.text.isEmpty ? null : _searchController.text,
      propertyId: widget.propertyId,
      isAvailable: _isAvailableFilter,
      pageNumber: _currentPage,
      pageSize: 20,
    );

    result.fold(
      (failure) {
        _currentPage--;
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(failure.message)),
        );
      },
      (paginatedResult) {
        setState(() {
          _units.addAll(paginatedResult.items);
        });
      },
    );

    setState(() {
      _isLoadingMore = false;
    });
  }

  void _toggleSelection(String id) {
    setState(() {
      if (widget.allowMultiSelect) {
        if (_selectedIds.contains(id)) {
          _selectedIds.remove(id);
        } else {
          _selectedIds.add(id);
        }
      } else {
        _selectedIds = [id];
      }
    });
  }

  void _confirmSelection() {
    final selectedUnits = _units
        .where((result) => _selectedIds.contains(result.id))
        .map((result) => result.item as Unit)
        .toList();

    if (widget.allowMultiSelect && widget.onUnitsSelected != null) {
      widget.onUnitsSelected!(selectedUnits);
    } else if (!widget.allowMultiSelect && 
               widget.onUnitSelected != null && 
               selectedUnits.isNotEmpty) {
      widget.onUnitSelected!(selectedUnits.first);
    }

    Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.darkBackground,
      body: SafeArea(
        child: Column(
          children: [
            // Header with Search
            SearchHeader(
              title: widget.allowMultiSelect ? 'اختر الوحدات' : 'اختر وحدة',
              searchHint: 'ابحث بالاسم أو الرقم...',
              searchController: _searchController,
              onSearchChanged: (value) {
                Future.delayed(const Duration(milliseconds: 500), () {
                  if (value == _searchController.text) {
                    _loadUnits(isRefresh: true);
                  }
                });
              },
              onClearSearch: () {
                _loadUnits(isRefresh: true);
              },
            ),
            
            // Filters
            SimpleFilterBar(
              filters: [
                FilterOption(
                  label: 'الكل',
                  isSelected: _isAvailableFilter == null,
                  onChanged: (selected) {
                    if (selected) {
                      setState(() => _isAvailableFilter = null);
                      _loadUnits(isRefresh: true);
                    }
                  },
                ),
                FilterOption(
                  label: 'متاح',
                  isSelected: _isAvailableFilter == true,
                  onChanged: (selected) {
                    setState(() => _isAvailableFilter = selected ? true : null);
                    _loadUnits(isRefresh: true);
                  },
                ),
                FilterOption(
                  label: 'غير متاح',
                  isSelected: _isAvailableFilter == false,
                  onChanged: (selected) {
                    setState(() => _isAvailableFilter = selected ? false : null);
                    _loadUnits(isRefresh: true);
                  },
                ),
              ],
              onClearFilters: () {
                setState(() {
                  _isAvailableFilter = null;
                  _pricingMethodFilter = null;
                });
                _loadUnits(isRefresh: true);
              },
            ),
            
            // Results
            Expanded(
              child: _buildContent(),
            ),
            
            // Action Buttons
            if (_selectedIds.isNotEmpty)
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: AppTheme.darkCard.withOpacity(0.5),
                  border: Border(
                    top: BorderSide(
                      color: AppTheme.darkBorder.withOpacity(0.3),
                      width: 1,
                    ),
                  ),
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: Text(
                        'تم اختيار ${_selectedIds.length} ${_selectedIds.length == 1 ? 'وحدة' : 'وحدات'}',
                        style: TextStyle(color: AppTheme.textWhite),
                      ),
                    ),
                    TextButton(
                      onPressed: () => setState(() => _selectedIds.clear()),
                      child: Text(
                        'إلغاء',
                        style: TextStyle(color: AppTheme.textMuted),
                      ),
                    ),
                    const SizedBox(width: 8),
                    ElevatedButton(
                      onPressed: _confirmSelection,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppTheme.primaryBlue,
                      ),
                      child: const Text('تأكيد'),
                    ),
                  ],
                ),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildContent() {
    if (_isLoading && _units.isEmpty) {
      return const LoadingWidget(
        message: 'جاري البحث عن الوحدات...',
      );
    }

    if (_hasError) {
      return CustomErrorWidget(
        message: _errorMessage,
        onRetry: () => _loadUnits(isRefresh: true),
      );
    }

    if (_units.isEmpty) {
      return const EmptyWidget(
        message: 'لا توجد نتائج',
      );
    }

    return RefreshIndicator(
      onRefresh: () => _loadUnits(isRefresh: true),
      child: ListView.builder(
        controller: _scrollController,
        padding: const EdgeInsets.only(bottom: 16),
        itemCount: _units.length + (_isLoadingMore ? 1 : 0),
        itemBuilder: (context, index) {
          if (index == _units.length) {
            return const Padding(
              padding: EdgeInsets.all(16),
              child: Center(
                child: CircularProgressIndicator(),
              ),
            );
          }

          final unit = _units[index];
          return SearchItemCard(
            item: unit,
            isSelected: _selectedIds.contains(unit.id),
            onTap: () => _toggleSelection(unit.id),
          );
        },
      ),
    );
  }
}