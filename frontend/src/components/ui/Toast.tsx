import React, { useEffect, useState } from 'react';
import { useNotifications } from '../../stores/appStore';

interface ToastProps {
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
  duration?: number;
  onClose: () => void;
}

const Toast: React.FC<ToastProps> = ({ message, type, duration = 5000, onClose }) => {
  const [isVisible, setIsVisible] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
      setIsVisible(false);
      setTimeout(onClose, 300); // Wait for animation to complete
    }, duration);

    return () => clearTimeout(timer);
  }, [duration, onClose]);

  const getToastStyles = () => {
    const baseStyles = 'fixed top-4 left-1/2 transform -translate-x-1/2 z-50 px-6 py-4 rounded-lg shadow-lg transition-all duration-300 max-w-md mx-auto';
    const typeStyles = {
      success: 'bg-green-500 text-white',
      error: 'bg-red-500 text-white',
      warning: 'bg-yellow-500 text-white',
      info: 'bg-blue-500 text-white'
    };
    
    const visibilityStyles = isVisible 
      ? 'opacity-100 transform translate-y-0' 
      : 'opacity-0 transform -translate-y-2';

    return `${baseStyles} ${typeStyles[type]} ${visibilityStyles}`;
  };

  const getIcon = () => {
    switch (type) {
      case 'success': return '✅';
      case 'error': return '❌';
      case 'warning': return '⚠️';
      case 'info': return 'ℹ️';
      default: return '';
    }
  };

  return (
    <div className={getToastStyles()}>
      <div className="flex items-center space-x-3 space-x-reverse">
        <span className="text-xl">{getIcon()}</span>
        <p className="text-sm font-medium flex-1">{message}</p>
        <button
          onClick={() => {
            setIsVisible(false);
            setTimeout(onClose, 300);
          }}
          className="text-white hover:text-gray-200 transition-colors"
        >
          <span className="text-lg">×</span>
        </button>
      </div>
    </div>
  );
};

// Toast Container Component
export const ToastContainer: React.FC = () => {
  const { error, success, clearNotifications } = useNotifications();
  const [toasts, setToasts] = useState<Array<{ id: string; message: string; type: 'success' | 'error' }>>([]);

  useEffect(() => {
    if (error) {
      const id = Date.now().toString();
      setToasts(prev => [...prev, { id, message: error, type: 'error' }]);
      clearNotifications();
    }
  }, [error, clearNotifications]);

  useEffect(() => {
    if (success) {
      const id = Date.now().toString();
      setToasts(prev => [...prev, { id, message: success, type: 'success' }]);
      clearNotifications();
    }
  }, [success, clearNotifications]);

  const removeToast = (id: string) => {
    setToasts(prev => prev.filter(toast => toast.id !== id));
  };

  return (
    <>
      {toasts.map((toast) => (
        <Toast
          key={toast.id}
          message={toast.message}
          type={toast.type}
          onClose={() => removeToast(toast.id)}
        />
      ))}
    </>
  );
};

export default Toast;