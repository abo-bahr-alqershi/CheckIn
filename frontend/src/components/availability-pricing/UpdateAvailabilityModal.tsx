// src/components/availability-pricing/availability/UpdateAvailabilityModal.tsx

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Form, FormField } from '../ui/Form';
import { Input } from '../ui/Input';
import { Select } from '../ui/Select';
import { Textarea } from '../ui/Textarea';
import { Switch } from '../ui/Switch';
import { Button } from '../ui/Button';
import { DatePicker } from '../ui/DatePicker';
import { useAvailability } from '../../hooks/useAvailability';
import { AvailabilityStatus, UpdateUnitAvailabilityCommand } from '../../types/availability.types';
import { format } from 'date-fns';
import { Calendar, Clock, FileText, AlertCircle } from 'lucide-react';

interface UpdateAvailabilityModalProps {
  unitId: string;
  selectedDates: string[];
  year: number;
  month: number;
  onClose: () => void;
  onSuccess: () => void;
}

export const UpdateAvailabilityModal: React.FC<UpdateAvailabilityModalProps> = ({
  unitId,
  selectedDates,
  year,
  month,
  onClose,
  onSuccess
}) => {
  const { updateAvailability } = useAvailability(unitId, year, month);
  
  const [formData, setFormData] = useState<Partial<UpdateUnitAvailabilityCommand>>({
    unitId,
    startDate: selectedDates[0],
    endDate: selectedDates[selectedDates.length - 1],
    status: AvailabilityStatus.Blocked,
    reason: '',
    notes: '',
    overwriteExisting: true
  });

  const [isSubmitting, setIsSubmitting] = useState(false);

  const statusOptions = [
    { value: AvailabilityStatus.Available, label: 'متاح' },
    { value: AvailabilityStatus.Blocked, label: 'محظور' },
    { value: AvailabilityStatus.Maintenance, label: 'صيانة' },
    { value: AvailabilityStatus.Hold, label: 'معلق' }
  ];

  const reasonOptions = {
    [AvailabilityStatus.Blocked]: [
      'حجز خاص',
      'صيانة مجدولة',
      'إغلاق موسمي',
      'طلب المالك',
      'أخرى'
    ],
    [AvailabilityStatus.Maintenance]: [
      'صيانة دورية',
      'إصلاحات',
      'تجديدات',
      'تنظيف عميق',
      'أخرى'
    ],
    [AvailabilityStatus.Hold]: [
      'انتظار التأكيد',
      'حجز مؤقت',
      'قيد المراجعة',
      'أخرى'
    ]
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    try {
      await updateAvailability.mutateAsync(formData as UpdateUnitAvailabilityCommand);
      onSuccess();
    } catch (error) {
      console.error('Error updating availability:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal
      isOpen={true}
      onClose={onClose}
      title="تحديث الإتاحة"
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

        {/* Status Selection */}
        <FormField label="الحالة" required>
          <Select
            value={formData.status}
            onChange={(value) => setFormData({ ...formData, status: value, reason: '' })}
          >
            {statusOptions.map(option => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </Select>
        </FormField>

        {/* Reason Selection */}
        {formData.status && formData.status !== AvailabilityStatus.Available && (
          <FormField label="السبب">
            <Select
              value={formData.reason}
              onChange={(value) => setFormData({ ...formData, reason: value })}
            >
              <option value="">اختر السبب</option>
              {reasonOptions[formData.status]?.map(reason => (
                <option key={reason} value={reason}>
                  {reason}
                </option>
              ))}
            </Select>
          </FormField>
        )}

        {/* Custom Reason */}
        {formData.reason === 'أخرى' && (
          <FormField label="السبب المخصص">
            <Input
              placeholder="أدخل السبب..."
              onChange={(e) => setFormData({ ...formData, reason: e.target.value })}
            />
          </FormField>
        )}

        {/* Notes */}
        <FormField label="ملاحظات">
          <Textarea
            value={formData.notes}
            onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
            placeholder="أضف ملاحظات إضافية..."
            rows={3}
          />
        </FormField>

        {/* Overwrite Existing */}
        <FormField>
          <div className="flex items-center gap-3">
            <Switch
              checked={formData.overwriteExisting}
              onChange={(checked) => setFormData({ ...formData, overwriteExisting: checked })}
            />
            <div>
              <label className="text-sm font-medium text-gray-700">
                استبدال البيانات الموجودة
              </label>
              <p className="text-xs text-gray-500">
                سيتم استبدال أي إتاحة موجودة في هذه الفترة
              </p>
            </div>
          </div>
        </FormField>

        {/* Warning Message */}
        {formData.status === AvailabilityStatus.Blocked && (
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3">
            <div className="flex items-start gap-2">
              <AlertCircle className="h-4 w-4 text-yellow-600 mt-0.5" />
              <div className="text-sm text-yellow-800">
                <p className="font-medium mb-1">تنبيه</p>
                <p>حظر هذه الأيام سيمنع العملاء من حجزها</p>
              </div>
            </div>
          </div>
        )}

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
            تحديث الإتاحة
          </Button>
        </div>
      </Form>
    </Modal>
  );
};