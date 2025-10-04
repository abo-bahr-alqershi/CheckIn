// src/components/ui/Form.tsx

import React, { createContext, useContext } from 'react';
import { cn } from '../../utils/cn';
import { AlertCircle } from 'lucide-react';

interface FormContextValue {
  errors?: Record<string, string>;
}

const FormContext = createContext<FormContextValue>({});

interface FormProps extends React.FormHTMLAttributes<HTMLFormElement> {
  errors?: Record<string, string>;
}

export const Form: React.FC<FormProps> & {
  Field: React.FC<FormFieldProps>;
  Section: React.FC<FormSectionProps>;
} = ({ 
  errors,
  className = '', 
  children, 
  ...props 
}) => (
  <FormContext.Provider value={{ errors }}>
    <form className={cn('space-y-6', className)} {...props}>
      {children}
    </form>
  </FormContext.Provider>
);

interface FormFieldProps {
  name?: string;
  label?: string;
  required?: boolean;
  help?: string;
  error?: string;
  className?: string;
  children: React.ReactNode;
}

export const FormField: React.FC<FormFieldProps> = ({ 
  name,
  label, 
  required, 
  help,
  error,
  className = '', 
  children 
}) => {
  const { errors } = useContext(FormContext);
  const fieldError = error || (name && errors?.[name]);

  return (
    <div className={cn('space-y-2', className)}>
      {label && (
        <label 
          htmlFor={name}
          className="block text-sm font-medium text-gray-700"
        >
          {label}
          {required && (
            <span className="ml-1 text-red-500">*</span>
          )}
        </label>
      )}
      <div>
        {children}
      </div>
      {help && !fieldError && (
        <p className="text-xs text-gray-500 mt-1">
          {help}
        </p>
      )}
      {fieldError && (
        <div className="flex items-center gap-1 mt-1">
          <AlertCircle className="h-3 w-3 text-red-500" />
          <p className="text-xs text-red-600">
            {fieldError}
          </p>
        </div>
      )}
    </div>
  );
};

interface FormSectionProps {
  title?: string;
  description?: string;
  children: React.ReactNode;
  className?: string;
}

export const FormSection: React.FC<FormSectionProps> = ({
  title,
  description,
  children,
  className = ''
}) => (
  <div className={cn('space-y-4', className)}>
    {(title || description) && (
      <div className="border-b pb-2">
        {title && (
          <h3 className="text-lg font-semibold text-gray-900">
            {title}
          </h3>
        )}
        {description && (
          <p className="text-sm text-gray-600 mt-1">
            {description}
          </p>
        )}
      </div>
    )}
    <div className="space-y-4">
      {children}
    </div>
  </div>
);

Form.Field = FormField;
Form.Section = FormSection;