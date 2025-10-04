import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';
import 'dart:ui';
import '../../domain/entities/payment.dart';
import '../../domain/entities/refund.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/theme/app_text_styles.dart';
import '../../../../../../core/theme/app_dimensions.dart';
import '../../../../../../core/widgets/price_widget.dart';

class RefundDialog extends StatefulWidget {
  final Payment payment;
  final Function(Money amount, String reason) onRefund;

  const RefundDialog({
    super.key,
    required this.payment,
    required this.onRefund,
  });

  @override
  State<RefundDialog> createState() => _RefundDialogState();
}

class _RefundDialogState extends State<RefundDialog>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;

  RefundType _refundType = RefundType.full;
  final TextEditingController _amountController = TextEditingController();
  final TextEditingController _reasonController = TextEditingController();
  String? _selectedReason;

  final List<String> _commonReasons = [
    'طلب العميل',
    'خطأ في الدفع',
    'إلغاء الحجز',
    'خدمة غير مرضية',
    'خطأ فني',
    'أخرى',
  ];

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );

    _scaleAnimation = CurvedAnimation(
      parent: _animationController,
      curve: Curves.elasticOut,
    );

    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    _amountController.dispose();
    _reasonController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return ScaleTransition(
      scale: _scaleAnimation,
      child: Center(
        child: Container(
          margin: const EdgeInsets.symmetric(horizontal: 24),
          constraints: const BoxConstraints(maxWidth: 400),
          decoration: BoxDecoration(
            color: AppTheme.darkCard,
            borderRadius: BorderRadius.circular(24),
            boxShadow: [
              BoxShadow(
                color: AppTheme.shadowDark.withValues(alpha: 0.3),
                blurRadius: 20,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(24),
            child: BackdropFilter(
              filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  _buildHeader(),
                  _buildContent(),
                  _buildActions(),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: AppTheme.primaryGradient,
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: AppTheme.textWhite.withValues(alpha: 0.2),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(
              CupertinoIcons.arrow_counterclockwise_circle_fill,
              color: AppTheme.textWhite,
              size: 24,
            ),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'استرداد المبلغ',
                  style: AppTextStyles.heading2.copyWith(
                    color: AppTheme.textWhite,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  'معاملة #${widget.payment.transactionId}',
                  style: AppTextStyles.caption.copyWith(
                    color: AppTheme.textWhite.withValues(alpha: 0.8),
                  ),
                ),
              ],
            ),
          ),
          IconButton(
            onPressed: () => Navigator.pop(context),
            icon: Icon(
              CupertinoIcons.xmark_circle_fill,
              color: AppTheme.textWhite,
              size: 24,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildContent() {
    return Container(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Payment Info
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: AppTheme.darkBackground.withValues(alpha: 0.5),
              borderRadius: BorderRadius.circular(16),
              border: Border.all(
                color: AppTheme.darkBorder,
                width: 1,
              ),
            ),
            child: Column(
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'المبلغ الأصلي',
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: AppTheme.textMuted,
                      ),
                    ),
                    PriceWidget(
                      price: widget.payment.amount.amount,
                      currency: widget.payment.amount.currency,
                      displayType: PriceDisplayType.normal,
                    ),
                  ],
                ),
                if (widget.payment.refundedAmount != null &&
                    widget.payment.refundedAmount! > 0) ...[
                  const SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'المبلغ المسترد',
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: AppTheme.textMuted,
                        ),
                      ),
                      Text(
                        '${widget.payment.refundedAmount} ${widget.payment.amount.currency}',
                        style: AppTextStyles.bodyMedium.copyWith(
                          color: AppTheme.warning,
                        ),
                      ),
                    ],
                  ),
                ],
                const SizedBox(height: 8),
                Divider(color: AppTheme.darkBorder),
                const SizedBox(height: 8),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'المتاح للاسترداد',
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: AppTheme.textWhite,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    Text(
                      '${widget.payment.remainingRefundableAmount} ${widget.payment.amount.currency}',
                      style: AppTextStyles.bodyLarge.copyWith(
                        color: AppTheme.success,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),

          const SizedBox(height: 20),

          // Refund Type
          Text(
            'نوع الاسترداد',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppTheme.textWhite,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: _buildRefundTypeOption(
                  RefundType.full,
                  'استرداد كامل',
                  CupertinoIcons.arrow_counterclockwise_circle_fill,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _buildRefundTypeOption(
                  RefundType.partial,
                  'استرداد جزئي',
                  CupertinoIcons.arrow_counterclockwise_circle,
                ),
              ),
            ],
          ),

          // Amount Input (for partial refund)
          if (_refundType == RefundType.partial) ...[
            const SizedBox(height: 20),
            Text(
              'المبلغ المراد استرداده',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppTheme.textWhite,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),
            Container(
              decoration: BoxDecoration(
                color: AppTheme.darkBackground.withValues(alpha: 0.5),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(
                  color: AppTheme.darkBorder,
                  width: 1,
                ),
              ),
              child: TextField(
                controller: _amountController,
                keyboardType: TextInputType.number,
                style: AppTextStyles.bodyLarge.copyWith(
                  color: AppTheme.textWhite,
                ),
                decoration: InputDecoration(
                  hintText: '0.00',
                  hintStyle: AppTextStyles.bodyLarge.copyWith(
                    color: AppTheme.textMuted,
                  ),
                  prefixIcon: Icon(
                    CupertinoIcons.money_dollar,
                    color: AppTheme.textMuted,
                  ),
                  suffixText: widget.payment.amount.currency,
                  suffixStyle: AppTextStyles.bodyMedium.copyWith(
                    color: AppTheme.textMuted,
                  ),
                  border: InputBorder.none,
                  contentPadding: const EdgeInsets.all(16),
                ),
              ),
            ),
          ],

          const SizedBox(height: 20),

          // Reason Selection
          Text(
            'سبب الاسترداد',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppTheme.textWhite,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: _commonReasons.map((reason) {
              final isSelected = _selectedReason == reason;
              return GestureDetector(
                onTap: () {
                  setState(() {
                    _selectedReason = reason;
                    if (reason != 'أخرى') {
                      _reasonController.text = reason;
                    } else {
                      _reasonController.clear();
                    }
                  });
                },
                child: Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 8,
                  ),
                  decoration: BoxDecoration(
                    color: isSelected
                        ? AppTheme.primaryBlue.withValues(alpha: 0.2)
                        : AppTheme.darkBackground.withValues(alpha: 0.5),
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(
                      color: isSelected
                          ? AppTheme.primaryBlue
                          : AppTheme.darkBorder,
                      width: 1,
                    ),
                  ),
                  child: Text(
                    reason,
                    style: AppTextStyles.caption.copyWith(
                      color: isSelected
                          ? AppTheme.primaryBlue
                          : AppTheme.textWhite,
                      fontWeight:
                          isSelected ? FontWeight.bold : FontWeight.normal,
                    ),
                  ),
                ),
              );
            }).toList(),
          ),

          // Custom Reason Input
          if (_selectedReason == 'أخرى') ...[
            const SizedBox(height: 12),
            Container(
              decoration: BoxDecoration(
                color: AppTheme.darkBackground.withValues(alpha: 0.5),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(
                  color: AppTheme.darkBorder,
                  width: 1,
                ),
              ),
              child: TextField(
                controller: _reasonController,
                maxLines: 3,
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppTheme.textWhite,
                ),
                decoration: InputDecoration(
                  hintText: 'اكتب السبب هنا...',
                  hintStyle: AppTextStyles.bodyMedium.copyWith(
                    color: AppTheme.textMuted,
                  ),
                  border: InputBorder.none,
                  contentPadding: const EdgeInsets.all(16),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildRefundTypeOption(
    RefundType type,
    String label,
    IconData icon,
  ) {
    final isSelected = _refundType == type;
    return GestureDetector(
      onTap: () {
        setState(() {
          _refundType = type;
          if (type == RefundType.full) {
            _amountController.text =
                widget.payment.remainingRefundableAmount.toStringAsFixed(2);
          }
        });
      },
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          gradient: isSelected ? AppTheme.primaryGradient : null,
          color: isSelected
              ? null
              : AppTheme.darkBackground.withValues(alpha: 0.5),
          borderRadius: BorderRadius.circular(16),
          border: Border.all(
            color: isSelected ? Colors.transparent : AppTheme.darkBorder,
            width: 1,
          ),
        ),
        child: Column(
          children: [
            Icon(
              icon,
              color: AppTheme.textWhite,
              size: 24,
            ),
            const SizedBox(height: 8),
            Text(
              label,
              style: AppTextStyles.caption.copyWith(
                color: AppTheme.textWhite,
                fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActions() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: AppTheme.darkBackground.withValues(alpha: 0.3),
        border: Border(
          top: BorderSide(
            color: AppTheme.darkBorder,
            width: 1,
          ),
        ),
      ),
      child: Row(
        children: [
          Expanded(
            child: CupertinoButton(
              padding: const EdgeInsets.symmetric(vertical: 14),
              color: AppTheme.darkCard,
              borderRadius: BorderRadius.circular(12),
              onPressed: () => Navigator.pop(context),
              child: Text(
                'إلغاء',
                style: AppTextStyles.buttonMedium.copyWith(
                  color: AppTheme.textWhite,
                ),
              ),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            flex: 2,
            child: Container(
              decoration: BoxDecoration(
                gradient: AppTheme.primaryGradient,
                borderRadius: BorderRadius.circular(12),
              ),
              child: CupertinoButton(
                padding: const EdgeInsets.symmetric(vertical: 14),
                color: Colors.transparent,
                borderRadius: BorderRadius.circular(12),
                onPressed: _processRefund,
                child: Text(
                  'تأكيد الاسترداد',
                  style: AppTextStyles.buttonMedium.copyWith(
                    color: AppTheme.textWhite,
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _processRefund() {
    if (_reasonController.text.isEmpty && _selectedReason != 'أخرى') {
      _reasonController.text = _selectedReason ?? '';
    }

    if (_reasonController.text.isEmpty) {
      // Show error
      return;
    }

    double amount;
    if (_refundType == RefundType.full) {
      amount = widget.payment.remainingRefundableAmount;
    } else {
      amount = double.tryParse(_amountController.text) ?? 0;
      if (amount <= 0 || amount > widget.payment.remainingRefundableAmount) {
        // Show error
        return;
      }
    }

    final refundAmount = Money(
      amount: amount,
      currency: widget.payment.amount.currency,
      formattedAmount: '$amount ${widget.payment.amount.currency}',
    );

    widget.onRefund(refundAmount, _reasonController.text);
    Navigator.pop(context);
  }
}
