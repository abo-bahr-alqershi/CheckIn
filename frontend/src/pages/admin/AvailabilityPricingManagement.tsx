// src/pages/availability-pricing/AvailabilityPricingManagement.tsx

import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '../../components/ui/Tabs';
import { AvailabilityCalendar } from '../../components/availability-pricing/AvailabilityCalendar';
import { PricingCalendar } from '../../components/availability-pricing/PricingCalendar';
import { Breadcrumb } from '../../components/ui/Breadcrumb';
import { Card } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { CalendarDays, DollarSign, Settings, ChevronRight, Home, Building } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import apiClient from '../../utils/api-client';
import { Loader } from '../../components/ui/Loader';
import { Alert } from '../../components/ui/Alert';

interface UnitDetails {
  id: string;
  name: string;
  type: string;
  propertyName: string;
  propertyId: string;
  maxAdults: number;
  maxChildren: number;
  basePrice: number | { amount: number; currency: string; exchangeRate?: number; formattedAmount?: string };
  currency: string;
}

export const AvailabilityPricingManagement: React.FC = () => {
  const { unitId } = useParams<{ unitId: string }>();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('availability');

  const { data: unitDetails, isLoading, error } = useQuery<UnitDetails>({
    queryKey: ['unit-details', unitId],
    queryFn: async () => {
      const response = await apiClient.get(`/api/admin/units/${unitId}`);
      return response.data.data;
    },
    enabled: !!unitId
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <Loader size="lg" />
      </div>
    );
  }

  if (error || !unitDetails) {
    return (
      <div className="p-6">
        <Alert variant="error">
          حدث خطأ في تحميل بيانات الوحدة
        </Alert>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white border-b">
        <div className="container mx-auto px-4 py-4">
          <Breadcrumb>
            <Breadcrumb.Item href="/admin" icon={<Home className="h-4 w-4" />}>
              لوحة التحكم
            </Breadcrumb.Item>
            <Breadcrumb.Item href="/admin/units" icon={<Building className="h-4 w-4" />}>
              الوحدات
            </Breadcrumb.Item>
            <Breadcrumb.Item active>
              إدارة {unitDetails.name}
            </Breadcrumb.Item>
          </Breadcrumb>
        </div>
      </div>

      {/* Unit Info Card */}
      <div className="container mx-auto px-4 py-6">
        <Card className="mb-6">
          <div className="p-6">
            <div className="flex items-start justify-between">
              <div>
                <h1 className="text-2xl font-bold text-gray-900 mb-2">
                  {unitDetails.name}
                </h1>
                <div className="flex items-center gap-4 text-sm text-gray-600">
                  <span className="bg-gray-100 px-3 py-1 rounded-full">
                    {unitDetails.type}
                  </span>
                  <span>
                    السعة: {unitDetails.maxAdults} بالغ، {unitDetails.maxChildren} طفل
                  </span>
                  <span>
                    السعر الأساسي: {
                      typeof unitDetails.basePrice === 'object' && unitDetails.basePrice !== null && 'amount' in unitDetails.basePrice
                        ? `${(unitDetails.basePrice as any).amount} ${(unitDetails.basePrice as any).currency}`
                        : `${unitDetails.basePrice} ${unitDetails.currency}`
                    }
                  </span>
                </div>
              </div>
              <Button
                variant="outline"
                size="sm"
                onClick={() => navigate(`/units/${unitId}/settings`)}
                icon={<Settings className="h-4 w-4" />}
              >
                الإعدادات
              </Button>
            </div>
          </div>
        </Card>

        {/* Main Content */}
        <Card>
          <Tabs value={activeTab} onValueChange={setActiveTab}>
            <div className="border-b">
              <TabsList className="w-full justify-start p-0">
                <TabsTrigger 
                  value="availability" 
                  className="flex items-center gap-2 px-6 py-4"
                >
                  <CalendarDays className="h-4 w-4" />
                  إدارة الإتاحة
                </TabsTrigger>
                <TabsTrigger 
                  value="pricing"
                  className="flex items-center gap-2 px-6 py-4"
                >
                  <DollarSign className="h-4 w-4" />
                  إدارة التسعير
                </TabsTrigger>
              </TabsList>
            </div>

            <div className="p-6">
              <TabsContent value="availability" className="mt-0">
                <AvailabilityCalendar
                  unitId={unitId!}
                  unitName={unitDetails.name}
                />
              </TabsContent>

              <TabsContent value="pricing" className="mt-0">
                <PricingCalendar
                  unitId={unitId!}
                  unitName={unitDetails.name}
                />
              </TabsContent>
            </div>
          </Tabs>
        </Card>
      </div>
    </div>
  );
};

export default AvailabilityPricingManagement;