erDiagram
    Users {
        TEXT UserId PK
        TEXT Name
        TEXT Email
        TEXT Password
        TEXT Phone
        TEXT ProfileImage
        datetime CreatedAt
        INTEGER IsActive
        datetime LastLoginDate
        TEXT TotalSpent
        TEXT LoyaltyTier
        INTEGER EmailConfirmed
        TEXT EmailConfirmationToken
        datetime EmailConfirmationTokenExpires
        TEXT PasswordResetToken
        datetime PasswordResetTokenExpires
        TEXT SettingsJson
        TEXT FavoritesJson
        TEXT CreatedBy
        TEXT UpdatedBy
        datetime UpdatedAt
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Roles {
        TEXT RoleId PK
        TEXT Name
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    UserRoles {
        TEXT UserId PK, FK
        TEXT RoleId PK, FK
        datetime AssignedAt
        TEXT UserRoleId
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Properties {
        TEXT PropertyId PK
        TEXT Address
        decimal AverageRating
        INTEGER BookingCount
        TEXT City
        datetime CreatedAt
        TEXT CreatedBy
        datetime DeletedAt
        TEXT DeletedBy
        TEXT Description
        INTEGER IsActive
        INTEGER IsApproved
        INTEGER IsDeleted
        decimal Latitude
        decimal Longitude
        TEXT Name
        TEXT OwnerId FK
        INTEGER StarRating
        TEXT TypeId FK
        TEXT UpdatedAt
        TEXT UpdatedBy
        INTEGER ViewCount
    }
    PropertyTypes {
        TEXT TypeId PK
        TEXT Name
        TEXT Description
        TEXT DefaultAmenities
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Amenities {
        TEXT AmenityId PK
        TEXT Name
        TEXT Description
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    PropertyTypeAmenities {
        TEXT PtaId PK
        TEXT PropertyTypeId FK
        TEXT AmenityId FK
        INTEGER IsDefault
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    PropertyAmenities {
        TEXT PaId PK
        TEXT PropertyId FK
        TEXT PtaId FK
        INTEGER IsAvailable
        TEXT ExtraCost_Amount
        TEXT ExtraCost_Currency
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Units {
        TEXT UnitId PK
        TEXT PropertyId FK
        TEXT UnitTypeId FK
        TEXT Name
        TEXT BasePrice_Amount
        TEXT BasePrice_Currency
        INTEGER MaxCapacity
        TEXT CustomFeatures
        INTEGER IsAvailable
        INTEGER ViewCount
        INTEGER BookingCount
        INTEGER PricingMethod
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    UnitTypes {
        TEXT UnitTypeId PK
        TEXT Description
        TEXT DefaultPricingRules
        TEXT PropertyTypeId FK
        TEXT Name
        INTEGER MaxCapacity
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    UnitTypeFields {
        TEXT FieldId PK
        TEXT UnitTypeId FK
        TEXT FieldTypeId
        TEXT FieldName
        TEXT DisplayName
        TEXT Description
        TEXT FieldOptions
        TEXT ValidationRules
        INTEGER IsRequired
        INTEGER IsSearchable
        INTEGER IsPublic
        INTEGER SortOrder
        TEXT Category
        INTEGER IsForUnits
        INTEGER IsPrimaryFilter
        INTEGER Priority
        INTEGER ShowInCards
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    UnitFieldValues {
        TEXT ValueId PK
        TEXT UnitId FK
        TEXT UnitTypeFieldId FK
        text FieldValue
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    FieldGroups {
        TEXT GroupId PK
        TEXT UnitTypeId FK
        TEXT GroupName
        TEXT DisplayName
        TEXT Description
        INTEGER SortOrder
        INTEGER IsCollapsible
        INTEGER IsExpandedByDefault
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    FieldGroupFields {
        TEXT FieldId PK, FK
        TEXT GroupId PK, FK
        INTEGER SortOrder
        TEXT Id
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    SearchFilters {
        TEXT FilterId PK
        TEXT FieldId FK
        TEXT FilterType
        TEXT DisplayName
        TEXT FilterOptions
        INTEGER IsActive
        INTEGER SortOrder
        TEXT UnitTypeId FK
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Bookings {
        TEXT BookingId PK
        TEXT UserId FK
        TEXT UnitId FK
        datetime CheckIn
        datetime CheckOut
        INTEGER GuestsCount
        TEXT TotalPrice_Amount
        TEXT TotalPrice_Currency
        INTEGER Status
        datetime BookedAt
        TEXT BookingSource
        TEXT CancellationReason
        INTEGER IsWalkIn
        TEXT PlatformCommissionAmount
        TEXT ActualCheckInDate
        TEXT ActualCheckOutDate
        TEXT FinalAmount
        TEXT CustomerRating
        TEXT CompletionNotes
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Reviews {
        TEXT Id PK
        decimal AverageRating
        TEXT BookingId FK
        INTEGER Cleanliness
        TEXT Comment
        datetime CreatedAt
        TEXT CreatedBy
        datetime DeletedAt
        TEXT DeletedBy
        INTEGER IsActive
        INTEGER IsDeleted
        INTEGER IsPendingApproval
        INTEGER Location
        TEXT ResponseDate
        TEXT ResponseText
        INTEGER Service
        TEXT UpdatedAt
        TEXT UpdatedBy
        INTEGER Value
    }
    ReviewImages {
        TEXT Id PK
        TEXT ReviewId FK
        TEXT Name
        TEXT Url
        INTEGER SizeBytes
        TEXT Type
        INTEGER Category
        TEXT Caption
        TEXT AltText
        TEXT Tags
        INTEGER IsMain
        INTEGER DisplayOrder
        datetime UploadedAt
        INTEGER Status
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }
    Payments {
        TEXT PaymentId PK
        TEXT BookingId FK
        TEXT Amount_Amount
        TEXT Amount_Currency
        INTEGER Method
        TEXT TransactionId
        INTEGER Status
        datetime PaymentDate
        TEXT GatewayTransactionId
        TEXT ProcessedBy
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    PropertyServices {
        TEXT ServiceId PK
        TEXT PropertyId FK
        TEXT Name
        TEXT Price_Amount
        TEXT Price_Currency
        INTEGER PricingModel
        TEXT CreatedBy
        datetime CreatedAt
        TEXT UpdatedBy
        datetime UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    BookingServices {
        TEXT BookingId PK, FK
        TEXT ServiceId PK, FK
        INTEGER Quantity
        TEXT TotalPrice_Amount
        TEXT TotalPrice_Currency
        TEXT BookingServiceId
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    PropertyPolicies {
        TEXT PolicyId PK
        TEXT PropertyId FK
        INTEGER Type
        INTEGER CancellationWindowDays
        INTEGER RequireFullPaymentBeforeConfirmation
        TEXT MinimumDepositPercentage
        INTEGER MinHoursBeforeCheckIn
        TEXT Description
        TEXT Rules
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    PropertyImages {
        TEXT ImageId PK
        TEXT PropertyId FK
        TEXT UnitId FK
        TEXT Name
        TEXT Url
        INTEGER SizeBytes
        TEXT Type
        INTEGER Category
        TEXT Caption
        TEXT AltText
        TEXT Tags
        TEXT Sizes
        INTEGER IsMain
        INTEGER SortOrder
        INTEGER Views
        INTEGER Downloads
        datetime UploadedAt
        INTEGER DisplayOrder
        INTEGER Status
        INTEGER IsMainImage
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Staff {
        TEXT StaffId PK
        TEXT UserId FK
        TEXT PropertyId FK
        INTEGER Position
        TEXT Permissions
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    AdminActions {
        TEXT ActionId PK
        TEXT AdminId FK
        TEXT TargetId
        INTEGER TargetType
        INTEGER ActionType
        datetime Timestamp
        TEXT Changes
        TEXT AdminId1 FK
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    AuditLogs {
        TEXT AuditLogId PK
        TEXT EntityType
        TEXT EntityId
        INTEGER Action
        TEXT OldValues
        TEXT NewValues
        TEXT PerformedBy FK
        TEXT Username
        TEXT IpAddress
        TEXT UserAgent
        TEXT Notes
        TEXT Metadata
        INTEGER IsSuccessful
        TEXT ErrorMessage
        INTEGER DurationMs
        TEXT SessionId
        TEXT RequestId
        TEXT Source
        TEXT CreatedBy
        datetime CreatedAt
        TEXT UpdatedBy
        datetime UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    Notifications {
        TEXT NotificationId PK
        TEXT Type
        TEXT Title
        TEXT Message
        TEXT TitleAr
        TEXT MessageAr
        TEXT Priority
        TEXT Status
        TEXT RecipientId FK
        TEXT SenderId FK
        TEXT Data
        TEXT Channels
        TEXT SentChannels
        INTEGER IsRead
        INTEGER IsDismissed
        INTEGER RequiresAction
        INTEGER CanDismiss
        datetime ReadAt
        datetime DismissedAt
        datetime ScheduledFor
        datetime ExpiresAt
        datetime DeliveredAt
        TEXT GroupId
        TEXT BatchId
        TEXT Icon
        TEXT Color
        TEXT ActionUrl
        TEXT ActionText
        TEXT CreatedBy
        datetime CreatedAt
        TEXT UpdatedBy
        datetime UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    SearchLogs {
        TEXT Id PK
        TEXT UserId FK
        TEXT SearchType
        TEXT CriteriaJson
        INTEGER ResultCount
        INTEGER PageNumber
        INTEGER PageSize
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }
    Reports {
        TEXT Id PK
        TEXT ReporterUserId FK
        TEXT ReportedUserId FK
        TEXT ReportedPropertyId FK
        TEXT Reason
        TEXT Description
        TEXT Status
        TEXT ActionNote
        TEXT AdminId
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }
    UnitAvailability {
        TEXT Id PK
        TEXT UnitId FK
        datetime StartDate
        datetime EndDate
        TEXT Status
        TEXT Reason
        TEXT Notes
        TEXT BookingId FK
        TEXT BookingId1 FK
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    PricingRules {
        TEXT PricingRuleId PK
        TEXT UnitId FK
        TEXT PriceType
        datetime StartDate
        datetime EndDate
        time StartTime
        time EndTime
        decimal PriceAmount
        TEXT PricingTier
        decimal PercentageChange
        decimal MinPrice
        decimal MaxPrice
        TEXT Description
        TEXT Currency
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        datetime DeletedAt
    }
    ChatSettings {
        TEXT Id PK
        TEXT UserId FK
        INTEGER NotificationsEnabled
        INTEGER SoundEnabled
        INTEGER ShowReadReceipts
        INTEGER ShowTypingIndicator
        TEXT Theme
        TEXT FontSize
        INTEGER AutoDownloadMedia
        INTEGER BackupMessages
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }
    ChatConversations {
        TEXT Id PK
        TEXT ConversationType
        TEXT Title
        TEXT Description
        TEXT Avatar
        INTEGER IsArchived
        INTEGER IsMuted
        TEXT PropertyId FK
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }
    ChatConversationParticipant {
        TEXT ConversationId PK, FK
        TEXT UserId PK, FK
    }
    ChatMessages {
        TEXT Id PK
        TEXT ConversationId FK
        TEXT SenderId
        TEXT MessageType
        TEXT Content
        TEXT Location
        TEXT ReplyToMessageId
        TEXT Status
        INTEGER IsEdited
        TEXT EditedAt
        TEXT DeliveredAt
        TEXT ReadAt
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }
    ChatAttachments {
        TEXT Id PK
        TEXT ConversationId FK
        TEXT FileName
        TEXT ContentType
        INTEGER FileSize
        TEXT FilePath
        TEXT UploadedBy
        TEXT CreatedAt
        TEXT ChatMessageId FK
        TEXT CreatedBy
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }
    MessageReactions {
        TEXT Id PK
        TEXT MessageId FK
        TEXT UserId
        TEXT ReactionType
        TEXT CreatedBy
        TEXT CreatedAt
        TEXT UpdatedBy
        TEXT UpdatedAt
        INTEGER IsActive
        INTEGER IsDeleted
        TEXT DeletedBy
        TEXT DeletedAt
    }

    Users ||--|{ UserRoles : "FK_UserRoles_Users_UserId"
    Roles ||--|{ UserRoles : "FK_UserRoles_Roles_RoleId"
    Properties }|--|| Users : "FK_Properties_Users_OwnerId"
    Properties }|--|| PropertyTypes : "FK_Properties_PropertyTypes_TypeId"
    PropertyTypeAmenities }|--|| Amenities : "FK_PropertyTypeAmenities_Amenities_AmenityId"
    PropertyTypeAmenities }|--|| PropertyTypes : "FK_PropertyTypeAmenities_PropertyTypes_PropertyTypeId"
    PropertyAmenities }|--|| Properties : "FK_PropertyAmenities_Properties_PropertyId"
    PropertyAmenities }|--|| PropertyTypeAmenities : "FK_PropertyAmenities_PropertyTypeAmenities_PtaId"
    Units }|--|| Properties : "FK_Units_Properties_PropertyId"
    Units }|--|| UnitTypes : "FK_Units_UnitTypes_UnitTypeId"
    UnitTypes }|--|| PropertyTypes : "FK_UnitTypes_PropertyTypes_PropertyTypeId"
    UnitTypeFields }|--|| UnitTypes : "FK_UnitTypeFields_UnitTypes_UnitTypeId"
    UnitFieldValues }|--|| Units : "FK_UnitFieldValues_Units_UnitId"
    UnitFieldValues }|--|| UnitTypeFields : "FK_UnitFieldValues_UnitTypeFields_UnitTypeFieldId"
    FieldGroups }|--|| UnitTypes : "FK_FieldGroups_UnitTypes_UnitTypeId"
    FieldGroupFields }|--|| FieldGroups : "FK_FieldGroupFields_FieldGroups_GroupId"
    FieldGroupFields }|--|| UnitTypeFields : "FK_FieldGroupFields_UnitTypeFields_FieldId"
    SearchFilters }|--|| UnitTypeFields : "FK_SearchFilters_UnitTypeFields_FieldId"
    SearchFilters }|--o| UnitTypes : "FK_SearchFilters_UnitTypes_UnitTypeId"
    Bookings }|--|| Users : "FK_Bookings_Users_UserId"
    Bookings }|--|| Units : "FK_Bookings_Units_UnitId"
    Reviews }|--|| Bookings : "FK_Reviews_Bookings_BookingId"
    ReviewImages }|--|| Reviews : "FK_ReviewImages_Reviews_ReviewId"
    Payments }|--|| Bookings : "FK_Payments_Bookings_BookingId"
    PropertyServices }|--|| Properties : "FK_PropertyServices_Properties_PropertyId"
    BookingServices }|--|| Bookings : "FK_BookingServices_Bookings_BookingId"
    BookingServices }|--|| PropertyServices : "FK_BookingServices_PropertyServices_ServiceId"
    PropertyPolicies }|--|| Properties : "FK_PropertyPolicies_Properties_PropertyId"
    PropertyImages }|--o| Properties : "FK_PropertyImages_Properties_PropertyId"
    PropertyImages }|--o| Units : "FK_PropertyImages_Units_UnitId"
    Staff }|--|| Users : "FK_Staff_Users_UserId"
    Staff }|--|| Properties : "FK_Staff_Properties_PropertyId"
    AdminActions }|--|| Users : "FK_AdminActions_Users_AdminId"
    AdminActions }|--|| Users : "FK_AdminActions_Users_AdminId1"
    AuditLogs }|--o| Users : "FK_AuditLogs_Users_PerformedBy"
    Notifications }|--|| Users : "FK_Notifications_Users_RecipientId"
    Notifications }|--o| Users : "FK_Notifications_Users_SenderId"
    Reports }|--|| Users : "FK_Reports_Users_ReporterUserId"
    Reports }|--o| Users : "FK_Reports_Users_ReportedUserId"
    Reports }|--o| Properties : "FK_Reports_Properties_ReportedPropertyId"
    UnitAvailability }|--|| Units : "FK_UnitAvailability_Units_UnitId"
    UnitAvailability }|--o| Bookings : "FK_UnitAvailability_Bookings_BookingId"
    UnitAvailability }|--o| Bookings : "FK_UnitAvailability_Bookings_BookingId1"
    PricingRules }|--|| Units : "FK_PricingRules_Units_UnitId"
    ChatSettings }|--|| Users : "FK_ChatSettings_Users_UserId"
    ChatConversations }|--o| Properties : "FK_ChatConversations_Properties_PropertyId"
    ChatConversationParticipant }|--|| ChatConversations : "FK_ChatConversationParticipant_ChatConversations_ConversationId"
    ChatConversationParticipant }|--|| Users : "FK_ChatConversationParticipant_Users_UserId"
    ChatMessages }|--|| ChatConversations : "FK_ChatMessages_ChatConversations_ConversationId"
    ChatAttachments }|--|| ChatConversations : "FK_ChatAttachments_ChatConversations_ConversationId"
    ChatAttachments }|--o| ChatMessages : "FK_ChatAttachments_ChatMessages_ChatMessageId"
    MessageReactions }|--|| ChatMessages : "FK_MessageReactions_ChatMessages_MessageId"