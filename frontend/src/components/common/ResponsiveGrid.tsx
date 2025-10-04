import React from 'react';
import { Box, type SxProps, type Theme } from '@mui/material';

interface ResponsiveGridProps {
  container?: boolean;
  item?: boolean;
  spacing?: number;
  xs?: number | 'auto';
  sm?: number | 'auto';
  md?: number | 'auto';
  lg?: number | 'auto';
  xl?: number | 'auto';
  alignItems?: string;
  sx?: SxProps<Theme>;
  children?: React.ReactNode;
  onClick?: (event: React.MouseEvent) => void;
  className?: string;
  key?: string | number;
}

const ResponsiveGrid: React.FC<ResponsiveGridProps> = ({
  container = false,
  item = false,
  spacing = 0,
  xs,
  sm,
  md,
  lg,
  xl,
  alignItems,
  sx = {},
  children,
  onClick,
  className,
  ...otherProps
}) => {
  const getWidth = (breakpoint: number | 'auto' | undefined) => {
    if (breakpoint === undefined) return undefined;
    if (breakpoint === 'auto') return 'auto';
    return `${(breakpoint / 12) * 100}%`;
  };

  const containerStyles: SxProps<Theme> = container ? {
    display: 'flex',
    flexWrap: 'wrap',
    gap: spacing,
    alignItems,
    width: '100%',
    ...sx
  } : {};

  const itemStyles: SxProps<Theme> = item ? {
    flexGrow: 0,
    flexShrink: 0,
    width: {
      xs: getWidth(xs) || '100%',
      sm: getWidth(sm) || getWidth(xs) || '100%',
      md: getWidth(md) || getWidth(sm) || getWidth(xs) || '100%',
      lg: getWidth(lg) || getWidth(md) || getWidth(sm) || getWidth(xs) || '100%',
      xl: getWidth(xl) || getWidth(lg) || getWidth(md) || getWidth(sm) || getWidth(xs) || '100%',
    },
    minWidth: 0,
    ...sx
  } : { ...sx };

  const finalStyles = container ? containerStyles : itemStyles;

  return (
    <Box
      style={{ direction: 'rtl' }}
      sx={finalStyles}
      onClick={onClick}
      className={className}
      {...otherProps}
    >
      {children}
    </Box>
  );
};

export default ResponsiveGrid;