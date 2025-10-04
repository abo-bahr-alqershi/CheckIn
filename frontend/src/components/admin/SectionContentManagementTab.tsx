// frontend/src/components/admin/SectionContentManagementTab.tsx

import React from 'react';
import ContentManagementPanel from './ContentManagementPanel';
import type { DynamicContent } from '../../types/homeSections.types';

interface ContentManagementTabProps {
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}

const ContentManagementTab: React.FC<ContentManagementTabProps> = ({ formData, onChange }) => {
  const handleContentChange = (content: DynamicContent[]) => {
    onChange('content', content);
  };

  return (
    <ContentManagementPanel
      sectionType={formData.sectionType}
      currentContent={formData.content || []}
      onContentChange={handleContentChange}
      maxItems={formData.displaySettings?.maxItems || 10}
    />
  );
};

export default ContentManagementTab;