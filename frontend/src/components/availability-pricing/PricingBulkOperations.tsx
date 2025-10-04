// src/components/availability-pricing/pricing/PricingBulkOperations.tsx

import React, { useState } from 'react';
import { X, Calendar, Copy, TrendingUp, Percent, DollarSign } from 'lucide-react';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '../ui/Tabs';
import { usePricing } from '../../hooks/usePricing';
import { 
  BulkUpdatePricingCommand, 
  CopyPricingCommand,
  PricingPeriodDto 
} from '../../types/pricing.types';
import { DateRangePicker } from '../ui/DateRangePicker';
import { Select } from '../ui/Select';
import { Input } from '../ui/Input';
import { Textarea } from '../ui/Textarea';
import { Switch } from '../ui/Switch';
import { Slider } from '../ui/Slider';
import { format, addDays, isWeekend } from 'date-fns';
import { toast } from 'react-hot-toast';

interface PricingBulkOperationsProps {
  unitId: string;
  selectedDates: string[];
  basePrice: number;
  currency: string;
  year: number;
  month: number;
  onClose: () => void;
  onSuccess: () => void;
}

export const PricingBulkOperations: React.FC<PricingBulkOperationsProps> = ({
  unitId,
  selectedDates,
  basePrice,
  currency,
  year,
  month,
  onClose,
  onSuccess
}) => {
  const { bulkUpdatePricing, copyPricing } = usePricing(unitId, year, month);
  const [activeTab, setActiveTab] = useState('bulk-update');

  // Bulk Update State
  const [bulkPriceType, setBulkPriceType] = useState('Custom');
  const [bulkPrice, setBulkPrice] = useState(basePrice);
  const [bulkPercentage, setBulkPercentage] = useState(0);
  const [bulkMode, setBulkMode] = useState<'fixed' | 'percentage'>('fixed');
  const [bulkDescription, setBulkDescription] = useState('');
  const [bulkOverwrite, setBulkOverwrite] = useState(true);

  // Copy State
  const [copySource, setCopySource] = useState({ start: '', end: '' });
  const [copyTarget, setCopyTarget] = useState({ start: '' });
  const [copyRepeat, setCopyRepeat] = useState(1);
  const [copyAdjustment, setCopyAdjustment] = useState({ type: 'none', value: 0 });

  // Dynamic Pricing State
  const [dynamicStrategy, setDynamicStrategy] = useState<'weekday' | 'occupancy' | 'season'>('weekday');
  const [weekdayPrices, setWeekdayPrices] = useState({
    monday: 0, tuesday: 0, wednesday: 0, thursday: 0,
    friday: 10, saturday: 20, sunday: 20
  });

  // Formula Pricing State
  const [formulaBase, setFormulaBase] = useState(basePrice);
  const [formulaFactors, setFormulaFactors] = useState({
    demandFactor: 1.0,
    seasonFactor: 1.0,
    competitionFactor: 1.0,
    minPrice: basePrice * 0.7,
    maxPrice: basePrice * 1.5
  });

  const handleBulkUpdate = async () => {
    const periods: PricingPeriodDto[] = selectedDates.map(date => ({
      startDate: date,
      endDate: date,
      priceType: bulkPriceType,
      price: bulkMode === 'fixed' ? bulkPrice : 0,
      currency,
      tier: '1',
      percentageChange: bulkMode === 'percentage' ? bulkPercentage : undefined,
      description: bulkDescription,
      overwriteExisting: bulkOverwrite
    }));

    try {
      await bulkUpdatePricing.mutateAsync({
        unitId,
        periods,
        overwriteExisting: bulkOverwrite
      });
      onSuccess();
    } catch (error) {
      console.error('Bulk update failed:', error);
    }
  };

  const handleCopy = async () => {
    if (!copySource.start || !copySource.end || !copyTarget.start) {
      toast.error('يرجى تحديد فترة المصدر والهدف');
      return;
    }

    try {
      await copyPricing.mutateAsync({
        unitId,
        sourceStartDate: copySource.start,
        sourceEndDate: copySource.end,
        targetStartDate: copyTarget.start,
        repeatCount: copyRepeat,
        adjustmentType: copyAdjustment.type,
        adjustmentValue: copyAdjustment.value,
        overwriteExisting: true
      });
      onSuccess();
    } catch (error) {
      console.error('Copy failed:', error);
    }
  };

  const handleDynamicPricing = async () => {
    const periods: PricingPeriodDto[] = [];

    if (dynamicStrategy === 'weekday') {
      selectedDates.forEach(date => {
        const dayOfWeek = new Date(date).getDay();
        const dayNames = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'];
        const dayName = dayNames[dayOfWeek] as keyof typeof weekdayPrices;
        const percentageChange = weekdayPrices[dayName];

        periods.push({
          startDate: date,
          endDate: date,
          priceType: isWeekend(new Date(date)) ? 'Weekend' : 'Custom',
          price: 0,
          currency,
          tier: '1',
          percentageChange,
          description: `تسعير ${isWeekend(new Date(date)) ? 'نهاية الأسبوع' : 'يوم عمل'}`,
          overwriteExisting: true
        });
      });
    }

    try {
      await bulkUpdatePricing.mutateAsync({
        unitId,
        periods,
        overwriteExisting: true
      });
      onSuccess();
    } catch (error) {
      console.error('Dynamic pricing failed:', error);
    }
  };

  const calculateFormulaPrice = () => {
    const calculatedPrice = formulaBase * 
      formulaFactors.demandFactor * 
      formulaFactors.seasonFactor * 
      formulaFactors.competitionFactor;
    
    return Math.min(
      Math.max(calculatedPrice, formulaFactors.minPrice),
      formulaFactors.maxPrice
    );
  };

  const handleFormulaPricing = async () => {
    const calculatedPrice = calculateFormulaPrice();
    
    const periods: PricingPeriodDto[] = selectedDates.map(date => ({
      startDate: date,
      endDate: date,
      priceType: 'Custom',
      price: calculatedPrice,
      currency,
      tier: '1',
      minPrice: formulaFactors.minPrice,
      maxPrice: formulaFactors.maxPrice,
      description: 'تسعير محسوب بالمعادلة',
      overwriteExisting: true
    }));

    try {
      await bulkUpdatePricing.mutateAsync({
        unitId,
        periods,
        overwriteExisting: true
      });
      onSuccess();
    } catch (error) {
      console.error('Formula pricing failed:', error);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <Card className="w-full max-w-3xl max-h-[90vh] overflow-hidden">
        <div className="flex items-center justify-between p-4 border-b">
          <h2 className="text-lg font-semibold">عمليات التسعير المتقدمة</h2>
          <Button
            variant="ghost"
            size="sm"
            onClick={onClose}
            icon={<X className="h-4 w-4" />}
          />
        </div>

        <div className="p-4 overflow-y-auto max-h-[calc(90vh-120px)]">
          <div className="bg-blue-50 rounded-lg p-3 mb-4">
            <p className="text-sm text-blue-800">
              سيتم تطبيق التحديث على {selectedDates.length} يوم محدد
            </p>
            <p className="text-xs text-blue-600 mt-1">
              السعر الأساسي الحالي: {basePrice} {currency}
            </p>
          </div>

          <Tabs value={activeTab} onValueChange={setActiveTab}>
            <TabsList className="grid grid-cols-4 w-full">
              <TabsTrigger value="bulk-update">تحديث جماعي</TabsTrigger>
              <TabsTrigger value="copy">نسخ</TabsTrigger>
              <TabsTrigger value="dynamic">ديناميكي</TabsTrigger>
              <TabsTrigger value="formula">معادلة</TabsTrigger>
            </TabsList>

            {/* Bulk Update Tab */}
            <TabsContent value="bulk-update" className="space-y-4 mt-4">
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    نوع التسعير
                  </label>
                  <Select
                    value={bulkPriceType}
                    onChange={setBulkPriceType}
                  >
                    <option value="Custom">مخصص</option>
                    <option value="Weekend">نهاية الأسبوع</option>
                    <option value="Holiday">عطلة رسمية</option>
                    <option value="Seasonal">موسمي</option>
                    <option value="Special">عرض خاص</option>
                  </Select>
                </div>

                <div className="flex gap-2">
                  <Button
                    type="button"
                    variant={bulkMode === 'fixed' ? 'primary' : 'outline'}
                    onClick={() => setBulkMode('fixed')}
                    className="flex-1"
                    icon={<DollarSign className="h-4 w-4" />}
                  >
                    سعر ثابت
                  </Button>
                  <Button
                    type="button"
                    variant={bulkMode === 'percentage' ? 'primary' : 'outline'}
                    onClick={() => setBulkMode('percentage')}
                    className="flex-1"
                    icon={<Percent className="h-4 w-4" />}
                  >
                    نسبة مئوية
                  </Button>
                </div>

                {bulkMode === 'fixed' ? (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      السعر
                    </label>
                    <div className="relative">
                      <Input
                        type="number"
                        value={bulkPrice}
                        onChange={(e) => setBulkPrice(parseFloat(e.target.value))}
                        min="0"
                        step="100"
                        className="pl-12"
                      />
                      <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500">
                        {currency}
                      </span>
                    </div>
                  </div>
                ) : (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      نسبة التغيير: {bulkPercentage > 0 ? '+' : ''}{bulkPercentage}%
                    </label>
                    <Slider
                      value={[bulkPercentage]}
                      onValueChange={(value) => setBulkPercentage(value[0])}
                      min={-50}
                      max={100}
                      step={5}
                      className="w-full"
                    />
                    <div className="flex justify-between text-xs text-gray-500 mt-1">
                      <span>-50%</span>
                      <span>0%</span>
                      <span>+100%</span>
                    </div>
                    <div className="bg-blue-50 rounded-lg p-3 mt-3">
                      <p className="text-sm text-blue-800">
                        السعر الجديد: {(basePrice + (basePrice * bulkPercentage / 100)).toFixed(0)} {currency}
                      </p>
                    </div>
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    وصف التحديث
                  </label>
                  <Textarea
                    value={bulkDescription}
                    onChange={(e) => setBulkDescription(e.target.value)}
                    placeholder="أضف وصف للتسعير..."
                    rows={2}
                  />
                </div>

                <div className="flex items-center gap-3">
                  <Switch
                    checked={bulkOverwrite}
                    onChange={setBulkOverwrite}
                  />
                  <label className="text-sm text-gray-700">
                    استبدال الأسعار الموجودة
                  </label>
                </div>

                <Button
                  variant="primary"
                  className="w-full"
                  onClick={handleBulkUpdate}
                  loading={bulkUpdatePricing.isPending}
                >
                  تطبيق التحديث
                </Button>
              </div>
            </TabsContent>

            {/* Copy Tab */}
            <TabsContent value="copy" className="space-y-4 mt-4">
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    الفترة المصدر
                  </label>
                  <DateRangePicker
                    startDate={copySource.start}
                    endDate={copySource.end}
                    onChange={(start, end) => setCopySource({ start, end })}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    تاريخ البداية للنسخ
                  </label>
                  <Input
                    type="date"
                    value={copyTarget.start}
                    onChange={(e) => setCopyTarget({ start: e.target.value })}
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
                    value={copyRepeat}
                    onChange={(e) => setCopyRepeat(parseInt(e.target.value))}
                  />
                  <p className="text-xs text-gray-500 mt-1">
                    سيتم نسخ الفترة {copyRepeat} مرة متتالية
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    تعديل الأسعار المنسوخة
                  </label>
                  <Select
                    value={copyAdjustment.type}
                    onChange={(value) => setCopyAdjustment({ ...copyAdjustment, type: value })}
                  >
                    <option value="none">بدون تعديل</option>
                    <option value="percentage">نسبة مئوية</option>
                    <option value="fixed">مبلغ ثابت</option>
                  </Select>
                  
                  {copyAdjustment.type !== 'none' && (
                    <div className="mt-2">
                      <Input
                        type="number"
                        value={copyAdjustment.value}
                        onChange={(e) => setCopyAdjustment({ ...copyAdjustment, value: parseFloat(e.target.value) })}
                        placeholder={copyAdjustment.type === 'percentage' ? 'النسبة المئوية' : 'المبلغ'}
                      />
                    </div>
                  )}
                </div>

                <Button
                  variant="primary"
                  className="w-full"
                  onClick={handleCopy}
                  loading={copyPricing.isPending}
                  icon={<Copy className="h-4 w-4" />}
                >
                  نسخ التسعير
                </Button>
              </div>
            </TabsContent>

            {/* Dynamic Pricing Tab */}
            <TabsContent value="dynamic" className="space-y-4 mt-4">
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    استراتيجية التسعير
                  </label>
                  <Select
                    value={dynamicStrategy}
                    onChange={(value) => setDynamicStrategy(value as any)}
                  >
                    <option value="weekday">حسب أيام الأسبوع</option>
                    <option value="occupancy">حسب معدل الإشغال</option>
                    <option value="season">حسب الموسم</option>
                  </Select>
                </div>

                {dynamicStrategy === 'weekday' && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-3">
                      نسبة التغيير لكل يوم
                    </label>
                    <div className="space-y-2">
                      {Object.entries(weekdayPrices).map(([day, value]) => (
                        <div key={day} className="flex items-center gap-3">
                          <span className="w-24 text-sm capitalize">
                            {day === 'monday' && 'الإثنين'}
                            {day === 'tuesday' && 'الثلاثاء'}
                            {day === 'wednesday' && 'الأربعاء'}
                            {day === 'thursday' && 'الخميس'}
                            {day === 'friday' && 'الجمعة'}
                            {day === 'saturday' && 'السبت'}
                            {day === 'sunday' && 'الأحد'}
                          </span>
                          <div className="flex-1">
                            <Slider
                              value={[value]}
                              onValueChange={(newValue) => 
                                setWeekdayPrices({ ...weekdayPrices, [day]: newValue[0] })
                              }
                              min={-30}
                              max={50}
                              step={5}
                            />
                          </div>
                          <span className={`text-sm w-12 text-left ${value > 0 ? 'text-green-600' : value < 0 ? 'text-red-600' : ''}`}>
                            {value > 0 ? '+' : ''}{value}%
                          </span>
                        </div>
                      ))}
                    </div>
                  </div>
                )}

                <Button
                  variant="primary"
                  className="w-full"
                  onClick={handleDynamicPricing}
                  icon={<TrendingUp className="h-4 w-4" />}
                >
                  تطبيق التسعير الديناميكي
                </Button>
              </div>
            </TabsContent>

            {/* Formula Tab */}
            <TabsContent value="formula" className="space-y-4 mt-4">
              <div className="space-y-4">
                <div className="bg-gray-50 rounded-lg p-4">
                  <p className="text-sm font-medium text-gray-700 mb-2">المعادلة:</p>
                  <code className="text-xs bg-white p-2 rounded block">
                    السعر = السعر الأساسي × معامل الطلب × معامل الموسم × معامل المنافسة
                  </code>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    السعر الأساسي
                  </label>
                  <Input
                    type="number"
                    value={formulaBase}
                    onChange={(e) => setFormulaBase(parseFloat(e.target.value))}
                    min="0"
                  />
                </div>

                <div className="space-y-3">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      معامل الطلب: {formulaFactors.demandFactor.toFixed(1)}
                    </label>
                    <Slider
                      value={[formulaFactors.demandFactor]}
                      onValueChange={(value) => 
                        setFormulaFactors({ ...formulaFactors, demandFactor: value[0] })
                      }
                      min={0.5}
                      max={2}
                      step={0.1}
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      معامل الموسم: {formulaFactors.seasonFactor.toFixed(1)}
                    </label>
                    <Slider
                      value={[formulaFactors.seasonFactor]}
                      onValueChange={(value) => 
                        setFormulaFactors({ ...formulaFactors, seasonFactor: value[0] })
                      }
                      min={0.5}
                      max={2}
                      step={0.1}
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      معامل المنافسة: {formulaFactors.competitionFactor.toFixed(1)}
                    </label>
                    <Slider
                      value={[formulaFactors.competitionFactor]}
                      onValueChange={(value) => 
                        setFormulaFactors({ ...formulaFactors, competitionFactor: value[0] })
                      }
                      min={0.5}
                      max={2}
                      step={0.1}
                    />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      الحد الأدنى
                    </label>
                    <Input
                      type="number"
                      value={formulaFactors.minPrice}
                      onChange={(e) => 
                        setFormulaFactors({ ...formulaFactors, minPrice: parseFloat(e.target.value) })
                      }
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      الحد الأقصى
                    </label>
                    <Input
                      type="number"
                      value={formulaFactors.maxPrice}
                      onChange={(e) => 
                        setFormulaFactors({ ...formulaFactors, maxPrice: parseFloat(e.target.value) })
                      }
                    />
                  </div>
                </div>

                <div className="bg-green-50 rounded-lg p-4">
                  <p className="text-sm font-medium text-green-800">
                    السعر المحسوب: {calculateFormulaPrice().toFixed(0)} {currency}
                  </p>
                </div>

                <Button
                  variant="primary"
                  className="w-full"
                  onClick={handleFormulaPricing}
                >
                  تطبيق المعادلة
                </Button>
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