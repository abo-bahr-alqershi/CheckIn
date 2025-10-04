import { useState, useCallback } from 'react';
import { useNotifications } from '../stores/appStore';

interface UseUXHelpersReturn {
  // Loading states
  loading: Record<string, boolean>;
  setLoading: (key: string, isLoading: boolean) => void;
  
  // Confirmation dialogs
  confirmDialog: {
    isOpen: boolean;
    title: string;
    message: string;
    type: 'danger' | 'warning' | 'info';
    onConfirm: () => void;
  } | null;
  showConfirmDialog: (config: {
    title: string;
    message: string;
    type?: 'danger' | 'warning' | 'info';
    onConfirm: () => void;
  }) => void;
  hideConfirmDialog: () => void;
  
  // Async operations with UX feedback
  executeWithFeedback: <T>(
    operation: () => Promise<T>,
    options?: {
      loadingKey?: string;
      successMessage?: string;
      errorMessage?: string;
      onSuccess?: (result: T) => void;
      onError?: (error: any) => void;
    }
  ) => Promise<T | null>;
  
  // Copy to clipboard with feedback
  copyToClipboard: (text: string, successMessage?: string) => Promise<void>;
  
  // Debounced state
  createDebouncedState: <T>(initialValue: T, delay: number) => {
    value: T;
    debouncedValue: T;
    setValue: (value: T) => void;
  };
}

export const useUXHelpers = (): UseUXHelpersReturn => {
  const { showSuccess, showError } = useNotifications();
  const [loading, setLoadingState] = useState<Record<string, boolean>>({});
  const [confirmDialog, setConfirmDialog] = useState<UseUXHelpersReturn['confirmDialog']>(null);

  const setLoading = useCallback((key: string, isLoading: boolean) => {
    setLoadingState(prev => ({
      ...prev,
      [key]: isLoading
    }));
  }, []);

  const showConfirmDialog = useCallback((config: {
    title: string;
    message: string;
    type?: 'danger' | 'warning' | 'info';
    onConfirm: () => void;
  }) => {
    setConfirmDialog({
      isOpen: true,
      title: config.title,
      message: config.message,
      type: config.type || 'info',
      onConfirm: config.onConfirm
    });
  }, []);

  const hideConfirmDialog = useCallback(() => {
    setConfirmDialog(null);
  }, []);

  const executeWithFeedback = useCallback(async <T>(
    operation: () => Promise<T>,
    options: {
      loadingKey?: string;
      successMessage?: string;
      errorMessage?: string;
      onSuccess?: (result: T) => void;
      onError?: (error: any) => void;
    } = {}
  ): Promise<T | null> => {
    const {
      loadingKey = 'default',
      successMessage,
      errorMessage,
      onSuccess,
      onError
    } = options;

    try {
      setLoading(loadingKey, true);
      const result = await operation();
      
      if (successMessage) {
        showSuccess(successMessage);
      }
      
      if (onSuccess) {
        onSuccess(result);
      }
      
      return result;
    } catch (error) {
      const message = errorMessage || 'حدث خطأ غير متوقع';
      showError(message);
      
      if (onError) {
        onError(error);
      }
      
      console.error('Operation failed:', error);
      return null;
    } finally {
      setLoading(loadingKey, false);
    }
  }, [setLoading, showSuccess, showError]);

  const copyToClipboard = useCallback(async (text: string, successMessage = 'تم النسخ بنجاح') => {
    try {
      await navigator.clipboard.writeText(text);
      showSuccess(successMessage);
    } catch (error) {
      showError('فشل في نسخ النص');
      console.error('Failed to copy text:', error);
    }
  }, [showSuccess, showError]);

  const createDebouncedState = useCallback(<T>(initialValue: T, delay: number) => {
    const [value, setValue] = useState<T>(initialValue);
    const [debouncedValue, setDebouncedValue] = useState<T>(initialValue);

    // This would need to be implemented with useEffect in the component
    // that uses this hook, but we provide the structure here
    return {
      value,
      debouncedValue,
      setValue: (newValue: T) => {
        setValue(newValue);
        // The component would handle the debouncing with useEffect
      }
    };
  }, []);

  return {
    loading,
    setLoading,
    confirmDialog,
    showConfirmDialog,
    hideConfirmDialog,
    executeWithFeedback,
    copyToClipboard,
    createDebouncedState
  };
};

export default useUXHelpers;