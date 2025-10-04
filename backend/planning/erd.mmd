erDiagram
    BaseEntity {
        Guid Id PK
        Guid CreatedBy FK
        DateTime CreatedAt
        Guid UpdatedBy FK
        DateTime UpdatedAt
        bool IsActive
        bool IsDeleted
        Guid DeletedBy FK
        DateTime DeletedAt
    }

    User {
        Guid Id PK
        string Name
        string Email
        string Password
        string Phone
        string ProfileImage
        DateTime CreatedAt
        bool IsActive
        DateTime LastLoginDate
        decimal TotalSpent
        string LoyaltyTier
        bool EmailConfirmed
        string EmailConfirmationToken
        DateTime EmailConfirmationTokenExpires
        string PasswordResetToken
        DateTime PasswordResetTokenExpires
        string SettingsJson
        string FavoritesJson
    }

    Role {
        Guid Id PK
        string Name
    }

    UserRole {
        Guid Id PK
        Guid UserId FK
        Guid RoleId FK
    }

    PropertyType {
        Guid Id PK
        string Name
        string Description
        string DefaultAmenities
    }

    Property {
        Guid Id PK
        Guid OwnerId FK
        Guid TypeId FK
        string Name
        string Address
        string City
        decimal Latitude
        decimal Longitude
        int StarRating
        string Description
        bool IsApproved
        DateTime CreatedAt
        int ViewCount
        int BookingCount
        decimal AverageRating
    }

    UnitType {
        Guid Id PK
        Guid PropertyTypeId FK
        string Name
        string Description
        string DefaultPricingRules
        int MaxCapacity
    }

    Unit {
        Guid Id PK
        Guid PropertyId FK
        Guid UnitTypeId FK
        string Name
        decimal BasePrice
        string Currency
        int MaxCapacity
        string CustomFeatures
        bool IsAvailable
        int ViewCount
        int BookingCount
        string PricingMethod
    }

    UnitTypeField {
        Guid Id PK
        Guid UnitTypeId FK
        string FieldKey
        string FieldName
        string FieldType
        bool IsRequired
        bool IsSearchable
        bool IsPublic
        string DefaultValue
        string ValidationRules
        string Options
        int SortOrder
        string HelpText
        string Placeholder
    }

    FieldGroup {
        Guid Id PK
        Guid UnitTypeId FK
        string GroupName
        string DisplayName
        string Description
        int SortOrder
        bool IsCollapsible
        bool IsExpandedByDefault
    }

    FieldGroupField {
        Guid Id PK
        Guid FieldId FK
        Guid GroupId FK
        int SortOrder
    }

    UnitFieldValue {
        Guid Id PK
        Guid UnitId FK
        Guid UnitTypeFieldId FK
        string FieldValue
    }

    SearchFilter {
        Guid Id PK
        Guid UnitTypeId FK
        Guid FieldId FK
        string FilterKey
        string FilterName
        string FilterType
        string Operators
        int SortOrder
        bool IsMainFilter
    }

    Booking {
        Guid Id PK
        Guid UserId FK
        Guid UnitId FK
        DateTime CheckIn
        DateTime CheckOut
        int GuestsCount
        decimal TotalPrice
        string Currency
        string Status
        DateTime BookedAt
        string BookingSource
        string CancellationReason
        bool IsWalkIn
        decimal PlatformCommissionAmount
        DateTime ActualCheckInDate
        DateTime ActualCheckOutDate
        decimal FinalAmount
        decimal CustomerRating
        string CompletionNotes
    }

    Payment {
        Guid Id PK
        Guid BookingId FK
        decimal Amount
        string Currency
        string Method
        string TransactionId
        string Status
        DateTime PaymentDate
        string GatewayTransactionId
        Guid ProcessedBy FK
    }

    PropertyService {
        Guid Id PK
        Guid PropertyId FK
        string Name
        decimal Price
        string Currency
        string PricingModel
    }

    BookingService {
        Guid Id PK
        Guid BookingId FK
        Guid ServiceId FK
        int Quantity
        decimal TotalPrice
        string Currency
    }

    Review {
        Guid Id PK
        Guid BookingId FK
        Guid PropertyId FK
        int Cleanliness
        int Service
        int Location
        int Value
        decimal AverageRating
        string Comment
        DateTime CreatedAt
        string ResponseText
        DateTime ResponseDate
        bool IsPendingApproval
    }

    ReviewImage {
        Guid Id PK
        Guid ReviewId FK
        string Name
        string Url
        long SizeBytes
        string Type
        string Category
        string Caption
        string AltText
        string Tags
        bool IsMain
        int DisplayOrder
        DateTime UploadedAt
        string Status
    }

    PropertyImage {
        Guid Id PK
        Guid PropertyId FK
        Guid UnitId FK
        string Name
        string Url
        long SizeBytes
        string Type
        string Category
        string Caption
        string AltText
        string Tags
        string Sizes
        bool IsMain
        int SortOrder
        int Views
        int Downloads
        DateTime UploadedAt
        int DisplayOrder
        string Status
        bool IsMainImage
    }

    Amenity {
        Guid Id PK
        string Name
        string Icon
        string Category
    }

    PropertyTypeAmenity {
        Guid Id PK
        Guid PropertyTypeId FK
        Guid AmenityId FK
        bool IsDefault
    }

    PropertyAmenity {
        Guid Id PK
        Guid PropertyId FK
        Guid PtaId FK
        bool IsAvailable
        decimal ExtraCost
        string Currency
    }

    PropertyPolicy {
        Guid Id PK
        Guid PropertyId FK
        string Type
        int CancellationWindowDays
        bool RequireFullPaymentBeforeConfirmation
        decimal MinimumDepositPercentage
        int MinHoursBeforeCheckIn
        string Description
        string Rules
    }

    Staff {
        Guid Id PK
        Guid PropertyId FK
        Guid UserId FK
        string Position
        string Permissions
        DateTime StartDate
        DateTime EndDate
        bool IsActive
    }

    PricingRule {
        Guid Id PK
        Guid UnitId FK
        string PriceType
        DateTime StartDate
        DateTime EndDate
        TimeSpan StartTime
        TimeSpan EndTime
        decimal PriceAmount
        string Currency
        string PricingTier
        decimal PercentageChange
        decimal MinPrice
        decimal MaxPrice
        string Description
    }

    UnitAvailability {
        Guid Id PK
        Guid UnitId FK
        Guid BookingId FK
        DateTime StartDate
        DateTime EndDate
        string Status
        string Reason
        string Notes
    }

    Notification {
        Guid Id PK
        string Type
        string Title
        string Message
        string TitleAr
        string MessageAr
        string Priority
        string Status
        Guid RecipientId FK
        Guid SenderId FK
        string Data
        string Channels
        string SentChannels
        bool IsRead
        bool IsDismissed
        bool RequiresAction
        bool CanDismiss
        DateTime ReadAt
        DateTime DismissedAt
        DateTime ScheduledFor
        DateTime ExpiresAt
        DateTime DeliveredAt
        string GroupId
        string BatchId
        string Icon
        string Color
        string ActionUrl
        string ActionText
    }

    Report {
        Guid Id PK
        Guid ReporterUserId FK
        Guid ReportedUserId FK
        Guid ReportedPropertyId FK
        string Reason
        string Description
        string Status
        string ActionNote
        Guid AdminId FK
    }

    ChatConversation {
        Guid Id PK
        string ConversationType
        string Title
        string Description
        string Avatar
        bool IsArchived
        bool IsMuted
        Guid PropertyId FK
    }

    ChatMessage {
        Guid Id PK
        Guid ConversationId FK
        Guid SenderId FK
        string MessageType
        string Content
        string LocationJson
        Guid ReplyToMessageId FK
        string Status
        bool IsEdited
        DateTime EditedAt
        DateTime DeliveredAt
        DateTime ReadAt
    }

    ChatAttachment {
        Guid Id PK
        Guid ConversationId FK
        string FileName
        string ContentType
        long FileSize
        string FilePath
        Guid UploadedBy FK
        DateTime CreatedAt
    }

    MessageReaction {
        Guid Id PK
        Guid MessageId FK
        Guid UserId FK
        string ReactionType
    }

    ChatSettings {
        Guid Id PK
        Guid UserId FK
        bool NotificationsEnabled
        bool SoundEnabled
        bool ShowReadReceipts
        bool ShowTypingIndicator
        string Theme
        string FontSize
        bool AutoDownloadMedia
        bool BackupMessages
    }

    %% العلاقات
    User ||--o{ UserRole : has
    Role ||--o{ UserRole : has
    User ||--o{ Property : owns
    User ||--o{ Booking : makes
    User ||--o{ Staff : works_as
    User ||--o{ Report : makes
    User ||--o{ Report : reported_in
    User ||--o{ Notification : receives
    User ||--o{ Notification : sends
    User ||--o{ ChatSettings : has
    User }o--o{ ChatConversation : participates_in
    User ||--o{ ChatMessage : sends
    User ||--o{ MessageReaction : makes

    PropertyType ||--o{ Property : categorizes
    PropertyType ||--o{ UnitType : has
    PropertyType ||--o{ PropertyTypeAmenity : has

    Property ||--o{ Unit : contains
    Property ||--o{ PropertyService : offers
    Property ||--o{ PropertyPolicy : has
    Property ||--o{ Review : receives
    Property ||--o{ Staff : employs
    Property ||--o{ PropertyImage : has
    Property ||--o{ PropertyAmenity : has
    Property ||--o{ Report : reported_in
    Property ||--o{ ChatConversation : related_to

    UnitType ||--o{ Unit : categorizes
    UnitType ||--o{ UnitTypeField : has
    UnitType ||--o{ FieldGroup : has
    UnitType ||--o{ SearchFilter : has

    Unit ||--o{ Booking : booked_in
    Unit ||--o{ PropertyImage : has
    Unit ||--o{ UnitAvailability : has
    Unit ||--o{ PricingRule : has
    Unit ||--o{ UnitFieldValue : has

    UnitTypeField ||--o{ FieldGroupField : grouped_in
    UnitTypeField ||--o{ UnitFieldValue : stores
    UnitTypeField ||--o{ SearchFilter : used_in

    FieldGroup ||--o{ FieldGroupField : contains

    Booking ||--o{ Payment : has
    Booking ||--o{ BookingService : includes
    Booking ||--o{ Review : generates
    Booking ||--o{ UnitAvailability : blocks

    PropertyService ||--o{ BookingService : included_in

    Review ||--o{ ReviewImage : has

    Amenity ||--o{ PropertyTypeAmenity : used_in
    PropertyTypeAmenity ||--o{ PropertyAmenity : implemented_as

    ChatConversation ||--o{ ChatMessage : contains
    ChatConversation ||--o{ ChatAttachment : has

    ChatMessage ||--o{ MessageReaction : has
    ChatMessage ||--o{ ChatAttachment : has