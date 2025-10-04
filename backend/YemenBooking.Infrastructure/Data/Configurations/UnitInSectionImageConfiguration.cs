using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations;

public class UnitInSectionImageConfiguration : IEntityTypeConfiguration<UnitInSectionImage>
{
    public void Configure(EntityTypeBuilder<UnitInSectionImage> builder)
    {
        builder.ToTable("UnitInSectionImages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TempKey).HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Type).HasMaxLength(100);
        builder.Property(x => x.Caption).HasMaxLength(300);
        builder.Property(x => x.AltText).HasMaxLength(300);
        builder.Property(x => x.Tags).HasColumnType("NVARCHAR(MAX)");
        builder.Property(x => x.Sizes).HasColumnType("NVARCHAR(MAX)");
        builder.Property(x => x.UploadedAt).HasColumnType("datetime");
        builder.Property(x => x.MediaType).HasMaxLength(20).HasDefaultValue("image");
        builder.Property(x => x.VideoThumbnailUrl).HasMaxLength(500);

        builder.Property(x => x.UnitInSectionId).IsRequired(false);
        builder.HasOne(x => x.UnitInSection)
            .WithMany(u => u.AdditionalImages)
            .HasForeignKey(x => x.UnitInSectionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.UnitInSectionId);
        builder.HasIndex(x => x.TempKey);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

