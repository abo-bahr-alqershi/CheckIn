import React from 'react';
import type { HomeScreenComponent, ComponentType } from '../types/homeScreen.types';
import type { ComponentTypeDefinition } from '../types/component.types';

// Dynamic component imports
const componentMap: Record<ComponentType, React.LazyExoticComponent<React.ComponentType<any>>> = {};

/**
 * Create component instance from type definition
 */
export const createComponentFromType = (
  type: ComponentTypeDefinition,
  sectionId: string,
  order: number = 0
): Partial<HomeScreenComponent> => {
  const id = generateComponentId();
  
  return {
    id,
    sectionId,
    componentType: type.type as ComponentType,
    name: `${type.name} ${Date.now()}`,
    order,
    isVisible: true,
    colSpan: type.defaultColSpan,
    rowSpan: type.defaultRowSpan,
    alignment: 'left',
    properties: type.properties.map(prop => ({
      id: generatePropertyId(),
      propertyKey: prop.key,
      propertyName: prop.name,
      propertyType: prop.type as any,
      value: prop.defaultValue,
      defaultValue: prop.defaultValue,
      isRequired: prop.isRequired,
      validationRules: prop.validationPattern,
      options: prop.options?.join(','),
      helpText: prop.description,
      order: 0
    })),
    styles: type.defaultStyles ? Object.entries(type.defaultStyles).map(([key, value], index) => ({
      id: generateStyleId(),
      styleKey: key,
      styleValue: String(value),
      isImportant: false,
      platform: 'All' as const
    })) : [],
    actions: []
  };
};

/**
 * Get React component for component type
 */
export const getComponentByType = (type: ComponentType): React.LazyExoticComponent<React.ComponentType<any>> | null => {
  return componentMap[type] || null;
};

/**
 * Render component with error boundary
 */
export const renderComponent = (
  component: HomeScreenComponent,
  props: Record<string, any> = {}
): React.ReactElement => {
  const Component = getComponentByType(component.componentType);
  
  if (!Component) {
    return React.createElement('div', {
      className: 'component-error',
      children: `Unknown component type: ${component.componentType}`
    });
  }
  
  // Map component properties to props
  const componentProps = component.properties.reduce((acc, prop) => {
    acc[prop.propertyKey] = prop.value ?? prop.defaultValue;
    return acc;
  }, {} as Record<string, any>);
  
  return React.createElement(
    React.Suspense,
    { fallback: React.createElement('div', { className: 'component-loading' }, 'Loading...') },
    React.createElement(Component, {
      ...componentProps,
      ...props,
      componentId: component.id,
      styles: component.styles,
      actions: component.actions,
      dataSource: component.dataSource
    })
  );
};

/**
 * Clone component with new ID
 */
export const cloneComponent = (
  component: HomeScreenComponent,
  newSectionId?: string
): HomeScreenComponent => {
  const cloned: HomeScreenComponent = {
    ...component,
    id: generateComponentId(),
    sectionId: newSectionId || component.sectionId,
    name: `${component.name} (Copy)`,
    properties: component.properties.map(prop => ({
      ...prop,
      id: generatePropertyId()
    })),
    styles: component.styles.map(style => ({
      ...style,
      id: generateStyleId()
    })),
    actions: component.actions.map(action => ({
      ...action,
      id: generateActionId()
    }))
  };
  
  if (component.dataSource) {
    cloned.dataSource = {
      ...component.dataSource,
      id: generateDataSourceId()
    };
  }
  
  return cloned;
};

/**
 * Validate component configuration
 */
export const validateComponent = (component: Partial<HomeScreenComponent>): string[] => {
  const errors: string[] = [];
  
  if (!component.componentType) {
    errors.push('Component type is required');
  }
  
  if (!component.name || component.name.trim() === '') {
    errors.push('Component name is required');
  }
  
  if (component.colSpan && (component.colSpan < 1 || component.colSpan > 12)) {
    errors.push('Column span must be between 1 and 12');
  }
  
  if (component.rowSpan && component.rowSpan < 1) {
    errors.push('Row span must be at least 1');
  }
  
  // Validate required properties
  component.properties?.forEach(prop => {
    if (prop.isRequired && !prop.value) {
      errors.push(`Property "${prop.propertyName}" is required`);
    }
  });
  
  return errors;
};

/**
 * Get default props for component type
 */
export const getDefaultComponentProps = (type: ComponentType): Record<string, any> => {
  const defaults: Record<ComponentType, Record<string, any>> = {
    Banner: {
      imageUrl: '',
      title: 'Banner Title',
      subtitle: '',
      linkUrl: '',
      height: 200
    },
    Carousel: {
      items: [],
      autoPlay: true,
      interval: 5000,
      showIndicators: true,
      showArrows: true
    },
    CategoryGrid: {
      categories: [],
      columns: 3,
      showLabels: true,
      imageShape: 'circle'
    },
    PropertyList: {
      properties: [],
      viewType: 'grid',
      showFilters: true,
      itemsPerPage: 10
    },
    SearchBar: {
      placeholder: 'Search...',
      showFilters: false,
      searchOnType: true,
      debounceMs: 300
    },
    OfferCard: {
      title: 'Special Offer',
      description: '',
      discount: '20%',
      validUntil: '',
      backgroundColor: '#ff6b6b'
    },
    TextBlock: {
      content: 'Enter your text here',
      fontSize: 16,
      fontWeight: 'normal',
      textAlign: 'left'
    },
    ImageGallery: {
      images: [],
      layout: 'grid',
      columns: 3,
      enableZoom: true
    },
    MapView: {
      center: { lat: 25.2048, lng: 55.2708 },
      zoom: 12,
      showMarkers: true,
      enableInteraction: true
    },
    FilterBar: {
      filters: [],
      layout: 'horizontal',
      showClearAll: true,
      collapsible: false
    }
  };
  
  return defaults[type] || {};
};

/**
 * Generate unique component ID
 */
export const generateComponentId = (): string => {
  return `comp_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

/**
 * Generate unique property ID
 */
export const generatePropertyId = (): string => {
  return `prop_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

/**
 * Generate unique style ID
 */
export const generateStyleId = (): string => {
  return `style_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

/**
 * Generate unique action ID
 */
export const generateActionId = (): string => {
  return `action_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

/**
 * Generate unique data source ID
 */
export const generateDataSourceId = (): string => {
  return `ds_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

/**
 * Get component category icon
 */
export const getCategoryIcon = (category: string): string => {
  const icons: Record<string, string> = {
    Display: '🖼️',
    Navigation: '🧭',
    Input: '⌨️',
    Data: '📊',
    Layout: '📐'
  };
  
  return icons[category] || '📦';
};

/**
 * Check if component supports data binding
 */
export const supportsDataBinding = (type: ComponentType): boolean => {
  const dataComponents: ComponentType[] = [
    'Carousel',
    'CategoryGrid',
    'PropertyList',
    'ImageGallery',
    'MapView'
  ];
  
  return dataComponents.includes(type);
};

/**
 * Get component preview data
 */
export const getComponentPreviewData = (type: ComponentType): any => {
  const previewData: Record<ComponentType, any> = {
    Banner: {
      imageUrl: 'https://via.placeholder.com/800x200',
      title: 'Sample Banner'
    },
    Carousel: {
      items: [
        { id: 1, image: 'https://via.placeholder.com/400x300', title: 'Slide 1' },
        { id: 2, image: 'https://via.placeholder.com/400x300', title: 'Slide 2' }
      ]
    },
    CategoryGrid: {
      categories: [
        { id: 1, name: 'Category 1', icon: '🏠' },
        { id: 2, name: 'Category 2', icon: '🏢' },
        { id: 3, name: 'Category 3', icon: '🏪' },
        { id: 4, name: 'Category 4', icon: '🏛️' }
      ]
    },
    PropertyList: {
      properties: [
        { id: 1, title: 'Property 1', price: 1000000, image: 'https://via.placeholder.com/300x200' },
        { id: 2, title: 'Property 2', price: 1500000, image: 'https://via.placeholder.com/300x200' }
      ]
    },
    SearchBar: {
      placeholder: 'Search properties...'
    },
    OfferCard: {
      title: 'Limited Time Offer',
      description: 'Get 20% off on premium listings'
    },
    TextBlock: {
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
    },
    ImageGallery: {
      images: [
        { id: 1, url: 'https://via.placeholder.com/200x200' },
        { id: 2, url: 'https://via.placeholder.com/200x200' },
        { id: 3, url: 'https://via.placeholder.com/200x200' }
      ]
    },
    MapView: {
      markers: [
        { id: 1, position: { lat: 25.2048, lng: 55.2708 }, title: 'Location 1' }
      ]
    },
    FilterBar: {
      filters: [
        { id: 1, type: 'select', label: 'Type', options: ['Apartment', 'Villa', 'Office'] },
        { id: 2, type: 'range', label: 'Price', min: 0, max: 5000000 }
      ]
    }
  };
  
  return previewData[type] || {};
};