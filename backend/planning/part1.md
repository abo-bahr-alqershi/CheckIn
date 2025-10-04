هذا المخطط
Backend Files (ASP.NET Core):
Domain/Entities:

* HomeScreenTemplate.cs
* HomeScreenSection.cs
* HomeScreenComponent.cs
* ComponentProperty.cs
* ComponentStyle.cs
* ComponentAction.cs
* ComponentDataSource.cs
* UserHomeScreen.cs

Application/Commands:

* CreateHomeScreenTemplateCommand.cs
* UpdateHomeScreenTemplateCommand.cs
* DeleteHomeScreenTemplateCommand.cs
* CreateHomeScreenSectionCommand.cs
* UpdateHomeScreenSectionCommand.cs
* DeleteHomeScreenSectionCommand.cs
* CreateHomeScreenComponentCommand.cs
* UpdateHomeScreenComponentCommand.cs
* DeleteHomeScreenComponentCommand.cs
* ReorderSectionsCommand.cs
* ReorderComponentsCommand.cs
* DuplicateTemplateCommand.cs
* PublishTemplateCommand.cs

Application/Queries:

* GetHomeScreenTemplatesQuery.cs
* GetHomeScreenTemplateByIdQuery.cs
* GetActiveHomeScreenQuery.cs
* GetComponentTypesQuery.cs
* GetDataSourcesQuery.cs
* PreviewHomeScreenQuery.cs

Application/DTOs:

* HomeScreenTemplateDto.cs
* HomeScreenSectionDto.cs
* HomeScreenComponentDto.cs
* ComponentPropertyDto.cs
* ComponentTypeDto.cs
* DataSourceDto.cs

Infrastructure/Repositories:

* HomeScreenRepository.cs
* IHomeScreenRepository.cs

Frontend Files (React TypeScript):
Types:

* types/homeScreen.types.ts
* types/component.types.ts
* types/dragDrop.types.ts

Services:

* services/homeScreenService.ts
* services/componentService.ts
* services/dataSourceService.ts

Hooks:

* hooks/useHomeScreenBuilder.ts
* hooks/useDragDrop.ts
* hooks/useComponentProperties.ts
* hooks/usePreview.ts

Components:

* components/HomeScreenBuilder/index.tsx
* components/HomeScreenBuilder/Canvas.tsx
* components/HomeScreenBuilder/ComponentPalette.tsx
* components/HomeScreenBuilder/PropertyPanel.tsx
* components/HomeScreenBuilder/SectionContainer.tsx
* components/HomeScreenBuilder/ComponentWrapper.tsx
* components/HomeScreenBuilder/PreviewModal.tsx
* components/HomeScreenBuilder/TemplateManager.tsx

Dynamic Components:

* components/DynamicComponents/Banner.tsx
* components/DynamicComponents/Carousel.tsx
* components/DynamicComponents/CategoryGrid.tsx
* components/DynamicComponents/PropertyList.tsx
* components/DynamicComponents/SearchBar.tsx
* components/DynamicComponents/OfferCard.tsx
* components/DynamicComponents/TextBlock.tsx
* components/DynamicComponents/ImageGallery.tsx
* components/DynamicComponents/MapView.tsx
* components/DynamicComponents/FilterBar.tsx

Pages:

* pages/HomeScreenBuilder/index.tsx

Utils:

* utils/dragDropUtils.ts
* utils/componentFactory.ts
* utils/styleGenerator.ts




وهذه الملفات المنفذة

using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class HomeScreenTemplate : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Version { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDefault { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public Guid? PublishedBy { get; private set; }
        public string Platform { get; private set; } // iOS, Android, All
        public string TargetAudience { get; private set; } // Guest, User, Premium
        public string MetaData { get; private set; } // JSON for additional settings
        
        private readonly List<HomeScreenSection> _sections = new();
        public IReadOnlyCollection<HomeScreenSection> Sections => _sections.AsReadOnly();

        protected HomeScreenTemplate() { }

        public HomeScreenTemplate(
            string name,
            string description,
            string version,
            string platform,
            string targetAudience,
            string metaData = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Version = version;
            Platform = platform;
            TargetAudience = targetAudience;
            MetaData = metaData;
            IsActive = false;
            IsDefault = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description, string metaData)
        {
            Name = name;
            Description = description;
            MetaData = metaData;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish(Guid userId)
        {
            IsActive = true;
            PublishedAt = DateTime.UtcNow;
            PublishedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddSection(HomeScreenSection section)
        {
            _sections.Add(section);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveSection(Guid sectionId)
        {
            _sections.RemoveAll(s => s.Id == sectionId);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class HomeScreenSection : BaseEntity
    {
        public Guid TemplateId { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public int Order { get; private set; }
        public bool IsVisible { get; private set; }
        public string BackgroundColor { get; private set; }
        public string BackgroundImage { get; private set; }
        public string Padding { get; private set; }
        public string Margin { get; private set; }
        public int MinHeight { get; private set; }
        public int MaxHeight { get; private set; }
        public string CustomStyles { get; private set; } // JSON
        public string Conditions { get; private set; } // JSON for visibility conditions
        
        public HomeScreenTemplate Template { get; private set; }
        
        private readonly List<HomeScreenComponent> _components = new();
        public IReadOnlyCollection<HomeScreenComponent> Components => _components.AsReadOnly();

        protected HomeScreenSection() { }

        public HomeScreenSection(
            Guid templateId,
            string name,
            string title,
            int order,
            string backgroundColor = null,
            string padding = "16",
            string margin = "0")
        {
            Id = Guid.NewGuid();
            TemplateId = templateId;
            Name = name;
            Title = title;
            Order = order;
            IsVisible = true;
            BackgroundColor = backgroundColor;
            Padding = padding;
            Margin = margin;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            string title,
            string subtitle,
            string backgroundColor,
            string backgroundImage,
            string padding,
            string margin,
            int minHeight,
            int maxHeight,
            string customStyles,
            string conditions)
        {
            Name = name;
            Title = title;
            Subtitle = subtitle;
            BackgroundColor = backgroundColor;
            BackgroundImage = backgroundImage;
            Padding = padding;
            Margin = margin;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
            CustomStyles = customStyles;
            Conditions = conditions;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrder(int newOrder)
        {
            Order = newOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddComponent(HomeScreenComponent component)
        {
            _components.Add(component);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveComponent(Guid componentId)
        {
            _components.RemoveAll(c => c.Id == componentId);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class HomeScreenComponent : BaseEntity
    {
        public Guid SectionId { get; private set; }
        public string ComponentType { get; private set; } // Banner, Carousel, CategoryGrid, etc.
        public string Name { get; private set; }
        public int Order { get; private set; }
        public bool IsVisible { get; private set; }
        public int ColSpan { get; private set; } // Grid columns (1-12)
        public int RowSpan { get; private set; } // Grid rows
        public string Alignment { get; private set; } // left, center, right
        public string CustomClasses { get; private set; }
        public string AnimationType { get; private set; }
        public int AnimationDuration { get; private set; }
        public string Conditions { get; private set; } // JSON
        
        public HomeScreenSection Section { get; private set; }
        
        private readonly List<ComponentProperty> _properties = new();
        public IReadOnlyCollection<ComponentProperty> Properties => _properties.AsReadOnly();
        
        private readonly List<ComponentStyle> _styles = new();
        public IReadOnlyCollection<ComponentStyle> Styles => _styles.AsReadOnly();
        
        private readonly List<ComponentAction> _actions = new();
        public IReadOnlyCollection<ComponentAction> Actions => _actions.AsReadOnly();
        
        public ComponentDataSource DataSource { get; private set; }

        protected HomeScreenComponent() { }

        public HomeScreenComponent(
            Guid sectionId,
            string componentType,
            string name,
            int order,
            int colSpan = 12,
            int rowSpan = 1)
        {
            Id = Guid.NewGuid();
            SectionId = sectionId;
            ComponentType = componentType;
            Name = name;
            Order = order;
            IsVisible = true;
            ColSpan = colSpan;
            RowSpan = rowSpan;
            Alignment = "left";
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            int colSpan,
            int rowSpan,
            string alignment,
            string customClasses,
            string animationType,
            int animationDuration,
            string conditions)
        {
            Name = name;
            ColSpan = colSpan;
            RowSpan = rowSpan;
            Alignment = alignment;
            CustomClasses = customClasses;
            AnimationType = animationType;
            AnimationDuration = animationDuration;
            Conditions = conditions;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrder(int newOrder)
        {
            Order = newOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddProperty(ComponentProperty property)
        {
            _properties.Add(property);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProperty(Guid propertyId, string value)
        {
            var property = _properties.FirstOrDefault(p => p.Id == propertyId);
            property?.UpdateValue(value);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddStyle(ComponentStyle style)
        {
            _styles.Add(style);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAction(ComponentAction action)
        {
            _actions.Add(action);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDataSource(ComponentDataSource dataSource)
        {
            DataSource = dataSource;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

using System;

namespace Domain.Entities
{
    public class ComponentProperty : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string PropertyKey { get; private set; }
        public string PropertyName { get; private set; }
        public string PropertyType { get; private set; } // text, number, boolean, select, color, image
        public string Value { get; private set; }
        public string DefaultValue { get; private set; }
        public bool IsRequired { get; private set; }
        public string ValidationRules { get; private set; } // JSON
        public string Options { get; private set; } // JSON for select options
        public string HelpText { get; private set; }
        public int Order { get; private set; }
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentProperty() { }

        public ComponentProperty(
            Guid componentId,
            string propertyKey,
            string propertyName,
            string propertyType,
            string defaultValue,
            bool isRequired,
            int order)
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            PropertyKey = propertyKey;
            PropertyName = propertyName;
            PropertyType = propertyType;
            Value = defaultValue;
            DefaultValue = defaultValue;
            IsRequired = isRequired;
            Order = order;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateValue(string value)
        {
            Value = value;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMetadata(
            string propertyName,
            bool isRequired,
            string validationRules,
            string options,
            string helpText)
        {
            PropertyName = propertyName;
            IsRequired = isRequired;
            ValidationRules = validationRules;
            Options = options;
            HelpText = helpText;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}using System;

namespace Domain.Entities
{
    public class ComponentStyle : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string StyleKey { get; private set; }
        public string StyleValue { get; private set; }
        public string Unit { get; private set; } // px, %, em, rem
        public bool IsImportant { get; private set; }
        public string MediaQuery { get; private set; } // Responsive styles
        public string State { get; private set; } // hover, active, focus
        public string Platform { get; private set; } // iOS, Android, All
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentStyle() { }

        public ComponentStyle(
            Guid componentId,
            string styleKey,
            string styleValue,
            string unit = null,
            string platform = "All")
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            StyleKey = styleKey;
            StyleValue = styleValue;
            Unit = unit;
            Platform = platform;
            IsImportant = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string styleValue,
            string unit,
            bool isImportant,
            string mediaQuery,
            string state)
        {
            StyleValue = styleValue;
            Unit = unit;
            IsImportant = isImportant;
            MediaQuery = mediaQuery;
            State = state;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
using System;

namespace Domain.Entities
{
    public class ComponentAction : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string ActionType { get; private set; } // Navigate, OpenModal, CallAPI, Share
        public string ActionTrigger { get; private set; } // Click, LongPress, Swipe
        public string ActionTarget { get; private set; } // Screen name, URL, API endpoint
        public string ActionParams { get; private set; } // JSON parameters
        public string Conditions { get; private set; } // JSON conditions
        public bool RequiresAuth { get; private set; }
        public string AnimationType { get; private set; }
        public int Priority { get; private set; }
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentAction() { }

        public ComponentAction(
            Guid componentId,
            string actionType,
            string actionTrigger,
            string actionTarget,
            int priority = 0)
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            ActionType = actionType;
            ActionTrigger = actionTrigger;
            ActionTarget = actionTarget;
            Priority = priority;
            RequiresAuth = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string actionTarget,
            string actionParams,
            string conditions,
            bool requiresAuth,
            string animationType)
        {
            ActionTarget = actionTarget;
            ActionParams = actionParams;
            Conditions = conditions;
            RequiresAuth = requiresAuth;
            AnimationType = animationType;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
using System;

namespace Domain.Entities
{
    public class ComponentDataSource : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string SourceType { get; private set; } // Static, API, Database, Cache
        public string DataEndpoint { get; private set; }
        public string HttpMethod { get; private set; }
        public string Headers { get; private set; } // JSON
        public string QueryParams { get; private set; } // JSON
        public string RequestBody { get; private set; } // JSON
        public string DataMapping { get; private set; } // JSON field mapping
        public string CacheKey { get; private set; }
        public int CacheDuration { get; private set; } // Minutes
        public string RefreshTrigger { get; private set; } // OnLoad, OnFocus, Manual, Timer
        public int RefreshInterval { get; private set; } // Seconds
        public string ErrorHandling { get; private set; } // JSON
        public string MockData { get; private set; } // JSON for development
        public bool UseMockInDev { get; private set; }
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentDataSource() { }

        public ComponentDataSource(
            Guid componentId,
            string sourceType,
            string dataEndpoint = null,
            string dataMapping = null)
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            SourceType = sourceType;
            DataEndpoint = dataEndpoint;
            DataMapping = dataMapping;
            HttpMethod = "GET";
            RefreshTrigger = "OnLoad";
            CacheDuration = 5;
            UseMockInDev = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateEndpoint(
            string dataEndpoint,
            string httpMethod,
            string headers,
            string queryParams,
            string requestBody)
        {
            DataEndpoint = dataEndpoint;
            HttpMethod = httpMethod;
            Headers = headers;
            QueryParams = queryParams;
            RequestBody = requestBody;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCaching(
            string cacheKey,
            int cacheDuration,
            string refreshTrigger,
            int refreshInterval)
        {
            CacheKey = cacheKey;
            CacheDuration = cacheDuration;
            RefreshTrigger = refreshTrigger;
            RefreshInterval = refreshInterval;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMapping(string dataMapping)
        {
            DataMapping = dataMapping;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetMockData(string mockData, bool useMockInDev)
        {
            MockData = mockData;
            UseMockInDev = useMockInDev;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
using System;

namespace Domain.Entities
{
    public class UserHomeScreen : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid TemplateId { get; private set; }
        public string CustomizationData { get; private set; } // JSON للتخصيصات الشخصية
        public string UserPreferences { get; private set; } // JSON
        public DateTime LastViewedAt { get; private set; }
        public string DeviceInfo { get; private set; } // JSON
        
        public User User { get; private set; }
        public HomeScreenTemplate Template { get; private set; }

        protected UserHomeScreen() { }

        public UserHomeScreen(
            Guid userId,
            Guid templateId,
            string deviceInfo = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TemplateId = templateId;
            DeviceInfo = deviceInfo;
            LastViewedAt = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateCustomization(string customizationData)
        {
            CustomizationData = customizationData;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePreferences(string userPreferences)
        {
            UserPreferences = userPreferences;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordView()
        {
            LastViewedAt = DateTime.UtcNow;
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Entities;
using Application.Common.Interfaces;
using Application.DTOs;

namespace Application.Commands
{
    public class CreateHomeScreenTemplateCommand : IRequest<HomeScreenTemplateDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
        public string TargetAudience { get; set; }
        public string MetaData { get; set; }
    }

    public class CreateHomeScreenTemplateCommandHandler : IRequestHandler<CreateHomeScreenTemplateCommand, HomeScreenTemplateDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateHomeScreenTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HomeScreenTemplateDto> Handle(CreateHomeScreenTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new HomeScreenTemplate(
                request.Name,
                request.Description,
                request.Version,
                request.Platform,
                request.TargetAudience,
                request.MetaData);

            template.CreatedBy = _currentUserService.UserId;

            await _repository.AddTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<HomeScreenTemplateDto>(template);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Entities;
using Application.Common.Interfaces;
using Application.DTOs;

namespace Application.Commands
{
    public class CreateHomeScreenSectionCommand : IRequest<HomeScreenSectionDto>
    {
        public Guid TemplateId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int Order { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Padding { get; set; }
        public string Margin { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public string CustomStyles { get; set; }
        public string Conditions { get; set; }
    }

    public class CreateHomeScreenSectionCommandHandler : IRequestHandler<CreateHomeScreenSectionCommand, HomeScreenSectionDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateHomeScreenSectionCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HomeScreenSectionDto> Handle(CreateHomeScreenSectionCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.TemplateId, cancellationToken);
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId);

            var section = new HomeScreenSection(
                request.TemplateId,
                request.Name,
                request.Title,
                request.Order,
                request.BackgroundColor,
                request.Padding ?? "16",
                request.Margin ?? "0");

            section.Update(
                request.Name,
                request.Title,
                request.Subtitle,
                request.BackgroundColor,
                request.BackgroundImage,
                request.Padding,
                request.Margin,
                request.MinHeight,
                request.MaxHeight,
                request.CustomStyles,
                request.Conditions);

            section.CreatedBy = _currentUserService.UserId;

            await _repository.AddSectionAsync(section, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<HomeScreenSectionDto>(section);
        }
    }
}
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Linq;

namespace Application.Commands
{
    public class CreateHomeScreenComponentCommand : IRequest<Guid>
    {
        public string ComponentType { get; set; }
        public string ComponentKey { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int DisplayOrder { get; set; }
        public int? RowIndex { get; set; }
        public int? ColumnIndex { get; set; }
        public int? ColumnSpan { get; set; }
        public string DataSource { get; set; }
        public string FilterCriteria { get; set; }
        public int? ItemLimit { get; set; }
        public string DisplayTemplate { get; set; }
        public string NavigationTarget { get; set; }
        public string NavigationParams { get; set; }
        public string CustomProperties { get; set; }
        public string StyleOverrides { get; set; }
        public string Permissions { get; set; }
        public string VisibilityConditions { get; set; }
        public bool IsActive { get; set; }
        public bool IsCacheable { get; set; }
        public int? CacheDurationMinutes { get; set; }
        public string LocalizationKey { get; set; }
        public Guid? ParentComponentId { get; set; }
    }

    public class CreateHomeScreenComponentCommandValidator : AbstractValidator<CreateHomeScreenComponentCommand>
    {
        public CreateHomeScreenComponentCommandValidator()
        {
            RuleFor(x => x.ComponentType)
                .NotEmpty().WithMessage("Component type is required")
                .MaximumLength(50).WithMessage("Component type must not exceed 50 characters");

            RuleFor(x => x.ComponentKey)
                .NotEmpty().WithMessage("Component key is required")
                .MaximumLength(100).WithMessage("Component key must not exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Component key can only contain alphanumeric characters, hyphens, and underscores");

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be non-negative");

            RuleFor(x => x.ColumnSpan)
                .InclusiveBetween(1, 12).When(x => x.ColumnSpan.HasValue)
                .WithMessage("Column span must be between 1 and 12");

            RuleFor(x => x.ItemLimit)
                .GreaterThan(0).When(x => x.ItemLimit.HasValue)
                .WithMessage("Item limit must be greater than 0");

            RuleFor(x => x.CacheDurationMinutes)
                .GreaterThan(0).When(x => x.IsCacheable && x.CacheDurationMinutes.HasValue)
                .WithMessage("Cache duration must be greater than 0 when caching is enabled");
        }
    }

    public class CreateHomeScreenComponentCommandHandler : IRequestHandler<CreateHomeScreenComponentCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public CreateHomeScreenComponentCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<Guid> Handle(CreateHomeScreenComponentCommand request, CancellationToken cancellationToken)
        {
            // Check if component key already exists
            var existingComponent = await _context.HomeScreenComponents
                .FirstOrDefaultAsync(x => x.ComponentKey == request.ComponentKey && !x.IsDeleted, cancellationToken);

            if (existingComponent != null)
            {
                throw new ValidationException($"Component with key '{request.ComponentKey}' already exists");
            }

            // Validate parent component if specified
            if (request.ParentComponentId.HasValue)
            {
                var parentExists = await _context.HomeScreenComponents
                    .AnyAsync(x => x.Id == request.ParentComponentId.Value && !x.IsDeleted, cancellationToken);

                if (!parentExists)
                {
                    throw new NotFoundException(nameof(HomeScreenComponent), request.ParentComponentId.Value);
                }
            }

            var component = new HomeScreenComponent
            {
                Id = Guid.NewGuid(),
                ComponentType = request.ComponentType,
                ComponentKey = request.ComponentKey,
                Title = request.Title,
                Subtitle = request.Subtitle,
                DisplayOrder = request.DisplayOrder,
                RowIndex = request.RowIndex,
                ColumnIndex = request.ColumnIndex,
                ColumnSpan = request.ColumnSpan,
                DataSource = request.DataSource,
                FilterCriteria = request.FilterCriteria,
                ItemLimit = request.ItemLimit,
                DisplayTemplate = request.DisplayTemplate,
                NavigationTarget = request.NavigationTarget,
                NavigationParams = request.NavigationParams,
                CustomProperties = request.CustomProperties,
                StyleOverrides = request.StyleOverrides,
                Permissions = request.Permissions,
                VisibilityConditions = request.VisibilityConditions,
                IsActive = request.IsActive,
                IsCacheable = request.IsCacheable,
                CacheDurationMinutes = request.CacheDurationMinutes,
                LocalizationKey = request.LocalizationKey,
                ParentComponentId = request.ParentComponentId,
                CreatedAt = _dateTime.Now,
                CreatedBy = _currentUserService.UserId ?? Guid.Empty,
                IsDeleted = false
            };

            _context.HomeScreenComponents.Add(component);
            await _context.SaveChangesAsync(cancellationToken);

            return component.Id;
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using Application.DTOs;
using FluentValidation;

namespace Application.Commands
{
    public class UpdateHomeScreenTemplateCommand : IRequest<HomeScreenTemplateDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaData { get; set; }
    }

    public class UpdateHomeScreenTemplateCommandValidator : AbstractValidator<UpdateHomeScreenTemplateCommand>
    {
        public UpdateHomeScreenTemplateCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Template ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Template name is required")
                .MaximumLength(200).WithMessage("Template name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        }
    }

    public class UpdateHomeScreenTemplateCommandHandler : IRequestHandler<UpdateHomeScreenTemplateCommand, HomeScreenTemplateDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateHomeScreenTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HomeScreenTemplateDto> Handle(UpdateHomeScreenTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.Id, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.Id);

            template.Update(request.Name, request.Description, request.MetaData);
            template.UpdatedBy = _currentUserService.UserId;

            await _repository.UpdateTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<HomeScreenTemplateDto>(template);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;

namespace Application.Commands
{
    public class DeleteHomeScreenTemplateCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteHomeScreenTemplateCommandHandler : IRequestHandler<DeleteHomeScreenTemplateCommand, Unit>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHomeScreenTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteHomeScreenTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.Id, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.Id);

            if (template.IsActive)
                throw new ConflictException("Cannot delete an active template. Please deactivate it first.");

            template.IsDeleted = true;
            template.DeletedBy = _currentUserService.UserId;
            template.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using Application.DTOs;
using FluentValidation;

namespace Application.Commands
{
    public class UpdateHomeScreenSectionCommand : IRequest<HomeScreenSectionDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Padding { get; set; }
        public string Margin { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public string CustomStyles { get; set; }
        public string Conditions { get; set; }
    }

    public class UpdateHomeScreenSectionCommandValidator : AbstractValidator<UpdateHomeScreenSectionCommand>
    {
        public UpdateHomeScreenSectionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Section ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Section name is required")
                .MaximumLength(100).WithMessage("Section name must not exceed 100 characters");

            RuleFor(x => x.MinHeight)
                .GreaterThanOrEqualTo(0).WithMessage("Min height must be non-negative");

            RuleFor(x => x.MaxHeight)
                .GreaterThanOrEqualTo(x => x.MinHeight)
                .When(x => x.MaxHeight > 0)
                .WithMessage("Max height must be greater than or equal to min height");
        }
    }

    public class UpdateHomeScreenSectionCommandHandler : IRequestHandler<UpdateHomeScreenSectionCommand, HomeScreenSectionDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateHomeScreenSectionCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HomeScreenSectionDto> Handle(UpdateHomeScreenSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repository.GetSectionByIdAsync(request.Id, cancellationToken);
            
            if (section == null)
                throw new NotFoundException(nameof(HomeScreenSection), request.Id);

            section.Update(
                request.Name,
                request.Title,
                request.Subtitle,
                request.BackgroundColor,
                request.BackgroundImage,
                request.Padding,
                request.Margin,
                request.MinHeight,
                request.MaxHeight,
                request.CustomStyles,
                request.Conditions);

            section.UpdatedBy = _currentUserService.UserId;

            await _repository.UpdateSectionAsync(section, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<HomeScreenSectionDto>(section);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;

namespace Application.Commands
{
    public class DeleteHomeScreenSectionCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteHomeScreenSectionCommandHandler : IRequestHandler<DeleteHomeScreenSectionCommand, Unit>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHomeScreenSectionCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteHomeScreenSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repository.GetSectionByIdAsync(request.Id, cancellationToken);
            
            if (section == null)
                throw new NotFoundException(nameof(HomeScreenSection), request.Id);

            // Check if section has components
            if (section.Components.Any())
                throw new ConflictException("Cannot delete section with existing components. Please remove all components first.");

            section.IsDeleted = true;
            section.DeletedBy = _currentUserService.UserId;
            section.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateSectionAsync(section, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using Application.DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands
{
    public class UpdateHomeScreenComponentCommand : IRequest<HomeScreenComponentDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Alignment { get; set; }
        public string CustomClasses { get; set; }
        public string AnimationType { get; set; }
        public int AnimationDuration { get; set; }
        public string Conditions { get; set; }
    }

    public class UpdateHomeScreenComponentCommandValidator : AbstractValidator<UpdateHomeScreenComponentCommand>
    {
        public UpdateHomeScreenComponentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Component ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Component name is required")
                .MaximumLength(100).WithMessage("Component name must not exceed 100 characters");

            RuleFor(x => x.ColSpan)
                .InclusiveBetween(1, 12).WithMessage("Column span must be between 1 and 12");

            RuleFor(x => x.RowSpan)
                .InclusiveBetween(1, 10).WithMessage("Row span must be between 1 and 10");

            RuleFor(x => x.AnimationDuration)
                .InclusiveBetween(0, 5000).WithMessage("Animation duration must be between 0 and 5000 ms");
        }
    }

    public class UpdateHomeScreenComponentCommandHandler : IRequestHandler<UpdateHomeScreenComponentCommand, HomeScreenComponentDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateHomeScreenComponentCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HomeScreenComponentDto> Handle(UpdateHomeScreenComponentCommand request, CancellationToken cancellationToken)
        {
            var component = await _repository.GetComponentByIdAsync(request.Id, cancellationToken);
            
            if (component == null)
                throw new NotFoundException(nameof(HomeScreenComponent), request.Id);

            component.Update(
                request.Name,
                request.ColSpan,
                request.RowSpan,
                request.Alignment,
                request.CustomClasses,
                request.AnimationType,
                request.AnimationDuration,
                request.Conditions);

            component.UpdatedBy = _currentUserService.UserId;

            await _repository.UpdateComponentAsync(component, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<HomeScreenComponentDto>(component);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;

namespace Application.Commands
{
    public class DeleteHomeScreenComponentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteHomeScreenComponentCommandHandler : IRequestHandler<DeleteHomeScreenComponentCommand, Unit>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHomeScreenComponentCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteHomeScreenComponentCommand request, CancellationToken cancellationToken)
        {
            var component = await _repository.GetComponentByIdAsync(request.Id, cancellationToken);
            
            if (component == null)
                throw new NotFoundException(nameof(HomeScreenComponent), request.Id);

            component.IsDeleted = true;
            component.DeletedBy = _currentUserService.UserId;
            component.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateComponentAsync(component, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using FluentValidation;

namespace Application.Commands
{
    public class ReorderSectionsCommand : IRequest<Unit>
    {
        public Guid TemplateId { get; set; }
        public List<SectionOrderDto> Sections { get; set; }
    }

    public class SectionOrderDto
    {
        public Guid SectionId { get; set; }
        public int NewOrder { get; set; }
    }

    public class ReorderSectionsCommandValidator : AbstractValidator<ReorderSectionsCommand>
    {
        public ReorderSectionsCommandValidator()
        {
            RuleFor(x => x.TemplateId)
                .NotEmpty().WithMessage("Template ID is required");

            RuleFor(x => x.Sections)
                .NotEmpty().WithMessage("Sections list is required")
                .Must(sections => sections.Select(s => s.NewOrder).Distinct().Count() == sections.Count)
                .WithMessage("All section orders must be unique");
        }
    }

    public class ReorderSectionsCommandHandler : IRequestHandler<ReorderSectionsCommand, Unit>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public ReorderSectionsCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(ReorderSectionsCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateWithSectionsAsync(request.TemplateId, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId);
            var sections = template.Sections.ToList();
            
            // Validate all section IDs exist
            var sectionIds = sections.Select(s => s.Id).ToHashSet();
            var requestedIds = request.Sections.Select(s => s.SectionId).ToHashSet();
            
            if (!requestedIds.IsSubsetOf(sectionIds))
                throw new ValidationException("One or more section IDs are invalid");

            // Update section orders
            foreach (var sectionOrder in request.Sections)
            {
                var section = sections.First(s => s.Id == sectionOrder.SectionId);
                section.UpdateOrder(sectionOrder.NewOrder);
                section.UpdatedBy = _currentUserService.UserId;
            }

            await _repository.UpdateSectionsAsync(sections, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using FluentValidation;

namespace Application.Commands
{
    public class ReorderComponentsCommand : IRequest<Unit>
    {
        public Guid SectionId { get; set; }
        public List<ComponentOrderDto> Components { get; set; }
    }

    public class ComponentOrderDto
    {
        public Guid ComponentId { get; set; }
        public int NewOrder { get; set; }
    }

    public class ReorderComponentsCommandValidator : AbstractValidator<ReorderComponentsCommand>
    {
        public ReorderComponentsCommandValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required");

            RuleFor(x => x.Components)
                .NotEmpty().WithMessage("Components list is required")
                .Must(components => components.Select(c => c.NewOrder).Distinct().Count() == components.Count)
                .WithMessage("All component orders must be unique");
        }
    }

    public class ReorderComponentsCommandHandler : IRequestHandler<ReorderComponentsCommand, Unit>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public ReorderComponentsCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(ReorderComponentsCommand request, CancellationToken cancellationToken)
        {
            var section = await _repository.GetSectionWithComponentsAsync(request.SectionId, cancellationToken);
            
            if (section == null)
                throw new NotFoundException(nameof(HomeScreenSection), request.SectionId);

            var components = section.Components.ToList();
            
            // Validate all component IDs exist
            var componentIds = components.Select(c => c.Id).ToHashSet();
            var requestedIds = request.Components.Select(c => c.ComponentId).ToHashSet();
            
            if (!requestedIds.IsSubsetOf(componentIds))
                throw new ValidationException("One or more component IDs are invalid");

            // Update component orders
            foreach (var componentOrder in request.Components)
            {
                var component = components.First(c => c.Id == componentOrder.ComponentId);
                component.UpdateOrder(componentOrder.NewOrder);
                component.UpdatedBy = _currentUserService.UserId;
            }

            await _repository.UpdateComponentsAsync(components, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using Application.DTOs;
using Domain.Entities;

namespace Application.Commands
{
    public class DuplicateTemplateCommand : IRequest<HomeScreenTemplateDto>
    {
        public Guid TemplateId { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
    }

    public class DuplicateTemplateCommandHandler : IRequestHandler<DuplicateTemplateCommand, HomeScreenTemplateDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public DuplicateTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HomeScreenTemplateDto> Handle(DuplicateTemplateCommand request, CancellationToken cancellationToken)
        {
            var sourceTemplate = await _repository.GetTemplateWithFullHierarchyAsync(request.TemplateId, cancellationToken);
            
            if (sourceTemplate == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId);

            // Create new template
            var newTemplate = new HomeScreenTemplate(
                request.NewName ?? $"{sourceTemplate.Name} - Copy",
                request.NewDescription ?? sourceTemplate.Description,
                sourceTemplate.Version,
                sourceTemplate.Platform,
                sourceTemplate.TargetAudience,
                sourceTemplate.MetaData);

            newTemplate.CreatedBy = _currentUserService.UserId;

            // Duplicate sections
            foreach (var sourceSection in sourceTemplate.Sections.OrderBy(s => s.Order))
            {
                var newSection = new HomeScreenSection(
                    newTemplate.Id,
                    sourceSection.Name,
                    sourceSection.Title,
                    sourceSection.Order,
                    sourceSection.BackgroundColor,
                    sourceSection.Padding,
                    sourceSection.Margin);

                newSection.Update(
                    sourceSection.Name,
                    sourceSection.Title,
                    sourceSection.Subtitle,
                    sourceSection.BackgroundColor,
                    sourceSection.BackgroundImage,
                    sourceSection.Padding,
                    sourceSection.Margin,
                    sourceSection.MinHeight,
                    sourceSection.MaxHeight,
                    sourceSection.CustomStyles,
                    sourceSection.Conditions);

                newSection.CreatedBy = _currentUserService.UserId;

                // Duplicate components
                foreach (var sourceComponent in sourceSection.Components.OrderBy(c => c.Order))
                {
                    var newComponent = new HomeScreenComponent(
                        newSection.Id,
                        sourceComponent.ComponentType,
                        sourceComponent.Name,
                        sourceComponent.Order,
                        sourceComponent.ColSpan,
                        sourceComponent.RowSpan);

                    newComponent.Update(
                        sourceComponent.Name,
                        sourceComponent.ColSpan,
                        sourceComponent.RowSpan,
                        sourceComponent.Alignment,
                        sourceComponent.CustomClasses,
                        sourceComponent.AnimationType,
                        sourceComponent.AnimationDuration,
                        sourceComponent.Conditions);

                    newComponent.CreatedBy = _currentUserService.UserId;

                    // Duplicate properties
                    foreach (var sourceProperty in sourceComponent.Properties)
                    {
                        var newProperty = new ComponentProperty(
                            newComponent.Id,
                            sourceProperty.PropertyKey,
                            sourceProperty.PropertyName,
                            sourceProperty.PropertyType,
                            sourceProperty.DefaultValue,
                            sourceProperty.IsRequired,
                            sourceProperty.Order);

                        newProperty.UpdateValue(sourceProperty.Value);
                        newProperty.UpdateMetadata(
                            sourceProperty.PropertyName,
                            sourceProperty.IsRequired,
                            sourceProperty.ValidationRules,
                            sourceProperty.Options,
                            sourceProperty.HelpText);

                        newProperty.CreatedBy = _currentUserService.UserId;
                        newComponent.AddProperty(newProperty);
                    }

                    // Duplicate styles
                    foreach (var sourceStyle in sourceComponent.Styles)
                    {
                        var newStyle = new ComponentStyle(
                            newComponent.Id,
                            sourceStyle.StyleKey,
                            sourceStyle.StyleValue,
                            sourceStyle.Unit,
                            sourceStyle.Platform);

                        newStyle.Update(
                            sourceStyle.StyleValue,
                            sourceStyle.Unit,
                            sourceStyle.IsImportant,
                            sourceStyle.MediaQuery,
                            sourceStyle.State);

                        newStyle.CreatedBy = _currentUserService.UserId;
                        newComponent.AddStyle(newStyle);
                    }

                    // Duplicate actions
                    foreach (var sourceAction in sourceComponent.Actions)
                    {
                        var newAction = new ComponentAction(
                            newComponent.Id,
                            sourceAction.ActionType,
                            sourceAction.ActionTrigger,
                            sourceAction.ActionTarget,
                            sourceAction.Priority);

                        newAction.Update(
                            sourceAction.ActionTarget,
                            sourceAction.ActionParams,
                            sourceAction.Conditions,
                            sourceAction.RequiresAuth,
                            sourceAction.AnimationType);

                        newAction.CreatedBy = _currentUserService.UserId;
                        newComponent.AddAction(newAction);
                    }

                    // Duplicate data source
                    if (sourceComponent.DataSource != null)
                    {
                        var newDataSource = new ComponentDataSource(
                            newComponent.Id,
                            sourceComponent.DataSource.SourceType,
                            sourceComponent.DataSource.DataEndpoint,
                            sourceComponent.DataSource.DataMapping);

                        newDataSource.UpdateEndpoint(
                            sourceComponent.DataSource.DataEndpoint,
                            sourceComponent.DataSource.HttpMethod,
                            sourceComponent.DataSource.Headers,
                            sourceComponent.DataSource.QueryParams,
                            sourceComponent.DataSource.RequestBody);

                        newDataSource.UpdateCaching(
                            sourceComponent.DataSource.CacheKey,
                            sourceComponent.DataSource.CacheDuration,
                            sourceComponent.DataSource.RefreshTrigger,
                            sourceComponent.DataSource.RefreshInterval);

                        newDataSource.SetMockData(
                            sourceComponent.DataSource.MockData,
                            sourceComponent.DataSource.UseMockInDev);

                        newDataSource.CreatedBy = _currentUserService.UserId;
                        newComponent.SetDataSource(newDataSource);
                    }

                    newSection.AddComponent(newComponent);
                }

                newTemplate.AddSection(newSection);
            }

            await _repository.AddTemplateAsync(newTemplate, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<HomeScreenTemplateDto>(newTemplate);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;

namespace Application.Commands
{
    public class PublishTemplateCommand : IRequest<Unit>
    {
        public Guid TemplateId { get; set; }
        public bool DeactivateOthers { get; set; }
    }

    public class PublishTemplateCommandHandler : IRequestHandler<PublishTemplateCommand, Unit>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public PublishTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(PublishTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.TemplateId, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId);

            // Validate template has at least one section
            var sections = await _repository.GetTemplateSectionsAsync(request.TemplateId, cancellationToken);
            if (!sections.Any())
                throw new ValidationException("Cannot publish template without sections");

            // Deactivate other templates if requested
            if (request.DeactivateOthers)
            {
                var activeTemplates = await _repository.GetActiveTemplatesAsync(
                    template.Platform, 
                    template.TargetAudience, 
                    cancellationToken);

                foreach (var activeTemplate in activeTemplates.Where(t => t.Id != request.TemplateId))
                {
                    activeTemplate.Unpublish();
                    activeTemplate.UpdatedBy = _currentUserService.UserId;
                    await _repository.UpdateTemplateAsync(activeTemplate, cancellationToken);
                }
            }

            template.Publish(_currentUserService.UserId.Value);
            await _repository.UpdateTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.DTOs;

namespace Application.Queries
{
    public class GetHomeScreenTemplatesQuery : IRequest<List<HomeScreenTemplateDto>>
    {
        public string Platform { get; set; }
        public string TargetAudience { get; set; }
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; }
    }

    public class GetHomeScreenTemplatesQueryHandler : IRequestHandler<GetHomeScreenTemplatesQuery, List<HomeScreenTemplateDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;

        public GetHomeScreenTemplatesQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<HomeScreenTemplateDto>> Handle(GetHomeScreenTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetTemplatesAsync(
                request.Platform,
                request.TargetAudience,
                request.IsActive,
                request.IncludeDeleted,
                cancellationToken);

            return _mapper.Map<List<HomeScreenTemplateDto>>(templates);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.Exceptions;
using Application.DTOs;

namespace Application.Queries
{
    public class GetHomeScreenTemplateByIdQuery : IRequest<HomeScreenTemplateDto>
    {
        public Guid TemplateId { get; set; }
        public bool IncludeHierarchy { get; set; }
    }

    public class GetHomeScreenTemplateByIdQueryHandler : IRequestHandler<GetHomeScreenTemplateByIdQuery, HomeScreenTemplateDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;

        public GetHomeScreenTemplateByIdQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<HomeScreenTemplateDto> Handle(GetHomeScreenTemplateByIdQuery request, CancellationToken cancellationToken)
                {
            var template = request.IncludeHierarchy
                ? await _repository.GetTemplateWithFullHierarchyAsync(request.TemplateId, cancellationToken)
                : await _repository.GetTemplateByIdAsync(request.TemplateId, cancellationToken);

            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId);

            return _mapper.Map<HomeScreenTemplateDto>(template);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.DTOs;

namespace Application.Queries
{
    public class GetActiveHomeScreenQuery : IRequest<HomeScreenTemplateDto>
    {
        public string Platform { get; set; }
        public string TargetAudience { get; set; }
        public Guid? UserId { get; set; }
    }

    public class GetActiveHomeScreenQueryHandler : IRequestHandler<GetActiveHomeScreenQuery, HomeScreenTemplateDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetActiveHomeScreenQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<HomeScreenTemplateDto> Handle(GetActiveHomeScreenQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId ?? _currentUserService.UserId;
            
            // First check for user-specific customization
            if (userId.HasValue)
            {
                var userScreen = await _repository.GetUserHomeScreenAsync(userId.Value, request.Platform, cancellationToken);
                if (userScreen != null)
                {
                    var userTemplate = await _repository.GetTemplateWithFullHierarchyAsync(userScreen.TemplateId, cancellationToken);
                    var dto = _mapper.Map<HomeScreenTemplateDto>(userTemplate);
                    
                    // Apply user customizations
                    if (!string.IsNullOrEmpty(userScreen.CustomizationData))
                    {
                        dto.CustomizationData = userScreen.CustomizationData;
                        dto.UserPreferences = userScreen.UserPreferences;
                    }
                    
                    return dto;
                }
            }

            // Get default active template
            var template = await _repository.GetActiveTemplateAsync(
                request.Platform,
                request.TargetAudience,
                cancellationToken);

            if (template == null)
            {
                // Get any default template as fallback
                template = await _repository.GetDefaultTemplateAsync(request.Platform, cancellationToken);
            }

            return _mapper.Map<HomeScreenTemplateDto>(template);
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.DTOs;

namespace Application.Queries
{
    public class GetComponentTypesQuery : IRequest<List<ComponentTypeDto>>
    {
        public string Platform { get; set; }
    }

    public class GetComponentTypesQueryHandler : IRequestHandler<GetComponentTypesQuery, List<ComponentTypeDto>>
    {
        public GetComponentTypesQueryHandler()
        {
        }

        public Task<List<ComponentTypeDto>> Handle(GetComponentTypesQuery request, CancellationToken cancellationToken)
        {
            var componentTypes = new List<ComponentTypeDto>
            {
                new ComponentTypeDto
                {
                    Type = "Banner",
                    Name = "Banner",
                    Description = "Full-width promotional banner",
                    Icon = "image",
                    Category = "Display",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 2,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "imageUrl",
                            Name = "Image URL",
                            Type = "image",
                            IsRequired = true,
                            DefaultValue = ""
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "title",
                            Name = "Title",
                            Type = "text",
                            IsRequired = false,
                            DefaultValue = ""
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "actionUrl",
                            Name = "Action URL",
                            Type = "text",
                            IsRequired = false,
                            DefaultValue = ""
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "Carousel",
                    Name = "Carousel",
                    Description = "Sliding carousel for multiple items",
                    Icon = "view_carousel",
                    Category = "Display",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 3,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "autoPlay",
                            Name = "Auto Play",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "interval",
                            Name = "Interval (seconds)",
                            Type = "number",
                            IsRequired = false,
                            DefaultValue = "5"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showIndicators",
                            Name = "Show Indicators",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "CategoryGrid",
                    Name = "Category Grid",
                    Description = "Grid layout for categories",
                    Icon = "grid_view",
                    Category = "Navigation",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 2,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "columns",
                            Name = "Columns",
                            Type = "select",
                            IsRequired = true,
                            DefaultValue = "4",
                            Options = new[] { "2", "3", "4", "6" }
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showLabels",
                            Name = "Show Labels",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "PropertyList",
                    Name = "Property List",
                    Description = "List of properties with filters",
                    Icon = "list",
                    Category = "Data",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 4,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "layout",
                            Name = "Layout",
                            Type = "select",
                            IsRequired = true,
                            DefaultValue = "grid",
                            Options = new[] { "grid", "list", "card" }
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "itemsPerPage",
                            Name = "Items Per Page",
                            Type = "number",
                            IsRequired = false,
                            DefaultValue = "10"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showFilters",
                            Name = "Show Filters",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "SearchBar",
                    Name = "Search Bar",
                    Description = "Search input with suggestions",
                    Icon = "search",
                    Category = "Input",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 1,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "placeholder",
                            Name = "Placeholder",
                            Type = "text",
                            IsRequired = false,
                            DefaultValue = "Search..."
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showSuggestions",
                            Name = "Show Suggestions",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "searchInFields",
                            Name = "Search Fields",
                            Type = "multiselect",
                            IsRequired = true,
                            DefaultValue = "name,description",
                            Options = new[] { "name", "description", "location", "tags" }
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "OfferCard",
                    Name = "Offer Card",
                    Description = "Special offers and deals",
                    Icon = "local_offer",
                    Category = "Display",
                    DefaultColSpan = 6,
                    DefaultRowSpan = 2,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "offerType",
                            Name = "Offer Type",
                            Type = "select",
                            IsRequired = true,
                            DefaultValue = "percentage",
                            Options = new[] { "percentage", "fixed", "bogo" }
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "offerValue",
                            Name = "Offer Value",
                            Type = "text",
                            IsRequired = true,
                            DefaultValue = ""
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "expiryDate",
                            Name = "Expiry Date",
                            Type = "date",
                            IsRequired = false,
                            DefaultValue = ""
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "MapView",
                    Name = "Map View",
                    Description = "Interactive map with markers",
                    Icon = "map",
                    Category = "Display",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 4,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "centerLat",
                            Name = "Center Latitude",
                            Type = "number",
                            IsRequired = true,
                            DefaultValue = "0"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "centerLng",
                            Name = "Center Longitude",
                            Type = "number",
                            IsRequired = true,
                            DefaultValue = "0"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "zoom",
                            Name = "Zoom Level",
                            Type = "number",
                            IsRequired = false,
                            DefaultValue = "12"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showUserLocation",
                            Name = "Show User Location",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                }
            };

            // Filter by platform if specified
            if (!string.IsNullOrEmpty(request.Platform) && request.Platform != "All")
            {
                // In a real implementation, you might have platform-specific components
                // For now, return all components for all platforms
            }

            return Task.FromResult(componentTypes);
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.DTOs;
using Application.Common.Interfaces;

namespace Application.Queries
{
    public class GetDataSourcesQuery : IRequest<List<DataSourceDto>>
    {
        public string ComponentType { get; set; }
    }

    public class GetDataSourcesQueryHandler : IRequestHandler<GetDataSourcesQuery, List<DataSourceDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDataSourcesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DataSourceDto>> Handle(GetDataSourcesQuery request, CancellationToken cancellationToken)
        {
            var dataSources = new List<DataSourceDto>();

            // Static data sources
            dataSources.Add(new DataSourceDto
            {
                Id = "static",
                Name = "Static Data",
                Type = "Static",
                Description = "Manually configured data",
                IsAvailable = true,
                RequiresAuth = false,
                SupportedComponents = new[] { "All" }
            });

            // API data sources based on component type
            switch (request.ComponentType)
            {
                case "PropertyList":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "properties-api",
                        Name = "Properties API",
                        Type = "API",
                        Description = "Fetch properties from API",
                        Endpoint = "/api/properties",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "PropertyList", "Carousel" },
                        Parameters = new List<DataSourceParameter>
                        {
                            new DataSourceParameter
                            {
                                Key = "limit",
                                Name = "Limit",
                                Type = "number",
                                DefaultValue = "10",
                                IsRequired = false
                            },
                            new DataSourceParameter
                            {
                                Key = "sort",
                                Name = "Sort By",
                                Type = "select",
                                Options = new[] { "price", "rating", "date" },
                                DefaultValue = "rating",
                                IsRequired = false
                            }
                        }
                    });
                    break;

                case "CategoryGrid":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "categories-api",
                        Name = "Categories API",
                        Type = "API",
                        Description = "Fetch property categories",
                        Endpoint = "/api/property-types",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "CategoryGrid" }
                    });
                    break;

                case "OfferCard":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "offers-api",
                        Name = "Offers API",
                        Type = "API",
                        Description = "Fetch active offers",
                        Endpoint = "/api/offers",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "OfferCard", "Carousel" },
                        Parameters = new List<DataSourceParameter>
                        {
                            new DataSourceParameter
                            {
                                Key = "active",
                                Name = "Active Only",
                                Type = "boolean",
                                DefaultValue = "true",
                                IsRequired = false
                            }
                        }
                    });
                    break;

                case "MapView":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "map-properties-api",
                                                Name = "Map Properties API",
                        Type = "API",
                        Description = "Fetch properties with location data",
                        Endpoint = "/api/properties/map",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "MapView" },
                        Parameters = new List<DataSourceParameter>
                        {
                            new DataSourceParameter
                            {
                                Key = "bounds",
                                Name = "Map Bounds",
                                Type = "object",
                                IsRequired = true,
                                Description = "NE and SW coordinates"
                            },
                            new DataSourceParameter
                            {
                                Key = "maxMarkers",
                                Name = "Max Markers",
                                Type = "number",
                                DefaultValue = "100",
                                IsRequired = false
                            }
                        }
                    });
                    break;
            }

            // Common data sources
            dataSources.Add(new DataSourceDto
            {
                Id = "user-favorites",
                Name = "User Favorites",
                Type = "API",
                Description = "User's favorite properties",
                Endpoint = "/api/users/favorites",
                IsAvailable = true,
                RequiresAuth = true,
                SupportedComponents = new[] { "PropertyList", "Carousel" }
            });

            dataSources.Add(new DataSourceDto
            {
                Id = "recent-bookings",
                Name = "Recent Bookings",
                Type = "API",
                Description = "User's recent bookings",
                Endpoint = "/api/bookings/recent",
                IsAvailable = true,
                RequiresAuth = true,
                SupportedComponents = new[] { "PropertyList" }
            });

            dataSources.Add(new DataSourceDto
            {
                Id = "trending-properties",
                Name = "Trending Properties",
                Type = "API",
                Description = "Most viewed properties",
                Endpoint = "/api/properties/trending",
                IsAvailable = true,
                RequiresAuth = false,
                SupportedComponents = new[] { "PropertyList", "Carousel" },
                CacheDuration = 300 // 5 minutes
            });

            return await Task.FromResult(dataSources);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Interfaces;
using Application.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Application.Queries
{
    public class PreviewHomeScreenQuery : IRequest<HomeScreenPreviewDto>
    {
        public Guid TemplateId { get; set; }
        public string Platform { get; set; }
        public string DeviceType { get; set; }
        public bool UseMockData { get; set; }
    }

    public class PreviewHomeScreenQueryHandler : IRequestHandler<PreviewHomeScreenQuery, HomeScreenPreviewDto>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;

        public PreviewHomeScreenQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<HomeScreenPreviewDto> Handle(PreviewHomeScreenQuery request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateWithFullHierarchyAsync(request.TemplateId, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId);

            var preview = new HomeScreenPreviewDto
            {
                TemplateId = template.Id,
                TemplateName = template.Name,
                Platform = request.Platform ?? template.Platform,
                DeviceType = request.DeviceType ?? "mobile",
                Sections = new List<HomeScreenSectionPreviewDto>()
            };

            foreach (var section in template.Sections.OrderBy(s => s.Order))
            {
                var sectionPreview = new HomeScreenSectionPreviewDto
                {
                    Id = section.Id,
                    Name = section.Name,
                    Title = section.Title,
                    Subtitle = section.Subtitle,
                    Order = section.Order,
                    IsVisible = section.IsVisible,
                    Styles = GenerateSectionStyles(section),
                    Components = new List<HomeScreenComponentPreviewDto>()
                };

                foreach (var component in section.Components.Where(c => c.IsVisible).OrderBy(c => c.Order))
                {
                    var componentPreview = new HomeScreenComponentPreviewDto
                    {
                        Id = component.Id,
                        Type = component.ComponentType,
                        Name = component.Name,
                        Order = component.Order,
                        ColSpan = component.ColSpan,
                        RowSpan = component.RowSpan,
                        Alignment = component.Alignment,
                        Properties = GetComponentProperties(component),
                        Styles = GetComponentStyles(component, request.Platform),
                        Data = await GetComponentData(component, request.UseMockData, cancellationToken)
                    };

                    // Apply animations
                    if (!string.IsNullOrEmpty(component.AnimationType))
                    {
                        componentPreview.Animation = new AnimationConfig
                        {
                            Type = component.AnimationType,
                            Duration = component.AnimationDuration,
                            Delay = 0
                        };
                    }

                    sectionPreview.Components.Add(componentPreview);
                }

                preview.Sections.Add(sectionPreview);
            }

            // Add preview metadata
            preview.Metadata = new PreviewMetadata
            {
                GeneratedAt = DateTime.UtcNow,
                TotalSections = preview.Sections.Count,
                TotalComponents = preview.Sections.Sum(s => s.Components.Count),
                EstimatedLoadTime = CalculateEstimatedLoadTime(preview),
                UsedMockData = request.UseMockData
            };

            return preview;
        }

        private Dictionary<string, string> GenerateSectionStyles(HomeScreenSection section)
        {
            var styles = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(section.BackgroundColor))
                styles["backgroundColor"] = section.BackgroundColor;
            
            if (!string.IsNullOrEmpty(section.BackgroundImage))
                styles["backgroundImage"] = $"url({section.BackgroundImage})";
            
            if (!string.IsNullOrEmpty(section.Padding))
                styles["padding"] = $"{section.Padding}px";
            
            if (!string.IsNullOrEmpty(section.Margin))
                styles["margin"] = $"{section.Margin}px";
            
            if (section.MinHeight > 0)
                styles["minHeight"] = $"{section.MinHeight}px";
            
            if (section.MaxHeight > 0)
                styles["maxHeight"] = $"{section.MaxHeight}px";

            // Parse custom styles
            if (!string.IsNullOrEmpty(section.CustomStyles))
            {
                try
                {
                    var customStyles = JsonConvert.DeserializeObject<Dictionary<string, string>>(section.CustomStyles);
                    foreach (var style in customStyles)
                    {
                        styles[style.Key] = style.Value;
                    }
                }
                catch { }
            }

            return styles;
        }

        private Dictionary<string, object> GetComponentProperties(HomeScreenComponent component)
        {
            var properties = new Dictionary<string, object>();
            
            foreach (var prop in component.Properties)
            {
                properties[prop.PropertyKey] = prop.Value ?? prop.DefaultValue;
            }

            return properties;
        }

        private Dictionary<string, string> GetComponentStyles(HomeScreenComponent component, string platform)
        {
            var styles = new Dictionary<string, string>();
            
            var relevantStyles = component.Styles
                .Where(s => s.Platform == "All" || s.Platform == platform)
                .OrderBy(s => s.Platform == "All" ? 1 : 0); // Platform-specific styles override

            foreach (var style in relevantStyles)
            {
                var value = style.StyleValue;
                if (!string.IsNullOrEmpty(style.Unit))
                    value += style.Unit;
                
                if (style.IsImportant)
                    value += " !important";
                
                styles[style.StyleKey] = value;
            }

            return styles;
        }

        private async Task<object> GetComponentData(HomeScreenComponent component, bool useMockData, CancellationToken cancellationToken)
        {
            if (component.DataSource == null)
                return null;

            if (useMockData && !string.IsNullOrEmpty(component.DataSource.MockData))
            {
                try
                {
                    return JsonConvert.DeserializeObject(component.DataSource.MockData);
                }
                catch
                {
                    return null;
                }
            }

            // In preview mode, we'll return sample data based on component type
            return GetSampleDataForComponentType(component.ComponentType);
        }

        private object GetSampleDataForComponentType(string componentType)
        {
            switch (componentType)
            {
                case "PropertyList":
                    return new
                    {
                        items = new[]
                        {
                            new { id = 1, name = "Sample Property 1", price = "\$100/night", rating = 4.5 },
                            new { id = 2, name = "Sample Property 2", price = "\$150/night", rating = 4.8 },
                            new { id = 3, name = "Sample Property 3", price = "\$200/night", rating = 4.9 }
                        },
                        total = 3
                    };

                case "CategoryGrid":
                    return new
                    {
                        categories = new[]
                        {
                            new { id = 1, name = "Hotels", icon = "hotel", count = 150 },
                            new { id = 2, name = "Apartments", icon = "apartment", count = 200 },
                            new { id = 3, name = "Villas", icon = "villa", count = 50 },
                            new { id = 4, name = "Hostels", icon = "hostel", count = 75 }
                        }
                    };

                case "Banner":
                    return new
                    {
                        imageUrl = "https://via.placeholder.com/1200x400",
                        title = "Summer Sale",
                        subtitle = "Up to 50% off on selected properties"
                    };

                default:
                    return null;
            }
        }

        private int CalculateEstimatedLoadTime(HomeScreenPreviewDto preview)
        {
            // Simple estimation based on component count and data sources
            var baseTime = 100; // Base load time in ms
            var componentTime = preview.Sections.Sum(s => s.Components.Count) * 50;
            var dataSourceTime = preview.Sections
                .SelectMany(s => s.Components)
                .Count(c => c.Data != null) * 200;
            
            return baseTime + componentTime + dataSourceTime;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class HomeScreenTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? PublishedAt { get; set; }
        public Guid? PublishedBy { get; set; }
        public string PublishedByName { get; set; }
        public string Platform { get; set; }
        public string TargetAudience { get; set; }
        public string MetaData { get; set; }
        public string CustomizationData { get; set; }
        public string UserPreferences { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<HomeScreenSectionDto> Sections { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class HomeScreenSectionDto
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Padding { get; set; }
        public string Margin { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public string CustomStyles { get; set; }
        public string Conditions { get; set; }
        public List<HomeScreenComponentDto> Components { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class HomeScreenComponentDto
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string ComponentType { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Alignment { get; set; }
        public string CustomClasses { get; set; }
        public string AnimationType { get; set; }
        public int AnimationDuration { get; set; }
        public string Conditions { get; set; }
        public List<ComponentPropertyDto> Properties { get; set; }
        public List<ComponentStyleDto> Styles { get; set; }
        public List<ComponentActionDto> Actions { get; set; }
        public ComponentDataSourceDto DataSource { get; set; }
    }

