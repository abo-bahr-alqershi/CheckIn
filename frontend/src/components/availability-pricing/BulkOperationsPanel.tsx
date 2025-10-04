// src/components/availability-pricing/availability/BulkOperationsPanel.tsx

import React, { useState } from 'react';
import { X, Calendar, Copy, Download, Upload, RefreshCw } from 'lucide-react';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '../ui/Tabs';
import { useAvailability } from '../../hooks/useAvailability';
import { 
  BulkUpdateAvailabilityCommand, 
  CloneAvailabilityCommand,
  AvailabilityPeriodDto,
  AvailabilityStatus 
} from '../../types/availability.types';
import { DateRangePicker } from '../ui/DateRangePicker';
import { Select } from '../ui/Select';
import { Input } from '../ui/Input';
import { Textarea } from '../ui/Textarea';
import { Switch } from '../ui/Switch';
import { format } from 'date-fns';
import { toast } from 'react-hot-toast';

interface BulkOperationsPanelProps {
  unitId: string;
  selectedDates: string[];
  year: number;
  month: number;
  onClose: () => void;
  onSuccess: () => void;
}

export const BulkOperationsPanel: React.FC<BulkOperationsPanelProps> = ({
  unitId,
  selectedDates,
  year,
  month,
  onClose,
  onSuccess
}) => {
  console.log('BulkOperationsPanel - Unit ID:', unitId);
  console.log('BulkOperationsPanel - Selected Dates:', selectedDates);
  
  const { bulkUpdateAvailability, cloneAvailability } = useAvailability(unitId, year, month);
  const [activeTab, setActiveTab] = useState('bulk-update');

  // Bulk Update State
  const [bulkStatus, setBulkStatus] = useState<string>('Blocked');
  const [bulkReason, setBulkReason] = useState('');
  const [bulkNotes, setBulkNotes] = useState('');
  const [bulkOverwrite, setBulkOverwrite] = useState(true);

  // Clone State
  const [cloneSource, setCloneSource] = useState({ start: '', end: '' });
  const [cloneTarget, setCloneTarget] = useState({ start: '', end: '' });
  const [cloneRepeat, setCloneRepeat] = useState(1);

  // Pattern State
  const [patternType, setPatternType] = useState<'weekly' | 'monthly'>('weekly');
  const [patternDays, setPatternDays] = useState<number[]>([]);
  const [patternMonths, setPatternMonths] = useState<number[]>([]);

  const handleBulkUpdate = async () => {
    // Validate required fields
    if (!bulkStatus) {
      toast.error('يرجى تحديد الحالة');
      return;
    }

    if (selectedDates.length === 0) {
      toast.error('يرجى تحديد التواريخ المراد تحديثها');
      return;
    }

    // Ensure dates are in the correct format (YYYY-MM-DD)
    const periods: AvailabilityPeriodDto[] = selectedDates.map(date => {
      // Ensure date is in YYYY-MM-DD format
      const formattedDate = date.includes('T') ? date.split('T')[0] : date;
      
      return {
        startDate: formattedDate,
        endDate: formattedDate,
        status: bulkStatus,
        reason: bulkReason || undefined,
        notes: bulkNotes || undefined,
        overwriteExisting: bulkOverwrite
      };
    });

    try {
      const command = {
        unitId,
        periods,
        overwriteExisting: bulkOverwrite
      };

      console.log('=== BULK UPDATE DEBUG INFO ===');
      console.log('Unit ID:', unitId);
      console.log('Selected Dates Count:', selectedDates.length);
      console.log('Selected Dates:', selectedDates);
      console.log('Bulk Status:', bulkStatus);
      console.log('Bulk Reason:', bulkReason);
      console.log('Bulk Notes:', bulkNotes);
      console.log('Overwrite Existing:', bulkOverwrite);
      console.log('Periods:', periods);
      console.log('Command Object:', command);
      console.log('Command JSON:', JSON.stringify(command, null, 2));
      console.log('=== END DEBUG INFO ===');

      await bulkUpdateAvailability.mutateAsync(command);
      
      toast.success('تم تحديث الإتاحة بنجاح');
      onSuccess();
    } catch (error: any) {
      console.error('Bulk update failed:', error);
      console.error('Error response:', error.response);
      console.error('Error data:', error.response?.data);
      
      // Show detailed error message with better error handling
      let errorMessage = 'حدث خطأ في تحديث الإتاحة';
      
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.response?.data?.errors?.[0]) {
        errorMessage = error.response.data.errors[0];
      } else if (error.message) {
        errorMessage = error.message;
      } else if (typeof error === 'string') {
        errorMessage = error;
      } else if (error && typeof error === 'object') {
        // Try to extract meaningful error information
        if (error.success === false && error.message) {
          errorMessage = error.message;
        } else if (error.errors && Array.isArray(error.errors) && error.errors.length > 0) {
          errorMessage = error.errors[0];
        }
      }
      
      console.error('Final error message:', errorMessage);
      toast.error(errorMessage);
    }
  };

  const handleClone = async () => {
    if (!cloneSource.start || !cloneSource.end || !cloneTarget.start) {
      toast.error('يرجى تحديد فترة المصدر والهدف');
      return;
    }

    try {
      console.log('Sending clone command:', {
        unitId,
        sourceStartDate: cloneSource.start,
        sourceEndDate: cloneSource.end,
        targetStartDate: cloneTarget.start,
        repeatCount: cloneRepeat
      });

      await cloneAvailability.mutateAsync({
        unitId,
        sourceStartDate: cloneSource.start,
        sourceEndDate: cloneSource.end,
        targetStartDate: cloneTarget.start,
        repeatCount: cloneRepeat
      });
      
      toast.success('تم نسخ الإتاحة بنجاح');
      onSuccess();
    } catch (error: any) {
      console.error('Clone failed:', error);
      
      // Show detailed error message with better error handling
      let errorMessage = 'حدث خطأ في نسخ الإتاحة';
      
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.response?.data?.errors?.[0]) {
        errorMessage = error.response.data.errors[0];
      } else if (error.message) {
        errorMessage = error.message;
      } else if (typeof error === 'string') {
        errorMessage = error;
      } else if (error && typeof error === 'object') {
        // Try to extract meaningful error information
        if (error.success === false && error.message) {
          errorMessage = error.message;
        } else if (error.errors && Array.isArray(error.errors) && error.errors.length > 0) {
          errorMessage = error.errors[0];
        }
      }
      
      console.error('Final error message:', errorMessage);
      toast.error(errorMessage);
    }
  };

  const handlePatternApply = async () => {
    // Generate dates based on pattern
    const patternDates: string[] = [];
    // Implementation for pattern generation
    
    if (patternDates.length === 0) {
      toast.error('لم يتم إنشاء أي تواريخ من النمط المحدد');
      return;
    }

    const periods: AvailabilityPeriodDto[] = patternDates.map(date => ({
      startDate: date,
      endDate: date,
      status: bulkStatus,
      reason: 'تطبيق نمط',
      notes: bulkNotes,
      overwriteExisting: bulkOverwrite
    }));

    try {
      const command = {
        unitId,
        periods,
        overwriteExisting: bulkOverwrite
      };

      console.log('Pattern apply command:', command);
      await bulkUpdateAvailability.mutateAsync(command);
      onSuccess();
    } catch (error: any) {
      console.error('Pattern apply failed:', error);
      
      // Show detailed error message with better error handling
      let errorMessage = 'حدث خطأ في تطبيق النمط';
      
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.response?.data?.errors?.[0]) {
        errorMessage = error.response.data.errors[0];
      } else if (error.message) {
        errorMessage = error.message;
      } else if (typeof error === 'string') {
        errorMessage = error;
      } else if (error && typeof error === 'object') {
        // Try to extract meaningful error information
        if (error.success === false && error.message) {
          errorMessage = error.message;
        } else if (error.errors && Array.isArray(error.errors) && error.errors.length > 0) {
          errorMessage = error.errors[0];
        }
      }
      
      console.error('Final error message:', errorMessage);
      toast.error(errorMessage);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <Card className="w-full max-w-2xl max-h-[90vh] overflow-hidden">
        <div className="flex items-center justify-between p-4 border-b">
          <h2 className="text-lg font-semibold">عمليات متعددة</h2>
          <Button
            variant="ghost"
            size="sm"
            onClick={onClose}
            icon={<X className="h-4 w-4" />}
          />
        </div>

        <div className="p-4 overflow-y-auto max-h-[calc(90vh-120px)]">
          <Tabs value={activeTab} onValueChange={setActiveTab}>
            <TabsList className="grid grid-cols-4 w-full">
              <TabsTrigger value="bulk-update">تحديث جماعي</TabsTrigger>
              <TabsTrigger value="clone">نسخ</TabsTrigger>
              <TabsTrigger value="pattern">نمط</TabsTrigger>
              <TabsTrigger value="import">استيراد</TabsTrigger>
            </TabsList>

            {/* Bulk Update Tab */}
            <TabsContent value="bulk-update" className="space-y-4">
              <div className="bg-blue-50 rounded-lg p-3">
                <p className="text-sm text-blue-800">
                  سيتم تطبيق التحديث على {selectedDates.length} يوم محدد
                </p>
              </div>

              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    الحالة
                  </label>
                  <Select
                    value={bulkStatus}
                    onChange={(value) => setBulkStatus(value)}
                  >
                    <option value="Available">متاح</option>
                    <option value="Blocked">محظور</option>
                    <option value="Maintenance">صيانة</option>
                    <option value="Hold">معلق</option>
                  </Select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    السبب
                  </label>
                  <Input
                    value={bulkReason}
                    onChange={(e) => setBulkReason(e.target.value)}
                    placeholder="أدخل سبب التحديث..."
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    ملاحظات
                  </label>
                  <Textarea
                    value={bulkNotes}
                    onChange={(e) => setBulkNotes(e.target.value)}
                    placeholder="ملاحظات إضافية..."
                    rows={3}
                  />
                </div>

                <div className="flex items-center gap-3">
                  <Switch
                    checked={bulkOverwrite}
                    onChange={setBulkOverwrite}
                  />
                  <label className="text-sm text-gray-700">
                    استبدال البيانات الموجودة
                  </label>
                </div>

                <Button
                  variant="primary"
                  className="w-full"
                  onClick={handleBulkUpdate}
                  loading={bulkUpdateAvailability.isPending}
                >
                  تطبيق التحديث
                </Button>
              </div>
            </TabsContent>

            {/* Clone Tab */}
            <TabsContent value="clone" className="space-y-4">
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    الفترة المصدر
                  </label>
                  <DateRangePicker
                    startDate={cloneSource.start}
                    endDate={cloneSource.end}
                    onChange={(start, end) => setCloneSource({ start, end })}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    تاريخ البداية للنسخ
                  </label>
                  <Input
                    type="date"
                    value={cloneTarget.start}
                    onChange={(e) => setCloneTarget({ ...cloneTarget, start: e.target.value })}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    عدد مرات التكرار
                  </label>
                  <Input
                    type="number"
                    min="1"
                    max="12"
                    value={cloneRepeat}
                    onChange={(e) => setCloneRepeat(parseInt(e.target.value))}
                  />
                </div>

                <Button
                  variant="primary"
                  className="w-full"
                  onClick={handleClone}
                  loading={cloneAvailability.isPending}
                  icon={<Copy className="h-4 w-4" />}
                >
                  نسخ الإتاحة
                </Button>
              </div>
            </TabsContent>

            {/* Pattern Tab */}
            <TabsContent value="pattern" className="space-y-4">
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    نوع النمط
                  </label>
                  <Select
                    value={patternType}
                    onChange={(value) => setPatternType(value as 'weekly' | 'monthly')}
                  >
                    <option value="weekly">أسبوعي</option>
                    <option value="monthly">شهري</option>
                  </Select>
                </div>

                {patternType === 'weekly' && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      أيام الأسبوع
                    </label>
                    <div className="flex gap-2 flex-wrap">
                      {['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'].map((day, index) => (
                        <Button
                          key={index}
                          variant={patternDays.includes(index) ? 'primary' : 'outline'}
                          size="sm"
                          onClick={() => {
                            if (patternDays.includes(index)) {
                              setPatternDays(patternDays.filter(d => d !== index));
                            } else {
                              setPatternDays([...patternDays, index]);
                            }
                          }}
                        >
                          {day}
                        </Button>
                      ))}
                    </div>
                  </div>
                )}

                <Button
                  variant="primary"
                  className="w-full"
                  onClick={handlePatternApply}
                  icon={<RefreshCw className="h-4 w-4" />}
                >
                  تطبيق النمط
                </Button>
              </div>
            </TabsContent>

            {/* Import Tab */}
            <TabsContent value="import" className="space-y-4">
              <div className="border-2 border-dashed border-gray-300 rounded-lg p-8">
                <div className="text-center">
                  <Upload className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                  <p className="text-sm text-gray-600 mb-2">
                    اسحب وأفلت ملف CSV أو Excel هنا
                  </p>
                  <Button variant="outline" size="sm">
                    اختر ملف
                  </Button>
                </div>
              </div>

              <div className="bg-gray-50 rounded-lg p-3">
                <p className="text-sm text-gray-600">
                  صيغة الملف المطلوبة: التاريخ، الحالة، السبب، الملاحظات
                </p>
              </div>
            </TabsContent>
          </Tabs>
        </div>

        <div className="flex items-center justify-end gap-3 p-4 border-t">
          <Button variant="outline" onClick={onClose}>
            إلغاء
          </Button>
        </div>
      </Card>
    </div>
  );
};