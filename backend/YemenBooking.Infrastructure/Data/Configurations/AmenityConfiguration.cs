using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان المرفق
/// Amenity entity configuration
/// </summary>
public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("Amenities");

        builder.HasKey(a => a.Id);

        builder.Property(b => b.Id).HasColumnName("AmenityId").IsRequired();
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAt).HasColumnType("datetime");

        builder.Property(a => a.Name).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Description).HasMaxLength(200);

        builder.HasIndex(a => a.Name).IsUnique();

        builder.HasMany(a => a.PropertyTypeAmenities)
            .WithOne(pta => pta.Amenity)
            .HasForeignKey(pta => pta.AmenityId);

        builder.HasQueryFilter(a => !a.IsDeleted);

        // Seed data
        builder.HasData(
            new Amenity { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "واي فاي", Description = "إنترنت لاسلكي مجاني" },
            new Amenity { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "مسبح", Description = "مسبح للسباحة" },
            new Amenity { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "جيم", Description = "صالة رياضية" },
            new Amenity { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "مطعم", Description = "خدمة المطعم" },
            new Amenity { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "موقف سيارات", Description = "موقف سيارات مجاني" },
            new Amenity { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "تكييف", Description = "تكييف الهواء" },
            new Amenity { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "تلفزيون", Description = "تلفزيون بشاشة مسطحة" },
            new Amenity { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "إفطار", Description = "وجبة إفطار مجانية" }
        );
    }
}
