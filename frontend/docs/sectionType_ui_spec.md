# SectionType UI Specification

This document details each `SectionType` from the mobile app, its data shape (`DynamicHomeSection` + `DynamicContent`), default config keys, content fields, and recommended front-end management UI (pages/components/forms).

---

## Section Types UI Spec

1. singlePropertyAd
2. multiPropertyAd
3. unitShowcaseAd
4. singlePropertyOffer
5. limitedTimeOffer
6. seasonalOffer
7. multiPropertyOffersGrid
8. offersCarousel
9. flashDeals
10. horizontalPropertyList
11. verticalPropertyGrid
12. mixedLayoutList
13. compactPropertyList
14. cityCardsGrid
15. destinationCarousel
16. exploreCities
17. premiumCarousel
18. interactiveShowcase

---

### Global UI Components
-
- **`SectionListPage`**: table or DnD canvas of sections
- **`SectionForm`**: dynamic form generator based on `type` schema
- **`ConfigPage`**: JSON editor with live preview
- **`DestinationsPage`**: CRUD table + stats modal
- **`AdsPage`**: preview cards + stats
-
- **Note**: Each `SectionForm` should:
- 1. Load defaultConfig from enum
- 2. Render common fields (id, order, schedule)
- 3. Render `content` editor per `contentType` schema
- 4. Render `config` panel (form fields matching defaultConfig keys)

## Suggested UI Pages & Components
The following pages and components should be generated to manage Home Sections:

### Pages
- **DynamicSectionsPage**: lists all dynamic sections, supports filtering, search, pagination
- **DynamicSectionFormPage**: create/edit form for a single dynamic section with section-specific inputs and JSON config panel
- **HomeConfigPage**: lists config versions, allows create/edit and publish
- **CityDestinationsPage**: CRUD table of city destinations with inline stats editor modal


### Reusable Components
For each SectionType, create a dedicated form component to handle its unique fields and content:
- **SinglePropertyAdForm**, **MultiPropertyAdForm**, **UnitShowcaseAdForm**
- **SinglePropertyOfferForm**, **LimitedTimeOfferForm**, **SeasonalOfferForm**, **FlashDealsForm**
- **HorizontalPropertyListForm**, **VerticalPropertyGridForm**, **MixedLayoutListForm**, etc.
- **CityCardsGridForm**, **DestinationCarouselForm**, **ExploreCitiesForm**
- **PremiumCarouselForm**, **InteractiveShowcaseForm**

Each form component should:
1. Load its defaultConfig from `SectionType.defaultConfig`
2. Expose inputs for all `DynamicHomeSection` fields (order, schedule, title, audience, etc.)
3. Provide controls to add/remove `DynamicContent` items, with nested editors based on `contentType`
4. Include a JSON viewer/editor for advanced `config` and `metadata` fields
5. Validate inputs according to the data model (e.g. date ranges, required fields)
6. On submission, call the corresponding service hook (e.g. `useCreateDynamicSection`, `useUpdateDynamicSection`)

Use this updated spec as the foundation for the AI agent to generate robust, type-safe management pages and components, ensuring 100% compatibility with the back-end contract.

---

## Detailed Data Model Reference

Below is a field-by-field breakdown of the core interfaces in `homeSections.types.ts`.

### DynamicHomeSection
```ts
interface DynamicHomeSection {
  id: string;            // section UUID
  type: string;          // one of SectionType values (e.g. 'SINGLE_PROPERTY_AD')
  order: number;         // display order index
  isActive: boolean;     // toggles visibility
  title?: string;        // localized title
  subtitle?: string;     // localized subtitle
  config: Record<string, any>;        // JSON object for layout/style/behavior settings
  metadata: Record<string, any>;      // analytics/tags
  scheduledAt?: string;  // ISO8601 show start
  expiresAt?: string;    // ISO8601 show end
  targetAudience: string[];          // e.g. ['Guest', 'User']
  priority: number;      // tie-breaker ordering
  content: DynamicContent[];         // list of content items
  createdAt: string;     // ISO8601 created timestamp
  updatedAt: string;     // ISO8601 last updated
}
```

### DynamicContent
```ts
interface DynamicContent {
  id: string;            // content UUID
  sectionId: string;     // back-reference to DynamicHomeSection.id
  contentType: string;   // e.g. 'PROPERTY', 'ADVERTISEMENT', 'DESTINATION'
  data: Record<string, any>;       // payload used by renderComponent()
  metadata: Record<string, any>;   // e.g. { priority: 1, tags: [...] }
  expiresAt?: string;    // optional content expiry
  createdAt: string;
  updatedAt: string;
}
```

### DynamicHomeConfig
```ts
interface DynamicHomeConfig {
  id: string;
  version: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  publishedAt?: string;
  globalSettings: Record<string, any>;
  themeSettings: Record<string, any>;
  layoutSettings: Record<string, any>;
  cacheSettings: Record<string, any>;
  analyticsSettings: Record<string, any>;
  enabledFeatures: string[];
  experimentalFeatures: Record<string, any>;
}
```

### CityDestination
```ts
interface CityDestination {
  id: string;
  name: string;
  nameAr: string;
  country: string;
  countryAr: string;
  description?: string;
  descriptionAr?: string;
  imageUrl: string;
  additionalImages: string[];
  latitude: number;
  longitude: number;
  propertyCount: number;
  averagePrice: number;
  currency: string;
  averageRating: number;
  reviewCount: number;
  isPopular: boolean;
  isFeatured: boolean;
  priority: number;
  highlights: string[];
  highlightsAr: string[];
  weatherData: Record<string, any>;
  attractionsData: Record<string, any>;
  metadata: Record<string, any>;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
  localizedFullName: string;
}
```

