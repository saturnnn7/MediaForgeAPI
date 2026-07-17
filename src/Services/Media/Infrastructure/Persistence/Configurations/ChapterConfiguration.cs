using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Media.Infrastructure.Persistence.Configurations;

public sealed class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable("chapters");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.AssetId)
            .HasColumnName("asset_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnName("start_time")
            .HasConversion(v => v.Ticks, v => TimeSpan.FromTicks(v))
            .IsRequired();

        builder.Property(x => x.EndTime)
            .HasColumnName("end_time")
            .HasConversion(
                v => v.HasValue ? v.Value.Ticks : (long?)null,
                v => v.HasValue ? TimeSpan.FromTicks(v.Value) : (TimeSpan?)null);

        builder.Property(x => x.Order)
            .HasColumnName("order");

        builder.HasIndex(x => x.AssetId).HasDatabaseName("ix_chapters_asset_id");
        builder.HasIndex(x => new { x.AssetId, x.Order }).IsUnique()
            .HasDatabaseName("ix_chapters_asset_id_order");
    }
}
