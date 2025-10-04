// src/hooks/usePricing.ts

import { useState, useCallback } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import pricingService from '../services/pricing.service';
import {
  UnitPricingDto,
  UpdateUnitPricingCommand,
  BulkUpdatePricingCommand,
  CopyPricingCommand,
  ApplySeasonalPricingCommand,
  PricingPeriodDto
} from '../types/pricing.types';
import { toast } from 'react-hot-toast';

export const usePricing = (unitId: string, year: number, month: number) => {
  const queryClient = useQueryClient();
  const queryKey = ['pricing', unitId, year, month];

  const { data, isLoading, error, refetch } = useQuery({
    queryKey,
    queryFn: () => pricingService.getMonthlyPricing(unitId, year, month),
    enabled: !!unitId && year > 0 && month > 0,
  });

  const updatePricing = useMutation({
    mutationFn: (command: UpdateUnitPricingCommand) =>
      pricingService.updatePricing(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: ['pricing', unitId, year, month - 1] });
      queryClient.invalidateQueries({ queryKey: ['pricing', unitId, year, month + 1] });
      toast.success('تم تحديث التسعير بنجاح');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'حدث خطأ في تحديث التسعير');
    },
  });

  const bulkUpdatePricing = useMutation({
    mutationFn: (command: BulkUpdatePricingCommand) =>
      pricingService.bulkUpdatePricing(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: ['pricing', unitId, year, month - 1] });
      queryClient.invalidateQueries({ queryKey: ['pricing', unitId, year, month + 1] });
      toast.success('تم تحديث التسعير بنجاح');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'حدث خطأ في تحديث التسعير');
    },
  });

  const copyPricing = useMutation({
    mutationFn: (command: CopyPricingCommand) =>
      pricingService.copyPricing(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pricing', unitId] });
      toast.success('تم نسخ التسعير بنجاح');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'حدث خطأ في نسخ التسعير');
    },
  });

  const applySeasonalPricing = useMutation({
    mutationFn: (command: ApplySeasonalPricingCommand) =>
      pricingService.applySeasonalPricing(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      toast.success('تم تطبيق التسعير الموسمي بنجاح');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'حدث خطأ في تطبيق التسعير الموسمي');
    },
  });

  const deletePricing = useMutation({
    mutationFn: (pricingId: string) =>
      pricingService.deletePricing(unitId, pricingId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      toast.success('تم حذف التسعير بنجاح');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'حدث خطأ في حذف التسعير');
    },
  });

  const applyPercentageChange = useCallback(
    (dates: string[], percentage: number) => {
      const periods: PricingPeriodDto[] = dates.map((date) => ({
        startDate: date,
        endDate: date,
        priceType: 'Custom',
        price: 0,
        currency: data?.currency || 'YER',
        tier: '1',
        percentageChange: percentage,
        overwriteExisting: true
      }));

      bulkUpdatePricing.mutate({
        unitId,
        periods,
        overwriteExisting: true
      });
    },
    [unitId, bulkUpdatePricing, data]
  );

  const applyWeekendPricing = useCallback(
    (startDate: string, endDate: string, priceIncrease: number) => {
      updatePricing.mutate({
        unitId,
        startDate,
        endDate,
        priceType: 'Weekend',
        price: 0,
        currency: data?.currency || 'YER',
        pricingTier: '2',
        percentageChange: priceIncrease,
        overwriteExisting: true
      });
    },
    [unitId, updatePricing, data]
  );

  return {
    data,
    isLoading,
    error,
    refetch,
    updatePricing,
    bulkUpdatePricing,
    copyPricing,
    applySeasonalPricing,
    deletePricing,
    applyPercentageChange,
    applyWeekendPricing,
  };
};

export const useSeasonalPricing = (unitId: string) => {
  const queryClient = useQueryClient();

  const { data, isLoading, error } = useQuery({
    queryKey: ['seasonal-pricing', unitId],
    queryFn: () => pricingService.getSeasonalPricing(unitId),
    enabled: !!unitId,
  });

  return {
    seasonalPricing: data?.seasons || [],
    statistics: data?.statistics,
    isLoading,
    error,
  };
};

export const usePricingBreakdown = () => {
  return useMutation({
    mutationFn: ({
      unitId,
      checkIn,
      checkOut,
    }: {
      unitId: string;
      checkIn: string;
      checkOut: string;
    }) => pricingService.getPricingBreakdown(unitId, checkIn, checkOut),
  });
};