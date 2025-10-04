# Home Sections Specification

This document serves as a complete reference for the Home Sections feature, covering the dynamic sections displayed in the mobile app, their configuration, data sources (destinations and ads), API endpoints, data models, and suggested UI components/pages.

---

## 1. Dynamic Home Sections

### Data Model (TypeScript interface: `DynamicHomeSection`)

- **id** (string): Unique identifier of the section.
- **type** (string): Section type (e.g. `"SINGLE_PROPERTY_AD"`, `"FEATURED_PROPERTY_LIST"`, `"TEXT_BLOCK"`, etc.).
- **order** (number): Display order among sections.
- **isActive** (boolean): Whether the section is active and should be shown.
- **title?** (string): Section title (localized via application logic).
- **subtitle?** (string): Section subtitle.
- **config** (`Record<string, any>`): Arbitrary JSON config for layout, style, behavior, caching, etc.
- **metadata** (`Record<string, any>`): Additional metadata (tracking, analytics, custom tags).
- **scheduledAt?** (string, ISO8601): Date/time when section becomes visible.
- **expiresAt?** (string, ISO8601): Date/time when section expires.
- **targetAudience** (`string[]`): List of audiences (e.g. `"Guest"`,`"User"`,`"Premium"`).
- **priority** (number): Priority used for sorting if equal `order`.
- **content** (`DynamicContent[]`): List of content items within the section.
- **createdAt** (string, ISO8601): Timestamp when section was created.
- **updatedAt** (string, ISO8601): Timestamp when section was last updated.

### Content Items (TypeScript interface: `DynamicContent`)

- **id** (string): Unique identifier of the content item.
- **sectionId** (string): Parent section ID.
- **contentType** (string): Content type (e.g. `"PROPERTY"`,`"OFFER"`,`"ADVERTISEMENT"`,`"DESTINATION"`).
- **data** (`Record<string, any>`): Payload data for rendering (entity-specific fields).
- **metadata** (`Record<string, any>`): Additional metadata per content.
- **expiresAt?** (string, ISO8601): Optional expiry timestamp.
- **createdAt** (string): Creation timestamp.
- **updatedAt** (string): Last update timestamp.

### API Endpoints (Admin)

- **GET** `/api/admin/home-sections/dynamic-sections`
  - Query params: `language?`, `targetAudience?[]`, `includeContent?`, `onlyActive?`
  - Returns `DynamicHomeSection[]`

- **POST** `/api/admin/home-sections/dynamic-sections`
  - Body: `CreateDynamicSectionCommand`
  - Returns new section `id:string`

- **PUT** `/api/admin/home-sections/dynamic-sections/{id}`
  - Body: `UpdateDynamicSectionCommand`
  - Returns `boolean` success

- **POST** `/api/admin/home-sections/dynamic-sections/{id}/toggle?setActive={boolean}`
  - Toggles (or sets) active status.
  - Returns `boolean`

- **DELETE** `/api/admin/home-sections/dynamic-sections/{id}`
  - Deletes the section.
  - Returns `boolean`

- **POST** `/api/admin/home-sections/dynamic-sections/reorder`
  - Body: `ReorderDynamicSectionsCommand`
  - Returns `boolean`

---

## 2. Dynamic Home Configuration

### Data Model (TypeScript interface: `DynamicHomeConfig`)

- **id** (string)
- **version** (string)
- **isActive** (boolean)
- **createdAt** (string)
- **updatedAt** (string)
- **publishedAt?** (string)
- **globalSettings** (`Record<string,any>`)
- **themeSettings** (`Record<string,any>`)
- **layoutSettings** (`Record<string,any>`)
- **cacheSettings** (`Record<string,any>`)
- **analyticsSettings** (`Record<string,any>`)
- **enabledFeatures** (`string[]`)
- **experimentalFeatures** (`Record<string,any>`)

### API Endpoints (Admin)

- **GET** `/api/admin/home-sections/dynamic-config?version={version?}`
  - Returns `DynamicHomeConfig`

- **POST** `/api/admin/home-sections/dynamic-config`
  - Body: `CreateDynamicConfigCommand`
  - Returns new config `id:string`

- **PUT** `/api/admin/home-sections/dynamic-config/{id}`
  - Body: `UpdateDynamicConfigCommand`
  - Returns `boolean`

- **POST** `/api/admin/home-sections/dynamic-config/{id}/publish`
  - Publishes config version.
  - Returns `boolean`

---

## 3. City Destinations

### Data Model (TypeScript interface: `CityDestination`)

- **id** (string)
- **name** / **nameAr** (localized names)
- **country** / **countryAr**
- **description?** / **descriptionAr?**
- **imageUrl** (string)
- **additionalImages** (`string[]`)
- **latitude**, **longitude** (number)
- **propertyCount**, **reviewCount** (number)
- **averagePrice**, **averageRating** (number)
- **currency** (string)
- **isPopular**, **isFeatured** (boolean)
- **priority** (number)
- **highlights** / **highlightsAr** (`string[]`)
- **weatherData**, **attractionsData**, **metadata** (`Record<string,any>`)
- **createdAt**, **updatedAt** (string)
- **isActive** (boolean)
- **localizedFullName** (string)

### API Endpoints (Admin)

- **GET** `/api/admin/home-sections/city-destinations`
  - Query params: `language`, `onlyActive`, `onlyPopular`, `onlyFeatured`, `limit`, `sortBy`
  - Returns `CityDestination[]`

- **POST** `/api/admin/home-sections/city-destinations`
  - Body: `CreateCityDestinationCommand`
  - Returns new `id:string`

- **PUT** `/api/admin/home-sections/city-destinations/{id}`
  - Body: `UpdateCityDestinationCommand`
  - Returns `boolean`

- **PUT** `/api/admin/home-sections/city-destinations/{id}/stats`
  - Body: `UpdateCityDestinationStatsCommand`
  - Returns `boolean`

---



---

## 5. UI Considerations & Suggested Pages

### Admin UI

1. **Dynamic Sections**
   - List page: sortable table or drag-and-drop canvas of sections
   - Create/Edit form: fields for type (dropdown), order, title/subtitle, schedule, expiry, targetAudience (multi-select), priority
   - JSON editor or form-driven fields for `config` and `metadata`
   - Toggle active, delete, reorder controls

2. **Dynamic Config**
   - Versions list: table of versions with status
   - Create/Edit form: JSON editors or form fields for each settings group
   - Publish button per version

3. **City Destinations**
   - CRUD table with inline editing or modal form
   - Stats update action (modal or inline form)
   - List filters: active/popular



### Mobile Preview (Client)

- **Sections Canvas**: horizontal scroll for each section type
- **Component Previews**: render via `DynamicContent` -> actual component mapping
- **Config Preview**: theme and layout global settings applied
- **Destinations**: dedicated sections with map view, image gallery, or carousel

---

This spec equips a front-end builder agent with all necessary types, endpoints, and UI guidelines to implement a robust management interface for the Home Sections system.