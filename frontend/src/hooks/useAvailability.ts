// src/hooks/useAvailability.ts

import { useState, useCallback } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import availabilityService from '../services/availability.service';
import {
  UnitAvailabilityDto,
  UpdateUnitAvailabilityCommand,
  BulkUpdateAvailabilityCommand,
  CloneAvailabilityCommand,
  AvailabilityStatus,
  CheckAvailabilityQuery
} from '../types/availability.types';
import { toast } from 'react-hot-toast';

export const useAvailability = (unitId: string, year: number, month: number) => {
  console.log('useAvailability - Unit ID:', unitId);
  console.log('useAvailability - Year:', year);
  console.log('useAvailability - Month:', month);
  
  const queryClient = useQueryClient();
  const queryKey = ['availability', unitId, year, month];

  const { data, isLoading, error, refetch } = useQuery({
    queryKey,
    queryFn: () => availabilityService.getMonthlyAvailability(unitId, year, month),
    enabled: !!unitId && year > 0 && month > 0,
  });

  const updateAvailability = useMutation({
    mutationFn: (command: UpdateUnitAvailabilityCommand) =>
      availabilityService.updateAvailability(command),
    onSuccess: () => {
      // Invalidate current and adjacent months to reflect cross-month changes
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: ['availability', unitId, year, month - 1] });
      queryClient.invalidateQueries({ queryKey: ['availability', unitId, year, month + 1] });
      toast.success('تم تحديث الإتاحة بنجاح');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'حدث خطأ في تحديث الإتاحة');
    },
  });

  const bulkUpdateAvailability = useMutation({
    mutationFn: (command: BulkUpdateAvailabilityCommand) =>
      availabilityService.bulkUpdateAvailability(command),
    onSuccess: () => {
      // Invalidate current and adjacent months for bulk updates
      queryClient.invalidateQueries({ queryKey });
      queryClient.invalidateQueries({ queryKey: ['availability', unitId, year, month - 1] });
      queryClient.invalidateQueries({ queryKey: ['availability', unitId, year, month + 1] });
      toast.success('تم تحديث الإتاحة بنجاح');
    },
    onError: (error: any) => {
      console.error('Bulk update mutation error:', error);
      
      // Better error handling for different error types
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
      
      toast.error(errorMessage);
    },
  });

  const cloneAvailability = useMutation({
    mutationFn: (command: CloneAvailabilityCommand) =>
      availabilityService.cloneAvailability(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['availability', unitId] });
      toast.success('تم نسخ الإتاحة بنجاح');
    },
    onError: (error: any) => {
      console.error('Clone availability mutation error:', error);
      
      // Better error handling for different error types
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
      
      toast.error(errorMessage);
    },
  });

  const deleteAvailability = useMutation({
    mutationFn: ({ startDate, endDate }: { startDate: string; endDate: string }) =>
      availabilityService.deleteAvailability(unitId, startDate, endDate),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey });
      toast.success('تم حذف الإتاحة بنجاح');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'حدث خطأ في حذف الإتاحة');
    },
  });

  const quickBlock = useCallback(
    (dates: string[]) => {
      const periods = dates.map((date) => ({
        startDate: date,
        endDate: date,
        status: 'Blocked',
        reason: 'حظر سريع',
        notes: '',
        overwriteExisting: true
      }));

      bulkUpdateAvailability.mutate({
        unitId,
        periods,
        overwriteExisting: true
      });
    },
    [unitId, bulkUpdateAvailability]
  );

  const quickAvailable = useCallback(
    (dates: string[]) => {
      const periods = dates.map((date) => ({
        startDate: date,
        endDate: date,
        status: 'Available',
        reason: '',
        notes: '',
        overwriteExisting: true
      }));

      bulkUpdateAvailability.mutate({
        unitId,
        periods,
        overwriteExisting: true
      });
    },
    [unitId, bulkUpdateAvailability]
  );

  return {
    data,
    isLoading,
    error,
    refetch,
    updateAvailability,
    bulkUpdateAvailability,
    cloneAvailability,
    deleteAvailability,
    quickBlock,
    quickAvailable,
  };
};

export const useCheckAvailability = () => {
  return useMutation({
    mutationFn: (query: CheckAvailabilityQuery) => 
      availabilityService.checkAvailability(query),
  });
};