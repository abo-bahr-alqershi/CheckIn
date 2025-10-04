export type ComponentType = string;

export interface HomeScreenComponent {
  id: string;
  sectionId: string;
  componentType: ComponentType;
  name: string;
  order: number;
  isVisible: boolean;
  colSpan: number;
  rowSpan: number;
  alignment: string;
  properties: Array<any>;
  styles: Array<any>;
  actions: any[];
  dataSource?: any;
}

export interface HomeScreenSection {
  id: string;
  // Additional properties can be defined as needed
}

export type ComponentStyle = {
  id: string;
  styleKey: string;
  styleValue: string;
  unit?: string;
  isImportant: boolean;
  platform: string;
  state?: string;
  mediaQuery?: string;
};

export type Platform = 'All' | string;

export type StyleState = 'normal' | string;