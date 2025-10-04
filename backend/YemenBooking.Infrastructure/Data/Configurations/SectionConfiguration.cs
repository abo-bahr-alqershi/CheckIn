using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Data.Configurations
{
	public class SectionConfiguration : IEntityTypeConfiguration<Section>
	{
		public void Configure(EntityTypeBuilder<Section> builder)
		{
			builder.ToTable("Sections");
			builder.HasKey(s => s.Id);
			builder.Property(s => s.Type).IsRequired();
			builder.Property(s => s.ContentType).IsRequired();
			builder.Property(s => s.DisplayStyle).IsRequired();
			builder.Property(s => s.DisplayOrder).IsRequired();
			builder.Property(s => s.Target).IsRequired();
			builder.Property(s => s.Name).HasMaxLength(150);
			builder.Property(s => s.Title).HasMaxLength(200);
			builder.Property(s => s.Subtitle).HasMaxLength(300);
			builder.Property(s => s.Description).HasColumnType("NVARCHAR(MAX)");
			builder.Property(s => s.ShortDescription).HasMaxLength(500);
			builder.Property(s => s.Icon).HasMaxLength(100);
			builder.Property(s => s.ColorTheme).HasMaxLength(50);
			builder.Property(s => s.BackgroundImage).HasMaxLength(500);
			builder.Property(s => s.BackgroundImageId).IsRequired(false);
			builder.Property(s => s.FilterCriteria).HasColumnType("NVARCHAR(MAX)");
			builder.Property(s => s.SortCriteria).HasColumnType("NVARCHAR(MAX)");
			builder.Property(s => s.CityName).HasMaxLength(100);
			builder.Property(s => s.MinPrice).HasColumnType("decimal(18,2)");
			builder.Property(s => s.MaxPrice).HasColumnType("decimal(18,2)");
			builder.Property(s => s.MinRating).HasColumnType("decimal(5,2)");
			builder.Property(s => s.Metadata).HasColumnType("NVARCHAR(MAX)");

			builder.HasMany(s => s.PropertyItems)
				.WithOne(pi => pi.Section)
				.HasForeignKey(pi => pi.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(s => s.UnitItems)
				.WithOne(ui => ui.Section)
				.HasForeignKey(ui => ui.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

            // Optional link to background image via SectionImages (new dedicated table)
            builder.HasOne<SectionImage>()
				.WithMany()
				.HasForeignKey(s => s.BackgroundImageId)
				.OnDelete(DeleteBehavior.SetNull);

            // Images collection mapped via SectionImageConfiguration
		}
	}
}