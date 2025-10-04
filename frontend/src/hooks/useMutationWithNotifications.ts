import { useMutation, type UseMutationOptions, type MutationFunction } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import { useNotificationContext } from '../components/ui/NotificationProvider';
import type { ResultDto } from '../types/common.types';

export function useMutationWithNotifications<TData, TError = unknown, TVariables = void>(
  mutationFn: MutationFunction<ResultDto<TData>, TVariables>,
  options?: UseMutationOptions<ResultDto<TData>, TError, TVariables, unknown>
) {
  const { showSuccess, showError } = useNotificationContext();
  return useMutation<ResultDto<TData>, TError, TVariables, unknown>({
    mutationFn,
    ...options,
    onSuccess: (data, variables, context) => {
      if (data?.success && data.message) {
        showSuccess(data.message);
      }
      options?.onSuccess?.(data, variables, context);
    },
    onError: (error: unknown, variables, context) => {
      let message = 'حدث خطأ ما';
      if (axios.isAxiosError(error) && error.response?.data && typeof (error.response.data as any).message === 'string') {
        message = (error.response.data as any).message;
      }
      showError(message);
      options?.onError?.(error as TError, variables, context);
    }
  });
} 