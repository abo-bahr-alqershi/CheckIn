// src/components/availability-pricing/pricing/SeasonalPricingModal.tsx

import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '../ui/Tabs';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import { useSeasonalPricing, usePricing } from '../../hooks/usePricing';
import { ApplySeasonalPricingCommand, SeasonDto } from '../../types/pricing.types';
import { Calendar, TrendingUp, Plus, Edit, Trash } from 'lucide-react';
import { format } from 'date-fns';
import { Loader } from '../ui/Loader';
import { Input } from '../ui/Input';
import { DateRangePicker } from '../ui/DateRangePicker';

interface SeasonalPricingModalProps {
  unitId: string;
  onClose: () => void;
  onSuccess: () => void;
}

export const SeasonalPricingModal: React.FC<SeasonalPricingModalProps> = ({
  unitId,
  onClose,
  onSuccess
}) => {
  const { seasonalPricing, statistics, isLoading } = useSeasonalPricing(unitId);
  const { applySeasonalPricing } = usePricing(unitId, 0, 0);
  const [activeTab, setActiveTab] = useState('templates');
  const [selectedSeasons, setSelectedSeasons] = useState<string[]>([]);
  const [newSeason, setNewSeason] = useState<Partial<SeasonDto>>({
    name: '',
    type: 'High',
    price: 0,
    percentageChange: 0,
    priority: 1
  });

  const seasonTypes = [
    { value: 'Low', label: 'موسم منخفض', color: 'bg-blue-100 text-blue-800' },
    { value: 'Regular', label: 'موسم عادي', color: 'bg-gray-100 text-gray-800' },
    { value: 'High', label: 'موسم مرتفع', color: 'bg-orange-100 text-orange-800' },
    { value: 'Peak', label: 'موسم ذروة', color: 'bg-red-100 text-red-800' }
  ];

  const predefinedTemplates = [
    {
      id: 'summer',
      name: 'الصيف',
      seasons: [
        { name: 'بداية الصيف', type: 'High', startDate: '2024-06-01', endDate: '2024-06-30', percentageChange: 20 },
        { name: 'ذروة الصيف', type: 'Peak', startDate: '2024-07-01', endDate: '2024-08-15', percentageChange: 40 },
        { name: 'نهاية الصيف', type: 'High', startDate: '2024-08-16', endDate: '2024-08-31', percentageChange: 15 }
      ]
    },
    {
      id: 'winter',
      name: 'الشتاء',
      seasons: [
        { name: 'بداية الشتاء', type: 'Low', startDate: '2024-12-01', endDate: '2024-12-20', percentageChange: -20 },
        { name: 'عطلة رأس السنة', type: 'Peak', startDate: '2024-12-21', endDate: '2025-01-05', percentageChange: 50 },
        { name: 'منتصف الشتاء', type: 'Regular', startDate: '2025-01-06', endDate: '2025-02-28', percentageChange: 0 }
      ]
    }
  ];

  const handleApplyTemplate = async (template: any) => {
    try {
      await applySeasonalPricing.mutateAsync({
        unitId,
        seasons: template.seasons,
        currency: 'YER'
      });
      onSuccess();
    } catch (error) {
      console.error('Error applying template:', error);
    }
  };

  if (isLoading) {
    return (
      <Modal isOpen={true} onClose={onClose} title="التسعير الموسمي">
        <div className="flex items-center justify-center h-64">
          <Loader size="lg" />
        </div>
      </Modal>
    );
  }

  return (
    <Modal
      isOpen={true}
      onClose={onClose}
      title="التسعير الموسمي"
      size="lg"
    >
      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList className="grid grid-cols-3 w-full">
          <TabsTrigger value="templates">قوالب جاهزة</TabsTrigger>
          <TabsTrigger value="current">المواسم الحالية</TabsTrigger>
          <TabsTrigger value="custom">إنشاء موسم</TabsTrigger>
        </TabsList>

        {/* Templates Tab */}
        <TabsContent value="templates" className="space-y-4">
          <div className="grid gap-4">
            {predefinedTemplates.map((template) => (
              <Card key={template.id} className="p-4">
                <div className="flex items-start justify-between mb-3">
                  <h3 className="font-semibold text-lg">{template.name}</h3>
                  <Button
                    size="sm"
                    variant="primary"
                    onClick={() => handleApplyTemplate(template)}
                  >
                    تطبيق القالب
                  </Button>
                </div>
                <div className="space-y-2">
                  {template.seasons.map((season, index) => (
                    <div key={index} className="flex items-center justify-between text-sm">
                      <div className="flex items-center gap-2">
                        <span className={`px-2 py-1 rounded text-xs ${
                          seasonTypes.find(t => t.value === season.type)?.color
                        }`}>
                          {season.type}
                        </span>
                        <span>{season.name}</span>
                      </div>
                      <div className="flex items-center gap-4 text-gray-600">
                        <span>{format(new Date(season.startDate), 'dd/MM')} - {format(new Date(season.endDate), 'dd/MM')}</span>
                        <span className={season.percentageChange > 0 ? 'text-green-600' : 'text-red-600'}>
                          {season.percentageChange > 0 ? '+' : ''}{season.percentageChange}%
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              </Card>
            ))}
          </div>
        </TabsContent>

        {/* Current Seasons Tab */}
        <TabsContent value="current" className="space-y-4">
          {statistics && (
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-4">
              <Card className="p-3">
                <p className="text-xs text-gray-600">إجمالي المواسم</p>
                <p className="text-lg font-semibold">{statistics.totalSeasons}</p>
              </Card>
              <Card className="p-3">
                <p className="text-xs text-gray-600">مواسم نشطة</p>
                <p className="text-lg font-semibold text-green-600">{statistics.activeSeasons}</p>
              </Card>
              <Card className="p-3">
                <p className="text-xs text-gray-600">متوسط السعر</p>
                <p className="text-lg font-semibold">{statistics.averageSeasonalPrice}</p>
              </Card>
              <Card className="p-3">
                <p className="text-xs text-gray-600">الأيام المغطاة</p>
                <p className="text-lg font-semibold">{statistics.totalDaysCovered}</p>
              </Card>
            </div>
          )}

          <div className="space-y-3">
            {seasonalPricing.map((season) => (
              <Card key={season.id} className="p-4">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className={`px-3 py-1 rounded ${
                      seasonTypes.find(t => t.value === season.type)?.color
                    }`}>
                      {season.type}
                    </div>
                    <div>
                      <h4 className="font-medium">{season.name}</h4>
                      <p className="text-sm text-gray-600">
                        {format(new Date(season.startDate), 'dd/MM/yyyy')} - 
                        {format(new Date(season.endDate), 'dd/MM/yyyy')}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-4">
                    <div className="text-right">
                      <p className="font-semibold">{season.price} {season.currency}</p>
                      {season.percentageChange && (
                        <p className={`text-sm ${season.percentageChange > 0 ? 'text-green-600' : 'text-red-600'}`}>
                          {season.percentageChange > 0 ? '+' : ''}{season.percentageChange}%
                        </p>
                      )}
                    </div>
                    <div className="flex gap-1">
                      <Button variant="ghost" size="sm" icon={<Edit className="h-4 w-4" />} />
                      <Button variant="ghost" size="sm" icon={<Trash className="h-4 w-4" />} />
                    </div>
                  </div>
                </div>
              </Card>
            ))}
          </div>
        </TabsContent>

        {/* Custom Season Tab */}
        <TabsContent value="custom" className="space-y-4">
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                اسم الموسم
              </label>
              <Input
                value={newSeason.name}
                onChange={(e) => setNewSeason({ ...newSeason, name: e.target.value })}
                placeholder="مثال: موسم الصيف"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                نوع الموسم
              </label>
              <div className="grid grid-cols-4 gap-2">
                {seasonTypes.map((type) => (
                  <Button
                    key={type.value}
                    variant={newSeason.type === type.value ? 'primary' : 'outline'}
                    size="sm"
                    onClick={() => setNewSeason({ ...newSeason, type: type.value })}
                    className="text-xs"
                  >
                    {type.label}
                  </Button>
                ))}
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                فترة الموسم
              </label>
              <DateRangePicker
                startDate={newSeason.startDate || ''}
                endDate={newSeason.endDate || ''}
                onChange={(start, end) => setNewSeason({ ...newSeason, startDate: start, endDate: end })}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  السعر
                </label>
                <Input
                  type="number"
                  value={newSeason.price}
                  onChange={(e) => setNewSeason({ ...newSeason, price: parseFloat(e.target.value) })}
                  placeholder="0"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  نسبة التغيير
                </label>
                <Input
                  type="number"
                  value={newSeason.percentageChange}
                  onChange={(e) => setNewSeason({ ...newSeason, percentageChange: parseFloat(e.target.value) })}
                  placeholder="0"
                />
              </div>
            </div>

            <Button
              variant="primary"
              className="w-full"
              icon={<Plus className="h-4 w-4" />}
            >
              إضافة الموسم
            </Button>
          </div>
        </TabsContent>
      </Tabs>

      <div className="flex items-center justify-end gap-3 mt-6 pt-4 border-t">
        <Button variant="outline" onClick={onClose}>
          إغلاق
        </Button>
      </div>
    </Modal>
  );
};