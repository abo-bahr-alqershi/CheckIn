import React from 'react';
import Modal from '../common/Modal';

interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'danger' | 'warning' | 'info';
  loading?: boolean;
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  confirmText = 'تأكيد',
  cancelText = 'إلغاء',
  type = 'info',
  loading = false
}) => {
  const getIcon = () => {
    switch (type) {
      case 'danger': return '🗑️';
      case 'warning': return '⚠️';
      case 'info': return 'ℹ️';
      default: return 'ℹ️';
    }
  };

  const getColorClasses = () => {
    switch (type) {
      case 'danger': return {
        bg: 'bg-red-50',
        border: 'border-red-200',
        icon: 'text-red-400',
        title: 'text-red-800',
        message: 'text-red-700',
        button: 'bg-red-600 hover:bg-red-700 text-white'
      };
      case 'warning': return {
        bg: 'bg-yellow-50',
        border: 'border-yellow-200',
        icon: 'text-yellow-400',
        title: 'text-yellow-800',
        message: 'text-yellow-700',
        button: 'bg-yellow-600 hover:bg-yellow-700 text-white'
      };
      case 'info': return {
        bg: 'bg-blue-50',
        border: 'border-blue-200',
        icon: 'text-blue-400',
        title: 'text-blue-800',
        message: 'text-blue-700',
        button: 'bg-blue-600 hover:bg-blue-700 text-white'
      };
      default: return {
        bg: 'bg-gray-50',
        border: 'border-gray-200',
        icon: 'text-gray-400',
        title: 'text-gray-800',
        message: 'text-gray-700',
        button: 'bg-gray-600 hover:bg-gray-700 text-white'
      };
    }
  };

  const colors = getColorClasses();

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title=""
      size="sm"
      footer={
        <div className="flex justify-end gap-3">
          <button
            onClick={onClose}
            disabled={loading}
            className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50 disabled:opacity-50"
          >
            {cancelText}
          </button>
          <button
            onClick={onConfirm}
            disabled={loading}
            className={`px-4 py-2 rounded-md disabled:opacity-50 transition-colors ${colors.button}`}
          >
            {loading ? (
              <div className="flex items-center space-x-2 space-x-reverse">
                <div className="animate-spin w-4 h-4 border-2 border-white border-t-transparent rounded-full"></div>
                <span>جارٍ التنفيذ...</span>
              </div>
            ) : (
              confirmText
            )}
          </button>
        </div>
      }
    >
      <div className={`rounded-md p-4 ${colors.bg} ${colors.border} border`}>
        <div className="flex">
          <div className="flex-shrink-0">
            <span className={`text-2xl ${colors.icon}`}>{getIcon()}</span>
          </div>
          <div className="mr-3">
            <h3 className={`text-lg font-medium ${colors.title}`}>
              {title}
            </h3>
            <div className={`mt-2 text-sm ${colors.message}`}>
              <p>{message}</p>
            </div>
          </div>
        </div>
      </div>
    </Modal>
  );
};

export default ConfirmDialog;