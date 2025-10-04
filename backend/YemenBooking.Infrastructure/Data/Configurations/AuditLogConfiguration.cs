using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

/// <summary>
/// تكوين كيان سجل التدقيق
/// AuditLog entity configuration
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // Table name
        builder.ToTable("AuditLogs");

        // Primary key
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("AuditLogId").IsRequired();

        // Base entity properties
        builder.Property(a => a.CreatedAt).HasColumnType("datetime").IsRequired();
        builder.Property(a => a.UpdatedAt).HasColumnType("datetime").IsRequired();
        builder.Property(a => a.IsActive).HasDefaultValue(true);
        builder.Property(a => a.IsDeleted).HasDefaultValue(false);
        builder.Property(a => a.DeletedAt).HasColumnType("datetime");

        // Properties
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityId);
        builder.Property(a => a.Action).IsRequired();
        builder.Property(a => a.OldValues).HasColumnType("NVARCHAR(MAX)");
        builder.Property(a => a.NewValues).HasColumnType("NVARCHAR(MAX)");
        builder.Property(a => a.PerformedBy);
        builder.Property(a => a.Username).HasMaxLength(100);
        builder.Property(a => a.IpAddress).HasMaxLength(50);
        builder.Property(a => a.UserAgent).HasMaxLength(255);
        builder.Property(a => a.Notes).HasColumnType("NVARCHAR(MAX)");
        builder.Property(a => a.Metadata).HasColumnType("NVARCHAR(MAX)");
        builder.Property(a => a.IsSuccessful).HasDefaultValue(true);
        builder.Property(a => a.ErrorMessage).HasMaxLength(500);
        builder.Property(a => a.DurationMs);
        builder.Property(a => a.SessionId).HasMaxLength(100);
        builder.Property(a => a.RequestId).HasMaxLength(100);
        builder.Property(a => a.Source).HasMaxLength(100);

        // Relationships
        builder.HasOne(a => a.PerformedByUser)
            .WithMany()
            .HasForeignKey(a => a.PerformedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // Global query filter
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
} 