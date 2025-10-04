// frontend/src/components/Home/Common/SectionHeader.tsx
import React from 'react';
import { Box, Typography, Button } from '@mui/material';
import { ArrowForward } from '@mui/icons-material';

interface SectionHeaderProps {
  title?: string;
  subtitle?: string;
  actionText?: string;
  onActionClick?: () => void;
}

const SectionHeader: React.FC<SectionHeaderProps> = ({
  title,
  subtitle,
  actionText,
  onActionClick,
}) => {
  if (!title && !subtitle) return null;

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'flex-end',
        mb: 2,
      }}
    >
      <Box>
        {title && (
          <Typography variant="h4" fontWeight="bold" gutterBottom>
            {title}
          </Typography>
        )}
        {subtitle && (
          <Typography variant="body1" color="text.secondary">
            {subtitle}
          </Typography>
        )}
      </Box>
      
      {actionText && onActionClick && (
        <Button
          variant="text"
          color="primary"
          endIcon={<ArrowForward />}
          onClick={onActionClick}
          sx={{ flexShrink: 0 }}
        >
          {actionText}
        </Button>
      )}
    </Box>
  );
};

export default SectionHeader;