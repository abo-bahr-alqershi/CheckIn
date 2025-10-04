export type AnimationConfig = Record<string, any>;

export interface ComponentTypeDefinition {
  type: string;
  name: string;
  defaultColSpan: number;
  defaultRowSpan: number;
  properties: Array<{
    key: string;
    name: string;
    type: string;
    defaultValue: any;
    isRequired: boolean;
    description: string;
    validationPattern?: any;
    options?: any;
  }>;
  defaultStyles?: Record<string, any>;
}