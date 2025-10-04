using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان نوع الكيان
/// Property Type entity configuration
/// </summary>
public class PropertyTypeConfiguration : IEntityTypeConfiguration<PropertyType>
{
    public void Configure(EntityTypeBuilder<PropertyType> builder)
    {
        // تعيين اسم الجدول
        // Set table name
        builder.ToTable("PropertyTypes");

        // تعيين المفتاح الأساسي
        // Set primary key
        builder.HasKey(pt => pt.Id);

        // تكوين الخصائص الأساسية من BaseEntity
        builder.Property(b => b.Id).HasColumnName("TypeId").IsRequired();
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAt).HasColumnType("datetime");
        
        // تكوين الخصائص الأخرى
        builder.Property(pt => pt.Name).IsRequired().HasMaxLength(50);
        builder.Property(pt => pt.Description).HasMaxLength(500);
        builder.Property(pt => pt.DefaultAmenities).HasColumnType("NVARCHAR(MAX)");
        
        // تكوين الفهرس
        builder.HasIndex(pt => pt.Name).IsUnique();

        // إعداد العلاقات
        // Configure relationships
        builder.HasMany(pt => pt.Properties)
            .WithOne(p => p.PropertyType)
            .HasForeignKey(p => p.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pt => pt.UnitTypes)
            .WithOne(ut => ut.PropertyType)
            .HasForeignKey(ut => ut.PropertyTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pt => pt.PropertyTypeAmenities)
            .WithOne(pta => pta.PropertyType)
            .HasForeignKey(pta => pta.PropertyTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // إدراج البيانات الأولية
        // Seed initial data
        builder.HasData(
            new PropertyType 
            { 
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                Name = "فندق",
                Description = "فندق تقليدي بغرف وخدمات فندقية",
                DefaultAmenities = "[]"
            },
            new PropertyType 
            { 
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                Name = "شاليه",
                Description = "شاليه للعائلات والمجموعات",
                DefaultAmenities = "[]"
            },
            new PropertyType 
            { 
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                Name = "استراحة",
                Description = "استراحة للإقامة المؤقتة",
                DefaultAmenities = "[]"
            },
            new PropertyType 
            { 
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                Name = "فيلا",
                Description = "فيلا خاصة للإقامة الفاخرة",
                DefaultAmenities = "[]"
            },
            new PropertyType 
            { 
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), 
                Name = "شقة",
                Description = "شقة مفروشة للإقامة قصيرة أو طويلة المدى",
                DefaultAmenities = "[]"
            }
        );

        // تطبيق مرشح الحذف الناعم
        // Apply soft delete filter
        builder.HasQueryFilter(pt => !pt.IsDeleted);
    }
}
