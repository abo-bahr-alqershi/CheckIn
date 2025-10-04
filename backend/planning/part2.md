    public class ComponentStyleDto
    {
        public Guid Id { get; set; }
        public string StyleKey { get; set; }
        public string StyleValue { get; set; }
        public string Unit { get; set; }
        public bool IsImportant { get; set; }
        public string MediaQuery { get; set; }
        public string State { get; set; }
        public string Platform { get; set; }
    }

    public class ComponentActionDto
    {
        public Guid Id { get; set; }
        public string ActionType { get; set; }
        public string ActionTrigger { get; set; }
        public string ActionTarget { get; set; }
        public string ActionParams { get; set; }
        public string Conditions { get; set; }
        public bool RequiresAuth { get; set; }
        public string AnimationType { get; set; }
        public int Priority { get; set; }
    }

    public class ComponentDataSourceDto
    {
        public Guid Id { get; set; }
        public string SourceType { get; set; }
        public string DataEndpoint { get; set; }
        public string HttpMethod { get; set; }
        public string Headers { get; set; }
        public string QueryParams { get; set; }
        public string RequestBody { get; set; }
        public string DataMapping { get; set; }
        public string CacheKey { get; set; }
        public int CacheDuration { get; set; }
        public string RefreshTrigger { get; set; }
        public int RefreshInterval { get; set; }
        public string ErrorHandling { get; set; }
        public string MockData { get; set; }
        public bool UseMockInDev { get; set; }
    }
}

// ComponentPropertyDto.cs
using System;

namespace Application.DTOs
{
    public class ComponentPropertyDto
    {
        public Guid Id { get; set; }
        public string PropertyKey { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public string ValidationRules { get; set; }
        public string Options { get; set; }
        public string HelpText { get; set; }
        public int Order { get; set; }
    }
}
// ComponentTypeDto.cs
using System.Collections.Generic;

namespace Application.DTOs
{
    public class ComponentTypeDto
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Category { get; set; }
        public int DefaultColSpan { get; set; }
        public int DefaultRowSpan { get; set; }
        public bool AllowResize { get; set; }
        public List<ComponentPropertyMetadata> Properties { get; set; }
        public List<string> SupportedPlatforms { get; set; }
        public Dictionary<string, object> DefaultStyles { get; set; }
    }

    public class ComponentPropertyMetadata
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsRequired { get; set; }
        public string DefaultValue { get; set; }
        public string[] Options { get; set; }
        public string Description { get; set; }
        public string ValidationPattern { get; set; }
    }
}
// DataSourceDto.cs
using System.Collections.Generic;

namespace Application.DTOs
{
    public class DataSourceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Endpoint { get; set; }
        public bool IsAvailable { get; set; }
        public bool RequiresAuth { get; set; }
        public string[] SupportedComponents { get; set; }
        public List<DataSourceParameter> Parameters { get; set; }
        public int? CacheDuration { get; set; }
    }

    public class DataSourceParameter
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
        public string[] Options { get; set; }
    }
}
// HomeScreenPreviewDto.cs
using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class HomeScreenPreviewDto
    {
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string Platform { get; set; }
        public string DeviceType { get; set; }
        public List<HomeScreenSectionPreviewDto> Sections { get; set; }
        public PreviewMetadata Metadata { get; set; }
    }

    public class HomeScreenSectionPreviewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
        public Dictionary<string, string> Styles { get; set; }
        public List<HomeScreenComponentPreviewDto> Components { get; set; }
    }

    public class HomeScreenComponentPreviewDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Alignment { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public Dictionary<string, string> Styles { get; set; }
        public object Data { get; set; }
        public AnimationConfig Animation { get; set; }
    }

    public class AnimationConfig
    {
        public string Type { get; set; }
        public int Duration { get; set; }
        public int Delay { get; set; }
        public string Easing { get; set; }
    }

    public class PreviewMetadata
    {
        public DateTime GeneratedAt { get; set; }
        public int TotalSections { get; set; }
        public int TotalComponents { get; set; }
        public int EstimatedLoadTime { get; set; }
        public bool UsedMockData { get; set; }
    }
}
// Infrastructure/Repositories/HomeScreenRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class HomeScreenRepository : IHomeScreenRepository
    {
        private readonly IApplicationDbContext _context;

        public HomeScreenRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HomeScreenTemplate> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<HomeScreenTemplate> GetTemplateWithSectionsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .Include(t => t.Sections.Where(s => !s.IsDeleted))
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<HomeScreenTemplate> GetTemplateWithFullHierarchyAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .Include(t => t.Sections.Where(s => !s.IsDeleted))
                    .ThenInclude(s => s.Components.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Properties)
                .Include(t => t.Sections)
                    .ThenInclude(s => s.Components)
                        .ThenInclude(c => c.Styles)
                .Include(t => t.Sections)
                    .ThenInclude(s => s.Components)
                        .ThenInclude(c => c.Actions)
                .Include(t => t.Sections)
                    .ThenInclude(s => s.Components)
                        .ThenInclude(c => c.DataSource)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<List<HomeScreenTemplate>> GetTemplatesAsync(
            string platform,
            string targetAudience,
            bool? isActive,
            bool includeDeleted,
            CancellationToken cancellationToken)
        {
            var query = _context.HomeScreenTemplates.AsQueryable();

            if (!includeDeleted)
                query = query.Where(t => !t.IsDeleted);

            if (!string.IsNullOrEmpty(platform))
                query = query.Where(t => t.Platform == platform || t.Platform == "All");

            if (!string.IsNullOrEmpty(targetAudience))
                query = query.Where(t => t.TargetAudience == targetAudience || t.TargetAudience == "All");

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive.Value);

            return await query
                .OrderByDescending(t => t.IsDefault)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<HomeScreenTemplate>> GetActiveTemplatesAsync(
            string platform,
            string targetAudience,
            CancellationToken cancellationToken)
        {
            return await GetTemplatesAsync(platform, targetAudience, true, false, cancellationToken);
        }

        public async Task<HomeScreenTemplate> GetActiveTemplateAsync(
            string platform,
            string targetAudience,
            CancellationToken cancellationToken)
        {
            var templates = await GetActiveTemplatesAsync(platform, targetAudience, cancellationToken);
            return templates.FirstOrDefault();
        }

        public async Task<HomeScreenTemplate> GetDefaultTemplateAsync(string platform, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .Where(t => !t.IsDeleted && t.IsDefault && (t.Platform == platform || t.Platform == "All"))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<HomeScreenSection> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenSections
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
        }

        public async Task<HomeScreenSection> GetSectionWithComponentsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenSections
                .Include(s => s.Components.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
        }

        public async Task<List<HomeScreenSection>> GetTemplateSectionsAsync(Guid templateId, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenSections
                .Where(s => s.TemplateId == templateId && !s.IsDeleted)
                .OrderBy(s => s.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<HomeScreenComponent> GetComponentByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenComponents
                .Include(c => c.Properties)
                .Include(c => c.Styles)
                .Include(c => c.Actions)
                .Include(c => c.DataSource)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<UserHomeScreen> GetUserHomeScreenAsync(Guid userId, string platform, CancellationToken cancellationToken)
        {
            return await _context.UserHomeScreens
                .Include(u => u.Template)
                .FirstOrDefaultAsync(u => u.UserId == userId && 
                    (u.Template.Platform == platform || u.Template.Platform == "All") &&
                    u.Template.IsActive &&
                    !u.Template.IsDeleted, 
                    cancellationToken);
        }

        public async Task AddTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken)
        {
            await _context.HomeScreenTemplates.AddAsync(template, cancellationToken);
        }

        public async Task AddSectionAsync(HomeScreenSection section, CancellationToken cancellationToken)
        {
            await _context.HomeScreenSections.AddAsync(section, cancellationToken);
        }

        public async Task AddComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken)
        {
            await _context.HomeScreenComponents.AddAsync(component, cancellationToken);
        }

        public async Task UpdateTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken)
        {
            _context.HomeScreenTemplates.Update(template);
        }

        public async Task UpdateSectionAsync(HomeScreenSection section, CancellationToken cancellationToken)
        {
            _context.HomeScreenSections.Update(section);
        }

        public async Task UpdateSectionsAsync(List<HomeScreenSection> sections, CancellationToken cancellationToken)
        {
            _context.HomeScreenSections.UpdateRange(sections);
        }

        public async Task UpdateComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken)
        {
            _context.HomeScreenComponents.Update(component);
        }

        public async Task UpdateComponentsAsync(List<HomeScreenComponent> components, CancellationToken cancellationToken)
        {
            _context.HomeScreenComponents.UpdateRange(components);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
// Infrastructure/Repositories/IHomeScreenRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IHomeScreenRepository
    {
        // Template operations
        Task<HomeScreenTemplate> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetTemplateWithSectionsAsync(Guid id, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetTemplateWithFullHierarchyAsync(Guid id, CancellationToken cancellationToken);
        Task<List<HomeScreenTemplate>> GetTemplatesAsync(string platform, string targetAudience, bool? isActive, bool includeDeleted, CancellationToken cancellationToken);
        Task<List<HomeScreenTemplate>> GetActiveTemplatesAsync(string platform, string targetAudience, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetActiveTemplateAsync(string platform, string targetAudience, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetDefaultTemplateAsync(string platform, CancellationToken cancellationToken);
        Task AddTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken);
        Task UpdateTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken);

        // Section operations
        Task<HomeScreenSection> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<HomeScreenSection> GetSectionWithComponentsAsync(Guid id, CancellationToken cancellationToken);
        Task<List<HomeScreenSection>> GetTemplateSectionsAsync(Guid templateId, CancellationToken cancellationToken);
        Task AddSectionAsync(HomeScreenSection section, CancellationToken cancellationToken);
        Task UpdateSectionAsync(HomeScreenSection section, CancellationToken cancellationToken);
        Task UpdateSectionsAsync(List<HomeScreenSection> sections, CancellationToken cancellationToken);

        // Component operations
        Task<HomeScreenComponent> GetComponentByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken);
        Task UpdateComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken);
        Task UpdateComponentsAsync(List<HomeScreenComponent> components, CancellationToken cancellationToken);

        // User customization operations
        Task<UserHomeScreen> GetUserHomeScreenAsync(Guid userId, string platform, CancellationToken cancellationToken);

        // Save changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

// types/homeScreen.types.ts
export interface HomeScreenTemplate {
  id: string;
  name: string;
  description: string;
  version: string;
  isActive: boolean;
  isDefault: boolean;
  publishedAt?: Date;
  publishedBy?: string;
  publishedByName?: string;
  platform: Platform;
  targetAudience: TargetAudience;
  metaData?: string;
  customizationData?: string;
  userPreferences?: string;
  createdAt: Date;
  updatedAt?: Date;
  sections: HomeScreenSection[];
}

export interface HomeScreenSection {
  id: string;
  templateId: string;
  name: string;
  title: string;
  subtitle?: string;
  order: number;
  isVisible: boolean;
  backgroundColor?: string;
  backgroundImage?: string;
  padding?: string;
  margin?: string;
  minHeight?: number;
  maxHeight?: number;
  customStyles?: string;
  conditions?: string;
  components: HomeScreenComponent[];
}

export interface HomeScreenComponent {
  id: string;
  sectionId: string;
  componentType: ComponentType;
  name: string;
  order: number;
  isVisible: boolean;
  colSpan: number;
  rowSpan: number;
  alignment: Alignment;
  customClasses?: string;
  animationType?: AnimationType;
  animationDuration?: number;
  conditions?: string;
  properties: ComponentProperty[];
  styles: ComponentStyle[];
  actions: ComponentAction[];
  dataSource?: ComponentDataSource;
}

export interface ComponentProperty {
  id: string;
  propertyKey: string;
  propertyName: string;
  propertyType: PropertyType;
  value?: any;
  defaultValue?: any;
  isRequired: boolean;
  validationRules?: string;
  options?: string;
  helpText?: string;
  order: number;
}

export interface ComponentStyle {
  id: string;
  styleKey: string;
  styleValue: string;
  unit?: string;
  isImportant: boolean;
  mediaQuery?: string;
  state?: StyleState;
  platform: Platform;
}

export interface ComponentAction {
  id: string;
  actionType: ActionType;
  actionTrigger: ActionTrigger;
  actionTarget: string;
  actionParams?: string;
  conditions?: string;
  requiresAuth: boolean;
  animationType?: string;
  priority: number;
}

export interface ComponentDataSource {
  id: string;
  sourceType: DataSourceType;
  dataEndpoint?: string;
  httpMethod?: HttpMethod;
  headers?: string;
  queryParams?: string;
  requestBody?: string;
  dataMapping?: string;
  cacheKey?: string;
  cacheDuration?: number;
  refreshTrigger?: RefreshTrigger;
  refreshInterval?: number;
  errorHandling?: string;
  mockData?: string;
  useMockInDev: boolean;
}

export type Platform = 'iOS' | 'Android' | 'All';
export type TargetAudience = 'Guest' | 'User' | 'Premium' | 'All';
export type ComponentType = 'Banner' | 'Carousel' | 'CategoryGrid' | 'PropertyList' | 
  'SearchBar' | 'OfferCard' | 'TextBlock' | 'ImageGallery' | 'MapView' | 'FilterBar';
export type Alignment = 'left' | 'center' | 'right';
export type AnimationType = 'fade' | 'slide' | 'zoom' | 'bounce' | 'rotate';
export type PropertyType = 'text' | 'number' | 'boolean' | 'select' | 'multiselect' | 
  'color' | 'image' | 'date' | 'object';
export type StyleState = 'normal' | 'hover' | 'active' | 'focus' | 'disabled';
export type ActionType = 'Navigate' | 'OpenModal' | 'CallAPI' | 'Share' | 'Download';
export type ActionTrigger = 'Click' | 'LongPress' | 'Swipe' | 'Load';
export type DataSourceType = 'Static' | 'API' | 'Database' | 'Cache';
export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE';
export type RefreshTrigger = 'OnLoad' | 'OnFocus' | 'Manual' | 'Timer';

// types/component.types.ts
export interface ComponentTypeDefinition {
  type: string;
  name: string;
  description: string;
  icon: string;
  category: ComponentCategory;
  defaultColSpan: number;
  defaultRowSpan: number;
  allowResize: boolean;
  properties: ComponentPropertyMetadata[];
  supportedPlatforms: string[];
  defaultStyles?: Record<string, any>;
}

export interface ComponentPropertyMetadata {
  key: string;
  name: string;
  type: string;
  isRequired: boolean;
  defaultValue?: any;
  options?: string[];
  description?: string;
  validationPattern?: string;
}

export interface DataSourceDefinition {
  id: string;
  name: string;
  type: string;
  description: string;
  endpoint?: string;
  isAvailable: boolean;
  requiresAuth: boolean;
  supportedComponents: string[];
  parameters?: DataSourceParameter[];
  cacheDuration?: number;
}

export interface DataSourceParameter {
  key: string;
  name: string;
  type: string;
  defaultValue?: any;
  isRequired: boolean;
  description?: string;
  options?: string[];
}

export interface ComponentPreview {
  id: string;
  type: string;
  name: string;
  order: number;
  colSpan: number;
  rowSpan: number;
  alignment: string;
  properties: Record<string, any>;
  styles: Record<string, string>;
  data?: any;
  animation?: AnimationConfig;
}

export interface AnimationConfig {
  type: string;
  duration: number;
  delay: number;
  easing?: string;
}

export type ComponentCategory = 'Display' | 'Navigation' | 'Input' | 'Data' | 'Layout';

// types/dragDrop.types.ts
export interface DragItem {
  id: string;
  type: DragItemType;
  componentType?: string;
  sourceIndex: number;
  sourceSectionId?: string;
  data: any;
}

export interface DropTarget {
  id: string;
  type: DropTargetType;
  accept: DragItemType[];
  canDrop: (item: DragItem) => boolean;
  onDrop: (item: DragItem, targetIndex?: number) => void;
}

export interface DragPreview {
  width: number;
  height: number;
  content: React.ReactNode;
}

export interface DragState {
  isDragging: boolean;
  draggedItem: DragItem | null;
  draggedOverTarget: string | null;
  draggedOverIndex: number | null;
}

export type DragItemType = 'new-component' | 'existing-component' | 'section';
export type DropTargetType = 'section' | 'canvas' | 'component-list';

export interface Position {
  x: number;
  y: number;
}

export interface Size {
  width: number;
  height: number;
}

export interface GridPosition {
  row: number;
  col: number;
  rowSpan: number;
  colSpan: number;
}
// services/homeScreenService.ts
import axios from 'axios';
import { 
  HomeScreenTemplate, 
  HomeScreenSection, 
  HomeScreenComponent 
} from '../types/homeScreen.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || '/api';

export class HomeScreenService {
  // Template operations
  async getTemplates(params?: {
    platform?: string;
    targetAudience?: string;
    isActive?: boolean;
    includeDeleted?: boolean;
  }): Promise<HomeScreenTemplate[]> {
    const response = await axios.get(`${API_BASE_URL}/home-screen/templates`, { params });
    return response.data;
  }

  async getTemplateById(id: string, includeHierarchy = true): Promise<HomeScreenTemplate> {
    const response = await axios.get(`${API_BASE_URL}/home-screen/templates/${id}`, {
      params: { includeHierarchy }
    });
    return response.data;
  }

  async createTemplate(template: Partial<HomeScreenTemplate>): Promise<HomeScreenTemplate> {
    const response = await axios.post(`${API_BASE_URL}/home-screen/templates`, template);
    return response.data;
  }

  async updateTemplate(id: string, updates: Partial<HomeScreenTemplate>): Promise<HomeScreenTemplate> {
    const response = await axios.put(`${API_BASE_URL}/home-screen/templates/${id}`, updates);
    return response.data;
  }

  async deleteTemplate(id: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/home-screen/templates/${id}`);
  }

  async duplicateTemplate(id: string, newName: string, newDescription?: string): Promise<HomeScreenTemplate> {
    const response = await axios.post(`${API_BASE_URL}/home-screen/templates/${id}/duplicate`, {
      newName,
      newDescription
    });
    return response.data;
  }

  async publishTemplate(id: string, deactivateOthers = true): Promise<void> {
    await axios.post(`${API_BASE_URL}/home-screen/templates/${id}/publish`, {
      deactivateOthers
    });
  }

  // Section operations
  async createSection(section: Partial<HomeScreenSection>): Promise<HomeScreenSection> {
    const response = await axios.post(`${API_BASE_URL}/home-screen/sections`, section);
    return response.data;
  }

  async updateSection(id: string, updates: Partial<HomeScreenSection>): Promise<HomeScreenSection> {
    const response = await axios.put(`${API_BASE_URL}/home-screen/sections/${id}`, updates);
    return response.data;
  }

  async deleteSection(id: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/home-screen/sections/${id}`);
  }

  async reorderSections(templateId: string, sections: Array<{ sectionId: string; newOrder: number }>): Promise<void> {
    await axios.post(`${API_BASE_URL}/home-screen/templates/${templateId}/reorder-sections`, {
      sections
    });
  }

  // Component operations
  async createComponent(component: Partial<HomeScreenComponent>): Promise<HomeScreenComponent> {
    const response = await axios.post(`${API_BASE_URL}/home-screen/components`, component);
    return response.data;
  }

  async updateComponent(id: string, updates: Partial<HomeScreenComponent>): Promise<HomeScreenComponent> {
    const response = await axios.put(`${API_BASE_URL}/home-screen/components/${id}`, updates);
    return response.data;
  }

  async deleteComponent(id: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/home-screen/components/${id}`);
  }

  async reorderComponents(sectionId: string, components: Array<{ componentId: string; newOrder: number }>): Promise<void> {
    await axios.post(`${API_BASE_URL}/home-screen/sections/${sectionId}/reorder-components`, {
      components
    });
  }

  // Preview
  async previewTemplate(templateId: string, options?: {
    platform?: string;
    deviceType?: string;
    useMockData?: boolean;
  }): Promise<any> {
    const response = await axios.get(`${API_BASE_URL}/home-screen/templates/${templateId}/preview`, {
      params: options
    });
    return response.data;
  }

  // Active home screen
  async getActiveHomeScreen(platform: string, targetAudience: string): Promise<HomeScreenTemplate> {
    const response = await axios.get(`${API_BASE_URL}/home-screen/active`, {
      params: { platform, targetAudience }
    });
    return response.data;
  }
}

export default new HomeScreenService();

// services/componentService.ts
import axios from 'axios';
import { ComponentTypeDefinition } from '../types/component.types';
import { ComponentProperty, ComponentStyle, ComponentAction, ComponentDataSource } from '../types/homeScreen.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || '/api';

export class ComponentService {
  async getComponentTypes(platform?: string): Promise<ComponentTypeDefinition[]> {
    const response = await axios.get(`${API_BASE_URL}/home-screen/component-types`, {
      params: { platform }
    });
    return response.data;
  }

  async updateComponentProperty(componentId: string, propertyId: string, value: any): Promise<void> {
    await axios.put(`${API_BASE_URL}/home-screen/components/${componentId}/properties/${propertyId}`, {
      value
    });
  }

  async addComponentStyle(componentId: string, style: Partial<ComponentStyle>): Promise<ComponentStyle> {
    const response = await axios.post(
      `${API_BASE_URL}/home-screen/components/${componentId}/styles`, 
      style
    );
    return response.data;
  }

  async updateComponentStyle(componentId: string, styleId: string, updates: Partial<ComponentStyle>): Promise<void> {
    await axios.put(
      `${API_BASE_URL}/home-screen/components/${componentId}/styles/${styleId}`, 
      updates
    );
  }

  async deleteComponentStyle(componentId: string, styleId: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/home-screen/components/${componentId}/styles/${styleId}`);
  }

  async addComponentAction(componentId: string, action: Partial<ComponentAction>): Promise<ComponentAction> {
    const response = await axios.post(
      `${API_BASE_URL}/home-screen/components/${componentId}/actions`, 
      action
    );
    return response.data;
  }

  async updateComponentAction(componentId: string, actionId: string, updates: Partial<ComponentAction>): Promise<void> {
    await axios.put(
      `${API_BASE_URL}/home-screen/components/${componentId}/actions/${actionId}`, 
      updates
    );
  }

  async deleteComponentAction(componentId: string, actionId: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/home-screen/components/${componentId}/actions/${actionId}`);
  }

  async setComponentDataSource(componentId: string, dataSource: Partial<ComponentDataSource>): Promise<ComponentDataSource> {
    const response = await axios.post(
      `${API_BASE_URL}/home-screen/components/${componentId}/data-source`, 
      dataSource
    );
    return response.data;
  }
  // services/dataSourceService.ts
import axios from 'axios';
import { DataSourceDefinition } from '../types/component.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || '/api';

export class DataSourceService {
  async getDataSources(componentType?: string): Promise<DataSourceDefinition[]> {
    const response = await axios.get(`${API_BASE_URL}/home-screen/data-sources`, {
      params: { componentType }
    });
    return response.data;
  }

  async testDataSource(
    dataSourceId: string, 
    parameters?: Record<string, any>
  ): Promise<any> {
    const response = await axios.post(
      `${API_BASE_URL}/home-screen/data-sources/${dataSourceId}/test`,
      { parameters }
    );
    return response.data;
  }

  async fetchData(
    endpoint: string,
    method: string = 'GET',
    params?: Record<string, any>,
    headers?: Record<string, string>
  ): Promise<any> {
    const response = await axios({
      method,
      url: `${API_BASE_URL}${endpoint}`,
      params: method === 'GET' ? params : undefined,
      data: method !== 'GET' ? params : undefined,
      headers
    });
    return response.data;
  }

  async validateDataMapping(
    data: any,
    mapping: string
  ): Promise<{ isValid: boolean; errors?: string[] }> {
    try {
      const mappingObj = JSON.parse(mapping);
      // Validate mapping logic here
      return { isValid: true };
    } catch (error) {
      return { 
        isValid: false, 
        errors: ['Invalid mapping JSON format'] 
      };
    }
  }
}

export default new DataSourceService();

// hooks/useHomeScreenBuilder.ts
import { useState, useCallback, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { 
  HomeScreenTemplate, 
  HomeScreenSection, 
  HomeScreenComponent 
} from '../types/homeScreen.types';
import homeScreenService from '../services/homeScreenService';
import { useNotification } from './useNotification';

export interface HomeScreenBuilderState {
  template: HomeScreenTemplate | null;
  selectedSection: HomeScreenSection | null;
  selectedComponent: HomeScreenComponent | null;
  isLoading: boolean;
  isSaving: boolean;
  hasUnsavedChanges: boolean;
  errors: Record<string, string>;
}

export const useHomeScreenBuilder = (templateId?: string) => {
  const [state, setState] = useState<HomeScreenBuilderState>({
    template: null,
    selectedSection: null,
    selectedComponent: null,
    isLoading: false,
    isSaving: false,
    hasUnsavedChanges: false,
    errors: {}
  });

  const { showSuccess, showError } = useNotification();

  // Load template
  const loadTemplate = useCallback(async (id: string) => {
    setState(prev => ({ ...prev, isLoading: true, errors: {} }));
    try {
      const template = await homeScreenService.getTemplateById(id);
      setState(prev => ({
        ...prev,
        template,
        isLoading: false
      }));
    } catch (error) {
      setState(prev => ({
        ...prev,
        isLoading: false,
        errors: { load: 'Failed to load template' }
      }));
      showError('Failed to load template');
    }
  }, [showError]);

  // Create section
  const createSection = useCallback(async (section: Partial<HomeScreenSection>) => {
    if (!state.template) return;

    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const newSection = await homeScreenService.createSection({
        ...section,
        templateId: state.template.id,
        order: state.template.sections.length
      });

      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: [...prev.template!.sections, newSection]
        },
        selectedSection: newSection,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Section created successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to create section');
    }
  }, [state.template, showSuccess, showError]);

  // Update section
  const updateSection = useCallback(async (
    sectionId: string, 
    updates: Partial<HomeScreenSection>
  ) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const updatedSection = await homeScreenService.updateSection(sectionId, updates);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(s => 
            s.id === sectionId ? updatedSection : s
          )
        },
        selectedSection: prev.selectedSection?.id === sectionId 
          ? updatedSection 
          : prev.selectedSection,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Section updated successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to update section');
    }
  }, [showSuccess, showError]);

  // Delete section
  const deleteSection = useCallback(async (sectionId: string) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      await homeScreenService.deleteSection(sectionId);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.filter(s => s.id !== sectionId)
        },
        selectedSection: prev.selectedSection?.id === sectionId 
          ? null 
          : prev.selectedSection,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Section deleted successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to delete section');
    }
  }, [showSuccess, showError]);

  // Create component
  const createComponent = useCallback(async (
    sectionId: string,
    component: Partial<HomeScreenComponent>
  ) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const section = state.template?.sections.find(s => s.id === sectionId);
      if (!section) throw new Error('Section not found');

      const newComponent = await homeScreenService.createComponent({
        ...component,
        sectionId,
        order: section.components.length
      });

      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(s => 
            s.id === sectionId 
              ? { ...s, components: [...s.components, newComponent] }
              : s
          )
        },
        selectedComponent: newComponent,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Component added successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to add component');
    }
  }, [state.template, showSuccess, showError]);

  // Update component
  const updateComponent = useCallback(async (
    componentId: string,
    updates: Partial<HomeScreenComponent>
  ) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const updatedComponent = await homeScreenService.updateComponent(componentId, updates);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(section => ({
            ...section,
            components: section.components.map(c => 
              c.id === componentId ? updatedComponent : c
            )
          }))
        },
        selectedComponent: prev.selectedComponent?.id === componentId 
          ? updatedComponent 
          : prev.selectedComponent,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Component updated successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to update component');
    }
  }, [showSuccess, showError]);

  // Delete component
  const deleteComponent = useCallback(async (componentId: string) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      await homeScreenService.deleteComponent(componentId);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(section => ({
            ...section,
            components: section.components.filter(c => c.id !== componentId)
          }))
        },
        selectedComponent: prev.selectedComponent?.id === componentId 
          ? null 
          : prev.selectedComponent,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Component deleted successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to delete component');
    }
  }, [showSuccess, showError]);

  // Reorder sections
  const reorderSections = useCallback(async (
    sections: Array<{ sectionId: string; newOrder: number }>
  ) => {
    if (!state.template) return;

    const originalSections = [...state.template.sections];
    
    // Optimistic update
    setState(prev => ({
      ...prev,
      template: {
        ...prev.template!,
        sections: sections
          .sort((a, b) => a.newOrder - b.newOrder)
          .map(({ sectionId }) => 
            prev.template!.sections.find(s => s.id === sectionId)!
          )
      },
      hasUnsavedChanges: true
    }));

    try {
      await homeScreenService.reorderSections(state.template.id, sections);
      setState(prev => ({ ...prev, hasUnsavedChanges: false }));
    } catch (error) {
      // Revert on error
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: originalSections
        }
      }));
      showError('Failed to reorder sections');
    }
  }, [state.template, showError]);

  // Select section
  const selectSection = useCallback((section: HomeScreenSection | null) => {
    setState(prev => ({
      ...prev,
      selectedSection: section,
      selectedComponent: null
    }));
  }, []);

  // Select component
  const selectComponent = useCallback((component: HomeScreenComponent | null) => {
    setState(prev => ({
      ...prev,
      selectedComponent: component,
      selectedSection: component 
        ? prev.template?.sections.find(s => 
            s.components.some(c => c.id === component.id)
          ) || null
        : prev.selectedSection
    }));
  }, []);

  // Publish template
  const publishTemplate = useCallback(async (deactivateOthers = true) => {
    if (!state.template) return;

    setState(prev => ({ ...prev, isSaving: true }));
    try {
      await homeScreenService.publishTemplate(state.template.id, deactivateOthers);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          isActive: true,
          publishedAt: new Date()
        },
        isSaving: false
      }));

      showSuccess('Template published successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to publish template');
    }
  }, [state.template, showSuccess, showError]);

  useEffect(() => {
    if (templateId) {
      loadTemplate(templateId);
    }
  }, [templateId, loadTemplate]);

  return {
    ...state,
    actions: {
      loadTemplate,
      createSection,
      updateSection,
      deleteSection,
      createComponent,
      updateComponent,
      deleteComponent,
      reorderSections,
      selectSection,
      selectComponent,
      publishTemplate
    }
  };
};

// hooks/useDragDrop.ts
import { useState, useCallback, useRef } from 'react';
import { useDrag, useDrop, DragSourceMonitor, DropTargetMonitor } from 'react-dnd';
import { DragItem, DragState, Position } from '../types/dragDrop.types';

export const useDragDrop = () => {
  const [dragState, setDragState] = useState<DragState>({
    isDragging: false,
    draggedItem: null,
    draggedOverTarget: null,
    draggedOverIndex: null
  });

  const dragCounter = useRef(0);

  const handleDragStart = useCallback((item: DragItem) => {
    setDragState({
      isDragging: true,
      draggedItem: item,
      draggedOverTarget: null,
      draggedOverIndex: null
    });
  }, []);

  const handleDragEnd = useCallback(() => {
    setDragState({
      isDragging: false,
      draggedItem: null,
      draggedOverTarget: null,
      draggedOverIndex: null
    });
    dragCounter.current = 0;
  }, []);

  const handleDragEnter = useCallback((targetId: string, index?: number) => {
    dragCounter.current++;
    setDragState(prev => ({
      ...prev,
      draggedOverTarget: targetId,
      draggedOverIndex: index ?? null
    }));
  }, []);

  const handleDragLeave = useCallback(() => {
    dragCounter.current--;
    if (dragCounter.current === 0) {
      setDragState(prev => ({
        ...prev,
        draggedOverTarget: null,
        draggedOverIndex: null
      }));
    }
  }, []);

  const useDraggable = useCallback((
    item: DragItem,
    options?: {
      canDrag?: () => boolean;
      preview?: React.ReactNode;
    }
  ) => {
    const [{ isDragging }, drag, preview] = useDrag({
      type: item.type,
      item: () => {
        handleDragStart(item);
        return item;
      },
      end: handleDragEnd,
      canDrag: options?.canDrag,
      collect: (monitor: DragSourceMonitor) => ({
        isDragging: monitor.isDragging()
      })
    });

    return { drag, preview, isDragging };
  }, [handleDragStart, handleDragEnd]);

  const useDroppable = useCallback((
    targetId: string,
    acceptTypes: string[],
    onDrop: (item: DragItem, index?: number) => void,
    options?: {
      canDrop?: (item: DragItem) => boolean;
      onHover?: (item: DragItem, index?: number) => void;
    }
  ) => {
    const [{ isOver, canDrop }, drop] = useDrop({
      accept: acceptTypes,
      drop: (item: DragItem, monitor: DropTargetMonitor) => {
        if (monitor.isOver({ shallow: true })) {
          onDrop(item, dragState.draggedOverIndex ?? undefined);
          handleDragEnd();
        }
      },
      canDrop: (item: DragItem) => options?.canDrop?.(item) ?? true,
      hover: (item: DragItem, monitor: DropTargetMonitor) => {
        if (monitor.isOver({ shallow: true })) {
          handleDragEnter(targetId);
          options?.onHover?.(item);
        }
      },
      collect: (monitor: DropTargetMonitor) => ({
        isOver: monitor.isOver({ shallow: true }),
        canDrop: monitor.canDrop()
      })
    });

    return { drop, isOver, canDrop };
  }, [dragState.draggedOverIndex, handleDragEnd, handleDragEnter]);

  const getDropIndex = useCallback((
    e: React.DragEvent,
    orientation: 'horizontal' | 'vertical',
    itemCount: number
  ): number => {
    const rect = e.currentTarget.getBoundingClientRect();
    const pos = orientation === 'horizontal' 
      ? (e.clientX - rect.left) / rect.width
      : (e.clientY - rect.top) / rect.height;
    
    return Math.min(Math.floor(pos * itemCount), itemCount - 1);
  }, []);

  return {
    dragState,
    useDraggable,
    useDroppable,
    getDropIndex,
    handleDragEnd
  };
};

// hooks/useComponentProperties.ts
import { useState, useCallback, useEffect } from 'react';
import { ComponentProperty, HomeScreenComponent } from '../types/homeScreen.types';
import { ComponentPropertyMetadata } from '../types/component.types';
import componentService from '../services/componentService';
import { useDebounce } from './useDebounce';

interface UseComponentPropertiesOptions {
  component: HomeScreenComponent | null;
  propertyDefinitions: ComponentPropertyMetadata[];
  onChange?: (propertyKey: string, value: any) => void;
  autoSave?: boolean;
  debounceMs?: number;
}

export const useComponentProperties = ({
  component,
  propertyDefinitions,
  onChange,
  autoSave = true,
  debounceMs = 500
}: UseComponentPropertiesOptions) => {
  const [properties, setProperties] = useState<Record<string, any>>({});
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isDirty, setIsDirty] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const debouncedProperties = useDebounce(properties, debounceMs);

  // Initialize properties from component
  useEffect(() => {
    if (component) {
      const initialProps: Record<string, any> = {};
      
      // Set values from component properties
      component.properties.forEach(prop => {
        initialProps[prop.propertyKey] = prop.value ?? prop.defaultValue;
      });

      // Add default values for missing properties
      propertyDefinitions.forEach(def => {
        if (!(def.key in initialProps)) {
          initialProps[def.key] = def.defaultValue;
        }
      });

      setProperties(initialProps);
      setIsDirty(false);
    }
  }, [component, propertyDefinitions]);

  // Validate property value
  const validateProperty = useCallback((
    key: string, 
    value: any, 
    definition: ComponentPropertyMetadata
  ): string | null => {
    // Required validation
    if (definition.isRequired && !value) {
      return `${definition.name} is required`;
    }

    // Type-specific validation
    switch (definition.type) {
      case 'number':
        if (value && isNaN(Number(value))) {
          return `${definition.name} must be a number`;
        }
        break;

      case 'url':
        if (value && !isValidUrl(value)) {
          return `${definition.name} must be a valid URL`;
        }
        break;

      case 'email':
        if (value && !isValidEmail(value)) {
          return `${definition.name} must be a valid email`;
        }
        break;

      case 'color':
        if (value && !isValidColor(value)) {
          return `${definition.name} must be a valid color`;
        }
        break;
    }

    // Custom validation pattern
    if (definition.validationPattern && value) {
      const regex = new RegExp(definition.validationPattern);
      if (!regex.test(value)) {
        return `${definition.name} format is invalid`;
      }
    }

    return null;
  }, []);

  // Update property value
  const updateProperty = useCallback((key: string, value: any) => {
    const definition = propertyDefinitions.find(d => d.key === key);
    if (!definition) return;

    // Validate
    const error = validateProperty(key, value, definition);
    setErrors(prev => ({
      ...prev,
      [key]: error || ''
    }));

    // Update value
    setProperties(prev => ({
      ...prev,
      [key]: value
    }));
    setIsDirty(true);

    // Notify change
    onChange?.(key, value);
  }, [propertyDefinitions, validateProperty, onChange]);

  // Reset property to default
  const resetProperty = useCallback((key: string) => {
    const definition = propertyDefinitions.find(d => d.key === key);
    if (definition) {
      updateProperty(key, definition.defaultValue);
    }
  }, [propertyDefinitions, updateProperty]);

  // Reset all properties
  const resetAllProperties = useCallback(() => {
    const resetProps: Record<string, any> = {};
    propertyDefinitions.forEach(def => {
      resetProps[def.key] = def.defaultValue;
    });
    setProperties(resetProps);
    setErrors({});
    setIsDirty(true);
  }, [propertyDefinitions]);

  // Auto-save debounced properties
  useEffect(() => {
    if (autoSave && isDirty && component && Object.keys(debouncedProperties).length > 0) {
      const hasErrors = Object.values(errors).some(error => error);
      if (!hasErrors) {
        saveProperties();
      }
    }
  }, [debouncedProperties, autoSave, isDirty, component, errors]);

  // Save properties to server
  const saveProperties = useCallback(async () => {
    if (!component) return;

    setIsSaving(true);
    try {
      // Update each changed property
      const updatePromises = Object.entries(properties).map(([key, value]) => {
        const property = component.properties.find(p => p.propertyKey === key);
        if (property && property.value !== value) {
          return componentService.updateComponentProperty(
            component.id,
            property.id,
            value
          );
        }
        return Promise.resolve();
      });

      await Promise.all(updatePromises);
      setIsDirty(false);
    } catch (error) {
      console.error('Failed to save properties:', error);
    } finally {
      setIsSaving(false);
    }
  }, [component, properties]);

  // Get property value with type coercion
  const getPropertyValue = useCallback((key: string): any => {
    const value = properties[key];
    const definition = propertyDefinitions.find(d => d.key === key);
    
    if (!definition) return value;

    switch (definition.type) {
      case 'number':
        return value ? Number(value) : 0;
      case 'boolean':
        return Boolean(value);
      case 'array':
        return Array.isArray(value) ? value : [];
      case 'object':
        return typeof value === 'object' ? value : {};
      default:
        return value;
    }
  }, [properties, propertyDefinitions]);

  return {
    properties,
    errors,
    isDirty,
    isSaving,
    updateProperty,
    resetProperty,
    resetAllProperties,
    saveProperties,
    getPropertyValue
  };
};

// Validation helpers
const isValidUrl = (url: string): boolean => {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
};

const isValidEmail = (email: string): boolean => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
};

const isValidColor = (color: string): boolean => {
  return /^#[0-9A-F]{6}$/i.test(color) || 
         /^rgb\(\d+,\s*\d+,\s*\d+\)$/.test(color) ||
         /^rgba\(\d+,\s*\d+,\s*\d+,\s*[\d.]+\)$/.test(color);
};

// hooks/usePreview.ts
import { useState, useCallback, useEffect } from 'react';
import { HomeScreenTemplate } from '../types/homeScreen.types';
import homeScreenService from '../services/homeScreenService';

interface PreviewOptions {
  platform: 'iOS' | 'Android' | 'Web';
  deviceType: 'mobile' | 'tablet' | 'desktop';
  orientation: 'portrait' | 'landscape';
  useMockData: boolean;
  darkMode: boolean;
}

interface PreviewState {
  isOpen: boolean;
  isLoading: boolean;
  previewData: any;
  options: PreviewOptions;
  error: string | null;
}

export const usePreview = (template: HomeScreenTemplate | null) => {
  const [state, setState] = useState<PreviewState>({
    isOpen: false,
    isLoading: false,
    previewData: null,
    options: {
      platform: 'iOS',
      deviceType: 'mobile',
      orientation: 'portrait',
      useMockData: true,
      darkMode: false
    },
    error: null
  });

  const [deviceDimensions, setDeviceDimensions] = useState({
    width: 375,
    height: 812
  });

  // Update device dimensions based on options
  useEffect(() => {
    const dimensions = getDeviceDimensions(
      state.options.deviceType,
      state.options.orientation
    );
    setDeviceDimensions(dimensions);
  }, [state.options.deviceType, state.options.orientation]);

  // Load preview data
  const loadPreview = useCallback(async () => {
    if (!template) return;

    setState(prev => ({ ...prev, isLoading: true, error: null }));
    try {
      const preview = await homeScreenService.previewTemplate(template.id, {
        platform: state.options.platform,
        deviceType: state.options.deviceType,
        useMockData: state.options.useMockData
      });

      setState(prev => ({
        ...prev,
        previewData: preview,
        isLoading: false
      }));
    } catch (error) {
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: 'Failed to load preview'
      }));
    }
  }, [template, state.options]);

  // Open preview
  const openPreview = useCallback(() => {
    setState(prev => ({ ...prev, isOpen: true }));
    loadPreview();
  }, [loadPreview]);

  // Close preview
  const closePreview = useCallback(() => {
    setState(prev => ({
      ...prev,
      isOpen: false,
      previewData: null,
      error: null
    }));
  }, []);

  // Update preview options
  const updateOptions = useCallback((updates: Partial<PreviewOptions>) => {
    setState(prev => ({
      ...prev,
      options: { ...prev.options, ...updates }
    }));
  }, []);

  // Refresh preview
  const refreshPreview = useCallback(() => {
    loadPreview();
  }, [loadPreview]);

  // Export preview as image
  const exportAsImage = useCallback(async (format: 'png' | 'jpg' = 'png') => {
    // Implementation would use html2canvas or similar
    console.log('Export preview as', format);
  }, []);

  // Share preview link
  const sharePreview = useCallback(async () => {
    if (!template) return;
    
    try {
      const shareUrl = `${window.location.origin}/preview/${template.id}`;
      
      if (navigator.share) {
        await navigator.share({
          title: `Preview: ${template.name}`,
          text: template.description,
          url: shareUrl
        });
      } else {
        // Fallback: copy to clipboard
        await navigator.clipboard.writeText(shareUrl);
        alert('Preview link copied to clipboard');
      }
    } catch (error) {
      console.error('Failed to share preview:', error);
    }
  }, [template]);

  return {
    ...state,
    deviceDimensions,
    actions: {
      openPreview,
      closePreview,
      updateOptions,
      refreshPreview,
      exportAsImage,
      sharePreview
    }
  };
};

// Device dimension presets
const getDeviceDimensions = (
  deviceType: string,
  orientation: string
): { width: number; height: number } => {
  const dimensions = {
    mobile: { width: 375, height: 812 },    // iPhone X
    tablet: { width: 768, height: 1024 },   // iPad
    desktop: { width: 1440, height: 900 }   // Desktop
  };

  const { width, height } = dimensions[deviceType as keyof typeof dimensions];
  
  return orientation === 'landscape' 
    ? { width: height, height: width }
    : { width, height };
};

// hooks/useDebounce.ts
import { useState, useEffect } from 'react';

export function useDebounce<T>(value: T, delay: number): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
}

// hooks/useNotification.ts
import { useCallback } from 'react';
import { toast } from 'react-toastify';

export const useNotification = () => {
  const showSuccess = useCallback((message: string) => {
    toast.success(message, {
      position: 'top-right',
      autoClose: 3000,
      hideProgressBar: false,
      closeOnClick: true,
      pauseOnHover: true,
      draggable: true
    });
  }, []);

  const showError = useCallback((message: string) => {
    toast.error(message, {
      position: 'top-right',
      autoClose: 5000,
      hideProgressBar: false,
      closeOnClick: true,
      pauseOnHover: true,
      draggable: true
    });
  }, []);

  const showWarning = useCallback((message: string) => {
    toast.warning(message, {
      position: 'top-right',
      autoClose: 4000,
      hideProgressBar: false,
      closeOnClick: true,
      pauseOnHover: true,
      draggable: true
    });
  }, []);

  const showInfo = useCallback((message: string) => {
    toast.info(message, {
      position: 'top-right',
      autoClose: 3000,
      hideProgressBar: false,
      closeOnClick: true,
      pauseOnHover: true,
      draggable: true
    });
  }, []);

  return {
    showSuccess,
    showError,
    showWarning,
    showInfo
  };
  
  // utils/componentFactory.ts
  
import React from 'react';
import {
  ViewModule,
  Image,
  Category,
  List,
  Search,
  LocalOffer,
  TextFields,
  Collections,
  Map,
  FilterList,
  Home,
  Carousel,
  Dashboard,
  ShoppingCart,
  Person,
  Settings
} from '@mui/icons-material';
import { ComponentType, ComponentProperty } from '../types/homeScreen.types';
import { ComponentTypeDefinition } from '../types/component.types';

// Dynamic component imports
import Banner from '../components/DynamicComponents/Banner';
import CarouselComponent from '../components/DynamicComponents/Carousel';
import CategoryGrid from '../components/DynamicComponents/CategoryGrid';
import PropertyList from '../components/DynamicComponents/PropertyList';
import SearchBar from '../components/DynamicComponents/SearchBar';
import OfferCard from '../components/DynamicComponents/OfferCard';
import TextBlock from '../components/DynamicComponents/TextBlock';
import ImageGallery from '../components/DynamicComponents/ImageGallery';
import MapView from '../components/DynamicComponents/MapView';
import FilterBar from '../components/DynamicComponents/FilterBar';

// Component registry with metadata
export const componentRegistry: Record<string, ComponentTypeDefinition> = {
  Banner: {
    type: 'Banner',
    name: 'Banner',
    description: 'Hero banner with image and text overlay',
    icon: <Image />,
    category: 'Display',
    defaultColSpan: 12,
    defaultRowSpan: 2,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'imageUrl',
        name: 'Image URL',
        type: 'image',
        isRequired: true,
        defaultValue: '',
        description: 'Banner background image'
      },
      {
        key: 'title',
        name: 'Title',
        type: 'text',
        isRequired: false,
        defaultValue: '',
        description: 'Banner title text'
      },
      {
        key: 'subtitle',
        name: 'Subtitle',
        type: 'text',
        isRequired: false,
        defaultValue: '',
        description: 'Banner subtitle text'
      },
      {
        key: 'ctaText',
        name: 'CTA Text',
        type: 'text',
        isRequired: false,
        defaultValue: 'Learn More',
        description: 'Call to action button text'
      },
      {
        key: 'ctaLink',
        name: 'CTA Link',
        type: 'text',
        isRequired: false,
        defaultValue: '',
        description: 'Call to action button link'
      },
      {
        key: 'height',
        name: 'Height',
        type: 'number',
        isRequired: false,
        defaultValue: 300,
        description: 'Banner height in pixels'
      },
      {
        key: 'overlayOpacity',
        name: 'Overlay Opacity',
        type: 'number',
        isRequired: false,
        defaultValue: 0.5,
        description: 'Dark overlay opacity (0-1)'
      }
    ],
    defaultStyles: {
      width: '100%',
      position: 'relative',
      overflow: 'hidden'
    }
  },

  Carousel: {
    type: 'Carousel',
    name: 'Carousel',
    description: 'Sliding carousel for multiple items',
    icon: <ViewModule />,
    category: 'Display',
    defaultColSpan: 12,
    defaultRowSpan: 2,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'autoPlay',
        name: 'Auto Play',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Enable automatic sliding'
      },
      {
        key: 'interval',
        name: 'Interval',
        type: 'number',
        isRequired: false,
        defaultValue: 3000,
        description: 'Auto play interval in milliseconds'
      },
      {
        key: 'showIndicators',
        name: 'Show Indicators',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show slide indicators'
      },
      {
        key: 'showArrows',
        name: 'Show Arrows',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show navigation arrows'
      },
      {
        key: 'itemsPerSlide',
        name: 'Items Per Slide',
        type: 'number',
        isRequired: false,
        defaultValue: 1,
        description: 'Number of items to show per slide'
      }
    ]
  },

  CategoryGrid: {
    type: 'CategoryGrid',
    name: 'Category Grid',
    description: 'Grid layout for property categories',
    icon: <Category />,
    category: 'Navigation',
    defaultColSpan: 12,
    defaultRowSpan: 2,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'columns',
        name: 'Columns',
        type: 'number',
        isRequired: false,
        defaultValue: 4,
        description: 'Number of columns in grid'
      },
      {
        key: 'showLabels',
        name: 'Show Labels',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show category labels'
      },
      {
        key: 'iconSize',
        name: 'Icon Size',
        type: 'select',
        isRequired: false,
        defaultValue: 'medium',
        options: ['small', 'medium', 'large'],
        description: 'Category icon size'
      },
      {
        key: 'shape',
        name: 'Shape',
        type: 'select',
        isRequired: false,
        defaultValue: 'circle',
        options: ['circle', 'square', 'rounded'],
        description: 'Category icon shape'
      }
    ]
  },

  PropertyList: {
    type: 'PropertyList',
    name: 'Property List',
    description: 'List of properties with filters',
    icon: <List />,
    category: 'Data',
    defaultColSpan: 12,
    defaultRowSpan: 3,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'layout',
        name: 'Layout',
        type: 'select',
        isRequired: false,
        defaultValue: 'list',
        options: ['list', 'grid', 'card'],
        description: 'List layout style'
      },
      {
        key: 'itemsPerPage',
        name: 'Items Per Page',
        type: 'number',
        isRequired: false,
        defaultValue: 10,
        description: 'Number of items to display per page'
      },
      {
        key: 'showFilters',
        name: 'Show Filters',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show filter options'
      },
      {
        key: 'showSorting',
        name: 'Show Sorting',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show sorting options'
      },
      {
        key: 'showPagination',
        name: 'Show Pagination',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show pagination controls'
      }
    ]
  },

  SearchBar: {
    type: 'SearchBar',
    name: 'Search Bar',
    description: 'Search input with suggestions',
    icon: <Search />,
    category: 'Input',
    defaultColSpan: 12,
    defaultRowSpan: 1,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'placeholder',
        name: 'Placeholder',
        type: 'text',
        isRequired: false,
        defaultValue: 'Search properties...',
        description: 'Search input placeholder text'
      },
      {
        key: 'showSuggestions',
        name: 'Show Suggestions',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Enable search suggestions'
      },
      {
        key: 'showFilters',
        name: 'Show Filters',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show advanced filter button'
      },
      {
        key: 'searchOnType',
        name: 'Search On Type',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Perform search while typing'
      },
      {
        key: 'debounceDelay',
        name: 'Debounce Delay',
        type: 'number',
        isRequired: false,
        defaultValue: 300,
        description: 'Delay before search in milliseconds'
      }
    ]
  },

  OfferCard: {
    type: 'OfferCard',
    name: 'Offer Card',
    description: 'Promotional offer display card',
    icon: <LocalOffer />,
    category: 'Display',
    defaultColSpan: 6,
    defaultRowSpan: 2,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'title',
        name: 'Title',
        type: 'text',
        isRequired: true,
        defaultValue: 'Special Offer',
        description: 'Offer title'
      },
      {
        key: 'description',
        name: 'Description',
        type: 'text',
        isRequired: false,
        defaultValue: '',
        description: 'Offer description'
      },
      {
        key: 'discount',
        name: 'Discount',
        type: 'text',
        isRequired: false,
        defaultValue: '20% OFF',
        description: 'Discount amount or percentage'
      },
      {
        key: 'validUntil',
        name: 'Valid Until',
        type: 'date',
        isRequired: false,
        defaultValue: '',
        description: 'Offer expiry date'
      },
      {
        key: 'backgroundColor',
        name: 'Background Color',
        type: 'color',
        isRequired: false,
        defaultValue: '#ff6b6b',
        description: 'Card background color'
      }
    ]
  },

  TextBlock: {
    type: 'TextBlock',
    name: 'Text Block',
    description: 'Rich text content block',
    icon: <TextFields />,
    category: 'Display',
    defaultColSpan: 12,
    defaultRowSpan: 1,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'content',
        name: 'Content',
        type: 'text',
        isRequired: true,
        defaultValue: '',
        description: 'Text content (supports markdown)'
      },
      {
        key: 'textAlign',
        name: 'Text Align',
        type: 'select',
        isRequired: false,
        defaultValue: 'left',
        options: ['left', 'center', 'right', 'justify'],
        description: 'Text alignment'
      },
      {
        key: 'fontSize',
        name: 'Font Size',
        type: 'select',
        isRequired: false,
        defaultValue: 'medium',
        options: ['small', 'medium', 'large', 'x-large'],
        description: 'Text size'
      },
      {
        key: 'fontWeight',
        name: 'Font Weight',
        type: 'select',
        isRequired: false,
        defaultValue: 'normal',
        options: ['normal', 'bold', 'light'],
        description: 'Text weight'
      }
    ]
  },

  ImageGallery: {
    type: 'ImageGallery',
    name: 'Image Gallery',
    description: 'Grid or slider image gallery',
    icon: <Collections />,
    category: 'Display',
    defaultColSpan: 12,
    defaultRowSpan: 3,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'layout',
        name: 'Layout',
        type: 'select',
        isRequired: false,
        defaultValue: 'grid',
        options: ['grid', 'masonry', 'slider'],
        description: 'Gallery layout style'
      },
      {
        key: 'columns',
        name: 'Columns',
        type: 'number',
        isRequired: false,
        defaultValue: 3,
        description: 'Number of columns (grid layout)'
      },
      {
        key: 'spacing',
        name: 'Spacing',
        type: 'number',
        isRequired: false,
        defaultValue: 8,
        description: 'Space between images in pixels'
      },
      {
        key: 'showCaptions',
        name: 'Show Captions',
        type: 'boolean',
        isRequired: false,
        defaultValue: false,
        description: 'Display image captions'
      },
      {
        key: 'enableLightbox',
        name: 'Enable Lightbox',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Open images in lightbox on click'
      }
    ]
  },

  MapView: {
    type: 'MapView',
    name: 'Map View',
    description: 'Interactive map with property markers',
    icon: <Map />,
    category: 'Data',
    defaultColSpan: 12,
    defaultRowSpan: 3,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'center',
        name: 'Center',
        type: 'object',
        isRequired: false,
        defaultValue: { lat: 0, lng: 0 },
        description: 'Map center coordinates'
      },
      {
        key: 'zoom',
        name: 'Zoom Level',
        type: 'number',
        isRequired: false,
        defaultValue: 10,
        description: 'Initial zoom level (1-20)'
      },
      {
        key: 'showControls',
        name: 'Show Controls',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show zoom and navigation controls'
      },
      {
        key: 'showMarkers',
        name: 'Show Markers',
        type: 'boolean',
        isRequired: false,
        defaultValue: true

// utils/componentFactory.ts (continued)
        description: 'Display property markers on map'
      },
      {
        key: 'clusterMarkers',
        name: 'Cluster Markers',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Group nearby markers into clusters'
      },
      {
        key: 'mapStyle',
        name: 'Map Style',
        type: 'select',
        isRequired: false,
        defaultValue: 'standard',
        options: ['standard', 'satellite', 'hybrid', 'terrain'],
        description: 'Map display style'
      }
    ]
  },

  FilterBar: {
    type: 'FilterBar',
    name: 'Filter Bar',
    description: 'Property filter controls',
    icon: <FilterList />,
    category: 'Input',
    defaultColSpan: 12,
    defaultRowSpan: 1,
    allowResize: true,
    supportedPlatforms: ['iOS', 'Android', 'Web'],
    properties: [
      {
        key: 'layout',
        name: 'Layout',
        type: 'select',
        isRequired: false,
        defaultValue: 'horizontal',
        options: ['horizontal', 'vertical', 'dropdown'],
        description: 'Filter bar layout'
      },
      {
        key: 'showClearAll',
        name: 'Show Clear All',
        type: 'boolean',
        isRequired: false,
        defaultValue: true,
        description: 'Show clear all filters button'
      },
      {
        key: 'collapsible',
        name: 'Collapsible',
        type: 'boolean',
        isRequired: false,
        defaultValue: false,
        description: 'Allow filter bar to be collapsed'
      },
      {
        key: 'sticky',
        name: 'Sticky',
        type: 'boolean',
        isRequired: false,
        defaultValue: false,
        description: 'Make filter bar sticky on scroll'
      }
    ]
  }
};

// Component factory function
export const createComponent = (componentData: {
  type: ComponentType;
  properties?: Record<string, any>;
  styles?: React.CSSProperties;
  dataSource?: any;
  actions?: any[];
}) => {
  const { type, properties = {}, styles = {}, dataSource, actions } = componentData;
  
  // Get component definition
  const componentDef = componentRegistry[type];
  if (!componentDef) {
    console.warn(`Unknown component type: ${type}`);
    return null;
  }

  // Merge default properties with provided properties
  const mergedProps = { ...getDefaultProps(componentDef), ...properties };

  // Component mapping
  const componentMap: Record<ComponentType, React.FC<any>> = {
    Banner: Banner,
    Carousel: CarouselComponent,
    CategoryGrid: CategoryGrid,
    PropertyList: PropertyList,
    SearchBar: SearchBar,
    OfferCard: OfferCard,
    TextBlock: TextBlock,
    ImageGallery: ImageGallery,
    MapView: MapView,
    FilterBar: FilterBar
  };

  const Component = componentMap[type];
  if (!Component) {
    console.warn(`Component implementation not found for type: ${type}`);
    return null;
  }

  // Return component with props
  return (
    <Component
      {...mergedProps}
      style={styles}
      dataSource={dataSource}
      actions={actions}
    />
  );
};

// Get default properties for a component type
export const getDefaultProps = (componentDef: ComponentTypeDefinition): Record<string, any> => {
  const defaults: Record<string, any> = {};
  
  componentDef.properties.forEach(prop => {
    if (prop.defaultValue !== undefined) {
      defaults[prop.key] = prop.defaultValue;
    }
  });
  
  return defaults;
};

// Validate component properties
export const validateComponentProps = (
  type: ComponentType,
  properties: ComponentProperty[]
): { valid: boolean; errors: string[] } => {
  const componentDef = componentRegistry[type];
  if (!componentDef) {
    return { valid: false, errors: [`Unknown component type: ${type}`] };
  }

  const errors: string[] = [];
  const propMap = new Map(properties.map(p => [p.propertyKey, p]));

  // Check required properties
  componentDef.properties.forEach(propDef => {
    if (propDef.isRequired) {
      const prop = propMap.get(propDef.key);
      if (!prop || prop.value === null || prop.value === undefined || prop.value === '') {
        errors.push(`Required property missing: ${propDef.name}`);
      }
    }
  });

  // Validate property types
  properties.forEach(prop => {
    const propDef = componentDef.properties.find(p => p.key === prop.propertyKey);
    if (propDef) {
      if (!isValidPropertyType(prop.value, propDef.type)) {
        errors.push(`Invalid type for property ${propDef.name}: expected ${propDef.type}`);
      }
    }
  });

  return { valid: errors.length === 0, errors };
};

// Check if property value matches expected type
const isValidPropertyType = (value: any, expectedType: string): boolean => {
  switch (expectedType) {
    case 'text':
    case 'image':
    case 'color':
      return typeof value === 'string';
    case 'number':
      return typeof value === 'number' && !isNaN(value);
    case 'boolean':
      return typeof value === 'boolean';
    case 'date':
      return value instanceof Date || !isNaN(Date.parse(value));
    case 'select':
    case 'multiselect':
      return true; // Validation should check against options
    case 'object':
      return typeof value === 'object' && value !== null;
    default:
      return true;
  }
};

// Get available components for a platform
export const getComponentsForPlatform = (platform: 'iOS' | 'Android' | 'Web'): ComponentTypeDefinition[] => {
  return Object.values(componentRegistry).filter(
    comp => comp.supportedPlatforms.includes(platform)
  );
};

// Get components by category
export const getComponentsByCategory = (category: string): ComponentTypeDefinition[] => {
  return Object.values(componentRegistry).filter(
    comp => comp.category === category
  );
};

// Create component preview
export const createComponentPreview = (type: ComponentType): React.ReactElement => {
  const componentDef = componentRegistry[type];
  if (!componentDef) {
    return <div>Unknown component</div>;
  }

  return (
    <div className="component-preview">
      <div className="component-preview-icon">{componentDef.icon}</div>
      <div className="component-preview-name">{componentDef.name}</div>
      <div className="component-preview-description">{componentDef.description}</div>
    </div>
  );
};

export default {
  componentRegistry,
  createComponent,
  getDefaultProps,
  validateComponentProps,
  getComponentsForPlatform,
  getComponentsByCategory,
  createComponentPreview
};