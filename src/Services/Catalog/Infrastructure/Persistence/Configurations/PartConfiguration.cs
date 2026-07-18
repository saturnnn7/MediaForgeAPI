using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.ToTable("parts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.OrderMajor)
            .HasColumnName("order_major");

        builder.Property(x => x.OrderMinor)
            .HasColumnName("order_minor")
            .HasDefaultValue(0);

        builder.Property(x => x.PartType)
            .HasColumnName("part_type")
            .HasConversion<string>();

        builder.Property(x => x.CoverUrl)
            .HasColumnName("cover_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.DurationSeconds)
            .HasColumnName("duration_seconds")
            .IsRequired(false);

        builder.Property(x => x.IsPublished)
            .HasColumnName("is_published")
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(x => x.Chapters);
        builder.Ignore(x => x.Assets);

        builder.HasMany<PartChapter>("_chapters")
            .WithOne()
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<PartAsset>("_assets")
            .WithOne()
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_chapters").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation("_assets").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.WorkId).HasDatabaseName("ix_parts_work_id");
        builder.HasIndex(x => new { x.WorkId, x.OrderMajor, x.OrderMinor })
            .IsUnique()
            .HasDatabaseName("ix_parts_work_order");
    }
}
