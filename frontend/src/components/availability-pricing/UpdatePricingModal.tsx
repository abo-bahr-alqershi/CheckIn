// src/components/availability-pricing/pricing/UpdatePricingModal.tsx

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Form, FormField } from '../ui/Form';
import { Input } from '../ui/Input';
import { Select } from '../ui/Select';
import { Textarea } from '../ui/Textarea';
import { Switch } from '../ui/Switch';
import { Button } from '../ui/Button';
import { Slider } from '../ui/Slider';
import { usePricing } from '../../hooks/usePricing';
import { UpdateUnitPricingCommand } from '../../types/pricing.types';
import { format } from 'date-fns';
import { Calendar, DollarSign, Percent, AlertCircle } from 'lucide-react';

interface UpdatePricingModalProps {
  unitId: string;
  selectedDates: string[];
  basePrice: number;
  currency: string;
  year: number;
  month: number;
  onClose: () => void;
  onSuccess: () => void;
}

export const UpdatePricingModal: React.FC<UpdatePricingModalProps> = ({
  unitId,
  selectedDates,
  basePrice,
  currency,
  year,
  month,
  onClose,
  onSuccess
}) => {
  const { updatePricing } = usePricing(unitId, year, month);
  
  const [priceMode, setPriceMode] = useState<'fixed' | 'percentage'>('fixed');
  const [formData, setFormData] = useState<Partial<UpdateUnitPricingCommand>>({
    unitId,
    startDate: selectedDates[0],
    endDate: selectedDates[selectedDates.length - 1],
    priceType: 'Custom',
    price: basePrice,
    currency,
    pricingTier: '1',
    percentageChange: 0,
    description: '',
    overwriteExisting: true
  });

  const [isSubmitting, setIsSubmitting] = useState(false);

  const priceTypes = [
    { value: 'Custom', label: 'سعر مخصص' },
    { value: 'Weekend', label: 'نهاية الأسبوع' },
    { value: 'Holiday', label: 'عطلة رسمية' },
    { value: 'Seasonal', label: 'موسمي' },
    { value: 'Special', label: 'عرض خاص' }
  ];

  const pricingTiers = [
    { value: '1', label: 'عادي' },
    { value: '2', label: 'مميز' },
    { value: '3', label: 'VIP' }
  ];

  const calculateNewPrice = () => {
    if (priceMode === 'percentage' && formData.percentageChange) {
      return basePrice + (basePrice * formData.percentageChange / 100);
    }
    return formData.price || basePrice;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    try {
      const command: UpdateUnitPricingCommand = {
        ...formData as UpdateUnitPricingCommand,
        price: priceMode === 'percentage' ? 0 : formData.price!,
        percentageChange: priceMode === 'percentage' ? formData.percentageChange : undefined
      };
      
      await updatePricing.mutateAsync(command);
      onSuccess();
    } catch (error) {
      console.error('Error updating pricing:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal
      isOpen={true}
      onClose={onClose}
      title="تحديث التسعير"
      size="md"
    >
      <Form onSubmit={handleSubmit} className="space-y-4">
        {/* Date Range Display */}
        <div className="bg-gray-50 rounded-lg p-4">
          <div className="flex items-center gap-2 mb-2">
            <Calendar className="h-4 w-4 text-gray-500" />
            <span className="text-sm font-medium text-gray-700">الفترة المحددة</span>
          </div>
          <div className="text-sm text-gray-600">
            {selectedDates.length === 1 ? (
              <span>{format(new Date(selectedDates[0]), 'dd/MM/yyyy')}</span>
            ) : (
              <span>
                من {format(new Date(selectedDates[0]), 'dd/MM/yyyy')} 
                إلى {format(new Date(selectedDates[selectedDates.length - 1]), 'dd/MM/yyyy')}
                <span className="mr-2 text-gray-500">({selectedDates.length} يوم)</span>
              </span>
            )}
          </div>
        </div>

        {/* Price Type */}
        <FormField label="نوع التسعير" required>
          <Select
            value={formData.priceType}
            onChange={(value) => setFormData({ ...formData, priceType: value })}
          >
            {priceTypes.map(type => (
              <option key={type.value} value={type.value}>
                {type.label}
              </option>
            ))}
          </Select>
        </FormField>

        {/* Pricing Mode Toggle */}
        <div className="flex gap-2">
          <Button
            type="button"
            variant={priceMode === 'fixed' ? 'primary' : 'outline'}
            onClick={() => setPriceMode('fixed')}
            className="flex-1"
            icon={<DollarSign className="h-4 w-4" />}
          >
            سعر ثابت
          </Button>
          <Button
            type="button"
            variant={priceMode === 'percentage' ? 'primary' : 'outline'}
            onClick={() => setPriceMode('percentage')}
            className="flex-1"
            icon={<Percent className="h-4 w-4" />}
          >
            نسبة مئوية
          </Button>
        </div>

        {/* Price Input */}
        {priceMode === 'fixed' ? (
          <FormField label="السعر" required>
            <div className="relative">
              <Input
                type="number"
                value={formData.price}
                onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) })}
                min="0"
                step="100"
                className="pl-12"
              />
              <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500">
                {currency}
              </span>
            </div>
            <p className="text-xs text-gray-500 mt-1">
              السعر الأساسي: {basePrice} {currency}
            </p>
          </FormField>
        ) : (
          <FormField label="نسبة التغيير">
            <div className="space-y-3">
              <Slider
                value={[formData.percentageChange || 0]}
                onValueChange={(value) => setFormData({ ...formData, percentageChange: value[0] })}
                min={-50}
                max={100}
                step={5}
                className="w-full"
              />
              <div className="flex justify-between text-sm">
                <span className="text-gray-500">-50%</span>
                <span className="font-medium text-lg">
                  {formData.percentageChange! > 0 ? '+' : ''}{formData.percentageChange}%
                </span>
                <span className="text-gray-500">+100%</span>
              </div>
              <div className="bg-blue-50 rounded-lg p-3">
                <p className="text-sm text-blue-800">
                  السعر الجديد: {calculateNewPrice().toFixed(0)} {currency}
                </p>
              </div>
            </div>
          </FormField>
        )}

        {/* Pricing Tier */}
        <FormField label="فئة التسعير">
          <Select
            value={formData.pricingTier}
            onChange={(value) => setFormData({ ...formData, pricingTier: value })}
          >
            {pricingTiers.map(tier => (
              <option key={tier.value} value={tier.value}>
                {tier.label}
              </option>
            ))}
          </Select>
        </FormField>

        {/* Description */}
        <FormField label="وصف التسعير">
          <Textarea
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            placeholder="مثال: عرض خاص لنهاية الأسبوع..."
            rows={3}
          />
        </FormField>

        {/* Min/Max Price */}
        <div className="grid grid-cols-2 gap-4">
          <FormField label="الحد الأدنى للسعر">
            <Input
              type="number"
              value={formData.minPrice || ''}
              onChange={(e) => setFormData({ ...formData, minPrice: parseFloat(e.target.value) || undefined })}
              placeholder="اختياري"
            />
          </FormField>
          <FormField label="الحد الأقصى للسعر">
            <Input
              type="number"
              value={formData.maxPrice || ''}
              onChange={(e) => setFormData({ ...formData, maxPrice: parseFloat(e.target.value) || undefined })}
              placeholder="اختياري"
            />
          </FormField>
        </div>

        {/* Overwrite Existing */}
        <FormField>
          <div className="flex items-center gap-3">
            <Switch
              checked={formData.overwriteExisting}
              onChange={(checked) => setFormData({ ...formData, overwriteExisting: checked })}
            />
            <div>
              <label className="text-sm font-medium text-gray-700">
                استبدال الأسعار الموجودة
              </label>
              <p className="text-xs text-gray-500">
                سيتم استبدال أي تسعير موجود في هذه الفترة
              </p>
            </div>
          </div>
        </FormField>

        {/* Info Message */}
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
          <div className="flex items-start gap-2">
            <AlertCircle className="h-4 w-4 text-blue-600 mt-0.5" />
            <div className="text-sm text-blue-800">
              <p>سيتم تطبيق هذا التسعير على جميع الأيام المحددة</p>
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center justify-end gap-3 pt-4 border-t">
          <Button
            type="button"
            variant="outline"
            onClick={onClose}
            disabled={isSubmitting}
          >
            إلغاء
          </Button>
          <Button
            type="submit"
            variant="primary"
            loading={isSubmitting}
          >
            تحديث التسعير
          </Button>
        </div>
      </Form>
    </Modal>
  );
};