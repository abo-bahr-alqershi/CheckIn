using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان الدور
/// Role entity configuration
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // تعيين اسم الجدول
        // Set table name
        builder.ToTable("Roles");

        // تعيين المفتاح الأساسي
        // Set primary key
        builder.HasKey(r => r.Id);

        // تكوين الخصائص الأساسية من BaseEntity
        builder.Property(b => b.Id).HasColumnName("RoleId").IsRequired();
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.DeletedAt).HasColumnType("datetime");
        
        // تكوين الخصائص الأخرى
        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        
        // تكوين الفهرس
        builder.HasIndex(r => r.Name).IsUnique();

        // إعداد العلاقات
        // Configure relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // إدراج البيانات الأولية
        // Seed initial data
        builder.HasData(
            new Role { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin" },
            new Role { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Owner" },
            new Role { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Manager" },
            new Role { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Customer" }
        );

        // تطبيق مرشح الحذف الناعم
        // Apply soft delete filter
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
