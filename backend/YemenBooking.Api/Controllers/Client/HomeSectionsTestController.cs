using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace YemenBooking.Api.Controllers.Client
{
    public class HomeSectionsTestController : BaseClientController //ControllerBase
    {

        public HomeSectionsTestController(IMediator mediator) : base(mediator) { }

        [HttpGet("config")]
        public IActionResult GetConfig([FromQuery] string? version)
        {
            var now = DateTime.UtcNow;
            var response = new
            {
                id = "cfg-001",
                version = version ?? "1.0.0",
                isActive = true,
                createdAt = now.AddDays(-10),
                updatedAt = now,
                publishedAt = now.AddDays(-1),
                globalSettings = new Dictionary<string, object>
                {
                    ["refreshInterval"] = 300,
                    ["enablePullToRefresh"] = true,
                    ["enableInfiniteScroll"] = true,
                    ["maxConcurrentRequests"] = 5,
                },
                themeSettings = new Dictionary<string, object>
                {
                    ["primaryColor"] = "#007AFF",
                    ["accentColor"] = "#FF3B30",
                    ["enableDarkMode"] = true,
                    ["fontFamily"] = "Cairo",
                },
                layoutSettings = new Dictionary<string, object>
                {
                    ["sectionSpacing"] = 16.0,
                    ["itemSpacing"] = 8.0,
                    ["borderRadius"] = 12.0,
                    ["enableSectionHeaders"] = true,
                },
                cacheSettings = new Dictionary<string, object>
                {
                    ["maxAge"] = 3600,
                    ["maxSize"] = 100,
                    ["enableOfflineMode"] = true,
                },
                analyticsSettings = new Dictionary<string, object>
                {
                    ["enabled"] = true,
                    ["trackImpressions"] = true,
                    ["trackInteractions"] = true,
                },
                enabledFeatures = new[] { "homeSections", "cityDestinations" },
                experimentalFeatures = new Dictionary<string, object>
                {
                    ["newLayout"] = false
                }
            };

            return Ok(response);
        }

        [HttpGet("sections")]
        public IActionResult GetSections([FromQuery] string? userId, [FromQuery] bool includeContent = true, [FromQuery] bool onlyActive = true, [FromQuery] string language = "ar")
        {
            var now = DateTime.UtcNow;

            object SectionConfig(string id, string sectionType, string title, string titleAr) => new
            {
                id,
                sectionType,
                displaySettings = new Dictionary<string, object>
                {
                    ["maxItems"] = 10,
                    ["showTitle"] = true,
                    ["showSubtitle"] = false,
                    ["showBadge"] = false,
                    ["badgeText"] = null,
                    ["showIndicators"] = false,
                    ["showViewAllButton"] = true,
                },
                layoutSettings = new Dictionary<string, object>
                {
                    ["layoutType"] = "horizontal",
                    ["columnsCount"] = 2,
                    ["itemHeight"] = 200.0,
                    ["itemSpacing"] = 8.0,
                    ["sectionPadding"] = 16.0,
                    ["sectionSize"] = "medium",
                },
                styleSettings = new Dictionary<string, object>
                {
                    ["borderRadius"] = 12.0,
                    ["elevation"] = 2.0,
                    ["enableGradient"] = false,
                    ["gradientColors"] = new[] { "#FFFFFF", "#F7F7F7" },
                },
                behaviorSettings = new Dictionary<string, object>
                {
                    ["autoPlay"] = false,
                    ["autoPlayDuration"] = 5,
                    ["infiniteScroll"] = true,
                    ["enablePullToRefresh"] = true,
                    ["lazy"] = true,
                },
                animationSettings = new Dictionary<string, object>
                {
                    ["animationType"] = "fade",
                    ["animationDuration"] = 300,
                    ["parallaxEnabled"] = false,
                    ["enableHeroAnimation"] = false,
                },
                cacheSettings = new Dictionary<string, object>
                {
                    ["enableCache"] = true,
                    ["maxAge"] = 3600,
                    ["cacheImages"] = true,
                },
                propertyIds = new[] { "p-1001", "p-1002", "p-1003" },
                title = title,
                titleAr = titleAr,
                subtitle = "Hand picked for you",
                subtitleAr = "مختارة لك",
                backgroundColor = "#FFFFFF",
                textColor = "#000000",
                customImage = (string?)null,
                customData = new Dictionary<string, object>()
            };

            object Content(string id, string sectionId, int order, string type, string title, string imageUrl) => new
            {
                id,
                sectionId,
                contentType = type, // PROPERTY | OFFER | DESTINATION | BANNER | PROMOTION | ADVERTISEMENT | ANNOUNCEMENT
                contentData = new Dictionary<string, object>
                {
                    ["title"] = title,
                    ["description"] = "",
                    ["imageUrl"] = imageUrl,
                    ["ctaText"] = "عرض",
                    ["ctaAction"] = "navigate",
                    ["price"] = 99.0,
                    ["discountPercentage"] = 10.0,
                },
                metadata = new Dictionary<string, object>
                {
                    ["priority"] = order,
                    ["tags"] = new[] { "home", "featured" },
                    ["targetAudience"] = "all",
                    ["analytics"] = new Dictionary<string, object> { ["exp"] = "A" }
                },
                expiresAt = (DateTime?)null,
                displayOrder = order,
                isActive = true,
                createdAt = DateTime.UtcNow.AddDays(-2),
                updatedAt = DateTime.UtcNow,
                isValid = (bool?)true,
                isExpired = (bool?)false,
            };

            var sections = new[]
            {
                new
                {
                    id = "sec-1",
                    sectionType = "horizontalPropertyList",
                    order = 1,
                    isActive = true,
                    title = "Popular Properties",
                    subtitle = "Top picks",
                    titleAr = "عقارات شائعة",
                    subtitleAr = "أفضل الاختيارات",
                    createdAt = now.AddDays(-5),
                    updatedAt = now,
                    sectionConfig = SectionConfig("cfg-sec-1", "horizontalPropertyList", "Popular", "شائع"),
                    content = includeContent
                        ? new[]
                        {
                            Content("c-1","sec-1",1,"PROPERTY","فندق السلام","https://picsum.photos/seed/p1/600/400"),
                            Content("c-2","sec-1",2,"PROPERTY","فندق صنعاء","https://picsum.photos/seed/p2/600/400"),
                            Content("c-3","sec-1",3,"PROPERTY","فندق عدن","https://picsum.photos/seed/p3/600/400"),
                        }
                        : Array.Empty<object>(),
                    metadata = new Dictionary<string, object>(),
                    scheduledAt = (DateTime?)null,
                    expiresAt = (DateTime?)null,
                    targetAudience = new[] { "all" },
                    priority = 10,
                    isVisible = true,
                    isExpired = false,
                    isScheduled = false,
                    isTimeSensitive = false,
                },
                new
                {
                    id = "sec-2",
                    sectionType = "cityCardsGrid",
                    order = 2,
                    isActive = true,
                    title = "Explore Cities",
                    subtitle = "",
                    titleAr = "اكتشف المدن",
                    subtitleAr = "",
                    createdAt = now.AddDays(-4),
                    updatedAt = now,
                    sectionConfig = SectionConfig("cfg-sec-2", "cityCardsGrid", "Cities", "مدن"),
                    content = includeContent
                        ? new[]
                        {
                            Content("c-4","sec-2",1,"DESTINATION","صنعاء","https://picsum.photos/seed/c1/600/400"),
                            Content("c-5","sec-2",2,"DESTINATION","عدن","https://picsum.photos/seed/c2/600/400"),
                        }
                        : Array.Empty<object>(),
                    metadata = new Dictionary<string, object>(),
                    scheduledAt = (DateTime?)null,
                    expiresAt = (DateTime?)null,
                    targetAudience = new[] { "all" },
                    priority = 8,
                    isVisible = true,
                    isExpired = false,
                    isScheduled = false,
                    isTimeSensitive = false,
                },
                new
                {
                    id = "sec-3",
                    sectionType = "offersCarousel",
                    order = 3,
                    isActive = true,
                    title = "Offers",
                    subtitle = "",
                    titleAr = "عروض",
                    subtitleAr = "",
                    createdAt = now.AddDays(-3),
                    updatedAt = now,
                    sectionConfig = SectionConfig("cfg-sec-3", "offersCarousel", "Offers", "عروض"),
                    content = includeContent
                        ? new[]
                        {
                            Content("c-6","sec-3",1,"BANNER","خصم 30%","https://picsum.photos/seed/b1/800/300"),
                            Content("c-7","sec-3",2,"BANNER","عروض الصيف","https://picsum.photos/seed/b2/800/300"),
                        }
                        : Array.Empty<object>(),
                    metadata = new Dictionary<string, object>(),
                    scheduledAt = (DateTime?)null,
                    expiresAt = (DateTime?)null,
                    targetAudience = new[] { "all" },
                    priority = 6,
                    isVisible = true,
                    isExpired = false,
                    isScheduled = false,
                    isTimeSensitive = true,
                },
            };

            if (onlyActive)
            {
                return Ok(sections);
            }

            return Ok(sections);
        }



        [HttpGet("destinations")]
        public IActionResult GetDestinations()
        {
            var now = DateTime.UtcNow;
            var items = new[]
            {
                new
                {
                    id = "dst-1",
                    name = "Sana'a",
                    nameAr = "صنعاء",
                    country = "Yemen",
                    countryAr = "اليمن",
                    description = "المدينة القديمة",
                    descriptionAr = "المدينة القديمة",
                    imageUrl = "https://picsum.photos/seed/d1/800/600",
                    additionalImages = new[] { "https://picsum.photos/seed/d1a/600/400", "https://picsum.photos/seed/d1b/600/400" },
                    latitude = 15.3694,
                    longitude = 44.1910,
                    propertyCount = 120,
                    averagePrice = 75.0,
                    currency = "USD",
                    averageRating = 4.3,
                    reviewCount = 1520,
                    isPopular = true,
                    isFeatured = true,
                    priority = 10,
                    highlights = new[] { "Old City", "Bazaars" },
                    highlightsAr = new[] { "المدينة القديمة", "الأسواق" },
                    weatherData = new Dictionary<string, object> { ["temperature"] = 26.0, ["condition"] = "Sunny", ["icon"] = "sunny" },
                    attractionsData = new Dictionary<string, object> { ["popular"] = new[] { "Old City", "Al Saleh Mosque" } },
                    metadata = new Dictionary<string, object>(),
                    createdAt = now.AddDays(-30),
                    updatedAt = now,
                    isActive = true,
                },
                new
                {
                    id = "dst-2",
                    name = "Aden",
                    nameAr = "عدن",
                    country = "Yemen",
                    countryAr = "اليمن",
                    description = "مدينة ساحلية",
                    descriptionAr = "مدينة ساحلية",
                    imageUrl = "https://picsum.photos/seed/d2/800/600",
                    additionalImages = new[] { "https://picsum.photos/seed/d2a/600/400" },
                    latitude = 12.7855,
                    longitude = 45.0187,
                    propertyCount = 80,
                    averagePrice = 60.0,
                    currency = "USD",
                    averageRating = 4.2,
                    reviewCount = 980,
                    isPopular = true,
                    isFeatured = false,
                    priority = 8,
                    highlights = new[] { "Beaches", "Harbor" },
                    highlightsAr = new[] { "شواطئ", "ميناء" },
                    weatherData = new Dictionary<string, object> { ["temperature"] = 30.0, ["condition"] = "Clear", ["icon"] = "clear" },
                    attractionsData = new Dictionary<string, object> { ["popular"] = new[] { "Gold Mohur Beach" } },
                    metadata = new Dictionary<string, object>(),
                    createdAt = now.AddDays(-20),
                    updatedAt = now,
                    isActive = true,
                }
            };

            return Ok(items);
        }

        [HttpGet("sections/{sectionId}/data")]
        public IActionResult GetSectionData([FromRoute] string sectionId)
        {
            var now = DateTime.UtcNow;

            var properties = new[]
            {
                new
                {
                    id = "p-1001",
                    name = "Al Salam Hotel",
                    address = "Main St 1",
                    city = "Sana'a",
                    latitude = 15.37,
                    longitude = 44.20,
                    starRating = 4,
                    description = "Nice stay",
                    isApproved = true,
                    createdAt = now.AddDays(-60),
                    viewCount = 2500,
                    bookingCount = 420,
                    averageRating = 4.4,
                    images = new[] { "https://picsum.photos/seed/p1001/600/400" },
                    basePrice = 80.0,
                    currency = "USD",
                    amenities = new[] { "WiFi", "Parking" },
                    mainImageUrl = "https://picsum.photos/seed/p1001m/800/600",
                    isFeatured = true,
                    isSponsored = false,
                    discountPercentage = 15.0,
                    propertyType = "Hotel",
                    customData = new Dictionary<string, object>()
                },
                new
                {
                    id = "p-1002",
                    name = "Sana'a Grand",
                    address = "Old City",
                    city = "Sana'a",
                    latitude = 15.35,
                    longitude = 44.22,
                    starRating = 5,
                    description = "Luxury experience",
                    isApproved = true,
                    createdAt = now.AddDays(-120),
                    viewCount = 5400,
                    bookingCount = 780,
                    averageRating = 4.7,
                    images = new[] { "https://picsum.photos/seed/p1002/600/400" },
                    basePrice = 120.0,
                    currency = "USD",
                    amenities = new[] { "Pool", "Spa", "Gym" },
                    mainImageUrl = "https://picsum.photos/seed/p1002m/800/600",
                    isFeatured = true,
                    isSponsored = true,
                    discountPercentage = 20.0,
                    propertyType = "Hotel",
                    customData = new Dictionary<string, object>()
                }
            };

            var destinations = new[]
            {
                new
                {
                    id = "dst-1",
                    name = "Sana'a",
                    nameAr = "صنعاء",
                    country = "Yemen",
                    countryAr = "اليمن",
                    description = "المدينة القديمة",
                    descriptionAr = "المدينة القديمة",
                    imageUrl = "https://picsum.photos/seed/d1/800/600",
                    additionalImages = new[] { "https://picsum.photos/seed/d1a/600/400" },
                    latitude = 15.3694,
                    longitude = 44.1910,
                    propertyCount = 120,
                    averagePrice = 75.0,
                    currency = "USD",
                    averageRating = 4.3,
                    reviewCount = 1520,
                    isPopular = true,
                    isFeatured = true,
                    priority = 10,
                    highlights = new[] { "Old City", "Bazaars" },
                    highlightsAr = new[] { "المدينة القديمة", "الأسواق" },
                    weatherData = new Dictionary<string, object>(),
                    attractionsData = new Dictionary<string, object>(),
                    metadata = new Dictionary<string, object>(),
                    createdAt = now.AddDays(-30),
                    updatedAt = now,
                    isActive = true,
                }
            };



            var data = new
            {
                sectionId,
                sectionType = sectionId == "sec-2" ? "cityCardsGrid" : (sectionId == "sec-3" ? "offersCarousel" : "horizontalPropertyList"),
                properties = sectionId == "sec-1" ? properties : Array.Empty<object>(),

                specialOffers = Array.Empty<object>(),
                destinations = sectionId == "sec-2" ? destinations : Array.Empty<object>(),
                customData = new Dictionary<string, object>(),
                lastUpdated = now,
                nextPageToken = (string?)null,
                hasMore = false,
                totalCount = sectionId == "sec-1" ? 2 : (sectionId == "sec-2" ? 1 : 1),
                metadata = new Dictionary<string, object>()
            };

            return Ok(data);
        }

        [HttpPost("sections/{sectionId}/impression")]
        public IActionResult RecordSectionImpression([FromRoute] string sectionId)
        {
            return Ok(new { success = true });
        }

        [HttpPost("sections/{sectionId}/interaction")]
        public IActionResult RecordSectionInteraction([FromRoute] string sectionId, [FromBody] Dictionary<string, object>? body)
        {
            return Ok(new { success = true });
        }


    }
}