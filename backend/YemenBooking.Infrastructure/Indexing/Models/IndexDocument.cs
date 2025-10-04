using System;
using System.Collections.Generic;
using LiteDB;

namespace YemenBooking.Infrastructure.Indexing.Models
{
    /// <summary>
    /// نموذج فهرس العقار الرئيسي
    /// </summary>
    public class PropertyIndexDocument
    {
        [BsonId]
        public string Id { get; set; } // GUID as string
        
        // معلومات أساسية
        public string Name { get; set; }
        public string NameLower { get; set; } // للبحث النصي
        public string Description { get; set; }
        public string DescriptionLower { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PropertyType { get; set; }
        public Guid PropertyTypeId { get; set; }
        public Guid OwnerId { get; set; }
        
        // التسعير
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Currency { get; set; }
        
        // التقييم
        public int StarRating { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int ViewCount { get; set; }
        public int BookingCount { get; set; }
        
        // الموقع
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        // السعة
        public int MaxCapacity { get; set; }
        public int UnitsCount { get; set; }
        
        // الحالة
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        
        // القوائم
        public List<string> UnitIds { get; set; } = new();
        public List<string> AmenityIds { get; set; } = new();
        public List<string> ServiceIds { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
        
        // الحقول الديناميكية
        public Dictionary<string, string> DynamicFields { get; set; } = new();
        
        // التواريخ
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// فهرس الإتاحة
    /// </summary>
    public class AvailabilityIndexDocument
    {
        [BsonId]
        public string Id { get; set; }
        public string PropertyId { get; set; }
        public string UnitId { get; set; }
        public List<DateRangeIndex> AvailableRanges { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// نطاق تاريخي للإتاحة
    /// </summary>
    public class DateRangeIndex
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// فهرس التسعير الديناميكي
    /// </summary>
    public class PricingIndexDocument
    {
        [BsonId]
        public string Id { get; set; }
        public string PropertyId { get; set; }
        public string UnitId { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; }
        public List<PricingRuleIndex> PricingRules { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// قاعدة تسعير
    /// </summary>
    public class PricingRuleIndex
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string RuleType { get; set; } // seasonal, weekend, special
    }

    /// <summary>
    /// فهرس الحقول الديناميكية
    /// </summary>
    public class DynamicFieldIndexDocument
    {
        [BsonId]
        public string Id { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public List<string> PropertyIds { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// فهرس المدن
    /// </summary>
    public class CityIndexDocument
    {
        [BsonId]
        public string City { get; set; }
        public int PropertyCount { get; set; }
        public List<string> PropertyIds { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// فهرس المرافق
    /// </summary>
    public class AmenityIndexDocument
    {
        [BsonId]
        public string AmenityId { get; set; }
        public string AmenityName { get; set; }
        public int PropertyCount { get; set; }
        public List<string> PropertyIds { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }
}