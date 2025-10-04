import type { ComponentStyle, Platform, StyleState } from '../types/homeScreen.types';
import type { AnimationConfig } from '../types/component.types';

/**
 * Generate CSS string from component styles
 */
export const generateStyleString = (
  styles: ComponentStyle[],
  platform: Platform = 'All',
  state: StyleState = 'normal'
): string => {
  const filteredStyles = styles.filter(style => {
    const platformMatch = style.platform === 'All' || style.platform === platform;
    const stateMatch = !style.state || style.state === state;
    return platformMatch && stateMatch;
  });
  
  return filteredStyles
    .map(style => {
      const value = style.unit ? `${style.styleValue}${style.unit}` : style.styleValue;
      const important = style.isImportant ? ' !important' : '';
      return `${camelToKebab(style.styleKey)}: ${value}${important}`;
    })
    .join('; ');
};

/**
 * Convert style object to ComponentStyle array
 */
export const styleObjectToArray = (
  styles: Record<string, any>,
  platform: Platform = 'All'
): ComponentStyle[] => {
  return Object.entries(styles).map(([key, value], index) => {
    const { value: styleValue, unit } = parseStyleValue(value);
    
    return {
      id: `style_${Date.now()}_${index}`,
      styleKey: key,
      styleValue: String(styleValue),
      unit,
      isImportant: false,
      platform
    };
  });
};

/**
 * Merge multiple style arrays with priority
 */
export const mergeStyles = (...styleArrays: ComponentStyle[][]): ComponentStyle[] => {
  const styleMap = new Map<string, ComponentStyle>();
  
  styleArrays.forEach(styles => {
    styles.forEach(style => {
      const key = `${style.styleKey}-${style.platform}-${style.state || 'normal'}`;
      styleMap.set(key, style);
    });
  });
  
  return Array.from(styleMap.values());
};

/**
 * Generate responsive styles
 */
export const generateResponsiveStyles = (
  baseStyles: ComponentStyle[],
  breakpoints: Record<string, number> = {
    mobile: 768,
    tablet: 1024,
    desktop: 1440
  }
): ComponentStyle[] => {
  const responsiveStyles: ComponentStyle[] = [...baseStyles];
  
  // Add media query styles
  Object.entries(breakpoints).forEach(([device, maxWidth]) => {
    baseStyles.forEach(style => {
      if (style.styleKey === 'fontSize' || style.styleKey === 'padding' || style.styleKey === 'margin') {
        const scaleFactor = device === 'mobile' ? 0.8 : device === 'tablet' ? 0.9 : 1;
        const scaledValue = parseFloat(style.styleValue) * scaleFactor;
        
        responsiveStyles.push({
          ...style,
          id: `${style.id}_${device}`,
          styleValue: String(scaledValue),
          mediaQuery: `(max-width: ${maxWidth}px)`
        });
      }
    });
  });
  
  return responsiveStyles;
};

/**
 * Generate animation styles
 */
export const generateAnimationStyles = (animation?: AnimationConfig): Record<string, string> => {
  if (!animation) return {};
  
  const { type, duration, delay, easing = 'ease' } = animation;
  
  const animations: Record<string, string> = {
    fade: `fade ${duration}ms ${easing} ${delay}ms forwards`,
    slide: `slideIn ${duration}ms ${easing} ${delay}ms forwards`,
    zoom: `zoomIn ${duration}ms ${easing} ${delay}ms forwards`,
    bounce: `bounce ${duration}ms ${easing} ${delay}ms forwards`,
    rotate: `rotate ${duration}ms ${easing} ${delay}ms forwards`
  };
  
  return {
    animation: animations[type] || 'none',
    opacity: type === 'fade' ? '0' : '1'
  };
};

/**
 * Generate grid styles for component
 */
export const generateGridStyles = (
  colSpan: number,
  rowSpan: number,
  alignment: string
): Record<string, string> => {
  const alignmentMap: Record<string, string> = {
    left: 'flex-start',
    center: 'center',
    right: 'flex-end'
  };
  
  return {
    gridColumn: `span ${colSpan}`,
    gridRow: `span ${rowSpan}`,
    display: 'flex',
    flexDirection: 'column',
    alignItems: alignmentMap[alignment] || 'flex-start',
    width: '100%',
    height: '100%'
  };
};

/**
 * Parse style value and extract unit
 */
export const parseStyleValue = (value: any): { value: string | number; unit?: string } => {
  if (typeof value === 'number') {
    return { value };
  }
  
  const stringValue = String(value);
  const match = stringValue.match(/^(-?\d+\.?\d*)(px|%|em|rem|vh|vw|pt)?$/);
  
  if (match) {
    return {
      value: parseFloat(match[1]),
      unit: match[2] || undefined
    };
  }
  
  return { value: stringValue };
};

/**
 * Convert camelCase to kebab-case
 */
export const camelToKebab = (str: string): string => {
  return str.replace(/[A-Z]/g, letter => `-${letter.toLowerCase()}`);
};

/**
 * Convert kebab-case to camelCase
 */
export const kebabToCamel = (str: string): string => {
  return str.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
};

/**
 * Validate CSS color value
 */
export const isValidColor = (color: string): boolean => {
  const s = new Option().style;
  s.color = color;
  return s.color !== '';
};

/**
 * Generate shadow styles
 */
export const generateShadowStyles = (elevation: number = 0): Record<string, string> => {
  const shadows = [
    'none',
    '0 1px 3px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.24)',
    '0 3px 6px rgba(0,0,0,0.16), 0 3px 6px rgba(0,0,0,0.23)',
    '0 10px 20px rgba(0,0,0,0.19), 0 6px 6px rgba(0,0,0,0.23)',
    '0 14px 28px rgba(0,0,0,0.25), 0 10px 10px rgba(0,0,0,0.22)',
    '0 19px 38px rgba(0,0,0,0.30), 0 15px 12px rgba(0,0,0,0.22)'
  ];
  
  return {
    boxShadow: shadows[Math.min(elevation, shadows.length - 1)]
  };
};

/**
 * Generate spacing styles
 */
export const generateSpacingStyles = (
  padding?: string,
  margin?: string
): Record<string, string> => {
  const styles: Record<string, string> = {};
  
  if (padding) {
    const paddingValues = padding.split(' ');
    if (paddingValues.length === 1) {
      styles.padding = padding;
    } else {
      styles.paddingTop = paddingValues[0];
      styles.paddingRight = paddingValues[1] || paddingValues[0];
      styles.paddingBottom = paddingValues[2] || paddingValues[0];
      styles.paddingLeft = paddingValues[3] || paddingValues[1] || paddingValues[0];
    }
  }
  
  if (margin) {
    const marginValues = margin.split(' ');
    if (marginValues.length === 1) {
      styles.margin = margin;
    } else {
      styles.marginTop = marginValues[0];
      styles.marginRight = marginValues[1] || marginValues[0];
      styles.marginBottom = marginValues[2] || marginValues[0];
      styles.marginLeft = marginValues[3] || marginValues[1] || marginValues[0];
    }
  }
  
  return styles;
};

/**
 * CSS animation keyframes
 */
export const animationKeyframes = `
  @keyframes fade {
    from { opacity: 0; }
    to { opacity: 1; }
  }
  
  @keyframes slideIn {
    from { transform: translateY(20px); opacity: 0; }
    to { transform: translateY(0); opacity: 1; }
  }
  
  @keyframes zoomIn {
    from { transform: scale(0.8); opacity: 0; }
    to { transform: scale(1); opacity: 1; }
  }
  
  @keyframes bounce {
    0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
    40% { transform: translateY(-10px); }
    60% { transform: translateY(-5px); }
  }
  
  @keyframes rotate {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
  }
`;/**
 * Default component styles by type
 */
export const getDefaultComponentStyles = (componentType: string): Record<string, string> => {
  const defaultStyles: Record<string, Record<string, string>> = {
    Banner: {
      width: '100%',
      minHeight: '200px',
      borderRadius: '8px',
      overflow: 'hidden',
      position: 'relative'
    },
    Carousel: {
      width: '100%',
      minHeight: '300px',
      position: 'relative',
      overflow: 'hidden'
    },
    CategoryGrid: {
      display: 'grid',
      gridTemplateColumns: 'repeat(auto-fill, minmax(100px, 1fr))',
      gap: '16px',
      width: '100%'
    },
    PropertyList: {
      width: '100%',
      display: 'flex',
      flexDirection: 'column',
      gap: '16px'
    },
    SearchBar: {
      width: '100%',
      maxWidth: '600px',
      margin: '0 auto'
    },
    OfferCard: {
      width: '100%',
      padding: '20px',
      borderRadius: '12px',
      textAlign: 'center'
    },
    TextBlock: {
      width: '100%',
      lineHeight: '1.6'
    },
    ImageGallery: {
      width: '100%',
      display: 'grid',
      gap: '8px'
    },
    MapView: {
      width: '100%',
      height: '400px',
      borderRadius: '8px',
      overflow: 'hidden'
    },
    FilterBar: {
      width: '100%',
      padding: '16px',
      backgroundColor: '#f5f5f5',
      borderRadius: '8px'
    }
  };

  return defaultStyles[componentType] || {};
};

/**
 * Generate themed styles
 */
export const generateThemedStyles = (
  theme: 'light' | 'dark',
  primaryColor: string = '#3b82f6'
): Record<string, string> => {
  const themes = {
    light: {
      backgroundColor: '#ffffff',
      color: '#1a1a1a',
      borderColor: '#e5e5e5',
      shadowColor: 'rgba(0, 0, 0, 0.1)'
    },
    dark: {
      backgroundColor: '#1a1a1a',
      color: '#ffffff',
      borderColor: '#333333',
      shadowColor: 'rgba(255, 255, 255, 0.1)'
    }
  };

  const currentTheme = themes[theme];

  return {
    ...currentTheme,
    primaryColor,
    secondaryColor: adjustColor(primaryColor, -20),
    accentColor: adjustColor(primaryColor, 20)
  };
};

/**
 * Adjust color brightness
 */
export const adjustColor = (color: string, amount: number): string => {
  const usePound = color[0] === '#';
  const col = usePound ? color.slice(1) : color;
  const num = parseInt(col, 16);
  let r = (num >> 16) + amount;
  let b = ((num >> 8) & 0x00FF) + amount;
  let g = (num & 0x0000FF) + amount;
  
  r = r > 255 ? 255 : r < 0 ? 0 : r;
  b = b > 255 ? 255 : b < 0 ? 0 : b;
  g = g > 255 ? 255 : g < 0 ? 0 : g;
  
  return (usePound ? '#' : '') + (g | (b << 8) | (r << 16)).toString(16).padStart(6, '0');
};

/**
 * Generate border styles
 */
export const generateBorderStyles = (
  width: number = 1,
  style: 'solid' | 'dashed' | 'dotted' = 'solid',
  color: string = '#e5e5e5',
  radius: number = 0
): Record<string, string> => {
  return {
    border: `${width}px ${style} ${color}`,
    borderRadius: radius ? `${radius}px` : '0'
  };
};

/**
 * Export styles to CSS string
 */
export const exportToCSS = (
  componentId: string,
  styles: ComponentStyle[]
): string => {
  const groupedStyles: Record<string, ComponentStyle[]> = {};
  
  // Group styles by state and media query
  styles.forEach(style => {
    const key = `${style.state || 'normal'}-${style.mediaQuery || 'default'}`;
    if (!groupedStyles[key]) {
      groupedStyles[key] = [];
    }
    groupedStyles[key].push(style);
  });
  
  let cssString = '';
  
  Object.entries(groupedStyles).forEach(([key, styleGroup]) => {
    const [state, mediaQuery] = key.split('-');
    const selector = state === 'normal' 
      ? `#${componentId}` 
      : `#${componentId}:${state}`;
    
    const stylesString = generateStyleString(styleGroup);
    
    if (mediaQuery !== 'default') {
      cssString += `${mediaQuery} {\n  ${selector} {\n    ${stylesString}\n  }\n}\n`;
    } else {
      cssString += `${selector} {\n  ${stylesString}\n}\n`;
    }
  });
  
  return cssString;
};