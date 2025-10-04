import React, { Component } from 'react';
import type { ErrorInfo, ReactNode } from 'react';

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
  onError?: (error: Error, errorInfo: ErrorInfo) => void;
}

interface State {
  hasError: boolean;
  error: Error | null;
  errorInfo: ErrorInfo | null;
}

class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      hasError: false,
      error: null,
      errorInfo: null,
    };
  }

  static getDerivedStateFromError(error: Error): State {
    return {
      hasError: true,
      error,
      errorInfo: null,
    };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    this.setState({
      error,
      errorInfo,
    });

    // إرسال تقرير الخطأ للمراقبة
    console.error('Error Boundary caught an error:', error, errorInfo);
    
    // استدعاء معالج الخطأ المخصص إذا كان متوفراً
    if (this.props.onError) {
      this.props.onError(error, errorInfo);
    }

    // يمكن إضافة خدمة مراقبة الأخطاء هنا مثل Sentry
    // Sentry.captureException(error, { contexts: { react: errorInfo } });
  }

  handleRetry = () => {
    this.setState({
      hasError: false,
      error: null,
      errorInfo: null,
    });
  };

  handleReload = () => {
    window.location.reload();
  };

  render() {
    if (this.state.hasError) {
      // عرض fallback مخصص إذا تم توفيره
      if (this.props.fallback) {
        return this.props.fallback;
      }

      // عرض صفحة خطأ افتراضية
      return (
        <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
          <div className="sm:mx-auto sm:w-full sm:max-w-md">
            <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
              <div className="text-center">
                <div className="text-6xl mb-4">⚠️</div>
                <h2 className="text-2xl font-bold text-gray-900 mb-4">
                  حدث خطأ غير متوقع
                </h2>
                <p className="text-gray-600 mb-6">
                  نعتذر عن الإزعاج. حدث خطأ في التطبيق ولا يمكن عرض هذه الصفحة.
                </p>
                
                {import.meta.env.DEV && this.state.error && (
                  <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-md text-left">
                    <h3 className="text-sm font-medium text-red-800 mb-2">
                      تفاصيل الخطأ (وضع التطوير):
                    </h3>
                    <div className="text-xs text-red-700 font-mono">
                      <div className="mb-2">
                        <strong>الخطأ:</strong> {this.state.error.message}
                      </div>
                      {this.state.error.stack && (
                        <div className="mb-2">
                          <strong>المكدس:</strong>
                          <pre className="mt-1 whitespace-pre-wrap text-xs">
                            {this.state.error.stack}
                          </pre>
                        </div>
                      )}
                      {this.state.errorInfo && (
                        <div>
                          <strong>معلومات المكون:</strong>
                          <pre className="mt-1 whitespace-pre-wrap text-xs">
                            {this.state.errorInfo.componentStack}
                          </pre>
                        </div>
                      )}
                    </div>
                  </div>
                )}

                <div className="space-y-3">
                  <button
                    onClick={this.handleRetry}
                    className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                  >
                    🔄 المحاولة مرة أخرى
                  </button>
                  
                  <button
                    onClick={this.handleReload}
                    className="w-full flex justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                  >
                    🔃 إعادة تحميل الصفحة
                  </button>
                  
                  <button
                    onClick={() => window.history.back()}
                    className="w-full flex justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                  >
                    ← العودة للصفحة السابقة
                  </button>
                </div>

                <div className="mt-6 text-sm text-gray-500">
                  إذا استمر هذا الخطأ، يرجى التواصل مع الدعم الفني.
                </div>
              </div>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

// Hook لمعالجة الأخطاء في المكونات الوظيفية
export const useErrorHandler = () => {
  const [error, setError] = React.useState<Error | null>(null);

  const resetError = React.useCallback(() => {
    setError(null);
  }, []);

  const handleError = React.useCallback((error: Error) => {
    console.error('Error handled:', error);
    setError(error);
  }, []);

  // إعادة رمي الخطأ ليتم التقاطه بواسطة Error Boundary
  React.useEffect(() => {
    if (error) {
      throw error;
    }
  }, [error]);

  return { handleError, resetError };
};

// مكون لعرض خطأ مخصص للشاشات الصغيرة
export const ErrorFallback: React.FC<{
  error?: Error;
  resetError?: () => void;
  message?: string;
}> = ({ error, resetError, message = 'حدث خطأ غير متوقع' }) => {
  return (
    <div className="min-h-[200px] flex flex-col items-center justify-center p-6 bg-gray-50 rounded-lg border border-gray-200">
      <div className="text-4xl mb-4">😕</div>
      <h3 className="text-lg font-medium text-gray-900 mb-2">{message}</h3>
      
      {error && import.meta.env.DEV && (
        <details className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-sm w-full">
          <summary className="cursor-pointer text-red-800 font-medium">
            عرض تفاصيل الخطأ
          </summary>
          <div className="mt-2 text-red-700 font-mono text-xs">
            {error.message}
            {error.stack && (
              <pre className="mt-2 whitespace-pre-wrap">{error.stack}</pre>
            )}
          </div>
        </details>
      )}
      
      <div className="flex space-x-3 space-x-reverse">
        {resetError && (
          <button
            onClick={resetError}
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
          >
            المحاولة مرة أخرى
          </button>
        )}
        
        <button
          onClick={() => window.location.reload()}
          className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500 text-sm"
        >
          إعادة التحميل
        </button>
      </div>
    </div>
  );
};

export default ErrorBoundary;