import React from 'react';

interface ErrorScreenProps {
  error: Error;
}

const ErrorScreen: React.FC<ErrorScreenProps> = ({ error }) => {
  return <div>Error: {error.message}</div>;
};

export default ErrorScreen;